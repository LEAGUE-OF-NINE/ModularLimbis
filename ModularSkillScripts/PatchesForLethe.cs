using System;
using System.Runtime.InteropServices;
using BattleUI;
using BattleUI.Operation;
using Lethe;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using UnhollowerRuntimeLib;
using UnityEngine;
using SD;
using Utils;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace ModularSkillScripts
{
	public class InjectedFunnyChange : MonoBehaviour
	{
		public InjectedFunnyChange(IntPtr ptr) : base(ptr) { }

		public InjectedFunnyChange() : base(ClassInjector.DerivedConstructorPointer<InjectedFunnyChange>())
		{
			ClassInjector.DerivedConstructorBody(this);
		}

		public long skillparentPtr_intlong = 0;
		public long objectPtr_intlong = 0;
		public int changeKind = 0;
		public string change = "";
		public int motionIdx = 0;
	}

	internal class PatchesForLethe
	{
		public static void InjectFunnyChange(int kind, string change, long objectPtr_intlong, long skillparentPtr_intlong, int motionIdx = 0)
		{
			InjectedFunnyChange funny = new InjectedFunnyChange();
			funny.changeKind = kind;
			funny.change = change;
			funny.objectPtr_intlong = objectPtr_intlong;
			funny.skillparentPtr_intlong = skillparentPtr_intlong;
			funny.motionIdx = motionIdx;
			MainClass.Logg.LogInfo("made funny: " + change);
			injectedFunnyChange_list.Add(funny);
		}

		public static List<InjectedFunnyChange> injectedFunnyChange_list = new List<InjectedFunnyChange>();




		//[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.Init), new Type[] { typeof(SkillModel), typeof(int), typeof(int), typeof(int), typeof(OPERATOR_TYPE) })]
		//[HarmonyPostfix]
		private static void Postfix_SkillModel_InitMany(SkillModel skill, SkillModel __instance)
		{
			MainClass.Logg.LogInfo("Postfix_SkillModel_InitMany");

			long OLDskillPtr_intlong = skill.Pointer.ToInt64();
			long NEWskillPtr_intlong = __instance.Pointer.ToInt64();
			foreach (InjectedFunnyChange funny in injectedFunnyChange_list)
			{
				if (funny.objectPtr_intlong == OLDskillPtr_intlong) funny.objectPtr_intlong = NEWskillPtr_intlong;
				else if (funny.skillparentPtr_intlong == OLDskillPtr_intlong) funny.skillparentPtr_intlong = NEWskillPtr_intlong;
			}
		}

		//[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.Init), new Type[] { typeof(SkillModel) })]
		//[HarmonyPostfix]
		private static void Postfix_SkillModel_InitOne(SkillModel skill, SkillModel __instance)
		{
			MainClass.Logg.LogInfo("Postfix_SkillModel_InitOne");

			long OLDskillPtr_intlong = skill.Pointer.ToInt64();
			long NEWskillPtr_intlong = __instance.Pointer.ToInt64();
			foreach (InjectedFunnyChange funny in injectedFunnyChange_list)
			{
				if (funny.objectPtr_intlong == OLDskillPtr_intlong) funny.objectPtr_intlong = NEWskillPtr_intlong;
				else if (funny.skillparentPtr_intlong == OLDskillPtr_intlong) funny.skillparentPtr_intlong = NEWskillPtr_intlong;
			}
		}


		[HarmonyPatch(typeof(CharacterAppearance), "ChangeMotion")]
		[HarmonyPrefix]
		private static void ChangeMotion(CharacterAppearance __instance, ref MOTION_DETAIL motiondetail, ref int index)
		{
			BattleActionLog currentActionLog = __instance._battleUnitView.CurrentActionLog;
			if (currentActionLog == null) return;

			BattleLog systemLog = currentActionLog._systemLog;
			string text = motiondetail.ToString();
			if (!text.StartsWith("S") && text != "Parrying") return;

			if (__instance._battleUnitView._currentDuelViewer != null || __instance._battleUnitView.CurrentActionLog == null) return;

			foreach (SubBattleLog_Behaviour subBattleLog_Behaviour in systemLog.GetAllBehaviourLog_Start())
			{
				BattleSkillViewer skillViewer = __instance._battleUnitView.GetCurrentSkillViewer();

				int curCoinIndex = skillViewer.curCoinIndex;
				SubBattleLog_CharacterInfo characterInfo = systemLog.GetCharacterInfo(subBattleLog_Behaviour._instanceID);
				if (subBattleLog_Behaviour._instanceID != characterInfo.instanceID || __instance._battleUnitView._instanceID != characterInfo.instanceID) continue;

				SkillModel skillModel = skillViewer.GetSkillModel();
				long skillPtr_intlong = skillModel.Pointer.ToInt64();
				foreach (InjectedFunnyChange funny in injectedFunnyChange_list)
				{
					if (funny.changeKind != 0) continue;
					if (funny.objectPtr_intlong != skillPtr_intlong) continue;

					break;
				}

				CoinModel coinModel = skillModel.CoinList[curCoinIndex];
				long coinPtr_intlong = skillModel.Pointer.ToInt64();
				foreach (InjectedFunnyChange funny in injectedFunnyChange_list)
				{
					if (funny.changeKind != 0) continue;
					if (funny.skillparentPtr_intlong != skillPtr_intlong) continue;
					if (funny.objectPtr_intlong != coinPtr_intlong) continue;
					
					MOTION_DETAIL motion_DETAIL;
					Enum.TryParse(funny.change, out motion_DETAIL);
					__instance._currentMotiondetail = motion_DETAIL;
					index = funny.motionIdx;
					motiondetail = motion_DETAIL;
					break;
				}

			}
		}

		[HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.Update_Cointoss))]
		[HarmonyPrefix]
		private static void Prefix_BattleUnitView_UpdateCointoss(BattleUnitView __instance, int index, bool isDuel)
		{
			BattleActionLog currentActionLog = __instance.CurrentActionLog;
			if (currentActionLog != null)
			{
				ChangeAppearance_Internal(__instance, currentActionLog._systemLog, index, isDuel);
				return;
			}
		}

		[HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.StartCoinToss))]
		[HarmonyPrefix]
		private static void Prefix_BattleUnitView_StartCoinToss(BattleUnitView __instance, BattleLog log, int coinIdx, bool isDuel)
		{
			if (__instance._battleCharacterState.IsEvade) ChangeAppearance_Internal(__instance, log, coinIdx, isDuel);
		}

		private static void ChangeAppearance_Internal(BattleUnitView __instance, BattleLog log, int coinIdx, bool isDuel)
		{
			foreach (SubBattleLog_Behaviour subBattleLog_Behaviour in log.GetAllBehaviourLog_Start())
			{
				MainClass.Logg.LogInfo("subBattleLog");
				int skillID = subBattleLog_Behaviour._skillID;
				MainClass.Logg.LogInfo("skillID: " + skillID);
				int gaksungLevel = subBattleLog_Behaviour._gaksungLevel;
				SubBattleLog_CharacterInfo characterInfo = log.GetCharacterInfo(subBattleLog_Behaviour._instanceID);
				SkillStaticData data = Singleton<StaticDataManager>.Instance._skillList.GetData(skillID);
				if (__instance._instanceID != characterInfo.instanceID) continue;
				MainClass.Logg.LogInfo("instance id correct");
				BattleSkillViewer skillViewer = __instance.GetCurrentSkillViewer();
				if (skillViewer == null) continue;
				MainClass.Logg.LogInfo("Skillviewer not null");
				SkillModel skillModel = skillViewer.GetSkillModel();
				MainClass.Logg.LogInfo("skillModel - skillID: " + skillModel.GetID());
				long skillPtr_intlong = skillModel.Pointer.ToInt64();
				foreach (InjectedFunnyChange funny in injectedFunnyChange_list)
				{
					MainClass.Logg.LogInfo("funny iterate");
					if (funny.changeKind != 1) continue;
					MainClass.Logg.LogInfo("funny intlong vs skill intlong: " + funny.objectPtr_intlong + " | " + skillPtr_intlong);
					if (funny.objectPtr_intlong != skillPtr_intlong) continue;
					__instance.ChangeAppearance(funny.change, true);
					MainClass.Logg.LogInfo("funny change " + funny.change);
					break;
				}

				CoinModel coinModel = skillModel.CoinList[coinIdx];
				long coinPtr_intlong = skillModel.Pointer.ToInt64();
				foreach (InjectedFunnyChange funny in injectedFunnyChange_list)
				{
					MainClass.Logg.LogInfo("funny iterate");
					if (funny.changeKind != 1) continue;
					if (funny.skillparentPtr_intlong != skillPtr_intlong) continue;
					if (funny.objectPtr_intlong != coinPtr_intlong) continue;
					__instance.ChangeAppearance(funny.change, true);
					MainClass.Logg.LogInfo("funny change " + funny.change);
					break;
				}

			}
		}

		[HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.OnRoundEnd))]
		[HarmonyPostfix]
		private static void Postfix_BattleUnitView_OnRoundEnd(BattleUnitView __instance)
		{
			injectedFunnyChange_list.Clear();

		}


		//public static MethodBase GetLetheFuck(string methodPath)
		//{
		//	var type = GetLetheClass("SkillAbility.ChangeAppearance");

		//	//AccessTools.FirstMethod(type, Func<MethodInfo, bool> predicate)
		//	return AccessTools.FirstMethod(type, method => method.Name.Contains("ChangeAppearance_Internal"));
		//}

		//public static Type GetLetheClass(string classPath)
		//{
		//	Type type = AccessTools.TypeByName("Lethe." + classPath);
		//	return type;
		//}

	}
}
