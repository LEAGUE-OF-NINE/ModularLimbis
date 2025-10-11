using Il2CppSystem.Collections.Generic;
using Lethe.Patches;
using UnityEngine;
using Utils;

namespace ModularSkillScripts.Patches
{
	public static class SupportPasPatch
	{
		public static void SupportPassiveInit(Dictionary<long, List<ModularSA>> modpaDict)
		{
			if (!MainClass.SupportPasInit)
			{
				TryToFindUnitSupporterAbility();
				MainClass.SupportPasInit = true;
			}
			ChangeSupporterPassiveState(modpaDict);
		}
		public static void TryToFindUnitSupporterAbility()
		{
			BattleObjectManager instance = SingletonBehavior<BattleObjectManager>.Instance;
			List<SupportUnitModel> supporterUnits = new List<SupportUnitModel>();
			foreach (SupportUnitModel supportUnitModel in instance.GetSupportUnitModels(UNIT_FACTION.PLAYER))
			{
				supporterUnits.Add(supportUnitModel);
			}
			foreach (SupportUnitModel supportUnitModel in instance.GetSupportUnitModels(UNIT_FACTION.ENEMY))
			{
				supporterUnits.Add(supportUnitModel);
			}
			foreach (SupportUnitModel supportUnitModel in instance.GetSupportUnitModels(UNIT_FACTION.NONE))
			{
				supporterUnits.Add(supportUnitModel);
			}
			supporterUnits.GetDistinctElements();

			for (int i = 0; i < supporterUnits.Count; i++)
			{
				List<SupporterPassiveModel> passives = supporterUnits[i].PassiveDetail.SupportPassiveList;

				foreach (SupporterPassiveModel supporterPassive in passives)
				{

					if (supporterPassive == null || supporterPassive.ClassInfo.requireIDList.Count == 0)
					{
						continue;
					}

					if (supporterPassive.ClassInfo.requireIDList.Count == 1 && !supporterPassive.ClassInfo.requireIDList[0].StartsWith("Modular/"))
					{
						continue;
					}
					supporterPassive._script = new SupporterPassiveAbility();
					supporterPassive._script._faction = supporterUnits[i].Faction;

					MainClass.supporterPassiveList.Add(supporterPassive);

					for (int j = 0; j < supporterPassive.ClassInfo.requireIDList.Count; j++)
					{
						if (supporterPassive.ClassInfo.requireIDList[j].StartsWith("Modular/"))
						{
							CreateModpa(supporterPassive.ClassInfo.requireIDList[j], supporterPassive);
						}
					}
				}
			}
		}
		public static void ChangeSupporterPassiveState(Dictionary<long, List<ModularSA>> modpaDict)
		{
			MainClass.activeSupporterPassiveList.Clear();
			foreach (SupporterPassiveModel supporterPassiveModel in MainClass.supporterPassiveList)
			{
				if (IsCustomSupportConditionsTrue(supporterPassiveModel))
				{
					//supporterPassiveModel._script._status = PASSIVE_STATUS.ACTIVE;
					//supporterPassiveModel.GetSatisfiedResonanceStatus();
					long ptr_intlong = supporterPassiveModel.Pointer.ToInt64();
					if (modpaDict.ContainsKey(ptr_intlong)) MainClass.activeSupporterPassiveList.Add(supporterPassiveModel);
				}
			}
		}
		public static bool IsCustomSupportConditionsTrue(SupporterPassiveModel supporterPassive)
		{
			SinManager sinManager = Singleton<SinManager>.Instance;
			SinManager.EgoStockManager stockManager = sinManager._egoStockMangaer;
			SinManager.ResonanceManager resManager = Singleton<SinManager>.Instance._resManager;

			List<PassiveConditionStaticData> stockCondition = supporterPassive.GetAttributeStockCondition();
			List<PassiveConditionStaticData> resCondition = supporterPassive.GetAttributeResonanceCondition();

			int stockConditionCount = stockCondition.Count;
			int fullFilledStockCondition = 1;

			int resConditionCount = resCondition.Count;
			int fullFilledResCondition = 1;

			if (stockCondition.Count > 0)
			{
				fullFilledStockCondition = 0;
				foreach (PassiveConditionStaticData stockData in stockCondition)
				{
					int dataValue = stockData.Value;
					int stockValue = stockManager.GetAttributeStockNumberByAttributeType(supporterPassive._script._faction, stockData.AttributeType);
					if (stockValue >= dataValue)
					{
						fullFilledStockCondition++;
					}
				}
			}

			if (stockCondition.Count > 0)
			{
				fullFilledResCondition = 1;
				foreach (PassiveConditionStaticData resData in resCondition)
				{
					int dataValue = resData.Value;
					int resValue = resManager.GetAttributeResonance(supporterPassive._script._faction, resData.AttributeType);
					if (resValue >= dataValue)
					{
						fullFilledResCondition++;
					}
				}
			}
			if (fullFilledStockCondition >= stockConditionCount && fullFilledResCondition >= resConditionCount)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void CreateModpa(string param, SupporterPassiveModel supporterPassiveModel)
		{
			long ptr = supporterPassiveModel.Pointer.ToInt64();

			var modpa = new ModularSA();
			modpa.originalString = param;
			modpa.ptr_intlong = ptr;
			modpa.passiveID = supporterPassiveModel.ClassInfo.id;
			modpa.abilityMode = 2;
			modpa.ResetAdders();

			modpa.SetupModular(param.Remove(0, 8));
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(ptr)) SkillScriptInitPatch.modpaDict.Add(ptr, new List<ModularSA>());
			SkillScriptInitPatch.modpaDict[ptr].Add(modpa);
		}
	}
}
