namespace ModularSkillScripts.Consequence;

public class ConsequenceChangeSkill : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		modular.modsa_selfAction?.TryChangeSkill(modular.GetNumFromParamString(circledSection));
	}
}