namespace ModularSkillScripts.Acquirer;

public class AcquirerIsUnbreakable : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		CoinModel targetAction = modular.modsa_coinModel;
		if (circles[0] == "Target") targetAction = modular.modsa_coinModel;
		if (targetAction == null) return -1;

		if (targetAction.IsSuperCoin == true)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
}