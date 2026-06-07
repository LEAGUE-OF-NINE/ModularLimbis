using Il2CppSystem;

namespace ModularSkillScripts.Consequence;

public class ConsequenceResource : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SinManager sinmanager_inst = Singleton<SinManager>.Instance;
		SinManager.EgoStockManager stock_manager = sinmanager_inst._egoStockMangaer;

		ATTRIBUTE_TYPE sin;
		UNIT_FACTION faction = modular.modsa_unitModel.Faction;
		UNIT_FACTION enemyFaction = faction == UNIT_FACTION.PLAYER ? UNIT_FACTION.ENEMY : UNIT_FACTION.PLAYER;
		if (circles.Length >= 3) faction = enemyFaction;
		
		string circle_0 = circles[0];
		int instruction_length = circle_0.Length;
		if (instruction_length <= 2 || circle_0.StartsWith("VAL"))
		{
			int sin_as_int = modular.GetNumFromParamString(circle_0);
			sin = (ATTRIBUTE_TYPE)sin_as_int;
		}
		else if (instruction_length >= 10) sin = MainClass.SortSinResourceGetEnum(stock_manager, circle_0, faction);
		else Enum.TryParse(circle_0, true, out sin);
		
		int amount = modular.GetNumFromParamString(circles[1]);

		if (amount >= 0) stock_manager.AddSinStock(faction, sin, amount, 0);
		else stock_manager.RemoveSinStock(faction, sin, amount * -1);
	}
}