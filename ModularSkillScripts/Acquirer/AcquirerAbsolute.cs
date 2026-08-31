using System;

namespace ModularSkillScripts.Acquirer;

public class AcquirerAbsolute : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int value = modular.GetNumFromParamString(circles[0]);
        return Math.Abs(value);
	}
}