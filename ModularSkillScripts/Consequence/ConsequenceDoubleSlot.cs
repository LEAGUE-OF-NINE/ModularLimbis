namespace ModularSkillScripts.Consequence;

public class ConsequenceDoubleSlot : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		foreach (BattleUnitModel targetModel in modular.GetTargetModelList(circles[0]))
		{
			int instID = targetModel.InstanceID;
			if (StagePatches.doubleslotterIDList.Contains(instID)) continue;
			StagePatches.doubleslotterIDList.Add(instID);
		}
	}
}