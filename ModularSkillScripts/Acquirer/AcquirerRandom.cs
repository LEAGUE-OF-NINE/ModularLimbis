namespace ModularSkillScripts.Acquirer;

public class AcquirerRandom : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int minRoll = modular.GetNumFromParamString(circles[0]);
		int maxRoll = modular.GetNumFromParamString(circles[1]);
		return MainClass.rng.Next(minRoll, maxRoll + 1);
	}
}