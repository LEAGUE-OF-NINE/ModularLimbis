namespace ModularSkillScripts.Acquirer;

public class AcquirerEGORisk : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel targetAction = modular.modsa_selfAction;
		if (circles[0] == "Target") targetAction = modular.modsa_oppoAction;
		if (targetAction == null) return -1;
		SkillModel skill = targetAction._skill;
		if (!skill.IsEgoSkill()) return -1;
		BattleEgoModel ego = targetAction._model.GetEgoModel(skill.GetID());
		if (ego == null) return -1;
		EGO_TYPE ego_risk = ego.GetEgoType();
		return (int)ego_risk;
	}
}