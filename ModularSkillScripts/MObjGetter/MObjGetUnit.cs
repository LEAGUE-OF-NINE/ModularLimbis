using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.MObjGetter;

public class MObjGetUnit : IModularMObjGetter
{
	public object ExecuteMObjGetter(ModularSA modular, string section, string circledSection, string[] circles)
	{
		string circle_0 = circles[0];
		return circle_0 switch
		{
			"Single" => modular.GetTargetModel(circles[1]),
			"List" => modular.GetTargetModelList(circles[1]),
			_ => null
		};
	}
}