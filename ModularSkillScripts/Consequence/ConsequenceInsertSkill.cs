using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceInsertSkill : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int skillID = modular.GetNumFromParamString(circles[0]);
		BattleUnitModel unit = modular.modsa_unitModel;
		BattleActionModel action = modular.modsa_selfAction;
		
		if (action != null && circles.Length < 2)
		{
			SinActionModel sinAction = action.SinAction;
			int numslot = circles[1] == "bottom" ? 0 : 1;
			UnitSinModel chosenSlot = sinAction.currentSinList[numslot];
			if (chosenSlot == null) return;
			SkillModel chosenSkill = chosenSlot.GetSkill();
			if (chosenSkill == null) return;
			if (chosenSkill.IsEgoSkill()) return;
			
			UnitSinModel sinModel_new = new(skillID, unit, sinAction);
			BattleActionModel action_new = new(sinModel_new, unit, sinAction);
			sinModel_new._currentAction = action_new;
			sinAction.currentSinList[numslot] = sinModel_new;
			return;
		}
	
		if (circles.Length > 1)
		{
			int slotID = modular.GetNumFromParamString(circles[2]);
			slotID = Math.Min(slotID, unit.GetPermanentSinActionListCount());
			
			SinActionModel sinAction = unit.GetSinActionList()[slotID];
			int numslot = circles[1] == "bottom" ? 0 : 1;
			UnitSinModel chosenSlot = sinAction.currentSinList[numslot];
			if (chosenSlot == null) return;
			SkillModel chosenSkill = chosenSlot.GetSkill();
			if (chosenSkill == null) return;
			if (chosenSkill.IsEgoSkill()) return;
			
			UnitSinModel sinModel_new = new(skillID, unit, sinAction);
			BattleActionModel action_new = new(sinModel_new, unit, sinAction);
			sinModel_new._currentAction = action_new;
			sinAction.currentSinList[numslot] = sinModel_new;
		}
		
	} // END ExecuteConsequence
} // END IModularConsequence