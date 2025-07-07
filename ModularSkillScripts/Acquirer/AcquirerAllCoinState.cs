namespace ModularSkillScripts.Acquirer;

public class AcquirerAllCoinState : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel targetAction = modular.modsa_selfAction;
		if (circles[0] == "Target") targetAction = modular.modsa_oppoAction;
		if (targetAction == null) return -1;

		string way = circles[1];

		int coinCount = targetAction.Skill.GetAliveCoins().Count;
		int headCount = targetAction.GetHeadCoinNum();
		int tailCount = targetAction.GetTailCoinNum();
		return way switch
		{
			"full" when coinCount == headCount => 1,
			"full" when coinCount == tailCount => 2,
			"headcount" => headCount,
			"tailcount" => tailCount,
			_ => 0
		};
	}
}