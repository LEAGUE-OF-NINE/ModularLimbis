using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System;
using BepInEx;
using Lua;

namespace ModularSkillScripts.LuaFunction;

public class LuaFunctionListDirectories : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        try //i copy and edit codes from listfiles()
        {
            var relativeDirPath = context.GetArgument(0).Read<string>();
            var pluginPath = Paths.PluginPath;

            // Join plugin path with the relative directory path (safer than Combine)
            var fullPath = Path.Join(pluginPath, relativeDirPath);

            // Normalize the path and ensure it's still within plugin directory (security check)
            fullPath = Path.GetFullPath(fullPath);
            if (!fullPath.StartsWith(pluginPath, StringComparison.OrdinalIgnoreCase))
            {
                MainClass.Logg.LogError($"Security violation: Directory path '{relativeDirPath}' resolves outside plugin directory");
                buffer[0] = LuaValue.Nil;
                return ValueTask.FromResult(1);
            }

            // Check if directory exists
            if (!Directory.Exists(fullPath))
            {
                MainClass.Logg.LogWarning($"Directory not found: '{relativeDirPath}' (resolved to '{fullPath}')");
                buffer[0] = LuaValue.Nil;
                return ValueTask.FromResult(1);
            }

            // Get directories in directory
            var files = Directory.GetDirectories(fullPath);
            var table = new LuaTable();

            for (int i = 0; i < files.Length; i++)
            {
                // Return relative paths from the plugin directory
                var relativePath = Path.GetRelativePath(pluginPath, files[i]);
                table[i + 1] = relativePath;
            }

            buffer[0] = table;
            return ValueTask.FromResult(1);
        }
        catch (Exception ex)
        {
            MainClass.Logg.LogError($"Error listing directories: {ex.Message}");
            buffer[0] = LuaValue.Nil;
            return ValueTask.FromResult(1);
        }
    }
}