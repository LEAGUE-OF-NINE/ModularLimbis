using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionGetGData : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        var index = context.GetArgument(0).Read<string>();
        buffer[0] = GlobalLuaValues.Instance.GetGlobalValue(index);

        return ValueTask.FromResult(1);
    }
}