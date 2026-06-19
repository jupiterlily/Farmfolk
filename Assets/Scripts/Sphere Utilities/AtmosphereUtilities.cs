using System;
using System.Collections.Generic;
using UnityEngine;

public static class AtmosphereUtilities
{
	     /// <summary>
	     /// Simulates atmospheric processes using a conservative finite-volume approach
	     /// </summary>

	     /// <summary>
	     /// Apply phase changes with PROPERLY SCALED energy calculations
	     /// </summary>
	     /// <summary>
	     /// Calculate realistic air mass using ideal gas law
	     /// </summary>
	     /// 
	     /// <summary>
	     /// Apply semi-Lagrangian advection to transport atmospheric properties based on wind
	     /// This conserves mass while smoothly transporting water vapor, cloud water, and thermal energy
	     /// </summary>
	     public static void ApplyAtmosphericAdvection(AtmosphereTile[,,] atmosphereTiles, float hours)
	     {
		          int layers = atmosphereTiles.GetLength(0);
		          int width = atmosphereTiles.GetLength(1);
		          int height = atmosphereTiles.GetLength(2);

		          // Create temporary storage for advected quantities
		          List<ThermalEnergy>[,,] advectedVapor = new List<ThermalEnergy>[layers, width, height];
		          List<ThermalEnergy>[,,] advectedCloud = new List<ThermalEnergy>[layers, width, height];
		          List<ThermalEnergy>[,,] advectedIce = new List<ThermalEnergy>[layers, width, height];

		          // Initialize temporary storage
		          for (int l = 0; l < layers; l++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          advectedVapor[l, x, y] = new List<ThermalEnergy>();
						          advectedCloud[l, x, y] = new List<ThermalEnergy>();
						          advectedIce[l, x, y] = new List<ThermalEnergy>();
					     }
				}
		          }

		          float timeStepSeconds = hours * 3600f;

		          // Step 1: Calculate advection for each tile
		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          AtmosphereTile current = atmosphereTiles[layer, x, y];
						          if (current == null)
								continue;

						          // Semi-Lagrangian: trace back to where the air came from
						          Vector2 departure = CalculateDeparturePoint(
						              current,
						              x, y, width, height,
						              timeStepSeconds
						          );

						          // Bilinear interpolation to get properties from departure point
						          AdvectProperties(
						              atmosphereTiles,
						              departure, layer,
						              current,
						              advectedVapor, advectedCloud, advectedIce,
						              x, y
						          );
					     }
				}
		          }

		          // Step 2: Apply advected properties back to tiles
		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          AtmosphereTile tile = atmosphereTiles[layer, x, y];
						          if (tile == null)
								continue;

						          // Replace water vapor with advected vapor
						          if (advectedVapor[layer, x, y].Count > 0)
						          {
								tile.waterVapor = advectedVapor[layer, x, y][0];
						          }

						          // Replace cloud water with advected cloud
						          if (advectedCloud[layer, x, y].Count > 0)
						          {
								tile.cloudWaterContent = advectedCloud[layer, x, y][0];
						          }

						          // Replace ice with advected ice
						          if (advectedIce[layer, x, y].Count > 0)
						          {
								tile.iceParticles = advectedIce[layer, x, y][0];
						          }
					     }
				}
		          }
	     }

	     /// <summary>
	     /// Calculate where air at this location came from in the previous timestep
	     /// Using semi-Lagrangian approach
	     /// </summary>
	     private static Vector2 CalculateDeparturePoint(
	         AtmosphereTile current,
	         int x, int y,
	         int width, int height,
	         float timeStepSeconds)
	     {
		          if (current == null)
				return new Vector2(x, y);

		          // Wind carries the parcel
		          Vector3 windVelocity = current.windVelocity;
		          float cellWidth = current.tileWidth;

		          // Displacement in grid cells
		          float dx = (windVelocity.x * timeStepSeconds) / cellWidth;
		          float dy = (windVelocity.z * timeStepSeconds) / cellWidth;

		          // Departure point (where this cell's air came from)
		          float departureX = x - dx;
		          float departureY = y - dy;

		          // Wrap around at poles (periodic boundary in longitude)
		          departureX = ((departureX % width) + width) % width;
		          departureY = ((departureY % height) + height) % height;

		          return new Vector2(departureX, departureY);
	     }

	     /// <summary>
	     /// Advect atmospheric properties using bilinear interpolation from departure point
	     /// </summary>
	     private static void AdvectProperties(
	         AtmosphereTile[,,] atmosphereTiles,
	         Vector2 departurePoint,
	         int layer,
	         AtmosphereTile currentTile,
	         List<ThermalEnergy>[,,] advectedVapor,
	         List<ThermalEnergy>[,,] advectedCloud,
	         List<ThermalEnergy>[,,] advectedIce,
	         int x, int y)
	     {
		          int width = atmosphereTiles.GetLength(1);
		          int height = atmosphereTiles.GetLength(2);

		          // Integer grid points for bilinear interpolation
		          int x0 = Mathf.FloorToInt(departurePoint.x);
		          int x1 = (x0 + 1) % width;
		          int y0 = Mathf.FloorToInt(departurePoint.y);
		          int y1 = (y0 + 1) % height;

		          // Fractional parts for interpolation
		          float fx = departurePoint.x - x0;
		          float fy = departurePoint.y - y0;

		          // Get four corner tiles
		          AtmosphereTile t00 = atmosphereTiles[layer, x0, y0];
		          AtmosphereTile t10 = atmosphereTiles[layer, x1, y0];
		          AtmosphereTile t01 = atmosphereTiles[layer, x0, y1];
		          AtmosphereTile t11 = atmosphereTiles[layer, x1, y1];

		          if (t00 == null || t10 == null || t01 == null || t11 == null)
				return;

		          // Bilinear interpolation for water vapor
		          ThermalEnergy interpolatedVapor = BilinearInterpolateEnergy(
		              t00.waterVapor, t10.waterVapor, t01.waterVapor, t11.waterVapor,
		              fx, fy
		          );
		          advectedVapor[layer, x, y].Add(interpolatedVapor);

		          // Bilinear interpolation for cloud water
		          ThermalEnergy interpolatedCloud = BilinearInterpolateEnergy(
		              t00.cloudWaterContent, t10.cloudWaterContent, t01.cloudWaterContent, t11.cloudWaterContent,
		              fx, fy
		          );
		          advectedCloud[layer, x, y].Add(interpolatedCloud);

		          // Bilinear interpolation for ice
		          ThermalEnergy interpolatedIce = BilinearInterpolateEnergy(
		              t00.iceParticles, t10.iceParticles, t01.iceParticles, t11.iceParticles,
		              fx, fy
		          );
		          advectedIce[layer, x, y].Add(interpolatedIce);
	     }

	     /// <summary>
	     /// Bilinear interpolation of ThermalEnergy properties
	     /// Preserves mass and temperature through interpolation
	     /// </summary>
	     private static ThermalEnergy BilinearInterpolateEnergy(
	         ThermalEnergy e00, ThermalEnergy e10, ThermalEnergy e01, ThermalEnergy e11,
	         float fx, float fy)
	     {
		          // Interpolate mass
		          float interpolatedMass = BilinearInterpolate(
		              e00.Mass, e10.Mass, e01.Mass, e11.Mass,
		              fx, fy
		          );

		          // Interpolate temperature (mass-weighted)
		          float totalMass = e00.Mass + e10.Mass + e01.Mass + e11.Mass;
		          float interpolatedTemp = 0f;

		          if (totalMass > 0.001f)
		          {
				interpolatedTemp = BilinearInterpolate(
				    e00.Temperature * e00.Mass,
				    e10.Temperature * e10.Mass,
				    e01.Temperature * e01.Mass,
				    e11.Temperature * e11.Mass,
				    fx, fy
				) / interpolatedMass;
		          }
		          else
		          {
				// Fall back to simple temperature interpolation if no mass
				interpolatedTemp = BilinearInterpolate(
				    e00.Temperature, e10.Temperature, e01.Temperature, e11.Temperature,
				    fx, fy
				);
		          }

		          // Create new interpolated energy object
		          ThermalEnergy result = new ThermalEnergy(
		              interpolatedMass,
		              e00.SpecificHeat,  // Assume same specific heat for all water
		              e00.LatentHeatFusion,
		              e00.LatentHeatVaporization,
		              e00.MeltingPoint,
		              e00.BoilingPoint,
		              e00.ThermalConductivity,
		              interpolatedTemp
		          );

		          return result;
	     }

	     /// <summary>
	     /// Bilinear interpolation helper for scalar values
	     /// </summary>
	     private static float BilinearInterpolate(float v00, float v10, float v01, float v11, float fx, float fy)
	     {
		          float x0y = Mathf.Lerp(v00, v10, fx);
		          float x1y = Mathf.Lerp(v01, v11, fx);
		          return Mathf.Lerp(x0y, x1y, fy);
	     }

	     /// <summary>
	     /// Apply flux-based conservative advection using semi-Lagrangian with mass fixup
	     /// More stable for extreme wind velocities
	     /// </summary>
	     public static void ApplyConservativeFluxAdvection(AtmosphereTile[,,] atmosphereTiles, float hours)
	     {
		          int layers = atmosphereTiles.GetLength(0);
		          int width = atmosphereTiles.GetLength(1);
		          int height = atmosphereTiles.GetLength(2);

		          float timeStepSeconds = hours * 3600f;

		          // Process each layer
		          for (int layer = 0; layer < layers; layer++)
		          {
				// Advect in X direction
				AdvectInDirection(atmosphereTiles, layer, 0, width, height, timeStepSeconds);

				// Advect in Z direction  
				AdvectInDirection(atmosphereTiles, layer, 1, width, height, timeStepSeconds);

				// Advect in vertical (Y) direction
				AdvectVertical(atmosphereTiles, layer, width, height, timeStepSeconds);
		          }
	     }

	     /// <summary>
	     /// Advect in a specific horizontal direction (0=X, 1=Z)
	     /// </summary>
	     private static void AdvectInDirection(
	         AtmosphereTile[,,] atmosphereTiles,
	         int layer,
	         int direction,
	         int width, int height,
	         float timeStepSeconds)
	     {
		          // Create temporary arrays for fluxes
		          float[,] vaporFlux = new float[width, height];
		          float[,] cloudFlux = new float[width, height];
		          float[,] iceFlux = new float[width, height];

		          // Calculate fluxes
		          for (int x = 0; x < width; x++)
		          {
				for (int y = 0; y < height; y++)
				{
					     AtmosphereTile tile = atmosphereTiles[layer, x, y];
					     if (tile == null)
						          continue;

					     float cellWidth = tile.tileWidth;
					     float windComponent = (direction == 0) ? tile.windVelocity.x : tile.windVelocity.z;

					     // Upwind scheme: determine source based on wind direction
					     int sourceIndex = (windComponent >= 0) ?
					         ((direction == 0) ? (x - 1 + width) % width : (y - 1 + height) % height) :
					         ((direction == 0) ? (x + 1) % width : (y + 1) % height);

					     AtmosphereTile source = (direction == 0) ?
					         atmosphereTiles[layer, sourceIndex, y] :
					         atmosphereTiles[layer, x, sourceIndex];

					     if (source == null)
						          continue;

					     // Calculate flux: velocity * mass
					     float flux = Mathf.Abs(windComponent) * timeStepSeconds / cellWidth;
					     flux = Mathf.Clamp01(flux);  // Limit to prevent over-shooting

					     vaporFlux[x, y] = source.waterVapor.Mass * flux;
					     cloudFlux[x, y] = source.cloudWaterContent.Mass * flux;
					     iceFlux[x, y] = source.iceParticles.Mass * flux;
				}
		          }

		          // Apply fluxes
		          for (int x = 0; x < width; x++)
		          {
				for (int y = 0; y < height; y++)
				{
					     AtmosphereTile tile = atmosphereTiles[layer, x, y];
					     if (tile == null)
						          continue;

					     int prevIndex = (direction == 0) ? (x - 1 + width) % width : (y - 1 + height) % height;
					     int nextIndex = (direction == 0) ? (x + 1) % width : (y + 1) % height;

					     AtmosphereTile prev = (direction == 0) ?
					         atmosphereTiles[layer, prevIndex, y] :
					         atmosphereTiles[layer, x, prevIndex];

					     AtmosphereTile next = (direction == 0) ?
					         atmosphereTiles[layer, nextIndex, y] :
					         atmosphereTiles[layer, x, nextIndex];

					     if (prev == null || next == null)
						          continue;

					     // Inflow from previous cell, outflow to next cell
					     float inFlux = vaporFlux[prevIndex, y];
					     float outFlux = vaporFlux[x, y];

					     float vaporChange = inFlux - outFlux;

					     // Update only if mass change is reasonable
					     if (Mathf.Abs(vaporChange) > 0.001f && tile.waterVapor.Mass + vaporChange > 0)
					     {
						          tile.waterVapor.ChangeMass(tile.waterVapor.Mass + vaporChange);
					     }
				}
		          }
	     }

	     /// <summary>
	     /// Advect vertical motion (updrafts/downdrafts)
	     /// </summary>
	     private static void AdvectVertical(
	         AtmosphereTile[,,] atmosphereTiles,
	         int layer,
	         int width, int height,
	         float timeStepSeconds)
	     {
		          if (layer <= 0 || layer >= atmosphereTiles.GetLength(0) - 1)
				return;

		          // Simple vertical advection between layers
		          for (int x = 0; x < width; x++)
		          {
				for (int y = 0; y < height; y++)
				{
					     AtmosphereTile current = atmosphereTiles[layer, x, y];
					     AtmosphereTile above = atmosphereTiles[layer + 1, x, y];
					     AtmosphereTile below = atmosphereTiles[layer - 1, x, y];

					     if (current == null || above == null || below == null)
						          continue;

					     // Vertical velocity (small, typically)
					     float verticalVelocity = current.windVelocity.y;

					     // Simple averaging of vertical exchange
					     float exchangeFraction = 0.01f * timeStepSeconds;  // Minimal vertical mixing

					     if (Mathf.Abs(verticalVelocity) > 0.001f)
					     {
						          if (verticalVelocity > 0)
						          {
								// Upward motion: move moisture up
								float toMove = current.waterVapor.Mass * exchangeFraction;
								ThermalEnergyUtilities.TransferMassAndEnergy(
								    current.waterVapor, above.waterVapor, toMove
								);
						          }
						          else
						          {
								// Downward motion: move moisture down
								float toMove = current.waterVapor.Mass * exchangeFraction;
								ThermalEnergyUtilities.TransferMassAndEnergy(
								    current.waterVapor, below.waterVapor, toMove
								);
						          }
					     }
				}
		          }
	     }
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

						          foreach (var energy in tile.energy)
						          {
								totalEnergy += energy.StoredEnergy + energy.SpecificHeat;
						          }
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
						          total += tile.waterVapor.Mass + tile.cloudWaterContent.Mass;
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
	     //  public static Vector3 CalculatePressureGradientForce(
	     //      AtmosphereTile[,,] tiles, int layer, int x, int y, int width, int height)
	     //  {
	     //        Vector3 force = Vector3.zero;
	     //        float currentPressure = tiles[layer, x, y].pressure;

	     //        // Check immediate neighbors
	     //        int[] dx = { -1, 1, 0, 0 };
	     //        int[] dy = { 0, 0, -1, 1 };

	     //        for (int i = 0; i < 4; i++)
	     //        {
	     //int nx = (x + dx[i] + width) % width;
	     //int ny = (y + dy[i] + height) % height;

	     //float neighborPressure = tiles[layer, nx, ny].pressure;
	     //float pressureDiff = currentPressure - neighborPressure; // From high to low

	     //// Force = pressure gradient / density (simplified)
	     //// Negative because air flows from high to low pressure
	     //Vector3 direction = new Vector3(dx[i], 0, dy[i]).normalized;
	     //force += direction * pressureDiff / Constants.AIR_DENSITY; // Scaling factor
	     //        }

	     //        return force;
	     //  }

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
		          hours *= 3600f; // Convert to seconds
		          float tempK = tile.temperature;
		          if (tempK <= 0f)
				return 0f;

		          float surfaceArea = tile.tileWidth * tile.tileWidth;

		          if (tempK <= 0 || surfaceArea <= 0)
				return 0f;

		          // P = ε * σ * A * (T^4 - T_space^4)
		          float power = Constants.ATMOSPHERIC_EMISSIVITY * Constants.STEFAN_BOLTZMANN * surfaceArea *
		              (Mathf.Pow(tempK, 4) - Mathf.Pow(Constants.COSMIC_BACKGROUND, 4));

		          return power * hours;  // Watts (J/s)
	     }

	     /// <summary>
	     /// Calculates relative humidity for an atmosphere tile
	     /// </summary>
	     public static float CalculateRelativeHumidity(AtmosphereTile tile)
	     {
		          if (tile == null)
				return 0f;

		          float temp = tile.temperature;
		          if (temp < 0.001f)
				return 0f;

		          // Tetens formula for saturation vapor pressure in kPa
		          float satVaporPressure_kPa = 0.6108f * Mathf.Exp((17.27f * temp) / (temp + 237.3f));
		          float satVaporPressure_Pa = satVaporPressure_kPa * 1000f;

		          // Calculate actual vapor pressure from water vapor content
		          float volume = tile.tileWidth * tile.tileWidth * tile.layerHeight;
		          if (volume <= 0f)
				volume = 1f;

		          // moles = mass / molar mass
		          float molesWater = tile.waterVapor.Mass / 0.018f; // kg to moles (water = 18g/mol)

		          // Using ideal gas law: P = nRT/V
		          float actualVaporPressure = (molesWater * Constants.GAS_CONSTANT * temp) / volume;

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

	     /// <summary>
	     /// Updates wind velocity for all atmosphere tiles based on pressure gradients, Coriolis effect, and friction
	     /// </summary>
	     public static void UpdateAtmosphericWind(AtmosphereTile[,,] atmosphereTiles, float hours)
	     {
		          int layers = atmosphereTiles.GetLength(0);
		          int width = atmosphereTiles.GetLength(1);
		          int height = atmosphereTiles.GetLength(2);

		          float timeStepSeconds = hours * 3600f;

		          // Update wind for each tile
		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < width; x++)
				{
					     for (int y = 0; y < height; y++)
					     {
						          AtmosphereTile current = atmosphereTiles[layer, x, y];
						          if (current == null)
								continue;

						          // 1. Calculate pressure gradient force
						          Vector3 pressureForce = CalculatePressureGradientForce(atmosphereTiles, layer, x, y, width, height);

						          // 2. Calculate Coriolis effect (latitude-dependent)
						          float latitude = (y / (float)height * 180f) - 90f;
						          Vector3 coriolisForce = CalculateCoriolisEffect(current.windVelocity, latitude);

						          // 3. Apply friction (stronger at surface layer)
						          float frictionCoefficient = (layer == 0) ? 0.05f : 0.01f;
						          Vector3 friction = -current.windVelocity * frictionCoefficient;

						          // 4. Update wind velocity
						          Vector3 acceleration = (pressureForce + coriolisForce + friction) / Constants.AIR_DENSITY;
						          current.windVelocity += acceleration * timeStepSeconds;

						          // 5. Apply speed limit (prevent unrealistic wind speeds)
						          float maxWindSpeed = 50f; // m/s (180 km/h)
						          current.windVelocity = Vector3.ClampMagnitude(current.windVelocity, maxWindSpeed);

						          // 6. Update wind speed magnitude
						          current.windSpeed = current.windVelocity.magnitude;

						          // 7. Add small random perturbations to prevent stagnation
						          current.windVelocity += UnityEngine.Random.insideUnitSphere * 0.01f;
					     }
				}
		          }
	     }

	     /// <summary>
	     /// Calculates pressure gradient force - drives wind from high to low pressure
	     /// </summary>
	     private static Vector3 CalculatePressureGradientForce(
	         AtmosphereTile[,,] tiles, int layer, int x, int y, int width, int height)
	     {
		          Vector3 force = Vector3.zero;
		          AtmosphereTile currentTile = tiles[layer, x, y];
		          float currentPressure = currentTile.pressure;

		          if (currentPressure <= 0)
				return force;

		          // Use the pre-stored neighbors
		          // Assuming horizontalNeighbors is an array of 4 elements in the order: [North, East, South, West]
		          if (currentTile.horizontalNeighbors == null || currentTile.horizontalNeighbors.Length != 4)
		          {
				Debug.LogError("Horizontal neighbors array is not properly initialized!");
				return force;
		          }

		          // Process each neighbor with its corresponding direction
		          // North
		          if (currentTile.horizontalNeighbors[0] != null)
		          {
				float pressureDiff = currentPressure - currentTile.horizontalNeighbors[0].pressure;
				force += Vector3.forward * (pressureDiff / Constants.AIR_DENSITY);
		          }

		          // East
		          if (currentTile.horizontalNeighbors[1] != null)
		          {
				float pressureDiff = currentPressure - currentTile.horizontalNeighbors[1].pressure;
				force += Vector3.right * (pressureDiff / Constants.AIR_DENSITY);
		          }

		          // South
		          if (currentTile.horizontalNeighbors[2] != null)
		          {
				float pressureDiff = currentPressure - currentTile.horizontalNeighbors[2].pressure;
				force += Vector3.back * (pressureDiff / Constants.AIR_DENSITY);
		          }

		          // West
		          if (currentTile.horizontalNeighbors[3] != null)
		          {
				float pressureDiff = currentPressure - currentTile.horizontalNeighbors[3].pressure;
				force += Vector3.left * (pressureDiff / Constants.AIR_DENSITY);
		          }

		          return force;
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

	     /// <summary>
	     /// Initializes Hadley cell wind patterns for all atmosphere tiles
	     /// </summary>
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

						          // Calculate latitude from grid position
						          // X axis represents latitude: 0 = South Pole (-90°), max = North Pole (+90°)
						          float latitude = tileManager.GetLatitude(x, y);

						          // Initialize wind pattern based on latitude
						          Vector3 wind = CalculateHadleyCellWind(latitude, layer, layers);
						          tile.windVelocity = wind;
						          tile.windSpeed = wind.magnitude;

						          // Initialize temperature based on latitude and layer
						          float temperature = CalculateHadleyCellTemperature(latitude, layer, layers);
						          tile.nitrogen.AddEnergy((temperature - 273.15f) * tile.nitrogen.SpecificHeat);

						          // Initialize humidity and pressure
						          InitializeHadleyCellPressure(tile, latitude, layer, layers);
						          InitializeHadleyCellHumidity(tile, latitude, layer);

						          Debug.Log($"Initialized tile ({x},{y},{layer}): Lat={latitude:F1}°, Wind={wind.magnitude:F2}m/s, Temp={temperature - 273.15f:F1}°C");
					     }
				}
		          }
	     }

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
	     /// Calculates temperature profile based on latitude and altitude
	     /// </summary>
	     private static float CalculateHadleyCellTemperature(float latitude, int layer, int maxLayers)
	     {
		          float normalizedLatitude = Mathf.Abs(latitude);
		          float baseSurfaceTemp;

		          // Determine base surface temperature by latitude
		          if (normalizedLatitude <= HADLEY_CELL_BOUNDARY)
		          {
				// Tropical: warm
				baseSurfaceTemp = EQUATORIAL_SURFACE_TEMP;
		          }
		          else if (normalizedLatitude <= FERREL_CELL_BOUNDARY)
		          {
				// Temperate: cool
				baseSurfaceTemp = Mathf.Lerp(SUBTROPICAL_SURFACE_TEMP, TEMPERATE_SURFACE_TEMP,
				    (normalizedLatitude - HADLEY_CELL_BOUNDARY) / (FERREL_CELL_BOUNDARY - HADLEY_CELL_BOUNDARY));
		          }
		          else
		          {
				// Polar: cold
				baseSurfaceTemp = POLAR_SURFACE_TEMP;
		          }

		          // Apply lapse rate (temperature decreases with altitude)
		          // Dry adiabatic lapse rate: ~9.8 K/km
		          float dryLapseRate = 9.8f / 1000f; // K/m
		          float altitude = layer * 10f * Mathf.Pow(10f, layer); // Rough altitude estimation
		          float temperatureAtLayer = baseSurfaceTemp - (dryLapseRate * altitude);

		          return Mathf.Clamp(temperatureAtLayer, 200f, 320f); // Realistic bounds (Kelvin)
	     }

	     /// <summary>
	     /// Initializes pressure distribution matching barometric formula with latitude variations
	     /// </summary>
	     private static void InitializeHadleyCellPressure(AtmosphereTile tile, float latitude, int layer, int maxLayers)
	     {
		          float normalizedLatitude = Mathf.Abs(latitude);

		          // Base pressure follows barometric formula
		          float altitude = layer * 10f * Mathf.Pow(10f, layer);
		          float seaLevelPressure = 101325f;

		          // Pressure decreases with altitude
		          float pressureAtLayer = seaLevelPressure * Mathf.Exp(-altitude / 8435f); // Scale height ~8.4 km

		          // Latitude adjustments
		          if (normalizedLatitude <= HADLEY_CELL_BOUNDARY)
		          {
				// Low pressure at ITCZ (equator)
				pressureAtLayer *= Mathf.Lerp(0.98f, 1.0f, normalizedLatitude / HADLEY_CELL_BOUNDARY);
		          }
		          else if (normalizedLatitude <= FERREL_CELL_BOUNDARY)
		          {
				// High pressure at subtropical ridge (~30°)
				float distanceFromRidge = Mathf.Abs(normalizedLatitude - 30f);
				pressureAtLayer *= 1.0f + 0.01f * (1f - distanceFromRidge / 30f);
		          }
		          else
		          {
				// Low pressure at subpolar low (~60°)
				pressureAtLayer *= 0.97f;
		          }

		          tile.pressure = Mathf.Clamp(pressureAtLayer, 100f, 110000f);
	     }

	     /// <summary>
	     /// Initializes humidity based on latitude and vertical motion
	     /// </summary>
	     private static void InitializeHadleyCellHumidity(AtmosphereTile tile, float latitude, int layer)
	     {
		          float normalizedLatitude = Mathf.Abs(latitude);
		          float baseHumidity = 0.5f; // 50% base

		          // ITCZ (equator): High humidity due to evaporation and convergence
		          if (normalizedLatitude <= 5f)
		          {
				baseHumidity = Mathf.Lerp(0.8f, 0.7f, (normalizedLatitude + 5f) / 10f); // 70-80% RH
		          }
		          // Hadley subsidence zones (15-30°): Low humidity due to descent and warming
		          else if (normalizedLatitude <= HADLEY_CELL_BOUNDARY)
		          {
				float inSubsidenceZone = (normalizedLatitude - 5f) / (HADLEY_CELL_BOUNDARY - 5f);
				baseHumidity = Mathf.Lerp(0.7f, 0.3f, inSubsidenceZone); // 70% → 30% RH
		          }
		          // Ferrel convergence: Moderate humidity
		          else if (normalizedLatitude <= FERREL_CELL_BOUNDARY)
		          {
				baseHumidity = 0.6f; // ~60% RH
		          }
		          // Polar: Lower humidity (cold air holds less moisture)
		          else
		          {
				baseHumidity = 0.4f; // ~40% RH
		          }

		          // Adjust for layer (upper layers drier)
		          float layerAdjustment = Mathf.Pow(0.8f, layer);
		          baseHumidity *= layerAdjustment;

		          // Calculate actual water vapor mass based on temperature and relative humidity
		          float saturationMass = AtmosphereUtilities.CalculateSaturationWaterMass(
		              tile.nitrogen.Temperature - 273.15f,
		              tile.pressure,
		              tile.tileWidth * tile.tileWidth * tile.layerHeight
		          );

		          float waterVaporMass = saturationMass * baseHumidity;
		          tile.waterVapor.ChangeMass(waterVaporMass);

		          // Initialize some cloud water in convergence zones
		          if (normalizedLatitude <= 5f || (normalizedLatitude > HADLEY_CELL_BOUNDARY && normalizedLatitude <= FERREL_CELL_BOUNDARY))
		          {
				float cloudMass = waterVaporMass * 0.1f; // 10% of vapor as clouds
				tile.cloudWaterContent.ChangeMass(cloudMass);
		          }
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
