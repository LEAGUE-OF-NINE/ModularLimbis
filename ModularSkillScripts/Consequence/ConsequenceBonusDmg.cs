using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceBonusDmg : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return;

		int amount = modular.GetNumFromParamString(circles[1]);

		int dmg_type = Math.Min(int.Parse(circles[2]), 2);
		int dmg_sin = Math.Min(int.Parse(circles[3]), 11);

		ATK_BEHAVIOUR atkBehv = ATK_BEHAVIOUR.NONE;
		ATTRIBUTE_TYPE sinKind = ATTRIBUTE_TYPE.NONE;
		if (dmg_type != -1) atkBehv = (ATK_BEHAVIOUR)dmg_type;
		if (dmg_sin != -1) sinKind = (ATTRIBUTE_TYPE)dmg_sin;

		foreach (BattleUnitModel targetModel in modelList)
		{
			if (dmg_type == -1 && dmg_sin == -1)
			{
				//AbilityTriggeredData_HpDamage triggerData = new AbilityTriggeredData_HpDamage(amount, targetModel.InstanceID, battleTiming);
				//targetModel.AddTriggeredData(triggerData);
				switch (modular.abilityMode)
				{
					case 2:
						modular.dummyPassiveAbility.GiveAbsHpDamage(modular.modsa_unitModel, targetModel, amount, out _, out _, modular.battleTiming,
							DAMAGE_SOURCE_TYPE.PASSIVE);
						break;
					case 1:
						modular.dummyCoinAbility.GiveAbsHpDamage(modular.modsa_unitModel, targetModel, amount, out _, out _, modular.battleTiming,
							DAMAGE_SOURCE_TYPE.SKILL, modular.modsa_selfAction);
						break;
					default:
						modular.dummySkillAbility.GiveAbsHpDamage(modular.modsa_unitModel, targetModel, amount, out _, out _, modular.battleTiming,
							DAMAGE_SOURCE_TYPE.SKILL, modular.modsa_selfAction);
						break;
				}
			}
			else if (dmg_type == -1 && dmg_sin != -1)
			{
				//AbilityTriggeredData_HpDamage triggerData = new AbilityTriggeredData_HpDamage(amount, targetModel.InstanceID, sinKind, battleTiming);
				//targetModel.AddTriggeredData(triggerData);
				switch (modular.abilityMode)
				{
					case 2:
						modular.dummyPassiveAbility.GiveHpDamageAppliedAttributeResist(modular.modsa_unitModel, targetModel, amount, sinKind,
							modular.battleTiming, DAMAGE_SOURCE_TYPE.PASSIVE, out _);
						break;
					case 1:
						modular.dummyCoinAbility.GiveHpDamageAppliedAttributeResist(modular.modsa_unitModel, targetModel, amount, sinKind,
							modular.battleTiming, DAMAGE_SOURCE_TYPE.SKILL, out _);
						break;
					default:
						modular.dummySkillAbility.GiveHpDamageAppliedAttributeResist(modular.modsa_unitModel, targetModel, amount, sinKind,
							modular.battleTiming, DAMAGE_SOURCE_TYPE.SKILL, out _);
						break;
				}
			}
			else if (dmg_type != -1 && dmg_sin == -1)
			{
				//AbilityTriggeredData_HpDamage triggerData = new AbilityTriggeredData_HpDamage(amount, targetModel.InstanceID, atkBehv, battleTiming);
				//targetModel.AddTriggeredData(triggerData);
				switch (modular.abilityMode)
				{
					case 2:
						modular.dummyPassiveAbility.GiveHpDamageAppliedAtkResist(modular.modsa_unitModel, targetModel, amount, atkBehv,
							modular.battleTiming, DAMAGE_SOURCE_TYPE.PASSIVE, modular.modsa_selfAction);
						break;
					case 1:
						modular.dummyCoinAbility.GiveHpDamageAppliedAtkResist(modular.modsa_unitModel, targetModel, amount, atkBehv, modular.battleTiming,
							DAMAGE_SOURCE_TYPE.SKILL, modular.modsa_selfAction);
						break;
					default:
						modular.dummySkillAbility.GiveHpDamageAppliedAtkResist(modular.modsa_unitModel, targetModel, amount, atkBehv,
							modular.battleTiming, DAMAGE_SOURCE_TYPE.SKILL, modular.modsa_selfAction);
						break;
				}
			}
			else
			{
				//AbilityTriggeredData_HpDamage triggerData = new AbilityTriggeredData_HpDamage(amount, targetModel.InstanceID, sinKind, atkBehv, battleTiming);
				//targetModel.AddTriggeredData(triggerData);
				switch (modular.abilityMode)
				{
					case 2:
						modular.dummyPassiveAbility.GiveHpDamageAppliedAttributeAndAtkResist(modular.modsa_unitModel, targetModel, amount,
							sinKind, atkBehv, modular.battleTiming, DAMAGE_SOURCE_TYPE.PASSIVE, modular.modsa_selfAction);
						break;
					case 1:
						modular.dummyCoinAbility.GiveHpDamageAppliedAttributeAndAtkResist(modular.modsa_unitModel, targetModel, amount, sinKind,
							atkBehv, modular.battleTiming, DAMAGE_SOURCE_TYPE.SKILL, modular.modsa_selfAction);
						break;
					default:
						modular.dummySkillAbility.GiveHpDamageAppliedAttributeAndAtkResist(modular.modsa_unitModel, targetModel, amount, sinKind,
							atkBehv, modular.battleTiming, DAMAGE_SOURCE_TYPE.SKILL, modular.modsa_selfAction);
						break;
				}
			}
		}
	}
}