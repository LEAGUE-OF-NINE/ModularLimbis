using BattleUI.Operation;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using BepInEx.Unity.IL2CPP.UnityEngine;

namespace ModularSkillScripts.Patches;

internal class UniquePatches
{
	[HarmonyPatch(typeof(NewOperationController), nameof(NewOperationController.EquipDefense))]
	[HarmonyPrefix]
	private static bool Postfix_NewOperationController_EquipDefense(bool equiped, SinActionModel sinAction)
	{
		if (!Input.GetKeyInt(KeyCode.LeftControl)) return true;

		BattleUnitModel unit = sinAction.actionSlot.Owner;
		if (!unit.IsActionable()) return true;
		int actevent = MainClass.timingDict["SpecialAction"];
		foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong]) {
				MainClass.Logg.LogInfo("Found modpassive - SPECIAL: " + modpa.passiveID);
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(passiveModel.Owner, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		foreach (PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
			{
				MainClass.Logg.LogInfo("Found modpassive - SPECIAL: " + modpa.passiveID);
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(passiveModel.Owner, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}

		//MainClass.Logg.LogInfo("1");
		//BattleUnitViewManager viewManager = SingletonBehavior<BattleUnitViewManager>.Instance;
		//MainClass.Logg.LogInfo("2");
		//foreach (BattleUnitView unitView in viewManager._unitViewList)
		//{
		//	MainClass.Logg.LogInfo("3");
		//	unitView.RefreshState(unitView.unitModel);
		//	MainClass.Logg.LogInfo("4");
		//	unitView.RefreshEffects();
		//}

		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		objManager.UpdatePassiveState();
		objManager.OnRoundStart_View_AfterChoice();
		objManager.UpdateViewState(false, false);

		foreach (BattleUnitView unitView in objManager.GetAliveViewList()) {
			unitView.RefreshAppearanceRenderer(true);
		}

		return false;
	}
}
