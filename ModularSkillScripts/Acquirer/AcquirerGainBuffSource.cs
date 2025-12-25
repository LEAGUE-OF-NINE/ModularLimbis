namespace ModularSkillScripts.Acquirer;

public class AcquirerGainBuffSource : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        if (modular.modsa_passiveModel == null) return -1;

        switch (modular.gainbuff_source)
        {
            default: return -1;
            case ABILITY_SOURCE_TYPE.NONE: return 0;
            case ABILITY_SOURCE_TYPE.SKILL: return 1;
            case ABILITY_SOURCE_TYPE.EVENT: return 2;
            case ABILITY_SOURCE_TYPE.BUFF: return 3;
            case ABILITY_SOURCE_TYPE.PASSIVE: return 4;
            case ABILITY_SOURCE_TYPE.SYSTEM_ABILITY: return 5;
            case ABILITY_SOURCE_TYPE.EGO_GIFT: return 6;
            case ABILITY_SOURCE_TYPE.PATTERN: return 7;
            case ABILITY_SOURCE_TYPE.STAGE: return 8;
            case ABILITY_SOURCE_TYPE.UNIT: return 9;
        }
    }
}