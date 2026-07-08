using ModularSkillScripts;

namespace ModularSkillScripts.Acquirer;

public class AcquirerGetBreakLevel : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleUnitModel target = modular.GetTargetModel(circles[0]);
        if (target == null) return -1;
        return target.GetCurrentBrokenLevel();
    }
}