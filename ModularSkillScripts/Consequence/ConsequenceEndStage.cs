namespace ModularSkillScripts.Consequence;

public class ConsequenceEndStage : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		Singleton<StageController>.Instance.EndStage();
	}
}