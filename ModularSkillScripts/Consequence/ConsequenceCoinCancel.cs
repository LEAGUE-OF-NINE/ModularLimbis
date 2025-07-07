using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceCoinCancel : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		foreach (string circle in circles)
		{
			int idx = modular.GetNumFromParamString(circle);
			if (idx < 0)
			{
				modular.modsa_skillModel.DisableCoin(modular.modsa_coinModel.GetOriginCoinIndex());
				continue;
			}

			idx = Math.Min(idx, modular.modsa_skillModel.CoinList.Count - 1);
			modular.modsa_skillModel.DisableCoin(idx);
		}
	}
}