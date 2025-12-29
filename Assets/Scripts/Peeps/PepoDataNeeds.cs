using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Need : NumericalAspect
{
	     public float currentLevel;
	     public float decayRate; // per hour
	     public float satietyThreshold; // below this level, peep shows signs of need
	     public float criticalThreshold;
	     public bool isVital; // if true, peep dies if this need reaches zero

	     public float warningThreshold => (satietyThreshold + criticalThreshold) / 2;

	     //flavour text
	     public string source; //ie food, drink, sleep
	     public string overMax; //ie stuffed, bloated, rested
	     public string aboveSatiety; //ie full, satisfied, rested
	     public string belowSatiety; //ie peckish, thirsty, tired

	     public string aboveCritical; //ie hungry, parched, exhausted	   	     
	     public string belowCritical; //ie starving, dehydrated, fatigued
	     public string increasing; //ie eating, drinking, resting
	     public Need(string noun, string sourceNoun, string verb, string excess, string sated, string falling, string urgent, string critical, float maxValue, bool vital = false, float value = -1, float decayRate = 0, float increaseRate = 0, float safeThreshold = 0.75f, float lowThreshold = 0.25f)
	     {
		          isVital = vital;
		          name = noun;
		          this.source = sourceNoun;
		          this.increasing = verb;
		          this.overMax = excess;
		          this.aboveSatiety = sated;
		          this.belowSatiety = falling;
		          this.aboveCritical = urgent;
		          this.belowCritical = critical;
		          this.maxValue = maxValue;
		          minValue = 0;
		          this.currentLevel = value >= 0 ? value : Random.Range(maxValue * lowThreshold, maxValue * safeThreshold);
		          this.decayRate = decayRate;
		          this.satietyThreshold = maxValue * safeThreshold;
		          this.criticalThreshold = maxValue * lowThreshold;
		          DescribeNeed();
	     }
	     public void Satisfy(float amount)
	     {
		          currentLevel = Mathf.Min(currentLevel + amount, maxValue);
	     }
	     public void Deplete(float amount)
	     {
		          currentLevel = Mathf.Max(currentLevel - amount, 0);
	     }
	     public void DescribeNeed()
	     {
		          //a switch that sets the description based on current level
		          if (currentLevel >= maxValue)
		          {
				descriptor = overMax;
		          }
		          else if (currentLevel >= satietyThreshold && currentLevel < maxValue)
		          {
				descriptor = aboveSatiety;
		          }
		          else if (currentLevel >= warningThreshold && currentLevel < satietyThreshold)
		          {
				descriptor = belowSatiety;
		          }
		          else if (currentLevel >= criticalThreshold && currentLevel < warningThreshold)
		          {
				descriptor = aboveCritical;
		          }
		          else
		          {
				descriptor = belowCritical;
		          }
	     }
}
public class MaslowHeirachy
{
	     public List<Need> Physiological = new List<Need>();
	     public List<Need> Safety = new List<Need>();
	     public MaslowHeirachy(PepoData lilGuy)
	     {
		          float wattsPerSecond = PepoUtilities.BMR(lilGuy.mass, PepoUtilities.BodySurfaceArea(lilGuy.mass, lilGuy.height, lilGuy.body.paternalChromosome.sexChromosome == "XY"), 20, 37.5f);
		          float wattsPerHour = wattsPerSecond * 3600;
		          float caloriesPerDay = wattsPerHour * 24 / 4.184f; //1 calorie = 4.184 joules
		          Need hunger = new Need("hunger", "food", "eat", "stuffed", "full", "peckish", "hungry", "starving", (caloriesPerDay / 2000 * lilGuy.mass / 70f) * 2000, true, -1, caloriesPerDay / 24);
		          Physiological.Add(hunger);

		          float dailyWaterIntake = (2 * lilGuy.mass) * 4; // in ml
		          Need thirst = new Need("thirst", "water", "drink", "bloated", "satisfied", "thirsty", "parched", "dehydrated", dailyWaterIntake * lilGuy.massGenetics / 70f, true, -1, dailyWaterIntake / 24);
		          Physiological.Add(thirst);
		          //pick a random number between 6 and 10 hours for rest need, decay rate is 1 full rest per 8 hours

		          float restHours = Random.Range(6f, 10f);
		          Need rest = new Need("rest", "sleep", "rest", "well-rested", "alert", "sleepy", "tired", "exhausted", restHours * 1.2f, true, -1, restHours / 24, 1);
		          Physiological.Add(rest);

		          //use geneticmass to work out bladder size.
		          float bladderSize = Random.Range(300f, 700f) * (lilGuy.mass / 70);
		          Need bladder = new Need("bladder", "toilet", "tinkle", "relieved", "comfortable", "urgent", "desperate", "incontinent", bladderSize, false, decayRate: bladderSize / 6f);
		          Physiological.Add(bladder);
		          Need health = new Need("health", "care", "heal", "healthy", "fine", "suffering", "critical", "dying", lilGuy.mass / 70f * lilGuy.height / 1.7f * lilGuy.age / 25f * lilGuy.age / 16f * 80f / lilGuy.age * 45 / lilGuy.age * 10, true);
		          Safety.Add(health);
		          foreach (var need in Physiological)
		          {
				need.DescribeNeed();
				need.satietyThreshold = need.maxValue * 0.75f;
				need.criticalThreshold = need.maxValue * 0.25f;
				need.value = Random.Range(need.criticalThreshold, need.maxValue);
		          }
		          foreach (var need in Safety)
		          {
				need.DescribeNeed();
				need.satietyThreshold = need.maxValue * 0.75f;
				need.criticalThreshold = need.maxValue * 0.25f;
				need.value = Random.Range(need.criticalThreshold, need.maxValue);
		          }

	     }
	     public List<Need> GetAllCriticalNeeds()
	     {
		          return Physiological.Concat(Safety)
		              .Where(need => need.value <= need.criticalThreshold)
		              .ToList();
	     }
	     public List<Need> GetAllUrgentNeeds()
	     {
		          return Physiological.Concat(Safety)
		              .Where(need => need.value <= need.warningThreshold)
		              .ToList();
	     }
}


public class PhysiologicalNeed : Need
{//hunger, thirst, sleep, etc.

	     public PhysiologicalNeed(string noun, string sourceNoun, string verb, string excess, string sated, string falling, string urgent, string critical, float maxValue, bool vital = true, float value = -1, float decayRate = 0, float increaseRate = 0, float safeThreshold = 0.75f, float lowThreshold = 0.25f)
		          : base(noun, sourceNoun, verb, excess, sated, falling, urgent, critical, maxValue, vital, value, decayRate, increaseRate, safeThreshold, lowThreshold)
	     {
		          isVital = true;
	     }
}