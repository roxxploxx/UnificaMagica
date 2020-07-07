using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace UnificaMagica
{

    //    [StaticConstructorOnStartup]
    public class ExtThingDef: Verse.ThingDef {
        public List<ExtComp> extcomps = new List<ExtComp>();
    }


    [StaticConstructorOnStartup]
    public class PlantExtended : Plant, IExtendedThing {

        protected List<ExtInstComp> extcomps = null; // set during InitializeExtendedComps

        //        ExtThingDef defint;

        public List<ExtInstComp> ExtComps {
            get { return this.extcomps; }
            set { this.extcomps = value; }
        }
        public new ExtThingDef def {
            get { return (ExtThingDef) base.def; }
            set {
                ExtThingDef dd = value as ExtThingDef;
                if ( dd == null ) {
                    Log.Error("Can not set a non-ExtThingDef to an IExtendedThing");
                    Log.Error("Can not set a non-ExtThingDef ("+( value )+") to an IExtendedThing");
                }
                base.def = value;
            }
        }

        public override void PostMake()
        {
            Log.Message("PlantExtended.PostMake");
            base.PostMake();
            ExtendedComponent.InitializeExtComps(this);
            this.initPot();
        }

        public override void ExposeData()
        {
            //Log.Message("PlantExtended.ExposeData 1");
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                ExtendedComponent.InitializeExtComps(this);
            }
            for (int i = 0; i < this.ExtComps.Count; i++)
            {
                this.ExtComps[i].PostExposeData();
            }
        }


        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            Log.Message("PlantExtended.SpawnSetup");
            base.SpawnSetup(map,respawningAfterLoad);
            ExtendedComponent.InitializeExtComps(this);
            this.initPot();
        }

        public override void PostMapInit()
        {
            base.PostMapInit();
            ExtendedComponent.InitializeExtComps(this);
            this.initPot(); // just reordered
            Log.Message("PlantExtended.PostMapInit");
            //this.CheckTemperatureMakeLeafless();
        }

        public UMBuilding_PlantGrower thepot = null;

        public List<ExtInstComp> planttraps = new List<ExtInstComp>();


        // find the pot this is in
        protected bool initPot() {
            bool retval = false;
            if ( base.Map == null ) return false;
            List<Thing> list = base.Map.thingGrid.ThingsListAt(base.Position);
            for (int i = 0; i < list.Count; i++)
            {
                Thing thing = list[i];
                if (thing is UMBuilding_PlantGrower ) {
                //    Log.Warning("PlantTrap found pot");
                    this.thepot = (UMBuilding_PlantGrower)thing;
//                    this.thepot.RecalcTemps();
                    retval = true;
                }
            }
            // Log.Warning("PlantTrap pot : "+this.thepot);
            //            Log.Warning("Creating Plant_Trap with properties:");
            //            Helpers.PrintObject(this.def.planttraps);
            //((DruidPlantProperties)this.def.plant).planttraps);

            return retval;
        }



        public IEnumerable<IntVec3> alertzone = null;
        public void initAlertZone() {

            //Log.Message("PlantTrap.initAlertZonw 1 ");
            float r = this.thepot.def.specialDisplayRadius;
            this.alertzone = GenRadial.RadialCellsAround (this.Position, r, true);
        }


        // protected int rearmAt = 0;


        // ---------------------------------------------------------------------
        // From Building_Trap below
        // ---------------------------------------------------------------------
        //        private List<Pawn> touchingPawns = new List<Pawn> ();
        //protected bool Armed = true;


        protected void Spring(Pawn p, ExtInstComp_PlantTrap _ptp)
        {
            //            Log.Message("PlantTrap.Spring! ");
            SoundDef.Named("PlantTrapSpring").PlayOneShot(new TargetInfo(base.Position, base.Map, false));
            /* if (p.Faction != null)
            {
                p.Faction.TacticalMemory.TrapRevealed(base.Position, base.Map);
            } */
            _ptp.SpringPlantTrap(p,this);
        }

        public override void Tick ()
        {
            int curtick = Find.TickManager.TicksGame;

            //Log.Message("PlantTrap.Tick 1 "+base.LabelMouseover+ " : " + curtick);
            //this.TickInterval
            if ( curtick % 2000 == 0 )  {
                //                this.RecalcTemps();
                this.TickLong();
            }

            if ( this.thepot == null ) {
                if ( this.initPot() == false ) {
                    //Log.Message("PlantTrap.Tick .. map not set on first pass so return");
                    return;
                }
            } // Map not set on first pass so return

            // only mature trips
            if ( base.Map != null && base.LifeStage == PlantLifeStage.Mature ) {

                // check for alert zone initialization
                if ( this.alertzone == null ) {
                    this.initAlertZone();
                }

                // find if there is an active trap; reactivate any needing it
                bool anyarmed = false;

                // handle all traps
                foreach ( ExtInstComp ptp2 in this.ExtComps ) {
                    ExtInstComp_PlantTrap ptp = ptp2 as ExtInstComp_PlantTrap;
                    if ( ptp != null ) {

                        // check for rearm if not armed
                        // Log.Message("PlantTrap.Tick rearm check "+this.Armed+" " + Find.TickManager.TicksGame +" "+ this.rearmAt+ " " + (Find.TickManager.TicksGame > this.rearmAt ));
                        if ( ptp.Armed == false && ( ptp.rearmAt != -1 ) && ( curtick > ptp.rearmAt ) ) {
                            ptp.Armed = true;
                        }

                        // if armed, check alert zones
                        if ( ptp.Armed == true ) {
                            anyarmed = true;
                        }

                    }
                }

                // find all Pawns nearby
                List<Pawn> pawnthings = new List<Pawn>();
                if ( anyarmed ) {
                    // find all pawns
                    foreach ( IntVec3 pp in this.alertzone) { // for each spot in alert zone
                        List<Thing> thingList = pp.GetThingList (base.Map);
                        for (int i = 0; i < thingList.Count; i++) {
                            Pawn pawn = thingList [i] as Pawn;
                            if (pawn != null ) {
                                pawnthings.Add(pawn);
                            }
                        }
                    }
                }

                // is anyarmed, chekc
                if ( anyarmed ) {
                    foreach ( ExtInstComp ptp2 in this.ExtComps ) {
                        ExtInstComp_PlantTrap ptp = ptp2 as ExtInstComp_PlantTrap;
                        if ( ptp != null ) {
                            if ( ptp.Armed ) {
                                foreach ( Pawn pawn in pawnthings ) {
                                    if ( this.CheckSpring (pawn,ptp)  ) {
                                        if ( ptp.Props.isAoE ) break; // apply only once if AoE
                                    }
                                }
                            }
                        }
                    }
                }

                //delete(pawnthings);

            }
        }



        protected virtual bool CheckSpring (Pawn p, ExtInstComp_PlantTrap _ptp)
        {
            bool retval = false;
            if ( ! _ptp.Armed ) return false;

            float sc = this.SpringChance(p);
            float rr = Rand.Value;
            //            Log.Message("PlantTrap.CheckSpring : rand("+rr+") chance("+sc+")");
            if (Rand.Value <  sc ) { //this.SpringChance (p))
                //                Log.Message("PlantTrap.CheckSpring : sprung");
                retval = true;
                this.Spring (p,_ptp);
                if (p.Faction == Faction.OfPlayer || p.HostFaction == Faction.OfPlayer) {
                    //Letter let = new Letter ("LetterFriendlyTrapSprungLabel".Translate (new object[] { p.NameStringShort }), "LetterFriendlyTrapSprung".Translate (new object[] { p.NameStringShort }), LetterType.BadNonUrgent, new TargetInfo (base.Position, base.Map, false));
                    //Find.LetterStack.ReceiveLetter (let, null);
                    Find.LetterStack.ReceiveLetter(
                        Translator.Translate("LetterFriendlyTrapSprungLabel"),
                        Translator.Translate("LetterFriendlyTrapSprung"),
                        LetterDefOf.ThreatSmall, new TargetInfo(base.Position, base.Map, false), null);
                }
            }
            return retval;
        }

        protected virtual float SpringChance (Pawn p)
        {
            float num;
            if (this.KnowsOfTrap (p)) {
                num = 0.0f; //0.004f;
                //                Log.Message("PlantTrap.SpringChance 1 : "+num);
            }
            else {
                num = this.GetStatValue (StatDefOf.TrapSpringChance, true);
                //                Log.Message("PlantTrap.SpringChance 1 : "+num);
            }
            num *= GenMath.LerpDouble (0.4f, 0.8f, 0f, 1f, p.BodySize);
            if (p.RaceProps.Animal) {
                num *= 0.1f;
            }
            //            Log.Message("PlantTrap.SpringChance chance: "+num+"   base is "+StatDefOf.TrapSpringChance);
            float retval = Mathf.Clamp01(num);
            //            Log.Message("PlantTrap.SpringChance chance: "+num+"   retval: "+retval);
            //return Mathf.Clamp01 (num);
            return retval;
        }

        public bool KnowsOfTrap(Pawn p)
        {
            if ( this.thepot == null ) {
                Log.Error("can not find pot of plant "+this+" at "+this.Position);
                return false;
            }
            Faction f = this.thepot.Faction;
            //            Log.Message("PlantTrap.KnowsOfTrap 0 : is faction null "+(f == null));

            bool retval = (p.Faction != null && !p.Faction.HostileTo(f)) || (p.Faction == null && p.RaceProps.Animal && !p.InAggroMentalState) || (p.guest != null && p.guest.Released);
            return (p.Faction != null && !p.Faction.HostileTo(f)) || (p.Faction == null && p.RaceProps.Animal && !p.InAggroMentalState) || (p.guest != null && p.guest.Released);
        }


        public override string GetInspectString()
        {
          StringBuilder stringBuilder = new StringBuilder();
          //            stringBuilder.Append(base.GetInspectString());
          if (this.LifeStage == PlantLifeStage.Growing)
          {
            stringBuilder.AppendLine(Verse.TranslatorFormattedStringExtensions.Translate("PercentGrowth: {0}", this.GrowthPercentString));
            stringBuilder.AppendLine(Verse.TranslatorFormattedStringExtensions.Translate("GrowthRate   : {0}", this.GrowthRate.ToStringPercent()));
            if (this.Resting)
            {
              stringBuilder.AppendLine("PlantResting".Translate());
            }
            if (!this.HasEnoughLightToGrow)
            {
              stringBuilder.AppendLine("PlantNeedsLightLevel".Translate() + ": " + this.def.plant.growMinGlow.ToStringPercent());
            }
            float growthRateFactor_Temperature = this.GrowthRateFactor_Temperature;
            if (growthRateFactor_Temperature < 0.99f)
            {
              if (growthRateFactor_Temperature < 0.01f)
              {
                stringBuilder.AppendLine("OutOfIdealTemperatureRangeNotGrowing".Translate());
              }
              else
              {
                stringBuilder.AppendLine(
                    "OutOfIdealTemeratureRange".Translate(
                      Mathf.RoundToInt(
                        growthRateFactor_Temperature * 100f).ToString()));
              }
            }
          }
          else if (this.LifeStage == PlantLifeStage.Mature) {
            if (this.def.plant.Harvestable)
            {
              stringBuilder.AppendLine("ReadyToHarvest".Translate());
            }
            else
            {
              stringBuilder.AppendLine("Mature".Translate());
            }
          }

          //Log.Message("PlantExtended.GetInspectString()");

          //if ( this.def.ExtComps.Count != 0 )
          if (this.LifeStage == PlantLifeStage.Growing)
          {
            stringBuilder.AppendLine("Druid Plant is not mature");
          } else if (this.LifeStage == PlantLifeStage.Mature) {
            bool fl = false;

            foreach ( ExtInstComp ptp2 in this.ExtComps ) {
              ExtInstComp_PlantTrap ptp = ptp2 as ExtInstComp_PlantTrap;
              if ( ptp != null ) {
                if ( ptp.Armed ) { stringBuilder.AppendLine( ptp.Props.ArmedLabel );  fl = true; }
                else {
                  stringBuilder.AppendLine( ptp.Props.ArmedLabel + " in " + ptp.rearmAt + " ticks");
                }
              }
            }
            if ( fl == false ) { stringBuilder.AppendLine("No armed traps"); }
          }

          return stringBuilder.ToString();
        }



          public override float GrowthRate
          {
            get
            {
              Log.Message("GrowthRate");
              return this.GrowthRateFactor_Fertility * this.GrowthRateFactor_Temperature * this.GrowthRateFactor_Light;
            }
          }


          protected override float LeaflessTemperatureThresh
          {
            get
            {
              //                        Log.Message("Horribly inefficient");
              float num = 8f;
              float basetemp = 0f;
              //                        Log.Message("LeaflessTemperatureThresh 1");
              if ( this.thepot != null ) {
                //                            Log.Message("LeaflessTemperatureThresh 1.1");
                //                            this.thepot.RecalcTemps();
                basetemp = this.thepot.MinGrowthTemperature; //nt_mg;
              } else {
                Log.Message("LeaflessTemperatureThresh 1.2 - should never reach because thepot should be defined at spawn or something");
                this.initPot();
                basetemp = this.thepot.MinGrowthTemperature;//nt_mg;
              }
              //                        Log.Message("LeaflessTemperatureThresh 2 "+basetemp);
              return (float)this.HashOffset() * 0.01f % num - num + -2f + basetemp ;
            }
          }


          // cached values for facilities' impact on temperature... recalc on TimeLong()
          /*
             protected int nt_facs  = 0;
             protected float nt_mg  = PlantExtended.MinGrowthTemperature;
             protected float nt_mog = PlantExtended.MinOptimalGrowthTemperature;
             protected float nt_Mog = PlantExtended.MaxOptimalGrowthTemperature;
             protected float nt_Mg  = PlantExtended.MaxGrowthTemperature;
             protected virtual void RecalcTemps() {
             float temp_opt = 10f;
             float temp_extreme = 20f;
             this.nt_facs = 0;
             if ( this.thepot != null ) {
             CompAffectedByFacilities cc = this.thepot.GetComp<CompAffectedByFacilities>();
             this.nt_facs = cc.LinkedFacilitiesListForReading.Count;
             this.nt_mg  = PlantExtended.MinGrowthTemperature -        (this.nt_facs*temp_extreme);
             this.nt_mog = PlantExtended.MinOptimalGrowthTemperature - (this.nt_facs*temp_opt);
             this.nt_Mog = PlantExtended.MaxOptimalGrowthTemperature + (this.nt_facs*temp_opt);
             this.nt_Mg  = PlantExtended.MaxGrowthTemperature +        (this.nt_facs*temp_extreme);
             } // counf the facilities
             }
             */

          public new float GrowthRateFactor_Temperature
          {
            get
            {
              float num;
              /*
                 int tmod = 0;
                 if ( this.thepot != null ) {
                 CompAffectedByFacilities cc = this.thepot.GetComp<CompAffectedByFacilities>();
                 tmod = cc.LinkedFacilitiesListForReading.Count;
                 } // counf the facilities
                 float nt_mg  = PlantExtended.MinGrowthTemperature -        (tmod*temp_extreme);
                 float nt_mog = PlantExtended.MinOptimalGrowthTemperature - (tmod*temp_opt);
                 float nt_Mog = PlantExtended.MaxOptimalGrowthTemperature + (tmod*temp_opt);
                 float nt_Mg  = PlantExtended.MaxGrowthTemperature +        (tmod*temp_extreme);
                 */
              float retval = 1f;
              float nt_mg  = (this.thepot == null ? PlantExtended.MinGrowthTemperature:        this.thepot.MinGrowthTemperature);
              float nt_mog = (this.thepot == null ? PlantExtended.MinOptimalGrowthTemperature: this.thepot.MinOptimalGrowthTemperature);
              float nt_Mog = (this.thepot == null ? PlantExtended.MaxOptimalGrowthTemperature: this.thepot.MaxOptimalGrowthTemperature);
              float nt_Mg  = (this.thepot == null ? PlantExtended.MaxGrowthTemperature:        this.thepot.MaxGrowthTemperature);

              //            Log.Message("the pot "+(this.thepot == null)+" "+ nt_mg +" " + PlantExtended.MinGrowthTemperature);

              if (!GenTemperature.TryGetTemperatureForCell(base.Position, base.Map, out num))
              {
                retval = 1f;
              } else {
                if (num < nt_mog )
                {
                  retval = Mathf.InverseLerp(nt_mg,nt_mog,num);
                }
                else if (num > nt_Mog )
                {
                  retval = Mathf.InverseLerp(nt_Mg,nt_Mog,num);
                }
                else retval = 1f;
                //                Log.Message("GrowthRateFactor is tmod "+nt_mg+"<"+nt_mog+"<>"+nt_Mog+">"+nt_Mg + "  from "+num +" returning "+retval);
                // probably not working because not calling this GrowRateFactor_Temperature but the parent's
              }

              return retval;
            }
          }

        }
}
