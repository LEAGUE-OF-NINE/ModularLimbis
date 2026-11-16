using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceChangeAffinity : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (Enum.TryParse(circles[0], out ATTRIBUTE_TYPE attribute))
		{
			BattleActionModel actionModel = modular.modsa_selfAction;
			SkillModel skillModel = actionModel._skill;
			ATTRIBUTE_TYPE sinType_before = skillModel.GetAttributeType();

			if (sinType_before == attribute) return;
			SkillAbility_1021305UpgradeAttribute skillAbility = new();
			skillAbility.OnReplace(attribute);

			bool changeGeneratedResource = circles.Length > 1 && circles[1] == "true";
			if (changeGeneratedResource)
			{
				bool hadSin = actionModel._expectedToEarnMap.TryGetValue(sinType_before, out int amount_before);
				if (hadSin)
				{
					skillModel.AddTemporarySkillAbility(skillAbility);
					actionModel._expectedToEarnMap[sinType_before] = 0;
					bool hasSin = actionModel._expectedToEarnMap.TryGetValue(sinType_before, out int amount_after);
					if (hasSin) actionModel._expectedToEarnMap[attribute] = amount_before + amount_after;
					else actionModel._expectedToEarnMap[attribute] = amount_before;
				}
			}
			else
			{
				skillModel.AddTemporarySkillAbility(skillAbility);
			}
		}
	}
}