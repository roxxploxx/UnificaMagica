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


		public override void Tick()
		{
			// Log.Message("ArcaneCooler: TickRare");
			//this.GetRoom().Temperature -= 10.0f;
			//return;
			if (this.compRefuelable.HasFuel)
			{
				float ambientTemperature = base.AmbientTemperature;
				float num = (ambientTemperature < 20f) ? 1f : ((!(ambientTemperature > 120f)) ? Mathf.InverseLerp(120f, 20f, ambientTemperature) : 0f);
				float energyLimit = compTempControl.Props.energyPerSecond * num; //  * 4.16666651f;
				float num2 = GenTemperature.ControlTemperatureTempChange(base.Position, base.Map, energyLimit, compTempControl.targetTemperature);
				bool flag = !Mathf.Approximately(num2, 0f);
				// CompProperties_Power props = compPowerTrader.Props;
				if (flag)
				{
					this.GetRoomGroup().Temperature += num2;
					// compPowerTrader.PowerOutput = 0f - props.basePowerConsumption;
				}
				else
				{
					//compPowerTrader.PowerOutput = (0f - props.basePowerConsumption) * compTempControl.Props.lowPowerConsumptionFactor;
				}
				compTempControl.operatingAtHighPower = flag;

				/*
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
					num = Mathf.InverseLerp(-274f, 20f, temperature);
				}
				num *= -1f;
				float energyLimit = this.compTempControl.Props.energyPerSecond * num * 4.16666651f;
				float num2 = GenTemperature.ControlTemperatureTempChange(base.Position, base.Map, energyLimit, this.compTempControl.targetTemperature);
				// not really sure what these calculations are doing, so asuming the energyLimit is not realizing it's negative so taking fraction of it
				float num3 = Mathf.Abs(this.compTempControl.Props.energyPerSecond / energyLimit); // fraction of num2 that cna be reached
				num2 = num2 * num3;
				bool flag = !Mathf.Approximately(num2, 0f);
				if (flag) {
					this.GetRoom().Group.Temperature += num2;
				}
				this.GetRoom().Group.Temperature -= 10.0;
				//CompProperties_Power props = this.compPowerTrader.Props;
				// Log.Message("ArcaneCooler "+flag+" num:"+num+" num2 :"+num2+ " energyLimit "+energyLimit+" energyPerSecond "+this.compTempControl.Props.energyPerSecond);
				/ *if (flag)
				{
					this.GetRoom().Group.Temperature += num2; //mod
					this.compPowerTrader.PowerOutput =  -props.basePowerConsumption;
				}
				else
				{
					this.compPowerTrader.PowerOutput = -props.basePowerConsumption * this.compTempControl.Props.lowPowerConsumptionFactor;
				} * /
				this.compTempControl.operatingAtHighPower = flag;
				*/
				/*
				float appliedenergy = compTempControl.Props.energyPerSecond * 1.0f; //  * 4.16666651f;
				float tempchange = GenTemperature.ControlTemperatureTempChange(base.PositionHeld, base.MapHeld, appliedenergy, compTempControl.targetTemperature);
				// float tempchange = Building_ArcaneCooler.MyControlTemperatureTempChange(base.PositionHeld, base.MapHeld, -10000.0f, compTempControl.targetTemperature);
				// TODO here need to calculate how much energy this pushes, and plug that into the algorithm.
				Log.Message("Controlling TemperatureChange " + compTempControl.targetTemperature + " " + base.PositionHeld + "," + base.Position+ " " +tempchange);
				base.PositionHeld.GetRoomGroup(base.MapHeld).Temperature += tempchange;
				// GenTemperature.PushHeat(base.PositionHeld, base.MapHeld, 1000.0f); // Props.heatPerSecond * 4.16666651f);
				*/



			}
		}

		/*
		static float MyControlTemperatureTempChange(IntVec3 cell, Map map, float energyLimit, float targetTemperature)
		{
			RoomGroup roomGroup = cell.GetRoomGroup(map);
			if (roomGroup == null || roomGroup.UsesOutdoorTemperature)
			{
				Log.Message('  - no room');
				return 0f;
			}
			float b = energyLimit / (float)roomGroup.CellCount;
			float a = targetTemperature - roomGroup.Temperature;
			float num = 0f;
			if (energyLimit > 0f)
			{
				num = Mathf.Min(a, b);
				return Mathf.Max(num, 0f);
			}
			num = Mathf.Max(a, b);
			return Mathf.Min(num, 0f);
		}*/
		
	}
}
