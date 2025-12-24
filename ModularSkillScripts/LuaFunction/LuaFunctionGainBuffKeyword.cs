using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionGainBuffKeyword : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        if (modular.modsa_passiveModel != null)
        {
            buffer[0] = modular.keywordTrigger.ToString();
            return ValueTask.FromResult(1);
        }
        return ValueTask.FromResult(0);
    }
}