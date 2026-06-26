using System;
using BattleUI;
using BattleUI.Operation;

namespace ModularSkillScripts.Consequence;

public class ConsequenceShine : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (action == null) return;
		SinActionModel sinAction = action.SinAction;
		if (sinAction == null) return;
		UnitSinModel unitSin = action.Sin;
		if (unitSin == null) return;
		
		BattleUIRoot battleUIRoot = SingletonBehavior<BattleUIRoot>.Instance;
		if (!battleUIRoot)
		{
			MainClass.LogModular("Dude This BattleUIRoot shit is FUCKING NULL");
			return;
		}
		NewOperationController opController = battleUIRoot.NewOperationController;
		if (!opController)
		{
			MainClass.LogModular("Dude This NewOperationController shit is FUCKING NULL");
			return;
		}
		
		//NewOperationParentSlot op_parentSlot = opController.FindSinSlot(sinAction, unitSin);
		NewOperationSinActionSlot op_sinAction = opController.GetSinActionSlot(sinAction);
		if (!op_sinAction) return;
		bool enable = circledSection == "true";

		NewOperationSinSlot opSinSlot = op_sinAction._secondSinSlot;
		if (opSinSlot)
		{
			OperationSkillEffectManager op_skillEffectManager = opSinSlot._effectManager;
			if (op_skillEffectManager && opSinSlot.UnitSin == unitSin)
			{
				op_skillEffectManager.SetActiveEffect_OneType(OPERATION_SKILL_EFFECT_TYPE.RING_FAVUISM_TEST, enable);
				return;
			}
		}

		opSinSlot = op_sinAction._firstSinSlot;
		if (opSinSlot)
		{
			OperationSkillEffectManager op_skillEffectManager = opSinSlot._effectManager;
			if (op_skillEffectManager && opSinSlot.UnitSin == unitSin)
			{
				op_skillEffectManager.SetActiveEffect_OneType(OPERATION_SKILL_EFFECT_TYPE.RING_FAVUISM_TEST, enable);
				return;
			}
		}
		
	} // END ExecuteConsequence
} // END IModularConsequence