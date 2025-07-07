namespace ModularSkillScripts.Consequence;

public class ConsequenceSetImmortal : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int amount = modular.GetNumFromParamString(circledSection);
		modular.immortality = amount > 0;
	}
}