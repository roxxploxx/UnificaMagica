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
		}

		public override void Tick()
		{
			if (this.compRefuelable.HasFuel)
			{
				/*float appliedenergy = compTempControl.Props.energyPerSecond * 1.0f; //  * 4.16666651f;
				float tempchange = GenTemperature.ControlTemperatureTempChange(base.PositionHeld, base.MapHeld, appliedenergy, compTempControl.targetTemperature);
				Log.Message("Controlling TemperatureChange " + compTempControl.targetTemperature + " " + base.PositionHeld + "," + base.Position + " " + tempchange);
				base.PositionHeld.GetRoomGroup(base.MapHeld).Temperature += tempchange;
				*/

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
				this.compTempControl.operatingAtHighPower = flag;*/
			}
		}
	}
}
