using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceStack : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_buffModel == null) return;
		int adder = modular.GetNumFromParamString(circles[0]);
		if (adder > 0)
		{
			var bufHistoryList = new List<BuffHistory>();
			BuffHistory bufHistory = new(adder, 0, ABILITY_SOURCE_TYPE.BUFF);
			bufHistoryList.Add(bufHistory);
			modular.modsa_buffModel.AddBuffStackOrTurn(modular.modsa_unitModel, bufHistoryList, 0, ABILITY_SOURCE_TYPE.BUFF, modular.battleTiming,
				null, out _, out _, out _, out _);
		}
		else modular.modsa_buffModel.LoseStack(modular.modsa_unitModel, 0, modular.battleTiming, out _, adder * -1);
	}
}