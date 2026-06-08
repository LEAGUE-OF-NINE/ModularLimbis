using ModularSkillScripts.Patches;

namespace ModularSkillScripts.Acquirer;

public class AcquirerTurn : IModularAcquirer
{
	private int mode = 0;
	public AcquirerTurn(int x = 0)
	{
		mode = x;
	}
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int result = -1;
		switch (mode)
		{
			case 0:
			{
				if (modular.modsa_buffModel != null) result = modular.modsa_buffModel.GetTurn(0);
			} break;
			case 1: result = SkillScriptInitPatch.onusebuf_turn; break;
		}
		
		return result;
	}
}