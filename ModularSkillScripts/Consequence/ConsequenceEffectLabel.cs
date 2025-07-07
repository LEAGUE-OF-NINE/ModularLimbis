using Il2CppSystem;

namespace ModularSkillScripts.Consequence;

public class ConsequenceEffectLabel : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		bool effectActive = modular.GetNumFromParamString(circles[2]) != 0;
		EFFECT_LAYER_TYPE layerType;
		Enum.TryParse(circles[3], true, out layerType);
		foreach (BattleUnitModel targetModel in modelList)
		{
			BattleUnitView unitView = objManager.GetView(targetModel);
			if (unitView == null) continue;
			unitView.SetEffect_Label(circles[1], effectActive, layerType);
		}
	}
}