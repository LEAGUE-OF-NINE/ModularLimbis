using ModularSkillScripts;

namespace ModularSkillScripts.Consequence;

public class ConsequenceSetSpUsage : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_skillModel == null) return;
        int value = modular.GetNumFromParamString(circles[0]);
        if (circles.Length > 1)
        {
            modular.modsa_skillModel._skillData.mpUsage = modular.modsa_skillModel._skillData.mpUsage + value;
        }
        else
        {
            modular.modsa_skillModel._skillData.mpUsage = value;
        }
	}
}