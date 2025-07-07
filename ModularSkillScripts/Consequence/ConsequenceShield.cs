namespace ModularSkillScripts.Consequence;

public class ConsequenceShield : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		int amount = modular.GetNumFromParamString(circles[1]);
		bool permashield = circles.Length > 2;
		foreach (BattleUnitModel targetModel in modelList)
		{
			if (amount >= 0) targetModel.AddShield(amount, !permashield, ABILITY_SOURCE_TYPE.SKILL, modular.battleTiming);
			else targetModel.ReduceShield(modular.battleTiming, amount *= -1, modular.modsa_unitModel);
		}
	}
}