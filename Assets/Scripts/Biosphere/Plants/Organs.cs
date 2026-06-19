using System.Collections.Generic;
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

public enum Aroma
{
	     Peppery, Spicy, Smoky, Ashy, Alufuric, Mineral, Pungemt, Caustic, Alkaline, Astringent, Fatty, Meaty, Delicate, Mild, Nippy, Piquant, Tangy, Tart, Sour, Acrid, Floral, Fragrant, Zesty, Berries, Herbaceous, Vegetal, Grassy, Cereal, Malty, Nutty, Syrupy, Candy, Vanilla, Chocolatey, Creamy, Buttery, Oily, Leathery, Woody, Musky, Musty, Medicinal, Mentholy, Terpeney, Toasty, Earthy, Yeasty
}
public class Organs
{
	     public OrganType OrganType;
	     public float GrowthCost; // g glucose per cm³ of organ
	     public float MaintenanceCost; // g glucose per cm³ per day
	     public float WaterRatio;
	     public float GlucoseRatio;
	     public float StarchRatio;
	     public float ProteinRatio;
	     public float LipidRatio;
	     public float CelluloseRatio;

	     public Dictionary<Aroma, float> flavourProfile;//
				       // Density of organ tissue (g/cm³)
	     private float density = 1.0f; // approx. water density

	     public Organs(OrganType organType, float waterRatio, float glucoseRatio,
		   float starchRatio, float proteinRatio, float lipidRatio,
		   float celluloseRatio)
	     {
		          OrganType = organType;
		          WaterRatio = waterRatio;
		          GlucoseRatio = glucoseRatio;
		          StarchRatio = starchRatio;
		          ProteinRatio = proteinRatio;
		          LipidRatio = lipidRatio;
		          CelluloseRatio = celluloseRatio;

		          // Calculate biosynthesis cost
		          GrowthCost = CalculateBiosynthesisCost();

		          // Calculate maintenance cost based on organ type
		          MaintenanceCost = CalculateMaintenanceCost();


	     }

	     private float CalculateBiosynthesisCost()
	     {
		          // Cost = sum(compoundRatio * conversionCost * density)
		          float totalCost = 0f;

		          totalCost += GlucoseRatio * BiosynthesisCosts.GlucoseToGlucose;
		          totalCost += StarchRatio * BiosynthesisCosts.GlucoseToStarch;
		          totalCost += CelluloseRatio * BiosynthesisCosts.GlucoseToCellulose;
		          totalCost += ProteinRatio * BiosynthesisCosts.GlucoseToProtein;
		          totalCost += LipidRatio * BiosynthesisCosts.GlucoseToLipid;

		          // Multiply by tissue density to get g glucose per cm³
		          return totalCost * density;
	     }

	     private float CalculateMaintenanceCost()
	     {
		          // Maintenance costs vary by organ type:
		          // Leaves: high (protein turnover, photosynthesis machinery)
		          // Roots: medium (ion pumping, defense compounds)
		          // Stems: low (mostly structural)
		          // Flowers: very high (nectar production, pigment turnover)
		          // Fruits: high (sugar accumulation, pigment synthesis)
		          // Seeds: low (dormant)

		          float baseMaintenance = 0f;

		          switch (OrganType)
		          {
		          case OrganType.Leaf:
		          baseMaintenance = 0.02f * (1 + ProteinRatio * 2); // High protein turnover
		          break;
		          case OrganType.Root:
		          baseMaintenance = 0.015f * (1 + ProteinRatio * 1.5f); // Ion transport
		          break;
		          case OrganType.Stem:
		          baseMaintenance = 0.005f * (1 + CelluloseRatio * 0.5f); // Mostly structural
		          break;
		          case OrganType.Flower:
		          baseMaintenance = 0.03f * (1 + GlucoseRatio * 3); // High metabolic activity
		          break;
		          case OrganType.Fruit:
		          baseMaintenance = 0.025f * (1 + GlucoseRatio * 2); // Sugar transport
		          break;
		          case OrganType.Seed:
		          baseMaintenance = 0.001f; // Mostly dormant
		          break;
		          }

		          return baseMaintenance * density;
	     }

	     // Calculate the actual metabolic cost to grow a specific volume
	     public MetabolicCostReport CalculateGrowthMetabolism(float volume, bool includeMaintenance = true)
	     {
		          MetabolicCostReport report = new MetabolicCostReport();

		          // Biosynthesis cost
		          report.GlucoseRequired = GrowthCost * volume;

		          // Compound requirements for structure
		          report.WaterRequired = WaterRatio * density * volume;
		          report.GlucoseInStructure = GlucoseRatio * density * volume;
		          report.StarchInStructure = StarchRatio * density * volume;
		          report.ProteinInStructure = ProteinRatio * density * volume;
		          report.LipidInStructure = LipidRatio * density * volume;
		          report.CelluloseInStructure = CelluloseRatio * density * volume;
		          report.WaterRequired = WaterRatio * density * volume;
		          // ATP cost (1 glucose ≈ 36 ATP in respiration)
		          report.ATPCost = report.GlucoseRequired * 36f;

		          // Maintenance cost if growing over time
		          if (includeMaintenance)
		          {
				// Assume 1 day growth period
				report.MaintenanceGlucose = MaintenanceCost * volume;
				report.TotalGlucose = report.GlucoseRequired + report.MaintenanceGlucose;
		          }
		          else
		          {
				report.TotalGlucose = report.GlucoseRequired;
		          }

		          return report;
	     }
}

public class MetabolicCostReport
{
	     public float TotalGlucose;
	     public float GlucoseRequired;
	     public float MaintenanceGlucose;
	     public float ATPCost;
	     public float WaterRequired;
	     public float GlucoseInStructure;
	     public float StarchInStructure;
	     public float ProteinInStructure;
	     public float LipidInStructure;
	     public float CelluloseInStructure;
}

public class PlantGrowthManager
{
	     private MetabolicPathways metabolism;
	     private Plant plant;

	     public bool TryGrowOrgan(OrganType organType, float volume, float growthDurationDays)
	     {
		          Organs organTemplate = GetOrganTemplate(organType);
		          MetabolicCostReport cost = organTemplate.CalculateGrowthMetabolism(volume, true);

		          // Check if plant has enough resources
		          if (!CanAffordGrowth(cost))
				return false;

		          // Deduct resources for growth
		          MetabolicCompounds afterGrowth = metabolism.GrowOrgan(
		              organTemplate,
		              volume,
		              plant.MyBiochemistry
		          );

		          // Add maintenance costs over growth period
		          float maintenancePerDay = organTemplate.MaintenanceCost * volume;
		          float totalMaintenance = maintenancePerDay * growthDurationDays;

		          afterGrowth.Glucose -= (int)totalMaintenance;
		          afterGrowth.ATP -= (int)(totalMaintenance * 36f); // ATP used for maintenance

		          plant.MyBiochemistry.Glucose -= afterGrowth.Glucose;
		          plant.MyBiochemistry.ATP -= afterGrowth.ATP;

		          // Actually create the organ in the plant

		          return true;
	     }

	     private bool CanAffordGrowth(MetabolicCostReport cost)
	     {
		          return plant.MyBiochemistry.Glucose >= cost.TotalGlucose &&
			  plant.MyBiochemistry.ATP >= cost.ATPCost &&
			  plant.MyBiochemistry.H2O >= cost.WaterRequired;
	     }

	     private Organs GetOrganTemplate(OrganType type)
	     {
		          switch (type)
		          {
		          case OrganType.Leaf:
		          return plant.Leaves.LeafOrgan;
		          case OrganType.Root:
		          return plant.Roots.RootOrgan;
		          case OrganType.Stem:
		          return plant.Stems.StemOrgan;
		          case OrganType.Flower:
		          return plant.Flowers.FlowerOrgan;
		          case OrganType.Fruit:
		          return plant.Fruits.FruitOrgan;
		          case OrganType.Seed:
		          return plant.Seeds.SeedOrgan;
		          default:
		          return new Organs(type, 0.8f, 0.05f, 0.05f, 0.05f, 0.02f, 0.03f);
		          }
	     }

	     // Calculate organ size from L-system parameters
	     public float CalculateOrganVolume(OrganType type, string lSystemSymbol, float stemDiameter = 0.01f)
	     {
		          switch (type)
		          {
		          case OrganType.Leaf:
		          // Leaf volume = surface area * thickness (approx 0.1mm)
		          return plant.Leaves.SurfaceArea * 0.0001f * plant.Leaves.NumberOfLeaves;

		          case OrganType.Stem:
		          // Stem volume = π * r² * length
		          float radius = stemDiameter / 2f;
		          return Mathf.PI * radius * radius * 0.1f; // Assume 10cm segment

		          case OrganType.Root:
		          // Similar to stem but thinner
		          return Mathf.PI * (stemDiameter / 4) * (stemDiameter / 4) * 0.1f;

		          case OrganType.Flower:
		          // Flower volume = sphere volume * size factor
		          float flowerRadius = plant.Flowers.Size / 2f;
		          return (4f / 3f) * Mathf.PI * flowerRadius * flowerRadius * flowerRadius;

		          case OrganType.Fruit:
		          // Fruit volume
		          float fruitRadius = plant.Fruits.Size / 2f;
		          return (4f / 3f) * Mathf.PI * fruitRadius * fruitRadius * fruitRadius;

		          case OrganType.Seed:
		          // Seed volume (small sphere)
		          return (4f / 3f) * Mathf.PI * 0.001f * 0.001f * 0.001f; // ~1mm seed

		          default:
		          return 0.001f; // Default small volume
		          }
	     }
}
public class BiosynthesisCosts
{
	     // Glucose required to build 1g of each compound (in g glucose/g compound)
	     // Based on biochemical pathways:
	     public const float GlucoseToGlucose = 1f;      // No conversion needed
	     public const float GlucoseToStarch = 1.1f;     // ~10% energy cost for polymerization
	     public const float GlucoseToCellulose = 1.3f;  // Higher for cellulose synthesis
	     public const float GlucoseToProtein = 2.4f;    // Very expensive! Amino acid synthesis + N fixation
	     public const float GlucoseToLipid = 3.1f;      // Most expensive - fatty acid synthesis
}
public class LeafProperties
{
	     /// <summary>
	     /// Unit system for leaf dimensions:
	     /// - All measurements in meters
	     /// - SurfaceArea: approximate leaf surface area (m²)
	     /// - LeafLength: from base to tip (m)
	     /// - LeafWidth: computed width (m)
	     /// - LeafThickness: estimated average thickness (m)
	     /// 
	     /// Typical values (Earth reference):
	     /// - Small leaves: 0.01-0.05 m² (clover, grass)
	     /// - Medium leaves: 0.05-0.15 m² (typical broadleaf)
	     /// - Large leaves: 0.2+ m² (hosta, banana)
	     /// - Leaf thickness: 0.0002-0.0005 m (0.2-0.5mm)
	     /// </summary>

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

	     /// <summary>
	     /// Approximate leaf surface area in m². Represents actual 2D leaf area,
	     /// not the square root. Used for photosynthesis calculations.
	     /// </summary>
	     public float SurfaceArea { get; set; } = 0.02f; // ~200 cm² (medium leaf)

	     /// <summary>
	     /// Leaf thickness in meters. Typical values: 0.0002-0.0005 m (0.2-0.5 mm).
	     /// Used for geometry and visual thickness.
	     /// </summary>
	     public float LeafThickness { get; set; } = 0.0003f; // 0.3mm - typical leaf thickness

	     /// <summary>
	     /// Approximate leaf length (base to tip) in meters.
	     /// Typical values: 0.05-0.20 m (5-20 cm).
	     /// </summary>
	     public float LeafLength { get; set; } = 0.08f; // 8cm - typical leaf

	     /// <summary>
	     /// Approximate leaf width derived from surface area and length.
	     /// Width ≈ SurfaceArea / LeafLength.
	     /// For ~0.02 m² area and 0.08 m length: width ≈ 0.025 m (2.5 cm).
	     /// </summary>
	     public float LeafWidth => SurfaceArea / LeafLength;

	     public int NumberOfLeaves { get; set; } = 1;
	     public float ChloroplastDensity { get; set; } = 0.5f;
	     public float StomataDensity { get; set; } = Random.Range(52f, 261f);//stomata per mm2

	     public Color LeafColor { get; set; } = new Color(0.5f, 1f, 0.5f);
	     public int LeafGrouping //leaf arrangement
	     {
		          get; set;
	     } = 1;

	     public LeafProperties(float surfaceArea, float leafLength, float leafThickness, int leavesInCluster, float chloroplastDensity, float leafHueVariety, int cotyledons)
	     {
		          float hueVariety = leafHueVariety / 2;

		          // Ensure positive dimensions (clamp to reasonable ranges)
		          SurfaceArea = surfaceArea; // 0.001 - 1 m² range
		          LeafLength = leafLength; // 1-50 cm range
		          LeafThickness = leafThickness; // 0.01-2mm range

		          LeafGrouping = leavesInCluster;
		          ChloroplastDensity = chloroplastDensity;
		          NumberOfLeaves = cotyledons;

		          LeafColor = Color.Lerp(new Color(0.5f, chloroplastDensity, 0.5f, 1),
			      Color.HSVToRGB(Random.Range((0.5f - hueVariety), 0.5f + hueVariety), Random.Range(0.25f, 1), Random.Range(0.25f, 1)), 0.4f);
	     }
	     public void AddorRemoveLeaves(int additionalLeaves)
	     {
		          NumberOfLeaves += additionalLeaves;
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
	     public float Size { get; set; } = Random.Range(0.0001f, 0.3f);
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
	     public float Size { get; set; } = Random.Range(0.0001f, 0.3f); //diameter in meters	     

	     public int FruitCount { get; set; } = 0;
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
	     public float Volume { get; set; } = 0;
	     public float Mass => StemOrgan.CalculateTotalMass(Volume); // in kg
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
		          Length = Diameter * Random.Range(1f, 50f); // Reasonable length-to-width ratio
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
	     public void AddStemLengths(int additionalStems)
	     {
		          Volume += additionalStems * Length * Diameter * Diameter;
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
