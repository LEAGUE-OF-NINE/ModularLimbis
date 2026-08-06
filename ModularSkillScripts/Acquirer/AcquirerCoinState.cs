namespace ModularSkillScripts.Acquirer;

public class AcquirerCoinState : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		CoinModel coin = modular.modsa_coinModel;
		if (coin == null) return -1;
		
		return coin.GetCoinResult() switch
		{
			COIN_RESULT.HEAD => 1,
			COIN_RESULT.TAIL => 0,
			_ => -1
		};
	}
}