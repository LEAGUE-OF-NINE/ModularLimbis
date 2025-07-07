namespace ModularSkillScripts.Acquirer;

public class AcquirerBreakCount : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return modular.GetTargetModel(circledSection).GetBreakSections().Count;
	}
}