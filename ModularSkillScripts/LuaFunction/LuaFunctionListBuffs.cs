using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionListBuffs : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        BattleUnitModel targetModel = modular.GetTargetModel(context.GetArgument(0).Read<string>());
        if (targetModel == null) return ValueTask.FromResult(0);

        var buffList = targetModel.GetBuffAllThisRoundOnly(modular.battleTiming).ToArray();
        var table = new LuaTable();

        for (int i = 0; i < buffList.Length; i++)
        {
            var stringB = buffList[i].GetKeyword().ToString();
            table[i + 1] = stringB;
        }

        buffer[0] = table;
        return ValueTask.FromResult(1);
    }
}