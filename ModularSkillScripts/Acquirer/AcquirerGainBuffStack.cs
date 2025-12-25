namespace ModularSkillScripts.Acquirer;

public class AcquirerGainBuffStack : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        if (modular.modsa_passiveModel != null) return modular.gainbuff_stack;
        return -1;
    }
}