using System.Collections.Generic;
using TMPro;
using UnityEngine;

// This class is the core of our simulation, managing the passage of time and calculating relevant environmental data.
public class SimulationMaster : MonoBehaviour
{
	     //public event EventHandler SecondPassed;
	     //public event EventHandler MinutePassed;
	     //public event EventHandler HourPassed;
	     //public event EventHandler DayPassed;
	     //public event EventHandler YearPassed;

	     public TextMeshProUGUI readout; // Text element to display simulation information.

	     public float dayOfYear = 0f; // Current day within the year.
	     public float year = 0f; // Current year in the simulation.

	     private float previousTimeOfDay = 0f; // The time of day recorded at the last update. between 0 and 1
	     private float currentTimeOfDay; // Represents the fraction of the day that has passed, from 0 to 1.

	     private int currentHourOfDay = 0; // Current hour of the day, ranges from 0 to 23.
	     private int currentMinuteOfHour = 0; // Current minute within the hour, ranges from 0 to 59.
	     private int currentSecondOfMinute = 0; // Current second within the minute, ranges from 0 to 59.

	     public float currentLatitude = 30f; // Latitude for solar energy calculations.

	     // Conversion constants based on simulation time parameters.
	     public float SecondsPerYear => SimulationConstants.YearLength * SecondsPerDay;
	     public float SecondsPerDay => SimulationConstants.DayLength * 3600f; // Converting day length in hours to seconds.
	     public float SecondsPerHour => SecondsPerDay / SimulationConstants.DayLength; // Seconds per hour.
	     public float SecondsPerMinute => SecondsPerHour / 60; // Seconds per minute.

	     // Constants for managing months and days.
	     private const int MonthsInYear = 12;
	     private int[] daysPerMonth; // Array to hold the number of days in each month.

	     private float elapsedTime = 0f; // Tracks elapsed time in seconds for frame-to-frame updates.
	     private float intervalsEnergy; // Energy calculated at specific intervals.

	     public TileManager tileManager;

	     [SerializeField]
	     public List<Sprite> plantSprites;

	     void Start()
	     {
		          // Initialize the days per month array based on the year length.
		          CalculateDaysPerMonth();
		          // Calculate and log solar energy for predefined latitudes at the start of the simulation.
		          List<int> days = new List<int> { 90, 91, 92, 182, 183, 184, 273, 275, 276, 363, 364, 365 };

		          foreach (float meridian in SimulationConstants.meridians.Values)
		          {
				foreach (int day in days)
				{
					     Debug.Log($"<color=#f9c74f>Current Solar Energy {SolarEnergyCalculator.GetLatitudeDescription(meridian)} at midday on day {day - 45}: {Mathf.FloorToInt(SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(meridian, day - 45, 0.5f))} W/m2</color>");
				}
		          }

		          tileManager = gameObject.GetComponent<TileManager>();
	     }

	     // Calculate the number of days per month, accounting for variations in month lengths.
	     void CalculateDaysPerMonth()
	     {
		          daysPerMonth = new int[MonthsInYear];
		          int baseDaysPerMonth = Mathf.FloorToInt(SimulationConstants.YearLength / MonthsInYear);
		          int extraDays = Mathf.RoundToInt(SimulationConstants.YearLength % MonthsInYear);

		          for (int i = 0; i < MonthsInYear; i++)
		          {
				daysPerMonth[i] = baseDaysPerMonth;
				if (i < extraDays)
				{
					     daysPerMonth[i]++;
				}
		          }
	     }

	     void Update()
	     {
		          // Progress time based on simulation speed and elapsed real time.
		          float timeProgression = Time.deltaTime * SimulationConstants.SimulationSpeed;
		          elapsedTime += timeProgression;
		          int secondsToPass = Mathf.FloorToInt(elapsedTime);
		          elapsedTime -= secondsToPass;

		          // Update the simulation time.
		          PassSeconds(secondsToPass);

		          // Update the on-screen readout with the current simulation state.

	     }

	     // Manage the passage of time in the simulation, incrementing seconds, minutes, hours, days, and years as needed.
	     private void PassSeconds(int secondsToPass)
	     {
		          while (secondsToPass > 0)
		          {
				if (secondsToPass >= SecondsPerYear)
				{
					     int yearsToPass = Mathf.FloorToInt(secondsToPass / SecondsPerYear);
					     YearPasses(yearsToPass);
					     secondsToPass -= yearsToPass * Mathf.FloorToInt(SecondsPerYear);
				}
				else if (secondsToPass >= SecondsPerDay)
				{
					     int daysToPass = Mathf.FloorToInt(secondsToPass / SecondsPerDay);
					     DayPasses(daysToPass);
					     secondsToPass -= daysToPass * Mathf.FloorToInt(SecondsPerDay);
				}
				else if (secondsToPass >= SecondsPerHour)
				{
					     int hoursToPass = Mathf.FloorToInt(secondsToPass / SecondsPerHour);
					     HourPasses(hoursToPass);
					     secondsToPass -= hoursToPass * Mathf.FloorToInt(SecondsPerHour);
				}
				else if (secondsToPass >= SecondsPerMinute)
				{
					     int minutesToPass = Mathf.FloorToInt(secondsToPass / SecondsPerMinute);
					     MinutePasses(minutesToPass);
					     secondsToPass -= minutesToPass * Mathf.FloorToInt(SecondsPerMinute);
				}
				else
				{
					     SecondPasses(secondsToPass);
					     secondsToPass = 0;
				}
		          }
	     }

	     // Update the on-screen readout with the current state of the simulation.
	     private void UpdateReadout()
	     {
		          readout.text = $"Year: {year}\n";
		          readout.text += $"Day of Year: {SimulationConstants.DayOfYear + 1}\n";
		          readout.text += $"Game time: {currentHourOfDay:00}:{currentMinuteOfHour:00}\n";
		          //readout.text += $"Elapsed time: {SimulationConstants.TimeConstants[5]:000}:{SimulationConstants.TimeConstants[4]:000}:{SimulationConstants.TimeConstants[3]:000}:{SimulationConstants.TimeConstants[2]:000}:{SimulationConstants.TimeConstants[1]:000}:{SimulationConstants.TimeConstants[0]:000}\n";
		          //readout.text += $"Energy @ Latitude: {intervalsEnergy}";
	     }

	     // Handle the passage of seconds within the simulation.
	     void SecondPasses(int seconds)
	     {
		          for (int i = 0; i < seconds; i++)
		          {
				currentSecondOfMinute++;
				if (currentSecondOfMinute >= 60)
				{
					     currentSecondOfMinute = 0;
					     MinutePasses(1);
				}
		          }
		          SimulationConstants.UpdateTimeConstants(seconds, 0, 0, 0, 0);
		          currentTimeOfDay = (currentHourOfDay * SecondsPerHour + currentMinuteOfHour * SecondsPerMinute + currentSecondOfMinute) / SecondsPerDay;
		          UpdateReadout();
	     }

	     // Handle the passage of minutes within the simulation.
	     void MinutePasses(int minutes)
	     {
		          for (int i = 0; i < minutes; i++)
		          {
				currentMinuteOfHour++;
				if (currentMinuteOfHour >= 60)
				{
					     currentMinuteOfHour = 0;
					     HourPasses(1);
				}
		          }
		          //every  10 minutes
		          if (currentMinuteOfHour == 0 || currentMinuteOfHour == 10 || currentMinuteOfHour == 20 || currentMinuteOfHour == 30 || currentMinuteOfHour == 40 || currentMinuteOfHour == 50)
		          {

		          }
		          if (currentMinuteOfHour == 9 || currentMinuteOfHour == 29 || currentMinuteOfHour == 49)
		          {

		          }
		          SimulationConstants.UpdateTimeConstants(0, minutes, 0, 0, 0);
		          SimulationConstants.UpdateTime(currentTimeOfDay);
	     }

	     // Handle the passage of hours within the simulation.
	     void HourPasses(int hours)
	     {
		          for (int i = 0; i < hours; i++)
		          {
				currentHourOfDay++;
				if (currentHourOfDay >= 24)
				{
					     currentHourOfDay = 0;
					     DayPasses(1);
				}
		          }
		          if (currentHourOfDay == 0 || currentHourOfDay == 4 || currentHourOfDay == 8 || currentHourOfDay == 12 || currentHourOfDay == 16 || currentHourOfDay == 20)
		          {

		          }
		          tileManager.TileUpdate(hours);
		          Debug.Log("tiles updated");
		          // Log solar energy every 3 hours.
		          if (currentHourOfDay % 3 == 0 && Mathf.FloorToInt(previousTimeOfDay * 24f) % 3 != currentHourOfDay % 3)
		          {
				float currentEnergy = SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(currentLatitude, dayOfYear, currentTimeOfDay);
				Debug.Log($"<color=#f9c74f>Current Solar Energy {SolarEnergyCalculator.GetLatitudeDescription(currentLatitude)} at {currentHourOfDay:00}:00: {Mathf.FloorToInt(currentEnergy)} W/m2</color>");
		          }
		          SimulationConstants.UpdateTimeConstants(0, 0, hours, 0, 0);
		          intervalsEnergy = Mathf.FloorToInt(SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(currentLatitude, dayOfYear, currentTimeOfDay));
		          UpdateReadout();
	     }

	     // Handle the passage of days within the simulation.
	     void DayPasses(int days)
	     {
		          for (int i = 0; i < days; i++)
		          {
				Debug.Log("<color=f8961e>A new day has started!</color>");
				dayOfYear++;
				if (dayOfYear >= SimulationConstants.YearLength)
				{
					     dayOfYear = 0;
					     YearPasses(1);
				}
		          }
		          tileManager.TileUpdate(days * 24);
		          Debug.Log("tiles updated");
		          SimulationConstants.UpdateTimeConstants(0, 0, 0, days, 0);
		          SimulationConstants.UpdateDay(Mathf.FloorToInt(dayOfYear));
		          UpdateReadout();
	     }

	     // Handle the passage of years within the simulation.
	     void YearPasses(int years)
	     {
		          for (int i = 0; i < years; i++)
		          {
				Debug.Log("<color=f94144>A new year has started!</color>");
				year++;
		          }

		          // Update the time constants to reflect the passage of years in seconds.
		          SimulationConstants.UpdateTimeConstants(0, 0, 0, 0, years);
		          UpdateReadout();
	     }
}
