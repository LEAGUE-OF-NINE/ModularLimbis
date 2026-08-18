namespace ModularSkillScripts.Acquirer;

public class AcquirerIsIgnoreDefense : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_selfAction == null) return -1;
		BattleUnitModel targetUnit = modular.GetTargetModel(circles[0]);
		if (targetUnit == null) return -1;
		
		return modular.modsa_selfAction.IgnoreDefenseSkill(targetUnit) ? 1 : 0;
	}
}