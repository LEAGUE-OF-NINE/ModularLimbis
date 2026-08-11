using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using Lethe.Patches;
using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerVisualBufCheck : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		string mode_s = circles[0];
		BattleLog log = mode_s switch
		{
			"SCT" => SkillScriptInitPatch.battleLog_sct,
			_ => null
		};
		if (log == null) return -1;
		int total = 0;

		string targeting_s = circles[1];
		
		//List<BattleUnitModel> modelList = modular.GetTargetModelList(targeting_s);
		List<BattleUnitModel> modelList = new();
		modelList.Add(modular.modsa_unitModel); // for now, only Self.
		if (modelList.Count < 1) return -1;

		BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[2]);
		BattleUnitBuffManager bufManager = Singleton<BattleUnitBuffManager>.Instance;
		foreach (BattleUnitModel unit in modelList) {
			if (unit == null) continue;
			total += VisualBufCheck(circles, buf_keyword, log._createdBufDict, bufManager);
		}
		return total;
	}

	public int VisualBufCheck(string[] circles, BUFF_UNIQUE_KEYWORD buf_keyword, Dictionary<BATTLE_EVENT_TIMING, List<SubBattleLog_BuffInfo>> log_bufdict, BattleUnitBuffManager bufManager)
	{
		int circles_length = circles.Length;
		int stack = 0;
		int turn = 0;
		
		bool check_more = circles_length > 4;
		if (check_more)
		{
			string circle_3 = circles[4];
			if (circle_3 == "main") {
				foreach (KeyValuePair<BATTLE_EVENT_TIMING, List<SubBattleLog_BuffInfo>> kvp in log_bufdict) {
					List<SubBattleLog_BuffInfo> log_bufinfo_list = kvp.value;
					foreach (SubBattleLog_BuffInfo log_bufinfo in log_bufinfo_list) {
						if (log_bufinfo.uniqueKeyword != buf_keyword) continue;
						stack += log_bufinfo.stack;
						turn += log_bufinfo.turn;
					}
				}
			} else if (circle_3 == "mainandsub") {
				foreach (KeyValuePair<BATTLE_EVENT_TIMING, List<SubBattleLog_BuffInfo>> kvp in log_bufdict) {
					List<SubBattleLog_BuffInfo> log_bufinfo_list = kvp.value;
					foreach (SubBattleLog_BuffInfo log_bufinfo in log_bufinfo_list) {
						if (!bufManager.HasKeyword(log_bufinfo.uniqueKeyword, buf_keyword)) continue;
						stack += log_bufinfo.stack;
						turn += log_bufinfo.turn;
					}
				}
			} else if (circle_3 == "mainandsubandcategory") {
				Il2CppSystem.Enum.TryParse<BUFF_CATEGORY_KEYWORD>(circles[4], out BUFF_CATEGORY_KEYWORD category);
				foreach (KeyValuePair<BATTLE_EVENT_TIMING, List<SubBattleLog_BuffInfo>> kvp in log_bufdict) {
					List<SubBattleLog_BuffInfo> log_bufinfo_list = kvp.value;
					foreach (SubBattleLog_BuffInfo log_bufinfo in log_bufinfo_list) {
						if (!bufManager.HasKeyword(log_bufinfo.uniqueKeyword, buf_keyword) && !bufManager.HasCategory(log_bufinfo.uniqueKeyword, category)) continue;
						stack += log_bufinfo.stack;
						turn += log_bufinfo.turn;
					}
				}
			} else if (circle_3 == "onlycategory") {
				Il2CppSystem.Enum.TryParse<BUFF_CATEGORY_KEYWORD>(circles[4], out BUFF_CATEGORY_KEYWORD category);
				foreach (KeyValuePair<BATTLE_EVENT_TIMING, List<SubBattleLog_BuffInfo>> kvp in log_bufdict) {
					List<SubBattleLog_BuffInfo> log_bufinfo_list = kvp.value;
					foreach (SubBattleLog_BuffInfo log_bufinfo in log_bufinfo_list) {
						if (!bufManager.HasCategory(log_bufinfo.uniqueKeyword, category)) continue;
						stack += log_bufinfo.stack;
						turn += log_bufinfo.turn;
					}
				}
			}
		}
		else
		{
			foreach (KeyValuePair<BATTLE_EVENT_TIMING, List<SubBattleLog_BuffInfo>> kvp in log_bufdict) {
				List<SubBattleLog_BuffInfo> log_bufinfo_list = kvp.value;
				foreach (SubBattleLog_BuffInfo log_bufinfo in log_bufinfo_list) {
					if (!bufManager.HasKeyword(log_bufinfo.uniqueKeyword, buf_keyword)) continue;
					stack += log_bufinfo.stack;
					turn += log_bufinfo.turn;
				}
			}
		}

		return circles[3] switch
		{
			"turn" => turn,
			"+" => stack + turn,
			"*" => stack * turn,
 			_ => stack
		};
	}
}
