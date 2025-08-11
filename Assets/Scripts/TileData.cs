using UnityEngine;

[System.Serializable]
public class TileData
{

	     public float compaction;  //derived
	     public Vector2Int tileGridRef;
	     public float tileWidth;
	     public Color tileColor;

}

//nutrient capacity,  water  erosion  succeptibility, wind erosion succeptibility,  compactability,  water capacity, aeration

//infiltration is the rate at which water enters the  soil  from the surface
//percolation  is  the rate at which  water can  travel through soil particles
//porosity is the volume of  pore  space

[System.Serializable]
public class SoilComposition
{//sand,  silt, clay, water, gas, organic matter

}

[System.Serializable]
public class TileGrid
{
	     public float tileWidth;
	     public int XGrid;
	     public int YGrid;
}
