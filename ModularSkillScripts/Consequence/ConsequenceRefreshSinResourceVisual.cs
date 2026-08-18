using System;
using BattleUI;
using BattleUI.Abnormality;
using BattleUI.BattleUnit;
using BattleUI.Operation;

namespace ModularSkillScripts.Consequence;

public class ConsequenceRefreshSinResourceVisual : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		ActivateRefresh();
	}

	public static void ActivateRefresh()
	{
		BattleUIRoot battleUIRoot = SingletonBehavior<BattleUIRoot>.Instance;
		if (!battleUIRoot) {
			MainClass.LogModular("Dude This BattleUIRoot shit is FUCKING NULL");
			return;
		}
		battleUIRoot.UpdateEvilStockWithInDecreaseAnim();
	}
}