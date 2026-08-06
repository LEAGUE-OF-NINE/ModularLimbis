namespace ModularSkillScripts.Acquirer;

public class AcquirerIsUsableInDuel : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		CoinModel coin = modular.modsa_coinModel;
		if (coin == null) return -1;
		return coin.IsUsableInDuel ? 1 : 0;
	}
}