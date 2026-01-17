using System.Collections.Generic;
using UnityEngine;
public class BlockData
{
	     public Vector3[,] Corners; //bottom layer and top layer. Allow for different shapes

	     public Vector2Int gridRef; //grid reference for the block
	     public float longitude; //longitude in degrees
	     public float latitude; //latitude in degrees

	     public float surfaceWidth; //width at cubed sphere surface in meters
	     public float surfaceLength; //length at cubed sphere surface in meters
	     public float Volume
	     {
		          get
		          {
				//layer 0 is the base layer, layer 1 is the top layer
				if (Corners.GetLength(0) < 3 && Corners.GetLength(1) < 3)
				{
					     Debug.LogError("Not enough vertices to calculate volume");
					     return 0f;
				}
				Vector3[] baseCorners = new Vector3[Corners.GetLength(0)];
				for (int i = 0; i < Corners.GetLength(0); i++)
				{
					     baseCorners[i] = Corners[0, i];
				}
				float baseArea = TileUtilities.CalculateIrregularPolygonArea(baseCorners);

				// 2. Calculate top area
				Vector3[] topCorners = new Vector3[Corners.GetLength(1)];
				for (int i = 0; i < Corners.GetLength(0); i++)
				{
					     topCorners[i] = Corners[1, i];
				}
				float topArea = TileUtilities.CalculateIrregularPolygonArea(baseCorners);
				// 3. Calculate average height using vertical displacement
				float totalHeight = 0f;

				int pairedVertices = Mathf.Min(baseCorners.Length, topCorners.Length);
				// Calculate average height by summing vertical displacements
				for (int i = 0; i < pairedVertices; i++)
				{
					     totalHeight += Corners[1, i].y - Corners[0, i].y;
				}
				float avgHeight = totalHeight / pairedVertices;

				// 4. Volume using prismatoid formula with vertex count normalization
				float areaFactor = (float)pairedVertices / Mathf.Max(baseCorners.Length, topCorners.Length);
				return (avgHeight / 3f) * (baseArea + topArea + Mathf.Sqrt(baseArea * topArea)) * areaFactor;
		          }
	     }

	     public Layer[] layers; //layers of matter in the block
}
public class Layer : BlockData
{
	     public int layerHeight; //height of the layer as a percentage of the block height
	     public float temperature; //temperature in Celsius of matter in the layer
	     public float pressure; //
	     public float density; //density in kg/m3
}

public class Liquid : Layer //will be used by rendering and physics systems. 
{
	     //composition
	     public List<Matter> liquidComposition; // list of matter in the liquid
			              //dissolved gases
	     public List<Matter> dissolvedGases; // gases dissolved in the liquid
			           //disolved solids
	     public List<Matter> dissolvedSolids; // solids dissolved in the liquid
}

public class Solid : Layer //can be both liquid and gas will be used by rendering and physics systems. 
{
	     public bool surface; // is this a surface layer?

	     //composition. include particle sise distribution and shape
	     public List<Matter> solidComposition; // list of matter in the solid
			             //particle sise distribution
	     public List<float> particleSiseDistribution; // list of particle sises in the solid
				     //infiltrated liquids
	     public List<Matter> infiltratedLiquids; // liquids infiltrated in the solid
				//infiltrated gases
	     public List<Matter> infiltratedGases; // gases infiltrated in the solid
}

public class Gas : Layer //can be both liquid and gas will be used by rendering and physics systems. 
{
	     public bool atmospheric; // is this an atmospheric layer?
			//composition
	     public List<Matter> gasComposition; // list of matter in the gas
			           //aerosolised solids
	     public List<Matter> aerosolisedSolids; // solids aerosolised in the gas
			              //liquid droplets
	     public List<Matter> liquidDroplets; // liquid droplets in the gas
}

public class Matter
{
	     //matter and energy composition of the block
	     public List<EnergyData> energyComposition; // energy stored in the matter, mass and temperature can be derived from this
	     public List<Compound> composition; // the composition of the matter in the block used to handle mixtures
	     public float density; // in kg/m3
}

public class Compound : Matter
{
	     public string name;
	     public Dictionary<Element, int> elements; // list of elements in the compound

	     public float meltingPoint; // in Celsius
	     public float boilingPoint; // in Celsius

	     public float specificHeatCapacity; // in J/(kg*K) amount of energy required to raise 1 kg of the compound by 1 Kelvin
	     public float thermalConductivity; // in W/(m*K) ability to conduct heat
	     public float electricalConductivity; // in S/m ability to conduct electricity

	     public Dictionary<Element, int> ParseMoleculeString(string molecule)
	     {
		          //parse molecular string into elements dictionary
		          Dictionary<Element, int> elementDict = new Dictionary<Element, int>();
		          //simple parser for molecular strings like H2O, CO2, C6H12O6
		          int i = 0;
		          while (i < molecule.Length)
		          {
				//get element symbol
				string symbol = molecule[i].ToString();
				i++;
				if (i < molecule.Length && char.IsLower(molecule[i]))
				{
					     symbol += molecule[i];
					     i++;
				}
				//get element count
				string countStr = "";
				while (i < molecule.Length && char.IsDigit(molecule[i]))
				{
					     countStr += molecule[i];
					     i++;
				}
				int count = 1;
				if (countStr != "")
				{
					     count = int.Parse(countStr);
				}
				//find element by symbol
				Element element = MatterLibrary.elements.Find(e => e.symbol == symbol);
				if (element != null)
				{
					     if (elementDict.ContainsKey(element))
					     {
						          elementDict[element] += count;
					     }
					     else
					     {
						          elementDict.Add(element, count);
					     }
				}

		          }
		          return elementDict;
	     }
}
public class Element : Compound
{
	     public string symbol;
	     public float atomicNumber; // number of protons
	     public float atomicMass; // in amu     
	     public float atomicRadius; // in pm

	     //constructor for element takes strings and floats name, symbol, atomic number, atomic mass, specific heat capacity, thermal conductivity, electrical conductivity and initialises the element including base class Compound and Matter
	     public Element(string Name, string Symbol, float AtNo, float AtMass, float SHC, float TC, float EC, float meltPoint, float boilPoint, string molecule, float latentFusion, float latentVaporisation, float bondEnergyKj)
	     {
		          name = Name;

		          symbol = Symbol;

		          atomicNumber = AtNo;
		          atomicMass = AtMass;

		          specificHeatCapacity = SHC;
		          thermalConductivity = TC;
		          electricalConductivity = EC;

		          elements = ParseMoleculeString(molecule);

		          meltingPoint = meltPoint;
		          boilingPoint = boilPoint;

		          energyComposition = new List<EnergyData>();
		          energyComposition.Add(new LatentEnergy(latentFusion, latentVaporisation));
		          //initialise chemical energy calculating it from atomic mass for total moles
		          float chemicalEnergyJoules = bondEnergyKj * 1000; // placeholder calculation
		          energyComposition.Add(new ChemicalEnergy(chemicalEnergyJoules));
	     }


}


