using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;


namespace UnificaMagica
{


	// NOTE: This is COMPLETELY taken from Building_TurretGun. Look for ADDITION tags to see differences
	[StaticConstructorOnStartup]
	public class Building_WizardTurret : Building_TurretGun
	{

		// ADDITION
		protected CompRefuelable refuelableComp;
		// /ADDITION


		public override void SpawnSetup(Map map,bool respawningAfterLoad)
		{
			base.SpawnSetup(map,respawningAfterLoad);
			this.refuelableComp = base.GetComp<RimWorld.CompRefuelable>();
		}

		public override void Tick()
		{
			if (this.refuelableComp != null && !this.refuelableComp.HasFuel)
			{
				return;
			}
			base.Tick();

			// if have a target, and cooldown ticks is zero, then firing, so pull from fuel.
			if (this.CurrentTarget != null && this.CurrentTarget.IsValid && this.burstWarmupTicksLeft == 0 &&
			    this.GunCompEq.PrimaryVerb.CanHitTarget(this.CurrentTarget ) )
			{
				if (this.burstCooldownTicksLeft == 0)
					this.refuelableComp.ConsumeFuel(this.refuelableComp.Props.fuelConsumptionRate);
			}
		}



	}

}
