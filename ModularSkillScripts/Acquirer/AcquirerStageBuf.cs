using System;
using CodeStage.AntiCheat.ObscuredTypes;
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


public class AcquirerStageBuf : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (circles.Length <= 1) {
			MainClass.LogModular("Bozo, Not enough arguments for stagebuf(), fucking idiot", true);
			return -1;
		}
		
		BattleUnitBuffManager bufManager = Singleton<BattleUnitBuffManager>.Instance;
		if (bufManager == null) {
			MainClass.LogModular("stagebuf() null BattleUnitBuffManager", true);
			return -1;
		}
		StageBuffManager stageBufManager = bufManager._stageBuffManager;
		
		string keyword_s = circles[0];
		if (!Il2CppSystem.Enum.TryParse(keyword_s, true, out BUFF_UNIQUE_KEYWORD keyword)) {
			MainClass.LogModular("stagebuf() invalid keyword idiot", true);
			return -1;
		}

		int stack = stageBufManager.GetCurrentStack(keyword);
		int turn = 0;
		
		return circles[1] switch
		{
			"turn" => turn,
			"+" => stack + turn,
			"*" => stack * turn,
			_ => stack
		};
	}
}