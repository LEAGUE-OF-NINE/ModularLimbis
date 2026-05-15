namespace ModularSkillScripts.Acquirer;

public class AcquirerCountBackup : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_unitModel == null) return -1;
		StageController stageController_inst = Singleton<StageController>.Instance;
		if (stageController_inst == null) return -1;
		BatonPassManager batonManager = stageController_inst._batonPassManager;
		if (batonManager == null) return -1;

		UNIT_FACTION faction = modular.modsa_unitModel.Faction;
		if (circles[0] == "Enemy") faction = faction == UNIT_FACTION.PLAYER ? UNIT_FACTION.ENEMY : UNIT_FACTION.PLAYER;

		if (circles[1] == "current") return batonManager.GetCurrentReadyUnitCount(faction);
		return batonManager.GetReadyUnitCount(faction);
	}
}