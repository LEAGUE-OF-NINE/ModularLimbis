namespace ModularSkillScripts.Acquirer;

public class AcquirerSkillTeamKill : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (circledSection == "Target") action = modular.modsa_oppoAction;
		if (action == null) return -1;
		return action.Skill.skillData.canTeamKill ? 1 : 0;
	}
}