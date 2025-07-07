namespace ModularSkillScripts.Acquirer;

public class AcquirerSkillRank : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (circles[0] == "Target") action = modular.modsa_oppoAction;
		if (action != null) return action.Skill.GetSkillTier();
		return -1;
	}
}