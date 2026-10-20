using Il2CppSystem.Collections.Generic;
using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerOtherModsaValue : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SkillModel skill = modular.modsa_skillModel;
		if (skill == null) return -1;

		string mode = circles[0];

		List<ModularSA> modsa_list = mode switch
		{
			"skill" => SkillScriptInitPatch.GetAllModsaFromSkillModel(modular.modsa_skillModel),
			"passive" => SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(modular.modsa_passiveModel),
			"buf" => SkillScriptInitPatch.GetAllModbaFromBuffModel_Fast(modular.modsa_buffModel),
			"buff" => SkillScriptInitPatch.GetAllModbaFromBuffModel_Fast(modular.modsa_buffModel),
			_ => new List<ModularSA>()
		};

		int modsa_idx = modular.GetNumFromParamString(circles[1]);
		int value_idx = modular.GetNumFromParamString(circles[2]);
		if (modsa_list.Count >= modsa_idx) return -1;
		ModularSA modsa = modsa_list[modsa_idx];
		return modsa.valueList[value_idx];
	}
}