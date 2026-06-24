using UnityEngine;

[System.Serializable]
public class TileData
{
	     public Vector2Int tileGridRef;
	     public float tileWidth;
	     public float latitude; // Latitude in degrees
	     public float longitude; // Longitude in degrees
	     public float elevation; // Elevation in meters


	     public Vector3[] tileCorners = new Vector3[8]; //tile corners for rendering and  physics

	     [Header("Dynamic States")]
	     //public float surfaceMoisture;
	     public ThermalEnergy surfaceWater = new ThermalEnergy(10, "liquid water");
	     public float surfaceTemperature => surfaceWater.Temperature;
	     public float volume;
	     public float soilTemperature => composition.water.Temperature; //should be derived from energy stored in soil composition

	     public AtmosphereTile atmosphereTile;
	     public SoilComposition composition;


	     public void Initialise(TileData neighbor = null, float temp = 10f)  //temp is in celcius
	     {
		          composition = new SoilComposition();
		          temp += SimulationConstants.Kelvin;
		          surfaceWater.AddHeat(temp * surfaceWater.SpecificHeat);
		          if (neighbor == null)
		          {
				temp *= 1 + Random.Range(-1f, 1f);
				// World-edge tile: Randomise but keep balanced
				composition.sand.AddMass(Random.Range(15.5f, 390f), temp);
				composition.silt.AddMass(Random.Range(21f, 525f), temp);
				composition.clay.AddMass(Random.Range(26.5f, 662f), temp);
				composition.organicMaterial.AddMass(Random.Range(2.5f, 12.5f), temp);
				composition.compaction = Random.Range(0.2f, 0.7f);


		          }
		          else
		          {
				temp = neighbor.surfaceTemperature + SimulationConstants.Kelvin;
				temp *= 1 + Random.Range(-0.15f, 0.15f);
				// Inherit neighbor's traits with slight variation
				composition.sand.AddMass(Mathf.Clamp(neighbor.composition.sand.Mass + Random.Range(-15.5f, 39f), 0.001f, 1550f), temp);
				composition.silt.AddMass(Mathf.Clamp(neighbor.composition.silt.Mass + Random.Range(-21f, 52.5f), 0.001f, 2100f), temp);
				composition.clay.AddMass(Mathf.Clamp(neighbor.composition.clay.Mass + Random.Range(-26.5f, 66.2f), 0.001f, 2650f), temp);
				composition.organicMaterial.AddMass(Mathf.Clamp(neighbor.composition.organicMaterial.Mass + Random.Range(-2.5f, 12.5f), 0, 250f), temp);
				composition.compaction = Mathf.Clamp01(Random.Range(neighbor.composition.compaction, 1f));
				// Inherit latitude and longitude from neighbor but offset slightly based on grid position				
		          }
		          //soilTemperature = Random.Range(0f, 15f); // initial temperature
		          SoilUtilities.RecalculateTilePhysics(this);
		          float wc = composition.waterCapacity * 3;
		          composition.water.AddMass(Mathf.Clamp(Random.Range(0, wc), neighbor.composition.water.Mass * 0.75f, wc), temp);
	     }


}


//nutrient capacity,  water  erosion  succeptibility, wind erosion succeptibility,  compactability,  water capacity, aeration

//infiltration is the rate at which water enters the  soil  from the surface
//percolation  is  the rate at which  water can  travel through soil particles
//porosity is the volume of  pore  space
[System.Serializable]
public class SoilComposition
{
	     // Component masses (kg/m3)
	     public ThermalEnergy sand = new ThermalEnergy(.257f, "sand", 1800000, 12500000, 1500f, 2330, 1.1f);  //kg
	     public ThermalEnergy silt = new ThermalEnergy(.257f, "silt", 1700000, 11000000, 1550f, 2650f, 2f);  //kg
	     public ThermalEnergy clay = new ThermalEnergy(.257f, "clay", 500000, 1300000, 1600f, 2980f, 2.5f);  //kg
	     public ThermalEnergy organicMaterial = new ThermalEnergy(.257f, "organic matter", 237500, 2250000); //kg     
	     public ThermalEnergy water = new ThermalEnergy(10, "liquid water", 334000, 2260000, 20);

	     //public List<ThermalEnergy> energy => new List<ThermalEnergy>() { sand, silt, clay, organicMaterial, water };
	     public ThermalEnergy[] energy = new ThermalEnergy[5];

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
	     public float albedo;

	     public float sandMass => sand.Mass;
	     public float siltMass => silt.Mass;
	     public float clayMass => clay.Mass;
	     public float organicMass => organicMaterial.Mass;
	     public float waterMass => water.Mass;

	     public SoilComposition()
	     {
		          energy[0] = sand;
		          energy[1] = silt;
		          energy[2] = clay;
		          energy[3] = organicMaterial;
		          energy[4] = water;
	     }
}
[System.Serializable]
public class AtmosphereTile
{
	     public Vector2Int tileGridRef;
	     public float tileWidth;
	     public int layerHeight; // 10m, 100m, 1000m
		              //normalise all the temperatures of the stuff within the atmosphere? calculate an average using the mass of each gas as a weighting factor.
	     public float temperature//in kelvin
	     {
		          get
		          {
				float totalMass = nitrogen.Mass + oxygen.Mass + carbonDioxide.Mass + waterVapor.Mass;
				float weightedTemperature = (nitrogen.Temperature * nitrogen.Mass + oxygen.Temperature * oxygen.Mass + carbonDioxide.Temperature * carbonDioxide.Mass + waterVapor.Temperature * waterVapor.Mass) / totalMass;
				return weightedTemperature;
		          }
	     }
	     public float density; // in liters
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
	     public ThermalEnergy[] energy;
	     //public float waterVapor;        // kg
	     public ThermalEnergy[] WaterEnergy;
	     public ThermalEnergy waterVapor = new ThermalEnergy(10, "water vapour");
	     //public float liquidWater;       // kg
	     public ThermalEnergy iceParticles = new ThermalEnergy(10, "ice");
	     //clouds
	     public ThermalEnergy cloudWaterContent = new ThermalEnergy(10, "liquid water"); // kg

	     public ThermalEnergy[] AtmosphericEnergy;
	     public ThermalEnergy oxygen = new ThermalEnergy(.257f, "oxygen", 13900, 213000, 54.35f, 90.15f, 0.024f);            // kg
	     public ThermalEnergy carbonDioxide = new ThermalEnergy(.000384f, "carbon dioxide", 184000, 574000, 216.59f, 194.69f, 0.0168f);     // kg
	     public ThermalEnergy nitrogen = new ThermalEnergy(.956f, "nitrogen", 25700, 199000, 63.25f, 77.65f, 0.02598f);

	     public ThermalEnergy[] AerosolisedEnergy;
	     public ThermalEnergy sand = new ThermalEnergy(.257f, "sand", 1800000, 12500000, 1500f, 2330, 1.1f);  //kg
	     public ThermalEnergy silt = new ThermalEnergy(.257f, "silt", 1700000, 11000000, 1550f, 2650f, 2f);  //kg
	     public ThermalEnergy clay = new ThermalEnergy(.257f, "clay", 500000, 1300000, 1600f, 2980f, 2.5f);  //kg
	     public ThermalEnergy organicMaterial = new ThermalEnergy(.257f, "organic matter", 237500, 2250000); //kg      

	     public AtmosphereTile()
	     {
		          AtmosphericEnergy = new ThermalEnergy[] { oxygen, carbonDioxide, nitrogen };
		          WaterEnergy = new ThermalEnergy[] { waterVapor, iceParticles, cloudWaterContent };
		          AerosolisedEnergy = new ThermalEnergy[] { sand, silt, clay, organicMaterial };
		          energy = new ThermalEnergy[] { waterVapor, iceParticles, cloudWaterContent, oxygen, carbonDioxide, nitrogen, sand, silt, clay, organicMaterial };
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
