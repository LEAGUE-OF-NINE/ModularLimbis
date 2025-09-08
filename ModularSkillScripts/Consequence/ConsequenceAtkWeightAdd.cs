using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceAtkWeightAdd : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		modular.atkWeightAdder = modular.GetNumFromParamString(circledSection);
	}
}