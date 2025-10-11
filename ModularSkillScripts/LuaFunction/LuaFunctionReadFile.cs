using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionReadFile : IModularLuaFunction
{
	public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
	{
		try
		{
			var relativePath = context.GetArgument(0).Read<string>();
			var pluginPath = MainClass.pluginPath.FullName;

			// Combine plugin path with the relative path
			var fullPath = Path.Combine(pluginPath, relativePath);

			// Normalize the path and ensure it's still within plugin directory (security check)
			fullPath = Path.GetFullPath(fullPath);
			if (!fullPath.StartsWith(pluginPath, StringComparison.OrdinalIgnoreCase))
			{
				MainClass.Logg.LogError($"Security violation: File path '{relativePath}' resolves outside plugin directory");
				buffer[0] = LuaValue.Nil;
				return ValueTask.FromResult(1);
			}

			// Check if file exists
			if (!File.Exists(fullPath))
			{
				MainClass.Logg.LogWarning($"File not found: '{relativePath}' (resolved to '{fullPath}')");
				buffer[0] = LuaValue.Nil;
				return ValueTask.FromResult(1);
			}

			// Read file content
			var content = File.ReadAllText(fullPath);
			buffer[0] = new LuaValue(content);
			return ValueTask.FromResult(1);
		}
		catch (Exception ex)
		{
			MainClass.Logg.LogError($"Error reading file: {ex.Message}");
			buffer[0] = LuaValue.Nil;
			return ValueTask.FromResult(1);
		}
	}
}