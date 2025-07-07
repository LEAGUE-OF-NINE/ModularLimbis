namespace ModularSkillScripts.Acquirer;

public class AcquirerGetHpDmg : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return modular.lastHpDmg;
	}
}