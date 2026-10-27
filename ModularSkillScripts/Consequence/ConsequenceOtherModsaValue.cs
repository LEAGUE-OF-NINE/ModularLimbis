using Il2CppSystem.Collections.Generic;
using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Consequence;

public class ConsequenceOtherModsaValue : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		string mode = circles[0];

		List<ModularSA> modsa_list = new();
		switch (mode)
		{
			case "skill":
				modsa_list = SkillScriptInitPatch.GetAllModsaFromSkillModel(modular.modsa_skillModel);
				break;
			case "passive":
				modsa_list = SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(modular.modsa_passiveModel, false);
				break;
			case "buf":
			case "buff":
				modsa_list = SkillScriptInitPatch.GetAllModbaFromBuffModel_Fast(modular.modsa_buffModel);
				break;
		}

		int modsa_idx = modular.GetNumFromParamString(circles[1]);
		int value_idx = modular.GetNumFromParamString(circles[2]);
		int val = modular.GetNumFromParamString(circles[3]);
		if (modsa_idx >= modsa_list.Count) return;
		ModularSA modsa = modsa_list[modsa_idx];
		modsa.valueList[value_idx] = val;

	} // END ExecuteConsequence
		
}