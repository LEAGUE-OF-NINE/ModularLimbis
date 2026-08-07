using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Lethe.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerBufCheck : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int total = 0;
		var modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return -1;

		BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
		BattleUnitBuffManager bufManager = Singleton<BattleUnitBuffManager>.Instance;
		foreach (BattleUnitModel unit in modelList)
		{
			if (unit == null) continue;
			total += BufCheck(unit, circles, buf_keyword, bufManager);
		}

		return total;
	}

	public int BufCheck(BattleUnitModel targetModel, string[] circles, BUFF_UNIQUE_KEYWORD buf_keyword, BattleUnitBuffManager bufManager)
	{
		int circles_length = circles.Length;
		BuffDetail bufDetail = targetModel._buffDetail;
		int stack = 0;
		int turn = 0;
		
		bool check_more = circles_length > 3;
		if (check_more)
		{
			string circle_3 = circles[3];
			if (circle_3 == "main") {
				stack = bufDetail.GetActivatedBuffStack(buf_keyword, true);
				turn = bufDetail.GetActivatedBuffTurn(buf_keyword, true);
			} else if (circle_3 == "mainandsub") {
				stack = bufDetail.GetActivatedBuffStack(buf_keyword, false);
				turn = bufDetail.GetActivatedBuffTurn(buf_keyword, false);
			} else if (circle_3 == "mainandsubandcategory") {
				BUFF_CATEGORY_KEYWORD category = BUFF_CATEGORY_KEYWORD.NONE;
				Il2CppSystem.Enum.TryParse<BUFF_CATEGORY_KEYWORD>(circles[4], out category);
				foreach (BuffModel buf in targetModel.GetActivatedBuffModels())
				{
					if (buf.HasCategoryKeyword(category) || buf.IsKeyword(buf_keyword))
					{
						stack += buf.GetStack(0);
						turn += buf.GetTurn(0);
					}
				}
			} else if (circle_3 == "onlycategory") {
				BUFF_CATEGORY_KEYWORD category = BUFF_CATEGORY_KEYWORD.NONE;
				Il2CppSystem.Enum.TryParse<BUFF_CATEGORY_KEYWORD>(circles[4], out category);
				stack = bufDetail.GetActivatedBuffStack(category, new Il2CppStructArray<BUFF_UNIQUE_KEYWORD>([BUFF_UNIQUE_KEYWORD.None]));
				turn = bufDetail.GetActivatedBuffTurn(category, new Il2CppStructArray<BUFF_UNIQUE_KEYWORD>([BUFF_UNIQUE_KEYWORD.None]));
			}
		}
		else
		{
			stack = bufDetail.GetActivatedBuffStack(buf_keyword, false);
			turn = bufDetail.GetActivatedBuffTurn(buf_keyword, false);
		}

		return circles[2] switch
		{
			"turn" => turn,
			"+" => stack + turn,
			"*" => stack * turn,
			"consumed" => bufManager.GetUsedBuffTurn(targetModel.InstanceID, buf_keyword), // keep compatibility
			"turnConsumed" => bufManager.GetUsedBuffTurn(targetModel.InstanceID, buf_keyword),
			"stackConsumed" => bufManager.GetUsedBuffStack(targetModel.InstanceID, buf_keyword),
 			_ => stack
		};
	}
}
