using Il2CppSystem;

namespace ModularSkillScripts.Acquirer;

public class AcquirerResonance : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SinManager sinmanager_inst = Singleton<SinManager>.Instance;
		SinManager.ResonanceManager res_manager = sinmanager_inst._resManager;

		ATTRIBUTE_TYPE sin; // default ATTRIBUTE_TYPE.NONE

		if (circledSection == "highres")
		{
			//List<ATTRIBUTE_TYPE> sinList = new List<ATTRIBUTE_TYPE>();
			//for (int i = 0; i<7; i++) sinList.Add((ATTRIBUTE_TYPE)i);
			//finalResult = res_manager.GetMaxAttributeResonanceOfAll(modsa_unitModel.Faction, out sinList);
			//List<ATTRIBUTE_TYPE> sinList = new List<ATTRIBUTE_TYPE>();
			int highest = 0;
			for (int i = 0; i < 7; i++)
			{
				int current = res_manager.GetAttributeResonance(modular.modsa_unitModel.Faction, (ATTRIBUTE_TYPE)i);
				if (current > highest) highest = current;
			}

			return highest;
		}
		if (circledSection == "highperfect")
		{
			return res_manager.GetMaxPerfectResonanceOfAll(modular.modsa_unitModel.Faction, out _);
		}
		if (circledSection.StartsWith("perfect"))
		{
			Enum.TryParse(circledSection.Remove(0, 7), true, out sin);
			return res_manager.GetMaxPerfectResonance(modular.modsa_unitModel.Faction, sin);
		}
		if (Enum.TryParse(circledSection, true, out sin))
		{
			return res_manager.GetAttributeResonance(modular.modsa_unitModel.Faction, sin);
		}

		return -1;
	}
}