namespace ModularSkillScripts.Acquirer;

public class AcquirerSkillDefType : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (circles[0] == "Target") action = modular.modsa_oppoAction;
		if (action != null) return (int)action.Skill.GetDefenseType();
		return -1;
	}
}