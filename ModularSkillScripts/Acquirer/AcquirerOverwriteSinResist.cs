using System;

namespace ModularSkillScripts.Acquirer;

public class ConsequenceOverwriteSinResist : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var modelList = modular.GetTargetModelList(circles[0]);
        if (modelList.Count < 1) return;

        int rest = modular.GetNumFromParamString(circles[2]);
        bool Meth = false;
        if (circles.Length < 4)
        {
            if (rest < 0 || rest > 200)
            {
                MainClass.Logg.LogError("Invalid 3rd argument for ovwsinres");
                return;
            }
        }
        else
        {
            Meth = true;
        }

        ATTRIBUTE_TYPE attrType;
        Enum.TryParse(circles[1], true, out attrType);
        float resist = rest / 100f;

        // MainClass.Logg.LogInfo("sinType: " + attrType + " ; resVal: " + resist);

        foreach (BattleUnitModel targetModel in modelList)
        {
            var currentRes = targetModel._resistDetail._attributeResist;
            foreach (var res in currentRes)
            {
                if (res.Type == attrType)
                {
                    if (Meth)
                    {
                        res.value += resist;
                        if (res.value < 0f) res.value = 0f;
                        if (res.value > 2f) res.value = 2f;
                    }
                    else res.value = resist;
                    break;
                }
            }
        }
    }
}