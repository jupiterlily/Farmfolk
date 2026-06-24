using UnityEngine;

public class ThermalEnergy : EnergyData
{
	     //tracks mass, temperature, phase and internal energy.
	     // --- Public Properties ---
	     private float temperature;  // Kelvin

	     public float Temperature
	     {
		          get => temperature;
		          private set
		          {
				if (Mathf.Abs(temperature - value) > 0.001f)
				{
					     temperature = value;
					     // Invalidate cache because temperature changed
					     cachedTemperatureForCp = float.NaN;
				}
		          }
	     }
	     private float cachedSpecificHeat;
	     private float cachedTemperatureForCp = float.NaN;

	     public float SpecificHeat
	     {
		          get
		          {
				// Recompute only if temperature has changed noticeably
				if (float.IsNaN(cachedTemperatureForCp) ||
				    Mathf.Abs(temperature - cachedTemperatureForCp) > 0.001f)
				{
					     cachedSpecificHeat = MatterLibrary.GetSpecificHeat(materialName, temperature);
					     cachedTemperatureForCp = temperature;
				}
				return cachedSpecificHeat;
		          }
	     }
	     public Phase CurrentPhase
	     {
		          get; private set;
	     }
	     public float Mass
	     {
		          get; private set;
	     }                 // kg
	     public float LatentHeatFusion
	     {
		          get; private set;
	     }     // J/kg
	     public float LatentHeatVaporization
	     {
		          get; private set;
	     } // J/kg
	     public float MeltingPoint
	     {
		          get; private set;
	     }         // Kelvin
	     public float BoilingPoint
	     {
		          get; private set;
	     }         // Kelvin
	     public float ThermalConductivity
	     {
		          get; private set;
	     }  // W/m·K

	     public override float StoredEnergy => sensibleEnergy + latentEnergy;

	     // --- Private Fields ---
	     private float sensibleEnergy; // J, affects temperature
	     private float latentEnergy;   // J, stored in phase bonds

	     public string materialName;  // used to look up cp from MatterLibrary
	     private float defaultCp;      // fallback if material not found

	     //private ThermalEnergy environ; //location that heat from  phase change goes or gets removed from. can be a soil component, an atmosphere component
	     // --- Constructor ---
	     public ThermalEnergy(float mass, string materialRef,
		           float latentHeatFusion = 334000f,
		           float latentHeatVaporization = 2260000f,
		           float meltingPoint = 273f,
		           float boilingPoint = 373f,
		           float thermalConductivity = 0.6089f,
		           float initialTemperature = 278f,
		           float fallbackCp = 4186f)
	     {
		          Type = EnergyType.Thermal;
		          Mass = mass;
		          materialName = materialRef;
		          defaultCp = fallbackCp;

		          LatentHeatFusion = latentHeatFusion;
		          LatentHeatVaporization = latentHeatVaporization;
		          MeltingPoint = meltingPoint;
		          BoilingPoint = boilingPoint;
		          ThermalConductivity = thermalConductivity;

		          // Determine initial phase
		          CurrentPhase = initialTemperature < meltingPoint ? Phase.Solid :
			          (initialTemperature < boilingPoint ? Phase.Liquid : Phase.Gas);

		          // Compute sensible energy using the cp at the initial temperature
		          float cpInit = SpecificHeat;
		          sensibleEnergy = mass * cpInit * initialTemperature;
		          latentEnergy = 0f;
		          temperature = initialTemperature;

		          ClampTemperatureToPhase();
	     }

	     // --- Public API ---

	     /// <summary>Add heat energy (sensible). Handles phase changes iteratively.</summary>
	     public void AddHeat(float joules)
	     {
		          if (joules <= 0f || Mass <= 0f)
				return;

		          float cp = SpecificHeat;                // uses cached value
		          float dT = joules / (Mass * cp);
		          Temperature += dT;
		          sensibleEnergy += joules;
		          //ProcessPhaseChanges();
		          ClampTemperatureToPhase();
	     }

	     /// <summary>Remove heat energy (sensible first, then latent). Handles phase changes.</summary>
	     public void RemoveHeat(float joules)
	     {
		          if (joules <= 0f || Mass <= 0f)
				return;

		          float toRemove = Mathf.Min(joules, StoredEnergy);
		          float removed = 0f;

		          // Remove from sensible first
		          float sensibleToRemove = Mathf.Min(toRemove, sensibleEnergy);
		          sensibleEnergy -= sensibleToRemove;
		          removed += sensibleToRemove;

		          // If still need more, remove from latent
		          toRemove -= sensibleToRemove;
		          if (toRemove > 0f && latentEnergy > 0f)
		          {
				float latentToRemove = Mathf.Min(toRemove, latentEnergy);
				latentEnergy -= latentToRemove;
				removed += latentToRemove;
		          }

		          // Recompute temperature based on remaining sensible energy
		          if (Mass > 0f)
		          {
				float cp = SpecificHeat;

				temperature = sensibleEnergy / (Mass * cp);
		          }
		          else
		          {
				temperature = 0f;
		          }

		          //ProcessPhaseChanges(); // handles freezing/condensation
		          ClampTemperatureToPhase();
	     }

	     /// <summary>Add mass at a given temperature. Energy is mixed.</summary>
	     public void AddMass(float kg, float temperature, float? specificHeatOverride = null)
	     {
		          if (kg <= 0f)
				return;

		          // Create temporary object representing the added mass
		          float sh = specificHeatOverride ?? SpecificHeat;
		          var tempMass = new ThermalEnergy(kg, materialName,
				             LatentHeatFusion, LatentHeatVaporization,
				             MeltingPoint, BoilingPoint,
				             ThermalConductivity, temperature, defaultCp);

		          // Transfer all mass and energy from tempMass to this
		          tempMass.TransferMassTo(this, kg);
	     }

	     /// <summary>Remove mass and proportional energy (preserves temperature).</summary>
	     public void RemoveMass(float kg)
	     {
		          if (kg <= 0f)
				return;
		          if (kg >= Mass)
		          {
				// Remove everything
				sensibleEnergy = 0f;
				latentEnergy = 0f;
				Mass = 0f;
				Temperature = 0f;
				CurrentPhase = Phase.Solid;
				return;
		          }

		          float ratio = kg / Mass;
		          sensibleEnergy *= (1f - ratio);
		          latentEnergy *= (1f - ratio);
		          Mass -= kg;
		          // Temperature remains the same (scaled)
	     }

	     /// <summary>Transfer mass and proportional energy to another ThermalEnergy.</summary>
	     public void TransferMassTo(ThermalEnergy target, float kg)
	     {
		          if (target == null || kg <= 0f)
				return;
		          if (kg > Mass)
				kg = Mass;

		          float ratio = kg / Mass;

		          // Extract energy proportional to the mass being moved
		          float sensibleToMove = sensibleEnergy * ratio;
		          float latentToMove = latentEnergy * ratio;

		          // Remove from source
		          sensibleEnergy -= sensibleToMove;
		          latentEnergy -= latentToMove;
		          Mass -= kg;

		          // Add to target
		          target.sensibleEnergy += sensibleToMove;
		          target.latentEnergy += latentToMove;
		          target.Mass += kg;

		          // Process phase changes on both objects
		          //ProcessPhaseChanges();
		          //target.ProcessPhaseChanges();

		          ClampTemperatureToPhase();
		          target.ClampTemperatureToPhase();
	     }

	     /// <summary>Mix with another ThermalEnergy to reach thermal equilibrium (no mass transfer).</summary>
	     public void MixWith(ThermalEnergy other)
	     {
		          if (other == null || other.Mass <= 0f || Mass <= 0f)
				return;

		          // Compute total energy and mass
		          float totalEnergy = StoredEnergy + other.StoredEnergy;
		          float totalMass = Mass + other.Mass;

		          // Approximate equilibrium temperature using cp at current temperatures (simple)
		          float cpThis = SpecificHeat;
		          float cpOther = other.SpecificHeat;

		          float avgCp = (Mass * cpThis + other.Mass * cpOther) / totalMass;
		          float equilibriumTemp = totalEnergy / (totalMass * avgCp);

		          // Set both objects to that temperature
		          sensibleEnergy = Mass * cpThis * equilibriumTemp;  // will be refined later
		          other.sensibleEnergy = other.Mass * cpOther * equilibriumTemp;
		          // Keep latent energies unchanged

		          //ProcessPhaseChanges();
		          //other.ProcessPhaseChanges();
		          ClampTemperatureToPhase();
		          other.ClampTemperatureToPhase();
	     }

	     // --- Overrides from EnergyData ---
	     public override void AddEnergy(float joules) => AddHeat(joules);
	     public override float RemoveEnergy(float maxJoules)
	     {
		          float removed = Mathf.Min(maxJoules, StoredEnergy);
		          RemoveHeat(removed);
		          return removed;
	     }

	     // --- Internal Helpers ---

	     //  /// <summary>Get specific heat at a given temperature from the library or fallback.</summary>
	     //  private float GetSpecificHeat(float temperature)
	     //  {
	     //        float cp = MatterLibrary.GetSpecificHeat(materialName, temperature);
	     //        if (cp <= 0f) // not found
	     //return defaultCp;
	     //        return cp;
	     //  }

	     /// <summary>Iteratively process phase changes until stable.</summary>


	     private void ClampTemperatureToPhase()
	     {
		          if (Mass <= 0f)
		          {
				Temperature = 0f;
				return;
		          }

		          switch (CurrentPhase)
		          {
		          case Phase.Solid:
		          Temperature = Mathf.Min(Temperature, MeltingPoint);
		          break;
		          case Phase.Liquid:
		          Temperature = Mathf.Clamp(Temperature, MeltingPoint, BoilingPoint);
		          break;
		          case Phase.Gas:
		          Temperature = Mathf.Max(Temperature, BoilingPoint);
		          break;
		          }
	     }

	     // Private mass change (used only internally)
	     private void ChangeMass(float newMass)
	     {
		          if (newMass <= 0f)
		          {
				Mass = 0f;
				sensibleEnergy = 0f;
				latentEnergy = 0f;
				Temperature = 0f;
				CurrentPhase = Phase.Solid;
				return;
		          }

		          float ratio = newMass / Mass;
		          Mass = newMass;
		          sensibleEnergy *= ratio;
		          latentEnergy *= ratio;
		          // Temperature remains same because energy scaled proportionally
		          // but we may need to clamp
		          ClampTemperatureToPhase();
	     }
}

public enum Phase
{
	     Solid,
	     Liquid,
	     Gas
}