using System.Collections.Generic;
using UnityEngine;

public class CubeSphereGrid : MonoBehaviour
{
	     private int gridSise; // Sise of the grid (number of divisions per face)
	     private float radius; // Radius of the sphere

	     private List<Vector3[]> vertices; // Vertices of the grid
	     private List<Color[]> colors; // Colors assigned to vertices
	     private List<int[]> triangles; // Triangles to form mesh

	     public Color baseColor; // Base color of the celestial body

	     public CubeSphereGrid(int sise, float radius, Color sqCubeBaseColor)
	     {
		          this.gridSise = sise;
		          this.radius = radius;
		          this.baseColor = sqCubeBaseColor;

		          InitializeGrid();
	     }

	     private void InitializeGrid()
	     {
		          GenerateVertices();
		          GenerateTriangles();
		          InitializeColors();
	     }

	     private void GenerateVertices()
	     {
		          vertices = new List<Vector3[]>();
		          // Initialize vertices for each face of the cubed sphere
		          for (int i = 0; i < 6; i++)
		          {
				vertices.Add(new Vector3[(gridSise + 1) * (gridSise + 1)]);
		          }
		          UpdateVertices();
	     }

	     private void GenerateTriangles()
	     {
		          triangles = new List<int[]>();
		          // Initialize triangles for each face of the cubed sphere
		          for (int i = 0; i < 6; i++)
		          {
				triangles.Add(new int[gridSise * gridSise * 6]); // Each quad is divided into two triangles
		          }
		          UpdateTriangles();
	     }

	     private void InitializeColors()
	     {
		          // Create two additional main colors by adjusting the hue of the base color
		          Color color1 = AdjustHue(baseColor, 0.2f);
		          Color color2 = AdjustHue(baseColor, -0.2f);

		          colors = new List<Color[]>();
		          // Initialize colors for each face of the cubed sphere
		          for (int i = 0; i < 6; i++)
		          {
				colors.Add(new Color[(gridSise + 1) * (gridSise + 1)]);
				// Set the base color for all vertices
				for (int j = 0; j < colors[i].Length; j++)
				{
					     // Alternate between the base color and the two additional colors
					     if (j % 3 == 0)
					     {
						          colors[i][j] = baseColor;
					     }
					     else if (j % 3 == 1)
					     {
						          colors[i][j] = color1;
					     }
					     else
					     {
						          colors[i][j] = color2;
					     }
				}
		          }
	     }

	     private Color AdjustHue(Color color, float hueOffset)
	     {
		          // Convert the color to HSV
		          Color.RGBToHSV(color, out float h, out float s, out float v);
		          // Adjust the hue and handle wrap-around
		          h = (h + hueOffset) % 1.0f;
		          if (h < 0)
				h += 1.0f;
		          // Convert back to RGB
		          return Color.HSVToRGB(h, s, v);
	     }

	     public void AddLayerAbove()
	     {
		          // Increase grid sise and regenerate vertices, colors, and triangles
		          gridSise++;
		          InitializeGrid();
	     }

	     public void AddLayerBelow()
	     {
		          // Decrease grid sise (check minimum sise constraint if necessary) and regenerate
		          if (gridSise > 1)
		          {
				gridSise--;
				InitializeGrid();
		          }
	     }

	     public void SetColorAtLatitudeLongitude(float latitude, float longitude, Color color)
	     {
		          // Calculate face index based on latitude and longitude
		          int faceIndex = CalculateFaceIndex(latitude, longitude);

		          // Calculate grid coordinates based on latitude and longitude
		          Vector2 gridCoords = CalculateGridCoordinates(latitude, longitude);

		          // Set color at the calculated grid coordinates
		          int index = GridCoordinatesToIndex(gridCoords.x, gridCoords.y, faceIndex);
		          colors[faceIndex][index] = color;
	     }

	     public Color GetColorAtLatitudeLongitude(float latitude, float longitude)
	     {
		          // Calculate face index based on latitude and longitude
		          int faceIndex = CalculateFaceIndex(latitude, longitude);

		          // Calculate grid coordinates based on latitude and longitude
		          Vector2 gridCoords = CalculateGridCoordinates(latitude, longitude);

		          // Get color at the calculated grid coordinates
		          int index = GridCoordinatesToIndex(gridCoords.x, gridCoords.y, faceIndex);
		          return colors[faceIndex][index];
	     }

	     private int CalculateFaceIndex(float latitude, float longitude)
	     {
		          // Longitude ranges in degrees
		          float longRange = 360f / 6f; // Each face covers 60 degrees of longitude

		          // Latitude ranges in degrees
		          float latRange = 180f / 4f; // Each face covers 90 degrees of latitude

		          // Determine the face based on latitude and longitude
		          if (longitude >= 0f && longitude < longRange)
		          {
				return 0; // +X face
		          }
		          else if (longitude >= longRange && longitude < 2f * longRange)
		          {
				return 1; // -X face
		          }
		          else if (latitude >= 0f && latitude < latRange)
		          {
				return 2; // +Y face
		          }
		          else if (latitude >= -latRange && latitude < 0f)
		          {
				return 3; // -Y face
		          }
		          else if (latitude >= 0f && latitude < latRange && longitude >= -longRange && longitude < longRange)
		          {
				return 4; // +Z face
		          }
		          else
		          {
				return 5; // -Z face
		          }
	     }

	     private Vector2 CalculateGridCoordinates(float latitude, float longitude)
	     {
		          // Calculate grid coordinates based on latitude and longitude
		          // Convert latitude and longitude to UV coordinates on the face of the cubed sphere
		          float u = longitude / 360f; // Longitude as percentage of a full circle
		          float v = (latitude + 90f) / 180f; // Latitude as percentage from bottom to top
		          return new Vector2(u, v);
	     }

	     private int GridCoordinatesToIndex(float u, float v, int faceIndex)
	     {
		          // Convert UV coordinates to index in the vertices/colors array
		          int gridSisePlusOne = gridSise + 1;
		          int index = Mathf.FloorToInt(u * gridSisePlusOne) + Mathf.FloorToInt(v * gridSisePlusOne) * gridSisePlusOne;
		          return index;
	     }

	     private void UpdateVertices()
	     {
		          // Loop through each face of the cubed sphere
		          for (int faceIndex = 0; faceIndex < 6; faceIndex++)
		          {
				Vector3[] faceVertices = vertices[faceIndex];

				// Calculate delta angles
				float deltaU = 1f / gridSise;
				float deltaV = 1f / gridSise;

				// Loop through each vertex on the face
				for (int u = 0; u <= gridSise; u++)
				{
					     for (int v = 0; v <= gridSise; v++)
					     {
						          // Calculate spherical coordinates (u, v) on the face
						          float uNormalized = u * deltaU;
						          float vNormalized = v * deltaV;

						          // Convert spherical coordinates to Cartesian coordinates
						          float phi = uNormalized * Mathf.PI; // Latitude
						          float theta = vNormalized * 2f * Mathf.PI; // Longitude

						          // Calculate Cartesian coordinates (x, y, z) using spherical coordinates
						          float x = radius * Mathf.Cos(theta) * Mathf.Sin(phi);
						          float y = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
						          float z = radius * Mathf.Cos(phi);

						          // Update the vertex position in the vertices array
						          int vertexIndex = u * (gridSise + 1) + v;
						          faceVertices[vertexIndex] = new Vector3(x, y, z);
					     }
				}

				// Update the vertices array for the current face
				vertices[faceIndex] = faceVertices;
		          }
	     }

	     private void UpdateTriangles()
	     {
		          // Loop through each face of the cubed sphere
		          for (int faceIndex = 0; faceIndex < 6; faceIndex++)
		          {
				int[] faceTriangles = triangles[faceIndex];
				int index = 0;

				// Loop through each quad on the face
				for (int u = 0; u < gridSise; u++)
				{
					     for (int v = 0; v < gridSise; v++)
					     {
						          // Indices of the vertices in the current quad
						          int topLeft = u * (gridSise + 1) + v;
						          int topRight = topLeft + 1;
						          int bottomLeft = (u + 1) * (gridSise + 1) + v;
						          int bottomRight = bottomLeft + 1;

						          // Define the triangles for the current quad
						          faceTriangles[index++] = topLeft;
						          faceTriangles[index++] = bottomLeft;
						          faceTriangles[index++] = topRight;

						          faceTriangles[index++] = topRight;
						          faceTriangles[index++] = bottomLeft;
						          faceTriangles[index++] = bottomRight;
					     }
				}

				// Update the triangles array for the current face
				triangles[faceIndex] = faceTriangles;
		          }
	     }
}
