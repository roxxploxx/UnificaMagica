using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;


namespace UnificaMagica {

    public class UMWorkGiver_GrowerSow : RimWorld.WorkGiver_GrowerSow
    {

        // copied over to change the GrowthSeasonNow
        public override Job JobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
        {
            if (c.IsForbidden(pawn))
            {
                return null;
            }
            /* CUT : replaced with code lower, to use plant temps instead of static value here
            if (!GenPlant.GrowthSeasonNow(c, pawn.Map))
            {
                return null;
            }
            */
            if (WorkGiver_Grower.wantedPlantDef == null)
            {
                WorkGiver_Grower.wantedPlantDef = WorkGiver_Grower.CalculateWantedPlantDef(c, pawn.Map);
                if (WorkGiver_Grower.wantedPlantDef == null)
                {
                    return null;
                }
            }
            UMBuilding_PlantGrower pg = null;
            List<Thing> thingList = c.GetThingList(pawn.Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing thing = thingList[i];
                if (thing.def == WorkGiver_Grower.wantedPlantDef)
                {
                    return null;
                }
                if ((thing is Blueprint || thing is Frame) && thing.Faction == pawn.Faction)
                {
                    return null;
                }
                if ( thing is UMBuilding_PlantGrower ) {
                    pg = thing as UMBuilding_PlantGrower;
                }
            }

            // ADDED: if normal tile, do normal temp, otherwise figure out from PlantGrower
            if ( pg != null ) {
//                pg.RecalcTemps();
//                Log.Message("WorkGiver_GrowerSow checking Building_PlantGrower : "+pg.MinGrowthTemperature+" <> "+pg.MaxGrowthTemperature);
                float temp = c.GetTemperature(pawn.Map);
                if ( temp < pg.MinGrowthTemperature || temp > pg.MaxGrowthTemperature) {
                    //Log.Message("   Too hot or too cold");
                    return null;
                }
            } else { if (!PlantUtility.GrowthSeasonNow(c, pawn.Map)) { return null; } }

            Plant plant = c.GetPlant(pawn.Map);
//            Log.Message("WorkGiver_GrowerSow plant is "+plant+ " from plantdef "+WorkGiver_Grower.wantedPlantDef);
            if (plant != null && plant.def.plant.blockAdjacentSow)
            {
                if (!pawn.CanReserve(plant, 1) || plant.IsForbidden(pawn))
                {
                    return null;
                }
                return new Job(JobDefOf.CutPlant, plant);
            }
            else
            {
                Thing thing2 = PlantUtility.AdjacentSowBlocker(WorkGiver_Grower.wantedPlantDef, c, pawn.Map);
                if (thing2 != null)
                {
                    Plant plant2 = thing2 as Plant;
                    if (plant2 != null && pawn.CanReserve(plant2, 1) && !plant2.IsForbidden(pawn))
                    {
                        Zone_Growing zone_Growing = pawn.Map.zoneManager.ZoneAt(plant2.Position) as Zone_Growing;
                        if (zone_Growing == null || zone_Growing.GetPlantDefToGrow() != plant2.def)
                        {
                            return new Job(JobDefOf.CutPlant, plant2);
                        }
                    }
                    return null;
                }
                if (WorkGiver_Grower.wantedPlantDef.plant.sowMinSkill > 0 && pawn.skills != null && pawn.skills.GetSkill(SkillDefOf.Plants).Level < WorkGiver_Grower.wantedPlantDef.plant.sowMinSkill)
                {
                    return null;
                }
                int j = 0;
                while (j < thingList.Count)
                {
                    Thing thing3 = thingList[j];
                    if (thing3.def.BlocksPlanting())
                    {
                        if (!pawn.CanReserve(thing3, 1))
                        {
                            return null;
                        }
                        if (thing3.def.category == ThingCategory.Plant)
                        {
                            if (!thing3.IsForbidden(pawn))
                            {
                                return new Job(JobDefOf.CutPlant, thing3);
                            }
                            return null;
                        }
                        else
                        {
                            if (thing3.def.EverHaulable)
                            {
                                return HaulAIUtility.HaulAsideJobFor(pawn, thing3);
                            }
                            return null;
                        }
                    }
                    else
                    {
                        j++;
                    }
                }
                //if (!WorkGiver_Grower.wantedPlantDef.CanEverPlantAt(c, pawn.Map) || !GenPlant.GrowthSeasonNow(c, pawn.Map) || !pawn.CanReserve(c, 1)) // redundant
                if (!WorkGiver_Grower.wantedPlantDef.CanEverPlantAt(c, pawn.Map) || !pawn.CanReserve(c, 1))
                {
                    return null;
                }
                return new Job(JobDefOf.Sow, c)
                {
                    plantDefToSow = WorkGiver_Grower.wantedPlantDef
                };
            }
        }
    }
}
