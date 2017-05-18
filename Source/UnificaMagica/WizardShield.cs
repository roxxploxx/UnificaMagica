using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace UnificaMagica
{
	[StaticConstructorOnStartup]
	public class WizardShield : Apparel
	{
		// turn off 'can't fire at range'
		/*public virtual bool AllowVerbCast(IntVec3 root, Map map, LocalTargetInfo targ)
		//public override bool AllowVerbCast(IntVec3 root, TargetInfo targ)
		{
			return true;
			//if (targ.HasThing && targ.Thing.def.size != IntVec2.One)
			//{
			//	return root.AdjacentTo8WayOrInside(targ.Thing);
			//}
			//return root.AdjacentTo8Way(targ.Cell);
		}
		*/
		//
		// Static Fields
		//
		private const float MinDrawSize = 1.2f;

		private static readonly Material BubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent);

		private const int JitterDurationTicks = 8;

		private const float MaxDamagedJitterDist = 0.05f;

		private const float MaxDrawSize = 1.55f;

		//
		// Fields
		//
		private float EnergyOnReset = 0.2f;

		private float EnergyLossPerDamage = 0.027f;

		private int KeepDisplayingTicks = 1000;

		private float ApparelScorePerEnergyMax = 0.25f;

		private int StartingTicksToReset = 3200;

		private float energy;

		private int ticksToReset = -1;

		private int lastKeepDisplayTick = -9999;

		private Vector3 impactAngleVect;

		private int lastAbsorbDamageTick = -9999;

		//
		// Properties
		//
		public float Energy
		{
			get
			{
				return this.energy;
			}
		}

		private float EnergyGainPerTick
		{
			get
			{
				return this.GetStatValue(StatDefOf.EnergyShieldRechargeRate, true) / 60f;
			}
		}

		private float EnergyMax
		{
			get
			{
				return this.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true);
			}
		}

		public ShieldState ShieldState
		{
			get
			{
				if (this.ticksToReset > 0)
				{
					return ShieldState.Resetting;
				}
				return ShieldState.Active;
			}
		}

		private bool ShouldDisplay
		{
			get
			{
				return !this.Wearer.Dead && !this.Wearer.Downed && (!this.Wearer.IsPrisonerOfColony || (this.Wearer.MentalStateDef != null && this.Wearer.MentalStateDef.IsAggro)) && (this.Wearer.Drafted || this.Wearer.Faction.HostileTo(Faction.OfPlayer) || Find.TickManager.TicksGame < this.lastKeepDisplayTick + this.KeepDisplayingTicks);
			}
		}

		//
		// Methods
		//
		private void AbsorbedDamage(DamageInfo dinfo)
		{
			SoundDefOf.EnergyShieldAbsorbDamage.PlayOneShot(new TargetInfo(this.Wearer.Position, this.Wearer.Map, false));
			this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
			Vector3 loc = this.Wearer.TrueCenter() + this.impactAngleVect.RotatedBy(180f) * 0.5f;
			float num = Mathf.Min(10f, 2f + (float)dinfo.Amount / 10f);
			MoteMaker.MakeStaticMote(loc, this.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, num);
			int num2 = (int)num;
			for (int i = 0; i < num2; i++)
			{
				MoteMaker.ThrowDustPuff(loc, this.Wearer.Map, Rand.Range(0.8f, 1.2f));
			}
			this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
			this.KeepDisplaying();
		}



		private void Break()
		{
			SoundDefOf.EnergyShieldBroken.PlayOneShot(new TargetInfo(this.Wearer.Position, this.Wearer.Map, false));
			MoteMaker.MakeStaticMote(this.Wearer.TrueCenter(), this.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
			for (int i = 0; i < 6; i++)
			{
				Vector3 loc = this.Wearer.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
				MoteMaker.ThrowDustPuff(loc, this.Wearer.Map, Rand.Range(0.8f, 1.2f));
			}
			this.energy = 0f;
			this.ticksToReset = this.StartingTicksToReset;
		}

		public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
		{
			if (this.ShieldState == ShieldState.Active && ((dinfo.Instigator != null && !dinfo.Instigator.Position.AdjacentTo8WayOrInside(this.Wearer.Position)) || dinfo.Def.isExplosive))
			{
				if (dinfo.Instigator != null)
				{
					AttachableThing attachableThing = dinfo.Instigator as AttachableThing;
					if (attachableThing != null && attachableThing.parent == this.Wearer)
					{
						return false;
					}
				}
				this.energy -= (float)dinfo.Amount * this.EnergyLossPerDamage;
				if (dinfo.Def == DamageDefOf.EMP)
				{
					this.energy = -1f;
				}
				if (this.energy < 0f)
				{
					this.Break();
				}
				else
				{
					this.AbsorbedDamage(dinfo);
				}
				return true;
			}
			return false;
		}

		public override void DrawWornExtras()
		{
			if (this.ShieldState == ShieldState.Active && this.ShouldDisplay)
			{
				float num = Mathf.Lerp(1.2f, 1.55f, this.energy);
				Vector3 vector = this.Wearer.Drawer.DrawPos;
				vector.y = Altitudes.AltitudeFor(AltitudeLayer.MoteOverhead);
				int num2 = Find.TickManager.TicksGame - this.lastAbsorbDamageTick;
				if (num2 < 8)
				{
					float num3 = (float)(8 - num2) / 8f * 0.05f;
					vector += this.impactAngleVect * num3;
					num -= num3;
				}
				float angle = (float)Rand.Range(0, 360);
				Vector3 s = new Vector3(num, 1f, num);
				Matrix4x4 matrix = default(Matrix4x4);
				matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
				Graphics.DrawMesh(MeshPool.plane10, matrix, WizardShield.BubbleMat, 0);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.energy, "energy", 0f, false);
			Scribe_Values.Look<int>(ref this.ticksToReset, "ticksToReset", -1, false);
			Scribe_Values.Look<int>(ref this.lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
		}

		public override float GetSpecialApparelScoreOffset()
		{
			return this.EnergyMax * this.ApparelScorePerEnergyMax;
		}


		public void KeepDisplaying()
		{
			this.lastKeepDisplayTick = Find.TickManager.TicksGame;
		}

		private void Reset()
		{
			if (this.Wearer.Spawned)
			{
				SoundDefOf.EnergyShieldReset.PlayOneShot(new TargetInfo(this.Wearer.Position, this.Wearer.Map, false));
				MoteMaker.ThrowLightningGlow(this.Wearer.TrueCenter(), this.Wearer.Map, 3f);
			}
			this.ticksToReset = -1;
			this.energy = this.EnergyOnReset;
		}

		public override void Tick()
		{
			base.Tick();
			if (this.Wearer == null)
			{
				this.energy = 0f;
				return;
			}
			if (this.ShieldState == ShieldState.Resetting)
			{
				this.ticksToReset--;
				if (this.ticksToReset <= 0)
				{
					this.Reset();
				}
			}
			else if (this.ShieldState == ShieldState.Active)
			{
				this.energy += this.EnergyGainPerTick;
				if (this.energy > this.EnergyMax)
				{
					this.energy = this.EnergyMax;
				}
			}
		}


	}
}
