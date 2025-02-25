using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using BattleUI;
using BattleUI.Operation;
using Dungeon;
using HarmonyLib;
using Il2CppSystem.Buffers;
using Il2CppSystem.Collections.Generic;
using UnhollowerRuntimeLib;
using UnityEngine;
using static BattleActionModel.TargetDataDetail;
using static BuffModel;
using static UIComponent.Icon.GaugeTextIconUI;
using IntPtr = System.IntPtr;

namespace ModularSkillScripts
{

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
			for (int i = 0; i < valueList.Length; i++)
			{
				valueList[i] = 0;
			}
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
			dummySkillAbility = null;
			dummyPassiveAbility = null;
			dummyCoinAbility = null;
			modsa_loopTarget = null;
			modsa_loopString = "";
		}

		public int activationTiming = 0;

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
		}
		public int coinScaleAdder = 0;
		public int skillPowerAdder = 0;
		public int skillPowerResultAdder = 0;
		public int parryingResultAdder = 0;
		public int atkDmgAdder = 0;
		public int atkMultAdder = 0;
		public int slotAdder = 0;

		public bool wasCrit = false;
		public bool wasClash = false;
		public bool wasWin = false;
		public int lastFinalDmg = 0;
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

		private bool immortality = false;
		private bool immortality_attempted = false;

		private bool _fullStop = false;
		BATTLE_EVENT_TIMING battleTiming = BATTLE_EVENT_TIMING.NONE;

		public void Enact(SkillModel skillModel_inst, int actevent, BATTLE_EVENT_TIMING timing)
		{
            //if (MainClass.logEnabled) MainClass.Logg.LogInfo("Enact");
            interactionTimer = 0;
            if (activationTiming != actevent) return;
            //if (activationTiming == 0) if (actevent != 0 && actevent != 0 && actevent != 0 &&)

            battleTiming = timing;
            if (skillModel_inst != null) modsa_skillModel = skillModel_inst;
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
				if (modsa_skillModel.SkillAbilityList.Count > 0) dummySkillAbility = modsa_skillModel.SkillAbilityList[0];
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
				if (modsa_coinModel.CoinAbilityList.Count > 0) dummyCoinAbility = modsa_coinModel.CoinAbilityList[0];
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
			List<BattleUnitModel> loopTarget_list = modsa_target_list;
			modsa_target_list.Clear();
			if (modsa_loopString.Any()) loopTarget_list = GetTargetModelList(modsa_loopString);
			else if (loopTarget_list.Count < 1) loopTarget_list.Add(GetTargetModel("MainTarget"));

			foreach (BattleUnitModel unit in loopTarget_list)
			{
				modsa_loopTarget = unit;
				_fullStop = false;
				for (int i = 0; i < batch_list.Count; i++)
				{
					if (_fullStop) break;
					string batch = batch_list[i];
					if (MainClass.logEnabled) MainClass.Logg.LogInfo("batch " + i.ToString() + ": " + batch);
					ProcessBatch(batch);
				}
			}
			activationCounter += 1;
		}

		public bool IsImmortal()
		{
			immortality_attempted = true;
			return immortality;
		}

		private bool CheckIF(string param)
		{
			string[] sectionArgs = param.Split(parenthesisSeparator);
			string circledSection = sectionArgs[1];
			if (MainClass.logEnabled) MainClass.Logg.LogInfo("IF circledSection: " + circledSection);

			MatchCollection symbols = Regex.Matches(circledSection, "(<|>|=)", RegexOptions.IgnoreCase);
			char[] ifSeparator = new char[] { '<', '>', '=' };
			string[] parameters = circledSection.Split(ifSeparator);
			string firstParam = parameters[0];
			string secondParam = parameters[1];

			int firstValue = GetNumFromParamString(firstParam);
			int secondValue = GetNumFromParamString(secondParam);

			bool success = false;
			string symbol = symbols[0].Value;
			if (symbol == "<") success = firstValue < secondValue;
			else if (symbol == ">") success = firstValue > secondValue;
			else if (symbol == "=") success = firstValue == secondValue;
			MainClass.Logg.LogInfo("ifsuccess: " + success);
			return success;
		}

		private int GetNumFromParamString(string param)
		{
			int value = 0;
			bool negative = false;
			if (param.StartsWith("VALUE_"))
			{
				int value_idx = 0;
				int.TryParse(param[6].ToString(), out value_idx);
				value = valueList[value_idx];
			}
			else
			{
				if (param.Last() == ')') param = param.Remove(param.Length - 1);
				negative = param[0] == '-';
				if (negative) param = param.Remove(0, 1);
				int.TryParse(param, out value);
			}
			if (negative) value *= -1;
			return value;
		}

		public List<BattleUnitModel> GetTargetModelList(string param)
		{
			List<BattleUnitModel> unitList = new List<BattleUnitModel>();
			if (param == "Target")
			{
				if (modsa_loopTarget != null) unitList.Add(modsa_loopTarget);
				return unitList;
			}
			else if (param == "MainTarget")
			{
				if (modsa_selfAction == null) { unitList.Add(null); return unitList; }
				TargetDataSet targetDataSet = modsa_selfAction._targetDataDetail.GetCurrentTargetSet();
				unitList.Add(targetDataSet.GetMainTarget());
				return unitList;
			}
			else if (param == "EveryTarget")
			{
				TargetDataSet targetDataSet = modsa_selfAction._targetDataDetail.GetCurrentTargetSet();
				unitList.Add(targetDataSet.GetMainTarget());
				foreach (SinActionModel sinActionModel in targetDataSet.GetSubTargetSinActionList())
				{
					BattleUnitModel model = sinActionModel.UnitModel;
					if (!unitList.Contains(model)) unitList.Add(sinActionModel.UnitModel);
				}
				return unitList;
			}
			else if (param == "SubTarget")
			{
				TargetDataSet targetDataSet = modsa_selfAction._targetDataDetail.GetCurrentTargetSet();
				foreach (SinActionModel sinActionModel in targetDataSet.GetSubTargetSinActionList())
				{
					BattleUnitModel model = sinActionModel.UnitModel;
					if (!unitList.Contains(model)) unitList.Add(sinActionModel.UnitModel);
				}
				return unitList;
			}

			SinManager sinManager_inst = Singleton<SinManager>.Instance;
			BattleObjectManager battleObjectManager = sinManager_inst._battleObjectManager;

			if (param.StartsWith("id"))
			{
				string id_string = param.Remove(0, 2);
				int id = GetNumFromParamString(id_string);

				foreach (BattleUnitModel unit in battleObjectManager.GetAliveList(false))
				{
					if (unit.GetUnitID() == id) unitList.Add(unit);
				}
				return unitList;
			}
			else if (param.StartsWith("inst"))
			{
				string id_string = param.Remove(0, 4);
				int id = GetNumFromParamString(id_string);

				foreach (BattleUnitModel unit in battleObjectManager.GetAliveList(false))
				{
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
					if (modelList.Count > 0) unitList.Add(modelList[0]);
                }
                else
                {
                    List<BattleUnitModel> modelList = battleObjectManager.GetNextUnitsByPortrait(modsa_unitModel, 1);
                    if (modelList.Count > 0) unitList.Add(modelList[0]);
                }
                return unitList;
            }

            UNIT_FACTION thisFaction = modsa_unitModel.Faction;
			UNIT_FACTION enemyFaction = thisFaction == UNIT_FACTION.PLAYER ? UNIT_FACTION.ENEMY : UNIT_FACTION.PLAYER;

			if (param == "EveryAlly") return battleObjectManager.GetAliveList(false, thisFaction);
			else if (param == "EveryAllyExceptSelf") return battleObjectManager.GetAliveAllyExceptSelf(modsa_unitModel);
			else if (param == "EveryEnemy") return battleObjectManager.GetAliveList(false, enemyFaction);
			else if (param == "EveryCoreAlly")
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
			else if (param.StartsWith("EveryAllyExceptSelf"))
			{
				string bufKeyword_string = param.Remove(0, 19);
				BUFF_UNIQUE_KEYWORD bufKeyword = BUFF_UNIQUE_KEYWORD.Enhancement;
				Enum.TryParse(bufKeyword_string, true, out bufKeyword);
				unitList = battleObjectManager.GetAliveList(bufKeyword, 0, false, thisFaction);
				unitList.Remove(modsa_unitModel);
				return unitList;
			}
			else if (param.StartsWith("EveryAlly"))
			{
				string bufKeyword_string = param.Remove(0, 9);
				BUFF_UNIQUE_KEYWORD bufKeyword = BUFF_UNIQUE_KEYWORD.Enhancement;
				Enum.TryParse(bufKeyword_string, true, out bufKeyword);
				unitList = battleObjectManager.GetAliveList(bufKeyword, 0, false, thisFaction);
				return unitList;
			}
			else if (param.StartsWith("EveryEnemy"))
			{
				string bufKeyword_string = param.Remove(0, 9);
				BUFF_UNIQUE_KEYWORD bufKeyword = BUFF_UNIQUE_KEYWORD.Enhancement;
				Enum.TryParse(bufKeyword_string, true, out bufKeyword);
				unitList = battleObjectManager.GetAliveList(bufKeyword, 0, false, enemyFaction);
				return unitList;
			}

			unitList.Add(modsa_unitModel);
			return unitList;
		}

		public BattleUnitModel GetTargetModel(string param)
		{
			if (param == "Target") return modsa_loopTarget;
			else if (param == "MainTarget")
			{
				if (modsa_selfAction == null) return null;
				TargetDataSet targetDataSet = modsa_selfAction._targetDataDetail.GetCurrentTargetSet();
				return targetDataSet.GetMainTarget();
			}

			if (param.StartsWith("id"))
			{
				SinManager sinManager_inst = Singleton<SinManager>.Instance;
				BattleObjectManager battleObjectManager = sinManager_inst._battleObjectManager;

				string id_string = param.Remove(0, 2);
				int id = GetNumFromParamString(id_string);

				foreach (BattleUnitModel unit in battleObjectManager.GetAliveList(false))
				{
					if (unit.GetUnitID() == id) return unit;
				}
				return null;
			}
			else if (param.StartsWith("inst"))
			{
				SinManager sinManager_inst = Singleton<SinManager>.Instance;
				BattleObjectManager battleObjectManager = sinManager_inst._battleObjectManager;

				string id_string = param.Remove(0, 4);
				int id = GetNumFromParamString(id_string);

				foreach (BattleUnitModel unit in battleObjectManager.GetAliveList(false))
				{
					if (unit.InstanceID == id) return unit;
				}
				return null;
			}
			else if (param.StartsWith("adj"))
			{
                BattleUnitModel foundUnit = null;
                BattleObjectManager battleObjectManager_inst = SingletonBehavior<BattleObjectManager>.Instance;
				if (battleObjectManager_inst == null) return foundUnit;
				
                string side_string = param.Remove(0, 3);
				if (side_string == "Left")
				{
					List<BattleUnitModel> modelList = battleObjectManager_inst.GetPrevUnitsByPortrait(modsa_unitModel, 1);
					if (modelList.Count > 0) foundUnit = modelList[0];
                }
				else
				{
                    List<BattleUnitModel> modelList = battleObjectManager_inst.GetNextUnitsByPortrait(modsa_unitModel, 1);
                    if (modelList.Count > 0) foundUnit = modelList[0];
                }
				return foundUnit;
            }

			return modsa_unitModel;
		}

		public void SetupModular(string instructions)
		{
			instructions = MainClass.sWhitespace.Replace(instructions, "");
			string[] batches = instructions.Split('/');

			for (int i = 0; i < batches.Length; i++)
			{
				string batch = batches[i];
				if (MainClass.logEnabled) MainClass.Logg.LogInfo("batch " + i.ToString() + ": " + batch);
				if (batch.StartsWith("TIMING:"))
				{
					string timingArg = batch.Remove(0, 7);
					if (timingArg == "RoundStart")
					{
						activationTiming = -1;
						if (abilityMode == 0) { SkillScriptInitPatch.skillPtrsRoundStart.Add(modsa_skillModel.Pointer.ToInt64()); }
					}
					else if (timingArg == "StartBattle") activationTiming = 0;
					else if (timingArg == "WhenUse") activationTiming = 1;
					else if (timingArg == "BeforeAttack") activationTiming = 2;
					else if (timingArg == "StartDuel") activationTiming = 3;
					else if (timingArg == "WinDuel") activationTiming = 4;
					else if (timingArg == "DefeatDuel") activationTiming = 5;
					else if (timingArg == "EndBattle") activationTiming = 6;
					else if (timingArg.StartsWith("OnSucceedAttack"))
					{
						activationTiming = 7;
						string[] sectionArgs = batch.Split(parenthesisSeparator);
						string circledSection = sectionArgs[1];
						string[] circles = circledSection.Split(',');

						if (circles[0] == "Head") _onlyHeads = true;
						else if (circles[0] == "Tail") _onlyTails = true;

						if (circles[1] == "Crit") _onlyCrit = true;
						else if (circles[1] == "NoCrit") _onlyNonCrit = true;

						if (circles[2] == "Win") _onlyClashWin = true;
						else if (circles[2] == "Lose") _onlyClashLose = true;
					}
					else if (timingArg == "WhenHit") activationTiming = 8;
					else if (timingArg == "EndSkill") activationTiming = 9;
					else if (timingArg == "FakePower") activationTiming = 10;
					else if (timingArg == "BeforeDefense") activationTiming = 11;
					else if (timingArg == "OnDie") activationTiming = 12;
					else if (timingArg == "OnOtherDie") activationTiming = 13;
					else if (timingArg == "DuelClash") activationTiming = 14;
					else if (timingArg == "DuelClashAfter") activationTiming = 15;
					else if (timingArg == "OnSucceedEvade") activationTiming = 16;
					else if (timingArg == "OnDefeatEvade") activationTiming = 17;
					else if (timingArg == "OnStartBehaviour") activationTiming = 18;
					else if (timingArg == "BeforeBehaviour") activationTiming = 19;
					else if (timingArg == "OnEndBehaviour") activationTiming = 20;
					else if (timingArg == "SpecialAction") activationTiming = 999;
				}
				else if (batch.StartsWith("LOOP:")) modsa_loopString = batch.Remove(0, 5);
				else batch_list.Add(batch);
			}
		}

		private void ProcessBatch(string batch)
		{
			string[] batchArgs = batch.Split(':');
			for (int i = 0; i < batchArgs.Length; i++)
			{
				if (MainClass.logEnabled) MainClass.Logg.LogInfo("batchArgs " + i.ToString() + ": " + batchArgs[i]);
				if (batchArgs[i].StartsWith("STOPIF"))
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
				else if (batchArgs[i].StartsWith("VALUE_"))
				{
					string numChar = batchArgs[i][6].ToString();
					int num = 0;
					int.TryParse(numChar, out num);
					AcquireValue(num, batchArgs[i + 1]);
					i += 1;
					continue;
				}

				if (batchArgs[i].StartsWith("log")) {
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');
					MainClass.Logg.LogInfo("ModularLog " + circles[0] + ": " + GetNumFromParamString(circles[1]));
					continue;
				}

				if (batchArgs[i].StartsWith("base")) { skillPowerAdder = GetNumFromParamString(batchArgs[i].Remove(0, 5)); continue; }
				else if (batchArgs[i].StartsWith("final")) { skillPowerResultAdder = GetNumFromParamString(batchArgs[i].Remove(0, 6)); continue; }
				else if (batchArgs[i].StartsWith("clash")) { parryingResultAdder = GetNumFromParamString(batchArgs[i].Remove(0, 6)); continue; }
				else if (batchArgs[i].StartsWith("scale"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');

					if (circles.Length == 1)
					{
						int power = 0;
						if (activationTiming != 10)
						{
							OPERATOR_TYPE coinOp = OPERATOR_TYPE.NONE;
							if (circledSection == "ADD") coinOp = OPERATOR_TYPE.ADD;
							else if (circledSection == "SUB") coinOp = OPERATOR_TYPE.SUB;
							else if (circledSection == "MUL") coinOp = OPERATOR_TYPE.MUL;
							else power = GetNumFromParamString(circledSection);
							if (coinOp != OPERATOR_TYPE.NONE) foreach (CoinModel coin in modsa_skillModel.CoinList) coin._operatorType = coinOp;
							else coinScaleAdder = power;
						}
						else
						{
							power = GetNumFromParamString(circledSection);
							coinScaleAdder = power;
						}
						continue;
					}
				}
				else if (batchArgs[i].StartsWith("dmgadd")) { atkDmgAdder = GetNumFromParamString(batchArgs[i].Remove(0, 7)); continue; }
				else if (batchArgs[i].StartsWith("dmgmult")) { atkMultAdder = GetNumFromParamString(batchArgs[i].Remove(0, 8)); continue; }

				if (activationTiming == 10) continue;
				if (batchArgs[i].StartsWith("mpdmg"))
				{
					if (MainClass.logEnabled) MainClass.Logg.LogInfo("mpdmg");
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');
					int mpAmount = GetNumFromParamString(circles[1]);
					if (MainClass.logEnabled) MainClass.Logg.LogInfo("mpAmount: " + mpAmount.ToString());

					if (mpAmount == 0) continue;

					List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
					if (modelList.Count < 1) continue;

					foreach (BattleUnitModel targetModel in modelList)
					{
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
								targetModel.GiveMpDamage(targetModel, mpAmount * -1, battleTiming, DAMAGE_SOURCE_TYPE.PASSIVE, mpAmount * -1, modsa_selfAction);
							}
							else if (abilityMode == 1)
							{
								dummyCoinAbility.AddTriggeredData_MpDamage(mpAmount * -1, targetModel.InstanceID, battleTiming);
								dummyCoinAbility.GiveMpDamage(modsa_unitModel, targetModel, mpAmount * -1, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, mpAmount * -1, modsa_selfAction);
							}
							else
							{
								dummySkillAbility.AddTriggeredData_MpDamage(mpAmount * -1, targetModel.InstanceID, battleTiming);
								targetModel.GiveMpDamage(targetModel, mpAmount * -1, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, mpAmount * -1, modsa_selfAction);
							}
						}
					}
					if (MainClass.logEnabled) MainClass.Logg.LogInfo("after mpdmg");
				}
				else if (batchArgs[i].StartsWith("scale"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');
					if (circles.Length != 2) continue;

					int coin_idx = -999;
					coin_idx = GetNumFromParamString(circles[1]);
					if (coin_idx == -999) continue;

					string firstCircle = circles[0];

					int power = 0;
					OPERATOR_TYPE coinOp = OPERATOR_TYPE.NONE;
					if (firstCircle == "ADD") coinOp = OPERATOR_TYPE.ADD;
					else if (firstCircle == "SUB") coinOp = OPERATOR_TYPE.SUB;
					else if (firstCircle == "MUL") coinOp = OPERATOR_TYPE.MUL;
					else power = GetNumFromParamString(firstCircle);

					coin_idx = Math.Min(modsa_skillModel.CoinList.Count - 1, coin_idx);
					modsa_skillModel.CoinList[coin_idx]._scale += power;
					if (coinOp != OPERATOR_TYPE.NONE) modsa_skillModel.CoinList[coin_idx]._operatorType = coinOp;
					
				}
				else if (batchArgs[i].StartsWith("reusecoin"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');
					foreach (string circle in circles)
					{
						int idx = GetNumFromParamString(circle);
						if (idx < 0) { modsa_skillModel.CopyCoin(modsa_selfAction, modsa_coinModel.GetOriginCoinIndex(), battleTiming); continue; }

						idx = Math.Min(idx, modsa_skillModel.CoinList.Count - 1);
						modsa_skillModel.CopyCoin(modsa_selfAction, idx, battleTiming);

					}
				}
				else if (batchArgs[i].StartsWith("bonusdmg"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');

					List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
					if (modelList.Count < 1) continue;

					int amount = GetNumFromParamString(circles[1]);

					int dmg_type = Math.Min(int.Parse(circles[2]), 2);
					int dmg_sin = Math.Min(int.Parse(circles[3]), 11);

					ATK_BEHAVIOUR atkBehv = ATK_BEHAVIOUR.NONE;
					ATTRIBUTE_TYPE sinKind = ATTRIBUTE_TYPE.NONE;
					if (dmg_type != -1) atkBehv = (ATK_BEHAVIOUR)dmg_type;
					if (dmg_sin != -1) sinKind = (ATTRIBUTE_TYPE)dmg_sin;

					foreach (BattleUnitModel targetModel in modelList)
					{
						if (dmg_type == -1 && dmg_sin == -1)
						{
							AbilityTriggeredData_HpDamage triggerData = new AbilityTriggeredData_HpDamage(amount, targetModel.InstanceID, battleTiming);
							if (abilityMode == 2)
							{
								dummyPassiveAbility.GiveAbsHpDamage(modsa_unitModel, targetModel, amount, amount, amount, battleTiming, DAMAGE_SOURCE_TYPE.PASSIVE);
								targetModel.AddTriggeredData(triggerData);
							}
							else if (abilityMode == 1)
							{
								dummyCoinAbility.GiveAbsHpDamage(modsa_unitModel, targetModel, amount, amount, amount, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, modsa_selfAction);
								targetModel.AddTriggeredData(triggerData);
							}
							else dummySkillAbility.GiveAbsHpDamage(modsa_unitModel, targetModel, amount, amount, amount, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, modsa_selfAction);
						}
						else if (dmg_type == -1 && dmg_sin != -1)
						{
							AbilityTriggeredData_HpDamage triggerData = new AbilityTriggeredData_HpDamage(amount, targetModel.InstanceID, sinKind, battleTiming);
							if (abilityMode == 2)
							{
								dummyPassiveAbility.GiveHpDamageAppliedAttributeResist(modsa_unitModel, targetModel, amount, sinKind, battleTiming, DAMAGE_SOURCE_TYPE.PASSIVE, amount);
								targetModel.AddTriggeredData(triggerData);
							}
							else if (abilityMode == 1)
							{
								dummyCoinAbility.GiveHpDamageAppliedAttributeResist(modsa_unitModel, targetModel, amount, sinKind, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, amount);
								targetModel.AddTriggeredData(triggerData);
							}
							else dummySkillAbility.GiveHpDamageAppliedAttributeResist(modsa_unitModel, targetModel, amount, sinKind, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, amount);
						}
						else if (dmg_type != -1 && dmg_sin == -1)
						{
							AbilityTriggeredData_HpDamage triggerData = new AbilityTriggeredData_HpDamage(amount, targetModel.InstanceID, atkBehv, battleTiming);
							if (abilityMode == 2)
							{
								dummyPassiveAbility.GiveHpDamageAppliedAtkResist(modsa_unitModel, targetModel, amount, atkBehv, battleTiming, DAMAGE_SOURCE_TYPE.PASSIVE, modsa_selfAction);
								targetModel.AddTriggeredData(triggerData);
							}
							else if (abilityMode == 1)
							{
								dummyCoinAbility.GiveHpDamageAppliedAtkResist(modsa_unitModel, targetModel, amount, atkBehv, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, modsa_selfAction);
								targetModel.AddTriggeredData(triggerData);
							}
							else dummySkillAbility.GiveHpDamageAppliedAtkResist(modsa_unitModel, targetModel, amount, atkBehv, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, modsa_selfAction);
						}
						else
						{
							AbilityTriggeredData_HpDamage triggerData = new AbilityTriggeredData_HpDamage(amount, targetModel.InstanceID, sinKind, atkBehv, battleTiming);
							if (abilityMode == 2)
							{
								dummyPassiveAbility.GiveHpDamageAppliedAttributeAndAtkResist(modsa_unitModel, targetModel, amount, sinKind, atkBehv, battleTiming, DAMAGE_SOURCE_TYPE.PASSIVE, modsa_selfAction);
								targetModel.AddTriggeredData(triggerData);
							}
							else if (abilityMode == 1)
							{
								dummyCoinAbility.GiveHpDamageAppliedAttributeAndAtkResist(modsa_unitModel, targetModel, amount, sinKind, atkBehv, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, modsa_selfAction);
								targetModel.AddTriggeredData(triggerData);
							}
							else dummySkillAbility.GiveHpDamageAppliedAttributeAndAtkResist(modsa_unitModel, targetModel, amount, sinKind, atkBehv, battleTiming, DAMAGE_SOURCE_TYPE.SKILL, modsa_selfAction);
						}
					}
				}
				else if (batchArgs[i].StartsWith("buf"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');

					List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
					if (modelList.Count < 1) continue;

					BUFF_UNIQUE_KEYWORD buf_keyword = BUFF_UNIQUE_KEYWORD.Enhancement;
					Enum.TryParse(circles[1], true, out buf_keyword);
					int stack = GetNumFromParamString(circles[2]);
					int turn = GetNumFromParamString(circles[3]);
					int activeRound = GetNumFromParamString(circles[4]);

					foreach (BattleUnitModel targetModel in modelList)
					{
						if (stack < 0)
						{
							if (abilityMode == 2) targetModel.UseBuffStack(buf_keyword, stack * -1, battleTiming);
							else if (abilityMode == 1) dummyCoinAbility.UseBuffStack(targetModel, modsa_selfAction, buf_keyword, battleTiming, stack * -1);
							else dummySkillAbility.UseBuffStack(targetModel, modsa_selfAction, buf_keyword, battleTiming, stack * -1);
							stack = 0;
						}
						if (turn < 0)
						{
							if (abilityMode == 2) targetModel.UseBuffTurn(buf_keyword, turn * -1, battleTiming);
							else if (abilityMode == 1) dummyCoinAbility.UseBuffTurn(targetModel, modsa_selfAction, buf_keyword, battleTiming, turn * -1);
							else dummySkillAbility.UseBuffTurn(targetModel, modsa_selfAction, buf_keyword, battleTiming, turn * -1);
							turn = 0;
						}
						if (stack > 0 || turn > 0)
						{
							AbilityTriggeredData_GiveBuff triggerData = new AbilityTriggeredData_GiveBuff(buf_keyword, stack, turn, activeRound, false, true, targetModel.InstanceID, battleTiming, BUF_TYPE.Neutral);
							if (abilityMode == 2)
							{
								dummyPassiveAbility.GiveBuff_Self(targetModel, buf_keyword, stack, turn, activeRound, battleTiming, modsa_selfAction);
								targetModel.AddTriggeredData(triggerData);
							}
							else if (abilityMode == 1)
							{
								dummyCoinAbility.GiveBuff_Self(targetModel, buf_keyword, stack, turn, activeRound, battleTiming, modsa_selfAction);
								targetModel.AddTriggeredData(triggerData);
							}
							else dummySkillAbility.GiveBuff_Self(targetModel, buf_keyword, stack, turn, activeRound, battleTiming, modsa_selfAction);
						}
					}
				}
				else if (batchArgs[i].StartsWith("shield"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');

					List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
					if (modelList.Count < 1) continue;

					int amount = GetNumFromParamString(circles[1]);
					bool permashield = circles.Length > 2;

					foreach (BattleUnitModel targetModel in modelList)
					{
						targetModel.AddShield(amount, !permashield, ABILITY_SOURCE_TYPE.SKILL, battleTiming);
					}
				}
				else if (batchArgs[i].StartsWith("break"))
				{
                    string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
                    string circledSection = sectionArgs[1];
                    string[] circles = circledSection.Split(',');

                    List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
                    if (modelList.Count < 1) continue;

                    string mode_string = batchArgs[i].Remove(0, 5);

					if (mode_string == "")
					{
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
					else if (mode_string == "dmg")
					{
                        int amount = GetNumFromParamString(circles[1]);
                        if (amount == 0) continue;
                        int times = 1;
                        if (circles.Length > 2) times = GetNumFromParamString(circles[2]);

                        foreach (BattleUnitModel targetModel in modelList)
                        {
                            for (int times_i = 0; times_i < times; times_i++)
                            {
                                if (amount < 0)
                                {
                                    amount *= -1;
                                    AbilityTriggeredData_BsGaugeDown triggerData = new AbilityTriggeredData_BsGaugeDown(amount, targetModel.InstanceID, battleTiming);
                                    if (abilityMode == 2)
                                    {
                                        dummyPassiveAbility.FirstBsGaugeDown(modsa_unitModel, targetModel, amount, battleTiming);
                                        targetModel.AddTriggeredData(triggerData);
                                    }
                                    else if (abilityMode == 1)
                                    {
                                        dummyCoinAbility.FirstBsGaugeDown(modsa_unitModel, targetModel, amount, battleTiming);
                                        targetModel.AddTriggeredData(triggerData);
                                    }
                                    else
                                    {
                                        dummySkillAbility.AddTriggeredData_BsGaugeDown(amount, targetModel.InstanceID, battleTiming);
                                        dummySkillAbility.FirstBsGaugeDown(modsa_unitModel, targetModel, amount, battleTiming);
                                    }
                                }
                                else
                                {
                                    AbilityTriggeredData_BsGaugeUp triggerData = new AbilityTriggeredData_BsGaugeUp(amount, targetModel.InstanceID, battleTiming);
                                    if (abilityMode == 2)
                                    {
                                        dummyPassiveAbility.FirstBsGaugeUp(modsa_unitModel, targetModel, amount, battleTiming, false);
                                        targetModel.AddTriggeredData(triggerData);
                                    }
                                    else if (abilityMode == 1)
                                    {
                                        dummyCoinAbility.FirstBsGaugeUp(modsa_unitModel, targetModel, amount, battleTiming, false, modsa_selfAction);
                                        targetModel.AddTriggeredData(triggerData);
                                    }
                                    else
                                    {
                                        dummySkillAbility.AddTriggeredData_BsGaugeUp(amount, targetModel.InstanceID, battleTiming, false);
                                        dummySkillAbility.FirstBsGaugeUp(modsa_unitModel, targetModel, amount, battleTiming, false, modsa_selfAction);
                                    }
                                }
                            }
                        }
                    }
					else if (mode_string == "recover")
					{
						bool force = circles.Length > 1;
						foreach (BattleUnitModel targetModel in modelList)
						{
							if (force) targetModel.RecoverAllBreak(battleTiming);
							else targetModel.RecoverBreak(battleTiming);
						}
					}
					else if (mode_string == "addbar")
					{
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
				}
				else if (batchArgs[i].StartsWith("explosion"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');

					List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
					if (modelList.Count < 1) continue;

					int times = GetNumFromParamString(circles[1]);

					foreach (BattleUnitModel targetModel in modelList)
					{
						int tremorStack = targetModel._buffDetail.GetActivatedBuffStack(BUFF_UNIQUE_KEYWORD.Vibration, false);
						for (int times_i = 0; times_i < times; times_i++)
						{
							if (abilityMode == 2)
							{
								dummyPassiveAbility.AddTriggeredData_BsGaugeUp(tremorStack, targetModel.InstanceID, battleTiming, true);
								dummyPassiveAbility.FirstBsGaugeUp(modsa_unitModel, targetModel, tremorStack, battleTiming, true);
								//targetModel.VibrationExplosion(battleTiming, modsa_unitModel, dummyPassiveAbility);
							}
							else
							{
								//dummySkillAbility.AddTriggeredData_BsGaugeUp(tremorStack, targetModel.InstanceID, battleTiming, true);
								dummySkillAbility.FirstBsGaugeUp(modsa_unitModel, targetModel, tremorStack, battleTiming, true, modsa_selfAction);
								//targetModel.VibrationExplosion(battleTiming, modsa_unitModel, dummySkillAbility);
							}
						}
					}
				}
				else if (batchArgs[i].StartsWith("healhp"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');

					List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
					if (modelList.Count < 1) continue;

					int amount = GetNumFromParamString(circles[1]);
					if (amount < 1) continue;

					foreach (BattleUnitModel targetModel in modelList)
					{
						if (abilityMode == 2)
						{
							dummyPassiveAbility.AddTriggeredData_HpHeal(amount, targetModel.InstanceID, battleTiming);
							//dummyPassiveAbility.HealTargetHp(modsa_unitModel, modsa_selfAction, targetModel, amount, battleTiming, amount);
							targetModel.TryRecoverHp(modsa_unitModel, null, amount, ABILITY_SOURCE_TYPE.PASSIVE, battleTiming, amount);
						}
						else if (abilityMode == 1)
						{
							dummyCoinAbility.AddTriggeredData_HpHeal(amount, targetModel.InstanceID, battleTiming);
							//dummyCoinAbility.HealTargetHp(modsa_unitModel, modsa_selfAction, targetModel, amount, battleTiming, amount);
							targetModel.TryRecoverHp(modsa_unitModel, null, amount, ABILITY_SOURCE_TYPE.SKILL, battleTiming, amount);
						}
						else
						{
							dummySkillAbility.AddTriggeredData_HpHeal(amount, targetModel.InstanceID, battleTiming);
							//dummySkillAbility.HealTargetHp(modsa_unitModel, modsa_selfAction, targetModel, amount, battleTiming, amount);
							targetModel.TryRecoverHp(modsa_unitModel, null, amount, ABILITY_SOURCE_TYPE.SKILL, battleTiming, amount);
						}
					}

				}
				else if (batchArgs[i].StartsWith("pattern"))
				{
					if (MainClass.logEnabled) MainClass.Logg.LogInfo("pattern() -> " + batchArgs[i]);
					BattleUnitModel_Abnormality abnoModel = null;
					if (modsa_unitModel is BattleUnitModel_Abnormality) abnoModel = (BattleUnitModel_Abnormality)modsa_unitModel;
					else if (modsa_unitModel is BattleUnitModel_Abnormality_Part)
					{
						BattleUnitModel_Abnormality_Part partModel = (BattleUnitModel_Abnormality_Part)modsa_unitModel;
						abnoModel = partModel.Abnormality;
					}
					if (abnoModel == null) continue;
					if (MainClass.logEnabled) MainClass.Logg.LogInfo("abnoModel not null");
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');

					PatternScript_Abnormality pattern = abnoModel.PatternScript;

					//List<BattlePattern> battlePattern_list = pattern._patternList;

					int pickedPattern_idx = GetNumFromParamString(circles[0]);
					if (MainClass.logEnabled) MainClass.Logg.LogInfo("pickedPattern_idx: " + pickedPattern_idx);
					pattern.currPatternIdx = pickedPattern_idx;
					//int slotCount = -1;
					//bool randomize = false;

					//pattern.PickSkillsByPattern(pickedPattern_idx, slotCount, randomize);
				}
				else if (batchArgs[i].StartsWith("setslotadder"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');

					int amount = GetNumFromParamString(circles[1]);
					if (amount < 0) continue;
					bool add_max_instead = circles.Length > 2;
					if (!add_max_instead) { slotAdder = amount; continue; }

					List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
					if (modelList.Count < 1) continue;

					foreach (BattleUnitModel targetModel in modelList)
					{
						BattleUnitModel_Abnormality abnoModel = null;
						if (targetModel is BattleUnitModel_Abnormality) abnoModel = (BattleUnitModel_Abnormality)targetModel;
						else if (targetModel is BattleUnitModel_Abnormality_Part)
						{
							BattleUnitModel_Abnormality_Part partModel = (BattleUnitModel_Abnormality_Part)targetModel;
							abnoModel = partModel.Abnormality;
						}
						if (abnoModel == null) continue;
						PatternScript_Abnormality pattern = abnoModel.PatternScript;
						pattern._slotMax = amount;
					}
				}
				else if (batchArgs[i].StartsWith("setdata"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');

					List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
					if (modelList.Count < 1) continue;

					int dataID = GetNumFromParamString(circles[1]);
					int dataValue = GetNumFromParamString(circles[2]);

					foreach (BattleUnitModel targetModel in modelList)
					{
						long targetPtr_intlong = targetModel.Pointer.ToInt64();
						bool found = false;
						foreach (ModUnitData unitMod in SkillScriptInitPatch.unitMod_list)
						{
							if (unitMod.unitPtr_intlong != targetPtr_intlong) continue;
							
							foreach (DataMod dataMod in unitMod.data_list) {
								if (dataMod.dataID != dataID) continue;
								found = true;
								dataMod.dataValue = dataValue;
								break;
							}
							if (!found) {
								var dataMod = new DataMod();
								dataMod.dataID = dataID;
								dataMod.dataValue = dataValue;
								unitMod.data_list.Add(dataMod);
							}

							found = true;
							break;
						}

						if (!found)
						{
							var unitMod = new ModUnitData();
							unitMod.unitPtr_intlong = targetPtr_intlong;
							SkillScriptInitPatch.unitMod_list.Add(unitMod);

							var dataMod = new DataMod();
							dataMod.dataID = dataID;
							dataMod.dataValue = dataValue;
							unitMod.data_list.Add(dataMod);
						}

					}
				}
				else if (batchArgs[i].StartsWith("changeskill"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					if (modsa_selfAction != null)
					{
						modsa_selfAction.TryChangeSkill(GetNumFromParamString(circledSection));
					}
					else
					{
						
					}
				}
				else if (batchArgs[i].StartsWith("setimmortal"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					int amount = GetNumFromParamString(circledSection);
					immortality = amount > 0;
				}
				else if (batchArgs[i].StartsWith("motion"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					string[] circles = circledSection.Split(',');

					int motionIdx = motionIdx = GetNumFromParamString(circles[1]);

					if (abilityMode == 0) {
						PatchesForLethe.InjectFunnyChange(0, circles[0], modsa_skillModel.Pointer.ToInt64(), 0, motionIdx);
					}
					else if (abilityMode == 1) {
						PatchesForLethe.InjectFunnyChange(0, circles[0], modsa_coinModel.Pointer.ToInt64(), modsa_skillModel.Pointer.ToInt64(), motionIdx);
					}
				}
				else if (batchArgs[i].StartsWith("appearance"))
				{
					string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
					string circledSection = sectionArgs[1];
					if (abilityMode == 0) {
						PatchesForLethe.InjectFunnyChange(1, circledSection, modsa_skillModel.Pointer.ToInt64(), 0);
					}
					else if (abilityMode == 1) {
						PatchesForLethe.InjectFunnyChange(1, circledSection, modsa_coinModel.Pointer.ToInt64(), modsa_skillModel.Pointer.ToInt64());
					}
				}
				else if (batchArgs[i].StartsWith("retreat"))
				{
					BattleObjectManager battleObjectManager_inst = SingletonBehavior<BattleObjectManager>.Instance;
					if (battleObjectManager_inst == null) continue;

                    string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
                    string circledSection = sectionArgs[1];
                    string[] circles = circledSection.Split(',');

                    List<BattleUnitModel> modelList = GetTargetModelList(circles[0]);
                    //if (modelList.Count < 1) continue;
					//bool comeback = circles.Length > 1;

                    foreach (BattleUnitModel targetModel in modelList)
					{
						if (battleObjectManager_inst.TryReservateForRetreat(targetModel, modsa_unitModel, BUFF_UNIQUE_KEYWORD.Retreat))
						{
							targetModel.Retreat(modsa_unitModel, battleTiming);
                        }

                    }
                }
				else if (batchArgs[i].StartsWith("aggro"))
                {
                    string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
                    string circledSection = sectionArgs[1];
                    string[] circles = circledSection.Split(',');

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
							if (slot == -2 && modsa_selfAction != null)
							{
								if (nextRound) modsa_selfAction.SinAction.StackNextTurnAggroAdder(amount);
								else modsa_selfAction.SinAction.StackThisTurnAggroAdder(amount);
                            }
                            else if (slot == -1)
                            {
                                int quotient = amount / sinActionCount;
                                int remainder = amount % sinActionCount;

                                foreach (SinActionModel sinAction in sinActionList)
                                {
                                    int finalAmount = amount;
                                    if (remainder > 0)
                                    {
                                        finalAmount += 1;
                                        remainder -= 1;
                                    }
                                    if (nextRound) sinAction.StackNextTurnAggroAdder(finalAmount);
                                    else sinAction.StackThisTurnAggroAdder(finalAmount);
                                }
								continue;
                            }
							
							int chosenSlot = Math.Min(slot, sinActionCount - 1);
                            if (nextRound) sinActionList[chosenSlot].StackNextTurnAggroAdder(amount);
                            else sinActionList[chosenSlot].StackThisTurnAggroAdder(amount);
                        }
						else
						{
                            int chosenSlot = Math.Min(slot, sinActionCount - 1);
							if (chosenSlot == -2) chosenSlot = 0;
							
							if (chosenSlot == -1)
							{
								int quotient = amount / sinActionCount;
								int remainder = amount % sinActionCount;

								foreach (SinActionModel sinAction in sinActionList)
								{
									int finalAmount = quotient;
									if (remainder > 0)
									{
										finalAmount += 1;
										remainder -= 1;
                                    }
									if (finalAmount < 1) break;
									if (nextRound) sinAction.StackNextTurnAggroAdder(finalAmount);
									else sinAction.StackThisTurnAggroAdder(finalAmount);
                                }
								continue;
							}
                            if (nextRound) sinActionList[chosenSlot].StackNextTurnAggroAdder(amount);
                            else sinActionList[chosenSlot].StackThisTurnAggroAdder(amount);
                        }
                        
                    }
                }
                else if (batchArgs[i].StartsWith("skill"))
                {
                    string[] sectionArgs = batchArgs[i].Split(parenthesisSeparator);
                    string circledSection = sectionArgs[1];
                    string[] circles = circledSection.Split(',');

                    string mode_string = batchArgs[i].Remove(0, 5);
                    if (mode_string == "send")
                    {
                        BattleUnitModel fromUnit = GetTargetModel(circles[0]);
                        BattleUnitModel toUnit = GetTargetModel(circles[1]);
                        if (fromUnit == null || fromUnit.IsDead()) continue;
                        if (toUnit == null) continue;

						int skillID = -1;
						string circle_2 = circles[2];
						if (circle_2[0] == 'S')
						{
							int tier = 0;
                            int.TryParse(circle_2[1].ToString(), out tier);
							if (tier > 0)
							{
								List<int> skillIDList = fromUnit.GetSkillIdByTier(tier);
								if (skillIDList.Count > 0) skillID = skillIDList[0];
                            }
                        }
						else int.TryParse(circle_2, out skillID);
                        if (skillID < 0) continue;

                        List<SinActionModel> sinActionList = toUnit.GetSinActionList();
                        int sinActionCount = sinActionList.Count;
                        if (sinActionCount < 1) continue;

						SinActionModel targetSinAction = sinActionList[0];
						SinActionModel fromSinAction_new = fromUnit.AddNewSinActionModel();

						UnitSinModel fromSinModel_new = new UnitSinModel(skillID, fromUnit, fromSinAction_new);
						BattleActionModel fromAction_new = new BattleActionModel(fromSinModel_new, fromUnit, fromSinAction_new);
                        List<SinActionModel> targetSinActionList = new List<SinActionModel>();
                        targetSinActionList.Add(targetSinAction);
                        fromAction_new.SetOriginTargetSinActions(targetSinActionList);
                        fromAction_new._targetDataDetail.ReadyOriginTargeting(fromAction_new);

                        fromUnit.CutInAction(fromAction_new);
                        
                    }
					else if (mode_string == "reuse")
					{
                        List<BattleUnitModel> modelList = GetTargetModelList(circledSection);
                        if (modelList.Count < 1) continue;

                        foreach (BattleUnitModel targetModel in modelList)
                        {
                            targetModel.ReuseAction(modsa_selfAction);
                        }
                    }
                }
            }
		}

		private void AcquireValue(int setvalue_idx, string section)
		{
			if (MainClass.logEnabled) MainClass.Logg.LogInfo("AcquireValue " + section);
			string[] sectionArgs = section.Split(parenthesisSeparator);

			if (char.IsNumber(section.Last()))
			{
				valueList[setvalue_idx] = GetNumFromParamString(sectionArgs[0]);
				return;
			}

			string methodology = sectionArgs[0];
			string circledSection = "";
			if (sectionArgs.Length > 1) circledSection = sectionArgs[1];
			if (MainClass.logEnabled) MainClass.Logg.LogInfo("AcquireValuecircledSection: " + circledSection);
			if (methodology == "math")
			{
				MatchCollection symbols = Regex.Matches(circledSection, "(-|\\+|\\*|%|!|¡|\\?)", RegexOptions.IgnoreCase);
				char[] mathSeparator = new char[] { '-', '+', '*', '%', '!', '¡', '?' };
				string[] parameters = circledSection.Split(mathSeparator);
				string firstParam = parameters[0];
				int finalValue = GetNumFromParamString(firstParam);

				for (int i = 0; i < symbols.Count; i++)
				{
					string param = parameters[i + 1];
					string symbol_string = symbols[i].Value;
					char symbol = symbol_string[0];
					int amount = GetNumFromParamString(param);
					if (MainClass.logEnabled) MainClass.Logg.LogInfo("mathparam " + param + " | mathsymbol " + symbol);

					switch (symbol)
					{
						case '+':
							finalValue += amount;
							break;
						case '-':
							finalValue -= amount;
							break;
						case '*':
							finalValue *= amount;
							break;
						case '%':
							finalValue /= amount;
							break;
						case '!':
							finalValue = Math.Min(finalValue, amount);
							break;
						case '¡':
							finalValue = Math.Max(finalValue, amount);
							break;
						case '?':
							finalValue %= amount;
                            break;
					}
					if (MainClass.logEnabled) MainClass.Logg.LogInfo("mathfinal " + finalValue);
				}

				valueList[setvalue_idx] = finalValue;
			}
			else if (methodology == "mpcheck")
			{
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel == null) return;
				valueList[setvalue_idx] = targetModel.Mp;
			}
			else if (methodology == "hpcheck")
			{
				string[] circles = circledSection.Split(',');
				BattleUnitModel targetModel = GetTargetModel(circles[0]);
				if (targetModel == null) return;

				int hp = targetModel.Hp;
				int hp_max = targetModel.MaxHp;
				float hp_ptg = (float)hp / hp_max;
				int hp_ptg_floor = (int)Math.Floor(hp_ptg * 100.0);

				int finalValue = hp;
				if (circles[1] == "%") finalValue = hp_ptg_floor;
				else if (circles[1] == "max") finalValue = hp_max;

				valueList[setvalue_idx] = finalValue;
			}
			else if (methodology == "bufcheck")
			{
				string[] circles = circledSection.Split(',');

				BattleUnitModel targetModel = GetTargetModel(circles[0]);
				if (targetModel == null) return;

				BUFF_UNIQUE_KEYWORD buf_keyword = BUFF_UNIQUE_KEYWORD.Enhancement;
				Enum.TryParse(circles[1], true, out buf_keyword);

				BuffDetail bufDetail = targetModel._buffDetail;
				//BuffModel buf = bufDetail.FindActivatedBuff(buf_keyword, false);
				int stack = bufDetail.GetActivatedBuffStack(buf_keyword, false);
				int turn = bufDetail.GetActivatedBuffTurn(buf_keyword, false);

				int finalValue = stack;
				if (circles[2] == "turn") finalValue = turn;
				else if (circles[2] == "+") finalValue = stack + turn;
				else if (circles[2] == "*") finalValue = stack * turn;
				valueList[setvalue_idx] = finalValue;
			}
			else if (methodology == "getdmg") valueList[setvalue_idx] = lastFinalDmg;
			else if (methodology == "round")
			{
				StageController stageController_inst = Singleton<StageController>.Instance;
				valueList[setvalue_idx] = stageController_inst.GetCurrentRound();
			}
			else if (methodology == "wave")
			{
				StageController stageController_inst = Singleton<StageController>.Instance;
				valueList[setvalue_idx] = stageController_inst.GetCurrentWave();
			}
			else if (methodology == "activations") valueList[setvalue_idx] = activationCounter;
			else if (methodology == "unitstate")
			{
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel == null)
				{
					if (MainClass.logEnabled) MainClass.Logg.LogInfo("unitstate() unit not found");
					valueList[setvalue_idx] = -1;
					return;
				}
				if (targetModel.IsDead())
				{
					valueList[setvalue_idx] = 0;
					return;
				}
				valueList[setvalue_idx] = 1;
				if (targetModel.IsBreak()) valueList[setvalue_idx] = 2;

				if (targetModel is BattleUnitModel_Abnormality_Part)
				{
					BattleUnitModel_Abnormality_Part partModel = (BattleUnitModel_Abnormality_Part)targetModel;
					if (!partModel.IsActionable()) valueList[setvalue_idx] = 2;
				}
			}
			else if (methodology == "getid")
			{
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel != null) valueList[setvalue_idx] = targetModel.GetUnitID();
			}
			else if (methodology == "instid")
			{
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel != null) valueList[setvalue_idx] = targetModel.InstanceID;
			}
			else if (methodology == "speedcheck")
			{
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel == null) valueList[setvalue_idx] = 0;
				else valueList[setvalue_idx] = targetModel.GetIntegerOfOriginSpeed();
			}
			else if (methodology == "getpattern")
			{
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel == null) { valueList[setvalue_idx] = 0; return; }

				BattleUnitModel_Abnormality abnoModel = null;
				if (targetModel is BattleUnitModel_Abnormality) abnoModel = (BattleUnitModel_Abnormality)targetModel;
				else if (targetModel is BattleUnitModel_Abnormality_Part)
				{
					BattleUnitModel_Abnormality_Part partModel = (BattleUnitModel_Abnormality_Part)targetModel;
					abnoModel = partModel.Abnormality;
				}
				if (abnoModel == null) { valueList[setvalue_idx] = 0; return; }
				if (MainClass.logEnabled) MainClass.Logg.LogInfo("getpattern: abnoModel exists");

				PatternScript_Abnormality pattern = abnoModel.PatternScript;

				int pattern_idx = pattern.currPatternIdx;
				if (MainClass.logEnabled) MainClass.Logg.LogInfo("get pattern_idx: " + pattern_idx);
				valueList[setvalue_idx] = pattern.currPatternIdx;
			}
			else if (methodology == "getabnoslotmax")
			{
				valueList[setvalue_idx] = 0;
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel == null) { return; }

				BattleUnitModel_Abnormality abnoModel = null;
				if (targetModel is BattleUnitModel_Abnormality) abnoModel = (BattleUnitModel_Abnormality)targetModel;
				else if (targetModel is BattleUnitModel_Abnormality_Part)
				{
					BattleUnitModel_Abnormality_Part partModel = (BattleUnitModel_Abnormality_Part)targetModel;
					abnoModel = partModel.Abnormality;
				}
				if (abnoModel == null) { return; }

				PatternScript_Abnormality pattern = abnoModel.PatternScript;
				valueList[setvalue_idx] = pattern.SlotMax;
			}
			else if (methodology == "getdata")
			{
				string[] circles = circledSection.Split(',');

				BattleUnitModel targetModel = GetTargetModel(circles[0]);
				if (targetModel == null) { valueList[setvalue_idx] = 0; return; }

				int finalValue = 0;
				int dataID = GetNumFromParamString(circles[1]);

				long targetPtr_intlong = targetModel.Pointer.ToInt64();
				foreach (ModUnitData unitMod in SkillScriptInitPatch.unitMod_list)
				{
					if (unitMod.unitPtr_intlong != targetPtr_intlong) continue;
					foreach (DataMod dataMod in unitMod.data_list) {
						if (dataMod.dataID != dataID) continue;
						finalValue = dataMod.dataValue;
						break;
					}
					break;
				}
				valueList[setvalue_idx] = finalValue;
			}
			else if (methodology == "deadallies")
			{
				BattleUnitModel targetModel = GetTargetModel(circledSection);
				if (targetModel == null) { valueList[setvalue_idx] = 0; return; }
				valueList[setvalue_idx] = targetModel.deadAllyCount;
			}
			else if (methodology == "trieddeath") valueList[setvalue_idx] = immortality_attempted ? 1 : 0;
			else if (methodology == "random")
			{
				string[] circles = circledSection.Split(',');
				int minroll = GetNumFromParamString(circles[0]);
				int maxroll = GetNumFromParamString(circles[1]);
				valueList[setvalue_idx] = MainClass.rng.Next(minroll, maxroll + 1);
			}
			else if (methodology == "areallied")
			{
				string[] circles = circledSection.Split(',');
				BattleUnitModel targetModel1 = GetTargetModel(circles[0]);
				BattleUnitModel targetModel2 = GetTargetModel(circles[1]);
				if (targetModel1 == null || targetModel2 == null) { valueList[setvalue_idx] = -1; return; }

				valueList[setvalue_idx] = targetModel1.Faction == targetModel2.Faction ? 1 : 0;
			}
			else if (methodology == "getshield")
			{
				string[] circles = circledSection.Split(',');
				BattleUnitModel targetModel = GetTargetModel(circles[0]);
				if (targetModel == null) { valueList[setvalue_idx] = 0; return; }
				valueList[setvalue_idx] = targetModel.GetShield();
			}
			else if (methodology == "getskillid")
			{
				valueList[setvalue_idx] = 0;
				if (modsa_skillModel != null) valueList[setvalue_idx] = modsa_skillModel.GetID();
				else if (modsa_selfAction != null) valueList[setvalue_idx] = modsa_selfAction.Skill.GetID();
			}
			else if (methodology == "getcoincount")
			{
				string[] circles = circledSection.Split(',');
				BattleActionModel targetAction = modsa_selfAction;
				if (circles[0] == "Target") targetAction = modsa_oppoAction;
				if (targetAction == null)
				{
					valueList[setvalue_idx] = -1;
					return;
				}
				
				int coinCount = targetAction.Skill.GetAliveCoins().Count;
				if (circles[1] == "og") coinCount = targetAction.Skill.CoinList.Count;

				valueList[setvalue_idx] = coinCount;
			}
			else if (methodology == "allcoinstate")
			{
				string[] circles = circledSection.Split(',');
				BattleActionModel targetAction = modsa_selfAction;
				if (circles[0] == "Target") targetAction = modsa_oppoAction;
				if (targetAction == null)
				{
					valueList[setvalue_idx] = -1;
					return;
				}

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

				valueList[setvalue_idx] = result;
			}
			else if (methodology == "resonance")
			{
				valueList[setvalue_idx] = 0;
				SinManager sinmanager_inst = Singleton<SinManager>.Instance;
				SinManager.ResonanceManager res_manager = sinmanager_inst._resManager;

				ATTRIBUTE_TYPE sin = ATTRIBUTE_TYPE.NONE;
				
				if (circledSection == "highres")
				{
					List<ATTRIBUTE_TYPE> sinList = new List<ATTRIBUTE_TYPE>();
					for (int i = 0; i<7; i++)
					{
						sinList.Add((ATTRIBUTE_TYPE)i);
					}
					valueList[setvalue_idx] = res_manager.GetMaxAttributeResonanceOfAll(modsa_unitModel.Faction, sinList);
				}
				else if (circledSection == "highperfect")
				{
					List<ATTRIBUTE_TYPE> sinList = new List<ATTRIBUTE_TYPE>();
					int highest = 0;
					for (int i = 0; i < 7; i++)
					{
						int current = res_manager.GetMaxPerfectResonance(modsa_unitModel.Faction, (ATTRIBUTE_TYPE)i);
						if (current > highest) highest = current;
					}
					valueList[setvalue_idx] = highest;
				}
				else if (circledSection.StartsWith("perfect"))
				{
					Enum.TryParse(circledSection.Remove(0,7), true, out sin);
					valueList[setvalue_idx] = res_manager.GetAttributeResonance(modsa_unitModel.Faction, sin);
				}
				else if (Enum.TryParse(circledSection, true, out sin))
				{
					valueList[setvalue_idx] = res_manager.GetAttributeResonance(modsa_unitModel.Faction, sin);
				}
			}
			else if (methodology == "haskey")
			{
				string[] circles = circledSection.Split(',');
				BattleUnitModel targetModel = GetTargetModel(circles[0]);
				if (targetModel == null) {
					valueList[setvalue_idx] = -1; // Target not found = -1
					return;
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
				valueList[setvalue_idx] = success ? 1 : 0;
			}
		}
	}


	//SinManager sinmanager_inst = Singleton<SinManager>.Instance;
	//SinManager.ResonanceManager res_manager = sinmanager_inst._resManager;
	//int gloom_res = res_manager.GetAttributeResonance(faction, ATTRIBUTE_TYPE.AZURE);
	//return gloom_res >= 3;


	//SinManager sinmanager_inst = Singleton<SinManager>.Instance;
	//SinManager.ResonanceManager res_manager = sinmanager_inst._resManager;
	//int gloom_perfectres = res_manager.GetMaxPerfectResonance(faction, ATTRIBUTE_TYPE.AZURE);
	//return gloom_perfectres >= 5;


}
