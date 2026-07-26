using UnityEngine;

public static class Constants
{
	     // Densities (kg/m3)
	     public const float SAND_DENSITY = 1550f;
	     public const float SILT_DENSITY = 2100f;
	     public const float CLAY_DENSITY = 2650f;
	     public const float ORGANIC_DENSITY = 250f;
	     public const float WATER_DENSITY = 1000f;
	     public const float ICE_DENSITY = 916.2F;
	     public const float GRANITE_DENSITY = 2600f;



	     public const float GLENN_FLOW_LAW_EXPONENT = 3f; // n in Glen's flow law
	     public const float MIN_ICE_THICKNESS_FOR_FLOW = 0.5f; // meters
	     public const float WATER_FLOW_RESISTANCE = 5f;   // m/s per unit slope
	     public const float ICE_FLOW_RESISTANCE = 0.005f;   // m/s per unit slope (slower)
	     public const float WIND_DRAG_COEFFICIENT = 0.03f;       // wind-to-current conversion	         

	     public const float SEDIMENT_ENTRAINMENT_COEFFICIENT = 0.001f;  // fraction of sediment eroded per unit flow
	     public const float SEDIMENT_DEPOSITION_COEFFICIENT = 0.05f; // fraction of sediment deposited per unit flow
	     public const float ICE_FLOATING_THRESHOLD = 0.9f; // ice floats if water depth > ice thickness * (ice_density/water_density) * threshold// Energy (Joules per kg)
	     public const float GLACIAL_EROSION_COEFFICIENT = 0.001f;


	     public const float AIR_DENSITY = 1.225f; // kg/m³
				 // Air heat capacity (used if transferring heat to atmosphere)

	     // Use a constant literal value for Stefan-Boltzmann constant
	     public const float STEFAN_BOLTZMANN = 0.00000005670374419f; // W/m2K4
	     public const float ATMOSPHERIC_EMISSIVITY = 0.8f; // Typical value for Earth's atmosphere
	     public const float COSMIC_BACKGROUND = 2.7f; // Cosmic microwave background (K)
				     //colors
	     public static Color SAND_COLOR = new Color(207f / 255f, 157f / 255f, 123f / 255f);
	     public static Color SILT_COLOR = new Color(117f / 255f, 153f / 255f, 139f / 255f);
	     public static Color CLAY_COLOR = new Color(164f / 255f, 111f / 255f, 94f / 255f);
	     public static Color ORGANIC_COLOR = new Color(58f / 255f, 53f / 255f, 52f / 255f);
	     public static Color SHALLOW_WATER_COLOR = new Color(62f / 255f, 107f / 255f, 117f / 255f);
	     public static Color DEEP_WATER_COLOR = new Color(1f / 255f, 36f / 255f, 45f / 255f);
	     public static Color ICE_COLOR = new Color(210f / 255f, 226f / 255f, 236f / 255f);
	     public static Color AIR_COLOR = new Color(0.8f, 0.9f, 1.0f);


	     // Physical constants
	     public const float GAS_CONSTANT = 8.314f;         // J/(mol·K)
	     public const float MOLAR_MASS_AIR = 0.02896f;     // kg/mol
	     public const float MOLAR_MASS_WATER = 0.01802f;   // kg/mol
	     public const float CORIOLIS_PARAM = 1e-4f;
	     public const float DRY_LAPSE_RATE = -0.0098f; // °C/m

	     //hair

}