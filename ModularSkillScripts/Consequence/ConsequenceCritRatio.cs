namespace ModularSkillScripts.Consequence;

public class ConsequenceCritRatio : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		modular.critRatioAdder = modular.GetNumFromParamString(circledSection);
	}
}