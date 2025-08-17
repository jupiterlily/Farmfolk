using UnityEngine;

[System.Serializable]
public class TileData
{
	     public Vector2Int tileGridRef;
	     public float tileWidth;
	     public SoilComposition composition;

	     [Header("Dynamic States")]
	     public float surfaceMoisture;
	     public float volume;
	     public float soilTemperature;
	     public AtmosphereTile atmosphereTile;

	     public void Initialize(TileData neighbor = null)
	     {
		          composition = new SoilComposition();

		          if (neighbor == null)
		          {
				// World-edge tile: Randomize but keep balanced
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
	     public float moisture; // in liters
	     public float pressure;
	     public Vector3 windDirection;
	     public float windSpeed;
	     public AtmosphereTile NWTile;
	     public AtmosphereTile NTile;
	     public AtmosphereTile NETile;
	     public AtmosphereTile WTile;
	     public AtmosphereTile ETile;
	     public AtmosphereTile SWTile;
	     public AtmosphereTile STile;
	     public AtmosphereTile SETile;
	     public AtmosphereTile BelowTile;
	     public AtmosphereTile AboveTile;

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
