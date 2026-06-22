using System;
using BattleUI;
using BattleUI.Abnormality;
using BattleUI.BattleUnit;
using BattleUI.Operation;

namespace ModularSkillScripts.Consequence;

public class ConsequenceRefreshAllSlotVisual : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
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
		//BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		//SinManager sinManager_inst = Singleton<SinManager>.Instance;
		foreach (NewOperationSinActionSlot sinActionSlot in opController.SinActionSlotList)
		{
			NewOperationReadySinSlot opReadySinSlot = sinActionSlot._readySinSlot;
			if (opReadySinSlot && opReadySinSlot._skillSlot && opReadySinSlot.UnitSin != null)
			{
				opReadySinSlot._skillSlot.SetSkill(opReadySinSlot.UnitSin, opReadySinSlot.UnitSin.SinOption == SIN_OPTION.Ego);
			}
			NewOperationSinSlot opSinSlot_top = sinActionSlot._secondSinSlot;
			if (opSinSlot_top && opSinSlot_top._skillSlot && opSinSlot_top.UnitSin != null)
			{
				opSinSlot_top._skillSlot.SetSkill(opSinSlot_top.UnitSin, opSinSlot_top.UnitSin.SinOption == SIN_OPTION.Ego);
			}
			NewOperationSinSlot opSinSlot_bottom = sinActionSlot._firstSinSlot;
			if (opSinSlot_bottom && opSinSlot_bottom._skillSlot && opSinSlot_bottom.UnitSin != null)
			{
				opSinSlot_bottom._skillSlot.SetSkill(opSinSlot_bottom.UnitSin, opSinSlot_bottom.UnitSin.SinOption == SIN_OPTION.Ego);
			}
		}
		
	}
}