namespace ModularSkillScripts.Consequence;

public class ConsequenceLog : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		MainClass.Logg.LogInfo("ModularLog " + circles[0] + ": " + modular.GetNumFromParamString(circles[1]));
	}
}