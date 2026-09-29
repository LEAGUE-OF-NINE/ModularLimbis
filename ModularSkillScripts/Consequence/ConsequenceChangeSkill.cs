namespace ModularSkillScripts.Consequence;

public class ConsequenceChangeSkill : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (action == null) return;
		if (circles.Length > 1 && circles[1] == "false")
		{
			int skill_ID = modular.GetNumFromParamString(circles[0]);
			ChangeSkillOfAction(action, skill_ID);
		} else {
			action.TryChangeSkill(modular.GetNumFromParamString(circles[0]));
		}
		//action.ChangeSkill()
	}

	public static void ChangeSkillOfAction(BattleActionModel action, int skill_ID)
	{
		StaticDataManager staticDataManager = Singleton<StaticDataManager>.Instance;
		SkillStaticData skill_new_staticData = staticDataManager._skillList.GetData(skill_ID);
		if (skill_new_staticData != null) {
			SkillModel skill_new = new SkillModel(skill_new_staticData, action._model.GetLevel(), 5);
			action.ChangeSkill(skill_new, false);
		}
	}
}