namespace ModularSkillScripts.Consequence;

public class ConsequenceSummonUnitFromQueue : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleObjectManager.Instance.FlushAddUnitOnlyQueue();
	}
}