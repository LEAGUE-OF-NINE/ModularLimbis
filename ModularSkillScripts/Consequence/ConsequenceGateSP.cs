using ModularSkillScripts;
using ModularSkillScripts.Patches;
using UnityEngine;

namespace ModularSkillScripts.Consequence;


public class ConsequenceGateSP : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var targets = modular.GetTargetModelList(circles[0]);
		int minSp = modular.GetNumFromParamString(circles[1]);
		int maxSp = modular.GetNumFromParamString(circles[2]);

		if (targets == null) return;
		foreach (BattleUnitModel target in targets)
		{
			if (target == null) { continue; }
			var k = target.InstanceID;
			SkillScriptInitPatch.gateSPDict[k] = (minSp, maxSp);
		}
	}
}
