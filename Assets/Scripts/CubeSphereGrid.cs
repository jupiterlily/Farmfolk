using System.Collections.Generic;
using UnityEngine;

public class CubeSphereGrid : MonoBehaviour
{
	     private int gridSize = 20; // Size of the grid (number of divisions per face)
	     private float radius = 30; // Radius of the sphere

	     private List<Vector3[]> vertices; // Vertices of the grid
	     private List<Color[]> colors; // Colors assigned to vertices
	     private List<int[]> triangles; // Triangles to form mesh

	     public Color baseColor = Color.blue; // Base color of the celestial body

	     private MeshFilter meshFilter;
	     private MeshRenderer meshRenderer;
	     private Mesh mesh;

	     void Start()
	     {
		          InitializeGrid();
		          CreateMeshObject();
		          UpdateMesh();
	     }

	     private void InitializeGrid()
	     {
		          GenerateVertices();
		          GenerateTriangles();
		          //  InitializeColors();
	     }

	     private void CreateMeshObject()
	     {
		          meshFilter = GetComponent<MeshFilter>();
		          if (meshFilter == null)
				meshFilter = gameObject.AddComponent<MeshFilter>();

		          meshRenderer = GetComponent<MeshRenderer>();
		          if (meshRenderer == null)
				meshRenderer = gameObject.AddComponent<MeshRenderer>();

		          meshRenderer.material = new Material(Shader.Find("Standard"));
		          meshRenderer.material.color = Constants.DEEP_WATER_COLOR;
		          mesh = new Mesh();
		          mesh.name = "CubeSphereMesh";
	     }

	     // Draw the vertices, colors and triangles to a mesh
	     public void UpdateMesh()
	     {
		          if (mesh == null)
				return;

		          mesh.Clear();

		          List<Vector3> allVertices = new List<Vector3>();
		          List<Color> allColors = new List<Color>();
		          List<int> allTriangles = new List<int>();

		          for (int faceIndex = 0; faceIndex < 6; faceIndex++)
		          {
				int vertexOffset = allVertices.Count;
				allVertices.AddRange(vertices[faceIndex]);
				allColors.AddRange(colors[faceIndex]);

				foreach (int triangleIndex in triangles[faceIndex])
				{
					     allTriangles.Add(triangleIndex + vertexOffset);
				}
		          }

		          mesh.vertices = allVertices.ToArray();
		          mesh.colors = allColors.ToArray();
		          mesh.triangles = allTriangles.ToArray();

		          mesh.RecalculateNormals();
		          mesh.RecalculateBounds();
		          mesh.RecalculateTangents();

		          meshFilter.mesh = mesh;
	     }

	     private void GenerateVertices()
	     {
		          vertices = new List<Vector3[]>(6);

		          for (int faceIndex = 0; faceIndex < 6; faceIndex++)
		          {
				Vector3[] faceVertices = new Vector3[(gridSize + 1) * (gridSize + 1)];

				// Calculate step size for UV coordinates
				float step = 2f / gridSize;

				for (int y = 0; y <= gridSize; y++)
				{
					     for (int x = 0; x <= gridSize; x++)
					     {
						          // UV coordinates from -1 to 1
						          float u = -1f + x * step;
						          float v = -1f + y * step;

						          // Get the direction vector based on face index
						          Vector3 direction = GetFaceDirection(u, v, faceIndex);

						          // **FIXED: Use cube to sphere projection formula**
						          // This creates a better sphere with less distortion
						          Vector3 pointOnSphere = CubeToSphere(direction).normalized * radius;

						          int vertexIndex = y * (gridSize + 1) + x;
						          faceVertices[vertexIndex] = pointOnSphere;
					     }
				}

				vertices.Add(faceVertices);
		          }
	     }

	     // Improved cube to sphere projection
	     private Vector3 CubeToSphere(Vector3 p)
	     {
		          float x = p.x;
		          float y = p.y;
		          float z = p.z;

		          // Avoid division by zero
		          float epsilon = 0.0001f;

		          float x2 = x * x;
		          float y2 = y * y;
		          float z2 = z * z;

		          Vector3 result = new Vector3(
		              x * Mathf.Sqrt(1f - (y2 + z2) / 2f + (y2 * z2) / 3f),
		              y * Mathf.Sqrt(1f - (z2 + x2) / 2f + (z2 * x2) / 3f),
		              z * Mathf.Sqrt(1f - (x2 + y2) / 2f + (x2 * y2) / 3f)
		          );

		          return result;
	     }

	     private Vector3 GetFaceDirection(float u, float v, int faceIndex)
	     {
		          switch (faceIndex)
		          {
		          case 0:
		          return new Vector3(1, v, u);  // +X face
		          case 1:
		          return new Vector3(-1, v, -u);  // -X face
		          case 2:
		          return new Vector3(u, 1, v);   // +Y face
		          case 3:
		          return new Vector3(u, -1, -v); // -Y face 
		          case 4:
		          return new Vector3(-u, v, 1);   // +Z face
		          case 5:
		          return new Vector3(u, v, -1); // -Z face 
		          default:
		          return Vector3.zero;
		          }
	     }

	     private void GenerateTriangles()
	     {
		          triangles = new List<int[]>(6);

		          for (int faceIndex = 0; faceIndex < 6; faceIndex++)
		          {
				int[] faceTriangles = new int[gridSize * gridSize * 6];
				int triangleIndex = 0;

				for (int y = 0; y < gridSize; y++)
				{
					     for (int x = 0; x < gridSize; x++)
					     {
						          int topLeft = y * (gridSize + 1) + x;
						          int topRight = topLeft + 1;
						          int bottomLeft = (y + 1) * (gridSize + 1) + x;
						          int bottomRight = bottomLeft + 1;

						          // Ensure proper winding order for normals
						          // First triangle (clockwise)
						          faceTriangles[triangleIndex++] = topLeft;
						          faceTriangles[triangleIndex++] = bottomLeft;
						          faceTriangles[triangleIndex++] = topRight;

						          // Second triangle (clockwise)
						          faceTriangles[triangleIndex++] = topRight;
						          faceTriangles[triangleIndex++] = bottomLeft;
						          faceTriangles[triangleIndex++] = bottomRight;
					     }
				}

				triangles.Add(faceTriangles);
		          }
	     }

	     private Color AdjustHue(Color color, float hueOffset)
	     {
		          Color.RGBToHSV(color, out float h, out float s, out float v);
		          h = (h + hueOffset) % 1.0f;
		          if (h < 0)
				h += 1.0f;
		          return Color.HSVToRGB(h, s, v);
	     }

	     // Debug method to visualize vertices
	     void OnDrawGizmosSelected()
	     {
		          if (vertices == null || vertices.Count == 0)
				return;

		          Gizmos.color = Color.red;
		          for (int faceIndex = 0; faceIndex < vertices.Count; faceIndex++)
		          {
				Vector3[] faceVerts = vertices[faceIndex];
				for (int i = 0; i < faceVerts.Length; i++)
				{
					     Gizmos.DrawSphere(transform.TransformPoint(faceVerts[i]), 0.1f);
				}
		          }
	     }

	     public void AddLayerAbove()
	     {
		          if (gridSize < 100) // Add upper limit to prevent performance issues
		          {
				gridSize++;
				InitializeGrid();
				UpdateMesh();
		          }
	     }

	     public void AddLayerBelow()
	     {
		          if (gridSize > 3) // Minimum grid size to maintain shape
		          {
				gridSize--;
				InitializeGrid();
				UpdateMesh();
		          }
	     }

	     public void SetColorAtLatitudeLongitude(float latitude, float longitude, Color color)
	     {
		          // Convert latitude and longitude to radians
		          float latRad = Mathf.Deg2Rad * latitude;
		          float lonRad = Mathf.Deg2Rad * longitude;

		          // Convert to Cartesian coordinates
		          float cosLat = Mathf.Cos(latRad);
		          Vector3 direction = new Vector3(
		              cosLat * Mathf.Cos(lonRad),
		              Mathf.Sin(latRad),
		              cosLat * Mathf.Sin(lonRad)
		          );

		          // Find which face this point belongs to
		          int faceIndex = GetFaceFromDirection(direction);

		          // Get UV coordinates on that face
		          Vector2 uv = GetUVFromDirection(direction, faceIndex);

		          // Convert UV to grid coordinates
		          int x = Mathf.Clamp(Mathf.RoundToInt((uv.x + 1f) * 0.5f * gridSize), 0, gridSize);
		          int y = Mathf.Clamp(Mathf.RoundToInt((uv.y + 1f) * 0.5f * gridSize), 0, gridSize);

		          // Set color
		          int index = y * (gridSize + 1) + x;
		          colors[faceIndex][index] = color;

		          UpdateMesh();
	     }

	     public Color GetColorAtLatitudeLongitude(float latitude, float longitude)
	     {
		          // Convert latitude and longitude to radians
		          float latRad = Mathf.Deg2Rad * latitude;
		          float lonRad = Mathf.Deg2Rad * longitude;

		          // Convert to Cartesian coordinates
		          float cosLat = Mathf.Cos(latRad);
		          Vector3 direction = new Vector3(
		              cosLat * Mathf.Cos(lonRad),
		              Mathf.Sin(latRad),
		              cosLat * Mathf.Sin(lonRad)
		          );

		          // Find which face this point belongs to
		          int faceIndex = GetFaceFromDirection(direction);

		          // Get UV coordinates on that face
		          Vector2 uv = GetUVFromDirection(direction, faceIndex);

		          // Convert UV to grid coordinates
		          int x = Mathf.Clamp(Mathf.RoundToInt((uv.x + 1f) * 0.5f * gridSize), 0, gridSize);
		          int y = Mathf.Clamp(Mathf.RoundToInt((uv.y + 1f) * 0.5f * gridSize), 0, gridSize);

		          // Get color
		          int index = y * (gridSize + 1) + x;
		          return colors[faceIndex][index];
	     }

	     private int GetFaceFromDirection(Vector3 direction)
	     {
		          float absX = Mathf.Abs(direction.x);
		          float absY = Mathf.Abs(direction.y);
		          float absZ = Mathf.Abs(direction.z);

		          if (absX >= absY && absX >= absZ)
				return direction.x > 0 ? 0 : 1;
		          else if (absY >= absX && absY >= absZ)
				return direction.y > 0 ? 2 : 3;
		          else
				return direction.z > 0 ? 4 : 5;
	     }

	     private Vector2 GetUVFromDirection(Vector3 direction, int faceIndex)
	     {
		          float u = 0f, v = 0f;

		          switch (faceIndex)
		          {
		          case 0: // +X
		          u = direction.z / direction.x;
		          v = direction.y / direction.x;
		          break;
		          case 1: // -X
		          u = -direction.z / direction.x;
		          v = direction.y / direction.x;
		          break;
		          case 2: // +Y
		          u = direction.x / direction.y;
		          v = direction.z / direction.y;
		          break;
		          case 3: // -Y
		          u = direction.x / direction.y;
		          v = -direction.z / direction.y;
		          break;
		          case 4: // +Z
		          u = direction.x / direction.z;
		          v = direction.y / direction.z;
		          break;
		          case 5: // -Z
		          u = -direction.x / direction.z;
		          v = direction.y / direction.z;
		          break;
		          }

		          return new Vector2(u, v);
	     }
}