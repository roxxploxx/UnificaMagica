using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace UnificaMagica
{
	public class Building_ArcaneHeater : Building_TempControl
	{
		private const float EfficiencyFalloffSpan = 100f;

		private CompRefuelable compRefuelable;

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map,respawningAfterLoad);
			this.compRefuelable = base.GetComp<CompRefuelable>();
			//delete this.compPower; this.compPower = null;
			//			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.GrowingFood, KnowledgeAmount.Total
		}

		public override void TickRare()
		{
//			this.GetRoom().Temperature += 10.0f;
//			return;
			if (this.compRefuelable.HasFuel)
			{
				float temperature = base.Position.GetTemperature(base.Map);
				float num;
				if (temperature < 20f)
				{
					num = 1f;
				}
				else if (temperature > 120f)
				{
					num = 0f;
				}
				else
				{
					num = Mathf.InverseLerp(120f, 20f, temperature);
				}
				float energyLimit = this.compTempControl.Props.energyPerSecond * num * 4.16666651f;
				float num2 = GenTemperature.ControlTemperatureTempChange(base.Position, base.Map, energyLimit, this.compTempControl.targetTemperature);
				bool flag = !Mathf.Approximately(num2, 0f);
				//CompProperties_Power props = this.compPowerTrader.Props;
				if (flag)
				{
					this.GetRoom().Group.Temperature += num2;
					//this.compPowerTrader.PowerOutput = -props.basePowerConsumption;
				}
				else
				{
					//this.compPowerTrader.PowerOutput = -props.basePowerConsumption * this.compTempControl.Props.lowPowerConsumptionFactor;
				}
				this.compTempControl.operatingAtHighPower = flag;
			}
		}
	}
}
