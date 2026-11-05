using System;
using System.Threading;
using System.Threading.Tasks;
using Il2CppSystem.Collections.Generic;
using Lua;
using Lua.Standard;
using ModularSkillScripts.Patches;


namespace ModularSkillScripts.LuaIntegration;

public sealed class LuaFunctions
{
	public static LuaFunctions Instance = new();
	public readonly Lua.LuaFunction[] F_Util;
	public readonly LuaTable F_Util_table;
	public readonly Lua.LuaFunction[] F_BufManager;
	public readonly LuaTable F_BufManager_table;
	public readonly Lua.LuaFunction[] F_BattleObjManager;
	public readonly LuaTable F_BattleObjManager_table;
	public readonly Lua.LuaFunction[] F_CoinModel;
	public readonly LuaTable F_CoinModel_table;

  public LuaFunctions()
  {
	  F_Util =
    [
	    new Lua.LuaFunction("ShuffleList", ShuffleList),
	    new Lua.LuaFunction("GetterRandom", GetterRandom),
	    new Lua.LuaFunction("FetchComplexEncounterData", FetchComplexEncounterData)
    ];
	  F_BufManager =
	  [
		  new Lua.LuaFunction("CompareBufKeyword", BufManager_CompareBufKeyword),
		  new Lua.LuaFunction("KeywordHasCategory", BufManager_KeywordHasCategory)
	  ];
	  F_BattleObjManager =
	  [
		  new Lua.LuaFunction("GetAliveList", BattleObjectManager_GetAliveList),
		  new Lua.LuaFunction("GetAliveListByBuf", BattleObjectManager_GetAliveListByBuf),
		  new Lua.LuaFunction("GetView", BattleObjectManager_GetView)
	  ];
	  F_CoinModel =
	  [
		  new Lua.LuaFunction("GetAliveList", BattleObjectManager_GetAliveList),
		  new Lua.LuaFunction("GetAliveListByBuf", BattleObjectManager_GetAliveListByBuf),
		  new Lua.LuaFunction("GetView", BattleObjectManager_GetView)
	  ];
	  
	  F_Util_table = InstantiateLuaTableForFunctions(F_Util);
	  F_BufManager_table = InstantiateLuaTableForFunctions(F_BufManager);
	  F_BattleObjManager_table = InstantiateLuaTableForFunctions(F_BattleObjManager);
	  F_CoinModel_table = InstantiateLuaTableForFunctions(F_BattleObjManager);
	  
  }

  public static LuaTable InstantiateLuaTableForFunctions(Lua.LuaFunction[] f_array)
  {
	  LuaTable luaTable = new (0, f_array.Length);
	  foreach (Lua.LuaFunction function in f_array) {
		  luaTable[(LuaValue) function.Name] = (LuaValue) function;
	  }
	  return luaTable;
  }
  
  public static void MergeTableToLuaState(string funnyName, LuaState state, LuaTable table)
  {
	  state.Environment[(LuaValue) funnyName] = (LuaValue) table;
	  state.LoadedModules[(LuaValue) funnyName] = (LuaValue) table;
  }
	
	public ValueTask<int> ShuffleList(
		LuaFunctionExecutionContext context,
		Memory<LuaValue> buffer,
		CancellationToken cancellationToken)
	{
		LuaTable table = context.GetArgument<LuaTable>(0);
		for (int i = table.ArrayLength; i >= 2; i--) {
			int k = MainClass.rng.Next(i) + 1;
			if (k != i) (table[k], table[i]) = (table[i], table[k]);
		}
		return new ValueTask<int>(0);
	}
	
	public ValueTask<int> GetterRandom(
		LuaFunctionExecutionContext context,
		Memory<LuaValue> buffer,
		CancellationToken cancellationToken)
	{
		int minRoll = context.GetArgument<int>(0);
		int maxRoll = context.GetArgument<int>(1);
		buffer.Span[0] = (LuaValue) MainClass.rng.Next(minRoll, maxRoll + 1);
		return new ValueTask<int>(1);
	}
	
	public ValueTask<int> FetchComplexEncounterData(
		LuaFunctionExecutionContext context,
		Memory<LuaValue> buffer,
		CancellationToken cancellationToken)
	{
		buffer.Span[0] = SkillScriptInitPatch.complex_lua_encounter_data;
		return new ValueTask<int>(1);
	}
	
	public ValueTask<int> BufManager_CompareBufKeyword(
		LuaFunctionExecutionContext context,
		Memory<LuaValue> buffer,
		CancellationToken cancellationToken)
	{
		BufKeyword keyword_main = context.GetArgument<BufKeyword>(0);
		BufKeyword keyword_another = context.GetArgument<BufKeyword>(1);
		string mode_s = context.GetArgument<string>(2);
		
		if (mode_s == "main_and_sub") {
			BattleUnitBuffManager bufManager = Singleton<BattleUnitBuffManager>.Instance;
			buffer.Span[0] = bufManager.HasKeyword(keyword_main.ref_buf_keyword, keyword_another.ref_buf_keyword);
		}
		else buffer.Span[0] = keyword_main.ref_buf_keyword == keyword_another.ref_buf_keyword;
		return new ValueTask<int>(1);
	}
	
	public ValueTask<int> BufManager_KeywordHasCategory(
		LuaFunctionExecutionContext context,
		Memory<LuaValue> buffer,
		CancellationToken cancellationToken)
	{
		BufKeyword keyword_main = context.GetArgument<BufKeyword>(0);
		BufCategory category_another = context.GetArgument<BufCategory>(1);
		
		BattleUnitBuffManager bufManager = Singleton<BattleUnitBuffManager>.Instance;
		buffer.Span[0] = bufManager.HasCategory(keyword_main.ref_buf_keyword, category_another.ref_buf_category);
		return new ValueTask<int>(1);
	}
	
	public ValueTask<int> BattleObjectManager_GetAliveList(
		LuaFunctionExecutionContext context,
		Memory<LuaValue> buffer,
		CancellationToken cancellationToken)
	{
		bool include_assistance = context.GetArgument<bool>(0);
		int faction_int = context.GetArgument<int>(1);
		
		UNIT_FACTION faction = (UNIT_FACTION)faction_int;
		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		List<BattleUnitModel> unit_list = objManager.GetAliveList(include_assistance, faction);
		int count = unit_list.Count;
		LuaTable table = new(count,0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new UnitModel(unit_list[i]);
		}
		buffer.Span[0] = table;
		return new ValueTask<int>(1);
	}
	
	public ValueTask<int> BattleObjectManager_GetAliveListByBuf(
		LuaFunctionExecutionContext context,
		Memory<LuaValue> buffer,
		CancellationToken cancellationToken)
	{
		BufKeyword bufKeyword = context.GetArgument<BufKeyword>(0);
		int activeRound = context.GetArgument<int>(1);
		bool include_assistance = context.GetArgument<bool>(2);
		int faction_int = context.GetArgument<int>(3);
		
		BUFF_UNIQUE_KEYWORD keyword = bufKeyword.ref_buf_keyword;
		UNIT_FACTION faction = (UNIT_FACTION)faction_int;
		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		List<BattleUnitModel> unit_list = objManager.GetAliveList(keyword, activeRound, include_assistance, faction);
		int count = unit_list.Count;
		LuaTable table = new(count,0);
		for (int i = 0; i < count; i++) {
			table[i + 1] = new UnitModel(unit_list[i]);
		}
		buffer.Span[0] = table;
		return new ValueTask<int>(1);
	}
	
	public ValueTask<int> BattleObjectManager_GetView(
		LuaFunctionExecutionContext context,
		Memory<LuaValue> buffer,
		CancellationToken cancellationToken)
	{
		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		BattleUnitView view = null;
		LuaValue arg = context.GetArgument(0);
		if (arg.TryRead(out int instID)) {
			view = objManager.GetView(instID);
		} else if (arg.TryRead(out UnitModel unit)) {
			view = objManager.GetView(unit.ref_unitModel);
		}
		LuaUnitView luaView = new(view);
		buffer.Span[0] = luaView;
		return new ValueTask<int>(1);
	}
	
}

