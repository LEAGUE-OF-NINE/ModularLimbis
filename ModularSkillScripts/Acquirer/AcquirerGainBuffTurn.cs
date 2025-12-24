namespace ModularSkillScripts.Acquirer;

public class AcquirerGainBuffTurn : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        if (modular.modsa_passiveModel != null) return modular.gainbuff_turn;
        return -1;
    }
}