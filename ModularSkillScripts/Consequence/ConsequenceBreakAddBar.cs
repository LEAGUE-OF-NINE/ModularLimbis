namespace ModularSkillScripts.Consequence;

public class ConsequenceBreakAddBar : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return;
		string circle_1 = circles[1];
		bool scaleWithHealth = false;
		if (circle_1.EndsWith("%"))
		{
			scaleWithHealth = true;
			circle_1 = circle_1.Remove(circle_1.Length - 1, 1);
		}

		int healthpoint = circles.Length >= 2 ? modular.GetNumFromParamString(circle_1) : 50;

		foreach (BattleUnitModel targetModel in modelList)
		{
			int finalPoint = healthpoint;
			if (scaleWithHealth)
			{
				int maxHP = targetModel.MaxHp;
				finalPoint = maxHP * healthpoint / 100;
			}
			else targetModel.AddBreakSectionForcely(healthpoint);
		}
	}
}