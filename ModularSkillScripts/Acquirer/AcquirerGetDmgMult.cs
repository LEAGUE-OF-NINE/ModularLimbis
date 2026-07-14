namespace ModularSkillScripts.Acquirer;

public class AcquirerGetDmgMult : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		float result = modular.modsa_skillModel.GetAttackDmgMultiplier(modular.modsa_selfAction, modular.modsa_coinModel, modular.modsa_loopTarget, false, false);

		foreach (BuffModel buffModel in modular.modsa_unitModel._buffDetail.GetActivatedBuffModelAll())
		{
			result += buffModel.GetAttackDmgMultiplier(modular.modsa_selfAction, modular.modsa_coinModel, modular.modsa_loopTarget, false, false, false);
		}

		foreach (PassiveModel passiveModel in modular.modsa_unitModel._passiveDetail.PassiveList)
		{
			result += passiveModel.GetAttackDmgMultiplier(modular.modsa_selfAction, modular.modsa_coinModel, modular.modsa_loopTarget, false, false);
		}

		if(circles.Length > 0 && modular.GetBoolFromParamString(circles[0]))
		{
			foreach (BuffModel buffModel in modular.modsa_loopTarget._buffDetail.GetActivatedBuffModelAll())
			{
				result += buffModel.GetTakeAttackDmgMultiplier(modular.modsa_loopTarget, modular.modsa_selfAction, modular.modsa_coinModel, modular.modsa_unitModel, false);
			}

			foreach (PassiveModel passiveModel in modular.modsa_loopTarget._passiveDetail.PassiveList)
			{
				result += passiveModel.GetTakeAttackDmgMultiplier(modular.modsa_selfAction, modular.modsa_unitModel, false);
			}

			BattleUnitModel_Abnormality_Part abnoPart = modular.modsa_loopTarget.TryCast<BattleUnitModel_Abnormality_Part>();
			if (abnoPart != null)
			{
				foreach (BuffModel buffModel in abnoPart._buffDetail.GetActivatedBuffModelAll())
				{
					result += buffModel.GetTakeAttackDmgPartMultiplier(abnoPart.Abnormality, abnoPart, modular.modsa_selfAction, modular.modsa_unitModel, false);
				}
			}
		}

		return (int)(result * 100f);
	}
}
