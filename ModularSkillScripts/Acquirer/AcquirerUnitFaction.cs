namespace ModularSkillScripts.Acquirer;

public class AcquirerUnitFaction : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var model = modular.GetTargetModel(circles[0]);
        if (model == null) return -1;
        UNIT_FACTION uf = model.Faction;
        switch (uf)
        {
            default:
                return -1;
            case UNIT_FACTION.PLAYER:
                return 1;
            case UNIT_FACTION.ENEMY:
                return 0;
        }
    }
}