using Il2CppSystem.Collections.Generic;
using System;

namespace ModularSkillScripts.Acquirer;

public class AcquirerGetChainAtkType : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SinManager sinManager = Singleton<SinManager>.Instance;
		var a = sinManager.GetSortedSinActionModelListByOriginSpeed(true);
		BattleUnitModel user_unit = modular.modsa_unitModel;
		UNIT_FACTION faction_this = user_unit.Faction;
		UNIT_FACTION faction_opps = faction_this == UNIT_FACTION.PLAYER ? UNIT_FACTION.ENEMY : UNIT_FACTION.PLAYER;
		UNIT_FACTION faction = circles[0] == "Ally" ? faction_this : faction_opps;
		int count = 0;
		int currentRes = 0;
		int absRes = 0;
		var atksearch = circles[1];
		bool retAbs = false;
		if (circles.Length >= 3)
		{
			retAbs = true;
		}
		ATK_BEHAVIOUR type;
		Enum.TryParse(atksearch, out type);
		foreach (SinActionModel sinAction in a)
		{
			MainClass.Logg.LogInfo($"name of unit = {sinAction.UnitModel.GetName()}");
			UnitSinModel sinModel = sinAction.currentSelectSin;
			if (sinModel == null) continue;
			SkillModel skill = sinModel.GetSkill();
			if (skill == null) continue;
			ATK_BEHAVIOUR atkType = skill.GetAttackType();
			if (atkType == type && faction == sinModel.Model.Faction)
			{
				count++;
				currentRes++;
			}
			else
			{
				if (currentRes > absRes)
				{
					absRes = currentRes;
				}
				currentRes = 0;
			}
		}

		if (currentRes > absRes)
		{
			absRes = currentRes;
		}

		if (retAbs)
		{
			return absRes;
		}
		return count;
	}
}