using System.Collections.Generic;
using UnityEngine;

public class Demeter : MonoBehaviour
{
	     [Header("Plant Management Settings")]
	     public LightManager lightManager;
	     public Gardener gardener;
	     public TileManager tileManager;
	     public Vector3 NightTint = new Vector3(0.5f, 0.9f, 0.2f);
	     public Vector3 HydrationTint = new Vector3(0.0f, 0.9f, 1.0f);


	     [Header("Plant Generation Settings")]
	     [Tooltip("Number of plants to create initially")]
	     public int initialPlantCount = 8;
	     [Tooltip("Radius around center where plants will be activated")]
	     public float activationRadius = 64f;
	     [Tooltip("Bounds for random plant placement")]
	     public Vector2 xBounds = new Vector2(-8.33f, 8.33f);
	     public Vector2 yBounds = new Vector2(-4.45f, 4.45f);
	     public List<Vector2> claimedSpots;

	     // Plant collections
	     public List<Plant> AllPlants { get; private set; } = new List<Plant>();
	     public Dictionary<Plant, GameObject> ActivePlants { get; private set; } = new Dictionary<Plant, GameObject>();
	     public Dictionary<Plant, Vector3> PlantMap { get; private set; } = new Dictionary<Plant, Vector3>();
	     public Dictionary<Plant, string> PlantCurrentStage { get; private set; } = new Dictionary<Plant, string>();

	     private Dictionary<string, string> stageAxiomCache = new Dictionary<string, string>();

	     private void Awake()
	     {
	     }


	     private void Start()
	     {
		          Debug.Log("Demeter initialization started.");
		          // Don't initialize here - wait for SimulationMaster to call it
	     }

	     public void InitializePlants(List<TileData> locations)
	     {
		          Debug.Log($"InitializePlants called with initialPlantCount: {initialPlantCount}");
		          BuildPlantDictionaries(locations);
		          Debug.Log($"Demeter initialized with {AllPlants.Count} total plants, {ActivePlants.Count} active.");
	     }

	     /// <summary>
	     /// Gets random coordinates within visible bounds
	     /// </summary>
	     public Vector3 GetRandomPosition()
	     {
		          return new Vector3(
	         Random.Range(xBounds.x, xBounds.y), 0,
	         Random.Range(yBounds.x, yBounds.y)
	     );
	     }

	     /// <summary>
	     /// Initializes plant dictionaries and creates initial plants
	     /// </summary>
	     private void BuildPlantDictionaries(List<TileData> locations)
	     {
		          PlantMap = new Dictionary<Plant, Vector3>();
		          AllPlants = new List<Plant>();
		          ActivePlants = new Dictionary<Plant, GameObject>();
		          PlantCurrentStage = new Dictionary<Plant, string>();

		          Vector3 activationCenter = GetRandomPosition();
		          Debug.Log($"Creating {locations.Count} plants around activation center: {activationCenter}");

		          for (int i = 0; i < locations.Count; i++)
		          {
				CreateNewPlant(i, locations[i]);
		          }
	     }

	     /// <summary>
	     /// Creates a new plant and adds it to the appropriate collections
	     /// </summary>
	     public void CreateNewPlant(int index, TileData tile)
	     {
		          Plant newPlant = new Plant();
		          Vector3 plantPosition = new Vector3(tile.tileGridRef.x, tile.volume, tile.tileGridRef.y);

		          // Add to all plants regardless of activation status
		          PlantMap.Add(newPlant, plantPosition);
		          AllPlants.Add(newPlant);
		          PlantCurrentStage.Add(newPlant, "Seedling");
		          newPlant.CurrentTile = tile;
		          ActivatePlant(newPlant, plantPosition, index);
	     }

	     /// <summary>
	     /// Activates a plant by creating its GameObject and initializing its LSystem
	     /// </summary>
	     private void ActivatePlant(Plant plant, Vector3 position, int index)
	     {
		          //  Debug.Log($"ActivatePlant called for plant {index} at {position}");

		          // Create parent GameObject for all growth stages
		          GameObject plantContainer = new GameObject($"Plant_{index}");
		          plantContainer.transform.position = position;
		          //plantContainer.transform.SetParent(transform);

		          // Add LSystem component
		          plant.lSystem = plantContainer.AddComponent<LSystem>();

		          //  Debug.Log($"LSystem component created: {plant.lSystem != null}");

		          // Initialize LSystem with the plant
		          //  Debug.Log("Calling Initialize...");
		          plant.lSystem.Initialize(plant, position);

		          //  Debug.Log($"Initialize complete. Checking LSystem state...");

		          // Check if initialization was successful
		          if (plant.lSystem == null)
		          {
				Debug.LogError("LSystem failed to initialize!");
				return;
		          }
		          else
		          {
				//iterate randomly between 3 and 7
				plant.axiom = plant.lSystem.GenerateLSystem(plant.axiom, Random.Range(3, 9));
				plant.lSystem.axiom = plant.axiom;
				plant.lSystem.DrawPlant(plant.axiom, plantContainer);
		          }

		          // Add to active plants
		          ActivePlants.Add(plant, plantContainer);
		          //    Debug.Log($"Activated plant {index} at {position}");
	     }

	     /// <summary>
	     /// Generates and displays the appropriate growth stage on demand
	     /// </summary>
	     public void UpdatePlantGrowthStage(Plant plant, string stageName)
	     {
		          if (!ActivePlants.ContainsKey(plant) || PlantCurrentStage[plant] == stageName)
				return;

		          GameObject plantContainer = ActivePlants[plant];

		          // Clear old stage
		          foreach (Transform child in plantContainer.transform)
		          {
				Destroy(child.gameObject);
		          }

		          // Generate the new stage
		          string axiom = GenerateStageAxiom(plant, stageName);
		          CreateGrowthStage(stageName, axiom, plantContainer, plant);
		          PlantCurrentStage[plant] = stageName;
		          //the amount of 'L' in the axiom represents the number of leaves
		          plant.Leaves.NumberOfLeaves = axiom.Split('L').Length - 1;
		          //update plant axiom.
		          plant.axiom = axiom;
		          plant.lSystem.axiom = axiom;
	     }

	     /// <summary>
	     /// Generates axiom for a specific growth stage - progressive iteration
	     /// </summary>
	     private string GenerateStageAxiom(Plant plant, string stageName)
	     {
		          // Check cache first
		          string cacheKey = $"{plant.GetHashCode()}_{stageName}";

		          if (stageAxiomCache.ContainsKey(cacheKey))
		          {
				return stageAxiomCache[cacheKey];
		          }

		          // Get iterations needed for this stage
		          int iterations = GetIterationsForStage(stageName);

		          // Start from the plant's current axiom (progressive growth)
		          string currentAxiom = plant.axiom;

		          // Generate additional iterations without consolidating yet		          
		          currentAxiom = plant.lSystem.GenerateLSystem(currentAxiom, iterations);


		          // Only consolidate at the end for rendering
		          currentAxiom = LSystem_Utilities.CosolidateDirection(currentAxiom);

		          stageAxiomCache[cacheKey] = currentAxiom;
		          return currentAxiom;
	     }

	     /// <summary>
	     /// Returns the iteration depth for each growth stage
	     /// </summary>
	     private int GetIterationsForStage(string stageName)
	     {
		          return stageName switch
		          {
				"Seedling" => 3,      // Initial growth
				"Juvenile" => 3,      // Additional iterations
				"Established" => 3,   // Additional iterations
				"Mature" => 3,        // Additional iterations
				"Ancient" => 3,       // Additional iterations
				_ => 1 // Default case
		          };
	     }

	     /// <summary>
	     /// Creates a single growth stage GameObject
	     /// </summary>
	     private void CreateGrowthStage(string stageName, string axiom, GameObject parent, Plant plant)
	     {
		          GameObject stageObject = new GameObject(stageName);
		          stageObject.transform.SetParent(parent.transform);
		          stageObject.transform.localPosition = Vector3.zero;

		          // Draw the plant for this stage
		          plant.lSystem.DrawPlant(axiom, stageObject);
	     }

	     /// <summary>
	     /// Updates which plants should be active based on camera position
	     /// </summary>
	     public void UpdatePlantActivation(Vector3 cameraPosition)
	     {
		          List<Plant> toActivate = new List<Plant>();
		          List<Plant> toDeactivate = new List<Plant>();

		          foreach (var plant in AllPlants)
		          {
				float distance = Vector3.Distance(PlantMap[plant], cameraPosition);
				bool shouldBeActive = distance <= activationRadius;

				if (shouldBeActive && !ActivePlants.ContainsKey(plant))
				{
					     toActivate.Add(plant);
				}
				else if (!shouldBeActive && ActivePlants.ContainsKey(plant))
				{
					     toDeactivate.Add(plant);
				}
		          }

		          // Process activation/deactivation
		          foreach (var plant in toActivate)
		          {
				ActivatePlant(plant, PlantMap[plant], ActivePlants.Count);
		          }

		          foreach (var plant in toDeactivate)
		          {
				RemovePlant(plant);
		          }
	     }

	     /// <summary>
	     /// Removes a plant from the active collection and destroys its GameObject
	     /// </summary>
	     public void RemovePlant(Plant plant)
	     {
		          if (ActivePlants.TryGetValue(plant, out GameObject plantObj))
		          {
				Destroy(plantObj);
				//add the plants mass to the soil organic matter of the tile it's on
				plant.CurrentTile.composition.organicMaterial.ChangeMass(plant.CurrentTile.composition.organicMass + Plant_Utilities.CalculateDryMass(plant));
				plant.CurrentTile.composition.water.ChangeMass(plant.CurrentTile.composition.organicMass + Plant_Utilities.CalculateWaterMass(plant));
				ActivePlants.Remove(plant);
				PlantCurrentStage.Remove(plant);
		          }
	     }

	     /// <summary>
	     /// Removes all dead plants from the collections
	     /// </summary>
	     public void RemoveDeadPlants(List<Plant> deadPlants)
	     {
		          Debug.Log($"Removing {deadPlants.Count} dead plants.");

		          foreach (var deadPlant in deadPlants)
		          {
				RemovePlant(deadPlant);
				PlantMap.Remove(deadPlant);
				AllPlants.Remove(deadPlant);
				// Don't remove from PlantCurrentStage here - RemovePlant already does it
		          }
	     }
}