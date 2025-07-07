namespace ModularSkillScripts.Acquirer;

public class AcquirerAmountAttacks : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SinManager sinManager_inst = Singleton<SinManager>.Instance;
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		if (targetModel == null || sinManager_inst == null) return -1;
		return sinManager_inst.GetActionListTargetingUnit(targetModel).Count;
	}
}