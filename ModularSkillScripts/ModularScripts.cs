using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using static BattleActionModel.TargetDataDetail;
using IntPtr = System.IntPtr;
using Lethe.Patches;
using BattleUI.Dialog;
using FMOD;
using FMODUnity;
using BattleUI;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem.Text.RegularExpressions;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Regex = System.Text.RegularExpressions.Regex;
using RegexOptions = System.Text.RegularExpressions.RegexOptions;
using TimeSpan = Il2CppSystem.TimeSpan;

//using CodeStage.AntiCheat.ObscuredTypes;
//using Il2CppSystem.Collections;


namespace ModularSkillScripts;

public class ModUnitData : Il2CppSystem.Object
{
	public ModUnitData(IntPtr ptr) : base(ptr) { }

	public ModUnitData() : base(ClassInjector.DerivedConstructorPointer<ModUnitData>())
	{
		ClassInjector.DerivedConstructorBody(this);
	}

	public long unitPtr_intlong = 0;
	public List<DataMod> data_list = new List<DataMod>();
}

public class DataMod : Il2CppSystem.Object
{
	public DataMod(IntPtr ptr) : base(ptr) { }

	public DataMod() : base(ClassInjector.DerivedConstructorPointer<DataMod>())
	{
		ClassInjector.DerivedConstructorBody(this);
	}

	public int dataID = 0;
	public int dataValue = 0;
}

public class ModularSA : Il2CppSystem.Object
{
	public ModularSA(IntPtr ptr) : base(ptr) { }

	public ModularSA() : base(ClassInjector.DerivedConstructorPointer<ModularSA>())
	{
		ClassInjector.DerivedConstructorBody(this);
	}

	public string originalString = "";
	public char[] parenthesisSeparator = new char[] { '(', ')' };

	public int[] valueList = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	public void ResetValueList()
	{
		activationCounter = 0;
		for (int i = 0; i < valueList.Length; i++) valueList[i] = 0;
	}

	public void EraseAllData()
	{
		ResetValueList();
		ResetAdders();
		ResetCoinConditionals();

		ptr_intlong = 0;
		passiveID = 0;

		modsa_selfAction = null;
		modsa_oppoAction = null;
		modsa_target_list.Clear();

		modsa_unitModel = null;
		modsa_skillModel = null;
		modsa_passiveModel = null;
		modsa_coinModel = null;
		modsa_buffModel = null;
		dummySkillAbility = null;
		dummyPassiveAbility = null;
		dummyCoinAbility = null;
		modsa_loopTarget = null;
		modsa_loopString = "";
		modsa_luaScript = "";
	}

	public int activationTiming = 0;
	public bool resetWhenUse = false;
	public bool clearValues = false;

	private List<string> batch_list = new List<string>();

	public BattleActionModel modsa_selfAction = null;
	public BattleActionModel modsa_oppoAction = null;
	public List<BattleUnitModel> modsa_target_list = new List<BattleUnitModel>();

	public int interactionTimer = 0;
	public bool markedForDeath = false;
	public int abilityMode = 0;
	public int passiveID = 0; // 0 means skill, 1 means coin, 2 means passive
	public long ptr_intlong;

	public BattleUnitModel modsa_unitModel = null;
	public SkillModel modsa_skillModel = null;
	public PassiveModel modsa_passiveModel = null;
	public CoinModel modsa_coinModel = null;
	public BuffModel modsa_buffModel = null;
	public SkillAbility dummySkillAbility = null;
	public PassiveAbility dummyPassiveAbility = null;
	public CoinAbility dummyCoinAbility = null;
	public BattleUnitModel modsa_loopTarget = null;
	public string modsa_loopString = "";
	public string modsa_luaScript = "";

	public void ResetAdders()
	{
		coinScaleAdder = 0;
		skillPowerAdder = 0;
		skillPowerResultAdder = 0;
		parryingResultAdder = 0;
		atkDmgAdder = 0;
		atkMultAdder = 0;
		//atkWeightAdder = 0;
		critAdder = 0;
	}
	public int coinScaleAdder = 0;
	public int skillPowerAdder = 0;
	public int skillPowerResultAdder = 0;
	public int parryingResultAdder = 0;
	public int atkDmgAdder = 0;
	public int atkMultAdder = 0;
	//public int atkWeightAdder = 0;
	public int slotAdder = 0;
	public int critAdder = 0;

	public bool wasCrit = false;
	public bool wasClash = false;
	public bool wasWin = false;
	public int lastFinalDmg = 0;
	public int lastHpDmg = 0;
	public int activationCounter = 0;

	public void ResetCoinConditionals()
	{
		_onlyHeads = false;
		_onlyTails = false;
		_onlyCrit = false;
		_onlyNonCrit = false;
		//_onlyClashWin = false;
		//_onlyClashLose = false;
	}
	private bool _onlyHeads = false;
	private bool _onlyTails = false;
	private bool _onlyCrit = false;
	private bool _onlyNonCrit = false;
	//private bool _onlyClashWin = false;
	//private bool _onlyClashLose = false;

	public bool immortality = false;

	private bool _fullStop = false;
	public BATTLE_EVENT_TIMING battleTiming = BATTLE_EVENT_TIMING.NONE;

	public void Enact(BattleUnitModel unitModel, SkillModel skillModel_inst, BattleActionModel selfAction, BattleActionModel oppoAction, int actevent, BATTLE_EVENT_TIMING timing)
	{
		interactionTimer = 0;
		if (activationTiming != actevent)
		{
			modsa_target_list.Clear();
			return;
		}

		modsa_unitModel = unitModel;
		modsa_skillModel = skillModel_inst;
		modsa_selfAction = selfAction;
		modsa_oppoAction = oppoAction;
		battleTiming = timing;
			
		if (modsa_selfAction != null) {
			if (modsa_skillModel == null) modsa_skillModel = modsa_selfAction.Skill;
			if (modsa_unitModel == null) modsa_unitModel = modsa_selfAction.Model;
		}
		if (MainClass.logEnabled) MainClass.Logg.LogInfo("activation good");

		if (activationTiming == 1) markedForDeath = true;
		if (activationTiming == MainClass.timingDict["OSA"] || activationTiming == MainClass.timingDict["BSA"])
		{
			if (modsa_coinModel == null)
			{
				if (MainClass.logEnabled) MainClass.Logg.LogInfo("succeed attack, null coin, report bug please");
				return;
			}

			//MainClass.Logg.LogWarning($"IsHead = {modsa_coinModel.IsHead()}");
			if (modsa_coinModel.IsHead() && _onlyTails) return;
			else if (modsa_coinModel.IsTail() && _onlyHeads) return;

			//MainClass.Logg.LogWarning($"wasCrit = {wasCrit}");
			if (wasCrit && _onlyNonCrit) return;
			else if (!wasCrit && _onlyCrit) return;

			//if (_onlyClashWin || _onlyClashLose)
			//{
			//	if (!wasClash) return;
			//	else if (wasWin && _onlyClashLose) return;
			//	else if (!wasWin && _onlyClashWin) return;
			//}
		}


		if (abilityMode == 2)
		{
			if (MainClass.logEnabled) if (MainClass.logEnabled) MainClass.Logg.LogInfo("hijacking dummy passive");
			foreach (PassiveModel otherPassive in modsa_unitModel.UnitDataModel.PassiveList)
			{
				if (otherPassive._script == null) continue;
				dummyPassiveAbility = otherPassive._script;
				break;
			}
			if (dummyPassiveAbility == null)
			{
				if (MainClass.logEnabled) MainClass.Logg.LogInfo("creating dummy passive");
				PassiveAbility pa = new PassiveAbility();
				pa.Init(modsa_unitModel, new List<PassiveConditionStaticData> { }, new List<PassiveConditionStaticData> { }, new List<int>());
				dummyPassiveAbility = pa;
			}
		}
		else
		{
			if (modsa_skillModel.SkillAbilityList.Count > 0) dummySkillAbility = modsa_skillModel.SkillAbilityList.ToArray()[0];
			if (dummySkillAbility == null)
			{
				if (MainClass.logEnabled) MainClass.Logg.LogInfo("creating dummy skillability");
				SkillAbility_Empty sa = new SkillAbility_Empty();
				sa._skillModel = modsa_skillModel;
				sa._index = 0;
				dummySkillAbility = sa;
			}
		}

		if (abilityMode == 1)
		{
			if (modsa_coinModel.CoinAbilityList.Count > 0) dummyCoinAbility = modsa_coinModel.CoinAbilityList.ToArray()[0];
			if (dummyCoinAbility == null)
			{
				if (MainClass.logEnabled) MainClass.Logg.LogInfo("creating dummy coinability");
				CoinAbility_Empty ca = new CoinAbility_Empty();
				ca._coin = modsa_coinModel;
				ca._index = modsa_coinModel._originCoinIndex;
				dummyCoinAbility = ca;
			}
		}

		ResetAdders();
		if (clearValues) ResetValueList();
		List<BattleUnitModel> loopTarget_list = modsa_target_list;
		if (modsa_loopString.Any()) loopTarget_list = GetTargetModelList(modsa_loopString);
		else if (loopTarget_list.Count < 1) loopTarget_list.Add(GetTargetModel("MainTarget"));
		foreach (BattleUnitModel unit in loopTarget_list) {
			modsa_loopTarget = unit;
			_fullStop = false;
			for (int i = 0; i < batch_list.Count; i++)
			{
				if (_fullStop) break;
				string batch = batch_list.ToArray()[i];
				if (MainClass.logEnabled) MainClass.Logg.LogInfo("batch " + i.ToString() + ": " + batch);
				ProcessBatch(batch);
			}
		}
		modsa_target_list.Clear();
		activationCounter += 1;
	}

	private bool CheckIF(string param)
	{
		string[] circles = param.Split(parenthesisSeparator)[1].Split(',');

		int mode = -1; // AND
		switch (circles[0])
		{
			case "AND": mode = 0; break;
			case "OR": mode = 1; break;
			case "XOR": mode = 2; break;
		}

		int idx = 0;
		if (mode == -1) mode = 0;
		else idx++;
		
		char[] ifSeparator = new char[] { '<', '>', '=' };
		bool success = false;
		bool success_first = false;
		for (int i = idx; i < circles.Length; i++) {
			string circle_string = circles[i];
			var symbols = Regex.Matches(circle_string, "(<|>|=)", RegexOptions.IgnoreCase, System.TimeSpan.FromMinutes(1));
			string[] parameters = circle_string.Split(ifSeparator);
			string firstParam = parameters[0];
			string secondParam = parameters[1];

			int firstValue = GetNumFromParamString(firstParam);
			int secondValue = GetNumFromParamString(secondParam);

			string symbol = symbols[0].Value;
			if (symbol == "<") success = firstValue < secondValue;
			else if (symbol == ">") success = firstValue > secondValue;
			else if (symbol == "=") success = firstValue == secondValue;

			if (mode == 0) {
				if (!success) break;
			} else if (mode == 1) {
				if (success) break;
			} else {
				if (i == idx) success_first = success;
				else {
					success = success_first == success;
					if (!success) break;
				}
			}
		}
		MainClass.Logg.LogInfo("ifsuccess: " + param + " | " + success);
		return success;
	}


	public int GetNumFromParamString(string param)
	{
		int value = 0;
		bool negative = param[0] == '-';
		if (negative) param = param.Remove(0, 1);
		bool math = param[0] == 'm';
		if (math) param = param.Remove(0, 1);
		bool acquire = param[0] == 'G';
		if (acquire) param = param.Remove(0, 1);
		if (param.Last() == ')') param = param.Remove(param.Length - 1);
		
		if (math) value = DoMath(param);
		else if (param.StartsWith("VALUE_")) {
			int value_idx = 0;
			int.TryParse(param[6].ToString(), out value_idx);
			value = valueList[value_idx];
		}
		else if (acquire)
		{
			param = Regex.Replace(param, @"{", "(");
			param = Regex.Replace(param, @"}", ")");
			param = Regex.Replace(param, @"-", ",");
			value = AcquireValue(param);
		}
		else int.TryParse(param, out value);
		
		if (negative) value *= -1;
		return value;
	}


	public List<BattleUnitModel> GetTargetModelList(string param)
	{
		List<BattleUnitModel> unitList = new List<BattleUnitModel>();
		switch (param) {
			case "Null": return unitList;
			case "Self": {
				unitList.Add(modsa_unitModel);
				return unitList;
			}
			case "SelfCore": {
				BattleUnitModel_Abnormality_Part part = modsa_unitModel.TryCast<BattleUnitModel_Abnormality_Part>();
				if (part != null) unitList.Add(part.Abnormality);
				else unitList.Add(modsa_unitModel);

				return unitList;
			}
			case "Target": {
				if (modsa_loopTarget != null) unitList.Add(modsa_loopTarget);
				return unitList;
			}
			case "TargetCore": {
				BattleUnitModel_Abnormality_Part part = modsa_loopTarget.TryCast<BattleUnitModel_Abnormality_Part>();
				if (part != null) unitList.Add(part.Abnormality);
				else unitList.Add(modsa_loopTarget);
				return unitList;
			}
			case "MainTarget": {
				if (modsa_selfAction == null) { unitList.Add(null); return unitList; }
				TargetDataSet targetDataSet = modsa_selfAction._targetDataDetail.GetCurrentTargetSet();
				unitList.Add(targetDataSet.GetMainTarget());
				return unitList;
			}
			case "EveryTarget": {
				TargetDataSet targetDataSet = modsa_selfAction._targetDataDetail.GetCurrentTargetSet();
				unitList.Add(targetDataSet.GetMainTarget());
				foreach (SinActionModel sinActionModel in targetDataSet.GetSubTargetSinActionList()) {
					BattleUnitModel model = sinActionModel.UnitModel;
					if (!unitList.Contains(model)) unitList.Add(sinActionModel.UnitModel);
				}
				return unitList;
			}
			case "SubTarget": {
				TargetDataSet targetDataSet = modsa_selfAction._targetDataDetail.GetCurrentTargetSet();
				foreach (SinActionModel sinActionModel in targetDataSet.GetSubTargetSinActionList()) {
					BattleUnitModel model = sinActionModel.UnitModel;
					if (!unitList.Contains(model)) unitList.Add(sinActionModel.UnitModel);
				}
				return unitList;
			}
		}

		SinManager sinManager_inst = Singleton<SinManager>.Instance;
		BattleObjectManager battleObjectManager = sinManager_inst._battleObjectManager;

		if (param.StartsWith("id")) {
			string id_string = param.Remove(0, 2);
			int id = GetNumFromParamString(id_string);
			foreach (BattleUnitModel unit in battleObjectManager.GetModelList()) {
				if (unit.GetUnitID() == id) unitList.Add(unit);
			}
			return unitList;
		}
		else if (param.StartsWith("inst")) {
			string id_string = param.Remove(0, 4);
			int id = GetNumFromParamString(id_string);
			foreach (BattleUnitModel unit in battleObjectManager.GetModelList()) {
				if (unit.InstanceID == id) unitList.Add(unit);
			}
			return unitList;
		}
		else if (param.StartsWith("adj"))
		{
			string side_string = param.Remove(0, 3);
			if (side_string == "Left")
			{
				List<BattleUnitModel> modelList = battleObjectManager.GetPrevUnitsByPortrait(modsa_unitModel, 1);
				if (modelList.Count > 0) unitList.Add(modelList.ToArray()[0]);
			}
			else
			{
				List<BattleUnitModel> modelList = battleObjectManager.GetNextUnitsByPortrait(modsa_unitModel, 1);
				if (modelList.Count > 0) unitList.Add(modelList.ToArray()[0]);
			}
			return unitList;
		}

		UNIT_FACTION thisFaction = modsa_unitModel.Faction;
		UNIT_FACTION enemyFaction = thisFaction == UNIT_FACTION.PLAYER ? UNIT_FACTION.ENEMY : UNIT_FACTION.PLAYER;

		if (param == "EveryCoreAlly")
		{
			foreach (BattleUnitModel unit in battleObjectManager.GetAliveList(false, thisFaction))
			{
				if (unit is BattleUnitModel_Abnormality || !unit.IsAbnormalityOrPart) unitList.Add(unit);
			}
			return unitList;
		}
		else if (param == "EveryAbnoCoreAlly")
		{
			foreach (BattleUnitModel unit in battleObjectManager.GetAliveList(false, thisFaction))
			{
				if (unit is BattleUnitModel_Abnormality) unitList.Add(unit);
			}
			return unitList;
		}
		else if (param == "EveryCoreEnemy")
		{
			foreach (BattleUnitModel unit in battleObjectManager.GetAliveList(false, enemyFaction))
			{
				if (unit is BattleUnitModel_Abnormality || !unit.IsAbnormalityOrPart) unitList.Add(unit);
			}
			return unitList;
		}
		else if (param == "EveryAbnoCoreEnemy")
		{
			foreach (BattleUnitModel unit in battleObjectManager.GetAliveList(false, enemyFaction))
			{
				if (unit is BattleUnitModel_Abnormality) unitList.Add(unit);
			}
			return unitList;
		}
		else
		{
			System.Collections.Generic.List<BattleUnitModel> list = GetCustomTargetingList(battleObjectManager, param, thisFaction, enemyFaction);
				
			int num = 1;
			if (param.Contains("VALUE_")) {
				string[] circles = param.Split('$');
				string numstring = circles[0].Substring(circles[0].Length - 7);
				num = GetNumFromParamString(numstring);
			}
			else {
				string text = Regex.Replace(param, "\\D", "");
				if (text.Length > 0) num = int.Parse(text);
			}
				
			num = Math.Min(num, list.Count);
			if (num > 0) {
				for (int i = 0; i < num; i++) unitList.Add(list.ToArray()[i]);
			}
		}

		return unitList;
	}
	
	private System.Collections.Generic.List<BattleUnitModel> GetCustomTargetingList(BattleObjectManager battleObjectManager, string param, UNIT_FACTION thisFaction, UNIT_FACTION enemyFaction) {
		System.Collections.Generic.List<BattleUnitModel> list = new System.Collections.Generic.List<BattleUnitModel>();
		UNIT_FACTION filterFaction = UNIT_FACTION.NONE;
		bool filterKeyword = param.Contains("$");
			
		bool noCores = param.Contains("NoCores");
		bool noParts = param.Contains("NoParts");

		bool assistance = param.Contains("Assist");

		if (param.Contains("Enemy")) filterFaction = enemyFaction;
		else if (param.Contains("Ally")) filterFaction = thisFaction;
			
		if (filterKeyword) {
			string[] circles = param.Split('$');
			param = circles[0];
			BUFF_UNIQUE_KEYWORD bufKeyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
				
			foreach (BattleUnitModel unit in battleObjectManager.GetAliveList(bufKeyword, 0, assistance, filterFaction)) list.Add(unit);
		}
		else {
			if (noCores) foreach (BattleUnitModel unit in battleObjectManager.GetAliveListExceptAbnormalitySelf(filterFaction, assistance)) list.Add(unit);
			else if (noParts) foreach (BattleUnitModel unit in battleObjectManager.GetAliveListExceptAbnormalityPart(filterFaction, assistance)) list.Add(unit);
			else foreach (BattleUnitModel unit in battleObjectManager.GetAliveList(assistance, filterFaction)) list.Add(unit);
		}

		if (param.Contains("AbnoOnly")) {
			System.Collections.Generic.List<BattleUnitModel> goodones = new();
			foreach (BattleUnitModel unit in list) {
				if (unit.IsAbnormalityOrPart) goodones.Add(unit);
			}
			list = goodones;
		}
		else if (param.Contains("NoAbnos")) {
			System.Collections.Generic.List<BattleUnitModel> goodones = new();
			foreach (BattleUnitModel unit in list) {
				if (!unit.IsAbnormalityOrPart) goodones.Add(unit);
			}
			list = goodones;
		}

		if (param.Contains("ExceptSelf")) list.Remove(modsa_unitModel);
		if (param.Contains("ExceptTarget")) list.Remove(modsa_loopTarget);
			
		if (param.Contains("Random")) list = MainClass.ShuffleUnits(list);
		else if (param.Contains("Deploy")) list.Sort((x, y) => x.PARTICIPATE_ORDER.CompareTo(y.PARTICIPATE_ORDER));
		else if (param.Contains("Reversedeploy")) list.Sort((x, y) => y.PARTICIPATE_ORDER.CompareTo(x.PARTICIPATE_ORDER));
			
		if (param.StartsWith("Slowest")) list.Sort((x, y) => x.GetOriginSpeedForCompare().CompareTo(y.GetOriginSpeedForCompare()));
		else if (param.StartsWith("Fastest")) list.Sort((x, y) => y.GetOriginSpeedForCompare().CompareTo(x.GetOriginSpeedForCompare()));
		else if (param.StartsWith("HighestHPRatio")) list.Sort((x, y) => y.GetHpRatio().CompareTo(x.GetHpRatio()));
		else if (param.StartsWith("LowestHPRatio")) list.Sort((x, y) => x.GetHpRatio().CompareTo(y.GetHpRatio()));
		else if (param.StartsWith("HighestHP")) list.Sort((x, y) => y.Hp.CompareTo(x.Hp));
		else if (param.StartsWith("LowestHP")) list.Sort((x, y) => x.Hp.CompareTo(y.Hp));
		else if (param.StartsWith("HighestMaxHP")) list.Sort((x, y) => y.MaxHp.CompareTo(x.MaxHp));
		else if (param.StartsWith("LowestMaxHP")) list.Sort((x, y) => x.MaxHp.CompareTo(y.MaxHp));
		else if (param.StartsWith("HighestMP")) list.Sort((x, y) => y.Mp.CompareTo(x.Mp));
		else if (param.StartsWith("LowestMP")) list.Sort((x, y) => x.Mp.CompareTo(y.Mp));

		return list;
	}
		
	public BattleUnitModel GetTargetModel(string param)
	{
		switch (param) {
			case "Null": return null;
			case "Self": return modsa_unitModel;
			case "SelfCore": {
				BattleUnitModel_Abnormality_Part part = modsa_unitModel.TryCast<BattleUnitModel_Abnormality_Part>();
				if (part != null) return part.Abnormality;
				else return modsa_unitModel;
			}
			case "Target": return modsa_loopTarget;
			case "TargetCore": {
				BattleUnitModel_Abnormality_Part part = modsa_loopTarget.TryCast<BattleUnitModel_Abnormality_Part>();
				if (part != null) return part.Abnormality;
				else return modsa_loopTarget;
			}
			case "MainTarget": {
				if (modsa_selfAction == null) return null;
				TargetDataSet targetDataSet = modsa_selfAction._targetDataDetail.GetCurrentTargetSet();
				return targetDataSet.GetMainTarget();
			}
		}
		
		if (param.StartsWith("id")) {
			SinManager sinManager_inst = Singleton<SinManager>.Instance;
			BattleObjectManager battleObjectManager = sinManager_inst._battleObjectManager;
			string id_string = param.Remove(0, 2);
			int id = GetNumFromParamString(id_string);
			return battleObjectManager.GetModelByUnitID(id);
		}
		else if (param.StartsWith("inst"))
		{
			SinManager sinManager_inst = Singleton<SinManager>.Instance;
			BattleObjectManager battleObjectManager = sinManager_inst._battleObjectManager;

			string id_string = param.Remove(0, 4);
			int id = GetNumFromParamString(id_string);

			foreach (BattleUnitModel unit in battleObjectManager.GetModelList()) {
				if (unit.InstanceID == id) return unit;
			}
			return null;
		}
		else if (param.StartsWith("adj")) {
			BattleObjectManager battleObjectManager_inst = SingletonBehavior<BattleObjectManager>.Instance;
			if (battleObjectManager_inst == null) return null;
			BattleUnitModel foundUnit = null;
				
			string side_string = param.Remove(0, 3);
			if (side_string == "Left")
			{
				List<BattleUnitModel> modelList = battleObjectManager_inst.GetPrevUnitsByPortrait(modsa_unitModel, 1);
				if (modelList.Count > 0) foundUnit = modelList.ToArray()[0];
			}
			else
			{
				List<BattleUnitModel> modelList = battleObjectManager_inst.GetNextUnitsByPortrait(modsa_unitModel, 1);
				if (modelList.Count > 0) foundUnit = modelList.ToArray()[0];
			}
			return foundUnit;
		}
		else {
			BattleObjectManager battleObjectManager_inst = SingletonBehavior<BattleObjectManager>.Instance;
			if (battleObjectManager_inst == null) return null;
			BattleUnitModel foundUnit = null;
				
			UNIT_FACTION thisFaction = modsa_unitModel.Faction;
			UNIT_FACTION enemyFaction = thisFaction == UNIT_FACTION.PLAYER ? UNIT_FACTION.ENEMY : UNIT_FACTION.PLAYER;

			System.Collections.Generic.List<BattleUnitModel> list = GetCustomTargetingList(battleObjectManager_inst, param, thisFaction, enemyFaction);
			if (list.Count > 0) foundUnit = list.ToArray()[0];
			return foundUnit;
		}
	}

	public void SetupModular(string instructions)
	{
		/*AntlrInputStream inputStream = new AntlrInputStream(instructions);
		ModsaLangLexer lexer = new ModsaLangLexer(inputStream);
		CommonTokenStream tokenStream = new CommonTokenStream(lexer);
		ModsaLangParser parser = new ModsaLangParser(tokenStream);
		ModsaLangParser.ProgramContext tree = parser.program();*/
			
		instructions = MainClass.sWhitespace.Replace(instructions, "");
		string[] batches = instructions.Split('/');
		bool luaFound = false;

		for (int i = 0; i < batches.Length; i++) {
			string batch = batches[i];
			if (MainClass.logEnabled) MainClass.Logg.LogInfo("batch " + i.ToString() + ": " + batch);
			if (batch.StartsWith("TIMING:")) {
				string timingArg = batch.Remove(0, 7);
				string[] circles = timingArg.Split(parenthesisSeparator);
				string circle_0 = circles[0];
				if (MainClass.timingDict.ContainsKey(circle_0)) activationTiming = MainClass.timingDict[circle_0];
					
				if (circles.Length > 1) {
					string hitArgs = circles[1];
					if (hitArgs.Contains("Head"))  _onlyHeads = true;
					else if (hitArgs.Contains("Tail")) _onlyTails = true;

					if (hitArgs.Contains("NoCrit")) _onlyNonCrit = true;
					else if (hitArgs.Contains("Crit")) _onlyCrit = true;

					//if (hitArgs.Contains("Win")) _onlyClashWin = true;
					//else if (hitArgs.Contains("Lose")) _onlyClashLose = true;
				}
			}
			else if (batch.StartsWith("LUA:", StringComparison.OrdinalIgnoreCase))
			{
				if (!String.IsNullOrWhiteSpace(modsa_loopString))
				{
					MainClass.Logg.LogError("LUA cannot be used with LOOP");
					return;
				}
				var luaScriptName = batch.Remove(0, 4);
				if (!ReloadPatches.loadedScripts.TryGetValue(luaScriptName, out modsa_luaScript))
				{
					MainClass.Logg.LogError("LUA script used but not found: " + luaScriptName);
					return;
				}
				luaFound = true;
			}
			else if (batch.StartsWith("LOOP:", StringComparison.OrdinalIgnoreCase))
			{
				if (!String.IsNullOrWhiteSpace(modsa_luaScript))
				{
					MainClass.Logg.LogError("LOOP cannot be used with LUA");
					return;
				}
				modsa_loopString = batch.Remove(0, 5);
			}
			else if (batch.Equals("RESETWHENUSE", StringComparison.OrdinalIgnoreCase)) resetWhenUse = true;
			else if (batch.Equals("CLEARVALUES", StringComparison.OrdinalIgnoreCase)) clearValues = true;
			else if (luaFound)
			{
				MainClass.Logg.LogError("LUA cannot be used with other batches");
				return;
			}
			else batch_list.Add(batch);
		}
	}
		
	private void ProcessBatch(string batch) {
		string[] batchArgs = batch.Split(':');
		for (int i = 0; i < batchArgs.Length; i++) {
			if (MainClass.logEnabled) MainClass.Logg.LogInfo("batchArgs " + i.ToString() + ": " + batchArgs[i]);
			if (batchArgs[i].StartsWith("STOPIF") || batchArgs[i].StartsWith("CONTINUEIF"))
			{
				if (!CheckIF(batchArgs[i]))
				{
					_fullStop = true;
					return;
				}
				continue;
			}
			else if (batchArgs[i].StartsWith("IFNOT")) { if (CheckIF(batchArgs[i])) break; else continue; }
			else if (batchArgs[i].StartsWith("IF")) { if (!CheckIF(batchArgs[i])) break; else continue; }
			else if (batchArgs[i].StartsWith("VALUE_")) {
				string numChar = batchArgs[i][6].ToString();
				int valueidx = 0;
				int.TryParse(numChar, out valueidx);
				valueList[valueidx] = AcquireValue(batchArgs[i + 1]); // GETTERS
				i += 1;
				continue;
			}

			Consequence(batchArgs[i]); // CONSEQUENCES
		}
	}

	private void Consequence(string section) {
		string[] sectionArgs = section.Split(parenthesisSeparator);
		string mEth = sectionArgs[0];
		string circledSection = "";
		if (sectionArgs.Length >= 2) circledSection = sectionArgs[1];
		string[] circles = circledSection.Split(',');
		if (MainClass.consequenceDict.TryGetValue(mEth, out var consequence))
		{
			consequence.ExecuteConsequence(this, section, circledSection, circles);
		}
		else
		{
			MainClass.Logg.LogInfo("Invalid Consequence: " + mEth);
		}
	}

	public int DoMath(string s) {
		var symbols = MainClass.mathsymbolRegex.Matches(s);
		string[] parameters = s.Split(MainClass.mathSeparator);
		string firstParam = parameters[0];
		double finalValue = GetNumFromParamString(firstParam);

		for (int i = 0; i < symbols.Count; i++) {
			string param = parameters[i + 1];
			string symbol_string = symbols[i].Value;
			char symbol = symbol_string[0];
			int amount = GetNumFromParamString(param);
			if (MainClass.logEnabled) MainClass.Logg.LogInfo("mathparam " + param + " | mathsymbol " + symbol);

			switch (symbol)
			{
				case '+': finalValue += amount; break;
				case '-': finalValue -= amount; break;
				case '*': finalValue *= amount; break;
				case '%': finalValue /= amount; break;
				case '!': finalValue = Math.Min(finalValue, amount); break;
				case '¡': finalValue = Math.Max(finalValue, amount); break;
				case '?': finalValue %= amount; break;
			}
		}

		return (int)finalValue;
	}
	
	private int AcquireValue(string section)
	{
		if (MainClass.logEnabled) MainClass.Logg.LogInfo("AcquireValue " + section);
		string[] sectionArgs = section.Split(parenthesisSeparator);

		if (char.IsNumber(section.Last())) return GetNumFromParamString(sectionArgs[0]);

		string methodology = sectionArgs[0];
		string circledSection = "";
		if (sectionArgs.Length > 1) circledSection = sectionArgs[1];
		string[] circles = circledSection.Split(',');

		if (MainClass.acquirerDict.TryGetValue(methodology, out var acquirer))
		{
			return acquirer.ExecuteAcquirer(this, section, circledSection, circles);
		}
		
		MainClass.Logg.LogInfo("Invalid Getter: " + methodology);
		return -1;
	}
	
	public static BattleUnitModel_Abnormality AsAbnormalityModel(BattleUnitModel targetModel)
	{
		var abnoPart = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
		return abnoPart != null ? abnoPart.Abnormality : targetModel.TryCast<BattleUnitModel_Abnormality>();
	}
		
}

/// <summary>
/// Interface for defining modular consequences in the system.
/// </summary>
public interface IModularConsequence
{
    /// <summary>
    /// Executes a consequence based on the provided parameters.
    /// </summary>
    /// <param name="modular">The modular instance, where all the controlling values and helper functions can be found</param>
    /// <param name="section">The raw string of the consequence declaration, e.g. "consequence(Self, argument, 3, 4)"</param>
    /// <param name="circledSection">The section inside parenthesis, e.g. "Self, argument, 3, 4"</param>
    /// <param name="circles">Arguments specified in the circled section, e.g. ["Self", "argument", "3", "4"]</param>
    void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles);
}

/// <summary>
/// Interface for defining modular value getters in the system.
/// </summary>
public interface IModularAcquirer
{
    /// <summary>
    /// Executes a value getter based on the provided parameters.
    /// </summary>
    /// <param name="modular">The modular instance, where all the controlling values and helper functions can be found</param>
    /// <param name="section">The raw string of the consequence declaration, e.g. "consequence(Self, argument, 3, 4)"</param>
    /// <param name="circledSection">The section inside parenthesis, e.g. "Self, argument, 3, 4"</param>
    /// <param name="circles">Arguments specified in the circled section, e.g. ["Self", "argument", "3", "4"]</param>
    /// <returns>The value which this value getter evalutes to</returns>
    int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles);
}
