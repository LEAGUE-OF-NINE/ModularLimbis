using Il2CppSystem.Collections.Generic;
using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerOtherModsaValue : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SkillModel skill = modular.modsa_skillModel;
		if (skill == null) return -1;

		int modsa_idx = modular.GetNumFromParamString(circles[0]);
		int value_idx = modular.GetNumFromParamString(circles[1]);
		
		List<ModularSA> modsa_list = SkillScriptInitPatch.GetAllModsaFromSkillModel(modular.modsa_skillModel);
		if (modsa_list.Count >= modsa_idx) return -1;
		ModularSA modsa = modsa_list[modsa_idx];
		return modsa.valueList[value_idx];
	}
}