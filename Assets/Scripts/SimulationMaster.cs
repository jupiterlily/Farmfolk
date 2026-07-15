using System.Collections.Generic;
using UnityEngine;

// This class is the core of our simulation, managing the passage of time and calculating relevant environmental data.
public class SimulationMaster : MonoBehaviour
{
	     //public event EventHandler SecondPassed;
	     //public event EventHandler MinutePassed;
	     //public event EventHandler HourPassed;
	     //public event EventHandler DayPassed;
	     //public event EventHandler YearPassed;

	     // public TextMeshProUGUI readout; // Text element to display simulation information.

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
	     public float SecondsPerDay => SimulationConstants.DayLength * 3600f; // Converting day length in h to seconds.
	     public float SecondsPerHour => SecondsPerDay / SimulationConstants.DayLength; // Seconds per hour.
	     public float SecondsPerMinute => SecondsPerHour / 60; // Seconds per minute.

	     // Constants for managing months and days.
	     private const int MonthsInYear = 12;
	     private int[] daysPerMonth; // Array to hold the number of days in each month.

	     private float elapsedTime = 0f; // Tracks elapsed time in seconds for frame-to-frame updates.
	     private float intervalsEnergy; // Energy calculated at specific intervals.

	     public TileManager tileManager;
	     public int initialPlantCount;

	     void Start()
	     {
		          // Initialise the days per month array based on the year length.
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

	     // Manage the passage of time in the simulation, incrementing seconds, minutes, h, days, and years as needed.
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
					     int hToPass = Mathf.FloorToInt(secondsToPass / SecondsPerHour);
					     HourPasses(hToPass);
					     secondsToPass -= hToPass * Mathf.FloorToInt(SecondsPerHour);
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
		          if (currentSecondOfMinute == 30 || currentSecondOfMinute == 0)
		          {

		          }
		          SimulationConstants.UpdateTimeConstants(seconds, 0, 0, 0, 0);
		          currentTimeOfDay = (currentHourOfDay * SecondsPerHour + currentMinuteOfHour * SecondsPerMinute + currentSecondOfMinute) / SecondsPerDay;
		          //UpdateReadout();
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
				foreach (var tile in tileManager.tiles)
				{
					     //   tileManager.AtmosphereStep((1f / 60) * 10);
					     // tileManager.HydrosphereStep((1f / 60) * 10);
					     //   tile.TileVisualUpdate();
				}
		          }
		          if (currentMinuteOfHour == 9 || currentMinuteOfHour == 29 || currentMinuteOfHour == 49)
		          {

				tileManager.TileUpdate(1 / 3);
				tileManager.demeter.lightManager.CheckPlantLightLevel(tileManager.demeter);
				//tileManager.demeter.gardener.GrowPlants(tileManager.demeter, Mathf.FloorToInt(20 * 60));
		          }
		          SimulationConstants.UpdateTimeConstants(0, minutes, 0, 0, 0);
		          SimulationConstants.UpdateTime(currentTimeOfDay);
		          //tileManager.AtmosphereStep(minutes / 60f);
	     }

	     // Handle the passage of h within the simulation.
	     void HourPasses(int h)
	     {

		          gameObject.GetComponent<PepoPopManager>().SimulateNeeds(h);
		          for (int i = 0; i < h; i++)
		          {
				currentHourOfDay++;
				if (currentHourOfDay >= 24)
				{
					     currentHourOfDay = 0;
					     gameObject.GetComponent<PepoPopManager>().uiManager.RefreshPepoPanels();
					     DayPasses(1);
				}
		          }
		          if (currentHourOfDay == 0 || currentHourOfDay == 4 || currentHourOfDay == 8 || currentHourOfDay == 12 || currentHourOfDay == 16 || currentHourOfDay == 20)
		          {
				tileManager.demeter.gardener.GrowPlants(tileManager.demeter, Mathf.FloorToInt(4 * 60 * 60));

				tileManager.demeter.gardener.UpdatePlantAges(tileManager.demeter);
		          }
		          //tileManager.TileUpdate(h);
		          //Debug.Log("tiles updated");
		          // Log solar energy every 3 h.
		          if (currentHourOfDay % 3 == 0 && Mathf.FloorToInt(previousTimeOfDay * 24f) % 3 != currentHourOfDay % 3)
		          {
				float currentEnergy = SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(currentLatitude, dayOfYear, currentTimeOfDay);
				Debug.Log($"<color=#f9c74f>Current Solar Energy {SolarEnergyCalculator.GetLatitudeDescription(currentLatitude)} at {currentHourOfDay:00}:00: {Mathf.FloorToInt(currentEnergy)} W/m2</color>");
		          }
		          SimulationConstants.UpdateTimeConstants(0, 0, h, 0, 0);
		          intervalsEnergy = Mathf.FloorToInt(SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(currentLatitude, dayOfYear, currentTimeOfDay));
		          // UpdateReadout();
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
		          Debug.Log("tiles updated");
		          ProgressPepoPopulation(days / SimulationConstants.YearLength);

		          SimulationConstants.UpdateTimeConstants(0, 0, 0, days, 0);
		          SimulationConstants.UpdateDay(Mathf.FloorToInt(dayOfYear));
		          //    UpdateReadout();
	     }

	     // Handle the passage of years within the simulation.
	     void YearPasses(int years)
	     {
		          for (int i = 0; i < years; i++)
		          {
				Debug.Log("<color=f94144>A new year has started!</color>");
				year++;
		          }
		          ProgressPepoPopulation(years);
		          // Update the time constants to reflect the passage of years in seconds.
		          SimulationConstants.UpdateTimeConstants(0, 0, 0, 0, years);
		          //    UpdateReadout();
	     }

	     void ProgressPepoPopulation(float years = 0)
	     {
		          gameObject.GetComponent<PepoPopManager>().agingIncrement = years;
		          gameObject.GetComponent<PepoPopManager>().SimulateGenerations();
	     }
}

//delegates for time events. at each minute, 
//1 min, 6 min, 10 min, 12 min, 15 min, 20 min, 30 min, 
//1 h, 3 h, 4 h, 6 h, 8h, 12 h, 
//1 day, 2 days, 3 days, 4 days, 5 days, 7 days, 13 days, 14 days, 26 days, 28 days, 52 days, 73 days, 91 days, 182 days, 364 days, 365 days, 366 days,
//1 week, 2 weeks, 4 weeks, 13 weeks, 26 weeks, 52 weeks
//1 month, 2 months, 3 months, 4 months, 6 months, 12 months
//1 year, 2 years, 5 years, 10 years, 20 years, 50 years, 100 years
