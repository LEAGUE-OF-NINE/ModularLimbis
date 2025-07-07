using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceReuseCoin : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		foreach (string circle in circles)
		{
			int idx = modular.GetNumFromParamString(circle);
			if (idx < 0)
			{
				modular.modsa_skillModel.CopyCoin(modular.modsa_selfAction, modular.modsa_coinModel.GetOriginCoinIndex(), modular.battleTiming);
				continue;
			}

			idx = Math.Min(idx, modular.modsa_skillModel.CoinList.Count - 1);
			modular.modsa_skillModel.CopyCoin(modular.modsa_selfAction, idx, modular.battleTiming);
		}
	}
}