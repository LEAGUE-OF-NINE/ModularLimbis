namespace ModularSkillScripts.Acquirer;

public class AcquirerBreakValue : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var sectionlist = modular.GetTargetModel(circles[0]).GetBreakSections();
		return sectionlist[modular.GetNumFromParamString(circles[1])].HP;
	}
}