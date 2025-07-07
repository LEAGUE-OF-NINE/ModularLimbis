namespace ModularSkillScripts.Consequence;

public class ConsequenceSummonAssistant : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int assistantID = modular.GetNumFromParamString(circles[0]);
		int assistantlevel = modular.GetNumFromParamString(circles[1]);
		int assistantSynclevel = modular.GetNumFromParamString(circles[2]);
		BattleUnitModel summonedUnit =
			BattleObjectManager.Instance.CreateAssistantUnit(assistantID, assistantlevel, assistantSynclevel, assistantID);
		summonedUnit.InitActionSlots();

		var aliveList = BattleObjectManager.Instance.GetAliveList(true);
		foreach (BattleUnitModel unit in aliveList) unit.RefreshSpeed();
	}
}