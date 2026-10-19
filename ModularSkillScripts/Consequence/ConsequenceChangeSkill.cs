namespace ModularSkillScripts.Consequence;

public class ConsequenceChangeSkill : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (action == null) return;
		bool erase_prev_skill = circles.Length > 1 && circles[1] == "true";
		ChangeSkillOfAction(action, modular.GetNumFromParamString(circles[0]), erase_prev_skill);
	}

	public static void ChangeSkillOfAction(BattleActionModel action, int skill_ID, bool erase_prev_skill = true)
	{
		if (erase_prev_skill)
		{
			if (action.TryChangeSkill(skill_ID))
			{
				action._sin._overwrited = false;
				action._previousSkillBeforeChanged = action._skill;
			}
			
			/*
			StaticDataManager staticDataManager = Singleton<StaticDataManager>.Instance;
			SkillStaticData skill_new_staticData = staticDataManager._skillList.GetData(skill_ID);
			if (skill_new_staticData != null) {
				SkillModel skill_new = new SkillModel(skill_new_staticData, action._model.GetLevel(), 5);
				//action.ChangeSkill(skill_new, false);
				//action._previousSkillBeforeChanged = null;
				action._skill = skill_new;
			}*/
		}
		else
		{
			action.TryChangeSkill(skill_ID);
			//action._previousSkillBeforeChanged = action.Skill;
			//action.ReturnPreviousSkill();
		}
		
		
		
	}
}