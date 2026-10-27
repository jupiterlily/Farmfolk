using System;

public static class Plant_Utilities
{
	     //get plant mass in kg
	     public static float GetPlantWaterMassKg(Plant plant)
	     {
		          float mass = 0f;

		          return mass;
	     }

	     public static float CalculateDryMass(Plant plant)
	     {
		          float dryMassGrams = 0f;
		          dryMassGrams += plant.Leaves.LeafOrgan.CalculateDryMass(plant.Leaves.NumberOfLeaves * plant.Leaves.SurfaceArea / plant.Leaves.LeafLength * plant.Leaves.LeafThickness * plant.Leaves.LeafLength);
		          dryMassGrams += plant.Stems.StemOrgan.CalculateDryMass(plant.Stems.Volume);
		          dryMassGrams += plant.Fruits.FruitOrgan.CalculateDryMass(plant.Fruits.FruitCount * MathF.PI * (MathF.Pow(plant.Fruits.Size, 3) / 6f));
		          return dryMassGrams / 1000f;
	     }

	     public static float CalculateWaterMass(Plant plant)
	     {
		          float waterMassGrams = 0f;
		          waterMassGrams += plant.Leaves.LeafOrgan.CalculateWaterMass(plant.Leaves.NumberOfLeaves * plant.Leaves.SurfaceArea / plant.Leaves.LeafLength * plant.Leaves.LeafThickness * plant.Leaves.LeafLength);
		          waterMassGrams += plant.Stems.StemOrgan.CalculateWaterMass(plant.Stems.Volume);
		          waterMassGrams += plant.Fruits.FruitOrgan.CalculateWaterMass(plant.Fruits.FruitCount * MathF.PI * (MathF.Pow(plant.Fruits.Size, 3) / 6f));
		          return waterMassGrams / 1000f;
	     }

	     public static MetabolicCompounds CalcualteTotalMetabolicCost(Plant plant, int leaves, int flowers, int fruits, int stems)
	     {
		          MetabolicCompounds comp = new MetabolicCompounds();
		          //get fruit volume, get leaf volume, get stem volume, get 
		          float leafVolume = leaves * plant.Leaves.SurfaceArea / plant.Leaves.LeafLength * plant.Leaves.LeafThickness * plant.Leaves.LeafLength;
		          float stemVolume = stems * plant.Stems.Length * plant.Stems.Diameter * plant.Stems.Diameter;
		          float fruitVolume = fruits * MathF.PI * (MathF.Pow(plant.Fruits.Size, 3) / 6f);

		          MetabolicCostReport leafReport = plant.Leaves.LeafOrgan.CalculateGrowthMetabolism(leafVolume);
		          MetabolicCostReport stemReport = plant.Stems.StemOrgan.CalculateGrowthMetabolism(stemVolume);
		          MetabolicCostReport fruitReport = plant.Fruits.FruitOrgan.CalculateGrowthMetabolism(fruitVolume);
		          return comp;
	     }

}

public static class OrganUtilities
{
	     // Density constants in g/cm³
	     private const float WATER_DENSITY = 1.0f;
	     private const float GLUCOSE_DENSITY = 1.54f;
	     private const float STARCH_DENSITY = 1.5f;
	     private const float PROTEIN_DENSITY = 1.35f;
	     private const float LIPID_DENSITY = 0.9f;
	     private const float CELLULOSE_DENSITY = 1.5f;

	     private const float CUBIC_METERS_TO_CUBIC_CM = 1000000f;
	     private const float GRAMS_TO_KG = 0.001f;

	     // Assumes ratios are volume fractions that sum to 1.0
	     public static float CalculateDryMass(this Organs organ, float volumeM3)
	     {
		          if (organ == null || volumeM3 <= 0)
				return 0f;

		          float totalVolumeCm3 = volumeM3 * CUBIC_METERS_TO_CUBIC_CM;

		          // Calculate mass of each dry component
		          float dryMassGrams =
		              (organ.GlucoseRatio * totalVolumeCm3 * GLUCOSE_DENSITY) +
		              (organ.StarchRatio * totalVolumeCm3 * STARCH_DENSITY) +
		              (organ.ProteinRatio * totalVolumeCm3 * PROTEIN_DENSITY) +
		              (organ.LipidRatio * totalVolumeCm3 * LIPID_DENSITY) +
		              (organ.CelluloseRatio * totalVolumeCm3 * CELLULOSE_DENSITY);

		          return dryMassGrams * GRAMS_TO_KG;
	     }

	     public static float CalculateWaterMass(this Organs organ, float volumeM3)
	     {
		          if (organ == null || volumeM3 <= 0)
				return 0f;

		          float totalVolumeCm3 = volumeM3 * CUBIC_METERS_TO_CUBIC_CM;
		          float waterMassGrams = organ.WaterRatio * totalVolumeCm3 * WATER_DENSITY;
		          return waterMassGrams * GRAMS_TO_KG;
	     }

	     public static float CalculateTotalMass(this Organs organ, float volumeM3)
	     {
		          if (organ == null || volumeM3 <= 0)
				return 0f;

		          float totalVolumeCm3 = volumeM3 * CUBIC_METERS_TO_CUBIC_CM;

		          float totalMassGrams =
		              (organ.WaterRatio * totalVolumeCm3 * WATER_DENSITY) +
		              (organ.GlucoseRatio * totalVolumeCm3 * GLUCOSE_DENSITY) +
		              (organ.StarchRatio * totalVolumeCm3 * STARCH_DENSITY) +
		              (organ.ProteinRatio * totalVolumeCm3 * PROTEIN_DENSITY) +
		              (organ.LipidRatio * totalVolumeCm3 * LIPID_DENSITY) +
		              (organ.CelluloseRatio * totalVolumeCm3 * CELLULOSE_DENSITY);

		          return totalMassGrams * GRAMS_TO_KG;
	     }
}
