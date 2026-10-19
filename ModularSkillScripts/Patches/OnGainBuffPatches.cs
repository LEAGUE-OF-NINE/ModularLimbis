using BattleUI.Operation;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using BepInEx.Unity.IL2CPP.UnityEngine;
using ModularSkillScripts;
using Utils;

namespace ModularSkillScripts.Patches;

internal class OnGainBuffPatches
{
	[HarmonyPatch(typeof(BattleUnitModel),nameof(BattleUnitModel.CheckOverwriteGettingBuff))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_CheckOverwriteGettingBuff(
		ABILITY_SOURCE_TYPE abilitySrcType,
		BATTLE_EVENT_TIMING timing,
		BattleUnitModel giverOrNull,
		ref BUFF_UNIQUE_KEYWORD keyword,
		ref int stack,
		ref int turn,
		ref int activeRound,
		BattleUnitModel __instance)
	{
		int key_int = (int)keyword;
		int srcType_int = (int)abilitySrcType;

		BattleUnitModel gainer = __instance;
		BattleUnitModel inflicter = giverOrNull;

		if (inflicter != null)
		{
			int actevent = MainClass.timingDict["OverwriteGiveBuf"];
			foreach (PassiveModel passiveModel in inflicter._passiveDetail._passivelist.CopyList()) {
				foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel)) {
					if (modsa.activationTiming != actevent) continue;
					modsa.valueList[6] = srcType_int;
					modsa.valueList[7] = stack;
					modsa.valueList[8] = turn;
					modsa.valueList[9] = key_int;
					modsa.modsa_victimModel = gainer;
					modsa.modsa_passiveModel = passiveModel;
					modsa.Enact(inflicter, null, null, null, actevent, timing);
					if (modsa.valueList[9] != key_int) {
						key_int = modsa.valueList[9];
						keyword = (BUFF_UNIQUE_KEYWORD)key_int;
					}
					stack = modsa.valueList[7];
					turn = modsa.valueList[8];
				}
			}

			foreach (EgoPassiveModel egoPassiveModel in inflicter._passiveDetail._egoPassiveList.CopyList()) {
				foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel(egoPassiveModel, false)) {
					if (modsa.activationTiming != actevent) continue;
					modsa.valueList[6] = srcType_int;
					modsa.valueList[7] = stack;
					modsa.valueList[8] = turn;
					modsa.valueList[9] = key_int;
					modsa.gainbuff_source = abilitySrcType;
					modsa.modsa_victimModel = gainer;
					modsa.modsa_passiveModel = egoPassiveModel;
					modsa.Enact(inflicter, null, null, null, actevent, timing);
					if (modsa.valueList[9] != key_int) {
						key_int = modsa.valueList[9];
						keyword = (BUFF_UNIQUE_KEYWORD)key_int;
					}
					stack = modsa.valueList[7];
					turn = modsa.valueList[8];
				}
			}
		}

		if (gainer != null) {
			int actevent = MainClass.timingDict["OverwriteGainBuf"];
			foreach (PassiveModel passiveModel in gainer._passiveDetail._passivelist.CopyList()) {
				foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel)) {
					if (modsa.activationTiming != actevent) continue;
					modsa.valueList[6] = srcType_int;
					modsa.valueList[7] = stack;
					modsa.valueList[8] = turn;
					modsa.valueList[9] = key_int;
					modsa.modsa_killerModel = inflicter;
					modsa.modsa_passiveModel = passiveModel;
					modsa.Enact(gainer, null, null, null, actevent, timing);
					if (modsa.valueList[9] != key_int) {
						key_int = modsa.valueList[9];
						keyword = (BUFF_UNIQUE_KEYWORD)key_int;
					}
					stack = modsa.valueList[7];
					turn = modsa.valueList[8];
				}
			}

			foreach (EgoPassiveModel egoPassiveModel in gainer._passiveDetail._egoPassiveList.CopyList()) {
				foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel(egoPassiveModel, false)) {
					if (modsa.activationTiming != actevent) continue;
					modsa.valueList[6] = srcType_int;
					modsa.valueList[7] = stack;
					modsa.valueList[8] = turn;
					modsa.valueList[9] = key_int;
					modsa.modsa_killerModel = inflicter;
					modsa.modsa_passiveModel = egoPassiveModel;
					modsa.Enact(gainer, null, null, null, actevent, timing);
					if (modsa.valueList[9] != key_int) {
						key_int = modsa.valueList[9];
						keyword = (BUFF_UNIQUE_KEYWORD)key_int;
					}
					stack = modsa.valueList[7];
					turn = modsa.valueList[8];
				}
			}
		}
	}
	
	/*
	[HarmonyPatch(typeof(BattleUnitModel),nameof(BattleUnitModel.CheckOverwriteGivingBuff))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_CheckOverwriteGivingBuff(
		ABILITY_SOURCE_TYPE abilitySrcType,
		BATTLE_EVENT_TIMING timing,
		BattleUnitModel target,
		ref BUFF_UNIQUE_KEYWORD keyword,
		ref int stack,
		ref int turn,
		ref int activeRound,
		BattleUnitModel __instance)
	{
		int key_int = (int)keyword;
		int srcType_int = (int)abilitySrcType;
		
		int actevent = MainClass.timingDict["OverwriteGiveBuf"];
		foreach (PassiveModel passiveModel in __instance._passiveDetail._passivelist.CopyList()) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel)) {
				if (modsa.activationTiming != actevent) continue;
				modsa.valueList[6] = srcType_int;
				modsa.valueList[7] = stack;
				modsa.valueList[8] = turn;
				modsa.valueList[9] = key_int;
				modsa.modsa_victimModel = target;
				modsa.modsa_passiveModel = passiveModel;
				modsa.Enact(__instance, null, null, null, actevent, timing);
				if (modsa.valueList[9] != key_int) {
					key_int = modsa.valueList[9];
					keyword = (BUFF_UNIQUE_KEYWORD)key_int;
				}
				stack = modsa.valueList[7];
				turn = modsa.valueList[8];
			}
		}

		foreach (EgoPassiveModel egoPassiveModel in __instance._passiveDetail._egoPassiveList.CopyList()) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel(egoPassiveModel, false)) {
				if (modsa.activationTiming != actevent) continue;
				modsa.valueList[6] = srcType_int;
				modsa.valueList[7] = stack;
				modsa.valueList[8] = turn;
				modsa.valueList[9] = key_int;
				modsa.gainbuff_source = abilitySrcType;
				modsa.modsa_killerModel = target;
				modsa.modsa_passiveModel = egoPassiveModel;
				modsa.Enact(__instance, null, null, null, actevent, timing);
				if (modsa.valueList[9] != key_int) {
					key_int = modsa.valueList[9];
					keyword = (BUFF_UNIQUE_KEYWORD)key_int;
				}
				stack = modsa.valueList[7];
				turn = modsa.valueList[8];
			}
		}
	}*/
	
	
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