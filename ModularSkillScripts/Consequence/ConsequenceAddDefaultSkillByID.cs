using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceAddDefaultSkillByID : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		List<BattleUnitModel> modelList = modular.GetTargetModelList(circles[0]); // gets target from first param 
		if (modelList.Count < 1) return;

		int skillID = modular.GetNumFromParamString(circles[1]);

		foreach (BattleUnitModel targetModel in modelList)
		{
			targetModel.AddDefaultSkillById(skillID);
		}
			
	} // END ExecuteConsequence
		
}