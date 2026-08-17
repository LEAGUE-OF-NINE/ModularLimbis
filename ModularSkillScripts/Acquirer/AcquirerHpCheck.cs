using System;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Acquirer;

public class AcquirerHpCheck : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		List<BattleUnitModel> modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return -1;

		string mode_s = circles[1];
		int mode = mode_s switch
		{
			"%" => 1,
			"max" => 2,
			"missing" => 3,
			"missing%" => 4,
			_ => 0
		};
		
		int total = 0;
		foreach (BattleUnitModel unit in modelList) {
			if (unit == null) continue;
			total += HpCheck(unit, mode);
		}
		return total;
	}

	public int HpCheck(BattleUnitModel targetModel, int mode)
	{
		int hp = targetModel.Hp;
		int hp_max = targetModel.MaxHp;
		float hp_ptg = (float)hp / hp_max;
		int hp_ptg_floor = (int)Math.Floor(hp_ptg * 100.0);

		return mode switch
		{
			1 => hp_ptg_floor,
			2 => hp_max,
			3 => hp_max - hp,
			4 => 100 - hp_ptg_floor,
			_ => hp
		};
		
	}
}