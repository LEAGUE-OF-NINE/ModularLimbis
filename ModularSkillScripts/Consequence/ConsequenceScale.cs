using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceScale : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int actevent_FakePower = MainClass.timingDict["FakePower"];
		if (circles.Length == 1)
		{
			int power = 0;
			if (modular.activationTiming != actevent_FakePower)
			{
				OPERATOR_TYPE coinOp = OPERATOR_TYPE.NONE;
				if (circledSection == "ADD") coinOp = OPERATOR_TYPE.ADD;
				else if (circledSection == "SUB") coinOp = OPERATOR_TYPE.SUB;
				else if (circledSection == "MUL") coinOp = OPERATOR_TYPE.MUL;
				else power = modular.GetNumFromParamString(circledSection);
				if (coinOp != OPERATOR_TYPE.NONE)
					foreach (CoinModel coin in modular.modsa_skillModel.CoinList)
						coin._operatorType = coinOp;
				else modular.coinScaleAdder = power;
			}
			else modular.coinScaleAdder = modular.GetNumFromParamString(circledSection);
		}
		else if (modular.activationTiming != actevent_FakePower)
		{
			int coin_idx = -999;
			coin_idx = modular.GetNumFromParamString(circles[1]);
			if (coin_idx == -999) return;

			string firstCircle = circles[0];

			int power = 0;
			OPERATOR_TYPE coinOp = OPERATOR_TYPE.NONE;
			if (firstCircle == "ADD") coinOp = OPERATOR_TYPE.ADD;
			else if (firstCircle == "SUB") coinOp = OPERATOR_TYPE.SUB;
			else if (firstCircle == "MUL") coinOp = OPERATOR_TYPE.MUL;
			else power = modular.GetNumFromParamString(firstCircle);

			coin_idx = Math.Min(modular.modsa_skillModel.CoinList.Count - 1, coin_idx);
			modular.modsa_skillModel.CoinList.ToArray()[coin_idx]._scale += power;
			if (coinOp != OPERATOR_TYPE.NONE) modular.modsa_skillModel.CoinList.ToArray()[coin_idx]._operatorType = coinOp;
		}
	}
}