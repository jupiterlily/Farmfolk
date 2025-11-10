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
		          AllocateNeighbors();

	     }
	     public void TileUpdate(float hours)
	     {
		          float latitude = gameObject.GetComponent<SimulationMaster>().currentLatitude;
		          int atmTileSise = 10;
		          int atmGridX = atmosphereTiles.GetLength(1);
		          int atmGridY = atmosphereTiles.GetLength(2);
		          //AtmosphereUtilities.SimulateAtmosphere(atmosphereTiles, hours);
		          for (int atmX = 0; atmX < atmGridX; atmX++)
		          {
				for (int atmY = 0; atmY < atmGridY; atmY++)
				{
					     AtmosphereTile atmo = atmosphereTiles[0, atmX, atmY];
					     // Determine which world tiles are covered by this atmosphere tile
					     int startX = atmX * atmTileSise;
					     int startY = atmY * atmTileSise;
					     int endX = Mathf.Min(startX + atmTileSise, worldsise.x);
					     int endY = Mathf.Min(startY + atmTileSise, worldsise.y);

					     int coveredTiles = (endX - startX) * (endY - startY);
					     for (int i = startX; i < endX; i++)
					     {
						          for (int j = startY; j < endY; j++)
						          {
								float rain = HydrosphereUtilities.CalculateCurrentPrecipitation(hours, atmo.cloudWaterContent, atmo.RelativeHumidity, atmo.temperature) / 100;
								WorldTile tile = tiles[i, j];
								TileData data = tile.tileData;
								SoilUtilities.ApplyRainfall(data, rain, hours);
								atmo.cloudWaterContent -= rain;
								SimulateSoilHour(data, hours, data.latitude);
								tile.TileVisualUpdate();


						          }
					     }
				}
		          }

		          UpdateTotals();
	     }
	     //get latitude of a cell based on its grid position
	     public float GetLatitude(int x, int y)
	     {
		          // Assuming latitude varies linearly across the grid
		          return (x / (float)worldsise.x) * 180 - 90f; // Map x to -90 to 90 range
	     }
	     //get longitude of a cell based on its grid position
	     public float GetLongitude(int x, int y)
	     {
		          // Assuming longitude varies linearly across the grid
		          return (y / (float)worldsise.y) * 360f - 180f; // Map y to -180 to 180 range
	     }

	     public void SimulateSoilHour(TileData data, float hours, float latitude)
	     {
		          float absorbedEnergy = SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(
		              latitude, SimulationConstants.DayOfYear + 1, SimulationConstants.TimeOfDay + 0.0001f);

		          // Update soil temperature  and evaporate water
		          SoilUtilities.UpdateSoilTemperature(data, absorbedEnergy, data.atmosphereTile.temperature, hours);
		          //HydrosphereUtilities.SurfaceCondensation(data, hours);
		          // Get heat from soil and accumulate it
		          float energyFromTile = SoilUtilities.PassHeatFromSoil(data, hours, this);

		          // Now apply the accumulated energy to warm the atmosphere:
		          float airMass = (worldsise.x * worldsise.y) * 3000f * Constants.AIR_DENSITY;
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

