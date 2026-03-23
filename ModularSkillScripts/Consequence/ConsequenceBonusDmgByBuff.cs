using Lethe.Patches;
using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceBonusDmgByBuff : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		// if (modelList.Count < 1) return; For now, we're not quitting if it's 0.

		int amount = modular.GetNumFromParamString(circles[1]);
		if (amount < 0) return;

		int dmg_type = Math.Min(int.Parse(circles[2]), 2);
		int dmg_sin = Math.Min(int.Parse(circles[3]), 11);

		ATK_BEHAVIOUR atkBehv = ATK_BEHAVIOUR.NONE;
		ATTRIBUTE_TYPE sinKind = ATTRIBUTE_TYPE.NONE;
		if (dmg_type != -1) atkBehv = (ATK_BEHAVIOUR)dmg_type;
		if (dmg_sin != -1) sinKind = (ATTRIBUTE_TYPE)dmg_sin;

		BATTLE_EVENT_TIMING timing = modular.battleTiming;
		string circle_4 = circles[4];
		if (circle_4 == "Magic")
		{
			var attacker = modular.GetTargetModel(circles[5]);
			if (!Enum.TryParse(circles[6], true, out DAMAGE_SOURCE_TYPE dmgSourceType)) dmgSourceType = DAMAGE_SOURCE_TYPE.BUFF;
			BattleActionModel action = null;
			CoinModel coin = null;
			if (circles[7] == "UseAction")
			{
				action = modular.modsa_selfAction;
				coin = modular.modsa_coinModel;
			}
			BUFF_UNIQUE_KEYWORD buffUniqueKeyword = CustomBuffs.ParseBuffUniqueKeyword(circles[8]);

			foreach (BattleUnitModel targetModel in modelList)
			{
				if (dmg_type == -1 && dmg_sin == -1)
					targetModel.TakeAbsHpDamage(attacker, amount, out _, out _,
						timing,
						dmgSourceType,
						action, coin,
						null,
						buffUniqueKeyword,
						false);
				else if (dmg_type == -1 && dmg_sin != -1)
					targetModel.TakeHpDamageAppliedAttributeResist(amount, out _,
						sinKind, timing, dmgSourceType, attacker, action, coin, buffUniqueKeyword);
				else if (dmg_type != -1 && dmg_sin == -1)
					targetModel.TakeHpDamageAppliedAtkResist(amount, out _,
						atkBehv, timing, dmgSourceType, attacker, action, coin, buffUniqueKeyword);
				else targetModel.TakeHpDamageAppliedAttributeAndAtkResist(amount, out _,
						sinKind, atkBehv, timing, dmgSourceType, attacker, action, coin, buffUniqueKeyword);
			}
		}
		else if (circle_4 == "BuffGiveDmg")
		{
			BuffModel buf = modular.modsa_buffModel;
			if (buf == null) return;
			if (modular.modsa_buffModel._abilityList.Count < 1) return;
			BuffAbility buffAbility = modular.modsa_buffModel._abilityList[0];
			if (buffAbility == null) return;
			
			SinManager sinManager_inst = Singleton<SinManager>.Instance;
			BattleObjectManager battleObjectManager = sinManager_inst._battleObjectManager;

			int owner_id = buf.GetOwnerInstanceID();
			BattleUnitModel owner_unit = null;
			foreach (BattleUnitModel unit in battleObjectManager.GetAliveList(true))
			{
				if (unit.InstanceID == owner_id)
				{
					owner_unit = unit;
					break;
				}
			}
			
			if (!Enum.TryParse(circles[5], true, out DAMAGE_SOURCE_TYPE dmgSourceType)) dmgSourceType = DAMAGE_SOURCE_TYPE.BUFF;

			if (owner_unit != null)
			{
				foreach (BattleUnitModel targetModel in modelList)
				{
					if (dmg_sin == -1)
						buffAbility.GiveAbsHpDamage_Buff(owner_unit, targetModel, amount, out _, out _,
							modular.battleTiming, dmgSourceType, buf.GetBuffInfo());
					else
						buffAbility.GiveHpDamageAppliedAttributeResist_Buff(owner_unit, targetModel, amount,
							sinKind, timing, dmgSourceType, buf.GetBuffInfo(), out _);
				}
			}
			
		}
		else if (circle_4 == "BuffTakeDmg")
		{
			BuffModel buf = modular.modsa_buffModel;
			if (buf == null) return;
			if (modular.modsa_buffModel._abilityList.Count < 1) return;
			BuffAbility buffAbility = modular.modsa_buffModel._abilityList[0];
			if (buffAbility == null) return;
			
			SinManager sinManager_inst = Singleton<SinManager>.Instance;
			BattleObjectManager battleObjectManager = sinManager_inst._battleObjectManager;

			int owner_id = buf.GetOwnerInstanceID();
			BattleUnitModel owner_unit = null;
			foreach (BattleUnitModel unit in battleObjectManager.GetAliveList(true))
			{
				if (unit.InstanceID == owner_id)
				{
					owner_unit = unit;
					break;
				}
			}

			if (owner_unit != null)
			{
				var attacker = modular.GetTargetModel(circles[5]);
				if (!Enum.TryParse(circles[6], true, out DAMAGE_SOURCE_TYPE dmgSourceType)) dmgSourceType = DAMAGE_SOURCE_TYPE.BUFF;
				bool isByStack = circles[7] == "ByStack";
				
				if (dmg_type == -1 && dmg_sin == -1)
					buffAbility.TakeAbsHpDamage_Buff(owner_unit, amount,
						modular.battleTiming, dmgSourceType, buf.GetBuffInfo(), isByStack, attacker);
				else if (dmg_sin != -1)
					buffAbility.TakeHpDamageAppliedAttributeResist_Buff(owner_unit, amount, sinKind, timing,
						dmgSourceType, buf.GetBuffInfo(), isByStack, attacker);
				else if (dmg_type != -1)
					buffAbility.TakeHpDamageAppliedAtkResist_Buff(owner_unit, amount, atkBehv, timing,
						dmgSourceType, buf.GetBuffInfo(), isByStack, attacker);
			}
		}
		
	}
}