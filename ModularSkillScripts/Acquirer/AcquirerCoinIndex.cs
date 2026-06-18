namespace ModularSkillScripts.Acquirer;

public class AcquirerCoinIndex : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		CoinModel coin = modular.modsa_coinModel;
		if (coin == null) return -1;
		int coin_index = coin.GetRealCoinIndex();
		if (action != null && circledSection == "isLastCoin")
		{
			return action.IsLastCoin(coin) ? 1 : 0;
		}
		
		return coin.GetRealCoinIndex();
	}
}