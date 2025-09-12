using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lua;
using Lua.Runtime;

namespace ModularSkillScripts;

public class LuaScript
{
	public string Body;
	public string Name;
	public Chunk Content;
	
	public static Dictionary<string, LuaScript> loadedScripts = new();
	
	public class ModularLuaModuleLoader : ILuaModuleLoader
	{
		public bool Exists(string moduleName)
		{
			return loadedScripts.ContainsKey(moduleName);
		}

		public ValueTask<LuaModule> LoadAsync(string moduleName, CancellationToken cancellationToken = new())
		{
			return new ValueTask<LuaModule>(new LuaModule(moduleName, loadedScripts[moduleName].Body));
		}
	}
}