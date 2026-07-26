using System.Collections.Generic;
using UnityEngine;
public abstract class BlockData
{
	     //  public Vector3[,] Corners; //bottom layer and top layer. Allows forvariation in shapes

	     // public char cubeFace; //(T, B, N, S, E, W) the top, bottom, north south east or west face at initilisation
	     public Vector2Int gridRef; //grid reference for the block
	     public float longitude; //longitude in degrees
	     public float latitude; //latitude in degrees

	     public float surfaceWidth; //width at cubed sphere surface in meters
	     public float surfaceLength; //length at cubed sphere surface in meters
			   //  public float Volume
			   //  {
			   //        get
			   //        {
			   ////layer 0 is the base layer, layer 1 is the top layer
			   //if (Corners.GetLength(0) < 3 && Corners.GetLength(1) < 3)
			   //{
			   //	     Debug.LogError("Not enough vertices to calculate volume");
			   //	     return 0f;
			   //}
			   //Vector3[] baseCorners = new Vector3[Corners.GetLength(0)];
			   //for (int i = 0; i < Corners.GetLength(0); i++)
			   //{
			   //	     baseCorners[i] = Corners[0, i];
			   //}
			   //float baseArea = TileUtilities.CalculateIrregularPolygonArea(baseCorners);

	     //// 2. Calculate top area
	     //Vector3[] topCorners = new Vector3[Corners.GetLength(1)];
	     //for (int i = 0; i < Corners.GetLength(0); i++)
	     //{
	     //	     topCorners[i] = Corners[1, i];
	     //}
	     //float topArea = TileUtilities.CalculateIrregularPolygonArea(baseCorners);
	     //// 3. Calculate average height using vertical displacement
	     //float totalHeight = 0f;

	     //int pairedVertices = Mathf.Min(baseCorners.Length, topCorners.Length);
	     //// Calculate average height by summing vertical displacements
	     //for (int i = 0; i < pairedVertices; i++)
	     //{
	     //	     totalHeight += Corners[1, i].y - Corners[0, i].y;
	     //}
	     //float avgHeight = totalHeight / pairedVertices;

	     //// 4. Volume using prismatoid formula with vertex count normalization
	     //float areaFactor = (float)pairedVertices / Mathf.Max(baseCorners.Length, topCorners.Length);
	     //return (avgHeight / 3f) * (baseArea + topArea + Mathf.Sqrt(baseArea * topArea)) * areaFactor;
	     //        }
	     //  }

	     public Layer[] layers; //layers of matter in the block
}
public abstract class Layer : BlockData
{
	     public float layerHeight; //height of the layer as a percentage of the block height
	     public float temperature; //temperature in Kelvin of matter in the layer
	     public float pressure; //not sure if this should be pressure exerted on me or pressure iexert on the world. 
	     public float density; //density in kg/m3
}

public class LiquidLayer : Layer //will be used by rendering and physics systems. 
{
	     //composition
	     public List<ThermalEnergy> liquidComposition; // list of matter in the liquid.
				      //dissolved gases
	     public List<ThermalEnergy> dissolvedGases; // gases dissolved in the liquid
				   //disolved solids
	     public List<ThermalEnergy> dissolvedSolids; // solids dissolved in the liquid

	     public List<ThermalEnergy> suspendedSolids;
}

public class SolidLayer : Layer
{
	     public bool surface; // is this a surface layer?

	     //composition. include particle sise distribution and shape
	     public List<ThermalEnergy> solidComposition; // list of matter in the solid
				     //particle sise distribution
	     public List<float> particleSiseDistribution; // list of particle sises in the solid
				     //infiltrated liquids
	     public List<ThermalEnergy> infiltratedLiquids; // liquids infiltrated in the solid
				       //infiltrated gases
	     public List<ThermalEnergy> infiltratedGases; // gases infiltrated in the solid
}

public class GasLayer : Layer
{
	     public bool atmospheric; // is this an atmospheric layer?
			//composition
	     public List<ThermalEnergy> gasComposition; // list of matter in the gas
				   //aerosolised solids
	     public List<ThermalEnergy> aerosolisedSolids; // solids aerosolised in the gas
				      //liquid droplets
	     public List<ThermalEnergy> liquidDroplets; // liquid droplets in the gas
}




