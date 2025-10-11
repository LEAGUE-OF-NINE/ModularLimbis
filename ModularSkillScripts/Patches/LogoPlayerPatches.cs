using BattleUI.Operation;
using HarmonyLib;

namespace ModularSkillScripts.Patches;

internal class LogoPlayerPatches
{

	[HarmonyPatch(typeof(LogoPlayer), nameof(LogoPlayer.Awake))]
	[HarmonyPrefix]
	private static void Prefix_NewOperationController_EquipDefense()
	{
		MainClass.Logg.LogInfo($"{MainClass.consequenceDict.Count} consequences registered: {string.Join(", ", MainClass.consequenceDict.Keys)}");
		MainClass.Logg.LogInfo($"{MainClass.acquirerDict.Count} acquirers registered: {string.Join(", ", MainClass.acquirerDict.Keys)}");
	}
	
}