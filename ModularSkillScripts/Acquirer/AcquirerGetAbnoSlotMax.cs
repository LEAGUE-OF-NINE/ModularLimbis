namespace ModularSkillScripts.Acquirer;

public class AcquirerGetAbnoSlotMax : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circledSection);
		if (targetModel == null) return 0;

		BattleUnitModel_Abnormality abnoModel = ModularSA.AsAbnormalityModel(targetModel);
		return abnoModel == null ? 0 : abnoModel.PatternScript.SlotMax;
	}
}