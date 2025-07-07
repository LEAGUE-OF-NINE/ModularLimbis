namespace ModularSkillScripts.Acquirer;

public class AcquirerTurn : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_buffModel != null) return modular.modsa_buffModel.GetTurn(0);
		return -1;
	}
}