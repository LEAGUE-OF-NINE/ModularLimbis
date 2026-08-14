namespace ModularSkillScripts.Acquirer;

public class AcquirerIsBaseSkill : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int skillID = -1;
		if (modular.modsa_skillModel != null) skillID = modular.modsa_skillModel.GetID();
		else if (modular.modsa_selfAction != null) skillID = modular.modsa_selfAction.Skill.GetID();

		if (skillID != -1) 
		{
			foreach (SkillModel skill in modular.modsa_unitModel.GetSkillList())
			{
				if (skill.GetID() == skillID)
				{
					return 1;
				}
			}
		}
		return 0;
	}
}