namespace ModularSkillScripts.Consequence;

public class ConsequenceExplosion : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		int times = modular.GetNumFromParamString(circles[1]);
		bool tremorCheck = circles.Length > 2;
		foreach (BattleUnitModel targetModel in modelList)
		{
			if (tremorCheck)
			{
				int tremorStack = targetModel._buffDetail.GetActivatedBuffStack(BUFF_UNIQUE_KEYWORD.Vibration, false);
				if (tremorStack < 1) continue;
			}

			for (int times_i = 0; times_i < times; times_i++)
			{
				switch (modular.abilityMode)
				{
					case 2:
						//modular.dummyPassiveAbility.AddTriggeredData_BsGaugeUp(tremorStack, targetModel.InstanceID, modular.battleTiming, true);
						//modular.dummyPassiveAbility.FirstBsGaugeUp(modular.modsa_unitModel, targetModel, tremorStack, modular.battleTiming, true);
						targetModel.VibrationExplosion(modular.battleTiming, modular.modsa_unitModel, modular.dummyPassiveAbility);
						break;
					case 1:
						//dummyCoinAbility.AddTriggeredData_BsGaugeUp(tremorStack, targetModel.InstanceID, modular.battleTiming, true);
						//dummyCoinAbility.FirstBsGaugeUp(modular.modsa_unitModel, targetModel, tremorStack, modular.battleTiming, true, modsa_selfAction, modsa_coinModel);
						targetModel.VibrationExplosion(modular.battleTiming, modular.modsa_unitModel, modular.dummyCoinAbility, modular.modsa_selfAction,
							modular.modsa_coinModel);
						break;
					default:
						//modular.dummySkillAbility.AddTriggeredData_BsGaugeUp(tremorStack, targetModel.InstanceID, modular.battleTiming, true);
						//modular.dummySkillAbility.FirstBsGaugeUp(modular.modsa_unitModel, targetModel, tremorStack, modular.battleTiming, true, modsa_selfAction);
						targetModel.VibrationExplosion(modular.battleTiming, modular.modsa_unitModel, modular.dummySkillAbility, modular.modsa_selfAction);
						break;
				}
			}
		}
	}
}