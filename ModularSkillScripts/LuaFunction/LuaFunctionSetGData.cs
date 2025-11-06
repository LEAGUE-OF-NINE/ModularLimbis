using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionSetGData : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        var index = context.GetArgument(0).Read<string>();
        var val = context.GetArgument(1);
        GlobalLuaValues.Instance.SetGlobalValue(index, val);

        return ValueTask.FromResult(0);
    }
}