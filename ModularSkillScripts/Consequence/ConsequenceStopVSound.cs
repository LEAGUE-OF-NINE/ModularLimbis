using Lethe.Patches;
using ModularSkillScripts;

namespace CSound
{
	public class ConsequenceStopVSound : IModularConsequence
	{
		public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
		{
			BattleSoundGenerator.StopBGM();
		}
	}
}
