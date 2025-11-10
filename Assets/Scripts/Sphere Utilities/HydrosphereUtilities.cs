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
		          //evaporate water from clouds and atmospheric liquid water based on temperature and humidity
		          if (atmotile == null || (atmotile.cloudWaterContent < 1 && atmotile.liquidWater < 1))
		          {
				return;
		          }

	     }

	     public static void SurfaceEvaporation(AtmosphereTile atmotile, TileData hydroTile, float hours)
	     {

	     }

	     public static void SolarEvaporation(AtmosphereTile atmotile, TileData hydroTile, float solarRadiation, float hours)
	     {
	     }
}
