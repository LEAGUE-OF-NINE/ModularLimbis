using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts;

namespace CSound
{
	public class ConsequencePlayCustomSound : IModularConsequence
	{
		public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
		{
			int vol = -1;
			string soundName = circles[0];
			bool loop = false;
			if (circles.Length >= 2)
			{
				vol = modular.GetNumFromParamString(circles[1]);
			}
			if (circles.Length >= 3)
			{
				loop = true;
			}
			Lethe.Patches.CustomAudio.Play(soundName, vol, loop);
		}
	}
}
