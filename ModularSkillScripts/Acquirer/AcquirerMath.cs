namespace ModularSkillScripts.Acquirer;

public class AcquirerMath : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return modular.DoMath(circledSection);
	}
}