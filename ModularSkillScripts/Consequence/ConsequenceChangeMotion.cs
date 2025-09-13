using System;

namespace ModularSkillScripts.Consequence;

public class ConsequenceChangeMotion : IModularConsequence
{

	public class MotionDetail
	{
		public MOTION_DETAIL Detail;
		public int Index;
	}
	
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		MotionDetail motionDetail = new MotionDetail();
		if (!Enum.TryParse(circles[0], out motionDetail.Detail))
		{
			MainClass.Logg.LogError($"Invalid motion detail: {circles[0]}");
			return;
		}
		motionDetail.Index = modular.GetNumFromParamString(circles[1]);
		modular.modsa_motionDetail = motionDetail;
	}
}