using UnityEngine;

public static class EnergyUtilities
{
	     //derive temperature from ThermalEnergy
	     public static float DeriveTemperature(ThermalEnergy thermalEnergy)
	     {
		          if (thermalEnergy.Mass <= 0f || thermalEnergy.SpecificHeat <= 0f)
		          {
				Debug.LogWarning("Mass or SpecificHeat is zero or negative, cannot derive temperature.");
				return 0f; // Avoid division by zero
		          }
		          return thermalEnergy.StoredEnergy / (thermalEnergy.Mass * thermalEnergy.SpecificHeat);
	     }

	     //derive mass from EnergyData
}
