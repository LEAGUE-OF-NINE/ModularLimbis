using Il2CppSystem.Collections.Generic;
using Lethe.EnumInjections;
using Lua;

namespace ModularSkillScripts.LuaIntegration;

[LuaObject]
public partial class ModsaInterface
{
	public ModularSA ref_modsa = null;
	public ModsaInterface(ModularSA x) {
		ref_modsa = x;
	}
	
	[LuaMember("FetchSingleUnit")]
	public UnitModel FetchSingleUnit(string find_instruction)
	{
		BattleUnitModel unit = ref_modsa.GetTargetModel(find_instruction);
		if (unit == null) return null;
		return new UnitModel(unit);
	}
	
	[LuaMember("FetchUnitList")]
	public LuaTable FetchUnitList(string find_instruction)
	{
		List<BattleUnitModel> unit_list = ref_modsa.GetTargetModelList(find_instruction);
		int count = unit_list.Count;
		LuaTable luaTable = new LuaTable(count, 0);
		for (int i = 0; i < count; i++) {
			luaTable[i + 1] = new UnitModel(unit_list[i]);
		}
		return luaTable;
	}
	
	[LuaMember("Buf")]
	public void Buf(UnitModel targetModel, UnitModel giver, string keyword_s, int stack, int turn, LuaTable activeRound_list, BufPacket bufPacket) {
		BattleUnitModel targetModel_unit = targetModel?.ref_unitModel;
		if (targetModel == null) return;
		
		BattleUnitModel giver_unit = giver?.ref_unitModel;
		BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(keyword_s);
		BATTLE_EVENT_TIMING timing = ref_modsa.battleTiming;

		BattleActionModel action = ref_modsa.modsa_selfAction;
		switch (bufPacket.mode_action) {
			case "nullaction": action = null; break;
			case "oppoaction": action = ref_modsa.modsa_oppoAction; break;
		}

		string mode_infliction = bufPacket.mode_infliction;
		string mode_source = bufPacket.mode_source;
		
		for (int i = 1; i < activeRound_list.ArrayLength+1; i++) {
			int activeRound = activeRound_list[i].Read<int>();

			switch (mode_infliction) {
				case "give": {
					switch (mode_source) {
						case "skill": {
							ref_modsa.dummySkillAbility.GiveBuff_Giver(targetModel_unit, buf_keyword, giver_unit, stack, turn, activeRound, timing, action);
						} break;
						case "coin": {
							ref_modsa.dummyCoinAbility.GiveBuff_Giver(targetModel_unit, buf_keyword, giver_unit, stack, turn, activeRound, timing, action);
						} break;
						case "passive": {
							ref_modsa.dummyPassiveAbility.GiveBuff_Giver(targetModel_unit, buf_keyword, giver_unit, stack, turn, activeRound, timing, action);
						} break;
						case "buf": {
							ref_modsa.modsa_buffModel._abilityList[0].GiveBuff_Giver(targetModel_unit, buf_keyword, giver_unit, stack, turn, activeRound, timing, action);
						} break;
						default: {
							targetModel_unit.AddBuff_Giver(buf_keyword, stack, giver_unit, timing, turn, activeRound,
								ABILITY_SOURCE_TYPE.UNIT, action,
								out _, out _, out _, out _);
						} break;
					}
				} break;
				case "remove": {
					switch (mode_source) {
						case "buf": {
							BuffModel buf = ref_modsa.modsa_buffModel;
							BuffInfo bufInfo = buf?.GetBuffInfo(activeRound);
							if (bufInfo == null) break;
							buf._abilityList[0].LoseBuff_ByBuff(targetModel_unit, giver_unit,
								stack, turn, timing, bufInfo);
						} break;
						default: {
							if (stack > 0) targetModel_unit.LoseBuffStack(buf_keyword, stack, timing, activeRound);
							if (turn > 0) targetModel_unit.LoseBuffTurn(buf_keyword, stack, timing);
						} break;
					}
				} break;
				case "spend": {
					switch (mode_source) {
						case "skill": {
							if (stack > 0) ref_modsa.dummySkillAbility.UseBuffStack(targetModel_unit, action, buf_keyword, timing, stack);
							if (turn > 0) ref_modsa.dummySkillAbility.UseBuffTurn(targetModel_unit, action, buf_keyword, timing, turn);
						} break;
						case "coin": {
							if (stack > 0) ref_modsa.dummyCoinAbility.UseBuffStack(targetModel_unit, action, buf_keyword, timing, stack);
							if (turn > 0) ref_modsa.dummyCoinAbility.UseBuffTurn(targetModel_unit, action, buf_keyword, timing, turn);
						} break;
						case "passive": {
							if (stack > 0) targetModel_unit.LoseBuffStack(buf_keyword, stack, timing, activeRound);
							if (turn > 0) ref_modsa.dummyPassiveAbility.UseBuffTurnByPassive(targetModel_unit, buf_keyword, turn, timing);
						} break;
						case "buf": {
							BuffModel buf = ref_modsa.modsa_buffModel;
							BuffInfo bufInfo = buf?.GetBuffInfo(activeRound);
							if (bufInfo == null) break;
							buf._abilityList[0].LoseBuff_ByBuff(targetModel_unit, giver_unit,
								stack, turn, timing, bufInfo);
						} break;
						default: {
							if (stack > 0) targetModel_unit.UseBuffStack(buf_keyword, stack, timing);
							if (turn > 0) targetModel_unit.UseBuffTurn(buf_keyword, turn, timing);
						} break;
					}
				} break;
			}
			
			
		}
	}


	[LuaMember("CreateBufPacket")]
	public static BufPacket CreateBufPacket() {
		return new BufPacket();
	}
}

[LuaObject]
public partial class BufPacket
{
	[LuaMember("mode_action")]
	public string mode_action = "normal";
	
	[LuaMember("mode_infliction")]
	public string mode_infliction = "give";
	
	[LuaMember("mode_source")]
	public string mode_source = "unit";
}

[LuaObject]
public partial class UnitModel
{
	public BattleUnitModel ref_unitModel = null;
	public UnitModel(BattleUnitModel x) {
		ref_unitModel = x;
	}
	
	[LuaMember("IsNull")]
	public bool IsNull() {
		return ref_unitModel == null;
	}
	
	[LuaMember("WhatAmI")]
	public int WhatAmI() {
		if (!ref_unitModel.IsAbnormalityOrPart) return 0;
		BattleUnitModel_Abnormality_Part part = ref_unitModel.TryCast<BattleUnitModel_Abnormality_Part>();
		return part == null ? 1 : 2;
	}
	
	[LuaMember("HP")]
	public int HPCheck() {
		return ref_unitModel.Hp;
	}
	
	[LuaMember("MaxHP")]
	public int MaxHPCheck() {
		return ref_unitModel.MaxHp;
	}
	
	[LuaMember("Speed")]
	public int Speed() {
		return ref_unitModel.GetIntegerOfOriginSpeed();
	}
	
	
}