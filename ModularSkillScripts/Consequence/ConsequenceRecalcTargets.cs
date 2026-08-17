using System;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceRecalcTargets : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (action == null) return;
			
		BattleActionModel.TargetDataDetail.TargetDataSet targetDataSet = action._targetDataDetail.GetCurrentTargetSet();
		if (targetDataSet == null) return;
		
		string circle_0 = circles[0];
		switch (circle_0)
		{
			case "Normal":{
				action.RecheckSubTargetWhenChangeTargetNum();
			} return;
			case "BegoneSubTargets":{
				targetDataSet.RemoveAllSubTarget(action);
				targetDataSet._subTargetList?.Clear(); // Force subtarget destruction
			} return;
			case "Remove": {
				List<BattleUnitModel> removal_list = modular.GetTargetModelList(circles[1]);
				List<TargetSinActionData> subtarget_list = targetDataSet._subTargetList;
				for (int i = subtarget_list.Count - 1; i >= 0; i--)
				{
					TargetSinActionData targetData = subtarget_list[i];
					BattleUnitModel targetUnit = targetData.GetTargetUnit();
					if (removal_list.Contains(targetUnit)) subtarget_list.RemoveAt(i);
				}
			} return;
			case "Add": {
				BattleUnitModel maintarget = targetDataSet.GetMainTarget();
				List<BattleUnitModel> add_list = modular.GetTargetModelList(circles[1]);
				List<SinActionModel> subtarget_sinaction_list = targetDataSet.GetSubTargetSinActionList();
				foreach(BattleUnitModel unit_add in add_list) {
					if (unit_add == null || unit_add == maintarget) continue;
					SinActionModel sinaction_add = unit_add.GetFirstSinAction();
					if (sinaction_add == null || sinaction_add.GetSlotWeight() < 1) continue;
					if (!subtarget_sinaction_list.Contains(sinaction_add)) {
						TargetSinActionData targetSinActionData = new(sinaction_add);
						targetDataSet._subTargetList?.Add(targetSinActionData);
					}
				}
			} return;
		}
		
	}
}