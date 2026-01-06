using System.Collections.Generic;
using UnityEngine;

public class SocioSexualData
{

	     public bool isFertile = false;
	     public bool isPrepubescent = false;
	     //public bool hasY = false; //can impregnate
	     //public bool hasX = true; //can have a womb
	     public int[] wantsChildren;

	     public string identity = string.Empty;

	     public bool isFemme = false; //gender identity internally
	     public bool isMasc = false;

	     public bool presentsFemme = false; //gender presentation
	     public bool presentsMasc = false;

	     public bool likesFemme = false; //sexuality
	     public bool likesMasc = false;

	     public bool platonicFemme = false; //who i will befriend
	     public bool platonicMasc = false;

	     public bool romanceFemme = false;
	     public bool romanceMasc = false;

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
		          int minBairns = Random.Range(0, 15);
		          wantsChildren = new int[] { minBairns, Random.Range(minBairns, 15) };
		          isMonogamous = (Random.value > 0.3f) ? true : false;
		          isAsexual = (Random.value < 0.3f) ? true : false;
		          isAplatonic = (Random.value < 0.7f) ? true : false;

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