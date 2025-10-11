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
			var filePath = context.GetArgument(0).Read<string>();

			// Security check: ensure the file path is within the plugin directory
			var fullPath = Path.GetFullPath(filePath);
			var pluginPath = MainClass.pluginPath.FullName;

			if (!fullPath.StartsWith(pluginPath, StringComparison.OrdinalIgnoreCase))
			{
				MainClass.Logg.LogError($"Security violation: File path '{filePath}' is outside plugin directory '{pluginPath}'");
				buffer[0] = LuaValue.Nil;
				return ValueTask.FromResult(1);
			}

			// Check if file exists
			if (!File.Exists(fullPath))
			{
				MainClass.Logg.LogWarning($"File not found: '{fullPath}'");
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