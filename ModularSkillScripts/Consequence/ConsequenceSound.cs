using FMOD;

namespace ModularSkillScripts.Consequence;

public class ConsequenceSound : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		string guid = circles[1];
		switch (circles[0])
		{
			case "bgm":
			{
				if(BattleSoundGenerator.IsSpecialBGM || StaticDataManager.Instance.FixedBgmStaticDataList.FindData(BattleSoundGenerator.currentBgmlist))
				{
					return;
				}

				bool stopAddBGM = circles.Length > 2;
				if (!stopAddBGM)
				{
					BattleSoundGenerator.GetCurrentBattleBGM().getDescription(out FMOD.Studio.EventDescription eventDescription);
					RESULT result = eventDescription.getPath(out string path);
					if (result == RESULT.OK && path != ("event:/BGM/Battle" + circles[1]))
					{
						BattleSoundGenerator.StopBGM();
						BattleSoundGenerator.SetAddBGMList(circles[1]);
						BattleSoundGenerator.StartAddBGM(circles[1]);
					}
				}
				else
				{
					bool flag = false;
					if(BattleSoundGenerator._currentAddBGMPath == circles[1]) flag = true;
					BattleSoundGenerator.RemoveAddBGMList(circles[1]);
					if (flag) BattleSoundGenerator.StartBGM();
				}
				MainClass.Logg.LogWarning(
				$"activating sound bgm consequence: first_param={circles[1]}");
				break;
			}
			case "sfx":
			{
				GUID parsed_guid = GUID.Parse(guid);
				SoundGenerator._soundManager.PlayOnShot(parsed_guid);
				MainClass.Logg.LogWarning($"activating sfx consequence with parsed guid: {parsed_guid}");
				break;
			}
			case "voice":
			{
				var unitID = modular.modsa_unitModel._unitDataModel.ClassInfo.id;
				BattleUnitView battleUnitView = BattleObjectManager.Instance.GetView(modular.modsa_unitModel);

				MainClass.Logg.LogWarning($"playing sound \"{circles[1]}\" for {unitID}");
				VoiceGenerator.PlayMultiVoice(unitID, circles[1], battleUnitView);
				break;
			}
			case "announcer"
				: // example: "Modular/TIMING:StartVisualCoinToss/sound(announcer,20,announcer_round_takebigdmg_20_1)"
			{
				VoiceGenerator.PlayAnnouncerBasicVoice(modular.GetNumFromParamString(circles[1]), circles[2]);
				break;
			}
		}
	}
}
