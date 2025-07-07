namespace ModularSkillScripts.Acquirer;

public class AcquirerGetCoinCount : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel targetAction = modular.modsa_selfAction;
		if (circles[0] == "Target") targetAction = modular.modsa_oppoAction;
		if (targetAction == null) return -1;

		int coinCount = targetAction.Skill.GetAliveCoins().Count;
		if (circles[1] == "og") coinCount = targetAction.Skill.CoinList.Count;

		return coinCount;
	}
}