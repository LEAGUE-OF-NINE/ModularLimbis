namespace ModularSkillScripts.Acquirer;

public class AcquirerUnitState : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circledSection);
		if (targetModel == null) return -1;
		if (targetModel.IsDead()) return 0;
		if (targetModel.IsBreak()) return 2;
		var abnoPart = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
		if (abnoPart != null && !abnoPart.IsActionable()) return 2;
		return 1;
	}
}