using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System;
using Il2CppSystem.Collections.Generic;
using System.Text.RegularExpressions;
using Il2CppInterop.Runtime.Injection;
//using Antlr4.Runtime;
//using Antlr4.Runtime.Tree;
//using ModsaLang;

namespace ModularSkillScripts;

[BepInPlugin(GUID, NAME, VERSION)]
public class MainClass : BasePlugin
{
	public override void Load()
	{
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
		if (fakepowerEnabled) harmony.PatchAll(typeof(FakePowerPatches));

		//modsaEval = new ModsaEvaluator();
		timingDict.Add("RoundStart", -1);
		timingDict.Add("StartBattle", 0);
		timingDict.Add("WhenUse", 1);
		timingDict.Add("BeforeAttack", 2);
		timingDict.Add("StartDuel", 3);
		timingDict.Add("WinDuel", 4);
		timingDict.Add("DefeatDuel", 5);
		timingDict.Add("EndBattle", 6);
		timingDict.Add("OnSucceedAttack", 7);
		timingDict.Add("WhenHit", 8);
		timingDict.Add("EndSkill", 9);
		timingDict.Add("FakePower", 10);
		timingDict.Add("BeforeDefense", 11);
		timingDict.Add("OnDie", 12);
		timingDict.Add("OnOtherDie", 13);
		timingDict.Add("DuelClash", 14);
		timingDict.Add("DuelClashAfter", 15);
		timingDict.Add("OnSucceedEvade", 16);
		timingDict.Add("OnDefeatEvade", 17);
		timingDict.Add("OnStartBehaviour", 18);
		timingDict.Add("BeforeBehaviour", 19);
		timingDict.Add("OnEndBehaviour", 20);
		timingDict.Add("EnemyKill", 21);
		timingDict.Add("OnBreak", 22);
		timingDict.Add("OnOtherBreak", 23);
		timingDict.Add("OnDiscard", 24);
		timingDict.Add("OnZeroHP", 25);
		timingDict.Add("EnemyEndSkill", 26);
		timingDict.Add("OnOtherBurst", 27);
		timingDict.Add("BeforeSA", 28);
		timingDict.Add("BeforeWhenHit", 29);
		timingDict.Add("SpecialAction", 999);
	}

	public static System.Collections.Generic.List<BattleUnitModel> ShuffleUnits(System.Collections.Generic.List<BattleUnitModel> list)
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
	public static Dictionary<string, int> timingDict = new();
		
	public static bool fakepowerEnabled = true;

	public static Random rng = new();

	public static readonly Regex sWhitespace = new(@"\s+");

	public static bool logEnabled = false;

	public const string NAME = "ModularSkillScripts";
	public const string VERSION = "2.6.0";
	public const string AUTHOR = "GlitchGames";
	public const string GUID = $"{AUTHOR}.{NAME}";

	public static ManualLogSource Logg;
}
	
/*
public class ModsaEvaluator : ModsaLangBaseVisitor<double>
{
	public void Appropriate(ModularSA modsa)
	{
		modsa_owner = modsa;
	}

	private ModularSA modsa_owner = null;

	public override double VisitCircleEx(ModsaLangParser.CircleExContext context)
	{
		// Evaluate the expression inside the parentheses
		return Visit(context.expression());
	}

	public override double VisitMathEx(ModsaLangParser.MathExContext context)
	{
		double left = Visit(context.expression(0)); // Evaluate the left expression
		double right = Visit(context.expression(1)); // Evaluate the right expression

		// Perform multiplication or division
		int num = context.op.Type;
		return num switch
		{
			ModsaLangParser.ADD => left + right,
			ModsaLangParser.SUB => left - right,
			ModsaLangParser.MUL => left * right,
			ModsaLangParser.DIV => left / right,
			ModsaLangParser.MAX => Math.Max(left, right),
			ModsaLangParser.MIN => Math.Min(left, right),
			_ => throw new Exception($"Unknown function: {num}")
		};
	}

	public override double VisitSetModvalEx(ModsaLangParser.SetModvalExContext context)
	{
		string valueString = context.MODVAL().GetText();
		int value_idx = int.Parse(valueString.Last().ToString());

		double arg = Visit(context.expression()); // Evaluate the argument

		modsa_owner.valueList[value_idx] = (int)arg;
		return 0;
	}

	public override double VisitFunctionEx(ModsaLangParser.FunctionExContext context)
	{
		string funcName = context.ID().GetText(); // Get the function name
		double arg = Visit(context.expression()); // Evaluate the argument

		// Perform the function
		return funcName switch
		{
			"sin" => Math.Sin(arg),
			"cos" => Math.Cos(arg),
			"tan" => Math.Tan(arg),
			_ => throw new Exception($"Unknown function: {funcName}")
		};
	}

	public override double VisitNumberEx(ModsaLangParser.NumberExContext context)
	{
		// Parse the number
		return double.Parse(context.NUMBER().GetText());
	}

	public override double VisitModvalEx(ModsaLangParser.ModvalExContext context)
	{
		// For now, assume variables are 0 (you can extend this to support variables)
		return 0;
	}
	public override double VisitVariableEx(ModsaLangParser.VariableExContext context)
	{
		// For now, assume variables are 0 (you can extend this to support variables)
		return 0;
	}
}
*/