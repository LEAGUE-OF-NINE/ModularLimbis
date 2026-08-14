using Il2CppSystem.Collections.Generic;
using SD;
using UnityEngine;

namespace ModularSkillScripts.Consequence;

public class ConsequenceUnitPosition : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
			
		string circle_0 = circles[0];
		switch (circle_0)
		{
			case "SetFormation": {
				List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[1]);
				float x = (float)modular.GetNumFromParamString(circles[0]) * 0.01f;
				float y = (float)modular.GetNumFromParamString(circles[1]) * 0.01f;
				float z = (float)modular.GetNumFromParamString(circles[2]) * 0.01f;
				Vector3 vec3 = new(x, y, z);

				foreach (BattleUnitModel unit in unitList) {
					if (unit == null || unit.IsDead()) continue;
					unit.SetFormationPosition(vec3);
					BattleUnitView view = objManager.GetView(unit);
					if (view) view.SetFormationPosition(vec3);
				}

			} return;
		}

	} // END ExecuteConsequence
		
}