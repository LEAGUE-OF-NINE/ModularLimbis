using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace ModularSkillScripts.LuaFunction;

/// <summary>
/// Interface for defining modular Lua functions in the system.
/// </summary>
public interface IModularLuaFunction
{
    /// <summary>
    /// Executes a Lua function based on the provided parameters.
    /// </summary>
    /// <param name="modular">The modular instance, where all the controlling values and helper functions can be found</param>
    /// <param name="context">The Lua function execution context containing arguments</param>
    /// <param name="buffer">Buffer for return values</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The number of return values</returns>
    ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct);
}