using Il2CppSystem;

namespace ModularSkillScripts.Acquirer;

public class AcquirerGetSkillId : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		int circle_count = circles.Length;
		if (circle_count > 0)
		{
			BattleUnitModel unit = modular.modsa_unitModel;
			UnitSinModel sin = null;
			if (circle_count > 1) sin = MainClass.GetSinInUnit(circles[0], unit, action, modular.GetNumFromParamString(circles[1]));
			else sin = MainClass.GetSinInUnit(circles[0], unit, action);
			if (sin == null) return -1;
			return sin.SkillID;
		}
		
		
		if (modular.modsa_skillModel != null) return modular.modsa_skillModel.GetID();
		if (action != null) return action.Skill.GetID();
		
		return 0;
	}
}