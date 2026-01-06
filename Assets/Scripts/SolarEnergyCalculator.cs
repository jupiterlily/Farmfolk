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
	     // Method that takes latitude, longitude, and day of year to calculate solar energy
	     public static float CalculateSolarEnergyAtLocation(
	         float latitude,
	         float longitude,
	         float dayOfYear,
	         float currentHourUTC = 0f,
	         float planetaryRotationPeriod = 24f) // Default to Earth's 24-hour day
	     {
		          // Calculate local solar time from longitude and UTC time
		          // Each 15 degrees of longitude equals 1 hour time difference
		          float longitudeHourOffset = longitude / 15f;
		          float localSolarTime = currentHourUTC + longitudeHourOffset;

		          // Normalize to 0-24 range
		          localSolarTime = Mathf.Repeat(localSolarTime, planetaryRotationPeriod);

		          // Convert to fraction of day
		          float timeOfDay = localSolarTime / planetaryRotationPeriod;

		          // Use existing method with calculated local time
		          return CalculateCurrentEnergyAtLatitude(latitude, dayOfYear, timeOfDay);
	     }

	     // Method that returns solar energy over time for a specific location
	     public static float[] CalculateDailySolarEnergyAtLocation(
	         float latitude,
	         float longitude,
	         float dayOfYear,
	         int samplesPerDay = 24)
	     {
		          float[] dailyEnergy = new float[samplesPerDay];

		          for (int i = 0; i < samplesPerDay; i++)
		          {
				float timeOfDay = (float)i / samplesPerDay;
				dailyEnergy[i] = CalculateCurrentEnergyAtLatitude(latitude, dayOfYear, timeOfDay);
		          }

		          return dailyEnergy;
	     }

	     // Method that calculates peak solar energy time for a location on a given day
	     public static float CalculatePeakSolarTime(
	         float latitude,
	         float longitude,
	         float dayOfYear,
	         float currentHourUTC = 0f)
	     {
		          // Calculate solar noon at this location
		          // Solar noon occurs when the sun is at the highest point
		          // For a spherical planet with uniform rotation, this is when local solar time = 12:00

		          float longitudeHourOffset = longitude / 15f;
		          float localSolarNoon = 12f; // Local solar noon

		          // Convert to UTC
		          float utcPeakTime = localSolarNoon - longitudeHourOffset;

		          // Normalize to 0-24 range
		          utcPeakTime = Mathf.Repeat(utcPeakTime, 24f);

		          return utcPeakTime;
	     }

	     // Method to calculate total daily solar energy at a location
	     public static float CalculateTotalDailyEnergyAtLocation(
	         float latitude,
	         float longitude,
	         float dayOfYear,
	         int integrationSteps = 96) // 96 = every 15 minutes
	     {
		          float totalEnergy = 0f;
		          float timeStep = 1f / integrationSteps;

		          for (int i = 0; i < integrationSteps; i++)
		          {
				float timeOfDay = i * timeStep;
				float energy = CalculateCurrentEnergyAtLatitude(latitude, dayOfYear, timeOfDay);
				totalEnergy += energy * timeStep; // Integrate over time
		          }

		          return totalEnergy;
	     }

	     // Light angle method using latitude and longitude
	     public static Vector3 CalculateLightAngleAtLocation(
	         float latitude,
	         float longitude,
	         float dayOfYear,
	         float currentHourUTC = 0f)
	     {
		          // Calculate local solar time
		          float longitudeHourOffset = longitude / 15f;
		          float localSolarTime = currentHourUTC + longitudeHourOffset;
		          localSolarTime = Mathf.Repeat(localSolarTime, 24f);
		          float timeOfDay = localSolarTime / 24f;

		          // Use existing method
		          return CalculateLightAngle(latitude, dayOfYear, timeOfDay);
	     }



	     // Method to get sunrise and sunset times for a location
	     public static Vector2 GetSunriseSunsetTimes(
	         float latitude,
	         float longitude,
	         float dayOfYear,
	         float currentHourUTC = 0f)
	     {
		          // Calculate day length based on latitude and declination
		          float declination = SimulationConstants.AxialTilt * Mathf.Sin(2 * Mathf.PI * (dayOfYear / SimulationConstants.YearLength));

		          // Calculate sunrise hour angle (in degrees)
		          float cosSunriseAngle = -Mathf.Tan(latitude * Mathf.Deg2Rad) * Mathf.Tan(declination * Mathf.Deg2Rad);
		          cosSunriseAngle = Mathf.Clamp(cosSunriseAngle, -1f, 1f);

		          float sunriseHourAngle = Mathf.Acos(cosSunriseAngle) * Mathf.Rad2Deg;

		          // Convert hour angle to time (hours)
		          float dayLengthHours = (2f * sunriseHourAngle / 15f);
		          float sunriseTime = 12f - (dayLengthHours / 2f);
		          float sunsetTime = 12f + (dayLengthHours / 2f);

		          // Adjust for polar day/night
		          if (cosSunriseAngle >= 1f)
		          {
				// Polar night (no sunrise)
				return new Vector2(-1f, -1f);
		          }
		          else if (cosSunriseAngle <= -1f)
		          {
				// Polar day (24 hours of sunlight)
				return new Vector2(0f, 24f);
		          }

		          // Convert to UTC using longitude
		          float longitudeHourOffset = longitude / 15f;
		          float sunriseUTC = sunriseTime - longitudeHourOffset;
		          float sunsetUTC = sunsetTime - longitudeHourOffset;

		          // Normalize to 0-24 range
		          sunriseUTC = Mathf.Repeat(sunriseUTC, 24f);
		          sunsetUTC = Mathf.Repeat(sunsetUTC, 24f);

		          return new Vector2(sunriseUTC, sunsetUTC);
	     }

	     // Get latitude description (your existing method)
	     public static string GetLatitudeDescription(float latitude)
	     {
		          // ... (your existing implementation)
		          string description = "";
		          float absLatitude = Mathf.Abs(latitude);

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

	     // Get longitude description
	     public static string GetLongitudeDescription(float longitude)
	     {
		          longitude = Mathf.Repeat(longitude + 180f, 360f) - 180f; // Normalize to -180 to 180

		          if (Mathf.Abs(longitude) < 0.1f)
		          {
				return "on the <b>Prime Meridian</b>";
		          }
		          else if (Mathf.Abs(longitude - 180f) < 0.1f || Mathf.Abs(longitude + 180f) < 0.1f)
		          {
				return "on the <b>International Date Line</b>";
		          }
		          else if (longitude > 0)
		          {
				return "in the <color=#f94144>Eastern Hemisphere</color>";
		          }
		          else
		          {
				return "in the <color=#f8961e>Western Hemisphere</color>";
		          }
	     }


}
