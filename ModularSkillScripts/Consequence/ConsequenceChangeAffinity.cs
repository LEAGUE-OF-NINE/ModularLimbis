using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceChangeAffinity : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var skillAbility = new SkillAbility_1021305UpgradeAttribute();
		var skillModel = modular.modsa_skillModel;
		ATTRIBUTE_TYPE attribute = ATTRIBUTE_TYPE.BLACK;
		Enum.TryParse(circles[0], out attribute);
		skillAbility.OnReplace(attribute);
		skillModel.AddTemporarySkillAbility(skillAbility);
	}
}