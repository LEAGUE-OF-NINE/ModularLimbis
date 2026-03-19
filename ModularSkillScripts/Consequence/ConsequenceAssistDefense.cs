using System.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceAssistDefense : IModularConsequence
{
	
	private readonly record struct AssistDefenseEntry(int defenderInstanceID, int defendedInstanceID);
	
	private static readonly Dictionary<AssistDefenseEntry, int> AssistDefenseEntries = new();
	
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var defenders = modular.GetTargetModelList(circles[0]);
		var defendeds = modular.GetTargetModelList(circles[1]);
		var skillId = modular.GetNumFromParamString(circles[2]);
		foreach (var defender in defenders)
		{
			foreach (var defended in defendeds)
			{
				AssistDefenseEntries[new AssistDefenseEntry(defender.InstanceID, defended.InstanceID)] = skillId;
			}
		}
	}
	
	public static void ClearAssistDefenseEntries()
	{
		MainClass.LogModular($"Clearing {AssistDefenseEntries.Count} assist defense entries");
		AssistDefenseEntries.Clear();
	}
	
	public static int GetAssistDefenseSkillId(int protectorInstanceId, int protectedInstanceId)
	{
		MainClass.LogModular("================================");
		MainClass.LogModular($"GetAssistDefenseSkillId: protectorInstanceId={protectorInstanceId}, protectedInstanceId={protectedInstanceId}");
		foreach (var assistDefenseEntry in AssistDefenseEntries)
		{
			MainClass.LogModular($"  Entry: protectorInstanceId={assistDefenseEntry.Key}, skillId={assistDefenseEntry.Value}");
		}
		MainClass.LogModular("================================");
		return AssistDefenseEntries.GetValueOrDefault(new AssistDefenseEntry(protectorInstanceId, protectedInstanceId), -1);
	}
	
}