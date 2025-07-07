using Il2CppSystem;

namespace ModularSkillScripts.Consequence;

public class ConsequenceRemoveAbility : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SYSTEM_ABILITY_KEYWORD ability;
		var modelList = modular.GetTargetModelList(circles[0]);
		Enum.TryParse(circles[1], true, out ability);
		foreach (BattleUnitModel targetModel in modelList)
		{
			targetModel._systemAbilityDetail.DestoryAbility(ability);
		}
	}
}