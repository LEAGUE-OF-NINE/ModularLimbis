namespace ModularSkillScripts.Acquirer;

public class AcquirerPartDestroy : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (circles.Length < 2)
		{
			MainClass.LogModular("AcquirerPartDestroy Not Enough Arguments", true);
			return -1;
		}
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		
		BattleUnitModel_Abnormality_Part part = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
		if (part == null)
		{
			MainClass.LogModular("AcquirerPartDestroy targetModel is not BattleUnitModel_Abnormality_Part", true);
			return -1;
		}

		string circle_1 = circles[1];
		if (circle_1 == "isdestroyed") return part.IsDestroyed() ? 1 : 0;
		if (circle_1 == "isdestroyable") return part.IsDestroyable() ? 1 : 0;
		if (circle_1 == "isregeneratable") return part.IsRegeneratable() ? 1 : 0;
		if (circle_1 == "isregenerating") return part.IsRegenerating() ? 1 : 0;
		if (circle_1 == "getregenerateturn") return part.GetRegenerateTurn();
		
		MainClass.LogModular("AcquirerPartDestroy invalid var_2", true);
		return -1;
	}
}