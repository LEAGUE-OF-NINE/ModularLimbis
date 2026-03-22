using SD;
using UnityEngine;

namespace ModularSkillScripts.Consequence;

public class ConsequenceAppearanceLocalEuler : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		MainClass.Logg.LogInfo("ConsequenceAppearanceLocalEuler");
		if (circles.Length < 3) return;
			
		BattleUnitModel unit = modular.modsa_unitModel;
		if (unit == null) return;
			
		int x = modular.GetNumFromParamString(circles[0]);
		int y = modular.GetNumFromParamString(circles[1]);
		int z = modular.GetNumFromParamString(circles[2]);

		Vector3 vec3 = new(x, y, z);
			
		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		CharacterAppearance aper = objManager.GetViewAppaearance(unit);
		Transform t = aper.transform;
		MainClass.Logg.LogInfo("EULER VEC3 OLD: " + t.localEulerAngles);
		MainClass.Logg.LogInfo("EULER VEC3 NEW: " + vec3);
		t.localEulerAngles = vec3;
			
	} // END ExecuteConsequence
		
}