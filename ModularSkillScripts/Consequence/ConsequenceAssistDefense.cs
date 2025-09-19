using System.Collections.Generic;
using System.Linq;

namespace ModularSkillScripts.Consequence;

public class ConsequenceAssistDefense : IModularConsequence
{
	
	private record AssistDefenseEntry(int[] protectedUnitInstanceIDs, int skillId);
	
	private static readonly Dictionary<int, AssistDefenseEntry> AssistDefenseEntries = new();
	
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var protectors = modular.GetTargetModelList(circles[0]);
		var protecteds = modular.GetTargetModelList(circles[1])
			.ToArray()
			.Select(x => x.InstanceID)
			.ToArray();
		var skillId = modular.GetNumFromParamString(circles[2]);
		foreach (var protector in protectors)
		{
			AssistDefenseEntries[protector.InstanceID] = new AssistDefenseEntry(protecteds, skillId);
		}
	}
	
	public static void ClearAssistDefenseEntries()
	{
		AssistDefenseEntries.Clear();
	}
	
	public static int GetAssistDefenseSkillId(int protectorInstanceId, int protectedInstanceId)
	{
		if (!AssistDefenseEntries.TryGetValue(protectedInstanceId, out var entry))
			return -1;
		if (!entry.protectedUnitInstanceIDs.Contains(protectorInstanceId))
			return -1;
		return entry.skillId;
	}
	
}