namespace ModularSkillScripts.Acquirer;

public class AcquirerWave : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return Singleton<StageController>.Instance.GetCurrentWave();
	}
}