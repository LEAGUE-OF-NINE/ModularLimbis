using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Consequence;

public class ConsequenceSetData : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return;

		int dataID = modular.GetNumFromParamString(circles[1]);
		int dataValue = modular.GetNumFromParamString(circles[2]);

		foreach (BattleUnitModel targetModel in modelList)
		{
			long targetPtr_intlong = targetModel.Pointer.ToInt64();
			SkillScriptInitPatch.SetModUnitData(targetPtr_intlong, dataID, dataValue);
		}
	}
}