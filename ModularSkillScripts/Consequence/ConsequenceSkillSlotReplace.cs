namespace ModularSkillScripts.Consequence;

public class ConsequenceSkillSlotReplace : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_unitModel == null)
		{
			MainClass.Logg.LogInfo("skillslotreplace Self == null");
			return;
		}

		var sinActionList = modular.modsa_unitModel.GetSinActionList();
		int skillID_1 = modular.GetNumFromParamString(circles[1]);
		int skillID_2 = modular.GetNumFromParamString(circles[2]);

		if (circles[1] == "All")
		{
			foreach (SinActionModel sinslot in sinActionList)
			{
				var skillList = sinslot.UnitModel.GetSkillList();
				foreach (SkillModel x in skillList)
				{
					var x_id = x.GetID();
					sinslot.ReplaceSkillAtoB(x_id, skillID_2);
				}
			}
		}
		else if (circles[0] == "All")
		{
			foreach (SinActionModel sinslot in sinActionList) sinslot.ReplaceSkillAtoB(skillID_1, skillID_2);
		}
		else
		{
			int slot = modular.GetNumFromParamString(circles[0]);
			if (slot >= sinActionList.Count || slot < 0)
			{
				MainClass.Logg.LogInfo("skillslotreplace invalid slot");
				return;
			}

			sinActionList.ToArray()[slot].ReplaceSkillAtoB(skillID_1, skillID_2);
		}
	}
}