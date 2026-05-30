using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceInsertSkill : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int skillID = modular.GetNumFromParamString(circles[0]);
		BattleUnitModel unit = modular.modsa_unitModel;
		BattleActionModel action = modular.modsa_selfAction;

		SinActionModel sinAction = null;
		int slotID = 0;
		if (circles.Length > 2) slotID = modular.GetNumFromParamString(circles[2]);
		else if (action != null)
		{
			slotID = -1;
			sinAction = action.SinAction;
		}

		if (slotID >= 0)
		{
			slotID = Math.Min(slotID, unit.GetPermanentSinActionListCount());
			sinAction = unit.GetSinActionList()[slotID];
		}

		if (sinAction == null) return;
	
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
		
		
	} // END ExecuteConsequence
} // END IModularConsequence