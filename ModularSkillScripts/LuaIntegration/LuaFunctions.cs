using System;
using System.Threading;
using System.Threading.Tasks;
using Lua;
using Lua.Standard;
using ModularSkillScripts.Patches;


namespace ModularSkillScripts.LuaIntegration;

public sealed class LuaFunctions
{
	public static LuaFunctions Instance = new();
	public readonly Lua.LuaFunction[] F_Util;
	public readonly Lua.LuaFunction[] F_BufManager;

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
  }

  public static void MergeFunctionArrayToLuaState(string funnyName, LuaState state, Lua.LuaFunction[] f_array)
  {
	  LuaTable luaTable = new (0, f_array.Length);
	  foreach (Lua.LuaFunction function in f_array) {
		  luaTable[(LuaValue) function.Name] = (LuaValue) function;
	  }
	  state.Environment[(LuaValue) funnyName] = (LuaValue) luaTable;
	  state.LoadedModules[(LuaValue) funnyName] = (LuaValue) luaTable;
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
	
}

