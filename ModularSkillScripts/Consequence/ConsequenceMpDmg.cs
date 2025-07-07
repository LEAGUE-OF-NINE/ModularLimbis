namespace ModularSkillScripts.Consequence;

public class ConsequenceMpDmg : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int mpAmount = modular.GetNumFromParamString(circles[1]);
		if (mpAmount == 0) return;

		var modelList = modular.GetTargetModelList(circles[0]);

		foreach (BattleUnitModel targetModel in modelList)
		{
			int loseAmount = 0;
			if (mpAmount > 0)
			{
				switch (modular.abilityMode)
				{
					case 2:
						modular.dummyPassiveAbility.AddTriggeredData_MpHeal(mpAmount, targetModel.InstanceID, modular.battleTiming);
						targetModel.HealTargetMp(targetModel, mpAmount, ABILITY_SOURCE_TYPE.PASSIVE, modular.battleTiming);
						break;
					case 1:
						modular.dummyCoinAbility.AddTriggeredData_MpHeal(mpAmount, targetModel.InstanceID, modular.battleTiming);
						modular.dummyCoinAbility.HealTargetMp(modular.modsa_unitModel, targetModel, mpAmount, modular.battleTiming);
						break;
					default:
						modular.dummySkillAbility.AddTriggeredData_MpHeal(mpAmount, targetModel.InstanceID, modular.battleTiming);
						targetModel.HealTargetMp(targetModel, mpAmount, ABILITY_SOURCE_TYPE.SKILL, modular.battleTiming);
						break;
				}
			}
			else
			{
				switch (modular.abilityMode)
				{
					case 2:
						modular.dummyPassiveAbility.AddTriggeredData_MpDamage(mpAmount * -1, targetModel.InstanceID, modular.battleTiming);
						targetModel.GiveMpDamage(targetModel, mpAmount * -1, modular.battleTiming, DAMAGE_SOURCE_TYPE.PASSIVE, out loseAmount,
							modular.modsa_selfAction);
						break;
					case 1:
						modular.dummyCoinAbility.AddTriggeredData_MpDamage(mpAmount * -1, targetModel.InstanceID, modular.battleTiming);
						modular.dummyCoinAbility.GiveMpDamage(modular.modsa_unitModel, targetModel, mpAmount * -1, modular.battleTiming,
							DAMAGE_SOURCE_TYPE.SKILL, out loseAmount, modular.modsa_selfAction);
						break;
					default:
						modular.dummySkillAbility.AddTriggeredData_MpDamage(mpAmount * -1, targetModel.InstanceID, modular.battleTiming);
						targetModel.GiveMpDamage(targetModel, mpAmount * -1, modular.battleTiming, DAMAGE_SOURCE_TYPE.SKILL, out loseAmount,
							modular.modsa_selfAction);
						break;
				}
			}
		}
	}
}