namespace ModularSkillScripts.Acquirer;

public class AcquirerHasKey : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		if (targetModel == null) return -1;

		if (targetModel.IsAbnormalityOrPart)
		{
			BattleUnitModel_Abnormality_Part part = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
			if (part != null) targetModel = part.Abnormality;
		}

		var unitKeywordList = targetModel._unitDataModel.ClassInfo.unitKeywordList;
		var associationList = targetModel._unitDataModel.ClassInfo.associationList;

		bool operator_OR = circles[1] == "OR";

		bool success = false;
		for (int i = 2; i < circles.Length; i++)
		{
			string keyword_string = circles[i];
			success = unitKeywordList.Contains(keyword_string) || associationList.Contains(keyword_string);

			if (operator_OR == success) break; // [IF Statement] Simplification
		}

		return success ? 1 : 0;
	}
}