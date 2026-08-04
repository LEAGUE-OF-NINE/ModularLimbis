using BattleUI.Operation;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using BepInEx.Unity.IL2CPP.UnityEngine;

namespace ModularSkillScripts.Patches;

public class UniquePatches
{
	[HarmonyPatch(typeof(NewOperationController), nameof(NewOperationController.EquipDefense))]
	[HarmonyPrefix]
	private static bool Prefix_NewOperationController_EquipDefense(bool equiped, SinActionModel sinAction)
	{
		bool successSpecial = RunSpecialAction(sinAction);
		if (successSpecial) return false;
		bool successDefenseCycle = RunDefenseCycle(equiped, sinAction);
		if (successDefenseCycle)
		{
			RunDefenseSwitch(sinAction);
			return false;
		}
		return true;
	}
	
	[HarmonyPatch(typeof(NewOperationController), nameof(NewOperationController.EquipDefense))]
	[HarmonyPostfix]
	private static void Postfix_NewOperationController_EquipDefense(bool equiped, SinActionModel sinAction)
	{
		RunDefenseSwitch(sinAction);
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
	
	public static bool RunDefenseCycle(bool equiped, SinActionModel sinAction)
	{
		BattleUnitModel unit = sinAction.UnitModel;
		if (unit == null || !unit.IsActionable()) return false;
		
		UnitDataModel unitData = unit.UnitDataModel;
		UnitStaticData unitStaticData = unitData?._classInfo;
		if (unitStaticData == null) return false;
		
		List<int> defID_list = unitData._defenseSkillIDList;
		int defID_count = defID_list.Count;
		if (defID_count < 2) return false; // Not enough defense skills to cycle
		
		List<UnitSinModel> currentSinList = sinAction.currentSinList;
		if (currentSinList.Count < 1) return false;
		UnitSinModel sin = currentSinList[0];
		SkillModel skill = sin.GetSkill();
		BattleActionModel action = sin.GetBattleActionModel();
		if (action == null) return false; // Please have Action
		int skillID = skill.GetID();
		if (!defID_list.Contains(skillID)) return false; // Skill is not in Defense List
		
		if (equiped) {
			if (sinAction.IsPrevSlotEgoBySwapDefense()) return false;
		}
		
		int actevent = MainClass.timingDict["DefenseCycle"];
		int overrideCycleWithPassive_skillID = -1;
		foreach (PassiveModel pasmodel in unit._passiveDetail._passivelist) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(pasmodel)) {
				if (modsa.activationTiming != actevent) continue;
				modsa.valueList[9] = skillID;
				modsa.modsa_passiveModel = pasmodel;
				modsa.Enact(unit, skill, action, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
				int checkID = modsa.valueList[9];
				if (checkID != skillID) {
					overrideCycleWithPassive_skillID = checkID;
					break;
				}
			}
		}

		if (overrideCycleWithPassive_skillID > 0)
		{
			action.TryChangeSkill(overrideCycleWithPassive_skillID);
			return true;
		}
		
		List<int> ID_whitelist = new();
		foreach (string keywordName in unitStaticData.unitKeywordList) {
			if (keywordName.StartsWith("DEFENSECYCLE_")) {
				string ID_whitelist_String = keywordName.Remove(0, 13);
				foreach (string allowedIDString in ID_whitelist_String.Split('_')) {
					int ID = int.Parse(allowedIDString);
					ID_whitelist.Add(ID);
				}
				break;
			}
		}
		if (ID_whitelist.Count <= 0) return false; // No whitelist found
		
		int defenseNextCycle = 0;
		bool cycling = false;
		for (int i = 1; i < defID_count; i++)
		{
			int previousSkillID = defID_list[i - 1];
			if (previousSkillID == skillID) cycling = true;
			if (cycling) {
				int checkID = defID_list[i];
				if (ID_whitelist.Contains(checkID)) {
					defenseNextCycle = checkID;
					break;
				}
			} 
		}
		if (defenseNextCycle < 1) return false; // Not Found or Reached End of List

		action.TryChangeSkill(defenseNextCycle);
		return true;
	}
	
	public static void RunDefenseSwitch(SinActionModel sinAction)
	{
		BattleUnitModel unit = sinAction.UnitModel;
		if (unit == null || !unit.IsActionable()) return;
		
		List<UnitSinModel> currentSinList = sinAction.currentSinList;
		if (currentSinList.Count < 1) return; // No Skills? no_bitches.png
		
		UnitSinModel sin = currentSinList[0];
		SkillModel skill = sin.GetSkill();
		BattleActionModel action = sin.GetBattleActionModel();
		if (action == null) return; // Please have Action
		
		int actevent = MainClass.timingDict["DefenseSwitch"];

		foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel(skill)) {
			if (modsa.activationTiming != actevent) continue;
			modsa.Enact(unit, skill, action, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
		}
		foreach (PassiveModel pasmodel in unit._passiveDetail._passivelist) {
			foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(pasmodel)) {
				if (modsa.activationTiming != actevent) continue;
				modsa.modsa_passiveModel = pasmodel;
				modsa.Enact(unit, skill, action, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
			}
		}
	}
}
