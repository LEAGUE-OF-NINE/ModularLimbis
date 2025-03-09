using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using UnhollowerRuntimeLib;
using System;
using Il2CppSystem.Collections.Generic;
using System.Text.RegularExpressions;

namespace ModularSkillScripts
{
	[BepInPlugin("GlitchGames.ModularSkillScripts", "ModularSkillScripts", "2.1.4")]
	public class MainClass : BasePlugin
	{
		public override void Load()
		{
			Harmony harmony = new Harmony("ModularSkillScripts");
			Logg = new ManualLogSource("ModularSkillScripts");
			BepInEx.Logging.Logger.Sources.Add(Logg);
			ClassInjector.RegisterTypeInIl2Cpp<DataMod>();
			ClassInjector.RegisterTypeInIl2Cpp<ModUnitData>();
			ClassInjector.RegisterTypeInIl2Cpp<ModularSA>();
			//ClassInjector.RegisterTypeInIl2Cpp<MODSA_HelloWorld>();
			//harmony.PatchAll(typeof(PatchesForLethe));
			harmony.PatchAll(typeof(SkillScriptInitPatch));
			harmony.PatchAll(typeof(StagePatches));
			harmony.PatchAll(typeof(UniquePatches));
			if (fakepowerEnabled) harmony.PatchAll(typeof(FakePowerPatches));
		}

		public static List<BattleUnitModel> ShuffleUnits(List<BattleUnitModel> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = MainClass.rng.Next(n + 1);
				BattleUnitModel value = list.ToArray()[k];
				list.ToArray()[k] = list.ToArray()[n];
				list.ToArray()[n] = value;
			}
			return list;
		}

		public static bool fakepowerEnabled = false;

		public static Random rng = new Random();

		public static readonly Regex sWhitespace = new Regex(@"\s+");

		public static bool logEnabled = false;

		public const string NAME = "ModularSkillScripts";

		public const string VERSION = "2.1.4";

		public const string AUTHOR = "GlitchGames";

		public const string GUID = "GlitchGames.ModularSkillScripts";

		public static ManualLogSource Logg;
	}

}
