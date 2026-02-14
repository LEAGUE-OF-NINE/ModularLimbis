namespace ModularSkillScripts.Acquirer;

public class AcquirerHasPassive : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		int id = modular.GetNumFromParamString(circles[1]);
		if (targetModel == null) return -1;
		if (targetModel.HasPassive(id)) 
		{ 
			return 1;
		} 
		else
		{
			return 0;
		}
	}
}