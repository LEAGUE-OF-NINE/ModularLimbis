using HarmonyLib;

namespace ModularSkillScripts
{
	class FakePowerPatches
	{
		[HarmonyPatch(typeof(SkillModelManager), nameof(SkillModelManager.GetExpectedWinRate))]
		[HarmonyPrefix]
		private static void Postfix_SkillModelManager_GetExpectedWinRate(BattleActionModel selfAction)
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.modpa_list)
			{
				modpa.ResetAdders();
			}

			long skillmodel_intlong = selfAction.Skill.Pointer.ToInt64();
			if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong)) {
				foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong]) {
					if (skillmodel_intlong != modsa.ptr_intlong) continue;
					modsa.modsa_selfAction = selfAction;
					modsa.Enact(selfAction.Skill, 10, BATTLE_EVENT_TIMING.NONE);
				}
			}

			foreach (PassiveModel passiveModel in selfAction.Model._passiveDetail.PassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in SkillScriptInitPatch.modpa_list)
				{
					if (passivemodel_intlong != modpa.ptr_intlong) continue;
					modpa.modsa_selfAction = selfAction;
					modpa.Enact(selfAction.Skill, 10, BATTLE_EVENT_TIMING.NONE);
				}
			}
		}


		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedSkillPowerAdder))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetExpectedSkillPowerAdder(BattleActionModel action, ref int __result, SkillModel __instance) {
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong)) {
				foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong]) {
					if (modsa.activationTiming != 10) continue;
					if (skillmodel_intlong != modsa.ptr_intlong) continue;
					int power = modsa.skillPowerAdder;
					__result += power;
				}
			}

			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList)
			{
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in SkillScriptInitPatch.modpa_list)
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
		private static void Postfix_SkillModel_GetExpectedSkillPowerResultAdder(BattleActionModel action, ref int __result,
			SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong))
			{
				foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong])
				{
					if (modsa.activationTiming != 10) continue;
					if (skillmodel_intlong != modsa.ptr_intlong) continue;
					int power = modsa.skillPowerResultAdder;
					__result += power;
				}
			}

			foreach (PassiveModel passiveModel in action.Model._passiveDetail.PassiveList)
			{
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in SkillScriptInitPatch.modpa_list)
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
		private static void Postfix_SkillModel_GetExpectedParryingResultAdder(BattleActionModel actorAction,
			ref int __result, SkillModel __instance)
		{
			long skillmodel_intlong = __instance.Pointer.ToInt64();
			if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong))
			{
				foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong])
				{
					if (modsa.activationTiming != 10) continue;
					if (skillmodel_intlong != modsa.ptr_intlong) continue;
					int power = modsa.parryingResultAdder;
					__result += power;
				}
			}

			foreach (PassiveModel passiveModel in actorAction.Model._passiveDetail.PassiveList)
			{
				long passivemodel_intlong = passiveModel.Pointer.ToInt64();
				foreach (ModularSA modpa in SkillScriptInitPatch.modpa_list)
				{
					if (modpa.activationTiming != 10) continue;
					if (passivemodel_intlong != modpa.ptr_intlong) continue;
					int power = modpa.parryingResultAdder;
					__result += power;
				}
			}
		}
	}
}