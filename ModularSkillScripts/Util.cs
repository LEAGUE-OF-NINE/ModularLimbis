namespace ModularSkillScripts;

public class Util
{
	
	public static BattleUnitModel_Abnormality AsAbnormalityModel(BattleUnitModel targetModel)
	{
		var abnoPart = targetModel.TryCast<BattleUnitModel_Abnormality_Part>();
		return abnoPart != null ? abnoPart.Abnormality : targetModel.TryCast<BattleUnitModel_Abnormality>();
	}
	
}