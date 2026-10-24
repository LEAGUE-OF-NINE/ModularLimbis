using System;
using Il2CppSystem.Collections.Generic;
using Lethe.EnumInjections;
using Lethe.Patches;
using CustomBuffs = Lethe.EnumInjections.CustomBuffs;

namespace ModularSkillScripts.Consequence;

public class ConsequenceBufTwo : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		List<BattleUnitModel> modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return;

		string giver_s = circles[1];
		BattleUnitModel giver = modular.GetTargetModel(giver_s);

		string mode_s = circles[2];

		BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[3]);
		int stack = modular.GetNumFromParamString(circles[4]);
		int turn = modular.GetNumFromParamString(circles[5]);
		
		string activeRound_s = circles[6];
		List<int> activeRound_list = new();
		string[] activeRound_s_list = activeRound_s.Split('+', StringSplitOptions.RemoveEmptyEntries);
		foreach (string s in activeRound_s_list) {
			int activeRound = modular.GetNumFromParamString(s);
			activeRound_list.Add(activeRound);
		}


		bool use = false;
		BattleActionModel action = modular.modsa_selfAction;
		
		for (int i = 7; i < circles.Length; i++)
		{
			string circle_cur = circles[i];
			if (circle_cur == "use") use = true;
			else if (circle_cur == "nullaction") action = null;
		}
		
		BATTLE_EVENT_TIMING timing = modular.battleTiming;

		foreach (BattleUnitModel targetModel in modelList)
		{
			int stack_temp = stack;
			int turn_temp = turn;
			if (stack_temp < 0)
			{
				switch (mode_s)
				{
					case "skill": {
						if (use) modular.dummySkillAbility.UseBuffStack(targetModel, action, buf_keyword, timing, stack_temp * -1);
						else {
							foreach (int activeRound in activeRound_list) {
								targetModel.LoseBuffStack(buf_keyword, stack_temp * -1, modular.battleTiming, activeRound);
							}
						}
					} break;
					case "buf": {
						int bufmode_loseStack = stack_temp * -1;
						int bufmode_loseTurn = turn_temp < 0 ? turn_temp * -1 : 0;
						modular.modsa_buffModel._abilityList[0].LoseBuff_ByBuff(targetModel, giver, bufmode_loseStack, bufmode_loseTurn, timing, modular.modsa_buffModel._buffInfo);
						turn_temp = 0;
					} break;
					case "passive":
					case "unit": {
						if (use) targetModel.UseBuffStack(buf_keyword, stack_temp * -1, modular.battleTiming);
						else {
							foreach (int activeRound in activeRound_list) {
								targetModel.LoseBuffStack(buf_keyword, stack_temp * -1, modular.battleTiming, activeRound);
							}
						}
					} break;
				}
				stack_temp = 0;
			}

			if (turn_temp < 0)
			{
				switch (mode_s)
				{
					case "skill": {
						if (use) modular.dummySkillAbility.UseBuffTurn(targetModel, action, buf_keyword, timing, turn_temp * -1);
						else targetModel.LoseBuffTurn(buf_keyword, turn_temp * -1, modular.battleTiming);
					} break;
					case "buf": {
						modular.modsa_buffModel._abilityList[0].LoseBuff_ByBuff(targetModel, giver, 0, turn_temp * -1, timing, modular.modsa_buffModel._buffInfo);
					} break;
					case "passive": {
						if (use) modular.dummyPassiveAbility.UseBuffTurnByPassive(targetModel, buf_keyword, turn_temp * -1, timing);
						else targetModel.LoseBuffTurn(buf_keyword, turn_temp * -1, modular.battleTiming);
					} break;
					case "unit": {
						if (use) targetModel.UseBuffTurn(buf_keyword, turn_temp * -1, modular.battleTiming);
						else targetModel.LoseBuffTurn(buf_keyword, turn_temp * -1, modular.battleTiming);
					} break;
				}
				
				turn_temp = 0;
			}

			if (stack_temp <= 0 && turn_temp <= 0) continue;
			switch (mode_s)
			{
				case "skill": {
					foreach (int activeRound in activeRound_list) {
						modular.dummySkillAbility.GiveBuff_BySkill(targetModel, action, buf_keyword, stack_temp, turn_temp, activeRound, timing, modular.modsa_coinModel);
					}
				} break;
				case "passive": {
					foreach (int activeRound in activeRound_list) {
						modular.dummyPassiveAbility.GiveBuff_Giver(targetModel, buf_keyword, giver, stack_temp, turn_temp, activeRound, timing, action);
					}
				} break;
				case "buf": {
					foreach (int activeRound in activeRound_list) {
						modular.modsa_buffModel._abilityList[0].GiveBuff_Giver(targetModel, buf_keyword, giver, stack_temp, turn_temp, activeRound, timing, action);
					}
				} break;
				case "unit": {
					foreach (int activeRound in activeRound_list) {
						targetModel.AddBuff_Giver(buf_keyword, stack_temp, giver, timing, turn_temp, activeRound,
							ABILITY_SOURCE_TYPE.UNIT, action,
							out _, out _, out _, out _);
					}
				} break;
			}
			
		}
	}
}