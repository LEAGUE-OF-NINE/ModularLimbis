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
		foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel)) {
				//MainClass.LogModular("Founds modpassive - GainBuff timing: " + modpa.passiveID);
				//MainClass.LogModular("Triggered Keyword: " + trigger);
				if (modpa.activationTiming != actevent) continue;
				BUFF_UNIQUE_KEYWORD trigger = modpa.keywordTrigger;
				if (trigger != BUFF_UNIQUE_KEYWORD.None && trigger != keyword) continue;
				modpa.modsa_passiveModel = passiveModel;
				modpa.gainbuff_stack = stack;
				modpa.gainbuff_turn = turn;
				modpa.gainbuff_activeRound = activeRound;
				modpa.gainbuff_source = srcType;
				modpa.Enact(__instance, null, null, actionOrNull, actevent, timing);
			}
		}

		foreach (EgoPassiveModel egoPassiveModel in __instance._passiveDetail.EgoPassiveList)
		{
			foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(egoPassiveModel, false)) {
				//MainClass.LogModular("Founds modpassive - GainBuff timing: " + modpa.passiveID);
				//MainClass.LogModular("Triggered Keyword: " + trigger);
				if (modpa.activationTiming != actevent) continue;
				BUFF_UNIQUE_KEYWORD trigger = modpa.keywordTrigger;
				if (trigger != BUFF_UNIQUE_KEYWORD.None && trigger != keyword) continue;
				modpa.modsa_passiveModel = egoPassiveModel;
				modpa.gainbuff_stack = stack;
				modpa.gainbuff_turn = turn;
				modpa.gainbuff_activeRound = activeRound;
				modpa.gainbuff_source = srcType;
				modpa.Enact(__instance, null, null, actionOrNull, actevent, timing);
			}
		}
		
	}
}