using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AtmosphereUtilities
{
	     public static void ProcessDownwelling(AtmosphereTile topTile, TileData[] surfaceTiles, float hours, RadiativeEnergy incomingSolar, float zenithAngle)
	     {
		          RadiativeEnergy incident = new RadiativeEnergy(incomingSolar.Wavelength, incomingSolar.StoredEnergy);

		          AtmosphereTile current = topTile;

		          while (current != null) //while there isn't a tile below us
		          {
				// Transfer stored downwelling and reflected energy (if any)
				//Debug.Log($"[Atmosphere Utilities] : {current.downwellingSolar.StoredEnergy} joules in downwelling solar of {current.tileGridRef}.");
				if (current.downwellingSolar != null && current.downwellingSolar.StoredEnergy > 0f)
					     EnergyTransferUtilities.TransferEnergy(current.downwellingSolar, incident, current.downwellingSolar.StoredEnergy);


				if (current.reflectedSolar != null && current.reflectedSolar.StoredEnergy > 0f)
					     EnergyTransferUtilities.TransferEnergy(current.reflectedSolar, incident, current.reflectedSolar.StoredEnergy);

				//Debug.Log($"[Atmosphere Utilities] : {incident.StoredEnergy} joules in incident solar of {current.tileGridRef}.");
				float area = current.tileWidth * current.tileWidth;
				float layerHeight = current.layerHeight;
				float cosZenith = Mathf.Cos(zenithAngle * Mathf.Deg2Rad);
				float pathFactor = 1f / Mathf.Max(cosZenith, 0.001f);

				// ---- Compute optical depth ----
				float tau = 0f;
				float weightedOmega = 0f;

				// Inline component additions (no local function overhead)
				// Water vapour
				float massPerArea = current.waterVapor.Mass / area;
				float tau_i = SimulationConstants.EXTINCTION_WATER_VAPOUR * massPerArea * pathFactor;
				tau += tau_i;
				weightedOmega += tau_i * SimulationConstants.SSA_WATER_VAPOUR;

				// Cloud water
				massPerArea = current.cloudWaterContent.Mass / area;
				tau_i = SimulationConstants.EXTINCTION_CLOUD_WATER * massPerArea * pathFactor;
				tau += tau_i;
				weightedOmega += tau_i * SimulationConstants.SSA_CLOUD_WATER;

				// Ice
				massPerArea = current.iceParticles.Mass / area;
				tau_i = SimulationConstants.EXTINCTION_ICE * massPerArea * pathFactor;
				tau += tau_i;
				weightedOmega += tau_i * SimulationConstants.SSA_ICE;

				// Aerosols (total)
				float aerosolMass = current.sand.Mass + current.silt.Mass + current.clay.Mass + current.organicMaterial.Mass;
				massPerArea = aerosolMass / area;
				tau_i = SimulationConstants.EXTINCTION_AEROSOL * massPerArea * pathFactor;
				tau += tau_i;
				weightedOmega += tau_i * SimulationConstants.SSA_AEROSOL;

				float omega = tau > 0f ? weightedOmega / tau : 0f;

				// ---- Two‑stream diffuse ----
				float k = Mathf.Sqrt(3f * (1f - omega));
				float exp_kt = Mathf.Exp(k * tau);
				float exp_neg_kt = 1f / exp_kt;
				float denom = (1f + k) * (1f + k) * exp_kt - (1f - k) * (1f - k) * exp_neg_kt;
				float T = 2f * exp_neg_kt / denom;
				float R = ((1f - k * k) * (exp_kt - exp_neg_kt)) / denom;
				float A = 1f - T - R;
				A = Mathf.Clamp(A, 0f, 1f);
				T = Mathf.Clamp(T, 0f, 1f);
				R = Mathf.Clamp(R, 0f, 1f);

				float incidentEnergy = incident.StoredEnergy;
				float absorbed = incidentEnergy * A;
				float reflected = incidentEnergy * R;
				float transmitted = incidentEnergy * T;

				// 1. Add absorbed energy to layer's thermal components (distribute by mass)
				float totalMass = current.Mass;
				if (totalMass > 0f)
				{
					     // Use a temporary pool to avoid multiple TransferEnergy calls? For now, use for loop.
					     ThermalEnergy[] energies = current.energies;
					     for (int i = 0; i < energies.Length; i++)
					     {
						          ThermalEnergy nrg = energies[i];
						          float ratio = nrg.Mass / totalMass;
						          float energyToAdd = ratio * absorbed;
						          if (energyToAdd > 0f)
								EnergyTransferUtilities.TransferEnergy(incident, nrg, energyToAdd);
					     }
				}

				// 2. Remove absorbed + reflected from incident (now incident carries transmitted)
				//incident.RemoveEnergy(absorbed + reflected);  // internal – okay
				if (current.above != null)
				{

					     EnergyTransferUtilities.TransferEnergy(incident, current.above.reflectedSolar, reflected);
				}
				else
				{
					     int len = current.horizontalNeighbors.Length;
					     EnergyTransferUtilities.TransferEnergy(incident, current.horizontalNeighbors[UnityEngine.Random.Range(0, len)].reflectedSolar, reflected);
				}
				//// 3. Store reflected energy in the layer's bucket
				//if (current.reflectedSolar == null)
				//	     current.reflectedSolar = new RadiativeEnergy(incident.Wavelength, reflected);
				//else
				//	     current.below.reflectedSolar.AddEnergy(reflected);   // internal

				current = current.below;
		          }

		          // Surface distribution
		          int tileCount = surfaceTiles.Length;
		          if (tileCount > 0 && incident.StoredEnergy > 0f)
		          {
				float perTile = incident.StoredEnergy / tileCount;
				for (int i = 0; i < tileCount; i++)
				{
					     TileData tile = surfaceTiles[i];
					     //float removed = incident.RemoveEnergy(perTile); // internal
					     //tile.incomingSolarEnergy.AddEnergy(removed);   // internal

					     EnergyTransferUtilities.TransferEnergy(incident, tile.incomingSolarEnergy, perTile);

					     ProcessSurfaceEnergyBalance(tile, hours);
				}
		          }
		          //        if (current.above != null)
		          //        {

		          //EnergyTransferUtilities.TransferEnergy(incident, current.above.reflectedSolar, incident.StoredEnergy);
		          //        }
	     }
	     public static void ProcessSurfaceEnergyBalance(TileData tile, float hours)
	     {
		          if (tile == null)
				return;

		          // 2. Incoming solar energy
		          float incomingEnergy = tile.incomingSolarEnergy.StoredEnergy;
		          if (incomingEnergy <= 0f)
				return;

		          // 3. Split into absorbed and reflected
		          float albedo = tile.albedo;
		          float absorbedEnergy = incomingEnergy * (1f - albedo);
		          float upwellingReflected = incomingEnergy * albedo;

		          // 4. Distribute absorbed energy to components (by mass fraction)
		          float totalMass = tile.surfaceLiquid.Mass + tile.surfaceIce.Mass + tile.composition.Mass;
		          if (totalMass > 0f)
		          {
				float waterRatio = tile.surfaceLiquid.Mass / totalMass;
				float iceRatio = tile.surfaceIce.Mass / totalMass;
				float soilRatio = tile.composition.Mass / totalMass;

				// Liquid gets its share
				float waterEnergy = absorbedEnergy * waterRatio;
				if (waterEnergy > 0f && tile.surfaceLiquid.Mass > 0f)
				{
					     EnergyTransferUtilities.TransferEnergy(tile.incomingSolarEnergy, tile.surfaceLiquid, waterEnergy);
					     ThermalEnergyUtilities.ConductHeat(tile.atmosphereTile.nitrogen, tile.surfaceLiquid, tile.tileWidth * tile.tileWidth, tile.atmosphereTile.layerHeight, hours * 3600);
				}

				// Ice gets its share
				float iceEnergy = absorbedEnergy * iceRatio;
				if (iceEnergy > 0f && tile.surfaceIce.Mass > 0f)
				{
					     EnergyTransferUtilities.TransferEnergy(tile.incomingSolarEnergy, tile.surfaceIce, iceEnergy);
					     ThermalEnergyUtilities.ConductHeat(tile.atmosphereTile.nitrogen, tile.surfaceIce, tile.tileWidth * tile.tileWidth, tile.atmosphereTile.layerHeight, hours * 3600);
				}

				// Soil gets its share
				float soilEnergy = absorbedEnergy * soilRatio;
				if (soilEnergy > 0f && tile.composition.Mass > 0f)
				{
					     ThermalEnergy[] energyData = tile.composition.energy;
					     var sorted = energyData.OrderByDescending(v => v.Mass).ToArray();
					     int i = 0;
					     foreach (var comp in tile.composition.energy)
					     {
						          float compRatio = comp.Mass / tile.composition.Mass;
						          float energyForComp = soilEnergy * compRatio;
						          if (energyForComp > 0f)
						          {
								EnergyTransferUtilities.TransferEnergy(tile.incomingSolarEnergy, comp, energyForComp);
								ThermalEnergyUtilities.ConductHeat(tile.atmosphereTile.nitrogen, sorted[i], tile.tileWidth * tile.tileWidth, tile.atmosphereTile.layerHeight, hours * 3600);
						          }
						          i++;
					     }
					     //   ThermalEnergyUtilities.ConductHeat(tile.atmosphereTile.nitrogen, sorted[0], tile.tileWidth * tile.tileWidth, tile.atmosphereTile.layerHeight, hours);
				}
		          }
		          else
		          {
				if (tile.surfaceIce.Mass > 1f)
				{
					     EnergyTransferUtilities.TransferEnergy(tile.incomingSolarEnergy, tile.surfaceIce, absorbedEnergy);
					     ThermalEnergyUtilities.ConductHeat(tile.atmosphereTile.nitrogen, tile.surfaceIce, tile.tileWidth * tile.tileWidth, tile.atmosphereTile.layerHeight, hours * 3600);
				}
				else
				{

					     // Fallback: all energy to liquid
					     EnergyTransferUtilities.TransferEnergy(tile.incomingSolarEnergy, tile.surfaceLiquid, absorbedEnergy);
					     ThermalEnergyUtilities.ConductHeat(tile.atmosphereTile.nitrogen, tile.surfaceLiquid, tile.tileWidth * tile.tileWidth, tile.atmosphereTile.layerHeight, hours * 3600);
				}
		          }

		          // 5. Move reflected energy to atmosphere
		          if (upwellingReflected > 0f)
		          {
				EnergyTransferUtilities.TransferEnergy(tile.incomingSolarEnergy, tile.atmosphereTile.reflectedSolar, upwellingReflected);
		          }

		          // 6. Process phase changes (melting, evaporation, sublimation, freezing)
		          HydrosphereUtilities.ProcessSurfacePhaseChanges(tile);
	     }
	     public static void ProcessUpwelling(AtmosphereTile bottomTile, TileData[] surfaceTiles, float hours, RadiativeEnergy upwellingStream, float zenithAngle, bool isThermal)
	     {
		          RadiativeEnergy stream = upwellingStream;
		          bool flip = true;
		          AtmosphereTile current = bottomTile;
		          foreach (var tile in surfaceTiles)
		          {
				EnergyTransferUtilities.TransferEnergy(tile.outgoingLongwave, stream, tile.outgoingLongwave.StoredEnergy);
		          }
		          while (current != null) //while ther eisnt a tile above us
		          {
				// Transfer stored upwelling thermal and reflected solar energy into the stream. although I think we should use thermal energy else where for mass transport.
				if (current.upwellingThermal != null && current.upwellingThermal.StoredEnergy > 0f)

					     // float added = current.upwellingThermal.RemoveEnergy(current.upwellingThermal.StoredEnergy);
					     // stream.AddEnergy(added);

					     EnergyTransferUtilities.TransferEnergy(current.upwellingThermal, stream, current.upwellingThermal.StoredEnergy);



				if (current.reflectedSolar != null && current.reflectedSolar.StoredEnergy > 0f)
					     EnergyTransferUtilities.TransferEnergy(current.reflectedSolar, stream, current.reflectedSolar.StoredEnergy);




				float area = current.tileWidth * current.tileWidth;
				float pathFactor = 1f; // diffuse

				// ---- Compute optical depth ----
				float tau = 0f;
				float weightedOmega = 0f;

				// Inline component loop
				float massPerArea = current.waterVapor.Mass / area;
				float tau_i = SimulationConstants.EXTINCTION_WATER_VAPOUR * massPerArea * pathFactor;
				tau += tau_i;
				weightedOmega += tau_i * SimulationConstants.SSA_WATER_VAPOUR;

				massPerArea = current.cloudWaterContent.Mass / area;
				tau_i = SimulationConstants.EXTINCTION_CLOUD_WATER * massPerArea * pathFactor;
				tau += tau_i;
				weightedOmega += tau_i * SimulationConstants.SSA_CLOUD_WATER;

				massPerArea = current.iceParticles.Mass / area;
				tau_i = SimulationConstants.EXTINCTION_ICE * massPerArea * pathFactor;
				tau += tau_i;
				weightedOmega += tau_i * SimulationConstants.SSA_ICE;

				float aerosolMass = current.sand.Mass + current.silt.Mass + current.clay.Mass + current.organicMaterial.Mass;
				massPerArea = aerosolMass / area;
				tau_i = SimulationConstants.EXTINCTION_AEROSOL * massPerArea * pathFactor;
				tau += tau_i;
				weightedOmega += tau_i * SimulationConstants.SSA_AEROSOL;

				float omega = tau > 0f ? weightedOmega / tau : 0f;

				// ---- Two‑stream ----
				float k = Mathf.Sqrt(3f * (1f - omega));
				float exp_kt = Mathf.Exp(k * tau);
				float exp_neg_kt = 1f / exp_kt;
				float denom = (1f + k) * (1f + k) * exp_kt - (1f - k) * (1f - k) * exp_neg_kt;
				float T = 2f * exp_neg_kt / denom;
				float R = ((1f - k * k) * (exp_kt - exp_neg_kt)) / denom;
				float A = 1f - T - R;
				A = Mathf.Clamp(A, 0f, 1f);
				T = Mathf.Clamp(T, 0f, 1f);
				R = Mathf.Clamp(R, 0f, 1f);

				float incidentEnergy = stream.StoredEnergy;
				float absorbed = incidentEnergy * A;
				float reflected = incidentEnergy * R;
				float transmitted = incidentEnergy * T;

				// ---- Thermal emission ----
				float emitted = 0f;
				if (isThermal)
				{
					     float emissivity = A;
					     float tempK = current.temperature;
					     float power = emissivity * Constants.STEFAN_BOLTZMANN * tempK * tempK * tempK * tempK * area;
					     emitted = power * hours * 3600f;
				}

				float netThermal = absorbed - emitted;
				float totalMass = current.Mass;
				if (totalMass > 0f)
				{
					     ThermalEnergy[] energies = current.energies;
					     for (int i = 0; i < energies.Length; i++)
					     {
						          ThermalEnergy nrg = energies[i];
						          float ratio = nrg.Mass / totalMass;
						          float energyChange = netThermal * ratio;
						          if (energyChange > 0f)
								EnergyTransferUtilities.TransferEnergy(stream, nrg, energyChange);
						          else if (energyChange < 0f)
								EnergyTransferUtilities.TransferEnergy(nrg, stream, energyChange * -1);
					     }
				}

				// Remove absorbed + reflected from stream (internal)
				//stream.RemoveEnergy(absorbed + reflected);
				int len = current.horizontalNeighbors.Length;
				int l = UnityEngine.Random.Range(0, len);
				// Store reflected (downward) in current.downwellingSolar
				if (current.below != null)
				{
					     int i = UnityEngine.Random.Range(0, 2);

					     RadiativeEnergy e;
					     if (i == 0)
					     {
						          EnergyTransferUtilities.TransferEnergy(stream, current.below.horizontalNeighbors[UnityEngine.Random.Range(0, len)].reflectedSolar, reflected);
					     }
					     else
						          EnergyTransferUtilities.TransferEnergy(stream, current.below.downwellingSolar, reflected);
				}
				else
				{
					     EnergyTransferUtilities.TransferEnergy(stream, current.horizontalNeighbors[UnityEngine.Random.Range(0, len)].reflectedSolar, reflected);
				}
				//current.downwellingSolar.AddEnergy(reflected);

				// Add emitted radiation to stream (internal)
				//if (isThermal)
				//	     stream.AddEnergy(emitted);

				current = current.above;
		          }

		          // Discard remaining (to space)
		          stream.RemoveEnergy(stream.StoredEnergy);
	     }

	     // Helper to get the correct ThermalEnergy component by name
	     //public static void IntraCellConduction(AtmosphereTile tile, float hours)
	     //{
	     //      //conduct heat between me and every other nrg that is not me.
	     //      ThermalEnergyUtilities.MixTemperaturesByConduction(tile.energies.ToList<ThermalEnergy>(), hours, tile.tileWidth * tile.layerHeight, tile.layerHeight);
	     //}
	     private static List<ThermalEnergy> tempList = new List<ThermalEnergy>(); // static, reused

	     public static void AtmosphericConduction(AtmosphereTile tile, float hours)
	     {
		          float seconds = hours * 3600;

		          tempList.AddRange(tile.energies);
		          if (tile.below == null)
		          {
				tempList.AddRange(tile.above.energies);
		          }
		          if (tile.above == null)
		          {
				tempList.AddRange(tile.below.energies);
		          }
		          //    ThermalEnergyUtilities.MixTemperaturesByConduction(tempList, seconds, tile.tileWidth * tile.tileWidth, tile.layerHeight);
		          // 1. Within‑tile mixing (all components)
		          //      tempList.Clear();
		          tempList.AddRange(tile.energies);
		          //     ThermalEnergyUtilities.MixTemperaturesByConduction(tempList, seconds, tile.tileWidth * tile.layerHeight, tile.layerHeight);

		          // 2. Horizontal neighbours (N,E,S,W)

		          foreach (var neighbor in tile.horizontalNeighbors)
				if (neighbor != null)
					     tempList.AddRange(neighbor.energies);
		          if (tile.below != null)
		          {
				tempList.AddRange(tile.below.energies);
		          }
		          if (tile.above != null)
		          {
				tempList.AddRange(tile.above.energies);
		          }
		          ThermalEnergyUtilities.MixTemperaturesByConduction(tempList, seconds, tile.tileWidth * tile.layerHeight, tile.tileWidth);

		          // 3. Vertical neighbours (above & below)
		          tempList.Clear();

	     }
	     private static ThermalEnergy GetWaterComponent(AtmosphereTile tile, string name)
	     {
		          switch (name)
		          {
		          case "vapor":
		          return tile.waterVapor;
		          case "cloud":
		          return tile.cloudWaterContent;
		          case "ice":
		          return tile.iceParticles;
		          default:
		          return null;
		          }
	     }
	     // Helper: interpolate mass and temperature for a given component selector
	     private static (float mass, float temp) InterpolateMassAndTemp(AtmosphereTile[,,] tiles, int layer, Vector2 departure, int x, int y, int width, int height,
	         Func<AtmosphereTile, ThermalEnergy> selector)
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
				// Fallback: keep current tile's values
				AtmosphereTile current = tiles[layer, x, y];
				ThermalEnergy e = selector(current);
				return (e.Mass, e.Temperature);
		          }

		          ThermalEnergy e00 = selector(t00);
		          ThermalEnergy e10 = selector(t10);
		          ThermalEnergy e01 = selector(t01);
		          ThermalEnergy e11 = selector(t11);

		          // Interpolate mass
		          float mass00 = e00.Mass, mass10 = e10.Mass, mass01 = e01.Mass, mass11 = e11.Mass;
		          float mass = BilinearInterpolate(mass00, mass10, mass01, mass11, fx, fy);

		          // Interpolate temperature as mass‑weighted
		          float totalMass = mass00 + mass10 + mass01 + mass11;
		          if (totalMass > 1e-6f)
		          {
				float weightedTemp00 = e00.Temperature * mass00;
				float weightedTemp10 = e10.Temperature * mass10;
				float weightedTemp01 = e01.Temperature * mass01;
				float weightedTemp11 = e11.Temperature * mass11;
				float weightedSum = BilinearInterpolate(weightedTemp00, weightedTemp10, weightedTemp01, weightedTemp11, fx, fy);
				float temp = mass > 0f ? weightedSum / mass : 0f;
				return (mass, temp);
		          }
		          else
		          {
				// No mass – interpolate temperature directly
				float temp = BilinearInterpolate(e00.Temperature, e10.Temperature, e01.Temperature, e11.Temperature, fx, fy);
				return (mass, temp);
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
						          InitializeHadleyCellHumidity(tile, latitude, layer);
						          float temperature = TileUtilities.GetTemperatureFromLatitude(latitude, 0);
						          float currentTemp = tile.nitrogen.Temperature;
						          float deltaTemp = temperature - currentTemp;
						          float energyToAdd = deltaTemp * tile.nitrogen.Mass * tile.nitrogen.SpecificHeat;
						          tile.nitrogen.AddHeat(energyToAdd);
						          //        foreach (var gas in tile.AtmosphericEnergy)
						          //        {
						          //gas.HeatToTemp(temperature);
						          //        }
						          // Similarly, set water vapor mass and cloud mass using AddMass

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
	     public static void UpdateAtmosphericPressure(AtmosphereTile[,,] atmosphereTiles)
	     {
		          int layers = atmosphereTiles.GetLength(0);
		          int width = atmosphereTiles.GetLength(1);
		          int height = atmosphereTiles.GetLength(2);
		          float R = 287.05f; // specific gas constant for dry air (J/kg·K)

		          for (int layer = 0; layer < layers; layer++)
				for (int x = 0; x < width; x++)
					     for (int y = 0; y < height; y++)
					     {
						          AtmosphereTile tile = atmosphereTiles[layer, x, y];
						          if (tile == null)
								continue;
						          float volume = tile.tileWidth * tile.tileWidth * tile.layerHeight;
						          if (volume <= 0f)
								continue;
						          // Use total mass (dry air + water vapour) as an approximation
						          float totalMass = 0;
						          foreach (var gas in tile.AerosolisedEnergy)
						          {
								totalMass += gas.Mass;
						          }
						          if (totalMass <= 0f)
						          {
								tile.pressure = 0f;
								continue;
						          }
						          // Ideal gas law: p = (m * R * T) / V
						          tile.pressure = (totalMass * R * tile.temperature) / volume;
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
		          float tempC = tile.temperature - SimulationConstants.Kelvin;
		          if (tempC < -SimulationConstants.Kelvin)
				return 0f; // guard

		          float satVP = CalculateSaturationVaporPressure(tempC);
		          float volume = tile.tileWidth * tile.tileWidth * tile.layerHeight;
		          if (volume <= 0f)
				volume = 1f;

		          float molesWater = tile.waterVapor.Mass / 0.018f;
		          float actualVP = (molesWater * Constants.GAS_CONSTANT * tile.temperature) / volume;
		          if (satVP < 0.001f)
				return 0f;
		          return Mathf.Clamp01(actualVP / satVP);
	     }

	     public static float CalculateSaturationVaporPressure(float tempK)
	     {
		          float T = tempK - SimulationConstants.Kelvin;
		          if (T >= 0)
				return 611.2f * Mathf.Exp(17.67f * T / (T + 243.5f));
		          else
				return 611.2f * Mathf.Exp(22.46f * T / (T + 272.62f));
	     }

	     public static float CalculateSaturationWaterMass(float tempK, float pressurePa, float volumeM3)
	     {
		          float temperatureC = tempK - SimulationConstants.Kelvin;
		          float satVP = CalculateSaturationVaporPressure(temperatureC);
		          float mixingRatio = 0.622f * satVP / Mathf.Max(pressurePa - satVP, 1f);
		          float T = temperatureC + SimulationConstants.Kelvin;
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
		          double T = temperature;
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
		          float T = current.temperature;
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

	     public static void ApplyAdiabaticCooling(AtmosphereTile tile, float hours)
	     {
		          // Returns energy change (Joules) – positive = warming, negative = cooling
		          float energyChange = CalculateAdiabaticCooling(tile) * hours * 3600f;
		          if (Mathf.Abs(energyChange) < 1e-6f)
				return;

		          float totalMass = tile.Mass;
		          if (totalMass <= 0f)
				return;

		          // Distribute energy change evenly across all thermal components (by mass fraction)
		          foreach (var nrg in tile.energies)
		          {
				float fraction = nrg.Mass / totalMass;
				if (energyChange > 0)
					     nrg.AddEnergy(energyChange * fraction);
				else
					     nrg.RemoveEnergy(-energyChange * fraction);
		          }
	     }

	     public static float CalculateRadiativeCooling(AtmosphereTile tile, float hours)
	     {
		          if (tile == null)
				return 0f;
		          hours *= 3600f;
		          float tempK = tile.temperature;
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
								if (tile.above != null)
								{
									     neighborPressureSum += tile.above.pressure;
									     neighborCount++;
								}
								if (tile.below != null)
								{
									     neighborPressureSum += tile.below.pressure;
									     neighborCount++;
								}
								if (neighborCount > 0)
								{
									     // Gentle smoothing (10% adjustment per iteration)
									     float avgNeighborPressure = neighborPressureSum / neighborCount;
									     tile.pressure = Mathf.Lerp(tile.pressure, avgNeighborPressure, .8f);
								}
						          }
					     }
				}
		          }
	     }
	     private static float[,,] _newMass;
	     private static float[,,] _newEnergy;

	     public static void EnsureAllocated(int layers, int width, int height)
	     {
		          if (_newMass == null || _newMass.GetLength(0) != layers ||
		              _newMass.GetLength(1) != width || _newMass.GetLength(2) != height)
		          {
				_newMass = new float[layers, width, height];
				_newEnergy = new float[layers, width, height];
		          }
	     }
	     /// <summary>
	     /// Performs 3D semi-Lagrangian advection on all ThermalEnergy components in the atmosphere grid.
	     /// Uses trilinear interpolation to compute new mass and energy at each cell, then overwrites.
	     /// Finally applies a global mass/energy fix to ensure perfect conservation.
	     /// </summary>
	     public static void AdvectAtmosphereSemiLagrangian(
	         AtmosphereTile[,,] atmosphereTiles,
	         float timeStepSeconds)
	     {
		          if (atmosphereTiles == null)
				return;

		          int layers = atmosphereTiles.GetLength(0);
		          int width = atmosphereTiles.GetLength(1);
		          int height = atmosphereTiles.GetLength(2);

		          if (layers == 0 || width == 0 || height == 0)
				return;

		          // --- Step 1: Compute new mass and energy for every cell ---
		          // We do this for each ThermalEnergy component separately
		          // because each has different material properties.

		          // Get all component arrays from the first tile (they're the same for all tiles)
		          AtmosphereTile firstTile = atmosphereTiles[0, 0, 0];
		          if (firstTile == null)
				return;
		          ThermalEnergy[] allComponents = firstTile.energies;

		          // For each component type (e.g., nitrogen, oxygen, waterVapor, etc.)
		          for (int compIdx = 0; compIdx < allComponents.Length; compIdx++)
		          {
				// Compute departure points and interpolate for every cell
				for (int layer = 0; layer < layers; layer++)
				{
					     for (int x = 0; x < width; x++)
					     {
						          for (int y = 0; y < height; y++)
						          {
								AtmosphereTile current = atmosphereTiles[layer, x, y];
								if (current == null)
									     continue;

								// 1. Compute departure point (backwards trajectory)
								Vector3 departure = CalculateDeparturePoint3D(
								    current, x, y, layer, width, height, layers, timeStepSeconds);

								// 2. Interpolate mass and total energy at the departure point
								(float mass, float energy) = TrilinearInterpolateComponent(
								    atmosphereTiles,
								    compIdx,
								    departure,
								    layers, width, height);

								_newMass[layer, x, y] = mass;
								_newEnergy[layer, x, y] = energy;
						          }
					     }
				}

				// --- Step 2: Overwrite all tiles with the interpolated values ---
				for (int layer = 0; layer < layers; layer++)
				{
					     for (int x = 0; x < width; x++)
					     {
						          for (int y = 0; y < height; y++)
						          {
								AtmosphereTile tile = atmosphereTiles[layer, x, y];
								if (tile == null)
									     continue;

								ThermalEnergy component = tile.energies[compIdx];
								if (component == null)
									     continue;

								float mass = _newMass[layer, x, y];
								float energy = _newEnergy[layer, x, y];

								// Set the new mass and total energy directly.
								// This preserves the component's material identity (materialName).
								component.SetMassAndEnergy(mass, energy, 0f);

						          }
					     }
				}
		          }

		          // --- Step 3: Global conservation fix ---
		          // Interpolation has a tiny mass/energy drift. Correct it globally.
		          //FixGlobalConservation(atmosphereTiles);
	     }

	     private static Vector3 CalculateDeparturePoint3D(
    AtmosphereTile current,
    int x, int y, int layer,
    int width, int height, int layers,
    float timeStepSeconds)
	     {
		          if (current == null)
				return new Vector3(x, y, layer);

		          float cellWidth = current.tileWidth;
		          float layerHeight = current.layerHeight;

		          // Wind displacement in grid units
		          float dx = (current.windVelocity.x * timeStepSeconds) / cellWidth;
		          float dy = (current.windVelocity.z * timeStepSeconds) / cellWidth;
		          float dz = (current.windVelocity.y * timeStepSeconds) / layerHeight;

		          // Departure point (backwards in time)
		          float departureX = x - dx;
		          float departureY = y - dy;
		          float departureZ = layer - dz;

		          // Periodic boundary in X (latitude) and Y (longitude)
		          // For latitude (X), clamp to [0, width-1] to handle poles
		          departureX = Mathf.Clamp(departureX, 0f, width - 1f);
		          departureY = ((departureY % height) + height) % height;
		          departureZ = Mathf.Clamp(departureZ, 0f, layers - 1f);

		          return new Vector3(departureX, departureY, departureZ);
	     }
	     private static (float mass, float energy) TricubicInterpolateComponent(
    AtmosphereTile[,,] tiles,
    int compIdx,
    Vector3 departure,
    int layers, int width, int height)
	     {
		          float px = departure.x;
		          float py = departure.y;
		          float pz = departure.z;

		          // Get the integer indices of the cell containing the departure point
		          int x0 = Mathf.FloorToInt(px);
		          int y0 = Mathf.FloorToInt(py);
		          int z0 = Mathf.FloorToInt(pz);

		          // Fractional distances (0 to 1)
		          float fx = px - x0;
		          float fy = py - y0;
		          float fz = pz - z0;

		          // Build 4x4x4 neighbourhood
		          // For cubic interpolation, we need indices: x0-1, x0, x0+1, x0+2
		          // We'll clamp indices to valid ranges
		          int[] xIndices = new int[4];
		          int[] yIndices = new int[4];
		          int[] zIndices = new int[4];

		          // x indices (latitude) - clamp to [0, width-1]
		          for (int i = 0; i < 4; i++)
		          {
				int idx = x0 - 1 + i;
				xIndices[i] = Mathf.Clamp(idx, 0, width - 1);
		          }

		          // y indices (longitude) - wrap around
		          for (int i = 0; i < 4; i++)
		          {
				int idx = y0 - 1 + i;
				yIndices[i] = ((idx % height) + height) % height;
		          }

		          // z indices (vertical) - clamp to [0, layers-1]
		          for (int i = 0; i < 4; i++)
		          {
				int idx = z0 - 1 + i;
				zIndices[i] = Mathf.Clamp(idx, 0, layers - 1);
		          }

		          // Build 4x4x4 grid of mass values and energy values
		          float[,,] massGrid = new float[4, 4, 4];
		          float[,,] energyGrid = new float[4, 4, 4];

		          for (int zi = 0; zi < 4; zi++)
		          {
				for (int yi = 0; yi < 4; yi++)
				{
					     for (int xi = 0; xi < 4; xi++)
					     {
						          int z = zIndices[zi];
						          int y = yIndices[yi];
						          int x = xIndices[xi];

						          ThermalEnergy comp = GetComponent(tiles, z, x, y, compIdx);
						          if (comp != null)
						          {
								massGrid[xi, yi, zi] = comp.Mass;
								energyGrid[xi, yi, zi] = comp.StoredEnergy;
						          }
						          else
						          {
								massGrid[xi, yi, zi] = 0f;
								energyGrid[xi, yi, zi] = 0f;
						          }
					     }
				}
		          }

		          // Perform tricubic interpolation
		          float mass = TricubicInterpolate(massGrid, fx, fy, fz);
		          float energy = TricubicInterpolate(energyGrid, fx, fy, fz);

		          return (mass, energy);
	     }

	     private static float TricubicInterpolate(float[,,] values, float fx, float fy, float fz)
	     {
		          // values is a 4x4x4 grid: values[x, y, z]
		          // fx, fy, fz are fractional positions within the cell [0,1]

		          // Interpolate along X for each (y, z) pair
		          float[,] xInterp = new float[4, 4];
		          for (int y = 0; y < 4; y++)
		          {
				for (int z = 0; z < 4; z++)
				{
					     xInterp[y, z] = CubicInterpolate(
					         values[0, y, z],
					         values[1, y, z],
					         values[2, y, z],
					         values[3, y, z],
					         fx
					     );
				}
		          }

		          // Interpolate along Y for each z
		          float[] zInterp = new float[4];
		          for (int z = 0; z < 4; z++)
		          {
				zInterp[z] = CubicInterpolate(
				    xInterp[0, z],
				    xInterp[1, z],
				    xInterp[2, z],
				    xInterp[3, z],
				    fy
				);
		          }

		          // Interpolate along Z
		          return CubicInterpolate(
		              zInterp[0],
		              zInterp[1],
		              zInterp[2],
		              zInterp[3],
		              fz
		          );
	     }
	     private static float CubicInterpolate(float p0, float p1, float p2, float p3, float t)
	     {
		          // Catmull‑Rom spline: passes through all control points
		          // p1 and p2 are the points on either side of the interval [0,1]
		          float a0 = p1;
		          float a1 = -0.5f * p0 + 0.5f * p2;
		          float a2 = p0 - 2.5f * p1 + 2f * p2 - 0.5f * p3;
		          float a3 = -0.5f * p0 + 1.5f * p1 - 1.5f * p2 + 0.5f * p3;
		          return a0 + a1 * t + a2 * t * t + a3 * t * t * t;
	     }

	     private static (float mass, float energy) TrilinearInterpolateComponent(
    AtmosphereTile[,,] tiles,
    int compIdx,
    Vector3 departure,
    int layers, int width, int height)
	     {
		          float px = departure.x;
		          float py = departure.y;
		          float pz = departure.z;

		          // Get the 8 surrounding cells
		          int x0 = Mathf.FloorToInt(px);
		          int x1 = Mathf.Min(x0 + 1, width - 1);
		          int y0 = Mathf.FloorToInt(py);
		          int y1 = (y0 + 1) % height;
		          int z0 = Mathf.FloorToInt(pz);
		          int z1 = Mathf.Min(z0 + 1, layers - 1);

		          // Fractional distances
		          float fx = px - x0;
		          float fy = py - y0;
		          float fz = pz - z0;

		          // Get the 8 component references
		          ThermalEnergy c000 = GetComponent(tiles, z0, x0, y0, compIdx);
		          ThermalEnergy c100 = GetComponent(tiles, z0, x1, y0, compIdx);
		          ThermalEnergy c010 = GetComponent(tiles, z0, x0, y1, compIdx);
		          ThermalEnergy c110 = GetComponent(tiles, z0, x1, y1, compIdx);
		          ThermalEnergy c001 = GetComponent(tiles, z1, x0, y0, compIdx);
		          ThermalEnergy c101 = GetComponent(tiles, z1, x1, y0, compIdx);
		          ThermalEnergy c011 = GetComponent(tiles, z1, x0, y1, compIdx);
		          ThermalEnergy c111 = GetComponent(tiles, z1, x1, y1, compIdx);

		          // If any are null, fallback to the nearest valid cell
		          if (c000 == null || c100 == null || c010 == null || c110 == null ||
		              c001 == null || c101 == null || c011 == null || c111 == null)
		          {
				// Fallback: use the cell at the departure point (clamped)
				int zc = Mathf.Clamp(Mathf.RoundToInt(pz), 0, layers - 1);
				int xc = Mathf.Clamp(Mathf.RoundToInt(px), 0, width - 1);
				int yc = Mathf.Clamp(Mathf.RoundToInt(py), 0, height - 1);
				ThermalEnergy fallback = GetComponent(tiles, zc, xc, yc, compIdx);
				if (fallback != null)
					     return (fallback.Mass, fallback.StoredEnergy);
				return (0f, 0f);
		          }

		          // Interpolate mass
		          float mass000 = c000.Mass, mass100 = c100.Mass;
		          float mass010 = c010.Mass, mass110 = c110.Mass;
		          float mass001 = c001.Mass, mass101 = c101.Mass;
		          float mass011 = c011.Mass, mass111 = c111.Mass;

		          float mass00 = Mathf.Lerp(mass000, mass100, fx);
		          float mass10 = Mathf.Lerp(mass010, mass110, fx);
		          float mass01 = Mathf.Lerp(mass001, mass101, fx);
		          float mass11 = Mathf.Lerp(mass011, mass111, fx);

		          float mass0 = Mathf.Lerp(mass00, mass10, fy);
		          float mass1 = Mathf.Lerp(mass01, mass11, fy);
		          float mass = Mathf.Lerp(mass0, mass1, fz);

		          // Interpolate energy (using total stored energy)
		          float energy000 = c000.StoredEnergy, energy100 = c100.StoredEnergy;
		          float energy010 = c010.StoredEnergy, energy110 = c110.StoredEnergy;
		          float energy001 = c001.StoredEnergy, energy101 = c101.StoredEnergy;
		          float energy011 = c011.StoredEnergy, energy111 = c111.StoredEnergy;

		          float energy00 = Mathf.Lerp(energy000, energy100, fx);
		          float energy10 = Mathf.Lerp(energy010, energy110, fx);
		          float energy01 = Mathf.Lerp(energy001, energy101, fx);
		          float energy11 = Mathf.Lerp(energy011, energy111, fx);

		          float energy0 = Mathf.Lerp(energy00, energy10, fy);
		          float energy1 = Mathf.Lerp(energy01, energy11, fy);
		          float energy = Mathf.Lerp(energy0, energy1, fz);

		          return (mass, energy);
	     }

	     private static ThermalEnergy GetComponent(
	         AtmosphereTile[,,] tiles,
	         int layer, int x, int y,
	         int compIdx)
	     {
		          if (layer < 0 || layer >= tiles.GetLength(0))
				return null;
		          if (x < 0 || x >= tiles.GetLength(1))
				return null;
		          if (y < 0 || y >= tiles.GetLength(2))
				return null;

		          AtmosphereTile tile = tiles[layer, x, y];
		          if (tile == null || tile.energies == null)
				return null;
		          if (compIdx < 0 || compIdx >= tile.energies.Length)
				return null;

		          return tile.energies[compIdx];
	     }

	     private static float globalInitialMass = -1f;
	     private static float globalInitialEnergy = -1f;

	     //  public static void FixGlobalConservation(AtmosphereTile[,,] tiles)
	     //  {
	     //        int layers = tiles.GetLength(0);
	     //        int width = tiles.GetLength(1);
	     //        int height = tiles.GetLength(2);

	     //        // Calculate current totals
	     //        float totalMass = 0f;
	     //        float totalEnergy = 0f;

	     //        for (int layer = 0; layer < layers; layer++)
	     //        {
	     //for (int x = 0; x < width; x++)
	     //{
	     //	     for (int y = 0; y < height; y++)
	     //	     {
	     //		          AtmosphereTile tile = tiles[layer, x, y];
	     //		          if (tile == null)
	     //				continue;

	     //		          foreach (var comp in tile.energies)
	     //		          {
	     //				if (comp == null)
	     //					     continue;
	     //				totalMass += comp.Mass;
	     //				totalEnergy += comp.StoredEnergy;
	     //		          }
	     //	     }
	     //}
	     //        }

	     //        // Store initial values on first call
	     //        if (globalInitialMass < 0f)
	     //        {
	     //globalInitialMass = totalMass;
	     //globalInitialEnergy = totalEnergy;
	     //return;
	     //        }

	     //        // Prevent division by zero
	     //        if (totalMass <= 0f || totalEnergy <= 0f)
	     //return;

	     //        // Calculate correction factors
	     //        float massCorrection = globalInitialMass / totalMass;
	     //        float energyCorrection = globalInitialEnergy / totalEnergy;

	     //        // Only apply if drift is significant (>0.001%)
	     //        if (Mathf.Abs(massCorrection - 1f) < 0.00001f &&
	     //            Mathf.Abs(energyCorrection - 1f) < 0.00001f)
	     //return;

	     //        // Apply correction to all components
	     //        for (int layer = 0; layer < layers; layer++)
	     //        {
	     //for (int x = 0; x < width; x++)
	     //{
	     //	     for (int y = 0; y < height; y++)
	     //	     {
	     //		          AtmosphereTile tile = tiles[layer, x, y];
	     //		          if (tile == null)
	     //				continue;

	     //		          foreach (var comp in tile.energies)
	     //		          {
	     //				if (comp == null || comp.Mass <= 0f)
	     //					     continue;

	     //				float newMass = comp.Mass * massCorrection;
	     //				float newEnergy = comp.StoredEnergy * energyCorrection;

	     //				// Set the corrected values
	     //				comp.SetMassAndEnergy(newMass, newEnergy, 0f);
	     //		          }
	     //	     }
	     //}
	     //        }
	     //  }
}

