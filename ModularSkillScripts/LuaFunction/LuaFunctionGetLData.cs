using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionGetLData : IModularLuaFunction
{
	public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
	{
		var target = modular.GetTargetModel(context.GetArgument(0).Read<string>());
		if (target == null) return ValueTask.FromResult(0);
		var key = context.GetArgument(1).Read<string>();
		var dataKey = new LuaUnitDataKey
		{
			unitPtr_intlong = target.Pointer.ToInt64(),
			dataID = key
		};
		buffer[0] = LuaUnitDataKey.LuaUnitValues.TryGetValue(dataKey, out var value) ? value : LuaValue.Nil;
		return ValueTask.FromResult(1);
	}
}