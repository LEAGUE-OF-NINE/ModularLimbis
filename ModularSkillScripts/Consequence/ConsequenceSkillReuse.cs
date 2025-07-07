namespace ModularSkillScripts.Consequence;

public class ConsequenceSkillReuse : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		foreach (BattleUnitModel targetModel in modular.GetTargetModelList(circledSection))
			targetModel.ReuseAction(modular.modsa_selfAction);
	}
}