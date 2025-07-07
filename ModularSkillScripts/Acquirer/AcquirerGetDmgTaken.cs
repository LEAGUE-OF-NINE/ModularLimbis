namespace ModularSkillScripts.Acquirer;

public class AcquirerGetDmgTaken : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		var turntime = circles[1];
		if (targetModel == null) return -1;
		return turntime == "prev" ? targetModel.GetHitAttackDamagePrevRound() : targetModel.GetHitAttackDamageThisRound();
	}
}