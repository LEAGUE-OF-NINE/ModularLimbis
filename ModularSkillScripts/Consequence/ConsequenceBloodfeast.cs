using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceBloodfeast : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int bloodDinner = modular.GetNumFromParamString(circles[1]);
		if (string.Equals(circles[0], "add", StringComparison.OrdinalIgnoreCase)) BloodDinnerBuff.BuffInstance.AddStack(bloodDinner, modular.battleTiming);
		else if (string.Equals(circles[0], "sub", StringComparison.OrdinalIgnoreCase)) BloodDinnerBuff.BuffInstance.SubStack(bloodDinner, modular.battleTiming);
		else if (circles.Length > 1 && string.Equals(circles[0], "use", StringComparison.OrdinalIgnoreCase))
		{
			BattleUnitModel targetModel = modular.GetTargetModel(circles[2]);
			BloodDinnerBuff.BuffInstance.UseBuffStack(targetModel, bloodDinner, modular.battleTiming, null);
		}
	}
}