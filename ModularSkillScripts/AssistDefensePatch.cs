using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Utils;

namespace ModularSkillScripts;

public class AssistDefensePatch
{
	[HarmonyPatch(typeof(BattleActionModel), nameof(BattleActionModel.CallTargetDefenseActionsByAttack))]
	[HarmonyPrefix]
	private static bool CallTargetDefenseActionsByAttack(
		BattleActionModel __instance,
		BattleRunLog runLog,
		DEFENSE_TYPE type,
		bool isDuelExpected,
		bool canDuel,
		BATTLE_EVENT_TIMING timing,
		OneCoinLog_Attack oneCoinLog,
		BattleUnitModel specificTarget,
		bool mainTargetExclusive)
	{
		if (type != DEFENSE_TYPE.GUARD) return true;
		// print out everything
		MainClass.Logg.LogInfo(
			$"__instance: {__instance}, runLog: {runLog}, type: {type}, isDuelExpected: {isDuelExpected}, canDuel: {canDuel}, timing: {timing}, oneCoinLog: {oneCoinLog}, specificTarget: {specificTarget}, mainTargetExclusive: {mainTargetExclusive}");
		var target = __instance.GetMainTarget();
		
		if (target.Faction == UNIT_FACTION.ENEMY) return true;
		var ally = BattleObjectManager.Instance.GetAliveList(false, UNIT_FACTION.PLAYER).GetFirstElement();
		if (ally == null || ally == target) return true;
		
		var targetName = target._unitDataModel._name.Replace("\n", " ");
		var allyName = ally._unitDataModel._name.Replace("\n", " ");
		var attackerName = __instance._model._unitDataModel._name.Replace("\n", " ");
		
		MainClass.Logg.LogInfo($"Redirecting attack to {targetName} from {attackerName} to ally - {allyName}");
		SpawnSkill(target, __instance, 1030201);
		return false;
	}

	private static void SpawnSkill(BattleUnitModel defender, BattleActionModel attackAction, int skillID)
	{
		//temporary action
		// var model = defender;
		var actionSlot = defender._actionSlotDetail;
		var sinActionModel = actionSlot.CreateSinActionModel(true);
		actionSlot.AddSinActionModelToSlot(sinActionModel);

		var defenderUnitView = SingletonBehavior<BattleObjectManager>.Instance.GetView(defender);
		var defenderUnitModel = defender._unitDataModel;

		//funny action stuff
		var sinModel = new UnitSinModel(skillID, defender, sinActionModel, false);
		var battleActionModel = new BattleActionModel(sinModel, defender, sinActionModel, -1);
		battleActionModel._targetDataDetail.ReadyOriginTargeting(battleActionModel);
		defender.CutInDefenseActionForcely(battleActionModel, true);
		battleActionModel.ChangeMainTargetSinAction(attackAction._sinAction, attackAction, true);

		//change skill and sinModel?
		battleActionModel._skill = new SkillModel(Singleton<StaticDataManager>.Instance._skillList.GetData(skillID),
			defenderUnitModel.Level, defenderUnitModel.SyncLevel)
		{
			_skillData =
			{
				_defenseType = (int)DEFENSE_TYPE.COUNTER,
				canDuel = true,
				_targetType = (int)SKILL_TARGET_TYPE.FRONT,
				_skillMotion = (int)MOTION_DETAIL.S3
			}
		};
		sinModel._skillId = skillID;

		//add BattleSkillViewer
		var skillViewer = new BattleSkillViewer(defenderUnitView, skillID.ToString(), battleActionModel._skill);
		defenderUnitView._battleSkillViewers.TryAdd(skillID.ToString(), skillViewer);
	}
}