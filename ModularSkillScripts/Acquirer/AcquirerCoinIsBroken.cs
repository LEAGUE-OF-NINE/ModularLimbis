namespace ModularSkillScripts.Acquirer;

public class AcquirerCoinIsBroken : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_coinModel == null) return -1;
		return modular.modsa_coinModel.IsUsableInDuel ? 0 : 1;
	}
}