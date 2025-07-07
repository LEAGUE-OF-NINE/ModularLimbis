namespace ModularSkillScripts.Acquirer;

public class AcquirerSkillAtkLevel : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (circles[0] == "Target") action = modular.modsa_oppoAction;
		if (action == null) return -1;
		return action.Skill.GetSkillLevelCorrection();
	}
}