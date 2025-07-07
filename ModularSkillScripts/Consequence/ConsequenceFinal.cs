namespace ModularSkillScripts.Consequence;

public class ConsequenceFinal : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		modular.skillPowerResultAdder = modular.GetNumFromParamString(circledSection);
	}
}