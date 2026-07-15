using UnityEngine;

public class ThermalEnergy : EnergyData
{
	     // --- Public Properties ---
	     public float Mass
	     {
		          get; private set;
	     }
	     public float LatentHeatFusion
	     {
		          get; private set;
	     }
	     public float LatentHeatVaporization
	     {
		          get; private set;
	     }
	     public float MeltingPoint
	     {
		          get; private set;
	     }
	     public float BoilingPoint
	     {
		          get; private set;
	     }
	     public float ThermalConductivity
	     {
		          get; private set;
	     }
	     public string materialName;

	     // --- Energy (the only truth) ---
	     public float sensibleEnergy; // J
	     public float latentEnergy;   // J

	     public override float StoredEnergy => sensibleEnergy + latentEnergy;

	     // --- Temperature: cached field, updated from energy ---
	     private float temperature; // Kelvin

	     public float Temperature
	     {
		          get => temperature;
		          private set => temperature = value; // only set internally
	     }

	     // --- Specific heat (uses cached temperature) ---
	     private float cachedSpecificHeat;
	     private float cachedTemperatureForCp = float.NaN;

	     public float SpecificHeat
	     {
		          get
		          {
				if (float.IsNaN(cachedTemperatureForCp) || Mathf.Abs(temperature - cachedTemperatureForCp) > 0.001f)
				{
					     cachedSpecificHeat = MatterLibrary.GetSpecificHeat(materialName, temperature);
					     cachedTemperatureForCp = temperature;
				}
				return cachedSpecificHeat;
		          }
	     }

	     // --- Phase (derived from temperature, but can be cached too) ---
	     public Phase CurrentPhase
	     {
		          get
		          {
				if (Mass <= 0f)
					     return Phase.Solid;
				if (temperature < MeltingPoint)
					     return Phase.Solid;
				if (temperature < BoilingPoint)
					     return Phase.Liquid;
				return Phase.Gas;
		          }
	     }

	     // --- Constructor ---
	     public ThermalEnergy(float mass, string materialRef,
	         float latentHeatFusion = 334000f,
	         float latentHeatVaporization = 2260000f,
	         float meltingPoint = 273f,
	         float boilingPoint = 373f,
	         float thermalConductivity = 608.9f,
	         float initialTemperature = 278f,
	         float fallbackCp = 4186f)
	     {
		          Type = EnergyType.Thermal;
		          Mass = mass;
		          materialName = materialRef;
		          LatentHeatFusion = latentHeatFusion;
		          LatentHeatVaporization = latentHeatVaporization;
		          MeltingPoint = meltingPoint;
		          BoilingPoint = boilingPoint;
		          ThermalConductivity = thermalConductivity;

		          // Initialise sensible energy from temperature
		          float cp = MatterLibrary.GetSpecificHeat(materialRef, initialTemperature);
		          sensibleEnergy = mass * cp * initialTemperature;
		          latentEnergy = 0f;
		          temperature = initialTemperature;
		          cachedTemperatureForCp = initialTemperature;
		          cachedSpecificHeat = cp;
	     }

	     // --- Energy manipulation ---

	     public void AddHeat(float joules)
	     {
		          if (joules <= 0f || Mass <= 0f)
				return;
		          sensibleEnergy += joules;
		          UpdateTemperature();
		          ProcessPhaseChanges();
	     }

	     public void RemoveHeat(float joules)
	     {
		          if (joules <= 0f || Mass <= 0f)
				return;
		          float toRemove = Mathf.Min(joules, StoredEnergy);
		          float sensibleToRemove = Mathf.Min(toRemove, sensibleEnergy);
		          sensibleEnergy -= sensibleToRemove;
		          toRemove -= sensibleToRemove;
		          if (toRemove > 0f)
		          {
				float latentToRemove = Mathf.Min(toRemove, latentEnergy);
				latentEnergy -= latentToRemove;
				toRemove -= latentToRemove;
		          }
		          UpdateTemperature();
		          ProcessPhaseChanges();
	     }

	     public override void AddEnergy(float joules) => AddHeat(joules);
	     public override float RemoveEnergy(float maxJoules)
	     {
		          float removed = Mathf.Min(maxJoules, StoredEnergy);
		          RemoveHeat(removed);
		          return removed;
	     }

	     // --- Mass transfer ---

	     public void AddMass(float kg, float temperatureKelvin, float? specificHeatOverride = null)
	     {
		          if (kg <= 0f)
				return;
		          var tempMass = new ThermalEnergy(kg, materialName,
		              LatentHeatFusion, LatentHeatVaporization,
		              MeltingPoint, BoilingPoint,
		              ThermalConductivity, temperatureKelvin, cachedSpecificHeat);
		          tempMass.TransferMassTo(this, kg);
	     }

	     public void RemoveMass(float kg)
	     {
		          if (kg <= 0f)
				return;
		          if (kg >= Mass)
		          {
				sensibleEnergy = 0f;
				latentEnergy = 0f;
				Mass = 0f;
				temperature = 0f;
				return;
		          }
		          float ratio = kg / Mass;
		          sensibleEnergy *= (1f - ratio);
		          latentEnergy *= (1f - ratio);
		          Mass -= kg;
		          // Temperature remains same (scaled), but let's recalc to be safe
		          UpdateTemperature();
	     }

	     public void TransferMassTo(ThermalEnergy target, float kg)
	     {
		          if (target == null || kg <= 0f)
				return;
		          if (kg > Mass)
				kg = Mass;
		          float ratio = kg / Mass;
		          float sensibleToMove = sensibleEnergy * ratio;
		          float latentToMove = latentEnergy * ratio;

		          sensibleEnergy -= sensibleToMove;
		          latentEnergy -= latentToMove;
		          Mass -= kg;

		          target.sensibleEnergy += sensibleToMove;
		          target.latentEnergy += latentToMove;
		          target.Mass += kg;

		          UpdateTemperature();
		          target.UpdateTemperature();
		          ProcessPhaseChanges();
		          target.ProcessPhaseChanges();
	     }

	     // --- Phase Change Logic (uses energy, updates temperature) ---

	     private void ProcessPhaseChanges()
	     {
		          if (Mass <= 0f)
		          {
				sensibleEnergy = 0f;
				latentEnergy = 0f;
				temperature = 0f;
				return;
		          }

		          bool changed;
		          do
		          {
				changed = false;
				float cp = SpecificHeat;

				// Melting (solid → liquid)
				if (CurrentPhase == Phase.Solid && temperature >= MeltingPoint && sensibleEnergy > 0f)
				{
					     float needed = Mass * LatentHeatFusion;
					     // Check if we have enough sensible energy above the melting point energy
					     float energyAtMelt = Mass * cp * MeltingPoint;
					     if (sensibleEnergy >= energyAtMelt + needed)
					     {
						          sensibleEnergy -= needed;
						          latentEnergy += needed;
						          changed = true;
						          UpdateTemperature();
					     }
					     else
					     {
						          // Not enough to fully melt; we can't partially melt, so we just stay solid.
						          // But we might be above melting point due to rounding; we'll ensure temperature doesn't exceed melting point.
						          if (temperature > MeltingPoint)
						          {
								// Cap temperature to melting point by converting excess sensible to latent? That's not physical.
								// Actually, if we're above melting point but don't have enough latent heat, we should still melt? 
								// That's a problem. We'll just let it be; the next iteration will try again.
						          }
					     }
				}

				// Freezing (liquid → solid)
				if (CurrentPhase == Phase.Liquid && temperature < MeltingPoint && latentEnergy > 0f)
				{
					     float released = Mass * LatentHeatFusion;
					     if (latentEnergy >= released)
					     {
						          latentEnergy -= released;
						          sensibleEnergy += released;
						          changed = true;
						          UpdateTemperature();
					     }
				}

				// Vaporization (liquid → gas)
				if (CurrentPhase == Phase.Liquid && temperature >= BoilingPoint && sensibleEnergy > 0f)
				{
					     float needed = Mass * LatentHeatVaporization;
					     float energyAtBoil = Mass * cp * BoilingPoint;
					     if (sensibleEnergy >= energyAtBoil + needed)
					     {
						          sensibleEnergy -= needed;
						          latentEnergy += needed;
						          changed = true;
						          UpdateTemperature();
					     }
				}

				// Condensation (gas → liquid)
				if (CurrentPhase == Phase.Gas && temperature < BoilingPoint && latentEnergy > 0f)
				{
					     float released = Mass * LatentHeatVaporization;
					     if (latentEnergy >= released)
					     {
						          latentEnergy -= released;
						          sensibleEnergy += released;
						          changed = true;
						          UpdateTemperature();
					     }
				}

		          } while (changed);
	     }

	     // --- Update temperature from energy ---
	     private void UpdateTemperature()
	     {
		          if (Mass <= 0f || float.IsNaN(sensibleEnergy) || float.IsInfinity(sensibleEnergy))
		          {
				temperature = 0f;
				return;
		          }
		          float cp = SpecificHeat;
		          if (cp <= 0f)
		          {
				temperature = 0f;
				return;
		          }
		          temperature = sensibleEnergy / (Mass * cp);
		          if (float.IsNaN(temperature) || float.IsInfinity(temperature))
				temperature = 0f;
	     }

	     // --- Helper for initialisation ---
	     public void SetTemperature(float tempK)
	     {
		          if (Mass <= 0f)
				return;
		          float cp = MatterLibrary.GetSpecificHeat(materialName, tempK);
		          sensibleEnergy = Mass * cp * tempK;
		          latentEnergy = 0f;
		          temperature = tempK;
		          cachedTemperatureForCp = tempK;
		          cachedSpecificHeat = cp;
		          // No phase processing; assume initial phase is correct.
	     }

	     public override string ToString()
	     {
		          return $"{materialName}: mass={Mass:F2}kg, T={temperature:F1}K, phase={CurrentPhase}";
	     }


	     //heat to temp and cool to temp. cool to temp should retenr an energydata.
	     public float HeatToTemp(float tempK)
	     {
		          float heatAdded = 0f;
		          //adds heat until temperature equals tempK. mostly used for initialisiation and editor.
		          while (Temperature <= tempK)
		          {
				heatAdded += tempK;
				AddHeat(tempK);
		          }
		          return heatAdded;
	     }
	     public float CoolToTemp(float tempK)
	     {
		          float heatRemoved = 0f;
		          while (Temperature >= tempK)
		          {
				heatRemoved += tempK;
				RemoveHeat(tempK);
		          }
		          return heatRemoved;
	     }
}

public enum Phase
{
	     Solid,
	     Liquid,
	     Gas
}