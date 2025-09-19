namespace ModularSkillScripts.Acquirer;

public class AcquirerSkillCanDuel : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (circledSection == "Target") action = modular.modsa_oppoAction;
		if (action == null) return -1;
		return action.Skill.skillData.canDuel ? 1 : 0;
	}
}