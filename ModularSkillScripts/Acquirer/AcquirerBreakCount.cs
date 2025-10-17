namespace ModularSkillScripts.Acquirer;

public class AcquirerBreakCount : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int count = 0;
		var targetModel = modular.GetTargetModel(circledSection);
		var breakSections = targetModel.GetBreakSections();
		foreach (BreakSection breakSection in breakSections)
		{
			if (breakSection.IsActive)
			{
				count += 1;
			}
		}
		return count;
	}
}