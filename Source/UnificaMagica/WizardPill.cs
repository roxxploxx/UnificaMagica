
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Verse;

namespace UnificaMagica
{
	public class WizardPill : ThingWithComps
	{

        protected override void PostIngested (Pawn ingester)
        {
            base.PostIngested (ingester);
            ingester.story.traits.GainTrait( new Trait(TraitDef.Named("WizardInclined")) );


        }
    }
}
