using System;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts
{
	internal class StagePatches
	{
		public static int extraSlot = 0;
		public static bool instantslot = false;
		public static List<int> doubleslotterIDList = new List<int>();
		public static bool forceSlotGain = false;

		[HarmonyPatch(typeof(StageModel), nameof(StageModel.Init))]
		[HarmonyPrefix]
		private static void Prefix_StageModel_Init(StageStaticData stageinfo, StageModel __instance)
		{
			SkillScriptInitPatch.ResetAllModsa();

			extraSlot = 0;
			instantslot = false;
			doubleslotterIDList.Clear();
			forceSlotGain = false;
			MainClass.Logg.LogInfo("Prefix_StageModel_Init");

			List<string> stageScriptList = stageinfo.stageScriptList;
			foreach (string stageScript_string in stageScriptList)
			{
				MainClass.Logg.LogInfo("Looping stage scriptnames: " + stageScript_string);
				if (!stageScript_string.StartsWith("Modular/")) continue;
				MainClass.Logg.LogInfo("Found Modular/");
				string instruction = stageScript_string.Remove(0, 8);
				string[] batches = instruction.Split('/');

				foreach (string batch in batches)
				{
					MainClass.Logg.LogInfo("Batches loop: " + batch);
					if (batch.StartsWith("extraslot:")) int.TryParse(batch.Remove(0, 10), out extraSlot);
					else if (batch == "instantslot") instantslot = true;
					else if (batch == "forceslotgain") forceSlotGain = true;
				}
			}

		}

		[HarmonyPatch(typeof(StageController), nameof(StageController.StartRound))]
		[HarmonyPostfix]
		private static void Postfix_StageController_StartRound(StageController __instance)
		{
			int round = __instance.GetCurrentRound();
			MainClass.Logg.LogInfo("Postfix_StageController_StartRound | Round: " + round);

			int amount_before = SkillScriptInitPatch.modsaglobal_list.Count;
			int duplicate_bad = 0;
			int duplicate_good = 0;

			List<ModularSA> goodOnes = new List<ModularSA>(amount_before);
			foreach (ModularSA modsa in SkillScriptInitPatch.modsaglobal_list)
			{
				int deathTime = 5;
				if (modsa.modsa_skillModel != null && modsa.modsa_skillModel.IsEgoSkill()) deathTime = 2;
				if (modsa.interactionTimer < deathTime && !modsa.markedForDeath)
				{
					modsa.interactionTimer += 1;
					goodOnes.Add(modsa);
					if (modsa.modsa_skillModel != null)
					{
						if (modsa.modsa_skillModel.IsEgoSkill()) duplicate_good += 1;
					}
				}
				else
				{
					if (modsa.modsa_skillModel != null)
					{
						if (modsa.modsa_skillModel.IsEgoSkill()) duplicate_bad += 1;
					}
					modsa.EraseAllData();
				}
			}
			SkillScriptInitPatch.modsaglobal_list = goodOnes;
			int amount_after = SkillScriptInitPatch.modsaglobal_list.Count;
				
			MainClass.Logg.LogInfo("Modsa cleanup before: " + amount_before + " - after: " + amount_after);
			MainClass.Logg.LogInfo("Modsa EGO removed: " + duplicate_bad + " - EGO kept: " + duplicate_good);
			

			SinManager sinManager_inst = Singleton<SinManager>.Instance;
			BattleObjectManager _battleObjectManager = sinManager_inst._battleObjectManager;
			StageModel stageModel = __instance.StageModel;

			List<BattleUnitModel> playerUnit_list = _battleObjectManager.GetAliveList(false, UNIT_FACTION.PLAYER);

			foreach (BattleUnitModel unitModel in playerUnit_list)
			{
				List<string> unitKeywordList = unitModel._unitDataModel.ClassInfo.unitKeywordList;
				if (!unitKeywordList.Contains("doubleslot")) continue;
				if (unitModel._actionSlotDetail.GetSinActionList().Count < 2)
				{
					sinManager_inst.AddSinActionModelOnRoundStart(UNIT_FACTION.PLAYER, unitModel.InstanceID);
					if (!doubleslotterIDList.Contains(unitModel.InstanceID)) doubleslotterIDList.Add(unitModel.InstanceID);
				}

				continue;
				ActionSlotDetail _actionSlotDetail = unitModel._actionSlotDetail;
				List<SinActionModel> _sinActionList = _actionSlotDetail._sinActionList;
				int actionList_count = _sinActionList.Count;
				if (actionList_count > 1) continue;
				ActionSlotModel actionSlotModel = sinManager_inst.CreateActionSlotModel(unitModel, actionList_count);
				//sinManager_inst.AddSinAction

				SinActionModel_Player sinActionModel_Player = new SinActionModel_Player(unitModel, actionSlotModel, false);
				unitModel.AddSinAction(sinActionModel_Player);
			}

			bool isRailway = stageModel.StageType == STAGE_TYPE.RAILWAY_DUNGEON;
			if (isRailway || forceSlotGain)
			{
				MainClass.Logg.LogInfo("forceSlotGain");
				if (isRailway) MainClass.Logg.LogInfo("IS RAILWAY_DUNGEON");

				//List<SinActionModel> sinAction_list = sinManager_inst.GetActionListByFaction(UNIT_FACTION.PLAYER);
				int sinAction_count = 0;
				foreach (BattleUnitModel unitModel in playerUnit_list) sinAction_count += unitModel._actionSlotDetail.GetSinActionList().Count;
				MainClass.Logg.LogInfo("current player slot count:" + sinAction_count);
				
				int slot_max = stageModel.GetStageMaxParticipantCount();
				int multislot_max = (int)Math.Ceiling((double)slot_max / stageModel.ClassInfo.ParticipantInfo.Max);

				int highestSlotter = 1;
				while (sinAction_count < slot_max && highestSlotter <= multislot_max)
				{
					//int slot_max_loop = slot_max;
					//int multislot_max_loop = multislot_max;

					foreach (BattleUnitModel unitModel in playerUnit_list)
					{
						int thisUnitSlotCount = unitModel._actionSlotDetail.GetSinActionList().Count;
						if (doubleslotterIDList.Contains(unitModel.InstanceID)) thisUnitSlotCount -= 1;

						if (thisUnitSlotCount < highestSlotter) {
							sinManager_inst.AddSinActionModelOnRoundStart(UNIT_FACTION.PLAYER, unitModel.InstanceID);
							sinAction_count += 1;
						}
						if (sinAction_count >= slot_max) break;
					}

					highestSlotter += 1;
				}
			}
			
		}

		[HarmonyPatch(typeof(StageController), nameof(StageController.StartStage))]
		[HarmonyPostfix]
		private static void Postfix_StageController_StartStage(StageController __instance)
		{
			int round = __instance.GetCurrentRound();
			MainClass.Logg.LogInfo("Postfix_StageController_StartStage | Round: " + round);
			SinManager sinManager_inst = Singleton<SinManager>.Instance;
			
			if (instantslot) {
				MainClass.Logg.LogInfo("InstantSlot");
				StageModel stageModel = __instance.StageModel;
				if (stageModel == null) {
					MainClass.Logg.LogInfo("null stagemodel");
					return;
				}

				StageStaticData stageData = stageModel.ClassInfo;
				for (int i = 0; i < stageData.ParticipantInfo.Max; i++) {
					MainClass.Logg.LogInfo("gave slot i: " + i);
					sinManager_inst.CheckAddSinActionmodel(stageModel.StageType != STAGE_TYPE.STORY_DUNGEON, UNIT_FACTION.PLAYER);
				}
			}
		}


		//[HarmonyPatch(typeof(StageScriptBase), nameof(StageScriptBase.GetStageMaxParticipantCount))]
		//[HarmonyPostfix]
		//private static void Postfix_StageScriptBase_GetStageMaxParticipantCount(ref Il2CppSystem.Nullable<int> __result, StageScriptBase __instance)
		//{
		//	MainClass.Logg.LogInfo("Postfix_GetStageMaxParticipantCount, doubleslot: " + doubleslot);
		//	if (doubleslot)
		//	{
		//		StageController stageController_inst = Singleton<StageController>.Instance;
		//		StageModel stageModel = stageController_inst._stageModel;
		//		StageStaticData stageData = stageModel.ClassInfo;
		//		__result.value = stageData.ParticipantInfo.Max * 2;
		//	}
		//}

		

		[HarmonyPatch(typeof(StageModel), nameof(StageModel.GetStageMaxParticipantCount))]
		[HarmonyPostfix]
		private static void Postfix_StageModel_GetStageMaxParticipantCount(ref int __result, StageModel __instance)
		{
			int slotAdder = extraSlot;
			if (doubleslotterIDList.Count > 0) {
				SinManager sinManager_inst = Singleton<SinManager>.Instance;
				BattleObjectManager _battleObjectManager = sinManager_inst._battleObjectManager;
				foreach (int ID in doubleslotterIDList)
				{
					BattleUnitModel unitModel = _battleObjectManager.GetModel(ID);
					if (unitModel != null && !unitModel.IsDead()) slotAdder += 1;
				}
			}

			MainClass.Logg.LogInfo("Postfix_GetStageMaxParticipantCount, slotadder: " + slotAdder);
			if (slotAdder > 0) {
				StageStaticData stageData = __instance.ClassInfo;
				__result = Math.Min(stageData.ParticipantInfo.Max + slotAdder, 12);
				MainClass.Logg.LogInfo("ParticipantCountResult: " + __result);
			}
		}

		//[HarmonyPatch(typeof(StageModel), nameof(StageModel.CanAddNewSinActionModelForAlly))]
		//[HarmonyPostfix]
		//private static void Postfix_StageModel_CanAddNewSinActionModelForAlly(ref bool __result, StageModel __instance)
		//{
		//	int slotAdder = extraSlot;
		//	if (doubleslotterIDList.Count > 0)
		//	{
		//		SinManager sinManager_inst = Singleton<SinManager>.Instance;
		//		BattleObjectManager _battleObjectManager = sinManager_inst._battleObjectManager;
		//		foreach (int ID in doubleslotterIDList)
		//		{
		//			BattleUnitModel unitModel = _battleObjectManager.GetModel(ID);
		//			if (unitModel != null && !unitModel.IsDead()) slotAdder += 1;
		//		}
		//	}
		//	MainClass.Logg.LogInfo("CanAddNewSinActionModelForAlly, slotadder: " + slotAdder);
		//	if (slotAdder > 0) __result = false;
		//}

		// end
	}
}
