using System;
using Lethe.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerGetBloodfeast : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (string.Equals(circles[0], "available", StringComparison.OrdinalIgnoreCase))
		{
			int bloodDinner = 0;
			if (BloodDinnerBuff.TryGetCurrentStack(out bloodDinner)) return bloodDinner;
			else return 0;
		}
		else if (string.Equals(circles[0], "spent", StringComparison.OrdinalIgnoreCase))
		{
			return BloodDinnerBuff.GetCommonAccumulativeUsedBloodDinner();
		}
		else return 0;
	}
}