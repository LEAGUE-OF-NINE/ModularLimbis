namespace ModularSkillScripts.Acquirer;

public class AcquirerRound : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return Singleton<StageController>.Instance.GetCurrentRound();
	}
}