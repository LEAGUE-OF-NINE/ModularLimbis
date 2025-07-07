namespace ModularSkillScripts.Acquirer;

public class AcquirerInstId : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return modular.GetTargetModel(circledSection).InstanceID;
	}
}