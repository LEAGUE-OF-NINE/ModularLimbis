namespace ModularSkillScripts.Acquirer;

public class AcquirerIsFocused : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return Singleton<StageController>.Instance.IsAbnormalityBattle() ? 1 : 0;
	}
}