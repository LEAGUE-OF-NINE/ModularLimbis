using BattleUI.Dialog;
using Il2CppSystem.Text.RegularExpressions;

namespace ModularSkillScripts.Consequence;

public class ConsequenceBattleDialogLine : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		string line_played = circledSection.Remove(0, circles[0].Length + 1);
		line_played = Regex.Replace(line_played, @"_", " ");
		line_played = line_played.Replace("^n", "\n");
		var modelList = modular.GetTargetModelList(circles[0]);
		foreach (BattleUnitModel targetModel in modelList)
		{
			BattleUnitView view = BattleObjectManager.Instance.GetView(targetModel);
			var dialogline = new BattleDialogLine(line_played, "");
			view._uiManager.ShowDialog(dialogline);
		}
	}
}