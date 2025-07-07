namespace ModularSkillScripts.Acquirer;

public class AcquirerGetData : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		if (targetModel == null) return -1;
		int dataID = modular.GetNumFromParamString(circles[1]);
		long targetPtr_intlong = targetModel.Pointer.ToInt64();
		return SkillScriptInitPatch.GetModUnitData(targetPtr_intlong, dataID);
	}
}