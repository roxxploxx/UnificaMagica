using System;
using System.Collections.Generic;
using Verse;
using RimWorld;


// <summary>Directy copy of CompProperites_Facility to enable notification to thing
namespace UnificaMagica
{
	public class UMCompProperties_Facility : CompProperties
	{
		[Unsaved]
		public List<ThingDef> linkableBuildings;

		public List<StatModifier> statOffsets;

		public int maxSimultaneous = 1;

		public bool mustBePlacedAdjacent;

		public bool canLinkToMedBedsOnly;

		public UMCompProperties_Facility()
		{
			this.compClass = typeof(UnificaMagica.UMCompFacility);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			Log.Message("UMCompProperties ResolveReferences to parent of "+parentDef.thingClass);
			this.linkableBuildings = new List<ThingDef>();
			List<ThingDef> allDefsListForReading = DefDatabase<ThingDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				CompProperties_AffectedByFacilities compProperties = allDefsListForReading[i].GetCompProperties<CompProperties_AffectedByFacilities>();
				if (compProperties != null && compProperties.linkableFacilities != null)
				{
					for (int j = 0; j < compProperties.linkableFacilities.Count; j++)
					{
						if (compProperties.linkableFacilities[j] == parentDef)
						{
							Log.Message("Found type "+compProperties.linkableFacilities[j].thingClass+" pointing to this thing "+parentDef.thingClass);
							this.linkableBuildings.Add(allDefsListForReading[i]);
							break;
						}
					}
				}
			}
		}
	}



    public class UMPlaceWorker_ShowFacilitiesConnections : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            if (def.HasComp(typeof(CompAffectedByFacilities)))
            {
                CompAffectedByFacilities.DrawLinesToPotentialThingsToLinkTo(def, center, rot, base.Map);
            }
            else
            {
                UMCompFacility.DrawLinesToPotentialThingsToLinkTo(def, center, rot, base.Map);
            }
        }
    }


}
