using Il2CppSystem;

namespace ModularSkillScripts.Consequence;

public class ConsequenceEffectLabel : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int circle_amount = circles.Length;
		if (circle_amount < 4)
		{
			MainClass.Logg.LogInfo("Consequence Fail: effectlabel (Less than 4 arguments)");
			return;
		}

		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		var modelList = modular.GetTargetModelList(circles[0]);
		string label_s = circles[1];
		bool effectActive = modular.GetNumFromParamString(circles[2]) != 0;
		Enum.TryParse(circles[3], true, out EFFECT_LAYER_TYPE layerType);
		bool isSetOverrideDie = false;
		bool isCenter = false;
		float scale = -1F;
		bool isAddScript = false;
		if (circle_amount >= 5) isSetOverrideDie = circles[4] == "true";
		if (circle_amount >= 6) isCenter = circles[5] == "true";
		if (circle_amount >= 7) scale = 0.01f * modular.GetNumFromParamString(circles[6]);
		if (circle_amount >= 8) isAddScript = circles[7] == "true";

		foreach (BattleUnitModel targetModel in modelList)
		{
			BattleUnitView unitView = objManager.GetView(targetModel);
			if (unitView == null) continue;
			unitView.SetEffect_Label(label_s, effectActive, layerType, isSetOverrideDie, isCenter, scale, isAddScript);
		}
	}
}
