using Lethe.Patches;

namespace ModularSkillScripts.Consequence;

public class ConsequenceUpdateMaxBuf : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return;

		foreach (BattleUnitModel targetModel in modelList) {
			UpdateMaxBufOnUnit(targetModel);
		}
	}

	public void UpdateMaxBufOnUnit(BattleUnitModel unit)
	{
		foreach (BuffModel buf in unit.GetActivatedBuffModels()) {
			buf.SetMaxStackAndTurn(unit);
			buf.UpdateMaxStackAndTurn(unit);
		}
	}
}