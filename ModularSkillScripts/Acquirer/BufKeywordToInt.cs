using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Lethe.EnumInjections;
using Lethe.Patches;
using CustomBuffs = Lethe.EnumInjections.CustomBuffs;

namespace ModularSkillScripts.Acquirer;

public class AcquirerBufKeywordToInt : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(circledSection);
		return (int)buf_keyword;
	}
}