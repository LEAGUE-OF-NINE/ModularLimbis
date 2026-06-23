using BattleUI;
using BattleUI.Operation;
using Il2CppSystem.Data;
using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceDashhoardEffect : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var targets = modular.GetTargetModelList(circles[0]);
		if (targets.Count == 0) return;
		int slotNum = modular.GetNumFromParamString(circles[1]);
		bool isActive = modular.GetBoolFromParamString(circles[2]);
		OPERATION_SKILL_EFFECT_TYPE effectType;
		Enum.TryParse(circles[3], true, out effectType);
		bool topSlot = circles[4] == "top";
	
		foreach (BattleUnitModel unit in targets)
		{
			var sinActionSlot = SingletonBehavior<BattleUIRoot>.Instance.NewOperationController.GetSinActionSlot(unit.GetSinActionList()[slotNum]);
			switch (topSlot)
			{
				case true:
					sinActionSlot._secondSinSlot._effectManager.SetActiveEffect_OneType(effectType, isActive);
					sinActionSlot._secondSinSlot._effectManager.SetEffectAlpha(effectType, 1);
					break;
				case false:
					sinActionSlot._firstSinSlot._effectManager.SetActiveEffect_OneType(effectType, isActive);
					sinActionSlot._firstSinSlot._effectManager.SetEffectAlpha(effectType, 1);
					break;
			}
		}
	} 
}