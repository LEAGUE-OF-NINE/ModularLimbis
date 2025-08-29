using Lethe.Patches;

namespace ModularSkillScripts.Consequence;

public class ConsequenceVibrationSwitch : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
		bool isEntangled = circles.Length > 2;
		foreach (BattleUnitModel targetModel in modelList)
		{
			targetModel.TakeSwitchVibrationToSpecial(modular.modsa_unitModel, buf_keyword, modular.battleTiming, ABILITY_SOURCE_TYPE.SKILL,
				modular.modsa_selfAction, out _, out _, out _, out _, isEntangled);
		}
	}
}