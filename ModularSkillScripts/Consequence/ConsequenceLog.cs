namespace ModularSkillScripts.Consequence;

public class ConsequenceLog : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		switch (circles.Length)
		{
			case 0:
				MainClass.LogModular("ModularLog");
				break;
			case 1:
				MainClass.LogModular("ModularLog " + circles[0]);
				break;
			default:
				MainClass.LogModular("ModularLog " + circles[0] + ": " + modular.GetNumFromParamString(circles[1]));
				break;
		}
	}
}