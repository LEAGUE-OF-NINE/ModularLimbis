using Il2CppSystem;

namespace ModularSkillScripts.Acquirer;

public class AcquirerResource : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SinManager sinmanager_inst = Singleton<SinManager>.Instance;
		SinManager.EgoStockManager stock_manager = sinmanager_inst._egoStockMangaer;

		ATTRIBUTE_TYPE sin;
		UNIT_FACTION faction = modular.modsa_unitModel.Faction;
		UNIT_FACTION enemyFaction = faction == UNIT_FACTION.PLAYER ? UNIT_FACTION.ENEMY : UNIT_FACTION.PLAYER;
		Enum.TryParse(circles[0], true, out sin);
		if (circles.Length >= 2) faction = enemyFaction;

		return stock_manager.GetAttributeStockNumberByAttributeType(faction, sin);
	}
}