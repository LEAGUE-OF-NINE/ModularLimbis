using BattleUI.Operation;
using HarmonyLib;

namespace ModularSkillScripts.Patches;

internal class LogoPlayerPatches
{

	[HarmonyPatch(typeof(LogoSceneManager), nameof(LogoSceneManager.Awake))]
	[HarmonyPrefix]
	private static void Prefix_NewOperationController_EquipDefense()
	{
		MainClass.LogModular($"{MainClass.consequenceDict.Count} consequences registered: {string.Join(", ", MainClass.consequenceDict.Keys)}");
		MainClass.LogModular($"{MainClass.acquirerDict.Count} acquirers registered: {string.Join(", ", MainClass.acquirerDict.Keys)}");
	}
	
}