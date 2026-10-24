using System;
using BattleUI;
using BattleUI.Abnormality;
using BattleUI.BattleUnit;
using BattleUI.Operation;

namespace ModularSkillScripts.Consequence;

public class ConsequenceUnitCutscene : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUIRoot battleUIRoot = SingletonBehavior<BattleUIRoot>.Instance;
		if (!battleUIRoot) {
			MainClass.LogModular("Dude This BattleUIRoot shit is FUCKING NULL");
			return;
		}

		BattleUnitModel unit = modular.modsa_unitModel;
		
		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		BattleUnitView view = objManager.GetView(unit);
		if (!view) return;

		//BattleCutSceneStaticData aa = new BattleCutSceneStaticData();
		//BattleCutScenePivotData pivotData = new();
		StaticDataManager staticDataManager = Singleton<StaticDataManager>.Instance;
		
		bool has_played = false;
		foreach (BattleCutSceneStaticData cutsceneStatic in staticDataManager._battleCutSceneDataList.list)
		{
			//MainClass.LogModular(cutsceneStatic.id, true);
			foreach (BattleCutScenePivotData pivot in cutsceneStatic.pivotList)
			{
				//MainClass.LogModular(pivot._cutsceneType.ToString(), true);
				if (!has_played && cutsceneStatic.id == 10206)
				{
					battleUIRoot.SetCutScene_Produce(true, unit._instanceID, cutsceneStatic, false, SKILL_TYPE.NONE, view._unitDirection);
					view.Appearance.ChangeMotion(MOTION_DETAIL.S3, true, 1);
					has_played = true;
				}
			}
			
			
		}
		
	}

}