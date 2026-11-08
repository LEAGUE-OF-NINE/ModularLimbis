namespace ModularSkillScripts.Acquirer;

public class AcquirerHasEquippedFacade : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitView unitView = SingletonBehavior<BattleObjectManager>.Instance.GetView(modular.modsa_unitModel);
		if (unitView._skinData != null)
		{
			return 1;
		}
		return 0;
	}
}