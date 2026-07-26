using Lethe.Patches;

namespace ModularSkillScripts.Consequence;

public class ConsequenceRunUpdateModsa : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		
		BattleObjectManager battleObjManager_inst = SingletonBehavior<BattleObjectManager>.Instance;
		if (!battleObjManager_inst)
		{
			MainClass.LogModular("ConsequenceRunUpdateModsa BattleObjectManager is Null");
			return;
		}
		string circle_0 = circles[0];
		foreach (BattleUnitModel unit in battleObjManager_inst.GetModelList())
		{
			//unit._passiveDetail._passivelist.
		}
	}
}