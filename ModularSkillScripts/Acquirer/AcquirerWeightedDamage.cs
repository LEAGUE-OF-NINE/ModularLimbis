using System;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Acquirer;

public class AcquirerWeightedDamage : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int dmg = modular.valueList[9];
		BattleUnitModel victim = modular.modsa_victimModel;
		if (victim == null) return dmg;
		List<BattleUnitModel> targetList = modular.modsa_selfAction.GetAliveTargetUnitModelList();
		if (targetList.Count < 2) return dmg;
		
		int hp_max_victim = victim.MaxHp;
		int hp_max_total = 0;
		foreach (BattleUnitModel unit in targetList) {
			if (unit == null) continue;
			hp_max_total += unit.MaxHp;
		}
		
		if (circles.Length > 0) {
			float ratio = modular.GetNumFromParamString(circles[0]) * 0.01f;
			hp_max_total = (int)MathF.Round((float)hp_max_total * ratio, MidpointRounding.AwayFromZero);
		}
		
		if (hp_max_total < 2) return dmg;
		float hp_ratio = (float)hp_max_victim / hp_max_total;
		
		return (int)MathF.Ceiling(dmg * hp_ratio);
	}
}