namespace ModularSkillScripts.Acquirer;

public class AcquirerGetBuffCount : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		var type = circles[1];
		if (targetModel == null) return -1;
		return type == "neg" ? targetModel.GetNegativeBuffCount() : targetModel.GetPositiveBuffCount();
	}
}