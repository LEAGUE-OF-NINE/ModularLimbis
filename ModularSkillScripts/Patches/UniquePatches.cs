using BattleUI.Operation;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using BepInEx.Unity.IL2CPP.UnityEngine;

namespace ModularSkillScripts.Patches;

public class UniquePatches
{
	[HarmonyPatch(typeof(NewOperationController), nameof(NewOperationController.EquipDefense))]
	[HarmonyPrefix]
	private static bool Postfix_NewOperationController_EquipDefense(SinActionModel sinAction)
	{
		//if (!Input.GetKeyInt(KeyCode.LeftControl)) return true;
		MainClass.LogModular("Ran EquipDefense");
		bool successSpecial = RunSpecialAction(sinAction);
		return !successSpecial;
	}
	
	public static bool RunSpecialAction(SinActionModel sinAction)
	{
		BattleUnitModel unit = sinAction.actionSlot.Owner;
		if (!unit.IsActionable()) return true;
		int actevent = MainClass.timingDict["SpecialAction"];
		bool success = false;

		List<BuffModel> buflist = unit.GetActivatedBuffModels();
		int buf_i = 0;
		while (buf_i < buflist.Count)
		{
			BuffModel buf = buflist[buf_i];
			buf_i += 1;
			foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel(buf))
			{
				if (!Input.GetKeyInt(modba.SpecialKey)) continue;
				MainClass.LogModular("Found bufpassive - SPECIAL");
				MainClass.LogModular("Triggered Key: " + modba.SpecialKey.ToString());
				success = true;
				modba.modsa_buffModel = buf;
				modba.Enact(unit, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}

		if (sinAction.currentSelectSin != null && sinAction.currentSelectSin.GetSkill() != null)
		{
			UnitSinModel sinModel = sinAction.currentSelectSin;
			BattleActionModel action = sinModel.GetBattleActionModel();
			SkillModel skill = sinModel.GetSkill();
			long skillmodel_intlong = skill.Pointer.ToInt64();
			if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong))
			{
				foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong])
				{
					if (!Input.GetKeyInt(modsa.SpecialKey)) continue;
					success = true;
					modsa.Enact(unit, skill, action, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING); // normal code
				}
			}
		}
		
		foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
			{
				if (!Input.GetKeyInt(modpa.SpecialKey)) continue;
				MainClass.LogModular("FoundS modpassive - SPECIAL: " + modpa.passiveID);
				MainClass.LogModular("Triggered Key: " + modpa.SpecialKey.ToString());
				success = true;
				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(unit, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
		foreach (EgoPassiveModel egoPassiveModel in unit._passiveDetail.EgoPassiveList)
		{
			if (!egoPassiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = egoPassiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
			{
				if (!Input.GetKeyInt(modpa.SpecialKey)) continue;
				success = true;
				MainClass.LogModular("FoundS modpassive - SPECIAL: " + modpa.passiveID);
				MainClass.LogModular("Triggered Key: " + modpa.SpecialKey.ToString());
				modpa.modsa_passiveModel = egoPassiveModel;
				modpa.Enact(unit, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}

		if (success) VisualUpdateForSpecial();
		return success;
	}

	public static void VisualUpdateForSpecial()
	{
		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		objManager.UpdatePassiveState();
		objManager.OnRoundStart_View_AfterChoice();
		objManager.UpdateViewState(false, false);

		foreach (BattleUnitView unitView in objManager.GetAliveViewList())
		{
			unitView.RefreshAppearanceRenderer(true);
		}
	}
}
