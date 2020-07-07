using System;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;


namespace UnificaMagica
{

	[StaticConstructorOnStartup]
	public class Building_ArcaneGrowVat : Building_PlantGrower
	{
		private CompRefuelable compRefuelable;

		/*
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			IEnumerator<Gizmo> enumerator = base.GetGizmos().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Gizmo current = enumerator.Current;
				yield return current;
			}
			yield return PlantToGrowSettableUtility.SetPlantToGrowCommand(this);
			yield break;
		}*/
		/*
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building_PlantGrower.< GetGizmos > c__Iterator143 < GetGizmos > c__Iterator = new Building_PlantGrower.< GetGizmos > c__Iterator143();
			< GetGizmos > c__Iterator.<> f__this = this;
			Building_PlantGrower.< GetGizmos > c__Iterator143 expr_0E = < GetGizmos > c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}*/






		public new bool CanAcceptSowNow()
		{
			return this.compRefuelable == null || this.compRefuelable.HasFuel;
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map,respawningAfterLoad);
			this.compRefuelable = base.GetComp<CompRefuelable>();
			//delete this.compPower; this.compPower = null;
//			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.GrowingFood, KnowledgeAmount.Total);
		}

		public override void TickRare()
		{
			if (this.compRefuelable != null && !this.compRefuelable.HasFuel)
			{
				foreach (Plant current in this.PlantsOnMe)
				{
					DamageInfo dinfo = new DamageInfo(DamageDefOf.Rotting, 4, -1f); //, null, null, null);

					current.TakeDamage(dinfo);
				}
			}
		}
	}
}
