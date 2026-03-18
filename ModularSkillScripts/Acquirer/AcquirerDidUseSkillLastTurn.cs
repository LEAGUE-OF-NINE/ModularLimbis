using ModularSkillScripts;

namespace ModularSkillScripts.Acquirer;

public class AcquirerDidUseSkillLastTurn : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleUnitModel target = modular.GetTargetModel(circles[0]);
        if (target == null) return -1;
        int skillId = modular.GetNumFromParamString(circles[1]);
        return target.DidActionPrevTurn(skillId) ? 1 : 0;
    }
}