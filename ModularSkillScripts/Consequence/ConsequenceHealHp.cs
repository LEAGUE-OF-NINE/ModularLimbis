using System.Linq;

namespace ModularSkillScripts.Consequence;

public class ConsequenceHealHp : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return;

		string circle_1 = circles[1];
		bool percentageheal = circle_1.Last() == '%';
		if (percentageheal) circle_1 = circle_1.Remove(circle_1.Length - 1);
		int amount = modular.GetNumFromParamString(circle_1);

		foreach (BattleUnitModel targetModel in modelList)
		{
			int finalAmount = amount;
			if (percentageheal) finalAmount = targetModel.MaxHp * finalAmount / 100;
			if (finalAmount < 0)
				targetModel.TakeAbsHpDamage(null, finalAmount * -1, out _, out _, modular.battleTiming, DAMAGE_SOURCE_TYPE.SKILL);
			else
			{
				switch (modular.abilityMode)
				{
					case 2:
						modular.dummyPassiveAbility.AddTriggeredData_HpHeal(finalAmount, targetModel.InstanceID, modular.battleTiming);
						//dummyPassiveAbility.HealTargetHp(modsa_unitModel, modsa_selfAction, targetModel, finalAmount, modular.battleTiming, finalAmount);
						targetModel.TryRecoverHp(modular.modsa_unitModel, null, finalAmount, ABILITY_SOURCE_TYPE.PASSIVE, modular.battleTiming,
							out _);
						break;
					case 1:
						modular.dummyCoinAbility.AddTriggeredData_HpHeal(finalAmount, targetModel.InstanceID, modular.battleTiming);
						//dummyCoinAbility.HealTargetHp(modular.modsa_unitModel, modsa_selfAction, targetModel, finalAmount, modular.battleTiming, finalAmount);
						targetModel.TryRecoverHp(modular.modsa_unitModel, null, finalAmount, ABILITY_SOURCE_TYPE.SKILL, modular.battleTiming, out _);
						break;
					default:
						modular.dummySkillAbility.AddTriggeredData_HpHeal(finalAmount, targetModel.InstanceID, modular.battleTiming);
						//dummySkillAbility.HealTargetHp(modular.modsa_unitModel, modsa_selfAction, targetModel, finalAmount, modular.battleTiming, finalAmount);
						targetModel.TryRecoverHp(modular.modsa_unitModel, null, finalAmount, ABILITY_SOURCE_TYPE.SKILL, modular.battleTiming, out _);
						break;
				}
			}
		}
	}
}