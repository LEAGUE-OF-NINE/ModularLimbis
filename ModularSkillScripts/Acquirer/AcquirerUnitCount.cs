namespace ModularSkillScripts.Acquirer;

public class AcquirerUnitCount : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return modular.GetTargetModelList(circledSection).Count;
	}
}