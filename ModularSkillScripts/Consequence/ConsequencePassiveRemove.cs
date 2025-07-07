using System.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequencePassiveRemove : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		int id = modular.GetNumFromParamString(circles[1]);
		foreach (BattleUnitModel targetModel in modelList)
		{
			List<PassiveModel> removal = new();
			foreach (PassiveModel passive in targetModel.GetPassiveList())
			{
				if (passive.GetID() == id) removal.Add(passive);
			}

			foreach (PassiveModel passive in removal) targetModel.GetPassiveList().Remove(passive);
		}
	}
}