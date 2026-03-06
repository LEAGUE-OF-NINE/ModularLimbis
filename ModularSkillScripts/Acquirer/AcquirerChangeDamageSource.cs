namespace ModularSkillScripts.Acquirer;

public class AcquirerChangeDamageSource : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        if (modular.modsa_passiveModel == null) return -1;

				return (int)modular.changedamage_source;
    }
}