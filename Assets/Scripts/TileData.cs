using UnityEngine;

[System.Serializable]
public class TileData
{
	     public Vector2Int tileGridRef;
	     public float tileWidth = 1;
	     public float latitude; // Latitude in degrees
	     public float longitude; // Longitude in degrees
	     public float elevation; // Elevation in meters

	     public float albedo;
	     public Vector3[] tileCorners = new Vector3[8]; //tile corners for rendering and  physics

	     [Header("Dynamic States")]
	     //public float surfaceMoisture;
	     public ThermalEnergy surfaceIce;
	     public ThermalEnergy surfaceLiquid;

	     //bedrock
	     public ThermalEnergy bedrock = new ThermalEnergy(10, "silicon dioxide", 1790000, 128000000, 1986, 3220, 1.48f);
	     // public ThermalEnergy surfaceVapor;
	     public float surfaceTemperature => (surfaceLiquid.Temperature + surfaceIce.Temperature) / 2;
	     public float volume = 1; //m3
	     public float groundElevation; // meters
	     public float soilTemperature => composition.temperature; //should be derived from energy stored in soil composition

	     public AtmosphereTile atmosphereTile;
	     public SoilComposition composition;

	     public RadiativeEnergy incomingSolarEnergy = new RadiativeEnergy(SimulationConstants.SunlightWavelength);
	     public RadiativeEnergy outgoingLongwave = new RadiativeEnergy(SimulationConstants.SunlightWavelength); // emitted by surface (will travel up)
								   //public MechanicalEnergy
	     public void Initialise(TileData neighbor = null, float temp = 10f)
	     {
		          float vol = tileWidth * tileWidth; // 1,000,000 m³
		          composition = new SoilComposition();
		          temp = TileUtilities.GetTemperatureFromLatitude(latitude, 0);
		          surfaceLiquid = new ThermalEnergy(0.5f, "liquid water");
		          surfaceIce = new ThermalEnergy(.5f, "ice", 334000f, 2260000f, 273f, 373f, 2.5f);

		          //   surfaceVapor = new ThermalEnergy(0.5f, "water vapour");
		          surfaceLiquid.HeatToTemp(temp);

		          if (neighbor == null)
		          {
				// World-edge tile: use a random density (kg/m³) multiplied by vol.
				float sandDensity = Random.Range(15.5f, 390f);      // kg/m³
				float siltDensity = Random.Range(21f, 525f);
				float clayDensity = Random.Range(26.5f, 662f);
				float organicDensity = Random.Range(2.5f, 12.5f);
				float waterDensity = Random.Range(100, 1500);
				bedrock.AddMass(Random.Range(0, Constants.GRANITE_DENSITY * 4), temp);
				if (temp > SimulationConstants.Kelvin)
				{
					     surfaceLiquid.AddMass(waterDensity * vol, temp + 30);
				}
				if (temp < SimulationConstants.Kelvin)
				{
					     surfaceIce.AddMass(waterDensity * vol, temp - 5);
				}
				composition.sand.AddMass(sandDensity * vol, temp);
				composition.silt.AddMass(siltDensity * vol, temp);
				composition.clay.AddMass(clayDensity * vol, temp);
				composition.organicMaterial.AddMass(organicDensity * vol, temp);
				composition.compaction = Random.Range(0.2f, 0.7f);

		          }
		          else
		          {
				// Inherit neighbor's mass, but apply a percentage variation (±15%)
				float variation = Random.Range(0.85f, 1.15f);

				float sandMass = neighbor.composition.sand.Mass * variation;
				float siltMass = neighbor.composition.silt.Mass * variation;
				float clayMass = neighbor.composition.clay.Mass * variation;
				float organicMass = neighbor.composition.organicMaterial.Mass * variation;
				float surfaceLiquidMass = neighbor.surfaceLiquid.Mass + neighbor.surfaceIce.Mass * variation;
				// Optional: clamp to physically plausible density ranges 
				// (e.g., 1 to 800 kg/m³) to prevent runaway extremes.
				float minSand = 1f * vol;   // 1e6 kg
				float maxSand = 800f * vol; // 8e8 kg
				sandMass = Mathf.Clamp(sandMass, minSand, maxSand);
				// Similarly for silt, clay, organic...
				if (temp > SimulationConstants.Kelvin)
				{
					     surfaceLiquid.AddMass(surfaceLiquidMass * vol, temp + 30);
				}
				if (temp < SimulationConstants.Kelvin)
				{
					     surfaceIce.AddMass(surfaceLiquidMass * vol, temp);
				}
				bedrock.AddMass(neighbor.bedrock.Mass, temp);
				composition.sand.AddMass(sandMass, temp);
				composition.silt.AddMass(siltMass, temp);
				composition.clay.AddMass(clayMass, temp);
				composition.organicMaterial.AddMass(organicMass, temp);
				composition.compaction = Mathf.Clamp01(
				    neighbor.composition.compaction * Random.Range(0.9f, 1.1f)
				);
		          }

		          // Now recalculate physics (porosity, water capacity, etc.)
		          SoilUtilities.RecalculateTilePhysics(this);

		          // Water inheritance: use a percentage of neighbor's water, capped by capacity.
		          float wc = composition.waterCapacity;
		          if (neighbor == null)
		          {
				float waterMass = Random.Range(0f, wc);
				if (temp > SimulationConstants.Kelvin)
				{
					     composition.water.AddMass(waterMass * vol, temp + 30);
				}
				if (temp < SimulationConstants.Kelvin)
				{
					     composition.ice.AddMass(waterMass * vol, temp);
				}
		          }
		          else
		          {
				// Start with 70-100% of neighbor's water, but cap at current capacity.
				float waterMass = neighbor.composition.water.Mass * Random.Range(0.8f, 1.2f);
				if (temp > SimulationConstants.Kelvin)
				{
					     composition.water.AddMass(waterMass * vol, temp + 30);
				}
				if (temp < SimulationConstants.Kelvin)
				{
					     composition.ice.AddMass(waterMass * vol, temp);
				}
		          }
	     }
	     public void CollectGases(float tilesPerAtmoTile)
	     {
		          //when collecting gases from the atmosphere, we should only collect our portion though. atmosphere tiles cover multiple surface tiles.

		          //transfer gaseous soil and surface water to atmosphere.

		          if (surfaceLiquid.CurrentPhase == Phase.Gas)
		          {
				surfaceLiquid.TransferMassTo(atmosphereTile.waterVapor, surfaceLiquid.Mass);
		          }
		          if (composition.water.CurrentPhase == Phase.Gas)
		          {
				composition.water.TransferMassTo(atmosphereTile.waterVapor, composition.water.Mass);
		          }
		          if (surfaceIce.CurrentPhase == Phase.Gas)
		          {
				surfaceIce.TransferMassTo(atmosphereTile.waterVapor, surfaceIce.Mass);
		          }
		          if (composition.ice.CurrentPhase == Phase.Gas)
		          {
				composition.ice.TransferMassTo(atmosphereTile.waterVapor, composition.ice.Mass);
		          }

		          //transfer solid water from atmosphere and suface and to surface ice
		          if (surfaceLiquid.CurrentPhase == Phase.Solid)
		          {
				surfaceLiquid.TransferMassTo(surfaceIce, surfaceLiquid.Mass);
		          }
		          //transfer solid water from soil liquid to soil ice
		          if (composition.water.CurrentPhase == Phase.Solid)
		          {
				composition.water.TransferMassTo(composition.ice, composition.water.Mass);
		          }

		          //transger liquid water from soil ice to soil water
		          if (composition.ice.CurrentPhase == Phase.Liquid)
		          {
				composition.ice.TransferMassTo(composition.water, composition.ice.Mass);
		          }
		          //transfer liquids in surface ice and atmopheric ice and vapour to surface water.
		          if (surfaceIce.CurrentPhase == Phase.Liquid)
		          {
				surfaceIce.TransferMassTo(surfaceLiquid, surfaceIce.Mass);
		          }

		          //        //if we have no surface water we should also move all the surface ice to the soil ice.
		          //        if (surfaceLiquid.Mass < 5f && surfaceIce.Mass < 10)
		          //        {
		          //surfaceIce.TransferMassTo(composition.ice, surfaceIce.Mass);
		          //        }
	     }

	     public void DetermineAlbedo()
	     {
		          float total = surfaceIce.Mass + surfaceLiquid.Mass + composition.Mass;
		          if (total <= 0f)
		          {
				albedo = SurfaceAlbedo.Soil;
				return;
		          }

		          float iceFrac = surfaceIce.Mass / total;
		          float waterFrac = surfaceLiquid.Mass / total;
		          float soilFrac = composition.Mass / total;

		          // Base albedo from fractions
		          float ab = iceFrac * SurfaceAlbedo.Ice +
			      waterFrac * SurfaceAlbedo.LiquidWater +
			      soilFrac * SurfaceAlbedo.Soil;

		          // Refine soil part based on its colour (if there's soil)
		          if (soilFrac > 0.01f && composition.Mass > 0f)
		          {
				Color soilColor = TileUtilities.GetSoilColor(composition);
				float luminance = soilColor.grayscale; // 0..1
						         // Map luminance to albedo range, e.g., dark soil ~0.1, light soil ~0.35
				float soilAlbedo = Mathf.Lerp(0.1f, 0.35f, luminance);
				// Replace the generic soil contribution with colour‑based one
				ab = iceFrac * SurfaceAlbedo.Ice +
				     waterFrac * SurfaceAlbedo.LiquidWater +
				     soilFrac * soilAlbedo;
		          }

		          albedo = Mathf.Clamp01(ab);
	     }

}
public static class SurfaceAlbedo
{
	     public const float Ice = 0.6f;        // snow/ice
	     public const float LiquidWater = 0.07f;
	     public const float Soil = 0.2f;       // bare soil (can be refined by colour)
	     public const float Vegetation = 0.18f; // if you add vegetation later
}

//nutrient capacity,  water  erosion  succeptibility, wind erosion succeptibility,  compactability,  water capacity, aeration

//infiltration is the rate at which water enters the  soil  from the surface
//percolation  is  the rate at which  water can  travel through soil particles
//porosity is the volume of  pore  space
[System.Serializable]
public class SoilComposition
{
	     // Component masses (kg/m3)
	     public ThermalEnergy sand = new ThermalEnergy(.257f, "sand", 1800000, 12500000, 1500f, 2330, 0.3347f);  //kg
	     public ThermalEnergy silt = new ThermalEnergy(.257f, "silt", 1700000, 11000000, 1550f, 2650f, 0.0004f);  //kg
	     public ThermalEnergy clay = new ThermalEnergy(.257f, "clay", 500000, 1300000, 1600f, 2980f, 0.676f);  //kg
	     public ThermalEnergy organicMaterial = new ThermalEnergy(.257f, "organic matter", 237500, 2250000); //kg     
	     public ThermalEnergy water = new ThermalEnergy(1, "liquid water");
	     public ThermalEnergy ice = new ThermalEnergy(1, "ice", 334000f, 2260000f, 273f, 373f, 2.5f);

	     //public List<ThermalEnergy> energy => new List<ThermalEnergy>() { sand, silt, clay, organicMaterial, water };
	     public ThermalEnergy[] energy = new ThermalEnergy[6];

	     // Derived properties
	     public float porosity;  //
	     public float bulkDensity;    // kg/m3
	     public float waterCapacity;  // m3/m3
	     public float saturation;     // 0-1
	     public float aeration;       // air-filled porosity
	     public float infiltrationRate; // mm/hour
	     public float percolationRate;  // cm/hour
	     public float hydraulicConductivity; // cm/day
	     public float erodibility;    // 0-1
	     public float horizontalPermeability;  //(cm/day)
	     public float verticalPermeability; //(cm/day)
	     public float compaction; //compaction percentage
			//    public float albedo;

	     public float sandMass => sand.Mass;
	     public float siltMass => silt.Mass;
	     public float clayMass => clay.Mass;
	     public float organicMass => organicMaterial.Mass;
	     public float waterMass => water.Mass;

	     public float Mass => sandMass + siltMass + clayMass + organicMass + waterMass;
	     private float cachedTemperature;
	     private bool temperatureDirty = true;

	     public float temperature // in kelvin
	     {
		          get
		          {
				if (temperatureDirty)
				{
					     RecalculateTemperature();
				}
				return cachedTemperature;
		          }
	     }

	     // Call this whenever nitrogen, oxygen, CO2, or waterVapor change mass or temp.
	     public void MarkTemperatureDirty()
	     {
		          temperatureDirty = true;
	     }

	     private void RecalculateTemperature()
	     {
		          float totalMass = sand.Mass + silt.Mass + clay.Mass + organicMaterial.Mass + water.Mass;
		          if (totalMass <= 0f)
		          {
				cachedTemperature = 273.15f;
		          }
		          else
		          {
				cachedTemperature = ((sand.Temperature * sand.Mass) +
					      (silt.Temperature * silt.Mass) +
					      (clay.Temperature * clay.Mass) + (organicMaterial.Temperature * organicMaterial.Mass) +
					      (water.Temperature * water.Mass))
					      / totalMass;
		          }
		          temperatureDirty = false;
	     }
	     public SoilComposition()
	     {
		          energy[0] = sand;
		          energy[1] = silt;
		          energy[2] = clay;
		          energy[3] = organicMaterial;
		          energy[4] = water;
		          energy[5] = ice;
	     }
}
[System.Serializable]
public class AtmosphereTile
{
	     public Vector2Int tileGridRef;
	     public float tileWidth;
	     public int layerHeight; // 10m, 100m, 1000m
		              //normalise all the temperatures of the stuff within the atmosphere? calculate an average using the mass of each gas as a weighting factor.
	     private float cachedTemperature;
	     private bool temperatureDirty = true;

	     public float temperature // in kelvin
	     {
		          get
		          {
				if (temperatureDirty)
				{
					     RecalculateTemperature();
				}
				return cachedTemperature;
		          }
	     }

	     // Call this whenever nitrogen, oxygen, CO2, or waterVapor change mass or temp.
	     public void MarkTemperatureDirty()
	     {
		          temperatureDirty = true;
	     }

	     private void RecalculateTemperature()
	     {
		          float totalMass = nitrogen.Mass + oxygen.Mass + carbonDioxide.Mass + waterVapor.Mass + cloudWaterContent.Mass + iceParticles.Mass;
		          if (totalMass <= 0f)
		          {
				cachedTemperature = SimulationConstants.Kelvin;
		          }
		          else
		          {
				cachedTemperature = ((nitrogen.Temperature * nitrogen.Mass) +
					      (oxygen.Temperature * oxygen.Mass) +
					      (carbonDioxide.Temperature * carbonDioxide.Mass) +
					      (waterVapor.Temperature * waterVapor.Mass) + (cloudWaterContent.Temperature * cloudWaterContent.Mass) + (iceParticles.Temperature * iceParticles.Mass))
					      / totalMass;
		          }
		          temperatureDirty = false;
	     }
	     private float _cachedMass;
	     private bool _massDirty = true;

	     public float Mass
	     {
		          get
		          {
				if (_massDirty)
				{
					     _cachedMass = 0f;
					     foreach (var nrg in energies)
						          _cachedMass += nrg.Mass;
					     _massDirty = false;
				}
				return _cachedMass;
		          }
	     }

	     public void MarkMassDirty()
	     {
		          _massDirty = true;
	     }

	     public float density; // in kg per m3
	     public float pressure; // in Pascals determined by temperature, 
	     public Vector3 windVelocity;    // m/s (3D vector)
	     public float RelativeHumidity
	     {
		          get
		          {
				return AtmosphereUtilities.CalculateRelativeHumidity(this);
		          }
	     }
	     public float DewPoint
	     {
		          get
		          {
				return AtmosphereUtilities.CalculateDewPoint(temperature, RelativeHumidity);
		          }
	     }
	     public float windSpeed;


	     public AtmosphereTile above;
	     public AtmosphereTile below;
	     public AtmosphereTile[] horizontalNeighbors = new AtmosphereTile[4]; //N, E, S, W
	     public float specificHumidity => waterVapor.Mass / (oxygen.Mass + carbonDioxide.Mass + nitrogen.Mass);
	     public ThermalEnergy[] energies;
	     //public float waterVapor;        // kg
	     public ThermalEnergy[] WaterEnergy;
	     public ThermalEnergy waterVapor = new ThermalEnergy(0.00001f, "water vapour", 334000, 2260000, 273, 373, 0.018f, 24.57f);
	     //public float liquidWater;       // kg
	     public ThermalEnergy iceParticles = new ThermalEnergy(0.00001f, "ice", 334000f, 2260000f, 273f, 373f, 2.5f);
	     //clouds
	     public ThermalEnergy cloudWaterContent = new ThermalEnergy(0.00001f, "liquid water"); // kg

	     public ThermalEnergy[] AtmosphericEnergy;
	     public ThermalEnergy oxygen = new ThermalEnergy(.257f, "oxygen", 13900, 213000, 54.35f, 90.15f, 0.024f);            // kg
	     public ThermalEnergy carbonDioxide = new ThermalEnergy(.000384f, "carbon dioxide", 184000, 574000, 216.59f, 194.69f, 0.0168f);     // kg
	     public ThermalEnergy nitrogen = new ThermalEnergy(.956f, "nitrogen", 25700, 199000, 63.25f, 77.65f, 0.02598f);

	     public ThermalEnergy[] AerosolisedEnergy;
	     public ThermalEnergy sand = new ThermalEnergy(.257f, "sand", 1800000, 12500000, 1500f, 2330, 1.1f);  //kg
	     public ThermalEnergy silt = new ThermalEnergy(.257f, "silt", 1700000, 11000000, 1550f, 2650f, 2f);  //kg
	     public ThermalEnergy clay = new ThermalEnergy(.257f, "clay", 500000, 1300000, 1600f, 2980f, 2.5f);  //kg
	     public ThermalEnergy organicMaterial = new ThermalEnergy(.257f, "organic matter", 237500, 2250000); //kg      

	     public RadiativeEnergy downwellingSolar = new RadiativeEnergy(SimulationConstants.SunlightWavelength, 0);  // solar coming from above
	     public RadiativeEnergy upwellingThermal = new RadiativeEnergy(SimulationConstants.SunlightWavelength, 0);  // thermal coming from below
	     public RadiativeEnergy reflectedSolar = new RadiativeEnergy(SimulationConstants.SunlightWavelength, 0);     // (optional) solar reflected upward

	     public AtmosphereTile()
	     {
		          AtmosphericEnergy = new ThermalEnergy[] { oxygen, carbonDioxide, nitrogen };
		          WaterEnergy = new ThermalEnergy[] { waterVapor, iceParticles, cloudWaterContent };
		          AerosolisedEnergy = new ThermalEnergy[] { sand, silt, clay, organicMaterial };
		          energies = new ThermalEnergy[] { waterVapor, iceParticles, cloudWaterContent, oxygen, carbonDioxide, nitrogen, sand, silt, clay, organicMaterial };
	     }
	     public void CollectGases()
	     {
		          //transfer surfaceLiquid mass to atmosphere if its phase is gas. 

		          //transfer gaseous soil water to atmosphere.

		          if (cloudWaterContent.CurrentPhase == Phase.Gas)
		          {
				cloudWaterContent.TransferMassTo(waterVapor, cloudWaterContent.Mass);
		          }
		          if (iceParticles.CurrentPhase == Phase.Gas)
		          {
				iceParticles.TransferMassTo(waterVapor, iceParticles.Mass);
		          }

		          if (waterVapor.CurrentPhase == Phase.Liquid)
		          {
				waterVapor.TransferMassTo(cloudWaterContent, waterVapor.Mass);
		          }
		          if (iceParticles.CurrentPhase == Phase.Liquid)
		          {
				iceParticles.TransferMassTo(cloudWaterContent, iceParticles.Mass);
		          }
		          if (waterVapor.CurrentPhase == Phase.Solid)
		          {
				waterVapor.TransferMassTo(iceParticles, waterVapor.Mass);
		          }
		          if (cloudWaterContent.CurrentPhase == Phase.Solid)
		          {
				cloudWaterContent.TransferMassTo(iceParticles, cloudWaterContent.Mass);
		          }


	     }

	     //here we need a script that calls an atmosphere utility that validates that phases are all in the right place

}
[System.Serializable]
public class TileGrid
{
	     public int tileWidth;
	     public int XGrid;
	     public int YGrid;
	     public Vector3 originPoint;

	     public TileData[,] Map;


	     //return neighbors.
	     public TileGrid(int worldX, int worldY, int tileSize)
	     {
		          tileWidth = tileSize;
		          XGrid = worldX;
		          YGrid = worldY;
		          Map = new TileData[worldX, worldY];
	     }
}
