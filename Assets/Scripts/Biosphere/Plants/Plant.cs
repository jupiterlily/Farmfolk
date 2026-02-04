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
	     public float WaterLiters { get; private set; } = 100; //water in the plant in liters
	     public float TranspirationModifier { get; private set; } = 1;
	     private MetabolicPathways metabolicPathways = new MetabolicPathways();
	     public LSystem lSystem; //gets added and  instantiated with the plant
	     public TileData CurrentTile; //gets assigned when the plant is placed
	     public GameObject PlantObject; //gets added and instantiated with the plant
	     public Plant()
	     {
		          Leaves = new LeafProperties(UnityEngine.Random.Range(1, 50), Mathf.FloorToInt(UnityEngine.Random.Range(1, 5)), UnityEngine.Random.Range(0.5f, 1), 1f, Mathf.FloorToInt((UnityEngine.Random.Range(1, 3))));
		          Roots = new RootProperties();
		          Stems = new StemProperties(Leaves.LeafColor);
		          Flowers = new FlowerProperties();
		          Fruits = new FruitProperties();
		          //get opposite stem and leaf colours
		          float colourSeed = UnityEngine.Random.value;

		          //flip a coin to decide which is flower s and which is fruits
		          if (colourSeed >= 0.75f)
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
		          else if (colourSeed >= 0.5f && colourSeed < 0.75f)
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
		          else if (colourSeed >= 0.25f && colourSeed < 0.5f)
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
		          else // colourSeed <= 0.25f
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

	     public GameObject InstantiatePlant()
	     {
		          GameObject plantObject = new GameObject("Plant");
		          PlantObject = plantObject;

		          // Proper MonoBehaviour instantiation
		          lSystem = plantObject.AddComponent<LSystem>();

		          // Provide per-instance rules and axiom to the component
		          lSystem.Initialize(this, plantObject.transform.position);

		          // Draw immediately if desired (or let Demeter create stages)
		          lSystem.DrawPlant(axiom, plantObject);

		          return plantObject;
	     }

	     public Plant(Plant parent1, Plant parent2, MetabolicCompounds metabolism)
	     {

		          Leaves = new LeafProperties(UnityEngine.Random.Range(1, 50), Mathf.FloorToInt(UnityEngine.Random.Range(1, 5)), UnityEngine.Random.Range(0.5f, 1), 1f, Mathf.FloorToInt((UnityEngine.Random.Range(1, 3))));
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

