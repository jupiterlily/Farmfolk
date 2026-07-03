// this class will hold energy data
using System.Collections.Generic;
using UnityEngine;

// Base energy class
public abstract class EnergyData
{
	     public virtual float StoredEnergy
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

	     public virtual float RemoveEnergy(float maxJoules)
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

