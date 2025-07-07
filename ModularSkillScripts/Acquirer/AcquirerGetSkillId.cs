namespace ModularSkillScripts.Acquirer;

public class AcquirerGetSkillId : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_skillModel != null) return modular.modsa_skillModel.GetID();
		if (modular.modsa_selfAction != null) return modular.modsa_selfAction.Skill.GetID();
		return 0;
	}
}