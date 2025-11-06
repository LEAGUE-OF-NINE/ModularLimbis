namespace ModularSkillScripts.Acquirer;

public class AcquirerCoinOperator : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleActionModel action = modular.modsa_selfAction;
        if (circles[0] == "Target") action = modular.modsa_oppoAction;
        if (action == null) return -1;

        int coin_idx = modular.GetNumFromParamString(circles[1]);
        int coinAmount = action.Skill.CoinList.Count;
        if (coinAmount < 1) return -1;
        if (coin_idx >= coinAmount) coin_idx = coinAmount - 1;

        var opType = action.Skill.CoinList.ToArray()[coin_idx]._operatorType;

        if (opType == OPERATOR_TYPE.ADD) return 1;
        if (opType == OPERATOR_TYPE.SUB) return 2;
        if (opType == OPERATOR_TYPE.MUL) return 3;
        return -1;
    }
}