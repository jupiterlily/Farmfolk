using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//instantiates tiles within a grid and  gives  them a grid reference

public class TileManager : MonoBehaviour
{
	     public Vector2Int worldsise;
	     public TileGrid grid;
	     public GameObject tilePrefab;
	     public WorldTile[,] tiles;
	     int atmTileSise => (int)grid.tileWidth * 10;
	     public float atmosphericMoisture; //in liters
	     public float airTemp;
	     public float totalWater; //all the water in the system, in liters
	     public float totalSoil; //used to track the total soil composition across the world
	     public float totalSand; //total sand in the world
	     public float totalSilt; //total silt in the world
	     public float totalClay; //total clay in the world
	     public float totalOrganicMaterial; //total organic matter in the world

	     // 3 layers, each with reduced resolution (10x tile sise)
	     public AtmosphereTile[,,] atmosphereTiles;

	     public Demeter demeter;
	     // Start is called before the first frame update
	     void Start()
	     {
		          grid = new TileGrid(worldsise.x, worldsise.y, grid.tileWidth);
		          // grid.tileWidth = 1;
		          grid.XGrid = worldsise.x;
		          grid.YGrid = worldsise.y;
		          tiles = new WorldTile[worldsise.x, worldsise.y];

		          // Atmosphere tile setup

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

						          //mTile.waterVapor.AddEnergy((airTemp + 273.15f)); //add the air temp iin kelvin and covert to kilojouls. idont fucking know. it's just initialisation.
						          atmTile.waterVapor.ChangeMass(atmosphericMoisture / (atmGridX * atmGridY * atmLayers));
						          //tmTile.waterVapor.ExtractEnergy(50000f);
						          atmTile.carbonDioxide.ChangeMass(0.481f * atmoFactor);
						          atmTile.nitrogen.ChangeMass(1014 * atmoFactor);
						          atmTile.oxygen.ChangeMass(273 * atmoFactor);
						          atmTile.pressure = 101325f - (layer * 12000f); // Example pressure drop per km	
						          atmTile.windVelocity = (Vector3.right + UnityEngine.Random.insideUnitSphere) * UnityEngine.Random.Range(0.95f, 1.05f);
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
		          demeter.initialPlantCount = Math.Min(100, plantPositions.Count);
		          demeter.InitializePlants(plantPositions);  // Call this explicitly

	     }
	     public List<TileData> FindPlantPositions()
	     {
		          List<TileData> plantPositions = new List<TileData>();
		          foreach (var tile in tiles)
		          {
				TileData data = tile.tileData;
				if (data.composition.water.Mass > 5f && data.surfaceWater.Mass < 10f && data.composition.organicMaterial.Mass > 10f)
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
		          AtmosphereUtilities.ApplyAtmosphericAdvection(atmosphereTiles, hours);
		          // First: Run atmospheric simulation
		          foreach (var tile in atmosphereTiles)
		          {

				if (tile.above == null)
				{
					     List<ThermalEnergy> toMix = new List<ThermalEnergy>(tile.energy);
					     toMix.AddRange(tile.below.energy);
					     ThermalEnergyUtilities.MixTemperaturesByConduction(toMix, hours, atmTileSise * tile.layerHeight);
					     toMix.Clear();
					     toMix = tile.below.energy;
					     toMix.AddRange(tile.below.below.energy);
					     ThermalEnergyUtilities.MixTemperaturesByConduction(toMix, hours, atmTileSise * tile.layerHeight);
					     float radiativeLoss = AtmosphereUtilities.CalculateRadiativeCooling(tile, hours);
					     //tile.energy.ForEach(e => e.ExtractEnergy(radiativeLoss / tile.energy.Count));
					     foreach (var material in tile.energy)
					     {
						          if (radiativeLoss <= 0)
								break;
						          //extract energy until radiative loss =0
						          radiativeLoss -= material.ExtractEnergy(radiativeLoss);
					     }
				}

				foreach (var neighbor in tile.horizontalNeighbors)
				{
					     if (neighbor != null)
					     {
						          List<ThermalEnergy> toMix = new List<ThermalEnergy>(tile.energy);
						          toMix.AddRange(neighbor.energy);
						          ThermalEnergyUtilities.MixTemperaturesByConduction(toMix, hours, atmTileSise * tile.layerHeight);
					     }
				}
				ThermalEnergyUtilities.MixTemperaturesByConduction(tile.energy, hours, atmTileSise * tile.layerHeight);
		          }

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
		          int atmGridX = atmosphereTiles.GetLength(1);
		          int atmGridY = atmosphereTiles.GetLength(2);

		          for (int atmX = 0; atmX < atmGridX; atmX++)
		          {
				for (int atmY = 0; atmY < atmGridY; atmY++)
				{
					     AtmosphereTile atmo = atmosphereTiles[0, atmX, atmY];

					     // Only process if there are clouds
					     if (atmo.cloudWaterContent.Mass <= 0)
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
					     DistributePrecipitationToSurfaceTiles(atmX, atmY, atmo.cloudWaterContent, hours);
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

	     /// <summary>
	     /// Distributes precipitation to individual surface tiles
	     /// </summary>
	     private void DistributePrecipitationToSurfaceTiles(
	         int atmX, int atmY,
	         ThermalEnergy totalPrecipitation,
	         float hours)
	     {
		          int startX = atmX * atmTileSise;
		          int startY = atmY * atmTileSise;
		          int endX = Mathf.Min(startX + atmTileSise, worldsise.x);
		          int endY = Mathf.Min(startY + atmTileSise, worldsise.y);

		          int tileCount = (endX - startX) * (endY - startY);
		          if (tileCount == 0)
				return;

		          // Calculate precipitation per surface tile (kg)
		          float precipitationPerTile = totalPrecipitation.Mass / tileCount;

		          for (int x = startX; x < endX; x++)
		          {
				for (int y = startY; y < endY; y++)
				{
					     WorldTile tile = tiles[x, y];
					     if (tile == null)
						          continue;

					     TileData data = tile.tileData;

					     // Add precipitation to surface moisture
					     //data.surfaceWater += precipitationPerTile;
					     ThermalEnergyUtilities.TransferMassAndEnergy(totalPrecipitation, data.surfaceWater, precipitationPerTile);
				}
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
		          float albedo = 0;
		          Color.RGBToHSV(tiles[data.tileGridRef.x, data.tileGridRef.y].GetComponent<Renderer>().material.color, out albedo, out albedo, out albedo);
		          data.composition.albedo = albedo;

		          solarRadiation *= albedo; //adjust solar radiation based on albedo

		          RadiativeEnergy sunlight = new RadiativeEnergy(635, solarRadiation);
		          // Calculate solar energy for soil heating
		          float absorbedEnergy = solarRadiation;

		          // Update soil temperature with proper energy balance
		          float soilMass = SoilUtilities.GetSoilMass(data.composition);
		          float heatCapacity = SoilUtilities.GetSoilHeatCapacity(data.composition);

		          if (soilMass > 0 && heatCapacity > 0)
		          {

				// Apply changes
				//data.soilTemperature += solarTempIncrease + conductiveTransfer;
				//rank the soil compisition in terms of mass and then add the solar energy to the biggest one
				ThermalEnergy target = data.surfaceWater;
				if (data.surfaceWater.Mass >= 10)
				{
					     RadiativeEnergyUtilities.ConvertRadiativeToThermal(sunlight, target, albedo, data.tileWidth * data.tileWidth, hours);
				}
				foreach (var component in data.composition.energy.OrderByDescending(m => m.Mass))
				{
					     RadiativeEnergyUtilities.ConvertRadiativeToThermal(sunlight, component, albedo, data.tileWidth * data.tileWidth, hours);
				}


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
		          SoilUtilities.UpdateSoilTemperature(data, hours);

		          //radiate heat from soil to space

	     }
	     //  public void SimulateSoilHour(TileData data, float hours, float latitude)
	     //  {
	     //        // Calculate solar energy ONCE
	     //        float absorbedEnergy = SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(
	     //            latitude, SimulationConstants.DayOfYear + 1, SimulationConstants.TimeOfDay + 0.0001f);

	     //        // Update soil temperature with proper energy balance
	     //        float soilMass = SoilUtilities.GetSoilMass(data.composition);
	     //        float heatCapacity = SoilUtilities.GetSoilHeatCapacity(data.composition);

	     //        if (soilMass > 0 && heatCapacity > 0)
	     //        {
	     //// Energy gain from solar radiation
	     //float solarTempIncrease = (absorbedEnergy * hours * 3600f) / (soilMass * heatCapacity);

	     //// Energy exchange with air
	     //float airTempDiff = data.atmosphereTile.temperature - data.soilTemperature;
	     //float conductiveTransfer = airTempDiff * 0.01f * hours; // Simple conduction

	     //// Apply changes
	     ////data.soilTemperature += solarTempIncrease + conductiveTransfer;

	     //// Evaporation cooling
	     //float evaporation = SoilUtilities.ApplyEvaporation(
	     //    data, absorbedEnergy, data.atmosphereTile.RelativeHumidity,
	     //    data.atmosphereTile.temperature, data.atmosphereTile.windSpeed, hours);
	     //        }

	     //        // Update soil temperature  and evaporate water
	     //        SoilUtilities.UpdateSoilTemperature(data, hours);
	     //        // Get heat from soil and accumulate it
	     //        float energyFromTile = SoilUtilities.PassHeatFromSoil(data, hours, this);

	     //        // Now apply the accumulated energy to warm the atmosphere:
	     //        float airMass = data.atmosphereTile.tileWidth * data.atmosphereTile.tileWidth * data.atmosphereTile.layerHeight * Constants.AIR_DENSITY;
	     //        float deltaAirTemp = energyFromTile / (airMass * Constants.AIR_SPECIFIC_HEAT);
	     //        //data.atmosphereTile.temperature += deltaAirTemp;


	     //        //lateral water movement
	     //        HydrosphereUtilities.RedistributeSurfaceWater(tiles[data.tileGridRef.x, data.tileGridRef.y], hours);
	     //        SoilUtilities.RedistributeWaterLateral(tiles[data.tileGridRef.x, data.tileGridRef.y], hours);
	     //        SoilUtilities.InfiltrateSurfaceMoisture(data, hours);
	     //        SoilUtilities.ApplySelfCompaction(data, hours);
	     //        SoilUtilities.UpdateDecompaction(data.composition, hours);

	     //        float rh = data.atmosphereTile.RelativeHumidity;
	     //        float evaporationTemp = (data.atmosphereTile.temperature + data.soilTemperature) * 0.5f;

	     //        SoilUtilities.CalculateDerivedProperties(data.composition);
	     //  }
	     //  public void AllocateNeighbors()
	     //  {
	     //        for (int x = 0; x < worldsise.x; x++)
	     //        {
	     //for (int y = 0; y < worldsise.y; y++)
	     //{
	     //	     WorldTile current = tiles[x, y];
	     //	     float lat = current.tileData.latitude;
	     //	     float lon = current.tileData.longitude;
	     //	     // Wrapping coordinates
	     //	     int xLeft = (x - 1 + worldsise.x) % worldsise.x;
	     //	     int xRight = (x + 1) % worldsise.x;
	     //	     int yDown = (y - 1 + worldsise.y) % worldsise.y;
	     //	     int yUp = (y + 1) % worldsise.y;

	     //	     // Assign neighbors with wrapping
	     //	     current.NTile = tiles[x, yUp];
	     //	     current.STile = tiles[x, yDown];
	     //	     current.WTile = tiles[xLeft, y];
	     //	     current.ETile = tiles[xRight, y];

	     //	     current.NWTile = tiles[xLeft, yUp];
	     //	     current.NETile = tiles[xRight, yUp];
	     //	     current.SWTile = tiles[xLeft, yDown];
	     //	     current.SETile = tiles[xRight, yDown];
	     //}
	     //        }
	     //  }
	     public void AllocateNeighbors()
	     {
		          int worldLatitude = worldsise.x;  // X = latitude (south to north)
		          int worldLongitude = worldsise.y; // Y = longitude (wraps)

		          int halfLongitude = worldLongitude / 2;
		          int atmoCounter = atmTileSise;
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
						          atmoCounter = atmTileSise;
					     }
					     else
					     {
						          atmoCounter--;
					     }
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
					     }
				}
		          }

		          totalWater = waterSum;
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
		          // AtmosphereUtilities.SimulateAtmosphere(atmosphereTiles, hours);

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


	     //  /// <summary>
	     //  /// Distributes precipitation to the surface tiles under an atmospheric cell
	     //  /// </summary>
	     //  private void DistributePrecipitationToSurface(int atmX, int atmY, float amount)
	     //  {
	     //        int tileSise = 10; // Matching the initialization ratio
	     //        int startX = atmX * tileSise;
	     //        int startY = atmY * tileSise;
	     //        int endX = Mathf.Min(startX + tileSise, worldsise.x);
	     //        int endY = Mathf.Min(startY + tileSise, worldsise.y);

	     //        int tileCount = (endX - startX) * (endY - startY);
	     //        if (tileCount == 0)
	     //return;

	     //        float perTile = amount / tileCount;

	     //        for (int x = startX; x < endX; x++)
	     //        {
	     //for (int y = startY; y < endY; y++)
	     //{
	     //	     tiles[x, y].tileData.surfaceMoisture += perTile;
	     //	     tiles[x, y].tileData.atmosphereTile.waterVapor -= perTile; // Also update atmosphere tile
	     //	     ThermalEnergyUtilities.TransferMassAndEnergy()
	     //}
	     //        }
	     //        Debug.Log($"Distributed {amount} kg of precipitation to surface tiles at ({atmX}, {atmY})");
	     //  }

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

		          // Calculate max values for normalization
		          float maxMoisture = 0.1f;  // Avoid division by zero
		          float maxWindSpeed = 0.1f; // Avoid division by zero

		          for (int layer = 0; layer < atmLayers; layer++)
		          {
				for (int x = 0; x < atmGridX; x++)
				{
					     for (int y = 0; y < atmGridY; y++)
					     {
						          AtmosphereTile atmo = atmosphereTiles[layer, x, y];
						          if (atmo != null)
						          {
								// Fixed: Access Mass instead of deprecated property
								maxMoisture = Mathf.Max(maxMoisture, atmo.waterVapor.Mass);
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
						          // Fixed: Correct layer height calculation (was multiplying by 1.2f incorrectly)
						          float worldY = layer * atmo.layerHeight + 2f; // Stack layers by actual height
						          float worldZ = (y + 0.5f) * atmo.tileWidth;
						          Vector3 pos = new Vector3(worldX, worldY, worldZ);

						          // ===== HUMIDITY SPHERE =====
						          // Fixed: Use RelativeHumidity property (was accessing it correctly but needed null check)
						          Gizmos.color = GetHumidityColor(atmo.RelativeHumidity);
						          float humiditySise = 0.3f + atmo.RelativeHumidity * 0.7f;
						          Gizmos.DrawSphere(pos + Vector3.down, humiditySise);

						          // ===== CLOUD WATER SPHERE =====
						          // Fixed: Check cloudWaterContent.Mass exists
						          if (atmo.cloudWaterContent != null && atmo.cloudWaterContent.Mass > 0)
						          {
								Gizmos.color = new Color(1f, 1f, 1f, Mathf.Clamp01(atmo.cloudWaterContent.Mass / 10f));
								float cloudSise = Mathf.Clamp(atmo.cloudWaterContent.Mass / 1000f, 0.1f, 5f);
								Gizmos.DrawSphere(pos + new Vector3(0, 2, 0), cloudSise);
						          }

						          // ===== WIND ARROWS =====
						          if (maxWindSpeed > 0.01f)  // Fixed: Better epsilon check
						          {
								Gizmos.color = GetWindColor(atmo.windSpeed, maxWindSpeed);
								Vector3 windDir = atmo.windVelocity.magnitude > 0.001f ?
								            atmo.windVelocity.normalized : Vector3.right;
								float windStrength = atmo.windSpeed / maxWindSpeed;
								Gizmos.DrawLine(pos, pos + windDir * (0.5f + windStrength * 2f));
								DrawArrow(pos, pos + windDir * (0.5f + windStrength * 2f), 0.2f);
						          }

						          // ===== TEMPERATURE CUBE =====
						          // Fixed: Handle both Celsius conversion and out-of-bounds temperatures
						          float tempC = atmo.temperature - 273.15f;
						          Gizmos.color = GetTemperatureColor(tempC);
						          float tempSise = 0.2f + Mathf.Clamp01(Mathf.InverseLerp(-20f, 40f, tempC)) * 0.5f;
						          Gizmos.DrawWireCube(pos + Vector3.up * 0.1f, Vector3.one * tempSise);

						          // ===== DEBUG LABELS =====
#if UNITY_EDITOR
						          string label = $"L{layer} ({x},{y})\n";
						          label += $"RH: {atmo.RelativeHumidity * 100f:F0}%\n";
						          label += $"T: {tempC:F1}°C\n";
						          //label += $"W: {atmo.windSpeed:F2}m/s\n";
						          // label += $"{atmo.waterVapor.Mass:F2} litres";
						          UnityEditor.Handles.Label(pos + Vector3.up * 0.5f, label);
#endif
					     }
				}
		          }
	     }

	     // Helper method to draw direction arrows
	     void DrawArrow(Vector3 start, Vector3 end, float arrowSise)
	     {
		          if ((end - start).magnitude < 0.001f)
				return; // Skip zero-length arrows

		          Gizmos.DrawLine(start, end);
		          Vector3 direction = (end - start).normalized;

		          // Fixed: Handle edge cases in quaternion operations
		          if (direction.magnitude < 0.001f)
				return;

		          Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 45, 0) * Vector3.forward;
		          Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 45, 0) * Vector3.forward;
		          Gizmos.DrawLine(end, end + right * arrowSise);
		          Gizmos.DrawLine(end, end + left * arrowSise);
	     }

	     // Color utilities for visualization
	     Color GetHumidityColor(float relativeHumidity)
	     {
		          // Blue gradient for humidity (fixed: clamp input)
		          float clamped = Mathf.Clamp01(relativeHumidity);
		          return Color.Lerp(
				new Color(0.7f, 0.7f, 1f, 0.5f),    // Light blue (dry)
				new Color(0f, 0f, 1f, 0.8f),        // Dark blue (humid)
				clamped
		          );
	     }

	     Color GetWindColor(float windSpeed, float maxWindSpeed)
	     {
		          // Red-yellow gradient for wind speed (fixed: handle zero maxWindSpeed)
		          if (maxWindSpeed < 0.001f)
				return Color.yellow;

		          float normalisedSpeed = Mathf.Clamp01(windSpeed / maxWindSpeed);
		          return Color.Lerp(
				Color.yellow,
				Color.red,
				normalisedSpeed
		          );
	     }

	     Color GetTemperatureColor(float tempC)
	     {
		          // Blue-red gradient for temperature (fixed: handle out-of-range temps)
		          float normalisedTemp = Mathf.Clamp01(Mathf.InverseLerp(-20f, 40f, tempC));
		          return Color.Lerp(
				Color.blue,
				Color.red,
				normalisedTemp
		          );
	     }
}
