namespace ModularSkillScripts.Consequence;

public class ConsequenceCritChance : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		modular.critAdder = modular.GetNumFromParamString(circledSection); 
	}
}