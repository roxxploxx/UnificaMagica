using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using AbilityUser;

namespace UnificaMagica
{

    public class CompAbilityUserWizard : CompAbilityUser
    {
/*
        public int ii = 1;
        public int jj;

        public CompAbilityUserWizard() {
            Log.Message("CompAblityUserWizard");
        }
        */
        //public AbilityDef wizardbolt = UnificaMagicaDefOf.WizardBoltAbilityDef;// AbilityDefOf.WizardBoltDef;

        public override void CompTick() {
            base.CompTick();
            if ( IsInitialized ) {
            }
        }

        public override void PostInitialize() {
            base.PostInitialize();
            this.AddPawnAbility(UnificaMagicaDefOf.WizardBoltAbility);
        }

        public override bool TryTransformPawn() {
            Pawn p = this.abilityUser;
//            Log.Message("CompAblityUserWizard.TryTransformPawn "+p.ThingID+" "+(p.Name != null ? p.Name.ToStringFull:" unnamed"));
            bool retval = false;

            if ( p.story != null && p.story.traits != null) {
                if (p.story.traits.HasTrait(UnificaMagicaDefOf.WizardInclined)) {
//                    Log.Message("CompAblityUserWizard.TryTransformPawn true");
                    retval = true;
                }
            }

            return retval;
        }

    }

}
