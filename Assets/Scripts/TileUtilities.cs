using UnityEngine;
public static class TileUtilities
{


	     public static Color GetSoilColor(SoilComposition sc)
	     {
		          // Base color mixing (USDA standards)
		          Color sandColor = new Color(1f, 0.95f, 0.77f); // Tan
		          Color siltColor = new Color(0.69f, 0.64f, 0.65f); // Brown-gray
		          Color clayColor = new Color(0.67f, 0.46f, 0.34f); // Reddish-brown
		          Color organicColor = new Color(0.24f, 0.18f, 0.20f); // Dark brown

		          // Calculate volume fractions for proper mixing
		          float sandVol = sc.sand / Constants.SAND_DENSITY;
		          float siltVol = sc.silt / Constants.SILT_DENSITY;
		          float clayVol = sc.clay / Constants.CLAY_DENSITY;
		          float totalVol = sandVol + siltVol + clayVol;

		          // Weighted blend by volume
		          Color baseColor = (sandColor * sandVol + siltColor * siltVol + clayColor * clayVol) / totalVol;

		          // Add organic matter influence (by mass ratio)
		          Color saturatedColor = Color.Lerp(baseColor, organicColor, sc.organicMatter / Constants.ORGANIC_DENSITY);

		          // Calculate proper saturation (0-1)
		          float maxWaterVolume = sc.porosity * (sandVol + siltVol + clayVol);
		          float currentWaterVolume = sc.water / Constants.WATER_DENSITY;
		          float saturation = Mathf.Clamp01(currentWaterVolume / maxWaterVolume);

		          // Adjust colors based on moisture
		          Color dryColor = AdjustLightness(saturatedColor, 0.8f); // Light when dry
		          Color wetColor = AdjustLightness(saturatedColor, 0.2f); // Dark when wet

		          return Color.Lerp(dryColor, wetColor, saturation);
	     }

	     private static Color AdjustLightness(Color color, float targetLightness)
	     {
		          // Convert to HSL-like space
		          float max = Mathf.Max(color.r, color.g, color.b);
		          float min = Mathf.Min(color.r, color.g, color.b);
		          float lightness = (max + min) / 2f;

		          // Adjust lightness
		          float factor = targetLightness / Mathf.Max(lightness, 0.001f);
		          Color adjusted = new Color(
		              Mathf.Clamp01(color.r * factor),
		              Mathf.Clamp01(color.g * factor),
		              Mathf.Clamp01(color.b * factor)
		          );

		          // Slightly reduce saturation when dry
		          if (targetLightness > 0.5f)
		          {
				float gray = (adjusted.r + adjusted.g + adjusted.b) / 3f;
				adjusted = Color.Lerp(adjusted, Color.white * gray, 0.3f);
		          }

		          return adjusted;
	     }
}