public static class Constants
{
	     // Densities (kg/m³)
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
	     public const float AIR_DENSITY = 1.225f; // kg/m³
}