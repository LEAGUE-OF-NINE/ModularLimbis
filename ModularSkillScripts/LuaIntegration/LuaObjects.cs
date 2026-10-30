using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Lethe.EnumInjections;
using Lua;
using ModularSkillScripts.Patches;

namespace ModularSkillScripts.LuaIntegration;

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
	public bool IsNull() {
		return ref_unitModel == null;
	}
	
	[LuaMember("WhatAmI")]
	public int WhatAmI() {
		if (!ref_unitModel.IsAbnormalityOrPart) return 0;
		BattleUnitModel_Abnormality_Part part = ref_unitModel.TryCast<BattleUnitModel_Abnormality_Part>();
		return part == null ? 1 : 2;
	}
	
	[LuaMember("UnitID")]
	public int GetUnitID() {
		return ref_unitModel.GetUnitID();
	}
	
	[LuaMember("InstanceID")]
	public int GetInstanceID() {
		return ref_unitModel.InstanceID;
	}
	
	[LuaMember("HP")]
	public int HPCheck() {
		return ref_unitModel.Hp;
	}
	
	[LuaMember("MaxHP")]
	public int MaxHPCheck() {
		return ref_unitModel.MaxHp;
	}
	
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
}

[LuaObject]
public partial class BufModel
{
	public BuffModel ref_buffModel = null;
	public BufModel(BuffModel x) {
		ref_buffModel = x;
	}
	
	[LuaMember("IsNull")]
	public bool IsNull() {
		return ref_buffModel == null;
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
	public string KeywordToString(int integ) {
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
	public string KeywordToString(int integ) {
		return ref_buf_category.ToString();
	}
}

[LuaObject]
public partial class BufManager
{
	[LuaMember("CompareBufKeyword")]
	public static bool CompareBufKeyword(BufKeyword keyword_main, BufKeyword keyword_another, string mode_s) {
		if (mode_s == "main_and_sub") {
			BattleUnitBuffManager bufManager = Singleton<BattleUnitBuffManager>.Instance;
			return bufManager.HasKeyword(keyword_main.ref_buf_keyword, keyword_another.ref_buf_keyword);
		}
		return keyword_main.ref_buf_keyword == keyword_another.ref_buf_keyword;
	}
	
	[LuaMember("KeywordHasCategory")]
	public static bool KeywordHasCategory(BufKeyword keyword_main, BufCategory category_another, string mode_s) {
		BattleUnitBuffManager bufManager = Singleton<BattleUnitBuffManager>.Instance;
		return bufManager.HasCategory(keyword_main.ref_buf_keyword, category_another.ref_buf_category);
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
	public string KeywordToString(int integ) {
		return ref_timing.ToString();
	}
}