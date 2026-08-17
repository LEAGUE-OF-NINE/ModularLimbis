using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using Lethe.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerSumGetter : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (circles.Length < 3) {
			MainClass.LogModular("SumGetter not enough circles", true);
			return -1;
		}

		if (!MainClass.acquirerDict.TryGetValue(circles[0], out IModularAcquirer acquirer)) {
			MainClass.LogModular("SumGetter invalid getter circle", true);
			return -1;
		}
		
		List<BattleUnitModel> modelList = modular.GetTargetModelList(circles[1]);
		if (modelList.Count < 1) {
			MainClass.LogModular("SumGetter unit list is empty", true);
			return -1;
		}

		string[] circles_new = circles.Skip(2).ToArray();
		int total = 0;
		
		foreach (BattleUnitModel unit in modelList) {
			if (unit == null) continue;
			total += acquirer.ExecuteAcquirer(modular, section, circledSection, circles);
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
