using System;
using BattleUI;
using BattleUI.Abnormality;
using BattleUI.BattleUnit;
using BattleUI.Operation;

namespace ModularSkillScripts.Consequence;

public class ConsequenceSetSlotAlarm : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var targetList = modular.GetTargetModelList(circles[0]);
		bool activateEffect = circles[1] == "active";
		int slotIdx = modular.GetNumFromParamString(circles[2]);
		BATTLE_UNITSKILLSLOT_ALARM_TYPE alarmType;
		Enum.TryParse(circles[3], true, out alarmType);
		foreach (BattleUnitModel unit in targetList)
		{
			var sinActionSlot = SingletonBehavior<BattleUIRoot>.Instance.NewOperationController.GetSinActionSlot(unit.GetSinActionList()[slotIdx]);
			switch (activateEffect)
			{
				case true:
					sinActionSlot.SinAction.SetUnitSkillSlotAlarmType(alarmType, 1);
					break;
				case false:
					sinActionSlot.SinAction.SetUnitSkillSlotAlarmType(BATTLE_UNITSKILLSLOT_ALARM_TYPE.NONE, 0);
					break;
			}
		}
	}
}