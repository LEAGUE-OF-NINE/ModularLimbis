using System.Collections;
using System.Globalization;
using BattleUI;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppSystem.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace ModularSkillScripts.Consequence;

public class ConsequenceUpperText : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		string line_played = circledSection.Remove(0, circles[0].Length + 1);
		line_played = Regex.Replace(line_played, @"_", " ");
		line_played = line_played.Replace("^n", "\n");
		var battleUiRoot_inst = SingletonBehavior<BattleUIRoot>.Instance;

		int.TryParse(circles[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var color);
		byte r = (byte)((color >> 16) & 0xFF);
		byte g = (byte)((color >> 8) & 0xFF);
		byte b = (byte)((color) & 0xFF);
		var controller = battleUiRoot_inst._outterGradiantEffectController;
		controller.StartCoroutine(upperTextCoroutine(controller, new Color32(r, g, b, 255), line_played));
	}
	
	private static IEnumerator upperTextCoroutine(OutterGradiantEffectController controller, Color32 color32, string linePlayed)
	{
		MainClass.Logg.LogInfo($"Upper text color: {color32}");
		const float waitTime = 2;
		var text = controller._dialogText_Upper;
		text.m_faceColor = color32;
		controller._text_faceColor_Upper = color32;
		controller._text_faceHDRFactor_Upper = 0.7f;
		controller.SetDialog_Upper(linePlayed, 1.5f, waitTime);
		var shadow = controller._dialogText_Upper.GetComponentInParent<Image>();
		if (shadow.enabled) yield break;
		shadow.enabled = true;
		yield return new WaitForSeconds(waitTime);
		shadow.enabled = false;
	}

}