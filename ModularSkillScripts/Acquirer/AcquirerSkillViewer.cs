using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerSkillViewer : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		string mode = circles[0];

		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		BattleUnitView view = objManager.GetView(modular.modsa_unitModel);
		if (!view) return -1;
		
		BattleSkillViewer skillViewer = view.GetCurrentSkillViewer();
		SkillModel skill = skillViewer?.GetSkillModel();
		if (skill == null) return -1;
		
		switch (mode) {
			case "isParrying": return !view._currentDuelViewer ? 0 : 1;
			case "isLastCoin": {
				return skillViewer.IsLastCoin(skillViewer._curCoinLogIndex) ? 1 : 0;
			} break;
			case "coinidx": {
				return skillViewer._curCoinLogIndex;
			} break;
			case "coinamount": {
				return view._currentActionLog._systemLog.GetRemainedCoinCount();
			} break;
		}

		return -1;
	}
}