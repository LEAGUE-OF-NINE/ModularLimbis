namespace ModularSkillScripts.Acquirer;

public class AcquirerSkillDefType : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SkillModel skill = modular.modsa_skillModel;
		if (circles.Length > 0 && circles[0] == "Target") skill = modular.modsa_oppoAction?.Skill;
		if (skill != null) return (int)skill.GetDefenseType();
		return -1;
	}
}