using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerGetData : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int value = 0;
		string circle_0 = circles[0];
		if (circle_0 == "Encounter")
		{
			int dataID = modular.GetNumFromParamString(circles[1]);
			value = SkillScriptInitPatch.GetModUnitData(0, dataID);
		}
		else
		{
			BattleUnitModel targetModel = modular.GetTargetModel(circle_0);
			if (targetModel == null) return -1;
			int dataID = modular.GetNumFromParamString(circles[1]);
			long targetPtr_intlong = targetModel.Pointer.ToInt64();
			value = SkillScriptInitPatch.GetModUnitData(targetPtr_intlong, dataID);
		}

		return value;
	}
}