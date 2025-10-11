using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Lethe;
using Lua.CodeAnalysis.Compilation;
using Lua.CodeAnalysis.Syntax;
using Lua.Runtime;
using MainUI;

namespace ModularSkillScripts.Patches;

public class ReloadPatches
{
	
	[HarmonyPatch(typeof(LobbyUIPresenter), nameof(LobbyUIPresenter.Initialize))]
	[HarmonyPostfix]
	private static void PostMainUILoad()
	{
		LuaScript.loadedScripts.Clear();
		MainClass.Logg.LogInfo("PostMainUILoad - Reloading Modular Lua Scripts");
		foreach (var modPath in Directory.GetDirectories(LetheMain.modsPath.FullPath))
		{
			var expectedPath = Path.Combine(modPath, "modular_lua");
			if (!Directory.Exists(expectedPath)) continue;

			foreach (var luaPath in Directory.GetFiles(expectedPath, "*.lua", SearchOption.AllDirectories))
			{
				var name = Path.GetFileNameWithoutExtension(luaPath);
				if (LuaScript.loadedScripts.ContainsKey(name))
				{
					MainClass.Logg.LogError($"Duplicate Modular Lua Script name '{name}' found in '{luaPath}'. Skipping.");
					continue;
				}
				var content = File.ReadAllText(luaPath);
				if (string.IsNullOrWhiteSpace(content))
				{
					MainClass.Logg.LogWarning($"Modular Lua Script '{name}' in '{luaPath}' is empty. Skipping.");
					continue;
				}

				try
				{
					var syntaxTree = LuaSyntaxTree.Parse(content, name);
					var script = LuaCompiler.Default.Compile(syntaxTree, name);
					LuaScript.loadedScripts[name] = new LuaScript { Body = content, Content = script, Name = name };
					MainClass.Logg.LogInfo($"Loaded Modular Lua Script '{name}' from '{luaPath}'.");
				}
				catch (Exception ex)
				{
					MainClass.Logg.LogError($"Failed to load Modular Lua Script '{name}' from '{luaPath}': {ex.Message}");
				}
			}
		}
	}
	
}