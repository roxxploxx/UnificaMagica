// using System;
using RimWorld;
using Verse;

namespace UnificaMagica
{
	        
	public class Verb_FueledShot : Verb_Shoot
	{
		


		// NOTE: this adds refuelable to a Turret, but could easily create something that for a Pawn, looks for something on them that is refuelable.
		protected override bool TryCastShot()
		{
			bool flag = false;

			Building_TurretGun turret = this.caster as Building_TurretGun; // only works on 
			CompRefuelable compRefuelable = turret.GetComp<CompRefuelable> ();

			if (compRefuelable != null && compRefuelable.HasFuel)
			{
				flag = base.TryCastShot();
				if (flag)
				{
					compRefuelable.ConsumeFuel(compRefuelable.Props.fuelConsumptionRate);
				}
			}

			return flag;
		}
	}
}
