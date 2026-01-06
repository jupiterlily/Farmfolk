using UnityEngine;

public static class AtmosphereUtilities
{
	     /// <summary>
	     /// Simulates atmospheric processes including wind, heat transfer, and moisture movement
	     /// </summary>
	     public static void SimulateAtmosphere(AtmosphereTile[,,] atmosphereTiles, float hours)
	     {
		          int layers = atmosphereTiles.GetLength(0);
		          int width = atmosphereTiles.GetLength(1);
		          int height = atmosphereTiles.GetLength(2);

		          // Temporary arrays to store calculated changes
		          float[,,] tempChanges = new float[layers, width, height];
		          float[,,] moistureChanges = new float[layers, width, height];
		          Vector3[,,] windChanges = new Vector3[layers, width, height];

		          // First pass: calculate all changes
		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          HydrosphereUtilities.CondenseVapour(atmosphereTiles[layer, x, y]);
						          atmosphereTiles[layer, x, y].temperature -= CalculateAdiabaticCooling(atmosphereTiles[layer, x, y]);
					     }
				}
		          }

		          // Second pass: apply all changes
		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {

					     }
				}
		          }
	     }

	     /// <summary>
	     /// Calculates pressure using the barometric formula and ideal gas law
	     /// </summary>
	     private static float CalculatePressure(float temperature, float moisture, float height)
	     {
		          // Calculate air density (kg/m³)
		          float density = (Constants.MOLAR_MASS_AIR * 101325f) / (Constants.GAS_CONSTANT * (temperature + 273.15f));

		          // Adjust for moisture content (water vapor is less dense than dry air)
		          float humidityRatio = moisture / (moisture + density);
		          density *= (1f - humidityRatio * (1f - Constants.MOLAR_MASS_WATER / Constants.MOLAR_MASS_AIR));

		          // Apply barometric formula for height
		          float scaleHeight = (Constants.GAS_CONSTANT * temperature) / (Constants.MOLAR_MASS_AIR * Constants.GRAVITY_ACCELERATION);
		          return 101325f * Mathf.Exp(-height / scaleHeight);
	     }

	     /// <summary>
	     /// Calculates pressure gradient force between neighboring cells
	     /// </summary>
	     private static Vector3 CalculatePressureGradientForce(
	         AtmosphereTile[,,] atmosphereTiles, int layer, int x, int y, int width, int height)
	     {
		          Vector3 force = Vector3.zero;
		          AtmosphereTile current = atmosphereTiles[layer, x, y];

		          // Check neighbors (using Moore neighborhood)
		          int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
		          int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };

		          for (int i = 0; i < 8; i++)
		          {
				int nx = (x + dx[i] + width) % width;
				int ny = (y + dy[i] + height) % height;

				AtmosphereTile neighbor = atmosphereTiles[layer, nx, ny];
				float pressureDiff = neighbor.pressure - current.pressure;

				// Force is proportional to pressure difference and inversely proportional to distance
				Vector3 direction = new Vector3(dx[i], 0, dy[i]).normalized;
				force += direction * pressureDiff * 0.1f; // Scaling factor
		          }

		          float tempK = current.temperature + 273.15f;
		          if (tempK < 1e-6f)
				tempK = 1e-6f;
		          return force / tempK; // Normalise by temperature
	     }

	     /// <summary>
	     /// Simplified Coriolis effect calculation
	     /// </summary>
	     private static Vector3 CalculateCoriolisEffect(Vector3 windDirection)
	     {
		          const float CORIOLIS_PARAM = 1e-4f;
		          return new Vector3(-windDirection.z, 0, windDirection.x) * CORIOLIS_PARAM;
	     }

	     /// <summary>
	     /// Calculates temperature change from adiabatic processes
	     /// </summary>
	     public static float CalculateAdiabaticCooling(
	           AtmosphereTile current)
	     {
		          // Use the saturated (moist) adiabatic lapse rate (SALR) when air is saturated.

		          // Ensure valid tile
		          if (current == null)
				return 0f;
		          // Temperature in K
		          float T = current.temperature + 273.15f;
		          if (T < 1e-3f)
				T = 1e-3f;

		          // Pressure in Pa (fall back to standard if invalid)
		          float p = current.pressure;
		          if (p <= 0f)
				p = 101325f;

		          // Saturation vapor pressure using same Tetens formula (returns kPa in CalculateRelativeHumidity)
		          float denom = current.temperature + 237.3f;
		          if (Mathf.Abs(denom) < 1e-6f)
				denom = 1e-6f;
		          float satVaporPressure_kPa = 0.6108f * Mathf.Exp((17.27f * current.temperature) / denom);
		          float e = satVaporPressure_kPa * 1000f; // convert kPa -> Pa

		          // Specific gas constants for dry air and water vapor (J/k.·K)
		          float Rsd = Constants.GAS_CONSTANT / Constants.MOLAR_MASS_AIR;
		          float Rsw = Constants.GAS_CONSTANT / Constants.MOLAR_MASS_WATER;

		          // Mixing ratio 
		          float epsilon = Rsd / Rsw; // equals ~0.622
		          float denom2 = p - e;
		          float r = 0f;
		          if (denom2 > 1e-6f)
				r = epsilon * e / denom2;
		          r = Mathf.Max(0f, r);

		          // Physical constants
		          float Hv = Constants.LATENT_HEAT_VAPORIZATION; // J/kg
		          float cpd = Constants.AIR_SPECIFIC_HEAT; // J/kg.K
		          float g = Constants.GRAVITY_ACCELERATION;

		          // Compute moist (saturated) adiabatic lapse rate (K/m). Result is positive magnitude (K/m).
		          float Gw = g * (1f + (Hv * r) / (Rsd * T)) / (cpd + (Hv * Hv * r) / (Rsw * T * T));

		          // Convert to °C/m with sign convention (temperature decreases with height -> negative)
		          float moistLapseRate = -Gw;

		          // Dry lapse rate is stored in Constants.DRY_LAPSE_RATE (already negative, °C/m)
		          float dryLapseRate = Constants.DRY_LAPSE_RATE;

		          // Blend between dry and moist lapse rates by relative humidity (simple approach)
		          float lapseRate = Mathf.Lerp(dryLapseRate, moistLapseRate, current.RelativeHumidity);

		          // Estimate vertical motion (m/s). Preserve previous simple logic.
		          float verticalMotion = current.windVelocity.y;
		          if (current.below != null)
				verticalMotion = current.below.windSpeed * 0.1f;

		          // Return temperature change using existing calling convention.

		          return verticalMotion * lapseRate * current.layerHeight;
	     }
	     //advection 

	     //convection

	     //pressure gradient force
	     //coriolis effect

	     /// <summary>
	     /// Calculates relative humidity for an atmosphere tile
	     /// </summary>
	     public static float CalculateRelativeHumidity(AtmosphereTile tile)
	     {
		          // Saturation vapor pressure (Tetens formula)
		          float denom = tile.temperature + 237.3f;
		          if (Mathf.Abs(denom) < 1e-6f)
				denom = 1e-6f;
		          float satVaporPressure = 0.6108f * Mathf.Exp((17.27f * tile.temperature) / denom);

		          // Actual vapor pressure
		          float vaporPressure = (tile.waterVapor * Constants.GAS_CONSTANT * (tile.temperature + 273.15f)) /
				(tile.layerHeight * Constants.MOLAR_MASS_WATER);

		          return Mathf.Clamp01(vaporPressure / satVaporPressure); // Return relative humidity as a fraction (0-1)
	     }

	     public static float CalculateDewPoint(float tempC, float relativeHumidity)
	     {
		          float a = 17.27f;// Constants for dew point calculation
		          float b = 237.7f;// Constants for dew point calculation
		          float safeRH = Mathf.Max(relativeHumidity, 0.0001f); // Avoid log(0)
		          float denom = b + tempC;
		          if (Mathf.Abs(denom) < 1e-6f)
				denom = 1e-6f; // Avoid division by zero
		          float alpha = ((a * tempC) / denom) + Mathf.Log(safeRH);
		          float resultDenom = a - alpha;
		          if (Mathf.Abs(resultDenom) < 1e-6f)
				resultDenom = 1e-6f;
		          return (b * alpha) / resultDenom; // Return dew point in Celsius
	     }

	     public static float CalculateSaturationVaporPressure(float temperature)
	     {
		          // Tetens formula for saturation vapor pressure in hPa
		          float t = temperature - 273.15f; // Convert to Celsius
		          return 6.11f * Mathf.Exp((17.27f * t) / (t + 237.3f)) * 100f; // Convert to Pa
	     }

}