// this class will hold energy data
using System;
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
	     public float SpecificHeat
	     {
		          get; private set;
	     } // J/kg·K
	     public Phase CurrentPhase
	     {
		          get; private set;
	     }
	     public float Mass
	     {
		          get; private set;
	     }
	     // Latent heat properties
	     public float LatentHeatFusion
	     {
		          get; private set;
	     } // J/kg
	     public float LatentHeatVaporization
	     {
		          get; private set;
	     } // J/kg
	     public float PhaseTransitionTemperature
	     {
		          get; private set;
	     } // Current phase transition temp
	     public float MeltingPoint
	     {
		          get; private set;
	     } // J/kg
	     public float BoilingPoint
	     {
		          get; private set;
	     } // J/kg
	     public float ThermalConductivity
	     {
		          get; private set;
	     } // Current phase transition temp
	       //Density
	     private float sensibleEnergy; // Energy that affects temperature
	     private float latentEnergy; // Energy stored in phase bonds


	     public ThermalEnergy(float mass, float specificHeat,
		          float latentHeatFusion = 334000f, float latentHeatVaporization = 2260000f, float meltingPoint = 273f, float boilingPoint = 373f, float thermalConductivity = 0.6089f,
		          float initialTemperature = 278f)
	     {
		          Type = EnergyType.Thermal;
		          Mass = mass;
		          SpecificHeat = specificHeat;
		          LatentHeatFusion = latentHeatFusion;
		          LatentHeatVaporization = latentHeatVaporization;
		          CurrentPhase = initialTemperature < meltingPoint ? Phase.Solid :
			          (initialTemperature < boilingPoint ? Phase.Liquid : Phase.Gas);
		          Temperature = initialTemperature;
		          ThermalConductivity = thermalConductivity;
		          // Calculate initial energies
		          sensibleEnergy = mass * specificHeat * Temperature;
		          UpdatePhaseTransitionTemperature();
		          StoredEnergy = sensibleEnergy + latentEnergy;
	     }

	     private void UpdatePhaseTransitionTemperature()
	     {
		          // Set transition temp based on current phase but this is just wrong.these are dependent on the material.

		          if (CurrentPhase == Phase.Solid)
				PhaseTransitionTemperature = MeltingPoint; // 0°C for water
		          else if (CurrentPhase == Phase.Liquid)
				PhaseTransitionTemperature = BoilingPoint; // 100°C for water
		          else
				PhaseTransitionTemperature = float.MaxValue; // Gas doesn't transition up
	     }

	     public override void AddEnergy(float joules)
	     {
		          sensibleEnergy += joules;
		          UpdateTemperature();

		          // Only check phase change ONCE, don't let it recursively trigger
		          CheckPhaseChangeOnHeating();

		          UpdateTotalEnergy();
	     }

	     public override float ExtractEnergy(float maxJoules)
	     {
		          float toExtract = Mathf.Min(maxJoules, StoredEnergy);
		          float extracted = 0;

		          // Extract from sensible energy first
		          float sensibleToExtract = Mathf.Min(toExtract, sensibleEnergy);
		          sensibleEnergy -= sensibleToExtract;
		          extracted += sensibleToExtract;

		          UpdateTemperature();
		          // If we still need more energy, extract from latent
		          toExtract -= sensibleToExtract;
		          if (toExtract > 0)
		          {
				float latentToExtract = Mathf.Min(toExtract, latentEnergy);
				latentEnergy -= latentToExtract;
				extracted += latentToExtract;
		          }
		          // Only check phase change ONCE
		          CheckPhaseChangeOnCooling();
		          StoredEnergy -= extracted;
		          return extracted;
	     }

	     private void CheckPhaseChangeOnHeating()
	     {
		          // Melting: Solid -> Liquid (when temp rises above melting point)
		          if (CurrentPhase == Phase.Solid && Temperature >= MeltingPoint)
		          {
				// Melting absorbs latent heat (endothermic)
				float meltingEnergy = Mass * LatentHeatFusion;

				// Only proceed if we have enough sensible energy
				if (sensibleEnergy >= meltingEnergy)
				{
					     sensibleEnergy -= meltingEnergy;
					     latentEnergy += meltingEnergy;
					     CurrentPhase = Phase.Liquid;
					     UpdatePhaseTransitionTemperature();
					     // DON'T call UpdateTemperature() here - temperature stays at MeltingPoint
					     // The temperature is HELD at the transition point during phase change
				}
				else
				{

					     Temperature = MeltingPoint;
				}
		          }
		          // Vaporization: Liquid -> Gas (when temp rises above boiling point)
		          else if (CurrentPhase == Phase.Liquid && Temperature >= BoilingPoint)
		          {
				// Vaporization absorbs latent heat (endothermic)
				float vaporizationEnergy = Mass * LatentHeatVaporization;

				if (sensibleEnergy >= vaporizationEnergy)
				{
					     sensibleEnergy -= vaporizationEnergy;
					     latentEnergy += vaporizationEnergy;
					     CurrentPhase = Phase.Gas;
					     UpdatePhaseTransitionTemperature();
					     // Temperature stays at BoilingPoint during vaporization
				}
				else
				{
					     // Not enough energy to complete vaporization - stay at boiling point

					     Temperature = BoilingPoint;
				}
		          }
	     }

	     private void CheckPhaseChangeOnCooling()
	     {
		          // Freezing: Liquid -> Solid (when temp drops below melting point)
		          if (CurrentPhase == Phase.Liquid && Temperature < MeltingPoint)
		          {
				// Freezing releases latent heat (exothermic)
				float freezingEnergy = Mass * LatentHeatFusion;

				latentEnergy -= freezingEnergy;
				sensibleEnergy += freezingEnergy;
				CurrentPhase = Phase.Solid;
				UpdatePhaseTransitionTemperature();
				// Temperature rises back to MeltingPoint due to latent heat release
				UpdateTemperature();
		          }
		          // Condensation: Gas -> Liquid (when temp drops below boiling point)
		          else if (CurrentPhase == Phase.Gas && Temperature < BoilingPoint)
		          {
				// Condensation releases latent heat (exothermic)
				float condensationEnergy = Mass * LatentHeatVaporization;

				latentEnergy -= condensationEnergy;
				sensibleEnergy += condensationEnergy;
				CurrentPhase = Phase.Liquid;
				UpdatePhaseTransitionTemperature();
				UpdateTemperature();
		          }
	     }

	     private void UpdateTemperature()
	     {
		          if (Mass > 0 && SpecificHeat > 0)
		          {
				// Clamp temperature at phase transition points
				float newTemp = sensibleEnergy / (Mass * SpecificHeat);

				if (CurrentPhase == Phase.Solid)
					     Temperature = Mathf.Min(newTemp, MeltingPoint);
				else if (CurrentPhase == Phase.Liquid)
					     Temperature = Mathf.Clamp(newTemp, MeltingPoint, BoilingPoint);
				else // Gas
					     Temperature = Mathf.Max(newTemp, BoilingPoint);
		          }
	     }

	     private void UpdateTotalEnergy()
	     {
		          StoredEnergy = sensibleEnergy + latentEnergy;
	     }

	     public void ChangeMass(float newMass)
	     {
		          float ratio = newMass / Mass;
		          Mass = newMass;
		          sensibleEnergy *= ratio;
		          latentEnergy *= ratio;
		          UpdateTemperature();
		          UpdateTotalEnergy();
	     }

	     public void TransferMassTo(ThermalEnergy target, float kgToTransfer, List<ThermalEnergy> energyRelease = null)
	     {
		          if (target == null || kgToTransfer <= 0)
				return;

		          if (kgToTransfer > Mass)
				kgToTransfer = Mass;

		          float transferRatio = kgToTransfer / Mass;

		          // Calculate proportional energy to move
		          float energyToTransfer = (sensibleEnergy + latentEnergy) * transferRatio;

		          // Remove mass and energy from source
		          ChangeMass(Mass - kgToTransfer);
		          float extracted = ExtractEnergy(energyToTransfer);

		          // Try to add to target
		          float beforeEnergy = target.StoredEnergy;
		          target.ChangeMass(target.Mass + kgToTransfer);
		          target.AddEnergy(extracted);

		          // If target couldn't absorb all (phase clamp), push excess to overflow
		          float absorbed = target.StoredEnergy - beforeEnergy;
		          float excess = extracted - absorbed;
		          if (excess > 0)
		          {
				if (energyRelease != null)
				{
					     foreach (var e in energyRelease)
						          e.AddEnergy(excess / energyRelease.Count);
				}
				else
				{
					     AddEnergy(excess);
				}

		          }
		          if (excess < 0)
		          {
				if (energyRelease != null)
				{
					     foreach (var e in energyRelease)
						          e.ExtractEnergy(excess / energyRelease.Count);
				}
				else
				{
					     target.ExtractEnergy(excess);
				}

		          }

		          // Final consistency updates
		          UpdateTemperature();
		          UpdateTotalEnergy();
		          target.UpdateTemperature();
		          target.UpdateTotalEnergy();
	     }



	     internal int Count()
	     {
		          throw new NotImplementedException();
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
