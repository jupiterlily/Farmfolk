using UnityEngine;

public static class HydrosphereUtilities
{
	     /// <summary>
	     /// Main entry point for all hydrosphere updates - coordinates all water phase changes
	     /// </summary>
	     public static void UpdateHydrosphere(
	         AtmosphereTile atmosphereTile,
	         TileData surfaceTile,
	         float solarRadiation,
	         float hours)
	     {
		          if (atmosphereTile == null || surfaceTile == null)
				return;

		          // Store initial water for conservation check
		          float initialTotalWater = GetTotalWater(atmosphereTile, surfaceTile);

		          // Calculate air mass once for all thermodynamic calculations
		          float airVolume = atmosphereTile.tileWidth * atmosphereTile.tileWidth * atmosphereTile.layerHeight;
		          float airMass = CalculateRealisticAirMass(
		              atmosphereTile.temperature,
		              atmosphereTile.pressure,
		              airVolume
		          );

		          float surfaceArea = surfaceTile.tileWidth * surfaceTile.tileWidth;
		          float soilMass = SoilUtilities.GetSoilMass(surfaceTile.composition);

		          // 1. Surface evaporation (driven by solar radiation and atmospheric conditions)
		          ApplySurfaceEvaporation(
		              atmosphereTile, surfaceTile,
		              solarRadiation, airMass, soilMass,
		              surfaceArea, hours
		          );

		          // 2. Atmospheric phase changes (cloud formation/precipitation)
		          ApplyAtmosphericPhaseChanges(
		              atmosphereTile, airMass, hours
		          );

		          // 3. Surface condensation (dew/frost formation)
		          ApplySurfaceCondensation(
		              atmosphereTile, surfaceTile,
		              airMass, soilMass, hours
		          );

		          // 4. Calculate and apply precipitation
		          float precipitation = CalculateAndApplyPrecipitation(
		              atmosphereTile, surfaceTile,
		              surfaceArea, hours
		          );

		          // 5. Validate water conservation
		          ValidateWaterConservation(
		              atmosphereTile, surfaceTile,
		              initialTotalWater, precipitation
		          );
	     }

	     /// <summary>
	     /// Calculates precipitation based on cloud water content and applies it to surface
	     /// </summary>
	     private static float CalculateAndApplyPrecipitation(
	         AtmosphereTile atmosphereTile,
	         TileData surfaceTile,
	         float surfaceArea,
	         float hours)
	     {
		          if (atmosphereTile == null || atmosphereTile.cloudWaterContent <= 0)
				return 0f;

		          // Calculate relative humidity
		          float relativeHumidity = CalculateRelativeHumidity(
		              atmosphereTile.temperature,
		              atmosphereTile.waterVapor,
		              atmosphereTile.pressure,
		              atmosphereTile.tileWidth * atmosphereTile.tileWidth * atmosphereTile.layerHeight
		          );

		          // Basic precipitation conditions
		          if (relativeHumidity < 0.95f)
				return 0f;

		          // Calculate saturation vapor pressure
		          float saturationVaporPressure = CalculateSaturationVaporPressure(atmosphereTile.temperature);

		          // Calculate dew point
		          float dewPoint = CalculateDewPoint(atmosphereTile.temperature, relativeHumidity);

		          // Precipitation intensity depends on how far below dew point temperature is
		          float temperatureBelowDewPoint = dewPoint - atmosphereTile.temperature;

		          if (temperatureBelowDewPoint <= 0)
				return 0f;

		          // Precipitation rate: 0-20 mm/hour based on conditions
		          float baseRate = Mathf.Lerp(0f, 20f, Mathf.Clamp01(temperatureBelowDewPoint / 10f));

		          // Adjust for relative humidity
		          float humidityFactor = Mathf.Pow(relativeHumidity, 3f);

		          // Adjust for cloud water content (more clouds = more precipitation)
		          float cloudFactor = Mathf.Clamp01(atmosphereTile.cloudWaterContent / 1000f);

		          // Total precipitation rate in mm/hour
		          float precipRate = baseRate * humidityFactor * cloudFactor;

		          // Convert to kg for this timestep (1 mm = 1 kg/m²)
		          float precipMass = precipRate * surfaceArea * hours;

		          // Cannot precipitate more than available cloud water
		          precipMass = Mathf.Min(precipMass, atmosphereTile.cloudWaterContent * 0.8f);

		          if (precipMass <= 0)
				return 0f;

		          // Apply precipitation
		          atmosphereTile.cloudWaterContent -= precipMass;
		          surfaceTile.surfaceMoisture += precipMass;

		          // If temperature is below freezing, add to snow/ice instead
		          if (atmosphereTile.temperature < 0)
		          {
				// In a more complex system, you'd track snow separately
				// For now, just add to surface moisture but note it's frozen
				surfaceTile.surfaceTemperature -= precipMass * 0.01f; // Cooling from snow
		          }

		          return precipMass;
	     }

	     /// <summary>
	     /// Applies surface evaporation with proper energy balance
	     /// </summary>
	     private static void ApplySurfaceEvaporation(
	         AtmosphereTile atmosphereTile,
	         TileData surfaceTile,
	         float solarRadiation,
	         float airMass,
	         float soilMass,
	         float surfaceArea,
	         float hours)
	     {
		          // Calculate saturation vapor pressures
		          float surfaceSaturationVP = CalculateSaturationVaporPressure(surfaceTile.surfaceTemperature);
		          float airSaturationVP = CalculateSaturationVaporPressure(atmosphereTile.temperature);
		          float airActualVP = atmosphereTile.RelativeHumidity * airSaturationVP;

		          // Vapor pressure deficit (driving force for evaporation)
		          float vaporPressureDeficit = surfaceSaturationVP - airActualVP;

		          if (vaporPressureDeficit <= 0 || surfaceTile.surfaceMoisture <= 0)
				return;

		          // Calculate available energy for evaporation
		          // Solar radiation (converted from W/m² to J/m² for the timestep)
		          float solarEnergy = solarRadiation * 3600f * hours; // J/m²

		          // Energy available for evaporation (typically 50-70% of net radiation)
		          float availableEnergy = solarEnergy * 0.6f * surfaceArea; // J

		          // Maximum evaporation from energy constraint
		          float maxEvaporationFromEnergy = availableEnergy / Constants.LATENT_HEAT_EVAPORATION; // kg

		          // Aerodynamic term (wind-dependent)
		          float windFactor = 1f + (atmosphereTile.windSpeed * 0.2f);
		          float aerodynamicEvaporation = vaporPressureDeficit * 0.0026f * windFactor * hours * surfaceArea;

		          // Total potential evaporation
		          float potentialEvaporation = Mathf.Min(maxEvaporationFromEnergy, aerodynamicEvaporation);

		          // Limit by available surface water
		          float availableWater = surfaceTile.surfaceMoisture +
				  (surfaceTile.composition.water * 0.1f); // Top 10cm of soil

		          float actualEvaporation = Mathf.Min(potentialEvaporation, availableWater);

		          if (actualEvaporation <= 0)
				return;

		          // Evaporate from surface first, then soil
		          float surfaceEvaporation = Mathf.Min(actualEvaporation, surfaceTile.surfaceMoisture);
		          surfaceTile.surfaceMoisture -= surfaceEvaporation;

		          float soilEvaporation = Mathf.Min(actualEvaporation - surfaceEvaporation,
				             surfaceTile.composition.water * 0.1f);
		          surfaceTile.composition.water -= soilEvaporation;

		          // Add to atmospheric vapor
		          atmosphereTile.waterVapor += actualEvaporation;

		          // Apply latent heat cooling
		          float energyAbsorbed = actualEvaporation * Constants.LATENT_HEAT_EVAPORATION; // J

		          // Partition cooling between air and surface based on mass
		          float airHeatCapacity = airMass * Constants.AIR_SPECIFIC_HEAT;
		          float soilHeatCapacity = soilMass * SoilUtilities.GetSoilHeatCapacity(surfaceTile.composition);
		          float totalHeatCapacity = airHeatCapacity + soilHeatCapacity;

		          if (totalHeatCapacity > 0)
		          {
				float airCooling = energyAbsorbed * (airHeatCapacity / totalHeatCapacity);
				float surfaceCooling = energyAbsorbed * (soilHeatCapacity / totalHeatCapacity);

				atmosphereTile.temperature -= airCooling / airHeatCapacity;
				surfaceTile.surfaceTemperature -= surfaceCooling / soilHeatCapacity;
		          }
	     }

	     /// <summary>
	     /// Applies atmospheric phase changes (cloud formation/evaporation)
	     /// </summary>
	     private static void ApplyAtmosphericPhaseChanges(
	         AtmosphereTile atmosphereTile,
	         float airMass,
	         float hours)
	     {
		          if (atmosphereTile == null)
				return;

		          float volume = atmosphereTile.tileWidth * atmosphereTile.tileWidth * atmosphereTile.layerHeight;

		          // Calculate saturation
		          float saturationMass = CalculateSaturationWaterMass(
		              atmosphereTile.temperature,
		              atmosphereTile.pressure,
		              volume
		          );

		          float relativeHumidity = atmosphereTile.waterVapor / Mathf.Max(saturationMass, 0.001f);

		          // Time step factor (limit changes to prevent oscillation)
		          float timeFactor = Mathf.Min(hours, 1.0f) * 0.1f; // Max 10% change per hour

		          if (relativeHumidity > 1.01f && atmosphereTile.waterVapor > 0)
		          {
				// CONDENSATION: Form clouds
				float excessVapor = atmosphereTile.waterVapor - saturationMass;
				float vaporToCondense = Mathf.Min(excessVapor * timeFactor, atmosphereTile.waterVapor);

				if (vaporToCondense > 0)
				{
					     atmosphereTile.waterVapor -= vaporToCondense;
					     atmosphereTile.cloudWaterContent += vaporToCondense;

					     // Release latent heat
					     float heatCapacity = airMass * Constants.AIR_SPECIFIC_HEAT;
					     if (heatCapacity > 0)
					     {
						          float temperatureChange = (vaporToCondense * Constants.LATENT_HEAT_CONDENSATION) / heatCapacity;
						          atmosphereTile.temperature += temperatureChange;
					     }
				}
		          }
		          else if (relativeHumidity < 0.98f && atmosphereTile.cloudWaterContent > 0)
		          {
				// EVAPORATION: Clouds evaporate
				float vaporDeficit = saturationMass - atmosphereTile.waterVapor;
				float maxEvaporation = vaporDeficit * timeFactor;
				float cloudToEvaporate = Mathf.Min(maxEvaporation, atmosphereTile.cloudWaterContent);

				if (cloudToEvaporate > 0)
				{
					     atmosphereTile.cloudWaterContent -= cloudToEvaporate;
					     atmosphereTile.waterVapor += cloudToEvaporate;

					     // Absorb latent heat
					     float heatCapacity = airMass * Constants.AIR_SPECIFIC_HEAT;
					     if (heatCapacity > 0)
					     {
						          float temperatureChange = -(cloudToEvaporate * Constants.LATENT_HEAT_EVAPORATION) / heatCapacity;
						          atmosphereTile.temperature += temperatureChange;
					     }
				}
		          }
	     }

	     /// <summary>
	     /// Applies surface condensation (dew/frost formation)
	     /// </summary>
	     private static void ApplySurfaceCondensation(
	         AtmosphereTile atmosphereTile,
	         TileData surfaceTile,
	         float airMass,
	         float soilMass,
	         float hours)
	     {
		          if (atmosphereTile == null || atmosphereTile.waterVapor <= 0)
				return;

		          // Calculate dew point
		          float relativeHumidity = CalculateRelativeHumidity(
		              atmosphereTile.temperature,
		              atmosphereTile.waterVapor,
		              atmosphereTile.pressure,
		              atmosphereTile.tileWidth * atmosphereTile.tileWidth * atmosphereTile.layerHeight
		          );

		          float dewPoint = CalculateDewPoint(atmosphereTile.temperature, relativeHumidity);

		          // Condensation occurs when surface is below dew point
		          if (surfaceTile.surfaceTemperature < dewPoint)
		          {
				float volume = atmosphereTile.tileWidth * atmosphereTile.tileWidth * atmosphereTile.layerHeight;
				float saturationMass = CalculateSaturationWaterMass(
				    atmosphereTile.temperature,
				    atmosphereTile.pressure,
				    volume
				);

				// How much vapor is available to condense
				float excessVapor = atmosphereTile.waterVapor - saturationMass;

				if (excessVapor <= 0)
					     return;

				// Condensation rate (depends on temperature difference)
				float tempDifference = dewPoint - surfaceTile.surfaceTemperature;
				float condensationRate = Mathf.Clamp01(tempDifference / 5f) * 0.1f * hours;

				float vaporToCondense = Mathf.Min(excessVapor * condensationRate, atmosphereTile.waterVapor);

				if (vaporToCondense <= 0)
					     return;

				// Apply condensation
				atmosphereTile.waterVapor -= vaporToCondense;
				surfaceTile.surfaceMoisture += vaporToCondense;

				// Release latent heat
				float energyReleased = vaporToCondense * Constants.LATENT_HEAT_CONDENSATION;

				// Partition heat between air and surface
				float airHeatCapacity = airMass * Constants.AIR_SPECIFIC_HEAT;
				float soilHeatCapacity = soilMass * SoilUtilities.GetSoilHeatCapacity(surfaceTile.composition);
				float totalHeatCapacity = airHeatCapacity + soilHeatCapacity;

				if (totalHeatCapacity > 0)
				{
					     float airWarming = energyReleased * (airHeatCapacity / totalHeatCapacity);
					     float surfaceWarming = energyReleased * (soilHeatCapacity / totalHeatCapacity);

					     atmosphereTile.temperature += airWarming / airHeatCapacity;
					     surfaceTile.surfaceTemperature += surfaceWarming / soilHeatCapacity;
				}
		          }
	     }

	     /// <summary>
	     /// Redistributes surface water between tiles (runoff)
	     /// </summary>
	     public static void RedistributeSurfaceWater(WorldTile tile, float hours)
	     {
		          if (tile == null || tile.tileData == null)
				return;

		          float flowRate = 1.0f; // m/hour (reduced from 100 m/hour)
		          var neighbors = tile.GetNeighbors();

		          foreach (var neighbor in neighbors)
		          {
				if (neighbor == null || neighbor.tileData == null)
					     continue;

				// Use elevation difference for flow direction
				float heightA = tile.tileData.elevation + tile.tileData.surfaceMoisture * 0.001f; // Water adds slight height
				float heightB = neighbor.tileData.elevation + neighbor.tileData.surfaceMoisture * 0.001f;
				float delta = heightA - heightB;

				if (delta > 0.001f) // Small threshold to prevent micro-oscillations
				{
					     // Flow proportional to gradient and available water
					     float gradient = delta / tile.tileData.tileWidth;
					     float flow = flowRate * gradient * hours * tile.tileData.tileWidth * tile.tileData.tileWidth;

					     // Cannot flow more than available water
					     float availableWater = tile.tileData.surfaceMoisture;
					     float maxTransfer = Mathf.Min(flow, availableWater * 0.1f); // Max 10% per step

					     if (maxTransfer <= 0)
						          continue;

					     // Transfer water
					     tile.tileData.surfaceMoisture -= maxTransfer;
					     neighbor.tileData.surfaceMoisture += maxTransfer;

					     // Apply erosion (optional)
					     // SoilUtilities.ApplySurfaceErosion(tile.tileData, neighbor.tileData, gradient, maxTransfer);
				}
		          }
	     }

	     /// <summary>
	     /// Validates water conservation after all updates
	     /// </summary>
	     private static void ValidateWaterConservation(
	         AtmosphereTile atmosphereTile,
	         TileData surfaceTile,
	         float initialTotalWater,
	         float precipitationMass)
	     {
		          float finalTotalWater = GetTotalWater(atmosphereTile, surfaceTile);
		          float waterError = Mathf.Abs(finalTotalWater - initialTotalWater);

		          // Precipitation is added from atmosphere to surface, so it's not an error
		          // We should account for this in our check
		          float expectedChange = precipitationMass; // Positive: water moved from air to surface
		          float actualChange = finalTotalWater - initialTotalWater;

		          // Allow small floating-point errors
		          if (Mathf.Abs(actualChange - expectedChange) > 0.001f)
		          {
				Debug.LogWarning($"Water conservation error: Expected change {expectedChange}, got {actualChange}");

				// Fix small errors by redistributing proportionally
				float correction = initialTotalWater + expectedChange - finalTotalWater;
				if (Mathf.Abs(correction) > 0.001f)
				{
					     // Add correction back to atmosphere (where most water is)
					     atmosphereTile.waterVapor += correction;
				}
		          }
	     }

	     /// <summary>
	     /// Calculates total water in atmosphere and surface system
	     /// </summary>
	     private static float GetTotalWater(AtmosphereTile atmosphereTile, TileData surfaceTile)
	     {
		          float total = 0f;

		          if (atmosphereTile != null)
		          {
				total += atmosphereTile.waterVapor;
				total += atmosphereTile.cloudWaterContent;
				total += atmosphereTile.liquidWater;
		          }

		          if (surfaceTile != null)
		          {
				total += surfaceTile.surfaceMoisture;
				total += surfaceTile.composition.water;
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
		          // Less evaporation in lower layers (more humid), more in upper layers
		          float layerFactor = Mathf.Clamp01(layer * 0.3f); // 30% per layer

		          // More evaporation in dry air
		          float humidityFactor = Mathf.Pow(1 - relativeHumidity, 2f);

		          // More evaporation when warm
		          float tempFactor = Mathf.Clamp01(temperature / 30f);

		          return Mathf.Clamp01(layerFactor * humidityFactor * tempFactor * 0.1f); // Max 10% loss
	     }

	     public static void CondenseVapour(AtmosphereTile atmotile)
	     {
		          if (atmotile == null || atmotile.waterVapor < 0.001f)
				return;

		          if (atmotile.temperature < atmotile.DewPoint && atmotile.waterVapor > 0)
		          {
				// Calculate saturation and only condense excess
				float saturationPressure = AtmosphereUtilities.CalculateSaturationVaporPressure(atmotile.temperature);
				float actualPressure = atmotile.RelativeHumidity * saturationPressure;

				if (actualPressure > saturationPressure)
				{
					     // Calculate excess vapor to condense
					     float excessRatio = 1f - (saturationPressure / actualPressure);
					     float waterToCondense = atmotile.waterVapor * excessRatio * 0.1f; // 10% per hour

					     waterToCondense = Mathf.Min(waterToCondense, atmotile.waterVapor);

					     // Calculate proper temperature change using air mass and heat capacity
					     float airMass = (float)AtmosphereUtilities.CalculateAirMass(atmotile.temperature, atmotile.pressure, atmotile.tileWidth * atmotile.tileWidth * atmotile.layerHeight);
					     float tempChange = (waterToCondense * Constants.LATENT_HEAT_CONDENSATION) /
						        (airMass * Constants.AIR_SPECIFIC_HEAT);

					     atmotile.cloudWaterContent += waterToCondense;
					     atmotile.waterVapor -= waterToCondense;
					     atmotile.temperature += tempChange;
				}
		          }
	     }
	     public static void SurfaceCondensation(TileData hydroTile, float hours)
	     {
		          AtmosphereTile atmotile = hydroTile.atmosphereTile;
		          //condense water vapour into surface liquid water based on temperature and humidity
		          if (atmotile == null || atmotile.waterVapor < 1 || hydroTile.surfaceTemperature >= atmotile.DewPoint)
		          {
				return;
		          }
		          if (hydroTile.surfaceTemperature < atmotile.DewPoint)
		          {
				float water = atmotile.waterVapor / 100;

				hydroTile.surfaceMoisture += water;
				float kjreleased = water * (Constants.LATENT_HEAT_CONDENSATION * 1000);
				float absorbedBySoil = kjreleased / (SoilUtilities.GetSoilHeatCapacity(hydroTile.composition) * SoilUtilities.GetSoilMass(hydroTile.composition));
				hydroTile.surfaceTemperature += absorbedBySoil / 100000;
				atmotile.temperature += (kjreleased - absorbedBySoil) / 100000;
				atmotile.waterVapor -= water;

		          }
	     }
	     public static void AtmosphericEvaporation(AtmosphereTile atmotile)
	     {
		          // Complete this method:
		          if (atmotile == null || (atmotile.cloudWaterContent < 1 && atmotile.liquidWater < 1))
				return;

		          // Calculate saturation vapor pressure
		          float saturationVaporPressure = AtmosphereUtilities.CalculateSaturationVaporPressure(atmotile.temperature);
		          float actualVaporPressure = atmotile.RelativeHumidity * saturationVaporPressure;

		          // Evaporation occurs when air is unsaturated and there's liquid water available
		          if (atmotile.RelativeHumidity < 0.95f && atmotile.temperature > atmotile.DewPoint)
		          {
				float evaporationRate = 0.01f; // Base rate in kg/hour

				// Adjust rate based on humidity deficit
				float humidityDeficit = 1f - atmotile.RelativeHumidity;
				evaporationRate *= humidityDeficit;

				// Adjust for temperature (more evaporation when warmer)
				float tempFactor = Mathf.Clamp01((atmotile.temperature - 273f) / 30f);
				evaporationRate *= (1f + tempFactor);

				// Wind enhances evaporation
				float windFactor = 1f + (atmotile.windSpeed * 0.1f);
				evaporationRate *= windFactor;

				// Determine what to evaporate (prioritize cloud water)
				float totalEvaporatable = atmotile.cloudWaterContent + atmotile.liquidWater;
				float evaporation = Mathf.Min(evaporationRate, totalEvaporatable);

				// Evaporate from clouds first, then suspended liquid
				float cloudEvaporation = Mathf.Min(evaporation, atmotile.cloudWaterContent);
				atmotile.cloudWaterContent -= cloudEvaporation;

				float liquidEvaporation = Mathf.Min(evaporation - cloudEvaporation, atmotile.liquidWater);
				atmotile.liquidWater -= liquidEvaporation;

				// Add to water vapor
				atmotile.waterVapor += (cloudEvaporation + liquidEvaporation);

				// Apply latent heat cooling
				float kjAbsorbed = (cloudEvaporation + liquidEvaporation) * Constants.LATENT_HEAT_EVAPORATION;
				atmotile.temperature -= kjAbsorbed / 10000f;
		          }
	     }

	     public static void SurfaceEvaporation(AtmosphereTile atmotile, TileData hydroTile, float hours)
	     {
		          // Complete this method:
		          if (atmotile == null || hydroTile == null)
				return;

		          // Calculate potential evaporation using Penman-Monteith simplified equation
		          float saturationVaporPressure = AtmosphereUtilities.CalculateSaturationVaporPressure(hydroTile.surfaceTemperature);
		          float actualVaporPressure = atmotile.RelativeHumidity *
		              AtmosphereUtilities.CalculateSaturationVaporPressure(atmotile.temperature);

		          float vaporPressureDeficit = saturationVaporPressure - actualVaporPressure;

		          if (vaporPressureDeficit <= 0)
				return;

		          // Calculate aerodynamic term (wind dependent)
		          float aerodynamicTerm = 0.0026f * (1f + 0.54f * atmotile.windSpeed);

		          // Calculate radiation term (simplified - you could add solar radiation here)
		          float radiationTerm = 0.001f; // Base radiation term

		          // Calculate potential evaporation (mm/hour)
		          float potentialEvaporation = (aerodynamicTerm * vaporPressureDeficit + radiationTerm) * hours;

		          // Convert to kg (1mm water = 1kg/m² for your tile area)
		          float potentialEvaporationKg = potentialEvaporation * hydroTile.tileWidth * hydroTile.tileWidth;

		          // Cap evaporation by available water
		          float availableSurfaceWater = hydroTile.surfaceMoisture;
		          float availableSoilWater = hydroTile.composition.water * 0.1f; // Only top layer evaporates

		          float totalAvailable = availableSurfaceWater + availableSoilWater;
		          float actualEvaporation = Mathf.Min(potentialEvaporationKg, totalAvailable);

		          if (actualEvaporation <= 0)
				return;

		          // Evaporate from surface first, then soil
		          float surfaceEvaporation = Mathf.Min(actualEvaporation, availableSurfaceWater);
		          hydroTile.surfaceMoisture -= surfaceEvaporation;

		          float soilEvaporation = Mathf.Min(actualEvaporation - surfaceEvaporation, availableSoilWater);
		          hydroTile.composition.water -= soilEvaporation;

		          // Add to atmospheric water vapor
		          atmotile.waterVapor += actualEvaporation;

		          // Apply latent heat cooling to surface
		          float kjAbsorbed = actualEvaporation * Constants.LATENT_HEAT_EVAPORATION;
		          float absorbedBySoil = kjAbsorbed * (soilEvaporation / actualEvaporation);
		          float absorbedBySurface = kjAbsorbed * (surfaceEvaporation / actualEvaporation);

		          // Cool the soil and surface
		          float soilHeatCapacity = SoilUtilities.GetSoilHeatCapacity(hydroTile.composition);
		          float soilMass = SoilUtilities.GetSoilMass(hydroTile.composition);
		          hydroTile.soilTemperature -= absorbedBySoil / (soilHeatCapacity * soilMass * 1000f);
		          hydroTile.surfaceTemperature -= absorbedBySurface / 100000f;
	     }

	     public static void SolarEvaporation(AtmosphereTile atmotile, TileData tile, float solarRadiation, float hours)
	     {
		          // Complete this method - focused on direct solar-driven evaporation
		          if (atmotile == null || tile == null || solarRadiation <= 0)
				return;

		          // Convert solar radiation from W/m² to energy (Joules)
		          float solarEnergy = solarRadiation * 3600f * hours; // Convert to Joules/hour

		          // Calculate energy available for evaporation (typically 50-70% of net radiation)
		          float availableEnergy = solarEnergy * 0.6f; // 60% efficiency

		          // Calculate maximum possible evaporation (latent heat of vaporization ~2.45 MJ/kg)
		          float maxEvaporation = availableEnergy / (Constants.LATENT_HEAT_EVAPORATION * 1000f);

		          // Adjust for surface conditions
		          //float albedo = SoilUtilities.GetAlbedo(tile.composition);
		          //maxEvaporation *= (1f - albedo); // Reflective surfaces evaporate less

		          // Adjust for soil moisture availability
		          float soilMoistureFactor = Mathf.Clamp01(tile.composition.saturation);
		          maxEvaporation *= soilMoistureFactor;

		          // Adjust for atmospheric demand
		          float atmosphericDemand = 1f - atmotile.RelativeHumidity;
		          maxEvaporation *= atmosphericDemand;

		          // Cap by available water
		          float availableWater = tile.surfaceMoisture + (tile.composition.water * 0.05f);
		          float actualEvaporation = Mathf.Min(maxEvaporation, availableWater);

		          if (actualEvaporation <= 0)
				return;

		          // Evaporate water
		          float surfaceEvaporation = Mathf.Min(actualEvaporation, tile.surfaceMoisture);
		          tile.surfaceMoisture -= surfaceEvaporation;

		          float soilEvaporation = Mathf.Min(actualEvaporation - surfaceEvaporation, tile.composition.water * 0.05f);
		          tile.composition.water -= soilEvaporation;

		          // Add to atmospheric moisture
		          atmotile.waterVapor += actualEvaporation;

		          // Note: Temperature effects are already accounted for in SurfaceEvaporation
		          // This method focuses on the solar radiation component
	     }
}
