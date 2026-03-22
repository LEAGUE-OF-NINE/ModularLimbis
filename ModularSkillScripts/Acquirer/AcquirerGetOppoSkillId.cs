using ModularSkillScripts;

namespace ModularSkillScripts.Acquirer;

public class AcquirerGetOpponentSkillId : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_oppoAction != null) return modular.modsa_oppoAction.Skill.GetID();
		return -1;
	}
}