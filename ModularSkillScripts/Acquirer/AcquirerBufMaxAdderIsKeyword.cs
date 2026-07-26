using Lethe.Patches;
using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerBufMaxAdderIsKeyword : IModularAcquirer
{
	private int mode = 0;
	public AcquirerBufMaxAdderIsKeyword(int x = 0)
	{
		mode = x;
	}
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BUFF_UNIQUE_KEYWORD buf_keyword_used = BUFF_UNIQUE_KEYWORD.None;
		BUFF_UNIQUE_KEYWORD buf_keyword_check = CustomBuffs.ParseBuffUniqueKeyword(circles[0]);
		BattleUnitBuffManager instance = Singleton<BattleUnitBuffManager>.Instance;
		
		switch (mode)
		{
			case 0: {
				buf_keyword_used = SkillScriptInitPatch.keyword_BufMaxStackAdder;
			} break;
			case 1: {
				buf_keyword_used = SkillScriptInitPatch.keyword_BufMaxTurnAdder;
			} break;
		}
		
		string circle_1 = circles[1];
		if (circle_1 == "sub") return instance.HasKeyword(buf_keyword_used, buf_keyword_check) ? 1 : 0;
		if (circle_1 == "category")
		{
			BUFF_CATEGORY_KEYWORD category = BUFF_CATEGORY_KEYWORD.NONE;
			Il2CppSystem.Enum.TryParse<BUFF_CATEGORY_KEYWORD>(circles[2], out category);
			return instance.HasKeyword(buf_keyword_used, buf_keyword_check) || instance.HasCategory(buf_keyword_used, category) ? 1 : 0;
		}
		return buf_keyword_used == buf_keyword_check ? 1 : 0;
	}
}