using ModularSkillScripts;

namespace ModularSkillScripts.Acquirer;

public class AcquirerGetCurrentPower : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (circles[0] != "Self") action = modular.modsa_oppoAction;
		if (action == null) return -1;
		
		if (circles.Length > 1 && circles[1] == "clash")
		{
			int final = action.GetFinalSkillPowerResult(modular.battleTiming);
			int clash = 0;
			if (action._oneOnOneDuelOpponentAction != null) clash = action._model.GetParryingResultAdder(action,0, action._oneOnOneDuelOpponentAction, 0, 1);
		
			int coin_total = action._skill.GetSkillDefaultPower();
			foreach (CoinModel coin in action._skill.GetCoinsUsableInDuel())
			{
				if (coin._result != COIN_RESULT.HEAD) continue;
				OPERATOR_TYPE coin_op = coin.GetOperatorType();
				if (coin_op == OPERATOR_TYPE.ADD) coin_total += action.GetCoinScale(coin, true);
				else if (coin_op == OPERATOR_TYPE.SUB) coin_total -= action.GetCoinScale(coin, true);
				else if (coin_op == OPERATOR_TYPE.MUL) coin_total *= action.GetCoinScale(coin, true);
			}
			
			return coin_total + final + clash;
		}
		
		return action.GetFinalSkillPowerResult(modular.battleTiming);
	}
}