using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceEndLimbus : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		Environment.FailFast("Immediately crashed Limbus");
	}
}