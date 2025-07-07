using Il2CppSystem;

namespace ModularSkillScripts.Consequence;

public class ConsequenceResistReveal : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var modelList = modular.GetTargetModelList(circles[0]);
		int partID = modular.GetNumFromParamString(circles[1]);
		ATK_BEHAVIOUR type = ATK_BEHAVIOUR.NONE;
		ATTRIBUTE_TYPE attribute = ATTRIBUTE_TYPE.NONE;
		if (circles[3] == "type") Enum.TryParse(circles[2], true, out type);
		if (circles[3] == "attribute") Enum.TryParse(circles[2], true, out attribute);
		UnlockInformationManager unlockInfo_inst = Singleton<UnlockInformationManager>.Instance;
		foreach (BattleUnitModel targetModel in modelList)
		{
			if (circles[3] == "type") unlockInfo_inst.UnlockResistStatus(targetModel.GetOriginUnitID(), partID, type);
			if (circles[3] == "attribute")
				unlockInfo_inst.UnlockResistStatus(targetModel.GetOriginUnitID(), partID, attribute);
		}
	}
}