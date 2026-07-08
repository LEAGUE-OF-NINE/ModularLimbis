using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using BepInEx.Configuration;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem;
using ModularSkillScripts.Consequence;
using Random = System.Random;
using Il2CppSystem.IO;
using ModularSkillScripts.Acquirer;
using ModularSkillScripts.Patches;
using Il2CppSystem.Text.RegularExpressions;
using StringSplitOptions = System.StringSplitOptions;

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
		List<string> timingStringList =
		[
			"StartBattle", // 0
			"WhenUse", // 1
			"BeforeAttack", // 2
			"StartDuel", // 3
			"WinDuel", // 4
			"DefeatDuel", // 5
			"EndBattle", // 6
			"OnSucceedAttack", // 7
			"WhenHit", // 8
			"EndSkill", // 9
			"FakePower", // 10
			"BeforeDefense", // 11
			"OnDie", // 12
			"OnOtherDie", // 13
			"DuelClash", // 14
			"DuelClashAfter", // 15
			"OnSucceedEvade", // 16
			"OnDefeatEvade", // 17
			"OnStartBehaviour", // 18
			"BeforeBehaviour", // 19
			"OnEndBehaviour", // 20
			"EnemyKill", // 21
			"OnBreak", // 22
			"OnOtherBreak", // 23
			"OnDiscard", // 24
			"OnZeroHP", // 25
			"EnemyEndSkill", // 26
			"OnOtherBurst", // 27
			"BeforeSA", // 28
			"BeforeWhenHit", // 29
			"BeforeUse", // 30
			"Immortal", // 31
			"ImmortalOther", // 32
			"SpecialAction", // 33
			"AfterSlots",
			"AfterSlotsReady",
			"OnCoinToss",
			"StartBattleSkill",
			"OnBurst",
			"StartVisualCoinToss",
			"StartVisualSkillUse",
			"WhenGained",
			"ChangeMotion",
			"IgnorePanic",
			"IgnoreBreak",
			"OnRetreat",
			"OnGainBuff",
			"OnUseBuff",
			"EncounterStart",
			"WinParrying",
			"DefeatParrying",
			"ChangeTakeDamage",
			"OnCoinAfterAttack",
			"EnemyStartBehaviour",
			"AfterChangeShield",
			"AfterChangeHP",
			"CanDealTarget",
			//timingStringList.Add("ChangeSinBuffDamage");
			"DelayedStart" // HBMBACMAB
		];
		//timingStringList.Add("ChangeSinBuffDamage");

		int count = timingStringList.Count;
		for (int i = 0; i < count; i++) timingDict.Add(timingStringList[i], i);

		timingDict.Add("RoundStart", -1);
		timingDict.Add("OSA", timingDict["OnSucceedAttack"]);
		timingDict.Add("WH", timingDict["WhenHit"]);
		timingDict.Add("BSA", timingDict["BeforeSA"]);
		timingDict.Add("BWH", timingDict["BeforeWhenHit"]);
		timingDict.Add("SBS", timingDict["StartBattleSkill"]);
		timingDict.Add("OnUseBuf", timingDict["OnUseBuff"]);

		// legacy to new stuff translator
		timingDict.Add("StartBehaviour", timingDict["OnStartBehaviour"]);
		timingDict.Add("EndBehaviour", timingDict["OnEndBehaviour"]);
		timingDict.Add("EnemyStartBehavior", timingDict["EnemyStartBehaviour"]);
		timingDict.Add("OnImmortal", timingDict["Immortal"]);
		timingDict.Add("OnOtherImmortal", timingDict["ImmortalOther"]);
		timingDict.Add("OnVisualCoinToss", timingDict["StartVisualCoinToss"]);
		timingDict.Add("OnVisualUse", timingDict["StartVisualSkillUse"]);
		
		FakePowerPatches.actevent_FakePower = timingDict["FakePower"];
		
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
		ClassInjector.RegisterTypeInIl2Cpp<SkillScriptInitPatch.CoroutineRunner>();

		//ClassInjector.RegisterTypeInIl2Cpp<ModularSA.BattleUnitComparer>();
		harmony.PatchAll(typeof(SkillScriptInitPatch));
		harmony.PatchAll(typeof(StagePatches));
		harmony.PatchAll(typeof(UniquePatches));
		// harmony.PatchAll(typeof(LogoPlayerPatches));
		harmony.PatchAll(typeof(ReloadPatches));
		harmony.PatchAll(typeof(OnGainBuffPatches));
		// We don't do this bullshit
		//harmony.PatchAll(typeof(LogPatches));
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
		consequenceDict["critratio"] = new ConsequenceCritRatio();
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
		consequenceDict["setdmgtaken"] = new ConsequenceSetChangeDamageTaken();
		consequenceDict["skillhide"] = new ConsequenceSkillHide();
		consequenceDict["lbreak"] = new ConsequenceBreak();
		consequenceDict["removecoin"] = new ConsequenceRemoveCoin();
		consequenceDict["setlevel"] = new ConsequenceSetLevel();
		consequenceDict["setmaxhp"] = new ConsequenceSetMaxHp();
		consequenceDict["changesp"] = new ConsequenceChangeSp();
		consequenceDict["adddefaultskillbyid"] = new ConsequenceAddDefaultSkillByID();
		consequenceDict["addskilltopool"] = new ConsequenceAddSkillToPool();
		consequenceDict["dropskill"] = new ConsequenceDropSkill();
		consequenceDict["giveskillscript"] = new ConsequenceGiveSkillScript();
		consequenceDict["appearancelocalscale"] = new ConsequenceAppearanceLocalScale();
		consequenceDict["appearancelocaleuler"] = new ConsequenceAppearanceLocalEuler();
		consequenceDict["bonusdmgbybuff"] = new ConsequenceBonusDmgByBuff();
		//consequenceDict["sinbuffmult"] = new ConsequenceSinBuffMult();
		consequenceDict["atkweight"] = new ConsequenceAtkWeight();
		consequenceDict["changeatktype"] = new ConsequenceChangeAtkType();
		consequenceDict["setslotweight"] = new ConsequenceSetSlotWeight();
		consequenceDict["bufcategory"] = new ConsequenceBuffCategory();
		consequenceDict["cutinaction"] = new ConsequenceCutInAction();
		consequenceDict["tagforsort"] = new ConsequenceTagForSort();
		consequenceDict["insertskill"] = new ConsequenceInsertSkill();
		consequenceDict["refreshallslotvisual"] = new ConsequenceRefreshAllSlotVisual();
		consequenceDict["shine"] = new ConsequenceShine();
		consequenceDict["dashboardeffect"] = new ConsequenceDashboardEffect();
		consequenceDict["setslotalarm"] = new ConsequenceSetSlotAlarm();
		consequenceDict["addduel"] = new ConsequenceAddDuel();
		consequenceDict["setspusage"] = new ConsequenceSetSpUsage();
		consequenceDict["instantdeath"] = new ConsequenceInstantDeath();

		// legacy consequences
		consequenceDict["mpdmg"] = new ConsequenceMpDmg();
		consequenceDict["buf"] = new ConsequenceBuf();
		consequenceDict["explosion"] = new ConsequenceExplosion();
		consequenceDict["surge"] = new ConsequenceSurge();
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
		acquirerDict["resourcegetenum"] = new AcquirerResourceGetEnum();
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
		acquirerDict["onusebufstack"] = new AcquirerStack(1);
		acquirerDict["onusebufturn"] = new AcquirerTurn(1);
		acquirerDict["onusebufiskeyword"] = new AcquirerOnUseBufIsKeyword();
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
		acquirerDict["coinindex"] = new AcquirerCoinIndex();
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
		acquirerDict["haspassive"] = new AcquirerHasPassive();
		acquirerDict["ctdsource"] = new AcquirerChangeDamageSource();
		acquirerDict["countbackup"] = new AcquirerCountBackup();
		acquirerDict["isbackupenabled"] = new AcquirerIsBackupEnabled();
		acquirerDict["getskillskillslot"] = new AcquirerGetSkillSkillSlot();
		acquirerDict["hasdashboardeffect"] = new AcquirerHasDashboardEffect();
		acquirerDict["getuptielevel"] = new AcquirerGetUptieLevel();
		acquirerDict["getbreaklevel"] = new AcquirerGetBreakLevel();

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
		acquirerDict["getopposkillid"] = new AcquirerGetOpponentSkillId();
		acquirerDict["getcurrentpower"] = new AcquirerGetCurrentPower();
		acquirerDict["getdefaultmaxhp"] = new AcquirerGetDefaultMaxHp();
		acquirerDict["gethpincrement"] = new AcquirerGetHpIncrementByLevel();
		acquirerDict["diduseskilllastturn"] = new AcquirerDidUseSkillLastTurn();
		acquirerDict["isactionable"] = new AcquirerIsActionable();

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
		luaFunctionDict["addresource"] = new ModularSkillScripts.LuaFunction.LuaFunctionAddResource();
		luaFunctionDict["getcurrentmapid"] = new ModularSkillScripts.LuaFunction.LuaFunctionGetCurrentMapID();
		luaFunctionDict["getappearanceid"] = new ModularSkillScripts.LuaFunction.LuaFunctionGetAppearanceID();
		luaFunctionDict["listbreakvalues"] = new ModularSkillScripts.LuaFunction.LuaFunctionListBreakSectionValue();
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

	public static UnitSinModel GetSinInUnit(string selection, BattleUnitModel unitOrNull, BattleActionModel actionOrNull, int trySlotID = -1)
	{
		SinActionModel sinAction = null;
		if (trySlotID >= 0)
		{
			if (unitOrNull == null) return null;

			trySlotID = Math.Min(trySlotID, unitOrNull.GetPermanentSinActionListCount());
			sinAction = unitOrNull.GetSinActionList().ToArray()[trySlotID];
		}
		else if (actionOrNull != null) sinAction = actionOrNull.SinAction;
		if (sinAction == null) return null;

		UnitSinModel sin = null;
		if (selection == "replaced") sin = sinAction.GetReplacedSinByDefenseSkill();
		else if (selection == "ready") sin = sinAction.readySin;
		else if (selection == "top") sin = sinAction.currentSinList.ToArray()[1];
		else if (selection == "bottom") sin = sinAction.currentSinList.ToArray()[0];
		return sin;
	}

	public static ATTRIBUTE_TYPE SortSinResourceGetEnum(SinManager.EgoStockManager stock_manager, string modestring, UNIT_FACTION faction)
	{
		bool mode_greatest = false;
		bool mode_random = false;

		List<ATTRIBUTE_TYPE> sin_list = [];
		if (modestring.Contains('+'))
		{
			string[] selections = modestring.Split('+', StringSplitOptions.RemoveEmptyEntries);
			int selections_count = selections.Length;
			if (selections_count < 1) return ATTRIBUTE_TYPE.CRIMSON;
			string selections_0 = selections[0];
			mode_greatest = !selections_0.Contains("lowest");
			mode_random = selections_0.Contains("random");

			for (int i = 1; i < selections_count; i++)
			{
				ATTRIBUTE_TYPE sin_temp = ATTRIBUTE_TYPE.NONE;
				string s = selections[i];
				Enum.TryParse(s, true, out sin_temp);
				if (sin_temp != ATTRIBUTE_TYPE.NONE) sin_list.Add(sin_temp);
			}
		}
		else
		{
			mode_greatest = !modestring.Contains("lowest");
			mode_random = modestring.Contains("random");
			sin_list = [
				ATTRIBUTE_TYPE.CRIMSON,
				ATTRIBUTE_TYPE.SCARLET,
				ATTRIBUTE_TYPE.AMBER,
				ATTRIBUTE_TYPE.SHAMROCK,
				ATTRIBUTE_TYPE.AZURE,
				ATTRIBUTE_TYPE.INDIGO,
				ATTRIBUTE_TYPE.VIOLET
			];
		}


		if (mode_random) //randomize order
		{
			int n = sin_list.Count;
			while (n > 1)
			{
				n--;
				int k = MainClass.rng.Next(n + 1); // 0 to X Exclusive upper bound
				if (k == n) continue; // No change to list
				(sin_list[k], sin_list[n]) = (sin_list[n], sin_list[k]);
			}
		}

		int best_stock = mode_greatest ? -999 : 999;
		ATTRIBUTE_TYPE best_sin = ATTRIBUTE_TYPE.CRIMSON;
		foreach (ATTRIBUTE_TYPE sin in sin_list)
		{
			int stock = stock_manager.GetAttributeStockNumberByAttributeType(faction, sin);
			if (mode_greatest)
			{
				if (stock > best_stock)
				{
					best_stock = stock;
					best_sin = sin;
				}
			}
			else
			{
				if (stock < best_stock)
				{
					best_stock = stock;
					best_sin = sin;
				}
			}
		}

		return best_sin;
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

	public static void CheckLoggingBoolean()
	{
		logEnabled = EnableLogging.Value;
	}
	public static void LogModular(object thing)
	{
		if (logEnabled) Logg.LogInfo(thing);
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
	public static bool logEnabled = false; // for useless logs

	public const string NAME = "ModularSkillScripts";
	public const string VERSION = "4.9.3";
	public const string AUTHOR = "GlitchGames";
	public const string GUID = $"{AUTHOR}.{NAME}";

	public static ManualLogSource Logg;
	public static DirectoryInfo pluginPath = Directory.CreateDirectory(Path.Combine(Paths.PluginPath, "Lethe", "ModTemplate", "modular_lua"));
}
