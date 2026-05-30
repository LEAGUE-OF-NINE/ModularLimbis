namespace ModularSkillScripts.Acquirer;

public class AcquirerSkillEgoType : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SkillModel skill = null;
		BattleActionModel action = modular.modsa_selfAction;
		if (circles[0] == "Self" && action != null) skill = action.Skill;
		else if (circles[0] == "Target")
		{
			action = modular.modsa_oppoAction;
			if (action != null) skill = action.Skill;
		}
		
		if (skill == null)
		{
			BattleUnitModel unit = modular.modsa_unitModel;
			UnitSinModel sin = null;
			if (circles.Length > 1) sin = MainClass.GetSinInUnit(circles[0], unit, action, modular.GetNumFromParamString(circles[1]));
			else sin = MainClass.GetSinInUnit(circles[0], unit, action);
			if (sin != null) skill = sin.GetSkill();
		}
		
		if (skill != null) return (int)skill.GetSkillEgoType();
		return -1;
	}
}