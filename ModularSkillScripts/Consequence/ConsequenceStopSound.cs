using Lethe.Patches;
using ModularSkillScripts;

namespace CSound
{
	public class ConsequenceStopSound : IModularConsequence
	{
		public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
		{
			CustomAudio.StopSound();
		}
	}
}
