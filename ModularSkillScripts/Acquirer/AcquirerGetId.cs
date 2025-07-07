namespace ModularSkillScripts.Acquirer;

public class AcquirerGetId : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return modular.GetTargetModel(circledSection).GetUnitID();
	}
}