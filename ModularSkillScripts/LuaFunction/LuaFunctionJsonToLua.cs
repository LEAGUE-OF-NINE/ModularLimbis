using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionJsonToLua : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {

        var rawStr = context.GetArgument(0).Read<string>();
        buffer[0] = Decode.decode(rawStr);
        return ValueTask.FromResult(1);
    }
}