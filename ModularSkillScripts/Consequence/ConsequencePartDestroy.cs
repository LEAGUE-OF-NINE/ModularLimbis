using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequencePartDestroy : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (circles.Length < 2)
		{
			MainClass.LogModular("ConsequencePartDestroy Not Enough Arguments", true);
			return;
		}
		List<BattleUnitModel> modelList = modular.GetTargetModelList(circles[0]);
		
		bool undestroy = circles[1] == "undestroy";
		bool regenerate = circles[1] == "regenerate";
		BATTLE_EVENT_TIMING timing = modular.battleTiming;
		foreach (BattleUnitModel targetModel in modelList)
		{
			BattleUnitModel_Abnormality_Part part = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
			if (part == null) {
				MainClass.LogModular("ConsequencePartDestroy targetModel is not BattleUnitModel_Abnormality_Part", true);
				continue;
			}
			if (undestroy) part.RecoverDestroyed();
			else if (regenerate) part.Regenerate(timing);
			else part.Destroy(null, timing, null, null, DAMAGE_SOURCE_TYPE.SYSTEM);
		}
	}
}