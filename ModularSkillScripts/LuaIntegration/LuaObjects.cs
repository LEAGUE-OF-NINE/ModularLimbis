#nullable enable
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Lethe.EnumInjections;
using Lua;
using ModularSkillScripts.Consequence;
using ModularSkillScripts.Patches;
using SD;
using Unity.Collections;
using UnityEngine;
using IntPtr = System.IntPtr;

namespace ModularSkillScripts.LuaIntegration;

[LuaObject]
public partial class LuaModularUtil
{
	[LuaMember("GetterRandom")]
	public static int GetterRandom(int minRoll, int maxRoll) {
		return MainClass.rng.Next(minRoll, maxRoll + 1);
	}
}

[LuaObject]
public partial class ModsaInterface
{
	public ModularSA ref_modsa = null;
	public ModsaInterface(ModularSA x) {
		ref_modsa = x;
	}
	
	[LuaMember("GetVALUE")]
	public int GetVALUE(int idx) {
		return ref_modsa.valueList[idx];
	}
	[LuaMember("SetVALUE")]
	public void SetVALUE(int idx, int value) {
		ref_modsa.valueList[idx] = value;
	}
	
	[LuaMember("GetUnitData")]
	public int GetUnitData(UnitModel unit, int dataID) {
		return SkillScriptInitPatch.GetModUnitData(unit.ref_unitModel.Pointer.ToInt64(), dataID);
	}
	[LuaMember("SetUnitData")]
	public void SetUnitData(UnitModel unit, int dataID, int value) {
		SkillScriptInitPatch.SetModUnitData(unit.ref_unitModel.Pointer.ToInt64(), dataID, value);
	}
	
	[LuaMember("ADDERSet")]
	public void ADDERSet(string adder_type, int amount_set) {
		switch (adder_type)
		{
			case "base": ref_modsa.skillPowerAdder = amount_set; break;
			case "clash": ref_modsa.parryingResultAdder = amount_set; break;
			case "scale": ref_modsa.coinScaleAdder = amount_set; break;
			case "final": ref_modsa.skillPowerResultAdder = amount_set; break;
			case "dmgmult": ref_modsa.atkMultAdder = amount_set; break;
			case "dmgadd": ref_modsa.atkDmgAdder = amount_set; break;
			case "critmult": ref_modsa.critRatioAdder = amount_set; break;
			case "critchance": ref_modsa.critAdder = amount_set; break;
			case "headschance": ref_modsa.headsChanceAdder = amount_set; break;
		}
	}
	
	[LuaMember("FetchModsaUnitModel")]
	public UnitModel FetchModsaUnitModel() {
		return new UnitModel(ref_modsa.modsa_unitModel);
	}
	
	[LuaMember("FetchModsaBufModel")]
	public BufModel FetchModsaBufModel() {
		return new BufModel(ref_modsa.modsa_buffModel);
	}
	
	[LuaMember("FetchModsaSkillModel")]
	public LuaSkillModel FetchModsaSkillModel() {
		return new LuaSkillModel(ref_modsa.modsa_skillModel);
	}
	
	[LuaMember("FetchModsaActionModel")]
	public ActionModel FetchModsaActionModel() => new(ref_modsa.modsa_selfAction);
	
	[LuaMember("FetchModsaActionModelOpponent")]
	public ActionModel FetchModsaActionModelOpponent() => new(ref_modsa.modsa_oppoAction);
	
	[LuaMember("FetchModsaCoinModel")]
	public LuaCoinModel FetchModsaCoinModel() {
		return new LuaCoinModel(ref_modsa.modsa_coinModel);
	}
	
	[LuaMember("FetchSingleUnit")]
	public UnitModel FetchSingleUnit(string find_instruction) => new(ref_modsa.GetTargetModel(find_instruction));
	
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
	public void Buf(UnitModel targetModel, UnitModel giver, BufKeyword lua_bufKeyword, int stack, int turn, LuaTable activeRound_list, BufPacket bufPacket) {
		BattleUnitModel targetModel_unit = targetModel?.ref_unitModel;
		if (targetModel == null) return;
		
		BattleUnitModel giver_unit = giver?.ref_unitModel;
		BUFF_UNIQUE_KEYWORD buf_keyword = lua_bufKeyword.ref_buf_keyword;
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
	[LuaMember("FetchEventTiming")]
	public EventTiming FetchEventTiming() {
		return new EventTiming {
			ref_timing = ref_modsa.battleTiming
		};
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
	public bool IsNull() => ref_unitModel == null;
	
	public static LuaTable MakeTable(List<BattleUnitModel> list) {
		int count = list.Count;
		LuaTable table = new LuaTable(count, 0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new UnitModel(list[i]);
		}
		return table;
	}
	
	public static List<BattleUnitModel> UnpackTable(LuaTable table) {
		int count = table.ArrayLength;
		List<BattleUnitModel> list = new(count);
		for (int i = 0; i < count; i++) {
			UnitModel luaval = table[i + 1].Read<UnitModel>();
			list.Add(luaval.ref_unitModel);
		}
		return list;
	}
	
	[LuaMember("WhatAmI")]
	public int WhatAmI() {
		if (!ref_unitModel.IsAbnormalityOrPart) return 0;
		BattleUnitModel_Abnormality_Part part = ref_unitModel.TryCast<BattleUnitModel_Abnormality_Part>();
		return part == null ? 1 : 2;
	}
	
	[LuaMember("UnitID")]
	public int GetUnitID() => ref_unitModel.GetUnitID();
	
	[LuaMember("InstanceID")]
	public int GetInstanceID() {
		return ref_unitModel.InstanceID;
	}
	
	[LuaMember("Faction")]
	public int GetFaction() => (int)ref_unitModel.Faction;
	
	[LuaMember("HP")]
	public int HPCheck() => ref_unitModel.Hp;
	
	[LuaMember("MaxHP")]
	public int MaxHPCheck() => ref_unitModel.MaxHp;
	
	[LuaMember("Speed")]
	public int Speed() => ref_unitModel.GetIntegerOfOriginSpeed();
	
	[LuaMember("RefreshSpeed")]
	public void RefreshSpeed() => ref_unitModel.RefreshSpeed();
	[LuaMember("RefreshState")]
	public void RefreshState() => ref_unitModel.RefreshState();
	
	[LuaMember("GetPatternIdx")]
	public int GetPatternIdx() {
		PatternScript_Abnormality pattern = ModularSA.AsAbnormalityModel(ref_unitModel)?.PatternScript;
		if (pattern == null) return -1;
		return pattern.currPatternIdx;
	}
	
	[LuaMember("SetPatternIdx")]
	public void SetPatternIdx(int pickedPattern_idx) {
		PatternScript_Abnormality pattern = ModularSA.AsAbnormalityModel(ref_unitModel)?.PatternScript;
		if (pattern == null) return;
		pattern.currPatternIdx = pickedPattern_idx;
	}
	
	[LuaMember("FetchCore")]
	public UnitModel FetchCore() {
		BattleUnitModel_Abnormality_Part part = ref_unitModel.TryCast<BattleUnitModel_Abnormality_Part>();
		return part == null ? this : new UnitModel(part._abnormality);
	}
	
	[LuaMember("FetchParts")]
	public LuaTable FetchParts() {
		LuaTable luaTable = null;
		BattleUnitModel_Abnormality_Part part = ref_unitModel.TryCast<BattleUnitModel_Abnormality_Part>();
		if (part != null) {
			List<BattleUnitModel_Abnormality_Part> partList = part._abnormality._partList;
			int count = partList.Count;
			luaTable = new (count, 0);
			for (int i = 0; i < count; i++) {
				luaTable[i + 1] = new UnitModel(partList[i]);
			}
			return luaTable;
		}
		
		BattleUnitModel_Abnormality core = ref_unitModel.TryCast<BattleUnitModel_Abnormality>();
		if (core != null) {
			List<BattleUnitModel_Abnormality_Part> partList = core._partList;
			int count = partList.Count;
			luaTable = new (count, 0);
			for (int i = 0; i < count; i++) {
				luaTable[i + 1] = new UnitModel(partList[i]);
			}
			return luaTable;
		}

		luaTable = new(1, 0);
		luaTable[1] = this;
		return luaTable;
	}
	
	[LuaMember("FetchBufList")]
	public LuaTable FetchBufList() {
		List<BuffModel> buf_list = ref_unitModel.GetActivatedBuffModels();
		int count = buf_list.Count;
		
		LuaTable luaTable = new (count, 0);
		for (int i = 0; i < count; i++) {
			luaTable[i + 1] = new BufModel(buf_list[i]);
		}
		
		return luaTable;
	}
	
	[LuaMember("BufCheckStack")]
	public int BufCheckStack(BufKeyword bufKeyword, bool main_only) {
		return ref_unitModel.GetActivatedBuffStack(bufKeyword.ref_buf_keyword, main_only);
	}
	
	[LuaMember("BufCheckStackByCategory")]
	public int BufCheckStackByCategory(BufCategory bufCategory, LuaTable exception_table)
	{
		List<BUFF_UNIQUE_KEYWORD> exception_list = new();
		for (int i = 1; i < exception_table.ArrayLength + 1; i++) {
			BufKeyword exception = exception_table[i].Read<BufKeyword>();
			exception_list.Add(exception.ref_buf_keyword);
		}
		
		Il2CppStructArray<BUFF_UNIQUE_KEYWORD> exception_struct = new(exception_list.ToArray());
		return ref_unitModel.GetActivatedBuffStack(bufCategory.ref_buf_category, exception_struct);
	}
	
	[LuaMember("BufCheckTurn")]
	public int BufCheckTurn(BufKeyword bufKeyword, bool main_only) {
		return ref_unitModel.GetActivatedBuffTurn(bufKeyword.ref_buf_keyword, main_only);
	}
	
	[LuaMember("BufCheckTurnByCategory")]
	public int BufCheckTurnByCategory(BufCategory bufCategory, LuaTable exception_table)
	{
		List<BUFF_UNIQUE_KEYWORD> exception_list = new();
		for (int i = 1; i < exception_table.ArrayLength + 1; i++) {
			BufKeyword exception = exception_table[i].Read<BufKeyword>();
			exception_list.Add(exception.ref_buf_keyword);
		}
		
		Il2CppStructArray<BUFF_UNIQUE_KEYWORD> exception_struct = new(exception_list.ToArray());
		return ref_unitModel.GetActivatedBuffTurn(bufCategory.ref_buf_category, exception_struct);
	}
	
	[LuaMember("GiveAbsHpDamage")]
	public LuaTable GiveAbsHpDamage(UnitModel target, int value, EventTiming timing, DamageSourceType dmgSrcType, ActionModel? actionOrNull = null, LuaCoinModel? coinOrNull = null)
	{
		int dmg_return = ref_unitModel.GiveAbsHpDamage(target.ref_unitModel, value, out int hpDamage, out int damageToShield, timing.ref_timing, dmgSrcType.ref_dmgSrcType,
			actionOrNull?.ref_action, coinOrNull?.ref_coin);

		LuaTable table = new(3, 0)
		{
			[1] = dmg_return,
			[2] = hpDamage,
			[3] = damageToShield
		};
		return table;
	}
	
	[LuaMember("TakeAbsHpDamage")]
	public LuaTable TakeAbsHpDamage(UnitModel? attackerOrNull, int value, EventTiming timing, DamageSourceType dmgSrcType,
		ActionModel? actionOrNull = null, LuaCoinModel? coinOrNull = null,
		LuaTable? relatedUnits_table = null, BufKeyword? bufKeyword = null,
		bool? deactivePassedBreakSection = false, bool? ignoreShield = false, bool? ignorePassedBreakSection = false)
	{
		List<BattleUnitModel>? relatedUnits_list = null;
		if (relatedUnits_table != null) relatedUnits_list = UnpackTable(relatedUnits_table);
		
		int dmg_return = ref_unitModel.TakeAbsHpDamage(attackerOrNull?.ref_unitModel, value, out int hpDamage, out int damageToShield, timing.ref_timing, dmgSrcType.ref_dmgSrcType,
			actionOrNull?.ref_action, coinOrNull?.ref_coin,
			relatedUnits_list, bufKeyword?.ref_buf_keyword ?? BUFF_UNIQUE_KEYWORD.None,
			deactivePassedBreakSection ?? false, ignoreShield ?? false, ignorePassedBreakSection ?? false);

		LuaTable table = new(3, 0)
		{
			[1] = dmg_return,
			[2] = hpDamage,
			[3] = damageToShield
		};
		return table;
	}
	
	[LuaMember("CutInAction")]
	public void CutInAction(ActionModel action, LuaSkillTargetType overwrite_skillTargetType, string lastOrFirst) {
		BattleActionModelManager.CUT_IN_ACTION_TYPE cutInType = lastOrFirst == "Last" ? BattleActionModelManager.CUT_IN_ACTION_TYPE.LAST_APPEND : BattleActionModelManager
			.CUT_IN_ACTION_TYPE.ZERO_INDEX;
		ref_unitModel.CutInAction(action.ref_action, overwrite_skillTargetType.ref_enum, cutInType);
	}
	
	[LuaMember("CutInActionPrevOtherAction")]
	public void CutInActionPrevOtherAction(ActionModel action, ActionModel otherAction, LuaSkillTargetType overwrite_skillTargetType) {
		ref_unitModel.CutInActionPrevOtherAction(action.ref_action, otherAction.ref_action, overwrite_skillTargetType.ref_enum);
	}
	
	[LuaMember("CutInDefenseActionForcely")]
	public void CutInDefenseActionForcely(ActionModel action, bool first) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		ref_unitModel.CutInDefenseActionForcely(action.ref_action, first);
	}
	
	[LuaMember("ReservateToCutInAction")]
	public void ReservateToCutInAction(ReserveCutInActionType reserveType, ActionModel action, bool removeDuel) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		ref_unitModel.ReservateToCutInAction(action.ref_action, reserveType.ref_enum, removeDuel);
	}
	
	[LuaMember("FetchDataModel")]
	public LuaUnitDataModel FetchDataModel() => new(ref_unitModel._unitDataModel);
	
	[LuaMember("FetchResistDetail")]
	public LuaResistDetail FetchResistDetail() => new(ref_unitModel._resistDetail);
	
	[LuaMember("FetchActionModelList")]
	public LuaTable FetchActionModelList(bool sorted) {
		List<BattleActionModel> action_list = sorted ? ref_unitModel.GetSortedActionList() : ref_unitModel._actionList;
		int count = action_list.Count;
		LuaTable table = new(count,0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new ActionModel(action_list[i]);
		}
		return table;
	}
	
	[LuaMember("FetchSinActionList")]
	public LuaTable FetchSinActionList(bool permanent_only) {
		List<SinActionModel> sinAction_list = ref_unitModel.GetSinActionList();
		int count = permanent_only ? ref_unitModel.GetPermanentSinActionListCount() : sinAction_list.Count;
		LuaTable table = new(count,0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new LuaSinActionModel(sinAction_list[i]);
		}
		return table;
	}
	
	[LuaMember("AddSinAction")]
	public void AddSinAction(LuaSinActionModel sinAction) => ref_unitModel.AddSinAction(sinAction.ref_sinAction);
	
	[LuaMember("AddNewSinActionModel")]
	public LuaSinActionModel AddNewSinActionModel() => new(ref_unitModel.AddNewSinActionModel());
	
	[LuaMember("AddNewSinActionModelWithSkillID")]
	public ActionModel AddNewSinActionModelWithSkillID(int skillID) => new(ref_unitModel.AddNewSinActionModelWithSkill(skillID));
	
	[LuaMember("FetchPassives")]
	public LuaTable FetchPassives(bool include_ego) {
		List<PassiveModel> passive_list = ref_unitModel._passiveDetail._passivelist;
		int count = passive_list.Count;
		LuaTable table = new(count,0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new LuaPassiveModel(passive_list[i]);
		}

		if (include_ego) {
			List<EgoPassiveModel> egoPassive_list = ref_unitModel._passiveDetail._egoPassiveList;
			count = passive_list.Count;
			for (int i = 0; i < count; i++) {
				table[i + 1] = new LuaPassiveModel(egoPassive_list[i]);
			}
		}
		
		return table;
	}
	
	[LuaMember("AddPassive")]
	public void AddPassive(int passiveID, bool allow_duplicate) {
		if (!allow_duplicate && ref_unitModel.HasPassive(passiveID)) return;
		ref_unitModel.AddPassive(passiveID);
	}

	[LuaMember("HasPassive")]
	public bool HasPassive(int passiveID) => ref_unitModel.HasPassive(passiveID);
}

[LuaObject]
public partial class LuaUnitDataModel
{
	public UnitDataModel ref_data = null;
	public LuaUnitDataModel(UnitDataModel x) => ref_data = x;
	
	[LuaMember("IsNull")]
	public bool IsNull() => ref_data == null;
	
	[LuaMember("FetchStatic")]
	public LuaUnitStatic FetchStatic() => new(ref_data._classInfo);

	[LuaMember("FetchSkillModel")]
	public LuaSkillModel FetchSkillModel(int skillID) => new(ref_data.GetSkillModel(skillID));

	[LuaMember("FetchAllSkillID")]
	public LuaTable FetchAllSkillID()
	{
		List<int> list = ref_data.GetSkillIds();
		int count = list.Count;
		LuaTable table = new(count,0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = list[i];
		}
		return table;
	}
}

[LuaObject]
public partial class LuaUnitStatic
{
	public UnitStaticData ref_data = null;
	public LuaUnitStatic(UnitStaticData x) => ref_data = x;
	
	[LuaMember("IsNull")]
	public bool IsNull() => ref_data == null;
	
}

[LuaObject]
public partial class BufModel
{
	public BuffModel ref_buffModel = null;
	public BufModel(BuffModel x) => ref_buffModel = x;
	
	[LuaMember("IsNull")]
	public bool IsNull() => ref_buffModel == null;
	
	[LuaMember("FetchAllModsa")]
	public LuaTable FetchAllModsa(bool fast_no_copy) {
		List<ModularSA> modsa_list = null;
		if (fast_no_copy) modsa_list = SkillScriptInitPatch.GetAllModbaFromBuffModel_Fast(ref_buffModel);
		else modsa_list = SkillScriptInitPatch.GetAllModbaFromBuffModel(ref_buffModel);
		
		int count = modsa_list.Count;
		LuaTable table = new(count,0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new ModsaInterface(modsa_list[i]);
		}
		return table;
	}
	
	[LuaMember("GetStack")]
	public int GetStack(int activeRound) {
		return ref_buffModel.GetStack(activeRound);
	}
	
	[LuaMember("GetTurn")]
	public int GetTurn(int activeRound) {
		return ref_buffModel.GetTurn(activeRound);
	}
	
	[LuaMember("CreateMainBufKeyword")]
	public BufKeyword CreateMainBufKeyword() {
		return new BufKeyword {
			ref_buf_keyword = ref_buffModel._buffKeyword._mainUniqueKeyword
		};
	}
	
	[LuaMember("CreateSubBufKeywordList")]
	public LuaTable CreateSubBufKeywordList() {
		List<BUFF_UNIQUE_KEYWORD> keyword_list = ref_buffModel.GetSubKeywordList();
		int count = keyword_list.Count;
		LuaTable luaTable = new (count, 0);
		for (int i = 0; i < count; i++) {
			luaTable[i+1] = new BufKeyword {
				ref_buf_keyword = keyword_list[i]
			};
		}
		return luaTable;
	}
	
	[LuaMember("CreateCategoryKeywordList")]
	public LuaTable CreateCategoryKeywordList() {
		List<BUFF_CATEGORY_KEYWORD> keyword_list = ref_buffModel.GetBuffCategoryKeywords();
		int count = keyword_list.Count;
		LuaTable luaTable = new (count, 0);
		for (int i = 0; i < count; i++) {
			luaTable[i+1] = new BufCategory {
				ref_buf_category = keyword_list[i]
			};
		}
		return luaTable;
	}
	
	[LuaMember("TriggerBuf")]
	public void TriggerBuf(UnitModel unit, UnitModel attacker, int loseStack, int loseTurn, EventTiming timing, LuaAttributeType sinType, int ov_amount)
	{
		BattleUnitModel van_unit = unit.ref_unitModel;
		BattleUnitModel van_attacker = attacker.ref_unitModel;
		Il2CppSystem.Nullable<int> nulint = new((int)999);
		BATTLE_EVENT_TIMING van_timing = timing.ref_timing;
		ATTRIBUTE_TYPE van_sinType = sinType.ref_enum;
		
		int actevent = MainClass.timingDict["BufActivate"];
		
		ref_buffModel.ForceToActivateBuffEffect(van_unit, van_attacker, loseStack, loseTurn, nulint, van_timing, van_sinType, ov_amount);
		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModbaFromBuffModel(ref_buffModel)) {
			if (modsa.activationTiming != actevent) continue;
			modsa.modsa_buffModel = ref_buffModel;
			modsa.modsa_killerModel = van_attacker;
			modsa.valueList[9] = ov_amount;
			modsa.valueList[8] = sinType.KeywordToInt();
			modsa.Enact(van_unit, null, null, null, actevent, van_timing);
		}
	}
}

[LuaObject]
public partial class BufKeyword
{
	public BUFF_UNIQUE_KEYWORD ref_buf_keyword = BUFF_UNIQUE_KEYWORD.None;
	
	[LuaMember("CreateFromString")]
	public static BufKeyword CreateFromString(string keyword_s) {
		BufKeyword new_self = new();
		BUFF_UNIQUE_KEYWORD new_buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(keyword_s);
		new_self.ref_buf_keyword = new_buf_keyword;
		return new_self;
	}
	
	[LuaMember("CreateFromInt")]
	public static BufKeyword CreateFromInt(int keyword_int) {
		BufKeyword new_self = new();
		new_self.ref_buf_keyword = (BUFF_UNIQUE_KEYWORD)keyword_int;
		return new_self;
	}
	
	[LuaMember("CreateFromEventFeed")]
	public static BufKeyword CreateFromEventFeed(int keyword_int) {
		BufKeyword new_self = new();
		new_self.ref_buf_keyword = (BUFF_UNIQUE_KEYWORD)keyword_int;
		return new_self;
	}
	
	[LuaMember("ToString")]
	public string KeywordToString() {
		return ref_buf_keyword.ToString();
	}
}

[LuaObject]
public partial class BufCategory
{
	public BUFF_CATEGORY_KEYWORD ref_buf_category = BUFF_CATEGORY_KEYWORD.NONE;
	
	[LuaMember("CreateFromString")]
	public static BufCategory CreateFromString(string s) {
		BufCategory new_self = new();
		if (Enum.TryParse<BUFF_CATEGORY_KEYWORD>(s, true, out BUFF_CATEGORY_KEYWORD result)) {
			new_self.ref_buf_category = result;
		}
		return new_self;
	}
	
	[LuaMember("CreateFromInt")]
	public static BufCategory CreateFromInt(int integ) {
		BufCategory new_self = new();
		new_self.ref_buf_category = (BUFF_CATEGORY_KEYWORD)integ;
		return new_self;
	}
	
	[LuaMember("ToString")]
	public string KeywordToString() {
		return ref_buf_category.ToString();
	}
}

[LuaObject]
public partial class LuaActionModelManager
{
	[LuaMember("FetchActionList")]
	public static LuaTable FetchActionList() {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		List<BattleActionModel> action_list = actionManager._actionList;
		int count = action_list.Count;
		LuaTable table = new(count,0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new ActionModel(action_list[i]);
		}
		return table;
	}

	[LuaMember("SetActionList")]
	public static void SetActionList(LuaTable actionList_table)
	{
		int count = actionList_table.ArrayLength;
		List<BattleActionModel> actionList_overwrite = new(count);
		for (int i = 0; i < count; i++) {
			ActionModel actionModel = actionList_table[i + 1].Read<ActionModel>();
			if (actionModel.IsNull()) continue;
			actionList_overwrite.Add(actionModel.ref_action);
		}
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		actionManager._actionList = actionList_overwrite;
	}
	
	[LuaMember("RemoveAction")]
	public static void RemoveAction(ActionModel action_remove) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		actionManager.RemoveAction(action_remove.ref_action);
	}
	
	[LuaMember("FetchDuelList")]
	public static LuaTable FetchDuelList() {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		List<BattleDuelModel> duel_list = actionManager._duelList;
		int count = duel_list.Count;
		LuaTable table = new(count,0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new LuaDuelModel(duel_list[i]);
		}
		return table;
	}
	
	[LuaMember("SetDuelList")]
	public static void SetDuelList(LuaTable duelList_table)
	{
		int count = duelList_table.ArrayLength;
		List<BattleDuelModel> duelList_overwrite = new(count);
		for (int i = 0; i < count; i++) {
			LuaDuelModel duelModel = duelList_table[i + 1].Read<LuaDuelModel>();
			if (duelModel.IsNull()) continue;
			duelList_overwrite.Add(duelModel.ref_duel);
		}
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		actionManager._duelList = duelList_overwrite;
	}
	
	[LuaMember("IsDuelForThisOne")]
	public static bool IsDuelForThisOne(ActionModel action) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		return actionManager.IsDuel(action.ref_action);
	}
	[LuaMember("IsDuelBetweenTheseTwo")]
	public static bool IsDuelBetweenTheseTwo(ActionModel action1, ActionModel action2) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		return actionManager.IsDuel(action1.ref_action, action2.ref_action);
	}
	
	[LuaMember("FetchDuelFromAction")]
	public static LuaDuelModel FetchDuelFromAction(ActionModel action) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		return new LuaDuelModel(actionManager.GetDuelModel(action.ref_action));
	}
	
	[LuaMember("RemoveDuelByAction")]
	public static void RemoveDuelByAction(ActionModel action) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		actionManager.RemoveDuel(action.ref_action);
	}
	
	[LuaMember("AddDuel")]
	public static void AddDuel(ActionModel action1, ActionModel action2) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		actionManager.AddDuel(action1.ref_action, action2.ref_action);
	}
	
	[LuaMember("AddAction")]
	public static void AddAction(ActionModel action) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		actionManager.AddAction(action.ref_action);
	}
	
	[LuaMember("CutInAction")]
	public static void CutInAction(ActionModel action, LuaSkillTargetType skillTargetType, string lastOrFirst) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		BattleActionModelManager.CUT_IN_ACTION_TYPE cutInType = lastOrFirst == "Last" ? BattleActionModelManager.CUT_IN_ACTION_TYPE.LAST_APPEND : BattleActionModelManager
			.CUT_IN_ACTION_TYPE.ZERO_INDEX;
		actionManager.CutInAction(action.ref_action, skillTargetType.ref_enum, cutInType);
	}
	
	[LuaMember("CutInActionPrevOtherAction")]
	public static void CutInActionPrevOtherAction(ActionModel action, ActionModel otherAction, LuaSkillTargetType skillTargetType) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		actionManager.CutInActionPrevOtherAction(action.ref_action, skillTargetType.ref_enum, otherAction.ref_action);
	}
	
	[LuaMember("CutInDefenseActionForcely")]
	public static void CutInDefenseActionForcely(ActionModel action, bool first) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		actionManager.CutInDefenseActionForcely(action.ref_action, first);
	}
	
	[LuaMember("AddUniqueCutIn")]
	public static void AddUniqueCutIn(string uniqueCutIn, ActionModel action) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		if (uniqueCutIn == "None") actionManager.AddUniqueCutIn(UniqueCutInTiming.None, action.ref_action);
		else if (uniqueCutIn == "JudgeDuelResult") actionManager.AddUniqueCutIn(UniqueCutInTiming.JudgeDuelResult, action.ref_action);
	}
	
	[LuaMember("ReserveToCutInActionUntilStartRunBattle")]
	public static void ReserveToCutInActionUntilStartRunBattle(ReserveCutInActionType reserveType, ActionModel action, bool removeDuel) {
		BattleActionModelManager actionManager = Singleton<BattleActionModelManager>.Instance;
		actionManager.ReserveToCutInActionUntilStartRunBattle(action.ref_action, reserveType.ref_enum, removeDuel);
	}
}

[LuaObject]
public partial class EventTiming
{
	public BATTLE_EVENT_TIMING ref_timing = BATTLE_EVENT_TIMING.ALL_TIMING;
	
	[LuaMember("CreateFromString")]
	public static EventTiming CreateFromString(string s) {
		EventTiming new_self = new();
		if (Enum.TryParse<BATTLE_EVENT_TIMING>(s, true, out BATTLE_EVENT_TIMING result)) {
			new_self.ref_timing = result;
		}
		return new_self;
	}
	
	[LuaMember("CreateFromInt")]
	public static EventTiming CreateFromInt(int integ) {
		return new EventTiming {
			ref_timing = (BATTLE_EVENT_TIMING)integ
		};
	}
	
	[LuaMember("ToString")]
	public string KeywordToString() => ref_timing.ToString();
}

[LuaObject]
public partial class DamageSourceType
{
	public DAMAGE_SOURCE_TYPE ref_dmgSrcType = DAMAGE_SOURCE_TYPE.NONE;
	
	[LuaMember("CreateFromString")]
	public static DamageSourceType CreateFromString(string s) {
		DamageSourceType new_self = new();
		if (Enum.TryParse(s, true, out DAMAGE_SOURCE_TYPE result)) {
			new_self.ref_dmgSrcType = result;
		}
		return new_self;
	}
	
	[LuaMember("CreateFromInt")]
	public static DamageSourceType CreateFromInt(int integ) {
		return new DamageSourceType {
			ref_dmgSrcType = (DAMAGE_SOURCE_TYPE)integ
		};
	}
	
	[LuaMember("ToString")]
	public string KeywordToString() {
		return ref_dmgSrcType.ToString();
	}
}

[LuaObject]
public partial class AbilitySourceType
{
	public ABILITY_SOURCE_TYPE ref_abilitySrcType = ABILITY_SOURCE_TYPE.NONE;
	
	[LuaMember("CreateFromString")]
	public static AbilitySourceType CreateFromString(string s) {
		AbilitySourceType new_self = new();
		if (Enum.TryParse(s, true, out ABILITY_SOURCE_TYPE result)) {
			new_self.ref_abilitySrcType = result;
		}
		return new_self;
	}
	
	[LuaMember("CreateFromInt")]
	public static AbilitySourceType CreateFromInt(int integ) {
		return new AbilitySourceType {
			ref_abilitySrcType = (ABILITY_SOURCE_TYPE)integ
		};
	}
	
	[LuaMember("ToString")]
	public string KeywordToString() {
		return ref_abilitySrcType.ToString();
	}
}

[LuaObject]
public partial class ReserveCutInActionType
{
	public BattleActionModelManager.RESERVE_CUT_IN_ACTION_TYPE ref_enum = BattleActionModelManager.RESERVE_CUT_IN_ACTION_TYPE.NONE;
	
	[LuaMember("CreateFromString")]
	public static ReserveCutInActionType CreateFromString(string s) {
		ReserveCutInActionType new_self = new();
		if (Enum.TryParse(s, true, out BattleActionModelManager.RESERVE_CUT_IN_ACTION_TYPE result)) {
			new_self.ref_enum = result;
		}
		return new_self;
	}
	
	[LuaMember("CreateFromInt")]
	public static ReserveCutInActionType CreateFromInt(int integ) {
		return new ReserveCutInActionType {
			ref_enum = (BattleActionModelManager.RESERVE_CUT_IN_ACTION_TYPE)integ
		};
	}
	
	[LuaMember("ToString")]
	public string KeywordToString() {
		return ref_enum.ToString();
	}
	
	[LuaMember("PrintAllEnumValues")]
	public void PrintAllEnumValues() {
		foreach (string keyname in Il2CppSystem.Enum.GetNames(Il2CppType.Of<BattleActionModelManager.RESERVE_CUT_IN_ACTION_TYPE>())) {
			MainClass.LogModular("RESERVE_CUT_IN_ACTION_TYPE: " + keyname, true);
		}
	}
}

[LuaObject]
public partial class LuaSkillTargetType
{
	public SKILL_TARGET_TYPE ref_enum = SKILL_TARGET_TYPE.NONE;
	
	[LuaMember("CreateFromString")]
	public static LuaSkillTargetType CreateFromString(string s) {
		LuaSkillTargetType new_self = new();
		if (Enum.TryParse(s, true, out SKILL_TARGET_TYPE result)) {
			new_self.ref_enum = result;
		}
		return new_self;
	}
	
	[LuaMember("CreateFromInt")]
	public static LuaSkillTargetType CreateFromInt(int integ) {
		return new LuaSkillTargetType {
			ref_enum = (SKILL_TARGET_TYPE)integ
		};
	}
	
	[LuaMember("ToString")]
	public string KeywordToString() {
		return ref_enum.ToString();
	}
	
	[LuaMember("PrintAllEnumValues")]
	public void PrintAllEnumValues() {
		foreach (string keyname in Il2CppSystem.Enum.GetNames(Il2CppType.Of<SKILL_TARGET_TYPE>())) {
			MainClass.LogModular("SKILL_TARGET_TYPE: " + keyname, true);
		}
	}
}

[LuaObject]
public partial class LuaAttributeType
{
	public ATTRIBUTE_TYPE ref_enum = ATTRIBUTE_TYPE.NONE;
	
	[LuaMember("CreateFromString")]
	public static LuaAttributeType CreateFromString(string s) {
		LuaAttributeType new_self = new();
		if (Enum.TryParse(s, true, out ATTRIBUTE_TYPE result)) {
			new_self.ref_enum = result;
		}
		return new_self;
	}
	
	[LuaMember("CreateFromInt")]
	public static LuaAttributeType CreateFromInt(int integ) => new LuaAttributeType { ref_enum = (ATTRIBUTE_TYPE)integ };
	
	[LuaMember("ToString")]
	public string KeywordToString() => ref_enum.ToString();
	
	[LuaMember("ToInt")]
	public int KeywordToInt() => (int)ref_enum;
	
	[LuaMember("PrintAllEnumValues")]
	public void PrintAllEnumValues() {
		foreach (string keyname in Il2CppSystem.Enum.GetNames(Il2CppType.Of<ATTRIBUTE_TYPE>())) {
			MainClass.LogModular("ATTRIBUTE_TYPE: " + keyname, true);
		}
	}

	[LuaMember("IsEqualTo")]
	public bool IsEqualTo(LuaAttributeType other) => ref_enum == other.ref_enum;
}

[LuaObject]
public partial class LuaAtkType
{
	public ATK_BEHAVIOUR ref_enum = ATK_BEHAVIOUR.NONE;
	
	[LuaMember("CreateFromString")]
	public static LuaAtkType CreateFromString(string s) {
		LuaAtkType new_self = new();
		if (Enum.TryParse(s, true, out ATK_BEHAVIOUR result)) {
			new_self.ref_enum = result;
		}
		return new_self;
	}
	
	[LuaMember("CreateFromInt")]
	public static LuaAtkType CreateFromInt(int integ) => new LuaAtkType { ref_enum = (ATK_BEHAVIOUR)integ };
	
	[LuaMember("ToString")]
	public string KeywordToString() => ref_enum.ToString();
	
	[LuaMember("ToInt")]
	public int KeywordToInt() => (int)ref_enum;
	
	[LuaMember("PrintAllEnumValues")]
	public void PrintAllEnumValues() {
		foreach (string keyname in Il2CppSystem.Enum.GetNames(Il2CppType.Of<ATK_BEHAVIOUR>())) {
			MainClass.LogModular("ATK_BEHAVIOUR: " + keyname, true);
		}
	}

	[LuaMember("IsEqualTo")]
	public bool IsEqualTo(LuaAtkType other) => ref_enum == other.ref_enum;
}

[LuaObject]
public partial class LuaDefType
{
	public DEFENSE_TYPE ref_enum;
	
	[LuaMember("CreateFromString")]
	public static LuaDefType CreateFromString(string s) {
		LuaDefType new_self = new();
		if (Enum.TryParse(s, true, out DEFENSE_TYPE result)) {
			new_self.ref_enum = result;
		}
		return new_self;
	}
	
	[LuaMember("CreateFromInt")]
	public static LuaDefType CreateFromInt(int integ) => new LuaDefType { ref_enum = (DEFENSE_TYPE)integ };
	
	[LuaMember("ToString")]
	public string KeywordToString() => ref_enum.ToString();
	
	[LuaMember("ToInt")]
	public int KeywordToInt() => (int)ref_enum;
	
	[LuaMember("PrintAllEnumValues")]
	public void PrintAllEnumValues() {
		foreach (string keyname in Il2CppSystem.Enum.GetNames(Il2CppType.Of<DEFENSE_TYPE>())) {
			MainClass.LogModular("DEFENSE_TYPE: " + keyname, true);
		}
	}

	[LuaMember("IsEqualTo")]
	public bool IsEqualTo(LuaDefType other) => ref_enum == other.ref_enum;
}

[LuaObject]
public partial class LuaPassiveModel
{
	public PassiveModel ref_passive = null;
	public LuaPassiveModel(PassiveModel x) {
		ref_passive = x;
	}
	
	[LuaMember("IsNull")]
	public bool IsNull() => ref_passive == null;
	
	[LuaMember("PassiveID")]
	public int PassiveID() => ref_passive.GetID();
	
	[LuaMember("FetchAllModsa")]
	public LuaTable FetchAllModsa(bool fast_no_copy, bool check_active) {
		List<ModularSA> modsa_list = null;
		if (fast_no_copy) modsa_list = SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(ref_passive, check_active);
		else modsa_list = SkillScriptInitPatch.GetAllModpaFromPasmodel(ref_passive, check_active);
		
		int count = modsa_list.Count;
		LuaTable table = new(count,0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new ModsaInterface(modsa_list[i]);
		}
		return table;
	}
}

[LuaObject]
public partial class LuaDuelModel
{
	public BattleDuelModel ref_duel = null;
	public LuaDuelModel(BattleDuelModel x) {
		ref_duel = x;
	}
	
	[LuaMember("IsNull")]
	public bool IsNull() {
		return ref_duel == null;
	}
	
	[LuaMember("HasAction")]
	public bool HasAction(ActionModel action) {
		return ref_duel.IsDuel(action.ref_action);
	}
	
	[LuaMember("FetchAction")]
	public ActionModel FetchAction(int action_number) {
		if (action_number == 1) return new ActionModel(ref_duel._action1);
		if (action_number == 2) return new ActionModel(ref_duel._action2);
		return new ActionModel(null);
	}
	
	[LuaMember("SetAction")]
	public void SetAction(int action_number, ActionModel action_set) {
		//ref_duel.IsDuel()
		if (action_number == 1) ref_duel._action1 = action_set.ref_action;
		else if (action_number == 2) ref_duel._action2 = action_set.ref_action;
	}
	
	[LuaMember("FetchOpponentAction")]
	public ActionModel FetchOpponentAction(ActionModel fromAction) {
		return new ActionModel(ref_duel.GetOpponentAction(fromAction.ref_action));
	}
}

[LuaObject]
public partial class ActionModel
{
	public BattleActionModel ref_action = null;
	public ActionModel(BattleActionModel x) => ref_action = x;
	
	[LuaMember("IsNull")]
	public bool IsNull() => ref_action == null;
	
	[LuaMember("CreateNull")]
	public static ActionModel CreateNull() => new(null);
	
	[LuaMember("CreateNew")]
	public static ActionModel CreateNew(LuaUnitSinModel unitSin, UnitModel unit, LuaSinActionModel sinAction, LuaSkillModel? skill = null)
	{
		UnitSinModel van_unitSin = unitSin.ref_unitSin;
		BattleUnitModel van_unit = unit.ref_unitModel;
		SinActionModel van_skillAction = sinAction.ref_sinAction;
		BattleActionModel action_new = new(van_unitSin, van_unit, van_skillAction, -1, skill?.ref_skill);
		return new ActionModel(action_new);
	}
	
	[LuaMember("CreateCopy")]
	public static ActionModel CreateCopy(ActionModel action, bool? canDuel = null)
	{
		if (canDuel != null) return new ActionModel(new BattleActionModel(action.ref_action, (bool)canDuel));
		return new ActionModel(new BattleActionModel(action.ref_action));
	}
	
	[LuaMember("FetchUnitModel")]
	public UnitModel FetchUnitModel() => new UnitModel(ref_action._model);
	
	[LuaMember("FetchSkill")]
	public LuaSkillModel FetchSkill() => new LuaSkillModel(ref_action._skill);
	
	[LuaMember("FetchSinAction")]
	public LuaSinActionModel FetchSinAction() => new LuaSinActionModel(ref_action._sinAction);
	
	[LuaMember("TryChangeSkillByID")]
	public void TryChangeSkillByID(int skillID, bool erase_prev_skill) => ConsequenceChangeSkill.ChangeSkillOfAction(ref_action, skillID, erase_prev_skill);
	[LuaMember("ChangeSkillByModel")]
	public bool TryChangeSkillByModel(LuaSkillModel skill, bool saveOriginSkill) => ref_action.ChangeSkill(skill.ref_skill, saveOriginSkill);
	
	[LuaMember("skill")]
	public LuaSkillModel skill {
		get => new(ref_action._skill);
		set => ref_action._skill = value.ref_skill;
	} 
	
	[LuaMember("unitSinModel")]
	public LuaUnitSinModel unitSinModel {
		get => new(ref_action._sin);
		set => ref_action._sin = value.ref_unitSin;
	} 
	
	[LuaMember("expectedToEarnMap")]
	public LuaTable expectedToEarnMap {
		get
		{
			Dictionary<ATTRIBUTE_TYPE, int> dict = ref_action._expectedToEarnMap;
			LuaTable table = new(0, dict.Count);
			foreach (KeyValuePair<ATTRIBUTE_TYPE, int> kvp in dict) {
				string sinType_s = kvp.Key.ToString();
				table[sinType_s] = kvp.Value;
			}
			return table;
		}
		set
		{
			Dictionary<ATTRIBUTE_TYPE, int> dict = new();
			for (int i = 0; i < 7; i++) {
				ATTRIBUTE_TYPE sinType = (ATTRIBUTE_TYPE)i;
				string sinType_s = sinType.ToString();
				if (value.TryGetValue(sinType_s, out LuaValue luaval)) {
					int amount = luaval.Read<int>();
					dict[sinType] = amount;
				}
			}
			ref_action._expectedToEarnMap = dict;
		}
	}
	
	[LuaMember("expectedToEarnMapForInit")]
	public LuaTable expectedToEarnMapForInit {
		get
		{
			Dictionary<ATTRIBUTE_TYPE, int> dict = ref_action._expectedToEarnMapForInit;
			LuaTable table = new(0, dict.Count);
			foreach (KeyValuePair<ATTRIBUTE_TYPE, int> kvp in dict) {
				string sinType_s = kvp.Key.ToString();
				table[sinType_s] = kvp.Value;
			}
			return table;
		}
		set
		{
			Dictionary<ATTRIBUTE_TYPE, int> dict = new();
			for (int i = 0; i < 7; i++) {
				ATTRIBUTE_TYPE sinType = (ATTRIBUTE_TYPE)i;
				string sinType_s = sinType.ToString();
				if (value.TryGetValue(sinType_s, out LuaValue luaval)) {
					int amount = luaval.Read<int>();
					dict[sinType] = amount;
				}
			}
			ref_action._expectedToEarnMapForInit = dict;
		}
	}

	[LuaMember("targetDataDetail")]
	public LuaTargetDetail targetDataDetail
	{
		get => new(ref_action._targetDataDetail);
		set => ref_action._targetDataDetail = value.ref_obj;
	} 
	
	[LuaMember("ResetAllTargetData")]
	public void ResetAllTargetData() => ref_action.ResetAllTargetData();
	[LuaMember("SetTargetAfterBattleStart_CutIn")]
	public void SetTargetAfterBattleStart_CutIn(bool canDuel, bool attackEnemyOnly, LuaSkillTargetType overwrite_skillTargetType) =>
		ref_action.SetTargetAfterBattleStart_CutIn(canDuel, attackEnemyOnly, overwrite_skillTargetType.ref_enum);
	
	[LuaMember("ChangeMainTargetSinAction")]
	public void ChangeMainTargetSinAction(LuaSinActionModel sinAction, ActionModel? action_opponent = null, bool forcibly = false) =>
		ref_action.ChangeMainTargetSinAction(sinAction.ref_sinAction, action_opponent?.ref_action, forcibly);
	
	[LuaMember("FetchMainTargetSinAction")]
	public LuaSinActionModel FetchMainTargetSinAction() => new(ref_action.GetMainTargetSinAction());
	[LuaMember("FetchExpectedMainTargetSinAction")]
	public static LuaSinActionModel FetchExpectedMainTargetSinAction(ModsaInterface modsa) => new(modsa.ref_modsa.modsa_expected_sinaction);
	
}

[LuaObject]
public partial class LuaTargetDetail
{
	public BattleActionModel.TargetDataDetail ref_obj = null;

	public LuaTargetDetail(BattleActionModel.TargetDataDetail x) => ref_obj = x;

	[LuaMember("IsNull")]
	public bool IsNull() => ref_obj == null;
	
	[LuaMember("FetchCurrentTargetSet")]
	public LuaTargetSet FetchCurrentTargetSet() => new(ref_obj.GetCurrentTargetSet());
	[LuaMember("FetchOriginTargetSet")]
	public LuaTargetSet FetchOriginTargetSet() => new(ref_obj.GetOriginTargetSet());
	
}

[LuaObject]
public partial class LuaTargetSet
{
	public BattleActionModel.TargetDataDetail.TargetDataSet ref_obj = null;

	public LuaTargetSet(BattleActionModel.TargetDataDetail.TargetDataSet x) => ref_obj = x;

	[LuaMember("IsNull")]
	public bool IsNull() => ref_obj == null;
	
	[LuaMember("mainTarget")]
	public LuaTargetSinActionData mainTarget
	{
		get => new(ref_obj._mainTarget);
		set => ref_obj._mainTarget = value.ref_obj;
	}
	
	[LuaMember("subTargetList")]
	public LuaTable subTargetList
	{
		get => LuaTargetSinActionData.MakeTable(ref_obj._subTargetList);
		set => ref_obj._subTargetList = LuaTargetSinActionData.UnpackTable(value);
	}
}

[LuaObject]
public partial class LuaTargetSinActionData
{
	public TargetSinActionData ref_obj = null;

	public LuaTargetSinActionData(TargetSinActionData x) => ref_obj = x;

	public static LuaTable MakeTable(List<TargetSinActionData> list) {
		int count = list.Count;
		LuaTable table = new LuaTable(count, 0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new LuaTargetSinActionData(list[i]);
		}
		return table;
	}
	
	public static List<TargetSinActionData> UnpackTable(LuaTable table) {
		int count = table.ArrayLength;
		List<TargetSinActionData> list = new(count);
		for (int i = 0; i < count; i++) {
			LuaTargetSinActionData luaval = table[i + 1].Read<LuaTargetSinActionData>();
			list.Add(luaval.ref_obj);
		}
		return list;
	}
	
	[LuaMember("IsNull")]
	public bool IsNull() => ref_obj == null;
	
	[LuaMember("CreateNew")]
	public static LuaTargetSinActionData CreateNew(LuaSinActionModel sinAction) => new(new TargetSinActionData(sinAction.ref_sinAction));
	
	[LuaMember("targetSinAction")]
	public LuaSinActionModel targetSinAction {
		get => new(ref_obj._targetSinAction);
		set => ref_obj._targetSinAction = value.ref_sinAction;
	}
}

[LuaObject]
public partial class LuaCoinModel
{
	public CoinModel ref_coin = null;
	public LuaCoinModel(CoinModel x) => ref_coin = x;
	
	[LuaMember("IsNull")]
	public bool IsNull() => ref_coin == null;

	public static LuaTable MakeTable(List<CoinModel> list) {
		int count = list.Count;
		LuaTable table = new LuaTable(count, 0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new LuaCoinModel(list[i]);
		}
		return table;
	}
	
	public static List<CoinModel> UnpackTable(LuaTable table) {
		int count = table.ArrayLength;
		List<CoinModel> list = new(count);
		for (int i = 0; i < count; i++) {
			LuaCoinModel luaval = table[i + 1].Read<LuaCoinModel>();
			list.Add(luaval.ref_coin);
		}
		return list;
	}

	[LuaMember("LuaSkillCoinData")]
	public LuaSkillCoinData FetchData() => new(ref_coin._classInfo);
	
	[LuaMember("FetchAllModsa")]
	public LuaTable FetchAllModsa(bool fast_no_copy) {
		List<ModularSA> modsa_list = null;
		if (fast_no_copy) modsa_list = SkillScriptInitPatch.GetAllModcaFromCoinModel(ref_coin);
		else modsa_list = SkillScriptInitPatch.GetAllModcaFromCoinModel(ref_coin);
		
		int count = modsa_list.Count;
		LuaTable table = new(count,0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new ModsaInterface(modsa_list[i]);
		}
		return table;
	}
	
	[LuaMember("isActive")]
	public bool isActive {
		get => ref_coin._isActive;
		set => ref_coin._isActive = value;
	}
	
	[LuaMember("isUsableInDuel")]
	public bool isUsableInDuel {
		get => ref_coin._isUsableInDuel;
		set => ref_coin._isUsableInDuel = value;
	}
	
	[LuaMember("isReRolled")]
	public bool isReRolled {
		get => ref_coin._isReRolled;
		set => ref_coin._isReRolled = value;
	}
	
	[LuaMember("isDestroyed")]
	public bool isDestroyed {
		get => ref_coin._isDestroyed;
		set => ref_coin._isDestroyed = value;
	}
	
	[LuaMember("result")]
	public int result {
		get => (int)ref_coin._result;
		set => ref_coin._result = (COIN_RESULT)value;
	}
	
	[LuaMember("originCoinIndex")]
	public int originCoinIndex {
		get => ref_coin._originCoinIndex;
		set => ref_coin._originCoinIndex = value;
	}
	
	[LuaMember("currentRealCoinIndex")]
	public int currentRealCoinIndex {
		get => ref_coin._currentRealCoinIndex;
		set => ref_coin._currentRealCoinIndex = value;
	}
	
	[LuaMember("grade")]
	public int GetGrade() => ref_coin.GetCoinGrade();
	
	[LuaMember("scale")]
	public int scale {
		get => ref_coin._scale;
		set => ref_coin._scale = value;
	}
	
	[LuaMember("operatorType")]
	public int operatorType {
		get => (int)ref_coin._operatorType;
		set => ref_coin._operatorType = (OPERATOR_TYPE)value;
	}
}

[LuaObject]
public partial class LuaSkillCoinData
{
	public SkillCoinData ref_data = null;
	public LuaSkillCoinData(SkillCoinData x) => ref_data = x;
	
	[LuaMember("actionIndex")]
	public int actionIndex {
		get => ref_data.actionIndex;
		set => ref_data.actionIndex = value;
	}
	
	[LuaMember("grade")]
	public int grade {
		get => ref_data.grade;
		set => ref_data.grade = value;
	}
	
	[LuaMember("scale")]
	public int scale {
		get => ref_data.scale;
		set => ref_data.scale = value;
	}
	
	[LuaMember("operatorType")]
	public int operatorType {
		get => (int)ref_data._operatorType;
		set => ref_data._operatorType = (OPERATOR_TYPE)value;
	}
}

[LuaObject]
public partial class LuaSkillModel
{
	public SkillModel ref_skill = null;
	public LuaSkillModel(SkillModel x) => ref_skill = x;
	
	[LuaMember("IsNull")]
	public bool IsNull() => ref_skill == null;
	
	[LuaMember("FetchAllModsa")]
	public LuaTable FetchAllModsa(bool fast_no_copy) {
		List<ModularSA> modsa_list = null;
		if (fast_no_copy) modsa_list = SkillScriptInitPatch.GetAllModsaFromSkillModel_Fast(ref_skill);
		else modsa_list = SkillScriptInitPatch.GetAllModsaFromSkillModel(ref_skill);
		int count = modsa_list.Count;
		LuaTable table = new(count,0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new ModsaInterface(modsa_list[i]);
		}
		return table;
	}
	
	[LuaMember("SkillID")]
	public int SkillID() => ref_skill.GetID();
	
	[LuaMember("SkillTier")]
	public int SkillTier() => ref_skill.GetSkillTier();
	
	[LuaMember("FetchAttribute")]
	public LuaAttributeType FetchAttribute() => new LuaAttributeType{ ref_enum = ref_skill.GetAttributeType()};
	
	[LuaMember("FetchAtkType")]
	public LuaAtkType FetchAtkType() => new LuaAtkType{ ref_enum = ref_skill.GetAttackType()};
	
	[LuaMember("FetchDefenseType")]
	public LuaDefType FetchDefenseType() => new LuaDefType{ ref_enum = ref_skill.GetDefenseType()};
	
	[LuaMember("coinList")]
	public LuaTable coinList {
		get => LuaCoinModel.MakeTable(ref_skill._coinList);
		set => ref_skill._coinList = LuaCoinModel.UnpackTable(value);
	}
	
	[LuaMember("FetchCoinsUsableInDuel")]
	public LuaTable FetchCoinsUsableInDuel() => LuaCoinModel.MakeTable(ref_skill.GetCoinsUsableInDuel());
	
	[LuaMember("FetchAliveCoins")]
	public LuaTable FetchAliveCoins() => LuaCoinModel.MakeTable(ref_skill.GetAliveCoins());
	
	[LuaMember("ReRollCoin")]
	public void ReRollCoin(ActionModel action, LuaCoinModel coin, EventTiming timing) {
		ref_skill.ReRollCoin(action.ref_action, coin.ref_coin, timing.ref_timing);
	}
	
	[LuaMember("CopyCoin")]
	public void CopyCoin(ActionModel action, int idx, EventTiming timing) {
		ref_skill.CopyCoin(action.ref_action, idx, timing.ref_timing);
	}
	
	[LuaMember("AddCoin")]
	public void AddCoin(LuaCoinModel coin, bool forcibly) {
		if (forcibly) ref_skill.ForcelyAddCoin(coin.ref_coin);
		else ref_skill.AddCoin(coin.ref_coin);
	}
	
	[LuaMember("RemoveCoinModelFromListByIndex")]
	public void RemoveCoinModelFromListByIndex(int idx) => ref_skill.RemoveCoinModelFromListByIndex(idx);
}

[LuaObject]
public partial class LuaSinActionModel
{
	public SinActionModel ref_sinAction = null;
	public LuaSinActionModel(SinActionModel x) => ref_sinAction = x;
	
	[LuaMember("IsNull")]
	public bool IsNull() => ref_sinAction == null;
	
	[LuaMember("IsEqualTo")]
	public bool IsEqualTo(LuaSinActionModel other) => ref_sinAction == other.ref_sinAction;
	
	[LuaMember("GetSlotIndex")]
	public int GetSlotIndex() => ref_sinAction.GetSlotIndex();
	
	[LuaMember("FetchUnitModel")]
	public UnitModel FetchUnitModel() => new UnitModel(ref_sinAction._unitModel);
	
	[LuaMember("currentSinList")]
	public LuaTable currentSinList {
		get => LuaUnitSinModel.MakeTable(ref_sinAction.currentSinList);
		set => ref_sinAction.currentSinList = LuaUnitSinModel.UnpackTable(value);
	}
	
	[LuaMember("readySinList")]
	public LuaTable readySinList {
		get => LuaUnitSinModel.MakeTable(ref_sinAction.readySinList);
		set => ref_sinAction.readySinList = LuaUnitSinModel.UnpackTable(value);
	}
	
	[LuaMember("replacedSinListByErode")]
	public LuaTable replacedSinListByErode {
		get => LuaUnitSinModel.MakeTable(ref_sinAction._replacedSinListByErode);
		set => ref_sinAction._replacedSinListByErode = LuaUnitSinModel.UnpackTable(value);
	}
	
	[LuaMember("currentSelectSin")]
	public LuaUnitSinModel currentSelectSin {
		get => new(ref_sinAction.currentSelectSin);
		set => ref_sinAction.currentSelectSin = value.ref_unitSin;
	}
	
	[LuaMember("readySin")]
	public LuaUnitSinModel readySin {
		get => new(ref_sinAction.readySin);
		set => ref_sinAction.readySin = value.ref_unitSin;
	}
	
	[LuaMember("replacedReadySinByErode")]
	public LuaUnitSinModel replacedReadySinByErode {
		get => new(ref_sinAction._replacedReadySinByErode);
		set => ref_sinAction._replacedReadySinByErode = value.ref_unitSin;
	}
	
	[LuaMember("replacedSinByDefenseSkill")]
	public LuaUnitSinModel replacedSinByDefenseSkill {
		get => new(ref_sinAction._replacedSinByDefenseSkill);
		set => ref_sinAction._replacedSinByDefenseSkill = value.ref_unitSin;
	}
	
}

[LuaObject]
public partial class LuaUnitSinModel
{
	public UnitSinModel ref_unitSin = null;
	public LuaUnitSinModel(UnitSinModel x) => ref_unitSin = x;

	[LuaMember("IsNull")]
	public bool IsNull() => ref_unitSin == null;
	
	[LuaMember("IsEqualTo")]
	public bool IsEqualTo(LuaUnitSinModel other) => ref_unitSin == other.ref_unitSin;
	
	public static LuaTable MakeTable(List<UnitSinModel> list) {
  	int count = list.Count;
  	LuaTable table = new LuaTable(count, 0);
  	for (int i = 0; i < count; i++) {
  		table[i + 1] = new LuaUnitSinModel(list[i]);
  	}
  	return table;
  }
  
  public static List<UnitSinModel> UnpackTable(LuaTable table) {
  	int count = table.ArrayLength;
  	List<UnitSinModel> list = new(count);
  	for (int i = 0; i < count; i++) {
		  LuaUnitSinModel luaval = table[i + 1].Read<LuaUnitSinModel>();
  		list.Add(luaval.ref_unitSin);
  	}
  	return list;
  }

  [LuaMember("CreateNewNormal")]
  public static LuaUnitSinModel CreateNewNormal(int skillID, UnitModel unit, LuaSinActionModel sinAction, bool overwrited) {
	  return new LuaUnitSinModel(new UnitSinModel(skillID, unit.ref_unitModel, sinAction.ref_sinAction, overwrited));
  }
  
  [LuaMember("CreateNewEGO")]
  public static LuaUnitSinModel CreateNewEGO(int skillID, UnitModel unit, LuaSinActionModel sinAction, bool alreadyUsedCost, bool overwrited) {
	  return new LuaUnitSinModel(new UnitSinModel(unit.ref_unitModel, skillID, sinAction.ref_sinAction, alreadyUsedCost, overwrited));
  }
  
  [LuaMember("currentAction")]
  public ActionModel currentAction {
	  get => new ActionModel(ref_unitSin._currentAction);
	  set => ref_unitSin._currentAction = value.ref_action;
  }
  
  [LuaMember("skillId")]
  public int skillId {
	  get => ref_unitSin._skillId;
	  set => ref_unitSin._skillId = value;
  } 
}

[LuaObject]
public partial class LuaResistDetail
{
	public ResistDetail ref_resdet = null;
	public LuaResistDetail(ResistDetail x) => ref_resdet = x;
	
	[LuaMember("atkResistList")]
	public LuaTable atkResistList {
		get
		{
			List<ATKResistData> list = ref_resdet._atkResist;
			int count = list.Count;
			LuaTable table = new LuaTable(count, 0);
			for (int i = 0; i < count; i++) {
				table[i + 1] = new LuaATKResistData(list[i]);
			}
			return table;
		}
		set
		{
			int count = value.ArrayLength;
			List<ATKResistData> list = new(count);
			for (int i = 0; i < count; i++) {
				LuaATKResistData luaval = value[i + 1].Read<LuaATKResistData>();
				list.Add(luaval.ref_data);
			}
			ref_resdet._atkResist = list;
		}
	}
	
	[LuaMember("attributeResistList")]
	public LuaTable attributeResistList {
		get
		{
			List<AttributeResistData> list = ref_resdet._attributeResist;
			int count = list.Count;
			LuaTable table = new LuaTable(count, 0);
			for (int i = 0; i < count; i++) {
				table[i + 1] = new LuaAttributeResistData(list[i]);
			}
			return table;
		}
		set
		{
			int count = value.ArrayLength;
			List<AttributeResistData> list = new(count);
			for (int i = 0; i < count; i++) {
				LuaAttributeResistData luaval = value[i + 1].Read<LuaAttributeResistData>();
				list.Add(luaval.ref_data);
			}
			ref_resdet._attributeResist = list;
		}
	}
}

[LuaObject]
public partial class LuaATKResistData
{
	public ATKResistData ref_data = null;
	public LuaATKResistData(ATKResistData x) => ref_data = x;
	
	[LuaMember("atkType")]
	public LuaAtkType atkType {
		get => new() { ref_enum = ref_data._type};
		set => ref_data._type = value.ref_enum;
	}
	
	[LuaMember("resistanceValue")]
	public double resistanceValue {
		get => ref_data.value;
		set => ref_data.value = (float)value;
	}
}

[LuaObject]
public partial class LuaAttributeResistData
{
	public AttributeResistData ref_data = null;
	public LuaAttributeResistData(AttributeResistData x) => ref_data = x;
	
	[LuaMember("sinType")]
	public LuaAttributeType sinType {
		get => new() { ref_enum = ref_data._type};
		set => ref_data._type = value.ref_enum;
	}
	
	[LuaMember("resistanceValue")]
	public double resistanceValue {
		get => ref_data.value;
		set => ref_data.value = (float)value;
	}
}

[LuaObject]
public partial class LuaUnitView
{
	public BattleUnitView ref_view = null;
	public LuaUnitView(BattleUnitView x) {
		ref_view = x;
	}
	[LuaMember("IsNull")]
	public bool IsNull() => !ref_view;
	[LuaMember("FetchTransform")]
	public LuaUnityTransform FetchTransform() => new(ref_view.transform);
	
	[LuaMember("FetchAppearance")]
	public LuaCharAppearance FetchAppearance() => new(ref_view.Appearance);
	
	[LuaMember("RefreshState")]
	public void RefreshState() => ref_view.RefreshState(ref_view._unitModel);
	
	[LuaMember("RefreshEffects")]
	public void RefreshEffects() => ref_view.RefreshEffects();
}

[LuaObject]
public partial class LuaCharAppearance
{
	public CharacterAppearance ref_appear = null;
	public LuaCharAppearance(CharacterAppearance x) {
		ref_appear = x;
	}
	[LuaMember("IsNull")]
	public bool IsNull() => !ref_appear;
	[LuaMember("FetchTransform")]
	public LuaUnityTransform FetchTransform() => new(ref_appear.transform);
}

[LuaObject]
public partial class LuaUnityTransform
{
	public Transform ref_transform = null;
	public LuaUnityTransform(Transform x) {
		ref_transform = x;
	}
	[LuaMember("IsNull")]
	public bool IsNull() => !ref_transform;
	
	[LuaMember("FetchParent")]
	public LuaUnityTransform FetchParent() => new(ref_transform.parent);
	[LuaMember("SetParent")]
	public void SetParent(LuaUnityTransform parent, bool worldPositionStays) {
		ref_transform.SetParent(parent.ref_transform, worldPositionStays);
	}
	
	[LuaMember("localEuler")]
	public LuaVector3 localEuler {
		get => new(ref_transform.localEulerAngles);
		set => ref_transform.localEulerAngles = value.ref_vec;
	}
	[LuaMember("globalEuler")]
	public LuaVector3 globalEuler {
		get => new(ref_transform.eulerAngles);
		set => ref_transform.eulerAngles = value.ref_vec;
	}
	
	[LuaMember("localScale")]
	public LuaVector3 localScale {
		get => new(ref_transform.localScale);
		set => ref_transform.localScale = value.ref_vec;
	}
	
	[LuaMember("localPos")]
	public LuaVector3 localPos {
		get => new(ref_transform.localPosition);
		set => ref_transform.localPosition = value.ref_vec;
	}
	[LuaMember("globalPos")]
	public LuaVector3 globalPos {
		get => new(ref_transform.position);
		set => ref_transform.position = value.ref_vec;
	}
}

[LuaObject]
public partial class LuaVector3
{
	public Vector3 ref_vec;

	public LuaVector3(Vector3 fromVec) {
		ref_vec = fromVec;
	}

	public LuaVector3(float x, float y, float z) {
		ref_vec = new Vector3(x, y, z);
	}	
	
	[LuaMember("x")]
	public float x {
		get => ref_vec.x;
		set => ref_vec = ref_vec with { x = value };
	}

	[LuaMember("y")]
	public float y {
		get => ref_vec.y;
		set => ref_vec = ref_vec with { y = value };
	}

	[LuaMember("z")]
	public float z {
		get => ref_vec.z;
		set => ref_vec = ref_vec with { z = value };
	}

	[LuaMember("CreateFromFloats")]
	public static LuaVector3 CreateFromFloats(double x, double y, double z) => new((float)x, (float)y, (float)z);

	[LuaMember("CreateNormalized")]
	public static LuaVector3 CreateNormalized(LuaVector3 vec) => new(Vector3.Normalize(vec.ref_vec));
	
	[LuaMember("Normalize")]
	public void Normalize() {
		ref_vec = Vector3.Normalize(ref_vec);
	}
	
	[LuaMember("Add")]
	public static LuaVector3 Add(LuaVector3 a, LuaVector3 b) => new(a.ref_vec + b.ref_vec);
	[LuaMember("Sub")]
	public static LuaVector3 Sub(LuaVector3 a, LuaVector3 b) => new(a.ref_vec - b.ref_vec);
	[LuaMember("Mul")]
	public static LuaVector3 Mul(LuaVector3 a, double scalar) => new(a.ref_vec * (float)scalar);
	
}

[LuaObject]
public partial class PacketChangeTakeDamage
{
	public BattleActionModel action = null;
	public CoinModel coin = null;
	public DAMAGE_SOURCE_TYPE dmgSrcType = DAMAGE_SOURCE_TYPE.NONE;
	public BUFF_UNIQUE_KEYWORD keyword = BUFF_UNIQUE_KEYWORD.None;

	[LuaMember("FetchAction")]
	public ActionModel FetchAction() {
		return new ActionModel(action);
	}
	[LuaMember("FetchCoin")]
	public LuaCoinModel FetchCoin() {
		return new LuaCoinModel(coin);
	}
	
	[LuaMember("FetchDmgSrcType")]
	public DamageSourceType FetchDmgSrcType() {
		return new DamageSourceType {
			ref_dmgSrcType = dmgSrcType
		};
	}
	
	[LuaMember("FetchBufKeyword")]
	public BufKeyword FetchBufKeyword() {
		return new BufKeyword {
			ref_buf_keyword = keyword
		};
	}
	
	[LuaMember("resultDmg")]
	public int resultDmg = 0;
	[LuaMember("preCalculatedDmg")]
	public int preCalculatedDmg = 0;
	
	[LuaMember("postfix_dmg")]
	public int postfix_dmg = 0;
}

[LuaObject]
public partial class PacketOverwriteGiveBuf
{
	[LuaMember("info_abilitySrcType")]
	public AbilitySourceType info_abilitySrcType = new();
	[LuaMember("postfix_stack")]
	public int postfix_stack = 0;
	[LuaMember("postfix_turn")]
	public int postfix_turn = 0;
	[LuaMember("postfix_activeRound")]
	public int postfix_activeRound = 0;
	[LuaMember("postfix_bufKeyword")]
	public BufKeyword postfix_bufKeyword = new();
}

[LuaObject]
public partial class PacketActionWheel
{
	[LuaMember("info_wheelTiming")]
	public int info_wheelTiming = 0;
	[LuaMember("info_selection")]
	public int info_selection = -1;
	[LuaMember("out_actionType")]
	public int out_actionType = 0;
	[LuaMember("out_optionList")]
	public LuaTable out_optionList = new();
	[LuaMember("out_wheel_radius_scale")]
	public double out_wheel_radius_scale = 1.0;
	[LuaMember("carry_uniqueInfo")]
	public LuaTable carry_uniqueInfo = new();
}

[LuaObject]
public partial class LuaActionWheelOption
{
	public ActionWheelOption ref_obj = null;
	public LuaActionWheelOption(ActionWheelOption x) => ref_obj = x;

	[LuaMember("CreateNew")]
	public static LuaActionWheelOption CreateNew() => new(new ActionWheelOption());
	
	public static LuaTable MakeTable(List<ActionWheelOption> list) {
		int count = list.Count;
		LuaTable table = new LuaTable(count, 0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new LuaActionWheelOption(list[i]);
		}
		return table;
	}
	
	public static List<ActionWheelOption> UnpackTable(LuaTable table) {
		int count = table.ArrayLength;
		List<ActionWheelOption> list = new(count);
		for (int i = 0; i < count; i++) {
			LuaActionWheelOption luaval = table[i + 1].Read<LuaActionWheelOption>();
			list.Add(luaval.ref_obj);
		}
		return list;
	}
	
	[LuaMember("skillID")]
	public int skillID {
		get => ref_obj.skillID;
		set => ref_obj.skillID = value;
	} 
	
	[LuaMember("can_select")]
	public bool can_select {
		get => ref_obj.can_select;
		set => ref_obj.can_select = value;
	} 
	
	[LuaMember("icon_string")]
	public string icon_string {
		get => ref_obj.icon_string;
		set => ref_obj.icon_string = value;
	} 
	
	[LuaMember("icon_scale")]
	public double icon_scale {
		get => ref_obj.icon_scale;
		set => ref_obj.icon_scale = value;
	} 
	
	[LuaMember("text_scale")]
	public double text_scale {
		get => ref_obj.text_scale;
		set => ref_obj.text_scale = value;
	} 
	
	[LuaMember("option_name")]
	public string option_name {
		get => ref_obj.option_name;
		set => ref_obj.option_name = value;
	} 
	
	[LuaMember("option_desc")]
	public string option_desc {
		get => ref_obj.option_desc;
		set => ref_obj.option_desc = value;
	} 
}

public class ActionWheelOption : Il2CppSystem.Object
{
	public ActionWheelOption(IntPtr ptr) : base(ptr) { }

	public ActionWheelOption() : base(ClassInjector.DerivedConstructorPointer<DataMod>())
	{
		ClassInjector.DerivedConstructorBody(this);
	}
	public int skillID = 0;
	public bool can_select = true;
	public string icon_string = "";
	public double icon_scale = 1.0;
	public double text_scale = 1.0;
	public string option_name = "";
	public string option_desc = "";
}