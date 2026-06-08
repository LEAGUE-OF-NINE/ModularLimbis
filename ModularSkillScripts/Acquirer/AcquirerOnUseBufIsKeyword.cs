using Lethe.Patches;
using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerOnUseBufIsKeyword : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BUFF_UNIQUE_KEYWORD buf_keyword_used = SkillScriptInitPatch.onusebuf_keyword;
		BUFF_UNIQUE_KEYWORD buf_keyword_check = CustomBuffs.ParseBuffUniqueKeyword(circles[0]);
		BattleUnitBuffManager instance = Singleton<BattleUnitBuffManager>.Instance;
		
		if (circles[1] == "mainandsub") return instance.HasKeyword(buf_keyword_used, buf_keyword_check) ? 1 : 0;
		return buf_keyword_used == buf_keyword_check ? 1 : 0;
	}
}