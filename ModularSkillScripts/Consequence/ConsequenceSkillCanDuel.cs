namespace ModularSkillScripts.Consequence;

public class ConsequenceSkillCanDuel : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		modular.modsa_skillModel.OverrideCanDuel(circledSection == "True");
	}
}