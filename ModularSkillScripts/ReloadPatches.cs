using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Lethe;
using MainUI;

namespace ModularSkillScripts;

public class ReloadPatches
{
	public static Dictionary<string, string> loadedScripts = new Dictionary<string, string>();
	
	[HarmonyPatch(typeof(LobbyUIPresenter), nameof(LobbyUIPresenter.Initialize))]
	[HarmonyPostfix]
	private static void PostMainUILoad()
	{
		loadedScripts.Clear();
		MainClass.Logg.LogInfo("PostMainUILoad - Reloading Modular Lua Scripts");
		foreach (var modPath in Directory.GetDirectories(LetheMain.modsPath.FullPath))
		{
			var expectedPath = Path.Combine(modPath, "modular_lua");
			if (!Directory.Exists(expectedPath)) continue;

			foreach (var luaPath in Directory.GetFiles(expectedPath, "*.lua", SearchOption.AllDirectories))
			{
				var name = Path.GetFileNameWithoutExtension(luaPath);
				if (loadedScripts.ContainsKey(name))
				{
					MainClass.Logg.LogError($"Duplicate Modular Lua Script name '{name}' found in '{luaPath}'. Skipping.");
					continue;
				}
				var content = File.ReadAllText(luaPath);
				if (String.IsNullOrWhiteSpace(content))
				{
					MainClass.Logg.LogWarning($"Modular Lua Script '{name}' in '{luaPath}' is empty. Skipping.");
					continue;
				}
				loadedScripts[name] = content;
				MainClass.Logg.LogInfo($"Loaded Modular Lua Script '{name}' from '{luaPath}'.");
			}
		}
	}
	
}