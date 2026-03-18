using ModularSkillScripts;
using System;

namespace ModularSkillScripts.Acquirer
{
    public class AcquirerGetHpIncrementByLevel : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            BattleUnitModel target = modular.GetTargetModel(circles[0]);
            return (int) Math.Floor(target.UnitDataModel.ClassInfo.hp.incrementByLevel * 100);
        }
    }
}
