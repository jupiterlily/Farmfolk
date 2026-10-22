using System.Collections.Generic;
using UnityEngine;

public static class HydrosphereUtilities
{                   /// <summary>
	     /// Calculates total water in atmosphere and surface system
	     /// </summary>
	     private static float GetTotalWater(AtmosphereTile atmosphereTile, TileData surfaceTile)
	     {
		          float total = 0f;

		          if (atmosphereTile != null)
		          {
				total += atmosphereTile.waterVapor.Mass;
				total += atmosphereTile.cloudWaterContent.Mass;
				total += atmosphereTile.iceParticles.Mass;
		          }


		          if (surfaceTile != null)
		          {
				total += surfaceTile.surfaceLiquid.Mass;
				total += surfaceTile.surfaceIce.Mass;
				total += surfaceTile.composition.water.Mass;
				total += surfaceTile.composition.ice.Mass;
		          }

		          return total;
	     }
	     /// <summary>
	     /// Calculates relative humidity from water vapor mass
	     /// </summary>
	     private static float CalculateRelativeHumidity(float temperature, float waterVapor, float pressure, float volume)
	     {
		          if (volume <= 0 || waterVapor <= 0)
				return 0f;

		          float saturationMass = CalculateSaturationWaterMass(temperature, pressure, volume);
		          if (saturationMass <= 0)
				return 0f;

		          return Mathf.Clamp01(waterVapor / saturationMass);
	     }

	     /// <summary>
	     /// Calculates saturation water mass in kg for given conditions
	     /// </summary>
	     private static float CalculateSaturationWaterMass(float temperatureC, float pressurePa, float volumeM3)
	     {
		          // Calculate saturation vapor pressure in Pa
		          float saturationVaporPressurePa = CalculateSaturationVaporPressure(temperatureC);

		          // Calculate saturation mixing ratio (kg water/kg dry air)
		          float mixingRatio = 0.622f * saturationVaporPressurePa /
			    Mathf.Max(pressurePa - saturationVaporPressurePa, 1f);

		          // Calculate dry air density (kg/m³)
		          float T = temperatureC + 273.15f;
		          float dryAirDensity = pressurePa / (Constants.GAS_CONSTANT * T);

		          // Saturation vapor density (kg/m³)
		          float saturationVaporDensity = mixingRatio * dryAirDensity;

		          // Total mass in this volume
		          return saturationVaporDensity * volumeM3;
	     }

	     /// <summary>
	     /// Calculates saturation vapor pressure using Magnus formula
	     /// </summary>
	     private static float CalculateSaturationVaporPressure(float temperatureC)
	     {
		          // Magnus formula for saturation vapor pressure in Pa
		          float T = temperatureC;

		          if (T >= 0)
		          {
				// Over water
				return 611.2f * Mathf.Exp(17.67f * T / (T + 243.5f));
		          }
		          else
		          {
				// Over ice
				return 611.2f * Mathf.Exp(22.46f * T / (T + 272.62f));
		          }
	     }

	     /// <summary>
	     /// Calculates dew point temperature
	     /// </summary>
	     private static float CalculateDewPoint(float temperatureC, float relativeHumidity)
	     {
		          if (relativeHumidity <= 0)
				return -100f; // Very cold if no humidity

		          // Magnus formula for dew point
		          float a = 17.27f;
		          float b = 237.7f;

		          float alpha = ((a * temperatureC) / (b + temperatureC)) + Mathf.Log(relativeHumidity);
		          return (b * alpha) / (a - alpha);
	     }

	     /// <summary>
	     /// Calculates realistic air mass using ideal gas law
	     /// </summary>
	     private static float CalculateRealisticAirMass(float temperatureC, float pressurePa, float volumeM3)
	     {
		          float T = temperatureC + 273.15f;

		          if (T <= 0 || pressurePa <= 0 || volumeM3 <= 0)
		          {
				// Return reasonable default for sea level conditions
				return 1.225f * volumeM3;
		          }

		          // Moles of air: n = PV/RT
		          float moles = (pressurePa * volumeM3) / (Constants.GAS_CONSTANT * T);

		          // Mass of dry air (approximate)
		          float mass = moles * Constants.MOLAR_MASS_AIR;

		          return Mathf.Max(mass, 0.1f);
	     }

	     /// <summary>
	     /// Calculates evaporation loss during precipitation fall through atmosphere
	     /// </summary>
	     public static float CalculateFallEvaporationLoss(int layer, float temperature, float relativeHumidity)
	     {
		          //this is terrible and completely against the priniciples of the game.
		          // Less evaporation in lower layers (more humid), more in upper layers
		          float layerFactor = Mathf.Clamp01(layer * 0.3f); // 30% per layer

		          // More evaporation in dry air
		          float humidityFactor = Mathf.Pow(1 - relativeHumidity, 2f);

		          // More evaporation when warm
		          float tempFactor = Mathf.Clamp01(temperature / 30f);

		          return Mathf.Clamp01(layerFactor * humidityFactor * tempFactor * 0.1f); // Max 10% loss
	     }
	     /// <summary>
	     /// Main entry point for all hydrosphere updates - coordinates all water phase changes
	     /// </summary>
	     public static void UpdateSurfaceHydrosphere(
	         AtmosphereTile atmosphereTile,
	         TileData surfaceTile,
	         float solarRadiation,
	         float hours)
	     {
		          if (atmosphereTile == null || surfaceTile == null)
				return;
		          SoilUtilities.RecalculateTilePhysics(surfaceTile);
		          // Store initial water for conservation check
		          //float initialTotalWater = GetTotalWater(atmosphereTile, surfaceTile);

		          //// Calculate air mass once for all thermodynamic calculations
		          //float airVolume = atmosphereTile.tileWidth * atmosphereTile.tileWidth * atmosphereTile.layerHeight;
		          //float airMass = CalculateRealisticAirMass(
		          //    atmosphereTile.temperature,
		          //    atmosphereTile.pressure,
		          //    airVolume
		          //);ti

		          //float surfaceArea = surfaceTile.tileWidth * surfaceTile.tileWidth;
		          //float soilMass = SoilUtilities.GetSoilMass(surfaceTile.composition);

		          // 1. Surface evaporation (driven by solar radiation and atmospheric conditions)
		          // (commented out in original – can be re‑enabled with updated calls)

		          // 2. Atmospheric phase changes (cloud formation/precipitation)
		          ApplyAtmosphericPhaseChanges(atmosphereTile, hours);

		          // 3. Surface condensation (dew/frost formation)
		          SurfaceCondensation(surfaceTile, hours);

		          SoilUtilities.InfiltrateSurfaceMoisture(surfaceTile, hours);
		          // 4. Surface evaporation (simplified)
		          SurfaceEvaporation(atmosphereTile, surfaceTile, hours);

		          // 5. Solar‑driven evaporation
		          SolarEvaporation(atmosphereTile, surfaceTile, solarRadiation, hours);

		          // 6. Precipitation (commented out in original – can be re‑enabled)
		          SensibleHeatExchange(atmosphereTile, surfaceTile, hours);
		          // 7. Validate water conservation

	     }

	     public static void RedistributeWaterLateral(WorldTile tile, float hours)
	     {//this is water movement within the soil.
	      // Helper to calculate total water for a tile + neighbors
	      //        float GetTotalWater(WorldTile t)
	      //        {
	      //float total = t.tileData.composition.water.Mass;
	      //foreach (var n in t.GetNeighbors())
	      //{
	      //	     if (n != null)
	      //		          total += n.tileData.composition.water.Mass;
	      //}
	      //return total;
	      //        }

		          // Debug.Log($"[LateralWater] Before redistribution: Total water = {GetTotalWater(tile)} kg");

		          SoilComposition sc = tile.tileData.composition;
		          float rate = sc.horizontalPermeability / 100f / 24f; // cm/day -> m/hour

		          float waterToMove = sc.water.Mass;
		          List<WorldTile> neighbors = tile.GetNeighbors();

		          foreach (WorldTile neighbor in neighbors)
		          {
				if (neighbor == null)
					     continue;

				SoilComposition nc = neighbor.tileData.composition;
				float deltaSat = sc.saturation - nc.saturation;
				if (deltaSat <= 0f)
					     continue;

				float transfer = deltaSat * rate * hours * 1000f; // m3 -> kg
				transfer = Mathf.Clamp(transfer, 0f, waterToMove);

				//sc.water -= transfer;
				//nc.water += transfer;

				ThermalEnergyUtilities.TransferMassAndEnergy(sc.water, nc.water, transfer);

				waterToMove -= transfer;

				float fineTotal = sc.silt.Mass + sc.clay.Mass;
				if (fineTotal > 0f)
				{
					     float sedimentFraction = sc.erodibility * Mathf.Clamp01(deltaSat);
					     float sedimentTransfer = sedimentFraction * transfer;

					     float siltRatio = sc.silt.Mass / fineTotal;
					     float clayRatio = sc.clay.Mass / fineTotal;

					     float siltMoved = sedimentTransfer * siltRatio;
					     float clayMoved = sedimentTransfer * clayRatio;

					     float organicMoved = sedimentTransfer * 0.1f;

					     ThermalEnergyUtilities.TransferMassAndEnergy(sc.silt, nc.silt, siltMoved);
					     ThermalEnergyUtilities.TransferMassAndEnergy(sc.clay, nc.clay, clayMoved);
					     ThermalEnergyUtilities.TransferMassAndEnergy(sc.organicMaterial, nc.organicMaterial, organicMoved);
				}

				SoilUtilities.RecalculateTilePhysics(neighbor.tileData);
		          }

		          SoilUtilities.RecalculateTilePhysics(tile.tileData);

		          //    Debug.Log($"[LateralWater] After redistribution: Total water = {GetTotalWater(tile)} kg");
	     }
	     public static void SensibleHeatExchange(AtmosphereTile atmo, TileData surface, float hours)
	     {
		          // Temperature difference (Air - Ground)
		          float tempDiff = atmo.temperature - surface.soilTemperature;
		          if (Mathf.Abs(tempDiff) < 0.1f)
				return;

		          float area = surface.tileWidth * surface.tileWidth;
		          float timeSeconds = hours * 3600f;

		          // Convective heat transfer coefficient (W/m²·K)
		          // Depends on wind speed. 5 is calm, 20 is very windy.
		          float h = 5f + 10f * atmo.windSpeed;

		          // Q = h * A * ΔT * Δt (Joules)
		          float heatFlow = h * area * tempDiff * timeSeconds;

		          // Limit the flow to prevent absolute equilibrium in 1 frame
		          float maxTransfer = Mathf.Min(atmo.Mass * 2000f, surface.composition.Mass * 2000f) * 0.1f;
		          heatFlow = Mathf.Clamp(heatFlow, -maxTransfer, maxTransfer);

		          if (heatFlow > 0) // Air is hotter -> Heat flows INTO the ground
		          {
				// 1. Take heat from the atmosphere proportionally
				float airMass = atmo.Mass;
				if (airMass <= 0)
					     return;
				foreach (var nrg in atmo.energies)
				{
					     float frac = nrg.Mass / airMass;
					     nrg.RemoveEnergy(heatFlow * frac);
				}

				// 2. Give heat to the ground (soil and surface water proportionally)
				float groundMass = surface.composition.Mass + surface.surfaceLiquid.Mass;
				if (groundMass <= 0)
					     return;
				float soilFrac = surface.composition.Mass / groundMass;
				float waterFrac = surface.surfaceLiquid.Mass / groundMass;

				surface.surfaceLiquid.AddEnergy(heatFlow * waterFrac);
				foreach (var comp in surface.composition.energy)
				{
					     comp.AddEnergy(heatFlow * soilFrac * (comp.Mass / surface.composition.Mass));
				}
		          }
		          else // Ground is hotter -> Heat flows INTO the air
		          {
				// Reverse the logic
				float groundMass = surface.composition.Mass + surface.surfaceLiquid.Mass;
				if (groundMass <= 0)
					     return;
				float soilFrac = surface.composition.Mass / groundMass;
				float waterFrac = surface.surfaceLiquid.Mass / groundMass;

				surface.surfaceLiquid.RemoveEnergy(-heatFlow * waterFrac);
				foreach (var comp in surface.composition.energy)
				{
					     comp.RemoveEnergy(-heatFlow * soilFrac * (comp.Mass / surface.composition.Mass));
				}

				float airMass = atmo.Mass;
				if (airMass <= 0)
					     return;
				float energyToAdd = -heatFlow; // heatFlow is negative, so this makes it positive
				foreach (var nrg in atmo.energies)
				{
					     float frac = nrg.Mass / airMass;
					     nrg.AddEnergy(energyToAdd * frac);
				}
		          }
	     }
	     // ---------- Atmospheric Phase Changes (updated) ----------
	     public static void ApplyAtmosphericPhaseChanges(AtmosphereTile atm, float hours)
	     {

		          if (atm.waterVapor == null || atm.cloudWaterContent == null)
				return;

		          float relativeHumidity = CalculateRelativeHumidity(atm);
		          float timeFactor = Mathf.Min(hours, 1.0f) * 0.1f; // prevents oscillation

		          // 1. Condensation (vapor → cloud)
		          if (relativeHumidity > 1.01f && atm.waterVapor.Mass > 0)
		          {
				float saturationMass = CalculateSaturationMass(atm);
				float excessVapor = atm.waterVapor.Mass - saturationMass;
				float massToCondense = Mathf.Min(excessVapor * timeFactor, atm.waterVapor.Mass);
				if (massToCondense > 0)
					     ApplyCondensation(atm, massToCondense);
		          }

		          // 2. Evaporation (cloud → vapor)
		          else if (relativeHumidity < 0.98f && atm.cloudWaterContent.Mass > 0)
		          {
				float saturationMass = CalculateSaturationMass(atm);
				float vaporDeficit = saturationMass - atm.waterVapor.Mass;
				float massToEvaporate = Mathf.Min(vaporDeficit * timeFactor, atm.cloudWaterContent.Mass);
				if (massToEvaporate > 0)
					     ApplyEvaporation(atm, massToEvaporate);
		          }
	     }

	     private static void ApplyEvaporation(AtmosphereTile atm, float massToEvaporate)
	     {
		          // Only call the utility; it handles all energy moves.
		          ThermalEnergyUtilities.TryEvaporate(atm.cloudWaterContent, atm.waterVapor, massToEvaporate);
		          // No extra steps!
	     }

	     private static void ApplyCondensation(AtmosphereTile atm, float massToCondense)
	     {
		          // Use the new TryCondense
		          ThermalEnergyUtilities.TryCondense(atm.waterVapor, atm.cloudWaterContent, massToCondense);
		          // No extra steps!
	     }
	     public static float TryEvaporate(ThermalEnergy sourceLiquid, ThermalEnergy destVapor, float requestedMass)
	     {
		          if (sourceLiquid.Mass <= 0 || requestedMass <= 0)
				return 0;

		          // We can only evaporate if we have enough sensible heat to supply the latent heat
		          // without dropping below the freezing point (or a defined minimum temperature).
		          float minAllowedTemp = sourceLiquid.MeltingPoint; // 273.15K for water
		          float currentTemp = sourceLiquid.Temperature;

		          // Energy available for evaporation = sensible energy above the melting point
		          float energyAboveMelt = sourceLiquid.Mass * sourceLiquid.SpecificHeat * (currentTemp - minAllowedTemp);
		          float latentRequiredPerKg = sourceLiquid.LatentHeatVaporization;

		          // Maximum mass we can evaporate with the available energy
		          float maxEvapMass = energyAboveMelt / latentRequiredPerKg;
		          float actualMass = Mathf.Min(requestedMass, maxEvapMass, sourceLiquid.Mass);

		          if (actualMass <= 0)
				return 0;

		          //// 1. Remove latent heat from the liquid's sensible energy (cools it down)
		          //float latentEnergy = actualMass * latentRequiredPerKg;
		          //sourceLiquid.RemoveEnergy(latentEnergy); // this cools it exactly to minAllowedTemp

		          // 2. Transfer the mass (and its remaining sensible energy) to the vapor
		          //    The vapor receives the mass but *not* the latent heat (that energy is spent).
		          sourceLiquid.TransferMassTo(destVapor, actualMass);

		          //       // 3. Now destVapor (which just got mass at minAllowedTemp) needs the latent heat added.
		          //       //    But that latent heat came from the liquid's sensible energy (already removed).
		          //       //    Actually, the vapor must have the latent heat to exist. 
		          //       //    We removed it from the liquid, we need to add it to the vapor.
		          //       destVapor.AddEnergy(latentEnergy); // This warms the vapor or adds to its internal latent energy.
		          //// Note: Your ThermalEnergy.ProcessPhaseChanges() will automatically allocate this to sensible 
		          //// or latent based on temperature. Since we added mass and energy, if it's above boiling point,
		          // it stays vapor.

		          return actualMass;
	     }
	     // ---------- Surface Condensation (updated) ----------
	     public static void SurfaceCondensation(TileData hydroTile, float hours)
	     {
		          AtmosphereTile atmotile = hydroTile.atmosphereTile;
		          if (atmotile == null || atmotile.waterVapor.Mass < 1 || hydroTile.surfaceTemperature >= atmotile.DewPoint)
				return;

		          float tempDiff = atmotile.DewPoint - hydroTile.surfaceTemperature;
		          if (tempDiff <= 0)
				return;

		          float condensationRate = Mathf.Clamp01(tempDiff / 10f) * 0.1f * hours;
		          float maxCondensation = atmotile.waterVapor.Mass * condensationRate;
		          float water = Mathf.Min(maxCondensation, atmotile.waterVapor.Mass * 0.01f);
		          if (water <= 0)
				return;

		          //float latentReleased = water * atmotile.waterVapor.LatentHeatVaporization;

		          //// 1. Cool the vapor
		          //atmotile.waterVapor.RemoveEnergy(latentReleased);

		          // 2. Move mass to surface
		          ThermalEnergyUtilities.TransferMassAndEnergy(atmotile.waterVapor, hydroTile.surfaceLiquid, water);

		          //        // 3. Warm the surface and atmosphere (distribute the released heat)
		          //        //    You can split between surface and air proportionally
		          //        float totalMass = hydroTile.surfaceLiquid.Mass + hydroTile.composition.Mass + atmotile.Mass;
		          //        if (totalMass > 0f)
		          //        {
		          //float perKg = latentReleased / totalMass;
		          //// Surface gets its share
		          //hydroTile.surfaceLiquid.AddEnergy(perKg * hydroTile.surfaceLiquid.Mass);
		          //foreach (var comp in hydroTile.composition.energy)
		          //	     comp.AddEnergy(perKg * comp.Mass);
		          //// Atmosphere gets its share
		          //foreach (var nrg in atmotile.energies)
		          //	     nrg.AddEnergy(perKg * nrg.Mass);
		          //        }
	     }

	     // ---------- Surface Evaporation (updated) ----------
	     public static void SurfaceEvaporation(AtmosphereTile atmotile, TileData hydroTile, float hours)
	     {
		          if (atmotile == null || hydroTile == null)
				return;

		          float saturationVaporPressure = AtmosphereUtilities.CalculateSaturationVaporPressure(hydroTile.surfaceTemperature);
		          float actualVaporPressure = atmotile.RelativeHumidity *
		              AtmosphereUtilities.CalculateSaturationVaporPressure(atmotile.temperature);

		          float vaporPressureDeficit = saturationVaporPressure - actualVaporPressure;
		          if (vaporPressureDeficit <= 0)
				return;

		          float aerodynamicTerm = 0.0026f * (1f + 0.54f * atmotile.windSpeed);
		          float radiationTerm = 0.001f;
		          float potentialEvaporation = (aerodynamicTerm * vaporPressureDeficit + radiationTerm) * hours;
		          float potentialEvaporationKg = potentialEvaporation * hydroTile.tileWidth * hydroTile.tileWidth;

		          float availablesurfaceLiquid = hydroTile.surfaceLiquid.Mass;
		          float availableSoilWater = hydroTile.composition.water.Mass * 0.1f;
		          float totalAvailable = availablesurfaceLiquid + availableSoilWater;
		          float actualEvaporation = Mathf.Min(potentialEvaporationKg, totalAvailable);

		          if (actualEvaporation <= 0)
				return;

		          // Evaporate from surface first, then soil
		          float surfaceEvaporation = Mathf.Min(actualEvaporation, availablesurfaceLiquid);
		          float soilEvaporation = Mathf.Min(actualEvaporation - surfaceEvaporation, availableSoilWater);

		          if (surfaceEvaporation > 0)
		          {
				float maxEvaporation = vaporPressureDeficit * 0.001f * hours * hydroTile.tileWidth * hydroTile.tileWidth;
				surfaceEvaporation = Mathf.Min(surfaceEvaporation, maxEvaporation);
				float latentRequired = surfaceEvaporation * hydroTile.surfaceLiquid.LatentHeatVaporization;

				// 1. Cool the surface water
				//hydroTile.surfaceLiquid.RemoveEnergy(latentRequired);

				// 2. Move mass
				ThermalEnergyUtilities.TransferMassAndEnergy(hydroTile.surfaceLiquid, atmotile.waterVapor, surfaceEvaporation);
				ThermalEnergyUtilities.TransferMassAndEnergy(hydroTile.composition.water, atmotile.waterVapor, soilEvaporation);
				//// 3. Warm the atmosphere
				//float totalAirMass = 0f;
				//foreach (var nrg in atmotile.energies)
				//	     totalAirMass += nrg.Mass;
				//if (totalAirMass > 0f)
				//{
				//	     float perKg = latentRequired / totalAirMass;
				//	     foreach (var nrg in atmotile.energies)
				//		          nrg.AddEnergy(perKg * nrg.Mass);
				//}
		          }
	     }

	     // ---------- Solar Evaporation (updated) ----------
	     public static void SolarEvaporation(AtmosphereTile atmotile, TileData tile, float solarRadiation, float hours)
	     {
		          if (atmotile == null || tile == null || solarRadiation <= 0)
				return;

		          float solarEnergy = solarRadiation * 3600f * hours;
		          float availableEnergy = solarEnergy * 0.6f;
		          float maxEvaporation = availableEnergy / (tile.surfaceLiquid.LatentHeatVaporization);

		          float soilMoistureFactor = Mathf.Clamp01(tile.composition.saturation);
		          maxEvaporation *= soilMoistureFactor;

		          float atmosphericDemand = 1f - atmotile.RelativeHumidity;
		          maxEvaporation *= atmosphericDemand;

		          float availableWater = tile.surfaceLiquid.Mass + (tile.composition.water.Mass * 0.05f);
		          float actualEvaporation = Mathf.Min(maxEvaporation, availableWater);

		          if (actualEvaporation <= 0)
				return;

		          float surfaceEvaporation = Mathf.Min(actualEvaporation, tile.surfaceLiquid.Mass);

		          if (surfaceEvaporation > 0)
		          {
				// IMPORTANT: TransferMassTo handles the energy removal automatically
				// Do NOT call RemoveEnergy separately!
				ThermalEnergyUtilities.TransferMassAndEnergy(tile.surfaceLiquid, atmotile.waterVapor, surfaceEvaporation);

				// The latent heat is already accounted for in the phase change logic
				// Just mark the atmosphere as dirty to recalculate temperature
				atmotile.MarkTemperatureDirty();
		          }
	     }

	     // ---------- Redistribute Surface Water (updated) ----------
	     //  public static void RedistributesurfaceLiquid(WorldTile tile, float hours)
	     //  {
	     //        if (tile.tileData.surfaceLiquid.CurrentPhase == Phase.Solid)
	     //return;

	     //        float flowRate = tile.tileData.surfaceLiquid.Mass; // kg/hour
	     //        var neighbors = tile.GetNeighbors();
	     //        var downhillNeighbors = new List<WorldTile>();

	     //        foreach (var wT in neighbors)
	     //        {
	     //float heightA = tile.tileData.volume;
	     //float heightB = wT.tileData.volume;
	     //float delta = heightA - heightB;
	     //if (delta > 0f)
	     //	     downhillNeighbors.Add(wT);
	     //        }

	     //        downhillNeighbors = downhillNeighbors.OrderByDescending(v => v.tileData.volume).ToList();

	     //        if (downhillNeighbors.Count > 0)
	     //        {
	     //foreach (var neighbor in downhillNeighbors)
	     //{
	     //	     if (neighbor == null)
	     //		          continue;
	     //	     float heightA = tile.tileData.volume;
	     //	     float heightB = neighbor.tileData.volume;
	     //	     float delta = heightA - heightB;
	     //	     float gradient = Mathf.Clamp01(delta);
	     //	     float flow = flowRate * gradient * hours;
	     //	     float availableWater = tile.tileData.surfaceLiquid.Mass;
	     //	     float toTransfer = Mathf.Min(flow, availableWater / downhillNeighbors.Count);

	     //	     if (toTransfer > 0f)
	     //	     {
	     //		          // Move water (and its energy) to the downhill tile
	     //		          ThermalEnergyUtilities.TransferMassAndEnergy(
	     //		              tile.tileData.surfaceLiquid,
	     //		              neighbor.tileData.surfaceLiquid,
	     //		              toTransfer
	     //		          );
	     //		          SoilUtilities.ApplySurfaceErosion(tile.tileData, neighbor.tileData, gradient, toTransfer);
	     //	     }
	     //}
	     //        }
	     //  }
	     public static void ConductHeatLateral(WorldTile tile, float hours)
	     {//this is conducting heat to our neighbors

		          hours = hours / 4;
		          ThermalEnergy water = tile.tileData.surfaceLiquid;
		          ThermalEnergy ice = tile.tileData.surfaceIce;
		          List<WorldTile> neighbors = tile.GetNeighbors();

		          float area = tile.tileData.tileWidth * tile.tileData.tileWidth;
		          float distance = tile.tileData.tileWidth;

		          foreach (WorldTile neighbor in neighbors)
		          {
				if (neighbor == null)
					     continue;


				ThermalEnergyUtilities.ConductHeat(water, neighbor.tileData.surfaceLiquid, area, distance, hours * 3600f);
				ThermalEnergyUtilities.ConductHeat(water, neighbor.tileData.surfaceIce, area, distance, hours * 3600f);
				ThermalEnergyUtilities.ConductHeat(ice, neighbor.tileData.surfaceLiquid, area, distance, hours * 3600f);
				ThermalEnergyUtilities.ConductHeat(ice, neighbor.tileData.surfaceIce, area, distance, hours * 3600f);


		          }

		          //    Debug.Log($"[LateralWater] After redistribution: Total water = {GetTotalWater(tile)} kg");
	     }
	     // ---------- Helper Methods (unchanged) ----------
	     private static float CalculateRelativeHumidity(AtmosphereTile atm)
	     {
		          if (atm.waterVapor == null || atm.waterVapor.Mass <= 0)
				return 0f;
		          float saturationMass = CalculateSaturationMass(atm);
		          if (saturationMass <= 0)
				return 0f;
		          return atm.waterVapor.Mass / saturationMass;
	     }

	     private static float CalculateSaturationMass(AtmosphereTile atm)
	     {
		          float tempK = atm.temperature;
		          float satVP = AtmosphereUtilities.CalculateSaturationVaporPressure(tempK); // Pa
		          float R = 461.5f; // specific gas constant for water vapour (J/kg·K)
		          float density = satVP / (R * atm.temperature); // kg/m³
		          float volume = atm.tileWidth * atm.tileWidth * atm.layerHeight;
		          return density * volume;
	     }


	     /// <summary>
	     /// Processes melting, freezing, evaporation, and sublimation for the tile's surface water.
	     /// Uses exact energy balance – no arbitrary fraction limits.
	     /// </summary>
	     public static void ProcessSurfacePhaseChanges(TileData tile)
	     {
		          if (tile == null)
				return;

		          // ---- MELTING (ice → liquid) ----
		          if (tile.surfaceIce.Mass > 0f)
		          {
				float meltTemp = tile.surfaceIce.MeltingPoint; // 273.15 K
				float cpIce = tile.surfaceIce.SpecificHeat;
				float energyAtMelt = tile.surfaceIce.Mass * cpIce * meltTemp;
				float excess = tile.surfaceIce.sensibleEnergy - energyAtMelt;

				if (excess > 0f)
				{
					     // Maximum ice that can melt with this excess energy
					     float maxMeltMass = excess / tile.surfaceIce.LatentHeatFusion;
					     float massToMelt = Mathf.Min(maxMeltMass, tile.surfaceIce.Mass);

					     if (massToMelt > 0f)
					     {
						          // Transfer mass (and its proportional energy) to liquid
						          ThermalEnergyUtilities.TransferMassAndEnergy(tile.surfaceIce, tile.surfaceLiquid, massToMelt);
					     }
				}
		          }

		          // ---- FREEZING (liquid → ice) ----
		          if (tile.surfaceLiquid.Mass > 0f)
		          {
				float freezeTemp = tile.surfaceLiquid.MeltingPoint;
				float cpLiq = tile.surfaceLiquid.SpecificHeat;
				float energyAtFreeze = tile.surfaceLiquid.Mass * cpLiq * freezeTemp;
				float deficit = energyAtFreeze - tile.surfaceLiquid.sensibleEnergy;

				if (deficit > 0f)
				{
					     // How much liquid must freeze to release enough latent heat to raise temperature to freezeTemp?
					     // The released heat warms the remaining liquid+ice mixture.
					     // We approximate by freezing a mass such that the released latent heat equals the deficit.
					     float massToFreeze = deficit / tile.surfaceLiquid.LatentHeatFusion;
					     massToFreeze = Mathf.Min(massToFreeze, tile.surfaceLiquid.Mass);

					     if (massToFreeze > 0f)
					     {
						          // Freezing releases latent heat, which reduces the deficit.
						          // Transfer mass from liquid to ice.
						          ThermalEnergyUtilities.TransferMassAndEnergy(tile.surfaceLiquid, tile.surfaceIce, massToFreeze);
						          // After transfer, the released latent heat is now in the ice object (as sensible energy? Actually, TransferMassAndEnergy moves proportional sensible+latent. Freezing should convert latent to sensible. We need to handle that.
						          // Better: we manually move mass and then convert latent heat to sensible.
						          // But we can trust TransferMassAndEnergy to move both sensible and latent correctly.
						          // However, freezing should release latent heat, which means the ice should gain sensible energy.
						          // In TransferMassAndEnergy, the source loses its latent energy proportionally, and the target gains it.
						          // So the ice will gain latent energy, which is fine – it will later be converted to sensible if needed.
					     }
				}
		          }

		          // ---- EVAPORATION (liquid → vapour) ----
		          if (tile.surfaceLiquid.Mass > 0f && tile.atmosphereTile != null)
		          {
				float boilTemp = tile.surfaceLiquid.BoilingPoint;
				float cpLiq = tile.surfaceLiquid.SpecificHeat;
				float energyAtBoil = tile.surfaceLiquid.Mass * cpLiq * boilTemp;
				float excess = tile.surfaceLiquid.sensibleEnergy - energyAtBoil;

				if (excess > 0f)
				{
					     float maxEvapMass = excess / tile.surfaceLiquid.LatentHeatVaporization;
					     float massToEvap = Mathf.Min(maxEvapMass, tile.surfaceLiquid.Mass);

					     if (massToEvap > 0f)
					     {
						          // Move mass from liquid to atmosphere's water vapour
						          ThermalEnergyUtilities.TransferMassAndEnergy(tile.surfaceLiquid, tile.atmosphereTile.waterVapor, massToEvap);
					     }
				}
		          }

		          // ---- SUBLIMATION (ice → vapour) ----
		          if (tile.surfaceIce.Mass > 0f && tile.atmosphereTile != null)
		          {

				float meltTemp = tile.surfaceIce.MeltingPoint;
				float boilTemp = tile.surfaceIce.BoilingPoint;
				float cpIce = tile.surfaceIce.SpecificHeat;
				float energyToBoil = tile.surfaceIce.Mass * cpIce * boilTemp;
				float excess = tile.surfaceIce.sensibleEnergy - energyToBoil;

				if (excess > 0f)
				{
					     float maxSubMass = excess / (tile.surfaceIce.LatentHeatFusion + tile.surfaceIce.LatentHeatVaporization);
					     float massToSub = Mathf.Min(maxSubMass, tile.surfaceIce.Mass);

					     if (massToSub > 0f)
					     {
						          // Move mass from ice to atmosphere's water vapour
						          ThermalEnergyUtilities.TransferMassAndEnergy(tile.surfaceIce, tile.atmosphereTile.waterVapor, massToSub);
					     }
				}
		          }
		          float atmoratio = tile.atmosphereTile.tileWidth / tile.tileWidth;
		          tile.CollectGases(atmoratio * atmoratio);
	     }
}
