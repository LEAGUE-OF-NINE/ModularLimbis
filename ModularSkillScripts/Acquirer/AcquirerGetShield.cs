namespace ModularSkillScripts.Acquirer;

public class AcquirerGetShield : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		if (targetModel != null) return targetModel.GetShield();
		return -1;
	}
}