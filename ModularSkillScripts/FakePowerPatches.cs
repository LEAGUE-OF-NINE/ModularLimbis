using HarmonyLib;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts;

class FakePowerPatches
{
	[HarmonyPatch(typeof(SkillModelManager), nameof(SkillModelManager.GetExpectedWinRate))]
	[HarmonyPrefix]
	private static void Postfix_SkillModelManager_GetExpectedWinRate(BattleActionModel selfAction)
	{
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		foreach (long key in SkillScriptInitPatch.modpaDict.Keys) {
			List<ModularSA> value = SkillScriptInitPatch.modpaDict[key];
			foreach (ModularSA modular in value) {
				if (modular.activationTiming != actevent_FakePower) continue;
				modular.ResetAdders();
			}
		}
			
		long skillmodel_intlong = selfAction.Skill.Pointer.ToInt64();
		if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong)) {
			foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong]) {
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				modsa.Enact(selfAction.Model, selfAction.Skill, selfAction, null, actevent_FakePower, BATTLE_EVENT_TIMING.NONE);
			}
		}

		foreach (PassiveModel passiveModel in selfAction.Model._passiveDetail.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong]) {
				if (modpa.activationTiming != actevent_FakePower) continue;
				modpa.Enact(selfAction.Model, selfAction.Skill, selfAction, null, actevent_FakePower, BATTLE_EVENT_TIMING.NONE);
			}
		}
	}


	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedSkillPowerAdder))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetExpectedSkillPowerAdder(BattleActionModel action, ref int __result, SkillModel __instance)
	{
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong)) {
			foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong]) {
				if (modsa.activationTiming != actevent_FakePower) continue;
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				int power = modsa.skillPowerAdder;
				__result += power;
			}
		}

		foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong]) {
				if (modpa.activationTiming != actevent_FakePower) continue;
				int power = modpa.skillPowerAdder;
				__result += power;
			}
		}
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedSkillPowerResultAdder))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetExpectedSkillPowerResultAdder(BattleActionModel action, ref int __result,
		SkillModel __instance)
	{
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong))
		{
			foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong])
			{
				if (modsa.activationTiming != actevent_FakePower) continue;
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				int power = modsa.skillPowerResultAdder;
				__result += power;
			}
		}

		foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong]) {
				if (modpa.activationTiming != actevent_FakePower) continue;
				int power = modpa.skillPowerResultAdder;
				__result += power;
			}
		}
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedParryingResultAdder))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetExpectedParryingResultAdder(BattleActionModel actorAction,
		ref int __result, SkillModel __instance)
	{
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong))
		{
			foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong])
			{
				if (modsa.activationTiming != actevent_FakePower) continue;
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				int power = modsa.parryingResultAdder;
				__result += power;
			}
		}

		foreach (PassiveModel passiveModel in actorAction.Model._passiveDetail.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong]) {
				if (modpa.activationTiming != actevent_FakePower) continue;
				int power = modpa.parryingResultAdder;
				__result += power;
			}
		}
	}
		
	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedCoinScaleAdder))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetExpectedCoinScaleAdder(BattleActionModel action, CoinModel coin, ref int __result, SkillModel __instance)
	{
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		long skillmodel_intlong = __instance.Pointer.ToInt64();
		if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong)) {
			foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong])
			{
				if (modsa.activationTiming != actevent_FakePower) continue;
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				int power = modsa.coinScaleAdder;
				__result += power;
			}
		}

		foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong]) {
				if (modpa.activationTiming != actevent_FakePower) continue;
				int power = modpa.coinScaleAdder;
				__result += power;
			}
		}
	}
		
}