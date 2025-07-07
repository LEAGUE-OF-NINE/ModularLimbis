namespace ModularSkillScripts.Acquirer;

public class AcquirerActivations : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return modular.activationCounter;
	}
}