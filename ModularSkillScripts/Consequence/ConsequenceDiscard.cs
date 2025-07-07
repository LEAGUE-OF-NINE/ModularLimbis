using Il2CppSystem;

namespace ModularSkillScripts.Consequence;

public class ConsequenceDiscard : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SORT_SKILL_BY_INDEX sorting;
		Enum.TryParse(circles[0], true, out sorting);
		int times = modular.GetNumFromParamString(circles[1]);
		for (int i = 0; i < times; i++)
			modular.modsa_unitModel.DiscardSkill(modular.battleTiming, sorting, modular.modsa_selfAction);
	}
}