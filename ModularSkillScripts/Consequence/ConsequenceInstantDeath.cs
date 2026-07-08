using ModularSkillScripts;
using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceInstantDeath : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
        if (targetList.Count < 1) return;
        bool ingoreImmortal = modular.GetBoolFromParamString(circles[1]);
        Enum.TryParse<DAMAGE_SOURCE_TYPE>(circles[2], true, out DAMAGE_SOURCE_TYPE source);
        BattleUnitModel killer = circles.Length > 3 ? modular.GetTargetModel(circles[3]) : null;
        BattleActionModel act = circles.Length > 4 ? (circles[4] == "Self" ? modular.modsa_selfAction : modular.modsa_oppoAction) : null;
        foreach(BattleUnitModel target in targetList)
        {
            if (ingoreImmortal) {target.InstantDeathIgnoreImmortal(source, modular.battleTiming, killer, act);} else {target.InstantDeath(source, modular.battleTiming, killer, act);}
        }
    }
}