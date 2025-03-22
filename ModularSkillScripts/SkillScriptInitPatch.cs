using System;
using BepInEx.IL2CPP.UnityEngine;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts
{
	public class SkillScriptInitPatch
	{
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

			modca_list.Clear();
			unitMod_list.Clear();
			skillPtrsRoundStart.Clear();
		}

		public static Dictionary<long, List<ModularSA>> modsaDict = new();
		public static Dictionary<long, List<ModularSA>> modpaDict = new();
		public static List<ModularSA> modca_list = new ();
		public static Dictionary<long, ModUnitData> unitMod_list = new ();
		public static List<long> skillPtrsRoundStart = new ();


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
			
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
				
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(__instance._owner, null, null, null, -1, timing);
				}
			}

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
						modsa.Enact(__instance._owner, skillModel, null, null, -1, timing);
					}
				}
			}
		}

		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnBattleStart))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnBattleStart(BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
				
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(__instance._owner, null, null, null, 0, timing);
				}
			}
		}


		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnBattleEnd))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnBattleEnd(BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
				
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(__instance._owner, null, null, null, 6, timing);
				}
			}
		}


		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnStartTurn_BeforeLog))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnStartTurn_BeforeLog(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
				
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					if (modpa.resetWhenUse) modpa.ResetAdders(); // on-demand power adder reset (used for passives)
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(__instance._owner, action.Skill, action, null, 1, timing);
				}
			}
		}


		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnStartDuel))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnStartDuel(BattleActionModel ownerAction, BattleActionModel opponentAction, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
				
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(__instance._owner, ownerAction.Skill, ownerAction, opponentAction, 3, BATTLE_EVENT_TIMING.ON_START_DUEL);
				}
			}
		}
		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnWinDuel))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnWinDuel(BattleActionModel selfAction, BattleActionModel oppoAction, int parryingCount, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
				
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(__instance._owner, selfAction.Skill, selfAction, oppoAction, 4, timing);
				}
			}
		}
		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnLoseDuel))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnLoseDuel(BattleActionModel selfAction, BattleActionModel oppoAction, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
				
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(__instance._owner, selfAction.Skill, selfAction, oppoAction, 5, timing);
				}
			}
		}


		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.BeforeAttack))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_BeforeAttack(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
				
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(__instance._owner, action.Skill, action, null, 2, timing);
				}
			}
		}


		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnEndTurn))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnEndTurn(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
				
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					if (Input.GetKeyInt(KeyCode.LeftControl)) MainClass.Logg.LogInfo("Found modpassive - OnEndTurn: " + modpa.passiveID);
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(__instance._owner, action.Skill, action, null, 9, timing);
				}
			}
		}


		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnEndBehaviour))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnEndBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
				
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(__instance._owner, action.Skill, action, null, 20, timing);
				}
			}
		}


		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnBeforeDefense))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnBeforeDefense(BattleActionModel action, PassiveDetail __instance)
		{
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
				
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					if (Input.GetKeyInt(KeyCode.LeftControl)) MainClass.Logg.LogInfo("Found modpassive - OnBeforeDefense: " + modpa.passiveID);
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(action.Model, action.Skill, action, null, 11, BATTLE_EVENT_TIMING.NONE);
				}
			}
		}

		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnDie))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnDie(BattleUnitModel killer, BattleActionModel actionOrNull, DAMAGE_SOURCE_TYPE dmgSrcType, BUFF_UNIQUE_KEYWORD keyword, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			BattleUnitModel deadUnit = __instance._owner;
			
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
				
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.modsa_target_list.Clear();
					modpa.modsa_target_list.Add(killer);
					modpa.Enact(deadUnit, null, null, actionOrNull, 12, timing);
				}
			}

			// onotherdie
			BattleObjectManager battleObjManager_inst = SingletonBehavior<BattleObjectManager>.Instance;
			foreach (BattleUnitModel unit in battleObjManager_inst.GetAliveListExceptSelf(deadUnit, false, false))
			{
				foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList) {
					if (!passiveModel.CheckActiveCondition()) continue;
					long passiveModel_intlong = passiveModel.Pointer.ToInt64();
					if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
					foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
						modpa.modsa_passiveModel = passiveModel;
						modpa.modsa_target_list.Clear();
						modpa.modsa_target_list.Add(deadUnit);
						modpa.Enact(unit, null, null, actionOrNull, 13, timing);
					}
				}
			}
		}

		[HarmonyPatch(typeof(BattleUnitModel_Abnormality), nameof(BattleUnitModel_Abnormality.GetActionSlotAdder))]
		[HarmonyPostfix]
		private static void Postfix_BattleUnitModel_Abnormality_GetActionSlotAdder(ref int __result, BattleUnitModel_Abnormality __instance)
		{
			foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList) {
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
			BattleUnitModel brokeUnit = __instance._owner;
			
			foreach (PassiveModel passiveModel in __instance.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
				
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(brokeUnit, null, null, null, 22, timing);
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
						modpa.Enact(unit, null, null, null, 23, timing);
					}
				}
			}
		}
		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnVibrationExplosionOtherUnit))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnVibrationExplosionOtherUnit(BattleUnitModel explodedUnit, BattleUnitModel giverOrNull, BattleActionModel actionOrNull, ABILITY_SOURCE_TYPE abilitySrc, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
		    foreach (PassiveModel passiveModel in __instance.PassiveList)
		    {
		        if (!passiveModel.CheckActiveCondition()) continue;
		        List<string> requireIDList = passiveModel.ClassInfo.requireIDList;
		        foreach (string param in requireIDList)
		        {
		            if (param.StartsWith("Modular/"))
		            {
		                passiveModel.OnVibrationExplosionOtherUnit(explodedUnit, giverOrNull, actionOrNull, abilitySrc, timing);
		                break;
		            }
		        }
		    }
		}
		[HarmonyPatch(typeof(PassiveModel), nameof(PassiveModel.OnVibrationExplosionOtherUnit))]
		[HarmonyPostfix]
		private static void Postfix_PassiveModel_OnVibrationExplosionOtherUnit(BattleUnitModel explodedUnit, BattleUnitModel giverOrNull, BattleActionModel actionOrNull, ABILITY_SOURCE_TYPE abilitySrc, BATTLE_EVENT_TIMING timing, PassiveModel __instance)
		{
		    long passiveModel_intlong = __instance.Pointer.ToInt64();
		    foreach (ModularSA modpa in modpaDict[passiveModel_intlong])
		    {
		        if (modpa.passiveID != __instance.ClassInfo.ID) continue;
		        if (passiveModel_intlong != modpa.ptr_intlong) continue;
		        modpa.modsa_passiveModel = __instance;
		        modpa.modsa_target_list.Clear();
		        modpa.modsa_target_list.Add(__instance.Owner);
		        modpa.Enact(explodedUnit, null, null, actionOrNull, 27, timing);
		    }
		}


		// PASSIVES END
		// PASSIVES END
		// PASSIVES END


		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetCoinScaleAdder))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetCoinScaleAdder(BattleActionModel action, ref int __result, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (modsaDict.ContainsKey(skillmodel_intlong)) {
				foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
					if (modsa.activationTiming == 10) continue;
					int power = modsa.coinScaleAdder;
					if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.LeftControl))
						MainClass.Logg.LogInfo("Found modsa - coin scale adder: " + power);
					__result += power;
				}
			}
			
			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					if (modpa.activationTiming == 10) continue;
					int power = modpa.coinScaleAdder;
					if (power != 0) MainClass.Logg.LogInfo("Found modpa - coin scale adder: ");
					__result += power;
				}
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetSkillPowerAdder))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetSkillPowerAdder(BattleActionModel action, ref int __result, SkillModel __instance) {
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (modsaDict.ContainsKey(skillmodel_intlong)) {
				foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
					if (modsa.activationTiming == 10) continue;
					int power = modsa.skillPowerAdder;
					if (power != 0) MainClass.Logg.LogInfo("Found modsa - base power adder: " + power);
					__result += power;
				}
			}
			
			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					if (modpa.activationTiming == 10) continue;
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
			if (modsaDict.ContainsKey(skillmodel_intlong)) {
				foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
					if (modsa.activationTiming == 10) continue;
					int power = modsa.skillPowerResultAdder;
					if (power != 0) MainClass.Logg.LogInfo("Found modsa - final power adder: " + power);
					__result += power;
				}
			}

			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					if (modpa.activationTiming == 10) continue;
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
			if (modsaDict.ContainsKey(skillmodel_intlong)) {
				foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
					if (modsa.activationTiming == 10) continue;
					int power = modsa.parryingResultAdder;
					if (power != 0) MainClass.Logg.LogInfo("Found modsa - clash power adder: " + power);
					__result += power;
				}
			}
			
			foreach (PassiveModel passiveModel in actorAction.Model._passiveDetail.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					if (modpa.activationTiming == 10) continue;
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
			foreach (ModularSA modca in modca_list) {
				if (modca.activationTiming == 10) continue;
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
					if (modsa.activationTiming == 10) continue;
					int power = modsa.atkDmgAdder;
					if (power != 0) __result += power;
				}
			}

			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					if (modpa.activationTiming == 10) continue;
					int power = modpa.atkDmgAdder;
					if (power != 0) __result += power;
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
				if (power != 0) __result += (float)power * 0.01f;
			}
			
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (modsaDict.ContainsKey(skillmodel_intlong)) {
				foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
					if (modsa.activationTiming == 10) continue;
					int power = modsa.atkMultAdder;
					if (power != 0) __result += (float)power * 0.01f;
				}
			}

			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					if (modpa.activationTiming == 10) continue;
					int power = modpa.atkMultAdder;
					if (power != 0) __result += power * 0.01f;
				}
			}
		}


		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnBattleStart))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnBattleStart(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(action.Model, __instance, action, null, 0, timing);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnStartTurn_BeforeLog))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnStartTurnBeforeLog(BattleActionModel action, List<BattleUnitModel> targets, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				// Reset clash modifiers if for some reason this skill is used again
				if (modsa.activationTiming == 14 || modsa.activationTiming == 15) modsa.ResetAdders();
				// normal code
				modsa.modsa_target_list = targets;
				modsa.Enact(action.Model, __instance, action, null, 1, timing);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.BeforeAttack))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_BeforeAttack(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(action.Model, __instance, action, null, 2, timing);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnBeforeParryingOnce))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnBeforeParryingOnce(BattleActionModel ownerAction, BattleActionModel oppoAction, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (modsaDict.ContainsKey(skillmodel_intlong)) {
				foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
					modsa.Enact(ownerAction.Model, __instance, ownerAction, oppoAction, 14, BATTLE_EVENT_TIMING.ALL_TIMING);
				}
			}

			foreach (PassiveModel passiveModel in ownerAction.Model._passiveDetail.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(ownerAction.Model, __instance, ownerAction, oppoAction, 14, BATTLE_EVENT_TIMING.ALL_TIMING);
				}
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnBeforeParryingOnce_AfterLog))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnBeforeParryingOnce_AfterLog(BattleActionModel ownerAction, BattleActionModel oppoAction, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(ownerAction.Model, __instance, ownerAction, oppoAction, 15, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnWinDuel))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnWinDuel(BattleActionModel selfAction, BattleActionModel oppoAction, BATTLE_EVENT_TIMING timing, int parryingCount, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(selfAction.Model, __instance, selfAction, oppoAction, 4, timing);
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnLoseDuel))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnLoseDuel(BattleActionModel selfAction, BattleActionModel oppoAction, BATTLE_EVENT_TIMING timing, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(selfAction.Model, __instance, selfAction, oppoAction, 5, timing);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnRoundEnd))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnRoundEnd(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(action.Model, __instance, action, null, 6, timing);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnEndTurn))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnEndTurn(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(action.Model, __instance, action, null, 9, timing);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnSucceedEvade))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnSucceedEvade(BattleActionModel attackerAction, BattleActionModel evadeAction, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(evadeAction.Model, __instance, evadeAction, attackerAction, 16, timing);
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnFailedEvade))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnFailedEvade(BattleActionModel attackerAction, BattleActionModel evadeAction, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(evadeAction.Model, __instance, evadeAction, attackerAction, 17, timing);
			}
		}
		

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnStartBehaviour))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnStartBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(action.Model, __instance, action, null, 18, timing);
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.BeforeBehaviour))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_BeforeBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(action.Model, __instance, action, null, 19, timing);
			}
		}
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnEndBehaviour))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnEndBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(action.Model, __instance, action, null, 20, timing);
			}
		}
		
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnDiscarded))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_OnDiscarded(BattleActionModel action, BATTLE_EVENT_TIMING timing, SkillModel __instance) {
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (!modsaDict.ContainsKey(skillmodel_intlong)) return;
			foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
				modsa.Enact(action.Model, __instance, action, null, 24, timing);
			}
		}

		// SKILLMODEL UP TO HERE
		// SKILLMODEL UP TO HERE
		// SKILLMODEL UP TO HERE


		[HarmonyPatch(typeof(BattleActionModel), nameof(BattleActionModel.OnAttackConfirmed))]
		[HarmonyPostfix]
		private static void Postfix_BattleActionModel_OnAttackConfirmed(CoinModel coin, BattleUnitModel target, BATTLE_EVENT_TIMING timing, bool isCritical, BattleActionModel __instance)
		{
			long coinmodel_intlong = coin.Pointer.ToInt64();
			foreach (ModularSA modca in modca_list)
			{
				if (coinmodel_intlong != modca.ptr_intlong) continue;
				//modca.lastFinalDmg = finalDmg;
				modca.wasCrit = isCritical;
				//modca.wasClash = isWinDuel.HasValue;
				//if (modca.wasClash) modca.wasWin = isWinDuel.Value;
				modca.modsa_coinModel = coin;
				modca.Enact(__instance.Model, __instance.Skill, __instance, null, 7, timing);
			}
			
			long skillmodel_intlong = __instance.Skill.Pointer.ToInt64();
			if (modsaDict.ContainsKey(skillmodel_intlong)) {
				foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
					modsa.wasCrit = isCritical;
					modsa.Enact(__instance.Model, __instance.Skill, __instance, null, 7, timing);
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
					modpa.Enact(__instance.Model, __instance.Skill, __instance, null, 7, timing);
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
					modpa.Enact(__instance.Model, __instance.Skill, __instance, null, 8, timing);
				}
			}
		}


		[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnKillTarget))]
		[HarmonyPostfix]
		private static void Postfix_BattleUnitModel_OnKillTarget(BattleActionModel actionOrNull, BattleUnitModel target, DAMAGE_SOURCE_TYPE dmgSrcType, BATTLE_EVENT_TIMING timing, BattleUnitModel killer, BattleUnitModel __instance)
		{
			if (actionOrNull == null || actionOrNull.Skill == null) return;
			
			SkillModel skill = actionOrNull.Skill;
			long skillmodel_intlong = skill.Pointer.ToInt64();
			if (modsaDict.ContainsKey(skillmodel_intlong)) {
				foreach (ModularSA modsa in modsaDict[skillmodel_intlong]) {
					modsa.Enact(__instance, skill, actionOrNull, null, 21, timing);
				}
			}

			foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(__instance, skill, actionOrNull, null, 21, timing);
				}
			}
		}


		[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnZeroHp))]
		[HarmonyPrefix]
		private static void Prefix_BattleUnitModel_OnZeroHp(BattleUnitModel __instance)
		{
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
		
		
		[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnEndEnemyAttack))]
		[HarmonyPostfix]
		private static void Postfix_BattleUnitModel_OnEndEnemyAttack(BattleActionModel action, BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
		{
			foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList) {
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
				foreach (ModularSA modpa in modpaDict[passiveModel_intlong]) {
					modpa.modsa_passiveModel = passiveModel;
					modpa.modsa_target_list.Clear();
					modpa.modsa_target_list.Add(__instance);
					modpa.Enact(action.Model, action.Skill, action, null, 26, timing);
				}
			}
		}
		
		
		// end
	}
}
