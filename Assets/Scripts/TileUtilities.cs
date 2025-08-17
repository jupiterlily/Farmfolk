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
		          float sandVol = sc.sand / Constants.SAND_DENSITY;
		          float siltVol = sc.silt / Constants.SILT_DENSITY;
		          float clayVol = sc.clay / Constants.CLAY_DENSITY;
		          float totalVol = sandVol + siltVol + clayVol;

		          // Base blended color
		          Color baseColor = (sandColor * sandVol + siltColor * siltVol + clayColor * clayVol) / totalVol;

		          // Organic matter influence - remap 0–10% to 0–0.4 strength roughly
		          float organicFactor = Mathf.Clamp01(sc.organicMatter / 25f);
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

}