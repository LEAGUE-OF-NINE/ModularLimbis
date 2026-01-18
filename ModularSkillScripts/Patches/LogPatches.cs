using BattleUI.Operation;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using BepInEx.Unity.IL2CPP.UnityEngine;
using System;
using BepInEx.Logging;

namespace ModularSkillScripts.Patches;

internal class LogPatches
{
	[HarmonyPatch(typeof(ManualLogSource), "Log", new Type[] { typeof(LogLevel), typeof(object) })]
	[HarmonyPrefix]
	private static bool loggingtoggle(ManualLogSource __instance)
	{
		if (__instance.SourceName == "ModularSkillScripts")
		{
			return MainClass.EnableLogging.Value;
		}
		return true;
	}
	[HarmonyPatch(typeof(ManualLogSource), "Log", new Type[] { typeof(LogLevel), typeof(BepInEx.Core.Logging.Interpolation.BepInExLogInterpolatedStringHandler) })]
	[HarmonyPrefix]
	private static bool loggingtoggle2(ManualLogSource __instance)
	{
		if (__instance.SourceName == "ModularSkillScripts")
		{
			return MainClass.EnableLogging.Value;
		}
		return true;
	}
}
