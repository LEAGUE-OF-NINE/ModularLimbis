using System;
using Lethe.Patches;

namespace ModularSkillScripts.Consequence;

public class ConsequenceDestroyBuff : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var modelList = modular.GetTargetModelList(circles[0]);
        if (modelList.Count < 1) return;

        BUF_TYPE bufftype = BUF_TYPE.None;
        BUFF_CATEGORY_KEYWORD buffCat = BUFF_CATEGORY_KEYWORD.NONE;
        bool isBuffType = Enum.TryParse(circles[1], true, out bufftype);
        bool isBuffCategory = Enum.TryParse(circles[1], true, out buffCat);
        // int destroyRound = modular.GetNumFromParamString(circles[2]);

        // bool mainKeywordOnly = circles.Length > 3;
        //destroybuff(Multi-Target,Mode,Count,turn,includeCantBeDispelled)
        if (!isBuffType && !isBuffCategory) //single buff destroy mode - (Multi-Target, keyword, destroyRound)
        {
            BUFF_UNIQUE_KEYWORD keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
            int destroyRound = modular.GetNumFromParamString(circles[2]);
            foreach (BattleUnitModel targetModel in modelList)
            {
                if (destroyRound == 2)
                {
                    targetModel.ForceToDestroyBuff(keyword, 0, modular.battleTiming);
                    targetModel.ForceToDestroyBuff(keyword, 1, modular.battleTiming);
                }
                else
                {
                    targetModel.ForceToDestroyBuff(keyword, destroyRound, modular.battleTiming);
                }
            }
        }
        else //Destroy buff based on buff type mode - (Multi-Target, buffType, destroyRound, amount/times, includeCantBeDespelled? = false)
        {
            foreach (BattleUnitModel targetModel in modelList)
            {
                Il2CppSystem.Collections.Generic.List<BuffInfo> bufftypedlist = new Il2CppSystem.Collections.Generic.List<BuffInfo>();
                int amount = modular.GetNumFromParamString(circles[3]);
                if (circles[1] == "All")
                {
                    bufftypedlist = targetModel.GetBuffAll();
                }
                else
                {
                    int destroyRound = modular.GetNumFromParamString(circles[2]);

                    foreach (BuffInfo buffinf in targetModel.GetBuffAll())
                    {
                        if ((buffinf._type == bufftype) || (buffinf.HasCategoryKeyword(buffCat)))
                        {
                            if (destroyRound == 2)
                            {
                                if (circles.Length == 5)
                                {
                                    bufftypedlist.Add(buffinf);
                                    continue;
                                }
                                if (StaticDataManager.Instance.GetBuffData(buffinf._mainKeyword.ToString()).canBeDespelled)
                                {
                                    bufftypedlist.Add(buffinf);
                                }
                                continue;
                            }
                            if (destroyRound == buffinf._activeRound)
                            {
                                if (circles.Length == 5)
                                {
                                    bufftypedlist.Add(buffinf);
                                    continue;
                                }
                                if (StaticDataManager.Instance.GetBuffData(buffinf._mainKeyword.ToString()).canBeDespelled)
                                {
                                    bufftypedlist.Add(buffinf);
                                }
                            }
                        }
                    }
                }
                if (circles[3] == "All") amount = bufftypedlist.Count;
                while ((amount > 0) && (bufftypedlist.Count > 0))
                {
                    int newIndex = MainClass.rng.Next(0, bufftypedlist.Count - 1);
                    targetModel.ForceToDestroyBuff(bufftypedlist[newIndex]._mainKeyword, bufftypedlist[newIndex]._activeRound, modular.battleTiming);
                    bufftypedlist.RemoveAt(newIndex);

                    amount--;
                }
            }
        }
    }
}