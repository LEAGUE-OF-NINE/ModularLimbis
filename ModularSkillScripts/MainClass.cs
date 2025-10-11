using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Text.RegularExpressions;
using ModularSkillScripts.Acquirer;
using ModularSkillScripts.Consequence;
using Random = System.Random;
using Il2CppSystem.IO;
using ModularSkillScripts.Patches;
//using Antlr4.Runtime;
//using Antlr4.Runtime.Tree;
//using ModsaLang;

namespace ModularSkillScripts;

[BepInPlugin(GUID, NAME, VERSION)]
[BepInDependency("Lethe")]
public class MainClass : BasePlugin
{
	public override void Load()
	{
		//modsaEval = new ModsaEvaluator();
		List<string> timingStringList = new();
		timingStringList.Add("StartBattle"); // 0
		timingStringList.Add("WhenUse");
		timingStringList.Add("BeforeAttack");
		timingStringList.Add("StartDuel");
		timingStringList.Add("WinDuel");
		timingStringList.Add("DefeatDuel"); // 5
		timingStringList.Add("EndBattle");
		timingStringList.Add("OnSucceedAttack");
		timingStringList.Add("WhenHit");
		timingStringList.Add("EndSkill");
		timingStringList.Add("FakePower"); // 10
		timingStringList.Add("BeforeDefense");
		timingStringList.Add("OnDie");
		timingStringList.Add("OnOtherDie");
		timingStringList.Add("DuelClash");
		timingStringList.Add("DuelClashAfter"); // 15
		timingStringList.Add("OnSucceedEvade");
		timingStringList.Add("OnDefeatEvade");
		timingStringList.Add("OnStartBehaviour");
		timingStringList.Add("BeforeBehaviour");
		timingStringList.Add("OnEndBehaviour"); // 20
		timingStringList.Add("EnemyKill");
		timingStringList.Add("OnBreak");
		timingStringList.Add("OnOtherBreak");
		timingStringList.Add("OnDiscard");
		timingStringList.Add("OnZeroHP"); // 25
		timingStringList.Add("EnemyEndSkill");
		timingStringList.Add("OnOtherBurst");
		timingStringList.Add("BeforeSA");
		timingStringList.Add("BeforeWhenHit");
		timingStringList.Add("BeforeUse"); // 30
		timingStringList.Add("Immortal");
		timingStringList.Add("ImmortalOther");
		timingStringList.Add("SpecialAction"); // 33
		timingStringList.Add("AfterSlots"); // 34
		timingStringList.Add("OnCoinToss"); // 35
		timingStringList.Add("StartBattleSkill"); // 36
		timingStringList.Add("OnBurst"); // 37
		timingStringList.Add("StartVisualCoinToss"); // 38
		timingStringList.Add("StartVisualSkillUse"); // 39
		timingStringList.Add("WhenGained"); // 40
		timingStringList.Add("ChangeMotion"); // 41
		timingStringList.Add("IgnorePanic"); // 42
		timingStringList.Add("IgnoreBreak"); // 43

		Il2CppArrayBase<string> timingStringArray = timingStringList.ToArray();
		int count = timingStringArray.Count;
		for (int i = 0; i < count; i++) timingDict.Add(timingStringArray[i], i);

		timingDict.Add("RoundStart", -1);
		timingDict.Add("OSA", timingDict["OnSucceedAttack"]);
		timingDict.Add("WH", timingDict["WhenHit"]);
		timingDict.Add("BSA", timingDict["BeforeSA"]);
		timingDict.Add("BWH", timingDict["BeforeWhenHit"]);
		timingDict.Add("SBS", timingDict["StartBattleSkill"]);

		Harmony harmony = new Harmony(NAME);
		Logg = new ManualLogSource(NAME);
		BepInEx.Logging.Logger.Sources.Add(Logg);
		ClassInjector.RegisterTypeInIl2Cpp<DataMod>();
		ClassInjector.RegisterTypeInIl2Cpp<ModUnitData>();
		ClassInjector.RegisterTypeInIl2Cpp<ModularSA>();
		//ClassInjector.RegisterTypeInIl2Cpp<ModularSA.BattleUnitComparer>();
		harmony.PatchAll(typeof(SkillScriptInitPatch));
		harmony.PatchAll(typeof(StagePatches));
		harmony.PatchAll(typeof(UniquePatches));
		harmony.PatchAll(typeof(LogoPlayerPatches));
		harmony.PatchAll(typeof(ReloadPatches));
		if (fakepowerEnabled) harmony.PatchAll(typeof(FakePowerPatches));
		RegisterConsequences();
		RegisterAcquirers();
	}

	private static void RegisterConsequences()
	{
		consequenceDict["log"] = new ConsequenceLog();
		consequenceDict["base"] = new ConsequenceBase();
		consequenceDict["final"] = new ConsequenceFinal();
		consequenceDict["clash"] = new ConsequenceClash();
		consequenceDict["scale"] = new ConsequenceScale();
		consequenceDict["dmgadd"] = new ConsequenceDmgAdd();
		consequenceDict["dmgmult"] = new ConsequenceDmgMult();
		consequenceDict["mpdmg"] = new ConsequenceMpDmg();
		consequenceDict["reusecoin"] = new ConsequenceReuseCoin();
		consequenceDict["bonusdmg"] = new ConsequenceBonusDmg();
		consequenceDict["buf"] = new ConsequenceBuf();
		consequenceDict["shield"] = new ConsequenceShield();
		consequenceDict["break"] = new ConsequenceBreak();
		consequenceDict["breakdmg"] = new ConsequenceBreakDmg();
		consequenceDict["breakrecover"] = new ConsequenceBreakRecover();
		consequenceDict["breakaddbar"] = new ConsequenceBreakAddBar();
		consequenceDict["explosion"] = new ConsequenceExplosion();
		consequenceDict["healhp"] = new ConsequenceHealHp();
		consequenceDict["pattern"] = new ConsequencePattern();
		consequenceDict["setslotadder"] = new ConsequenceSetSlotAdder();
		consequenceDict["setdata"] = new ConsequenceSetData();
		consequenceDict["changeskill"] = new ConsequenceChangeSkill();
		consequenceDict["setimmortal"] = new ConsequenceSetImmortal();
		consequenceDict["retreat"] = new ConsequenceRetreat();
		consequenceDict["aggro"] = new ConsequenceAggro();
		consequenceDict["skillsend"] = new ConsequenceSkillSend();
		consequenceDict["skillreuse"] = new ConsequenceSkillReuse();
		consequenceDict["skillslotreplace"] = new ConsequenceSkillSlotReplace();
		consequenceDict["resource"] = new ConsequenceResource();
		consequenceDict["discard"] = new ConsequenceDiscard();
		consequenceDict["passiveadd"] = new ConsequencePassiveAdd();
		consequenceDict["passiveremove"] = new ConsequencePassiveRemove();
		consequenceDict["endstage"] = new ConsequenceEndStage();
		consequenceDict["endbattle"] = new ConsequenceEndBattle();
		consequenceDict["endlimbus"] = new ConsequenceEndLimbus();
		consequenceDict["skillcanduel"] = new ConsequenceSkillCanDuel();
		consequenceDict["skillslotgive"] = new ConsequenceSkillSlotGive();
		consequenceDict["doubleslot"] = new ConsequenceDoubleSlot();
		consequenceDict["stageextraslot"] = new ConsequenceStageExtraSlot();
		consequenceDict["passivereveal"] = new ConsequencePassiveReveal();
		consequenceDict["skillreveal"] = new ConsequenceSkillReveal();
		consequenceDict["resistreveal"] = new ConsequenceResistReveal();
		consequenceDict["appearance"] = new ConsequenceAppearance();
		consequenceDict["coincancel"] = new ConsequenceCoinCancel();
		consequenceDict["summonassistant"] = new ConsequenceSummonAssistant();
		consequenceDict["summonenemy"] = new ConsequenceSummonEnemy();
		consequenceDict["summonunitfromqueue"] = new ConsequenceSummonUnitFromQueue();
		consequenceDict["stack"] = new ConsequenceStack();
		consequenceDict["turn"] = new ConsequenceTurn();
		consequenceDict["vibrationswitch"] = new ConsequenceVibrationSwitch();
		consequenceDict["changemap"] = new ConsequenceChangeMap();
		consequenceDict["lyrics"] = new ConsequenceLyrics();
		consequenceDict["uppertext"] = new ConsequenceUpperText();
		consequenceDict["battledialogline"] = new ConsequenceBattleDialogLine();
		consequenceDict["gnome"] = new ConsequenceGnome();
		consequenceDict["effectlabel"] = new ConsequenceEffectLabel();
		consequenceDict["sanchoshield"] = new ConsequenceSanchoShield();
		consequenceDict["sound"] = new ConsequenceSound();
		consequenceDict["addability"] = new ConsequenceAddAbility();
		consequenceDict["removeability"] = new ConsequenceRemoveAbility();
		consequenceDict["surge"] = new ConsequenceSurge();
		consequenceDict["makeunbreakable"] = new ConsequenceMakeUnbreakable();
		consequenceDict["bloodfeast"] = new ConsequenceBloodfeast();
		consequenceDict["critchance"] = new ConsequenceCritChance();
		consequenceDict["changemotion"] = new ConsequenceChangeMotion();
		consequenceDict["assistdefense"] = new ConsequenceAssistDefense();
		consequenceDict["ignorepanic"] = new ConsequenceIgnorePanic();
		consequenceDict["ignorebreak"] = new ConsequenceIgnoreBreak();
		consequenceDict["skillslotremove"] = new ConsequenceSkillSlotRemove();
		consequenceDict["changeaffinity"] = new ConsequenceChangeAffinity();
	}

	private static void RegisterAcquirers()
	{
 		acquirerDict["getskilllevel"] = new AcquirerGetSkillLevel();
 		acquirerDict["getcharacterid"] = new AcquirerGetCharacterID();
 		acquirerDict["getlevel"] = new AcquirerGetLevel();
		acquirerDict["math"] = new AcquirerMath();
		acquirerDict["mpcheck"] = new AcquirerMpCheck();
		acquirerDict["hpcheck"] = new AcquirerHpCheck();
		acquirerDict["bufcheck"] = new AcquirerBufCheck();
		acquirerDict["timeget"] = new AcquirerTimeGet();
		acquirerDict["getdmg"] = new AcquirerGetDmg();
		acquirerDict["gethpdmg"] = new AcquirerGetHpDmg();
		acquirerDict["round"] = new AcquirerRound();
		acquirerDict["wave"] = new AcquirerWave();
		acquirerDict["activations"] = new AcquirerActivations();
		acquirerDict["unitstate"] = new AcquirerUnitState();
		acquirerDict["getid"] = new AcquirerGetId();
		acquirerDict["instid"] = new AcquirerInstId();
		acquirerDict["speedcheck"] = new AcquirerSpeedCheck();
		acquirerDict["getpattern"] = new AcquirerGetPattern();
		acquirerDict["getabnoslotmax"] = new AcquirerGetAbnoSlotMax();
		acquirerDict["getdata"] = new AcquirerGetData();
		acquirerDict["deadallies"] = new AcquirerDeadAllies();
		acquirerDict["random"] = new AcquirerRandom();
		acquirerDict["areallied"] = new AcquirerAreAllied();
		acquirerDict["getshield"] = new AcquirerGetShield();
		acquirerDict["getdmgtaken"] = new AcquirerGetDmgTaken();
		acquirerDict["getbuffcount"] = new AcquirerGetBuffCount();
		acquirerDict["getskillid"] = new AcquirerGetSkillId();
		acquirerDict["getcoincount"] = new AcquirerGetCoinCount();
		acquirerDict["allcoinstate"] = new AcquirerAllCoinState();
		acquirerDict["resonance"] = new AcquirerResonance();
		acquirerDict["resource"] = new AcquirerResource();
		acquirerDict["haskey"] = new AcquirerHasKey();
		acquirerDict["skillbase"] = new AcquirerSkillBase();
		acquirerDict["skillatkweight"] = new AcquirerSkillAtkWeight();
		acquirerDict["onescale"] = new AcquirerOneScale();
		acquirerDict["skillatk"] = new AcquirerSkillAtk();
		acquirerDict["skillatklevel"] = new AcquirerSkillAtkLevel();
		acquirerDict["skillattribute"] = new AcquirerSkillAttribute();
		acquirerDict["skilldeftype"] = new AcquirerSkillDefType();
		acquirerDict["skillegotype"] = new AcquirerSkillEgoType();
		acquirerDict["skillrank"] = new AcquirerSkillRank();
		acquirerDict["skillslotcount"] = new AcquirerSkillSlotCount();
		acquirerDict["amountattacks"] = new AcquirerAmountAttacks();
		acquirerDict["getstat"] = new AcquirerGetStat();
		acquirerDict["coinisbroken"] = new AcquirerCoinIsBroken();
		acquirerDict["stack"] = new AcquirerStack();
		acquirerDict["turn"] = new AcquirerTurn();
		acquirerDict["isfocused"] = new AcquirerIsFocused();
		acquirerDict["unitcount"] = new AcquirerUnitCount();
		acquirerDict["breakcount"] = new AcquirerBreakCount();
		acquirerDict["breakvalue"] = new AcquirerBreakValue();
		acquirerDict["coinrerolled"] = new AcquirerCoinRerolled();
		acquirerDict["stageextraslot"] = new AcquirerStageExtraSlot();
		acquirerDict["getbloodfeast"] = new AcquirerGetBloodfeast();
  	acquirerDict["isunbreakable"] = new AcquirerIsUnbreakable();
		acquirerDict["isusableinduel"] = new AcquirerIsUsableInDuel();
		acquirerDict["sameunit"] = new AcquirerSameUnit();
		acquirerDict["skillcanduel"] = new AcquirerSkillCanDuel();
		acquirerDict["skillteamkill"] = new AcquirerSkillTeamKill();
		acquirerDict["skillfixedtarget"] = new AcquirerSkillFixed();
	}

	public static System.Collections.Generic.List<BattleUnitModel> ShuffleUnits(
		System.Collections.Generic.List<BattleUnitModel> list)
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

	//public static ModsaEvaluator modsaEval = null;
	public static readonly System.Collections.Generic.Dictionary<string, int> timingDict = new();
	public static readonly System.Collections.Generic.Dictionary<string, IModularConsequence> consequenceDict = new();
	public static readonly System.Collections.Generic.Dictionary<string, IModularAcquirer> acquirerDict = new();
	public static System.Collections.Generic.List<SupporterPassiveModel> supporterPassiveList = new System.Collections.Generic.List<SupporterPassiveModel>();
	public static System.Collections.Generic.List<SupporterPassiveModel> activeSupporterPassiveList = new System.Collections.Generic.List<SupporterPassiveModel>();
	public static bool fakepowerEnabled = true;
	public static bool SupportPasInit = false;

	public static Random rng = new();

	public static readonly Regex sWhitespace = new(@"\s+");
	public static readonly Regex mathsymbolRegex = new("(-|\\+|\\*|%|!|¡|\\?)");
	public static readonly char[] mathSeparator = new[] { '-', '+', '*', '%', '!', '¡', '?' };

	public static bool logEnabled = false;

	public const string NAME = "ModularSkillScripts";
	public const string VERSION = "4.2.5";
	public const string AUTHOR = "GlitchGames";
	public const string GUID = $"{AUTHOR}.{NAME}";

	public static ManualLogSource Logg;
 	public static DirectoryInfo pluginPath = Directory.CreateDirectory(Path.Combine(Paths.PluginPath, "Lethe", "ModTemplate", "modular_lua"));
}

