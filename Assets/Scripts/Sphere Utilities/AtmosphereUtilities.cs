using System;
using System.Collections.Generic;
using UnityEngine;

public static class AtmosphereUtilities
{
	     /// <summary>
	     /// Simulates atmospheric processes using a conservative finite-volume approach
	     /// </summary>
	     //  public static void SimulateAtmosphere(AtmosphereTile[,,] atmosphereTiles, float hours)
	     //  {
	     //        int layers = atmosphereTiles.GetLength(0);
	     //        int width = atmosphereTiles.GetLength(1);
	     //        int height = atmosphereTiles.GetLength(2);

	     //        // Store initial energy state for conservation check
	     //        float initialTotalEnergy = CalculateTotalEnergy(atmosphereTiles);
	     //        float initialTotalWater = CalculateTotalWater(atmosphereTiles);

	     //        // Use floats but with proper scaling to avoid precision issues
	     //        float[,,] newTemps = new float[layers, width, height];
	     //        float[,,] newVapor = new float[layers, width, height];
	     //        float[,,] newCloudWater = new float[layers, width, height];
	     //        float[,,] newLiquidWater = new float[layers, width, height];
	     //        Vector3[,,] newWinds = new Vector3[layers, width, height];

	     //        // First: Calculate all temperature changes WITHOUT water phase changes
	     //        for (int layer = 0; layer < layers; layer++)
	     //        {
	     //for (int x = 0; x < width; x++)
	     //{
	     //	     for (int y = 0; y < height; y++)
	     //	     {
	     //		          AtmosphereTile current = atmosphereTiles[layer, x, y];

	     //		          // 1. Adiabatic change - ensure small values
	     //		          float adiabaticChange = CalculateAdiabaticChange(current, layer, layers) * hours;

	     //		          // 2. Radiative change - ensure small values
	     //		          float radiativeChange = CalculateRadiativeChange(current, layer, layers, hours);

	     //		          // Apply temperature changes
	     //		          newTemps[layer, x, y] = current.temperature + adiabaticChange + radiativeChange;

	     //		          // Copy water values
	     //		          newVapor[layer, x, y] = current.waterVapor.Mass;
	     //		          newCloudWater[layer, x, y] = current.cloudWaterContent.Mass;
	     //		          newLiquidWater[layer, x, y] = current.liquidWater.Mass;

	     //		          // Calculate wind changes
	     //		          Vector3 pressureForce = CalculatePressureGradientForce(atmosphereTiles, layer, x, y, width, height);
	     //		          float latitude = (y / (float)height * 180f) - 90f;
	     //		          Vector3 coriolisForce = CalculateCoriolisEffect(current.windVelocity, latitude);
	     //		          float frictionCoefficient = (layer == 0) ? 0.05f : 0.01f;
	     //		          Vector3 friction = -current.windVelocity * frictionCoefficient;

	     //		          newWinds[layer, x, y] = current.windVelocity +
	     //		              (pressureForce + coriolisForce + friction) * hours;
	     //		          newWinds[layer, x, y] = Vector3.ClampMagnitude(newWinds[layer, x, y], 50f);
	     //	     }
	     //}
	     //        }

	     //        // Second: Apply conservative advection
	     //        ApplyConservativeAdvection(
	     //            atmosphereTiles,
	     //            ref newTemps, ref newVapor, ref newCloudWater, ref newLiquidWater,
	     //            newWinds, hours
	     //        );

	     //        // Third: Apply phase changes with PROPER energy scaling
	     //        for (int layer = 0; layer < layers; layer++)
	     //        {
	     //for (int x = 0; x < width; x++)
	     //{
	     //	     for (int y = 0; y < height; y++)
	     //	     {
	     //		          AtmosphereTile current = atmosphereTiles[layer, x, y];
	     //		          float volume = current.tileWidth * current.tileWidth * current.layerHeight;

	     //		          // Get the advected temperature
	     //		          float tempAfterAdvection = newTemps[layer, x, y];

	     //		          // Apply phase changes with proper scaling
	     //		          ApplyPhaseChangesWithProperScaling(
	     //		              ref tempAfterAdvection,
	     //		              ref newVapor[layer, x, y],
	     //		              ref newCloudWater[layer, x, y],
	     //		              ref newLiquidWater[layer, x, y],
	     //		              current.pressure,
	     //		              volume,
	     //		              current.windSpeed,
	     //		              hours
	     //		          );

	     //		          // Store final temperature
	     //		          newTemps[layer, x, y] = tempAfterAdvection;
	     //	     }
	     //}
	     //        }

	     //        // Fourth: Apply all changes to original tiles
	     //        for (int layer = 0; layer < layers; layer++)
	     //        {
	     //for (int x = 0; x < width; x++)
	     //{
	     //	     for (int y = 0; y < height; y++)
	     //	     {
	     //		          AtmosphereTile tile = atmosphereTiles[layer, x, y];

	     //		          // Apply temperature with smoothing and strict bounds
	     //		          float targetTemp = Mathf.Clamp(newTemps[layer, x, y], -100f, 100f);
	     //		          tile.temperature = Mathf.Lerp(tile.temperature, targetTemp, 0.3f);

	     //		          // Apply water components with conservation
	     //		          float totalWaterBefore = tile.waterVapor.Mass + tile.cloudWaterContent.Mass + tile.liquidWater.Mass;
	     //		          float totalWaterAfter = newVapor[layer, x, y] + newCloudWater[layer, x, y] + newLiquidWater[layer, x, y];

	     //		          // Redistribute to maintain exact conservation
	     //		          if (totalWaterAfter > 0)
	     //		          {
	     //				float ratio = totalWaterBefore / totalWaterAfter;
	     //				tile.waterVapor = Mathf.Max(0, newVapor[layer, x, y] * ratio);
	     //				tile.cloudWaterContent = Mathf.Max(0, newCloudWater[layer, x, y] * ratio);
	     //				tile.liquidWater = Mathf.Max(0, newLiquidWater[layer, x, y] * ratio);
	     //		          }

	     //		          // Apply wind
	     //		          tile.windVelocity = newWinds[layer, x, y];
	     //		          tile.windSpeed = tile.windVelocity.magnitude;

	     //		          // Recalculate pressure
	     //		          float vaporDensity = tile.waterVapor.Mass / Mathf.Max(tile.layerHeight * tile.tileWidth * tile.tileWidth, 0.001f);
	     //		          tile.pressure = CalculateRealisticPressure(
	     //		              tile.temperature,
	     //		              vaporDensity,
	     //		              layer * tile.layerHeight,
	     //		              tile.waterVapor.Mass
	     //		          );
	     //	     }
	     //}
	     //        }

	     //        // Fifth: Validate conservation
	     //        float finalTotalEnergy = CalculateTotalEnergy(atmosphereTiles);
	     //        float finalTotalWater = CalculateTotalWater(atmosphereTiles);

	     //        float energyError = Mathf.Abs(finalTotalEnergy - initialTotalEnergy);
	     //        float waterError = Mathf.Abs(finalTotalWater - initialTotalWater);

	     //        if (energyError > 1e6f) // If error is more than 1 MJ
	     //        {
	     //Debug.LogError($"Energy conservation error: {energyError} J");
	     //        }

	     //        if (waterError > 0.1f)
	     //        {
	     //Debug.LogWarning($"Water conservation error: {waterError} kg");
	     //        }
	     //  }

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

						          // Sensible heat
						          tile.energy.ForEach(e =>


								totalEnergy += e.StoredEnergy + e.SpecificHeat);
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
				force += direction * pressureDiff / Constants.AIR_DENSITY; // Scaling factor
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
}
