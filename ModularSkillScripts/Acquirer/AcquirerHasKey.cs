using System.Linq;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Acquirer;

public class AcquirerHasKey : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		List<BattleUnitModel> modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return -1;
		
		bool operator_OR = circles[1] == "OR";

		int total = 0;
		foreach (BattleUnitModel unit in modelList) {
			if (unit == null) continue;
			if (HasUnitKeywordOrAssociation(unit, operator_OR, circles.Skip(2).ToArray())) total += 1;
		}
		
		return total;
	}

	public static bool HasUnitKeywordOrAssociation(BattleUnitModel targetModel, bool operator_OR, string[] keyword_s_array)
	{
		if (targetModel.IsAbnormalityOrPart)
		{
			BattleUnitModel_Abnormality_Part part = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
			if (part != null) targetModel = part.Abnormality;
		}

		var unitKeywordList = targetModel._unitDataModel._classInfo.unitKeywordList;
		var associationList = targetModel._unitDataModel._classInfo.associationList;

		bool success = false;
		for (int i = 0; i < keyword_s_array.Length; i++) {
			string keyword_string = keyword_s_array[i];
			success = unitKeywordList.Contains(keyword_string) || associationList.Contains(keyword_string);

			if (operator_OR == success) break; // [IF Statement] Simplification
		}
		
		return success;
	}
}