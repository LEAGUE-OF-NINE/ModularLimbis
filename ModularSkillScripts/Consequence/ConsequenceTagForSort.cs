using System;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Consequence;

public class ConsequenceTagForSort : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (action == null) return;

		string circle_0 = circles[0];
		switch (circle_0)
		{
			default: SkillScriptInitPatch.rangedList_modular.Add(action); break;
			case "late": SkillScriptInitPatch.lateList_modular.Add(action); break;
		}
		
	}
}