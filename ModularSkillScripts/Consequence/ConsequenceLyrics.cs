using System.Collections;
using System.Linq;
using BattleUI;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppSystem.Text.RegularExpressions;
using UnityEngine;

namespace ModularSkillScripts.Consequence;

public class ConsequenceLyrics : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		string line_played = circledSection.Remove(0, circles[0].Length + 1);
		line_played = Regex.Replace(line_played, @"_", " ");
		line_played = line_played.Replace("^n", "\n");
		var battleUiRoot_inst = SingletonBehavior<BattleUIRoot>.Instance;

		battleUiRoot_inst._outterGradiantEffectController.SetDialog_Upper(line_played, 1.5f, 2);
		var num = Util.RandomRangeInclusive(0, 2);
		const string lyricsObjectName = "ModularUpperDialog";
		var pool = SingletonBehavior<BattleEffectManager>.Instance.EffectPool;
		MainClass.Logg.LogInfo($"Checking for {lyricsObjectName}");
		if (!pool.GetComponentsInChildren<Transform>().Any(child => child.name.Contains(lyricsObjectName)))
		{
			MainClass.Logg.LogInfo($"Could not find {lyricsObjectName}! Making one");
			var lyricsObject = Resources.Load<GameObject>("Prefab/Battle/Effect/Tmp/BattleLyricsController");
			lyricsObject.name = lyricsObjectName;
			pool.AddObject(lyricsObject);
		}

		MainClass.Logg.LogInfo($"Spawning lyrics");
		var obj = SingletonBehavior<BattleEffectManager>.Instance.EffectPool.Get(lyricsObjectName);
		obj.GetComponent<BattleLyricsContoller>().StartCoroutine(lyricsCoroutine(obj, line_played, num));
		MainClass.Logg.LogInfo($"Lyrics: {line_played}");
	}
	
	private static IEnumerator lyricsCoroutine(GameObject obj, string linePlayed, int num)
	{
		var position = SingletonBehavior<BattleCamManager>.Instance.MainCam.transform.position;
		obj.SetActive(true);
		obj.transform.localScale = Vector3.one * Eases.Linear(0.65f, 1f, position.z / 150f);
		var controller = obj.GetComponent<BattleLyricsContoller>();
		var x = Random.RandomRange(-10f, 5f);
		var y = Random.RandomRange(-5f, 5f);
		controller.transform.localPosition = new Vector3(x, y, 0);
		controller.Init(linePlayed, 120f, num, false, 0);
		yield return new WaitForSeconds(linePlayed.Length / 6.0f);
		obj.SetActiveRecursively(false);
	}

}