namespace ModularSkillScripts.Consequence;

public class ConsequenceIgnoreBreak : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int amount = modular.GetNumFromParamString(circledSection);
		modular.ignorebreak = amount > 0;
	}
}