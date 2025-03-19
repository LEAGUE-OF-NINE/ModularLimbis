using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Text.RegularExpressions;

namespace Drastic
{
	[BepInPlugin("GlitchGames.Drastic", "Drastic", "0.4.5")]
	public class MainClass : BasePlugin
	{
		public override void Load()
		{
			Harmony harmony = new Harmony("Drastic");
			Logg = new ManualLogSource("Drastic");
			BepInEx.Logging.Logger.Sources.Add(Logg);
			harmony.PatchAll(typeof(BasicBufPatch));
			harmony.PatchAll(typeof(DrasticMeasures));
		}

		public static bool logEnabled = false;

		public const string NAME = "Drastic";

		public const string VERSION = "0.4.5";

		public const string AUTHOR = "GlitchGames";

		public const string GUID = "GlitchGames.Drastic";

		public static ManualLogSource Logg;
	}

}
