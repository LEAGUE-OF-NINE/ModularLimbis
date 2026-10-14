using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Consequence;

public class ConsequenceTrigData : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
		if (targetList.Count < 1) return;
		int valuevalue = modular.GetNumFromParamString(circles[1]);
		BATTLE_EVENT_TIMING timing = modular.battleTiming;
		
		foreach (BattleUnitModel unit in targetList)
		{
			AbilityTriggeredData trigdata = new(valuevalue, unit._instanceID, timing);
			SkillScriptInitPatch.trigdata_pointer_list.Add(trigdata.Pointer.ToInt64());
			unit.AddTriggeredData(trigdata);
		}
	}
}