namespace ModularSkillScripts.Consequence;

public class ConsequenceAtkWeight : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		modular.atkWeightAdder = modular.GetNumFromParamString(circledSection);
	}
}