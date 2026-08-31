using Lethe.Patches;
using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerTimingBufIsKeyword : IModularAcquirer
{
	private int mode = 0;
	public AcquirerTimingBufIsKeyword(int x = 0) => mode = 0;
	
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitBuffManager bufManager = Singleton<BattleUnitBuffManager>.Instance;
		switch (mode)
		{
			case 0: {
				BUFF_UNIQUE_KEYWORD buf_keyword_used = SkillScriptInitPatch.onusebuf_keyword;
				BUFF_UNIQUE_KEYWORD buf_keyword_check = CustomBuffs.ParseBuffUniqueKeyword(circles[0]);
		
				if (circles[1] == "mainandsub") return bufManager.HasKeyword(buf_keyword_used, buf_keyword_check) ? 1 : 0;
				return buf_keyword_used == buf_keyword_check ? 1 : 0;
			}
			case 1: {
				BUFF_UNIQUE_KEYWORD buf_keyword_used = OnGainBuffPatches.ongainbuf_keyword;
				BUFF_UNIQUE_KEYWORD buf_keyword_check = CustomBuffs.ParseBuffUniqueKeyword(circles[0]);
				
				if (circles[1] == "mainandsub") return bufManager.HasKeyword(buf_keyword_used, buf_keyword_check) ? 1 : 0;
				return buf_keyword_used == buf_keyword_check ? 1 : 0;
			}
		}

		return -1;
	}
}