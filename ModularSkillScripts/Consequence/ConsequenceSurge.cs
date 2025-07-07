namespace ModularSkillScripts.Consequence;

public class ConsequenceSurge : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		int amount = modular.GetNumFromParamString(circles[1]);
		foreach (BattleUnitModel targetModel in modelList)
		{
			switch (modular.abilityMode)
			{
				case 2:
					modular.dummyPassiveAbility.AddTriggeredData_SinkingSurge(amount, targetModel.InstanceID, modular.battleTiming);
					targetModel.SinkingSurge(amount, modular.battleTiming, modular.modsa_unitModel, modular.dummyPassiveAbility, modular.modsa_selfAction,
						modular.modsa_coinModel);
					break;
				case 1:
					modular.dummyCoinAbility.AddTriggeredData_SinkingSurge(amount, targetModel.InstanceID, modular.battleTiming);
					modular.dummyCoinAbility.SinkingSurge(modular.modsa_unitModel, targetModel, modular.battleTiming, modular.modsa_selfAction, modular.modsa_coinModel);
					break;
				default:
					modular.dummySkillAbility.AddTriggeredData_SinkingSurge(amount, targetModel.InstanceID, modular.battleTiming);
					targetModel.SinkingSurge(amount, modular.battleTiming, modular.modsa_unitModel, modular.dummySkillAbility, modular.modsa_selfAction,
						modular.modsa_coinModel);
					break;
			}
		}
	}
}