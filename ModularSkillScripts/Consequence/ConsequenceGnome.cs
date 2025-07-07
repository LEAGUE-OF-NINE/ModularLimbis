using UnityEngine;

namespace ModularSkillScripts.Consequence;

public class ConsequenceGnome : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		var modelList = modular.GetTargetModelList(circles[0]);
		foreach (BattleUnitModel targetModel in modelList)
		{
			BattleUnitView unitView = objManager.GetView(targetModel);
			if (unitView == null) continue;
			var vec3 = unitView.transform.localScale;
			unitView.transform.localScale = new Vector3(vec3.x, vec3.y * 0.5f, vec3.z);
		}
	}
}