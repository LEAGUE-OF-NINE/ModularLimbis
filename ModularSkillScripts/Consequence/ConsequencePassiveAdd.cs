namespace ModularSkillScripts.Consequence;

public class ConsequencePassiveAdd : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		int id = modular.GetNumFromParamString(circles[1]);

		// opt_3 = Tell users to add 'nodupe' as a third, optional argument. Otherwise, leave empty.
		// Default is 'yesdupe'.
		bool dupe = true;
		if (circles.Length >= 3)
		{
			string dupe_string = circles[2];
			dupe = dupe_string == "yesdupe";
		}

		foreach (BattleUnitModel targetModel in modelList)
		{
			if (!dupe && targetModel.HasPassive(id)) continue;
			targetModel.AddPassive(id);
		}
	}
}