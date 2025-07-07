namespace ModularSkillScripts.Acquirer;

public class AcquirerStack : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_buffModel != null) return modular.modsa_buffModel.GetStack(0);
		return -1;
	}
}