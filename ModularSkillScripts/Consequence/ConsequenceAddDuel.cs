using ModularSkillScripts;
using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceAddDuel : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel unit_1 = modular.GetTargetModel(circles[0]);
        BattleUnitModel unit_2 = modular.GetTargetModel(circles[1]);
        if (unit_1 == null || unit_2 == null) return;

        int s_1 = modular.GetNumFromParamString(circles[2]);
        int s_2 = modular.GetNumFromParamString(circles[3]);
        // int parryingSpeed = modular.GetNumFromParamString(circles[4]);

        SinActionModel newSAM_1 = unit_1.AddNewSinActionModel();
        SinActionModel newSAM_2 = unit_2.AddNewSinActionModel();

        UnitSinModel newUSM_1 = new UnitSinModel(s_1, unit_1, newSAM_1);
        UnitSinModel newUSM_2 = new UnitSinModel(s_2, unit_2, newSAM_2);        
        
        BattleActionModel newBAM_1 = new BattleActionModel(newUSM_1, unit_1, newSAM_1);
        BattleActionModel newBAM_2 = new BattleActionModel(newUSM_2, unit_2, newSAM_2);

        newBAM_1._targetDataDetail.ReadyOriginTargeting(newBAM_1);
        newBAM_2._targetDataDetail.ReadyOriginTargeting(newBAM_2);

        // newBAM_1.ModifySpeedByParrying(parryingSpeed);
        // newBAM_2.ModifySpeedByParrying(parryingSpeed);

        unit_1.CutInAction(newBAM_1);
        unit_2.CutInAction(newBAM_2);

        newBAM_1.ChangeMainTargetSinAction(newSAM_2, newBAM_2, true);
        newBAM_2.ChangeMainTargetSinAction(newSAM_1, newBAM_1, true);

        Singleton<BattleActionModelManager>.Instance.AddDuel(newBAM_1, newBAM_2);
	}
}