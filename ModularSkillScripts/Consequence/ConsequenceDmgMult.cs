namespace ModularSkillScripts.Consequence;

public class ConsequenceDmgMult : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		modular.atkMultAdder = modular.GetNumFromParamString(circledSection);
	}
}