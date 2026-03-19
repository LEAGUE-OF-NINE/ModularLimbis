using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using Lua;
using System.Threading.Tasks;
using System.Threading;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionListBreakSectionValue : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        BattleUnitModel target = modular.GetTargetModel(context.GetArgument(0).Read<string>());
        if (target == null) return ValueTask.FromResult(0);

        bool isActiveOnly = context.GetArgument(1).Read<bool>();
        Il2CppSystem.Collections.Generic.List<int> breakSectionValue = target.GetBreakSectionValueList(isActiveOnly);

        var table = new LuaTable();

        for (int i = 0; i < breakSectionValue.Count; i++)
        {
            table[i + 1] = breakSectionValue[i];
        }

        buffer[0] = table;
        return ValueTask.FromResult(1);
    }
}