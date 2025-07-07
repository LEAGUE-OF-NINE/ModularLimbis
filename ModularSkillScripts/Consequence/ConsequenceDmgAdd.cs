namespace ModularSkillScripts.Consequence;

public class ConsequenceDmgAdd : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		modular.atkDmgAdder = modular.GetNumFromParamString(circledSection); 
	}
}