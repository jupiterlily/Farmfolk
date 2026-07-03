using System;
using System.Collections.Generic;
using UnityEngine;

//instantiates tiles within a grid and  gives  them a grid reference

public class TileManager : MonoBehaviour
{
	     public Vector2Int worldsise;
	     public TileGrid grid;
	     public int tilesize = 100;
	     public GameObject tilePrefab;
	     public WorldTile[,] tiles;
	     public int atmoRatio = 10;
	     int atmTileSise => (int)grid.tileWidth * atmoRatio;
	     public float atmosphericMoisture; //in liters per m2
	     public float airTemp; //in celcius so add 272.15
	     public float totalWater; //all the water in the system, in liters
	     public float totalSoil; //used to track the total soil composition across the world
	     public float totalSand; //total sand in the world
	     public float totalSilt; //total silt in the world
	     public float totalClay; //total clay in the world
	     public float totalOrganicMaterial; //total organic matter in the world
	     public float totalNRG;
	     public List<Sprite> leafSprites;
	     // 3 layers, each with reduced resolution (10x tile sise)
	     public AtmosphereTile[,,] atmosphereTiles;

	     public Demeter demeter;
	     // Start is called before the first frame update
	     void Start()
	     {
		          grid = new TileGrid(worldsise.x, worldsise.y, tilesize);
		          grid.XGrid = worldsise.x;
		          grid.YGrid = worldsise.y;
		          tiles = new WorldTile[worldsise.x, worldsise.y];

		          // Atmosphere tile setup

		          int atmGridX = Mathf.CeilToInt((float)worldsise.x / atmoRatio);
		          int atmGridY = Mathf.CeilToInt((float)worldsise.y / atmoRatio);
		          int atmLayers = 8;
		          atmosphereTiles = new AtmosphereTile[atmLayers, atmGridX, atmGridY];
		          int layorFactor = 2;
		          //  atmosphericMoisture *= atmoRatio * (atmGridX * atmGridY * atmLayers);
		          for (int layer = 0; layer < atmLayers; layer++)
		          {

				for (int i = 0; i < atmGridX; i++)
				{
					     for (int j = 0; j < atmGridY; j++)
					     {

						          AtmosphereTile atmTile = new AtmosphereTile();
						          atmTile.tileGridRef = new Vector2Int(i, j);
						          atmTile.tileWidth = atmTileSise;
						          atmTile.layerHeight = layorFactor;
						          float atmoFactor = (atmTileSise * layorFactor);
						          float targetTempK = airTemp + SimulationConstants.Kelvin;
						          float volume = atmTileSise * atmTileSise * layorFactor; // m³
										      // In your atmosphere initialization loop
						          float targetRH = 0.75f; // 75% humidity to start
						          float satMass = AtmosphereUtilities.CalculateSaturationWaterMass(targetTempK, atmTile.pressure, volume);
						          float vaporMass = satMass * targetRH * UnityEngine.Random.Range(0.9f, 1.1f);
						          atmTile.waterVapor.AddMass(vaporMass, targetTempK);
						          // Do NOT initialize cloud water with the same mass; make it very small.
						          atmTile.cloudWaterContent.AddMass(atmosphericMoisture, targetTempK);
						          //atmTile.waterVapor.AddHeat(atmTile.waterVapor.SpecificHeat * atmTile.waterVapor.Mass * targetTempK);
						          // Compute volume of this atmosphere tile


						          // Gas constants
						          float R_specific_air = 287.05f; // J/(kg·K) for dry air
								            //float T_K = targetTempK;        // already in Kelvin
						          atmTile.pressure = 101325f - (layer * 12000f); // Example pressure drop per km	
									            // Total air mass from ideal gas law: m = P * V / (R * T)
						          float totalAirMass = (atmTile.pressure * volume) / (R_specific_air * targetTempK);

						          // Mass fractions (by mass) – approximate dry air
						          float fracN2 = 0.755f;
						          float fracO2 = 0.231f;
						          float fracCO2 = 0.0004f; // you can increase for simulation

						          // Add masses
						          atmTile.nitrogen.AddMass(totalAirMass * fracN2, targetTempK);
						          atmTile.oxygen.AddMass(totalAirMass * fracO2, targetTempK);
						          atmTile.carbonDioxide.AddMass(totalAirMass * fracCO2, targetTempK);



						          atmTile.windVelocity = (Vector3.right + UnityEngine.Random.insideUnitSphere) * UnityEngine.Random.Range(0.95f, 1.05f);
						          atmTile.windSpeed = atmTile.windVelocity.magnitude;
						          atmosphereTiles[layer, i, j] = atmTile;
					     }

				}
				layorFactor *= 3; // Increase layer height by factor of 10 for each layer, later 1 is 10m, layer 2 is 100m, layer 3 is 1km 
					   // Increase layer height by factor of 3 for each layer, later 1 is 3m, 2 is 9m, 3 is 27m, 4 is 81m, 5 is 243m, 6 is 729m, 7 is 2187m (2.187km) 
		          }

		          for (int i = 0; i < grid.XGrid; i++)
		          {
				//row numbering
				//if i  exceeds 26, start double letters
				char rowChar = (char)('A' + (i % 26));
				string rowLabel = (i / 26 > 0) ? ((char)('A' + (i / 26 - 1))).ToString() + rowChar.ToString() : rowChar.ToString();

				for (int j = 0; j < grid.YGrid; j++)
				{
					     TileData data = new TileData();
					     data.tileWidth = tilesize;
					     data.tileGridRef = new Vector2Int(i, j);
					     //allocate longitude and latitude based on the grid position
					     data.latitude = GetLatitude(i, j);
					     data.longitude = GetLongitude(i, j);
					     GameObject tile = Instantiate(tilePrefab, transform);

					     tile.GetComponent<WorldTile>().Setup(data);
					     grid.Map.SetValue(data, i, j);
					     tiles.SetValue(tile.GetComponent<WorldTile>(), i, j);
					     tile.transform.position = new Vector3(i, 0.5f, j);
					     tile.GetComponent<WorldTile>().tileData.Initialise(TileUtilities.GetValidNeighbour(data, grid), airTemp);
					     tile.GetComponent<Renderer>().material.color = TileUtilities.GetSoilColor(tile.GetComponent<WorldTile>().tileData.composition);
					     //set the name of the tile based on its grid position
					     tile.name = "Tile " + rowLabel + (j + 1).ToString();

					     // Optionally, link the lowest atmosphere tile to the TileData
					     int atmX = i / atmoRatio;
					     int atmY = j / atmoRatio;
					     data.atmosphereTile = atmosphereTiles[0, atmX, atmY];
				}
		          }

		          // connect atmosphere tiles
		          for (int layer = 0; layer < atmLayers; layer++)
		          {
				for (int i = 0; i < atmGridX; i++)
				{
					     for (int j = 0; j < atmGridY; j++)
					     {
						          AtmosphereTile atmTile = atmosphereTiles[layer, i, j];

						          // Connect to layer below
						          if (layer > 0)
						          {
								atmTile.below = atmosphereTiles[layer - 1, i, j];
						          }

						          // Connect to layer above
						          if (layer < atmLayers - 1)
						          {
								atmTile.above = atmosphereTiles[layer + 1, i, j];
						          }
					     }
				}
		          }
		          AllocateNeighbors();
		          AssignAtmosphereHorizontalNeighbors();
		          AtmosphereUtilities.InitializeHadleyCellWinds(atmosphereTiles, this);
		          AtmosphereUtilities.FinalizePressurePatterns(atmosphereTiles);
		          //  AtmosphereStep(10f); // Initial atmosphere update
		          int k = atmLayers * 2;
		          for (int i = 0; i < k; i++)
		          {
				AtmosphereStep(0.5f); // Initial atmosphere update
		          }

		          TileUpdate(0.5f); // Initial update
		          TileUpdate(0.5f); // Initial update
		          TileUpdate(0.5f); // Initial update
		          TileUpdate(0.5f); // Initial update
		          TileUpdate(0.5f); // Initial update
			             // WakeDemeter();

	     }
	     void WakeDemeter()
	     {
		          demeter = new Demeter();
		          // Initialize systems early in the lifecycle
		          demeter.lightManager = new LightManager(demeter.NightTint);
		          demeter.gardener = new Gardener();
		          demeter.tileManager = this;
		          demeter.xBounds = new Vector2(0, worldsise.x);
		          demeter.yBounds = new Vector2(0, worldsise.y);
		          demeter.leafSprites = leafSprites;
		          //go through tile data and find tiles that have no surface water but some soil moisture.
		          List<TileData> plantPositions = FindPlantPositions();
		          // Set the plant count and initialize
		          demeter.initialPlantCount = Math.Min(100, plantPositions.Count);
		          demeter.InitializePlants(plantPositions);  // Call this explicitly

	     }
	     public List<TileData> FindPlantPositions()
	     {
		          List<TileData> plantPositions = new List<TileData>();
		          foreach (var tile in tiles)
		          {
				TileData data = tile.tileData;
				if (data.composition.water.Mass > 5f && data.surfaceWater.Mass < 10f && (data.composition.organicMaterial.Mass > 1f || data.composition.clay.Mass > 1f || data.composition.silt.Mass > 1f || data.composition.sand.Mass > 1f))
				{
					     plantPositions.Add(data);
				}
		          }
		          return plantPositions;
	     }
	     /// <summary>
	     /// Main update method - called from SimulationMaster
	     /// </summary>
	     public void TileUpdate(float hours)
	     {


		          // HydrosphereUtilities.InfiltrateAndFlow();

		          //HydrosphereUtilities.Evaporate();
		          foreach (var tile in grid.Map)
		          {
				ApplySoilProcesses(tile, hours);
		          }
		          //SoilUtilities.ProcessWindErosion();
		          // Fifth: Update each tile's soil and hydrosphere
		          UpdateAllTiles(hours);

		          // Sixth: Validate conservation
		          UpdateTotals();
	     }
	     public void AtmosphereStep(float hours)
	     {
		          int atmLayers = atmosphereTiles.GetLength(0);
		          int atmGridX = atmosphereTiles.GetLength(1);
		          int atmGridY = atmosphereTiles.GetLength(2);


		          // --- Phase 1: Column-local radiative transfer ---
		          for (int x = 0; x < atmGridX; x++)
		          {
				for (int y = 0; y < atmGridY; y++)
				{
					     // Get column
					     AtmosphereTile top = atmosphereTiles[atmLayers - 1, x, y];   // layer with index atmLayers-1
					     AtmosphereTile bottom = atmosphereTiles[0, x, y];            // layer 0
					     TileData[] under = GetSurfaceTilesUnderAtmo(x, y);

					     // Compute solar energy for this column
					     int halfIndex = under.Length / 2;
					     TileData center = under[halfIndex];
					     float solarPower = SolarEnergyCalculator.CalculateSolarEnergyAtLocation(
					         center.latitude, center.longitude, SimulationConstants.DayOfYear);
					     float area = top.tileWidth * top.tileWidth;
					     float totalEnergy = solarPower * area * hours * 3600f;

					     Vector3 lightDir = SolarEnergyCalculator.CalculateLightAngleAtLocation(
					         center.latitude, center.longitude, SimulationConstants.DayOfYear);
					     float zenithAngle = Vector3.Angle(Vector3.up, lightDir);

					     // Downwelling
					     RadiativeEnergy incoming = new RadiativeEnergy(SimulationConstants.SunlightWavelength, totalEnergy);
					     AtmosphereUtilities.ProcessDownwelling(top, under, hours, incoming, zenithAngle);


					     // Combine them (or process separately)
					     RadiativeEnergy upStream = new RadiativeEnergy(SimulationConstants.SunlightWavelength, 0);
					     AtmosphereUtilities.ProcessUpwelling(bottom, under, hours, upStream, 0f, true);
				}
		          }
		          foreach (var tile in grid.Map)
		          {
				// Process the surface energy balance and get the reflected solar
				AtmosphereUtilities.ProcessSurfaceEnergyBalance(tile, hours);

		          }


		          foreach (var tile in atmosphereTiles)
		          {
				AtmosphereUtilities.AtmosphericConduction(tile, hours);
				AtmosphereUtilities.ApplyAdiabaticCooling(tile, hours);
				//HydrosphereUtilities.ApplyAtmosphericPhaseChanges(tile, hours);
				//TileData[] under = GetSurfaceTilesUnderAtmo(tile.tileGridRef.x, tile.tileGridRef.y);
				tile.CollectGases();

		          }
		          // --- Phase 2: Global atmospheric dynamics (ONCE per timestep) ---
		          // Advection (moves air, vapor, clouds)
		          AtmosphereUtilities.ApplyAtmosphericAdvection(atmosphereTiles, hours);

		          AtmosphereUtilities.UpdateAtmosphericPressure(atmosphereTiles);
		          AtmosphereUtilities.UpdateAtmosphericWind(atmosphereTiles, hours);

		          // Latent heat release (condensation) – you need to implement this
		          //ProcessLatentHeat(atmosphereTiles, hours);
		          AtmosphereUtilities.FinalizePressurePatterns(atmosphereTiles);
		          // Precipitation (from clouds to surface)
		          ApplyPrecipitationToSurface(hours);
		          foreach (var tile in grid.Map)
		          {
				//  surface.CollectGases();
				HydrosphereUtilities.UpdateSurfaceHydrosphere(tile.atmosphereTile, tile, tile.incomingSolarEnergy.RemoveEnergy(tile.incomingSolarEnergy.StoredEnergy), hours);
				tile.CollectGases();
		          }
		          // --- Phase 3: Surface processes (ONCE per timestep) ---
		          //    TileUpdate(hours);

		          // --- Phase 4: Diagnostics (ONCE per timestep) ---
		          //UpdateTotals();
	     }


	     private TileData[] GetSurfaceTilesUnderAtmo(int atmX, int atmY)
	     {
		          int startX = atmX * atmoRatio;
		          int startY = atmY * atmoRatio;
		          int endX = Mathf.Min(startX + atmoRatio, worldsise.x);
		          int endY = Mathf.Min(startY + atmoRatio, worldsise.y);
		          List<TileData> list = new List<TileData>();
		          for (int x = startX; x < endX; x++)
				for (int y = startY; y < endY; y++)
					     list.Add(tiles[x, y].tileData);
		          return list.ToArray();
	     }
	     private void ApplyPrecipitationToSurface(float hours)
	     {
		          int atmGridX = atmosphereTiles.GetLength(1);
		          int atmGridY = atmosphereTiles.GetLength(2);

		          for (int atmX = 0; atmX < atmGridX; atmX++)
		          {
				for (int atmY = 0; atmY < atmGridY; atmY++)
				{
					     AtmosphereTile atmo = atmosphereTiles[0, atmX, atmY];
					     if (atmo.cloudWaterContent.Mass <= 0)
						          continue;

					     float relativeHumidity = AtmosphereUtilities.CalculateRelativeHumidity(atmo);
					     float precipitation = CalculatePrecipitationForAtmosphereTile(atmo, relativeHumidity, hours);
					     if (precipitation <= 0)
						          continue;

					     // Pass the cloud and the total mass
					     DistributePrecipitationToSurfaceTiles(atmX, atmY, atmo.cloudWaterContent, precipitation, hours);
				}
		          }
	     }

	     /// <summary>
	     /// Calculates precipitation for an atmosphere tile
	     /// </summary>
	     private float CalculatePrecipitationForAtmosphereTile(
	         AtmosphereTile atmosphereTile,
	         float relativeHumidity,
	         float hours)
	     {
		          if (atmosphereTile == null || atmosphereTile.cloudWaterContent.Mass <= 0)
				return 0f;

		          // Basic precipitation conditions
		          if (relativeHumidity < 0.95f)
				return 0f;

		          // Calculate dew point
		          float dewPoint = AtmosphereUtilities.CalculateDewPoint(
		              atmosphereTile.temperature,
		              relativeHumidity
		          );

		          // Precipitation intensity depends on temperature below dew point
		          float temperatureBelowDewPoint = dewPoint - atmosphereTile.temperature;

		          if (temperatureBelowDewPoint <= 0)
				return 0f;

		          // Base precipitation rate: 0-20 mm/hour based on conditions
		          float baseRate = Mathf.Lerp(0f, 20f, Mathf.Clamp01(temperatureBelowDewPoint / 10f));

		          // Adjust for relative humidity
		          float humidityFactor = Mathf.Pow(relativeHumidity, 3f);

		          // Adjust for cloud water content
		          float cloudFactor = Mathf.Clamp01(atmosphereTile.cloudWaterContent.Mass / 1000f);

		          // Total precipitation rate in mm/hour
		          float precipRate = baseRate * humidityFactor * cloudFactor;

		          // Convert to kg for this atmosphere tile area
		          float tileArea = atmosphereTile.tileWidth * atmosphereTile.tileWidth;
		          float precipMass = precipRate * tileArea * hours; // mm * m² = kg

		          // Cannot precipitate more than available cloud water
		          precipMass = Mathf.Min(precipMass, atmosphereTile.cloudWaterContent.Mass * 0.8f);

		          return precipMass;
	     }

	     private void DistributePrecipitationToSurfaceTiles(
	         int atmX, int atmY,
	         ThermalEnergy cloudWater,   // the cloud's ThermalEnergy
	         float totalPrecipMass,      // total kg to precipitate (pre‑computed)
	         float hours)
	     {
		          if (totalPrecipMass <= 0)
				return;

		          int startX = atmX * atmoRatio;
		          int startY = atmY * atmoRatio;
		          int endX = Mathf.Min(startX + atmoRatio, worldsise.x);
		          int endY = Mathf.Min(startY + atmoRatio, worldsise.y);
		          int tileCount = (endX - startX) * (endY - startY);
		          if (tileCount == 0)
				return;

		          float perTile = totalPrecipMass / tileCount;

		          for (int x = startX; x < endX; x++)
		          {
				for (int y = startY; y < endY; y++)
				{
					     WorldTile tile = tiles[x, y];
					     if (tile == null)
						          continue;
					     TileData data = tile.tileData;

					     // Transfer directly from cloud to surface – no temporary object
					     ThermalEnergyUtilities.TransferMassAndEnergy(cloudWater, data.surfaceWater, perTile);
				}
		          }

	     }

	     private void UpdateAllTiles(float hours)
	     {
		          for (int x = 0; x < worldsise.x; x++)
		          {
				for (int y = 0; y < worldsise.y; y++)
				{
					     WorldTile tile = tiles[x, y];
					     if (tile == null)
						          continue;
					     tile.TileVisualUpdate();
				}
		          }
	     }

	     //get latitude of a cell based on its grid position
	     public float GetLatitude(float x, float y)
	     {
		          // Assuming latitude varies linearly across the grid
		          return (x / (float)worldsise.x) * 180 - 90f; // Map x to -90 to 90 range
	     }
	     //get longitude of a cell based on its grid position
	     public float GetLongitude(float x, float y)
	     {
		          // Assuming longitude varies linearly across the grid
		          return (y / (float)worldsise.y) * 360f - 180f; // Map y to -180 to 180 range
	     }

	     private void ApplySoilProcesses(TileData data, float hours)
	     {
		          if (data == null)
				return;

		          WorldTile tile = tiles[data.tileGridRef.x, data.tileGridRef.y];
		          if (tile == null)
				return;

		          // Soil physical processes
		          SoilUtilities.InfiltrateSurfaceMoisture(data, hours);
		          SoilUtilities.ApplySelfCompaction(data, hours);
		          SoilUtilities.UpdateDecompaction(data.composition, hours);
		          SoilUtilities.CalculateDerivedProperties(data.composition);

		          // Water redistribution
		          HydrosphereUtilities.RedistributeSurfaceWater(tile, hours);
		          SoilUtilities.RedistributeWaterLateral(tile, hours);

		          // 3. Surface‑atmosphere conduction (already in UpdateSoilTemperature, but we can keep it)
		          //    We'll keep the call to SoilUtilities.UpdateSoilTemperature which handles surface‑air conduction.
		          SoilUtilities.UpdateSoilTemperature(data, hours);
	     }

	     /// <summary>
	     /// Computes a bulk thermal conductivity for the soil (excluding air).
	     /// Uses a weighted harmonic mean (series model) for layered materials, or a geometric mean.
	     /// For simplicity, we use a weighted arithmetic mean based on volume fractions.
	     /// </summary>
	     private float ComputeBulkThermalConductivity(SoilComposition sc)
	     {
		          // Thermal conductivities (W/m·K) - approximate values
		          float kSand = 1.1f;
		          float kSilt = 2.0f;
		          float kClay = 2.5f;
		          float kOrganic = 0.2f;
		          float kWater = 0.6f;

		          // Volume fractions (m³) – using mass / density
		          float volSand = sc.sand.Mass / Constants.SAND_DENSITY;
		          float volSilt = sc.silt.Mass / Constants.SILT_DENSITY;
		          float volClay = sc.clay.Mass / Constants.CLAY_DENSITY;
		          float volOrganic = sc.organicMaterial.Mass / Constants.ORGANIC_DENSITY;
		          float volWater = sc.water.Mass / 1000f; // water density ~1000 kg/m³
		          float totalVol = volSand + volSilt + volClay + volOrganic + volWater;
		          if (totalVol <= 0f)
				return 0.5f; // fallback

		          // Weighted arithmetic mean (isotropic approximation)
		          float k = (volSand * kSand + volSilt * kSilt + volClay * kClay + volOrganic * kOrganic + volWater * kWater) / totalVol;
		          return Mathf.Clamp(k, 0.1f, 5f);
	     }


	     public void AllocateNeighbors()
	     {
		          int worldLatitude = worldsise.x;  // X = latitude (south to north)
		          int worldLongitude = worldsise.y; // Y = longitude (wraps)

		          int halfLongitude = worldLongitude / 2;
		          int atmoCounter = atmoRatio;

		          for (int x = 0; x < worldLatitude; x++)  // x = latitude index
		          {
				for (int y = 0; y < worldLongitude; y++)  // y = longitude index
				{
					     WorldTile current = tiles[x, y];

					     // Longitude wraps (east-west)
					     int yWest = (y - 1 + worldLongitude) % worldLongitude;
					     int yEast = (y + 1) % worldLongitude;

					     // For poles, all longitudes converge to a point
					     if (x == 0)  // South Pole (latitude = -90°)
					     {
						          // North neighbor: go toward equator (increase latitude)
						          current.NTile = tiles[x + 1, y];

						          // South neighbor: doesn't exist at South Pole
						          // We'll make it wrap to opposite longitude at same pole
						          int oppositeY = (y + halfLongitude) % worldLongitude;
						          current.STile = tiles[x, oppositeY];

						          // East and West: along same latitude (South Pole)
						          current.WTile = tiles[x, yWest];
						          current.ETile = tiles[x, yEast];

						          // Diagonals
						          // NW and NE: combine north and horizontal movement
						          current.NWTile = tiles[x + 1, yWest];
						          current.NETile = tiles[x + 1, yEast];

						          // SW and SE: stay at South Pole but opposite longitude
						          int swY = (int)(halfLongitude * 0.5f) % worldLongitude;
						          int seY = (int)(halfLongitude * 1.5f) % worldLongitude;
						          current.SWTile = tiles[x, swY];
						          current.SETile = tiles[x, seY];
					     }
					     else if (x == worldLatitude - 1)  // North Pole (latitude = +90°)
					     {
						          // North neighbor: doesn't exist at North Pole
						          // Wrap to opposite longitude at same pole
						          int oppositeY = (y + halfLongitude) % worldLongitude;
						          current.NTile = tiles[x, oppositeY];

						          // South neighbor: go toward equator (decrease latitude)
						          current.STile = tiles[x - 1, y];

						          // East and West: along same latitude (North Pole)
						          current.WTile = tiles[x, yWest];
						          current.ETile = tiles[x, yEast];

						          // Diagonals
						          // NW and NE: stay at North Pole but opposite longitude
						          int nwY = (int)(halfLongitude * 0.5f) % worldLongitude;
						          int neY = (int)(halfLongitude * 1.5f) % worldLongitude;
						          current.NWTile = tiles[x, nwY];
						          current.NETile = tiles[x, neY];

						          // SW and SE: combine south and horizontal movement
						          current.SWTile = tiles[x - 1, yWest];
						          current.SETile = tiles[x - 1, yEast];
					     }
					     else  // Normal tile (not at pole)
					     {
						          // Standard assignment
						          current.NTile = tiles[x + 1, y];  // North = increase latitude
						          current.STile = tiles[x - 1, y];  // South = decrease latitude
						          current.WTile = tiles[x, yWest];  // West = decrease longitude (wrap)
						          current.ETile = tiles[x, yEast];  // East = increase longitude (wrap)

						          // Diagonals
						          current.NWTile = tiles[x + 1, yWest];
						          current.NETile = tiles[x + 1, yEast];
						          current.SWTile = tiles[x - 1, yWest];
						          current.SETile = tiles[x - 1, yEast];
					     }
					     if (atmoCounter <= 0)
					     {
						          current.tileData.atmosphereTile.horizontalNeighbors[0] = tiles[x, (y - 1 + worldLongitude) % worldLongitude].tileData.atmosphereTile;
						          current.tileData.atmosphereTile.horizontalNeighbors[1] = tiles[x, (y + 1) % worldLongitude].tileData.atmosphereTile;
						          current.tileData.atmosphereTile.horizontalNeighbors[2] = tiles[Mathf.Max(0, x - 1), y].tileData.atmosphereTile;
						          current.tileData.atmosphereTile.horizontalNeighbors[3] = tiles[Mathf.Min(worldLatitude - 1, x + 1), y].tileData.atmosphereTile;
						          atmoCounter = atmoRatio;
					     }
					     else
					     {
						          atmoCounter--;
					     }
				}
		          }
	     }

	     private void AssignAtmosphereHorizontalNeighbors()
	     {
		          int layers = atmosphereTiles.GetLength(0);
		          int atmX = atmosphereTiles.GetLength(1);
		          int atmY = atmosphereTiles.GetLength(2);

		          for (int layer = 0; layer < layers; layer++)
		          {
				for (int x = 0; x < atmX; x++)
				{
					     for (int y = 0; y < atmY; y++)
					     {
						          AtmosphereTile current = atmosphereTiles[layer, x, y];
						          if (current == null)
								continue;

						          // Longitude wrapping (east-west)
						          int yWest = (y - 1 + atmY) % atmY;
						          int yEast = (y + 1) % atmY;

						          // Latitude pole handling (north-south)
						          int xNorth, xSouth;
						          if (x == 0) // South Pole
						          {
								// North: toward equator
								xNorth = 1;
								// South: stays at pole, opposite longitude
								int oppositeY = (y + atmY / 2) % atmY;
								current.horizontalNeighbors[0] = atmosphereTiles[layer, x, oppositeY]; // North (actually South, but we'll assign)
								current.horizontalNeighbors[2] = atmosphereTiles[layer, x, oppositeY]; // South
						          }
						          else if (x == atmX - 1) // North Pole
						          {
								// South: toward equator
								xSouth = atmX - 2;
								// North: stays at pole, opposite longitude
								int oppositeY = (y + atmY / 2) % atmY;
								current.horizontalNeighbors[0] = atmosphereTiles[layer, x, oppositeY]; // North
								current.horizontalNeighbors[2] = atmosphereTiles[layer, x, oppositeY]; // South
						          }
						          else
						          {
								xNorth = x + 1;
								xSouth = x - 1;
								current.horizontalNeighbors[0] = atmosphereTiles[layer, xNorth, y]; // North
								current.horizontalNeighbors[2] = atmosphereTiles[layer, xSouth, y]; // South
						          }

						          // East-West (always wrap)
						          current.horizontalNeighbors[1] = atmosphereTiles[layer, x, yEast];  // East
						          current.horizontalNeighbors[3] = atmosphereTiles[layer, x, yWest];  // West
					     }
				}
		          }
	     }
	     // Call this before and after simulation steps to check for conservation
	     public void UpdateTotals()
	     {
		          float waterSum = 0f;
		          SoilComposition soilSum = new SoilComposition();
		          totalSand = 0f;
		          totalSilt = 0f;
		          totalClay = 0f;
		          totalOrganicMaterial = 0f;
		          // Sum water from all world tiles
		          for (int i = 0; i < worldsise.x; i++)
		          {
				for (int j = 0; j < worldsise.y; j++)
				{
					     var comp = tiles[i, j].tileData.composition;
					     waterSum += comp.water.Mass + tiles[i, j].tileData.surfaceWater.Mass;

					     totalSand += comp.sand.Mass;
					     totalSilt += comp.silt.Mass;
					     totalClay += comp.clay.Mass;
					     totalOrganicMaterial += comp.organicMaterial.Mass;
				}
		          }

		          // Sum water from all atmosphere tiles
		          int atmLayers = atmosphereTiles.GetLength(0);
		          int atmGridX = atmosphereTiles.GetLength(1);
		          int atmGridY = atmosphereTiles.GetLength(2);
		          for (int layer = 0; layer < atmLayers; layer++)
		          {
				for (int i = 0; i < atmGridX; i++)
				{
					     for (int j = 0; j < atmGridY; j++)
					     {
						          waterSum += atmosphereTiles[layer, i, j].waterVapor.Mass;
						          waterSum += atmosphereTiles[layer, i, j].cloudWaterContent.Mass;
						          waterSum += atmosphereTiles[layer, i, j].iceParticles.Mass;
						          totalSand += atmosphereTiles[layer, i, j].sand.Mass;
						          totalSilt
								+= atmosphereTiles[layer, i, j].silt.Mass;
						          totalClay += atmosphereTiles[layer, i, j].clay.Mass;
						          totalOrganicMaterial += atmosphereTiles[layer, i, j].organicMaterial.Mass;
					     }
				}
		          }
		          foreach (var plant in demeter.ActivePlants.Keys)
		          {
				waterSum += plant.storedWater.Mass;
		          }
		          totalWater = waterSum;
	     }
	     /// <summary>
	     /// Updates global atmospheric properties by averaging all cells
	     /// </summary>
	     private void UpdateGlobalAtmosphere()
	     {
		          float totalTemp = 0f;
		          float totalMoisture = 0f;
		          int count = 0;

		          int atmLayers = atmosphereTiles.GetLength(0);
		          int atmGridX = atmosphereTiles.GetLength(1);
		          int atmGridY = atmosphereTiles.GetLength(2);

		          for (int layer = 0; layer < atmLayers; layer++)
		          {
				for (int x = 0; x < atmGridX; x++)
				{
					     for (int y = 0; y < atmGridY; y++)
					     {
						          AtmosphereTile cell = atmosphereTiles[layer, x, y];
						          totalTemp += cell.temperature;
						          totalMoisture += cell.waterVapor.Mass;
						          count++;
					     }
				}
		          }

		          // Update global averages
		          airTemp = totalTemp / count;
		          atmosphericMoisture = totalMoisture;
	     }
	     void OnDrawGizmos()
	     {
		          if (atmosphereTiles == null || atmosphereTiles.Length == 0)
				return;

		          int atmLayers = atmosphereTiles.GetLength(0);
		          int atmGridX = atmosphereTiles.GetLength(1);
		          int atmGridY = atmosphereTiles.GetLength(2);

		          // ----- 1. Draw surface tiles (moisture & temperature) -----
		          DrawSurfaceTilesGizmos();

		          // ----- 2. Draw atmosphere columns (stacked layers) -----
		          for (int layer = 0; layer < atmLayers; layer++)
		          {
				for (int x = 0; x < atmGridX; x++)
				{
					     for (int y = 0; y < atmGridY; y++)
					     {
						          AtmosphereTile atmo = atmosphereTiles[layer, x, y];
						          if (atmo == null)
								continue;

						          Vector3 pos = GetAtmoTileWorldPos(x, y, layer, atmo);

						          // ---- a) Humidity sphere (blue gradient) ----
						          float rh = atmo.RelativeHumidity;
						          Gizmos.color = new Color(0.2f, 0.2f, 0.8f + 0.2f * rh, 0.6f);
						          float humSize = 0.3f + 0.7f * rh;
						          Gizmos.DrawSphere(pos, humSize);

						          // ---- b) Cloud water (white fluffy cloud) ----
						          if (atmo.cloudWaterContent.Mass > 0.01f)
						          {
								float cloudMass = atmo.cloudWaterContent.Mass;
								float cloudSize = 0.2f + 0.8f * Mathf.Clamp01(cloudMass / 10f);
								// Draw several overlapping spheres for fluffiness
								Gizmos.color = new Color(1f, 1f, 1f, 0.4f);
								Gizmos.DrawSphere(pos + new Vector3(0, 0.5f, 0), cloudSize);
								Gizmos.DrawSphere(pos + new Vector3(0.3f, 0.6f, 0.2f), cloudSize * 0.7f);
								Gizmos.DrawSphere(pos + new Vector3(-0.2f, 0.4f, -0.3f), cloudSize * 0.6f);
								Gizmos.DrawSphere(pos + new Vector3(0.1f, 0.8f, -0.1f), cloudSize * 0.5f);
						          }

						          // ---- c) Temperature cube (colour‑coded) ----
						          float tempC = atmo.temperature - SimulationConstants.Kelvin;
						          Color tempColor = GetTemperatureColor(tempC);
						          Gizmos.color = tempColor;
						          float tempSize = 0.3f + Mathf.Clamp01(Mathf.Abs(tempC) / 60f) * 0.5f;
						          Gizmos.DrawWireCube(pos + Vector3.up * 0.1f, Vector3.one * tempSize);

						          // ---- d) Wind arrow (dynamic) ----
						          if (atmo.windSpeed > 0.01f)
						          {
								float maxWind = 30f; // for normalisation
								float speedNorm = Mathf.Clamp01(atmo.windSpeed / maxWind);
								Color windColor = Color.Lerp(Color.green, Color.red, speedNorm);
								Gizmos.color = windColor;
								Vector3 windDir = atmo.windVelocity.normalized;
								float arrowLength = 0.5f + speedNorm * 2f;
								Vector3 end = pos + windDir * arrowLength;
								Gizmos.DrawLine(pos, end);
								DrawArrowHead(pos, end, 0.3f);
						          }

						          // ---- e) Pressure as a vertical bar ----
						          float pressureNorm = Mathf.Clamp01(atmo.pressure / 110000f);
						          Gizmos.color = new Color(0.8f, 0.8f, 0.2f, 0.5f);
						          Vector3 barBase = pos + new Vector3(0.8f, -0.3f, 0);
						          Vector3 barTop = barBase + Vector3.up * pressureNorm * 0.8f;
						          Gizmos.DrawLine(barBase, barTop);
						          Gizmos.DrawWireCube((barBase + barTop) / 2, new Vector3(0.1f, 0.8f * pressureNorm, 0.1f));

						          // ---- f) Debug label (optional) ----
#if UNITY_EDITOR
						          string label = $"L{layer}\n{tempC:F0}°C\n{rh:P0}";
						          UnityEditor.Handles.Label(pos + Vector3.up * 1.2f, label);
#endif
					     }
				}
		          }
	     }

	     private Vector3 GetAtmoTileWorldPos(int x, int y, int layer, AtmosphereTile atmo)
	     {
		          // X and Z: visual grid. Each atm tile covers 'atmoRatio' surface tiles (spacing 1).
		          float visualSpacing = atmoRatio; // == atmTileSise / tilesize
		          float worldX = (x + 0.5f) * visualSpacing;
		          float worldZ = (y + 0.5f) * visualSpacing;

		          // Y: visual stacking.
		          // Option A: fixed vertical separation (good for debugging)
		          float worldY = 1.0f + layer * 1.5f;

		          // Option B: physically scaled (uncomment if you prefer)
		          // float cumulativeHeight = 0f;
		          // // We don't have the array here, but you could precompute layerHeights in Start.
		          // // For now, approximate using the same logic as your loop:
		          // int h = 2;
		          // for (int i = 0; i < layer; i++) { cumulativeHeight += h; h *= 3; }
		          // cumulativeHeight += atmo.layerHeight; // current layer's top
		          // float worldY = 0.5f + cumulativeHeight / tilesize;

		          return new Vector3(worldX, worldY, worldZ);
	     }

	     // Helper: draw an arrow head at the end of a line
	     private void DrawArrowHead(Vector3 start, Vector3 end, float size)
	     {
		          Vector3 dir = (end - start).normalized;
		          Vector3 right = Vector3.Cross(dir, Vector3.up).normalized;
		          if (right.magnitude < 0.01f)
				right = Vector3.right;
		          Vector3 up = Vector3.Cross(right, dir).normalized;
		          Vector3 back = -dir;
		          Vector3[] points = new Vector3[3];
		          points[0] = end + back * size + right * size;
		          points[1] = end + back * size - right * size;
		          points[2] = end + back * size + up * size;
		          Gizmos.DrawLine(end, points[0]);
		          Gizmos.DrawLine(end, points[1]);
		          Gizmos.DrawLine(end, points[2]);
		          Gizmos.DrawLine(points[0], points[1]);
		          Gizmos.DrawLine(points[0], points[2]);
		          Gizmos.DrawLine(points[1], points[2]);
	     }

	     // Surface tiles Gizmos
	     private void DrawSurfaceTilesGizmos()
	     {
		          if (tiles == null)
				return;
		          for (int x = 0; x < worldsise.x; x++)
		          {
				for (int y = 0; y < worldsise.y; y++)
				{
					     WorldTile tile = tiles[x, y];
					     if (tile == null)
						          continue;
					     TileData data = tile.tileData;

					     Vector3 pos = new Vector3(x * data.tileWidth, 0.1f, y * data.tileWidth);

					     // ---- a) Soil temperature ----
					     float soilTempC = data.soilTemperature - SimulationConstants.Kelvin;
					     Color soilColor = GetTemperatureColor(soilTempC);
					     Gizmos.color = soilColor;
					     Gizmos.DrawCube(pos, new Vector3(0.8f, 0.05f, 0.8f));

					     // ---- b) Surface water depth (translucent disc) ----
					     float waterDepth = data.surfaceWater.Mass / 1000f; // m (assuming 1m² area)
					     if (waterDepth > 0.001f)
					     {
						          float maxDepth = 0.5f; // visual max
						          float depthNorm = Mathf.Clamp01(waterDepth / maxDepth);
						          Color waterColor = Color.Lerp(new Color(0.2f, 0.4f, 0.8f, 0.3f), new Color(0.0f, 0.2f, 0.6f, 0.7f), depthNorm);
						          Gizmos.color = waterColor;
						          Vector3 waterPos = pos + Vector3.up * 0.05f;
						          Gizmos.DrawCube(waterPos, new Vector3(0.9f, 0.02f + depthNorm * 0.1f, 0.9f));
					     }

					     // ---- c) Soil moisture as small sphere (if > 0) ----
					     float soilMoist = data.composition.water.Mass / data.composition.waterCapacity;
					     if (soilMoist > 0.01f)
					     {
						          Gizmos.color = new Color(0.0f, 0.6f, 0.8f, 0.8f);
						          float moistureSize = 0.05f + 0.15f * soilMoist;
						          Gizmos.DrawSphere(pos + new Vector3(0.4f, 0.05f, 0.4f), moistureSize);
					     }

					     // ---- d) Elevation/volume as a wire box ----
					     float vol = data.volume;
					     Gizmos.color = new Color(0.8f, 0.8f, 0.8f, 0.2f);
					     Gizmos.DrawWireCube(pos + Vector3.up * vol / 2, new Vector3(1f, vol, 1f));

#if UNITY_EDITOR
					     // Optional: label with water depth
					     if (waterDepth > 0.01f)
						          UnityEditor.Handles.Label(pos + Vector3.up * 0.3f, waterDepth.ToString("F2") + "m");
#endif
				}
		          }
	     }

	     // Temperature colour mapping (same as before, but you can make it more vibrant)
	     private Color GetTemperatureColor(float tempC)
	     {
		          // Map -30°C → blue, 0°C → green, 30°C → red, 60°C → orange
		          float t = Mathf.Clamp01((tempC + 30f) / 90f);
		          if (t < 0.333f)
				return Color.Lerp(Color.blue, Color.cyan, t * 3f);
		          else if (t < 0.666f)
				return Color.Lerp(Color.cyan, Color.yellow, (t - 0.333f) * 3f);
		          else
				return Color.Lerp(Color.yellow, Color.red, (t - 0.666f) * 3f);
	     }
}
