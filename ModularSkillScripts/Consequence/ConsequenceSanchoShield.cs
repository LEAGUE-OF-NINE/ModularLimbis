namespace ModularSkillScripts.Consequence;

public class ConsequenceSanchoShield : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		bool effectActive = modular.GetNumFromParamString(circles[1]) != 0;
		foreach (BattleUnitModel targetModel in modelList)
		{
			var appearance = SingletonBehavior<BattleObjectManager>.Instance.GetViewAppaearance(targetModel);
			if (appearance == null) continue;
			var berserkAppearance = appearance.TryCast<CharacterAppearance_1079>();
			var managerAppearance = appearance.TryCast<CharacterAppearance_8380>();
			var identityAppearance = appearance.TryCast<CharacterAppearance_10310>();
			berserkAppearance?.SetShieldEffect(effectActive);
			managerAppearance?.SetShieldEffect(effectActive);
			identityAppearance?.SetShieldEffect(effectActive);
		}
	}
}