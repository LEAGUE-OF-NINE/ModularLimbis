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
		
		NewOperationSinActionSlot op_sinAction = opController.GetSinActionSlot(sinAction);
		if (!op_sinAction) return;
		bool enable = circledSection == "true";
		
		NewOperationSinSlot opSinSlot_top = op_sinAction._secondSinSlot;
		if (opSinSlot_top)
		{
			BattleActionModel cur_action = opSinSlot_top.SinAction?.CurrentBattleAction;
			OperationSkillEffectManager op_skillEffectManager = opSinSlot_top._effectManager;
			if (op_skillEffectManager && cur_action == action)
			{
				op_skillEffectManager.SetActiveEffect_OneType(OPERATION_SKILL_EFFECT_TYPE.RING_FAVUISM_TEST, enable);
				return;
			}
		}
		
		NewOperationSinSlot opSinSlot_bottom = op_sinAction._firstSinSlot;
		if (opSinSlot_bottom)
		{
			BattleActionModel cur_action = opSinSlot_bottom.SinAction?.CurrentBattleAction;
			OperationSkillEffectManager op_skillEffectManager = opSinSlot_bottom._effectManager;
			if (op_skillEffectManager && cur_action == action)
			{
				op_skillEffectManager.SetActiveEffect_OneType(OPERATION_SKILL_EFFECT_TYPE.RING_FAVUISM_TEST, enable);
				return;
			}
		}
	} // END ExecuteConsequence
} // END IModularConsequence