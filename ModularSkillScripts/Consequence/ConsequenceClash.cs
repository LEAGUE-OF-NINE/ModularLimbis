namespace ModularSkillScripts.Consequence;

public class ConsequenceClash : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		modular.parryingResultAdder = modular.GetNumFromParamString(circledSection);
	}
}