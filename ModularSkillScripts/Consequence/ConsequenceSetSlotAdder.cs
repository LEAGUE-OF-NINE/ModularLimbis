namespace ModularSkillScripts.Consequence;

public class ConsequenceSetSlotAdder : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int amount = modular.GetNumFromParamString(circles[1]);
		if (amount < 0) return;
		bool add_max_instead = circles.Length > 2;
		if (!add_max_instead)
		{
			modular.slotAdder = amount;
			return;
		}

		var modelList = modular.GetTargetModelList(circles[0]);
		foreach (BattleUnitModel targetModel in modelList)
		{
			if (targetModel == null || !targetModel.IsAbnormalityOrPart) continue;
			BattleUnitModel_Abnormality abnoModel;
			BattleUnitModel_Abnormality_Part part = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
			if (part != null) abnoModel = part.Abnormality;
			else abnoModel = targetModel.TryCast<BattleUnitModel_Abnormality>();
			if (abnoModel == null) continue;
			PatternScript_Abnormality pattern = abnoModel.PatternScript;
			pattern._slotMax = amount;
		}
	}
}