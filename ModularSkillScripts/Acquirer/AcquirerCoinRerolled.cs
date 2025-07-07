namespace ModularSkillScripts.Acquirer;

public class AcquirerCoinRerolled : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return modular.modsa_coinModel.IsReRolled() ? 1 : 0;
	}
}