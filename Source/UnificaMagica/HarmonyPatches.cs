//using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;


namespace UnificaMagica
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        // this static constructor runs to create a HarmonyInstance and install a patch.
        static HarmonyPatches()
        {
            // HarmonyInstance harmony = HarmonyInstance.Create("rimworld.roxxploxx.unificamagica");

            // find the RimWorld.ITab_Pawn_Character class's method of FillTab
            // CAW      MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.ITab_Pawn_Character),"FillTab");

            // find the static method to call before (i.e. Prefix) the targetmethod
            // CAW      HarmonyMethod prefixmethod = new HarmonyMethod(typeof(UnificaMagica.HarmonyPatches).GetMethod("FillTab_Prefix"));

            // patch the targetmethod, by calling prefixmethod before it runs, with no postfixmethod
            // CAW      harmony.Patch( targetmethod, prefixmethod, null ) ;


            /*
                        harmony.Patch(
                            AccessTools.Method(typeof(UnificaMagica.Verb_UseAbility_TrueBurst),"VerbTick"),
                            null,
                            new HarmonyMethod(typeof(UnificaMagica.HarmonyPatches).GetMethod("VerbTick_Postfix"))
                            );
                        harmony.Patch(
                            AccessTools.Method(typeof(Verse.Verb),"Reset"),
                            null,
                            new HarmonyMethod(typeof(UnificaMagica.HarmonyPatches).GetMethod("Verb_Reset_Postfix"))
                            );
                            */

            // Log.Message("AddTab in HarmonyPatches");
            // HarmonyPatches.AddTab(typeof(ITab_Wizard), def => def.race != null && def.race.Humanlike);


        }

//        public static void Verb_Reset_Postfix(Verb __instance) { Log.Message("Verb_Reset_Postfix:   bursting?:"+__instance.Bursting); }

/*
        public static void VerbTick_Postfix(Verb __instance) {
            UnificaMagica.Verb_UseAbility_TrueBurst tb = __instance as UnificaMagica.Verb_UseAbility_TrueBurst;
            //Log.Message("Verb_UseAbility_TrueBurst.VerbTick_Postfix "+(tb!= null) +" " + __instance.GetType());
            if ( tb != null ) {
                Log.Message("Verb_UseAbility_TrueBurst.VerbTick_Postfix    Bursting?:"+__instance.Bursting+"  stateisbursting:"+(__instance.state == VerbState.Bursting) );//ticksToNextBurstShot:"+ __instance.ticksToNextBurstShot);
                ((UnificaMagica.Verb_UseAbility_TrueBurst)__instance).DebugVerbForBurst();
            }
        }
        */

        /* CAW
        // So, before the ITab_Pawn_Character is instantiated, reset the height of the dialog window
        public static void FillTab_Prefix() {
            // NOTE: commented out on update to Rimworld 1.1 ? Maybe this is fixed now?
            // RimWorld.CharacterCardUtility.PawnCardSize.y = DefDatabase<RimWorld.SkillDef>.AllDefsListForReading.Count * 47.5f;
        }
        */


        // create the Component AbilityUser
        /*
        public static void InitializeComps_PostFix(ThingWithComps __instance)
        {
            if (__instance != null)
            {
                Pawn p = __instance as Pawn;
                if (p != null)
                {
                    if (p.RaceProps != null && p.RaceProps.Humanlike)
                    {
                        AbilityUserUtilty.AddAllToThisPawn(p);
                        Log.Message("InitializeComps_PostFix : Adding CompAbiiltyUser to Pawn : ");
                        ThingComp thingComp = (ThingComp)Activator.CreateInstance(typeof(AbilityUser.CompAbilityUser));
                        thingComp.parent = __instance;
                        var comps = AccessTools.Field(typeof(ThingWithComps), "comps").GetValue(__instance);
                        if (comps != null)
                        {
                            ((List<ThingComp>)comps).Add(thingComp);
                        }
                        thingComp.Initialize(null);
                    }
                }
            }
        }
        */


        
        // From Hospitality Mod ...
        public static void AddTab(Type tabType, Func<ThingDef, bool> qualifier)
        {
            var defs = DefDatabase<ThingDef>.AllDefs.Where(qualifier).ToList();
            defs.RemoveDuplicates();

            foreach (var def in defs)
            {
                if (!def.inspectorTabs.Contains(tabType))
                {
                    def.inspectorTabs.Add(tabType);
                    def.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(tabType));
                    Log.Message(def.defName+": "+def.inspectorTabsResolved.Select(d=>d.GetType().Name).Aggregate((a,b)=>a+", "+b));
                }
            }
        }
        

    }
}
