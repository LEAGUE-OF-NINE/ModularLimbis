using BattleUI.Operation;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using BepInEx.Unity.IL2CPP.UnityEngine;
using ModularSkillScripts;

namespace ModularSkillScripts.Patches;

internal class OnGainBuffPatches
{
	[HarmonyPatch(typeof(BattleUnitModel),nameof(BattleUnitModel.RightAfterGetAnyBuff))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_RightAfterGetAnyBuff( BUFF_UNIQUE_KEYWORD keyword, int stack, int turn, int activeRound, ABILITY_SOURCE_TYPE srcType, BATTLE_EVENT_TIMING timing, BattleUnitModel giverOrNull, BattleActionModel actionOrNull, int overStack, int overTurn, BattleUnitModel __instance)
	{
		int actevent = MainClass.timingDict["OnGainBuff"];
		if (__instance.IsActionable())
		{
			bool noSkip = true;
			foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
			{
				if (!passiveModel.CheckActiveCondition())
					continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong))
					continue;

				foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
				{
					BUFF_UNIQUE_KEYWORD trigger = modpa.keywordTrigger;
					if ((trigger != BUFF_UNIQUE_KEYWORD.None) && (trigger != keyword))
						continue;
					MainClass.Logg.LogInfo("Founds modpassive - GainBuff timing: " + modpa.passiveID);
					MainClass.Logg.LogInfo("Triggered Keyword: " + trigger);
					noSkip = false;
					modpa.modsa_passiveModel = passiveModel;
					modpa.gainbuff_keyword = keyword;
					modpa.gainbuff_stack = stack;
					modpa.gainbuff_turn = turn;
					modpa.gainbuff_activeRound = activeRound;
					modpa.gainbuff_source = srcType;
					modpa.Enact(__instance, null, null, null, actevent, timing);
				}
			}

			foreach (PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList)
			{
				if (!passiveModel.CheckActiveCondition())
					continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong))
					continue;

				foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
				{
					BUFF_UNIQUE_KEYWORD trigger = modpa.keywordTrigger;
					if ((trigger != BUFF_UNIQUE_KEYWORD.None) && (trigger != keyword))
						continue;

					MainClass.Logg.LogInfo("Founds modpassive - GainBuff timing: " + modpa.passiveID);
					MainClass.Logg.LogInfo("Triggered Keyword: " + trigger);

					noSkip = false;

					modpa.modsa_passiveModel = passiveModel;
					modpa.gainbuff_keyword = keyword;
					modpa.gainbuff_stack = stack;
					modpa.gainbuff_turn = turn;
					modpa.gainbuff_activeRound = activeRound;
					modpa.gainbuff_source = srcType;
					modpa.Enact(__instance, null, null, null, actevent, timing);
				}
			}
		}
	}
}