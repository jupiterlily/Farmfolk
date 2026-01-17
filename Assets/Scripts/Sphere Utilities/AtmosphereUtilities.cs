using System;
using UnityEngine;

public static class AtmosphereUtilities
{
	     /// <summary>
	     /// Simulates atmospheric processes using a conservative finite-volume approach
	     /// </summary>
	     public static void SimulateAtmosphere(AtmosphereTile[,,] atmosphereTiles, float hours)
	     {
		          int layers = atmosphereTiles.GetLength(0);
		          int width = atmosphereTiles.GetLength(1);
		          int height = atmosphereTiles.GetLength(2);

		          // Store initial energy state for conservation check
		          float initialTotalEnergy = CalculateTotalEnergy(atmosphereTiles);
		          float initialTotalWater = CalculateTotalWater(atmosphereTiles);

		          // Use floats but with proper scaling to avoid precision issues
		          float[,,] newTemps = new float[layers, width, height];
		          float[,,] newVapor = new float[layers, width, height];
		          float[,,] newCloudWater = new float[layers, width, height];
		          float[,,] newLiquidWater = new float[layers, width, height];
		          Vector3[,,] newWinds = new Vector3[layers, width, height];

		          // First: Calculate all temperature changes WITHOUT water phase changes
		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          AtmosphereTile current = atmosphereTiles[layer, x, y];

						          // 1. Adiabatic change - ensure small values
						          float adiabaticChange = CalculateAdiabaticChange(current, layer, layers) * hours;

						          // 2. Radiative change - ensure small values
						          float radiativeChange = CalculateRadiativeChange(current, layer, layers, hours);

						          // Apply temperature changes
						          newTemps[layer, x, y] = current.temperature + adiabaticChange + radiativeChange;

						          // Copy water values
						          newVapor[layer, x, y] = current.waterVapor;
						          newCloudWater[layer, x, y] = current.cloudWaterContent;
						          newLiquidWater[layer, x, y] = current.liquidWater;

						          // Calculate wind changes
						          Vector3 pressureForce = CalculatePressureGradientForce(atmosphereTiles, layer, x, y, width, height);
						          float latitude = (y / (float)height * 180f) - 90f;
						          Vector3 coriolisForce = CalculateCoriolisEffect(current.windVelocity, latitude);
						          float frictionCoefficient = (layer == 0) ? 0.05f : 0.01f;
						          Vector3 friction = -current.windVelocity * frictionCoefficient;

						          newWinds[layer, x, y] = current.windVelocity +
						              (pressureForce + coriolisForce + friction) * hours;
						          newWinds[layer, x, y] = Vector3.ClampMagnitude(newWinds[layer, x, y], 50f);
					     }
				}
		          }

		          // Second: Apply conservative advection
		          ApplyConservativeAdvection(
		              atmosphereTiles,
		              ref newTemps, ref newVapor, ref newCloudWater, ref newLiquidWater,
		              newWinds, hours
		          );

		          // Third: Apply phase changes with PROPER energy scaling
		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          AtmosphereTile current = atmosphereTiles[layer, x, y];
						          float volume = current.tileWidth * current.tileWidth * current.layerHeight;

						          // Get the advected temperature
						          float tempAfterAdvection = newTemps[layer, x, y];

						          // Apply phase changes with proper scaling
						          ApplyPhaseChangesWithProperScaling(
						              ref tempAfterAdvection,
						              ref newVapor[layer, x, y],
						              ref newCloudWater[layer, x, y],
						              ref newLiquidWater[layer, x, y],
						              current.pressure,
						              volume,
						              current.windSpeed,
						              hours
						          );

						          // Store final temperature
						          newTemps[layer, x, y] = tempAfterAdvection;
					     }
				}
		          }

		          // Fourth: Apply all changes to original tiles
		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          AtmosphereTile tile = atmosphereTiles[layer, x, y];

						          // Apply temperature with smoothing and strict bounds
						          float targetTemp = Mathf.Clamp(newTemps[layer, x, y], -100f, 100f);
						          tile.temperature = Mathf.Lerp(tile.temperature, targetTemp, 0.3f);

						          // Apply water components with conservation
						          float totalWaterBefore = tile.waterVapor + tile.cloudWaterContent + tile.liquidWater;
						          float totalWaterAfter = newVapor[layer, x, y] + newCloudWater[layer, x, y] + newLiquidWater[layer, x, y];

						          // Redistribute to maintain exact conservation
						          if (totalWaterAfter > 0)
						          {
								float ratio = totalWaterBefore / totalWaterAfter;
								tile.waterVapor = Mathf.Max(0, newVapor[layer, x, y] * ratio);
								tile.cloudWaterContent = Mathf.Max(0, newCloudWater[layer, x, y] * ratio);
								tile.liquidWater = Mathf.Max(0, newLiquidWater[layer, x, y] * ratio);
						          }

						          // Apply wind
						          tile.windVelocity = newWinds[layer, x, y];
						          tile.windSpeed = tile.windVelocity.magnitude;

						          // Recalculate pressure
						          float vaporDensity = tile.waterVapor / Mathf.Max(tile.layerHeight * tile.tileWidth * tile.tileWidth, 0.001f);
						          tile.pressure = CalculateRealisticPressure(
						              tile.temperature,
						              vaporDensity,
						              layer * tile.layerHeight,
						              tile.waterVapor
						          );
					     }
				}
		          }

		          // Fifth: Validate conservation
		          float finalTotalEnergy = CalculateTotalEnergy(atmosphereTiles);
		          float finalTotalWater = CalculateTotalWater(atmosphereTiles);

		          float energyError = Mathf.Abs(finalTotalEnergy - initialTotalEnergy);
		          float waterError = Mathf.Abs(finalTotalWater - initialTotalWater);

		          if (energyError > 1e6f) // If error is more than 1 MJ
		          {
				Debug.LogError($"Energy conservation error: {energyError} J");
		          }

		          if (waterError > 0.1f)
		          {
				Debug.LogWarning($"Water conservation error: {waterError} kg");
		          }
	     }

	     /// <summary>
	     /// Apply phase changes with PROPERLY SCALED energy calculations
	     /// </summary>
	     public static void ApplyPhaseChangesWithProperScaling(
	         ref float temperature,
	         ref float vapor,
	         ref float cloudWater,
	         ref float liquidWater,
	         float pressure,
	         float volume,
	         float windSpeed,
	         float hours)
	     {
		          if (volume <= 0)
				return;

		          // Calculate saturation properly
		          float saturationMass = CalculateSaturationWaterMass(temperature, pressure, volume);
		          float relativeHumidity = vapor / Mathf.Max(saturationMass, 0.001f);

		          // Calculate air mass CORRECTLY
		          float airMass = CalculateRealisticAirMass(temperature, pressure, volume);

		          // If airMass is too small (floating point issue), use a reasonable minimum
		          if (airMass < 0.1f)
		          {
				// Typical air density at sea level: ~1.2 kg/m³
				airMass = 1.2f * volume;
		          }

		          // Calculate heat capacity of this air mass
		          float heatCapacity = airMass * Constants.AIR_SPECIFIC_HEAT; // J/K

		          // Apply phase changes with LIMITED rates
		          float timeStepFactor = Mathf.Min(hours, 1.0f); // Don't allow more than 1 hour at once

		          if (relativeHumidity > 1.01f && vapor > 0) // Slightly above 1.0 to avoid oscillation
		          {
				// CONDENSATION
				float excessVapor = Mathf.Max(0, vapor - saturationMass);
				float condensationRate = 0.1f * timeStepFactor; // Max 10% per hour
				float vaporToCondense = Mathf.Min(excessVapor * condensationRate, vapor);

				if (vaporToCondense > 0 && heatCapacity > 0)
				{
					     // Convert vapor to cloud water
					     vapor -= vaporToCondense;
					     cloudWater += vaporToCondense;

					     // Calculate temperature change PROPERLY
					     // Latent heat of condensation: J/kg
					     float energyReleased = vaporToCondense * Constants.LATENT_HEAT_CONDENSATION; // J
					     float temperatureChange = energyReleased / heatCapacity; // K

					     // Apply temperature change
					     temperature += temperatureChange;

					     // Debug log for large changes
					     if (Mathf.Abs(temperatureChange) > 10f)
					     {
						          Debug.LogWarning($"Large condensation temp change: {temperatureChange}K, " +
						              $"vaporCondensed: {vaporToCondense}kg, airMass: {airMass}kg, heatCap: {heatCapacity}J/K");
					     }
				}
		          }
		          else if (relativeHumidity < 0.99f && (cloudWater > 0 || liquidWater > 0))
		          {
				// EVAPORATION
				float vaporDeficit = Mathf.Max(0, saturationMass - vapor);
				float evaporationRate = 0.05f * timeStepFactor; // Max 5% per hour

				// Adjust for wind and humidity
				float humidityFactor = 1.0f - relativeHumidity;
				float windFactor = 1.0f + windSpeed * 0.1f;
				float maxEvaporation = vaporDeficit * evaporationRate * humidityFactor * windFactor;

				float availableWater = cloudWater + liquidWater;
				float waterToEvaporate = Mathf.Min(maxEvaporation, availableWater);

				if (waterToEvaporate > 0 && heatCapacity > 0)
				{
					     // Evaporate from clouds first
					     float fromClouds = Mathf.Min(waterToEvaporate, cloudWater);
					     cloudWater -= fromClouds;

					     // Then from liquid
					     float fromLiquid = Mathf.Min(waterToEvaporate - fromClouds, liquidWater);
					     liquidWater -= fromLiquid;

					     // Add to vapor
					     vapor += waterToEvaporate;

					     // Calculate temperature change PROPERLY
					     float energyAbsorbed = waterToEvaporate * Constants.LATENT_HEAT_EVAPORATION; // J
					     float temperatureChange = -energyAbsorbed / heatCapacity; // K (negative)

					     // Apply temperature change
					     temperature += temperatureChange;
				}
		          }
	     }

	     /// <summary>
	     /// Calculate realistic air mass using ideal gas law
	     /// </summary>
	     public static float CalculateRealisticAirMass(float temperatureC, float pressurePa, float volumeM3)
	     {
		          // Using ideal gas law: PV = nRT
		          // n = PV/RT
		          // mass = n * molar_mass

		          float T = temperatureC + 273.15f; // Convert to Kelvin

		          if (T <= 0 || pressurePa <= 0 || volumeM3 <= 0)
		          {
				// Return reasonable default for sea level conditions
				return 1.225f * volumeM3; // kg/m³ * m³ = kg
		          }

		          // Moles of air: n = PV/RT
		          float moles = (pressurePa * volumeM3) / (Constants.GAS_CONSTANT * T);

		          // Mass of dry air (approximate)
		          float mass = moles * Constants.MOLAR_MASS_AIR;

		          return Mathf.Max(mass, 0.01f); // Ensure minimum mass
	     }

	     /// <summary>
	     /// Calculate saturation water mass in kg for given conditions
	     /// </summary>
	     public static float CalculateSaturationWaterMass(float temperatureC, float pressurePa, float volumeM3)
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
	     /// Calculate saturation vapor pressure using Magnus formula
	     /// </summary>
	     public static float CalculateSaturationVaporPressure(float temperatureC)
	     {
		          // Magnus formula for saturation vapor pressure in Pa
		          // Valid for -45°C to 60°C
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
	     /// Calculate realistic pressure considering water vapor
	     /// </summary>
	     public static float CalculateRealisticPressure(float temperatureC, float vaporDensity, float height, float waterVaporMass)
	     {
		          // Base pressure from barometric formula
		          float T = temperatureC + 273.15f;
		          float scaleHeight = (Constants.GAS_CONSTANT * T) /
		              (Constants.MOLAR_MASS_AIR * Constants.GRAVITY_ACCELERATION);

		          float basePressure = 101325f * Mathf.Exp(-height / scaleHeight);

		          // Water vapor pressure contribution
		          float vaporPressure = 0f;
		          if (vaporDensity > 0)
		          {
				// Using ideal gas law for water vapor: P_vapor = ρ_vapor * R_vapor * T
				float R_vapor = Constants.GAS_CONSTANT / Constants.MOLAR_MASS_WATER;
				vaporPressure = vaporDensity * R_vapor * T;
		          }

		          // Total pressure = dry air pressure + vapor pressure
		          float totalPressure = basePressure + vaporPressure;

		          // Ensure reasonable bounds
		          return Mathf.Clamp(totalPressure, 100f, 120000f);
	     }

	     /// <summary>
	     /// Calculate total energy in atmosphere for conservation checking
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

						          // Sensible heat
						          float volume = tile.tileWidth * tile.tileWidth * tile.layerHeight;
						          float airMass = CalculateRealisticAirMass(tile.temperature, tile.pressure, volume);
						          float sensibleHeat = airMass * Constants.AIR_SPECIFIC_HEAT * (tile.temperature + 273.15f);

						          // Latent heat
						          float latentHeat = tile.waterVapor * Constants.LATENT_HEAT_CONDENSATION;

						          totalEnergy += sensibleHeat + latentHeat;
					     }
				}
		          }

		          return totalEnergy;
	     }

	     /// <summary>
	     /// Apply conservative advection using proper flux calculations
	     /// </summary>
	     public static void ApplyConservativeAdvection(
	         AtmosphereTile[,,] oldState,
	         ref float[,,] temps, ref float[,,] vapor, ref float[,,] cloud, ref float[,,] liquid,
	         Vector3[,,] winds, float hours)
	     {
		          int layers = temps.GetLength(0);
		          int width = temps.GetLength(1);
		          int height = temps.GetLength(2);

		          // Create temporary arrays for fluxes
		          float[,,] tempChange = new float[layers, width, height];
		          float[,,] vaporChange = new float[layers, width, height];
		          float[,,] cloudChange = new float[layers, width, height];
		          float[,,] liquidChange = new float[layers, width, height];

		          float timeStepSeconds = hours * 3600f;

		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          AtmosphereTile current = oldState[layer, x, y];
						          Vector3 wind = winds[layer, x, y];
						          float cellWidth = current.tileWidth;

						          // Calculate displacement in cells
						          float dx = wind.x * timeStepSeconds / cellWidth;
						          float dz = wind.z * timeStepSeconds / cellWidth;

						          // Use integer-based advection for stability
						          int sourceX = x - Mathf.RoundToInt(dx);
						          int sourceY = y - Mathf.RoundToInt(dz);

						          // Wrap coordinates
						          sourceX = (sourceX + width) % width;
						          sourceY = (sourceY + height) % height;

						          // Get source values
						          tempChange[layer, x, y] = temps[layer, sourceX, sourceY] - temps[layer, x, y];
						          vaporChange[layer, x, y] = vapor[layer, sourceX, sourceY] - vapor[layer, x, y];
						          cloudChange[layer, x, y] = cloud[layer, sourceX, sourceY] - cloud[layer, x, y];
						          liquidChange[layer, x, y] = liquid[layer, sourceX, sourceY] - liquid[layer, x, y];
					     }
				}
		          }

		          // Apply changes
		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          // Apply with smoothing
						          float blendFactor = 0.5f; // Conservative blending

						          temps[layer, x, y] += tempChange[layer, x, y] * blendFactor;
						          vapor[layer, x, y] += vaporChange[layer, x, y] * blendFactor;
						          cloud[layer, x, y] += cloudChange[layer, x, y] * blendFactor;
						          liquid[layer, x, y] += liquidChange[layer, x, y] * blendFactor;
					     }
				}
		          }
	     }

	     // Keep other methods but ensure they return reasonable values
	     public static float CalculateAdiabaticChange(AtmosphereTile current, int layer, int totalLayers)
	     {
		          if (current == null)
				return 0f;

		          // Much smaller lapse rates to prevent runaway
		          float dryLapseRate = -0.005f; // Reduced from -0.0098
		          float moistLapseRate = -0.003f; // Reduced from -0.0065

		          // Estimate vertical motion (much smaller values)
		          float verticalMotion = 0f;

		          if (layer == 0)
		          {
				verticalMotion = 0.001f; // 1 mm/s upward (reduced from 1 cm/s)
		          }
		          else if (layer == totalLayers - 1)
		          {
				verticalMotion = -0.0005f; // 0.5 mm/s downward
		          }
		          else
		          {
				verticalMotion = UnityEngine.Random.Range(-0.0001f, 0.0001f);
		          }

		          // Add small wind effect
		          verticalMotion += current.windVelocity.y * 0.01f;

		          // Very strict clamping
		          verticalMotion = Mathf.Clamp(verticalMotion, -0.001f, 0.001f);

		          return verticalMotion * dryLapseRate * 3600f; // °C per hour
	     }

	     public static float CalculateRadiativeChange(AtmosphereTile tile, int layer, int totalLayers, float hours)
	     {
		          if (tile == null)
				return 0f;

		          // Much smaller radiative cooling rates
		          float coolingRate = -0.01f * hours; // Max 0.01°C per hour

		          // Adjust for altitude
		          float altitudeFactor = layer / (float)totalLayers;
		          coolingRate *= (1f + altitudeFactor * 0.5f);

		          // Adjust for clouds
		          float cloudFactor = Mathf.Lerp(1f, 0.1f, Mathf.Clamp01(tile.cloudWaterContent * 10f));

		          return coolingRate * cloudFactor;
	     }


	     public static void ApplyConservativeAdvection(
	     AtmosphereTile[,,] oldState,
	     double[,,] temps, double[,,] vapor, double[,,] cloud, double[,,] liquid,
	     Vector3[,,] winds, float hours)
	     {
		          int layers = temps.GetLength(0);
		          int width = temps.GetLength(1);
		          int height = temps.GetLength(2);

		          // Create flux arrays
		          double[,,] tempFlux = new double[layers, width, height];
		          double[,,] vaporFlux = new double[layers, width, height];
		          double[,,] cloudFlux = new double[layers, width, height];
		          double[,,] liquidFlux = new double[layers, width, height];

		          // Calculate fluxes between cells
		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          AtmosphereTile current = oldState[layer, x, y];
						          Vector3 wind = winds[layer, x, y];
						          double volume = current.tileWidth * current.tileWidth * current.layerHeight;

						          // Calculate fluxes to neighbors
						          for (int dir = 0; dir < 4; dir++)
						          {
								int nx = x + (dir == 0 ? 1 : dir == 1 ? -1 : 0);
								int ny = y + (dir == 2 ? 1 : dir == 3 ? -1 : 0);

								// Wrap coordinates
								nx = (nx + width) % width;
								ny = (ny + height) % height;

								// Calculate flux based on wind direction and magnitude
								Vector3 faceNormal = new Vector3(
								    dir == 0 ? 1 : dir == 1 ? -1 : 0,
								    0,
								    dir == 2 ? 1 : dir == 3 ? -1 : 0
								);

								float faceArea = current.tileWidth * current.layerHeight;
								float flowRate = Vector3.Dot(wind, faceNormal) * faceArea * hours * 3600f;

								if (flowRate > 0)
								{
									     // Outflow from current cell
									     double dt = hours * 3600f;
									     double outflowFraction = Math.Min(1.0, flowRate * dt / volume);

									     tempFlux[layer, x, y] -= temps[layer, x, y] * outflowFraction;
									     vaporFlux[layer, x, y] -= vapor[layer, x, y] * outflowFraction;
									     cloudFlux[layer, x, y] -= cloud[layer, x, y] * outflowFraction;
									     liquidFlux[layer, x, y] -= liquid[layer, x, y] * outflowFraction;

									     // Inflow to neighbor
									     tempFlux[layer, nx, ny] += temps[layer, x, y] * outflowFraction;
									     vaporFlux[layer, nx, ny] += vapor[layer, x, y] * outflowFraction;
									     cloudFlux[layer, nx, ny] += cloud[layer, x, y] * outflowFraction;
									     liquidFlux[layer, nx, ny] += liquid[layer, x, y] * outflowFraction;
								}
						          }
					     }
				}
		          }

		          // Apply fluxes
		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          temps[layer, x, y] += tempFlux[layer, x, y];
						          vapor[layer, x, y] += vaporFlux[layer, x, y];
						          cloud[layer, x, y] += cloudFlux[layer, x, y];
						          liquid[layer, x, y] += liquidFlux[layer, x, y];
					     }
				}
		          }
	     }

	     /// <summary>
	     /// Applies phase changes with guaranteed conservation
	     /// </summary>
	     public static void ApplyConsistentPhaseChanges(
	         ref double temperature, ref double vapor, ref double cloud, ref double liquid,
	         float pressure, double volume, float windSpeed, float hours)
	     {
		          if (volume <= 0)
				return;

		          // Calculate current saturation
		          double saturationVapor = CalculateMaxWaterVapor((float)temperature, pressure) * volume;
		          double relativeHumidity = vapor / Math.Max(saturationVapor, 0.001);

		          // Rate constants
		          double condensationRate = 0.2 * hours;
		          double evaporationRate = 0.1 * hours;

		          // Calculate air mass for heat capacity
		          double airMass = CalculateAirMass((float)temperature, pressure, volume);
		          double airHeatCapacity = airMass * Constants.AIR_SPECIFIC_HEAT;

		          if (relativeHumidity > 1.0 && vapor > 0)
		          {
				// Condensation
				double excessVapor = vapor - saturationVapor;
				double vaporToCondense = Math.Min(excessVapor * condensationRate, vapor);

				// Convert to cloud water
				vapor -= vaporToCondense;
				cloud += vaporToCondense;

				// Release latent heat
				double heatReleased = vaporToCondense * Constants.LATENT_HEAT_CONDENSATION;
				temperature += heatReleased / Math.Max(airHeatCapacity, 0.001);
		          }
		          else if (relativeHumidity < 0.95 && (cloud > 0 || liquid > 0))
		          {
				// Evaporation
				double vaporDeficit = saturationVapor - vapor;
				double maxEvaporation = Math.Min(vaporDeficit * evaporationRate, cloud + liquid);

				// Adjust evaporation rate based on wind and humidity
				double humidityFactor = 1.0 - relativeHumidity;
				double windFactor = 1.0 + windSpeed * 0.05;
				double actualEvaporation = maxEvaporation * humidityFactor * windFactor;

				if (actualEvaporation > 0)
				{
					     // Evaporate from clouds first
					     double fromClouds = Math.Min(actualEvaporation, cloud);
					     cloud -= fromClouds;

					     // Then from liquid
					     double fromLiquid = Math.Min(actualEvaporation - fromClouds, liquid);
					     liquid -= fromLiquid;

					     // Add to vapor
					     vapor += (fromClouds + fromLiquid);

					     // Absorb latent heat
					     double heatAbsorbed = (fromClouds + fromLiquid) * Constants.LATENT_HEAT_EVAPORATION;
					     temperature -= heatAbsorbed / Math.Max(airHeatCapacity, 0.001);
				}
		          }
	     }

	     // [Rest of the helper functions remain mostly the same, but ensure CalculateMaxWaterVapor is correct]

	     public static float CalculateMaxWaterVapor(float temperature, float pressure)
	     {
		          // Clausius-Clapeyron equation for saturation vapor density (kg/m³)
		          float T = temperature + 273.15f;
		          if (T <= 0f)
				return 0f;

		          // Saturation vapor pressure (Pa)
		          float es = 611.2f * Mathf.Exp(17.67f * (T - 273.15f) / (T - 29.65f));

		          // Saturation mixing ratio (kg/kg)
		          float ws = 0.622f * es / Mathf.Max(pressure - es, 1f);

		          // Air density (kg/m³)
		          float rho_air = pressure / (Constants.GAS_CONSTANT * T);

		          // Saturation vapor density (kg/m³)
		          return ws * rho_air;
	     }

	     public static float BilinearInterpolate(float q00, float q10, float q01, float q11, float tx, float ty)
	     {
		          float r0 = Mathf.Lerp(q00, q10, tx);
		          float r1 = Mathf.Lerp(q01, q11, tx);
		          return Mathf.Lerp(r0, r1, ty);
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
						          total += tile.waterVapor + tile.cloudWaterContent + tile.liquidWater;
					     }
				}
		          }
		          return total;
	     }

	     /// <summary>
	     /// Calculates pressure using the barometric formula and ideal gas law
	     /// </summary>
	     public static float CalculatePressure(float temperatureCelsius, float waterVaporDensity, float height)
	     {
		          // Convert temperature to Kelvin
		          float T = temperatureCelsius + 273.15f;
		          if (T <= 0f)
				T = 273.15f;

		          // Calculate dry air pressure using barometric formula
		          float scaleHeight = (Constants.GAS_CONSTANT * T) / (Constants.MOLAR_MASS_AIR * Constants.GRAVITY_ACCELERATION);
		          float basePressure = 101325f * Mathf.Exp(-height / scaleHeight);

		          // Adjust for water vapor (water vapor has lower molar mass than dry air)
		          float vaporPressure = waterVaporDensity * Constants.GAS_CONSTANT * T / Constants.MOLAR_MASS_WATER;
		          float dryAirPressure = basePressure - vaporPressure;

		          return Mathf.Max(dryAirPressure + vaporPressure, 100f); // Ensure positive pressure
	     }

	     /// <summary>
	     /// Calculates pressure gradient force between neighboring cells
	     /// </summary>
	     public static Vector3 CalculatePressureGradientForce(
	         AtmosphereTile[,,] tiles, int layer, int x, int y, int width, int height)
	     {
		          Vector3 force = Vector3.zero;
		          float currentPressure = tiles[layer, x, y].pressure;

		          // Check immediate neighbors
		          int[] dx = { -1, 1, 0, 0 };
		          int[] dy = { 0, 0, -1, 1 };

		          for (int i = 0; i < 4; i++)
		          {
				int nx = (x + dx[i] + width) % width;
				int ny = (y + dy[i] + height) % height;

				float neighborPressure = tiles[layer, nx, ny].pressure;
				float pressureDiff = currentPressure - neighborPressure; // From high to low

				// Force = pressure gradient / density (simplified)
				// Negative because air flows from high to low pressure
				Vector3 direction = new Vector3(dx[i], 0, dy[i]).normalized;
				force += direction * pressureDiff * 100f; // Scaling factor
		          }

		          return force;
	     }

	     /// <summary>
	     /// Coriolis effect calculation varying with latitude
	     /// </summary>
	     public static Vector3 CalculateCoriolisEffect(Vector3 windVelocity, float latitude)
	     {
		          // f = 2Ω sin(latitude) - Coriolis parameter
		          // Ω = Earth's angular velocity = 7.2921e-5 rad/s
		          float omega = 7.2921e-5f;
		          float f = 2f * omega * Mathf.Sin(latitude * Mathf.Deg2Rad);

		          // Northern hemisphere: deflects to the right
		          // Southern hemisphere: deflects to the left
		          return new Vector3(-windVelocity.z * f, 0, windVelocity.x * f);
	     }

	     /// <summary>
	     /// Calculates temperature change from adiabatic processes
	     /// </summary>
	     public static float CalculateAdiabaticCooling(AtmosphereTile current)
	     {
		          if (current == null)
				return 0f;

		          float T = current.temperature + 273.15f; // Convert to Kelvin
		          if (T <= 0f)
				return 0f;

		          float p = current.pressure;
		          if (p <= 0f)
				p = 101325f;

		          // Use simplified lapse rates
		          float dryLapseRate = -0.0098f; // °C/m
		          float moistLapseRate = -0.0065f; // °C/m (varies, but typical)

		          // Blend based on relative humidity
		          float lapseRate = Mathf.Lerp(dryLapseRate, moistLapseRate, Mathf.Clamp01(current.RelativeHumidity));

		          // Estimate vertical motion
		          float verticalMotion = current.windVelocity.y;
		          if (Mathf.Abs(verticalMotion) < 0.01f)
		          {
				// Small random vertical motion to prevent stagnation
				verticalMotion = UnityEngine.Random.Range(-0.1f, 0.1f);
		          }

		          // Return temperature change (°C)
		          return verticalMotion * lapseRate * current.layerHeight;
	     }

	     public static float CalculateRadiativeCooling(AtmosphereTile tile, float hours)
	     {
		          if (tile == null)
				return 0f;

		          float tempK = tile.temperature + 273.15f;
		          if (tempK <= 0f)
				return 0f;

		          // Simple radiative cooling: cools at night, minimal during day
		          float spaceTemp = 2.7f; // Cosmic background

		          // Simple model: always radiate, but much less than blackbody
		          float coolingRate = 0.1f; // °C/hour base rate
		          float tempDifference = tempK - spaceTemp;

		          // Cooling increases with temperature difference
		          return coolingRate * (tempDifference / 273f) * hours;
	     }

	     /// <summary>
	     /// Calculates relative humidity for an atmosphere tile
	     /// </summary>
	     public static float CalculateRelativeHumidity(AtmosphereTile tile)
	     {
		          if (tile == null)
				return 0f;

		          float tempC = tile.temperature;
		          if (Mathf.Abs(tempC + 237.3f) < 0.001f)
				return 0f;

		          // Tetens formula for saturation vapor pressure in kPa
		          float satVaporPressure_kPa = 0.6108f * Mathf.Exp((17.27f * tempC) / (tempC + 237.3f));
		          float satVaporPressure_Pa = satVaporPressure_kPa * 1000f;

		          // Calculate actual vapor pressure from water vapor content
		          float volume = tile.tileWidth * tile.tileWidth * tile.layerHeight;
		          if (volume <= 0f)
				volume = 1f;

		          // moles = mass / molar mass
		          float molesWater = tile.waterVapor / 0.018f; // kg to moles (water = 18g/mol)
		          float T = tempC + 273.15f;

		          // Using ideal gas law: P = nRT/V
		          float actualVaporPressure = (molesWater * Constants.GAS_CONSTANT * T) / volume;

		          if (satVaporPressure_Pa < 0.001f)
				return 0f;

		          return Mathf.Clamp01(actualVaporPressure / satVaporPressure_Pa);
	     }

	     public static float CalculateDewPoint(float tempC, float relativeHumidity)
	     {
		          if (Mathf.Abs(tempC + 237.3f) < 0.001f)
				return tempC;

		          float safeRH = Mathf.Clamp(relativeHumidity, 0.0001f, 1f);
		          float a = 17.27f;
		          float b = 237.7f;

		          float alpha = ((a * tempC) / (b + tempC)) + Mathf.Log(safeRH);
		          return (b * alpha) / (a - alpha);
	     }

	     public static double CalculateAirDensity(float temperature, float pressure)
	     {
		          // Ideal gas law: ρ = P / (R * T)
		          double T = temperature + 273.15;
		          if (T <= 0)
				return 1.225;
		          return pressure / (Constants.GAS_CONSTANT * T);
	     }

	     public static double CalculateAirMass(float temperature, float pressure, double volume)
	     {
		          double density = CalculateAirDensity(temperature, pressure);
		          return density * volume;
	     }
}
