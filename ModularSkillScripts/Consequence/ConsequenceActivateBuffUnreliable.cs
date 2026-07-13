using Lethe.Patches;
using System;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceActivateBuffUnreliable : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		List<BattleUnitModel> modelList = modular.GetTargetModelList(circles[0]);
		if (modelList.Count < 1) return;
		BattleUnitModel attacker = modular.GetTargetModel(circles[1]);
		
		string bufkeyword_string = circles[2];
		BUFF_UNIQUE_KEYWORD buffUniqueKeyword = CustomBuffs.ParseBuffUniqueKeyword(bufkeyword_string);
		
		int dmg_sin = 0;
		if (circles.Length > 3) dmg_sin = Math.Min(modular.GetNumFromParamString(circles[3]), 11);
		ATTRIBUTE_TYPE sinKind = ATTRIBUTE_TYPE.NONE;
		if (dmg_sin != -1) sinKind = (ATTRIBUTE_TYPE)dmg_sin;
		
		int overwrite_amount = -1;
		if (circles.Length > 4) overwrite_amount = modular.GetNumFromParamString(circles[4]);
		
		Il2CppSystem.Nullable<int> nulint = new((int)999);
		foreach (BattleUnitModel targetModel in modelList)
		{
			BuffDetail bufDetail = targetModel._buffDetail;
			List<BuffModel> buflist = bufDetail.GetActivatedBuffModelAll();
			foreach (BuffModel buf in buflist)
			{
				if (buf._buffKeyword._mainUniqueKeyword != buffUniqueKeyword) continue;
				if (overwrite_amount >= 0)
				{
					buf.ForceToActivateBuffEffect(targetModel, attacker, 0, 0, nulint, modular.battleTiming, sinKind, overwrite_amount);
				}
				else
				{
					buf.ForceToActivateBuffEffect(targetModel, attacker, 0, 0, nulint, modular.battleTiming, sinKind);
				}
				
			}
			
		}
		
		
	}
}