using System;
using System.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceAssistDefense : IModularConsequence
{
	private bool AreListsEqualIgnoreOrder(List<BattleUnitModel> a, List<BattleUnitModel> b)
	{
		if (a.Count != b.Count) return false;
		var temp = new List<BattleUnitModel>(b);
		for (int i = 0; i < a.Count; i++)
		{
			var item = a[i];
			if (temp.Contains(item)) temp.Remove(item);
			else return false;
		}
		return temp.Count == 0;
	}
	void DeleteAssistData(BattleUnitModel defender, int skillId, List<BattleUnitModel> defended)
	{
		void DeleteFromMod(ModularSA mod)
		{
			if (mod.assistData == null) return;

			for (int i = mod.assistData.Count - 1; i >= 0; i--)
			{
				var x = mod.assistData[i];
				MainClass.Logg.LogInfo(
						$"Checking assistData[{i}] in {mod} -> defender: {x.defender}, skillId: {x.skillID}, defended count: {x.defended?.Count ?? 0}"
				);

				if (x.defender == defender && x.skillID == skillId && AreListsEqualIgnoreOrder(x.defended, defended))
				{
					MainClass.Logg.LogInfo($"Removing assistData[{i}] from {mod}");
					mod.assistData.RemoveAt(i);
				}
			}
		}

		// use gimmeshi() instead of raw modsaDict / modpaDict etc.
		foreach (var dict in SkillScriptInitPatch.gimmeshi())
		{
			foreach (var kvp in dict)
			{
				foreach (var mod in kvp.Value)
				{
					DeleteFromMod(mod);
				}
			}
		}

		MainClass.Logg.LogInfo("Delete mode complete across all ModularSAs.");
	}

	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		MainClass.Logg.LogInfo("ExecuteConsequence START");
		MainClass.Logg.LogInfo($"section: {section}, circledSection: {circledSection}, circles len: {circles.Length}");

		BattleUnitModel defender = modular.GetTargetModel(circles[0]);
		MainClass.Logg.LogInfo($"Defender: {(defender == null ? "NULL" : defender.ToString())}");

		var defendedIl2cpp = modular.GetTargetModelList(circles[1]);
		MainClass.Logg.LogInfo($"defendedIl2cpp count: {defendedIl2cpp?.Count ?? -1}");

		var defended = new List<BattleUnitModel>(defendedIl2cpp.Count);
		for (int i = 0; i < defendedIl2cpp.Count; i++)
		{
			defended.Add(defendedIl2cpp[i]);
			MainClass.Logg.LogInfo($"defended[{i}]: {defendedIl2cpp[i]}");
		}

		int skillId = modular.GetNumFromParamString(circles[2]);
		MainClass.Logg.LogInfo($"skillId: {skillId}");

		int priority = 3;
		bool delete = circles.Length > 4 && circles[4].Equals("delete", StringComparison.OrdinalIgnoreCase);
		if (circles.Length > 3 && !delete)
		{
			priority = modular.GetNumFromParamString(circles[3]);
		}
		MainClass.Logg.LogInfo($"priority: {priority}, delete: {delete}");

		var assistdefense = new ModularSA.AssistData
		{
			defender = defender,
			defended = defended,
			skillID = skillId,
			priority = priority
		};
		MainClass.Logg.LogInfo($"AssistData created -> defender: {assistdefense.defender}, defended count: {assistdefense.defended.Count}, skillId: {assistdefense.skillID}, priority: {assistdefense.priority}");

		if (delete)
		{
			MainClass.Logg.LogInfo($"Delete mode, assistData count before: {modular.assistData.Count}");
			DeleteAssistData(defender, skillId, defended);
			MainClass.Logg.LogInfo($"Delete mode complete, assistData count after: {modular.assistData.Count}");
		}
		else
		{
			modular.assistData.Add(assistdefense);
			MainClass.Logg.LogInfo($"Added assistData -> new count: {modular.assistData.Count}");
		}

		MainClass.Logg.LogInfo("ExecuteConsequence END");
	}
}
