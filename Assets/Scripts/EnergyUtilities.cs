using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnergyTransferUtilities
{
	     /// <summary>Transfer energy between any two EnergyData objects. Returns actual transferred.</summary>
	     public static float TransferEnergy(EnergyData source, EnergyData sink, float joules)
	     {
		          if (source == null || sink == null || joules <= 0f)
				return 0f;

		          float available = source.StoredEnergy;
		          float toTransfer = Mathf.Min(joules, available);
		          if (toTransfer <= 0f)
				return 0f;


		          source.RemoveEnergy(toTransfer);
		          sink.AddEnergy(toTransfer);
		          return toTransfer;
	     }

	     /// <summary>Transfer energy with efficiency (0‑1).</summary>
	     public static float TransferEnergyWithLoss(EnergyData source, EnergyData sink, float joules, float efficiency)
	     {
		          if (efficiency <= 0f)
				return 0f;
		          float extracted = source.RemoveEnergy(joules);
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
		          if (distance <= 0)
		          {

		          }

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
	     //  public static float TryEvaporate(ThermalEnergy sourceLiquid, ThermalEnergy destVapor, float requestedMass)
	     //  {
	     //        if (sourceLiquid.Mass <= 0 || requestedMass <= 0)
	     //return 0;

	     //        // We can only evaporate if we have enough sensible heat to supply the latent heat
	     //        // without dropping below the freezing point (or a defined minimum temperature).
	     //        float minAllowedTemp = sourceLiquid.MeltingPoint; // 273.15K for water
	     //        float currentTemp = sourceLiquid.Temperature;

	     //        // Energy available for evaporation = sensible energy above the melting point
	     //        float energyAboveMelt = sourceLiquid.Mass * sourceLiquid.SpecificHeat * (currentTemp - minAllowedTemp);
	     //        float latentRequiredPerKg = sourceLiquid.LatentHeatVaporization;

	     //        // Maximum mass we can evaporate with the available energy
	     //        float maxEvapMass = energyAboveMelt / latentRequiredPerKg;
	     //        float actualMass = Mathf.Min(requestedMass, maxEvapMass, sourceLiquid.Mass);

	     //        if (actualMass <= 0)
	     //return 0;

	     //        //    The vapor receives the mass but *not* the latent heat (that energy is spent).
	     //        sourceLiquid.TransferMassTo(destVapor, actualMass);


	     //        return actualMass;
	     //  }


	     //  public static float TryCondense(ThermalEnergy sourceVapor, ThermalEnergy destLiquid, float requestedMass)
	     //  {
	     //        if (sourceVapor.Mass <= 0 || requestedMass <= 0)
	     //return 0;

	     //        // Vapor must have enough mass and latent heat to condense.
	     //        // We can condense only if the vapor temperature is below boiling point? 
	     //        // Actually condensation happens when saturated; we just take the requested mass.
	     //        float actualMass = Mathf.Min(requestedMass, sourceVapor.Mass);

	     //        // 1. Latent heat released = mass * latent heat of vaporization
	     //        float latentReleased = actualMass * sourceVapor.LatentHeatVaporization;

	     //        // 2. Remove that latent heat from the vapor (cools it down)
	     //        sourceVapor.RemoveEnergy(latentReleased);

	     //        // 3. Move mass + remaining sensible energy to liquid
	     //        sourceVapor.TransferMassTo(destLiquid, actualMass);

	     //        // 4. Add the released latent heat to the liquid (warms it)
	     //        destLiquid.AddEnergy(latentReleased);

	     //        return actualMass;
	     //  }
	     /// <summary>Transfer mass and associated thermal energy</summary>
	     public static void TransferMassAndEnergy(ThermalEnergy source, ThermalEnergy sink,
		 float massToTransfer)
	     {
		          if (source == null || sink == null || massToTransfer <= 0)
				return;

		          float actualTransfer = Mathf.Min(massToTransfer, source.Mass);
		          if (actualTransfer <= 0)
				return;

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
	     public static void MixTemperaturesByPhase(ThermalEnergy[] thermalEnergies)
	     {
		          if (thermalEnergies == null || thermalEnergies.Length <= 1)
				return;

		          // Group by phase (gas, liquid, solid)
		          var phaseGroups = thermalEnergies
		              .GroupBy(te => te.CurrentPhase)
		              .Where(g => g.Count() > 1); // Only mix if there are multiple items

		          foreach (var group in phaseGroups)
		          {
				var groupList = group.ToList();
				MixTemperaturesForPhaseGroupDirect(groupList);
		          }
	     }

	     private static void MixTemperaturesForPhaseGroupDirect(List<ThermalEnergy> phaseGroup)
	     {
		          if (phaseGroup.Count <= 1)
				return;

		          // Calculate total sensible energy and total heat capacity
		          float totalEnergy = 0f;
		          float totalHeatCapacity = 0f;
		          foreach (var te in phaseGroup)
		          {
				totalEnergy += te.Mass * te.SpecificHeat * te.Temperature;
				totalHeatCapacity += te.Mass * te.SpecificHeat;
		          }

		          if (totalHeatCapacity <= 0f)
				return;

		          float equilibriumTemp = totalEnergy / totalHeatCapacity;

		          // Apply equilibrium temperature to each component, conserving total energy
		          foreach (var te in phaseGroup)
		          {
				float currentEnergy = te.StoredEnergy;
				float targetEnergy = te.Mass * te.SpecificHeat * equilibriumTemp;
				float diff = targetEnergy - currentEnergy;

				if (diff > 0.0001f)
					     te.AddEnergy(diff);
				else if (diff < -0.0001f)
					     te.RemoveEnergy(-diff);
				// If diff is tiny, ignore to avoid floating-point noise
		          }
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
	     // </summary>
	     public static void MixTemperaturesByConduction(List<ThermalEnergy> thermalEnergies,
    float seconds, // <--- MUST BE IN SECONDS
    float area = 1f, float distance = 1f)
	     {
		          if (thermalEnergies == null || thermalEnergies.Count <= 1)
				return;

		          // Conduct between every pair in the ENTIRE list (all phases mixed together)
		          for (int i = 0; i < thermalEnergies.Count; i++)
		          {
				for (int j = i + 1; j < thermalEnergies.Count; j++)
				{
					     ThermalEnergy a = thermalEnergies[i];
					     ThermalEnergy b = thermalEnergies[j];

					     // Skip if either has zero mass to avoid division issues
					     if (a.Mass <= 0f || b.Mass <= 0f)
						          continue;

					     ThermalEnergy hot, cold;
					     if (a.Temperature >= b.Temperature)
					     {
						          hot = a;
						          cold = b;
					     }
					     else
					     {
						          hot = b;
						          cold = a;
					     }

					     float tempDiff = hot.Temperature - cold.Temperature;
					     if (tempDiff <= 0f)
						          continue;

					     // Use the arithmetic mean conductivity for the interface
					     float effectiveK = (hot.ThermalConductivity + cold.ThermalConductivity) * 0.5f;

					     // Fourier's Law: Q = k * A * (ΔT / d) * Δt
					     float heatFlow = effectiveK * area * (tempDiff / distance) * seconds;

					     // TransferEnergy safely caps the transfer to what 'hot' actually has
					     EnergyTransferUtilities.TransferEnergy(hot, cold, heatFlow);
				}
		          }
	     }
	     /// <summary>
	     /// Mixes all ThermalEnergy objects in an AtmosphereTile using energy transfer
	     /// </summary>
	     //  public static void MixAtmosphereTileTemperatures(AtmosphereTile atmosphere)
	     //  {
	     //        if (atmosphere == null)
	     //return;

	     //        // Collect all thermal energies
	     //        ThermalEnergy[] allEnergies = ThermalEnergy[];

	     //        if (atmosphere.AtmosphericEnergy != null)
	     //allEnergies.AddRange(atmosphere.AtmosphericEnergy);

	     //        if (atmosphere.WaterEnergy != null)
	     //allEnergies.AddRange(atmosphere.WaterEnergy);

	     //        if (atmosphere.AerosolisedEnergy != null)
	     //allEnergies.AddRange(atmosphere.AerosolisedEnergy);

	     //        MixTemperaturesByPhase(allEnergies);
	     //  }

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
	     public static void DistributeEnergyEvenly(ThermalEnergy[] energies)
	     {
		          if (energies == null || energies.Length <= 1)
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

	     //  public static void ProcessPhaseChanges(ThermalEnergy nrg)
	     //  {
	     //        bool changed;
	     //        do
	     //        {
	     //changed = false;

	     //// --- HEATING (Melting / Vaporization) ---
	     //// These take energy FROM sensible (cooling the mass) to break bonds.
	     //if (nrg.CurrentPhase == Phase.Solid && nrg.Temperature >= nrg.MeltingPoint)
	     //{
	     //	     float needed = nrg.Mass * nrg.LatentHeatFusion;
	     //	     if (nrg.sensibleEnergy >= needed)
	     //	     {
	     //		          // In evaporation/melting, heat comes FROM the object itself OR the environment.
	     //		          // If we have enough sensible, we just convert it internally.
	     //		          nrg.sensibleEnergy -= needed;
	     //		          nrg.latentEnergy += needed;
	     //		          nrg.CurrentPhase = Phase.Liquid;
	     //		          changed = true;
	     //		          if (nrg.materialName == "ice")
	     //		          {
	     //				nrg.materialName = "liquid water";
	     //		          }
	     //		          MatterLibrary.GetSpecificHeat(nrg.materialName, nrg.Temperature);
	     //	     }
	     //	     else
	     //	     {
	     //		          nrg.Temperature = nrg.MeltingPoint;
	     //	     }
	     //}
	     //else if (CurrentPhase == Phase.Liquid && Temperature >= BoilingPoint)
	     //{
	     //	     float needed = Mass * LatentHeatVaporization;
	     //	     if (sensibleEnergy >= needed)
	     //	     {
	     //		          sensibleEnergy -= needed;
	     //		          latentEnergy += needed;
	     //		          CurrentPhase = Phase.Gas;
	     //		          changed = true;
	     //		          if (materialName == "liquid water")
	     //		          {
	     //				materialName = "water vapour";
	     //		          }
	     //		          MatterLibrary.GetSpecificHeat(materialName, temperature);
	     //	     }
	     //	     else
	     //	     {
	     //		          Temperature = BoilingPoint;
	     //	     }
	     //}

	     //// Cooling (freezing, condensation)
	     //if (CurrentPhase == Phase.Liquid && Temperature < MeltingPoint)
	     //{
	     //	     float released = Mass * LatentHeatFusion;
	     //	     if (latentEnergy >= released)
	     //	     {
	     //		          latentEnergy -= released;
	     //		          sensibleEnergy += released;
	     //		          CurrentPhase = Phase.Solid;
	     //		          changed = true;
	     //		          if (materialName == "liquid water")
	     //		          {
	     //				materialName = "ice";
	     //		          }
	     //		          MatterLibrary.GetSpecificHeat(materialName, temperature);
	     //	     }
	     //	     else
	     //	     {
	     //		          Temperature = MeltingPoint;
	     //	     }
	     //}
	     //else if (CurrentPhase == Phase.Gas && Temperature < BoilingPoint)
	     //{
	     //	     float released = Mass * LatentHeatVaporization;
	     //	     if (latentEnergy >= released)
	     //	     {
	     //		          latentEnergy -= released;
	     //		          sensibleEnergy += released;
	     //		          CurrentPhase = Phase.Liquid;
	     //		          changed = true;
	     //		          if (materialName == "water vapour")
	     //		          {
	     //				materialName = "liquid water";
	     //		          }
	     //		          MatterLibrary.GetSpecificHeat(materialName, temperature);
	     //	     }
	     //	     else
	     //	     {
	     //		          Temperature = BoilingPoint;
	     //	     }
	     //}

	     //// Recalculate temperature from sensible energy using current cp
	     //if (Mass > 0f)
	     //{
	     //	     float cp = SpecificHeat;
	     //	     Temperature = sensibleEnergy / (Mass * cp);
	     //}
	     //else
	     //	     Temperature = 0f;

	     //ClampTemperatureToPhase();

	     //        } while (changed);
	     //  }
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
		          float availableEnergy = radiative.StoredEnergy;
		          float absorbed = availableEnergy * Mathf.Clamp01(absorptionEfficiency);

		          EnergyTransferUtilities.TransferEnergy(radiative, thermal, absorbed);

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
				chemicals.RemoveEnergy(-energyChange);
				if (sink != null)
					     sink.AddEnergy(-energyChange);
				return -energyChange;
		          }
		          else // Endothermic - consume energy
		          {
				if (sink != null && sink.CanTransferEnergy(energyChange))
				{
					     sink.RemoveEnergy(energyChange);
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

		          chemical.RemoveEnergy(converted);
		          thermal.AddEnergy(converted);

		          return converted;
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

		          mechanical.RemoveEnergy(heatGenerated);
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