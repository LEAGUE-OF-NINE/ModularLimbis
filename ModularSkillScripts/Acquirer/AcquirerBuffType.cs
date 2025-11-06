namespace ModularSkillScripts.Acquirer;

public class AcquirerBuffType : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BUF_TYPE buffType = StaticDataManager.Instance.GetBuffData(circles[0]).GetBuffType();

        if (buffType == BUF_TYPE.Positive) return 1;
        if (buffType == BUF_TYPE.Negative) return 2;
        if (buffType == BUF_TYPE.Neutral) return 0;
        return -1;
    }
}