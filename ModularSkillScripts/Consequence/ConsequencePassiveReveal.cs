using System;

namespace ModularSkillScripts.Consequence;

public class ConsequencePassiveReveal : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		int pasID = modular.GetNumFromParamString(circles[1]);
		UnlockInformationManager unlockInfo_inst = Singleton<UnlockInformationManager>.Instance;

		bool unlockAllPassives = circles[1].Equals("all", StringComparison.OrdinalIgnoreCase);
		foreach (var targetModel in modelList)
		{
			if (unlockAllPassives)
			{
				var allPassives = targetModel.GetPassiveList();
				foreach (var passive in allPassives)
				{
					unlockInfo_inst.UnlockPassiveStatus(targetModel.GetOriginUnitID(), pasID);
				}
			}
			else
			{
				unlockInfo_inst.UnlockPassiveStatus(targetModel.GetOriginUnitID(), pasID);
			}
		}
	}
}