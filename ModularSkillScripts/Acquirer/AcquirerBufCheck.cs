using Lethe.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerBufCheck : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		if (targetModel == null) return -1;

		BUFF_UNIQUE_KEYWORD buf_keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
		BattleUnitBuffManager instance = Singleton<BattleUnitBuffManager>.Instance;
		BuffDetail bufDetail = targetModel._buffDetail;
		//BuffModel buf = bufDetail.FindActivatedBuff(buf_keyword, false);
		int stack = 0;
		int turn = 0;
		int amtConsumed = 0;
		bool usedcheck = circles.Length >= 4;
		if (usedcheck)
		{
			stack = 0;
		}
		else
		{
			stack = bufDetail.GetActivatedBuffStack(buf_keyword, false);
			turn = bufDetail.GetActivatedBuffTurn(buf_keyword, false);
			amtConsumed = instance.GetUsedBuffTurn(targetModel.InstanceID, buf_keyword);
		}


		return circles[2] switch
		{
			"turn" => turn,
			"+" => stack + turn,
			"*" => stack * turn,
			"consumed" => amtConsumed,
			_ => stack
		};
	}
}
