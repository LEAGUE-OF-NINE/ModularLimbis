using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using BattleUI;
using BattleUI.Operation;
using Dungeon;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using static BattleUI.Abnormality.AbnormalityPartSkills;
using static UnityEngine.GraphicsBuffer;
using IntPtr = System.IntPtr;

namespace ModularSkillScripts
{
	class SkillScriptInitPatch
	{
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.Init), new Type[] { })]
		[HarmonyPostfix]
		private static void Postfix_SkillModelInit_AddSkillScript(SkillModel __instance)
		{
			//List<ModularSA> modsa_list_goodones = new List<ModularSA>();
			//foreach (ModularSA modsa in modsaglobal_list)
			//{
			//	//GCHandle skillModel_gchandle = (GCHandle)modsa.skillModel_ptr;
			//	if (modsa.skillModel != null)
			//	{
			//		modsa_list_goodones.Add(modsa);
			//		//MainClass.Logg.LogInfo("found ptr");
			//	}
			//	else
			//	{
			//		//MainClass.Logg.LogInfo("null ptr");
			//	}
			//}
			//modsaglobal_list = modsa_list_goodones;

			List<AbilityData> abilityData_list = __instance.GetSkillAbilityScript();
			for (int i = 0; i < abilityData_list.Count; i++)
			{
				AbilityData abilityData = abilityData_list[i];
				string abilityScriptname = abilityData.ScriptName;
				if (!abilityScriptname.StartsWith("Modular/")) continue;
				//InjectSkillAbility(__instance, i, abilityData, abilityScriptname);

				long ptr = __instance.Pointer.ToInt64();
				bool existsAlready = false;
				foreach (ModularSA existingModsa in modsaglobal_list)
				{
					if (existingModsa.ptr_intlong == ptr && existingModsa.originalString == abilityScriptname)
					{
						existsAlready = true;
						MainClass.Logg.LogInfo(ptr + ": exists already " + abilityScriptname);
						existingModsa.ResetValueList();
						existingModsa.ResetAdders();
						existingModsa.modsa_skillModel = __instance;
						break;
					}
				}
				if (existsAlready) continue;

				var modsa = new ModularSA();
				modsa.originalString = abilityScriptname;
				modsa.modsa_skillModel = __instance;
				modsa.ptr_intlong = ptr;
				modsa.SetupModular(abilityScriptname.Remove(0, 8));
				MainClass.Logg.LogInfo("modSkillAbility init: " + abilityScriptname);
				modsaglobal_list.Add(modsa);
			}

			//if (injectedAbilities)
			//{
			//	MainClass.Logg.LogInfo("about to calc all datas");
			//	__instance.CalculateAllPossibleDatas(0, null);
			//	MainClass.Logg.LogInfo("calced all datas");
			//	//MainClass.Logg.LogInfo("skillabilityCount: " + __instance._skillAbilityList.Count);
			//	//foreach (SkillAbility skillab in __instance._skillAbilityList) MainClass.Logg.LogInfo("index: " + skillab._index);
				
			//}
		}

		private static void InjectSkillAbility(SkillModel skillModel_inst, int ability_idx, AbilityData abilityData, string abilityScriptname)
		{
			//ModularSA modsa = Activator.CreateInstance(typeof(ModularSA)) as ModularSA;
			//ModularSA modsa = new ModularSA();
			MainClass.Logg.LogInfo("injecting");
			var modsa = new ModularSA();
			MainClass.Logg.LogInfo("new modsa");
			//MODSA_HelloWorld modsa = Activator.CreateInstance(typeof(MODSA_HelloWorld)) as MODSA_HelloWorld;
			//ModularScripts.modsa_list.Add(modsa);
			//modsa.skillmodel_ref = skillModel_inst;

			//modsa.Init(skillModel_inst, abilityScriptname, ability_idx, abilityData.BuffData);
			modsa.modsa_skillModel = skillModel_inst;
			//GCHandle handle = GCHandle.Alloc(skillModel_inst);
			//IntPtr pointer = (IntPtr)handle;
			modsa.ptr_intlong = skillModel_inst.Pointer.ToInt64();
			MainClass.Logg.LogInfo("modsa init: " + modsa.ptr_intlong.ToString());
			//skillModel_inst._skillAbilityList.Add(modsa);
			//MainClass.Logg.LogInfo("addtolist");
			//modsa.skillModel = skillModel_inst;
			modsa.SetupModular(abilityScriptname.Remove(0, 8));
			MainClass.Logg.LogInfo("setup modular");
			modsaglobal_list.Add(modsa);
			MainClass.Logg.LogInfo("added to modsaglobal");
		}

		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.Init))]
		[HarmonyPrefix]
		private static void Prefix_PassiveModel_Init(BattleUnitModel owner, PassiveModel __instance)
		{
			if (__instance._script != null) return;
			
			bool isModular = false;
			List<string> requireIDList = __instance.ClassInfo.requireIDList;
			for (int i = 0; i < requireIDList.Count; i++) {
				string param = requireIDList[i];
				if (param.StartsWith("Modular/")) { isModular = true; break; }
			}

			if (isModular)
			{
				PassiveAbility pa = new PassiveAbility();
				pa.Init(owner, __instance.ClassInfo.attributeResonanceCondition, __instance.ClassInfo.attributeStockCondition);
				__instance._script = pa;
			}
		}

		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.Init))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_Init(BattleUnitModel owner, PassiveModel __instance)
		{
			List<string> requireIDList = __instance.ClassInfo.requireIDList;
			for (int i = 0; i < requireIDList.Count; i++)
			{
				string param = requireIDList[i];
				if (!param.StartsWith("Modular/")) continue;

				long ptr = __instance.Pointer.ToInt64();
				bool existsAlready = false;
				foreach (ModularSA existingModpa in modpa_list)
				{
					if (existingModpa.ptr_intlong == ptr && existingModpa.originalString == param)
					{
						existsAlready = true;
						MainClass.Logg.LogInfo(ptr + ": exists already " + param);
						existingModpa.ResetValueList();
						existingModpa.ResetAdders();
						existingModpa.modsa_unitModel = owner;
						break;
					}
				}
				if (existsAlready) continue;

				var modpa = new ModularSA();
				modpa.originalString = param;
				modpa.ptr_intlong = ptr;
				modpa.passiveID = __instance.ClassInfo.ID;
				modpa.abilityMode = 2; // 2 means passive
				modpa.modsa_unitModel = owner;
				MainClass.Logg.LogInfo("modPassiveAbility init: " + param);
				modpa.SetupModular(param.Remove(0, 8));
				modpa_list.Add(modpa);
			}
		}

		[HarmonyPatch(typeof(CoinModel), nameof(CoinModel.Init))]
		[HarmonyPostfix]
		private static void Postfix_CoinModel_Init(CoinModel __instance)
		{
			if (modca_list.Count > 1000)
			{
				List<ModularSA> goodones = new List<ModularSA>(500);
				for (int i = 0; i < 500; i++)
				{
					goodones[i] = modca_list[modca_list.Count - 500 + i];
				}
				modca_list.Clear();
				modca_list = goodones;
				MainClass.Logg.LogInfo("Refreshed ModcaList");
			}

			List<AbilityData> abilityData_list = __instance.ClassInfo.abilityScriptList;
			for (int i = 0; i < abilityData_list.Count; i++)
			{
				AbilityData abilityData = abilityData_list[i];
				string abilityScriptname = abilityData.ScriptName;
				if (!abilityScriptname.StartsWith("Modular/")) continue;

				long ptr = __instance.Pointer.ToInt64();
				bool existsAlready = false;
				foreach (ModularSA existingModca in modca_list)
				{
					if (existingModca.ptr_intlong == ptr && existingModca.originalString == abilityScriptname)
					{
						existsAlready = true;
						MainClass.Logg.LogInfo(ptr + ": exists already " + abilityScriptname);
						existingModca.ResetValueList();
						existingModca.ResetAdders();
						break;
					}
				}
				if (existsAlready) continue;

				var modca = new ModularSA();
				modca.originalString = abilityScriptname;
				modca.ptr_intlong = ptr;
				modca.abilityMode = 1; // 1 means coin
				MainClass.Logg.LogInfo("modCoinAbility init: " + abilityScriptname);
				modca.SetupModular(abilityScriptname.Remove(0, 8));
				modca_list.Add(modca);
			}
		}

		public static void ResetAllModsa()
		{
			foreach (ModularSA modular in modsaglobal_list) { modular.EraseAllData(); }
			foreach (ModularSA modular in modpa_list) { modular.EraseAllData(); }
			foreach (ModularSA modular in modca_list) { modular.EraseAllData(); }

			modsaglobal_list.Clear();
			modpa_list.Clear();
			modca_list.Clear();
			unitMod_list.Clear();
			skillPtrsRoundStart.Clear();
		}

		public static List<ModularSA> modsaglobal_list = new List<ModularSA>();
		public static List<ModularSA> modpa_list = new List<ModularSA>();
		public static List<ModularSA> modca_list = new List<ModularSA>();
		public static List<ModUnitData> unitMod_list = new List<ModUnitData>();
		public static List<long> skillPtrsRoundStart = new List<long>();

		//[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.IsActive))]
		//[HarmonyPostfix]
		//private static void Postfix_PassiveModel_IsActive(PassiveModel __instance, ref bool __result)
		//{
		//	List<string> requireIDList = __instance.ClassInfo.requireIDList;
		//	for (int i = 0; i < requireIDList.Count; i++)
		//	{
		//		string param = requireIDList[i];
		//		if (param.StartsWith("Modular/"))
		//		{
		//			__result = true;
		//			break;
		//		}
		//	}
		//}

		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnRoundStart_After_Event))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnRoundStart_After_Event(BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (ModularSA modpa in modpa_list) { modpa.ResetAdders(); }

			foreach (PassiveModel passiveModel in __instance.PassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				List<string> requireIDList = passiveModel.ClassInfo.requireIDList;
				foreach (string param in requireIDList)
				{
					if (param.StartsWith("Modular/"))
					{
						passiveModel.OnRoundStart_After_Event(timing);
						break;
					}
				}
			}
		}
		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.OnRoundStart_After_Event))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_OnRoundStart_After_Event(BATTLE_EVENT_TIMING timing, PassiveModel __instance)
		{
			if (__instance.Owner == null) MainClass.Logg.LogInfo("THE FUCKING PASSIVE OWNER IS NULLLLL");
			long passiveModel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modpa in modpa_list)
			{
				if (modpa.passiveID != __instance.ClassInfo.ID) continue;
				if (passiveModel_intlong != modpa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modpassive - RoundStart: " + modpa.passiveID);
				modpa.modsa_passiveModel = __instance;
				modpa.modsa_unitModel = __instance.Owner;
				modpa.Enact(null, -1, timing);
			}

			foreach (SinActionModel sinAction in __instance.Owner.GetSinActionList())
			{
				foreach (UnitSinModel sinModel in __instance.Owner.GetSinActionList()[0].currentSinList)
				{
					SkillModel skillModel = sinModel.GetSkill();
					if (skillModel == null) continue;
					long skillmodel_intlong = skillModel.Pointer.ToInt64();
					if (!skillPtrsRoundStart.Contains(skillmodel_intlong)) continue;

					foreach (ModularSA modsa in modsaglobal_list)
					{
						if (skillmodel_intlong != modsa.ptr_intlong) continue;
						MainClass.Logg.LogInfo("Found modsa - RoundStart");
						modsa.modsa_unitModel = __instance.Owner;
						modsa.Enact(skillModel, -1, timing);
					}
				}
			}
		}



		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnBattleStart))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnBattleStart(BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				List<string> requireIDList = passiveModel.ClassInfo.requireIDList;
				foreach (string param in requireIDList)
				{
					if (param.StartsWith("Modular/"))
					{
						passiveModel.OnBattleStart(timing);
						break;
					}
				}
			}
		}
		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.OnBattleStart))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_OnBattleStart(BATTLE_EVENT_TIMING timing, PassiveModel __instance)
		{
			//MainClass.Logg.LogInfo("postfix passive combat start");
			long passiveModel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modpa in modpa_list)
			{
				MainClass.Logg.LogInfo("iterating modpassives combat start: " + modpa.passiveID + " | " + __instance.ClassInfo.ID);
				if (modpa.passiveID != __instance.ClassInfo.ID) continue;
				if (passiveModel_intlong != modpa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modpassive - combat start: " + modpa.passiveID);
				modpa.modsa_passiveModel = __instance;
				modpa.modsa_unitModel = __instance.Owner;
				modpa.Enact(null, 0, timing);
			}
		}



		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnBattleEnd))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnBattleEnd(BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				List<string> requireIDList = passiveModel.ClassInfo.requireIDList;
				foreach (string param in requireIDList)
				{
					if (param.StartsWith("Modular/"))
					{
						passiveModel.OnBattleEnd(timing);
						break;
					}
				}
			}
		}
		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.OnBattleEnd))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_OnBattleEnd(BATTLE_EVENT_TIMING timing, PassiveModel __instance)
		{
			long passiveModel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modpa in modpa_list)
			{
				MainClass.Logg.LogInfo("iterating modpassives combat end: " + modpa.passiveID + " | " + __instance.ClassInfo.ID);
				if (modpa.passiveID != __instance.ClassInfo.ID) continue;
				if (passiveModel_intlong != modpa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modpassive - combat end: " + modpa.passiveID);
				modpa.modsa_passiveModel = __instance;
				modpa.modsa_unitModel = __instance.Owner;
				modpa.Enact(null, 6, timing);
			}
		}

		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.IsAbnormalityImmortal))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_IsImmortal(BATTLE_EVENT_TIMING timing, int newHp, bool isInstantDeath, ref bool __result, PassiveDetail __instance)
		{
			PassiveDetail pasdet = new PassiveDetail(null, null);

			if (isInstantDeath) return;
			foreach (PassiveModel passiveModel in __instance.PassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				List<string> requireIDList = passiveModel.ClassInfo.requireIDList;
				foreach (string param in requireIDList)
				{
					if (param.StartsWith("Modular/"))
					{
						if (passiveModel.IsAbnormalityImmortal(timing, newHp, isInstantDeath)) __result = true;
						break;
					}
				}
			}
		}

		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.IsAbnormalityImmortal))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_IsImmortal(BATTLE_EVENT_TIMING timing, int newHp, bool isInstantDeath, ref bool __result, PassiveModel __instance)
		{
			//MainClass.Logg.LogInfo("postfix passive combat start");
			long passiveModel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modpa in modpa_list)
			{
				if (modpa.passiveID != __instance.ClassInfo.ID) continue;
				if (passiveModel_intlong != modpa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modpassive - immortal: " + modpa.passiveID);
				if (modpa.IsImmortal()) __result = true;
			}
		}



		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnStartTurn_BeforeLog))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnStartTurn_BeforeLog(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (ModularSA modpa in modpa_list) // Resets passive adders for modpa timings that suck
			{
				if (modpa.activationTiming != 2 && modpa.activationTiming != 20) continue;
				modpa.ResetAdders();
			}

			foreach (PassiveModel passiveModel in __instance.PassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				List<string> requireIDList = passiveModel.ClassInfo.requireIDList;
				foreach (string param in requireIDList)
				{
					if (param.StartsWith("Modular/"))
					{
						passiveModel.OnStartTurn_BeforeLog(action, timing);
						break;
					}
				}
			}
		}
		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.OnStartTurn_BeforeLog))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_OnStartTurn_BeforeLog(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveModel __instance)
		{
			long passiveModel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modpa in modpa_list)
			{
				if (modpa.passiveID != __instance.ClassInfo.ID) continue;
				if (passiveModel_intlong != modpa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modpassive - OnStartTurn_BeforeLog: " + modpa.passiveID);
				modpa.modsa_passiveModel = __instance;
				modpa.modsa_unitModel = __instance.Owner;
				modpa.modsa_selfAction = action;
				modpa.Enact(action.Skill, 1, timing);
			}
		}



		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.BeforeAttack))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_BeforeAttack(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				List<string> requireIDList = passiveModel.ClassInfo.requireIDList;
				foreach (string param in requireIDList)
				{
					if (param.StartsWith("Modular/")) {
						passiveModel.BeforeAttack(action, timing);
						break;
					}
				}
			}
		}
		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.BeforeAttack))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_BeforeAttack(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveModel __instance)
		{
			long passiveModel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modpa in modpa_list) {
				if (modpa.passiveID != __instance.ClassInfo.ID) continue;
				if (passiveModel_intlong != modpa.ptr_intlong) continue;
				modpa.modsa_passiveModel = __instance;
				modpa.modsa_unitModel = __instance.Owner;
				modpa.modsa_selfAction = action;
				modpa.Enact(action.Skill, 2, timing);
			}
		}



		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnEndTurn))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnEndTurn(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				List<string> requireIDList = passiveModel.ClassInfo.requireIDList;
				foreach (string param in requireIDList)
				{
					if (param.StartsWith("Modular/"))
					{
						passiveModel.OnEndTurn(action, timing);
						break;
					}
				}
			}
		}
		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.OnEndTurn))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_OnEndTurn(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveModel __instance)
		{
			long passiveModel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modpa in modpa_list)
			{
				if (modpa.passiveID != __instance.ClassInfo.ID) continue;
				if (passiveModel_intlong != modpa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modpassive - OnEndTurn: " + modpa.passiveID);
				modpa.modsa_passiveModel = __instance;
				modpa.modsa_unitModel = __instance.Owner;
				modpa.modsa_selfAction = action;
				modpa.Enact(action.Skill, 9, timing);
			}
		}



		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnEndBehaviour))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnEndBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				List<string> requireIDList = passiveModel.ClassInfo.requireIDList;
				foreach (string param in requireIDList)
				{
					if (param.StartsWith("Modular/")) {
						passiveModel.OnEndBehaviour(action, timing);
						break;
					}
				}
			}
		}
		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.OnEndBehaviour))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_OnEndBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveModel __instance)
		{
			long passiveModel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modpa in modpa_list) {
				if (modpa.passiveID != __instance.ClassInfo.ID) continue;
				if (passiveModel_intlong != modpa.ptr_intlong) continue;
				modpa.modsa_passiveModel = __instance;
				modpa.modsa_unitModel = __instance.Owner;
				modpa.modsa_selfAction = action;
				modpa.Enact(action.Skill, 20, timing);
			}
		}




		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnBeforeDefense))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnBeforeDefense(BattleActionModel action, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				List<string> requireIDList = passiveModel.ClassInfo.requireIDList;
				foreach (string param in requireIDList)
				{
					if (param.StartsWith("Modular/"))
					{
						passiveModel.OnBeforeDefense(action);
						break;
					}
				}
			}
		}
		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.OnBeforeDefense))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_OnBeforeDefense(BattleActionModel action, PassiveModel __instance)
		{
			long passiveModel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modpa in modpa_list)
			{
				if (modpa.passiveID != __instance.ClassInfo.ID) continue;
				if (passiveModel_intlong != modpa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modpassive - OnBeforeDefense: " + modpa.passiveID);
				modpa.modsa_passiveModel = __instance;
				modpa.modsa_unitModel = __instance.Owner;
				modpa.modsa_selfAction = action;
				modpa.Enact(action.Skill, 11, BATTLE_EVENT_TIMING.NONE);
			}
		}


		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnDie))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnDie(BattleUnitModel killer, BattleActionModel actionOrNull, DAMAGE_SOURCE_TYPE dmgSrcType, BUFF_UNIQUE_KEYWORD keyword, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				List<string> requireIDList = passiveModel.ClassInfo.requireIDList;
				foreach (string param in requireIDList)
				{
					if (param.StartsWith("Modular/"))
					{
						passiveModel.OnDie(killer, actionOrNull, dmgSrcType, keyword, timing);
						break;
					}
				}
			}
		}
		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.OnDie))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_OnDie(BattleUnitModel killer, BattleActionModel actionOrNull, DAMAGE_SOURCE_TYPE dmgSrcType, BUFF_UNIQUE_KEYWORD keyword, BATTLE_EVENT_TIMING timing, PassiveModel __instance)
		{
			long passiveModel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modpa in modpa_list)
			{
				if (modpa.passiveID != __instance.ClassInfo.ID) continue;
				if (passiveModel_intlong != modpa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modpassive - OnDie: " + modpa.passiveID);
				modpa.modsa_passiveModel = __instance;
				modpa.modsa_unitModel = __instance.Owner;
				modpa.modsa_target_list.Add(killer);
				modpa.Enact(null, 12, timing);
			}
		}

		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnDieOtherUnit))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnDieOtherUnit(BattleUnitModel killer, BattleUnitModel dead, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				List<string> requireIDList = passiveModel.ClassInfo.requireIDList;
				foreach (string param in requireIDList)
				{
					if (param.StartsWith("Modular/"))
					{
						passiveModel.OnDieOtherUnit(killer, dead, timing, DAMAGE_SOURCE_TYPE.PASSIVE, BUFF_UNIQUE_KEYWORD.None);
						break;
					}
				}
			}
		}
		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.OnDieOtherUnit))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_OnDieOtherUnit(BattleUnitModel dead, BATTLE_EVENT_TIMING timing, PassiveModel __instance)
		{
			long passiveModel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modpa in modpa_list)
			{
				if (modpa.passiveID != __instance.ClassInfo.ID) continue;
				if (passiveModel_intlong != modpa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modpassive - OnDieOtherUnit: " + modpa.passiveID);
				modpa.modsa_passiveModel = __instance;
				modpa.modsa_unitModel = __instance.Owner;
				modpa.modsa_target_list.Add(dead);
				modpa.Enact(null, 13, timing);
			}
		}

		[HarmonyPatch(typeof(BattleUnitModel_Abnormality), nameof(BattleUnitModel_Abnormality.GetActionSlotAdder))]
		[HarmonyPostfix]
		private static void Postfix_BattleUnitModel_Abnormality_GetActionSlotAdder(ref int __result, BattleUnitModel_Abnormality __instance)
		{
			foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
			{
				List<string> requireIDList = passiveModel.ClassInfo.requireIDList;
				foreach (string param in requireIDList) {
					if (!param.StartsWith("Modular/")) continue;
					__result += passiveModel.GetActionSlotAdder();
					break;
				}
			}
		}
		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.GetActionSlotAdder))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_GetActionSlotAdder(ref int __result, PassiveModel __instance)
		{
			long passiveModel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modpa in modpa_list)
			{
				if (modpa.passiveID != __instance.ClassInfo.ID) continue;
				if (passiveModel_intlong != modpa.ptr_intlong) continue;
				__result += modpa.slotAdder;
			}
		}

		// PASSIVES END


		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetCoinScaleAdder))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetCoinScaleAdder(BattleActionModel action, ref int __result, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (modsa.activationTiming == 10) continue;
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				int power = modsa.coinScaleAdder;
				MainClass.Logg.LogInfo("Found modsa - coin scale adder: " + power);
				__result += power;
			}
			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList)
			{
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in modpa_list)
				{
					if (modpa.activationTiming == 10) continue;
					if (passivemodel_intlong != modpa.ptr_intlong) continue;
					int power = modpa.coinScaleAdder;
					if (power != 0) MainClass.Logg.LogInfo("Found modpa - coin scale adder: ");
					__result += power;
				}
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetSkillPowerAdder))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetSkillPowerAdder(BattleActionModel action, ref int __result, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (modsa.activationTiming == 10) continue;
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				int power = modsa.skillPowerAdder;
				if (power != 0) MainClass.Logg.LogInfo("Found modsa - base power adder: " + power);
				__result += power;
			}
			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList)
			{
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in modpa_list)
				{
					if (modpa.activationTiming == 10) continue;
					if (passivemodel_intlong != modpa.ptr_intlong) continue;
					int power = modpa.skillPowerAdder;
					if (power != 0) MainClass.Logg.LogInfo("Found modpa - base power adder: ");
					__result += power;
				}
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetSkillPowerResultAdder))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetSkillPowerResultAdder(BattleActionModel action, ref int __result, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (modsa.activationTiming == 10) continue;
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				int power = modsa.skillPowerResultAdder;
				if (power != 0) MainClass.Logg.LogInfo("Found modsa - final power adder: " + power);
				__result += power;
			}
			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList)
			{
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in modpa_list)
				{
					if (modpa.activationTiming == 10) continue;
					if (passivemodel_intlong != modpa.ptr_intlong) continue;
					int power = modpa.skillPowerResultAdder;
					if (power != 0) MainClass.Logg.LogInfo("Found modpa - final power adder: ");
					__result += power;
				}
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetParryingResultAdder))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetParryingResultAdder(BattleActionModel actorAction, ref int __result, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (modsa.activationTiming == 10) continue;
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				int power = modsa.parryingResultAdder;
				if (power != 0) MainClass.Logg.LogInfo("Found modsa - clash power adder: " + power);
				__result += power;
			}
			foreach (PassiveModel passiveModel in actorAction.Model._passiveDetail.PassiveList)
			{
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in modpa_list)
				{
					if (modpa.activationTiming == 10) continue;
					if (passivemodel_intlong != modpa.ptr_intlong) continue;
					int power = modpa.parryingResultAdder;
					if (power != 0) MainClass.Logg.LogInfo("Found modpa - clash power adder: " + power);
					__result += power;
				}
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetAttackDmgAdder))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetAttackDmgAdder(BattleActionModel action, CoinModel coin, ref int __result, SkillModel __instance)
		{
			long coinmodel_intlong = coin.Pointer.ToInt64();
			foreach (ModularSA modca in modca_list)
			{
				if (modca.activationTiming == 10) continue;
				if (coinmodel_intlong != modca.ptr_intlong) continue;
				int power = modca.atkDmgAdder;
				if (power != 0)
				{
					MainClass.Logg.LogInfo("Found modca - atkdmg adder: " + power);
					__result += power;
				}
			}
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (modsa.activationTiming == 10) continue;
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				int power = modsa.atkDmgAdder;
				if (power != 0)
				{
					MainClass.Logg.LogInfo("Found modsa - atkdmg adder: " + power);
					__result += power;
				}
			}
			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList)
			{
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in modpa_list)
				{
					if (modpa.activationTiming == 10) continue;
					if (passivemodel_intlong != modpa.ptr_intlong) continue;
					int power = modpa.atkDmgAdder;
					if (power != 0)
					{
						MainClass.Logg.LogInfo("Found modpa - atkdmg adder: " + power);
						__result += power;
					}
				}
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetAttackDmgMultiplier))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetAttackDmgMultiplier(BattleActionModel action, CoinModel coin, ref float __result, SkillModel __instance)
		{
			long coinmodel_intlong = coin.Pointer.ToInt64();
			foreach (ModularSA modca in modca_list)
			{
				if (modca.activationTiming == 10) continue;
				if (coinmodel_intlong != modca.ptr_intlong) continue;
				int power = modca.atkMultAdder;
				if (power != 0)
				{
					MainClass.Logg.LogInfo("Found modca - atkmult adder: " + power);
					__result += (float)power * 0.01f;
				}
			}
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (modsa.activationTiming == 10) continue;
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				int power = modsa.atkMultAdder;
				if (power != 0)
				{
					MainClass.Logg.LogInfo("Found modsa - atkmult adder: " + power);
					__result += (float)power * 0.01f;
				}
			}
			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList)
			{
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in modpa_list)
				{
					if (modpa.activationTiming == 10) continue;
					if (passivemodel_intlong != modpa.ptr_intlong) continue;
					int power = modpa.atkMultAdder;
					if (power != 0)
					{
						MainClass.Logg.LogInfo("Found modpa - atkmult adder: " + power);
						__result += (float)power * 0.01f;
					}
				}
			}
		}


		[HarmonyPatch(typeof(SkillModelManager), nameof(SkillModelManager.GetExpectedWinRate))]
		[HarmonyPrefix]
		private static void Postfix_SkillModelManager_GetExpectedWinRate(BattleActionModel selfAction)
		{
			foreach (ModularSA modpa in modpa_list) { modpa.ResetAdders(); }

			long skillmodel_intlong = selfAction.Skill.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				modsa.modsa_selfAction = selfAction;
				modsa.Enact(selfAction.Skill, 10, BATTLE_EVENT_TIMING.NONE);
			}
			foreach (PassiveModel passiveModel in selfAction.Model._passiveDetail.PassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in modpa_list)
				{
					if (passivemodel_intlong != modpa.ptr_intlong) continue;
					modpa.modsa_selfAction = selfAction;
					modpa.Enact(selfAction.Skill, 10, BATTLE_EVENT_TIMING.NONE);
				}
			}
		}


		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedSkillPowerAdder))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetExpectedSkillPowerAdder(BattleActionModel action, ref int __result, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (modsa.activationTiming != 10) continue;
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				int power = modsa.skillPowerAdder;
				__result += power;
			}
			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList)
			{
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in modpa_list)
				{
					if (modpa.activationTiming != 10) continue;
					if (passivemodel_intlong != modpa.ptr_intlong) continue;
					int power = modpa.skillPowerAdder;
					__result += power;
				}
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedSkillPowerResultAdder))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetExpectedSkillPowerResultAdder(BattleActionModel action, ref int __result, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (modsa.activationTiming != 10) continue;
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				int power = modsa.skillPowerResultAdder;
				__result += power;
			}
			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList)
			{
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in modpa_list)
				{
					if (modpa.activationTiming != 10) continue;
					if (passivemodel_intlong != modpa.ptr_intlong) continue;
					int power = modpa.skillPowerResultAdder;
					__result += power;
				}
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedParryingResultAdder))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetExpectedParryingResultAdder(BattleActionModel actorAction, ref int __result, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (modsa.activationTiming != 10) continue;
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				int power = modsa.parryingResultAdder;
				__result += power;
			}
			foreach (PassiveModel passiveModel in actorAction.Model._passiveDetail.PassiveList)
			{
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in modpa_list)
				{
					if (modpa.activationTiming != 10) continue;
					if (passivemodel_intlong != modpa.ptr_intlong) continue;
					int power = modpa.parryingResultAdder;
					__result += power;
				}
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnBattleStart))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnBattleStart(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modsa - battlestart");
				modsa.modsa_selfAction = action;
				modsa.Enact(__instance, 0, timing);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnStartTurn_BeforeLog))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnStartTurnBeforeLog(BattleActionModel action, List<BattleUnitModel> targets, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				// Reset clash modifiers if for some reason this skill is used again
				if (modsa.activationTiming == 14 || modsa.activationTiming == 15) modsa.ResetAdders();
				// normal code
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modsa - onuse");
				modsa.modsa_selfAction = action;
				modsa.modsa_target_list = targets;
				modsa.Enact(__instance, 1, timing);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.BeforeAttack))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_BeforeAttack(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modsa (but from globallist)");
				modsa.modsa_selfAction = action;
				modsa.Enact(__instance, 2, timing);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnBeforeParryingOnce))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnBeforeParryingOnce(BattleActionModel ownerAction, BattleActionModel oppoAction, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				modsa.modsa_selfAction = ownerAction;
				modsa.modsa_oppoAction = oppoAction;
				modsa.Enact(__instance, 14, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnBeforeParryingOnce_AfterLog))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnBeforeParryingOnce_AfterLog(BattleActionModel ownerAction, BattleActionModel oppoAction, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				modsa.modsa_selfAction = ownerAction;
				modsa.modsa_oppoAction = oppoAction;
				modsa.Enact(__instance, 15, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnWinDuel))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnWinDuel(BattleActionModel selfAction, BattleActionModel oppoAction, BATTLE_EVENT_TIMING timing, int parryingCount, SkillModel __instance)
		{
			//MainClass.Logg.LogInfo("Won Duel");
			//foreach (var skillab in __instance.SkillAbilityList)
			//{
			//	MainClass.Logg.LogInfo("skillab search: "+ skillab._index);
			//	ModularSA modsa = skillab as ModularSA;
			//	if (modsa == null) continue;
			//	MainClass.Logg.LogInfo("Found modsa");
			//	modsa.selfAction = selfAction;
			//	modsa.oppoAction = oppoAction;
			//	modsa.Enact(__instance, 4);
			//}
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			//MainClass.Logg.LogInfo("instance ptr: " + skillmodel_intlong);
			foreach (ModularSA modsa in modsaglobal_list)
			{
				//MainClass.Logg.LogInfo("iterating modsaglobal_list: " + modsa.skillModel_ptr_intlong.ToString());
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modsa (but from globallist)");
				modsa.modsa_selfAction = selfAction;
				modsa.modsa_oppoAction = oppoAction;
				modsa.Enact(__instance, 4, timing);
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnLoseDuel))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnLoseDuel(BattleActionModel selfAction, BattleActionModel oppoAction, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modsa (but from globallist)");
				modsa.modsa_selfAction = selfAction;
				modsa.modsa_oppoAction = oppoAction;
				modsa.Enact(__instance, 5, timing);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnRoundEnd))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnRoundEnd(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modsa (in skill, EndBattle)");
				modsa.modsa_selfAction = action;
				modsa.Enact(__instance, 6, timing);
			}
			
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnEndTurn))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnEndTurn(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modsa (in skill, EndSkill)");
				modsa.modsa_selfAction = action;
				modsa.Enact(__instance, 9, timing);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnSucceedEvade))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnSucceedEvade(BattleActionModel attackerAction, BattleActionModel evadeAction, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				modsa.modsa_selfAction = evadeAction;
				modsa.modsa_oppoAction = attackerAction;
				modsa.Enact(__instance, 16, timing);
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnFailedEvade))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnFailedEvade(BattleActionModel attackerAction, BattleActionModel evadeAction, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				modsa.modsa_selfAction = evadeAction;
				modsa.modsa_oppoAction = attackerAction;
				modsa.Enact(__instance, 17, timing);
			}
		}




		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnStartBehaviour))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnStartBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				modsa.modsa_selfAction = action;
				modsa.Enact(__instance, 18, timing);
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.BeforeBehaviour))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_BeforeBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				modsa.modsa_selfAction = action;
				modsa.Enact(__instance, 19, timing);
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnEndBehaviour))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnEndBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				modsa.modsa_selfAction = action;
				modsa.Enact(__instance, 20, timing);
			}
		}





		// SKILLMODEL END

		//[HarmonyPatch(typeof(BattleActionModel), nameof(BattleActionModel.OnAttackConfirmed))]
		//[HarmonyPrefix]
		//private static void Prefix_BattleActionModel_OnAttackConfirmed(CoinModel coin, BattleUnitModel target, BATTLE_EVENT_TIMING timing, bool isCritical, BattleActionModel __instance)
		//{
		//	long coinmodel_intlong = coin.Pointer.ToInt64();
		//	MainClass.Logg.LogInfo("OnAttackConfirmed timing: " + timing.ToString());
		//	foreach (ModularSA modca in modca_list)
		//	{
		//		if (coinmodel_intlong != modca.ptr_intlong) continue;
		//		MainClass.Logg.LogInfo("Found modca (in coin, succeed attack)");
		//		//modca.lastFinalDmg = finalDmg;
		//		modca.wasCrit = isCritical;
		//		//modca.wasClash = isWinDuel.HasValue;
		//		//if (modca.wasClash) modca.wasWin = isWinDuel.Value;
		//		modca.modsa_selfAction = __instance;
		//		modca.modsa_coinModel = coin;
		//		modca.Enact(__instance.Skill, 7, timing);
		//	}

		//	foreach (PassiveModel passiveModel in __instance.Model._passiveDetail.PassiveList)
		//	{
		//		long passivemodel_intlong = passiveModel.Pointer.ToInt64();
		//		foreach (ModularSA modpa in modpa_list)
		//		{
		//			if (passivemodel_intlong != modpa.ptr_intlong) continue;
		//			MainClass.Logg.LogInfo("Found modpa (in coin, succeed attack)");
		//			modpa.wasCrit = isCritical;
		//			modpa.modsa_selfAction = __instance;
		//			modpa.modsa_coinModel = coin;
		//			modpa.Enact(__instance.Skill, 7, timing);
		//		}
		//	}
		//}

		[HarmonyPatch(typeof(BattleActionModel), nameof(BattleActionModel.OnAttackConfirmed))]
		[HarmonyPostfix]
		private static void Postfix_BattleActionModel_OnAttackConfirmed(CoinModel coin, BattleUnitModel target, BATTLE_EVENT_TIMING timing, bool isCritical, BattleActionModel __instance)
		{
			long coinmodel_intlong = coin.Pointer.ToInt64();
			foreach (ModularSA modca in modca_list)
			{
				if (coinmodel_intlong != modca.ptr_intlong) continue;
				MainClass.Logg.LogInfo("Found modca (in coin, OnAttackConfirmed)");
				//modca.lastFinalDmg = finalDmg;
				modca.wasCrit = isCritical;
				//modca.wasClash = isWinDuel.HasValue;
				//if (modca.wasClash) modca.wasWin = isWinDuel.Value;
				modca.modsa_selfAction = __instance;
				modca.modsa_coinModel = coin;
				modca.Enact(__instance.Skill, 7, timing);
			}

			long skillmodel_intlong = __instance.Skill.Pointer.ToInt64();
			foreach (ModularSA modsa in modsaglobal_list)
			{
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				modsa.modsa_selfAction = __instance;
				modsa.Enact(__instance.Skill, 7, timing);
			}

			foreach (PassiveModel passiveModel in __instance.Model._passiveDetail.PassiveList)
			{
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in modpa_list)
				{
					if (passivemodel_intlong != modpa.ptr_intlong) continue;
					MainClass.Logg.LogInfo("Found modpa (in coin, OnAttackConfirmed)");
					modpa.wasCrit = isCritical;
					modpa.modsa_selfAction = __instance;
					modpa.modsa_coinModel = coin;
					modpa.Enact(__instance.Skill, 7, timing);
				}
			}

		}


		// end
	}
}
