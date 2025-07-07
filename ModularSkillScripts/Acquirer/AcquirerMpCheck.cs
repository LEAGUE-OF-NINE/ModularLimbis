namespace ModularSkillScripts.Acquirer;

public class AcquirerMpCheck : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circledSection);
		if (targetModel == null) return -1;
		return targetModel.Mp;
	}
}