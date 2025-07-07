using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceAggro : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		int amount = modular.GetNumFromParamString(circles[1]);
		bool nextRound = true;
		int slot = -2;
		if (circles.Length > 2) nextRound = circles[2] == "next";
		if (circles.Length > 3) slot = modular.GetNumFromParamString(circles[3]);

		foreach (BattleUnitModel targetModel in modelList)
		{
			var sinActionList = targetModel.GetSinActionList();
			int sinActionCount = sinActionList.Count;
			if (sinActionCount < 1) continue;

			if (targetModel == modular.modsa_unitModel)
			{
				switch (slot)
				{
					case -2 when modular.modsa_selfAction != null:
					{
						if (nextRound) modular.modsa_selfAction.SinAction.StackNextTurnAggroAdder(amount);
						else modular.modsa_selfAction.SinAction.StackThisTurnAggroAdder(amount);
						break;
					}
					case -1:
					{
						int quotient = amount / sinActionCount;
						int remainder = amount % sinActionCount;

						foreach (SinActionModel sinAction in sinActionList)
						{
							int finalAmount = amount;
							if (remainder > 0)
							{
								finalAmount += 1;
								remainder -= 1;
							}

							if (nextRound) sinAction.StackNextTurnAggroAdder(finalAmount);
							else sinAction.StackThisTurnAggroAdder(finalAmount);
						}

						break;
					}
					default:
					{
						int chosenSlot = slot;
						if (chosenSlot > sinActionCount - 1) chosenSlot = sinActionCount - 1;
						if (chosenSlot < 0) chosenSlot = 0;
						if (nextRound) sinActionList.ToArray()[chosenSlot].StackNextTurnAggroAdder(amount);
						else sinActionList.ToArray()[chosenSlot].StackThisTurnAggroAdder(amount);
						break;
					}
				}
			}
			else
			{
				int chosenSlot = Math.Min(slot, sinActionCount - 1);
				if (chosenSlot == -2) chosenSlot = 0;

				if (chosenSlot == -1)
				{
					int quotient = amount / sinActionCount;
					int remainder = amount % sinActionCount;

					foreach (SinActionModel sinAction in sinActionList)
					{
						int finalAmount = quotient;
						if (remainder > 0)
						{
							finalAmount += 1;
							remainder -= 1;
						}

						if (finalAmount < 1) break;
						if (nextRound) sinAction.StackNextTurnAggroAdder(finalAmount);
						else sinAction.StackThisTurnAggroAdder(finalAmount);
					}

					continue;
				}

				if (nextRound) sinActionList.ToArray()[chosenSlot].StackNextTurnAggroAdder(amount);
				else sinActionList.ToArray()[chosenSlot].StackThisTurnAggroAdder(amount);
			}
		}
	}
}