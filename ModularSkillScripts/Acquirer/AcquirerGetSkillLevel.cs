namespace ModularSkillScripts.Acquirer;

public class AcquirerGetSkillLevel : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel unit = modular.GetTargetModel(circles[0]);
		BattleActionModel action = modular.modsa_selfAction;
		if (circles[0] == "Target") action = modular.modsa_oppoAction;
		if (action == null) return -1;
		var offenseLevel = unit.GetSkillLevel(action.Skill, out _, out _);
		return offenseLevel;
	}
}