using System.Linq;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceShield : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		List<BattleUnitModel> modelList = modular.GetTargetModelList(circles[0]);
		string circle_1 = circles[1];
		
		bool percentageheal = circle_1.Last() == '%';
		if (percentageheal) circle_1 = circle_1.Remove(circle_1.Length - 1);
		int amount = modular.GetNumFromParamString(circle_1);
		
		bool permashield = circles.Length > 2;
		foreach (BattleUnitModel targetModel in modelList)
		{
			int finalAmount = amount;
			if (percentageheal) finalAmount = targetModel.MaxHp * finalAmount / 100;
			if (finalAmount >= 0) targetModel.AddShield(finalAmount, !permashield, ABILITY_SOURCE_TYPE.SKILL, modular.battleTiming);
			else targetModel.ReduceShield(modular.battleTiming, finalAmount *= -1, modular.modsa_unitModel);
		}
	}
}