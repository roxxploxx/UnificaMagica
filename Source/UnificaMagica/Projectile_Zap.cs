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

/*	public class BeamProperties {
		public string texPath;
	} */

	public class SkillModifierProperties {
		public SkillDef Skill;
		public float fraction;
	}

	// overrides Explosion Projectiles by including here
	public class ZapProjectileProperties : Verse.ProjectileProperties {
		public bool  LightningBolt = false;    // lighting bolt animation
		public bool  InstantOldInjury = false;
		public int ExplosionDamageAmountBase = 0;  // damage from explosions
		public SkillModifierProperties DamageSkillModifier = null;
//		public BeamProperties Beam = null;
//		public SkillModifierDef ExplosionSkillModifier = null;
	}

	public class ZapProjectile : Verse.Projectile // RimWorld.Bullet //Verse.Projectile
	{
//		public static RimWorld.SkillDef Wizardry;// = null;

		private int ticksToDetonation;

		protected override void Impact(Thing hitThing)
		{
			Map map = base.Map;
			ZapProjectileProperties pp =  (ZapProjectileProperties) this.def.projectile;

			// destory now if not explosion
			if( pp.explosionRadius == 0  ) {
				base.Impact(hitThing); // destroy
			}

			// damage
			if (hitThing != null)
			{
				int damageAmountBase = pp.damageAmountBase;

				if ( pp.DamageSkillModifier != null ) {
					Verse.Pawn p = this.launcher as Verse.Pawn;
					RimWorld.SkillRecord wiz = p.skills.GetSkill(pp.DamageSkillModifier.Skill);
					damageAmountBase +=  (int) (((float)wiz.Level) * pp.DamageSkillModifier.fraction);
				}

				ThingDef equipmentDef = this.equipmentDef;
				DamageInfo dinfo = new DamageInfo(this.def.projectile.damageDef, damageAmountBase, this.ExactRotation.eulerAngles.y, this.launcher, null, equipmentDef);
				hitThing.TakeDamage(dinfo);


				// lightning strike!
				if ( pp.LightningBolt == true ) {
					Verse.Pawn p = this.launcher as Verse.Pawn;
					MoteMaker.ThrowLightningGlow(hitThing.Position.ToVector3(),map,2.5f);
					MeshBolt lightning = new MeshBolt(hitThing.Position, p.Position.ToVector3(), MeshBolt.Lightning) ;// MatLoader.LoadMat( pp.Beam.texPath) ); //MeshBolt.Lightning
					//LightningBolt lightning = new LightningBolt(hitThing.Position, p.Position.ToVector3());
	                lightning.CreateBolt();
	//				new WeatherEvent_LightningStrike (map, hitThing.Position);
//					Log.Warning("Hit with lightning");
				}
				Log.Warning("Hit with Zap");
			}
			else
			{
				RimWorld.SoundDefOf.BulletImpactGround.PlayOneShot(new TargetInfo(base.Position, map, false));
				RimWorld.MoteMaker.MakeStaticMote(this.ExactPosition, map, RimWorld.ThingDefOf.Mote_ShotHit_Dirt, 1f);
			}

			// handle explosions
			if( pp.explosionRadius > 0.0  ) {
				Log.Warning("Setting up explosion");
				if (pp.explosionDelay == 0) {
					this.Explode ();
					return;
				}
				this.landed = true;
				this.ticksToDetonation = pp.explosionDelay;
				GenExplosion.NotifyNearbyPawnsOfDangerousExplosive (this, DamageDefOf.Bomb, this.launcher.Faction);
			}

		}


		//
		// Methods
		//
		protected virtual void Explode ()
		{
			Log.Warning("Hitting with Explosion");
			ZapProjectileProperties pp =  (ZapProjectileProperties) this.def.projectile;
			Map map = base.Map;
			this.Destroy (DestroyMode.Vanish);
			ThingDef preExplosionSpawnThingDef = pp.preExplosionSpawnThingDef;
			float explosionSpawnChance = pp.explosionSpawnChance;

/*
			pp.damageAmountBase = pp.ExplosionDamageAmountBase; // set to explosion damage now
			if ( pp.ExplosionSkillModifier != null ) {
				Verse.Pawn p = this.launcher as Verse.Pawn;
				RimWorld.SkillRecord wiz = p.skills.GetSkill(pp.ExplosionSkillModifier.Skill);
				pp.damageAmountBase +=  (int) (((float)wiz.Level) * pp.ExplosionSkillModifier.fraction);
			}
			*/
			GenExplosion.DoExplosion (base.Position, map, pp.explosionRadius, DamageDefOf.Bomb, this.launcher,
					pp.soundExplode, this.def, this.equipmentDef, pp.postExplosionSpawnThingDef,
					pp.explosionSpawnChance, 1, false, preExplosionSpawnThingDef, explosionSpawnChance, 1);
		}

		public override void ExposeData ()
		{
			base.ExposeData ();
			Scribe_Values.Look<int> (ref this.ticksToDetonation, "ticksToDetonation", 0, false);
		}


		public override void Tick ()
		{
			base.Tick ();
			if (this.ticksToDetonation > 0) {
				this.ticksToDetonation--;
				if (this.ticksToDetonation <= 0) {
					this.Explode ();
				}
			}
		}
	}


	[StaticConstructorOnStartup]
	public class MeshBolt : Thing {
        public static readonly Material Lightning = MatLoader.LoadMat("Weather/LightningBolt");
//        public static readonly Material Beam = MatLoader.LoadMat("Weather/BeamBolt");

		private IntVec3 hitThing;
		private Vector3 origin;
		private Mesh boltMesh;
		private Quaternion direction;
		private Material mat;


		public MeshBolt(IntVec3 hitThing, Vector3 origin, Material _mat)
		{
			this.hitThing = hitThing;
			this.origin = origin;
			this.mat = _mat;
		}

		public void CreateBolt()
		{
			Vector3 hitPosition;
			hitPosition.x = hitThing.x;
			hitPosition.y = hitThing.y;
			hitPosition.z = hitThing.z;
			this.direction = Quaternion.LookRotation((hitPosition - origin).normalized);
			float distance = Vector3.Distance(origin, hitPosition);
			this.boltMesh = ModBeamMeshMaker.NewBoltMesh(distance); //MeshBoltMaker.NewBolt(distance);//

			Graphics.DrawMesh(
				this.boltMesh,
				origin,
				this.direction,
				this.mat, //LightningMat,
				0);
		}
	}
}
