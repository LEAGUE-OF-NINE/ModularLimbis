namespace ModularSkillScripts.Consequence;

public class ConsequenceSkillSlotGive : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SinManager sinManager_inst = Singleton<SinManager>.Instance;
		var modelList = modular.GetTargetModelList(circles[0]);
		foreach (BattleUnitModel targetModel in modelList) {
			sinManager_inst.AddSinActionModelOnRoundStart(UNIT_FACTION.PLAYER, targetModel.InstanceID);
		}
	}
}