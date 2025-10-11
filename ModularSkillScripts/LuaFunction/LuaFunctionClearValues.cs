using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionClearValues : IModularLuaFunction
{
	public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
	{
		modular.ResetValueList();
		return ValueTask.FromResult(0);
	}
}