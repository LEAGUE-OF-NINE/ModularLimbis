using Lethe.Patches;
using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceBuffCategory : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var modelList = modular.GetTargetModelList(circles[0]);
        if (modelList.Count < 1) return;

        BUFF_CATEGORY_KEYWORD buffCategory;
        Enum.TryParse(circles[1], true, out buffCategory);

        int stack = modular.GetNumFromParamString(circles[2]);
        int turn = modular.GetNumFromParamString(circles[3]);
        int round = modular.GetNumFromParamString(circles[4]);
        bool respectively = modular.GetBoolFromParamString(circles[5]);
        int amount = modular.GetNumFromParamString(circles[6]);

        foreach (BattleUnitModel targetModel in modelList)
        {
            Il2CppSystem.Collections.Generic.List<BuffInfo> bufList = new Il2CppSystem.Collections.Generic.List<BuffInfo>();
            foreach (BuffInfo buffinf in targetModel.GetBuffAll())
            {
                // MainClass.Logg.LogInfo(buffinf._mainKeyword + " " + buffinf.HasCategoryKeyword(buffCategory));
                if (buffinf.HasCategoryKeyword(buffCategory))
                {
                    if (round == 2)
                    {
                        bufList.Add(buffinf);
                        continue;
                    }
                    if (buffinf._activeRound == round) bufList.Add(buffinf);
                }
            }
            if (amount != -1)
            {
                // MainClass.Logg.LogInfo("Choose " + amount + " buff(s) only");
                Il2CppSystem.Collections.Generic.List<BuffInfo> newList = new Il2CppSystem.Collections.Generic.List<BuffInfo>();
                for (int i = 0; i < amount; i++)
                {
                    if (bufList.Count == 0) break;
                    int nIndex = MainClass.rng.Next(0, bufList.Count - 1);
                    newList.Add(bufList[nIndex]);
                    bufList.RemoveAt(nIndex);
                }
                bufList = newList;
                // BuffInfo theonebuff = bufList[MainClass.rng.Next(0, bufList.Count - 1)];
                // bufList = new Il2CppSystem.Collections.Generic.List<BuffInfo>();
                // bufList.Add(theonebuff);
            }
            foreach (BuffInfo buffinf in bufList)
            {
                // MainClass.Logg.LogInfo("Choosen buff: " + buffinf._mainKeyword);
                if (respectively)
                {
                    switch (modular.abilityMode)
                    {
                        case 2:
                            modular.dummyPassiveAbility.GiveBuff_Self(targetModel, buffinf._mainKeyword, stack, turn, buffinf._activeRound, modular.battleTiming, modular.modsa_selfAction);
                            break;
                        case 1:
                            modular.dummyCoinAbility.GiveBuff_Self(targetModel, buffinf._mainKeyword, stack, turn, buffinf._activeRound, modular.battleTiming, modular.modsa_selfAction);
                            break;
                        default:
                            modular.dummySkillAbility.GiveBuff_Self(targetModel, buffinf._mainKeyword, stack, turn, buffinf._activeRound, modular.battleTiming, modular.modsa_selfAction);
                            break;
                    }
                }
                else
                {
                    bool hasCount = buffinf.IsCountableBuff() || buffinf.IsSinBuff();
                    if (!hasCount)
                    {
                        switch (modular.abilityMode)
                        {
                            case 2:
                                modular.dummyPassiveAbility.GiveBuff_Self(targetModel, buffinf._mainKeyword, stack + turn, 0, buffinf._activeRound, modular.battleTiming, modular.modsa_selfAction);
                                break;
                            case 1:
                                modular.dummyCoinAbility.GiveBuff_Self(targetModel, buffinf._mainKeyword, stack + turn, 0, buffinf._activeRound, modular.battleTiming, modular.modsa_selfAction);
                                break;
                            default:
                                modular.dummySkillAbility.GiveBuff_Self(targetModel, buffinf._mainKeyword, stack + turn, 0, buffinf._activeRound, modular.battleTiming, modular.modsa_selfAction);
                                break;
                        }
                    }
                    else
                    {
                        int total = stack + turn;
                        int randomTurn = 0;
                        int remainer1 = 0;
                        int randomStack = MainClass.rng.Next(0, total);
                        int remainer = buffinf._stack + randomStack - buffinf._maxStack;
                        if (remainer > 0)
                        {
                            randomTurn = total - randomStack + remainer;
                        }
                        else
                        {
                            randomTurn = total - randomStack;
                            remainer1 = buffinf._turn + randomTurn - buffinf._maxTurn;
                            if (remainer1 > 0) randomStack += remainer1;
                        }
                        switch (modular.abilityMode)
                        {
                            case 2:
                                modular.dummyPassiveAbility.GiveBuff_Self(targetModel, buffinf._mainKeyword, randomStack, randomTurn, buffinf._activeRound, modular.battleTiming, modular.modsa_selfAction);
                                break;
                            case 1:
                                modular.dummyCoinAbility.GiveBuff_Self(targetModel, buffinf._mainKeyword, randomStack, randomTurn, buffinf._activeRound, modular.battleTiming, modular.modsa_selfAction);
                                break;
                            default:
                                modular.dummySkillAbility.GiveBuff_Self(targetModel, buffinf._mainKeyword, randomStack, randomTurn, buffinf._activeRound, modular.battleTiming, modular.modsa_selfAction);
                                break;
                        }
                    }
                }
            }
        }
    }
}