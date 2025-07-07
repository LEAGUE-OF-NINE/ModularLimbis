using System;

namespace ModularSkillScripts.Acquirer;

public class AcquirerGetStat : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		if (targetModel == null) return -1;
		int value = -1;

		string circle_1 = circles[1];
		if (circle_1 == "deployment") value = targetModel.PARTICIPATE_ORDER;
		else if (circle_1 == "deadAllyCount") value = targetModel.deadAllyCount;
		else if (circle_1 == "panicType") value = Convert.ToInt32(targetModel._defaultPanicType);
		else if (circle_1 == "isRetreated") value = targetModel.IsRetreated() ? 1 : 0;
		else if (circle_1 == "hasMp") value = targetModel.HasMp() ? 1 : 0;
		else if (circle_1 == "deflevel") value = targetModel.GetDefense(out _, out _);
		//else if (circle_1 == "level") value = targetModel.Level;
		else if (circle_1.StartsWith("res"))
		{
			string word = circle_1.Remove(0, 3);
			ATK_BEHAVIOUR atk = ATK_BEHAVIOUR.NONE;
			Enum.TryParse(word, true, out atk);
			if (atk != ATK_BEHAVIOUR.NONE)
			{
				value = (int)(targetModel.GetAtkResistMultiplier(atk) * 100.0f);
			}
			else
			{
				ATTRIBUTE_TYPE sin = ATTRIBUTE_TYPE.NONE;
				Enum.TryParse(word, true, out sin);
				if (sin != ATTRIBUTE_TYPE.NONE) value = (int)(targetModel.GetAttributeResistMultiplier(sin) * 100.0f);
			}
		}
		else if (circle_1.StartsWith("speed"))
		{
			bool original = circle_1.Length >= 9;
			if (circle_1[7] == 'x')
			{
				value = targetModel.GetDefaultMaxSpeed();
				if (!original) value += targetModel.GetMaxSpeedAdder();
			}
			else
			{
				value = targetModel.GetDefaultMinSpeed();
				if (!original) value += targetModel.GetMinSpeedAdder();
			}
		}

		return value;
	}
}