namespace ModularSkillScripts.Acquirer;

public class AcquirerGetPattern : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circledSection);
		if (targetModel is not { IsAbnormalityOrPart: true }) return -1;
		PatternScript_Abnormality pattern = ModularSA.AsAbnormalityModel(targetModel)?.PatternScript;
		if (pattern == null) return -1;
		int pattern_idx = pattern.currPatternIdx;
		MainClass.Logg.LogInfo("getpattern pattern_idx: " + pattern_idx);
		return pattern.currPatternIdx;
	}
}