namespace ModularSkillScripts.Acquirer;

public class AcquirerSameUnit : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel1 = modular.GetTargetModel(circles[0]);
		BattleUnitModel targetModel2 = modular.GetTargetModel(circles[1]);
		if (targetModel1 == null || targetModel2 == null) return -1;
		return targetModel1.InstanceID == targetModel2.InstanceID ? 1 : 0;
	}
}