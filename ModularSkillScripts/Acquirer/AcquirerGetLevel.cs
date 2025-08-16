using Lethe.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerGetLevel : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		if (targetModel == null) return -1;
		return targetModel.Level;
	}
}
