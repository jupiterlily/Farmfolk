using UnityEngine;

public static class HydrosphereUtilities
{
	     public static void RedistributeSurfaceWater(WorldTile tile, float hours)
	     {
		          //  Debug.Log($"[SurfaceWater] Before redistribution: Total water = {GetTotalWater(tile)} kg");

		          float flowRate = 0.1f * 1000f; // m3/hour -> kg/hour
		          var neighbors = tile.GetNeighbors();

		          foreach (var neighbor in neighbors)
		          {
				if (neighbor == null)
					     continue;

				float heightA = tile.tileData.volume;
				float heightB = neighbor.tileData.volume;
				float delta = heightA - heightB;

				if (delta > 0f)
				{
					     float gradient = Mathf.Clamp01(delta);
					     float flow = flowRate * gradient * hours;

					     float availableWater = tile.tileData.surfaceMoisture;
					     float maxTransfer = Mathf.Min(flow, availableWater);

					     if (maxTransfer <= 0f)
						          continue;

					     tile.tileData.surfaceMoisture -= maxTransfer;
					     neighbor.tileData.surfaceMoisture += maxTransfer;

					     SoilUtilities.ApplySurfaceErosion(tile.tileData, neighbor.tileData, gradient, maxTransfer);
				}
		          }

		          // Debug.Log($"[SurfaceWater] After redistribution: Total water = {GetTotalWater(tile)} kg");
	     }

	     public static float CalculateCurrentPrecipitation(float hours, float cloudLitres, float rH, float temp)
	     {
		          // 1. Check basic precipitation conditions
		          if (cloudLitres <= 0 || rH <= 0.7f)
				return 0f;

		          // 2. Calculate dew point depression (temperature - dew point)
		          float dewPoint = AtmosphereUtilities.CalculateDewPoint(temp, rH);
		          float dewPointDepression = temp - dewPoint;

		          // 3. Determine precipitation probability (0-1)
		          float precipProbability = Mathf.Clamp01(
		              1f - (dewPointDepression / 5f)  // More likely when temp close to dew point
		              * Mathf.Pow(rH, 2f)        // Much more likely at high humidity
		              * Mathf.InverseLerp(25f, 5f, temp) // More likely at cooler temps
		          );

		          // 4. Roll for precipitation occurrence
		          if (Random.value > precipProbability)
				return 0f;

		          // 5. Calculate precipitation intensity (kg/m2/hour)
		          float maxPossibleRate = 0.01f * cloudLitres; // Max 1% of total moisture per hour
		          float baseRate = Mathf.Lerp(0.1f, 10f, precipProbability); // 0.1-10 mm/hour range

		          // Adjust for temperature (snow vs rain)
		          float precipRate = temp > 0f ?
		              baseRate :
		              baseRate * 0.7f; // Snow falls slower than rain

		          // 6. Calculate total precipitation mass
		          float precipMass = precipRate * hours; // Total kg for world
		          precipMass = Mathf.Min(precipMass, maxPossibleRate * hours);
		          precipMass = Mathf.Min(precipMass, cloudLitres * 0.5f); // Never remove more than 50% at once

		          return precipMass;
	     }
	     public static float CalculateFallEvaporationLoss(int layer, float temperature, float relativeHumidity)
	     {
		          // More evaporation in dry mid-layers
		          float layerLoss = 0.1f * layer; // 10% loss per layer

		          // More evaporation in dry air
		          float humidityFactor = Mathf.Pow(1 - relativeHumidity, 2f);

		          // Less evaporation when cold
		          float tempFactor = Mathf.Clamp01((temperature - 273f) / 30f); // 0 at 0°C, 1 at 30°C

		          return Mathf.Clamp01(layerLoss * humidityFactor * tempFactor);
	     }


	     public static void CondenseVapour(AtmosphereTile atmotile)
	     {
		          //if the temperature is below the dew point, condense water vapour into liquid water and remove heat energy from air
		          if (atmotile == null || atmotile.waterVapor < 1)
				return;
		          if (atmotile.temperature < atmotile.DewPoint && atmotile.waterVapor > 0)
		          {
				float water = atmotile.waterVapor;
				atmotile.cloudWaterContent += water;

				float kjreleased = water * (Constants.LATENT_HEAT_CONDENSATION);
				atmotile.temperature += kjreleased / 1000000;
				atmotile.waterVapor -= water;
		          }
		          //later we will ensure consensation nuclei are present

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
				atmotile.temperature -= kjAbsorbed / 1000000f;
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
