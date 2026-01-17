using System.Collections.Generic;
using UnityEngine;

//instantiates tiles within a grid and  gives  them a grid reference

public class TileManager : MonoBehaviour
{
	     public Vector2Int worldsise;
	     public TileGrid grid;
	     public GameObject tilePrefab;
	     public WorldTile[,] tiles;
	     public float atmosphericMoisture; //in liters
	     public float airTemp;
	     public float totalWater; //all the water in the system, in liters
	     public float totalSoil; //used to track the total soil composition across the world
	     public float totalSand; //total sand in the world
	     public float totalSilt; //total silt in the world
	     public float totalClay; //total clay in the world
	     public float totalOrganicMatter; //total organic matter in the world

	     // 3 layers, each with reduced resolution (10x tile sise)
	     public AtmosphereTile[,,] atmosphereTiles;

	     public Demeter demeter;
	     // Start is called before the first frame update
	     void Start()
	     {
		          grid = new TileGrid(worldsise.x, worldsise.y);
		          grid.tileWidth = 1;
		          grid.XGrid = worldsise.x;
		          grid.YGrid = worldsise.y;
		          tiles = new WorldTile[worldsise.x, worldsise.y];

		          // Atmosphere tile setup
		          int atmTileSise = 10;
		          int atmGridX = Mathf.CeilToInt((float)worldsise.x / atmTileSise);
		          int atmGridY = Mathf.CeilToInt((float)worldsise.y / atmTileSise);
		          int atmLayers = 3;
		          atmosphereTiles = new AtmosphereTile[atmLayers, atmGridX, atmGridY];

		          for (int layer = 0; layer < atmLayers; layer++)
		          {
				int layorFactor = 10;
				for (int i = 0; i < atmGridX; i++)
				{
					     for (int j = 0; j < atmGridY; j++)
					     {
						          AtmosphereTile atmTile = new AtmosphereTile();
						          atmTile.tileGridRef = new Vector2Int(i, j);
						          atmTile.tileWidth = atmTileSise;
						          atmTile.layerHeight = layorFactor;
						          float atmoFactor = (atmTileSise * layorFactor);

						          atmTile.temperature = airTemp;
						          atmTile.waterVapor = atmosphericMoisture / (atmGridX * atmGridY * atmLayers);

						          atmTile.carbonDioxide = 0.481f * atmoFactor;
						          atmTile.nitrogen = 1014 * atmoFactor;
						          atmTile.oxygen = 273 * atmoFactor;
						          atmTile.pressure = 101325f - (layer * 12000f); // Example pressure drop per km	
						          atmTile.windVelocity = (Vector3.right + Random.insideUnitSphere) * Random.Range(0.95f, 1.05f);
						          atmTile.windSpeed = atmTile.windVelocity.magnitude;
						          atmosphereTiles[layer, i, j] = atmTile;
					     }

				}
				layorFactor *= 10; // Increase layer height by factor of 10 for each layer, later 1 is 10m, layer 2 is 100m, layer 3 is 1km 
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
					     data.tileWidth = grid.tileWidth;
					     data.tileGridRef = new Vector2Int(i, j);
					     //allocate longitude and latitude based on the grid position
					     data.latitude = GetLatitude(i, j);
					     data.longitude = GetLongitude(i, j);
					     GameObject tile = Instantiate(tilePrefab, transform);

					     tile.GetComponent<WorldTile>().Setup(data);
					     grid.Map.SetValue(data, i, j);
					     tiles.SetValue(tile.GetComponent<WorldTile>(), i, j);
					     tile.transform.position = new Vector3(i, 1 / 2, j);
					     tile.GetComponent<WorldTile>().tileData.Initialise(TileUtilities.GetValidNeighbour(data, grid));
					     tile.GetComponent<Renderer>().material.color = TileUtilities.GetSoilColor(tile.GetComponent<WorldTile>().tileData.composition);
					     //set the name of the tile based on its grid position
					     tile.name = "Tile " + rowLabel + (j + 1).ToString();

					     // Optionally, link the lowest atmosphere tile to the TileData
					     int atmX = i / atmTileSise;
					     int atmY = j / atmTileSise;
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
		          TileUpdate(1f); // Initial update
		          WakeDemeter();

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
		          //go through tile data and find tiles that have no surface water but some soil moisture.
		          List<TileData> plantPositions = FindPlantPositions();
		          // Set the plant count and initialize
		          demeter.initialPlantCount = plantPositions.Count;
		          demeter.InitializePlants(plantPositions);  // Call this explicitly

	     }
	     public List<TileData> FindPlantPositions()
	     {
		          List<TileData> plantPositions = new List<TileData>();
		          foreach (var tile in tiles)
		          {
				TileData data = tile.tileData;
				if (data.composition.water > 50f && data.surfaceMoisture < 1f)
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
		          // First: Run atmospheric simulation
		          AtmosphereUtilities.SimulateAtmosphere(atmosphereTiles, hours);

		          // Second: Apply precipitation from atmosphere to surface
		          ApplyPrecipitationToSurface(hours);

		          // Third: Update each tile's soil and hydrosphere
		          UpdateAllTiles(hours);

		          // Fourth: Validate conservation
		          UpdateTotals();
	     }

	     /// <summary>
	     /// Applies precipitation from atmosphere tiles to surface tiles
	     /// </summary>
	     private void ApplyPrecipitationToSurface(float hours)
	     {
		          int atmTileSise = 10;
		          int atmGridX = atmosphereTiles.GetLength(1);
		          int atmGridY = atmosphereTiles.GetLength(2);

		          for (int atmX = 0; atmX < atmGridX; atmX++)
		          {
				for (int atmY = 0; atmY < atmGridY; atmY++)
				{
					     AtmosphereTile atmo = atmosphereTiles[0, atmX, atmY];

					     // Only process if there are clouds
					     if (atmo.cloudWaterContent <= 0)
						          continue;

					     // Calculate relative humidity for precipitation check
					     float relativeHumidity = AtmosphereUtilities.CalculateRelativeHumidity(atmo);

					     // Calculate precipitation using the refactored method
					     float precipitation = CalculatePrecipitationForAtmosphereTile(
					         atmo,
					         relativeHumidity,
					         hours
					     );

					     if (precipitation <= 0)
						          continue;

					     // Distribute precipitation to surface tiles under this atmosphere tile
					     DistributePrecipitationToSurfaceTiles(atmX, atmY, precipitation, hours);

					     // Remove precipitated water from atmosphere
					     atmo.cloudWaterContent -= precipitation;

					     // Debug logging
					     if (precipitation > 0.1f)
					     {
						          Debug.Log($"Precipitation: {precipitation}kg from atmosphere tile ({atmX},{atmY})");
					     }
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
		          if (atmosphereTile == null || atmosphereTile.cloudWaterContent <= 0)
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
		          float cloudFactor = Mathf.Clamp01(atmosphereTile.cloudWaterContent / 1000f);

		          // Total precipitation rate in mm/hour
		          float precipRate = baseRate * humidityFactor * cloudFactor;

		          // Convert to kg for this atmosphere tile area
		          float tileArea = atmosphereTile.tileWidth * atmosphereTile.tileWidth;
		          float precipMass = precipRate * tileArea * hours; // mm * m² = kg

		          // Cannot precipitate more than available cloud water
		          precipMass = Mathf.Min(precipMass, atmosphereTile.cloudWaterContent * 0.8f);

		          return precipMass;
	     }

	     /// <summary>
	     /// Distributes precipitation to individual surface tiles
	     /// </summary>
	     private void DistributePrecipitationToSurfaceTiles(
	         int atmX, int atmY,
	         float totalPrecipitation,
	         float hours)
	     {
		          int atmTileSise = 10;
		          int startX = atmX * atmTileSise;
		          int startY = atmY * atmTileSise;
		          int endX = Mathf.Min(startX + atmTileSise, worldsise.x);
		          int endY = Mathf.Min(startY + atmTileSise, worldsise.y);

		          int tileCount = (endX - startX) * (endY - startY);
		          if (tileCount == 0)
				return;

		          // Calculate precipitation per surface tile (kg)
		          float precipitationPerTile = totalPrecipitation / tileCount;

		          for (int x = startX; x < endX; x++)
		          {
				for (int y = startY; y < endY; y++)
				{
					     WorldTile tile = tiles[x, y];
					     if (tile == null)
						          continue;

					     TileData data = tile.tileData;

					     // Add precipitation to surface moisture
					     data.surfaceMoisture += precipitationPerTile;

					     // Apply cooling from precipitation
					     ApplyPrecipitationCooling(data, precipitationPerTile, hours);

					     // If temperature is below freezing, handle snow/ice
					     if (data.atmosphereTile.temperature < 0)
					     {
						          // In a more complete system, you'd track snow accumulation
						          data.surfaceTemperature -= precipitationPerTile * 0.005f;
					     }
				}
		          }
	     }

	     /// <summary>
	     /// Applies cooling effect from precipitation
	     /// </summary>
	     private void ApplyPrecipitationCooling(TileData data, float precipitation, float hours)
	     {
		          if (precipitation <= 0)
				return;

		          // Rain cools the surface when it's warmer than the rain temperature
		          float rainTemperature = data.atmosphereTile.temperature;
		          float temperatureDifference = data.surfaceTemperature - rainTemperature;

		          if (temperatureDifference > 0)
		          {
				// Simple cooling model: rain absorbs heat from surface
				float coolingEffect = precipitation * temperatureDifference * 0.1f * hours;
				data.surfaceTemperature = Mathf.Max(
				    data.surfaceTemperature - coolingEffect,
				    rainTemperature
				);
		          }
	     }

	     /// <summary>
	     /// Updates all tiles' soil and hydrosphere
	     /// </summary>
	     private void UpdateAllTiles(float hours)
	     {
		          for (int x = 0; x < worldsise.x; x++)
		          {
				for (int y = 0; y < worldsise.y; y++)
				{
					     WorldTile tile = tiles[x, y];
					     if (tile == null)
						          continue;

					     TileData data = tile.tileData;

					     // Calculate solar radiation for this tile
					     float solarRadiation = SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(
					         data.latitude,
					         SimulationConstants.DayOfYear,
					         SimulationConstants.TimeOfDay
					     );

					     // Update hydrosphere for this tile
					     UpdateTileHydrosphere(data, solarRadiation, hours);

					     // Update soil processes
					     UpdateTileSoil(data, hours, data.latitude, solarRadiation);

					     // Visual update
					     tile.TileVisualUpdate();
				}
		          }
	     }

	     /// <summary>
	     /// Updates hydrosphere for a single tile
	     /// </summary>
	     private void UpdateTileHydrosphere(TileData data, float solarRadiation, float hours)
	     {
		          if (data == null || data.atmosphereTile == null)
				return;

		          // Use the refactored HydrosphereUtilities
		          HydrosphereUtilities.UpdateHydrosphere(
		              data.atmosphereTile,
		              data,
		              solarRadiation,
		              hours
		          );
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
	     /// <summary>
	     /// Updates soil processes for a single tile
	     /// </summary>
	     private void UpdateTileSoil(TileData data, float hours, float latitude, float solarRadiation)
	     {
		          if (data == null)
				return;

		          // Calculate solar energy for soil heating
		          float absorbedEnergy = solarRadiation;

		          // Update soil temperature with proper energy balance
		          float soilMass = SoilUtilities.GetSoilMass(data.composition);
		          float heatCapacity = SoilUtilities.GetSoilHeatCapacity(data.composition);

		          if (soilMass > 0 && heatCapacity > 0)
		          {
				// Energy gain from solar radiation
				float solarTempIncrease = (absorbedEnergy * hours * 3600f) / (soilMass * heatCapacity);

				// Energy exchange with air
				float airTempDiff = data.atmosphereTile.temperature - data.soilTemperature;
				float conductiveTransfer = airTempDiff * 0.01f * hours;

				// Apply changes
				data.soilTemperature += solarTempIncrease + conductiveTransfer;
		          }

		          // Apply soil processes
		          ApplySoilProcesses(data, hours);
	     }

	     /// <summary>
	     /// Applies various soil processes
	     /// </summary>
	     private void ApplySoilProcesses(TileData data, float hours)
	     {
		          if (data == null)
				return;

		          // Get the world tile for this data
		          WorldTile tile = tiles[data.tileGridRef.x, data.tileGridRef.y];
		          if (tile == null)
				return;

		          // Apply soil utilities
		          SoilUtilities.InfiltrateSurfaceMoisture(data, hours);
		          SoilUtilities.ApplySelfCompaction(data, hours);
		          SoilUtilities.UpdateDecompaction(data.composition, hours);
		          SoilUtilities.CalculateDerivedProperties(data.composition);

		          // Redistribute water (surface and lateral)
		          HydrosphereUtilities.RedistributeSurfaceWater(tile, hours);
		          SoilUtilities.RedistributeWaterLateral(tile, hours);
	     }
	     public void SimulateSoilHour(TileData data, float hours, float latitude)
	     {
		          // Calculate solar energy ONCE
		          float absorbedEnergy = SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(
		              latitude, SimulationConstants.DayOfYear + 1, SimulationConstants.TimeOfDay + 0.0001f);

		          // Update soil temperature with proper energy balance
		          float soilMass = SoilUtilities.GetSoilMass(data.composition);
		          float heatCapacity = SoilUtilities.GetSoilHeatCapacity(data.composition);

		          if (soilMass > 0 && heatCapacity > 0)
		          {
				// Energy gain from solar radiation
				float solarTempIncrease = (absorbedEnergy * hours * 3600f) / (soilMass * heatCapacity);

				// Energy exchange with air
				float airTempDiff = data.atmosphereTile.temperature - data.soilTemperature;
				float conductiveTransfer = airTempDiff * 0.01f * hours; // Simple conduction

				// Apply changes
				data.soilTemperature += solarTempIncrease + conductiveTransfer;

				// Evaporation cooling
				float evaporation = SoilUtilities.ApplyEvaporation(
				    data, absorbedEnergy, data.atmosphereTile.RelativeHumidity,
				    data.atmosphereTile.temperature, data.atmosphereTile.windSpeed, hours);
		          }

		          // Update soil temperature  and evaporate water
		          SoilUtilities.UpdateSoilTemperature(data, absorbedEnergy, data.atmosphereTile.temperature, hours);
		          // Get heat from soil and accumulate it
		          float energyFromTile = SoilUtilities.PassHeatFromSoil(data, hours, this);

		          // Now apply the accumulated energy to warm the atmosphere:
		          float airMass = data.atmosphereTile.tileWidth * data.atmosphereTile.tileWidth * data.atmosphereTile.layerHeight * Constants.AIR_DENSITY;
		          float deltaAirTemp = energyFromTile / (airMass * Constants.AIR_SPECIFIC_HEAT);
		          data.atmosphereTile.temperature += deltaAirTemp;


		          //lateral water movement
		          HydrosphereUtilities.RedistributeSurfaceWater(tiles[data.tileGridRef.x, data.tileGridRef.y], hours);
		          SoilUtilities.RedistributeWaterLateral(tiles[data.tileGridRef.x, data.tileGridRef.y], hours);
		          SoilUtilities.InfiltrateSurfaceMoisture(data, hours);
		          SoilUtilities.ApplySelfCompaction(data, hours);
		          SoilUtilities.UpdateDecompaction(data.composition, hours);

		          float rh = data.atmosphereTile.RelativeHumidity;
		          float evaporationTemp = (data.atmosphereTile.temperature + data.soilTemperature) * 0.5f;

		          SoilUtilities.CalculateDerivedProperties(data.composition);
	     }
	     public void AllocateNeighbors()
	     {
		          for (int x = 0; x < worldsise.x; x++)
		          {
				for (int y = 0; y < worldsise.y; y++)
				{
					     WorldTile current = tiles[x, y];

					     // Wrapping coordinates
					     int xLeft = (x - 1 + worldsise.x) % worldsise.x;
					     int xRight = (x + 1) % worldsise.x;
					     int yDown = (y - 1 + worldsise.y) % worldsise.y;
					     int yUp = (y + 1) % worldsise.y;

					     // Assign neighbors with wrapping
					     current.NTile = tiles[x, yUp];
					     current.STile = tiles[x, yDown];
					     current.WTile = tiles[xLeft, y];
					     current.ETile = tiles[xRight, y];

					     current.NWTile = tiles[xLeft, yUp];
					     current.NETile = tiles[xRight, yUp];
					     current.SWTile = tiles[xLeft, yDown];
					     current.SETile = tiles[xRight, yDown];
				}
		          }
	     }

	     // Call this before and after simulation steps to check for conservation
	     public void UpdateTotals()
	     {
		          float waterSum = 0f;
		          SoilComposition soilSum = new SoilComposition();

		          // Sum water from all world tiles
		          for (int i = 0; i < worldsise.x; i++)
		          {
				for (int j = 0; j < worldsise.y; j++)
				{
					     var comp = tiles[i, j].tileData.composition;
					     waterSum += comp.water + tiles[i, j].tileData.surfaceMoisture;

					     soilSum.sand += comp.sand;
					     soilSum.silt += comp.silt;
					     soilSum.clay += comp.clay;
					     soilSum.organicMatter += comp.organicMatter;
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
						          waterSum += atmosphereTiles[layer, i, j].waterVapor;
						          waterSum += atmosphereTiles[layer, i, j].cloudWaterContent;
						          waterSum += atmosphereTiles[layer, i, j].liquidWater;
					     }
				}
		          }

		          totalWater = waterSum;
		          totalSand = soilSum.sand;
		          totalSilt = soilSum.silt;
		          totalClay = soilSum.clay;
		          totalOrganicMatter = soilSum.organicMatter;
	     }
	     public void SimulateAtmosphereStep(float hours)
	     {
		          int atmLayers = atmosphereTiles.GetLength(0);
		          int atmGridX = atmosphereTiles.GetLength(1);
		          int atmGridY = atmosphereTiles.GetLength(2);

		          // 1. Calculate solar radiation absorption
		          float latitude = GetComponent<SimulationMaster>().currentLatitude;
		          float solarEnergy = SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(
		              latitude, SimulationConstants.DayOfYear, SimulationConstants.TimeOfDay);

		          // Distribute solar energy to top layer
		          for (int x = 0; x < atmGridX; x++)
		          {
				for (int y = 0; y < atmGridY; y++)
				{

				}
		          }

		          // 2. Run atmospheric physics simulation
		          AtmosphereUtilities.SimulateAtmosphere(atmosphereTiles, hours);

		          // 4. Handle cloud formation and precipitation


		          // 3. Handle surface-atmosphere interactions
		          for (int x = 0; x < atmGridX; x++)
		          {
				for (int y = 0; y < atmGridY; y++)
				{
					     AtmosphereTile surfaceLayer = atmosphereTiles[0, x, y];

					     // Calculate evaporation from surface tiles
					     int startX = x * 10; // Assuming 10:1 ratio
					     int startY = y * 10;
					     int endX = Mathf.Min(startX + 10, worldsise.x);
					     int endY = Mathf.Min(startY + 10, worldsise.y);

					     float totalEvaporation = 0f;
					     for (int i = startX; i < endX; i++)
					     {
						          for (int j = startY; j < endY; j++)
						          {

						          }
					     }
					     //here we need to update the surface layer moisture and temperature and ensure that we 					     

				}
		          }


		          // 5. Update global atmospheric properties
		          UpdateGlobalAtmosphere();
	     }


	     /// <summary>
	     /// Distributes precipitation to the surface tiles under an atmospheric cell
	     /// </summary>
	     private void DistributePrecipitationToSurface(int atmX, int atmY, float amount)
	     {
		          int tileSise = 10; // Matching the initialization ratio
		          int startX = atmX * tileSise;
		          int startY = atmY * tileSise;
		          int endX = Mathf.Min(startX + tileSise, worldsise.x);
		          int endY = Mathf.Min(startY + tileSise, worldsise.y);

		          int tileCount = (endX - startX) * (endY - startY);
		          if (tileCount == 0)
				return;

		          float perTile = amount / tileCount;

		          for (int x = startX; x < endX; x++)
		          {
				for (int y = startY; y < endY; y++)
				{
					     tiles[x, y].tileData.surfaceMoisture += perTile;
					     tiles[x, y].tileData.atmosphereTile.waterVapor -= perTile; // Also update atmosphere tile
				}
		          }
		          Debug.Log($"Distributed {amount} kg of precipitation to surface tiles at ({atmX}, {atmY})");
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
						          totalMoisture += cell.waterVapor;
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

		          // Calculate max values for normalization
		          float maxMoisture = 0f;
		          float maxWindSpeed = 0f;
		          for (int layer = 0; layer < atmLayers; layer++)
		          {
				for (int x = 0; x < atmGridX; x++)
				{
					     for (int y = 0; y < atmGridY; y++)
					     {
						          AtmosphereTile atmo = atmosphereTiles[layer, x, y];
						          if (atmo != null)
						          {
								maxMoisture = Mathf.Max(maxMoisture, atmo.waterVapor);
								maxWindSpeed = Mathf.Max(maxWindSpeed, atmo.windSpeed);
						          }
					     }
				}
		          }

		          for (int layer = 0; layer < atmLayers; layer++)
		          {
				for (int x = 0; x < atmGridX; x++)
				{
					     for (int y = 0; y < atmGridY; y++)
					     {
						          AtmosphereTile atmo = atmosphereTiles[layer, x, y];
						          if (atmo == null)
								continue;

						          // Calculate world position for the center of the tile
						          float worldX = (x + 0.5f) * atmo.tileWidth;
						          float worldY = layer * 1.2f * atmo.layerHeight + 2f; // Offset above ground, stack layers
						          float worldZ = (y + 0.5f) * atmo.tileWidth;
						          Vector3 pos = new Vector3(worldX, worldY, worldZ);


						          // Visualise humidity with color and sise
						          Gizmos.color = GetHumidityColor(atmo.RelativeHumidity);
						          float humiditySise = 0.3f + atmo.RelativeHumidity * 0.7f;
						          Gizmos.DrawSphere(pos + Vector3.down, humiditySise);

						          //visualise cloud water content with a semi transparent sphere
						          if (atmo.cloudWaterContent > 0)
						          {
								Gizmos.color = new Color(1f, 1f, 1f, Mathf.Clamp01(atmo.cloudWaterContent / 10f));
								float cloudSise = atmo.cloudWaterContent / 1000f;
								Gizmos.DrawSphere(pos + new Vector3(0, 2, 0), cloudSise);
						          }

						          // Visualise wind with direction arrows
						          if (maxWindSpeed > 0)
						          {
								Gizmos.color = GetWindColor(atmo.windSpeed, maxWindSpeed);
								Vector3 windDir = atmo.windVelocity.normalized;
								float windStrength = atmo.windSpeed / maxWindSpeed;
								Gizmos.DrawLine(pos, pos + windDir * (0.5f + windStrength * 2f));
								DrawArrow(pos, pos + windDir * (0.5f + windStrength * 2f), 0.2f);
						          }

						          // Visualise temperature with wireframe cube sise
						          Gizmos.color = GetTemperatureColor(atmo.temperature);
						          float tempSise = 0.2f + Mathf.InverseLerp(-20f, 40f, atmo.temperature) * 0.5f;
						          Gizmos.DrawWireCube(pos + Vector3.up * 0.1f, Vector3.one * tempSise);

						          // Draw labels in editor
#if UNITY_EDITOR
						          string label = $"RH: {atmo.RelativeHumidity * 100f:F0}%\n";
						          label += $"T: {atmo.temperature:F1}°C\n";
						          label += $"W: {atmo.windSpeed:F2}m/s";
						          UnityEditor.Handles.Label(pos + Vector3.up * 0.5f, label);
#endif
					     }
				}
		          }
	     }

	     // Helper method to draw direction arrows
	     void DrawArrow(Vector3 start, Vector3 end, float arrowSise)
	     {
		          Gizmos.DrawLine(start, end);
		          Vector3 direction = (end - start).normalized;
		          Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 45, 0) * Vector3.forward;
		          Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 45, 0) * Vector3.forward;
		          Gizmos.DrawLine(end, end + right * arrowSise);
		          Gizmos.DrawLine(end, end + left * arrowSise);
	     }

	     // Color utilities for visualization
	     Color GetHumidityColor(float relativeHumidity)
	     {
		          // Blue gradient for humidity
		          return Color.Lerp(
		              new Color(0.7f, 0.7f, 1f, 0.5f),    // Light blue (dry)
		              new Color(0f, 0f, 1f, 0.8f),        // Dark blue (humid)
		              relativeHumidity
		          );
	     }

	     Color GetWindColor(float windSpeed, float maxWindSpeed)
	     {
		          // Red-yellow gradient for wind speed
		          float normalisedSpeed = windSpeed / maxWindSpeed;
		          return Color.Lerp(
		              Color.yellow,
		              Color.red,
		              normalisedSpeed
		          );
	     }

	     Color GetTemperatureColor(float tempC)
	     {
		          // Blue-red gradient for temperature
		          float normalisedTemp = Mathf.InverseLerp(-20f, 40f, tempC);
		          return Color.Lerp(
		              Color.blue,
		              Color.red,
		              normalisedTemp
		          );
	     }
}

