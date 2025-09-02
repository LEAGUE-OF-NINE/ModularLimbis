using System;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceSkillSend : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel fromUnit = modular.GetTargetModel(circles[0]);
		if (fromUnit == null || fromUnit.IsDead()) return;

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
					if (skillIDList.Count > 0) skillID = skillIDList.ToArray()[0];
				}

				break;
			}
			case 'D':
			{
				int index = 0;
				if (int.TryParse(circle_2[1].ToString(), out index)) index -= 1;
				var skillIDList = fromUnit.GetDefenseSkillIDList();
				index = Math.Min(index, skillIDList.Count - 1);
				skillID = skillIDList.ToArray()[index];
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

		var targetSinActionList = new List<SinActionModel>();
		var targetList = modular.GetTargetModelList(circles[1]);
		foreach (BattleUnitModel targetModel in targetList)
		{
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
		else
		{
			fromUnit.CutInAction(fromAction_new);
			if (targetSinActionList.Count > 0)
			{
				fromAction_new.ChangeMainTargetSinAction(targetSinActionList.ToArray()[0], null, true);
			}
		}
	}
}