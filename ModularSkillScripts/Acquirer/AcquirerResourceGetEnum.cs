using System.Collections.Generic;
using Il2CppSystem;

namespace ModularSkillScripts.Acquirer;

public class AcquirerResourceGetEnum : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SinManager sinmanager_inst = Singleton<SinManager>.Instance;
		SinManager.EgoStockManager stock_manager = sinmanager_inst._egoStockMangaer;

		UNIT_FACTION faction = modular.modsa_unitModel.Faction;
		UNIT_FACTION enemyFaction = faction == UNIT_FACTION.PLAYER ? UNIT_FACTION.ENEMY : UNIT_FACTION.PLAYER;
		if (circles.Length >= 2) faction = enemyFaction;
		string circle_0 = circles[0];

		ATTRIBUTE_TYPE best_sin = MainClass.SortSinResourceGetEnum(stock_manager, circle_0, faction);
		return (int)best_sin;
	}

	
	
}