using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceBreakRecover : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		List<BattleUnitModel> modelList = modular.GetTargetModelList(circles[0]);
		bool force = circles.Length > 1 && (circles[1] == "force" || circles[1] == "forced");
		BATTLE_EVENT_TIMING timing = modular.battleTiming;
		foreach (BattleUnitModel targetModel in modelList)
		{
			if (!targetModel.IsForcelyBreak() || force) targetModel.RecoverAllBreak(timing);
		}
	}
}