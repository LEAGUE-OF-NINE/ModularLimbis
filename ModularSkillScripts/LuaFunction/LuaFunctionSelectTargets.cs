using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionSelectTargets : IModularLuaFunction
{
	public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
	{
		var table = new LuaTable();
		var targets = modular.GetTargetModelList(context.GetArgument(0).Read<string>()).ToArray();
		for (int i = 0; i < targets.Length; i++)
		{
			table[i + 1] = $"inst{targets[i].InstanceID}";
		}
		buffer[0] = table;
		return ValueTask.FromResult(1);
	}
}