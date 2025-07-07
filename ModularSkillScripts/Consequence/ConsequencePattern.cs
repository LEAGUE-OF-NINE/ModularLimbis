namespace ModularSkillScripts.Consequence;

public class ConsequencePattern : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel_Abnormality abnoModel = null;
		if (modular.modsa_unitModel.IsAbnormalityOrPart)
		{
			var abnoPart = modular.modsa_unitModel.TryCast<BattleUnitModel_Abnormality_Part>();
			abnoModel = abnoPart != null ? abnoPart.Abnormality : modular.modsa_unitModel.TryCast<BattleUnitModel_Abnormality>();
		}
		else return;

		if (abnoModel == null) return;

		PatternScript_Abnormality pattern = abnoModel.PatternScript;

		//List<BattlePattern> battlePattern_list = pattern._patternList;

		int pickedPattern_idx = modular.GetNumFromParamString(circles[0]);
		MainClass.Logg.LogInfo("pickedPattern_idx: " + pickedPattern_idx);
		pattern.currPatternIdx = pickedPattern_idx;
		//int slotCount = -1;
		//bool randomize = false;

		//pattern.PickSkillsByPattern(pickedPattern_idx, slotCount, randomize);
	}
}