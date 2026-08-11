using Il2CppSystem.Collections.Generic;
using System;

namespace ModularSkillScripts.Acquirer;

public class AcquirerGetSkillAmountOnDash : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SinManager sinManager = Singleton<SinManager>.Instance;
		var a = sinManager.GetSortedSinActionModelListByOriginSpeed(true);
		int count = 0;
		foreach (var slot in a)
		{
			if (slot._unitModel.Faction == UNIT_FACTION.PLAYER)
			{
				count++;
			}
		}
		return count;
	}
}