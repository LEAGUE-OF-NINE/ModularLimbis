using System;
using System.Linq;
using ModularSkillScripts;

namespace ModularSkillScripts.Consequence;

public class ConsequenceRemoveCoin : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SkillModel skill = (circles[0] == "Self") ? modular.modsa_skillModel : modular.modsa_oppoAction._skill;
		foreach (string circle in circles.Skip(1))
		{
			int idx = modular.GetNumFromParamString(circle);
			if (idx < 0)
			{
				skill.RemoveCoinModelFromListByIndex(modular.modsa_coinModel.GetOriginCoinIndex());
				continue;
			}

			idx = Math.Min(idx, skill.CoinList.Count - 1);
			skill.RemoveCoinModelFromListByIndex(idx);
		}
	}
}