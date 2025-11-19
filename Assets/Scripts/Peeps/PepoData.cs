using System;
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

	     public bool isAlive = true;
	     public bool isActive = false;

	     public SocioSexualData ssd;
	     public BodyData body;
	     public HEXACO personality;
	     public Mood mood;
	     public MaslowHeirachy needs;
	     public List<Value> values = new List<Value>(PepoFactoryValues.RandomValues());

	     public List<PregnancyData> womb;

	     public float age = 25f;
	     public float mass; //increases with age until adulthood, decreases with old age
	     public float height = 1.8f;
	     public float massGenetics = 70f;

	     //biological, not social
	     public PepoData mother;
	     public PepoData father;
	     public List<PepoData> offspring = new List<PepoData>();
	     public List<PepoData> knownOffspring => PepoUtilitiesBonds.ShipsToPepos(RelationshipLibrary[RelationshipType.offspring]);
	     public int lovers => PepoUtilitiesBonds.ShipsToPepos(RelationshipLibrary[RelationshipType.lover]).Count;

	     //public float relationshipLength = 0f; // in years
	     public Dictionary<PepoData, float> exPatners = new Dictionary<PepoData, float>();

	     public Dictionary<RelationshipType, List<PepoRelationshipData>> RelationshipLibrary = new Dictionary<RelationshipType, List<PepoRelationshipData>>();
	     public PepoData()
	     {

	     }
	     public void InitialiseDeath()
	     {
		          isAlive = false;
		          PepoLogUtilities.LogDeath(this);
	     }
	     public void InitialisePregnancy(int pregnancies, PepoData sperms = null, bool identical = false)
	     {                   //if i'm already pregnant, sack the whole thing off.
		          if (sperms == null)
		          {
				int i = 0;
				int bd = UnityEngine.Random.Range(0, ssd.potBabyDaddies.Count);
				sperms = ssd.potBabyDaddies[bd];
				while (i < pregnancies)
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
		          ssd.isFertile = false;
		          Debug.Log($"<color=#6667ab>[BirthManager]</color>{sperms.fullname} got {fullname} pregnant");
	     }
	     public void CompletePregnancy(PregnancyData pregnancy)
	     {//debug log birthisfertile
		          womb.Remove(pregnancy);
		          ssd.isFertile = true;
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
	     public float minValue;
	     public bool polar; //does the aspect have poles?
	     public float polarPoint; //ie 0 for values, 5 for personality.
	     public string descriptor;
	     public int timesIntegrated = 1;
	     //normalises and averages
	     // Normalizes and averages aspects
	     public void IntegrateAspect(PepoAspect aspect, bool additive = true)
	     {
		          float scaledAspectValue = aspect.value;
		          float factor = maxValue / aspect.maxValue;

		          // Handle polar aspects
		          if (polar && aspect.polar)
		          {
				// Calculate normalized positions relative to polar point
				float currentPolarPosition = (value - polarPoint) / (maxValue - polarPoint);
				float incomingPolarPosition = (aspect.value - aspect.polarPoint) / (aspect.maxValue - aspect.polarPoint);

				// For polar aspects, we want to consider the direction from the polar point
				// Values on the same "side" of the pole should reinforce each other
				// Values on opposite sides should balance each other

				if (Math.Sign(currentPolarPosition) == Math.Sign(incomingPolarPosition))
				{
					     // Same side of pole - weighted average
					     scaledAspectValue = polarPoint + (incomingPolarPosition * (maxValue - polarPoint));
				}
				else
				{
					     // Opposite sides - move toward pole (balance out)
					     float balanceFactor = 0.5f; // Adjust this for how quickly opposites balance
					     scaledAspectValue = value + ((polarPoint - value) * balanceFactor);
				}
		          }
		          else
		          {
				// Non-polar integration - simple scaling
				scaledAspectValue = aspect.value * factor;
		          }

		          // Update value with weighted average
		          if (additive)
		          {
				value = ((scaledAspectValue) + (value * timesIntegrated)) / (timesIntegrated + 1);
				timesIntegrated++;
		          }
		          else
		          {
				// Simple average without increasing integration count
				value = (scaledAspectValue + value) / 2f;
		          }
	     }

	     // Helper method to get normalized value (0-1 range)
	     public float GetNormalizedValue()
	     {
		          return (value - minValue) / (maxValue - minValue);
	     }

	     // Helper method to get polar-relative position (-1 to 1, where 0 is at polar point)
	     public float GetPolarPosition()
	     {
		          if (!polar)
				return 0f;

		          if (value >= polarPoint)
				return (value - polarPoint) / (maxValue - polarPoint);
		          else
				return -(polarPoint - value) / (polarPoint - minValue);
	     }
}

