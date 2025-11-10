
using System.Collections.Generic;
using UnityEngine;

public class Need : PepoAspect
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
		          else if (currentLevel >= satietyThreshold)
		          {
				descriptor = aboveSatiety;
		          }
		          else if (currentLevel >= warningThreshold)
		          {
				descriptor = belowSatiety;
		          }
		          else if (currentLevel >= criticalThreshold)
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
	     public List<Need> PhysiologicalNeeds = new List<Need>()
	     { new Need("hunger", "food", "eat", "stuffed", "full", "peckish", "hungry", "starving", 2000, true) };

	     public static List<Need> NeedTemplates = new List<Need>()
{
	     // Physiological Needs. breathing, flood, water, sleep, heat, cold, pee, poop
	     new Need("hunger", "food", "eat", "stuffed", "full", "peckish", "hungry", "starving", 2000, true, decayRate: 83.33f), //2000 calories, 83.33 cal per hour = 24 hours to deplete
	     new Need("thirst", "drink", "drink", "bloated", "satisfied", "thirsty", "parched", "dehydrated", 3000, true, decayRate: 125f), //3000 ml, 125 ml per hour = 24 hours to deplete
	     new Need("rest", "sleep", "rest", "well-rested", "alert", "sleepy", "tired", "exhausted", 8, true, decayRate: 1f), //8 hours of sleep, 1 hour per hour = 8 hours to deplete
	     new Need("oxygen", "air", "breathe", "hyperventilating", "breathing", "breathless", "gasping", "suffocating", 1440, true, decayRate: 60f), //1440 minutes of oxygen, 60 minutes per hour = 24 hours to deplete
	     new Need("temperature", "warmth", "warm up", "overheated", "warm", "comfortable", "cold", "freezing", 100, true, decayRate: 4.16f), //100 units of temperature comfort, 4.16 units per hour = 24 hours to deplete
	     new Need("elimination", "toilet", "use the toilet", "relieved", "comfortable", "urge to go", "desperate", "incontinent", 100, true, decayRate: 4.16f), //100 units of bladder/bowel comfort, 4.16 units per hour = 24 hours to deplete
	     // Safety Needs. health, security of resources, morality, family, health, property
	     new Need("health", "medical care", "heal", "robust", "healthy", "suffering", "critical", "dying", 100, true, decayRate: 0.1f), //health out of 100, decays slowly without care)
	     new Need("security", "shelter", "secure", "safe", "protected", "vulnerable", "threatened", "in danger", 100, false, decayRate: 0.5f), //security out of 100
										    // Social Needs. connection, love, belonging, romance, friendhsip
	     new Need("social connection", "social interaction", "socialize", "overwhelmed", "popular", "connected", "lonely", "isolated", 100, false, decayRate: 1f), //social connection out of 100
	     new Need("intimacy", "intimate interaction", "be intimate", "satisfied", "fulfilled", "desiring", "craving", "deprived", 100, false, decayRate: 1f), //intimacy out of 100
											    // Esteem Needs. self-esteem, respect, recognition, strength, freedom
	     new Need("esteem", "achievement", "achieve", "accomplished", "confident", "doubtful", "insecure", "worthless", 100, false, decayRate: 0.5f), //esteem out of 100
										           // Cognitive Needs. knowledge, meaning, self-awareness
	     new Need("cognition", "learning", "learn", "enlightened", "knowledgeable", "curious", "confused", "ignorant", 100, false, decayRate: 0.5f), //cognition out of 100
										          // Aesthetic Needs. appreciation of beauty and excellence
	     new Need("aesthetic appreciation", "artistic experience", "appreciate", "inspired", "pleased", "indifferent", "displeased", "averse", 100, false, decayRate: 0.5f), //aesthetic appreciation out of 100
	     //self-actualization Needs. personal growth, self-fulfillment, peak experiences
	     new Need("self-actualization", "personal growth", "grow", "fulfilled", "satisfied", "unfulfilled", "stagnant", "lost", 100, false, decayRate: 0.2f), //self-actualization out of 100
	     // Transcendence Needs. helping others achieve self-actualization
	     new Need("transcendence", "helping others", "help", "enlightened", "fulfilled", "desiring", "craving", "deprived", 100, false, decayRate: 0.2f) }; //transcendence out of 100
	     public MaslowHeirachy(PepoData lilGuy)
	     {
		          float wattsPerSecond = PepoUtilities.BMR(lilGuy.mass, PepoUtilities.BodySurfaceArea(lilGuy.mass, lilGuy.height, lilGuy.ssd.hasY), 20, 37.5f);
		          float wattsPerHour = wattsPerSecond * 3600;
		          float caloriesPerDay = wattsPerHour * 24 / 4.184f; //1 calorie = 4.184 joules

		          Need hunger = NeedTemplates.Find(n => n.name == "hunger");
		          hunger.maxValue = (caloriesPerDay / 2000 * lilGuy.mass / 70f) * 2000; //scaling hunger max by mass
		          hunger.decayRate = caloriesPerDay / 24;

		          Need thirst = NeedTemplates.Find(n => n.name == "thirst");
		          float dailyWaterIntake = (2 * lilGuy.mass) * 4; // in ml
		          thirst.maxValue = dailyWaterIntake / 2000 * lilGuy.mass / 70f * 3000; //scaling thirst max by mass
		          thirst.decayRate = dailyWaterIntake / 24;
		          //pick a random number between 6 and 10 hours for rest need, decay rate is 1 full rest per 8 hours

		          //rest = new Need("Rest", restHours * 1.2f, restHours / 24, true);
		          Need rest = NeedTemplates.Find(n => n.name == "rest");
		          float restHours = Random.Range(6f, 10f);
		          rest.maxValue = restHours * 1.2f;
		          rest.decayRate = restHours / 24;

		          Need health = NeedTemplates.Find(n => n.name == "health");
		          //breakpoints //decreases above 70 years old 55 years old, decreases below 25 years old, decreases below 16 years old
		          health.maxValue = lilGuy.mass / 70f * lilGuy.height / 1.7f * lilGuy.age / 25f * lilGuy.age / 16f * 80f / lilGuy.age * 45 / lilGuy.age * 10;
		          health.value = health.maxValue;
		          foreach (var need in NeedTemplates)
		          {
				need.DescribeNeed();
		          }
		          Debug.Log($"{lilGuy.fullname} is {hunger.descriptor}, {thirst.descriptor}, {rest.descriptor}. They have {health.value:0}/{health.maxValue:0} hitpoints");
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