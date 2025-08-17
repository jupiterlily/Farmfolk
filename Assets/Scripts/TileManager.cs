using UnityEngine;

//instantiates tiles within a grid and  gives  them a grid reference

public class TileManager : MonoBehaviour
{
	     public Vector2Int worldsize;
	     public TileGrid grid;
	     public GameObject tilePrefab;
	     public WorldTile[,] tiles;
	     public float atmosphericMoisture; //in liters
	     public float airTemp;
	     public float humidity; //the 
	     public float totalWater; //all the water in the system, in liters
	     public float totalSoil; //used to track the total soil composition across the world
	     public float totalSand; //total sand in the world
	     public float totalSilt; //total silt in the world
	     public float totalClay; //total clay in the world
	     public float totalOrganicMatter; //total organic matter in the world

	     // 3 layers, each with reduced resolution (10x tile size)
	     public AtmosphereTile[,,] atmosphereTiles;
	     // Start is called before the first frame update
	     void Start()
	     {
		          grid = new TileGrid(worldsize.x, worldsize.y);
		          grid.tileWidth = 1;
		          grid.XGrid = worldsize.x;
		          grid.YGrid = worldsize.y;
		          tiles = new WorldTile[worldsize.x, worldsize.y];

		          // Atmosphere tile setup
		          int atmTileSize = 10;
		          int atmGridX = Mathf.CeilToInt((float)worldsize.x / atmTileSize);
		          int atmGridY = Mathf.CeilToInt((float)worldsize.y / atmTileSize);
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
						          atmTile.tileWidth = atmTileSize;
						          atmTile.layerHeight = layorFactor;

						          atmTile.temperature = airTemp;
						          atmTile.moisture = atmosphericMoisture / (atmGridX * atmGridY * atmLayers);
						          atmTile.pressure = 101325f - (layer * 12000f); // Example pressure drop per km
						          atmTile.windDirection = Vector3.right;
						          atmTile.windSpeed = 0.01f;
						          atmosphereTiles[layer, i, j] = atmTile;
					     }

				}
				layorFactor *= 10; // Increase layer height by factor of 10 for each layer, later 1 is 10m, layer 2 is 100m, layer 3 is 1km 
		          }

		          for (int i = 0; i < grid.XGrid; i++)
		          {
				for (int j = 0; j < grid.YGrid; j++)
				{
					     TileData data = new TileData();
					     data.tileWidth = grid.tileWidth;
					     data.tileGridRef = new Vector2Int(i, j);

					     GameObject tile = Instantiate(tilePrefab, transform);

					     tile.GetComponent<WorldTile>().Setup(data);
					     grid.Map.SetValue(data, i, j);
					     tiles.SetValue(tile.GetComponent<WorldTile>(), i, j);
					     tile.transform.position = new Vector3(i, 1 / 2, j);
					     tile.GetComponent<WorldTile>().tileData.Initialize(TileUtilities.GetValidNeighbour(data, grid));
					     tile.GetComponent<Renderer>().material.color = TileUtilities.GetSoilColor(tile.GetComponent<WorldTile>().tileData.composition);

					     // Optionally, link the lowest atmosphere tile to the TileData
					     int atmX = i / atmTileSize;
					     int atmY = j / atmTileSize;
					     data.atmosphereTile = atmosphereTiles[0, atmX, atmY];
				}
		          }
		          AllocateNeighbors();

	     }
	     public void TileUpdate(float hours)
	     {
		          float latitude = gameObject.GetComponent<SimulationMaster>().currentLatitude;
		          int atmTileSize = 10;
		          int atmGridX = atmosphereTiles.GetLength(1);
		          int atmGridY = atmosphereTiles.GetLength(2);

		          // First, process precipitation for each atmosphere tile in the lowest layer
		          for (int atmX = 0; atmX < atmGridX; atmX++)
		          {
				for (int atmY = 0; atmY < atmGridY; atmY++)
				{
					     AtmosphereTile atmo = atmosphereTiles[0, atmX, atmY];

					     // Calculate precipitation for this atmosphere tile
					     float precipMass = CalculateCurrentPrecipitation(
					         hours,
					         atmo.moisture,
					         CalculateRelativeHumidity(atmo),
					         atmo.temperature
					     );

					     // Determine which world tiles are covered by this atmosphere tile
					     int startX = atmX * atmTileSize;
					     int startY = atmY * atmTileSize;
					     int endX = Mathf.Min(startX + atmTileSize, worldsize.x);
					     int endY = Mathf.Min(startY + atmTileSize, worldsize.y);

					     int coveredTiles = (endX - startX) * (endY - startY);
					     float kgPerTile = coveredTiles > 0 ? precipMass / coveredTiles : 0f;

					     // Apply precipitation to each covered world tile
					     for (int i = startX; i < endX; i++)
					     {
						          for (int j = startY; j < endY; j++)
						          {
								WorldTile tile = tiles[i, j];
								TileData data = tile.tileData;

								SoilUtilities.RedistributeSurfaceWater(tile, hours);
								SoilUtilities.RedistributeWaterLateral(tile, hours);
								SimulateSoilHour(data, hours,
								    SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(latitude, SimulationConstants.DayOfYear, SimulationConstants.TimeOfDay),
								    kgPerTile, latitude);

								tile.TileVisualUpdate(latitude);
						          }
					     }

					     // Remove total precipitation from the atmosphere tile
					     atmo.moisture -= precipMass;
				}
		          }

		          UpdateTotals();
	     }

	     // Precipitation calculation now takes atmosphere tile values
	     private float CalculateCurrentPrecipitation(float hours, float atmoMoisture, float rH, float temp)
	     {
		          // 1. Check basic precipitation conditions
		          if (atmoMoisture <= 0 || rH <= 0.7f)
				return 0f;

		          // 2. Calculate dew point depression (temperature - dew point)
		          float dewPoint = CalculateDewPoint(temp, rH);
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
		          float maxPossibleRate = 0.01f * atmoMoisture; // Max 1% of total moisture per hour
		          float baseRate = Mathf.Lerp(0.1f, 10f, precipProbability); // 0.1-10 mm/hour range

		          // Adjust for temperature (snow vs rain)
		          float precipRate = temp > 0f ?
		              baseRate :
		              baseRate * 0.7f; // Snow falls slower than rain

		          // 6. Calculate total precipitation mass
		          float precipMass = precipRate * hours * (worldsize.x * worldsize.y); // Total kg for world
		          precipMass = Mathf.Min(precipMass, maxPossibleRate * hours * (worldsize.x * worldsize.y));
		          precipMass = Mathf.Min(precipMass, atmoMoisture * 0.5f); // Never remove more than 50% at once

		          return precipMass;
	     }

	     // Helper method to calculate dew point
	     private float CalculateDewPoint(float tempC, float relativeHumidity)
	     {
		          // Magnus formula for dew point
		          float a = 17.27f;
		          float b = 237.7f;
		          float alpha = ((a * tempC) / (b + tempC)) + Mathf.Log(relativeHumidity);
		          return (b * alpha) / (a - alpha);
	     }
	     public void SimulateSoilHour(TileData data, float hours, float energyWatts, float rainfallKg, float latitude)
	     {
		          float absorbedEnergy = SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(
		              latitude, SimulationConstants.DayOfYear + 1, SimulationConstants.TimeOfDay + 0.0001f);

		          // Update soil temperature
		          SoilUtilities.UpdateSoilTemperature(data, absorbedEnergy, data.atmosphereTile.temperature, hours);

		          // Get heat from soil and accumulate it
		          float energyFromTile = SoilUtilities.PassHeatFromSoil(data, hours, this);

		          // Now apply the accumulated energy to warm the atmosphere:
		          float airMass = (worldsize.x * worldsize.y) * 3000f * Constants.AIR_DENSITY;
		          float deltaAirTemp = energyFromTile / (airMass * Constants.AIR_HEAT_CAPACITY);
		          data.atmosphereTile.temperature += deltaAirTemp;

		          // Rest of your existing code...
		          SoilUtilities.ApplyRainfall(data, rainfallKg, hours);
		          SoilUtilities.InfiltrateSurfaceMoisture(data, hours);
		          SoilUtilities.ApplySelfCompaction(data, hours);
		          SoilUtilities.UpdateDecompaction(data.composition, hours);

		          float rh = CalculateRelativeHumidity(data.atmosphereTile);
		          float evaporationTemp = (data.atmosphereTile.temperature + data.soilTemperature) * 0.5f;
		          data.atmosphereTile.moisture += SoilUtilities.ApplyEvaporation(data, energyWatts, rh, evaporationTemp, data.atmosphereTile.windSpeed, hours);

		          SoilUtilities.CalculateDerivedProperties(data.composition);
	     }
	     // Calculate RH for a specific atmosphere tile
	     private float CalculateRelativeHumidity(AtmosphereTile atmo)
	     {
		          float tileArea = atmo.tileWidth * atmo.tileWidth;
		          float height = atmo.layerHeight;
		          float totalVolume = tileArea * height;
		          float vaporKg = atmo.moisture;
		          float vaporDensity = vaporKg / totalVolume;
		          float saturationVaporDensity = 0.0173f; // kg/m3 at 20°C
		          float relativeHumidity = vaporDensity / saturationVaporDensity;
		          return Mathf.Clamp01(relativeHumidity);
	     }

	     public void AllocateNeighbors()
	     {
		          for (int x = 0; x < worldsize.x; x++)
		          {
				for (int y = 0; y < worldsize.y; y++)
				{
					     WorldTile current = tiles[x, y];

					     // Wrapping coordinates
					     int xLeft = (x - 1 + worldsize.x) % worldsize.x;
					     int xRight = (x + 1) % worldsize.x;
					     int yDown = (y - 1 + worldsize.y) % worldsize.y;
					     int yUp = (y + 1) % worldsize.y;

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
		          for (int i = 0; i < worldsize.x; i++)
		          {
				for (int j = 0; j < worldsize.y; j++)
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
						          waterSum += atmosphereTiles[layer, i, j].moisture;
					     }
				}
		          }

		          totalWater = waterSum;
		          totalSand = soilSum.sand;
		          totalSilt = soilSum.silt;
		          totalClay = soilSum.clay;
		          totalOrganicMatter = soilSum.organicMatter;
	     }
}
