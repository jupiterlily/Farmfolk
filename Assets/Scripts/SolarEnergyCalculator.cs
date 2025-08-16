using UnityEngine;

// This class is responsible for simulating solar energy calculations based on time and location on Earth.
public static class SolarEnergyCalculator
{
	     // This method calculates the current solar energy at a specific latitude, day of the year, and time of day.
	     public static float CalculateCurrentEnergyAtLatitude(float latitude, float dayOfYear, float timeOfDay)
	     {
		          // Calculate the solar declination angle based on the day of the year.
		          // The declination angle is the tilt of the Earth's axis relative to its orbit around the Sun.
		          float declination = SimulationConstants.AxialTilt * Mathf.Sin(2 * Mathf.PI * (dayOfYear / SimulationConstants.YearLength));

		          // Calculate the hour angle based on the time of day.
		          // The hour angle measures the time since solar noon, expressed in degrees (0° at solar noon, ±180° at midnight).
		          float hourAngle = 360f * timeOfDay - 180f;

		          // Calculate the zenith angle based on latitude, declination, and hour angle.
		          // The zenith angle is the angle between the zenith (directly overhead) and the center of the Sun's disc.
		          float zenithAngle = Mathf.Acos(Mathf.Sin(latitude * Mathf.Deg2Rad) * Mathf.Sin(declination * Mathf.Deg2Rad) +
		              Mathf.Cos(latitude * Mathf.Deg2Rad) * Mathf.Cos(declination * Mathf.Deg2Rad) * Mathf.Cos(hourAngle * Mathf.Deg2Rad)) * Mathf.Rad2Deg;

		          // Calculate the solar radiation based on the zenith angle.
		          // Solar radiation is the power per unit area received from the Sun in the form of electromagnetic radiation.
		          float solarRadiation = SimulationConstants.SolarConstant * Mathf.Cos(zenithAngle * Mathf.Deg2Rad);

		          // Return the calculated solar radiation, ensuring it is not negative.
		          return Mathf.Max(solarRadiation, 0f);
	     }

	     //angle that light woulld hit the object at this latitude
	     public static Vector3 CalculateLightAngle(float latitude, float dayOfYear, float timeOfDay)
	     {

		          // Calculate the solar declination angle based on the day of the year.
		          float declination = SimulationConstants.AxialTilt * Mathf.Sin(2 * Mathf.PI * (dayOfYear / SimulationConstants.YearLength));

		          // Calculate the hour angle based on the time of day.
		          float hourAngle = 360f * timeOfDay - 180f;

		          // Calculate the zenith angle based on latitude, declination, and hour angle.
		          float zenithAngle = Mathf.Acos(Mathf.Sin(latitude * Mathf.Deg2Rad) * Mathf.Sin(declination * Mathf.Deg2Rad) +
									Mathf.Cos(latitude * Mathf.Deg2Rad) * Mathf.Cos(declination * Mathf.Deg2Rad) * Mathf.Cos(hourAngle * Mathf.Deg2Rad)) * Mathf.Rad2Deg;

		          // Calculate the light direction vector based on the zenith angle and latitude.
		          Vector3 lightDirection = new Vector3(Mathf.Cos(zenithAngle), Mathf.Sin(zenithAngle), 0);
		          return lightDirection;
	     }


	     // This method returns a descriptive string for a given latitude, providing context about its geographical location.
	     public static string GetLatitudeDescription(float latitude)
	     {
		          string description = "";
		          float absLatitude = Mathf.Abs(latitude);

		          // Determine if the latitude is close to any major meridian, providing a specific description if so.
		          foreach (var meridian in SimulationConstants.meridians)
		          {
				float meridianLatitude = meridian.Value;

				if (Mathf.Abs(absLatitude - meridianLatitude) <= 1f)
				{
					     description = "<b>at the </b>" + meridian.Key;
					     return description;
				}
				else if (Mathf.Abs(absLatitude - meridianLatitude) <= 10f)
				{
					     description = "<b>near the </b>" + meridian.Key;
					     return description;
				}
		          }

		          // Classify the latitude into broader geographical regions.
		          if (absLatitude < 23.5f)
		          {
				description = "in the <color=#43aa8b>Tropical Region</color>";
		          }
		          else if (absLatitude < 66.5f)
		          {
				description = "in the <color=#90be6d>Temperate Zone</color>";
		          }
		          else
		          {
				description = "in the <color=#577590>Polar Region</color>";
		          }

		          return description;
	     }
}
