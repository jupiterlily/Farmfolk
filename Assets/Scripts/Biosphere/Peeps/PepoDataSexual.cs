using System.Collections.Generic;
using UnityEngine;

public class SocioSexualData
{

	     public bool isFertile = false;
	     public bool isPrepubescent = false;
	     //public bool hasY = false; //can impregnate
	     //public bool hasX = true; //can have a womb
	     // public int[] wantsChildren;

	     public string identity = string.Empty;

	     public bool isFemme = false; //gender identity internally
	     public bool isMasc = false;

	     public bool presentsFemme = false; //gender presentation
	     public bool presentsMasc = false;

	     public bool likesFemme = false; //sexuality
	     public bool likesMasc = false;

	     public bool platonicFemme = false; //who i will befriend
	     public bool platonicMasc = false;

	     public bool romanceFemme = false;//who i will romance with
	     public bool romanceMasc = false;

	     //maybe who we will formally bond with.

	     public NumericalAspect numberOfChildren = new NumericalAspect("children", 30, 0, false);
	     public NumericalAspect numberOfSexualPartners = new NumericalAspect("sexual partners", 45, 0, false);
	     public NumericalAspect numberOfFriends = new NumericalAspect("friends", 30, 0, false);
	     public NumericalAspect numberOfRomanticPartners = new NumericalAspect("romantic partners", 15, 0, false);


	     public bool isMonogamous = false;
	     public bool isAsexual = true;
	     public bool isAplatonic = false;
	     public bool isAromantic = false;

	     public List<PepoData> potBabyDaddies = new List<PepoData>();
	     public SocioSexualData(BodyData body)
	     {
		          // hasY = (Random.value > 0.5f) ? true : false;
		          //hasX = (Random.value > 0.01f) ? true : false;
		          isFertile = (Random.value > 0.01f) ? true : false;
		          isPrepubescent = true;
		          int minBairns = Random.Range(0, (int)numberOfChildren.maxValue);
		          int maxBairns = Random.Range(minBairns, (int)numberOfChildren.maxValue);
		          numberOfChildren.minValue = minBairns;
		          numberOfChildren.maxValue = maxBairns;
		          numberOfChildren.value = 0;
		          isMonogamous = (Random.value < 0.7f) ? true : false;
		          isAsexual = (Random.value < 0.1f) ? true : false;
		          isAplatonic = (Random.value < 0.7f) ? true : false;

		          if (isAsexual)
		          {
				numberOfSexualPartners.minValue = 0;
				numberOfSexualPartners.maxValue = 0;
		          }
		          else if (isMonogamous)
		          {
				numberOfRomanticPartners.minValue = 1;
				numberOfRomanticPartners.maxValue = 1;

				numberOfSexualPartners.minValue = 1;
				numberOfSexualPartners.maxValue = 1;
		          }
		          else
		          {
				numberOfRomanticPartners.minValue = Random.Range(2, numberOfRomanticPartners.maxValue);
				numberOfRomanticPartners.maxValue = Random.Range(numberOfRomanticPartners.minValue, numberOfRomanticPartners.maxValue);

				numberOfSexualPartners.minValue = Random.Range(2, numberOfRomanticPartners.maxValue);
				numberOfSexualPartners.maxValue = Random.Range(numberOfRomanticPartners.minValue, numberOfSexualPartners.maxValue);
		          }

		          if (body.paternalChromosome.sexChromosome == "XY")
		          {
				isMasc = (Random.value > 0.02f) ? true : false;
				isFemme = (Random.value > 0.9f) ? true : false;
				likesMasc = (Random.value > 0.5f) ? true : false;
				likesFemme = (Random.value > 0.1f) ? true : false;
				romanceFemme = (Random.value > 0.4f) ? true : false;
				romanceMasc = (Random.value > 0.8f) ? true : false;
				presentsMasc = (Random.value > 0.05f) ? true : false;
				presentsFemme = (Random.value > 0.7f) ? true : false;
		          }
		          else
		          {
				isMasc = (Random.value > 0.9f) ? true : false;
				isFemme = (Random.value > 0.02f) ? true : false;
				likesMasc = (Random.value > 0.1f) ? true : false;
				likesFemme = (Random.value > 0.5f) ? true : false;
				romanceMasc = (Random.value > 0.4f) ? true : false;
				romanceFemme = (Random.value > 0.8f) ? true : false;
				presentsMasc = (Random.value > 0.7f) ? true : false;
				presentsFemme = (Random.value > 0.05f) ? true : false;
		          }
		          if (romanceFemme || romanceMasc)
		          {
				isAromantic = false;
		          }
	     }
}