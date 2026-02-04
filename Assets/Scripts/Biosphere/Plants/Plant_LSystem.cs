using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LSystem : MonoBehaviour
{
	     [Header("L-System Configuration")]
	     [Tooltip("The initial axiom (starting string) for the L-system")]
	     public string axiom = "s";

	     [Tooltip("Scale factor for each iteration")]
	     public float iterationScaleFactor = 0.8f;

	     // Rules - kept the same as yours

	     //public static Dictionary<char, string> currentRules = new Dictionary<char, string>();

	     public LSystem_Ruleset rulesets;
	     // Private fields
	     private Plant plant;
	     private Vector3 position;
	     public float angle;
	     public AnimationCurve widthCurve;

	     // A simpler approach: store all lines to be drawn
	     private List<BranchData> allBranches = new List<BranchData>();
	     private GameObject plantContainer;

	     // Data structure for branches
	     private class BranchData
	     {
		          public Vector3 startPos;
		          public Vector3 endPos;
		          public float startWidth;
		          public float endWidth;
		          public Color color;
		          public GameObject parent;
		          public int depth;
	     }

	     // Stack state for L-system processing
	     private struct TurtleState
	     {
		          public Vector3 position;
		          public Vector3 direction;
		          public Quaternion rotation;
		          public float currentWidth;
		          public int depth;
	     }

	     /// <summary>
	     /// Initialize the L-system with plant data
	     /// </summary>
	     public void Initialize(Plant plant, Vector3 position)
	     {
		          this.plant = plant;
		          this.position = position;

		          if (!string.IsNullOrEmpty(plant.axiom))
		          {
				axiom = plant.axiom;
		          }

		          // Calculate angle
		          this.angle = Mathf.Lerp(7.5f, 30f,
		              Mathf.Lerp(0.0f, 1f, plant.Stems.StemColor.g));

		          SetupWidthCurve();
	     }

	     private void SetupWidthCurve()
	     {
		          widthCurve = new AnimationCurve(
		              new Keyframe(0, plant.Stems.Diameter, 0, 0.1f),
		              new Keyframe(0.5f, plant.Stems.Diameter * 0.9f, -0.2f, -0.2f),
		              new Keyframe(1, plant.Stems.Diameter * 0.7f)
		          );
		          widthCurve.preWrapMode = WrapMode.Clamp;
		          widthCurve.postWrapMode = WrapMode.Clamp;
	     }

	     /// <summary>
	     /// Main method to draw the plant
	     /// </summary>
	     public void DrawPlant(string axiomToDraw, GameObject plantGameObject)
	     {
		          if (string.IsNullOrEmpty(axiomToDraw))
		          {
				Debug.LogError("Cannot draw plant - axiom string is empty!");
				return;
		          }

		          plantContainer = plantGameObject;
		          allBranches.Clear();

		          // Clear existing LineRenderers
		          foreach (var lr in plantGameObject.GetComponentsInChildren<LineRenderer>())
		          {
				Destroy(lr.gameObject);
		          }

		          // Process the axiom
		          ProcessAxiom(axiomToDraw);

		          // Draw all collected branches
		          DrawAllBranches();
	     }

	     /// <summary>
	     /// Process the entire axiom string
	     /// </summary>
	     private void ProcessAxiom(string axiom)
	     {
		          Stack<TurtleState> stateStack = new Stack<TurtleState>();
		          TurtleState currentState = new TurtleState
		          {
				position = position,
				direction = Vector3.up,
				rotation = Quaternion.identity,
				currentWidth = plant.Stems.Diameter,
				depth = 0
		          };

		          // Track the current line segment points
		          List<Vector3> currentLinePoints = new List<Vector3>();
		          List<float> currentLineWidths = new List<float>();

		          // Start with initial point
		          currentLinePoints.Add(currentState.position);
		          currentLineWidths.Add(currentState.currentWidth);

		          int leafCount = 0;

		          for (int i = 0; i < axiom.Length; i++)
		          {
				char c = axiom[i];

				switch (c)
				{
				case 'S': // Draw stem segment
				ProcessStemSegment(ref currentState, ref currentLinePoints, ref currentLineWidths);
				plant.Stems.AddStemLengths(1);
				break;

				case '[': // Save state and start new branch
				          // Finalize current line if it has points
				if (currentLinePoints.Count > 1)
				{
					     CreateLineRendererFromPoints(currentLinePoints, currentLineWidths, currentState.depth);
				}

				// Save current state
				stateStack.Push(currentState);

				// Start new line for the branch
				currentLinePoints.Clear();
				currentLineWidths.Clear();
				currentLinePoints.Add(currentState.position);
				currentLineWidths.Add(currentState.currentWidth * 0.8f); // Branches are thinner

				currentState.depth++;
				break;

				case ']': // End current branch and restore state
				          // Finalize current branch line
				if (currentLinePoints.Count > 1)
				{

					     CreateLineRendererFromPoints(currentLinePoints, currentLineWidths, currentState.depth);
				}

				// Restore previous state
				if (stateStack.Count > 0)
				{
					     currentState = stateStack.Pop();

					     // Start new line from the branch point
					     currentLinePoints.Clear();
					     currentLineWidths.Clear();
					     currentLinePoints.Add(currentState.position);
					     currentLineWidths.Add(currentState.currentWidth);
				}
				break;

				case '+': // Turn right
				currentState.rotation *= Quaternion.Euler(0, 0, -angle);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case '-': // Turn left
				currentState.rotation *= Quaternion.Euler(0, 0, angle);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				// Use Z rotations for these as well (they previously used Y/X and had no effect on Vector3.up)
				case '>': // Yaw right
				currentState.rotation *= Quaternion.Euler(0, angle, 0);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case '<': // Yaw left
				currentState.rotation *= Quaternion.Euler(0, -angle, 0);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case '^': // Pitch up
				currentState.rotation *= Quaternion.Euler(-angle, 0, 0);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case 'v': // Pitch down
				currentState.rotation *= Quaternion.Euler(angle, 0, 0);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case '/': // Roll right
				currentState.rotation *= Quaternion.Euler(0, 0, angle);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case '|': // Roll left
				currentState.rotation *= Quaternion.Euler(0, 0, -angle);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case 'L': // Create leaf
				CreateLeaf(currentState.position, currentState.direction, currentState.currentWidth);
				leafCount++;
				plant.Leaves.AddorRemoveLeaves(1);
				break;

				case 'f': // Create flower
				CreateFlower(currentState.position, currentState.direction, currentState.currentWidth);
				break;

				case 'F': // Create fruit
				CreateFruit(currentState.position, currentState.direction, currentState.currentWidth);
				plant.Fruits.FruitCount++;
				break;

				case 'B': // Branch indicator (just a marker, doesn't draw)
				case 'C': // Branch indicator (just a marker, doesn't draw)
				case 'D': // Branch indicator (just a marker, doesn't draw)
				case 'E': // Branch indicator (just a marker, doesn't draw)
				case 'A': // Apical bud
				case 'a': // Axillary bud
				case 'T': // Terminal bud
				case 'b': // Bud
				case 'l': // leafNode
				case 'c': // Cotyledon
				case 's': // Seed
				case 'r': // Root
				          // These symbols are structural but don't draw directly
				break;

				default:
				Debug.LogWarning($"Unhandled L-system symbol: '{c}'");
				break;
				}
		          }

		          // Finalize the main line
		          if (currentLinePoints.Count > 1)
		          {
				CreateLineRendererFromPoints(currentLinePoints, currentLineWidths, currentState.depth);
		          }

		          Debug.Log($"Processed axiom. Leaf count = {leafCount}");
	     }

	     /// <summary>
	     /// Process a single stem segment
	     /// </summary>
	     private void ProcessStemSegment(ref TurtleState state,
			       ref List<Vector3> points,
			       ref List<float> widths)
	     {
		          // Calculate new position
		          Vector3 newPosition = state.position + state.direction * (plant.Stems.Length * 0.01f);

		          // Calculate width based on depth and progression
		          float progression = points.Count > 0 ? (float)points.Count / 20f : 0f; // Adjust 20 as needed
		          float newWidth = widthCurve.Evaluate(progression) * Mathf.Pow(0.85f, state.depth);

		          // Add new point
		          points.Add(newPosition);
		          widths.Add(newWidth);

		          // Update state
		          state.position = newPosition;
		          state.currentWidth = newWidth;
	     }

	     /// <summary>
	     /// Create a LineRenderer from collected points
	     /// </summary>
	     private void CreateLineRendererFromPoints(List<Vector3> points, List<float> widths, int depth)
	     {
		          if (points.Count < 2)
				return;

		          // Create branch GameObject
		          GameObject branch = new GameObject($"Branch_Depth{depth}");
		          branch.transform.SetParent(plantContainer.transform);
		          branch.transform.position = Vector3.zero;

		          // Add LineRenderer
		          LineRenderer lr = branch.AddComponent<LineRenderer>();
		          lr.useWorldSpace = true;
		          lr.positionCount = points.Count;
		          lr.SetPositions(points.ToArray());

		          // Configure appearance
		          lr.startWidth = widths[0];
		          lr.endWidth = widths[widths.Count - 1];
		          lr.startColor = plant.Stems.StemColor;
		          lr.endColor = plant.Stems.StemColor;
		          lr.material = new Material(Shader.Find("Sprites/Default"));
		          lr.material.color = plant.Stems.StemColor;
		          lr.numCornerVertices = 3;
		          lr.numCapVertices = 3;

		          // Create width curve
		          AnimationCurve widthCurveLR = new AnimationCurve();
		          for (int i = 0; i < points.Count; i++)
		          {
				float t = i / (float)(points.Count - 1);
				widthCurveLR.AddKey(t, widths[i]);
		          }
		          lr.widthCurve = widthCurveLR;
	     }

	     /// <summary>
	     /// Draw all collected branches
	     /// </summary>
	     private void DrawAllBranches()
	     {
		          // Group branches by depth for better organization
		          var branchesByDepth = allBranches.GroupBy(b => b.depth);

		          foreach (var depthGroup in branchesByDepth)
		          {
				GameObject depthContainer = new GameObject($"Depth_{depthGroup.Key}");
				depthContainer.transform.SetParent(plantContainer.transform);
				depthContainer.transform.position = Vector3.zero;

				foreach (var branch in depthGroup)
				{
					     CreateSingleBranchRenderer(branch, depthContainer);
				}
		          }
	     }

	     /// <summary>
	     /// Create a single branch LineRenderer
	     /// </summary>
	     private void CreateSingleBranchRenderer(BranchData branch, GameObject parent)
	     {
		          GameObject branchObj = new GameObject("Branch");
		          branchObj.transform.SetParent(parent.transform);
		          branchObj.transform.position = Vector3.zero;

		          LineRenderer lr = branchObj.AddComponent<LineRenderer>();
		          lr.useWorldSpace = true;
		          lr.positionCount = 2;
		          lr.SetPosition(0, branch.startPos);
		          lr.SetPosition(1, branch.endPos);
		          lr.startWidth = branch.startWidth;
		          lr.endWidth = branch.endWidth;
		          lr.startColor = branch.color;
		          lr.endColor = branch.color;
		          lr.material = new Material(Shader.Find("Sprites/Default"));
		          lr.material.color = branch.color;
	     }

	     // Leaf creation
	     private void CreateLeaf(Vector3 position, Vector3 direction, float branchWidth)
	     {
		          if (plant?.Leaves == null)
				return;

		          GameObject leaf = new GameObject("Leaf");
		          leaf.transform.SetParent(plantContainer.transform);
		          leaf.transform.position = position;

		          LineRenderer lr = leaf.AddComponent<LineRenderer>();
		          float leafSize = Mathf.Max(0.0005f, branchWidth) * Mathf.Sqrt(plant.Leaves.SurfaceArea) * 2f;

		          Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;

		          Vector3[] leafPoints = new Vector3[3];
		          leafPoints[0] = position + perpendicular * (branchWidth * 0.5f); // offset so base doesn't coincide with stem center
		          leafPoints[1] = position + (direction + perpendicular * 0.3f).normalized * leafSize * 0.5f;
		          leafPoints[2] = position + direction.normalized * leafSize;

		          lr.positionCount = 3;
		          lr.SetPositions(leafPoints);
		          lr.startWidth = leafSize * 0.3f;
		          // lr.alignment = LineAlignment.TransformZ;
		          lr.endWidth = leafSize * 0.1f;
		          lr.widthCurve.AddKey(1, leafSize * 0.5f);
		          lr.startColor = Color.Lerp(plant.Leaves.LeafColor, plant.Stems.StemColor, .3f);
		          lr.endColor = plant.Leaves.LeafColor;
		          lr.material = new Material(Shader.Find("Sprites/Default"));
		          lr.material.color = plant.Leaves.LeafColor;


	     }

	     // Flower creation
	     private void CreateFlower(Vector3 position, Vector3 direction, float branchWidth)
	     {
		          if (plant?.Flowers == null)
				return;

		          GameObject flower = new GameObject("Flower");
		          flower.transform.SetParent(plantContainer.transform);
		          flower.transform.position = position;

		          LineRenderer lr = flower.AddComponent<LineRenderer>();
		          float size = plant.Flowers.Size;

		          // Simple circle for flower
		          int segments = 5;
		          lr.positionCount = segments + 1;
		          lr.loop = true;

		          for (int i = 0; i <= segments; i++)
		          {
				float angle = i * Mathf.PI * 2f / segments;
				lr.SetPosition(i, position + new Vector3(
				    Mathf.Cos(angle) * size,
				    Mathf.Sin(angle) * size,
				    0
				));
		          }

		          lr.startWidth = lr.endWidth = size * 0.5f;
		          lr.startColor = plant.Flowers.FlowerColor;
		          lr.endColor = plant.Flowers.FlowerColor;
		          lr.material = new Material(Shader.Find("Sprites/Default"));
		          lr.material.color = plant.Flowers.FlowerColor;
	     }

	     //  // Fruit creation
	     //  private void CreateFruit(Vector3 position, Vector3 direction, float branchWidth)
	     //  {
	     //        if (plant?.Fruits == null)
	     //return;

	     //        GameObject fruit = new GameObject("Fruit");
	     //        fruit.transform.SetParent(plantContainer.transform);
	     //        fruit.transform.position = position;


	     //        LineRenderer lr = fruit.AddComponent<LineRenderer>();
	     //        float radius = plant.Fruits.Size;

	     //        // Simple circle for fruit
	     //        int segments = 8;
	     //        lr.positionCount = segments + 1;
	     //        lr.loop = true;

	     //        for (int i = 0; i <= segments; i++)
	     //        {
	     //float angle = i * Mathf.PI * 2f / segments;
	     //lr.SetPosition(i, position + new Vector3(
	     //    Mathf.Cos(angle) * radius,
	     //    Mathf.Sin(angle) * radius,
	     //    0
	     //));
	     //        }

	     //        lr.startWidth = lr.endWidth = radius * 0.3f;
	     //        lr.startColor = plant.Fruits.FruitColor;
	     //        lr.endColor = plant.Fruits.FruitColor;
	     //        lr.material = new Material(Shader.Find("Sprites/Default"));
	     //        lr.material.color = plant.Fruits.FruitColor;
	     //  }
	     // Fruit creation with 3D sphere mesh
	     private void CreateFruit(Vector3 position, Vector3 direction, float branchWidth)
	     {
		          if (plant?.Fruits == null)
				return;

		          GameObject fruit = new GameObject("Fruit");
		          fruit.transform.SetParent(plantContainer.transform);
		          fruit.transform.position = position;

		          // Rotate to face the direction of the branch
		          if (direction != Vector3.zero)
				fruit.transform.rotation = Quaternion.LookRotation(direction);

		          // Add mesh components
		          MeshFilter meshFilter = fruit.AddComponent<MeshFilter>();
		          MeshRenderer meshRenderer = fruit.AddComponent<MeshRenderer>();

		          // Create sphere mesh
		          float radius = plant.Fruits.Size;
		          Mesh sphereMesh = CreateSimpleSphereMesh(radius);

		          meshFilter.mesh = sphereMesh;

		          // Set material and color
		          Material fruitMaterial = new Material(Shader.Find("Standard"));
		          fruitMaterial.color = plant.Fruits.FruitColor;
		          meshRenderer.material = fruitMaterial;

		          // Optional: Add collider if needed for interaction
		          SphereCollider collider = fruit.AddComponent<SphereCollider>();
		          collider.radius = radius;
		          collider.isTrigger = true; // Make it a trigger if you want interaction without physics
	     }
	     // Simpler sphere for lower performance cost
	     private Mesh CreateSimpleSphereMesh(float radius, int segments = 8)
	     {
		          Mesh mesh = new Mesh();

		          // Create vertices in a cube and normalize to sphere
		          // This creates a more uniform distribution than UV sphere
		          List<Vector3> vertices = new List<Vector3>();
		          List<int> triangles = new List<int>();

		          // Create an icosphere (approximation)
		          float t = (1f + Mathf.Sqrt(5f)) / 2f;

		          // Icosahedron vertices (12 vertices)
		          vertices.Add(new Vector3(-1, t, 0).normalized * radius);
		          vertices.Add(new Vector3(1, t, 0).normalized * radius);
		          vertices.Add(new Vector3(-1, -t, 0).normalized * radius);
		          vertices.Add(new Vector3(1, -t, 0).normalized * radius);

		          vertices.Add(new Vector3(0, -1, t).normalized * radius);
		          vertices.Add(new Vector3(0, 1, t).normalized * radius);
		          vertices.Add(new Vector3(0, -1, -t).normalized * radius);
		          vertices.Add(new Vector3(0, 1, -t).normalized * radius);

		          vertices.Add(new Vector3(t, 0, -1).normalized * radius);
		          vertices.Add(new Vector3(t, 0, 1).normalized * radius);
		          vertices.Add(new Vector3(-t, 0, -1).normalized * radius);
		          vertices.Add(new Vector3(-t, 0, 1).normalized * radius);

		          // Icosahedron triangles (20 faces)
		          triangles.AddRange(new int[] { 0, 11, 5 });
		          triangles.AddRange(new int[] { 0, 5, 1 });
		          triangles.AddRange(new int[] { 0, 1, 7 });
		          triangles.AddRange(new int[] { 0, 7, 10 });
		          triangles.AddRange(new int[] { 0, 10, 11 });

		          triangles.AddRange(new int[] { 1, 5, 9 });
		          triangles.AddRange(new int[] { 5, 11, 4 });
		          triangles.AddRange(new int[] { 11, 10, 2 });
		          triangles.AddRange(new int[] { 10, 7, 6 });
		          triangles.AddRange(new int[] { 7, 1, 8 });

		          triangles.AddRange(new int[] { 3, 9, 4 });
		          triangles.AddRange(new int[] { 3, 4, 2 });
		          triangles.AddRange(new int[] { 3, 2, 6 });
		          triangles.AddRange(new int[] { 3, 6, 8 });
		          triangles.AddRange(new int[] { 3, 8, 9 });

		          triangles.AddRange(new int[] { 4, 9, 5 });
		          triangles.AddRange(new int[] { 2, 4, 11 });
		          triangles.AddRange(new int[] { 6, 2, 10 });
		          triangles.AddRange(new int[] { 8, 6, 7 });
		          triangles.AddRange(new int[] { 9, 8, 1 });

		          mesh.vertices = vertices.ToArray();
		          mesh.triangles = triangles.ToArray();

		          // Calculate normals (perpendicular to surface)
		          Vector3[] normals = new Vector3[vertices.Count];
		          for (int i = 0; i < vertices.Count; i++)
				normals[i] = vertices[i].normalized;

		          mesh.normals = normals;
		          mesh.RecalculateBounds();

		          return mesh;
	     }

	     /// <summary>
	     /// Generate L-system string (keep your existing method)
	     /// </summary>
	     public string GenerateLSystem(string plantAxiom, int iterations)
	     {
		          if (iterations < 0 || iterations > 10)
		          {
				Debug.LogWarning($"Unusual iteration count: {iterations}. Clamping to 0-10 range.");
				iterations = Mathf.Clamp(iterations, 0, 10);
		          }

		          string currentString = plantAxiom;
		          System.Random random = new System.Random();

		          for (int i = 0; i < iterations; i++)
		          {
				float scale = Mathf.Pow(iterationScaleFactor, i);
				currentString = LSystem_Utilities.ExpandString(currentString, random, scale, plant.Rules.rules);
		          }

		          //Debug.Log($"Generated L-System string ({iterations} iterations): {currentString}");
		          return currentString;
	     }

	     private void OnDestroy()
	     {
		          // Cleanup
		          if (plantContainer != null)
		          {
				Destroy(plantContainer);
		          }
	     }
}