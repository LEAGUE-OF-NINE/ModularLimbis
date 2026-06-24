using BattleUI;
using BattleUI.Operation;
using Il2CppSystem.Data;
using System;

namespace ModularSkillScripts.Acquirer;

public class ConsequenceHasDashboardEffect : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var target = modular.GetTargetModel(circles[0]);
		if (target == null) return -1;
		int slotNum = modular.GetNumFromParamString(circles[1]);
		OPERATION_SKILL_EFFECT_TYPE effectType;
		Enum.TryParse(circles[2], true, out effectType);
		bool topSlot = circles[3] == "top";

		var sinActionSlot = SingletonBehavior<BattleUIRoot>.Instance.NewOperationController.GetSinActionSlot(target.GetSinActionList()[slotNum]);
		switch (topSlot)
		{
			case true:
				return sinActionSlot._secondSinSlot._effectManager.ContainEffect(effectType) ? 1 : 0;
			case false:
				return sinActionSlot._firstSinSlot._effectManager.ContainEffect(effectType) ? 1 : 0;
		}
	}
}
