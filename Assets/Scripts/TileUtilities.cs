using UnityEngine;
public static class TileUtilities
{

	     public static Color GetSoilColor(SoilComposition sc)
	     {
		          // USDA-like base colors
		          Color sandColor = Constants.SAND_COLOR;
		          Color siltColor = Constants.SILT_COLOR;
		          Color clayColor = Constants.CLAY_COLOR;
		          Color organicColor = Constants.ORGANIC_COLOR;

		          // Volume fractions
		          float sandVol = sc.sand.Mass / Constants.SAND_DENSITY;
		          float siltVol = sc.silt.Mass / Constants.SILT_DENSITY;
		          float clayVol = sc.clay.Mass / Constants.CLAY_DENSITY;
		          float totalVol = sandVol + siltVol + clayVol;

		          // Base blended color
		          Color baseColor = (sandColor * sandVol + siltColor * siltVol + clayColor * clayVol) / totalVol;

		          // Organic matter influence - remap 0–10% to 0–0.4 strength roughly
		          float organicFactor = Mathf.Clamp01(sc.organicMaterial.Mass / 25f);
		          Color withOM = Color.Lerp(baseColor, organicColor, organicFactor);

		          // Moisture-based saturation (already computed)
		          float saturation = sc.saturation;

		          // Darken as wetness increases
		          Color dryColor = AdjustLightness(withOM, 0.9f);
		          Color wetColor = AdjustLightness(withOM, 0.3f);

		          return Color.Lerp(dryColor, wetColor, saturation);
	     }

	     private static Color AdjustLightness(Color color, float targetLightness)
	     {
		          float gray = (color.r + color.g + color.b) / 3f;
		          float factor = targetLightness / Mathf.Max(gray, 0.01f);

		          return new Color(
		              Mathf.Clamp01(color.r * factor),
		              Mathf.Clamp01(color.g * factor),
		              Mathf.Clamp01(color.b * factor)
		          );
	     }


	     public static TileData GetValidNeighbour(TileData data, TileGrid grid)
	     {
		          int xref;
		          int yref;
		          if (data.tileGridRef.x == 0)
		          {
				xref = 0;
		          }
		          else
		          {
				xref = Random.Range(data.tileGridRef.x - 1, data.tileGridRef.x + 1);
		          }
		          if (data.tileGridRef.y == 0)
		          {
				yref = 0;
		          }
		          else
		          {
				yref = Random.Range(data.tileGridRef.y - 1, data.tileGridRef.y + 1);
		          }
		          return grid.Map[xref, yref];

	     }

	     public static float MMToWaterMass(float mm) => mm / 1000f * Constants.WATER_DENSITY;
	     public static float WaterMassToMM(float kgPerM2) => kgPerM2 / Constants.WATER_DENSITY * 1000f;

	     public static AtmosphereTile GetMyAtmosphere(TileData data, TileManager map, int layer)
	     {
		          //get the grid reference of the tile and find the corresponding atmosphere tile in the tile manager. Remember that the atmosphere tiles are stored at a different resolution than the surface tiles		          
		          float resolution = map.tiles[data.tileGridRef.x, data.tileGridRef.y].tileData.tileWidth / map.atmosphereTiles[0, 0, layer].tileWidth;
		          int nx = Mathf.FloorToInt(data.tileGridRef.x / resolution);
		          int ny = Mathf.FloorToInt(data.tileGridRef.y / resolution);

		          return map.atmosphereTiles[nx, ny, layer];
	     }

	     public static float CalculateIrregularPolygonArea(Vector3[] corners)
	     {
		          int n = corners.GetLength(1);
		          float area = 0f;

		          // Using 3D shoelace formula that projects to 2D plane
		          for (int i = 0; i < n; i++)
		          {
				Vector3 current = corners[i];
				Vector3 next = corners[(i + 1) % n];

				// Project onto dominant plane (handles non-planar polygons)
				area += (current.x * next.z - next.x * current.z);
		          }

		          return Mathf.Abs(area) * 0.5f;
	     }

	     // Alternative exact volume calculation using tetrahedral decomposition
	     public static float CalculateExactVolume(Vector3[,] Corners)
	     {
		          int n = Corners.GetLength(1);
		          Vector3 centroidBottom = CalculateCentroid(0, Corners);
		          Vector3 centroidTop = CalculateCentroid(1, Corners);
		          float volume = 0f;

		          // Create tetrahedrons between each pair of edges
		          for (int i = 0; i < n; i++)
		          {
				int j = (i + 1) % n;

				// Bottom tetrahedron
				volume += SignedTetrahedronVolume(
				    centroidBottom,
				    Corners[0, i],
				    Corners[0, j],
				    Corners[1, i]
				);

				// Top tetrahedron
				volume += SignedTetrahedronVolume(
				    centroidTop,
				    Corners[1, i],
				    Corners[1, j],
				    Corners[0, j]
				);
		          }

		          return Mathf.Abs(volume);
	     }

	     public static Vector3 CalculateCentroid(int layer, Vector3[,] Corners)
	     {
		          Vector3 sum = Vector3.zero;
		          int n = Corners.GetLength(1);
		          for (int i = 0; i < n; i++)
		          {
				sum += Corners[layer, i];
		          }
		          return sum / n;
	     }

	     public static float SignedTetrahedronVolume(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	     {
		          return Vector3.Dot((b - a), Vector3.Cross((c - a), (d - a))) / 6f;
	     }

}