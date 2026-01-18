using Il2CppSystem;
using BattleUI.Operation;

namespace ModularSkillScripts.Acquirer;

public class AcquirerSinsInDashboard : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		NewOperationController operationController = UnityEngine.Object.FindFirstObjectByType<NewOperationController>();
		var list = operationController.SinActionSlotList;
		if (list == null) return -1;
		var array = list.ToArray();
		System.Collections.Generic.List<ATTRIBUTE_TYPE> bottomrow = new System.Collections.Generic.List<ATTRIBUTE_TYPE>();
		System.Collections.Generic.List<ATTRIBUTE_TYPE> toprow = new System.Collections.Generic.List<ATTRIBUTE_TYPE>();
		System.Collections.Generic.List<ATTRIBUTE_TYPE> readyrow = new System.Collections.Generic.List<ATTRIBUTE_TYPE>();
		foreach (var slot in array)
		{
			var firstskill = slot?.FirstSinSlot?.SkillSlot?._skill;
			if (firstskill == null) continue;
			bottomrow.Add(firstskill.GetAttributeType());
			var secondskill = slot?.SecondSinSlot?.SkillSlot?._skill;
			if (secondskill == null) continue;
			toprow.Add(secondskill.GetAttributeType());
			var readyskill = slot?.ReadySinSlot?._skillSlot?._skill;
			if (readyskill == null) continue;
			readyrow.Add(readyskill.GetAttributeType());
		}

		ATTRIBUTE_TYPE sin = ATTRIBUTE_TYPE.NONE;
		bool includeReady = circles.Length > 2 && circles[2] == "1";
		int result = 0;
		if (Enum.TryParse(circles[0], true, out sin))
		{
			if (circles[1].Equals("bottom", System.StringComparison.OrdinalIgnoreCase) || circles[1].Equals("both", System.StringComparison.OrdinalIgnoreCase))
			{
				foreach (ATTRIBUTE_TYPE type in bottomrow)
				{
					if (type == sin) result++;
				}
			}
			if (circles[1].Equals("top", System.StringComparison.OrdinalIgnoreCase) || circles[1].Equals("both", System.StringComparison.OrdinalIgnoreCase))
			{
				foreach (ATTRIBUTE_TYPE type in toprow)
				{
					if (type == sin) result++;
				}
			}
			if (includeReady)
			{
				foreach (ATTRIBUTE_TYPE type in readyrow)
				{
					if (type == sin) result++;
				}
			}
		}
		else if (circles[0].Equals("highest", System.StringComparison.OrdinalIgnoreCase))
		{
			System.Collections.Generic.Dictionary<ATTRIBUTE_TYPE, int> counts = new System.Collections.Generic.Dictionary<ATTRIBUTE_TYPE, int>();

			void Add(ATTRIBUTE_TYPE type)
			{
				if (type == ATTRIBUTE_TYPE.NONE) return;
				if (!counts.ContainsKey(type))
					counts[type] = 0;
				counts[type]++;
			}

			if (circles[1].Equals("bottom", System.StringComparison.OrdinalIgnoreCase) || circles[1].Equals("both", System.StringComparison.OrdinalIgnoreCase))
			{
				foreach (ATTRIBUTE_TYPE t in bottomrow)
				{
					Add(t);
				}
			}
			if (circles[1].Equals("top", System.StringComparison.OrdinalIgnoreCase) || circles[1].Equals("both", System.StringComparison.OrdinalIgnoreCase))
			{
				foreach (ATTRIBUTE_TYPE t in toprow)
				{
					Add(t);
				}
			}
			if (includeReady)
			{
				foreach (ATTRIBUTE_TYPE t in readyrow)
				{
					Add(t);
				}
			}

			int highestCount = -1;
			ATTRIBUTE_TYPE highestSin = ATTRIBUTE_TYPE.NONE;

			foreach (var kvp in counts)
			{
				if (kvp.Value > highestCount)
				{
					highestCount = kvp.Value;
					highestSin = kvp.Key;
				}
			}

			return (int)highestSin;
		}
		else if (circles[0].Equals("lowest", System.StringComparison.OrdinalIgnoreCase))
		{
			System.Collections.Generic.Dictionary<ATTRIBUTE_TYPE, int> counts = new System.Collections.Generic.Dictionary<ATTRIBUTE_TYPE, int>();

			void Add(ATTRIBUTE_TYPE type)
			{
				if (type == ATTRIBUTE_TYPE.NONE) return;
				if (!counts.ContainsKey(type))
					counts[type] = 0;
				counts[type]++;
			}

			if (circles[1].Equals("bottom", System.StringComparison.OrdinalIgnoreCase) || circles[1].Equals("both", System.StringComparison.OrdinalIgnoreCase))
			{
				foreach (var t in bottomrow)
				{
					Add(t);
				}
			}
			if (circles[1].Equals("top", System.StringComparison.OrdinalIgnoreCase) || circles[1].Equals("both", System.StringComparison.OrdinalIgnoreCase))
			{
				foreach (var t in toprow)
				{
					Add(t);
				}
			}
			if (includeReady)
			{
				foreach (var t in readyrow) 
				{
					Add(t);
				} 
			}

			int lowestCount = int.MaxValue;
			ATTRIBUTE_TYPE lowestSin = ATTRIBUTE_TYPE.NONE;

			foreach (var kvp in counts)
			{
				if (kvp.Value == 0) continue;

				if (kvp.Value < lowestCount)
				{
					lowestCount = kvp.Value;
					lowestSin = kvp.Key;
				}
			}

			return (int)lowestSin;
		}

		MainClass.Logg.LogInfo(result);
		return result;
	}
}