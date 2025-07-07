namespace ModularSkillScripts.Acquirer;

public class AcquirerGetDmg : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return modular.lastFinalDmg;
	}
}