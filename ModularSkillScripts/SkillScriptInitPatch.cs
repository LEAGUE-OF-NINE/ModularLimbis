using System;
using BepInEx.Unity.IL2CPP.UnityEngine;
using Dungeon;
using HarmonyLib;
using Il2CppSystem.Collections;
using Il2CppSystem.Collections.Generic;
using static MirrorDungeonSelectThemeUIPanel.UIResources;

namespace ModularSkillScripts;

public class SkillScriptInitPatch
{
	private static void copypastesolution(BattleUnitModel unitModel, SkillModel skillModel_inst, BattleActionModel selfAction, BattleActionModel oppoAction, string actevent, BATTLE_EVENT_TIMING timing, PassiveDetail __instance, bool resetWhenUse = false)
	{
		int acteventint = MainClass.timingDict[actevent];
		foreach (PassiveModel passiveModel in __instance.PassiveList)
		{
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel))
			{
				if (resetWhenUse && modpa.resetWhenUse) modpa.ResetAdders(); // on-demand power adder reset (used for passives)
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(unitModel, skillModel_inst, selfAction, oppoAction, acteventint, timing);
			}
		}
		foreach (PassiveModel passiveModel in __instance.EgoPassiveList)
		{
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false))
			{
				if (resetWhenUse && modpa.resetWhenUse) modpa.ResetAdders(); // on-demand power adder reset (used for passives)
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(unitModel, skillModel_inst, selfAction, oppoAction, acteventint, timing);
			}
		}
	}
	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.Init), new Type[] { })]
	[HarmonyPostfix]
	private static void Postfix_SkillModelInit_AddSkillScript(SkillModel __instance)
	{
		long ptr = __instance.Pointer.ToInt64();

		List<AbilityData> abilityData_list = __instance.GetSkillAbilityScript();
		for (int i = 0; i < abilityData_list.Count; i++)
		{
			AbilityData abilityData = abilityData_list.ToArray()[i];
			string abilityScriptname = abilityData.ScriptName;
			if (!abilityScriptname.StartsWith("Modular/")) continue;
			if (!MainClass.fakepowerEnabled && abilityScriptname.Contains("FakePower")) continue;

			bool existsAlready = false;
			if (modsaDict.ContainsKey(ptr)) {
				foreach (ModularSA existingModsa in modsaDict[ptr]) {
					if (existingModsa.originalString != abilityScriptname) continue;
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
			if (!modsaDict.ContainsKey(ptr)) modsaDict.Add(ptr, new List<ModularSA>());
			modsaDict[ptr].Add(modsa);
		}
	}


	[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.Init))]
	[HarmonyPrefix]
	private static void Prefix_PassiveModel_Init(BattleUnitModel owner, PassiveModel __instance)
	{
		if (__instance._script != null) return;

		bool isModular = false;
		List<string> requireIDList = __instance.ClassInfo.requireIDList;
		for (int i = 0; i < requireIDList.Count; i++) {
			string param = requireIDList.ToArray()[i];
			if (param.StartsWith("Modular/")) { isModular = true; break; }
		}

		if (isModular)
		{
			PassiveAbility pa = new PassiveAbility();
			pa.Init(owner, __instance.ClassInfo.attributeResonanceCondition, __instance.ClassInfo.attributeStockCondition, new List<int>());
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
			string param = requireIDList.ToArray()[i];
			if (!param.StartsWith("Modular/")) continue;

			long ptr = __instance.Pointer.ToInt64();

			var modpa = new ModularSA();
			modpa.originalString = param;
			modpa.ptr_intlong = ptr;
			modpa.passiveID = __instance.ClassInfo.ID;
			modpa.abilityMode = 2; // 2 means passive
			modpa.modsa_unitModel = owner;
			MainClass.Logg.LogInfo("modPassiveAbility init: " + param);

			modpa.SetupModular(param.Remove(0, 8));
			if (!modpaDict.ContainsKey(ptr)) modpaDict.Add(ptr, new List<ModularSA>());
			modpaDict[ptr].Add(modpa);
		}
	}
	/*
	[HarmonyPatch(typeof(EgoPassiveModel), nameof(EgoPassiveModel.Init))]
	[HarmonyPrefix]
	private static void Prefix_EgoPassiveModel_Init(BattleUnitModel owner, EgoPassiveModel __instance)
	{
		Prefix_PassiveModel_Init(owner, __instance);
	}
	[HarmonyPatch(typeof(EgoPassiveModel), nameof(EgoPassiveModel.Init))]
	[HarmonyPostfix]
	private static void Postfix_EgoPassiveModel_Init(BattleUnitModel owner, EgoPassiveModel __instance)
	{
		Postfix_PassiveModel_Init(owner, __instance);
	}
	*/
	[HarmonyPatch(typeof(CoinModel), nameof(CoinModel.Init))]
	[HarmonyPostfix]
	private static void Postfix_CoinModel_Init(CoinModel __instance)
	{
		if (modca_list.Count > 1000)
		{
			List<ModularSA> goodones = new List<ModularSA>(500);
			for (int i = 0; i < 500; i++)
			{
				goodones.ToArray()[i] = modca_list.ToArray()[modca_list.Count - 500 + i];
			}
			modca_list.Clear();
			modca_list = goodones;
			MainClass.Logg.LogInfo("Refreshed ModcaList");
		}

		List<AbilityData> abilityData_list = __instance.ClassInfo.abilityScriptList;
		for (int i = 0; i < abilityData_list.Count; i++)
		{
			AbilityData abilityData = abilityData_list.ToArray()[i];
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


	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.Init))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_Init(BattleUnitModel owner, BuffModel __instance)
	{
		long ptr = __instance.Pointer.ToInt64();

		List<BuffAbilityStaticData> abilityData_list = __instance._buffData.list;
		for (int i = 0; i < abilityData_list.Count; i++)
		{
			BuffAbilityStaticData abilityData = abilityData_list.ToArray()[i];
			string abilityScriptname = abilityData.ability;
			if (!abilityScriptname.StartsWith("Modular/")) continue;
			if (!MainClass.fakepowerEnabled && abilityScriptname.Contains("FakePower")) continue;

			bool dictContainsKey = modbaDict.ContainsKey(ptr);
			bool existsAlready = false;
			if (dictContainsKey) {
				foreach (ModularSA existingModsa in modbaDict[ptr]) {
					if (existingModsa.originalString != abilityScriptname) continue;
					existsAlready = true;
					existingModsa.ResetValueList();
					existingModsa.ResetAdders();
					existingModsa.modsa_buffModel = __instance;
					break;
				}
			}
			if (existsAlready) continue;

			var modsa = new ModularSA();
			modsa.originalString = abilityScriptname;
			modsa.modsa_buffModel = __instance;
			modsa.abilityMode = 2; // 2 means passive
			modsa.ptr_intlong = ptr;

			modsa.SetupModular(abilityScriptname.Remove(0, 8));
			if (!dictContainsKey) modbaDict.Add(ptr, new List<ModularSA>());
			modbaDict[ptr].Add(modsa);
		}
	}


	public static void ResetAllModsa() {
		foreach (long key in modsaDict.Keys) {
			List<ModularSA> value = modsaDict[key];
			foreach (ModularSA modular in value) modular.EraseAllData();
			value.Clear();
		}
		modsaDict.Clear();

		foreach (long key in modpaDict.Keys) {
			List<ModularSA> value = modpaDict[key];
			foreach (ModularSA modular in value) modular.EraseAllData();
			value.Clear();
		}
		modpaDict.Clear();

		foreach (ModularSA modular in modca_list) { modular.EraseAllData(); }

		foreach (long key in modbaDict.Keys) {
			List<ModularSA> value = modbaDict[key];
			foreach (ModularSA modular in value) modular.EraseAllData();
			value.Clear();
		}
		modbaDict.Clear();

		modca_list.Clear();
		unitMod_list.Clear();
		skillPtrsRoundStart.Clear();
	}

	public static Dictionary<long, List<ModularSA>> modsaDict = new();
	public static Dictionary<long, List<ModularSA>> modpaDict = new();
	public static List<ModularSA> modca_list = new();
	public static Dictionary<long, List<ModularSA>> modbaDict = new();
	public static Dictionary<long, ModUnitData> unitMod_list = new();
	public static List<long> skillPtrsRoundStart = new();


	public static int GetModUnitData(long targetPtr_intlong, int dataID)
	{
		if (unitMod_list.ContainsKey(targetPtr_intlong)) {
			foreach (DataMod dataMod in unitMod_list[targetPtr_intlong].data_list) {
				if (dataMod.dataID != dataID) continue;
				return dataMod.dataValue;
			}
		}
		return 0;
	}
	public static void SetModUnitData(long targetPtr_intlong, int dataID, int dataValue)
	{
		bool found = false;
		if (unitMod_list.ContainsKey(targetPtr_intlong)) {
			foreach (DataMod dataMod in unitMod_list[targetPtr_intlong].data_list) {
				if (dataMod.dataID != dataID) continue;
				found = true;
				dataMod.dataValue = dataValue;
				break;
			}
			if (!found) {
				var dataMod = new DataMod();
				dataMod.dataID = dataID;
				dataMod.dataValue = dataValue;
				unitMod_list[targetPtr_intlong].data_list.Add(dataMod);
			}

			found = true;
		}

		if (!found) {
			var unitMod = new ModUnitData();
			unitMod.unitPtr_intlong = targetPtr_intlong;
			unitMod_list.Add(targetPtr_intlong, unitMod);

			var dataMod = new DataMod();
			dataMod.dataID = dataID;
			dataMod.dataValue = dataValue;
			unitMod.data_list.Add(dataMod);
		}
	}

	private static List<ModularSA> GetAllModpaFromPasmodel(PassiveModel passiveModel, bool checkActive = true)
	{
		if (!checkActive || passiveModel.CheckActiveCondition()) {
			long ptr_intlong = passiveModel.Pointer.ToInt64();
			if (modpaDict.ContainsKey(ptr_intlong)) return modpaDict[ptr_intlong];
		}
		return new List<ModularSA>();
	}

	private static List<ModularSA> GetAllModbaFromBuffModel(BuffModel buffModel)
	{
		long ptr_intlong = buffModel.Pointer.ToInt64();
		if (modbaDict.ContainsKey(ptr_intlong)) return modbaDict[ptr_intlong];
		return new List<ModularSA>();
	}

	// REAL PATCHES START HERE
	// REAL PATCHES START HERE
	// REAL PATCHES START HERE


	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnRoundStart_After_Event))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnRoundStart_After_Event(BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		foreach (long key in modpaDict.Keys) {
			List<ModularSA> value = modpaDict[key];
			foreach (ModularSA modular in value) modular.ResetAdders();
		}

		//Il2CppSystem.Collections.Generic.List<SupportUnitModel> supportUnitList = BattleObjectManager.Instance.GetSupportUnitModels(UNIT_FACTION.PLAYER);

		//foreach (SupportUnitModel supportUnitModel in supportUnitList) {
		//	var passiveModel = supportUnitModel.PassiveDetail;
		//	foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel)) {
		//		modpa.modsa_passiveModel = passiveModel;S
		//		modpa.Enact(__instance._owner, null, null, null, actevent, timing);
		//	}
		//}
		copypastesolution(__instance._owner, null, null, null, "RoundStart", timing, __instance);
		foreach (SinActionModel sinAction in __instance._owner.GetSinActionList())
		{
			foreach (UnitSinModel sinModel in sinAction.currentSinList)
			{
				SkillModel skillModel = sinModel.GetSkill();
				if (skillModel == null) continue;
				long skillmodel_intlong = skillModel.Pointer.ToInt64();

				if (!modsaDict.ContainsKey(skillmodel_intlong)) continue;
				foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
					//MainClass.Logg.LogInfo("Found modsa - RoundStart");
					modsa.Enact(__instance._owner, skillModel, null, null, MainClass.timingDict["RoundStart"], timing);
				}
			}
		}
	}

	[HarmonyPatch(typeof(StageController), nameof(StageController.StartRoundAfterAbnormalityChoice_Init))]
	[HarmonyPostfix]
	private static void Postfix_StageController_StartRoundAfterAbnormalityChoice_Init()
	{
		foreach (KeyValuePair<int, BattleObjectManager.BattleUnit> allUnit in SingletonBehavior<BattleObjectManager>.Instance._allUnitDictionary)
		{
			PassiveDetail __instance = allUnit.Value?.Model._passiveDetail;
			foreach (long key in modpaDict.Keys)
			{
				List<ModularSA> value = modpaDict[key];
				foreach (ModularSA modular in value) modular.ResetAdders();
			}
			copypastesolution(__instance._owner, null, null, null, "AfterSlots", BATTLE_EVENT_TIMING.ALL_TIMING, __instance);

			foreach (SinActionModel sinAction in __instance._owner.GetSinActionList())
			{
				foreach (UnitSinModel sinModel in sinAction.currentSinList)
				{
					SkillModel skillModel = sinModel.GetSkill();
					if (skillModel == null) continue;
					long skillmodel_intlong = skillModel.Pointer.ToInt64();

					if (!modsaDict.ContainsKey(skillmodel_intlong)) continue;
					foreach (ModularSA modsa in modsaDict[skillmodel_intlong])
					{
						//MainClass.Logg.LogInfo("Found modsa - RoundStart");
						modsa.Enact(__instance._owner, skillModel, null, null, MainClass.timingDict["AfterSlots"], BATTLE_EVENT_TIMING.ALL_TIMING);
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnBattleStart))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnBattleStart(BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		copypastesolution(__instance._owner, null, null, null, "StartBattle", timing, __instance);
	}


	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnBattleEnd))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnBattleEnd(BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		copypastesolution(__instance._owner, null, null, null, "EndBattle", timing, __instance);
	}


	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnStartTurn_BeforeLog))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnStartTurnBeforeLog(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		copypastesolution(__instance._owner, action.Skill, action, null, "WhenUse", timing, __instance, true);
	}


	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnStartDuel))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnStartDuel(BattleActionModel ownerAction, BattleActionModel opponentAction, PassiveDetail __instance)
	{
		copypastesolution(__instance._owner, ownerAction.Skill, ownerAction, opponentAction, "StartDuel", BATTLE_EVENT_TIMING.ON_START_DUEL, __instance);
	}
	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnWinDuel))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnWinDuel(BattleActionModel selfAction, BattleActionModel oppoAction, int parryingCount, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		copypastesolution(__instance._owner, selfAction.Skill, selfAction, oppoAction, "WinDuel", timing, __instance);
	}
	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnLoseDuel))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnLoseDuel(BattleActionModel selfAction, BattleActionModel oppoAction, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		copypastesolution(__instance._owner, selfAction.Skill, selfAction, oppoAction, "DefeatDuel", timing, __instance); ;
	}


	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.BeforeAttack))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_BeforeAttack(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		copypastesolution(__instance._owner, action.Skill, action, null, "BeforeAttack", timing, __instance);
	}


	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnEndTurn))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnEndTurn(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		copypastesolution(__instance._owner, action.Skill, action, null, "EndSkill", timing, __instance);
	}


	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnStartBehaviour))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnStartBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		copypastesolution(__instance._owner, action.Skill, action, null, "OnStartBehaviour", timing, __instance);
	}

	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnEndBehaviour))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnEndBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		copypastesolution(__instance._owner, action.Skill, action, null, "OnEndBehaviour", timing, __instance);
	}


	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnBeforeDefense))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnBeforeDefense(BattleActionModel action, PassiveDetail __instance)
	{
		int actevent = MainClass.timingDict["BeforeDefense"];
		foreach (PassiveModel passiveModel in __instance.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
				if (Input.GetKeyInt(KeyCode.LeftControl)) MainClass.Logg.LogInfo("Found modpassive - OnBeforeDefense: " + modpa.passiveID);
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(action.Model, action.Skill, action, null, actevent, BATTLE_EVENT_TIMING.NONE);
			}
		}
		foreach (PassiveModel passiveModel in __instance.EgoPassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong])
			{
				if (Input.GetKeyInt(KeyCode.LeftControl)) MainClass.Logg.LogInfo("Found modpassive - OnBeforeDefense: " + modpa.passiveID);
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(action.Model, action.Skill, action, null, actevent, BATTLE_EVENT_TIMING.NONE);
			}
		}
	}

	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnDie))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnDie(BattleUnitModel killer, BattleActionModel actionOrNull, DAMAGE_SOURCE_TYPE dmgSrcType, BUFF_UNIQUE_KEYWORD keyword, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		int actevent_OnDie = MainClass.timingDict["OnDie"];
		int actevent_OnOtherDie = MainClass.timingDict["OnOtherDie"];
		BattleUnitModel deadUnit = __instance._owner;

		foreach (PassiveModel passiveModel in __instance.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
				modpa.modsa_passiveModel = passiveModel;
				modpa.modsa_target_list.Clear();
				modpa.modsa_target_list.Add(killer);
				modpa.Enact(deadUnit, null, null, actionOrNull, actevent_OnDie, timing);
			}
		}
		foreach (PassiveModel passiveModel in __instance.EgoPassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong])
			{
				modpa.modsa_passiveModel = passiveModel;
				modpa.modsa_target_list.Clear();
				modpa.modsa_target_list.Add(killer);
				modpa.Enact(deadUnit, null, null, actionOrNull, actevent_OnDie, timing);
			}
		}

		// onotherdie
		BattleObjectManager battleObjManager_inst = SingletonBehavior<BattleObjectManager>.Instance;
		foreach (BattleUnitModel unit in battleObjManager_inst.GetAliveListExceptSelf(deadUnit, false, false))
		{
			foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList) {
				foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel)) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.modsa_target_list.Clear();
					modpa.modsa_target_list.Add(deadUnit);
					modpa.Enact(unit, null, null, actionOrNull, actevent_OnOtherDie, timing);
				}
			}

			foreach (PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList) {
				foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false)) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.modsa_target_list.Clear();
					modpa.modsa_target_list.Add(deadUnit);
					modpa.Enact(unit, null, null, actionOrNull, actevent_OnOtherDie, timing);
				}
			}
		}
	}

	[HarmonyPatch(typeof(BattleUnitModel_Abnormality), nameof(BattleUnitModel_Abnormality.GetActionSlotAdder))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_Abnormality_GetActionSlotAdder(ref int __result, BattleUnitModel_Abnormality __instance)
	{
		foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
			foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) __result += modpa.slotAdder;
		}
		foreach (PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
			foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) __result += modpa.slotAdder;
		}
	}

	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnBreak))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnBreak(BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		int actevent_OnBreak = MainClass.timingDict["OnBreak"];
		int actevent_OnOtherBreak = MainClass.timingDict["OnOtherBreak"];
		BattleUnitModel brokeUnit = __instance._owner;

		foreach (PassiveModel passiveModel in __instance.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(brokeUnit, null, null, null, actevent_OnBreak, timing);
			}
		}
		foreach (PassiveModel passiveModel in __instance.EgoPassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong])
			{
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(brokeUnit, null, null, null, actevent_OnBreak, timing);
			}
		}

		// onotherbreak
		BattleObjectManager battleObjManager_inst = SingletonBehavior<BattleObjectManager>.Instance;
		foreach (BattleUnitModel unit in battleObjManager_inst.GetAliveListExceptSelf(brokeUnit, false, false))
		{
			foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.modsa_target_list.Clear();
					modpa.modsa_target_list.Add(brokeUnit);
					modpa.Enact(unit, null, null, null, actevent_OnOtherBreak, timing);
				}
			}
			foreach (PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

				foreach (ModularSA modpa in modpaDict[passiveModel_intlong])
				{
					modpa.modsa_passiveModel = passiveModel;
					modpa.modsa_target_list.Clear();
					modpa.modsa_target_list.Add(brokeUnit);
					modpa.Enact(unit, null, null, null, actevent_OnOtherBreak, timing);
				}
			}
		}
	}


	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnVibrationExplosionOtherUnit))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnVibrationExplosionOtherUnit(BattleUnitModel explodedUnit, BattleUnitModel giverOrNull, BattleActionModel actionOrNull, ABILITY_SOURCE_TYPE abilitySrc, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		int actevent = MainClass.timingDict["OnOtherBurst"];
		foreach (PassiveModel passiveModel in __instance.PassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong])
			{
				modpa.modsa_passiveModel = passiveModel;
				modpa.modsa_target_list.Clear();
				modpa.modsa_target_list.Add(explodedUnit);
				modpa.Enact(__instance._owner, null, null, null, actevent, timing);
			}
		}
		foreach (PassiveModel passiveModel in __instance.EgoPassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong])
			{
				modpa.modsa_passiveModel = passiveModel;
				modpa.modsa_target_list.Clear();
				modpa.modsa_target_list.Add(explodedUnit);
				modpa.Enact(__instance._owner, null, null, null, actevent, timing);
			}
		}
	}

	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnDiscardSin))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnDiscardSin(UnitSinModel sin, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	{
		copypastesolution(__instance._owner, sin.GetSkill(), sin._currentAction, null, "OnDiscard", timing, __instance);
	}

	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.CheckImmortal))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_CheckImmortal(BATTLE_EVENT_TIMING timing, int newHp, bool isInstantDeath, ref bool __result, BattleUnitModel __instance)
	{
		if (isInstantDeath) return;
		int actevent = MainClass.timingDict["Immortal"];
		foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel)) {
				modpa.immortality = false;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance, null, null, null, actevent, timing);
				if (modpa.immortality) __result = true;
			}
		}
		foreach (PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false)) {
				modpa.immortality = false;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance, null, null, null, actevent, timing);
				if (modpa.immortality) __result = true;
			}
		}
	}
	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.CheckImmortalOtherUnit), new Type[] { typeof(BattleUnitModel), typeof(int), typeof(bool), typeof(BUFF_UNIQUE_KEYWORD) })]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_CheckImmortalOtherUnit(BattleUnitModel checkTarget, int newHp, bool isInstantDeath, ref bool __result, BattleUnitModel __instance)
	{
		if (isInstantDeath) return;
		int actevent = MainClass.timingDict["ImmortalOther"];
		foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel)) {
				modpa.immortality = false;
				modpa.modsa_passiveModel = passiveModel;
				modpa.modsa_target_list.Clear();
				modpa.modsa_target_list.Add(checkTarget);
				modpa.Enact(__instance, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
				if (modpa.immortality) __result = true;
			}
		}
		foreach (PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false)) {
				modpa.immortality = false;
				modpa.modsa_passiveModel = passiveModel;
				modpa.modsa_target_list.Clear();
				modpa.modsa_target_list.Add(checkTarget);
				modpa.Enact(__instance, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
				if (modpa.immortality) __result = true;
			}
		}
	}


	// PASSIVES END
	// PASSIVES END
	// PASSIVES END


	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetCoinScaleAdder))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetCoinScaleAdder(BattleActionModel action, ref int __result, SkillModel __instance)
	{
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (modsaDict.ContainsKey(skillmodel_intlong)) {
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				if (modsa.activationTiming == actevent_FakePower) continue;
				int power = modsa.coinScaleAdder;
				__result += power;
			}
		}

		foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel)) {
				if (modpa.activationTiming == actevent_FakePower) continue;
				int power = modpa.coinScaleAdder;
				if (power != 0) __result += power;
			}
		}

		foreach (PassiveModel passiveModel in action.Model._passiveDetail.EgoPassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false)) {
				if (modpa.activationTiming == actevent_FakePower) continue;
				int power = modpa.coinScaleAdder;
				if (power != 0) __result += power;
			}
		}
	}
	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetSkillPowerAdder))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetSkillPowerAdder(BattleActionModel action, ref int __result, SkillModel __instance) {
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (modsaDict.ContainsKey(skillmodel_intlong)) {
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				if (modsa.activationTiming == actevent_FakePower) continue;
				int power = modsa.skillPowerAdder;
				if (power != 0) MainClass.Logg.LogInfo("Found modsa - base power adder: " + power);
				__result += power;
			}
		}

		foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel)) {
				if (modpa.activationTiming == actevent_FakePower) continue;
				int power = modpa.skillPowerAdder;
				if (power != 0) MainClass.Logg.LogInfo("Found modpa - base power adder: ");
				__result += power;
			}
		}

		foreach (PassiveModel passiveModel in action.Model._passiveDetail.EgoPassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false)) {
				if (modpa.activationTiming == actevent_FakePower) continue;
				int power = modpa.skillPowerAdder;
				if (power != 0) MainClass.Logg.LogInfo("Found modpa - base power adder: ");
				__result += power;
			}
		}
	}
	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetSkillPowerResultAdder))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetSkillPowerResultAdder(BattleActionModel action, BATTLE_EVENT_TIMING timing, CoinModel coinOrNull, ref int __result, SkillModel __instance)
	{
		int actevent_FakePower = MainClass.timingDict["FakePower"];

		if (coinOrNull != null)
		{
			long coinmodel_intlong = coinOrNull.Pointer.ToInt64();
			foreach (ModularSA modca in modca_list)
			{
				if (modca.activationTiming == actevent_FakePower) continue;
				if (coinmodel_intlong != modca.ptr_intlong) continue;
				int power = modca.skillPowerResultAdder;
				if (power != 0) MainClass.Logg.LogInfo("Found modca - final power adder: " + power);
				if (power != 0) __result += power;
			}
		}

		long skillmodel_intlong = __instance.Pointer.ToInt64();

		if (modsaDict.ContainsKey(skillmodel_intlong)) {
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				if (modsa.activationTiming == actevent_FakePower) continue;
				int power = modsa.skillPowerResultAdder;
				if (power != 0) MainClass.Logg.LogInfo("Found modsa - final power adder: " + power);
				__result += power;
			}
		}

		foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel)) {
				if (modpa.activationTiming == actevent_FakePower) continue;
				int power = modpa.skillPowerResultAdder;
				if (power != 0) MainClass.Logg.LogInfo("Found modpa - final power adder: ");
				__result += power;
			}
		}

		foreach (PassiveModel passiveModel in action.Model._passiveDetail.EgoPassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false)) {
				if (modpa.activationTiming == actevent_FakePower) continue;
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
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (modsaDict.ContainsKey(skillmodel_intlong)) {
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				if (modsa.activationTiming == actevent_FakePower) continue;
				int power = modsa.parryingResultAdder;
				if (power != 0) MainClass.Logg.LogInfo("Found modsa - clash power adder: " + power);
				__result += power;
			}
		}

		foreach (PassiveModel passiveModel in actorAction.Model._passiveDetail.PassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel)) {
				if (modpa.activationTiming == actevent_FakePower) continue;
				int power = modpa.parryingResultAdder;
				if (power != 0) MainClass.Logg.LogInfo("Found modpa - clash power adder: " + power);
				__result += power;
			}
		}

		foreach (PassiveModel passiveModel in actorAction.Model._passiveDetail.EgoPassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false)) {
				if (modpa.activationTiming == actevent_FakePower) continue;
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
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		long coinmodel_intlong = coin.Pointer.ToInt64();
		foreach (ModularSA modca in modca_list) {
			if (modca.activationTiming == actevent_FakePower) continue;
			if (coinmodel_intlong != modca.ptr_intlong) continue;
			int power = modca.atkDmgAdder;
			if (power != 0)
			{
				__result += power;
			}
		}

		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (modsaDict.ContainsKey(skillmodel_intlong)) {
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				if (modsa.activationTiming == actevent_FakePower) continue;
				int power = modsa.atkDmgAdder;
				if (power != 0) __result += power;
			}
		}

		foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel)) {
				if (modpa.activationTiming == actevent_FakePower) continue;
				int power = modpa.atkDmgAdder;
				if (power != 0) __result += power;
			}
		}

		foreach (PassiveModel passiveModel in action.Model._passiveDetail.EgoPassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false)) {
				if (modpa.activationTiming == actevent_FakePower) continue;
				int power = modpa.atkDmgAdder;
				if (power != 0) __result += power;
			}
		}
	}
	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetAttackDmgMultiplier))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetAttackDmgMultiplier(BattleActionModel action, CoinModel coin, ref float __result, SkillModel __instance)
	{
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		long coinmodel_intlong = coin.Pointer.ToInt64();
		foreach (ModularSA modca in modca_list)
		{
			if (modca.activationTiming == actevent_FakePower) continue;
			if (coinmodel_intlong != modca.ptr_intlong) continue;
			int power = modca.atkMultAdder;
			if (power != 0) __result += (float)power * 0.01f;
		}

		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (modsaDict.ContainsKey(skillmodel_intlong)) {
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				if (modsa.activationTiming == actevent_FakePower) continue;
				int power = modsa.atkMultAdder;
				if (power != 0) __result += power * 0.01f;
			}
		}

		foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel)) {
				if (modpa.activationTiming == actevent_FakePower) continue;
				int power = modpa.atkMultAdder;
				if (power != 0) __result += power * 0.01f;
			}
		}

		foreach (PassiveModel passiveModel in action.Model._passiveDetail.EgoPassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false)) {
				if (modpa.activationTiming == actevent_FakePower) continue;
				int power = modpa.atkMultAdder;
				if (power != 0) __result += power * 0.01f;
			}
		}
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnBattleStart))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnBattleStart(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance)
	{
		BattleUnitModel unit = action.Model;
		int actevent = MainClass.timingDict["StartBattle"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (modsaDict.ContainsKey(skillmodel_intlong))
		{
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong])
			{
				modsa.Enact(unit, __instance, action, null, actevent, timing);
			}
		}

		actevent = MainClass.timingDict["SBS"];

		foreach (BuffModel buf in action.Model.GetActivatedBuffModels())
		{
			foreach (ModularSA modba in GetAllModbaFromBuffModel(buf))
			{
				modba.modsa_buffModel = buf;
				modba.Enact(unit, __instance, action, null, actevent, timing);
			}
		}

		foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList)
		{
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel))
			{
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(unit, __instance, action, null, actevent, timing);
			}
		}
		foreach (PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList)
		{
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false))
			{
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(unit, __instance, action, null, actevent, timing);
			}
		}
	}

	//[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.GetAttackWeightAdder))]
	//[HarmonyPostfix]
	//private static void Postfix_BattleUnitModel_GetAttackWeightAdder(BattleActionModel action, int __result)
	//{
	//	__result += 5;
	//}

	//[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetAttackWeight))]
	//[HarmonyPostfix]
	//private static void Postfix_SkillModel_GetAttackWeight(BattleActionModel action, int __result, SkillModel __instance)
	//{
	//	__result += 5;
	//}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnBeforeTurn))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnBeforeTurn(BattleActionModel action, SkillModel __instance)
	{
		int actevent = MainClass.timingDict["BeforeUse"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.Enact(action.Model, __instance, action, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
		}
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnStartTurn_BeforeLog))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnStartTurnBeforeLog(BattleActionModel action, List<BattleUnitModel> targets, BATTLE_EVENT_TIMING timing, SkillModel __instance)
	{
		int actevent = MainClass.timingDict["WhenUse"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			if (modsa.resetWhenUse) modsa.ResetAdders(); // Reset Adders if for some reason this skill is used again
			modsa.Enact(action.Model, __instance, action, null, actevent, timing); // normal code
		}
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.BeforeAttack))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_BeforeAttack(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance)
	{
		int actevent = MainClass.timingDict["BeforeAttack"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.Enact(action.Model, __instance, action, null, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnBeforeParryingOnce))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnBeforeParryingOnce(BattleActionModel ownerAction, BattleActionModel oppoAction, SkillModel __instance)
	{
		int actevent = MainClass.timingDict["DuelClash"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (modsaDict.ContainsKey(skillmodel_intlong)) {
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(ownerAction.Model, __instance, ownerAction, oppoAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}

		foreach (PassiveModel passiveModel in ownerAction.Model._passiveDetail.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(ownerAction.Model, __instance, ownerAction, oppoAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		foreach (PassiveModel passiveModel in ownerAction.Model._passiveDetail.EgoPassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong])
			{
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(ownerAction.Model, __instance, ownerAction, oppoAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnBeforeParryingOnce_AfterLog))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnBeforeParryingOnce_AfterLog(BattleActionModel ownerAction, BattleActionModel oppoAction, SkillModel __instance)
	{
		int actevent = MainClass.timingDict["DuelClashAfter"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.Enact(ownerAction.Model, __instance, ownerAction, oppoAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
		}
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnStartDuel))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnStartDuel(BattleActionModel selfAction, BattleActionModel oppoAction, BATTLE_EVENT_TIMING timing, SkillModel __instance)
	{
		int actevent = MainClass.timingDict["StartDuel"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.Enact(selfAction.Model, __instance, selfAction, oppoAction, actevent, timing);
		}
	}
	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnWinDuel))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnWinDuel(BattleActionModel selfAction, BattleActionModel oppoAction, BATTLE_EVENT_TIMING timing, int parryingCount, SkillModel __instance)
	{
		int actevent = MainClass.timingDict["WinDuel"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.Enact(selfAction.Model, __instance, selfAction, oppoAction, actevent, timing);
		}
	}
	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnLoseDuel))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnLoseDuel(BattleActionModel selfAction, BattleActionModel oppoAction, BATTLE_EVENT_TIMING timing, SkillModel __instance)
	{
		int actevent = MainClass.timingDict["DefeatDuel"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.Enact(selfAction.Model, __instance, selfAction, oppoAction, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnRoundEnd))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnRoundEnd(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
		int actevent = MainClass.timingDict["EndBattle"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.Enact(action.Model, __instance, action, null, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnEndTurn))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnEndTurn(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
		int actevent = MainClass.timingDict["EndSkill"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.Enact(action.Model, __instance, action, null, actevent, timing);
		}
	}


	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnStartBehaviour))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnStartBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
		int actevent = MainClass.timingDict["OnStartBehaviour"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.Enact(action.Model, __instance, action, null, actevent, timing);
		}
	}
	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.BeforeBehaviour))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_BeforeBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
		int actevent = MainClass.timingDict["BeforeBehaviour"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.Enact(action.Model, __instance, action, null, actevent, timing);
		}
	}
	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnEndBehaviour))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnEndBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
		int actevent = MainClass.timingDict["OnEndBehaviour"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.Enact(action.Model, __instance, action, null, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnDiscarded))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnDiscarded(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
		int actevent = MainClass.timingDict["OnDiscard"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) modsa.Enact(action.Model, __instance, action, null, actevent, timing);
	}

	// SKILLMODEL UP TO HERE
	// SKILLMODEL UP TO HERE
	// SKILLMODEL UP TO HERE



	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnRoundStart_After_Event))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_OnRoundStart_After_Event(BattleUnitModel unit, BATTLE_EVENT_TIMING timing, BuffModel __instance)
	{
		int actevent = MainClass.timingDict["RoundStart"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance)) {
			modba.modsa_buffModel = __instance;
			modba.Enact(unit, null, null, null, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnBattleStart))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_OnBattleStart(BattleUnitModel unit, BATTLE_EVENT_TIMING timing, BuffModel __instance)
	{
		int actevent = MainClass.timingDict["StartBattle"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance))
		{
			modba.modsa_buffModel = __instance;
			modba.Enact(unit, null, null, null, actevent, timing);
		}
	}


	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnRoundEnd))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_OnRoundEnd(BattleUnitModel unit, BATTLE_EVENT_TIMING timing, BuffModel __instance) {
		int actevent = MainClass.timingDict["EndBattle"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance)) {
			modba.modsa_buffModel = __instance;
			modba.Enact(unit, null, null, null, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnStartTurn_BeforeLog))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_OnStartTurnBeforeLog(BattleUnitModel unit, BattleActionModel action, BATTLE_EVENT_TIMING timing, BuffModel __instance) {
		int actevent = MainClass.timingDict["WhenUse"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance)) {
			modba.modsa_buffModel = __instance;
			modba.Enact(unit, action.Skill, action, null, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnEndTurn))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_OnEndTurn(BattleActionModel action, BATTLE_EVENT_TIMING timing, BuffModel __instance)
	{
		int actevent = MainClass.timingDict["EndSkill"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance))
		{
			modba.modsa_buffModel = __instance;
			modba.Enact(action.Model, action.Skill, action, null, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnStartDuel))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_OnStartDuel(BattleActionModel ownerAction, BattleActionModel opponentAction, BATTLE_EVENT_TIMING timing, BuffModel __instance)
	{
		int actevent = MainClass.timingDict["StartDuel"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance)) {
			modba.modsa_buffModel = __instance;
			modba.Enact(ownerAction.Model, ownerAction.Skill, ownerAction, opponentAction, actevent, timing);
		}
	}
	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnWinDuel))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_OnWinDuel(BattleActionModel ownerAction, BattleActionModel opponentAction, int parryingCount, BATTLE_EVENT_TIMING timing, BuffModel __instance)
	{
		int actevent = MainClass.timingDict["WinDuel"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance)) {
			modba.modsa_buffModel = __instance;
			modba.Enact(ownerAction.Model, ownerAction.Skill, ownerAction, opponentAction, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnLoseDuel))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_OnLoseDuel(BattleActionModel ownerAction, BattleActionModel opponentAction, BATTLE_EVENT_TIMING timing, BuffModel __instance)
	{
		int actevent = MainClass.timingDict["DefeatDuel"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance)) {
			modba.modsa_buffModel = __instance;
			modba.Enact(ownerAction.Model, ownerAction.Skill, ownerAction, opponentAction, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnStartBehaviour))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_OnStartBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, BuffModel __instance)
	{
		int actevent = MainClass.timingDict["OnStartBehaviour"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance)) {
			modba.modsa_buffModel = __instance;
			modba.Enact(action.Model, action.Skill, action, null, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnEndBehaviour))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_OnEndBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, BuffModel __instance)
	{
		int actevent = MainClass.timingDict["OnEndBehaviour"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance))
		{
			modba.modsa_buffModel = __instance;
			modba.Enact(action.Model, action.Skill, action, null, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnDiscardSin))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_OnDiscardSint(BattleUnitModel unit, UnitSinModel sin, BATTLE_EVENT_TIMING timing, BuffModel __instance)
	{
		int actevent = MainClass.timingDict["OnDiscard"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance))
		{
			modba.modsa_buffModel = __instance;
			modba.Enact(unit, sin.GetSkill(), null, null, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnBeforeParryingOnce_AfterLog))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_OnBeforeParryingOnce_AfterLog(BattleActionModel ownerAction, BattleActionModel opponentAction, BuffModel __instance)
	{
		int actevent = MainClass.timingDict["DuelClash"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance))
		{
			modba.modsa_buffModel = __instance;
			modba.Enact(ownerAction.Model, null, ownerAction, opponentAction, actevent, BATTLE_EVENT_TIMING.ON_START_DUEL);
		}
	}

	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnVibrationExplosion))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_OnVibrationExplosion(BattleUnitModel unit, BattleUnitModel giverOrNull, BattleActionModel actionOrNull, ABILITY_SOURCE_TYPE abilitySrc, BATTLE_EVENT_TIMING timing, BuffModel __instance)
	{
		int actevent = MainClass.timingDict["OnBurst"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance))
		{
			modba.modsa_target_list.Clear();
			modba.modsa_target_list.Add(giverOrNull);
			modba.modsa_buffModel = __instance;
			modba.Enact(unit, actionOrNull.Skill, actionOrNull, null, actevent, timing);
		}
	}

	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetSkillPowerAdder))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_GetSkillPowerAdder(ref int __result, BuffModel __instance) {
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance)) {
			if (modba.activationTiming == actevent_FakePower) continue;
			int power = modba.skillPowerAdder;
			if (power != 0) MainClass.Logg.LogInfo("Found modba - base power adder: " + power);
			__result += power;
		}
	}
	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetSkillPowerResultAdder))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_GetSkillPowerResultAdder(ref int __result, BuffModel __instance) {
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance)) {
			if (modba.activationTiming == actevent_FakePower) continue;
			int power = modba.skillPowerResultAdder;
			if (power != 0) MainClass.Logg.LogInfo("Found modba - final power adder: " + power);
			__result += power;
		}
	}
	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetParryingResultAdder))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_GetParryingResultAdder(ref int __result, BuffModel __instance) {
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance)) {
			if (modba.activationTiming == actevent_FakePower) continue;
			int power = modba.parryingResultAdder;
			if (power != 0) MainClass.Logg.LogInfo("Found modba - clash power adder: " + power);
			__result += power;
		}
	}
	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetCoinScaleAdder))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_GetCoinScaleAdder(ref int __result, BuffModel __instance) {
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance)) {
			if (modba.activationTiming == actevent_FakePower) continue;
			int power = modba.coinScaleAdder;
			if (power != 0) MainClass.Logg.LogInfo("Found modba - coin power adder: " + power);
			__result += power;
		}
	}
	[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.RightAfterGettingBuff))]
	[HarmonyPostfix]
	private static void Postfix_BuffModel_RightAfterGettingBuff(BattleUnitModel unit, int gettingNewStack, ABILITY_SOURCE_TYPE abilitySrcType, BATTLE_EVENT_TIMING timing, BuffModel __instance)
	{
		int actevent = MainClass.timingDict["WhenGained"];
		foreach (ModularSA modba in GetAllModbaFromBuffModel(__instance))
		{
			modba.modsa_buffModel = __instance;
			modba.Enact(unit, null, null, null, actevent, timing);
		}
	}



	// BUFFMODEL UP TO HERE
	// BUFFMODEL UP TO HERE
	// BUFFMODEL UP TO HERE

	/*[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnGiveHpDamage))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_OnGiveHpDamage(BattleUnitModel target, BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
	{
		MainClass.Logg.LogInfo(" OnGiveHpDamage ");
	}*/

	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnTakeAttackDamage))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_OnTakeAttackDamage(BattleActionModel action, CoinModel coin, int realDmg, int hpDamage, BATTLE_EVENT_TIMING timing, bool isCritical, BattleUnitModel __instance)
	{
		//MainClass.Logg.LogInfo(" OnTakeAttackDamage ");
		int actevent_OSA = MainClass.timingDict["OSA"];
		int actevent_WH = MainClass.timingDict["WH"];
		BattleUnitModel attacker = action.Model;
		if (__instance.TryCast<BattleUnitModel_Abnormality>() == null)
		{
			long coinmodel_intlong = coin.Pointer.ToInt64();
			foreach (ModularSA modca in modca_list)
			{
				if (coinmodel_intlong != modca.ptr_intlong) continue;
				modca.lastFinalDmg = realDmg;
				modca.lastHpDmg = hpDamage;
				modca.wasCrit = isCritical;
				//modca.wasClash = isWinDuel.HasValue;
				//if (modca.wasClash) modca.wasWin = isWinDuel.Value;
				modca.modsa_coinModel = coin;
				modca.modsa_target_list.Clear();
				modca.modsa_target_list.Add(__instance);
				modca.Enact(attacker, action.Skill, action, null, actevent_OSA, timing);
			}

			long skillmodel_intlong = action.Skill.Pointer.ToInt64();
			if (modsaDict.ContainsKey(skillmodel_intlong))
			{
				foreach (ModularSA modsa in modsaDict[skillmodel_intlong])
				{
					modsa.lastFinalDmg = realDmg;
					modsa.lastHpDmg = hpDamage;
					modsa.wasCrit = isCritical;
					modsa.modsa_coinModel = coin;
					modsa.modsa_target_list.Clear();
					modsa.modsa_target_list.Add(__instance);
					modsa.Enact(attacker, action.Skill, action, null, actevent_OSA, timing);
				}
			}

			foreach (BuffModel buffModel in attacker._buffDetail.GetActivatedBuffModelAll()) {
				foreach (ModularSA modba in GetAllModbaFromBuffModel(buffModel)) {
					modba.lastFinalDmg = realDmg;
					modba.lastHpDmg = hpDamage;
					modba.wasCrit = isCritical;
					modba.modsa_coinModel = coin;
					modba.modsa_buffModel = buffModel;
					modba.modsa_target_list.Clear();
					modba.modsa_target_list.Add(__instance);
					modba.Enact(attacker, action.Skill, action, null, actevent_OSA, timing);
				}
			}

			foreach (PassiveModel passiveModel in attacker._passiveDetail.PassiveList) {
				foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel)) {
					modpa.lastFinalDmg = realDmg;
					modpa.lastHpDmg = hpDamage;
					modpa.wasCrit = isCritical;
					modpa.modsa_coinModel = coin;
					modpa.modsa_passiveModel = passiveModel;
					modpa.modsa_target_list.Clear();
					modpa.modsa_target_list.Add(__instance);
					modpa.Enact(attacker, action.Skill, action, null, actevent_OSA, timing);
				}
			}
			foreach (PassiveModel passiveModel in attacker._passiveDetail.EgoPassiveList) {
				foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false)) {
					modpa.lastFinalDmg = realDmg;
					modpa.lastHpDmg = hpDamage;
					modpa.wasCrit = isCritical;
					modpa.modsa_coinModel = coin;
					modpa.modsa_passiveModel = passiveModel;
					modpa.modsa_target_list.Clear();
					modpa.modsa_target_list.Add(__instance);
					modpa.Enact(attacker, action.Skill, action, null, actevent_OSA, timing);
				}
			}
		}

		foreach (BuffModel buffModel in __instance._buffDetail.GetActivatedBuffModelAll()) {
			foreach (ModularSA modba in GetAllModbaFromBuffModel(buffModel)) {
				modba.lastFinalDmg = realDmg;
				modba.lastHpDmg = hpDamage;
				modba.wasCrit = isCritical;
				modba.modsa_coinModel = coin;
				modba.modsa_buffModel = buffModel;
				modba.modsa_target_list.Clear();
				modba.modsa_target_list.Add(__instance);
				modba.Enact(attacker, action.Skill, action, null, actevent_WH, timing);
			}
		}

		foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel)) {
				modpa.lastFinalDmg = realDmg;
				modpa.lastHpDmg = hpDamage;
				modpa.wasCrit = isCritical;
				modpa.modsa_coinModel = coin;
				modpa.modsa_passiveModel = passiveModel;
				modpa.modsa_target_list.Clear();
				modpa.modsa_target_list.Add(__instance);
				modpa.Enact(attacker, action.Skill, action, null, actevent_WH, timing);
			}
		}

		foreach (PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList) {
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false)) {
				modpa.lastFinalDmg = realDmg;
				modpa.lastHpDmg = hpDamage;
				modpa.wasCrit = isCritical;
				modpa.modsa_coinModel = coin;
				modpa.modsa_passiveModel = passiveModel;
				modpa.modsa_target_list.Clear();
				modpa.modsa_target_list.Add(__instance);
				modpa.Enact(attacker, action.Skill, action, null, actevent_WH, timing);
			}
		}


	}


	[HarmonyPatch(typeof(BattleActionModel), nameof(BattleActionModel.OnAttackConfirmed))]
	[HarmonyPostfix]
	private static void Postfix_BattleActionModel_OnAttackConfirmed(CoinModel coin, BattleUnitModel target, BATTLE_EVENT_TIMING timing, bool isCritical, BattleActionModel __instance)
	{
		int actevent_BSA = MainClass.timingDict["BSA"];
		int actevent_BWH = MainClass.timingDict["BWH"];
		long coinmodel_intlong = coin.Pointer.ToInt64();
		foreach (ModularSA modca in modca_list)
		{
			if (coinmodel_intlong != modca.ptr_intlong) continue;
			modca.wasCrit = isCritical;
			//modca.wasClash = isWinDuel.HasValue;
			//if (modca.wasClash) modca.wasWin = isWinDuel.Value;
			modca.modsa_coinModel = coin;
			modca.Enact(__instance.Model, __instance.Skill, __instance, null, actevent_BSA, timing);
		}

		long skillmodel_intlong = __instance.Skill.Pointer.ToInt64();
		if (modsaDict.ContainsKey(skillmodel_intlong)) {
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.wasCrit = isCritical;
				modsa.modsa_coinModel = coin;
				modsa.Enact(__instance.Model, __instance.Skill, __instance, null, actevent_BSA, timing);
			}
		}

		foreach (PassiveModel passiveModel in __instance.Model._passiveDetail.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
				modpa.wasCrit = isCritical;
				modpa.modsa_coinModel = coin;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance.Model, __instance.Skill, __instance, null, actevent_BSA, timing);
			}
		}

		foreach (PassiveModel passiveModel in target._passiveDetail.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
				modpa.wasCrit = isCritical;
				modpa.modsa_coinModel = coin;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance.Model, __instance.Skill, __instance, null, actevent_BWH, timing);
			}
		}

		foreach (PassiveModel passiveModel in __instance.Model._passiveDetail.EgoPassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong])
			{
				modpa.wasCrit = isCritical;
				modpa.modsa_coinModel = coin;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance.Model, __instance.Skill, __instance, null, actevent_BSA, timing);
			}
		}

		foreach (PassiveModel passiveModel in target._passiveDetail.EgoPassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong])
			{
				modpa.wasCrit = isCritical;
				modpa.modsa_coinModel = coin;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance.Model, __instance.Skill, __instance, null, actevent_BWH, timing);
			}
		}
	}

	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnSucceedEvade))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_OnSucceedEvade(BattleActionModel evadeAction, BattleActionModel attackAction, BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
	{
		SkillModel skill = evadeAction.Skill;
		long skillmodel_intlong = skill.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		int actevent = MainClass.timingDict["OnSucceedEvade"];
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.modsa_target_list.Clear();
			modsa.modsa_target_list.Add(attackAction.Model);
			modsa.Enact(__instance, skill, evadeAction, attackAction, actevent, timing);
		}
	}
	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnFailedEvade))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_OnFailedEvade(BattleActionModel evadeAction, BattleActionModel attackAction, BATTLE_EVENT_TIMING timing, BattleUnitModel __instance) {
		SkillModel skill = evadeAction.Skill;
		long skillmodel_intlong = skill.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		int actevent = MainClass.timingDict["OnDefeatEvade"];
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.modsa_target_list.Clear();
			modsa.modsa_target_list.Add(attackAction.Model);
			modsa.Enact(__instance, skill, evadeAction, attackAction, actevent, timing);
		}
	}


	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnKillTarget))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_OnKillTarget(BattleActionModel actionOrNull, BattleUnitModel target, DAMAGE_SOURCE_TYPE dmgSrcType, BATTLE_EVENT_TIMING timing, BattleUnitModel killer, BattleUnitModel __instance)
	{
		if (actionOrNull == null || actionOrNull.Skill == null) return;
		int actevent = MainClass.timingDict["EnemyKill"];
		SkillModel skill = actionOrNull.Skill;
		long skillmodel_intlong = skill.Pointer.ToInt64();
		if (modsaDict.ContainsKey(skillmodel_intlong)) {
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(__instance, skill, actionOrNull, null, actevent, timing);
			}
		}

		foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance, skill, actionOrNull, null, actevent, timing);
			}
		}
		foreach (PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong])
			{
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance, skill, actionOrNull, null, actevent, timing);
			}
		}
	}

	/*
	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnZeroHp))]
	[HarmonyPrefix]
	private static void Prefix_BattleUnitModel_OnZeroHp(BattleUnitModel __instance)
	{
		int actevent = MainClass.timingDict["OnZeroHP"];
		foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
			foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance, null, null, null, 25, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
	}
	*/

	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnEndEnemyAttack))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_OnEndEnemyAttack(BattleActionModel action, BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
	{
		int actevent = MainClass.timingDict["EnemyEndSkill"];
		foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
				modpa.modsa_passiveModel = passiveModel;
				modpa.modsa_target_list.Clear();
				modpa.modsa_target_list.Add(__instance);
				modpa.Enact(action.Model, action.Skill, action, null, actevent, timing);
			}
		}
		foreach (PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in modpaDict[passiveModel_intlong])
			{
				modpa.modsa_passiveModel = passiveModel;
				modpa.modsa_target_list.Clear();
				modpa.modsa_target_list.Add(__instance);
				modpa.Enact(action.Model, action.Skill, action, null, actevent, timing);
			}
		}
	}


	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnStartCoin))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_OnStartCoin(BattleActionModel action, CoinModel coin, BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
	{
		int actevent = MainClass.timingDict["OnCoinToss"];

		long coinmodel_intlong = coin.Pointer.ToInt64();
		foreach (ModularSA modca in modca_list)
		{
			if (coinmodel_intlong != modca.ptr_intlong) continue;
			modca.modsa_coinModel = coin;
			modca.Enact(__instance, action.Skill, action, null, actevent, timing);
		}

		long skillmodel_intlong = action.Skill.Pointer.ToInt64();
		if (modsaDict.ContainsKey(skillmodel_intlong))
		{
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong])
			{
				modsa.modsa_coinModel = coin;
				modsa.Enact(__instance, action.Skill, action, null, actevent, timing);
			}
		}

		foreach (BuffModel buffModel in __instance._buffDetail.GetActivatedBuffModelAll())
		{
			foreach (ModularSA modba in GetAllModbaFromBuffModel(buffModel))
			{
				modba.modsa_coinModel = coin;
				modba.modsa_buffModel = buffModel;
				modba.Enact(__instance, action.Skill, action, null, actevent, timing);
			}
		}

		foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
		{
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel))
			{
				modpa.modsa_coinModel = coin;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance, action.Skill, action, null, actevent, timing);
			}
		}
		foreach (PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList)
		{
			foreach (ModularSA modpa in GetAllModpaFromPasmodel(passiveModel, false))
			{
				modpa.modsa_coinModel = coin;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance, action.Skill, action, null, actevent, timing);
			}
		}
	}
	// end

	[HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.OpenSkillInfoUI))]
	[HarmonyPrefix]
	private static void OpenSkillInfoUI(BattleUnitView __instance, int skillID)
	{
		var skillData = Singleton<StaticDataManager>.Instance._skillList.GetData(skillID);
		var model = __instance._unitModel.UnitDataModel;
		MainClass.Logg.LogInfo($"Coin toss, skill = {skillID}, model level = {model.Level}, model sync level = {model.SyncLevel}");
		
		var skillModel = new SkillModel(skillData, model.Level, model.SyncLevel);
		skillModel.Init(); // needed to get noticed by modular skill timing?
		long skillmodel_intlong = skillModel.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.Enact(__instance._unitModel, skillModel, null, null, MainClass.timingDict["StartVisualCoinToss"], BATTLE_EVENT_TIMING.ALL_TIMING);
		}
	}

	[HarmonyPatch(typeof(BattleUnitView),  nameof(BattleUnitView.StartBehaviourAction))]
	[HarmonyPrefix]
	private static void StartBehaviourAction(BattleUnitView __instance)
	{
		var skillID = __instance.GetCurrentSkillViewer().curSkillID;
		var skillData = Singleton<StaticDataManager>.Instance._skillList.GetData(skillID);
		var model = __instance._unitModel.UnitDataModel;
		MainClass.Logg.LogInfo($"SBA, skill = {skillID}, model level = {model.Level}, model sync level = {model.SyncLevel}");
		
		var skillModel = new SkillModel(skillData, model.Level, model.SyncLevel);
		skillModel.Init(); // needed to get noticed by modular skill timing?
		long skillmodel_intlong = skillModel.Pointer.ToInt64();
		if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
		foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
			modsa.Enact(__instance._unitModel, skillModel, null, null, MainClass.timingDict["StartVisualSkillUse"], BATTLE_EVENT_TIMING.ALL_TIMING);
		}
	}

}
