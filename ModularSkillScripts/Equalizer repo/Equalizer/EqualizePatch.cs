using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Battle;

namespace Equalizer
{
    class EqualizePatch
    {
        [HarmonyPatch(typeof(StageController), nameof(StageController.InitStageModel))]
        [HarmonyPrefix]
        public static void Prefix_StageController_InitStageModel(StageStaticData stageInfo, StageController __instance)
        {
            int recLevel = stageInfo.RecommendedLevel;
            stageInfo.recommendedLevel = 45;
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
            if (stageInfo._enemyDataDictionary.Count <= 0) return;
            foreach (List<EnemyDataByStage> enemyDataList in stageInfo._enemyDataDictionary.Values)
            {
                foreach (EnemyDataByStage enemyData in enemyDataList)
                {
                    int dataLevel = enemyData._level;
                    int levelDiff = recLevel - dataLevel;
                    enemyData._level = 45 - levelDiff;
                }
            }
        }
    }
}
