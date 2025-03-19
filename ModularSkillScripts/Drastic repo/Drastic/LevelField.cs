using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using BattleUI;
using BattleUI.Operation;
using Dungeon;
using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using static BattleUI.Abnormality.AbnormalityPartSkills;
using static MirrorDungeonSelectThemeUIPanel.UIResources;
using CodeStage.AntiCheat.ObscuredTypes;
using MainUI;
using Battle;

namespace Drastic
{
    class LevelField
    {
        //[HarmonyPatch(typeof(BattleUnitModel), "Level", MethodType.Getter)]
        //[HarmonyPostfix]
        //public static void Postfix_BattleUnitModel_LevelGetter(ref ObscuredInt __result, BattleUnitModel __instance)
        //{
        //    if (__instance.IsFaction(UNIT_FACTION.PLAYER))
        //    {
        //        __result = 45;
        //    }
        //    else
        //    {
        //        int dataLevel = __instance.UnitDataModel.Level;

        //        StageController stageController_inst = Singleton<StageController>.Instance;
        //        if (stageController_inst == null) return;
        //        StageModel stageModel = stageController_inst._stageModel;
        //        if (stageModel == null) return;
        //        StageStaticData stageData = stageModel.ClassInfo;
        //        if (stageData == null) return;
        //        int recLevel = stageData.RecommendedLevel;
        //        int levelDiff = recLevel - dataLevel;

        //        __result = System.Math.Max(45 - levelDiff, 1);
        //    }
        //}

        //[HarmonyPatch(typeof(UnitDataModel), "Level", MethodType.Getter)]
        //[HarmonyPostfix]
        //public static void Postfix_UnitDataModel_LevelGetter(ref int __result, UnitDataModel __instance)
        //{
        //    if (__instance._defenseSkillIDList.Count > 0)
        //    {
        //        __result = 45;
        //    }
        //    else
        //    {
        //        int dataLevel = __instance.Level;
        //        __result = 45;

        //        StageController stageController_inst = Singleton<StageController>.Instance;
        //        if (stageController_inst == null) return;
        //        StageModel stageModel = stageController_inst._stageModel;
        //        if (stageModel == null) return;
        //        StageStaticData stageData = stageModel.ClassInfo;
        //        if (stageData == null) return;
        //        int recLevel = stageData.RecommendedLevel;
        //        int levelDiff = recLevel - dataLevel;

        //        __result = System.Math.Max(45 - levelDiff, 1);
        //    }
        //}

        //[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.GetLevelAdder))]
        //[HarmonyPostfix]
        //public static void Postfix_BattleUnitModel_GetLevelAdder(ref PersonalityAdderInfo __result, BattleUnitModel __instance)
        //{
        //    //EGOGiftAbility_ParticipantOrderBase egogift = new EGOGiftAbility_ParticipantOrderBase();
        //    if (__instance.IsFaction(UNIT_FACTION.PLAYER)) return;

        //    StageController stageController_inst = Singleton<StageController>.Instance;
        //    StageModel stageModel = stageController_inst._stageModel;
        //    StageStaticData stageData = stageModel.ClassInfo;
        //    int recLevel = stageData.RecommendedLevel;
        //    int dataLevel = __instance.Level;
        //    int levelDiff = recLevel - dataLevel;
        //    EGOGiftAbility gift = new EGOGiftAbility("stake", 9118, 9118, ATTRIBUTE_TYPE.VIOLET, 9118);
        //    EachAdderInfo eachadder = new EachAdderInfo(gift, 45 - dataLevel - levelDiff);

        //    if (__result._hiddenAdderList.ContainsKey(3585)) __result._hiddenAdderList[3585] = eachadder;
        //    else __result._hiddenAdderList.Add(3585, eachadder);
        //}

        //[HarmonyPatch(typeof(UnitDataModel), nameof(UnitDataModel.InitData))]
        //[HarmonyPrefix]
        //public static void Prefix_UnitDataModel_InitData(UnitStaticData data, ref int level, BattleUnitModel __instance)
        //{
        //    if (data.defenseSkillIDList.Count > 0)
        //    {
        //        level = 45;
        //        return;
        //    }

        //    int dataLevel = level;
        //    level = 45;
        //    StageController stageController_inst = Singleton<StageController>.Instance;
        //    if (stageController_inst == null) return;
        //    StageModel stageModel = stageController_inst._stageModel;
        //    if (stageModel == null) return;
        //    StageStaticData stageData = stageModel.ClassInfo;
        //    if (stageData == null) return;
        //    int recLevel = stageData.RecommendedLevel;
        //    int levelDiff = recLevel - dataLevel;

        //    level = 45 - levelDiff;

        //}

        [HarmonyPatch(typeof(StageController), nameof(StageController.InitStageModel))]
        [HarmonyPrefix]
        public static void Prefix_StageController_InitStageModel(StageStaticData stageInfo, StageController __instance)
        {
            int recLevel = stageInfo.RecommendedLevel;
            foreach (Wave wave in stageInfo.waveList)
            {
                foreach (EnemyData enemyData in wave.unitList)
                {
                    int dataLevel = enemyData.unitLevel;
                    int levelDiff = recLevel - dataLevel;
                    enemyData.unitLevel = 45 - levelDiff;
                }
                foreach (EnemyData enemyData in wave.subUnitList)
                {
                    int dataLevel = enemyData.unitLevel;
                    int levelDiff = recLevel - dataLevel;
                    enemyData.unitLevel = 45 - levelDiff;
                }
                foreach (EnemyData enemyData in wave.supportUnitList)
                {
                    int dataLevel = enemyData.unitLevel;
                    int levelDiff = recLevel - dataLevel;
                    enemyData.unitLevel = 45 - levelDiff;
                }
            }
            foreach (List<EnemyDataByStage> enemyDataList in stageInfo._enemyDataDictionary.Values)
            {
                foreach (EnemyDataByStage enemyData in enemyDataList)
                {
                    int dataLevel = enemyData._level;
                    int levelDiff = recLevel - dataLevel;
                    enemyData._level = 45 - levelDiff;
                }
            }
            stageInfo.recommendedLevel = 45;
        }

        //[HarmonyPatch(typeof(StageModel), nameof(StageModel.))]
        //[HarmonyPrefix]
        //private static void Prefix_StageModel_Init(StageStaticData stageinfo, StageModel __instance)
        //{
        //    int recLevel = stageinfo.RecommendedLevel;

        //    //List<Wave> waveList = stageinfo.waveList;

        //    foreach (Wave wave in stageinfo.waveList)
        //    {
        //        foreach (EnemyData enemyData in wave.unitList)
        //        {
        //            int dataLevel = enemyData.unitLevel;
        //            int levelDiff = recLevel - dataLevel;
        //            enemyData.unitLevel = System.Math.Max(45 - levelDiff, 1);
        //        }
        //        foreach (EnemyData enemyData in wave.subUnitList)
        //        {
        //            int dataLevel = enemyData.unitLevel;
        //            int levelDiff = recLevel - dataLevel;
        //            enemyData.unitLevel = System.Math.Max(45 - levelDiff, 1);
        //        }
        //    }
        //}

        // end
    }
}
