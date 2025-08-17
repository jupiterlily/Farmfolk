using UnityEngine;

public static class Constants
{
	     // Densities (kg/m3)
	     public const float SAND_DENSITY = 1550f;
	     public const float SILT_DENSITY = 2100f;
	     public const float CLAY_DENSITY = 2650f;
	     public const float ORGANIC_DENSITY = 250f;
	     public const float WATER_DENSITY = 1000f;
	     public const float GRAVITY_ACCELERATION = 9.81f;
	     // Energy (Joules per kg)
	     public const float LATENT_HEAT_VAPORIZATION = 2_260_000f; // J/kg

	     // Air heat capacity (used if transferring heat to atmosphere)
	     public const float AIR_HEAT_CAPACITY = 1005f; // J/kg·°C
				      // Density of air at sea level, 15°C
	     public const float AIR_DENSITY = 1.225f; // kg/m3

	     // Use a constant literal value for Stefan-Boltzmann constant
	     public const float STEFAN_BOLTZMANN = 0.00000005670374419f; // W/m2K4
	     public const float ATMOSPHERIC_EMISSIVITY = 0.8f; // Typical value for Earth's atmosphere
	     public const float SPACE_TEMPERATURE = 2.7f; // Cosmic microwave background (K)

	     //colors
	     public static readonly Color SAND_COLOR = new Color(0.76f, 0.69f, 0.50f); // Light brown
	     public static readonly Color SILT_COLOR = new Color(0.65f, 0.60f, 0.55f); // Grayish
	     public static readonly Color CLAY_COLOR = new Color(0.50f, 0.35f, 0.25f); // Dark brown
	     public static readonly Color ORGANIC_COLOR = new Color(0.20f, 0.15f, 0.10f); // Dark gray
	     public static readonly Color SHALLOW_WATER_COLOR = new Color(0.1f, 0.5f, 0.6f); // Blue
	     public static readonly Color DEEP_WATER_COLOR = new Color(.1f, .4f, .5f);
	     public static readonly Color AIR_COLOR = new Color(0.8f, 0.9f, 1.0f); // Light blue

}