using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;

namespace Equalizer
{
    [BepInPlugin("GlitchGames.Equalizer", "Equalizer", "1.0")]
    public class MainClass : BasePlugin
    {
        public override void Load()
        {
            Harmony harmony = new Harmony("Equalizer");
            Logg = new ManualLogSource("Equalizer");
            BepInEx.Logging.Logger.Sources.Add(Logg);
            harmony.PatchAll(typeof(EqualizePatch));
        }

        public const string NAME = "Equalizer";

        public const string VERSION = "1.0";

        public const string AUTHOR = "GlitchGames";

        public const string GUID = "GlitchGames.Equalizer";

        public static ManualLogSource Logg;
    }

}
