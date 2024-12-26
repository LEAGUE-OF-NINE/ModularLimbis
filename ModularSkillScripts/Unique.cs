using System;
using System.Runtime.InteropServices;
using BattleUI;
using BattleUI.Operation;
using Lethe;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using MainUI;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using static BattleUI.Abnormality.AbnormalityPartSkills;
using static UnityEngine.GraphicsBuffer;
using IntPtr = System.IntPtr;
using System.Reflection;
using System.Linq;

namespace ModularSkillScripts
{
	internal class Unique
	{
		[HarmonyPatch(typeof(SkillModel), nameof(SkillModel.Init), new Type[] { })]
		[HarmonyPostfix]
		private static void Postfix_SkillModelInit_AddSkillScript(SkillModel __instance)
		{

		}
	}
}
