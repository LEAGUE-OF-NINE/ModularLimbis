using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionResetAdders : IModularLuaFunction
{
	public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
	{
		modular.ResetAdders();
		return ValueTask.FromResult(0);
	}
}