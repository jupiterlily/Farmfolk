using System.Collections.Generic;
using UnityEngine;

public class PepoData
{
	     public string forename = "John";
	     public string surname = "Doe";
	     public string fullname => PepoUtilities.GetFullName(this);
	     //latitute, longitude, altitude
	     public Vector3 currentLocation = Vector3.zero;
	     public Vector3 homeLocation = Vector3.zero;

	     public bool isAlive = false;
	     public bool isActive = false;

	     public SocioSexualData ssd;
	     public HEXACO personality;
	     public Mood mood;
	     public MaslowHeirachy needs;

	     public List<PregnancyData> womb;

	     public float age = 25f;
	     public float mass; //increases with age until adulthood, decreases with old age
	     public float height = 1.8f;
	     public float massGenetics = 70f;

	     //biological, not social
	     public PepoData mother;
	     public PepoData father;
	     public List<PepoData> offspring = new List<PepoData>();
	     public List<PepoData> knownOffspring => PepoRelationalUtilities.ShipsToPepos(RelationshipLibrary[RelationshipType.offspring]);
	     public int lovers => PepoRelationalUtilities.ShipsToPepos(RelationshipLibrary[RelationshipType.lover]).Count;

	     //public float relationshipLength = 0f; // in years
	     public Dictionary<PepoData, float> exPatners = new Dictionary<PepoData, float>();

	     public Dictionary<RelationshipType, List<PepoRelationshipData>> RelationshipLibrary = new Dictionary<RelationshipType, List<PepoRelationshipData>>();
	     public PepoData(bool random, float age, float weight, float height, PepoData mat = null, PepoData pat = null, PepoData donor = null, SocioSexualData sd = null)
	     {
		          if (random)
		          {
				InitialiseRandom();
		          }
		          else
		          {
				if (mat != null && pat != null)
				{
					     InitialiseFromParents(mat, pat);
				}
				else
				{
					     InitialiseFixed(age, weight, height, sd, donor);
				}
		          }

	     }
	     public PepoData ClonePepo(bool newborn = false)
	     {
		          PepoData clone = new PepoData(false, age, mass, height, null, null, this, new SocioSexualData());
		          if (newborn)
		          {
				clone.age = 0;
		          }

		          return clone;
	     }
	     public void InitialiseFixed(float ageYears, float weightKg, float htMeters, SocioSexualData sosex = null, PepoData donor = null)
	     {
		          if (donor == null)
		          {
				//here we copy the values from the donor to our own pepodata

		          }
		          if (sosex == null)
		          {
				sosex = new SocioSexualData();
		          }
		          age = ageYears;
		          massGenetics = weightKg;
		          height = htMeters;
		          ssd = sosex;
		          InitialiseStats();
		          InitialiseRelationshipLibrary();
		          needs = new MaslowHeirachy(this);
	     }
	     public void InitialiseRandom()
	     {
		          age = Random.Range(16f, 50f);
		          massGenetics = massGenetics * Random.Range(0.6f, 2f);
		          mass = massGenetics;//
		          height = height * Random.Range(0.8f, 1.2f);

		          ssd = new SocioSexualData();
		          if (!ssd.hasY)
		          {
				womb = new List<PregnancyData>();
		          }

		          InitialiseStats();
		          mother = this;
		          father = this;
		          homeLocation = new Vector3(Random.Range(0, (mass + age) / 2), Random.Range(0, (mass + age) / 2), 0);
		          currentLocation = homeLocation;

		          InitialiseRelationshipLibrary();

		          forename = PepoNameUtilities.InitialiseForname(this);
		          surname = PepoNameUtilities.InitialiseSurname();
		          personality = new HEXACO();
		          if (age > 25)
		          {
				InitialisePuberty(25);
		          }
		          needs = new MaslowHeirachy(this);
		          PepoLogUtilities.LogPepo(this);
	     }
	     public void InitialiseFromParents(PepoData mat, PepoData pat)
	     {
		          //Inherit average of parents' traits with some variation
		          age = 0.0001f; //newborn
		          massGenetics = (mat.massGenetics + pat.massGenetics) / 2f * Random.Range(0.9f, 1.1f);
		          mass = massGenetics * 0.03428f; //newborn mass is about 3.428% of adult mass
		          height = (mat.height + pat.height) / 2f * Random.Range(0.95f, 1.05f);
		          ssd = new SocioSexualData();
		          ssd.isPrepubescent = true;
		          InitialiseStats();
		          InitialiseRelationshipLibrary();
		          personality = new HEXACO();
		          mother = mat;
		          father = pat;
		          PepoNameUtilities.InitialiseForname(this);
		          surname = mat.surname; //pat's surname
		          currentLocation = mat.currentLocation;
		          homeLocation = mat.homeLocation;
		          needs = new MaslowHeirachy(this);
	     }
	     public void InitialiseRelationshipLibrary()
	     {
		          RelationshipLibrary.Add(RelationshipType.lover, new List<PepoRelationshipData>());
		          RelationshipLibrary.Add(RelationshipType.offspring, new List<PepoRelationshipData>());
		          RelationshipLibrary.Add(RelationshipType.kin, new List<PepoRelationshipData>());
	     }
	     //public void InitialiseNeeds()
	     //{
	     //      float wattsPerSecond = PepoUtilities.BMR(mass, PepoUtilities.BodySurfaceArea(mass, height, ssd.hasY), 20, 37.5f);
	     //      float wattsPerHour = wattsPerSecond * 3600;
	     //      float caloriesPerDay = wattsPerHour * 24 / 4.184f; //1 calorie = 4.184 joules
	     //      Need hunger = new Need("hunger", "food", "eat", "stuffed", "full", "peckish", "hungry", "starving", caloriesPerDay * 1.5f, true, -1, caloriesPerDay / 24);
	     //      //thirst = new Need("Thirst", (2 * mass) * 4 * 24, (2 * mass) * 4, true);
	     //      float dailyWaterIntake = (2 * mass) * 4; // in ml

	     //      Need thirst = new Need("thirst", "drink", "drink", "bloated", "satisfied", "thirsty", "parched", "dehydrated", dailyWaterIntake * 1.5f, true, -1, dailyWaterIntake / 24);//3000 ml, 125 ml per hour = 24 hours to deplete
	     //							              //pick a random number between 6 and 10 hours for rest need, decay rate is 1 full rest per 8 hours
	     //      float restHours = Random.Range(6f, 10f);
	     //      //rest = new Need("Rest", restHours * 1.2f, restHours / 24, true);
	     //      Need rest = new Need("rest", "sleep", "rest", "well-rested", "alert", "sleepy", "tired", "exhausted", restHours * 1.2f, true, -1, restHours / 24); //8 hours of sleep, 1 hour per hour = 8 hours to deplete
	     //      Need health = MaslowHeirachy.NeedTemplates.Find(n => n.name == "health");
	     //      health.maxValue = mass / 70f * age / 25f;
	     //      Debug.Log($"{fullname} is {hunger.descriptor}, {thirst.descriptor}, {rest.descriptor}");
	     //}
	     public void InitialiseStats()
	     {
		          //speed is height in meters and then modified by age and mass. returns avalue in m/s
		          float baseSpeed = height * 0.75f;
		          //reduce speed by 1% for every year over 20, and by 1% for every 2kg over 70kg
		          baseSpeed *= Mathf.Clamp01(1f - ((age - 20f) / 100f));
		          baseSpeed *= Mathf.Clamp01(1f - ((massGenetics - 70f) / 200f));
	     }
	     public void InitialisePuberty(float pubertyAge)
	     {

		          Debug.Log($"[PepoManager]{PepoUtilities.GetFullName(this)} has hit puberty");
		          ssd.isFertile = true;
		          ssd.isPrepubescent = false;
		          if (!ssd.hasY)
		          {
				womb = new List<PregnancyData>();
		          }
	     }
	     public void InitialiseDeath()
	     {
		          isAlive = false;
		          PepoLogUtilities.LogDeath(this);
	     }
	     public void InitialisePregnancy(int pregnancies, PepoData sperms = null, bool identical = false)
	     {
		          if (sperms == null)
		          {
				int i = 0;
				int bd = Random.Range(0, ssd.potBabyDaddies.Count);
				sperms = ssd.potBabyDaddies[bd];
				while (i <= pregnancies)
				{
					     womb.Add(new PregnancyData(sperms, this));
					     i++;
				}
		          }
		          else
		          {
				int i = 0;
				while (i <= pregnancies)
				{
					     womb.Add(new PregnancyData(sperms, this));
					     i++;
				}
		          }
		          ssd.potBabyDaddies = new List<PepoData>();
	     }
	     public void CompletePregnancy(PregnancyData pregnancy)
	     {
		          womb.Remove(pregnancy);
	     }
	     public void InitialiseAging(float years = 0)
	     {
		          age += years;
		          if (age >= 13f && ssd.isPrepubescent)
		          {
				InitialisePuberty(age);
		          }
		          if (womb != null && womb.Count > 0)
		          {
				foreach (var p in womb)
				{
					     p.Progress(years);
				}
		          }

	     }
}
public class Stat
{
	     public string name;
	     public float currentValue;
	     public float maxValue;
	     public Stat(string name, float maxValue)
	     {
		          this.name = name;
		          this.maxValue = maxValue;
		          currentValue = maxValue;
	     }
}

public class PepoLook
{
	     public Color hairColor;
	     public Color eyeColor;
}

public class Aspect
{
	     public string name;
	     public float value;
}
public class PepoAspect : Aspect
{
	     public float maxValue;
	     public string descriptor;
	     public int timesIntegrated = 1;
	     public void IntegrateAspect(PepoAspect aspect)
	     {
		          value = (aspect.value + (value * timesIntegrated)) / (timesIntegrated + 1);
	     }
}

