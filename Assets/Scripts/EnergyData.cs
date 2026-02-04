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
		          float remainingEnergy = joules;

		          // Handle phase changes first
		          remainingEnergy = ProcessPhaseChange(remainingEnergy);

		          // Add remaining as sensible heat
		          if (remainingEnergy > 0)
		          {
				sensibleEnergy += remainingEnergy;
				UpdateTemperature();
		          }

		          UpdateTotalEnergy();
	     }

	     public override float ExtractEnergy(float maxJoules)
	     {
		          float toExtract = Mathf.Min(maxJoules, StoredEnergy);
		          float extracted = 0;

		          // First extract from latent energy if at phase transition
		          if (IsAtPhaseTransition())
		          {
				float latentToExtract = Mathf.Min(toExtract, latentEnergy);
				latentEnergy -= latentToExtract;
				extracted += latentToExtract;
				toExtract -= latentToExtract;
		          }

		          // Then extract from sensible energy
		          if (toExtract > 0)
		          {
				float sensibleToExtract = Mathf.Min(toExtract, sensibleEnergy);
				sensibleEnergy -= sensibleToExtract;
				extracted += sensibleToExtract;
				toExtract -= sensibleToExtract;

				// Update temperature after sensible energy change
				UpdateTemperature();

				// Check for reverse phase change due to cooling
				if (toExtract > 0 && IsAtPhaseTransition())
				{
					     // Can potentially extract more by reversing phase change
					     float reversePhaseEnergy = GetReversePhaseChangeEnergy(toExtract);
					     if (reversePhaseEnergy > 0)
					     {
						          latentEnergy -= reversePhaseEnergy;
						          extracted += reversePhaseEnergy;
						          toExtract -= reversePhaseEnergy;
						          CheckPhaseChangeOnCooling();
					     }
				}
		          }

		          StoredEnergy -= extracted;
		          return extracted;
	     }

	     private float ProcessPhaseChange(float energyToAdd)
	     {
		          float remainingEnergy = energyToAdd;

		          // Check if we're at a phase transition temperature
		          if (IsAtPhaseTransition())
		          {
				float phaseChangeEnergyNeeded = GetPhaseChangeEnergyToNextPhase();

				if (phaseChangeEnergyNeeded > 0 && remainingEnergy >= phaseChangeEnergyNeeded)
				{
					     // Energy goes into latent heat, not temperature
					     latentEnergy += phaseChangeEnergyNeeded;
					     remainingEnergy -= phaseChangeEnergyNeeded;

					     // Change phase
					     CurrentPhase = GetNextPhase();
					     UpdatePhaseTransitionTemperature();
				}
		          }

		          return remainingEnergy;
	     }

	     private bool IsAtPhaseTransition()
	     {
		          float tolerance = 0.1f;

		          if (CurrentPhase == Phase.Solid)
				return Mathf.Abs(Temperature - 273f) < tolerance;
		          else if (CurrentPhase == Phase.Liquid)
				return Mathf.Abs(Temperature - 373f) < tolerance;

		          return false;
	     }

	     private float GetPhaseChangeEnergyToNextPhase()
	     {
		          return CurrentPhase switch
		          {
				Phase.Solid => Mass * LatentHeatFusion,
				Phase.Liquid => Mass * LatentHeatVaporization,
				_ => 0f
		          };
	     }

	     private Phase GetNextPhase()
	     {
		          return CurrentPhase switch
		          {
				Phase.Solid => Phase.Liquid,
				Phase.Liquid => Phase.Gas,
				_ => CurrentPhase
		          };
	     }

	     private float GetReversePhaseChangeEnergy(float availableEnergy)
	     {
		          if (CurrentPhase == Phase.Liquid && Temperature <= 273f)
				return Mathf.Min(availableEnergy, Mass * LatentHeatFusion);
		          else if (CurrentPhase == Phase.Gas && Temperature <= 373f)
				return Mathf.Min(availableEnergy, Mass * LatentHeatVaporization);

		          return 0f;
	     }

	     private void CheckPhaseChangeOnCooling()
	     {
		          if (CurrentPhase == Phase.Liquid && Temperature < 273f && latentEnergy >= Mass * LatentHeatFusion)
		          {
				// Freezing
				CurrentPhase = Phase.Solid;
				latentEnergy -= Mass * LatentHeatFusion;
				UpdatePhaseTransitionTemperature();
		          }
		          else if (CurrentPhase == Phase.Gas && Temperature < 373f && latentEnergy >= Mass * LatentHeatVaporization)
		          {
				// Condensation
				CurrentPhase = Phase.Liquid;
				latentEnergy -= Mass * LatentHeatVaporization;
				UpdatePhaseTransitionTemperature();
		          }
	     }

	     private void UpdateTemperature()
	     {
		          if (Mass > 0 && SpecificHeat > 0)
				Temperature = sensibleEnergy / (Mass * SpecificHeat);
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

	     public void TransferMassTo(ThermalEnergy target, float kgToTransfer)
	     {
		          if (target == null || kgToTransfer <= 0)
				return;

		          // Calculate ratio of mass to transfer
		          float transferRatio = kgToTransfer / Mass;

		          // Transfer sensible energy proportionally
		          float sensibleToTransfer = sensibleEnergy * transferRatio;
		          sensibleEnergy -= sensibleToTransfer;

		          // Transfer latent energy proportionally
		          float latentToTransfer = latentEnergy * transferRatio;
		          latentEnergy -= latentToTransfer;

		          // Add to target
		          target.sensibleEnergy += sensibleToTransfer;
		          target.latentEnergy += latentToTransfer;

		          // Update masses
		          ChangeMass(Mass - kgToTransfer);
		          target.ChangeMass(target.Mass + kgToTransfer);

		          // Update temperatures
		          UpdateTemperature();
		          target.UpdateTemperature();
		          UpdateTotalEnergy();
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
