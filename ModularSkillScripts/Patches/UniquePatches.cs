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
		//if (!Input.GetKeyInt(KeyCode.LeftControl)) return true;
		MainClass.Logg.LogInfo("Ran EquipDefense");
		BattleUnitModel unit = sinAction.actionSlot.Owner;
		if (!unit.IsActionable()) return true;
		int actevent = MainClass.timingDict["SpecialAction"];
		bool returnval = true;
		foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList) {
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;
					
			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong]) {
				if (!Input.GetKeyInt(modpa.SpecialKey)) continue;
				MainClass.Logg.LogInfo("FoundS modpassive - SPECIAL: " + modpa.passiveID);
				MainClass.Logg.LogInfo("Triggered Key: " + modpa.SpecialKey.ToString());
				returnval = false;
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
				if (!Input.GetKeyInt(modpa.SpecialKey)) continue;
				returnval = false;
				MainClass.Logg.LogInfo("FoundS modpassive - SPECIAL: " + modpa.passiveID);
				MainClass.Logg.LogInfo("Triggered Key: " + modpa.SpecialKey.ToString());
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(passiveModel.Owner, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		if (returnval) return true;
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
		MainClass.Logg.LogInfo("EquipDefense over");
		return false;
	}
}
