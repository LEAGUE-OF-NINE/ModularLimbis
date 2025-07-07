namespace ModularSkillScripts.Consequence;

public class ConsequencePassiveAdd : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		int id = modular.GetNumFromParamString(circles[1]);
		foreach (BattleUnitModel targetModel in modelList) targetModel.AddPassive(id);
	}
}