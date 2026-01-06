using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plant
{
	     public List<int> TimeConstantBirthday;
	     public string axiom = "s";
	     public Dictionary<char, string[]> Rules = new Dictionary<char, string[]>();
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
	     public GameObject PlantObject; //gets added and instantiated with the plant
	     public Plant()
	     {
		          Leaves = new LeafProperties(UnityEngine.Random.Range(1, 50), Mathf.FloorToInt(UnityEngine.Random.Range(1, 5)), UnityEngine.Random.Range(0.5f, 1), 1f, Mathf.FloorToInt((UnityEngine.Random.Range(1, 3))));
		          Roots = new RootProperties();
		          Stems = new StemProperties(Leaves.LeafColor);
		          Flowers = new FlowerProperties();
		          Fruits = new FruitProperties();

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
		          foreach (var ruleSet in LSystem.MasterRules)
		          {
				foreach (string rule in ruleSet.Value)
				{
					     if (rule.Contains("<") || rule.Contains(">") || rule.Contains("/") || rule.Contains("|") || rule.Contains("v") || rule.Contains("^"))
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
		          foreach (var kvp in LSystem.MasterRules)
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
						          if (!rule.Contains("<") && !rule.Contains(">") &&
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

				Rules.Add(symbol, selectedRules.ToArray());
				Debug.Log($"Final rules for {symbol}: {string.Join(", ", selectedRules)}");
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

