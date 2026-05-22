using System;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceCutInAction : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (action == null) return;
		BattleUnitModel unit = action.Model;
		if (unit == null) return;
		unit.CutInAction(action);
		//action.OnOrderCutInAction(modular.modsa_selfAction);

		BattleActionModelManager battleActManager = Singleton<BattleActionModelManager>.Instance;
		// Still not done, trying to figure out what to do here.
		// battleActManager
		
	}
}