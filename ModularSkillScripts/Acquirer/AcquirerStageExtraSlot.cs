namespace ModularSkillScripts.Acquirer;

public class AcquirerStageExtraSlot : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		return StagePatches.extraSlot;
	}
}