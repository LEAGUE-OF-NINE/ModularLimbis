using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceSkillReveal : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		int skillID = modular.GetNumFromParamString(circles[1]);
		UnlockInformationManager unlockInfo_inst = Singleton<UnlockInformationManager>.Instance;

		bool unlockAllSkills = circles[1].Equals("all", StringComparison.OrdinalIgnoreCase);
		foreach (var targetModel in modelList)
		{
			if (unlockAllSkills)
			{
				var allSkills = targetModel.GetSkillList();
				foreach (var skill in allSkills)
				{
					unlockInfo_inst.UnlockSkill(targetModel.GetOriginUnitID(), skill);
				}
			}
			else
			{
				unlockInfo_inst.UnlockSkill(targetModel.GetOriginUnitID(), targetModel.UnitDataModel.GetSkillModel(skillID));
			}
		}
	}
}