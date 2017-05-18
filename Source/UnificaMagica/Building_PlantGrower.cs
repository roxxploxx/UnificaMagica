
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


    // Adds in functionality to deal with Facilities that affect temp
    public class UMBuilding_PlantGrower : RimWorld.Building_PlantGrower, IFacilityChangeNotifier {

        public void Notify_NewLink(UMCompFacility _f) {
            Log.Message("UMBuidling_PlantGrower.Notify_NewLink");
            this.dirty_tempsettings = true;
        }
        public void Notify_LinkRemoved(UMCompFacility _f) {
            Log.Message("UMBuidling_PlantGrower.Notify_LinkRemoved");
            this.dirty_tempsettings = true;
        }


        public override void SpawnSetup(Map m,bool respawningAfterLoad) {
            base.SpawnSetup(m,respawningAfterLoad);
            this.facilitiesComp = this.GetComp<CompAffectedByFacilities>();
            this.RecalcTemps();
        }


        public override void PostMake()
        {
            base.PostMake();
            this.facilitiesComp = this.GetComp<CompAffectedByFacilities>();
            this.RecalcTemps();
        }

        public override void TickRare() {
            base.TickRare();
            this.RecalcTemps();
        }

        public float MinGrowthTemperature { get { if ( this.dirty_tempsettings ) { this.RecalcTemps(); } return this.nt_mg; } }
        public float MinOptimalGrowthTemperature { get { if ( this.dirty_tempsettings ) { this.RecalcTemps(); } return this.nt_mog; } }
        public float MaxOptimalGrowthTemperature { get { if ( this.dirty_tempsettings ) { this.RecalcTemps(); } return this.nt_Mog; } }
        public float MaxGrowthTemperature { get { if ( this.dirty_tempsettings ) { this.RecalcTemps(); } return this.nt_Mg; } }

        // cached values for facilities' impact on temperature... recalc on TimeLong()
        public CompAffectedByFacilities facilitiesComp;
        protected bool dirty_tempsettings;
        protected float nt_mg  = Plant.MinGrowthTemperature;
        protected float nt_mog = Plant.MinOptimalGrowthTemperature;
        protected float nt_Mog = Plant.MaxOptimalGrowthTemperature;
        protected float nt_Mg  = Plant.MaxGrowthTemperature;
        protected virtual void RecalcTemps() {
//            Log.Message("RecalcTemps");
//            float temp_opt = 10f; // needto make these part of the Compoennt definition in XML
//            float temp_extreme = 20f;
            int nt_facs = this.facilitiesComp.LinkedFacilitiesListForReading.Count;
            //CompAffectedByFacilities cc = this.GetComp<CompAffectedByFacilities>();
            //this.nt_facs = cc.LinkedFacilitiesListForReading.Count;

/*
            this.nt_mg  = Plant.MinGrowthTemperature -        (nt_facs*temp_extreme);
            this.nt_mog = Plant.MinOptimalGrowthTemperature - (nt_facs*temp_opt);
            this.nt_Mog = Plant.MaxOptimalGrowthTemperature + (nt_facs*temp_opt);
            this.nt_Mg  = Plant.MaxGrowthTemperature +        (nt_facs*temp_extreme);
            */
            this.nt_mg  = Plant.MinGrowthTemperature;
            this.nt_mog = Plant.MinOptimalGrowthTemperature;
            this.nt_Mog = Plant.MaxOptimalGrowthTemperature;
            this.nt_Mg  = Plant.MaxGrowthTemperature;
            foreach ( Thing th in this.facilitiesComp.LinkedFacilitiesListForReading ) {
                ThingWithComps twc = th as ThingWithComps;
                if ( twc != null ) {
                    IEnumerable<CompFacility> cfl = twc.GetComps<CompFacility>();
                    foreach (CompFacility cf in cfl ) {
                        CompProperties_Facility props = cf.Props;
                        if (props.statOffsets == null) { return ; }
                        //Log.Message("CompFacility has "+props.statOffsets.Count+" stats");
                        for (int i = 0; i < props.statOffsets.Count; i++)
                        {
                            StatModifier statModifier = props.statOffsets[i];
                            if ( statModifier.stat.defName == "PlantMinTemp" ) { this.nt_mg += statModifier.value; }
                            else if ( statModifier.stat.defName == "PlantMaxTemp" ) { this.nt_Mg += statModifier.value; }
                            else if ( statModifier.stat.defName == "PlantMinOptimalTemp" ) { this.nt_mog += statModifier.value; }
                            else if ( statModifier.stat.defName == "PlantMaxOptimalTemp" ) { this.nt_Mog += statModifier.value; }
                            else {
                                //Log.Message("Unknown stat of type "+statModifier.stat.defName);
                            }
                        }
                    }
                }
            }
            this.dirty_tempsettings = false;

        }


        // had to override... unless fix GenPlant.GrowthSeasonNow
        // fix WorkGiver_GrowerSow by overriding it
        public override string GetInspectString()
        {
            //Log.Message("UMBuilding_PlantGrower 1");
            string text = base.GetInspectString();
            text = text.Remove(text.LastIndexOf(Environment.NewLine)); // remove last line
            if (base.Spawned)
            {
                float num;
                if ( ! GenTemperature.TryGetTemperatureForCell(base.Position, base.Map, out num)) {
                    text = text + "\n" + "CannotGrowTooCold".Translate();
                } else {
                    if ( num > this.MinGrowthTemperature && num < this.MaxGrowthTemperature) { text = text + "\n" + "GrowSeasonHereNow".Translate(); }
                    else { text = text + "\n" + "CannotGrowTooCold".Translate(); }
                }

                //if (GenPlant.GrowthSeasonNow(base.Position, base.Map)) { text = text + "\n" + "GrowSeasonHereNow".Translate(); }
                //else { text = text + "\n" + "CannotGrowTooCold".Translate(); }
            }
            return text;
        }

    }

}
