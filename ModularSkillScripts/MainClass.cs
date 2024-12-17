using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using UnhollowerRuntimeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ModularSkillScripts
{
	[BepInPlugin("GlitchGames.ModularSkillScripts", "ModularSkillScripts", "1.6")]
	public class MainClass : BasePlugin
	{
		public override void Load()
		{
			Harmony harmony = new Harmony("ModularSkillScripts");
			Logg = new ManualLogSource("ModularSkillScripts");
			Logger.Sources.Add(Logg);
			ClassInjector.RegisterTypeInIl2Cpp<InjectedFunnyChange>();
			ClassInjector.RegisterTypeInIl2Cpp<DataMod>();
			ClassInjector.RegisterTypeInIl2Cpp<ModUnitData>();
			ClassInjector.RegisterTypeInIl2Cpp<ModularSA>();
			//ClassInjector.RegisterTypeInIl2Cpp<MODSA_HelloWorld>();
			//harmony.PatchAll(typeof(PatchesForLethe));
			harmony.PatchAll(typeof(SkillScriptInitPatch));
			harmony.PatchAll(typeof(StagePatches));
			//OnSAGiveBuffFactionCheck.Setup(harmony);
			//GiveBuffOnUseFactionCheck.Setup(harmony);
			//GiveBuffOnUseFactionCheckReson.Setup(harmony);
			//GiveBuffOnUseFactionCheckPerfectReson.Setup(harmony);
			//CustomReloadScript.Setup(harmony);
			//AtValueSpeedBecomeUnclashable.Setup(harmony);
		}

		public static Random rng = new Random();

		public static readonly Regex sWhitespace = new Regex(@"\s+");

		public static bool logEnabled = false;

		public const string NAME = "ModularSkillScripts";

		public const string VERSION = "1.6";

		public const string AUTHOR = "GlitchGames";

		public const string GUID = "GlitchGames.ModularSkillScripts";

		public static ManualLogSource Logg;
	}

}
