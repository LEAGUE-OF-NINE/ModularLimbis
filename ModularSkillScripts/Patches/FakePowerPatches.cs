using HarmonyLib;
using Utils;

namespace ModularSkillScripts.Patches;

public class FakePowerPatches
{
	public static int actevent_FakePower = 0;
	[HarmonyPatch(typeof(BattleActionModel), nameof(BattleActionModel.OnSetExpectedTarget))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_OnSetExpectedTarget(BattleActionModel targetAction, BattleActionModel __instance)
	{
		BattleUnitModel unit = __instance.Model;
		SkillModel skill = __instance.Skill;
		if (unit == null || skill == null) return;
		
		foreach (BuffModel buf in unit._buffDetail.GetActivatedBuffModelAll())
		{
			foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel(buf))
			{
				if (modba.activationTiming != actevent_FakePower) continue;
				modba.modsa_buffModel = buf;
				modba.Enact(unit, skill, __instance, targetAction, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel(skill)) {
			if (modsa.activationTiming != actevent_FakePower) continue;
			modsa.Enact(unit, skill, __instance, targetAction, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
		}
		
		foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(unit, skill, __instance, targetAction, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in unit._passiveDetail.EgoPassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(egoPassiveModel, false))
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
		
		BattleActionModel action = actorAction?.CurrentBattleAction;
		SkillModel skill = action?.Skill;
		if (skill == null) return;

		BattleActionModel tgtact = targetAction?.CurrentBattleAction;
		
		foreach (BuffModel buf in __instance._buffDetail.GetActivatedBuffModelAll())
		{
			foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel(buf))
			{
				if (modba.activationTiming != actevent_FakePower) continue;
				modba.modsa_buffModel = buf;
				modba.Enact(__instance, skill, action, tgtact, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel(skill)) {
			if (modsa.activationTiming != actevent_FakePower) continue;
			modsa.Enact(__instance, skill, action, tgtact, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
		}
		
		foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance, skill, action, tgtact, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in __instance._passiveDetail.EgoPassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(egoPassiveModel, false))
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
		
		foreach (BuffModel buf in __instance._buffDetail.GetActivatedBuffModelAll())
		{
			foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel(buf))
			{
				if (modba.activationTiming != actevent_FakePower) continue;
				modba.modsa_buffModel = buf;
				modba.Enact(__instance, skill, action, null, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel(skill)) {
			if (modsa.activationTiming != actevent_FakePower) continue;
			modsa.Enact(__instance, skill, action, null, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
		}
		
		foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance, skill, action, null, actevent_FakePower, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in __instance._passiveDetail.EgoPassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(egoPassiveModel, false))
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
	private static void Postfix_SkillModel_GetExpectedSkillPowerAdder(BattleActionModel action, ref int __result, SkillModel __instance)
	{
		BattleUnitModel unit = action.Model;
		if (unit == null) return;
		
		foreach (BuffModel buf in unit._buffDetail.GetActivatedBuffModelAll())
		{
			foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel(buf))
			{
				if (modba.activationTiming != actevent_FakePower) continue;
				int power = modba.skillPowerAdder;
				__result += power;
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel(__instance)) {
			if (modsa.activationTiming != actevent_FakePower) continue;
			int power = modsa.skillPowerAdder;
			__result += power;
		}

		foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				int power = modpa.skillPowerAdder;
				__result += power;
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in unit._passiveDetail.EgoPassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(egoPassiveModel, false))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				int power = modpa.skillPowerAdder;
				__result += power;
			}
		}
		
		/* I'm not doing this
		SupportPasPatch.SupportPassiveInit(SkillScriptInitPatch.modpaDict);
		foreach (SupporterPassiveModel supportPassive in MainClass.activeSupporterPassiveList)
		{
			List<ModularSA> modpaList = SkillScriptInitPatch.GetAllModpaFromPasmodelSupport(supportPassive);
			for (int i = 0; i < modpaList.Count; i++)
			{
				if (modpaList[i].activationTiming != actevent) continue;
				int power = modpaList[i].skillPowerAdder;
				__result += power;
			}
		}*/
	}

	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedSkillPowerResultAdder))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetExpectedSkillPowerResultAdder(BattleActionModel action, ref int __result,
		SkillModel __instance)
	{
		BattleUnitModel unit = action.Model;
		if (unit == null) return;
		
		foreach (BuffModel buf in unit._buffDetail.GetActivatedBuffModelAll())
		{
			foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel(buf))
			{
				if (modba.activationTiming != actevent_FakePower) continue;
				int power = modba.skillPowerResultAdder;
				__result += power;
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel(__instance)) {
			if (modsa.activationTiming != actevent_FakePower) continue;
			int power = modsa.skillPowerResultAdder;
			__result += power;
		}

		foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				int power = modpa.skillPowerResultAdder;
				__result += power;
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in unit._passiveDetail.EgoPassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(egoPassiveModel, false))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				int power = modpa.skillPowerResultAdder;
				__result += power;
			}
		}
	}
		
	[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedParryingResultAdder))]
	[HarmonyPostfix]
	private static void Postfix_SkillModel_GetExpectedParryingResultAdder(BattleActionModel actorAction, ref int __result, SkillModel __instance)
	{
		BattleUnitModel unit = actorAction.Model;
		if (unit == null) return;
		
		foreach (BuffModel buf in unit._buffDetail.GetActivatedBuffModelAll())
		{
			foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel(buf))
			{
				if (modba.activationTiming != actevent_FakePower) continue;
				int power = modba.parryingResultAdder;
				__result += power;
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel(__instance)) {
			if (modsa.activationTiming != actevent_FakePower) continue;
			int power = modsa.parryingResultAdder;
			__result += power;
		}

		foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				int power = modpa.parryingResultAdder;
				__result += power;
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in unit._passiveDetail.EgoPassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(egoPassiveModel, false))
			{
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
		BattleUnitModel unit = action.Model;
		if (unit == null) return;
		
		foreach (BuffModel buf in unit._buffDetail.GetActivatedBuffModelAll())
		{
			foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel(buf))
			{
				if (modba.activationTiming != actevent_FakePower) continue;
				int power = modba.coinScaleAdder;
				__result += power;
			}
		}
		
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel(__instance)) {
			if (modsa.activationTiming != actevent_FakePower) continue;
			int power = modsa.coinScaleAdder;
			__result += power;
		}

		foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				int power = modpa.coinScaleAdder;
				__result += power;
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in unit._passiveDetail.EgoPassiveList.CopyList())
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(egoPassiveModel, false))
			{
				if (modpa.activationTiming != actevent_FakePower) continue;
				int power = modpa.coinScaleAdder;
				__result += power;
			}
		}
	}

}