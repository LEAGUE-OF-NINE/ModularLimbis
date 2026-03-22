using SD;
using UnityEngine;

namespace ModularSkillScripts.Consequence;

public class ConsequenceAppearanceLocalScale : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		MainClass.Logg.LogInfo("ConsequenceAppearanceLocalScale");
		if (circles.Length < 3) return;
			
		BattleUnitModel unit = modular.modsa_unitModel;
		if (unit == null) return;
			
		float x = (float)modular.GetNumFromParamString(circles[0]) * 0.01f;
		float y = (float)modular.GetNumFromParamString(circles[1]) * 0.01f;
		float z = (float)modular.GetNumFromParamString(circles[2]) * 0.01f;

		Vector3 vec3 = new(x, y, z);
		MainClass.Logg.LogInfo("VECTOR3: " + vec3);
			
		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		CharacterAppearance aper = objManager.GetViewAppaearance(unit);
		aper.transform.localScale = vec3;
			
		/*
		MainClass.Logg.LogInfo("rot1: " + aper.transform.localEulerAngles);

		Transform t2 = aper.transform.parent;
		if (!t2) return;
		MainClass.Logg.LogInfo("rot2: " + t2.localEulerAngles);

		Transform t3 = t2.parent;
		if (!t3) return;
		MainClass.Logg.LogInfo("rot3: " + t3.localEulerAngles);
		*/
	} // END ExecuteConsequence
		
}