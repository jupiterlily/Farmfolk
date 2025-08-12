using UnityEngine;

[System.Serializable]
public class TileData
{
	     public Vector2Int tileGridRef;
	     public float tileWidth;
	     public SoilComposition composition;

	     [Header("Dynamic States")]
	     public float compaction;  // 0 (loose) - 1 (concrete)
	     public float surfaceMoisture;

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
		          }
		          else
		          {
				// Inherit neighbor's traits with slight variation
				composition.sand = Mathf.Clamp(neighbor.composition.sand + Random.Range(-5f, 5f), 0, 100);
				composition.silt = Mathf.Clamp(neighbor.composition.silt + Random.Range(-5f, 5f), 0, 100);
				composition.clay = 100f - composition.sand - composition.silt;
				composition.organicMatter = Mathf.Max(0, neighbor.composition.organicMatter + Random.Range(-1f, 1f));
		          }
		          SoilUtilities.CalculateDerivedProperties(composition);
	     }
}

//nutrient capacity,  water  erosion  succeptibility, wind erosion succeptibility,  compactability,  water capacity, aeration

//infiltration is the rate at which water enters the  soil  from the surface
//percolation  is  the rate at which  water can  travel through soil particles
//porosity is the volume of  pore  space
[System.Serializable]
public class SoilComposition
{
	     // Component masses (kg/m³)
	     [Range(0, 1550)] public float sand;
	     [Range(0, 2100)] public float silt;
	     [Range(0, 2650)] public float clay;
	     [Range(0, 250)] public float organicMatter;
	     [Range(0, 1000)] public float water;

	     // Derived properties
	     public float porosity;        // 0-1
	     public float bulkDensity;    // kg/m³
	     public float waterCapacity;  // m³/m³
	     public float saturation;     // 0-1
	     public float aeration;       // air-filled porosity
	     public float infiltrationRate; // mm/hour
	     public float percolationRate;  // cm/hour
	     public float hydraulicConductivity; // cm/day
	     public float erodibility;    // 0-1
	     public float horizontalPermeability;  //(cm/day)
	     public float verticalPermeability; //(cm/day)
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
}
