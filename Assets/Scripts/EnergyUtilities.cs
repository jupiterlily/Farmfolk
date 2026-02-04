using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnergyTransferUtilities
{
	     // ========== CORE ENERGY TRANSFER ==========

	     /// <summary>Transfer energy between any two EnergyData objects</summary>
	     public static float TransferEnergy(EnergyData source, EnergyData sink, float joules)
	     {
		          if (source == null || sink == null || joules <= 0)
				return 0;

		          float available = Mathf.Min(joules, source.StoredEnergy);
		          float extracted = source.ExtractEnergy(available);
		          sink.AddEnergy(extracted);

		          return extracted;
	     }

	     /// <summary>Transfer energy with efficiency factor (0-1)</summary>
	     public static float TransferEnergyWithLoss(EnergyData source, EnergyData sink,
				  float joules, float efficiency)
	     {
		          if (efficiency <= 0)
				return 0;

		          float extracted = source.ExtractEnergy(joules);
		          float delivered = extracted * Mathf.Clamp01(efficiency);
		          sink.AddEnergy(delivered);

		          return delivered;
	     }
}

public static class ThermalEnergyUtilities
{
	     // ========== THERMAL TRANSFER MECHANISMS ==========

	     /// <summary>Heat conduction between two thermal masses</summary>
	     public static float ConductHeat(ThermalEnergy ThermA, ThermalEnergy ThermB, float area,
			      float distance, float timeStep)
	     {

		          float tempDiff;

		          if (ThermB.Temperature < ThermA.Temperature)
		          {
				tempDiff = ThermA.Temperature - ThermB.Temperature;
				if (tempDiff <= 0)
					     return 0;
				// Fourier's Law: Q = k * A * (ΔT) / d * Δt
				float heatFlow = ThermA.ThermalConductivity * area * tempDiff / distance * timeStep;
				return EnergyTransferUtilities.TransferEnergy(ThermA, ThermB, heatFlow);
		          }
		          else
		          {
				tempDiff = ThermB.Temperature - ThermA.Temperature;
				if (tempDiff <= 0)
					     return 0;
				// Fourier's Law: Q = k * A * (ΔT) / d * Δt
				float heatFlow = ThermB.ThermalConductivity * area * tempDiff / distance * timeStep;
				return EnergyTransferUtilities.TransferEnergy(ThermB, ThermA, heatFlow);
		          }
	     }

	     /// <summary>Transfer mass and associated thermal energy</summary>
	     public static void TransferMassAndEnergy(ThermalEnergy source, ThermalEnergy sink,
			              float massToTransfer)
	     {
		          if (source == null || sink == null || massToTransfer <= 0)
				return;

		          float actualTransfer = Mathf.Min(massToTransfer, source.Mass);
		          if (actualTransfer <= 0)
				return;

		          // Calculate energy proportional to mass being transferred
		          float energyPerKg = source.StoredEnergy / source.Mass;
		          float energyToTransfer = energyPerKg * actualTransfer;

		          // Transfer energy
		          //    source.ExtractEnergy(energyToTransfer);
		          //    sink.AddEnergy(energyToTransfer);

		          // Update masses
		          //   source.ChangeMass(source.Mass - actualTransfer);
		          //   sink.ChangeMass(sink.Mass + actualTransfer);

		          source.TransferMassTo(sink, actualTransfer);
	     }

	     /// <summary>Convective heat transfer to/from a surface</summary>


	     /// <summary>Calculate thermal equilibrium temperature when masses mix</summary>
	     public static float CalculateEquilibriumTemperature(ThermalEnergy a, ThermalEnergy b)
	     {
		          float totalEnergy = a.StoredEnergy + b.StoredEnergy;
		          float totalMass = a.Mass + b.Mass;
		          float avgSpecificHeat = (a.Mass * a.SpecificHeat + b.Mass * b.SpecificHeat) / totalMass;

		          return totalEnergy / (totalMass * avgSpecificHeat);
	     }
	     /// <summary>
	     /// Mixes temperatures of all ThermalEnergy objects in the same phase within a list
	     /// This conserves total energy while equalizing temperatures within each phase group
	     /// </summary>
	     /// <param name="thermalEnergies">List of ThermalEnergy objects to mix</param>
	     public static void MixTemperaturesByPhase(List<ThermalEnergy> thermalEnergies)
	     {
		          if (thermalEnergies == null || thermalEnergies.Count <= 1)
				return;

		          // Group by phase
		          var phaseGroups = thermalEnergies.GroupBy(te => te.CurrentPhase)
		              .Where(g => g.Count() > 1); // Only mix if there are multiple items in phase

		          foreach (var group in phaseGroups)
		          {
				MixTemperaturesForPhaseGroup(group.ToList());
		          }
	     }

	     /// <summary>
	     /// Mixes temperatures for a specific phase group
	     /// </summary>
	     private static void MixTemperaturesForPhaseGroup(List<ThermalEnergy> phaseGroup)
	     {
		          if (phaseGroup.Count <= 1)
				return;

		          if (phaseGroup.Count <= 1)
				return;

		          // Calculate total energy and equilibrium temperature
		          float totalSensibleEnergy = CalculateTotalSensibleEnergy(phaseGroup);
		          float totalHeatCapacity = CalculateTotalHeatCapacity(phaseGroup);

		          if (totalHeatCapacity <= 0f)
		          {
				Debug.LogWarning("Total heat capacity is zero or negative, cannot mix temperatures.");
				return;
		          }

		          float equilibriumTemperature = totalSensibleEnergy / totalHeatCapacity;

		          // Use energy transfer to reach equilibrium
		          TransferEnergyToReachEquilibrium(phaseGroup, equilibriumTemperature);
	     }

	     /// <summary>
	     /// Calculates total sensible energy for a group of ThermalEnergy objects
	     /// </summary>
	     private static float CalculateTotalSensibleEnergy(List<ThermalEnergy> thermalEnergies)
	     {
		          float total = 0f;
		          foreach (var thermal in thermalEnergies)
		          {
				// Sensible energy = mass * specific heat * temperature
				total += thermal.Mass * thermal.SpecificHeat * thermal.Temperature;
		          }
		          return total;
	     }

	     /// <summary>
	     /// Calculates total heat capacity for a group of ThermalEnergy objects
	     /// </summary>
	     private static float CalculateTotalHeatCapacity(List<ThermalEnergy> thermalEnergies)
	     {
		          float total = 0f;
		          foreach (var thermal in thermalEnergies)
		          {
				total += thermal.Mass * thermal.SpecificHeat;
		          }
		          return total;
	     }

	     /// <summary>
	     /// Uses energy transfer between objects to reach equilibrium temperature
	     /// </summary>
	     private static void TransferEnergyToReachEquilibrium(List<ThermalEnergy> thermalEnergies, float targetTemperature)
	     {
		          // Find which objects need to gain or lose energy
		          List<ThermalEnergy> sources = new List<ThermalEnergy>();
		          List<ThermalEnergy> sinks = new List<ThermalEnergy>();
		          List<float> energyNeeded = new List<float>();
		          List<float> energyAvailable = new List<float>();

		          // Calculate energy deficits and surpluses
		          foreach (var thermal in thermalEnergies)
		          {
				float currentEnergy = thermal.Mass * thermal.SpecificHeat * thermal.Temperature;
				float targetEnergy = thermal.Mass * thermal.SpecificHeat * targetTemperature;
				float energyDifference = targetEnergy - currentEnergy;

				if (energyDifference > 0)
				{
					     // Needs energy
					     sinks.Add(thermal);
					     energyNeeded.Add(energyDifference);
				}
				else if (energyDifference < 0)
				{
					     // Has excess energy
					     sources.Add(thermal);
					     energyAvailable.Add(-energyDifference);
				}
		          }

		          // Sort by magnitude (largest transfers first)
		          var sortedSinks = sinks.Select((sink, index) => new { Sink = sink, Needed = energyNeeded[index] })
				  .OrderByDescending(x => x.Needed).ToList();
		          var sortedSources = sources.Select((source, index) => new { Source = source, Available = energyAvailable[index] })
				      .OrderByDescending(x => x.Available).ToList();

		          // Transfer energy from sources to sinks
		          int sourceIndex = 0;
		          int sinkIndex = 0;

		          while (sourceIndex < sortedSources.Count && sinkIndex < sortedSinks.Count)
		          {
				var source = sortedSources[sourceIndex];
				var sink = sortedSinks[sinkIndex];

				// Transfer the smaller of available energy or needed energy
				float transferAmount = Mathf.Min(source.Available, sink.Needed);

				if (transferAmount > 0)
				{
					     // Use the existing energy transfer utility
					     float actualTransfer = EnergyTransferUtilities.TransferEnergy(source.Source, sink.Sink, transferAmount);

					     // Update remaining amounts
					     sortedSources[sourceIndex] = new
					     {
						          source.Source,
						          Available = source.Available - actualTransfer
					     };
					     sortedSinks[sinkIndex] = new
					     {
						          sink.Sink,
						          Needed = sink.Needed - actualTransfer
					     };

					     // Check if source is depleted
					     if (sortedSources[sourceIndex].Available <= Mathf.Epsilon)
					     {
						          sourceIndex++;
					     }

					     // Check if sink is satisfied
					     if (sortedSinks[sinkIndex].Needed <= Mathf.Epsilon)
					     {
						          sinkIndex++;
					     }
				}
				else
				{
					     break;
				}
		          }
	     }

	     /// <summary>
	     /// Mixes temperatures using pairwise conduction between all objects in a phase group
	     /// More realistic but slower - good for small groups
	     /// </summary>
	     public static void MixTemperaturesByConduction(List<ThermalEnergy> thermalEnergies,
				       float timeStep,
				       float widthByHeight = 1f)
	     {
		          if (thermalEnergies == null || thermalEnergies.Count <= 1)
				return;

		          // Group by phase
		          var phaseGroups = thermalEnergies
		              .GroupBy(te => te.CurrentPhase)
		              .Where(g => g.Count() > 1);

		          foreach (var group in phaseGroups)
		          {
				var groupList = group.ToList();

				// Perform pairwise conduction between all objects in the group
				for (int i = 0; i < groupList.Count; i++)
				{
					     for (int j = i + 1; j < groupList.Count; j++)
					     {
						          ThermalEnergy hot, cold;

						          if (groupList[i].Temperature >= groupList[j].Temperature)
						          {
								hot = groupList[i];
								cold = groupList[j];
						          }
						          else
						          {
								hot = groupList[j];
								cold = groupList[i];
						          }

						          // Use Fourier's law for conduction
						          float tempDiff = hot.Temperature - cold.Temperature;
						          if (tempDiff > 0)
						          {
								// Simplified: assume unit area and distance with mixing factor
								float heatFlow = hot.ThermalConductivity * widthByHeight * tempDiff * timeStep;
								EnergyTransferUtilities.TransferEnergy(hot, cold, heatFlow);
						          }
					     }
				}
		          }
	     }
	     /// <summary>
	     /// Mixes all ThermalEnergy objects in an AtmosphereTile using energy transfer
	     /// </summary>
	     public static void MixAtmosphereTileTemperatures(AtmosphereTile atmosphere)
	     {
		          if (atmosphere == null)
				return;

		          // Collect all thermal energies
		          List<ThermalEnergy> allEnergies = new List<ThermalEnergy>();

		          if (atmosphere.AtmosphericEnergy != null)
				allEnergies.AddRange(atmosphere.AtmosphericEnergy);

		          if (atmosphere.WaterEnergy != null)
				allEnergies.AddRange(atmosphere.WaterEnergy);

		          if (atmosphere.AerosolisedEnergy != null)
				allEnergies.AddRange(atmosphere.AerosolisedEnergy);

		          MixTemperaturesByPhase(allEnergies);
	     }

	     /// <summary>
	     /// Mixes temperatures between two ThermalEnergy objects to achieve equilibrium
	     /// Uses the existing ThermalEnergyUtilities.CalculateEquilibriumTemperature
	     /// </summary>
	     public static void MixTwoTemperatures(ThermalEnergy a, ThermalEnergy b)
	     {
		          if (a == null || b == null || a.CurrentPhase != b.CurrentPhase)
				return;

		          // Calculate equilibrium temperature
		          float equilibriumTemp = ThermalEnergyUtilities.CalculateEquilibriumTemperature(a, b);

		          // Calculate energy needed for each to reach equilibrium
		          float energyA = a.Mass * a.SpecificHeat * a.Temperature;
		          float targetEnergyA = a.Mass * a.SpecificHeat * equilibriumTemp;
		          float energyDifferenceA = targetEnergyA - energyA;

		          // Transfer energy based on which has deficit/surplus
		          if (energyDifferenceA > 0)
		          {
				// A needs energy from B
				EnergyTransferUtilities.TransferEnergy(b, a, energyDifferenceA);
		          }
		          else if (energyDifferenceA < 0)
		          {
				float energyB = a.Mass * a.SpecificHeat * a.Temperature;
				float targetEnergyB = a.Mass * a.SpecificHeat * equilibriumTemp;
				float energyDifferenceB = targetEnergyB - energyB;
				// B needs energy from A
				EnergyTransferUtilities.TransferEnergy(a, b, energyDifferenceB);
		          }
	     }

	     /// <summary>
	     /// Gets the average temperature for a specific phase from a list of ThermalEnergy objects
	     /// </summary>
	     public static float GetAverageTemperatureByPhase(List<ThermalEnergy> energies, Phase phase)
	     {
		          if (energies == null)
				return 0f;

		          var phaseEnergies = energies.Where(e => e.CurrentPhase == phase).ToList();
		          if (phaseEnergies.Count == 0)
				return 0f;

		          float totalMass = 0f;
		          float weightedTemp = 0f;

		          foreach (var energy in phaseEnergies)
		          {
				weightedTemp += energy.Temperature * energy.Mass;
				totalMass += energy.Mass;
		          }

		          return totalMass > 0f ? weightedTemp / totalMass : 0f;
	     }

	     /// <summary>
	     /// Gets the mass-weighted average temperature for all ThermalEnergy objects in a list
	     /// </summary>
	     public static float GetAverageTemperature(List<ThermalEnergy> energies)
	     {
		          if (energies == null || energies.Count == 0)
				return 0f;

		          float totalMass = 0f;
		          float weightedTemp = 0f;

		          foreach (var energy in energies)
		          {
				weightedTemp += energy.Temperature * energy.Mass;
				totalMass += energy.Mass;
		          }

		          return totalMass > 0f ? weightedTemp / totalMass : 0f;
	     }

	     /// <summary>
	     /// Mixes boundary layer temperatures between soil and atmosphere
	     /// Uses conduction between adjacent components
	     /// </summary>
	     public static void MixBoundaryTemperatures(SoilComposition soil, AtmosphereTile atmosphere,
				   float boundaryArea, float timeStep)
	     {
		          if (soil == null || atmosphere == null || soil.energy == null)
				return;

		          // Simplified: mix soil surface layer with atmosphere bottom layer
		          // Use conduction for each soil component with atmosphere gases

		          foreach (var soilComponent in soil.energy)
		          {
				// Mix with atmospheric gases
				foreach (var gas in atmosphere.AtmosphericEnergy)
				{
					     if (soilComponent.CurrentPhase == Phase.Solid && gas.CurrentPhase == Phase.Gas)
					     {
						          // Use conduction with appropriate conductivity for solid-gas interface
						          float conductivity = 0.1f; // Approximate for soil-air interface
						          float tempDiff = soilComponent.Temperature - gas.Temperature;

						          if (Mathf.Abs(tempDiff) > 0.1f) // Only transfer if there's a significant difference
						          {
								// Simple conduction model
								float heatFlow = conductivity * boundaryArea * tempDiff * timeStep;
								if (heatFlow > 0)
									     EnergyTransferUtilities.TransferEnergy(soilComponent, gas, heatFlow);
								else
									     EnergyTransferUtilities.TransferEnergy(gas, soilComponent, -heatFlow);
						          }
					     }
				}
		          }
	     }

	     /// <summary>
	     /// Distributes energy evenly among ThermalEnergy objects of the same phase
	     /// Useful for simulating rapid mixing (like stirring)
	     /// </summary>
	     public static void DistributeEnergyEvenly(List<ThermalEnergy> energies)
	     {
		          if (energies == null || energies.Count <= 1)
				return;

		          // Group by phase
		          var phaseGroups = energies.GroupBy(e => e.CurrentPhase);

		          foreach (var group in phaseGroups)
		          {
				var groupList = group.ToList();
				if (groupList.Count <= 1)
					     continue;

				// Calculate total energy in the group
				float totalEnergy = 0f;
				foreach (var energy in groupList)
				{
					     totalEnergy += energy.StoredEnergy;
				}

				// Calculate energy per unit mass
				float totalMass = 0f;
				foreach (var energy in groupList)
				{
					     totalMass += energy.Mass;
				}

				if (totalMass <= 0f)
					     continue;

				float energyPerKg = totalEnergy / totalMass;

				// Reset all to zero and redistribute proportionally
				// This is a bit heavy-handed but ensures even distribution
				foreach (var energy in groupList)
				{
					     float currentEnergy = energy.StoredEnergy;
					     float targetEnergy = energy.Mass * energyPerKg;
					     float difference = targetEnergy - currentEnergy;

					     // We can't directly set energy, so we need to transfer from/to a pool
					     // Instead, we'll use a more iterative approach
				}
		          }

		          // For simplicity, use the mixing method which already handles energy transfer
		          MixTemperaturesByPhase(energies);
	     }
}


public static class RadiativeEnergyUtilities
{
	     // ========== RADIATIVE TRANSFER ==========

	     /// <summary>Calculate blackbody radiation emission</summary>
	     public static float CalculateBlackbodyEmission(float temperature,
				      float area, float timeStep)
	     {
		          // Stefan-Boltzmann Law: P = ε * σ * A * T⁴
		          const float STEFAN_BOLTZMANN = 5.670374419e-8f; // W/m²·K⁴
		          float power = Constants.ATMOSPHERIC_EMISSIVITY * STEFAN_BOLTZMANN * area *
			       (Mathf.Pow(temperature, 4) - Mathf.Pow(Constants.COSMIC_BACKGROUND, 4));
		          return power * timeStep; // Energy over time step
	     }

	     /// <summary>Calculate absorbed radiation using Beer-Lambert law</summary>
	     public static float CalculateAbsorbedRadiation(RadiativeEnergy incident,
				      float absorptionCoefficient,
				      float pathLength, float area,
				      float timeStep)
	     {
		          float absorbedIntensity = incident.Intensity *
				     (1 - Mathf.Exp(-absorptionCoefficient * pathLength));
		          float absorbedPower = absorbedIntensity * area;
		          return absorbedPower * timeStep;
	     }

	     /// <summary>Convert radiative to thermal energy (e.g., sunlight heating surface)</summary>
	     public static float ConvertRadiativeToThermal(RadiativeEnergy radiative,
				     ThermalEnergy thermal,
				     float absorptionEfficiency,
				     float area, float timeStep)
	     {
		          float availableEnergy = radiative.StoredEnergy * area * timeStep;
		          float absorbed = availableEnergy * Mathf.Clamp01(absorptionEfficiency);

		          radiative.ExtractEnergy(absorbed);
		          thermal.AddEnergy(absorbed);

		          return absorbed;
	     }
}

public static class ChemicalEnergyUtilities
{
	     // ========== CHEMICAL TRANSFORMATIONS ==========

	     /// <summary>Perform a chemical reaction and transfer energy</summary>
	     public static float PerformChemicalReaction(ChemicalEnergy chemicals,
				   string reactant, string product,
				   float moles, EnergyData sink)
	     {
		          float energyChange = chemicals.CalculateEnergyFromReaction(reactant, product, moles);

		          if (energyChange < 0) // Exothermic - release energy
		          {
				chemicals.ExtractEnergy(-energyChange);
				if (sink != null)
					     sink.AddEnergy(-energyChange);
				return -energyChange;
		          }
		          else // Endothermic - consume energy
		          {
				if (sink != null && sink.CanTransferEnergy(energyChange))
				{
					     sink.ExtractEnergy(energyChange);
					     chemicals.AddEnergy(energyChange);
					     return energyChange;
				}
		          }
		          return 0;
	     }

	     /// <summary>Convert chemical energy to thermal (combustion)</summary>
	     public static float ConvertChemicalToThermal(ChemicalEnergy chemical,
				    ThermalEnergy thermal,
				    float efficiency = 0.8f)
	     {
		          float available = chemical.StoredEnergy;
		          float converted = available * Mathf.Clamp01(efficiency);

		          chemical.ExtractEnergy(converted);
		          thermal.AddEnergy(converted);

		          return converted;
	     }
}

public static class LatentEnergyUtilities
{
	     // ========== PHASE CHANGE HELPERS ==========

	     /// <summary>Handle phase change with energy balance</summary>
	     public static bool TryPhaseChangeWithEnergyBalance(LatentEnergy latent,
				          ThermalEnergy thermal,
				          Phase newPhase, float mass)
	     {
		          float requiredEnergy = latent.GetPhaseChangeEnergy(newPhase, mass);
		          float availableEnergy = thermal.StoredEnergy;

		          if (Mathf.Abs(requiredEnergy) <= availableEnergy)
		          {
				// Phase change occurs - energy is transferred
				if (requiredEnergy > 0) // Endothermic (needs energy)
				{
					     thermal.ExtractEnergy(requiredEnergy);
					     // Energy is stored in latent form (bonds breaking)
					     latent.AddEnergy(requiredEnergy);
				}
				else // Exothermic (releases energy)
				{
					     latent.ExtractEnergy(-requiredEnergy);
					     thermal.AddEnergy(-requiredEnergy);
				}

				latent.TryPhaseChange(newPhase, mass, ref requiredEnergy);
				return true;
		          }
		          return false;
	     }

	     /// <summary>Calculate melting/freezing at boundary</summary>
	     public static void ProcessPhaseBoundary(LatentEnergy ice, LatentEnergy water,
			             ThermalEnergy thermal, float interfaceArea,
			             float timeStep)
	     {
		          // Simplified: heat transfer drives phase change
		          float heatForMelting = 100f * interfaceArea * timeStep; // Simplified rate

		          if (thermal.Temperature > 273.15f) // Above freezing
		          {
				// Melting: ice -> water
				float massThatCanMelt = heatForMelting / ice.LatentHeatFusion;
				TryPhaseChangeWithEnergyBalance(ice, thermal, Phase.Liquid, massThatCanMelt);
		          }
		          else // Below freezing
		          {
				// Freezing: water -> ice
				float massThatCanFreeze = heatForMelting / water.LatentHeatFusion;
				TryPhaseChangeWithEnergyBalance(water, thermal, Phase.Solid, massThatCanFreeze);
		          }
	     }
}

public static class MechanicalEnergyUtilities
{
	     // ========== MECHANICAL CONVERSIONS ==========

	     /// <summary>Convert mechanical to thermal energy (friction)</summary>
	     public static float ConvertFrictionToHeat(MechanicalEnergy mechanical,
				ThermalEnergy thermal,
				float efficiency = 0.95f)
	     {
		          float available = mechanical.StoredEnergy;
		          float heatGenerated = available * Mathf.Clamp01(efficiency);

		          mechanical.ExtractEnergy(heatGenerated);
		          thermal.AddEnergy(heatGenerated);

		          return heatGenerated;
	     }

	     /// <summary>Convert potential to kinetic energy (falling)</summary>
	     public static void ConvertPotentialToKinetic(MechanicalEnergy mechanical, float mass,
				    float heightChange, float gravity = 9.81f)
	     {
		          // ΔPE = m * g * Δh
		          // This would need mass information - you might need to extend MechanicalEnergy
	     }
}

// ========== EXTENSION METHODS ==========

public static class EnergyExtensions
{
	     public static float GetTotalMass(this SoilComposition composition)
	     {
		          return composition.sand.Mass + composition.silt.Mass + composition.clay.Mass +
			  composition.organicMaterial.Mass + composition.water.Mass;
	     }

	     public static float GetEnergyDensity(this EnergyData energy)
	     {
		          // Base implementation - override in derived classes if needed
		          return energy.StoredEnergy;
	     }

	     public static string GetEnergySummary(this EnergyData energy)
	     {
		          return $"{energy.Type}: {energy.StoredEnergy:F0} J";
	     }
	     //derive temperature from ThermalEnergy
	     public static float DeriveTemperature(ThermalEnergy thermalEnergy)
	     {
		          if (thermalEnergy.Mass <= 0f || thermalEnergy.SpecificHeat <= 0f)
		          {
				Debug.LogWarning("Mass or SpecificHeat is zero or negative, cannot derive temperature.");
				return 0f; // Avoid division by zero
		          }
		          return thermalEnergy.StoredEnergy / (thermalEnergy.Mass * thermalEnergy.SpecificHeat);
	     }
}