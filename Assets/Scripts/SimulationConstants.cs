using System.Collections.Generic;
using UnityEngine;

public static class SimulationConstants
{
	     public const float SolarConstant = 1361f; // W/m2
	     public const float DayLength = 24f; // Length of a day in hours (constant)
	     public const float YearLength = 365f; // Length of a year in days (constant)
	     public const float AxialTilt = 23.5f; // Earth's axial tilt in degrees (constant, modifiable by user)
	     public const float OrbitalEccentricity = 0.0167f; // Earth's orbital eccentricity (constant, default for Earth)

	     public const float Kelvin = 272.15f;
	     public const float SunlightWavelength = 635;

	     public const float EXTINCTION_WATER_VAPOUR = 0.02f;
	     public const float EXTINCTION_CLOUD_WATER = 0.1f;
	     public const float EXTINCTION_ICE = 0.05f;
	     public const float EXTINCTION_AEROSOL = 0.5f;
	     public const float SSA_WATER_VAPOUR = 0.0f;
	     public const float SSA_CLOUD_WATER = 0.8f;
	     public const float SSA_ICE = 0.7f;
	     public const float SSA_AEROSOL = 0.6f;


	     public static int DayOfYear = 0;
	     public static float TimeOfDay = 0.0f;
	     public static int YearsPassed = 0;

	     public static float SimulationSpeed = 1f; // Speed of the simulation

	     public static List<int> TimeConstants = new List<int>
    {
        0, // SecondsPassed
        0, // KilosecondsPassed
        0, // MegasecondsPassed
        0, // GigasecondsPassed
        0, // TerasecondsPassed
        0  // PetasecondsPassed
    };

	     public static void UpdateTime(float currentTime)
	     {
		          TimeOfDay = currentTime;
	     }

	     public static void UpdateDay(int currentDay)
	     {
		          DayOfYear = currentDay;
	     }

	     public static void UpdateYear()
	     {
		          YearsPassed++;
	     }

	     public static Dictionary<string, float> meridians = new Dictionary<string, float>
    {
        { "<color=#577590>Pole</color>", 90f },
        { "<color=#577590>Polar Circle</color>", 66.5f },
        { "<color=#43aa8b>Tropics</color>", 23.5f },
        { "<color=#f9c74f>Equator</color>", 0f }
    };

	     // Function to update time constants.
	     public static void UpdateTimeConstants(int elapsedSeconds, int elapsedMinutes, int elapsedHours, int elapsedDays, int elapsedYears)
	     {
		          float SecondsPerMinute = (DayLength * 2.5f);
		          float SecondsPerHour = (SecondsPerMinute * SecondsPerMinute);
		          float SecondsPerDay = (DayLength * SecondsPerHour);
		          float SecondsPerYear = (YearLength * SecondsPerDay);
		          // Convert all time units to seconds
		          double totalSeconds = elapsedSeconds +
		              elapsedMinutes * SecondsPerMinute +
		              elapsedHours * SecondsPerHour +
		              elapsedDays * SecondsPerDay +
		              elapsedYears * SecondsPerYear;

		          // Call TimeConstantCycler to distribute the total seconds into the appropriate time constants
		          TimeConstantCycler(totalSeconds);
	     }

	     public static void TimeConstantCycler(double decimalToCycle)
	     {
		          // Thresholds for each time unit transition
		          double[] thresholds = new double[] { 1000, 1000, 1000, 1000, 1000, 1000 };

		          double remainder = decimalToCycle;

		          for (int i = 0; i < TimeConstants.Count; i++)
		          {
				if (i < TimeConstants.Count - 1)
				{
					     // Calculate the number of units for the current time constant
					     int units = (int)(remainder % thresholds[i]);
					     remainder = (remainder - units) / thresholds[i];

					     // Add the units to the current time constant
					     TimeConstants[i] += units;
				}
				else
				{
					     // For the highest time constant, add the remaining value directly
					     TimeConstants[i] += (int)remainder;
				}

				// Ensure no overflow within each time constant
				if (TimeConstants[i] >= thresholds[i] && i < TimeConstants.Count - 1)
				{
					     int overflow = TimeConstants[i] / (int)thresholds[i];
					     TimeConstants[i] %= (int)thresholds[i];
					     TimeConstants[i + 1] += overflow;
				}
		          }
	     }

	     // Function to subtract one list of time constants from another and convert to seconds
	     public static int? SubtractTimeConstantsAndConvertToSeconds(List<int> timeConstants1, List<int> timeConstants2)
	     {
		          if (timeConstants1.Count != timeConstants2.Count)
		          {
				Debug.LogError("Time constants lists must be of the same length.");
				return null;
		          }

		          List<int> result = new List<int>(new int[timeConstants1.Count]);
		          bool borrow = false;

		          // Subtract the time constants
		          for (int i = 0; i < timeConstants1.Count; i++)
		          {
				int diff = timeConstants1[i] - timeConstants2[i] - (borrow ? 1 : 0);
				if (diff < 0)
				{
					     diff += 1000; // assuming each time constant threshold is 1000
					     borrow = true;
				}
				else
				{
					     borrow = false;
				}
				result[i] = diff;
		          }

		          // Convert the result list to seconds
		          double totalSeconds = 0;
		          double[] thresholds = new double[] { 1, 1000, 1000000, 1e9, 1e12, 1e15 };

		          for (int i = 0; i < result.Count; i++)
		          {
				totalSeconds += result[i] * thresholds[i];
				if (totalSeconds > int.MaxValue)
				{
					     Debug.LogError("Total seconds value exceeds the maximum value for an int.");
					     return null;
				}
		          }

		          return (int)totalSeconds;
	     }


}

public static class ListExtensions
{
	     private static System.Random rng = new System.Random();

	     public static void Shuffle<T>(this IList<T> list)
	     {
		          int n = list.Count;
		          while (n > 1)
		          {
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
		          }
	     }
}
