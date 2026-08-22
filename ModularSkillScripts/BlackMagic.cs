using System;
using System.Globalization;
using System.Linq;
using BattleUI.Operation;
using BepInEx.Unity.IL2CPP.UnityEngine;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Febucci.UI.Core;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts.Consequence;
using SD;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
using Utils;
using static BattleUI.Abnormality.AbnormalityPartSkills;
using static MirrorDungeonSelectThemeUIPanel.UIResources;
using Math = System.Math;
using MathF = Il2CppSystem.MathF;
using Type = Il2CppSystem.Type;

namespace ModularSkillScripts;

public class BlackMagic
{

	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnRoundStart_After_Event))]
	[HarmonyPostfix]
	private static void Postfix_Orbtest(BattleUnitModel __instance)
	{
		BattleUnitModel unit = __instance;
		if (!unit.IsFaction(UNIT_FACTION.PLAYER)) return;
		
		BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
		if (!objManager) return;
		UISpriteDataManager uiSpriteDataManager = SingletonBehavior<UISpriteDataManager>.Instance;
		if (!uiSpriteDataManager) return;
		Sprite wrathsprite = uiSpriteDataManager.GetAttributeFrameSprite(ATTRIBUTE_TYPE.CRIMSON, 3);
		
		if (!OrbOrbiter.Instance) return;
		OrbOrbiter orbOrbiter = OrbOrbiter.Instance;
		BattleUnitView view = objManager.GetView(unit);
		if (!view) return;
		CharacterAppearance aper = objManager.GetViewAppaearance(unit);
		orbOrbiter.appearance_ref = aper;
		//MainClass.LogModular("Aper1: " + aper.gameObject.name);
		//MainClass.LogModular("Aper2: " + aper.transform.parent.gameObject.name);
		//MainClass.LogModular("Aper3: " + aper.transform.parent.parent.gameObject.name);
		//MainClass.LogModular("Aper4: " + aper.transform.parent.parent.parent.gameObject.name);
		//MainClass.LogModular("Aper5: " + aper.transform.parent.parent.parent.parent.gameObject.name);
		Transform orbsibling = view.transform;
		Transform orbparent = orbsibling.parent;
		//orbOrbiter.transform.SetParent(view.transform.parent);
		orbOrbiter.unittransform_ref = orbsibling;
		orbOrbiter.view_pos_prev = orbsibling.localPosition;
		if (orbOrbiter.orbsprite_list.Count < 1) {
			for (int i = 0; i < 8; i++) {
				GameObject newobj = new ("Orb" + i.ToString());
				newobj.transform.SetParent(orbparent, false);
				Orb orb = newobj.AddComponent<Orb>();
				orb.orbsprite = wrathsprite;
				orbOrbiter.orbsprite_list.Add(orb);
			}
		}
		orbOrbiter.enabled = true;
	}

	public class OrbOrbiter : MonoBehaviour
	{
		public OrbOrbiter(IntPtr ptr) : base(ptr) { }

		private static OrbOrbiter _instance;

		public static OrbOrbiter Instance
		{
			get
			{
				if (!_instance)
				{
					GameObject go = new ("OrbOrbiter");
					DontDestroyOnLoad(go);
					_instance = go.AddComponent<OrbOrbiter>();
				}
				return _instance;
			}
		}
		
		public List<Orb> orbsprite_list = new();
		public Transform unittransform_ref = null;
		public CharacterAppearance appearance_ref = null;
		private const float pi_fast = 3.1416f;
		private const float pi_fast_circle = pi_fast*2;
		private float encirclement_rotation = 0f;
		
		private bool flipped = false;
		public Vector3 view_pos_prev = Vector3.zero;
			
		
		void Update()
		{
			if (!unittransform_ref) return;
			if (!appearance_ref) return;
			PlayableDirector director = appearance_ref._playableDirector;
			PlayableAsset playableAsset = director.playableAsset;
			if (!playableAsset) return;
			TimelineAsset timelineAsset = playableAsset.TryCast<TimelineAsset>();
			if (!timelineAsset) return;
			MainClass.LogModular("TimelineAsset: " + timelineAsset.name, true);
			MainClass.LogModular("Progress: " + director.time, true);
			foreach (TrackAsset trackAsset in timelineAsset.GetOutputTracks().CopyToList())
			{
				MainClass.LogModular("TrackName: " + trackAsset.name, true);
			}

			return;
			float delta = Time.deltaTime;

			Vector3 view_pos = unittransform_ref.localPosition;
			//Vector3 view_pos_diff = view_pos - view_pos_prev;
			//view_pos_prev = view_pos;
			
			int orbcount = orbsprite_list.Count;
			float segment = pi_fast_circle / orbcount;
			encirclement_rotation += pi_fast_circle * delta * 0.5f;
			
			float partition_inclination = 0.8f / orbcount;
			if (encirclement_rotation > pi_fast_circle) encirclement_rotation -= pi_fast_circle;
			for (int i = 0; i < orbcount; i++)
			{
				Orb orb = orbsprite_list[i];
				//orb.transform.localPosition -= view_pos_diff;
				
				float segment_offset = (segment * i) + encirclement_rotation;

				float x = Mathf.Sin(segment_offset);
				float y = Mathf.Sin(segment_offset + (pi_fast * 0.5f));
				float z = Mathf.Sin(segment_offset + pi_fast);
				
				
				Vector3 inclination = new Vector3(x, z, y);
				inclination.Normalize();
				inclination *= 2.0f;
				
				Vector3 targetPos = view_pos + inclination;
				
				Vector3 targetVec = targetPos - orb.transform.localPosition;
				Vector3 targetNorm = new Vector3(targetVec.x, targetVec.y, targetVec.z);
				targetNorm.Normalize();
				float targetDist = targetVec.magnitude;
				
				orb.vel *= Mathf.Pow(0.5f, delta);
				if (orb.vel.magnitude < 0.001f) orb.vel = Vector3.zero;
				float accel = delta * 15.0f * targetDist;
				if (accel >= 0.001) orb.vel += targetNorm * accel;
				
				if (orb.vel.magnitude >= 0.001f)
				{
					orb.transform.Translate(orb.vel * delta);
				}

				if (orb.transform.position.z > view_pos.z) orb.sortingGroup.sortingOrder = 99;
				else orb.sortingGroup.sortingOrder = 5;
			}
		}
		
	}
	
	public class Orb : MonoBehaviour
	{
		public Orb(IntPtr ptr) : base(ptr) { }
		public Vector3 vel = new(0f,0f,0f);
		
		public Sprite orbsprite = null;
		public SpriteRenderer sprite2D = null;
		public SortingGroup sortingGroup = null;
		
		private const int trailAmount = 8;
		
		void Start()
		{
			transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * 0.15f;
			SpriteRenderer renderer = gameObject.AddComponent<SpriteRenderer>();
			renderer.sprite = orbsprite;
			renderer.color = new Color(1.0f, 0.1f, 0.1f, 1.0f);
			sprite2D = renderer;
			
			sortingGroup = gameObject.AddComponent<SortingGroup>();
			sortingGroup.sortingLayerName = "SingleOrbLayer";
			sortingGroup.sortingOrder = 5;

			float partition = 0.8f / trailAmount;
			for (int i = 0; i < 4; i++)
			{
				GameObject newobj = new ("OrbTrail" + i.ToString());
				Transform newTransform = newobj.transform;
				newTransform.SetParent(transform, false);
				SpriteRenderer trailRenderer = newobj.AddComponent<SpriteRenderer>();
				trailRenderer.sprite = orbsprite;
				trailRenderer.color = new Color(1.0f, 0.1f, 0.1f, 1.0f);
				int step_mult = i + 1;
				float scalefactor = 1.0f - (step_mult * partition);
				newTransform.localScale = new Vector3(scalefactor, scalefactor, scalefactor);
				trail_list.Add(newTransform);
			}
		}
		
		public List<Transform> trail_list = new();
		private Vector3 lastPos = Vector3.zero;
		
		void Update()
		{
			Vector3 diff = transform.localPosition - lastPos;
			lastPos = transform.localPosition;
			int trail_count = trail_list.Count;
			Vector3 hold_vec3 = Vector3.zero;
			for (int i = 0; i < trail_count; i++)
			{
				Transform trail = trail_list[i];
				Vector3 set_pos = hold_vec3 - diff;
				hold_vec3 = trail.localPosition;
				trail.localPosition = set_pos;
			}
		}
	}
}

