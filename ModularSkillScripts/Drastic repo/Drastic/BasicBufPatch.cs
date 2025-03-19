using HarmonyLib;

namespace Drastic
{
	class BasicBufPatch
	{
		[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnRoundEnd))]
		[HarmonyPrefix]
		private static bool Prefix_BuffModel_OnRoundEnd(BattleUnitModel unit, BATTLE_EVENT_TIMING timing, BuffModel __instance)
		{
			if (__instance.IsKeyword(BUFF_UNIQUE_KEYWORD.Combustion))
			{
				int stack = __instance.GetCurrentStack();
				int turn = __instance.GetCurrentTurn();

				int dmg = stack;
				if (!unit.IsFaction(UNIT_FACTION.PLAYER)) dmg *= 2;

				float mult = 1.0f + (float)((turn - 1) * 0.05);
				dmg = (int)System.Math.Floor(dmg * mult);

				__instance._abilityList[0].TakeAbsHpDamage_Buff(unit, dmg, timing, DAMAGE_SOURCE_TYPE.BUFF, __instance._buffInfo, true, null, null);

				//int stackLoss = turn;
				//if (stackLoss >= stack) stackLoss = stack - 1;
				if (stack > 1) __instance.LoseStack(unit, 0, timing, (int)System.Math.Ceiling(stack * 0.2));
				__instance.LoseTurn(unit, timing, (int)System.Math.Ceiling((double)turn / 3));
				return false;
			}
			else if (__instance.IsKeyword(BUFF_UNIQUE_KEYWORD.Laceration) || __instance.IsKeyword(BUFF_UNIQUE_KEYWORD.Sinking))
			{
				int stack = __instance.GetCurrentStack();
				int loss = (int)System.Math.Floor((double)stack / 3);
				if (loss > 0) __instance.LoseStack(unit, 0, timing, loss);
			}
			else if (__instance.IsKeyword(BUFF_UNIQUE_KEYWORD.Burst))
			{
				int stack = __instance.GetCurrentStack();
				int loss = System.Math.Min(stack - 1, 2);
				if (loss > 0) __instance.LoseStack(unit, 0, timing, loss);
			}
			return true;
		}

		//[HarmonyPatch(typeof(BuffAbility_Laceration), nameof(BuffAbility_Laceration.Ability))]
		//[HarmonyPostfix]
		//private static void Postfix_BuffAbilityLaceration_Ability(BattleUnitModel unit, BATTLE_EVENT_TIMING timing, BuffInfo info, BuffAbility_Laceration __instance)
		//{
		//    if (info._stack > 20) unit.LoseBuffStack(info.GetKeyword(), 2, timing);
		//    else if (info._stack > 1) unit.LoseBuffStack(info.GetKeyword(), 1, timing);
		//}

		//[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.TakeSinBuffHpDamage))]
		//[HarmonyPrefix]
		//private static void Prefix_BattleUnitModel_TakeSinBuffHpDamage(ref int value, ref int hpDamage, BUFF_UNIQUE_KEYWORD keyword)
		//{
		//    if (keyword == BUFF_UNIQUE_KEYWORD.Laceration)
		//    {
		//        value *= 2;
		//        hpDamage *= 2;
		//    }
		//}

		[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnAfterParryingOnce_BeforeLog))]
		[HarmonyPrefix]
		private static bool Prefix_BuffModel_OnRollOneCoin_OnAfterParryingOnceBeforeLog(BattleActionModel selfAction, BATTLE_EVENT_TIMING timing, BuffModel __instance)
		{
			if (__instance.IsKeyword(BUFF_UNIQUE_KEYWORD.Laceration))
			{
				if (!selfAction.IsDefense())
				{
					int stack = __instance.GetCurrentStack();
					int dmg = stack;
					if (!selfAction.Model.IsFaction(UNIT_FACTION.PLAYER)) dmg *= 2;
					__instance._abilityList[0].TakeAbsHpDamage_Buff(selfAction.Model, dmg, timing, DAMAGE_SOURCE_TYPE.BUFF, __instance._buffInfo, true, null, null);
					if (stack > 20) __instance.LoseStack(selfAction.Model, 0, timing, 2);
					else if (stack > 1) __instance.LoseStack(selfAction.Model, 0, timing, 1);
					__instance.LoseTurn(selfAction.Model, timing, 1);
				}
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnRollOneCoin_AfterAttack))]
		[HarmonyPrefix]
		private static bool Prefix_BuffModel_OnRollOneCoinAfterAttack(BattleActionModel action, CoinModel coin, BATTLE_EVENT_TIMING timing, BuffModel __instance)
		{
			if (__instance.IsKeyword(BUFF_UNIQUE_KEYWORD.Laceration))
			{
				if (!action.IsDefense())
				{
					int stack = __instance.GetCurrentStack();
					int dmg = stack;
					if (!action.Model.IsFaction(UNIT_FACTION.PLAYER)) dmg *= 2;
					__instance._abilityList[0].TakeAbsHpDamage_Buff(action.Model, dmg, timing, DAMAGE_SOURCE_TYPE.BUFF, __instance._buffInfo, true, null, null);
					if (stack > 20) __instance.LoseStack(action.Model, 0, timing, 2);
					else if (stack > 1) __instance.LoseStack(action.Model, 0, timing, 1);
					__instance.LoseTurn(action.Model, timing, 1);
				}
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnTakeAttackDamage))]
		[HarmonyPrefix]
		private static void Prefix_BuffModel_OnTakeAttackDamage(BattleUnitModel unit, BATTLE_EVENT_TIMING timing, BuffModel __instance)
		{
			if (__instance.IsKeyword(BUFF_UNIQUE_KEYWORD.Sinking))
			{
				//int stack = __instance.GetCurrentStack();
				//int turn = __instance.GetCurrentTurn();

				//ChangeStat changeStat = unit._changeStat;
				//if (unit.Mp == 0 && (!changeStat.HasMp() || unit.Mp <= -45))
				//{
				//    __instance._abilityList[0].TakeHpDamageAppliedAttributeResist_Buff(unit, stack, ATTRIBUTE_TYPE.AZURE, timing, DAMAGE_SOURCE_TYPE.BUFF, __instance.GetBuffInfo(0, BATTLE_EVENT_TIMING.ALL_TIMING), true, null, null);
				//    unit.RecordAttackDamamgeByBuff(stack, __instance.GetMainKeyword(), true);
				//}
				//else
				//{
				//    __instance._abilityList[0].TakeAbsMpDamage_SinBuff(unit, turn, timing, __instance.GetBuffInfo(0, BATTLE_EVENT_TIMING.ALL_TIMING), DAMAGE_SOURCE_TYPE.BUFF, null, null);
				//}

				//__instance.LoseTurn(unit, timing, 1);

				int stack = __instance.GetCurrentStack();
				if (unit.Mp <= -45)
				{
					__instance._abilityList[0].TakeHpDamageAppliedAttributeResist_Buff(unit, stack, ATTRIBUTE_TYPE.AZURE, timing, DAMAGE_SOURCE_TYPE.BUFF, __instance.GetBuffInfo(0, BATTLE_EVENT_TIMING.ALL_TIMING), true, null, null);
					unit.RecordAttackDamamgeByBuff(stack, __instance.GetMainKeyword(), true);
				}
				if (stack > 1) __instance.LoseStack(unit, 0, timing, 1);
			}
		}

		[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnTakeAttackDamagePart))]
		[HarmonyPrefix]
		private static void Prefix_BuffModel_OnTakeAttackDamagePart(BattleUnitModel_Abnormality_Part part, BATTLE_EVENT_TIMING timing, BuffModel __instance)
		{
			if (__instance.IsKeyword(BUFF_UNIQUE_KEYWORD.Sinking))
			{
				BattleUnitModel_Abnormality abno = part.Abnormality;
				if (abno == null) return;

				int stack = __instance.GetCurrentStack();
				if (abno.Mp <= -45)
				{
					__instance._abilityList[0].TakeHpDamageAppliedAttributeResist_Buff(part, stack, ATTRIBUTE_TYPE.AZURE, timing, DAMAGE_SOURCE_TYPE.BUFF, __instance.GetBuffInfo(0, BATTLE_EVENT_TIMING.ALL_TIMING), true, null, null);
					part.RecordAttackDamamgeByBuff(stack, __instance.GetMainKeyword(), true);
				}
				if (stack > 1) __instance.LoseStack(part, 0, timing, 1);
			}
		}

		[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnTakeAttackDamage))]
		[HarmonyPostfix]
		private static void Postfix_BuffModel_OnTakeAttackDamage(BattleUnitModel unit, BATTLE_EVENT_TIMING timing, BuffModel __instance)
		{
			if (__instance.IsKeyword(BUFF_UNIQUE_KEYWORD.Burst))
			{
				int stack = __instance.GetCurrentStack();
				int extra = (int)System.Math.Floor(stack * 0.1);
				if (extra > 0) __instance.LoseTurn(unit, timing, extra);
			}
		}

		[HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnTakeAttackDamagePart))]
		[HarmonyPostfix]
		private static void Postfix_BuffModel_OnTakeAttackDamagePart(BattleUnitModel_Abnormality_Part part, BATTLE_EVENT_TIMING timing, BuffModel __instance)
		{
			if (__instance.IsKeyword(BUFF_UNIQUE_KEYWORD.Burst))
			{
				int stack = __instance.GetCurrentStack();
				int extra = (int)System.Math.Floor(stack * 0.1);
				if (extra > 0) __instance.LoseTurn(part, timing, extra);
			}
		}

		// end
	}
}
