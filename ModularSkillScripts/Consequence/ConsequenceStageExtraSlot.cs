using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Consequence;

public class ConsequenceStageExtraSlot : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int amount = modular.GetNumFromParamString(circles[0]);
		bool isAdder = circles.Length >= 2;
		if (isAdder) StagePatches.extraSlot += amount;
		else StagePatches.extraSlot = amount;
	}
}