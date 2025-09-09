namespace ModularSkillScripts.Consequence;

public class ConsequenceSummonEnemy : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int enemy_ID = modular.GetNumFromParamString(circles[0]);
		int enemy_level = modular.GetNumFromParamString(circles[1]);
		int enemy_Synclevel = modular.GetNumFromParamString(circles[2]);
		int waveIndex = modular.GetNumFromParamString(circles[3]);
		bool isenemy = circles.Length >= 5;
		BattleUnitModel summonedUnit = SingletonBehavior<BattleObjectManager>.Instance.CreateEnemyUnit(enemy_ID, enemy_level, enemy_Synclevel,
			waveIndex, enemy_ID, null, UNIT_POSITION.MAIN);
		if (isenemy) summonedUnit.InitActionSlots();

		var aliveList = BattleObjectManager.Instance.GetAliveList(true);
		foreach (BattleUnitModel unit in aliveList) unit.RefreshSpeed();
	}
}