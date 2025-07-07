using Il2CppSystem;

namespace ModularSkillScripts.Consequence;

public class ConsequenceResource : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SinManager sinmanager_inst = Singleton<SinManager>.Instance;
		SinManager.EgoStockManager stock_manager = sinmanager_inst._egoStockMangaer;

		ATTRIBUTE_TYPE sin = ATTRIBUTE_TYPE.NONE;
		Enum.TryParse(circles[0], true, out sin);

		int amount = modular.GetNumFromParamString(circles[1]);

		UNIT_FACTION faction = modular.modsa_unitModel.Faction;
		UNIT_FACTION enemyFaction = faction == UNIT_FACTION.PLAYER ? UNIT_FACTION.ENEMY : UNIT_FACTION.PLAYER;
		if (circles.Length >= 3) faction = enemyFaction;

		if (amount >= 0) stock_manager.AddSinStock(faction, sin, amount, 0);
		else stock_manager.RemoveSinStock(faction, sin, amount * -1);
	}
}