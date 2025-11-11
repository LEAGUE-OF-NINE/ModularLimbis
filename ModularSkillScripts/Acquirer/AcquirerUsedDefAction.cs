namespace ModularSkillScripts.Acquirer;

public class AcquirerUsedDefenseActionThisTurn : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var model = modular.GetTargetModel(circles[0]);
        if (model == null) return -1;
        if (model._usedDefenseActionThisTurn == true)
        {
            return 1;
        }
        return 0;
    }
}