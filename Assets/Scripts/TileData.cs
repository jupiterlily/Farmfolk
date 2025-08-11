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
{//sand  is  a max of 1550kg/m3 ,
 //silt  is  a max of 2100kg/m3,
 //clay is  a max of 2650kg/m3,
 //water is a max of 1000kg/m3,
 //gas,
 //organic matter is 250kg/m3

}

[System.Serializable]
public class TileGrid
{
	     public float tileWidth;
	     public int XGrid;
	     public int YGrid;
}
