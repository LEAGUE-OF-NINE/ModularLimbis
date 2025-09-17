namespace ModularSkillScripts.Consequence;

public class ConsequenceIgnorePanic : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int amount = modular.GetNumFromParamString(circledSection);
		modular.ignorepanic = amount > 0;
	}
}