using UnityEngine;

public class LightManager //should be plant colour manager. we'll update the organs on the plant depending on how much water is in the plant, how much light is hitting the plant
{
	     private Vector3 NightTint;

	     public LightManager(Vector3 nightTint)
	     {
		          NightTint = nightTint;
	     }

	     // Method CheckPlantLightLevel goes here
	     public void CheckPlantLightLevel(Demeter demeter)
	     {
		          foreach (var plant in demeter.ActivePlants)
		          {
				if (plant.Key == null)
				{
					     Debug.LogError($"activePlants contains a null value for plant: {plant}");
					     continue;
				}

				float latitude = (demeter.PlantMap[plant.Key].y + 4.45f) * 10;
				float wattM2 = SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(latitude, SimulationConstants.DayOfYear, SimulationConstants.TimeOfDay);
				Color plantLeafColor = plant.Key.Leaves.LeafColor;
				float nightPlantH;
				float nightPlantS;
				float nightPlantV;
				Color.RGBToHSV(plantLeafColor, out nightPlantH, out nightPlantS, out nightPlantV);
				nightPlantH += NightTint.x;
				nightPlantS += NightTint.y;
				nightPlantV += NightTint.z;
				Color nightColor = Color.HSVToRGB(nightPlantH, nightPlantS, nightPlantV);
				Color currentColor = Color.Lerp(nightColor, plantLeafColor, wattM2 / SimulationConstants.SolarConstant);
				//plant.Value.GetComponent<SpriteRenderer>().color = currentColor;
		          }
	     }
}

