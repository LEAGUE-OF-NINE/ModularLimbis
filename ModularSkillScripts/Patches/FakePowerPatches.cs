using HarmonyLib;
using Utils;

namespace ModularSkillScripts.Patches;

public class FakePowerPatches
{
	public static int actevent_FakePower = 0;
	public static int actevent_BaseCheck = 0;
	[HarmonyPatch(typeof(BattleActionModel), nameof(BattleActionModel.OnSetExpectedTarget))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnSetExpectedTarget(BattleActionModel targetAction, BattleActionModel __instance)
	{
		BattleUnitModel unit = __instance.Model;
		SkillModel skill = __instance.Skill;
		if (unit == null || skill == null) return;
		
		foreach (BuffModel buf in unit.GetActivatedBuffModels()) {
			foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel_Fast(buf))
			{
				if (modba.activationTiming != actevent_FakePower) continue;
				modba.modsa_buffModel = buf;
				modba.Enact(unit, skill, __instance, targetAction, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel_Fast(skill)) {
			if (modsa.activationTiming != actevent_FakePower) continue;
			modsa.Enact(unit, skill, __instance, targetAction, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
		}
		
		foreach (PassiveModel passiveModel in unit._passiveDetail._passivelist) {
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(passiveModel))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(unit, skill, __instance, targetAction, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in unit._passiveDetail._egoPassiveList) {
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(egoPassiveModel, false))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				modpa.modsa_passiveModel = egoPassiveModel;
				modpa.Enact(unit, skill, __instance, targetAction, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
	}
	
	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnAddBattleAction))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnAddBattleAction(SinActionModel actorAction, SinActionModel targetAction, BattleUnitModel __instance)
	{
		// Intentionally not copying list because we don't need to. It's FakePower.
		BattleActionModel action = actorAction?.CurrentBattleAction;
		SkillModel skill = action?.Skill;
		if (skill == null) return;

		BattleActionModel tgtact = targetAction?.CurrentBattleAction;
		
		foreach (BuffModel buf in __instance.GetActivatedBuffModels()) {
			foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel_Fast(buf))
			{
				if (modba.activationTiming != actevent_FakePower) continue;
				modba.modsa_buffModel = buf;
				modba.Enact(__instance, skill, action, tgtact, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel_Fast(skill)) {
			if (modsa.activationTiming != actevent_FakePower) continue;
			modsa.Enact(__instance, skill, action, tgtact, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
		}
		
		foreach (PassiveModel passiveModel in __instance._passiveDetail._passivelist)
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(passiveModel))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance, skill, action, tgtact, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in __instance._passiveDetail._egoPassiveList)
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(egoPassiveModel, false))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				modpa.modsa_passiveModel = egoPassiveModel;
				modpa.Enact(__instance, skill, action, tgtact, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
	}
	
	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnRemoveBattleAction))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnRemoveBattleAction(SinActionModel actorAction, BattleUnitModel __instance)
	{
		
		BattleActionModel action = actorAction?.CurrentBattleAction;
		SkillModel skill = action?.Skill;
		if (skill == null) return;
		
		foreach (BuffModel buf in __instance.GetActivatedBuffModels()) {
			foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel_Fast(buf)) {
				if (modba.activationTiming != actevent_FakePower) continue;
				modba.modsa_buffModel = buf;
				modba.Enact(__instance, skill, action, null, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel_Fast(skill)) {
			if (modsa.activationTiming != actevent_FakePower) continue;
			modsa.Enact(__instance, skill, action, null, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
		}
		
		foreach (PassiveModel passiveModel in __instance._passiveDetail._passivelist) {
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(passiveModel))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance, skill, action, null, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in __instance._passiveDetail._egoPassiveList) {
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(egoPassiveModel, false))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				modpa.modsa_passiveModel = egoPassiveModel;
				modpa.Enact(__instance, skill, action, null, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
	}
/*
	[HarmonyPatch(typeof(SkillModelManager), nameof(SkillModelManager.GetExpectedWinRate))]
	[HarmonyPrefix]
	private static void Postfix_SkillModelManager_GetExpectedWinRate(BattleActionModel selfAction, BattleActionModel oppoAction)
	{
		foreach (long key in SkillScriptInitPatch.modpaDict.Keys) {
			List<ModularSA> value = SkillScriptInitPatch.modpaDict[key];
			foreach (ModularSA modular in value) {
				if (modular.activationTiming != actevent_FakePower) continue;
				modular.ResetAdders();
			}
		}
		SkillAbility_RingFingerFavuismTestEffectOnSetTarget
		long skillmodel_intlong = selfAction.Skill.Pointer.ToInt64();
		if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong)) {
			foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong]) {
				if (skillmodel_intlong != modsa.ptr_intlong) continue;
				modsa.Enact(selfAction.Model, selfAction.Skill, selfAction, oppoAction, actevent_FakePower, BATTLE_EVENT_TIMING.NONE);
			}
		}

		foreach (PassiveModel passiveModel in selfAction.Model._passiveDetail.PassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				modpa.Enact(selfAction.Model, selfAction.Skill, selfAction, oppoAction, actevent_FakePower, BATTLE_EVENT_TIMING.NONE);
			}
		}
		SupportPasPatch.SupportPassiveInit(SkillScriptInitPatch.modpaDict);
		foreach (SupporterPassiveModel supportPassive in MainClass.activeSupporterPassiveList)
		{
			List<ModularSA> modpaList = SkillScriptInitPatch.GetAllModpaFromPasmodelSupport(supportPassive);
			for (int i = 0; i < modpaList.Count; i++)
			{
				if (modpaList[i].activationTiming != actevent_FakePower) continue;
				modpaList[i].Enact(selfAction.Model, selfAction.Skill, selfAction, oppoAction, actevent_FakePower, BATTLE_EVENT_TIMING.NONE);
			}
		}
	}
*/

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedSkillPowerAdder))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetExpectedSkillPowerAdder(
		BattleActionModel action,
		COIN_ROLL_TYPE rollType,
		SinActionModel expectedTargetSinActionOrNull, 
		ref int __result, SkillModel __instance)
	{
		BattleUnitModel unit = action.Model;
		if (unit == null) return;
		SkillModel skill = action.Skill;
		if (skill == null) return;

		BattleActionModel opposing_action = expectedTargetSinActionOrNull?._currentBattleAction;
		
		foreach (BuffModel buf in unit.GetActivatedBuffModels()) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModbaFromBuffModel_Fast(buf)) {
				if (modsa.activationTiming != actevent_BaseCheck) continue;
				modsa.modsa_buffModel = buf;
				modsa.modsa_expected_sinaction = expectedTargetSinActionOrNull;
				modsa.Enact(unit, skill, action, opposing_action, actevent_BaseCheck, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel_Fast(skill)) {
			if (modsa.activationTiming != actevent_BaseCheck) continue;
			modsa.modsa_expected_sinaction = expectedTargetSinActionOrNull;
			modsa.Enact(unit, skill, action, opposing_action, actevent_BaseCheck, BATTLE_EVENT_TIMING.ALL_TIMING);
		}
		
		foreach (PassiveModel passiveModel in unit._passiveDetail._passivelist) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(passiveModel))
			{
				if (modsa.activationTiming != actevent_BaseCheck) continue;
				modsa.modsa_passiveModel = passiveModel;
				modsa.modsa_expected_sinaction = expectedTargetSinActionOrNull;
				modsa.Enact(unit, skill, action, opposing_action, actevent_BaseCheck, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in unit._passiveDetail._egoPassiveList) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(egoPassiveModel, false))
			{
				if (modsa.activationTiming != actevent_BaseCheck) continue;
				modsa.modsa_passiveModel = egoPassiveModel;
				modsa.modsa_expected_sinaction = expectedTargetSinActionOrNull;
				modsa.Enact(unit, skill, action, opposing_action, actevent_BaseCheck, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		
		foreach (BuffModel buf in unit.GetActivatedBuffModels()) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModbaFromBuffModel_Fast(buf)) {
				if (!modsa.EXPECTED) continue;
				__result += modsa.skillPowerAdder;
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel_Fast(__instance)) {
			if (!modsa.EXPECTED) continue;
			__result += modsa.skillPowerAdder;
		}

		foreach (PassiveModel passiveModel in unit._passiveDetail._passivelist) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(passiveModel)) {
				if (!modsa.EXPECTED) continue;
				__result += modsa.skillPowerAdder;
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in unit._passiveDetail._egoPassiveList) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(egoPassiveModel, false)) {
				if (!modsa.EXPECTED) continue;
				__result += modsa.skillPowerAdder;
			}
		}
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedSkillPowerResultAdder))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetExpectedSkillPowerResultAdder(BattleActionModel action, ref int __result,
		SkillModel __instance)
	{
		BattleUnitModel unit = action.Model;
		if (unit == null) return;
		
		foreach (BuffModel buf in unit.GetActivatedBuffModels()) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModbaFromBuffModel_Fast(buf)) {
				if (!modsa.EXPECTED) continue;
				__result += modsa.skillPowerResultAdder;
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel_Fast(__instance)) {
			if (!modsa.EXPECTED) continue;
			__result += modsa.skillPowerResultAdder;
		}

		foreach (PassiveModel passiveModel in unit._passiveDetail._passivelist) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(passiveModel)) {
				if (!modsa.EXPECTED) continue;
				__result += modsa.skillPowerResultAdder;
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in unit._passiveDetail._egoPassiveList) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(egoPassiveModel, false)) {
				if (!modsa.EXPECTED) continue;
				__result += modsa.skillPowerResultAdder;
			}
		}
	}
		
	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedParryingResultAdder))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetExpectedParryingResultAdder(BattleActionModel actorAction, ref int __result, SkillModel __instance)
	{
		BattleUnitModel unit = actorAction.Model;
		if (unit == null) return;
		
		foreach (BuffModel buf in unit.GetActivatedBuffModels()) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModbaFromBuffModel_Fast(buf)) {
				if (!modsa.EXPECTED) continue;
				__result += modsa.parryingResultAdder;
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel_Fast(__instance)) {
			if (!modsa.EXPECTED) continue;
			__result += modsa.parryingResultAdder;
		}

		foreach (PassiveModel passiveModel in unit._passiveDetail._passivelist) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(passiveModel))
			{
				if (!modsa.EXPECTED) continue;
				__result += modsa.parryingResultAdder;
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in unit._passiveDetail._egoPassiveList) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(egoPassiveModel, false))
			{
				if (!modsa.EXPECTED) continue;
				__result += modsa.parryingResultAdder;
			}
		}
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedCoinScaleAdder))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetExpectedCoinScaleAdder(BattleActionModel action, CoinModel coin, ref int __result, SkillModel __instance)
	{
		BattleUnitModel unit = action.Model;
		if (unit == null) return;
		PassiveDetail pasdetail = unit._passiveDetail;
		
		foreach (BuffModel buf in unit.GetActivatedBuffModels()) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModbaFromBuffModel_Fast(buf))
			{
				if (!modsa.EXPECTED) continue;
				__result += modsa.coinScaleAdder;
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel_Fast(__instance)) {
			if (!modsa.EXPECTED) continue;
			__result += modsa.coinScaleAdder;
		}

		foreach (PassiveModel passiveModel in pasdetail._passivelist) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(passiveModel))
			{
				if (!modsa.EXPECTED) continue;
				__result += modsa.coinScaleAdder;
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in pasdetail._egoPassiveList) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(egoPassiveModel, false))
			{
				if (!modsa.EXPECTED) continue;
				__result += modsa.coinScaleAdder;
			}
		}
	}

}