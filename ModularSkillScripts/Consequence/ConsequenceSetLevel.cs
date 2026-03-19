using ModularSkillScripts;
using System;

namespace ModularSkillScripts.Consequence
{
    public class ConsequenceSetLevel : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            Il2CppSystem.Collections.Generic.List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[0]);
            if (unitList.Count < 1) return;
            int newLevel = modular.GetNumFromParamString(circles[1]);
            foreach (BattleUnitModel unit in unitList)
            {
                unit.UnitDataModel._level = newLevel;
            }
        }
    }
}
