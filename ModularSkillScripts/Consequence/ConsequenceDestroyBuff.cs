using System;
using Il2CppSystem.Collections.Generic;
using Lethe.EnumInjections;
using Lethe.Patches;
using Utils;
using CustomBuffs = Lethe.EnumInjections.CustomBuffs;

namespace ModularSkillScripts.Consequence;

public class ConsequenceDestroyBuff : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		List<BattleUnitModel> modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return;
		BattleUnitBuffManager bufManager = Singleton<BattleUnitBuffManager>.Instance;
		int circle_count = circles.Length;
		
		string keyword_s = circles[1];
		int activeRound = modular.GetNumFromParamString(circles[2]);
		int destroy_amount = circle_count > 3 ? modular.GetNumFromParamString(circles[3]) : 1;
		bool include_uncleansable = circle_count > 4;

		BATTLE_EVENT_TIMING timing = modular.battleTiming;
		
		BUF_TYPE buf_type = BUF_TYPE.None;
		BUFF_CATEGORY_KEYWORD buffCat = BUFF_CATEGORY_KEYWORD.NONE;
		bool isBuffType = Enum.TryParse(keyword_s, true, out buf_type);
		bool isBuffCategory = Enum.TryParse(keyword_s, true, out buffCat);

		if (!isBuffType && !isBuffCategory) //single buff destroy mode - (Multi-Target, keyword, destroyRound)
		{
			BUFF_UNIQUE_KEYWORD keyword = CustomBuffs.ParseBuffUniqueKeyword(keyword_s);
			foreach (BattleUnitModel targetModel in modelList) {
				targetModel.ForceToDestroyBuff(keyword, activeRound, timing);
			}
		}
		else //Destroy buff based on buff type mode - (Multi-Target, buffType, destroyRound, amount/times, includeCantBeDespelled? = false)
		{
			foreach (BattleUnitModel targetModel in modelList)
			{
				List<BuffModel> buf_list = targetModel._buffDetail.GetActivatedBuffModelAll();//[0].ForceToDestroyBuff(targetModel, activeRound, timing);
				foreach (BuffModel buf in buf_list)
				{
					if (!include_uncleansable && !buf.CanBeDespelled) continue;
					if (
						(buf_type != BUF_TYPE.None && buf._buffType == buf_type)
						||
						(buffCat != BUFF_CATEGORY_KEYWORD.NONE && buf.HasCategoryKeyword(buffCat))
					) {
						targetModel.ForceToDestroyBuff(buf._buffKeyword._mainUniqueKeyword, activeRound, timing, true);
						destroy_amount -= 1;
						if (destroy_amount <= 0) break;
					}
				}
				
				/*
				List<BuffInfo> bufInfo_list = targetModel.GetBuffInfos(activeRound).CopyList();
				foreach (BuffInfo bufInfo in bufInfo_list)
				{
					if (
						(buf_type != BUF_TYPE.None && bufInfo.GetBuffType() == buf_type)
						||
						(buffCat != BUFF_CATEGORY_KEYWORD.NONE && bufInfo.HasCategoryKeyword(buffCat))
						)
					{
						targetModel.ForceToDestroyBuff(bufInfo._mainKeyword, activeRound, timing, true);
						destroy_amount -= 1;
						if (destroy_amount <= 0) break;
					}
				}*/
				
			}
		}
	}
}