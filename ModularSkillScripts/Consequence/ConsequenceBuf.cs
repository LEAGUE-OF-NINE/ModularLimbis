using Lethe.Patches;

namespace ModularSkillScripts.Consequence;

public class ConsequenceBuf : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return;

		BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
		int stack = modular.GetNumFromParamString(circles[2]);
		int turn = modular.GetNumFromParamString(circles[3]);
		int activeRound = modular.GetNumFromParamString(circles[4]);
		bool use = circles.Length >= 6;

		foreach (BattleUnitModel targetModel in modelList)
		{
			int stack_temp = stack;
			int turn_temp = turn;
			if (stack_temp < 0)
			{
				if (use) targetModel.UseBuffStack(buf_keyword, stack_temp * -1, modular.battleTiming);
				else targetModel.LoseBuffStack(buf_keyword, stack_temp * -1, modular.battleTiming, activeRound);
				stack_temp = 0;
			}

			if (turn_temp < 0)
			{
				if (use) targetModel.UseBuffTurn(buf_keyword, turn_temp * -1, modular.battleTiming);
				else targetModel.LoseBuffTurn(buf_keyword, turn_temp * -1, modular.battleTiming);
				turn_temp = 0;
			}

			if (stack_temp <= 0 && turn_temp <= 0) continue;
			//AbilityTriggeredData_GiveBuff triggerData = new AbilityTriggeredData_GiveBuff(buf_keyword, stack_temp, turn_temp, activeRound, false, true, targetModel.InstanceID, battleTiming, BUF_TYPE.Neutral);
			if (activeRound == 2)
			{
				switch (modular.abilityMode)
				{
					case 2:
						modular.dummyPassiveAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, 0, modular.battleTiming,
							modular.modsa_selfAction);
						modular.dummyPassiveAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, 1, modular.battleTiming,
							modular.modsa_selfAction);
						break;
					case 1:
						modular.dummyCoinAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, 0, modular.battleTiming,
							modular.modsa_selfAction);
						modular.dummyCoinAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, 1, modular.battleTiming,
							modular.modsa_selfAction);
						break;
					default:
						modular.dummySkillAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, 0, modular.battleTiming,
							modular.modsa_selfAction);
						modular.dummySkillAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, 1, modular.battleTiming,
							modular.modsa_selfAction);
						break;
				}
			}
			else
			{
				switch (modular.abilityMode)
				{
					case 2:
						modular.dummyPassiveAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, activeRound,
							modular.battleTiming, modular.modsa_selfAction);
						break;
					case 1:
						modular.dummyCoinAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, activeRound, modular.battleTiming,
							modular.modsa_selfAction);
						break;
					default:
						modular.dummySkillAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, activeRound, modular.battleTiming,
							modular.modsa_selfAction);
						break;
				}
			}
		}
	}
}