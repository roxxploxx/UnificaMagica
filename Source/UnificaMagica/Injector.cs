using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using UnityEngine;
using HugsLib.Source.Detour;
using RimWorld;

namespace UnificaMagica
{
	[StaticConstructorOnStartup]
	internal static class DetourInjector
	{
		private static Assembly Assembly { get { return Assembly.GetAssembly(typeof(DetourInjector)); } }

		private static string AssemblyName { get { return DetourInjector.Assembly.FullName.Split(new char[] { ',' }).First<string>(); } }

		static DetourInjector() { LongEventHandler.QueueLongEvent(new Action(DetourInjector.Inject), "Initializing", true, null); }

		private static void Inject()
		{
			var injector = new UnificaMagica_SpecialInjector();
			if (injector.Inject()) Log.Message(AssemblyName + " injected.");
			else Log.Error(AssemblyName + " failed to get injected properly.");
		}

/*
		[DetourMethod(typeof(GenPlant), "GrowthSeasonNow")]
		public static bool GrowthSeasonNow(IntVec3 c, Map map)
		{
			Room roomOrAdjacent = c.GetRoomOrAdjacent(map);
			Log.Message("GenPlant.GrowthSeasonNow called");
			if (roomOrAdjacent == null)
			{
				return false;
			}
			if (roomOrAdjacent.UsesOutdoorTemperature)
			{
				return map.weatherManager.growthSeasonMemory.GrowthSeasonOutdoorsNow;
			}
			float temperature = c.GetTemperature(map);
			return temperature > 0f && temperature < 58f;
		}
		*/




	}

	public class SpecialInjector { public virtual bool Inject() { Log.Error("This should never be called."); return false; } }

	public class UnificaMagica_SpecialInjector : SpecialInjector
	{

		private static Assembly Assembly { get { return Assembly.GetAssembly(typeof(UnificaMagica_SpecialInjector)); } }

		private static readonly BindingFlags[] bindingFlagCombos = {
			BindingFlags.Instance | BindingFlags.Public, BindingFlags.Static | BindingFlags.Public,
			BindingFlags.Instance | BindingFlags.NonPublic, BindingFlags.Static | BindingFlags.NonPublic,
		};

		public override bool Inject()
		{
//			Log.Warning("pre mod of CharacterCardUtility.PawnCardSize : ");//+RimWorld.CharacterCardUtility.PawnCardSize.toString());
			RimWorld.CharacterCardUtility.PawnCardSize.y = 570f; //DefDatabase<RimWorld.SkillDef>.AllDefsListForReading.Count * 35f; // can't do this code becuase Skill isn't loaded yet
//			Log.Warning("post mod of CharacterCardUtility.PawnCardSize");
//			Log.Warning("post mod of CharacterCardUtility.PawnCardSize"+RimWorld.CharacterCardUtility.PawnCardSize.y);
			//			if( !Detours.TryDetourFromTo(
			//			typeof (Building_GuestBed).GetMethod("GetGizmos", BindingFlags.Instance | BindingFlags.Public),
			//			typeof (Building).GetMethod("GetGizmos", BindingFlags.Instance | BindingFlags.Public))) return false;

			//			Log.Warning("pre detour of CharacterCardUtility.PawnCardSize");
			//		if( !DetourProvider.TryCompatibleDetour(
			//		(MethodInfo) typeof (CharacterCardUtility).GetField("PawnCardSize"),
			//	(MethodInfo) typeof (UnificaMagica.CharacterCardUtility).GetField("PawnCardSize")) )
			//return false;
			//	Log.Warning("post detour of CharacterCardUtility.PawnCardSize");




			/*
			Func<ThingDef,bool> qualifier  = def => def.race != null && def.race.Humanlike;

			// Inject
			var defs = DefDatabase<ThingDef>.AllDefs.Where(qualifier).ToList();
			Log.Warning("pre def dup remove "+defs.Count);
			defs.RemoveDuplicates();
			Log.Warning("prost def dup remove "+defs.Count);
			Type tabtype = typeof(ITab_Pawn_Character);



			foreach (var def in defs)
			{
			if (!def.inspectorTabs.Contains(tabType))
			{
			def.inspectorTabs.Add(tabType);
			def.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(tabType));
			//Log.Message(def.defName+": "+def.inspectorTabsResolved.Select(d=>d.GetType().Name).Aggregate((a,b)=>a+", "+b));
		}
	}
	*/

	return true;
}




}



}
