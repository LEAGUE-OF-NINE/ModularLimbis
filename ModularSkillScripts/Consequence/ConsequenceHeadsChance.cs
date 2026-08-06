namespace ModularSkillScripts.Consequence;

public class ConsequenceHeadsChance : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		modular.headsChanceAdder = modular.GetNumFromParamString(circledSection); 
	}
}