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
		
		bool plusCore = circles.Length > 2 && circles[2] == "+core";

		BattleUnitModel healsource = modular.modsa_unitModel;
		BattleActionModel actionOrNull = modular.modsa_selfAction;
		
		foreach (BattleUnitModel targetModel in modelList)
		{
			int finalAmount = amount;
			if (percentageheal) finalAmount = targetModel.MaxHp * finalAmount / 100;
			if (finalAmount < 0) {
				targetModel.TakeAbsHpDamage(null, finalAmount * -1, out _, out _, modular.battleTiming, DAMAGE_SOURCE_TYPE.SKILL);
				if (plusCore) {
					BattleUnitModel_Abnormality_Part target_part = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
					BattleUnitModel target_core = target_part?._abnormality;
					if (target_core != null && target_core != targetModel) targetModel.TakeAbsHpDamage(null, finalAmount * -1, out _, out _, modular.battleTiming, DAMAGE_SOURCE_TYPE.SKILL);
				}
			} else {
				switch (modular.abilityMode)
				{
					case 2: {
						modular.dummyPassiveAbility.HealTargetHp(healsource, actionOrNull, targetModel, finalAmount, modular.battleTiming, out _);
						if (plusCore) {
							BattleUnitModel_Abnormality_Part target_part = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
							BattleUnitModel target_core = target_part?._abnormality;
							if (target_core != null && target_core != targetModel) {
								modular.dummyPassiveAbility.HealTargetHp(healsource, actionOrNull, target_core, finalAmount, modular.battleTiming, out _);
							}
						}
					} break;
					case 1: {
						modular.dummyCoinAbility.HealTargetHp(healsource, actionOrNull, targetModel, finalAmount, modular.battleTiming, out _);
						if (plusCore) {
							BattleUnitModel_Abnormality_Part target_part = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
							BattleUnitModel target_core = target_part?._abnormality;
							if (target_core != null && target_core != targetModel) {
								modular.dummyCoinAbility.HealTargetHp(healsource, actionOrNull, target_core, finalAmount, modular.battleTiming, out _);
							}
						}
					} break;
					default: {
						modular.dummySkillAbility.HealTargetHp(healsource, actionOrNull, targetModel, finalAmount, modular.battleTiming, out _);
						if (plusCore) {
							BattleUnitModel_Abnormality_Part target_part = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
							BattleUnitModel target_core = target_part?._abnormality;
							if (target_core != null && target_core != targetModel) {
								modular.dummySkillAbility.HealTargetHp(healsource, actionOrNull, target_core, finalAmount, modular.battleTiming, out _);
							}
						}
					} break;
				}
			}
		}
	}
}