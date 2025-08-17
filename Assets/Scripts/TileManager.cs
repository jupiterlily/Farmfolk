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
			        // Start is called before the first frame update
	     void Start()
	     {
		          grid = new TileGrid(worldsize.x, worldsize.y);


		          grid.tileWidth = 1;

		          grid.XGrid = worldsize.x;
		          grid.YGrid = worldsize.y;
		          tiles = new WorldTile[worldsize.x, worldsize.y];
		          //for each row of worlsize, allocate a tile and a position.
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
					     //get a valid neighbor from ref
					     tile.GetComponent<WorldTile>().tileData.Initialize(TileUtilities.GetValidNeighbour(data, grid));

					     tile.GetComponent<Renderer>().material.color = TileUtilities.GetSoilColor(tile.GetComponent<WorldTile>().tileData.composition);
				}
		          }
		          AllocateNeighbors();

	     }
	     public void TileUpdate(float hours)
	     {
		          // Calculate if precipitation should occur and how much
		          float precipMass = CalculateCurrentPrecipitation(hours);
		          float latitude = gameObject.GetComponent<SimulationMaster>().currentLatitude; // Example latitude, can be dynamic based on world location
		          if (precipMass > 0)
		          {
				// Evenly distribute rainfall across all tiles for now
				float kgPerTile = precipMass / (worldsize.x * worldsize.y);

				foreach (var tile in tiles)
				{
					     SoilUtilities.RedistributeSurfaceWater(tile, hours);
					     SoilUtilities.RedistributeWaterLateral(tile, hours);
					     SimulateSoilHour(tile.tileData, hours,
					         SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(latitude, SimulationConstants.DayOfYear, SimulationConstants.TimeOfDay),
					         kgPerTile, latitude);

					     tile.TileVisualUpdate(latitude);
				}

				atmosphericMoisture -= precipMass;
		          }
		          else
		          {
				foreach (var tile in tiles)
				{
					     SoilUtilities.RedistributeSurfaceWater(tile, hours);
					     SoilUtilities.RedistributeWaterLateral(tile, hours);
					     SimulateSoilHour(tile.tileData, hours,
					         SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(latitude, SimulationConstants.DayOfYear, SimulationConstants.TimeOfDay),
					         0, latitude);

					     tile.TileVisualUpdate(latitude);
				}
		          }



		          UpdateTotals();
	     }

	     private float CalculateCurrentPrecipitation(float hours)
	     {
		          // 1. Check basic precipitation conditions
		          if (atmosphericMoisture <= 0 || humidity <= 0.7f)
				return 0f;

		          // 2. Calculate dew point depression (temperature - dew point)
		          float dewPoint = CalculateDewPoint(airTemp, humidity);
		          float dewPointDepression = airTemp - dewPoint;

		          // 3. Determine precipitation probability (0-1)
		          float precipProbability = Mathf.Clamp01(
		              1f - (dewPointDepression / 5f)  // More likely when temp close to dew point
		              * Mathf.Pow(humidity, 2f)        // Much more likely at high humidity
		              * Mathf.InverseLerp(25f, 5f, airTemp) // More likely at cooler temps
		          );

		          // 4. Roll for precipitation occurrence
		          if (Random.value > precipProbability)
				return 0f;

		          // 5. Calculate precipitation intensity (kg/m2/hour)
		          float maxPossibleRate = 0.01f * atmosphericMoisture; // Max 1% of total moisture per hour
		          float baseRate = Mathf.Lerp(0.1f, 10f, precipProbability); // 0.1-10 mm/hour range

		          // Adjust for temperature (snow vs rain)
		          float precipRate = airTemp > 0f ?
		              baseRate :
		              baseRate * 0.7f; // Snow falls slower than rain

		          // 6. Calculate total precipitation mass
		          float precipMass = precipRate * hours * (worldsize.x * worldsize.y); // Total kg for world
		          precipMass = Mathf.Min(precipMass, maxPossibleRate * hours * (worldsize.x * worldsize.y));
		          precipMass = Mathf.Min(precipMass, atmosphericMoisture * 0.5f); // Never remove more than 50% at once

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
		          SoilUtilities.UpdateSoilTemperature(data, absorbedEnergy, airTemp, hours);

		          // Get heat from soil and accumulate it
		          float energyFromTile = SoilUtilities.PassHeatFromSoil(data, hours, this);

		          // Now apply the accumulated energy to warm the atmosphere:
		          float airMass = (worldsize.x * worldsize.y) * 3000f * Constants.AIR_DENSITY;
		          float deltaAirTemp = energyFromTile / (airMass * Constants.AIR_HEAT_CAPACITY);
		          airTemp += deltaAirTemp;

		          // Rest of your existing code...
		          SoilUtilities.ApplyRainfall(data, rainfallKg, hours);
		          SoilUtilities.InfiltrateSurfaceMoisture(data, hours);
		          SoilUtilities.ApplySelfCompaction(data, hours);
		          SoilUtilities.UpdateDecompaction(data.composition, hours);

		          humidity = CalculateRelativeHumidity();
		          float evaporationTemp = (airTemp + data.soilTemperature) * 0.5f;
		          atmosphericMoisture += SoilUtilities.ApplyEvaporation(data, energyWatts, humidity, evaporationTemp, 1f, hours);

		          SoilUtilities.CalculateDerivedProperties(data.composition);
	     }
	     private float CalculateRelativeHumidity()
	     {
		          // 1. Calculate volume of atmosphere in m3
		          float tileArea = 1f; // 1m2 per tile
		          float height = 3000f; // 3 km atmospheric block
		          float totalVolume = worldsize.x * worldsize.y * tileArea * height;

		          // 2. Convert moisture to kg
		          float vaporKg = atmosphericMoisture; // assuming 1L = 1kg

		          // 3. Get vapor density
		          float vaporDensity = vaporKg / totalVolume; // kg/m3

		          // 4. Max saturation at 20�C (approx)
		          float saturationVaporDensity = 0.0173f; // kg/m3 at 20�C

		          // 5. RH = actual / saturation
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

		          // Add atmospheric moisture to total water
		          totalWater = waterSum + atmosphericMoisture;
		          totalSand = soilSum.sand;
		          totalSilt = soilSum.silt;
		          totalClay = soilSum.clay;
		          totalOrganicMatter = soilSum.organicMatter;
	     }
}
