namespace ModularSkillScripts.Consequence;

public class ConsequenceAppearance : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		foreach (BattleUnitModel targetModel in modelList)
		{
			SingletonBehavior<BattleObjectManager>.Instance.GetView(targetModel).ChangeAppearance(circles[1], true);
		}
	}
}