using System;
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
//using CodeStage.AntiCheat.ObscuredTypes;
//using Il2CppSystem.Collections;

namespace ModularSkillScripts;

public class ModUnitData : MonoBehaviour
{
	public ModUnitData(IntPtr ptr) : base(ptr) { }

	public ModUnitData() : base(ClassInjector.DerivedConstructorPointer<ModUnitData>())
	{
		ClassInjector.DerivedConstructorBody(this);
	}

	public long unitPtr_intlong = 0;
	public List<DataMod> data_list = new List<DataMod>();
}

public class DataMod : MonoBehaviour
{
	public DataMod(IntPtr ptr) : base(ptr) { }

	public DataMod() : base(ClassInjector.DerivedConstructorPointer<DataMod>())
	{
		ClassInjector.DerivedConstructorBody(this);
	}

	public int dataID = 0;
	public int dataValue = 0;
}

public class ModularSA : MonoBehaviour
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

	public void ResetAdders()
	{
		coinScaleAdder = 0;
		skillPowerAdder = 0;
		skillPowerResultAdder = 0;
		parryingResultAdder = 0;
		atkDmgAdder = 0;
		atkMultAdder = 0;
		//atkWeightAdder = 0;
	}
	public int coinScaleAdder = 0;
	public int skillPowerAdder = 0;
	public int skillPowerResultAdder = 0;
	public int parryingResultAdder = 0;
	public int atkDmgAdder = 0;
	public int atkMultAdder = 0;
	//public int atkWeightAdder = 0;
	public int slotAdder = 0;

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
		_onlyClashWin = false;
		_onlyClashLose = false;
	}
	private bool _onlyHeads = false;
	private bool _onlyTails = false;
	private bool _onlyCrit = false;
	private bool _onlyNonCrit = false;
	private bool _onlyClashWin = false;
	private bool _onlyClashLose = false;

	public bool immortality = false;

	private bool _fullStop = false;
	BATTLE_EVENT_TIMING battleTiming = BATTLE_EVENT_TIMING.NONE;

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
		if (activationTiming == 7)
		{
			if (modsa_coinModel == null)
			{
				if (MainClass.logEnabled) MainClass.Logg.LogInfo("succeed attack, null coin, report bug please");
				return;
			}

			if (modsa_coinModel.IsHead() && _onlyTails) return;
			else if (modsa_coinModel.IsTail() && _onlyHeads) return;

			if (wasCrit && _onlyNonCrit) return;
			else if (!wasCrit && _onlyCrit) return;

			if (_onlyClashWin || _onlyClashLose)
			{
				if (!wasClash) return;
				else if (wasWin && _onlyClashLose) return;
				else if (!wasWin && _onlyClashWin) return;
			}
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
				pa.Init(modsa_unitModel, new List<PassiveConditionStaticData> { }, new List<PassiveConditionStaticData> { });
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
			MatchCollection symbols = Regex.Matches(circle_string, "(<|>|=)", RegexOptions.IgnoreCase);
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

	private int GetNumFromParamString(string param)
	{
		int value = 0;
		bool negative = param[0] == '-';
		if (negative) param = param.Remove(0, 1);
		bool math = param[0] == 'm';
		if (math) param = param.Remove(0, 1);
		if (param.Last() == ')') param = param.Remove(param.Length - 1);
		
		if (math) value = DoMath(param);
		else if (param.StartsWith("VALUE_")) {
			int value_idx = 0;
			int.TryParse(param[6].ToString(), out value_idx);
			value = valueList[value_idx];
		} else int.TryParse(param, out value);
		
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

					if (hitArgs.Contains("Win")) _onlyClashWin = true;
					else if (hitArgs.Contains("Lose")) _onlyClashLose = true;						
				}
			}
			else if (batch.StartsWith("LOOP:", StringComparison.OrdinalIgnoreCase)) modsa_loopString = batch.Remove(0, 5);
			else if (batch.Equals("RESETWHENUSE", StringComparison.OrdinalIgnoreCase)) resetWhenUse = true;
			else if (batch.Equals("CLEARVALUES", StringComparison.OrdinalIgnoreCase)) clearValues = true;
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
				valueList[valueidx] = AcquireValue(valueidx, batchArgs[i + 1]); // GETTERS
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

		switch (mEth)
		{
			case "log": MainClass.Logg.LogInfo("ModularLog " + circles[0] + ": " + GetNumFromParamString(circles[1]));
				break;
			case "base": skillPowerAdder = GetNumFromParamString(circledSection);
				break;
			case "final": skillPowerResultAdder = GetNumFromParamString(circledSection);
				break;
			case "clash": parryingResultAdder = GetNumFromParamString(circledSection);
				break;
			case "scale":{
				int actevent_FakePower = MainClass.timingDict["FakePower"];
				if (circles.Length == 1) {
					int power = 0;
					if (activationTiming != actevent_FakePower) {
						OPERATOR_TYPE coinOp = OPERATOR_TYPE.NONE;
						if (circledSection == "ADD") coinOp = OPERATOR_TYPE.ADD;
						else if (circledSection == "SUB") coinOp = OPERATOR_TYPE.SUB;
						else if (circledSection == "MUL") coinOp = OPERATOR_TYPE.MUL;
						else power = GetNumFromParamString(circledSection);
						if (coinOp != OPERATOR_TYPE.NONE) foreach (CoinModel coin in modsa_skillModel.CoinList) coin._operatorType = coinOp;
						else coinScaleAdder = power;
					}
					else coinScaleAdder = GetNumFromParamString(circledSection);
				} else if (activationTiming != actevent_FakePower) {
					int coin_idx = -999;
					coin_idx = GetNumFromParamString(circles[1]);
					if (coin_idx == -999) return;

					string firstCircle = circles[0];

					int power = 0;
					OPERATOR_TYPE coinOp = OPERATOR_TYPE.NONE;
					if (firstCircle == "ADD") coinOp = OPERATOR_TYPE.ADD;
					else if (firstCircle == "SUB") coinOp = OPERATOR_TYPE.SUB;
					else if (firstCircle == "MUL") coinOp = OPERATOR_TYPE.MUL;
					else power = GetNumFromParamString(firstCircle);

					coin_idx = Math.Min(modsa_skillModel.CoinList.Count - 1, coin_idx);
					modsa_skillModel.CoinList.ToArray()[coin_idx]._scale += power;
					if (coinOp != OPERATOR_TYPE.NONE) modsa_skillModel.CoinList.ToArray()[coin_idx]._operatorType = coinOp;
				}
			}
				break;
			case "dmgadd": atkDmgAdder = GetNumFromParamString(circledSection); 
				break;
			case "dmgmult": atkMultAdder = GetNumFromParamString(circledSection);
				break;
			case "mpdmg":{
				int mpAmount = GetNumFromParamString(circles[1]);
				if (mpAmount == 0) return;

				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);

				foreach (BattleUnitModel targetModel in modelList) {
					int loseAmount = 0;
					if (mpAmount > 0)
					{
						if (abilityMode == 2)
						{
							dummyPassiveAbility.AddTriggeredData_MpHeal(mpAmount, targetModel.InstanceID, battleTiming);
							targetModel.HealTargetMp(targetModel, mpAmount, ABILITY_SOURCE_TYPE.PASSIVE, battleTiming);
						}
						else if (abilityMode == 1)
						{
							dummyCoinAbility.AddTriggeredData_MpHeal(mpAmount, targetModel.InstanceID, battleTiming);
							dummyCoinAbility.HealTargetMp(modsa_unitModel, targetModel, mpAmount, battleTiming);
						}
						else
						{
							dummySkillAbility.AddTriggeredData_MpHeal(mpAmount, targetModel.InstanceID, battleTiming);
							targetModel.HealTargetMp(targetModel, mpAmount, ABILITY_SOURCE_TYPE.SKILL, battleTiming);
						}
					}
					else
					{
						if (abilityMode == 2)
						{
							dummyPassiveAbility.AddTriggeredData_MpDamage(mpAmount * -1, targetModel.InstanceID, battleTiming);
							targetModel.GiveMpDamage(targetModel, mpAmount * -1, battleTiming, DAMAGE_SOURCE_TYPE.PASSIVE, out loseAmount, modsa_selfAction);
						}
						else if (abilityMode == 1)
						{
							dummyCoinAbility.AddTriggeredData_MpDamage(mpAmount * -1, targetModel.InstanceID, battleTiming);
							dummyCoinAbility.GiveMpDamage(modsa_unitModel, targetModel, mpAmount * -1, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, out loseAmount, modsa_selfAction);
						}
						else
						{
							dummySkillAbility.AddTriggeredData_MpDamage(mpAmount * -1, targetModel.InstanceID, battleTiming);
							targetModel.GiveMpDamage(targetModel, mpAmount * -1, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, out loseAmount, modsa_selfAction);
						}
					}
				}
			}
				break;
			case "reusecoin":{
				foreach (string circle in circles)
				{
					int idx = GetNumFromParamString(circle);
					if (idx < 0) { modsa_skillModel.CopyCoin(modsa_selfAction, modsa_coinModel.GetOriginCoinIndex(), battleTiming); continue; }

					idx = Math.Min(idx, modsa_skillModel.CoinList.Count - 1);
					modsa_skillModel.CopyCoin(modsa_selfAction, idx, battleTiming);
				}
			}
				break;
			case "bonusdmg":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				if (modelList.Count < 1) return;

				int amount = GetNumFromParamString(circles[1]);

				int dmg_type = Math.Min(int.Parse(circles[2]), 2);
				int dmg_sin = Math.Min(int.Parse(circles[3]), 11);

				ATK_BEHAVIOUR atkBehv = ATK_BEHAVIOUR.NONE;
				ATTRIBUTE_TYPE sinKind = ATTRIBUTE_TYPE.NONE;
				if (dmg_type != -1) atkBehv = (ATK_BEHAVIOUR)dmg_type;
				if (dmg_sin != -1) sinKind = (ATTRIBUTE_TYPE)dmg_sin;

				foreach (BattleUnitModel targetModel in modelList) {
					if (dmg_type == -1 && dmg_sin == -1) {
						//AbilityTriggeredData_HpDamage triggerData = new AbilityTriggeredData_HpDamage(amount, targetModel.InstanceID, battleTiming);
						//targetModel.AddTriggeredData(triggerData);
						if (abilityMode == 2)
							dummyPassiveAbility.GiveAbsHpDamage(modsa_unitModel, targetModel, amount, out _, out _, battleTiming,
								DAMAGE_SOURCE_TYPE.PASSIVE);
						else if (abilityMode == 1)
							dummyCoinAbility.GiveAbsHpDamage(modsa_unitModel, targetModel, amount, out _, out _, battleTiming,
								DAMAGE_SOURCE_TYPE.SKILL, modsa_selfAction);
						else
							dummySkillAbility.GiveAbsHpDamage(modsa_unitModel, targetModel, amount, out _, out _, battleTiming,
								DAMAGE_SOURCE_TYPE.SKILL, modsa_selfAction);
					}
					else if (dmg_type == -1 && dmg_sin != -1) {
						//AbilityTriggeredData_HpDamage triggerData = new AbilityTriggeredData_HpDamage(amount, targetModel.InstanceID, sinKind, battleTiming);
						//targetModel.AddTriggeredData(triggerData);
						if (abilityMode == 2)
							dummyPassiveAbility.GiveHpDamageAppliedAttributeResist(modsa_unitModel, targetModel, amount, sinKind,
								battleTiming, DAMAGE_SOURCE_TYPE.PASSIVE, out _);
						else if (abilityMode == 1)
							dummyCoinAbility.GiveHpDamageAppliedAttributeResist(modsa_unitModel, targetModel, amount, sinKind,
								battleTiming, DAMAGE_SOURCE_TYPE.SKILL, out _);
						else
							dummySkillAbility.GiveHpDamageAppliedAttributeResist(modsa_unitModel, targetModel, amount, sinKind,
								battleTiming, DAMAGE_SOURCE_TYPE.SKILL, out _);
					}
					else if (dmg_type != -1 && dmg_sin == -1) {
						//AbilityTriggeredData_HpDamage triggerData = new AbilityTriggeredData_HpDamage(amount, targetModel.InstanceID, atkBehv, battleTiming);
						//targetModel.AddTriggeredData(triggerData);
						if (abilityMode == 2)
							dummyPassiveAbility.GiveHpDamageAppliedAtkResist(modsa_unitModel, targetModel, amount, atkBehv,
								battleTiming, DAMAGE_SOURCE_TYPE.PASSIVE, modsa_selfAction);
						else if (abilityMode == 1)
							dummyCoinAbility.GiveHpDamageAppliedAtkResist(modsa_unitModel, targetModel, amount, atkBehv, battleTiming,
								DAMAGE_SOURCE_TYPE.SKILL, modsa_selfAction);
						else
							dummySkillAbility.GiveHpDamageAppliedAtkResist(modsa_unitModel, targetModel, amount, atkBehv,
								battleTiming, DAMAGE_SOURCE_TYPE.SKILL, modsa_selfAction);
					}
					else
					{
						//AbilityTriggeredData_HpDamage triggerData = new AbilityTriggeredData_HpDamage(amount, targetModel.InstanceID, sinKind, atkBehv, battleTiming);
						//targetModel.AddTriggeredData(triggerData);
						if (abilityMode == 2)
							dummyPassiveAbility.GiveHpDamageAppliedAttributeAndAtkResist(modsa_unitModel, targetModel, amount,
								sinKind, atkBehv, battleTiming, DAMAGE_SOURCE_TYPE.PASSIVE, modsa_selfAction);
						else if (abilityMode == 1)
							dummyCoinAbility.GiveHpDamageAppliedAttributeAndAtkResist(modsa_unitModel, targetModel, amount, sinKind,
								atkBehv, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, modsa_selfAction);
						else
							dummySkillAbility.GiveHpDamageAppliedAttributeAndAtkResist(modsa_unitModel, targetModel, amount, sinKind,
								atkBehv, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, modsa_selfAction);
					}
				}
			}
				break;
			case "buf":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				if (modelList.Count < 1) return;

				BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
				int stack = GetNumFromParamString(circles[2]);
				int turn = GetNumFromParamString(circles[3]);
				int activeRound = GetNumFromParamString(circles[4]);
				bool use = circles.Length >= 6;

				foreach (BattleUnitModel targetModel in modelList) {
					int stack_temp = stack;
					int turn_temp = turn;
					if (stack_temp < 0) {
						if (use) targetModel.UseBuffStack(buf_keyword, stack_temp * -1, battleTiming);
						else targetModel.LoseBuffStack(buf_keyword, stack_temp * -1, battleTiming, activeRound);
						stack_temp = 0;
					}
					if (turn_temp < 0) {
						if (use) targetModel.UseBuffTurn(buf_keyword, turn_temp * -1, battleTiming);
						else targetModel.LoseBuffTurn(buf_keyword, turn_temp * -1, battleTiming);
						turn_temp = 0;
					}
					if (stack_temp > 0 || turn_temp > 0) {
						//AbilityTriggeredData_GiveBuff triggerData = new AbilityTriggeredData_GiveBuff(buf_keyword, stack_temp, turn_temp, activeRound, false, true, targetModel.InstanceID, battleTiming, BUF_TYPE.Neutral);
						if (activeRound == 2) {
							if (abilityMode == 2) { 
								dummyPassiveAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, 0, battleTiming, modsa_selfAction);
								dummyPassiveAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, 1, battleTiming, modsa_selfAction);
							}
							else if (abilityMode == 1) { 
								dummyCoinAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, 0, battleTiming, modsa_selfAction);
								dummyCoinAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, 1, battleTiming, modsa_selfAction);
							}
							else {
								dummySkillAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, 0, battleTiming, modsa_selfAction);
								dummySkillAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, 1, battleTiming, modsa_selfAction);
							}
						}
						else {
							if (abilityMode == 2) dummyPassiveAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, activeRound, battleTiming, modsa_selfAction);
							else if (abilityMode == 1) dummyCoinAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, activeRound, battleTiming, modsa_selfAction);
							else dummySkillAbility.GiveBuff_Self(targetModel, buf_keyword, stack_temp, turn_temp, activeRound, battleTiming, modsa_selfAction);
						}
					}
				}
			}
				break;
			case "shield":
			{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				int amount = GetNumFromParamString(circles[1]);
				bool permashield = circles.Length > 2;
				foreach (BattleUnitModel targetModel in modelList)
				{
					if (amount >= 0) targetModel.AddShield(amount, !permashield, ABILITY_SOURCE_TYPE.SKILL, battleTiming);
					else targetModel.ReduceShield(battleTiming, amount *= -1, modsa_unitModel);
				}
			}
				break;
			//case "activatebuf":
			//	{
			//		List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
			//		BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
			//		int stack = 0;
			//		int count = 1;
			//		int limit = 99;
			//		if (circles.Length > 2) count = GetNumFromParamString(circles[2]);
			//		if (circles.Length > 3) stack = GetNumFromParamString(circles[3]);
			//		if (circles.Length > 4) limit = GetNumFromParamString(circles[4]);

			//		var nullableLimit = new Il2CppSystem.Nullable<int>(limit);
			//		//var nullableLimit = new Il2CppSystem.Nullable<int>(99);
			//		//if (!nullableLimit.HasValue)
			//		//{
			//		//	nullableLimit.value = limit;
			//		//}

			//		foreach (BattleUnitModel targetModel in modelList)
			//		{
			//			var buff = targetModel.GetBuffInfo(buf_keyword, 0);
			//			if (buff != null && buff.IsSinBuff())
			//			{
			//				MainClass.Logg.LogMessage($"Calling ForceToActivateSinBuffEffect: stack={stack}, count={count}, limit={nullableLimit.Value}, battleTiming= {battleTiming}, keyword={buf_keyword}");
			//				try { targetModel.ForceToActivateSinBuffEffect(modsa_unitModel, stack, count, nullableLimit, battleTiming, buf_keyword);}
			//				catch (Exception ex) { MainClass.Logg.LogWarning("Error in ForceToActivateSinBuf	fEffect: " + ex); }			
			//			}
			//			else
			//			{
			//				MainClass.Logg.LogMessage("Activating ForceToActivateBuffEffect");
			//				try { targetModel.ForceToActivateBuffEffect(buf_keyword, modsa_unitModel, stack, count, nullableLimit, battleTiming); }
			//				catch (Exception ex) { MainClass.Logg.LogWarning("Error in ForceToActivateBuffEffect: " + ex); }
			//			}
			//		}
			//	}
			//	break;
			case "break":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				if (modelList.Count < 1) return;
				string opt2_string = circles.Length >= 2 ? circles[1] : "natural";
				bool force = opt2_string != "natural";
				bool both = opt2_string == "both";
				bool resistancebreak = circles.Length <= 2;

				foreach (BattleUnitModel targetModel in modelList)
				{
					ABILITY_SOURCE_TYPE abilitySourceType = ABILITY_SOURCE_TYPE.SKILL;
					if (abilityMode == 2) abilitySourceType = ABILITY_SOURCE_TYPE.PASSIVE;

					if (force) targetModel.BreakForcely(modsa_unitModel, abilitySourceType, battleTiming, false, modsa_selfAction);
					if (!force || both) targetModel.Break(modsa_unitModel, battleTiming, modsa_selfAction);
					if (resistancebreak) targetModel.ChangeResistOnBreak();
				}
			}
				break;
			case "breakdmg":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				if (modelList.Count < 1) return;
				int amount = GetNumFromParamString(circles[1]);
				if (amount == 0) return;
				bool isBreakDmg = amount < 0;
				if (isBreakDmg) amount *= -1;
				int times = 1;
				if (circles.Length > 2) times = GetNumFromParamString(circles[2]);

				foreach (BattleUnitModel targetModel in modelList) {
					for (int times_i = 0; times_i < times; times_i++) {
						if (isBreakDmg) {
							//AbilityTriggeredData_BsGaugeDown triggerData = new AbilityTriggeredData_BsGaugeDown(amount, targetModel.InstanceID, battleTiming);
							if (abilityMode == 2) dummyPassiveAbility.FirstBsGaugeDown(modsa_unitModel, targetModel, amount, battleTiming);
							else if (abilityMode == 1) dummyCoinAbility.FirstBsGaugeDown(modsa_unitModel, targetModel, amount, battleTiming);
							else dummySkillAbility.FirstBsGaugeDown(modsa_unitModel, targetModel, amount, battleTiming);
						} else {
							//AbilityTriggeredData_BsGaugeUp triggerData = new AbilityTriggeredData_BsGaugeUp(amount, targetModel.InstanceID, battleTiming);
							if (abilityMode == 2) dummyPassiveAbility.FirstBsGaugeUp(modsa_unitModel, targetModel, amount, battleTiming, false);
							else if (abilityMode == 1) dummyCoinAbility.FirstBsGaugeUp(modsa_unitModel, targetModel, amount, battleTiming, false, modsa_selfAction);
							else dummySkillAbility.FirstBsGaugeUp(modsa_unitModel, targetModel, amount, battleTiming, false, modsa_selfAction);
							//targetModel.AddTriggeredData(triggerData);
							//dummySkillAbility.AddTriggeredData_BsGaugeUp(amount, targetModel.InstanceID, battleTiming, false);
						}
					}
				}
			}
				break;
			case "breakrecover":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				bool force = circles.Length > 1;
				foreach (BattleUnitModel targetModel in modelList)
				{
					if (force) targetModel.RecoverAllBreak(battleTiming);
					else targetModel.RecoverBreak(battleTiming);
				}
			}
				break;
			case "breakaddbar":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				if (modelList.Count < 1) return;
				string circle_1 = circles[1];
				bool scaleWithHealth = false;
				if (circle_1.EndsWith("%"))
				{
					scaleWithHealth = true;
					circle_1 = circle_1.Remove(circle_1.Length - 1, 1);
				}
				int healthpoint = circles.Length >= 2 ? GetNumFromParamString(circle_1) : 50;

				foreach (BattleUnitModel targetModel in modelList) {
					int finalPoint = healthpoint;
					if (scaleWithHealth) {
						int maxHP = targetModel.MaxHp;
						finalPoint = maxHP * healthpoint / 100;
					}
					else targetModel.AddBreakSectionForcely(healthpoint);
				}
			}
				break;
			case "explosion":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				int times = GetNumFromParamString(circles[1]);
				bool tremorCheck = circles.Length > 2;
				foreach (BattleUnitModel targetModel in modelList)
				{
					if (tremorCheck) {
						int tremorStack = targetModel._buffDetail.GetActivatedBuffStack(BUFF_UNIQUE_KEYWORD.Vibration, false);
						if (tremorStack < 1) continue;
					}
					for (int times_i = 0; times_i < times; times_i++) {
						if (abilityMode == 2) {
							//dummyPassiveAbility.AddTriggeredData_BsGaugeUp(tremorStack, targetModel.InstanceID, battleTiming, true);
							//dummyPassiveAbility.FirstBsGaugeUp(modsa_unitModel, targetModel, tremorStack, battleTiming, true);
							targetModel.VibrationExplosion(battleTiming, modsa_unitModel, dummyPassiveAbility);
						}
						else if (abilityMode == 1) {
							//dummyCoinAbility.AddTriggeredData_BsGaugeUp(tremorStack, targetModel.InstanceID, battleTiming, true);
							//dummyCoinAbility.FirstBsGaugeUp(modsa_unitModel, targetModel, tremorStack, battleTiming, true, modsa_selfAction, modsa_coinModel);
							targetModel.VibrationExplosion(battleTiming, modsa_unitModel, dummyCoinAbility, modsa_selfAction, modsa_coinModel);
						} else {
							//dummySkillAbility.AddTriggeredData_BsGaugeUp(tremorStack, targetModel.InstanceID, battleTiming, true);
							//dummySkillAbility.FirstBsGaugeUp(modsa_unitModel, targetModel, tremorStack, battleTiming, true, modsa_selfAction);
							targetModel.VibrationExplosion(battleTiming, modsa_unitModel, dummySkillAbility, modsa_selfAction);
						}
					}
				}
			}
				break;
			case "healhp":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				if (modelList.Count < 1) return;

				string circle_1 = circles[1];
				bool percentageheal = circle_1.Last() == '%';
				if (percentageheal) circle_1 = circle_1.Remove(circle_1.Length - 1);
				int amount = GetNumFromParamString(circle_1);

				foreach (BattleUnitModel targetModel in modelList) {
					int finalAmount = amount;
					if (percentageheal) finalAmount = targetModel.MaxHp * finalAmount / 100;
					if (finalAmount < 0) targetModel.TakeAbsHpDamage(null, finalAmount * -1, out _, out _, battleTiming, DAMAGE_SOURCE_TYPE.NONE);
					else {
						if (abilityMode == 2) {
							dummyPassiveAbility.AddTriggeredData_HpHeal(finalAmount, targetModel.InstanceID, battleTiming);
							//dummyPassiveAbility.HealTargetHp(modsa_unitModel, modsa_selfAction, targetModel, finalAmount, battleTiming, finalAmount);
							targetModel.TryRecoverHp(modsa_unitModel, null, finalAmount, ABILITY_SOURCE_TYPE.PASSIVE, battleTiming, out _);
						}
						else if (abilityMode == 1) {
							dummyCoinAbility.AddTriggeredData_HpHeal(finalAmount, targetModel.InstanceID, battleTiming);
							//dummyCoinAbility.HealTargetHp(modsa_unitModel, modsa_selfAction, targetModel, finalAmount, battleTiming, finalAmount);
							targetModel.TryRecoverHp(modsa_unitModel, null, finalAmount, ABILITY_SOURCE_TYPE.SKILL, battleTiming, out _);
						}
						else {
							dummySkillAbility.AddTriggeredData_HpHeal(finalAmount, targetModel.InstanceID, battleTiming);
							//dummySkillAbility.HealTargetHp(modsa_unitModel, modsa_selfAction, targetModel, finalAmount, battleTiming, finalAmount);
							targetModel.TryRecoverHp(modsa_unitModel, null, finalAmount, ABILITY_SOURCE_TYPE.SKILL, battleTiming, out _);
						}
					}
					
				}

			}
				break;
			case "pattern":{
				BattleUnitModel_Abnormality abnoModel = null;
				if (modsa_unitModel.IsAbnormalityOrPart)
				{
					if (modsa_unitModel.TryCast<BattleUnitModel_Abnormality_Part>() != null) {
						abnoModel = modsa_unitModel.TryCast<BattleUnitModel_Abnormality_Part>().Abnormality;
					}
					else {
						abnoModel = modsa_unitModel.TryCast<BattleUnitModel_Abnormality>();
					}
				}
				else return;
					
				if (abnoModel == null) return;

				PatternScript_Abnormality pattern = abnoModel.PatternScript;

				//List<BattlePattern> battlePattern_list = pattern._patternList;

				int pickedPattern_idx = GetNumFromParamString(circles[0]);
				MainClass.Logg.LogInfo("pickedPattern_idx: " + pickedPattern_idx);
				pattern.currPatternIdx = pickedPattern_idx;
				//int slotCount = -1;
				//bool randomize = false;

				//pattern.PickSkillsByPattern(pickedPattern_idx, slotCount, randomize);
			}
				break;
			case "setslotadder":{
				int amount = GetNumFromParamString(circles[1]);
				if (amount < 0) return;
				bool add_max_instead = circles.Length > 2;
				if (!add_max_instead) { slotAdder = amount; return; }

				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				foreach (BattleUnitModel targetModel in modelList) {
					if (targetModel == null || !targetModel.IsAbnormalityOrPart) continue;
					BattleUnitModel_Abnormality abnoModel;
					BattleUnitModel_Abnormality_Part part = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
					if (part != null) abnoModel = part.Abnormality;
					else abnoModel = targetModel.TryCast<BattleUnitModel_Abnormality>();
					if (abnoModel == null) continue;
					PatternScript_Abnormality pattern = abnoModel.PatternScript;
					pattern._slotMax = amount;
				}
			}
				break;
			case "setdata":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				if (modelList.Count < 1) return;

				int dataID = GetNumFromParamString(circles[1]);
				int dataValue = GetNumFromParamString(circles[2]);

				foreach (BattleUnitModel targetModel in modelList)
				{
					long targetPtr_intlong = targetModel.Pointer.ToInt64();
					SkillScriptInitPatch.SetModUnitData(targetPtr_intlong, dataID, dataValue);
				}
			}
				break;
			case "changeskill": if (modsa_selfAction != null) modsa_selfAction.TryChangeSkill(GetNumFromParamString(circledSection));
				break;
			case "setimmortal":{
				int amount = GetNumFromParamString(circledSection);
				immortality = amount > 0;
			}
				break;
			case "retreat":
				{
					BattleObjectManager battleObjectManager_inst = SingletonBehavior<BattleObjectManager>.Instance;
					if (battleObjectManager_inst == null) return;

					List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
					BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
					//if (modelList.Count < 1) continue;
					//bool comeback = circles.Length > 1;

					foreach (BattleUnitModel targetModel in modelList)
					{
						battleObjectManager_inst.TryReservateForRetreat(targetModel, modsa_unitModel, buf_keyword);
					}
				}
				break;
			case "aggro":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				int amount = GetNumFromParamString(circles[1]);
				bool nextRound = true;
				int slot = -2;
				if (circles.Length > 2) nextRound = circles[2] == "next";
				if (circles.Length > 3) slot = GetNumFromParamString(circles[3]);

				foreach (BattleUnitModel targetModel in modelList)
				{
					List<SinActionModel> sinActionList = targetModel.GetSinActionList();
					int sinActionCount = sinActionList.Count;
					if (sinActionCount < 1) continue;

					if (targetModel == modsa_unitModel)
					{
						if (slot == -2 && modsa_selfAction != null) {
							if (nextRound) modsa_selfAction.SinAction.StackNextTurnAggroAdder(amount);
							else modsa_selfAction.SinAction.StackThisTurnAggroAdder(amount);
						} else if (slot == -1) {
							int quotient = amount / sinActionCount;
							int remainder = amount % sinActionCount;

							foreach (SinActionModel sinAction in sinActionList) {
								int finalAmount = amount;
								if (remainder > 0) {
									finalAmount += 1;
									remainder -= 1;
								}
								if (nextRound) sinAction.StackNextTurnAggroAdder(finalAmount);
								else sinAction.StackThisTurnAggroAdder(finalAmount);
							}
						} else {
							int chosenSlot = slot;
							if (chosenSlot > sinActionCount - 1) chosenSlot = sinActionCount - 1;
							if (chosenSlot < 0) chosenSlot = 0;
							if (nextRound) sinActionList.ToArray()[chosenSlot].StackNextTurnAggroAdder(amount);
							else sinActionList.ToArray()[chosenSlot].StackThisTurnAggroAdder(amount);
						}
					}
					else
					{
						int chosenSlot = Math.Min(slot, sinActionCount - 1);
						if (chosenSlot == -2) chosenSlot = 0;
							
						if (chosenSlot == -1) {
							int quotient = amount / sinActionCount;
							int remainder = amount % sinActionCount;

							foreach (SinActionModel sinAction in sinActionList)
							{
								int finalAmount = quotient;
								if (remainder > 0) {
									finalAmount += 1;
									remainder -= 1;
								}
								if (finalAmount < 1) break;
								if (nextRound) sinAction.StackNextTurnAggroAdder(finalAmount);
								else sinAction.StackThisTurnAggroAdder(finalAmount);
							}
							continue;
						} else {
							if (nextRound) sinActionList.ToArray()[chosenSlot].StackNextTurnAggroAdder(amount);
							else sinActionList.ToArray()[chosenSlot].StackThisTurnAggroAdder(amount);
						}
						
					}
						
				}
			}
				break;
			case "skillsend":{
				BattleUnitModel fromUnit = GetTargetModel(circles[0]);
				if (fromUnit == null || fromUnit.IsDead()) return;

				int skillID = -1;
				string circle_2 = circles[2];
				if (circle_2[0] == 'S')
				{
					int tier = 0;
					int.TryParse(circle_2[1].ToString(), out tier);
					if (tier > 0)
					{
						List<int> skillIDList = fromUnit.GetSkillIdByTier(tier);
						if (skillIDList.Count > 0) skillID = skillIDList.ToArray()[0];
					}
				}
				else if (circle_2[0] == 'D')
				{
					int index = 0;
					if (int.TryParse(circle_2[1].ToString(), out index)) index -= 1;
					List<int> skillIDList = fromUnit.GetDefenseSkillIDList();
					index = Math.Min(index, skillIDList.Count - 1);
					skillID = skillIDList.ToArray()[index];
				}
				else int.TryParse(circle_2, out skillID);
				if (skillID < 0) return;

				SinActionModel fromSinAction_new = fromUnit.AddNewSinActionModel();
				UnitSinModel fromSinModel_new = new UnitSinModel(skillID, fromUnit, fromSinAction_new);
				BattleActionModel fromAction_new = new BattleActionModel(fromSinModel_new, fromUnit, fromSinAction_new);
				//fromAction_new._targetDataDetail.ClearAllTargetData(fromAction_new);
					
				List<SinActionModel> targetSinActionList = new List<SinActionModel>();
				List<BattleUnitModel> targetList = GetTargetModelList(circles[1]);
				foreach (BattleUnitModel targetModel in targetList) {
					List<SinActionModel> sinActionList = targetModel.GetSinActionList();
					if (sinActionList.Count < 1) continue;
					targetSinActionList.Add(sinActionList.ToArray()[0]);
					//fromAction_new._targetDataDetail.AddTargetSinAction(sinActionList.ToArray()[0]);
				}
				//fromAction_new._targetDataDetail.SetOriginTargetSinAction(fromAction_new, targetSinActionList);
				//fromAction_new.SetOriginTargetSinActions(targetSinActionList);
					
				/*TargetDataSet targetDataSet = fromAction_new._targetDataDetail.GetCurrentTargetSet();
				if (!targetSinActionList.Contains(targetDataSet._mainTarget.GetTargetSinAction())) {
					if (targetSinActionList.Count > 0) targetDataSet.SetMainTargetSinAction(targetSinActionList.ToArray()[0]);
				}
				List<TargetSinActionData> goodones = new List<TargetSinActionData>();
				foreach (TargetSinActionData targetSinActionData in targetDataSet._subTargetList) {
					if (targetSinActionList.Contains(targetSinActionData.GetTargetSinAction())) goodones.Add(targetSinActionData);
				}
				targetDataSet._subTargetList = goodones;*/
					
				fromAction_new._targetDataDetail.ReadyOriginTargeting(fromAction_new);
				if (circles.Length > 3) fromUnit.CutInDefenseActionForcely(fromAction_new, true);
				else {
					fromUnit.CutInAction(fromAction_new);
					if (targetSinActionList.Count > 0) {
						fromAction_new.ChangeMainTargetSinAction(targetSinActionList.ToArray()[0], null, true);
					}
				}
			}
				break;
			case "skillreuse": foreach (BattleUnitModel targetModel in GetTargetModelList(circledSection)) targetModel.ReuseAction(modsa_selfAction);
				break;
			case "skillslotreplace": {
					if (modsa_unitModel == null) {
						MainClass.Logg.LogInfo("skillslotreplace Self == null");
						return;
					}

					List<SinActionModel> sinActionList = modsa_unitModel.GetSinActionList();
					int skillID_1 = GetNumFromParamString(circles[1]);
					int skillID_2 = GetNumFromParamString(circles[2]);


					if (circles[1] == "All") 
					{ 
						foreach (SinActionModel sinslot in sinActionList)
						{
							var skillList = sinslot.UnitModel.GetSkillList();
							foreach (SkillModel x in skillList)
							{
								var x_id = x.GetID();
								sinslot.ReplaceSkillAtoB(x_id, skillID_2);
							}

						}
					}
					else if (circles[0] == "All")
					{
						foreach (SinActionModel sinslot in sinActionList) sinslot.ReplaceSkillAtoB(skillID_1, skillID_2);
					}
					else
					{
						int slot = GetNumFromParamString(circles[0]);
						if (slot >= sinActionList.Count || slot < 0)
						{
							MainClass.Logg.LogInfo("skillslotreplace invalid slot");
							return;
						}
						sinActionList.ToArray()[slot].ReplaceSkillAtoB(skillID_1, skillID_2);
					}
			}
				break;
			case "resource":{
				SinManager sinmanager_inst = Singleton<SinManager>.Instance;
				SinManager.EgoStockManager stock_manager = sinmanager_inst._egoStockMangaer;

				ATTRIBUTE_TYPE sin = ATTRIBUTE_TYPE.NONE;
				Enum.TryParse(circles[0], true, out sin);

				int amount = GetNumFromParamString(circles[1]);

				UNIT_FACTION faction = modsa_unitModel.Faction;
				UNIT_FACTION enemyFaction = faction == UNIT_FACTION.PLAYER ? UNIT_FACTION.ENEMY : UNIT_FACTION.PLAYER;
				if (circles.Length >= 3) faction = enemyFaction;

				if (amount >= 0) stock_manager.AddSinStock(faction, sin, amount, 0);
				else stock_manager.RemoveSinStock(faction, sin, amount * -1);
			}
				break;
			case "discard":{
				SORT_SKILL_BY_INDEX sorting;
				Enum.TryParse(circles[0], true, out sorting);
				int times = GetNumFromParamString(circles[1]);
				for (int i = 0; i < times; i++) modsa_unitModel.DiscardSkill(battleTiming, sorting, modsa_selfAction);
			}
				break;
			case "passiveadd":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				int id = GetNumFromParamString(circles[1]);
				foreach (BattleUnitModel targetModel in modelList) targetModel.AddPassive(id);
			}
				break;
			case "passiveremove":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				int id = GetNumFromParamString(circles[1]);
				foreach (BattleUnitModel targetModel in modelList) {
					List<PassiveModel> removal = new();
					foreach (PassiveModel passive in targetModel.GetPassiveList()) {
						if (passive.GetID() == id) removal.Add(passive);
					}
					foreach (PassiveModel passive in removal) targetModel.GetPassiveList().Remove(passive);
				}
			}
				break;
			case "endstage": Singleton<StageController>.Instance.EndStage(); break;
			case "endbattle": Singleton<StageController>.Instance.EndBattlePhaseForcely(true); break;
			case "endlimbus": {
				Environment.FailFast("Immediately crashed Limbus");
				}
				break;
			case "skillteamkill": break;
			case "skillcanduel": modsa_skillModel.OverrideCanDuel(circledSection == "True"); break;
			case "skillchangetarget": break;
			case "skilltargetnum": break;
			case "skillcancel": break;
			case "skillslotgive":{
				SinManager sinManager_inst = Singleton<SinManager>.Instance;
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				foreach (BattleUnitModel targetModel in modelList) {
					sinManager_inst.AddSinActionModelOnRoundStart(UNIT_FACTION.PLAYER, targetModel.InstanceID);
				}
			}
				break;
			case "doubleslot":{
				foreach (BattleUnitModel targetModel in GetTargetModelList(circles[0])) {
					int instID = targetModel.InstanceID;
					if (StagePatches.doubleslotterIDList.Contains(instID)) continue;
					StagePatches.doubleslotterIDList.Add(instID);
				}
			}
				break;
			case "passivereveal":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				int pasID = GetNumFromParamString(circles[1]);
				UnlockInformationManager unlockInfo_inst = Singleton<UnlockInformationManager>.Instance;
				foreach (BattleUnitModel targetModel in modelList) {
					unlockInfo_inst.UnlockPassiveStatus(targetModel.GetOriginUnitID(), pasID);
				}
			}
				break;
			case "appearance":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]); 
				foreach (BattleUnitModel targetModel in modelList) {
					SingletonBehavior<BattleObjectManager>.Instance.GetView(targetModel).ChangeAppearance(circles[1], true); 
				}
			}
				break;
			case "coincancel":{
				foreach (string circle in circles) {
					int idx = GetNumFromParamString(circle);
					if (idx < 0) { modsa_skillModel.DisableCoin(modsa_coinModel.GetOriginCoinIndex()); continue; }

					idx = Math.Min(idx, modsa_skillModel.CoinList.Count - 1);
					modsa_skillModel.DisableCoin(idx);
				}
			}
				break;
			case "summonassistant":{
				int assistantID = GetNumFromParamString(circles[0]);
				int assistantlevel = GetNumFromParamString(circles[1]);
				int assistantSynclevel = GetNumFromParamString(circles[2]);
				BattleUnitModel summonedUnit = BattleObjectManager.Instance.CreateAssistantUnit(assistantID, assistantlevel, assistantSynclevel, assistantID);
				summonedUnit.InitActionSlots();

				var aliveList = BattleObjectManager.Instance.GetAliveList(true);
				foreach (BattleUnitModel unit in aliveList) unit.RefreshSpeed();
			}
				break;
			case "summonenemy":{
				int enemy_ID = GetNumFromParamString(circles[0]);
				int enemy_level = GetNumFromParamString(circles[1]);
				int enemy_Synclevel = GetNumFromParamString(circles[2]);
				int waveIndex = GetNumFromParamString(circles[3]);
				bool isenemy = circles.Length >= 5;
				BattleUnitModel summonedUnit = BattleObjectManager.Instance.CreateEnemyUnit(enemy_ID, enemy_level, enemy_Synclevel, waveIndex, enemy_ID, null, UNIT_POSITION.MAIN);
				if (isenemy) summonedUnit.InitActionSlots();

				var aliveList = BattleObjectManager.Instance.GetAliveList(true);
				foreach (BattleUnitModel unit in aliveList) unit.RefreshSpeed();
			}
				break;
			case "summonunitfromqueue": BattleObjectManager.Instance.FlushAddUnitOnlyQueue(); break;
			case "stack": {
				if (modsa_buffModel == null) return;
				int adder = GetNumFromParamString(circles[0]);
				if (adder > 0) {
					List<BuffHistory> bufHistoryList = new List<BuffHistory>();
					BuffHistory bufHistory = new(adder, 0, ABILITY_SOURCE_TYPE.BUFF);
					bufHistoryList.Add(bufHistory);
					modsa_buffModel.AddBuffStackOrTurn(modsa_unitModel, bufHistoryList, 0, ABILITY_SOURCE_TYPE.BUFF, battleTiming, null, out _, out _);
				}
				else modsa_buffModel.LoseStack(modsa_unitModel, 0, battleTiming, adder * -1);
			}
				break;
			case "turn":{
				if (modsa_buffModel == null) return;
				int adder = GetNumFromParamString(circles[0]);
				if (adder > 0) {
					List<BuffHistory> bufHistoryList = new List<BuffHistory>();
					BuffHistory bufHistory = new(0, adder, ABILITY_SOURCE_TYPE.BUFF);
					bufHistoryList.Add(bufHistory);
					modsa_buffModel.AddBuffStackOrTurn(modsa_unitModel, bufHistoryList, 0, ABILITY_SOURCE_TYPE.BUFF, battleTiming, null, out _, out _);
				}
				else modsa_buffModel.LoseTurn(modsa_unitModel, battleTiming, adder * -1);
			}
				break;
			case "vibrationswitch":{
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
				bool isEntangled = circles.Length > 2;
				foreach (BattleUnitModel targetModel in modelList) {
					targetModel.SwitchVibrationToSpecial(modsa_unitModel, buf_keyword, battleTiming, ABILITY_SOURCE_TYPE.SKILL, modsa_selfAction, out _, out _, out _, out _, isEntangled);
				}
			}
				break;
			case "changemap": {
				string mapname = circles[0];
				float mapsize = GetNumFromParamString(circles[1]);
				BattleMapManager.Instance.LoadAndAddMap(mapname, mapsize);
				BattleMapManager.Instance.ChangeMap(mapname, mapsize);
			}
				break;
			case "battledialogline": {
					string line_played = circledSection.Remove(0, circles[0].Length + 1);
					line_played = Regex.Replace(line_played, @"_", " ");
			  
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				foreach (BattleUnitModel targetModel in modelList) {
					BattleUnitView view = BattleObjectManager.Instance.GetView(targetModel);
					BattleDialogLine dialogline = new BattleDialogLine(line_played, "");
					view._uiManager.ShowDialog(dialogline);
				}
			}
				break;
			case "gnome":{
				BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
				List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
				foreach (BattleUnitModel targetModel in modelList) {
					BattleUnitView unitView = objManager.GetView(targetModel);
					if (unitView == null) continue;
					Vector3 vec3 = unitView.transform.localScale;
					unitView.transform.localScale = new Vector3(vec3.x,vec3.y * 0.5f,vec3.z);
				}
			}
				break;
			case "effectlabel":
				{
					List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
					BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
					bool effectActive = GetNumFromParamString(circles[2]) != 0;
					EFFECT_LAYER_TYPE layerType = EFFECT_LAYER_TYPE.NONE;
					Enum.TryParse(circles[3], true, out layerType);
					foreach (BattleUnitModel targetModel in modelList)
					{
						BattleUnitView unitView = objManager.GetView(targetModel);
						if (unitView == null) continue;
						unitView.SetEffect_Label(circles[1], effectActive, layerType);
					}
				}
				break;
			case "sanchoshield":
				{
					List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
					bool effectActive = GetNumFromParamString(circles[1]) != 0;
					foreach (BattleUnitModel targetModel in modelList){
						var appearance = SingletonBehavior<BattleObjectManager>.Instance.GetViewAppaearance(targetModel);
						if (appearance == null) continue;
						var berserkAppearance = appearance.TryCast<CharacterAppearance_1079>();
						var managerAppearance = appearance.TryCast<CharacterAppearance_8380>();
						var identityAppearance = appearance.TryCast<CharacterAppearance_10310>();
						berserkAppearance?.SetShieldEffect(effectActive);
						managerAppearance?.SetShieldEffect(effectActive);
						identityAppearance?.SetShieldEffect(effectActive);
					}
					break;
				}
			case "sfx":
				{
					string guid = circles[0];
					SoundGenerator._soundManager.PlayOnShot(GUID.Parse(circles[0]));
				}
				break;
			case "bgm":
				{
					BattleSoundGenerator.StopBGM();
					var Bgm = RuntimeManager.CreateInstance(GUID.Parse(circles[0]));
					SingletonBehavior<SoundManager>.Instance.ChangeBGM(Bgm);
				}
				break;
			case "addability":
				{
					SYSTEM_ABILITY_KEYWORD ability = SYSTEM_ABILITY_KEYWORD.NONE;
					List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
					Enum.TryParse(circles[1], true, out ability);
					int stack = GetNumFromParamString(circles[2]);
					int turn = GetNumFromParamString(circles[3]);
					int activeround = GetNumFromParamString(circles[4]);
					foreach (BattleUnitModel targetModel in modelList)
					{
						if (activeround == 0) { SystemAbility addAbility = targetModel._systemAbilityDetail.FindOrAddAbilityThisRound(targetModel.InstanceID, ability, stack, turn); }
						else { SystemAbility addAbility = targetModel._systemAbilityDetail.FindOrAddAbilityNextRound(targetModel.InstanceID, ability, stack, turn); }
					}
				}
				break;
			case "removeability":
				{
					SYSTEM_ABILITY_KEYWORD ability = SYSTEM_ABILITY_KEYWORD.NONE;
					List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
					Enum.TryParse(circles[1], true, out ability);
					foreach (BattleUnitModel targetModel in modelList)
					{
						targetModel._systemAbilityDetail.DestoryAbility(ability);
					}
				}
				break;
			default: MainClass.Logg.LogInfo("Invalid Consequence: " + mEth); break;
		}
	}


	private int DoMath(string s) {
		MatchCollection symbols = MainClass.mathsymbolRegex.Matches(s);
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
	
	private int AcquireValue(int setvalue_idx, string section)
	{
		if (MainClass.logEnabled) MainClass.Logg.LogInfo("AcquireValue " + section);
		string[] sectionArgs = section.Split(parenthesisSeparator);

		if (char.IsNumber(section.Last())) return GetNumFromParamString(sectionArgs[0]);

		string methodology = sectionArgs[0];
		string circledSection = "";
		if (sectionArgs.Length > 1) circledSection = sectionArgs[1];
		string[] circles = circledSection.Split(',');

		int finalResult = -1; // Default -1
		
		switch (methodology)
		{
			case "math": finalResult = DoMath(circledSection); break;
			case "mpcheck": {
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel == null) break;
				finalResult = targetModel.Mp;
			}
				break;
			case "hpcheck":{
				BattleUnitModel targetModel = GetTargetModel(circles[0]);
				if (targetModel == null) break;

				int hp = targetModel.Hp;
				int hp_max = targetModel.MaxHp;
				float hp_ptg = (float)hp / hp_max;
				int hp_ptg_floor = (int)Math.Floor(hp_ptg * 100.0);

				int finalValue = hp;
					if (circles[1] == "%") finalValue = hp_ptg_floor;
					else if (circles[1] == "max") finalValue = hp_max;
					else if (circles[1] == "missing") finalValue = hp_max - hp;
					else if (circles[1] == "missing%") finalValue = 100 - hp_ptg_floor;

				finalResult = finalValue;
			}
				break;
			case "bufcheck":{
				BattleUnitModel targetModel = GetTargetModel(circles[0]);
				if (targetModel == null) break;

				BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);

				BuffDetail bufDetail = targetModel._buffDetail;
				//BuffModel buf = bufDetail.FindActivatedBuff(buf_keyword, false);
				int stack = 0;
				int turn = 0;
				bool usedcheck = circles.Length >= 4;
				if (usedcheck) {
					stack = 0;
				}
				else {
					stack = bufDetail.GetActivatedBuffStack(buf_keyword, false);
					turn = bufDetail.GetActivatedBuffTurn(buf_keyword, false);
				}
					

				int finalValue = stack;
				string circle_2 = circles[2];
				if (circle_2 == "turn") finalValue = turn;
				else if (circle_2 == "+") finalValue = stack + turn;
				else if (circle_2== "*") finalValue = stack * turn;
				finalResult = finalValue;
			}
				break;
			case "timeget": {
				var balls = circles[0];
				int year = 1984;
				if (circles.Length >= 2)
				{
					year = GetNumFromParamString(circles[1]);
				}

				switch (balls)
				{
					case "dayofweek":	finalResult = (int)DateTime.Now.DayOfWeek; break;
					case "dayofmonth": finalResult = DateTime.Now.Day; break;
					case "dayofyear": finalResult = DateTime.Now.DayOfYear; break;
					case "hours": finalResult = DateTime.Now.Hour; break;
					case "minutes": finalResult = DateTime.Now.Minute; break;
					case "seconds": finalResult = DateTime.Now.Second; break;
					case "milliseconds": finalResult = DateTime.Now.Millisecond; break;
					case "ticks": finalResult = (int)DateTime.Now.Ticks; break;
					case "month": finalResult = DateTime.Now.Month; break;
					case "year": finalResult = DateTime.Now.Year; break;
					case "isleapyear": finalResult = DateTime.IsLeapYear(year) ? 0 : 1; break;
				}
			};
				break;
			case "getdmg": finalResult = lastFinalDmg;
				break;
			case "gethpdmg": finalResult = lastHpDmg;
				break;
			case "round":{
				StageController stageController_inst = Singleton<StageController>.Instance;
				finalResult = stageController_inst.GetCurrentRound();
			}
				break;
			case "wave":
			{
				StageController stageController_inst = Singleton<StageController>.Instance;
				finalResult = stageController_inst.GetCurrentWave();
			}
				break;
			case "activations": finalResult = activationCounter;
				break;
			case "unitstate":{
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel == null) break;
				if (targetModel.IsDead())
				{
					finalResult = 0;
					break;
				}
				finalResult = 1;
				if (targetModel.IsBreak()) finalResult = 2;

				if (targetModel is BattleUnitModel_Abnormality_Part)
				{
					BattleUnitModel_Abnormality_Part partModel = (BattleUnitModel_Abnormality_Part)targetModel;
					if (!partModel.IsActionable()) finalResult = 2;
				}
			}
				break;
			case "getid":{
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel != null) finalResult = targetModel.GetUnitID();
			}
				break;
			case "instid":{
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel != null) finalResult = targetModel.InstanceID;
			}
				break;
			case "speedcheck":{
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel == null) finalResult = 0;
				else finalResult = targetModel.GetIntegerOfOriginSpeed();
			}
				break;
			case "getpattern":{
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel == null) break;

				BattleUnitModel_Abnormality abnoModel = null;
				if (targetModel.IsAbnormalityOrPart)
				{
					// ReSharper disable once ConditionIsAlwaysTrueOrFalse
					if (targetModel?.TryCast<BattleUnitModel_Abnormality_Part>() != null)
					{
						abnoModel = targetModel.TryCast<BattleUnitModel_Abnormality_Part>().Abnormality;
					}
						
					// ReSharper disable once ConditionIsAlwaysTrueOrFalse
					if (targetModel?.TryCast<BattleUnitModel_Abnormality>() != null)
					{
						abnoModel = targetModel.TryCast<BattleUnitModel_Abnormality>();
					}
				}
				else break;
					
				if (abnoModel == null) break;
					
				PatternScript_Abnormality pattern = abnoModel.PatternScript;

				int pattern_idx = pattern.currPatternIdx;
				MainClass.Logg.LogInfo("getpattern pattern_idx: " + pattern_idx);
				finalResult = pattern.currPatternIdx;
			}
				break;
			case "getabnoslotmax":{
				finalResult = 0;
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel == null) break;

				BattleUnitModel_Abnormality abnoModel = null;
				if (targetModel is BattleUnitModel_Abnormality) abnoModel = (BattleUnitModel_Abnormality)targetModel;
				else if (targetModel is BattleUnitModel_Abnormality_Part)
				{
					BattleUnitModel_Abnormality_Part partModel = (BattleUnitModel_Abnormality_Part)targetModel;
					abnoModel = partModel.Abnormality;
				}
				if (abnoModel == null) break;

				PatternScript_Abnormality pattern = abnoModel.PatternScript;
				finalResult = pattern.SlotMax;
			}
				break;
			case "getdata":{
				BattleUnitModel targetModel = GetTargetModel(circles[0]);
				if (targetModel == null) break;

				int finalValue = 0;
				int dataID = GetNumFromParamString(circles[1]);
				long targetPtr_intlong = targetModel.Pointer.ToInt64();
					
				finalValue = SkillScriptInitPatch.GetModUnitData(targetPtr_intlong, dataID);
				finalResult = finalValue;
			}
				break;
			case "deadallies":{
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel == null) break;
				finalResult = targetModel.deadAllyCount;
			}
				break;
			case "random":{
				int minroll = GetNumFromParamString(circles[0]);
				int maxroll = GetNumFromParamString(circles[1]);
				finalResult = MainClass.rng.Next(minroll, maxroll + 1);
			}
				break;
			case "areallied":{
				BattleUnitModel targetModel1 = GetTargetModel(circles[0]);
				BattleUnitModel targetModel2 = GetTargetModel(circles[1]);
				if (targetModel1 == null || targetModel2 == null) break;

				finalResult = targetModel1.Faction == targetModel2.Faction ? 1 : 0;
			}
				break;
			case "getshield":{
				BattleUnitModel targetModel = GetTargetModel(circles[0]);
				if (targetModel != null) finalResult = targetModel.GetShield();
			}
				break;
			case "getdmgtaken":
				{
					BattleUnitModel targetModel = GetTargetModel(circles[0]);
					var turntime = circles[1];
					if (targetModel != null && turntime == "prev") finalResult = targetModel.GetHitAttackDamagePrevRound(); 
					else if (targetModel != null) finalResult = targetModel.GetHitAttackDamageThisRound();
				}
				break;
			case "getbuffcount":
				{
					BattleUnitModel targetModel = GetTargetModel(circles[0]);
					var type = circles[1];
					if (targetModel != null && type == "neg") finalResult = targetModel.GetNegativeBuffCount();
					else if (targetModel != null) finalResult = targetModel.GetPositiveBuffCount();
				}
				break;
			case "getskillid":{
				finalResult = 0;
				if (modsa_skillModel != null) finalResult = modsa_skillModel.GetID();
				else if (modsa_selfAction != null) finalResult = modsa_selfAction.Skill.GetID();
			}
				break;
			case "getcoincount":{
				BattleActionModel targetAction = modsa_selfAction;
				if (circles[0] == "Target") targetAction = modsa_oppoAction;
				if (targetAction == null) break;
				
				int coinCount = targetAction.Skill.GetAliveCoins().Count;
				if (circles[1] == "og") coinCount = targetAction.Skill.CoinList.Count;

				finalResult = coinCount;
			}
				break;
			case "allcoinstate":{
				BattleActionModel targetAction = modsa_selfAction;
				if (circles[0] == "Target") targetAction = modsa_oppoAction;
				if (targetAction == null) break;

				string way = circles[1];

				int coinCount = targetAction.Skill.GetAliveCoins().Count;
				int headCount = targetAction.GetHeadCoinNum();
				int tailCount = targetAction.GetTailCoinNum();
				int result = 0;
				if (way == "full")
				{
					if (coinCount == headCount) result = 1;
					else if (coinCount == tailCount) result = 2;
				}
				else if (way == "headcount") result = headCount;
				else if (way == "tailcount") result = tailCount;

				finalResult = result;
			}
				break;
			case "resonance":{
				finalResult = 0;
				SinManager sinmanager_inst = Singleton<SinManager>.Instance;
				SinManager.ResonanceManager res_manager = sinmanager_inst._resManager;
	
				ATTRIBUTE_TYPE sin; // default ATTRIBUTE_TYPE.NONE
				
				if (circledSection == "highres") {
					//List<ATTRIBUTE_TYPE> sinList = new List<ATTRIBUTE_TYPE>();
					//for (int i = 0; i<7; i++) sinList.Add((ATTRIBUTE_TYPE)i);
					//finalResult = res_manager.GetMaxAttributeResonanceOfAll(modsa_unitModel.Faction, out sinList);
					//List<ATTRIBUTE_TYPE> sinList = new List<ATTRIBUTE_TYPE>();
					int highest = 0;
					for (int i = 0; i < 7; i++) {
						int current = res_manager.GetAttributeResonance(modsa_unitModel.Faction, (ATTRIBUTE_TYPE)i);
						if (current > highest) highest = current;
					}
					finalResult = highest;
				}
				else if (circledSection == "highperfect") {
					finalResult = res_manager.GetMaxPerfectResonanceOfAll(modsa_unitModel.Faction, out _);
				}
				else if (circledSection.StartsWith("perfect")) {
					Enum.TryParse(circledSection.Remove(0,7), true, out sin);
					finalResult = res_manager.GetMaxPerfectResonance(modsa_unitModel.Faction, sin);
				}
				else if (Enum.TryParse(circledSection, true, out sin)) {
					finalResult = res_manager.GetAttributeResonance(modsa_unitModel.Faction, sin);
				}
			}
				break;
			case "resource":{
				finalResult = 0;

				SinManager sinmanager_inst = Singleton<SinManager>.Instance;
				SinManager.EgoStockManager stock_manager = sinmanager_inst._egoStockMangaer;

				ATTRIBUTE_TYPE sin = ATTRIBUTE_TYPE.NONE;
				UNIT_FACTION faction = modsa_unitModel.Faction;
				UNIT_FACTION enemyFaction = faction == UNIT_FACTION.PLAYER ? UNIT_FACTION.ENEMY : UNIT_FACTION.PLAYER;
				Enum.TryParse(circles[0], true, out sin);
				if (circles.Length >= 2) faction = enemyFaction;

				finalResult = stock_manager.GetAttributeStockNumberByAttributeType(faction, sin);
			}
				break;
			case "haskey":{
				BattleUnitModel targetModel = GetTargetModel(circles[0]);
				if (targetModel == null) break;

				if (targetModel.IsAbnormalityOrPart) {
					BattleUnitModel_Abnormality_Part part = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
					if (part != null) targetModel = part.Abnormality;
				}
				
				List<string> unitKeywordList = targetModel._unitDataModel.ClassInfo.unitKeywordList;
				List<string> associationList = targetModel._unitDataModel.ClassInfo.associationList;

				bool operator_OR = circles[1] == "OR";

				bool success = false;
				for (int i = 2; i < circles.Length; i++)
				{
					string keyword_string = circles[i];
					success = unitKeywordList.Contains(keyword_string) || associationList.Contains(keyword_string);

					if (operator_OR == success) break; // [IF Statement] Simplification
				}
				finalResult = success ? 1 : 0;
			}
				break;
			case "skillbase":{
				BattleActionModel action = modsa_selfAction;
				if (circledSection == "Target") action = modsa_oppoAction;
				if (action == null) break;
				finalResult = action.Skill.GetSkillDefaultPower();
			}
				break;
			case "skillatkweight":{
				BattleActionModel action = modsa_selfAction;
				if (circledSection == "Target") action = modsa_oppoAction;
				if (action == null) break;
				finalResult = action.Skill.GetAttackWeight(action);
			}
				break;
			case "onescale":{
				BattleActionModel action = modsa_selfAction;
				if (circles[0] == "Target") action = modsa_oppoAction;
				if (action == null) break;

				int coin_idx = GetNumFromParamString(circles[1]);
				int coinAmount = action.Skill.CoinList.Count;
				if (coinAmount < 1) break;
				if (coin_idx >= coinAmount) coin_idx = coinAmount - 1;

				finalResult = action.Skill.CoinList.ToArray()[coin_idx]._scale;
			}
				break;
			case "skillatk":{
				BattleActionModel action = modsa_selfAction;
				if (circles[0] == "Target") action = modsa_oppoAction;
				if (action == null) break;

				finalResult = (int)action.Skill.GetAttackType();
			}
				break;
			case "skillatklevel":{
				BattleActionModel action = modsa_selfAction;
				if (circles[0] == "Target") action = modsa_oppoAction;
				if (action == null) break;

				finalResult = action.Skill.GetSkillLevelCorrection();
			}
				break;
			case "skillattribute":{
				BattleActionModel action = modsa_selfAction;
				if (circles[0] == "Target") action = modsa_oppoAction;
				if (action == null) break;
				finalResult = (int)action.Skill.GetAttributeType();
			}
				break;
			case "skilldeftype": {
				BattleActionModel action = modsa_selfAction;
				if (circles[0] == "Target") action = modsa_oppoAction;
				if (action != null) finalResult = (int)action.Skill.GetDefenseType();
			}
				break;
			case "skillegotype": {
				BattleActionModel action = modsa_selfAction;
				if (circles[0] == "Target") action = modsa_oppoAction;
				if (action != null) finalResult = (int)action.Skill.GetSkillEgoType();
			}
				break;
			case "skillslotcount":{
				BattleUnitModel targetModel = GetTargetModel(circles[0]);
				if (targetModel == null) break;
				finalResult = targetModel.GetSinActionList().Count;
			}
				break;
			case "amountattacks":{
				SinManager sinManager_inst = Singleton<SinManager>.Instance;
				BattleUnitModel targetModel = GetTargetModel(circles[0]);
				if (targetModel == null || sinManager_inst == null) break;
				finalResult = sinManager_inst.GetActionListTargetingUnit(targetModel).Count;
			}
				break;
			case "getstat":{
				BattleUnitModel targetModel = GetTargetModel(circles[0]);
				if (targetModel == null) break;
				int value = -1;

				string circle_1 = circles[1];
				if (circle_1 == "deployment") value = targetModel.PARTICIPATE_ORDER;
				else if (circle_1 == "deadAllyCount") value = targetModel.deadAllyCount;
				else if (circle_1 == "panicType") value = Convert.ToInt32(targetModel._defaultPanicType);
				else if (circle_1 == "isRetreated") value = targetModel.IsRetreated() ? 1 : 0;
				else if (circle_1 == "hasMp") value = targetModel.HasMp() ? 1 : 0;
				else if (circle_1 == "deflevel") value = targetModel.GetDefense(out _, out _);
				//else if (circle_1 == "level") value = targetModel.Level;
				else if (circle_1.StartsWith("res")) {
					string word = circle_1.Remove(0, 3);
					ATK_BEHAVIOUR atk = ATK_BEHAVIOUR.NONE;
					Enum.TryParse(word, true, out atk);
					if (atk != ATK_BEHAVIOUR.NONE) {
						value = (int)(targetModel.GetAtkResistMultiplier(atk) * 100.0f);
					}
					else {
						ATTRIBUTE_TYPE sin = ATTRIBUTE_TYPE.NONE;
						Enum.TryParse(word, true, out sin);
						if (sin != ATTRIBUTE_TYPE.NONE) value = (int)(targetModel.GetAttributeResistMultiplier(sin) * 100.0f);
					}
				}
				else if (circle_1.StartsWith("speed")) {
					bool original = circle_1.Length >= 9;
					if (circle_1[7] == 'x') {
						value = targetModel.GetDefaultMaxSpeed();
						if (!original) value += targetModel.GetMaxSpeedAdder();
					}
					else {
						value = targetModel.GetDefaultMinSpeed();
						if (!original) value += targetModel.GetMinSpeedAdder();
					}
				}
				finalResult = value;
			}
				break;
			case "coinisbroken":{
				if (modsa_coinModel == null) break;
				finalResult = modsa_coinModel.IsUsableInDuel ? 0 : 1;
			}
			break;
			case "stack":{
				if (modsa_buffModel != null) finalResult = modsa_buffModel.GetStack(0);
			}
				break;
			case "turn":{
				if (modsa_buffModel != null) finalResult = modsa_buffModel.GetTurn(0);
			}
				break;
			case "isfocused": finalResult = Singleton<StageController>.Instance.IsAbnormalityBattle() ? 1 : 0; break;
			default: MainClass.Logg.LogInfo("Invalid Getter: " + methodology); break;
		}

		return finalResult;
	}
		
}
