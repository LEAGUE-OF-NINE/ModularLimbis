namespace ModularSkillScripts.Acquirer;

public class AcquirerIsCrit : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return modular.wasCrit ? 1 : 0;
	}
}