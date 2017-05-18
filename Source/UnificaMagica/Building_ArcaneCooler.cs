using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace UnificaMagica
{
	public class Building_ArcaneCooler : Building_TempControl
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
						//this.GetRoom().Temperature -= 10.0f;
						//return;
			if (this.compRefuelable.HasFuel)
			{
				float temperature = base.Position.GetTemperature(base.Map);
				float num;
				if (temperature > 20f)
				{
					num = 1f;
				}
				else if (temperature < -274f)
				{
					num = 0f;
				}
				else
				{
					num = Mathf.InverseLerp(-274f,20f, temperature);
				}
				num *= -1f;
				float energyLimit = this.compTempControl.Props.energyPerSecond * num * 4.16666651f;
				float num2 = GenTemperature.ControlTemperatureTempChange(base.Position, base.Map, energyLimit, this.compTempControl.targetTemperature);
				// not really sure what these calculations are doing, so asuming the energyLimit is not realizing it's negative so taking fraction of it
				float num3 = Mathf.Abs(this.compTempControl.Props.energyPerSecond / energyLimit); // fraction of num2 that cna be reached
				num2 = num2 * num3;
				bool flag = !Mathf.Approximately(num2, 0f);
				//CompProperties_Power props = this.compPowerTrader.Props;
				//Log.Message("ArcaneCooler "+flag+" num:"+num+" num2 :"+num2+ " energyLimit "+energyLimit+" energyPerSecond "+this.compTempControl.Props.energyPerSecond);
				if (flag)
				{
					this.GetRoom().Group.Temperature += num2; //mod
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
