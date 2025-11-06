using System;

namespace ModularSkillScripts.Acquirer;

public class AcquirerGetSinResSinner : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var model = modular.GetTargetModel(circles[0]);
        if (model == null) return -1;

        ATTRIBUTE_TYPE sinType;
        Enum.TryParse(circles[1], true, out sinType);

        foreach (var res in model._resistDetail._attributeResist)
        {
            if (res.Type == sinType) return (int)(res.value * 100f);
        }
        return -1;
    }
}