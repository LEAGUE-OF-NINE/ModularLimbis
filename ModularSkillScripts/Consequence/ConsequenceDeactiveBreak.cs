namespace ModularSkillScripts.Consequence;

public class ConsequenceDeactiveBreak : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var modelList = modular.GetTargetModelList(circles[0]);
        if (modelList.Count < 1) return;

        int breakindex = modular.GetNumFromParamString(circles[1]);

        foreach (BattleUnitModel targetModel in modelList)
        {
            if (breakindex == -1)
            {
                foreach (BreakSection breaksec in targetModel.GetBreakSections())
                {
                    breaksec.Deactivate();
                }
            }
            else
            {
                Il2CppSystem.Collections.Generic.List<int> valueList = targetModel.GetBreakSectionValueList(true);
                if (modular.GetBoolFromParamString(circles[2]))
                {
                    valueList.Sort();
                    valueList.Reverse();
                }
                if (circles.Length == 4) valueList.Reverse();
                int exacthpval = valueList[breakindex];
                foreach (BreakSection breaksec in targetModel.GetBreakSections())
                {
                    if (breaksec.HP == exacthpval)
                    {
                        breaksec.Deactivate();
                        break;
                    }
                }
            }
            // MainClass.Logg.LogInfo(targetModel.GetBreakSections()[0].HP);
            // MainClass.Logg.LogInfo(targetModel.GetBreakSectionValueList(true).Count + " " + targetModel.GetBreakSectionValueList(true)[0]);
            // targetModel.ForceToDeactiveBreakSections(breaksection);
            // MainClass.Logg.LogInfo("after" + targetModel.GetBreakSectionValueList(true).Count + " " + targetModel.GetBreakSectionValueList(true)[0]);
        }
    }
}