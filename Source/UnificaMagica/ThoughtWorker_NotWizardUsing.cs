using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace UnificaMagica
{
	// based upon ThoughtWorker_ApparelDamaged
	public class ThoughtWorker_NotWizardUsing: ThoughtWorker
	{
		//
		// Methods
		//
		protected override ThoughtState CurrentStateInternal (Pawn p)
		{
			int cnt = 0;

			// ignore if has WizardInclined
			if (this.def.nullifyingTraits != null) {
				for (int i = 0; i < this.def.nullifyingTraits.Count; i++) {
					if (p.story.traits.HasTrait (this.def.nullifyingTraits [i])) {
						return ThoughtState.Inactive;
					}
				}
			}

			List<Apparel> wornApparel = p.apparel.WornApparel;
			for (int i = 0; i < wornApparel.Count; i++) {
				CompWizardTech wizarditem = wornApparel[i].GetComp<CompWizardTech>();
				if ( wizarditem != null ) cnt ++;
			}

			if ( cnt > 0 ) return ThoughtState.ActiveAtStage(cnt);
			return ThoughtState.Inactive;
		}
	}
}
