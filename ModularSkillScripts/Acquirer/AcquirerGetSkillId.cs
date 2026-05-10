namespace ModularSkillScripts.Acquirer;

public class AcquirerGetSkillId : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (circles.Length > 0 && circles[0] == "replaced" && action != null)
		{
			UnitSinModel replacedsin = action.SinAction.GetReplacedSinByDefenseSkill();
			if (replacedsin != null) return replacedsin.SkillID;
		}
		if (modular.modsa_skillModel != null) return modular.modsa_skillModel.GetID();
		if (action != null) return action.Skill.GetID();
		
		return 0;
	}
}