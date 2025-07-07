using Lethe.Patches;

namespace ModularSkillScripts.Consequence;

public class ConsequenceRetreat : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleObjectManager battleObjectManager_inst = SingletonBehavior<BattleObjectManager>.Instance;
		if (battleObjectManager_inst == null) return;

		var modelList = modular.GetTargetModelList(circles[0]);
		BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
		//if (modelList.Count < 1) continue;
		//bool comeback = circles.Length > 1;

		foreach (BattleUnitModel targetModel in modelList)
		{
			battleObjectManager_inst.TryReservateForRetreat(targetModel, modular.modsa_unitModel, buf_keyword);
		}
	}
}