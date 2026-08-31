using BattleUI.Operation;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using BepInEx.Unity.IL2CPP.UnityEngine;
using ModularSkillScripts;
using Utils;

namespace ModularSkillScripts.Patches;

internal class OnGainBuffPatches
{
	public static BUFF_UNIQUE_KEYWORD ongainbuf_keyword = BUFF_UNIQUE_KEYWORD.None;
	
	[HarmonyPatch(typeof(BattleUnitModel),nameof(BattleUnitModel.RightAfterGetAnyBuff))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_RightAfterGetAnyBuff( BUFF_UNIQUE_KEYWORD keyword, int stack, int turn, int activeRound, ABILITY_SOURCE_TYPE srcType, BATTLE_EVENT_TIMING timing, BattleUnitModel giverOrNull, BattleActionModel actionOrNull, int overStack, int overTurn, BattleUnitModel __instance)
	{
		ongainbuf_keyword = keyword;
		int actevent = MainClass.timingDict["OnGainBuff"];
		foreach (PassiveModel passiveModel in __instance._passiveDetail._passivelist.CopyList()) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel)) {
				if (modsa.activationTiming != actevent) continue;
				BUFF_UNIQUE_KEYWORD trigger = modsa.keywordTrigger;
				if (trigger != BUFF_UNIQUE_KEYWORD.None && trigger != keyword) continue;
				modsa.modsa_passiveModel = passiveModel;
				modsa.gainbuff_stack = stack;
				modsa.gainbuff_turn = turn;
				modsa.gainbuff_activeRound = activeRound;
				modsa.gainbuff_source = srcType;
				modsa.Enact(__instance, null, actionOrNull, null, actevent, timing);
			}
		}

		foreach (EgoPassiveModel egoPassiveModel in __instance._passiveDetail._egoPassiveList.CopyList()) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel(egoPassiveModel, false)) {
				if (modsa.activationTiming != actevent) continue;
				BUFF_UNIQUE_KEYWORD trigger = modsa.keywordTrigger;
				if (trigger != BUFF_UNIQUE_KEYWORD.None && trigger != keyword) continue;
				modsa.modsa_passiveModel = egoPassiveModel;
				modsa.gainbuff_stack = stack;
				modsa.gainbuff_turn = turn;
				modsa.gainbuff_activeRound = activeRound;
				modsa.gainbuff_source = srcType;
				modsa.Enact(__instance, null, actionOrNull, null, actevent, timing);
			}
		}
		
	}
}