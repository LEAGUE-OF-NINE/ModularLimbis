namespace ModularSkillScripts.Consequence;

public class ConsequenceDropSkill : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int skillID = modular.GetNumFromParamString(circles[0]);
		BattleUnitModel unit = modular.modsa_unitModel;
		BattleActionModel action = modular.modsa_selfAction;
		if (action != null)
		{
			action.SinAction.ReplaceReadySkillAtoB(skillID);
			return;
		}

		if (circles.Length < 2) return;
		//unit.AddNewSinActionModelWithSkill(skillID);
		int slotID = modular.GetNumFromParamString(circles[1]);
		unit.ReplaceReadySkillAtoB(skillID, -1, slotID);
	} // END ExecuteConsequence
} // END IModularConsequence