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
		          // Distribute 10% of current atmosphericMoisture as rainfall
		          float precipMass = atmosphericMoisture * 0.10f;
		          float kgPerTile = precipMass / (worldsize.x * worldsize.y);
		          float mmRain = TileUtilities.WaterMassToMM(kgPerTile);

		          foreach (var tile in tiles)
		          {

				SoilUtilities.RedistributeSurfaceWater(tile, hours);
				SoilUtilities.RedistributeWaterLateral(tile, hours);
				SimulateSoilHour(tile.tileData, hours,
				    SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(45f, SimulationConstants.DayOfYear, SimulationConstants.TimeOfDay),
				    mmRain);

				tile.TileVisualUpdate();
		          }

		          atmosphericMoisture -= precipMass; // Reduce once, not per tile
	     }

	     public void SimulateSoilHour(TileData data, float hours, float energyWatts, float rainfall)
	     {
		          // 2. Apply rainfall to this tile
		          SoilUtilities.ApplyRainfall(data, rainfall, hours);


		          // 4. Simulate rest of soil physics
		          SoilUtilities.InfiltrateSurfaceMoisture(data, hours);
		          SoilUtilities.ApplySelfCompaction(data, hours);
		          SoilUtilities.UpdateDecompaction(data.composition, hours);

		          // 5. Simulate evaporation > return vapor to atmosphere
		          humidity = CalculateRelativeHumidity();
		          atmosphericMoisture += SoilUtilities.ApplyEvaporation(data, energyWatts, humidity, airTemp, 1f, hours);


		          // 6. Recalculate derived properties
		          SoilUtilities.CalculateDerivedProperties(data.composition);
	     }

	     private float CalculateRelativeHumidity()
	     {
		          // 1. Calculate volume of atmosphere in m³
		          float tileArea = 1f; // 1m2 per tile
		          float height = 3000f; // 3 km atmospheric block
		          float totalVolume = worldsize.x * worldsize.y * tileArea * height;

		          // 2. Convert moisture to kg
		          float vaporKg = atmosphericMoisture; // assuming 1L = 1kg

		          // 3. Get vapor density
		          float vaporDensity = vaporKg / totalVolume; // kg/m3

		          // 4. Max saturation at 20°C (approx)
		          float saturationVaporDensity = 0.0173f; // kg/m3 at 20°C

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



}
