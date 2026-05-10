namespace ModularSkillScripts.Acquirer;

public class AcquirerSkillAttribute : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (circles[0] == "Target") action = modular.modsa_oppoAction;
		else if (circles[0] == "replaced" && action != null)
		{
			UnitSinModel replacedsin = action.SinAction.GetReplacedSinByDefenseSkill();
			if (replacedsin?.GetSkill() != null) return (int)replacedsin.GetSkill().GetAttributeType();
		}
		if (action == null) return -1;
		return (int)action.Skill.GetAttributeType();
	}
}