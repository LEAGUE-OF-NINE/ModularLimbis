namespace ModularSkillScripts.Consequence;

public class ConsequenceBreak : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return;
		string opt2_string = circles.Length >= 2 ? circles[1] : "natural";
		bool force = opt2_string != "natural";
		bool both = opt2_string == "both";
		bool resistancebreak = circles.Length <= 2;

		foreach (BattleUnitModel targetModel in modelList)
		{
			ABILITY_SOURCE_TYPE abilitySourceType = ABILITY_SOURCE_TYPE.SKILL;
			if (modular.abilityMode == 2) abilitySourceType = ABILITY_SOURCE_TYPE.PASSIVE;

			if (force) targetModel.BreakForcely(modular.modsa_unitModel, abilitySourceType, modular.battleTiming, false, modular.modsa_selfAction);
			if (!force || both) targetModel.Break(modular.modsa_unitModel, modular.battleTiming, modular.modsa_selfAction);
			if (resistancebreak) targetModel.ChangeResistOnBreak();
		}
	}
}