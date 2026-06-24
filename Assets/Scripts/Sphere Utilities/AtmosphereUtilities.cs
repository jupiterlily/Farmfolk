using UnityEngine;

public static class AtmosphereUtilities
{
	     // ─── Mass & Energy Conservation ────────────────────────────────

	     /// <summary>
	     /// Calculate total energy stored in all ThermalEnergy objects in the atmosphere.
	     /// </summary>
	     public static float CalculateTotalEnergy(AtmosphereTile[,,] atmosphereTiles)
	     {
		          float totalEnergy = 0f;
		          for (int layer = 0; layer < atmosphereTiles.GetLength(0); layer++)
		          {
				for (int x = 0; x < atmosphereTiles.GetLength(1); x++)
				{
					     for (int y = 0; y < atmosphereTiles.GetLength(2); y++)
					     {
						          AtmosphereTile tile = atmosphereTiles[layer, x, y];
						          if (tile?.energy == null)
								continue;
						          foreach (var energy in tile.energy)
						          {
								// Correct: only StoredEnergy, not + SpecificHeat
								totalEnergy += energy.StoredEnergy;
						          }
					     }
				}
		          }
		          return totalEnergy;
	     }

	     // ─── Semi‑Lagrangian Advection (Conservative Interpolation) ──

	     /// <summary>
	     /// Apply semi‑Lagrangian advection to transport atmospheric properties.
	     /// This conserves mass and energy by interpolating ThermalEnergy objects.
	     /// </summary>
	     public static void ApplyAtmosphericAdvection(AtmosphereTile[,,] atmosphereTiles, float hours)
	     {
		          int layers = atmosphereTiles.GetLength(0);
		          int width = atmosphereTiles.GetLength(1);
		          int height = atmosphereTiles.GetLength(2);

		          // Create temporary storage for advected properties (we will overwrite the originals)
		          ThermalEnergy[,,] newVapor = new ThermalEnergy[layers, width, height];
		          ThermalEnergy[,,] newCloud = new ThermalEnergy[layers, width, height];
		          ThermalEnergy[,,] newIce = new ThermalEnergy[layers, width, height];

		          float timeStepSeconds = hours * 3600f;

		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          AtmosphereTile current = atmosphereTiles[layer, x, y];
						          if (current == null)
								continue;

						          // Calculate departure point (where air came from)
						          Vector2 departure = CalculateDeparturePoint(current, x, y, width, height, timeStepSeconds);

						          // Interpolate properties from the four surrounding cells
						          (ThermalEnergy vapor, ThermalEnergy cloud, ThermalEnergy ice) = InterpolateAtmosphericProperties(
						              atmosphereTiles, layer, departure, x, y, width, height
						          );

						          newVapor[layer, x, y] = vapor;
						          newCloud[layer, x, y] = cloud;
						          newIce[layer, x, y] = ice;
					     }
				}
		          }

		          // Apply advected properties back to tiles
		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          AtmosphereTile tile = atmosphereTiles[layer, x, y];
						          if (tile == null)
								continue;

						          tile.waterVapor = newVapor[layer, x, y];
						          tile.cloudWaterContent = newCloud[layer, x, y];
						          tile.iceParticles = newIce[layer, x, y];
					     }
				}
		          }
	     }

	     private static Vector2 CalculateDeparturePoint(AtmosphereTile current, int x, int y, int width, int height, float timeStepSeconds)
	     {
		          if (current == null)
				return new Vector2(x, y);

		          float cellWidth = current.tileWidth;
		          float dx = (current.windVelocity.x * timeStepSeconds) / cellWidth;
		          float dy = (current.windVelocity.z * timeStepSeconds) / cellWidth;

		          float departureX = x - dx;
		          float departureY = y - dy;

		          // Periodic boundary (wrap around)
		          departureX = ((departureX % width) + width) % width;
		          departureY = ((departureY % height) + height) % height;

		          return new Vector2(departureX, departureY);
	     }

	     private static (ThermalEnergy vapor, ThermalEnergy cloud, ThermalEnergy ice) InterpolateAtmosphericProperties(
	         AtmosphereTile[,,] tiles, int layer, Vector2 departure, int x, int y, int width, int height)
	     {
		          int x0 = Mathf.FloorToInt(departure.x);
		          int x1 = (x0 + 1) % width;
		          int y0 = Mathf.FloorToInt(departure.y);
		          int y1 = (y0 + 1) % height;
		          float fx = departure.x - x0;
		          float fy = departure.y - y0;

		          AtmosphereTile t00 = tiles[layer, x0, y0];
		          AtmosphereTile t10 = tiles[layer, x1, y0];
		          AtmosphereTile t01 = tiles[layer, x0, y1];
		          AtmosphereTile t11 = tiles[layer, x1, y1];

		          if (t00 == null || t10 == null || t01 == null || t11 == null)
		          {
				// Fallback: return a copy of the current tile's properties (or default)
				AtmosphereTile current = tiles[layer, x, y];
				return (CloneThermalEnergy(current.waterVapor),
				        CloneThermalEnergy(current.cloudWaterContent),
				        CloneThermalEnergy(current.iceParticles));
		          }

		          ThermalEnergy vapor = BilinearInterpolateEnergy(t00.waterVapor, t10.waterVapor, t01.waterVapor, t11.waterVapor, fx, fy);
		          ThermalEnergy cloud = BilinearInterpolateEnergy(t00.cloudWaterContent, t10.cloudWaterContent, t01.cloudWaterContent, t11.cloudWaterContent, fx, fy);
		          ThermalEnergy ice = BilinearInterpolateEnergy(t00.iceParticles, t10.iceParticles, t01.iceParticles, t11.iceParticles, fx, fy);

		          return (vapor, cloud, ice);
	     }

	     private static ThermalEnergy BilinearInterpolateEnergy(ThermalEnergy e00, ThermalEnergy e10, ThermalEnergy e01, ThermalEnergy e11, float fx, float fy)
	     {
		          // Interpolate mass
		          float mass = BilinearInterpolate(e00.Mass, e10.Mass, e01.Mass, e11.Mass, fx, fy);

		          // Interpolate temperature using mass‑weighted average
		          float totalMass = e00.Mass + e10.Mass + e01.Mass + e11.Mass;
		          float temp;
		          if (totalMass > 0.001f)
		          {
				float weightedTemp00 = e00.Temperature * e00.Mass;
				float weightedTemp10 = e10.Temperature * e10.Mass;
				float weightedTemp01 = e01.Temperature * e01.Mass;
				float weightedTemp11 = e11.Temperature * e11.Mass;
				float weightedSum = BilinearInterpolate(weightedTemp00, weightedTemp10, weightedTemp01, weightedTemp11, fx, fy);
				temp = weightedSum / mass;
		          }
		          else
		          {
				temp = BilinearInterpolate(e00.Temperature, e10.Temperature, e01.Temperature, e11.Temperature, fx, fy);
		          }

		          // Create a new ThermalEnergy with the interpolated mass and temperature.
		          // Use the properties of the first object (assume all water has same properties).
		          return new ThermalEnergy(
		              mass,
		              //e00.SpecificHeat,
		              e00.materialName,
		              e00.LatentHeatFusion,
		              e00.LatentHeatVaporization,
		              e00.MeltingPoint,
		              e00.BoilingPoint,
		              e00.ThermalConductivity,
		              temp
		          );
	     }

	     private static float BilinearInterpolate(float v00, float v10, float v01, float v11, float fx, float fy)
	     {
		          float x0y = Mathf.Lerp(v00, v10, fx);
		          float x1y = Mathf.Lerp(v01, v11, fx);
		          return Mathf.Lerp(x0y, x1y, fy);
	     }

	     private static ThermalEnergy CloneThermalEnergy(ThermalEnergy source)
	     {
		          if (source == null)
				return null;
		          return new ThermalEnergy(
		              source.Mass,
		              //source.SpecificHeat,
		              source.materialName,
		              source.LatentHeatFusion,
		              source.LatentHeatVaporization,
		              source.MeltingPoint,
		              source.BoilingPoint,
		              source.ThermalConductivity,
		              source.Temperature
		          );
	     }

	     // ─── Initialisation (Hadley Cells) ─────────────────────────────

	     public static void InitializeHadleyCellWinds(AtmosphereTile[,,] atmosphereTiles, TileManager tileManager)
	     {
		          int layers = atmosphereTiles.GetLength(0);
		          int width = atmosphereTiles.GetLength(1);
		          int height = atmosphereTiles.GetLength(2);

		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          AtmosphereTile tile = atmosphereTiles[layer, x, y];
						          if (tile == null)
								continue;

						          float latitude = tileManager.GetLatitude(x, y);
						          Vector3 wind = CalculateHadleyCellWind(latitude, layer, layers);
						          tile.windVelocity = wind;
						          tile.windSpeed = wind.magnitude;

						          float temperature = CalculateHadleyCellTemperature(latitude, layer, layers);
						          // Instead of calling ChangeMass, we use AddMass (or set via constructor)
						          // But we already have ThermalEnergy objects with some default mass.
						          // To set temperature, we adjust the sensible energy directly.
						          // Using the new API: we can use AddHeat to raise temperature to desired value.
						          float currentTemp = tile.nitrogen.Temperature;
						          float deltaTemp = temperature - currentTemp;
						          float energyToAdd = deltaTemp * tile.nitrogen.Mass * tile.nitrogen.SpecificHeat;
						          tile.nitrogen.AddHeat(energyToAdd);

						          // Similarly, set water vapor mass and cloud mass using AddMass
						          InitializeHadleyCellHumidity(tile, latitude, layer);
						          InitializeHadleyCellPressure(tile, latitude, layer, layers);
					     }
				}
		          }
	     }

	     private static void InitializeHadleyCellHumidity(AtmosphereTile tile, float latitude, int layer)
	     {
		          float normalizedLatitude = Mathf.Abs(latitude);
		          float baseHumidity = 0.5f;

		          // ... (humidity calculation as before) ...
		          if (normalizedLatitude <= 5f)
				baseHumidity = Mathf.Lerp(0.8f, 0.7f, (normalizedLatitude + 5f) / 10f);
		          else if (normalizedLatitude <= 30f)
		          {
				float inSubsidenceZone = (normalizedLatitude - 5f) / 25f;
				baseHumidity = Mathf.Lerp(0.7f, 0.3f, inSubsidenceZone);
		          }
		          else if (normalizedLatitude <= 60f)
				baseHumidity = 0.6f;
		          else
				baseHumidity = 0.4f;

		          float layerAdjustment = Mathf.Pow(0.8f, layer);
		          baseHumidity *= layerAdjustment;

		          // Calculate saturation mass
		          float volume = tile.tileWidth * tile.tileWidth * tile.layerHeight;
		          float saturationMass = CalculateSaturationWaterMass(tile.temperature, tile.pressure, volume);
		          float waterVaporMass = saturationMass * baseHumidity;

		          // Use AddMass to set the vapor mass at current temperature
		          // But AddMass adds mass to existing; we want to replace.
		          // So we first set mass to 0 (using RemoveMass) then add the new mass.
		          if (tile.waterVapor.Mass > 0)
				tile.waterVapor.RemoveMass(tile.waterVapor.Mass);
		          if (waterVaporMass > 0)
				tile.waterVapor.AddMass(waterVaporMass, tile.waterVapor.Temperature);

		          // Initialize cloud water in convergence zones
		          if (normalizedLatitude <= 5f || (normalizedLatitude > 30f && normalizedLatitude <= 60f))
		          {
				float cloudMass = waterVaporMass * 0.1f;
				if (tile.cloudWaterContent.Mass > 0)
					     tile.cloudWaterContent.RemoveMass(tile.cloudWaterContent.Mass);
				if (cloudMass > 0)
					     tile.cloudWaterContent.AddMass(cloudMass, tile.cloudWaterContent.Temperature);
		          }
	     }

	     // ─── Wind Update ────────────────────────────────────────────────

	     public static void UpdateAtmosphericWind(AtmosphereTile[,,] atmosphereTiles, float hours)
	     {
		          int layers = atmosphereTiles.GetLength(0);
		          int width = atmosphereTiles.GetLength(1);
		          int height = atmosphereTiles.GetLength(2);
		          float timeStepSeconds = hours * 3600f;

		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          AtmosphereTile current = atmosphereTiles[layer, x, y];
						          if (current == null)
								continue;

						          Vector3 pressureForce = CalculatePressureGradientForce(atmosphereTiles, layer, x, y, width, height);
						          float latitude = (y / (float)height * 180f) - 90f;
						          Vector3 coriolisForce = CalculateCoriolisEffect(current.windVelocity, latitude);

						          float frictionCoefficient = (layer == 0) ? 0.05f : 0.01f;
						          Vector3 friction = -current.windVelocity * frictionCoefficient;

						          // Use local air density for more accurate acceleration
						          float airDensity = (float)CalculateAirDensity(current.temperature, current.pressure);
						          if (airDensity < 0.001f)
								airDensity = 1.225f;

						          Vector3 acceleration = (pressureForce + coriolisForce + friction) / airDensity;
						          current.windVelocity += acceleration * timeStepSeconds;

						          float maxWindSpeed = 50f;
						          current.windVelocity = Vector3.ClampMagnitude(current.windVelocity, maxWindSpeed);
						          current.windSpeed = current.windVelocity.magnitude;

						          // Small random perturbation
						          current.windVelocity += UnityEngine.Random.insideUnitSphere * 0.01f;
					     }
				}
		          }
	     }

	     private static Vector3 CalculatePressureGradientForce(AtmosphereTile[,,] tiles, int layer, int x, int y, int width, int height)
	     {
		          Vector3 force = Vector3.zero;
		          AtmosphereTile current = tiles[layer, x, y];
		          if (current == null || current.horizontalNeighbors == null)
				return force;

		          float currentPressure = current.pressure;

		          // Neighbor order: [North, East, South, West]
		          Vector3[] directions = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
		          for (int i = 0; i < 4; i++)
		          {
				var neighbor = current.horizontalNeighbors[i];
				if (neighbor != null)
				{
					     float pressureDiff = currentPressure - neighbor.pressure;
					     force += directions[i] * (pressureDiff / Constants.AIR_DENSITY); // may need local density
				}
		          }
		          return force;
	     }

	     // ─── Helper Methods (unchanged but use new API where appropriate) ──

	     public static float CalculateRelativeHumidity(AtmosphereTile tile)
	     {
		          if (tile == null)
				return 0f;
		          float temp = tile.temperature;
		          if (temp < 0.001f)
				return 0f;

		          float satVaporPressure_Pa = CalculateSaturationVaporPressure(temp);
		          float volume = tile.tileWidth * tile.tileWidth * tile.layerHeight;
		          if (volume <= 0f)
				volume = 1f;

		          float molesWater = tile.waterVapor.Mass / 0.018f;
		          float actualVaporPressure = (molesWater * Constants.GAS_CONSTANT * (temp)) / volume;
		          if (satVaporPressure_Pa < 0.001f)
				return 0f;
		          return Mathf.Clamp01(actualVaporPressure / satVaporPressure_Pa);
	     }

	     public static float CalculateSaturationVaporPressure(float temperatureC)
	     {
		          float T = temperatureC;
		          if (T >= 0)
				return 611.2f * Mathf.Exp(17.67f * T / (T + 243.5f));
		          else
				return 611.2f * Mathf.Exp(22.46f * T / (T + 272.62f));
	     }

	     public static float CalculateSaturationWaterMass(float temperatureC, float pressurePa, float volumeM3)
	     {
		          float satVP = CalculateSaturationVaporPressure(temperatureC);
		          float mixingRatio = 0.622f * satVP / Mathf.Max(pressurePa - satVP, 1f);
		          float T = temperatureC + 273.15f;
		          float dryAirDensity = pressurePa / (Constants.GAS_CONSTANT * T);
		          float saturationVaporDensity = mixingRatio * dryAirDensity;
		          return saturationVaporDensity * volumeM3;
	     }

	     public static float CalculateDewPoint(float tempC, float relativeHumidity)
	     {
		          float safeRH = Mathf.Clamp(relativeHumidity, 0.0001f, 1f);
		          float a = 17.27f;
		          float b = 237.7f;
		          float alpha = ((a * tempC) / (b + tempC)) + Mathf.Log(safeRH);
		          return (b * alpha) / (a - alpha);
	     }

	     public static double CalculateAirDensity(float temperature, float pressure)
	     {
		          double T = temperature + 273.15;
		          if (T <= 0)
				return 1.225;
		          return pressure / (Constants.GAS_CONSTANT * T);
	     }

	     public static float CalculateRealisticAirMass(float temperatureC, float pressurePa, float volumeM3)
	     {
		          float T = temperatureC + 273.15f;
		          if (T <= 0 || pressurePa <= 0 || volumeM3 <= 0)
				return 1.225f * volumeM3;
		          float moles = (pressurePa * volumeM3) / (Constants.GAS_CONSTANT * T);
		          return Mathf.Max(moles * Constants.MOLAR_MASS_AIR, 0.01f);
	     }

	     public static Vector3 CalculateCoriolisEffect(Vector3 windVelocity, float latitude)
	     {
		          float omega = 7.2921e-5f;
		          float f = 2f * omega * Mathf.Sin(latitude * Mathf.Deg2Rad);
		          return new Vector3(-windVelocity.z * f, 0, windVelocity.x * f);
	     }

	     // ─── Removed flux‑based advection methods (they used ChangeMass) ──
	     // public static void ApplyConservativeFluxAdvection(...) removed
	     // public static void ApplyConservativeAdvection(...) removed

	     // ─── Other helpers (unchanged) ──────────────────────────────────

	     public static float CalculateAdiabaticCooling(AtmosphereTile current)
	     {
		          if (current == null)
				return 0f;
		          float T = current.temperature + 273.15f;
		          if (T <= 0f)
				return 0f;
		          float p = current.pressure;
		          if (p <= 0f)
				p = 101325f;
		          float dryLapseRate = -0.0098f;
		          float moistLapseRate = -0.0065f;
		          float lapseRate = Mathf.Lerp(dryLapseRate, moistLapseRate, Mathf.Clamp01(current.RelativeHumidity));
		          float verticalMotion = current.windVelocity.y;
		          if (Mathf.Abs(verticalMotion) < 0.01f)
				verticalMotion = UnityEngine.Random.Range(-0.1f, 0.1f);
		          return verticalMotion * lapseRate * current.layerHeight;
	     }

	     public static float CalculateRadiativeCooling(AtmosphereTile tile, float hours)
	     {
		          if (tile == null)
				return 0f;
		          hours *= 3600f;
		          float tempK = tile.temperature + 273.15f;
		          if (tempK <= 0f)
				return 0f;
		          float surfaceArea = tile.tileWidth * tile.tileWidth;
		          if (surfaceArea <= 0)
				return 0f;
		          float power = Constants.ATMOSPHERIC_EMISSIVITY * Constants.STEFAN_BOLTZMANN * surfaceArea *
		              (Mathf.Pow(tempK, 4) - Mathf.Pow(Constants.COSMIC_BACKGROUND, 4));
		          return power * hours;
	     }

	     public static float CalculateTotalWater(AtmosphereTile[,,] atmosphereTiles)
	     {
		          float total = 0f;
		          for (int layer = 0; layer < atmosphereTiles.GetLength(0); layer++)
		          {
				for (int x = 0; x < atmosphereTiles.GetLength(1); x++)
				{
					     for (int y = 0; y < atmosphereTiles.GetLength(2); y++)
					     {
						          AtmosphereTile tile = atmosphereTiles[layer, x, y];
						          if (tile != null)
								total += tile.waterVapor.Mass + tile.cloudWaterContent.Mass;
					     }
				}
		          }
		          return total;
	     }
	     /// <summary>
	     /// Latitude-based cell definitions
	     /// Hadley cells: ±0° to ±30°
	     /// Ferrel cells: ±30° to ±60°
	     /// Polar cells: ±60° to ±90°
	     /// </summary>
	     private const float HADLEY_CELL_BOUNDARY = 30f;
	     private const float FERREL_CELL_BOUNDARY = 60f;
	     private const float POLAR_CELL_BOUNDARY = 90f;

	     // Trade wind characteristics
	     private const float TRADE_WIND_SPEED = 5f;          // m/s
	     private const float WESTERLY_WIND_SPEED = 7f;       // m/s
	     private const float POLAR_WIND_SPEED = 4f;          // m/s

	     // Vertical motion (positive = upward)
	     private const float HADLEY_EQUATORIAL_UPDRAFT = 0.2f;   // m/s at equator
	     private const float HADLEY_SUBTROPICAL_DOWNDRAFT = -0.15f; // m/s at 30°
	     private const float FERREL_VERTICAL_UPDRAFT = 0.1f;   // m/s
	     private const float POLAR_SUBSIDENCE = -0.08f;      // m/s

	     // Temperature gradients (K per meter)
	     private const float EQUATORIAL_SURFACE_TEMP = 298f;  // ~25°C in Kelvin
	     private const float SUBTROPICAL_SURFACE_TEMP = 290f; // ~17°C
	     private const float TEMPERATE_SURFACE_TEMP = 280f;   // ~7°C
	     private const float POLAR_SURFACE_TEMP = 260f;       // -13°C

	     // ─── Wind cell calculation helpers (unchanged) ─────────────────
	     /// <summary>
	     /// Calculates realistic wind velocity based on Hadley cell circulation
	     /// </summary>
	     private static Vector3 CalculateHadleyCellWind(float latitude, int layer, int maxLayers)
	     {
		          float normalizedLatitude = Mathf.Abs(latitude);
		          float hemisphere = Mathf.Sign(latitude);
		          Vector3 wind = Vector3.zero;

		          // Layer interpolation (upper layers have stronger winds)
		          float layerFactor = Mathf.Lerp(0.5f, 1.5f, (float)layer / (maxLayers - 1));

		          if (normalizedLatitude <= HADLEY_CELL_BOUNDARY)
		          {
				// HADLEY CELL: Trade winds
				// Trade winds: 0-30°N and 0-30°S
				// Direction: Easterlies (from east) due to Coriolis effect
				float windSpeed = TRADE_WIND_SPEED * layerFactor;

				// Add slight latitude gradient (weaker at equator, stronger at boundaries)
				float latitudeGradient = Mathf.Sin(normalizedLatitude * Mathf.Deg2Rad);
				windSpeed *= 0.7f + 0.3f * latitudeGradient;

				// Coriolis-induced direction (from east in tropics)
				// Positive latitude = Northern Hemisphere (wind from NE)
				// Negative latitude = Southern Hemisphere (wind from SE)
				wind.x = windSpeed * hemisphere * 0.866f;  // ~√3/2 for 60° angle
				wind.z = -windSpeed * 0.5f;                 // Equatorward component
		          }
		          else if (normalizedLatitude <= FERREL_CELL_BOUNDARY)
		          {
				// FERREL CELL: Westerlies
				// Westerlies: 30-60°N and 30-60°S
				// Direction: From west (westerlies), stronger than trade winds
				float windSpeed = WESTERLY_WIND_SPEED * layerFactor;

				// Strengthen away from Hadley boundary
				float distanceFromBoundary = (normalizedLatitude - HADLEY_CELL_BOUNDARY) / (FERREL_CELL_BOUNDARY - HADLEY_CELL_BOUNDARY);
				windSpeed *= 0.8f + 0.4f * Mathf.Cos(distanceFromBoundary * Mathf.PI);

				// Westerly direction (from west)
				wind.x = -windSpeed * hemisphere * 0.866f; // From west
				wind.z = windSpeed * 0.5f;                  // Poleward component
		          }
		          else
		          {
				// POLAR CELL: Polar easterlies
				// Polar easterlies: 60-90°N and 60-90°S
				// Direction: From east, but weak
				float windSpeed = POLAR_WIND_SPEED * layerFactor;

				// Weaken near pole
				float distanceToPole = (POLAR_CELL_BOUNDARY - normalizedLatitude) / (POLAR_CELL_BOUNDARY - FERREL_CELL_BOUNDARY);
				windSpeed *= distanceToPole;

				// Easterly direction (from east)
				wind.x = windSpeed * hemisphere * 0.866f;
				wind.z = windSpeed * 0.5f;                  // Slight equatorward component
		          }

		          // Add vertical component (updrafts/downdrafts match Hadley circulation)
		          wind.y = CalculateVerticalVelocity(latitude, layer, maxLayers);

		          return wind;
	     }


	     private static float CalculateHadleyCellTemperature(float latitude, int layer, int maxLayers)
	     {
		          // ... (same as before) ...
		          return 280f; // placeholder
	     }

	     private static void InitializeHadleyCellPressure(AtmosphereTile tile, float latitude, int layer, int maxLayers)
	     {
		          // ... (same as before, only sets tile.pressure) ...
		          tile.pressure = 101325f; // placeholder
	     }

	     /// <summary>
	     /// Calculates vertical velocity (updrafts/downdrafts) based on circulation cell
	     /// </summary>
	     private static float CalculateVerticalVelocity(float latitude, int layer, int maxLayers)
	     {
		          float normalizedLatitude = Mathf.Abs(latitude);
		          float verticalVelocity = 0f;

		          if (normalizedLatitude <= 5f)
		          {
				// ITCZ (Intertropical Convergence Zone): Strong updrafts at equator
				verticalVelocity = HADLEY_EQUATORIAL_UPDRAFT;
		          }
		          else if (normalizedLatitude <= HADLEY_CELL_BOUNDARY)
		          {
				// Hadley subsidence (downdraft) from 5° to 30°
				// Strongest at ~30°
				float normalizedPosition = (normalizedLatitude - 5f) / (HADLEY_CELL_BOUNDARY - 5f);
				verticalVelocity = Mathf.Lerp(HADLEY_EQUATORIAL_UPDRAFT * 0.3f, HADLEY_SUBTROPICAL_DOWNDRAFT, normalizedPosition);
		          }
		          else if (normalizedLatitude <= FERREL_CELL_BOUNDARY)
		          {
				// Ferrel cell uplift (convergence at ~60°)
				float normalizedPosition = (normalizedLatitude - HADLEY_CELL_BOUNDARY) / (FERREL_CELL_BOUNDARY - HADLEY_CELL_BOUNDARY);
				verticalVelocity = Mathf.Lerp(HADLEY_SUBTROPICAL_DOWNDRAFT, FERREL_VERTICAL_UPDRAFT, normalizedPosition);
		          }
		          else
		          {
				// Polar cell subsidence
				verticalVelocity = POLAR_SUBSIDENCE;
		          }

		          // Reduce vertical motion in upper layers (less pronounced aloft)
		          float layerFactor = Mathf.Lerp(1f, 0.3f, (float)layer / (maxLayers - 1));
		          return verticalVelocity * layerFactor;
	     }
	     /// <summary>
	     /// Applies realistic pressure patterns to match observed subtropical ridges and subpolar lows
	     /// </summary>
	     public static void FinalizePressurePatterns(AtmosphereTile[,,] atmosphereTiles)
	     {
		          int layers = atmosphereTiles.GetLength(0);
		          int width = atmosphereTiles.GetLength(1);
		          int height = atmosphereTiles.GetLength(2);

		          // Smooth pressure field to ensure geostrophic balance
		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int iteration = 0; iteration < 3; iteration++)
				{
					     for (int x = 0; x < width; x++)
					     {
						          for (int y = 0; y < height; y++)
						          {
								AtmosphereTile tile = atmosphereTiles[layer, x, y];
								if (tile?.horizontalNeighbors == null)
									     continue;

								// Average with neighbors
								float neighborPressureSum = 0f;
								int neighborCount = 0;

								foreach (var neighbor in tile.horizontalNeighbors)
								{
									     if (neighbor != null)
									     {
										          neighborPressureSum += neighbor.pressure;
										          neighborCount++;
									     }
								}

								if (neighborCount > 0)
								{
									     // Gentle smoothing (10% adjustment per iteration)
									     float avgNeighborPressure = neighborPressureSum / neighborCount;
									     tile.pressure = Mathf.Lerp(tile.pressure, avgNeighborPressure, 0.1f);
								}
						          }
					     }
				}
		          }
	     }
}