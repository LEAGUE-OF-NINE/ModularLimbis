namespace ModularSkillScripts.Consequence;

public class ConsequenceBase : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		modular.skillPowerAdder = modular.GetNumFromParamString(circledSection);
	}
}