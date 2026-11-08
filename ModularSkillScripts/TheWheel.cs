using System;
using System.Globalization;
using System.Linq;
using Addressable;
using BattleUI;
using BattleUI.Abnormality;
using BattleUI.Operation;
using BattleUI.UIRoot;
using FMOD;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Lua;
using ModularSkillScripts;
using ModularSkillScripts.LuaIntegration;
using ModularSkillScripts.Patches;
using SD;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utils;
using Math = Il2CppSystem.Math;

namespace ModularSkillScripts;

public class TheWheel
{
	public class InputChecker : MonoBehaviour
	{
		public InputChecker(IntPtr ptr) : base(ptr) { }

		private static InputChecker _instance;

		public static InputChecker Instance
		{
			get
			{
				if (!_instance)
				{
					GameObject go = new ("InputChecker");
					DontDestroyOnLoad(go);
					_instance = go.AddComponent<InputChecker>();
				}
				return _instance;
			}
		}
		
		private const float pi_fast = 3.1416f;
		private const float tau_fast = pi_fast*2;
		
		public int option_selected_idx = -1;

		private Vector3 mousePos_last = new();

		public WheelVis ref_wheelVis = null;
		public bool wantWheel_last = false;
		public int options = 0;
		public List<ActionWheelOption> option_list = new();
		
		void Update()
		{
			Vector3 mousePos = Input.mousePosition;
			mousePos_last = mousePos;
			Vector2 screenSize = new Vector2(Screen.width, Screen.height);
			float shortest = Mathf.Min(Screen.width, Screen.height);
			Vector2 mouseVecFromCenter = new Vector2(mousePos.x, mousePos.y) - (screenSize/2);
			float mouseDist = mouseVecFromCenter.magnitude;
			float mouseDist_percentage = mouseDist / shortest;
			bool in_cancel_range = mouseDist_percentage <= 0.1f || mouseDist_percentage >= 0.45f;
			Vector2 mouseNormFromCenter = mouseVecFromCenter.normalized;
			float aimRot = Mathf.Atan2(mouseNormFromCenter.y, mouseNormFromCenter.x);
			if (aimRot < 0) aimRot += tau_fast;
			float wheelPercentage = aimRot / tau_fast;
			BattleUIRoot battleUIRoot = SingletonBehavior<BattleUIRoot>.Instance;
			if (!battleUIRoot) return;
			Canvas canvas = battleUIRoot.PerspectiveUICanvas;
			if (!canvas) return;
			AbnormalityUIController uiController = battleUIRoot._abUIController;
			
			bool isDrag = uiController.IsDragginSin();
			bool wantWheel = Input.GetKey(KeyCode.W) && isDrag;
			bool wantWheel_changed = wantWheel_last != wantWheel;
			wantWheel_last = wantWheel;

			if (wantWheel_changed) {
				if (!ref_wheelVis) {
					ref_wheelVis = MakeWheelVis(canvas.GetComponent<RectTransform>());
				}
				ref_wheelVis.gameObject.SetActive(wantWheel);
				
				int actevent = MainClass.timingDict["ActionWheel"];
				if (wantWheel) {
					UnitSinModel unitSin = uiController.GetDragginSin();
					BattleUnitModel unit = unitSin._model;
					BattleActionModel action = unitSin.GetBattleActionModel();
					SkillModel skill = unitSin.GetSkill();
					
					option_list.Clear();
					bool foundScript = false;
					foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel(skill)) {
						if (foundScript) break;
						if (modsa.activationTiming != actevent) continue;
						if (modsa.modsa_luaState == null) continue;
						bool has = modsa.modsa_luaState.Environment.TryGetValue("PacketActionWheel", out LuaValue packet_lua);
						PacketActionWheel packet = null;
						if (has) packet = packet_lua.Read<PacketActionWheel>();
						else {
							packet = new();
							modsa.modsa_luaState.Environment["PacketActionWheel"] = packet;
							modsa.modsa_luaState.Environment["ActionWheelOption"] = new LuaActionWheelOption(null);
						}
						
						packet.info_wheelTiming = 0;
						packet.out_actionType = 0;
						modsa.Enact(unit, skill, action, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
						if (packet.out_optionList.ArrayLength < 1) continue;
						option_list = LuaActionWheelOption.UnpackTable(packet.out_optionList);
						foundScript = true;
					}

					if (!foundScript)
					{
						foreach (PassiveModel passive in unit._passiveDetail._passivelist) {
							foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel_Fast(passive)) {
								if (foundScript) break;
								if (modsa.activationTiming != actevent) continue;
								if (modsa.modsa_luaState == null) continue;
								bool has = modsa.modsa_luaState.Environment.TryGetValue("PacketActionWheel", out LuaValue packet_lua);
								PacketActionWheel packet = null;
								if (has) packet = packet_lua.Read<PacketActionWheel>();
								else {
									packet = new();
									modsa.modsa_luaState.Environment["PacketActionWheel"] = packet;
									modsa.modsa_luaState.Environment["ActionWheelOption"] = new LuaActionWheelOption(null);
								}

								modsa.modsa_passiveModel = passive;
								packet.info_wheelTiming = 0;
								packet.out_actionType = 0;
								modsa.Enact(unit, skill, action, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
								if (packet.out_optionList.ArrayLength < 1) continue;
								option_list = LuaActionWheelOption.UnpackTable(packet.out_optionList);
								foundScript = true;
							}
						}
					}
					

					options = option_list.Count;
					float rot_per_option = 360.0f / options;
					for (int i = 0; i < options; i++)
					{
						ActionWheelOption option = option_list[i];
						SkillModel option_skillModel = unit._unitDataModel.GetSkillModel(option.skillID);
						Vector2 option_pos = new(450, 0);
						float option_rot = rot_per_option * i + 45.0f;
						option_pos = option_pos.RotateAroundPoint(Vector2.zero, -option_rot);
						ref_wheelVis.CreateOptionCenter(option_pos, option_skillModel, option);
					}
					
				}
				else
				{
					if (isDrag && option_selected_idx >= 0) {
						UnitSinModel unitSin = uiController.GetDragginSin();
						BattleUnitModel unit = unitSin._model;
						BattleActionModel action = unitSin.GetBattleActionModel();
						SkillModel skill = unitSin.GetSkill();
						
						bool foundScript = false;
						foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModsaFromSkillModel(skill)) {
							if (foundScript) break;
							if (modsa.activationTiming != actevent) continue;
							if (modsa.modsa_luaState == null) continue;
							bool has = modsa.modsa_luaState.Environment.TryGetValue("PacketActionWheel", out LuaValue packet_lua);
							PacketActionWheel packet = null;
							if (has) packet = packet_lua.Read<PacketActionWheel>();
							else {
								packet = new();
								modsa.modsa_luaState.Environment["PacketActionWheel"] = packet;
								modsa.modsa_luaState.Environment["ActionWheelOption"] = new LuaActionWheelOption(null);
							}
							foundScript = true;
							packet.info_wheelTiming = 1;
							packet.info_selection = option_list[option_selected_idx].skillID;
							modsa.Enact(unit, skill, action, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
						}
						
						if (!foundScript)
						{
							foreach (PassiveModel passive in unit._passiveDetail._passivelist.CopyList()) {
								foreach (ModularSA modsa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passive)) {
									if (foundScript) break;
									if (modsa.activationTiming != actevent) continue;
									if (modsa.modsa_luaState == null) continue;
									bool has = modsa.modsa_luaState.Environment.TryGetValue("PacketActionWheel", out LuaValue packet_lua);
									PacketActionWheel packet = null;
									if (has) packet = packet_lua.Read<PacketActionWheel>();
									else {
										packet = new();
										modsa.modsa_luaState.Environment["PacketActionWheel"] = packet;
										modsa.modsa_luaState.Environment["ActionWheelOption"] = new LuaActionWheelOption(null);
									}

									modsa.modsa_passiveModel = passive;
									packet.info_wheelTiming = 1;
									packet.info_selection = option_list[option_selected_idx].skillID;
									modsa.Enact(unit, skill, action, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
									foundScript = true;
								}
							}
						}
					}

					option_selected_idx = -1;
					options = 0;
					option_list.Clear();
					ref_wheelVis.DestroyAllCenters();
				}
			}

			if (wantWheel && ref_wheelVis && options > 0)
			{
				float selectionVariant = wheelPercentage * options;
				int selection_idx = -1;
				if (!in_cancel_range) {
					selection_idx = Mathf.FloorToInt(selectionVariant);
					selection_idx = Math.Clamp(selection_idx, 0, options - 1);
				}
				
				if (selection_idx != option_selected_idx) {
					option_selected_idx = selection_idx;

					for (int i = 0; i < options; i++) {
						WheelOptionCenter center = ref_wheelVis.center_list[i];
						center.SetSelected(i == option_selected_idx);
					}
					
					/*SoundManager soundManager = SoundGenerator._soundManager;
					if (soundManager) {
						GUID parsed_guid = GUID.Parse("014929a9-0639-4e7c-ac44-61a00d7c97a5");
						soundManager.PlayOnShot(parsed_guid);
					}*/
				}
			}
			
			
			
		}
		
	}
	
	
	public static WheelVis MakeWheelVis(RectTransform rectParent)
	{
		GameObject lineObject = new("WheelVis");
		WheelVis line = lineObject.AddComponent<WheelVis>();
		
		/*Canvas pivotCanvas = lineObject.AddComponent<Canvas>();
		pivotCanvas.overrideSorting = true; 
		pivotCanvas.sortingOrder = 100; 
		*/
		RectTransform rect = lineObject.AddComponent<RectTransform>();
		rect.SetParent(rectParent, false);
		rect.SetAsLastSibling();
		/*rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;*/
		rect.anchorMin = new Vector2(0.5f, 0.5f);
		rect.anchorMax = new Vector2(0.5f, 0.5f);
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.anchoredPosition = Vector2.zero;
		rect.sizeDelta = Vector2.zero;
		
		return line;
	}
	
	
	
	public static WheelOptionText CreateOptionText(RectTransform rectParent, string text, bool is_desc, float fontSize, Vector2 offset)
	{
		GameObject lineObject = new GameObject("WheelOptionText");
		WheelOptionText line = lineObject.AddComponent<WheelOptionText>();
		
		RectTransform rect = lineObject.AddComponent<RectTransform>();
		rect.SetParent(rectParent, false);
		rect.anchorMin = new Vector2(0.5f, 0);
		rect.anchorMax = new Vector2(0.5f, 0);
		rect.pivot = new Vector2(0.5f, 1.0f);
		rect.anchoredPosition = offset;
		rect.sizeDelta = is_desc ? new Vector2(250, 500) : new Vector2(500, 300);
		
		TextMeshProUGUI tmpText = lineObject.AddComponent<TextMeshProUGUI>();
		line.textMesh = tmpText;
		tmpText.text = text;
		tmpText.fontSize = fontSize;
		tmpText.horizontalAlignment = HorizontalAlignmentOptions.Center;
		tmpText.enableWordWrapping = true;
		
		return line;
	}
	
	public class WheelVis : MonoBehaviour
	{
		public WheelVis(IntPtr ptr) : base(ptr) { }

		public List<WheelOptionCenter> center_list = new();
		
		public void DestroyAllCenters()
		{
			foreach(WheelOptionCenter center in center_list) Destroy(center.gameObject);
			center_list.Clear();
		}

		public WheelOptionCenter CreateOptionCenter(Vector2 pos, SkillModel skill, ActionWheelOption option)
		{
			GameObject lineObject = new("WheelOptionCenter");
			WheelOptionCenter line = lineObject.AddComponent<WheelOptionCenter>();
			center_list.Add(line);
			line.option = option;


			float icon_scale = (float)option.icon_scale;

			RectTransform rect = lineObject.AddComponent<RectTransform>();
			rect.SetParent(GetComponent<RectTransform>(), false);
			line.rect = rect;
			rect.anchorMin = new Vector2(0.5f, 0.5f);
			rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = pos;
			rect.sizeDelta = new Vector2(200, 200) * icon_scale; // Size of the image

			Image uiImage = lineObject.AddComponent<Image>();
			line.skillIcon = uiImage;
			uiImage.preserveAspect = true;

			//UISpriteDataManager uiSpriteDataManager = SingletonBehavior<UISpriteDataManager>.Instance;
			//uiSpriteDataManager.GetAttributeFrameSprite(ATTRIBUTE_TYPE.CRIMSON, 3);
			//Il2CppSystem.ValueTuple<Sprite, DelegateEvent> valueTuple = AddressableManager.Instance.LoadAssetSync<Sprite>("SkillIcon", option_skillID.ToString(), null, null);
			//Sprite option_skillIcon = valueTuple.Item1;
			Sprite option_skillIcon = null;
			if (option.icon_string.Length > 0)
				option_skillIcon = Lethe.Patches.Texture.LoadSpriteFromFile(option.icon_string);
			if (option_skillIcon == null) option_skillIcon = skill.GetSkillSprite();
			uiImage.sprite = option_skillIcon;

			string skillName = skill.GetSkillName();
			if (string.IsNullOrEmpty(skillName)) skillName = "Hello World";
			//WheelOptionText optionText = CreateOptionText(rect, skillName, 60, new Vector2(0, 60.0f*icon_scale) + pos);
			WheelOptionText optionText = CreateOptionText(rect, skillName, false, 55, new Vector2(0, -10.0f));
			line.text1 = optionText;

			string option_desc = option.option_desc;
			if (!string.IsNullOrEmpty(option_desc)){
				//optionText = CreateOptionText(rect, option_desc, 45, new Vector2(0, 120.0f*icon_scale + 10.0f) + pos);
				optionText = CreateOptionText(rect, option_desc, true, 40, new Vector2(0, -75.0f));
				line.text2 = optionText;
			}

			line.SetSelected(false);
			return line;
		}
	}
	
	public class WheelOptionCenter : MonoBehaviour
	{
		public WheelOptionCenter(IntPtr ptr) : base(ptr) { }

		public RectTransform rect = null;
		public Image skillIcon = null;
		public ActionWheelOption option = null;
		public WheelOptionText text1 = null;
		public WheelOptionText text2 = null;

		public void SetSelected(bool b)
		{
			bool can_select = option.can_select;
			Color modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			if (can_select) {
				rect.localScale = b ? new Vector3(1.2f,1.2f,1.2f) : Vector3.one;
				if (!b) modulate = new Color(0.7f, 0.7f, 0.7f, 0.9f);
			} else {
				rect.localScale = Vector3.one;
				modulate = new Color(0.9f, 0.4f, 0.4f, 0.9f);
			}
			
			skillIcon.color = modulate;
			if (text1) text1.SetModulate(modulate);
			if (text2) text2.SetModulate(modulate);
		}

		void OnDestroy()
		{
			if (text1 != null) Destroy(text1);
			if (text2 != null) Destroy(text2);
		}
	}
	
	public class WheelOptionText : MonoBehaviour
	{
		public WheelOptionText(IntPtr ptr) : base(ptr) { }

		public TextMeshProUGUI textMesh = null;
		
		public void SetModulate(Color modulate) {
			textMesh.color = modulate;
		}
	}
}

