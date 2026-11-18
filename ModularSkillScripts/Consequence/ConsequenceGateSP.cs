using System;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using UnityEngine;

namespace ModularSkillScripts.Consequence;
public class ConsequenceGateSP : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var targets = modular.GetTargetModelList(circles[0]);
		if (targets == null || circles.Length < 2) return;

		string mode = circles[1].Trim();
		bool isMin = mode.Equals("Min", StringComparison.OrdinalIgnoreCase);
		bool isMax = mode.Equals("Max", StringComparison.OrdinalIgnoreCase);
		bool isClear = mode.Equals("Clear", StringComparison.OrdinalIgnoreCase);

		int value = 0, minSp = 0, maxSp = 0;
		if (isMin || isMax)
		{
			if (circles.Length < 3) { MainClass.Logg.LogError($"gatesp: '{mode}' needs a value"); return; }
			value = modular.GetNumFromParamString(circles[2]);
		}
		else if (!isClear)
		{
			if (circles.Length < 3) { MainClass.Logg.LogError("gatesp: expected (target, Min|Max, value), (target, Clear) or (target, min, max)"); return; }
			minSp = modular.GetNumFromParamString(circles[1]);
			maxSp = modular.GetNumFromParamString(circles[2]);
		}


		foreach (BattleUnitModel target in targets)
		{
			if (target == null) continue;
			var k = target.InstanceID;

			if (isClear) { SkillScriptInitPatch.gateSPDict.Remove(k); continue; }

			var g = SkillScriptInitPatch.gateSPDict.TryGetValue(k, out var existing)
				? existing
				: (Min: int.MinValue, Max: int.MaxValue);

			if (isMin) g.Min = value;
			else if (isMax) g.Max = value;
			else g = (minSp, maxSp);

			if (g.Min > g.Max)
				MainClass.Logg.LogWarning($"gatesp: unit {k} has min {g.Min} > max {g.Max}; gate will be ignored until fixed");

			SkillScriptInitPatch.gateSPDict[k] = g;
		}
	}
}
