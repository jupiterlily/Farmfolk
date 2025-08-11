using UnityEngine;

[System.Serializable]
public class TileData
{
	     public Vector2Int tileGridRef;
	     public float tileWidth;
	     public SoilComposition composition;

	     [Header("Dynamic States")]
	     public float compaction;  // 0 (loose) - 1 (concrete)
	     public float moisture;

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

		          composition.CalculateDerivedProperties();
		          UpdateCompaction();
	     }

	     public void UpdateCompaction()
	     {
		          // Clay compacts more, organic matter reduces compaction
		          compaction = 0.7f * (composition.clay / 100f)
			      - 0.3f * (composition.organicMatter / 20f)
			      + 0.1f * Random.Range(-1f, 1f); // Small noise
		          compaction = Mathf.Clamp01(compaction);
	     }
}

//nutrient capacity,  water  erosion  succeptibility, wind erosion succeptibility,  compactability,  water capacity, aeration

//infiltration is the rate at which water enters the  soil  from the surface
//percolation  is  the rate at which  water can  travel through soil particles
//porosity is the volume of  pore  space

[System.Serializable]
public class SoilComposition
{//sand  is  a max of 1550kg/m3 ,
 //silt  is  a max of 2100kg/m3,
 //clay is  a max of 2650kg/m3,
 //water is a max of 1000kg/m3,
 //gas,
 //organic matter is 250kg/m3

	     [Range(0, 1550)] public float sand;      // Gritty, drains fast
	     [Range(0, 2100)] public float silt;      // Smooth, holds moisture
	     [Range(0, 2650)] public float clay;      // Sticky, compacts easily
	     [Range(0, 250)] public float organicMatter; // Darkens color, lowers density

	     // Derived properties (calculated on initialization)
	     public float porosity;       // Total pore space
	     public float infiltrationRate; // mm/hour
	     public float waterHoldingCapacity;

	     public void CalculateDerivedProperties()
	     {
		          // USDA soil texture empirical formulas
		          porosity = 0.3f + (0.002f * clay) - (0.001f * sand);
		          infiltrationRate = 25f * Mathf.Pow(sand / 100f, 2f);
		          waterHoldingCapacity = 0.1f * clay + 0.05f * silt;
	     }

	     public Color GetSoilColor()
	     {
		          // Base color mixing (USDA standards)
		          Color sandColor = new Color(0.9f, 0.8f, 0.6f); // Tan
		          Color siltColor = new Color(0.7f, 0.7f, 0.6f); // Brown-gray
		          Color clayColor = new Color(0.6f, 0.5f, 0.4f); // Reddish-brown
		          Color organicColor = new Color(0.3f, 0.2f, 0.1f); // Dark brown

		          // Weighted blend
		          Color baseColor = (sandColor * (sand / 1550) + siltColor * (silt / 2100) + clayColor * (clay / 2650)) / 100f;
		          return Color.Lerp(baseColor, organicColor, organicMatter / 20f);

		          // return Color.blue;
	     }
}


[System.Serializable]
public class TileGrid
{
	     public float tileWidth;
	     public int XGrid;
	     public int YGrid;
}
