namespace ModularSkillScripts.Acquirer;

public class AcquirerDeadAllies : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var targetModel = modular.GetTargetModel(circledSection);
		return targetModel?.deadAllyCount ?? -1;
	}
}