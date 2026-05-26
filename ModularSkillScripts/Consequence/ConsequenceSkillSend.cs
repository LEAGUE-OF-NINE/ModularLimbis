using System;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceSkillSend : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel fromUnit = modular.GetTargetModel(circles[0]);
		if (fromUnit == null || fromUnit.IsDead() || !fromUnit.IsActionable()) return;

		int skillID = -1;
		string circle_2 = circles[2];
		switch (circle_2[0])
		{
			case 'S':
			{
				int tier = 0;
				int.TryParse(circle_2[1].ToString(), out tier);
				if (tier > 0)
				{
					var skillIDList = fromUnit.GetSkillIdByTier(tier);
					if (skillIDList.Count > 0) skillID = skillIDList[0];
				}

				break;
			}
			case 'D':
			{
				int index = 0;
				if (int.TryParse(circle_2[1].ToString(), out index)) index -= 1;
				var skillIDList = fromUnit.GetDefenseSkillIDList();
				index = Math.Min(index, skillIDList.Count - 1);
				skillID = skillIDList[index];
				break;
			}
			default:
				skillID = modular.GetNumFromParamString(circle_2);
				break;
		}

		if (skillID < 0) return;
	
		SinActionModel fromSinAction_new = fromUnit.AddNewSinActionModel();
		UnitSinModel fromSinModel_new = new UnitSinModel(skillID, fromUnit, fromSinAction_new);
		BattleActionModel fromAction_new = new BattleActionModel(fromSinModel_new, fromUnit, fromSinAction_new);
		//fromAction_new._targetDataDetail.ClearAllTargetData(fromAction_new);

		List<BattleUnitModel> targetList = new();
		string[] target_stringList = circles[1].Split('+', StringSplitOptions.RemoveEmptyEntries);
		foreach (string target_string in target_stringList)
		{
			List<BattleUnitModel> targetList_temp = modular.GetTargetModelList(target_string);
			foreach (BattleUnitModel unit in targetList_temp)
			{
				if (unit.IsDead()) continue;
				if (targetList.Contains(unit)) continue;
				targetList.Add(unit);
			}
		}
		
		List<SinActionModel> subtargetSinActionList = new();
		SinActionModel maintarget_sinAction = null;
		foreach (BattleUnitModel targetModel in targetList)
		{
			List<SinActionModel> sinActionList = targetModel.GetSinActionList();
			if (sinActionList.Count < 1) continue;
			SinActionModel sinAction = sinActionList[0];
			if (maintarget_sinAction == null) maintarget_sinAction = sinAction;
			else subtargetSinActionList.Add(sinAction);
			//fromAction_new._targetDataDetail.AddTargetSinAction(sinActionList[0]);
		}

		if (maintarget_sinAction == null) return;
		
		//fromAction_new._targetDataDetail.SetOriginTargetSinAction(fromAction_new, targetSinActionList);
		//fromAction_new.SetOriginTargetSinActions(targetSinActionList);

		/*TargetDataSet targetDataSet = fromAction_new._targetDataDetail.GetCurrentTargetSet();
		if (!targetSinActionList.Contains(targetDataSet._mainTarget.GetTargetSinAction())) {
			if (targetSinActionList.Count > 0) targetDataSet.SetMainTargetSinAction(targetSinActionList[0]);
		}
		List<TargetSinActionData> goodones = new List<TargetSinActionData>();
		foreach (TargetSinActionData targetSinActionData in targetDataSet._subTargetList) {
			if (targetSinActionList.Contains(targetSinActionData.GetTargetSinAction())) goodones.Add(targetSinActionData);
		}
		targetDataSet._subTargetList = goodones;*/
		
		bool isdef = circles.Length > 3 && circles[3] == "def";
		bool late = circles.Length > 4 && circles[4] == "late";
		
		//fromAction_new.ReadyOriginTargeting();
		if (isdef) fromUnit.CutInDefenseActionForcely(fromAction_new, true);
		else
		{
			BattleActionModelManager.CUT_IN_ACTION_TYPE cutin = BattleActionModelManager.CUT_IN_ACTION_TYPE.ZERO_INDEX;
			if (late) cutin = BattleActionModelManager.CUT_IN_ACTION_TYPE.LAST_APPEND;
			fromUnit.CutInAction(fromAction_new, SKILL_TARGET_TYPE.NONE, cutin);
			
			fromAction_new.ResetAllTargetData();
			//fromAction_new.ReadyOriginTargeting();
			fromAction_new.SetTargetAfterBattleStart_CutIn(false, false, SKILL_TARGET_TYPE.RANDOM);
			fromAction_new.ChangeMainTargetSinAction(maintarget_sinAction, null, true);
			
			BattleActionModel.TargetDataDetail.TargetDataSet targetDataSet =
				fromAction_new._targetDataDetail.GetCurrentTargetSet();
			targetDataSet._subTargetList.Clear(); // Force subtarget destruction
			
			int subtargetSinActionList_count = subtargetSinActionList.Count;
			if (subtargetSinActionList_count > 0)
			{
				List<TargetSinActionData> subtargetSinActionData_list = new();
				int subtargetnumlimit = Math.Min(subtargetSinActionList_count, fromAction_new.GetAttackWeight() - 1);
				if (subtargetnumlimit > 0)
				{
					for (int i = 0; i < subtargetnumlimit; i++)
					{
						SinActionModel sinAction = subtargetSinActionList[i];
						TargetSinActionData targetSinActionData = new(sinAction);
						subtargetSinActionData_list.Add(targetSinActionData);
					}
					targetDataSet._subTargetList = subtargetSinActionData_list;
				}
			}
			//TargetSinActionData act = new()
			//fromAction_new.SetOriginTargetSinActions(targetSinActionList);
			//fromAction_new.SetTarget(targetSinActionList, fromAction_new.Skill.CanDuel(fromAction_new));
			//fromAction_new.ChangeMainTargetSinAction(targetSinActionList[0], null, true);
			
		}
	}
}