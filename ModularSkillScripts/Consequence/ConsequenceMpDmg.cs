namespace ModularSkillScripts.Consequence;

public class ConsequenceMpDmg : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int mpAmount = modular.GetNumFromParamString(circles[1]);
		if (mpAmount == 0) return;

		var modelList = modular.GetTargetModelList(circles[0]);
		BattleUnitModel sourceUnit = modular.modsa_unitModel;
		if (circles.Length > 2) sourceUnit = modular.GetTargetModel(circles[2]);
		
		foreach (BattleUnitModel targetModel in modelList)
		{
			int loseAmount = 0;
			if (mpAmount > 0)
			{
				switch (modular.abilityMode)
				{
					case 2:
						modular.dummyPassiveAbility.AddTriggeredData_MpHeal(mpAmount, targetModel.InstanceID, modular.battleTiming);
						sourceUnit.HealTargetMp(targetModel, mpAmount, ABILITY_SOURCE_TYPE.PASSIVE, modular.battleTiming);
						break;
					case 1:
						modular.dummyCoinAbility.HealTargetMp(modular.modsa_unitModel, targetModel, mpAmount, modular.battleTiming);
						break;
					default:
						modular.dummySkillAbility.AddTriggeredData_MpHeal(mpAmount, targetModel.InstanceID, modular.battleTiming);
						sourceUnit.HealTargetMp(targetModel, mpAmount, ABILITY_SOURCE_TYPE.SKILL, modular.battleTiming);
						break;
				}
			}
			else
			{
				switch (modular.abilityMode)
				{
					case 2:
						modular.dummyPassiveAbility.GiveMpDamage(sourceUnit, targetModel, mpAmount * -1, modular.battleTiming,
							DAMAGE_SOURCE_TYPE.SKILL, out loseAmount, modular.modsa_selfAction);
						break;
					case 1:
						modular.dummyCoinAbility.GiveMpDamage(sourceUnit, targetModel, mpAmount * -1, modular.battleTiming,
							DAMAGE_SOURCE_TYPE.SKILL, out loseAmount, modular.modsa_selfAction);
						break;
					default:
						modular.dummySkillAbility.GiveMpDamage(sourceUnit, targetModel, mpAmount * -1, modular.battleTiming,
							DAMAGE_SOURCE_TYPE.SKILL, out loseAmount, modular.modsa_selfAction);
						break;
				}
			}
		}
	}
}