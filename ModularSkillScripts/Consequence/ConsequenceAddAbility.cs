using Il2CppSystem;

namespace ModularSkillScripts.Consequence;

public class ConsequenceAddAbility : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SYSTEM_ABILITY_KEYWORD ability = SYSTEM_ABILITY_KEYWORD.NONE;
		var modelList = modular.GetTargetModelList(circles[0]);
		Enum.TryParse(circles[1], true, out ability);
		int stack = modular.GetNumFromParamString(circles[2]);
		int turn = modular.GetNumFromParamString(circles[3]);
		int activeround = modular.GetNumFromParamString(circles[4]);
		foreach (BattleUnitModel targetModel in modelList)
		{
			if (activeround == 0)
			{
				SystemAbility addAbility =
					targetModel._systemAbilityDetail.FindOrAddAbilityThisRound(targetModel.InstanceID, ability, stack, turn);
			}
			else
			{
				SystemAbility addAbility =
					targetModel._systemAbilityDetail.FindOrAddAbilityNextRound(targetModel.InstanceID, ability, stack, turn);
			}
		}
	}
}