// this class will hold energy data
using System.Collections.Generic;
using UnityEngine;

// Base energy class
public abstract class EnergyData
{
	     public float StoredEnergy
	     {
		          get; protected set;
	     } // Joules
	     public EnergyType Type
	     {
		          get; protected set;
	     }

	     public virtual void AddEnergy(float joules)
	     {
		          StoredEnergy += joules;
	     }

	     public virtual float ExtractEnergy(float maxJoules)
	     {
		          float available = Mathf.Min(maxJoules, StoredEnergy);
		          StoredEnergy -= available;
		          return available;
	     }

	     public bool CanTransferEnergy(float joules)
	     {
		          return StoredEnergy >= joules;
	     }
}

public enum EnergyType
{
	     Thermal,
	     Chemical,
	     Radiative,
	     Mechanical,
	     Latent
}

public class ThermalEnergy : EnergyData
{
	     public float Temperature
	     {
		          get; private set;
	     } // Kelvin
	     public float Mass
	     {
		          get; private set;
	     } // kg
	     public float SpecificHeat
	     {
		          get; private set;
	     } // J/kg·K

	     public ThermalEnergy(float mass, float specificHeat, float initialTemperature = 293f) // 20°C default
	     {
		          Type = EnergyType.Thermal;
		          Mass = mass;
		          SpecificHeat = specificHeat;
		          Temperature = initialTemperature;
		          StoredEnergy = CalculateEnergyFromTemperature();
	     }

	     private float CalculateEnergyFromTemperature()
	     {
		          // E = m * c * T (relative to absolute zero)
		          return Mass * SpecificHeat * Temperature;
	     }

	     private void UpdateTemperatureFromEnergy()
	     {
		          if (Mass > 0 && SpecificHeat > 0)
				Temperature = StoredEnergy / (Mass * SpecificHeat);
	     }

	     public override void AddEnergy(float joules)
	     {
		          base.AddEnergy(joules);
		          UpdateTemperatureFromEnergy();
	     }

	     public override float ExtractEnergy(float maxJoules)
	     {
		          float extracted = base.ExtractEnergy(maxJoules);
		          UpdateTemperatureFromEnergy();
		          return extracted;
	     }

	     public void ChangeMass(float newMass)
	     {
		          // When mass changes, energy density changes but total energy stays constant
		          Mass = newMass;
		          UpdateTemperatureFromEnergy();
	     }
}

public class ChemicalEnergy : EnergyData
{
	     public Dictionary<string, float> ChemicalPotentials
	     {
		          get; private set;
	     } // J/mol per compound

	     public ChemicalEnergy(float stored)
	     {
		          Type = EnergyType.Chemical;
		          ChemicalPotentials = new Dictionary<string, float>();
		          StoredEnergy = stored;
	     }

	     public void AddChemicalPotential(string compound, float energyPerMole)
	     {
		          ChemicalPotentials[compound] = energyPerMole;
	     }

	     public float CalculateEnergyFromReaction(string reactant, string product, float moles)
	     {
		          if (ChemicalPotentials.ContainsKey(reactant) && ChemicalPotentials.ContainsKey(product))
		          {
				float deltaG = ChemicalPotentials[product] - ChemicalPotentials[reactant];
				return deltaG * moles; // Negative = exothermic, Positive = endothermic
		          }
		          return 0f;
	     }
}

public class RadiativeEnergy : EnergyData
{
	     public float Wavelength
	     {
		          get; private set;
	     } // nanometers
	     public float Intensity
	     {
		          get; private set;
	     } // W/m²

	     public RadiativeEnergy(float wavelength, float initialIntensity = 0f)
	     {
		          Type = EnergyType.Radiative;
		          Wavelength = wavelength;
		          Intensity = initialIntensity;
		          StoredEnergy = Intensity; // Simplified - in real physics this would be energy density
	     }

	     public float CalculateAbsorption(float absorptionCoefficient, float pathLength)
	     {
		          // Beer-Lambert law: I = I₀ * e^(-α * l)
		          float absorbedIntensity = Intensity * (1 - Mathf.Exp(-absorptionCoefficient * pathLength));
		          return absorbedIntensity;
	     }
}

public class MechanicalEnergy : EnergyData
{
	     public float KineticEnergy
	     {
		          get; private set;
	     } // J
	     public float PotentialEnergy
	     {
		          get; private set;
	     } // J

	     public MechanicalEnergy()
	     {
		          Type = EnergyType.Mechanical;
		          StoredEnergy = 0f;
	     }

	     public void UpdateKineticEnergy(float mass, float velocity)
	     {
		          KineticEnergy = 0.5f * mass * velocity * velocity;
		          UpdateTotalEnergy();
	     }

	     public void UpdatePotentialEnergy(float mass, float height, float gravity = 9.81f)
	     {
		          PotentialEnergy = mass * gravity * height;
		          UpdateTotalEnergy();
	     }

	     private void UpdateTotalEnergy()
	     {
		          StoredEnergy = KineticEnergy + PotentialEnergy;
	     }
}

public class LatentEnergy : EnergyData
{
	     public Phase Phase
	     {
		          get; private set;
	     }
	     public float LatentHeatFusion
	     {
		          get; private set;
	     } // J/kg
	     public float LatentHeatVaporization
	     {
		          get; private set;
	     } // J/kg

	     public LatentEnergy(float latentHeatFusion, float latentHeatVaporization)
	     {
		          Type = EnergyType.Latent;
		          LatentHeatFusion = latentHeatFusion;
		          LatentHeatVaporization = latentHeatVaporization;
		          Phase = Phase.Solid;
	     }

	     public float GetPhaseChangeEnergy(Phase newPhase, float mass)
	     {
		          if (Phase == newPhase)
				return 0f;

		          return (Phase, newPhase) switch
		          {
				(Phase.Solid, Phase.Liquid) => mass * LatentHeatFusion,
				(Phase.Liquid, Phase.Solid) => -mass * LatentHeatFusion,
				(Phase.Liquid, Phase.Gas) => mass * LatentHeatVaporization,
				(Phase.Gas, Phase.Liquid) => -mass * LatentHeatVaporization,
				(Phase.Solid, Phase.Gas) => mass * (LatentHeatFusion + LatentHeatVaporization),
				(Phase.Gas, Phase.Solid) => -mass * (LatentHeatFusion + LatentHeatVaporization),
				_ => 0f
		          };
	     }

	     public bool TryPhaseChange(Phase newPhase, float mass, ref float availableEnergy)
	     {
		          float requiredEnergy = GetPhaseChangeEnergy(newPhase, mass);

		          if (Mathf.Abs(requiredEnergy) <= availableEnergy)
		          {
				Phase = newPhase;
				availableEnergy -= requiredEnergy;
				return true;
		          }
		          return false;
	     }
}

public enum Phase
{
	     Solid,
	     Liquid,
	     Gas
}
