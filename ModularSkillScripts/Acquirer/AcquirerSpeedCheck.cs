namespace ModularSkillScripts.Acquirer;

public class AcquirerSpeedCheck : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circledSection);
		return targetModel?.GetIntegerOfOriginSpeed() ?? 0;
	}
}