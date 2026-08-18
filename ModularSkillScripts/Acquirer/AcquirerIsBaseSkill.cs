using System;

namespace ModularSkillScripts.Acquirer;

public class AcquirerIsBaseSkill : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		string unit_s = circles[0];
		string skill_s = circles[1];
		
		BattleUnitModel targetModel = unit_s == "This" ? modular.modsa_unitModel : modular.GetTargetModel(unit_s);
		if (targetModel == null) return -1;
		
		int skillID = -1;
		if (skill_s == "This") {
			SkillModel skill = modular.modsa_skillModel;
			if (skill != null) skillID = skill.GetID();
		}
		else skillID = modular.GetNumFromParamString(skill_s);

		if (skillID < 1) return -1;

		switch (circles[2]) {
			case "CheckSkillList": {
				foreach (SkillModel skill_x in modular.modsa_unitModel.GetSkillList()) {
					if (skill_x.GetID() == skillID) return 1;
				}
			} return 0;
			case "IsDefaultSkill": return targetModel.HasSkillAsDefaultSkillIncludeDefenseSkill(modular.modsa_skillModel) ? 1 : 0;
		}
		
		return -1;
	}
}