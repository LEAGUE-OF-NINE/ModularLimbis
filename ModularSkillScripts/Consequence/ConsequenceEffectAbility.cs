using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace ModularSkillScripts.Consequence;

public class ConsequenceEffectAbility : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_buffModel == null) return;

		BUFF_UNIQUE_KEYWORD keyword = modular.modsa_buffModel.GetMainKeyword();
		BattleUnitView unitView = SingletonBehavior<BattleObjectManager>.Instance.GetView(modular.modsa_unitModel);

		if (Enum.TryParse(circles[0], out BUFF_UNIQUE_KEYWORD keywordToCopy))
		{
			Effect_Ability effectAbility = null;
			foreach (Effect_Label effect in unitView._effects)
			{
				Effect_Ability ability = effect.TryCast<Effect_Ability>();
				if (ability != null && ability.keyword == keyword)
				{
						effectAbility = ability;
						break;
				}
			}

			bool addOrRemove = circles[1] == "add";
			if (addOrRemove)
			{
				bool active = circles.Length > 2 && circles[2] == "true";
				if (effectAbility == null)
				{
					bool isOnce = circles.Length > 3 && circles[3] == "true";
					BattleEffectManager effectManager = SingletonBehavior<BattleEffectManager>.Instance;
					Effect_Ability effectAbilityToCopy = effectManager.GetEffect_Ability(keywordToCopy);
					if (effectAbilityToCopy == null) return;

					effectAbility = new()
					{
						keyword = keyword,
						effectObj = effectAbilityToCopy.effectObj
					};

					effectAbility.effectObj.transform.SetParent(isOnce ? unitView.viewEffectRootOnce : unitView.viewEffectRootDirection);
					effectAbility.effectObj.transform.localPosition = Vector3.zero;
					effectAbility.effectObj.transform.localRotation = Quaternion.identity;
					effectAbility.effectObj.transform.localScale = Vector3.one * unitView.Appearance.charInfo.transform_Height.localPosition.y * 0.25f;

					SortingGroup sortingGroup = effectAbility.effectObj.GetComponent<SortingGroup>() ?? effectAbility.effectObj.AddComponent<SortingGroup>();
					sortingGroup.sortingLayerName = "BattleUnit";
					sortingGroup.sortingOrder = 2;
				}
				effectAbility.effectObj.SetActive(false);
				effectAbility.effectObj.SetActive(active);
				unitView._effects.Add(effectAbility);
			}
			else if (!addOrRemove && effectAbility != null)
			{
					effectAbility.effectObj.active = false;
					effectAbility.effectObj = null;
					unitView._effects.Remove(effectAbility);
			}
		}
	}
}