namespace ModularSkillScripts.Consequence;

public class ConsequenceWinStage : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		// kill all enemies
		if (SingletonBehavior<BattleObjectManager>.Instance._unitList.TryGetValue(UNIT_FACTION.ENEMY, out var enemies))
			enemies.Clear();
		
		// skip all waves
		Singleton<StageController>.Instance._waveCount = 999999999;
		
		// end stage
		Singleton<StageController>.Instance.EndStage();
	}
}