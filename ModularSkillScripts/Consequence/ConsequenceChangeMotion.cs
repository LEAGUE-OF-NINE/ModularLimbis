using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceChangeMotion : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (Enum.TryParse(circles[0], out modular.modsa_motionDetail)) return;
		MainClass.Logg.LogError($"Invalid motion detail: {circles[0]}");
	}
}