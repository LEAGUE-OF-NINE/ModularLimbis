using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceChangeAtkType : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		ATK_BEHAVIOUR attribute = ATK_BEHAVIOUR.NONE;
		Enum.TryParse(circles[0], out attribute);
		modular.atktype = attribute;
	}
}