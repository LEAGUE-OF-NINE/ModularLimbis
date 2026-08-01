namespace ModularSkillScripts.Acquirer;

public class AcquirerUnitState : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		if (targetModel == null) return -1;
		
		if (circles.Length > 1)
		{
			if (targetModel.IsDead()) return -1;
			if (targetModel.IsBreak()) return targetModel.IsForcelyBreak() ? 2 : 1;
			return 0;
		}
		
		if (targetModel.IsDead()) return 0;
		if (targetModel.IsBreak()) return 2;
		var abnoPart = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
		if (abnoPart != null && !abnoPart.IsActionable()) return 2;
		return 1;
	}
}