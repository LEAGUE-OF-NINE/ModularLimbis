using UnityEngine;
using DG.Tweening;

namespace ModularSkillScripts.Consequence;

public class ConsequenceSetPosition : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		try
		{
			if (circles.Length < 4) return;
			var targets = modular.GetTargetModelList(circles[0]);
			float x = modular.GetNumFromParamString(circles[1]) / 100f;
			float y = modular.GetNumFromParamString(circles[2]) / 100f;
			float z = modular.GetNumFromParamString(circles[3]) / 100f;

			var inst = SingletonBehavior<BattleObjectManager>.Instance;
			Vector3 p = new Vector3(x, y, z);

			foreach (BattleUnitModel model in targets)
			{
				var view = inst.GetView(model);
				if (view == null || view.gameObject == null) continue;

				DOTween.Kill(view.transform);
				Sequence seq = DOTween.Sequence();
				seq.Pause();
				seq.SetLink(view.gameObject);
				seq.Append(view.transform.DOMove(p, 0.01f).SetEase(Ease.Linear));
				seq.Play();
			}
		}
		catch (System.Exception e)
		{
			MainClass.Logg.LogError($"[SetPosition] {e}");
		}
	}
}
