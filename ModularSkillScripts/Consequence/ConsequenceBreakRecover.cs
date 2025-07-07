namespace ModularSkillScripts.Consequence;

public class ConsequenceBreakRecover : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		bool force = circles.Length > 1;
		foreach (BattleUnitModel targetModel in modelList)
		{
			if (force) targetModel.RecoverAllBreak(modular.battleTiming);
			else targetModel.RecoverBreak(modular.battleTiming);
		}
	}
}