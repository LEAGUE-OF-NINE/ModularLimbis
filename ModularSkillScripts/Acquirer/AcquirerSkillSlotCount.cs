namespace ModularSkillScripts.Acquirer;

public class AcquirerSkillSlotCount : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		if (targetModel == null) return -1;
		if (circles.Length > 1) return targetModel.GetPermanentSinActionListCount();
		return targetModel.GetSinActionListCount();
	}
}
