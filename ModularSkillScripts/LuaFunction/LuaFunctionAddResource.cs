using Il2CppSystem;
using Lua;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal.UTess;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionAddResource : IModularLuaFunction
{
	public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
	{
		SinManager sinmanager_inst = Singleton<SinManager>.Instance;
		SinManager.EgoStockManager stock_manager = sinmanager_inst._egoStockMangaer;
		ATTRIBUTE_TYPE sin = ATTRIBUTE_TYPE.NONE;
		UNIT_FACTION faction = modular.modsa_unitModel.Faction;
		UNIT_FACTION enemyFaction = faction == UNIT_FACTION.PLAYER ? UNIT_FACTION.ENEMY : UNIT_FACTION.PLAYER;
		if (context.ArgumentCount >= 3) faction = enemyFaction;

		var sinInput = context.GetArgument<string>(0);
		Enum.TryParse(sinInput, true, out sin);
		int amount = context.GetArgument<int>(1);

		if (amount >= 0) stock_manager.AddSinStock(faction, sin, amount, 0);
		else stock_manager.RemoveSinStock(faction, sin, amount * -1);
		return ValueTask.FromResult(1);
	}
}