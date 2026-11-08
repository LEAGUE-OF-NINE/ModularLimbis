using Il2CppSystem.Collections.Generic;

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
			ATTRIBUTE_TYPE sinType_before = action._skill.GetAttributeType();
			bool hadSin = action._expectedToEarnMap.TryGetValue(sinType_before, out int amount_before);
			
			if (action.TryChangeSkill(skill_ID))
			{
				action._sin._overwrited = false;
				action._previousSkillBeforeChanged = null;

				if (hadSin) {
					ATTRIBUTE_TYPE sinType_after = action._skill.GetAttributeType();
					if (sinType_before != sinType_after) {
						action._expectedToEarnMap[sinType_before] = 0;
						bool hasSin = action._expectedToEarnMap.TryGetValue(sinType_before, out int amount_after);
						if (hasSin) action._expectedToEarnMap[sinType_after] = amount_before + amount_after;
						else action._expectedToEarnMap[sinType_after] = amount_before;
					}
				}
				
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