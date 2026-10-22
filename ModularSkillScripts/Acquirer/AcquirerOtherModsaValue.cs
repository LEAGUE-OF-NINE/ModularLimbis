using Il2CppSystem.Collections.Generic;
using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerOtherModsaValue : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		string mode = circles[0];

		List<ModularSA> modsa_list = new();
		switch (mode)
		{
			case "skill":
				modsa_list = SkillScriptInitPatch.GetAllModsaFromSkillModel(modular.modsa_skillModel);
				break;
			case "realskill": {
				BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
				BattleUnitView view = objManager.GetView(modular.modsa_unitModel);
				if (!view) return -1;
		
				BattleSkillViewer skillViewer = view.GetCurrentSkillViewer();
				SkillModel skill = skillViewer?.CurrentSkillModel;
				if (skill == null) return -1;
				modsa_list = SkillScriptInitPatch.GetAllModsaFromSkillModel(skill);
			} break;
			case "skinskill": {
				BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
				BattleUnitView view = objManager.GetView(modular.modsa_unitModel);
				if (!view) return -1;
		
				BattleSkillViewer skillViewer = view.GetCurrentSkillViewer();
				SkillModel skill = skillViewer?.CurrentSkinSkillModel;
				if (skill == null) return -1;
				modsa_list = SkillScriptInitPatch.GetAllModsaFromSkillModel(skill);
			} break;
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
		if (modsa_idx >= modsa_list.Count) return -1;
		ModularSA modsa = modsa_list[modsa_idx];
		return modsa.valueList[value_idx];
	}
}