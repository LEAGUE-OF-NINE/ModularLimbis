namespace ModularSkillScripts.Consequence;

public class ConsequenceSetChangeDamageTaken : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int amount = modular.GetNumFromParamString(circledSection);
		modular.ischangedamagetaken = true;
		modular.changedamagetaken = modular.GetNumFromParamString(circledSection);
	}
}