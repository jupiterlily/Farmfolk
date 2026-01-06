using UnityEngine;
public enum OrganType
{
	     Root,
	     Stem,
	     Leaf,
	     Flower,
	     Fruit,
	     Seed
}
public class Organs
{
	     //we'll be able to work out what the true values are from the size of the organ
	     public OrganType OrganType;
	     public int GrowthCost;
	     public float WaterRatio;
	     public float GlucoseRatio;
	     public float StarchRatio;
	     public float ProteinRatio;
	     public float LipidRatio;
	     public float CelluloseRatio;

	     public Organs(OrganType organType, float waterRatio, float glucoseRatio, float starchRatio, float proteinRatio, float lipidRatio, float celluloseRatio)
	     {
		          OrganType = organType;
		          WaterRatio = waterRatio;
		          GlucoseRatio = glucoseRatio;
		          StarchRatio = starchRatio;
		          ProteinRatio = proteinRatio;
		          LipidRatio = lipidRatio;
		          CelluloseRatio = celluloseRatio;
	     }
}
public class LeafProperties
{
	     public Organs LeafOrgan
	     {
		          get; set;
	     } = new Organs(OrganType.Leaf,
		          Random.Range(0.7f, 0.9f), //water
		          Random.Range(0.0f, 0.01f),  //glucose
		          Random.Range(0.0f, 0.01f), //starch
		          Random.Range(0.02f, 0.05f), //protein
		          Random.Range(0.0f, 0.01f),  //lipid
		          Random.Range(0.2f, 0.3f)); //cellulose
	     public float SurfaceArea { get; set; } = 0.01f; //this is technically the square root of surface area
	     public int NumberOfLeaves { get; set; } = 1;
	     public float ChloroplastDensity { get; set; } = 0.5f;
	     public Color LeafColor { get; set; } = new Color(0.5f, 1f, 0.5f);
	     public int LeafGrouping //leaf arrangement
	     {
		          get; set;
	     } = 1;

	     public LeafProperties(float surfaceArea, int leavesInCluster, float chloroplastDensity, float leafHueVariety, int cotyledons)
	     {
		          float hueVariety = leafHueVariety / 2;
		          SurfaceArea = surfaceArea;
		          LeafGrouping = leavesInCluster;
		          ChloroplastDensity = chloroplastDensity;
		          NumberOfLeaves = cotyledons;
		          LeafColor = Color.Lerp(new Color(0.5f, chloroplastDensity, 0.5f, 1), Color.HSVToRGB(Random.Range((0.5f - hueVariety), 0.5f + hueVariety), Random.Range(0.25f, 1), Random.Range(0.25f, 1)), 0.4f);
	     }
}
public class RootProperties
{//moves water from soil to plant and interacts with soil food web
	     public Organs RootOrgan
	     {
		          get; set;
	     } = new Organs(OrganType.Root,
		          Random.Range(0.7f, 0.9f), //water
		          Random.Range(0, 0.01f),  //glucose
		          Random.Range(0.1f, 0.3f), //starch
		          Random.Range(0.01f, 0.02f), //protein
		          Random.Range(0, 0.01f),  //lipid
		          Random.Range(0.1f, 0.2f)); //cellulose
	     public float Radius { get; set; } = 0.01f;
	     public float Depth { get; set; } = 0.1f;
}
public class FlowerProperties
{
	     public Organs FlowerOrgan
	     {
		          get; set;
	     } = new Organs(OrganType.Flower,
	         Random.Range(0.7f, 0.9f), //water
	         Random.Range(0.5f, 0.18f),  //glucose
	         Random.Range(0.0f, 0.01f), //starch
	         Random.Range(0.02f, 0.05f), //protein
	         Random.Range(0, 0.01f),  //lipid
	         Random.Range(0.1f, 0.2f)); //cellulose
	     public bool IsFlowering { get; set; } = false;
	     public float Size { get; set; } = Random.Range(0.01f, 0.5f);
	     public Color FlowerColor { get; set; } = Color.HSVToRGB(Random.value, Random.value, Random.Range(0.2f, 1f));
}
public class FruitProperties
{
	     public Organs FruitOrgan
	     {
		          get; set;
	     } = new Organs(OrganType.Fruit,
	         Random.Range(0.75f, 0.95f), //water
	         Random.Range(0.0f, 0.25f),  //glucose
	         Random.Range(0.1f, 0.3f), //starch
	         Random.Range(0.01f, 0.02f), //protein
	         Random.Range(0, 0.01f),  //lipid
	         Random.Range(0.1f, 0.2f)); //cellulose
	     public float Ripeness { get; set; } = 0;
	     public Color FruitColor { get; set; } = Color.HSVToRGB(Random.value, Random.Range(0.4f, 1f), Random.Range(0.2f, 1f));
}
public class StemProperties
{
	     public Organs StemOrgan
	     {
		          get; set;
	     }
	     public float Diameter { get; set; } = 0.001f; // 
	     public float Length { get; set; } = 0.01f; //
	     private Color leafColor; // Store the leaf color

	     // Constructor that accepts leaf color
	     public StemProperties(Color leafColor)
	     {
		          this.leafColor = leafColor;
		          StemOrgan = new Organs(OrganType.Stem,
		              Random.Range(0.6f, 0.8f), // water
		              Random.Range(0.0f, 0.01f),  // glucose
		              Random.Range(0.01f, 0.05f), // starch
		              Random.Range(0.01f, 0.02f), // protein
		              Random.Range(0.0f, 0.01f),  // lipid
		              Random.Range(0.2f, 0.3f)); // cellulose
		          Diameter = Random.Range(0.001f, 0.25f); //in meters
		          Length = Diameter * Random.Range(1f, 150f); // Reasonable length-to-width ratio
	     }

	     public Color StemColor
	     {
		          get
		          {
				Color brown = new Color(0.59f, 0.29f, 0.1f); // A brown color
				float lerpValue = Mathf.Clamp01((StemOrgan.CelluloseRatio - 0.2f) / (0.3f - 0.2f)); // Normalize between 0.2 and 0.3
				return Color.Lerp(leafColor, brown, lerpValue);
		          }
	     }
}


public class SeedProperties
{

	     public Organs SeedOrgan
	     {
		          get; set;
	     } = new Organs(OrganType.Seed,
		          Random.Range(0.1f, 0.15f), //water
		          Random.Range(0.0f, 0.01f),  //glucose
		          Random.Range(0.2f, 0.5f), //starch
		          Random.Range(0.1f, 0.3f), //protein
		          Random.Range(0.1f, 0.3f),  //lipid
		          Random.Range(0.05f, 0.1f)); //cellulose
				        //this should be the initial metabolic compounds a plant has
	     public int Amount { get; set; } = 0;
	     public float Size { get; set; } = 0.01f;
	     public MetabolicCompounds initialMetabolism
	     {
		          get; set;
	     }
	     public SeedProperties(Plant parent1, Plant parent2, MetabolicCompounds metabolism)
	     {
		          // Create baseline metabolism
		          if (parent1 == null || parent2 == null)
		          {
				// New plant from nothing - start with modest amounts
				initialMetabolism = new MetabolicCompounds(
				     h2O: 5000f,
				     o2: 5000f,
				     cO2: 1000f,
				     glucose: 2000f,    // Start with some glucose!
				     aDP: 1000f,
				     aTP: 500f,
				     aMP: 100f,
				     nADP: 1000f,
				     nADPH: 500f,
				     pyruvate: 100f,
				     pi: 1000f
				);
		          }
		          else
		          {
				// Inherit from parents
				initialMetabolism = new MetabolicCompounds(
				     h2O: (parent1.Seeds.initialMetabolism.H2O + parent2.Seeds.initialMetabolism.H2O) / 2 * 0.5f,
				     o2: (parent1.Seeds.initialMetabolism.O2 + parent2.Seeds.initialMetabolism.O2) / 2 * 0.5f,
				     cO2: (parent1.Seeds.initialMetabolism.CO2 + parent2.Seeds.initialMetabolism.CO2) / 2 * 0.5f,
				     glucose: (parent1.Seeds.initialMetabolism.Glucose + parent2.Seeds.initialMetabolism.Glucose) / 2 * 0.5f,
				     aDP: (parent1.Seeds.initialMetabolism.ADP + parent2.Seeds.initialMetabolism.ADP) / 2 * 0.5f,
				     aTP: (parent1.Seeds.initialMetabolism.ATP + parent2.Seeds.initialMetabolism.ATP) / 2 * 0.5f,
				     aMP: (parent1.Seeds.initialMetabolism.AMP + parent2.Seeds.initialMetabolism.AMP) / 2 * 0.5f,
				     nADP: (parent1.Seeds.initialMetabolism.NADP + parent2.Seeds.initialMetabolism.NADP) / 2 * 0.5f,
				     nADPH: (parent1.Seeds.initialMetabolism.NADPH + parent2.Seeds.initialMetabolism.NADPH) / 2 * 0.5f,
				     pyruvate: (parent1.Seeds.initialMetabolism.Pyruvate + parent2.Seeds.initialMetabolism.Pyruvate) / 2 * 0.5f,
				     pi: (parent1.Seeds.initialMetabolism.Pi + parent2.Seeds.initialMetabolism.Pi) / 2 * 0.5f
			);
		          }
	     }
}
