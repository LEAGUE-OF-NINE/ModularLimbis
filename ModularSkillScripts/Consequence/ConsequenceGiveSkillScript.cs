using ModularSkillScripts.Patches;
using Il2CppSystem.Collections.Generic;

namespace ModularSkillScripts.Consequence;

public class ConsequenceGiveSkillScript : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel selfAction = modular.modsa_selfAction;
		if (selfAction == null) return;
			
		BattleActionModel action = selfAction;
		if (circles[0] == "Target") action = modular.modsa_oppoAction;
		if (action == null) return;
			
		SkillModel skill = action.Skill;
		if (skill == null) return;
		//SkillDataModel skillData = skill.skillData;
		//List<AbilityData> abdata_list = skillData.abilityScriptList;
		//AbilityData abdata = abdata_list[0];
		//string circle_0 = circles[0];
			
		int mode = modular.GetNumFromParamString(circles[1]);
		switch (mode)
		{
			case 0:
			{
				int power = modular.GetNumFromParamString(circles[2]);
				SkillAbility_SkillPowerResultUp ab = new();
				skill.AddTemporarySkillAbility(ab);
				ab._POWER_ADDER = power;
			}
				break;
			case 1:
			{
				int power = modular.GetNumFromParamString(circles[2]);
				SkillAbility_ParryingPowerResultAdderSelf ab = new();
				skill.AddTemporarySkillAbility(ab);
				ab._POWER_ADDER = power;
			}
				break;
			case 2:
			{
				SkillModel skill_self = selfAction.Skill;
				int index = modular.GetNumFromParamString(circles[2]);
					
				List<AbilityData> abilityData_list = skill_self.GetSkillAbilityScript();
				string abilityScriptname = abilityData_list[index].ScriptName;
				string fullscript = "Modular/TIMING:" + abilityScriptname;
					
				long ptr = skill.Pointer.ToInt64();
					
				var modsa = new ModularSA();
				modsa.originalString = fullscript;
				modsa.modsa_skillModel = skill;
				modsa.ptr_intlong = ptr;

				modsa.SetupModular("TIMING:" + abilityScriptname);
				if (!SkillScriptInitPatch.modsaDict.ContainsKey(ptr)) SkillScriptInitPatch.modsaDict.Add(ptr, new List<ModularSA>());
				SkillScriptInitPatch.modsaDict[ptr].Add(modsa);
			}
				break;
		}
			
	} // END ExecuteConsequence
}