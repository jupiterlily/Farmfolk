using System.Collections.Generic;
using UnityEngine;

public class Gardener
{
	     /// <summary>
	     /// Simulates plant growth over a number of seconds
	     /// </summary>
	     public void GrowPlants(Demeter demeter, int seconds)
	     {
		          List<Plant> deadPlants = new List<Plant>();

		          // Create a copy of keys since we'll be modifying the dictionary
		          //var PlantsSnapshot = new List<Plant>(demeter.ActivePlants.Keys);

		          foreach (var plant in demeter.AllPlants)
		          {
				if (plant.isDead)
				{
					     deadPlants.Add(plant);
					     continue;
				}

				// Calculate solar energy for plant's position
				//derive latitude from plant map and tilemanager y/z is latitude
				//get plant position y value as a percentage of world size

				//derive time of day from longitude. x is longitude every 15 degrees is one hour difference
				float day = SimulationConstants.DayOfYear;
				float time = SimulationConstants.TimeOfDay; //a value between 0 and 1 that represents time of day
				float longitude = demeter.tileManager.GetLongitude(demeter.PlantMap[plant].x, demeter.PlantMap[plant].y);


				float latitude = demeter.tileManager.GetLatitude(demeter.PlantMap[plant].x, demeter.PlantMap[plant].y);
				float wattM2 = SolarEnergyCalculator.CalculateSolarEnergyAtLocation(latitude, longitude, day, time
				);

				// Simulate growth
				plant.SimulateGrowth(latitude, wattM2, seconds);
		          }
		          //add the mass of the plant to soil as organic matter.


		          // Remove any plants that died during growth
		          if (deadPlants.Count > 0)
		          {
				demeter.RemoveDeadPlants(deadPlants);

				// Replace dead plants to maintain minimum
				foreach (var deadPlant in deadPlants)
				{

					     Vector3 activationCenter = demeter.GetRandomPosition();
					     demeter.CreateNewPlant(demeter.AllPlants.Count, demeter.tileManager.grid.tiles[(int)activationCenter.x, (int)activationCenter.z].tileData);
				}
		          }
	     }

	     /// <summary>
	     /// Redraws all active plants based on their current growth state
	     /// </summary>
	     public void UpdatePlantAges(Demeter demeter)
	     {
		          // Create a copy of keys since dictionary might be modified
		          var activePlantsSnapshot = new List<Plant>(demeter.ActivePlants.Keys);

		          foreach (var plant in activePlantsSnapshot)
		          {
				Vector3 plantPosition = demeter.PlantMap[plant];

				// Calculate plant age in seconds
				int? ageInSeconds = SimulationConstants.SubtractTimeConstantsAndConvertToSeconds(
				    SimulationConstants.TimeConstants,
				    plant.TimeConstantBirthday
				);

				if (ageInSeconds.HasValue && ageInSeconds.Value >= 0)
				{
					     // Determine which growth stage to show based on age
					     string growthStage = GetGrowthStage(ageInSeconds.Value);
					     demeter.UpdatePlantGrowthStage(plant, growthStage);
				}
				else
				{
					     Debug.LogWarning($"Failed to calculate plant age or invalid age: {ageInSeconds}");
				}
		          }
	     }

	     /// <summary>
	     /// Determines which growth stage should be visible based on plant age
	     /// </summary>
	     private string GetGrowthStage(int ageInSeconds)
	     {
		          // Check from oldest to youngest to ensure correct stage assignment
		          if (ageInSeconds >= 3 * 365 * 24 * 60 * 60) // 3 years
				return "Ancient";
		          if (ageInSeconds >= 90 * 24 * 60 * 60) // 90 days
				return "Mature";
		          if (ageInSeconds >= 60 * 24 * 60 * 60) // 60 days
				return "Established";
		          if (ageInSeconds >= 14 * 24 * 60 * 60) // 14 days
				return "Juvenile";

		          // Default to Seedling for young plants
		          return "Seedling";
	     }
}