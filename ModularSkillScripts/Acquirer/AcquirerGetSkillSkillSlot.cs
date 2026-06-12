namespace ModularSkillScripts.Acquirer;

public class AcquirerGetSkillSkillSlot : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (circles.Length > 0 && circles[0] == "Target") action = modular.modsa_oppoAction;
		
		if (action != null) return action.SinAction.GetSlotIndex();
		return -1;
	}
}