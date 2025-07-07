namespace ModularSkillScripts.Consequence;

public class ConsequenceBreakDmg : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return;
		int amount = modular.GetNumFromParamString(circles[1]);
		if (amount == 0) return;
		bool isBreakDmg = amount < 0;
		if (isBreakDmg) amount *= -1;
		int times = 1;
		if (circles.Length > 2) times = modular.GetNumFromParamString(circles[2]);

		foreach (BattleUnitModel targetModel in modelList)
		{
			for (int times_i = 0; times_i < times; times_i++)
			{
				if (isBreakDmg)
				{
					//AbilityTriggeredData_BsGaugeDown triggerData = new AbilityTriggeredData_BsGaugeDown(amount, targetModel.InstanceID, modular.battleTiming);
					switch (modular.abilityMode)
					{
						case 2:
							modular.dummyPassiveAbility.FirstBsGaugeDown(modular.modsa_unitModel, targetModel, amount, modular.battleTiming);
							break;
						case 1:
							modular.dummyCoinAbility.FirstBsGaugeDown(modular.modsa_unitModel, targetModel, amount, modular.battleTiming);
							break;
						default:
							modular.dummySkillAbility.FirstBsGaugeDown(modular.modsa_unitModel, targetModel, amount, modular.battleTiming);
							break;
					}
				}
				else
				{
					//AbilityTriggeredData_BsGaugeUp triggerData = new AbilityTriggeredData_BsGaugeUp(amount, targetModel.InstanceID, modular.battleTiming);
					switch (modular.abilityMode)
					{
						case 2:
							modular.dummyPassiveAbility.FirstBsGaugeUp(modular.modsa_unitModel, targetModel, amount, modular.battleTiming, false);
							break;
						case 1:
							modular.dummyCoinAbility.FirstBsGaugeUp(modular.modsa_unitModel, targetModel, amount, modular.battleTiming, false,
								modular.modsa_selfAction);
							break;
						default:
							modular.dummySkillAbility.FirstBsGaugeUp(modular.modsa_unitModel, targetModel, amount, modular.battleTiming, false,
								modular.modsa_selfAction);
							break;
					}
					//targetModel.AddTriggeredData(triggerData);
					//modular.dummySkillAbility.AddTriggeredData_BsGaugeUp(amount, targetModel.InstanceID, modular.battleTiming, false);
				}
			}
		}
	}
}