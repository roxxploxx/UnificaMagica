using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using AbilityUser;

namespace UnificaMagica
{
    public class Verb_Blink : Verb_UseAbility
    {

//        public PawnAbility ability = null;


/*        public VerbProperties_Ability useAbilityProps
        {
            get
            {
                return (VerbProperties_Ability)verbProps;
            }
        }
        */

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ) {
//            Log.Error("Blink can't seem to target ground");
            Log.Message("CanHitTargetFrom : Blink in TryCastShot with root"+root+" and target"+targ);
            if ( this.UseAbilityProps.AbilityTargetCategory != AbilityTargetCategory.TargetLocation ) {
                return base.CanHitTargetFrom(root,targ);
            }
            ShootLine shootLine; // shootLine seems wasted
            return this.TryFindShootLineFromTo(root, targ, out shootLine);
        }

        protected override bool TryCastShot()
        {
            bool result = false;

//            Log.Message("Blink in TryCastShot ");
//            Log.Message("  Cell  : "+this.currentTarget.Cell +" " +this.currentTarget.Cell.IsValid);
            if ( this.currentTarget != null && this.CasterPawn != null && this.currentTarget.Cell != null && this.currentTarget.Cell.IsValid ) {
//                Log.Message("  success in Verb_Blink.TryCastShot");
                this.CasterPawn.SetPositionDirect( this.currentTarget.Cell );
                result = true;
            } else {
                Log.Warning("failed to TryCastShot");
            }

            this.burstShotsLeft = 0;
//            Log.Message("  abilty : "+this.ability);
            this.Ability.CooldownTicksLeft = (int)this.UseAbilityProps.SecondsToRecharge * GenTicks.TicksPerRealSecond;

            return result;
        }

/*
        protected override bool TryCastShot()
        {
            Thing thing = this.currentTarget.Thing;
            Pawn casterPawn = base.CasterPawn;
            FireUtility.TryStartFireIn(thing.OccupiedRect().ClosestCellTo(casterPawn.Position), casterPawn.Map, 0.3f);
            casterPawn.Drawer.Notify_MeleeAttackOn(thing);
            return true;
        }
*/
    }
}
