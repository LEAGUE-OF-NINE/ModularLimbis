using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionListFiles : IModularLuaFunction
{
	public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
	{
		try
		{
			var dirPath = context.GetArgument(0).Read<string>();

			// Security check: ensure the directory path is within the plugin directory
			var fullPath = Path.GetFullPath(dirPath);
			var pluginPath = MainClass.pluginPath.FullName;

			if (!fullPath.StartsWith(pluginPath, StringComparison.OrdinalIgnoreCase))
			{
				MainClass.Logg.LogError($"Security violation: Directory path '{dirPath}' is outside plugin directory '{pluginPath}'");
				buffer[0] = LuaValue.Nil;
				return ValueTask.FromResult(1);
			}

			// Check if directory exists
			if (!Directory.Exists(fullPath))
			{
				MainClass.Logg.LogWarning($"Directory not found: '{fullPath}'");
				buffer[0] = LuaValue.Nil;
				return ValueTask.FromResult(1);
			}

			// Get files in directory
			var files = Directory.GetFiles(fullPath);
			var table = new LuaTable();

			for (int i = 0; i < files.Length; i++)
			{
				// Return relative paths from the plugin directory for security
				var relativePath = Path.GetRelativePath(pluginPath, files[i]);
				table[i + 1] = relativePath;
			}

			buffer[0] = table;
			return ValueTask.FromResult(1);
		}
		catch (Exception ex)
		{
			MainClass.Logg.LogError($"Error listing files: {ex.Message}");
			buffer[0] = LuaValue.Nil;
			return ValueTask.FromResult(1);
		}
	}
}