using System;

namespace ModularSkillScripts.Acquirer;

public class AcquirerHpCheck : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		if (targetModel == null) return -1;

		int hp = targetModel.Hp;
		int hp_max = targetModel.MaxHp;
		float hp_ptg = (float)hp / hp_max;
		int hp_ptg_floor = (int)Math.Floor(hp_ptg * 100.0);

		return circles[1] switch
		{
			"%" => hp_ptg_floor,
			"max" => hp_max,
			"missing" => hp_max - hp,
			"missing%" => 100 - hp_ptg_floor,
			_ => -1
		};
	}
}