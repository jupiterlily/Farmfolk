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
	     public float surfaceMoisture;
	     public float surfaceTemperature; // Surface temperature in Celsius
	     public float volume;
	     public float soilTemperature;
	     public AtmosphereTile atmosphereTile;
	     public SoilComposition composition;
	     public void Initialise(TileData neighbor = null)
	     {
		          composition = new SoilComposition();

		          if (neighbor == null)
		          {
				// World-edge tile: Randomise but keep balanced
				composition.sand = Random.Range(15.5f, 390f);
				composition.silt = Random.Range(21f, 525f);
				composition.clay = Random.Range(26.5f, 662f);
				composition.organicMatter = Random.Range(2.5f, 12.5f);
				composition.compaction = Random.Range(0.2f, 0.7f);


		          }
		          else
		          {
				// Inherit neighbor's traits with slight variation
				composition.sand = Mathf.Clamp(neighbor.composition.sand + Random.Range(-15.5f, 39f), 0.001f, 1550f);
				composition.silt = Mathf.Clamp(neighbor.composition.silt + Random.Range(-21f, 52.5f), 0.001f, 2100f);
				composition.clay = Mathf.Clamp(neighbor.composition.clay + Random.Range(-26.5f, 66.2f), 0.001f, 2650f);

				composition.organicMatter = Mathf.Clamp(neighbor.composition.organicMatter + Random.Range(-2.5f, 12.5f), 0, 250f);
				composition.compaction = Mathf.Clamp01(Random.Range(neighbor.composition.compaction, 1f));
				// Inherit latitude and longitude from neighbor but offset slightly based on grid position				
		          }
		          soilTemperature = Random.Range(0f, 15f); // initial temperature
		          SoilUtilities.RecalculateTilePhysics(this);
		          float wc = composition.waterCapacity;
		          composition.water = Mathf.Clamp(Random.Range(0, wc), neighbor.composition.water * 0.75f, wc);
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
	     [Range(0, 1550)] public float sand;
	     [Range(0, 2100)] public float silt;
	     [Range(0, 2650)] public float clay;
	     [Range(0, 250)] public float organicMatter;
	     [Range(0, 1000)] public float water;

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
}
[System.Serializable]
public class AtmosphereTile
{
	     public Vector2Int tileGridRef;
	     public float tileWidth;
	     public int layerHeight; // 10m, 100m, 1000m
	     public float temperature;
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
	     public AtmosphereTile[] horizontalNeighbors;
	     public float waterVapor;        // kg
	     public float liquidWater;       // kg
	     public float oxygen;            // kg
	     public float carbonDioxide;     // kg
	     public float nitrogen;
	     public float sand; //kg
	     public float silt; //kg
	     public float clay; //kg
	     public float organicMatter; //kg
			   //clouds
	     public float cloudWaterContent; // kg


}
[System.Serializable]
public class TileGrid
{
	     public float tileWidth;
	     public int XGrid;
	     public int YGrid;
	     public Vector3 originPoint;

	     public TileData[,] Map;


	     //return neighbors.
	     public TileGrid(int worldX, int worldY)
	     {
		          tileWidth = 1;
		          XGrid = worldX;
		          YGrid = worldY;
		          Map = new TileData[worldX, worldY];
	     }
}
