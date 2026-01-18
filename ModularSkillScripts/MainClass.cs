using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Text.RegularExpressions;
using ModularSkillScripts.Acquirer;
using BepInEx.Configuration;
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
		timingStringList.Add("WhenUse"); // 1
		timingStringList.Add("BeforeAttack"); // 2
		timingStringList.Add("StartDuel"); // 3
		timingStringList.Add("WinDuel"); // 4
		timingStringList.Add("DefeatDuel"); // 5
		timingStringList.Add("EndBattle"); // 6
		timingStringList.Add("OnSucceedAttack"); // 7
		timingStringList.Add("WhenHit"); // 8
		timingStringList.Add("EndSkill"); // 9
		timingStringList.Add("FakePower"); // 10
		timingStringList.Add("BeforeDefense"); // 11
		timingStringList.Add("OnDie"); // 12
		timingStringList.Add("OnOtherDie"); // 13 
		timingStringList.Add("DuelClash"); // 14
		timingStringList.Add("DuelClashAfter"); // 15
		timingStringList.Add("OnSucceedEvade"); // 16
		timingStringList.Add("OnDefeatEvade"); // 17
		timingStringList.Add("OnStartBehaviour"); // 18
		timingStringList.Add("BeforeBehaviour"); // 19
		timingStringList.Add("OnEndBehaviour"); // 20
		timingStringList.Add("EnemyKill"); // 21
		timingStringList.Add("OnBreak"); // 22
		timingStringList.Add("OnOtherBreak"); // 23
		timingStringList.Add("OnDiscard"); // 24
		timingStringList.Add("OnZeroHP"); // 25
		timingStringList.Add("EnemyEndSkill"); // 26
		timingStringList.Add("OnOtherBurst"); // 27
		timingStringList.Add("BeforeSA"); // 28
		timingStringList.Add("BeforeWhenHit"); // 29
		timingStringList.Add("BeforeUse"); // 30
		timingStringList.Add("Immortal"); // 31
		timingStringList.Add("ImmortalOther"); // 32
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
		timingStringList.Add("OnRetreat"); // 44
		timingStringList.Add("OnGainBuff"); // 45
		timingStringList.Add("EncounterStart"); // 46
		timingStringList.Add("WinParrying"); // 47
		timingStringList.Add("DefeatParrying"); // 48

		Il2CppArrayBase<string> timingStringArray = timingStringList.ToArray();
		int count = timingStringArray.Count;
		for (int i = 0; i < count; i++) timingDict.Add(timingStringArray[i], i);

		timingDict.Add("RoundStart", -1);
		timingDict.Add("OSA", timingDict["OnSucceedAttack"]);
		timingDict.Add("WH", timingDict["WhenHit"]);
		timingDict.Add("BSA", timingDict["BeforeSA"]);
		timingDict.Add("BWH", timingDict["BeforeWhenHit"]);
		timingDict.Add("SBS", timingDict["StartBattleSkill"]);

		// legacy to new stuff translator
		timingDict.Add("StartBehaviour", timingDict["OnStartBehaviour"]);
		timingDict.Add("EndBehaviour", timingDict["OnEndBehaviour"]);
		timingDict.Add("OnImmortal", timingDict["Immortal"]);
		timingDict.Add("OnOtherImmortal", timingDict["ImmortalOther"]);
		timingDict.Add("OnVisualCoinToss", timingDict["StartVisualCoinToss"]);
		timingDict.Add("OnVisualUse", timingDict["StartVisualSkillUse"]);
		Harmony harmony = new Harmony(NAME);
		Logg = new ManualLogSource(NAME);
		BepInEx.Logging.Logger.Sources.Add(Logg);
		EnableLogging = Config.Bind(
				"Logging",
				"EnableLogging",
				true,
				"Enable or disable logs"
		);
		ClassInjector.RegisterTypeInIl2Cpp<DataMod>();
		ClassInjector.RegisterTypeInIl2Cpp<ModUnitData>();
		ClassInjector.RegisterTypeInIl2Cpp<ModularSA>();
		//ClassInjector.RegisterTypeInIl2Cpp<ModularSA.BattleUnitComparer>();
		harmony.PatchAll(typeof(SkillScriptInitPatch));
		harmony.PatchAll(typeof(StagePatches));
		harmony.PatchAll(typeof(UniquePatches));
		harmony.PatchAll(typeof(LogoPlayerPatches));
		harmony.PatchAll(typeof(ReloadPatches));
		harmony.PatchAll(typeof(OnGainBuffPatches));
		harmony.PatchAll(typeof(LogPatches));
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
		consequenceDict["healsp"] = new ConsequenceMpDmg();
		consequenceDict["reusecoin"] = new ConsequenceReuseCoin();
		consequenceDict["bonusdmg"] = new ConsequenceBonusDmg();
		consequenceDict["buff"] = new ConsequenceBuf();
		consequenceDict["shield"] = new ConsequenceShield();
		consequenceDict["break"] = new ConsequenceBreak();
		consequenceDict["breakdmg"] = new ConsequenceBreakDmg();
		consequenceDict["breakrecover"] = new ConsequenceBreakRecover();
		consequenceDict["breakaddbar"] = new ConsequenceBreakAddBar();
		consequenceDict["burst"] = new ConsequenceExplosion();
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
		consequenceDict["deluge"] = new ConsequenceSurge();
		consequenceDict["makeunbreakable"] = new ConsequenceMakeUnbreakable();
		consequenceDict["bloodfeast"] = new ConsequenceBloodfeast();
		consequenceDict["critchance"] = new ConsequenceCritChance();
		consequenceDict["changemotion"] = new ConsequenceChangeMotion();
		consequenceDict["assistdefense"] = new ConsequenceAssistDefense();
		consequenceDict["ignorepanic"] = new ConsequenceIgnorePanic();
		consequenceDict["ignorebreak"] = new ConsequenceIgnoreBreak();
		consequenceDict["skillslotremove"] = new ConsequenceSkillSlotRemove();
		consequenceDict["changeaffinity"] = new ConsequenceChangeAffinity();
		consequenceDict["winstage"] = new ConsequenceWinStage();
		consequenceDict["ovwatkres"] = new ConsequenceOverwriteAtkResist();
		consequenceDict["ovwsinres"] = new ConsequenceOverwriteSinResist();
		consequenceDict["refreshspeed"] = new ConsequenceRefreshSpeed();
		consequenceDict["deactivebreak"] = new ConsequenceDeactiveBreak();
		consequenceDict["defcorrection"] = new ConsequenceDefCorrection();
		consequenceDict["buffcategory"] = new ConsequenceBuffCategory();
		consequenceDict["destroybuff"] = new ConsequenceDestroyBuff();

		// legacy consequences
		consequenceDict["mpdmg"] = new ConsequenceMpDmg();
		consequenceDict["buf"] = new ConsequenceBuf();
		consequenceDict["explosion"] = new ConsequenceExplosion();
		consequenceDict["surge"] = new ConsequenceSurge();
		consequenceDict["bufcategory"] = new ConsequenceBuffCategory();

	}

	private static void RegisterAcquirers()
	{
		acquirerDict["getskilllevel"] = new AcquirerGetSkillLevel();
		acquirerDict["getcharacterid"] = new AcquirerGetCharacterID();
		acquirerDict["getlevel"] = new AcquirerGetLevel();
		acquirerDict["math"] = new AcquirerMath();
		acquirerDict["getsp"] = new AcquirerMpCheck();
		acquirerDict["gethp"] = new AcquirerHpCheck();
		acquirerDict["getbuff"] = new AcquirerBufCheck();
		acquirerDict["gettime"] = new AcquirerTimeGet();
		acquirerDict["getdmg"] = new AcquirerGetDmg();
		acquirerDict["gethpdmg"] = new AcquirerGetHpDmg();
		acquirerDict["getround"] = new AcquirerRound();
		acquirerDict["getwave"] = new AcquirerWave();
		acquirerDict["getactivations"] = new AcquirerActivations();
		acquirerDict["getunitstate"] = new AcquirerUnitState();
		acquirerDict["getid"] = new AcquirerGetId();
		acquirerDict["getinstid"] = new AcquirerInstId();
		acquirerDict["getspeed"] = new AcquirerSpeedCheck();
		acquirerDict["getpattern"] = new AcquirerGetPattern();
		acquirerDict["getabnoslotmax"] = new AcquirerGetAbnoSlotMax();
		acquirerDict["getdata"] = new AcquirerGetData();
		acquirerDict["getdeadallies"] = new AcquirerDeadAllies();
		acquirerDict["random"] = new AcquirerRandom();
		acquirerDict["areallies"] = new AcquirerAreAllied();
		acquirerDict["getshield"] = new AcquirerGetShield();
		acquirerDict["getdmgtaken"] = new AcquirerGetDmgTaken();
		acquirerDict["getbuffcount"] = new AcquirerGetBuffCount();
		acquirerDict["getskillid"] = new AcquirerGetSkillId();
		acquirerDict["getcoincount"] = new AcquirerGetCoinCount();
		acquirerDict["getallcoinstates"] = new AcquirerAllCoinState();
		acquirerDict["getresonance"] = new AcquirerResonance();
		acquirerDict["getresource"] = new AcquirerResource();
		acquirerDict["haskey"] = new AcquirerHasKey();
		acquirerDict["getskillbase"] = new AcquirerSkillBase();
		acquirerDict["getskillatkweight"] = new AcquirerSkillAtkWeight();
		acquirerDict["getcoinscale"] = new AcquirerOneScale();
		acquirerDict["getskillatk"] = new AcquirerSkillAtk();
		acquirerDict["getskillatklevel"] = new AcquirerSkillAtkLevel();
		acquirerDict["getskillattribute"] = new AcquirerSkillAttribute();
		acquirerDict["getskilldeftype"] = new AcquirerSkillDefType();
		acquirerDict["getskillegotype"] = new AcquirerSkillEgoType();
		acquirerDict["getskillrank"] = new AcquirerSkillRank();
		acquirerDict["getskillslotcount"] = new AcquirerSkillSlotCount();
		acquirerDict["getattackamount"] = new AcquirerAmountAttacks();
		acquirerDict["getstat"] = new AcquirerGetStat();
		acquirerDict["iscoinbroken"] = new AcquirerCoinIsBroken();
		acquirerDict["stack"] = new AcquirerStack();
		acquirerDict["turn"] = new AcquirerTurn();
		acquirerDict["isfocused"] = new AcquirerIsFocused();
		acquirerDict["getunitcount"] = new AcquirerUnitCount();
		acquirerDict["getbreakcount"] = new AcquirerBreakCount();
		acquirerDict["getbreakvalue"] = new AcquirerBreakValue();
		acquirerDict["iscoinrerolled"] = new AcquirerCoinRerolled();
		acquirerDict["stageextraslot"] = new AcquirerStageExtraSlot();
		acquirerDict["getbloodfeast"] = new AcquirerGetBloodfeast();
		acquirerDict["isunbreakable"] = new AcquirerIsUnbreakable();
		acquirerDict["isusableinduel"] = new AcquirerIsUsableInDuel();
		acquirerDict["issameunit"] = new AcquirerSameUnit();
		acquirerDict["getskillcanduel"] = new AcquirerSkillCanDuel();
		acquirerDict["getskillteamkill"] = new AcquirerSkillTeamKill();
		acquirerDict["getskillfixedtarget"] = new AcquirerSkillFixed();
		acquirerDict["getcoinoperator"] = new AcquirerCoinOperator();
		acquirerDict["getbufftype"] = new AcquirerBuffType();
		acquirerDict["getatkres"] = new AcquirerGetAtkResSinner();
		acquirerDict["getsinres"] = new AcquirerGetSinResSinner();
		acquirerDict["hasuseddefense"] = new AcquirerUsedDefenseActionThisTurn();
		acquirerDict["getunitfaction"] = new AcquirerUnitFaction();
		acquirerDict["gbstack"] = new AcquirerGainBuffStack();
		acquirerDict["gbturn"] = new AcquirerGainBuffTurn();
		acquirerDict["gbactiveround"] = new AcquirerGainBuffActiveRound();
		acquirerDict["gbsource"] = new AcquirerGainBuffSource();
		acquirerDict["getchainstatus"] = new AcquirerChainStatus();
		acquirerDict["getsinsindashboard"] = new AcquirerSinsInDashboard();

		// legacy acquirers
		acquirerDict["hpcheck"] = new AcquirerHpCheck();
		acquirerDict["mpcheck"] = new AcquirerMpCheck();
		acquirerDict["bufcheck"] = new AcquirerBufCheck();
		acquirerDict["timeget"] = new AcquirerTimeGet();
		acquirerDict["round"] = new AcquirerRound();
		acquirerDict["wave"] = new AcquirerWave();
		acquirerDict["activations"] = new AcquirerActivations();
		acquirerDict["unitstate"] = new AcquirerUnitState();
		acquirerDict["instid"] = new AcquirerInstId();
		acquirerDict["speedcheck"] = new AcquirerSpeedCheck();
		acquirerDict["deadallies"] = new AcquirerDeadAllies();
		acquirerDict["areallied"] = new AcquirerAreAllied();
		acquirerDict["allcoinstate"] = new AcquirerAllCoinState();
		acquirerDict["resonance"] = new AcquirerResonance();
		acquirerDict["resource"] = new AcquirerResource(); 
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
		acquirerDict["coinisbroken"] = new AcquirerCoinIsBroken();
		acquirerDict["unitcount"] = new AcquirerUnitCount();
		acquirerDict["breakcount"] = new AcquirerBreakCount();
		acquirerDict["breakvalue"] = new AcquirerBreakValue();
		acquirerDict["coinrerolled"] = new AcquirerCoinRerolled();
		acquirerDict["sameunit"] = new AcquirerSameUnit();
		acquirerDict["skillcanduel"] = new AcquirerSkillCanDuel();
		acquirerDict["skillteamkill"] = new AcquirerSkillTeamKill();
		acquirerDict["skillfixedtarget"] = new AcquirerSkillFixed();
		acquirerDict["coinoperator"] = new AcquirerCoinOperator();
		acquirerDict["bufftype"] = new AcquirerBuffType();
		acquirerDict["useddefaction"] = new AcquirerUsedDefenseActionThisTurn();
		acquirerDict["unitfaction"] = new AcquirerUnitFaction();
		acquirerDict["chainstatus"] = new AcquirerChainStatus();



		// Register Lua functions
		luaFunctionDict["clearvalues"] = new ModularSkillScripts.LuaFunction.LuaFunctionClearValues();
		luaFunctionDict["resetadders"] = new ModularSkillScripts.LuaFunction.LuaFunctionResetAdders();
		luaFunctionDict["selecttargets"] = new ModularSkillScripts.LuaFunction.LuaFunctionSelectTargets();
		luaFunctionDict["setldata"] = new ModularSkillScripts.LuaFunction.LuaFunctionSetLData();
		luaFunctionDict["getldata"] = new ModularSkillScripts.LuaFunction.LuaFunctionGetLData();
		luaFunctionDict["readfile"] = new ModularSkillScripts.LuaFunction.LuaFunctionReadFile();
		luaFunctionDict["listfiles"] = new ModularSkillScripts.LuaFunction.LuaFunctionListFiles();
		luaFunctionDict["jsontolua"] = new ModularSkillScripts.LuaFunction.LuaFunctionJsonToLua();
		luaFunctionDict["listdirectories"] = new ModularSkillScripts.LuaFunction.LuaFunctionListDirectories();
		luaFunctionDict["listbuffs"] = new ModularSkillScripts.LuaFunction.LuaFunctionListBuffs();
		luaFunctionDict["setgdata"] = new ModularSkillScripts.LuaFunction.LuaFunctionSetGData();
		luaFunctionDict["getgdata"] = new ModularSkillScripts.LuaFunction.LuaFunctionGetGData();
		luaFunctionDict["clearallgdata"] = new ModularSkillScripts.LuaFunction.LuaFunctionClearAllGData();
		luaFunctionDict["gbkeyword"] = new ModularSkillScripts.LuaFunction.LuaFunctionGainBuffKeyword();
	}

	public static System.Collections.Generic.List<BattleUnitModel> ShuffleUnits(
		System.Collections.Generic.List<BattleUnitModel> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = MainClass.rng.Next(n + 1); // 0 to X Exclusive upper bound
			if (k == n) continue; // No change to list
			BattleUnitModel hold_value = list[k];
			list[k] = list[n];
			list[n] = hold_value;
		}

		return list;
	}
	public static System.Collections.Generic.KeyValuePair<string, int> GetMaxValue(
			System.Collections.Generic.Dictionary<string, int> dict)
	{
		var enumerator = dict.GetEnumerator();
		enumerator.MoveNext();

		var max = enumerator.Current;

		while (enumerator.MoveNext())
		{
			if (enumerator.Current.Value > max.Value)
				max = enumerator.Current;
		}

		return max;
	}




	//public static ModsaEvaluator modsaEval = null;
	public static readonly System.Collections.Generic.Dictionary<string, int> timingDict = new();
	public static readonly System.Collections.Generic.Dictionary<string, IModularConsequence> consequenceDict = new();
	public static readonly System.Collections.Generic.Dictionary<string, IModularAcquirer> acquirerDict = new();
	public static readonly System.Collections.Generic.Dictionary<string, ModularSkillScripts.LuaFunction.IModularLuaFunction> luaFunctionDict = new();
	public static System.Collections.Generic.List<SupporterPassiveModel> supporterPassiveList = new System.Collections.Generic.List<SupporterPassiveModel>();
	public static System.Collections.Generic.List<SupporterPassiveModel> activeSupporterPassiveList = new System.Collections.Generic.List<SupporterPassiveModel>();
	public static bool fakepowerEnabled = true;
	public static bool SupportPasInit = false;

	public static Random rng = new();

	public static readonly Regex sWhitespace = new(@"\s+");
	public static readonly Regex mathsymbolRegex = new("(-|\\+|\\*|%|!|¡|\\?)");
	public static readonly char[] mathSeparator = new[] { '-', '+', '*', '%', '!', '¡', '?' };

	public static ConfigEntry<bool> EnableLogging;

	public const string NAME = "ModularSkillScripts";
	public const string VERSION = "4.4.1";
	public const string AUTHOR = "GlitchGames";
	public const string GUID = $"{AUTHOR}.{NAME}";

	public static ManualLogSource Logg;
	public static DirectoryInfo pluginPath = Directory.CreateDirectory(Path.Combine(Paths.PluginPath, "Lethe", "ModTemplate", "modular_lua"));
}

