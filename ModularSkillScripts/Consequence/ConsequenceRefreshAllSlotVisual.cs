using System;
using BattleUI.Operation;

namespace ModularSkillScripts.Consequence;

public class ConsequenceRefreshAllSlotVisual : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		NewOperationController opController = Singleton<NewOperationController>.Instance;
		if (!opController)
		{
			MainClass.LogModular("Dude This NewOperationController shit is FUCKING NULL");
			return;
		}

		foreach (NewOperationSinActionSlot sinActionSlot in opController.SinActionSlotList)
		{
			sinActionSlot.UpdateStateForAb();
		}
		
	}
}