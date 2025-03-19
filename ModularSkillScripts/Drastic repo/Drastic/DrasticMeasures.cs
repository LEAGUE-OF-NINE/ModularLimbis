using System;
using HarmonyLib;

namespace Drastic
{
	class DrasticMeasures
	{
		[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnLoseDuel))]
		[HarmonyPostfix]
		private static void Postfix_BuffModel_OnLoseDuel(BattleActionModel ownerAction, BATTLE_EVENT_TIMING timing, BuffModel __instance)
		{
			if (__instance.IsKeyword(BUFF_UNIQUE_KEYWORD.Paralysis))
			{
				int stack = __instance.GetCurrentStack();
				int loss = Math.Min(stack - 5, 2);
				if (loss > 0) __instance.LoseStack(ownerAction.Model, 0, timing, loss);
			}
			else if (__instance.IsKeyword(BUFF_UNIQUE_KEYWORD.Reduction) && !ownerAction.IsDefense())
			{
				__instance.LoseStack(ownerAction.Model, 0, timing, 1);
			}
		}

		[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnEndBehaviour))]
		[HarmonyPostfix]
		private static void Postfix_BuffModel_OnEndBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing, BuffModel __instance)
		{
			if ((__instance.IsKeyword(BUFF_UNIQUE_KEYWORD.Reduction) || __instance.IsKeyword(BUFF_UNIQUE_KEYWORD.AttackDmgDown)) && !action.IsDefense())
			{
				__instance.LoseStack(action.Model, 0, timing, 1);
			}
		}

		[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnSucceedEvade))]
		[HarmonyPostfix]
		private static void Postfix_PassiveDetail_OnSucceedEvade(BattleActionModel evadeAction, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
		{
			BattleUnitModel unit = evadeAction.Model;

			BuffDetail bufDetail = unit._buffDetail;
			int disarm = bufDetail.GetActivatedBuffStack(BUFF_UNIQUE_KEYWORD.Disarming, false);
			if (disarm >= 6) return;

			SkillModel skill = evadeAction.Skill;
			if (skill.SkillAbilityList.Count > 0)
			{
				skill.SkillAbilityList[0].GiveBuff_Self(unit, BUFF_UNIQUE_KEYWORD.Disarming, 1, 0, 0, timing, evadeAction);
			}
			else if (unit._passiveDetail.PassiveList.Count > 0)
			{
				PassiveModel passive = unit._passiveDetail.PassiveList[0];
				passive._script.GiveBuff_Self(unit, BUFF_UNIQUE_KEYWORD.Disarming, 1, 0, 0, timing, evadeAction);
			}
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetAttackDmgMultiplier))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetAttackDmgMultiplier(ref float __result, SkillModel __instance)
		{
			float bonus = 0.0f;
			if (__instance.IsEgoSkill()) bonus += 0.1f;
			if (__instance.GetAttributeType() == ATTRIBUTE_TYPE.CRIMSON) bonus += 0.1f;

			if (bonus > 0.01f) __result += bonus;
		}

		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetExpectedAttackDmgMultiplier))]
		[HarmonyPostfix]
		private static void Postfix_SkillModel_GetExpectedAttackDmgMultiplier(ref float __result, SkillModel __instance)
		{
			float bonus = 0.0f;
			if (__instance.IsEgoSkill()) bonus += 0.1f;
			if (__instance.GetAttributeType() == ATTRIBUTE_TYPE.CRIMSON) bonus += 0.1f;

			if (bonus > 0.01f) __result += bonus;
		}

		[HarmonyPatch(typeof(CoinModel), nameof(CoinModel.GetCoinScaleAdder))]
		[HarmonyPostfix]
		private static void Postfix_CoinModel_GetCoinScaleAdder(ref int __result, BattleActionModel action, CoinModel __instance)
		{
			if (action.Model == null) return;
			BattleUnitModel unit = action.Model;
			if (unit._buffDetail.HasActivatedBuff(BUFF_UNIQUE_KEYWORD.Paralysis))
			{
				int coinpower = __instance._scale;
				__result -= (int)Math.Ceiling(coinpower * 0.5);
			}
		}
	}
}
