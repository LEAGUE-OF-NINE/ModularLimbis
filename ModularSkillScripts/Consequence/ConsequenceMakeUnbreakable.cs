using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceMakeUnbreakable : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (string.Equals(circles[0], "all", StringComparison.OrdinalIgnoreCase))
		{
			foreach (CoinModel coin in modular.modsa_skillModel.CoinList)
			{
				Singleton<SkillAbility_OverwriteToSuperCoinViaBuffCheck>.Instance.AddScriptToCoin(coin);
			}
		}
		else
		{
			foreach (string circle in circles)
			{
				int idx = modular.GetNumFromParamString(circle);
				if (idx < 0)
				{
					Singleton<SkillAbility_OverwriteToSuperCoinViaBuffCheck>.Instance.AddScriptToCoin(
						modular.modsa_skillModel.GetCoin(modular.modsa_coinModel.GetOriginCoinIndex()));
					continue;
				}

				idx = Math.Min(idx, modular.modsa_skillModel.CoinList.Count - 1);
				Singleton<SkillAbility_OverwriteToSuperCoinViaBuffCheck>.Instance
					.AddScriptToCoin(modular.modsa_skillModel.GetCoin(idx));
			}
		}
	}
}