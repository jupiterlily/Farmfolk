using System.Collections.Generic;
using UnityEngine;

public static class PepoUtilitiesInteractions
{
	     public static List<PepoData> MakeAcquaintences(PepoData pepo, List<PepoData> group, float hours = 1 / 6f)
	     {
		          //an introduction takes 10 minutes
		          float introChance = Random.value - 0.5f;
		          float personalisyModifier = Random.value - 0.5f;
		          float valuesModifier = Random.value - 0.5f;

		          int minutesLeft = (int)hours * 60;
		          List<PepoData> acquaintences = new List<PepoData>();

		          personalisyModifier += (pepo.personality.Extraversion.value / 5) - 1;
		          personalisyModifier += (pepo.personality.Openness.value / 5) - 1;
		          personalisyModifier += (pepo.personality.Agreeableness.value / 5) - 1;

		          personalisyModifier /= 4f;

		          valuesModifier += (pepo.values[0].value); //curiosity
		          valuesModifier += (pepo.values[2].value); //excitement
		          valuesModifier += (pepo.values[8].value); //status
		          valuesModifier += (pepo.values[13].value); //cohesion
		          valuesModifier += (pepo.values[17].value); //tolerance
		          valuesModifier += (pepo.values[19].value); //dependability

		          valuesModifier /= 7f;
		          valuesModifier /= 7f;

		          introChance = (introChance + valuesModifier + valuesModifier) / 3;

		          if (introChance > 0)
		          {
				foreach (var lilguy in group)
				{
					     if (Random.value >= introChance && minutesLeft >= 10)
					     {
						          PepoUtilitiesBonds.AddRelation(pepo, lilguy, RelationshipType.acquaintence);
						          acquaintences.Add(lilguy);
						          minutesLeft -= 10;
					     }
				}
		          }
		          return acquaintences;
	     }


}