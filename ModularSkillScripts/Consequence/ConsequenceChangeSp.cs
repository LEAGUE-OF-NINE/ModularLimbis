using ModularSkillScripts;

namespace ModularSkillScripts.Consequence;

public class ConsequenceChangeSp : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
        if (targetList.Count < 1) return;
        int newMp = modular.GetNumFromParamString(circles[1]);
        foreach(BattleUnitModel target in targetList)
        {
            target.ChangeMp(newMp);
        }
    }
}