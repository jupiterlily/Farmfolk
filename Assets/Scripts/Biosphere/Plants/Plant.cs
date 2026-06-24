using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plant
{
	     public List<int> TimeConstantBirthday;
	     public string axiom = "s";
	     public LSystem_Ruleset Rules = new LSystem_Ruleset();
	     public bool isDead;
	     public LeafProperties Leaves
	     {
		          get; private set;
	     }
	     public RootProperties Roots
	     {
		          get; private set;
	     }
	     public StemProperties Stems
	     {
		          get; private set;
	     }
	     public FlowerProperties Flowers
	     {
		          get; private set;
	     }
	     public FruitProperties Fruits
	     {
		          get; private set;
	     }
	     public SeedProperties Seeds
	     {
		          get; private set;
	     }
	     public MetabolicCompounds MyBiochemistry
	     {
		          get; private set;
	     }
	     // public float WaterLiters { get; private set; } = 100; //water in the plant in liters
	     public float TranspirationModifier { get; private set; } = 1;
	     private MetabolicPathways metabolicPathways = new MetabolicPathways();
	     public LSystem lSystem; //gets added and  instantiated with the plant
	     public TileData OriginTile; //gets assigned when the plant is placed
			   //dictionary of tiles that the plant is in, with the tile as the key and the amount of roots in that tile as the value
	     public Dictionary<TileData, float> RootTiles = new Dictionary<TileData, float>();
	     public ThermalEnergy storedWater = new ThermalEnergy(100, "liquid water", 334000, 2260000, 273, 373, 0.6089f, 20f); //water in the plant in liters
	     public GameObject PlantObject; //gets added and instantiated with the plant
	     public Plant()
	     {
		          Leaves = new LeafProperties(
        UnityEngine.Random.Range(0.01f, 0.8f),        // SurfaceArea: 0.01-0.2 m² (100-2000 cm²)
        UnityEngine.Random.Range(0.03f, 1.2f),        // LeafLength: 5-20 cm (0.05-0.2 m)
        UnityEngine.Random.Range(0.0002f, 0.0005f),   // LeafThickness: 0.2-0.5 mm
        Mathf.FloorToInt(UnityEngine.Random.Range(0, 20)),
        UnityEngine.Random.Range(0.5f, 1),
        1f,
        Mathf.FloorToInt(UnityEngine.Random.Range(1, 3))
    );
		          Roots = new RootProperties();
		          Stems = new StemProperties(Leaves.LeafColor);
		          Flowers = new FlowerProperties();
		          Fruits = new FruitProperties();
		          //get opposite stem and leaf colours
		          float colourSeed = UnityEngine.Random.value;

		          //flip a coin to decide which is flower s and which is fruits
		          if (colourSeed >= 0.8f)
		          {
				Color leafComplementary = new Color(1 - Leaves.LeafColor.r, 1 - Leaves.LeafColor.g, 1 - Leaves.LeafColor.b);
				Color stemComplementary = new Color(1 - Stems.StemColor.r, 1 - Stems.StemColor.g, 1 - Stems.StemColor.b);
				if (UnityEngine.Random.value > 0.5f)
				{
					     Flowers.FlowerColor = leafComplementary;
					     Fruits.FruitColor = stemComplementary;
				}
				else
				{
					     Flowers.FlowerColor = stemComplementary;
					     Fruits.FruitColor = leafComplementary;
				}
		          }
		          else if (colourSeed >= 0.6f && colourSeed < 0.8f)
		          {
				Color midColor = new Color((Leaves.LeafColor.r + Stems.StemColor.r) / 2,
					            (Leaves.LeafColor.g + Stems.StemColor.g) / 2,
					            (Leaves.LeafColor.b + Stems.StemColor.b) / 2);
				Color midComplementary = new Color(1 - midColor.r, 1 - midColor.g, 1 - midColor.b);
				if (UnityEngine.Random.value > 0.5f)
				{
					     Flowers.FlowerColor = midColor;
					     Fruits.FruitColor = midComplementary;
				}
				else
				{
					     Flowers.FlowerColor = midComplementary;
					     Fruits.FruitColor = midColor;
				}
		          }
		          else if (colourSeed >= 0.4f && colourSeed < 0.6f)
		          {
				// Get distance between leaf and stem colors in RGB space
				float distanceR = Mathf.Abs(Leaves.LeafColor.r - Stems.StemColor.r);
				float distanceG = Mathf.Abs(Leaves.LeafColor.g - Stems.StemColor.g);
				float distanceB = Mathf.Abs(Leaves.LeafColor.b - Stems.StemColor.b);

				// Create colors on either side of leaf color by the same distance
				Color leafColorPlus = new Color(
				    Mathf.Clamp01(Leaves.LeafColor.r + distanceR),
				    Mathf.Clamp01(Leaves.LeafColor.g + distanceG),
				    Mathf.Clamp01(Leaves.LeafColor.b + distanceB)
				);

				Color leafColorMinus = new Color(
				    Mathf.Clamp01(Leaves.LeafColor.r - distanceR),
				    Mathf.Clamp01(Leaves.LeafColor.g - distanceG),
				    Mathf.Clamp01(Leaves.LeafColor.b - distanceB)
				);

				// Create colors on either side of stem color by the same distance
				Color stemColorPlus = new Color(
				    Mathf.Clamp01(Stems.StemColor.r + distanceR),
				    Mathf.Clamp01(Stems.StemColor.g + distanceG),
				    Mathf.Clamp01(Stems.StemColor.b + distanceB)
				);

				Color stemColorMinus = new Color(
				    Mathf.Clamp01(Stems.StemColor.r - distanceR),
				    Mathf.Clamp01(Stems.StemColor.g - distanceG),
				    Mathf.Clamp01(Stems.StemColor.b - distanceB)
				);

				// Randomly assign one pair to flower and fruit
				if (UnityEngine.Random.value > 0.5f)
				{
					     Flowers.FlowerColor = leafColorPlus;
					     Fruits.FruitColor = leafColorMinus;
				}
				else
				{
					     Flowers.FlowerColor = stemColorPlus;
					     Fruits.FruitColor = stemColorMinus;
				}
		          }
		          else if (colourSeed >= 0.2f && colourSeed < 0.4f)
		          {
				// For this case, we can do something else - maybe random colors or other scheme
				// Here's one option: create analogous colors (colors next to each other on the color wheel)

				// Convert leaf color to HSV to work with hue
				Color.RGBToHSV(Leaves.LeafColor, out float leafH, out float leafS, out float leafV);
				Color.RGBToHSV(Stems.StemColor, out float stemH, out float stemS, out float stemV);

				// Calculate average hue
				float avgH = (leafH + stemH) / 2f;

				// Create two analogous colors (+/- 0.1 on hue wheel)
				float hueVariation = 0.1f;
				float hue1 = Mathf.Repeat(avgH + hueVariation, 1f);
				float hue2 = Mathf.Repeat(avgH - hueVariation, 1f);

				// Use average saturation and value, or create variations
				float avgS = (leafS + stemS) / 2f;
				float avgV = (leafV + stemV) / 2f;

				Color analogous1 = Color.HSVToRGB(hue1, avgS, avgV);
				Color analogous2 = Color.HSVToRGB(hue2, avgS, avgV);

				// Randomly assign
				if (UnityEngine.Random.value > 0.5f)
				{
					     Flowers.FlowerColor = analogous1;
					     Fruits.FruitColor = analogous2;
				}
				else
				{
					     Flowers.FlowerColor = analogous2;
					     Fruits.FruitColor = analogous1;
				}
		          }
		          // Create initial metabolism FIRST
		          SeedProperties tempSeed = new SeedProperties(null, null, null);
		          MyBiochemistry = tempSeed.initialMetabolism;

		          // NOW create seeds with valid biochemistry
		          Seeds = new SeedProperties(null, null, MyBiochemistry);

		          TimeConstantBirthday = new List<int>(SimulationConstants.TimeConstants);
		          InitializeRules();
	     }
	     private void InitializeRules()
	     {
		          // First create mirrored rule pairs for all directional rules
		          Dictionary<string, string> mirroredRulePairs = new Dictionary<string, string>();
		          foreach (var ruleSet in LSystemLibrary.MasterRules)
		          {
				foreach (string rule in ruleSet.Value)
				{
					     if (rule.Contains("<") || rule.Contains(">") || rule.Contains("/") || rule.Contains("|") || rule.Contains("+") || rule.Contains("-"))
					     {
						          string mirrored = LSystem_Utilities.MirrorRule(rule);
						          if (!mirroredRulePairs.ContainsKey(rule) &&
						              !mirroredRulePairs.ContainsValue(rule))
						          {
								mirroredRulePairs.Add(rule, mirrored);
						          }
					     }
				}
		          }

		          // Now build the plant's rules
		          foreach (var kvp in LSystemLibrary.MasterRules)
		          {
				char symbol = kvp.Key;
				List<string> availableRules = new List<string>(kvp.Value);
				List<string> selectedRules = new List<string>();

				// Handle single-rule symbols (c, s, l, b, B)
				if (new[] { 'c', 's', 'l', 'b', 'B', 'T', 'S', 'f', 'A', 'C', 'D', 'E' }.Contains(symbol))
				{
					     // Select exactly one random rule
					     string selectedRule = availableRules[Random.Range(0, availableRules.Count)];
					     selectedRules.Add(selectedRule);

					     // Special case: if it's a directional rule, add its mirror
					     if (mirroredRulePairs.ContainsKey(selectedRule))
					     {
						          selectedRules.Add(mirroredRulePairs[selectedRule]);
					     }
				}
				else // Handle other symbols (S, etc.)
				{
					     // First add non-directional base rules
					     foreach (string rule in availableRules)
					     {
						          if (!rule.Contains("<") && !rule.Contains(">") && !rule.Contains("+") && !rule.Contains("-") &&
						              !rule.Contains("v") && !rule.Contains("^") && !rule.Contains("/") && !rule.Contains("|"))
						          {
								selectedRules.Add(rule);
						          }
					     }

					     // Then add directional rules in mirrored pairs
					     foreach (string rule in availableRules)
					     {
						          if (mirroredRulePairs.ContainsKey(rule) &&
						              selectedRules.Count < Random.Range(1, availableRules.Count / 2))
						          {
								// Add original and mirrored version together
								selectedRules.Add(rule);
								selectedRules.Add(mirroredRulePairs[rule]);
						          }
					     }

					     // Ensure we always have at least one rule
					     if (selectedRules.Count == 0)
					     {
						          selectedRules.Add(availableRules[0]);
						          if (mirroredRulePairs.ContainsKey(availableRules[0]))
						          {
								selectedRules.Add(mirroredRulePairs[availableRules[0]]);
						          }
					     }
				}
				Rules.rules.Add(symbol, selectedRules.ToArray());
				//Debug.Log($"Final rules for {symbol}: {string.Join(", ", selectedRules)}");
		          }
	     }

	     public GameObject InstantiatePlant(List<Sprite> LeafSprites)
	     {
		          GameObject plantObject = new GameObject("Plant");
		          PlantObject = plantObject;

		          // Proper MonoBehaviour instantiation
		          lSystem = plantObject.AddComponent<LSystem>();

		          // Provide per-instance rules and axiom to the component
		          lSystem.Initialize(this, plantObject.transform.position);

		          if (LeafSprites != null && LeafSprites.Count > 0)
		          {
				lSystem.leafSprite = LeafSprites[Leaves.LeafGrouping];
		          }
		          // Draw immediately if desired (or let Demeter create stages)
		          lSystem.DrawPlant(axiom, plantObject);

		          return plantObject;
	     }

	     public Plant(Plant parent1, Plant parent2, MetabolicCompounds metabolism)
	     {
		          //this is wrong, we should be chosing a random mix of properties from each parent
		          Leaves = new LeafProperties(
        Random.value > 0.5f ? parent1.Leaves.SurfaceArea : parent2.Leaves.SurfaceArea,        // SurfaceArea: 0.01-0.2 m² (100-2000 cm²)
        Random.value > 0.5f ? parent1.Leaves.LeafLength : parent2.Leaves.LeafLength,        // LeafLength: 5-20 cm (0.05-0.2 m)
        Random.value > 0.5f ? parent1.Leaves.LeafThickness : parent2.Leaves.LeafThickness,   // LeafThickness: 0.2-0.5 mm
        Random.value > 0.5f ? parent1.Leaves.LeafGrouping : parent2.Leaves.LeafGrouping,
        Random.value > 0.5f ? parent1.Leaves.ChloroplastDensity : parent2.Leaves.ChloroplastDensity,
        1f,
        Mathf.FloorToInt(UnityEngine.Random.Range(1, 3))
    );
		          Roots = new RootProperties();
		          Stems = new StemProperties(Leaves.LeafColor);
		          Flowers = new FlowerProperties();
		          Fruits = new FruitProperties();
		          Seeds = new SeedProperties(parent1, parent2, metabolism);
		          MyBiochemistry = Seeds.initialMetabolism;

		          // ALWAYS create a NEW copy at the moment of creation
		          TimeConstantBirthday = new List<int>(SimulationConstants.TimeConstants);
		          InitializeRules();
	     }
	     private float CalculateMaintenanceCost()
	     {
		          float leafMaintenanceCost = Leaves.SurfaceArea * Leaves.NumberOfLeaves * 0.01f;
		          float rootMaintenanceCost = Roots.Radius * Roots.Radius * 3.14f * 0.02f;
		          return leafMaintenanceCost + rootMaintenanceCost;
	     }
	     private float CalculateTranspirationRate(float lightIntensity)
	     {
		          float leafWaterUsage = Leaves.SurfaceArea * Leaves.NumberOfLeaves * TranspirationModifier * lightIntensity / SimulationConstants.SolarConstant;
		          float rootWaterArea = Roots.Radius * Roots.Radius * 3.14f;
		          float litersPerHour = leafWaterUsage / rootWaterArea;
		          return litersPerHour / 3600;
	     }
	     public void SimulateGrowth(float latitude, float lightIntensity, int duration)
	     {
		          Debug.Log($"Simulating growth for duration: {duration} with light intensity: {lightIntensity}");
		          //how much light hits 1cm2, multiplied by our availiable leaf surface area and then by chloroplast density
		          int totalLeafEfficiency = Mathf.FloorToInt((lightIntensity / 100) * (Leaves.NumberOfLeaves * Leaves.SurfaceArea) * Leaves.ChloroplastDensity);
		          Photosynthesis(totalLeafEfficiency);
		          Respiration(duration);
		          Transpire(OriginTile, lightIntensity, duration);
	     }
	     public void Photosynthesis(int watts)
	     {
		          Debug.Log($"Photosynthesis with {watts} watts.");
		          if (!isDead)
		          {
				if (watts > 0)
				{
					     MyBiochemistry = metabolicPathways.LightReactions(watts, MyBiochemistry);
				}

				MyBiochemistry = metabolicPathways.CarbonFixation(MyBiochemistry, Mathf.FloorToInt(Leaves.SurfaceArea * Leaves.NumberOfLeaves));
				MyBiochemistry = metabolicPathways.Photosynthesis(MyBiochemistry);
		          }
	     }
	     public void Respiration(int seconds)
	     {
		          Debug.Log($"Respiration for {seconds} seconds.");
		          if (!isDead)
		          {
				// Calculate how much glucose is available for respiration
				int glucoseAvailable = (int)MyBiochemistry.Glucose;

				if (glucoseAvailable > 0)
				{
					     // Only respire what we have
					     MyBiochemistry = metabolicPathways.Glycolysis(MyBiochemistry);
					     MyBiochemistry = metabolicPathways.CitricAcidCycle(MyBiochemistry);
					     // Use actual available glucose, not time in seconds
					     MyBiochemistry = metabolicPathways.Respiration(glucoseAvailable, MyBiochemistry);
				}

				if (!metabolicPathways.IsValidUpdate(MyBiochemistry))
				{
					     isDead = true;
				}
		          }
	     }

	     //transpiration. move water from soil tiles that roots are in. take it to plant stores and then push to atmosphere based on leaf surface area, number of leaves, and a transpiration modifier that can be affected by genetics and environment. also calculate how much water is available in the plant and if it runs out, the plant dies
	     // ─── Transpiration (updated to use refactored ThermalEnergy) ───
	     public void Transpire(TileData currentTile, float lightIntensity, int seconds)
	     {
		          if (isDead)
				return;

		          // Calculate transpiration rate (litres per second)
		          float stomatalConductance = CalculateTranspirationRate(lightIntensity) * seconds;
		          float vapourPressureDifference = (currentTile.atmosphereTile.temperature + 273.15f) * currentTile.atmosphereTile.RelativeHumidity / 100f;
		          float atmosphericPressure = currentTile.atmosphereTile.pressure;
		          float transpirationRate = stomatalConductance * vapourPressureDifference / atmosphericPressure;

		          // Ensure we don't transpirate more than available in soil
		          float soilWaterAvailable = currentTile.composition.water.Mass;
		          float waterToMove = Mathf.Min(transpirationRate, soilWaterAvailable);

		          if (waterToMove > 0f)
		          {
				// Move water from soil to atmosphere (most of it)
				float toAtmosphere = waterToMove * 0.98f;
				float toPlantStore = waterToMove * 0.02f; // small amount retained in plant

				// Transfer from soil to atmosphere (latent heat of vaporization will cool)
				currentTile.composition.water.TransferMassTo(currentTile.atmosphereTile.waterVapor, toAtmosphere);
				// Remove latent heat from the atmosphere's water vapor to simulate cooling
				currentTile.atmosphereTile.waterVapor.RemoveHeat(toAtmosphere * currentTile.atmosphereTile.waterVapor.LatentHeatVaporization);

				// Transfer some water from soil to plant store
				if (toPlantStore > 0f)
				{
					     currentTile.composition.water.TransferMassTo(storedWater, toPlantStore);
				}

				Debug.Log($"Plant at {currentTile.tileGridRef}: Transpired {waterToMove} litres over {seconds}s.");
		          }
		          else
		          {
				// Not enough soil water – transpirate what's left and potentially kill the plant later
				if (soilWaterAvailable > 0f)
				{
					     currentTile.composition.water.TransferMassTo(currentTile.atmosphereTile.waterVapor, soilWaterAvailable);
					     currentTile.atmosphereTile.waterVapor.RemoveHeat(soilWaterAvailable * currentTile.atmosphereTile.waterVapor.LatentHeatVaporization);
					     Debug.Log($"Plant at {currentTile.tileGridRef}: Transpired remaining {soilWaterAvailable} litres from soil.");
				}
				else
				{
					     Debug.Log($"Plant at {currentTile.tileGridRef}: No soil water available for transpiration.");
					     // Plant may die from drought – handle elsewhere
				}
		          }
	     }
	     public void Grow(float growthAmount, Organs organ)
	     {
		          if (!isDead)
		          {
				MyBiochemistry = metabolicPathways.GrowOrgan(organ, growthAmount, MyBiochemistry);
				Leaves.NumberOfLeaves += Mathf.FloorToInt(growthAmount);
		          }
	     }
	     public Plant GeneratePlantSeed()
	     {
		          //this is wrong, we should be deep copying the plant and its properties to create a new seed plant
		          Plant seed = this;
		          return seed;
	     }

}

