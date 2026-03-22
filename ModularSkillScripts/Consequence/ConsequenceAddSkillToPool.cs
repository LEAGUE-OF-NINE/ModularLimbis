using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceAddSkillToPool : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		List<BattleUnitModel> modelList = modular.GetTargetModelList(circles[0]); // gets target from first param 
		if (modelList.Count < 1) return;

		int skillID = modular.GetNumFromParamString(circles[1]);
		int num = 1;
		if (circles.Length >= 3) num = modular.GetNumFromParamString(circles[2]);

		foreach (BattleUnitModel targetModel in modelList)
		{
			targetModel.AddSkillToSkillPool(skillID, num);
		}
			
	} // END ExecuteConsequence
} // END IModularConsequence