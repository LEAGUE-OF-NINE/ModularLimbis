using Il2CppSystem.Collections.Generic;
using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceSetSlotWeight : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var targets = modular.GetTargetModelList(circles[0]);
		int amount = modular.GetNumFromParamString(circles[1]);
		foreach (var targetModel in targets)
		{
			List<SlotWeightCondition> slotWeight = targetModel._slotWeightList;
			foreach (var slot in slotWeight) 
			{
				//MainClass.Logg.LogError($"slot weight before {slot.GetSlotWeight()}");
				slot._COUNT = amount;
				//MainClass.Logg.LogError($"slot weight after {slot.GetSlotWeight()}");
			}
		}
	}
}