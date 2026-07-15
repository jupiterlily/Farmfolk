using System.Collections.Generic;
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
		          Color iceColor = Constants.ICE_COLOR;
		          // Volume fractions
		          float sandVol = sc.sand.Mass / Constants.SAND_DENSITY;
		          float siltVol = sc.silt.Mass / Constants.SILT_DENSITY;
		          float clayVol = sc.clay.Mass / Constants.CLAY_DENSITY;
		          float orgVol = sc.organicMaterial.Mass / Constants.ORGANIC_DENSITY;
		          //  float iceVol = sc.ice.Mass / Constants.ICE_DENSITY;
		          float totalVol = sandVol + siltVol + clayVol + orgVol;

		          // Base blended color
		          Color baseColor = (sandColor * sandVol + siltColor * siltVol + clayColor * clayVol) / totalVol;


		          Color withOM = Color.Lerp(baseColor, organicColor, orgVol);

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
				xref = UnityEngine.Random.Range(data.tileGridRef.x - 1, data.tileGridRef.x + 1);
		          }
		          if (data.tileGridRef.y == 0)
		          {
				yref = 0;
		          }
		          else
		          {
				yref = UnityEngine.Random.Range(data.tileGridRef.y - 1, data.tileGridRef.y + 1);
		          }
		          return grid.Map[xref, yref];

	     }
	     public static float CalculateSoilVolume(SoilComposition sc)
	     {
		          // Volume of each solid component (m3/m3)
		          float sandVol = sc.sand.Mass / Constants.SAND_DENSITY;
		          float siltVol = sc.silt.Mass / Constants.SILT_DENSITY;
		          float clayVol = sc.clay.Mass / Constants.CLAY_DENSITY;
		          float organicVol = sc.organicMaterial.Mass / Constants.ORGANIC_DENSITY;

		          float solidVolume = sandVol + siltVol + clayVol + organicVol;
		          float waterVolume = sc.water.Mass / Constants.WATER_DENSITY; // Convert kg/m2 back to m3/m3 for volume calculation
		          float iceVolume = sc.ice.Mass / Constants.ICE_DENSITY;

		          float airVolume = sc.aeration; // already calculated from porosity and saturation

		          float total = solidVolume + waterVolume + iceVolume + airVolume;

		          return (total);
	     }
	     public static float CalculateSurfaceWaterVolume(TileData td)
	     {
		          // Volume of each solid component (m3/m3)
		          float waterVolume = td.surfaceLiquid.Mass / Constants.WATER_DENSITY; // Convert kg/m2 back to m3/m3 for volume calculation
		          float iceVolume = td.surfaceIce.Mass / Constants.ICE_DENSITY;

		          float total = waterVolume + iceVolume;

		          return (total);
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

	     //take a tile, a string and a float and add that to the composition of the tile.

	     public static void AddMassToTileComponent(TileData tile, string mat, float kg, float temp)
	     {
		          if (tile == null)
				return;
		          if (mat == null)
				return;

		          float newMass = kg;
		          if (mat == "sand" || mat == "Sand")
		          {
				newMass += tile.composition.energy[0].Mass;
				tile.composition.energy[0].AddMass(kg, temp);
		          }
		          if (mat == "sand" || mat == "Sand")
		          {
				newMass += tile.composition.energy[1].Mass;
				tile.composition.energy[1].AddMass(kg, temp);
		          }
		          if (mat == "sand" || mat == "Sand")
		          {
				newMass += tile.composition.energy[2].Mass;
				tile.composition.energy[2].AddMass(kg, temp);
		          }
		          if (mat == "sand" || mat == "Sand")
		          {
				newMass += tile.composition.energy[3].Mass;
				tile.composition.energy[3].AddMass(kg, temp);
		          }
		          if (mat == "sand" || mat == "Sand")
		          {
				newMass += tile.composition.energy[4].Mass;
				tile.composition.energy[4].AddMass(kg, temp);
		          }

	     }
	     public static void GetDownhillWaterNeighbors(WorldTile tile, List<(WorldTile, float)> result)
	     {
		          result.Clear();
		          TileData data = tile.tileData;
		          float area = data.tileWidth * data.tileWidth;
		          float waterDepth = data.surfaceLiquid.Mass / (Constants.WATER_DENSITY * area);
		          float waterSurface = data.groundElevation + waterDepth;

		          foreach (var neighbor in tile.GetNeighbors())
		          {
				if (neighbor == null)
					     continue;
				TileData nd = neighbor.tileData;
				float nWaterDepth = nd.surfaceLiquid.Mass / (Constants.WATER_DENSITY * area);
				float nWaterSurface = nd.groundElevation + nWaterDepth;
				float diff = waterSurface - nWaterSurface;
				if (diff > 0f)
				{
					     float slope = diff / data.tileWidth; // now dimensionless
					     result.Add((neighbor, slope));
				}
		          }
	     }

	     public static void GetDownhillIceNeighbors(WorldTile tile, List<(WorldTile, float)> result)
	     {
		          result.Clear();
		          TileData data = tile.tileData;
		          float area = data.tileWidth * data.tileWidth;

		          // Current tile's ice surface
		          float waterDepth = data.surfaceLiquid.Mass / (Constants.WATER_DENSITY * area);
		          float iceDepth = data.surfaceIce.Mass / (Constants.ICE_DENSITY * area);
		          float iceSurface;
		          if (CryosphereUtilities.IsIceFloating(data))   // you'll need to call your floating check here
				iceSurface = data.groundElevation + waterDepth + iceDepth;
		          else
				iceSurface = data.groundElevation + iceDepth;

		          foreach (var neighbor in tile.GetNeighbors())
		          {
				if (neighbor == null)
					     continue;
				TileData nd = neighbor.tileData;

				float nWaterDepth = nd.surfaceLiquid.Mass / (Constants.WATER_DENSITY * area);
				float nIceDepth = nd.surfaceIce.Mass / (Constants.ICE_DENSITY * area);
				float nIceSurface;
				if (CryosphereUtilities.IsIceFloating(nd))
					     nIceSurface = nd.groundElevation + nWaterDepth + nIceDepth;
				else
					     nIceSurface = nd.groundElevation + nIceDepth;

				float diff = iceSurface - nIceSurface;
				if (diff > 0f)
				{
					     float slope = diff / data.tileWidth;
					     result.Add((neighbor, slope));
				}
		          }
	     }
	     /// <summary>
	     /// Returns the estimated air temperature (in Kelvin) at a given latitude and day of the year.
	     /// Uses a simple annual sinusoidal model with latitudinal gradient.
	     /// </summary>
	     /// <param name="latitude">Degrees from -90 (South Pole) to +90 (North Pole).</param>
	     /// <param name="dayOfYear">Day number from 1 to 365 (or fractional).</param>
	     /// <returns>Temperature in Kelvin.</returns>
	     public static float GetTemperatureFromLatitude(float latitude, float dayOfYear)
	     {
		          // Clamp latitude
		          latitude = Mathf.Clamp(latitude, -90f, 90f);
		          // Fraction of the year (0..1)
		          // float yearFraction = dayOfYear / SimulationConstants.YearLength;

		          // --- 1. Base annual mean temperature (latitudinal gradient) ---
		          // 30°C at equator, -20°C at poles → linear drop of 70°C over 90°
		          float meanTempC = 30f - 50f * (Mathf.Abs(latitude) / 90f);

		          // --- 2. Seasonal amplitude ---
		          // Varies from 0 at equator to ±15°C at the poles (adjustable)
		          float amplitude = 15f * Mathf.Sin(Mathf.Abs(latitude) * Mathf.Deg2Rad);

		          // --- 3. Phase shift (summer peak) ---
		          // Northern hemisphere: summer solstice around day 172 (June 21)
		          // Southern hemisphere: shifted by half a year
		          float summerPeakDay = (latitude >= 0) ? 172f : 172f + 182.625f; // ~6 months offset

		          // --- 4. Seasonal anomaly ---
		          // Sinusoidal cycle: maximum at summer, minimum at winter
		          float seasonal = amplitude * Mathf.Sin(2f * Mathf.PI * (dayOfYear - summerPeakDay) / 365.25f);

		          // --- 5. Final temperature in Celsius, then convert to Kelvin ---
		          float tempC = meanTempC + seasonal;
		          return tempC + SimulationConstants.Kelvin;
	     }
}