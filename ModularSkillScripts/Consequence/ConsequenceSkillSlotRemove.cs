namespace ModularSkillScripts.Consequence;

public class ConsequenceSkillSlotRemove : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SinManager sinManager_inst = Singleton<SinManager>.Instance;
		var modelList = modular.GetTargetModelList(circles[0]);
		var index = modular.GetNumFromParamString(circles[1]);
		foreach (BattleUnitModel targetModel in modelList)
		{
			var sinAction = targetModel.GetSinAction(index);
			sinManager_inst.RemoveSinAction(sinAction);
		}
	}
}