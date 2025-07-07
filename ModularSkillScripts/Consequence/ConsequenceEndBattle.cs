namespace ModularSkillScripts.Consequence;

public class ConsequenceEndBattle : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		Singleton<StageController>.Instance.EndBattlePhaseForcely(true);
	}
}