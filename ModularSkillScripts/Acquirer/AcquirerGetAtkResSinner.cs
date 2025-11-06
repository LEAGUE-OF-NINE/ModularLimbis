using System;

namespace ModularSkillScripts.Acquirer;

public class AcquirerGetAtkResSinner : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var model = modular.GetTargetModel(circles[0]);
        if (model == null) return -1;

        ATK_BEHAVIOUR atkType;
        Enum.TryParse(circles[1], true, out atkType);

        foreach (var res in model._resistDetail._atkResist)
        {
            if (res.Type == atkType) return (int)(res.value * 100f);
        }
        return -1;
    }
}