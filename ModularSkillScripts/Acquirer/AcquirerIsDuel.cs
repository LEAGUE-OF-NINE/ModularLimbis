namespace ModularSkillScripts.Acquirer;

public class AcquirerIsDuel : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		BattleActionModelManager actionManager = BattleActionModelManager.Instance;
		if (modular.modsa_selfAction == null || actionManager == null) return -1;

		return actionManager.IsDuel(action) ? 1 : 0;
	}
}