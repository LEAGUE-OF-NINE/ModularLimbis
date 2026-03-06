using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceSkillHide : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		UnlockInformationManager unlockInfo_inst = Singleton<UnlockInformationManager>.Instance;
		foreach (var targetModel in modelList)
		{
			var allSkills = targetModel.GetSkillList();
			foreach (var skill in allSkills)
			{
				unlockInfo_inst.ClearUpdatedSkillUnlockInformation(targetModel.GetOriginUnitID());
			}
		}
	}
}