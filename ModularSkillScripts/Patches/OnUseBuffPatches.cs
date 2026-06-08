using HarmonyLib;

namespace ModularSkillScripts.Patches;

public class OnUseBuffPatches
{
	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnUseBuff))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_RightAfterGetAnyBuffMT(BUFF_UNIQUE_KEYWORD keyword, int stack, int turn, BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
	{
		int actevent = MainClass.timingDict["OnUseBuff"];
		foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;
			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
			{
				BUFF_UNIQUE_KEYWORD trigger = modpa.keywordTrigger;
				if ((trigger != BUFF_UNIQUE_KEYWORD.None) && (trigger != keyword)) continue;

				modpa.modsa_passiveModel = passiveModel;
				modpa.gainbuff_stack = stack;
				modpa.gainbuff_turn = turn;
				modpa.Enact(__instance, null, null, null, actevent, timing);
			}
		}

		foreach (PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
			{
				BUFF_UNIQUE_KEYWORD trigger = modpa.keywordTrigger;
				if ((trigger != BUFF_UNIQUE_KEYWORD.None) && (trigger != keyword)) continue;

				modpa.modsa_passiveModel = passiveModel;
				modpa.gainbuff_stack = stack;
				modpa.gainbuff_turn = turn;
				modpa.Enact(__instance, null, null, null, actevent, timing);
			}
		}
	}
}
