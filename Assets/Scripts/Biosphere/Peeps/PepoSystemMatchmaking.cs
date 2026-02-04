using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PepoSystemMatchmaking
{
	     public static bool AmIAttracted(PepoData me, PepoData target, float degree = 5, bool recip = false)
	     {

		          if (target == null || me == null)
		          {
				Debug.LogWarning("No targets to check attraction for");
				return false;
		          }
		          if (degree >= 1 && AreRelated(me, target, degree))
		          {
				PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.kin);
				return false;
		          }
		          //Am I sexually attracted to them?
		          if (degree >= 2 && (!me.ssd.likesFemme && (target.ssd.presentsFemme || target.ssd.isFemme)) || (!me.ssd.likesMasc && (target.ssd.presentsMasc || target.ssd.isMasc) && !PepoUtilitiesPersonality.IsLying(target)))
		          {
				Debug.LogWarning($"[Relationship Manager]{me.fullname} doesn't like {target.fullname}'s gender expression");
				PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
				return false;
		          }
		          //what do we think of theri appearnace? people who value conformity or have low openness want people who look like them
		          if (degree >= 2)
		          {
				float similarityscore = PepoUtilitiesMatchmaking.ComparePhysicalAppearance(me, target);
				if (me.values[11].value > 1)
				{
					     float parentalsimilarityscore = PepoUtilitiesMatchmaking.ComparePhysicalAppearance(me.mother, target);
					     parentalsimilarityscore += PepoUtilitiesMatchmaking.ComparePhysicalAppearance(me.father, target);
					     parentalsimilarityscore /= 2f;
					     //tradition
					     parentalsimilarityscore *= (me.values[11].value / me.values[11].maxValue);
					     similarityscore *= parentalsimilarityscore;
				}
				float requiredSimilarity = 0f;
				//low openness or high conformity means you want people who look like you
				requiredSimilarity += (1 - me.personality.Openness.value / me.personality.Openness.maxValue) / 4;
				requiredSimilarity += (me.values[13].value / me.values[13].maxValue) / 4;

				//high universalism means you want people who look different to you
				requiredSimilarity -= (me.values[17].value / me.values[17].maxValue) / 4;
				//high excitement means you want different and new
				requiredSimilarity -= (me.values[2].value / me.values[2].maxValue) / 4;
				if (similarityscore < requiredSimilarity)
				{
					     Debug.LogWarning($"[Relationship Manager]{me.fullname} doesn't find {target.fullname} physically attractive (similarity: {similarityscore}, required: {requiredSimilarity})");

					     PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
					     return false;
				}

		          }
		          //am I romantically inclined towards them?
		          if (degree >= 3 && (!me.ssd.romanceFemme && (target.ssd.presentsFemme || target.ssd.isFemme)) || (!me.ssd.romanceMasc && (target.ssd.presentsMasc || target.ssd.isMasc) && !PepoUtilitiesPersonality.IsLying(target)))
		          {
				Debug.LogWarning($"[Relationship Manager]{me.fullname} isn't romantically attracted to {target.fullname}");
				PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
				return false;
		          }
		          //are we sexually compatible?
		          if (degree >= 4 && (!me.ssd.isAsexual && (target.ssd.isAsexual || (target.ssd.isAsexual && !PepoUtilitiesPersonality.IsLying(target)))))
		          {
				Debug.LogWarning($"[Relationship Manager]{me.fullname} and {target.fullname} don't want the same things sexually");
				PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
				return false;
		          }
		          //are they old enough
		          if (degree >= 5 && (target.age < (me.age / 2) + 7 && !PepoUtilitiesPersonality.IsLying(target) && !PepoUtilitiesPersonality.IsLying(me)))
		          {
				Debug.LogWarning($"[Relationship Manager]{target.fullname} is too young for {me.fullname}");
				PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
				return false;
		          }
		          //are they too old?
		          if (degree >= 6 && (target.age > me.age * 2 && !PepoUtilitiesPersonality.IsLying(target) && !PepoUtilitiesPersonality.IsLying(me)))
		          {
				Debug.LogWarning($"[Relationship Manager]{target.fullname} is too old for {me.fullname}");
				PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
				return false;
		          }
		          //am I old enough
		          if (degree >= 7 && (me.age > (target.age / 2) + 7 && !PepoUtilitiesPersonality.IsLying(target) && !PepoUtilitiesPersonality.IsLying(me)))
		          {
				Debug.LogWarning($"[Relationship Manager]{me.fullname} thinks they aren't old enough to date {target.fullname}");
				PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
				return false;
		          }
		          //am I too old?
		          if (degree >= 8 && (me.age < target.age * 2 && !PepoUtilitiesPersonality.IsLying(target) && !PepoUtilitiesPersonality.IsLying(me)))
		          {
				Debug.LogWarning($"[Relationship Manager]{me.fullname} thinks they are too old for {target.fullname}");
				PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
				return false;
		          }
		          //if i am monogomous i am not attracted to people who aren't monogomous, unlessss they are dishonest about it
		          if (degree >= 8 && (me.ssd.isMonogamous && (target.lovers != 0 || (target.lovers != 0 && !PepoUtilitiesPersonality.IsLying(target)))))
		          {
				Debug.LogWarning($"[Relationship Manager]{me.fullname} wants a monogamous relationship but {target.fullname} prefers to keep things open");
				PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
				return false;
		          }
		          int iWant = me.ssd.wantsChildren[1] - me.knownOffspring?.Count(o => o != null && o.isAlive) ?? 0;
		          int theyWant = target.ssd.wantsChildren[0] - target.knownOffspring?.Count(o => o != null && o.isAlive) ?? 0;
		          int diff = iWant - theyWant;
		          if (degree >= 9 && (iWant > theyWant || (iWant > theyWant && !PepoUtilitiesPersonality.IsLying(target))))
		          {
				Debug.LogWarning($"[Relationship Manager]{me.fullname} wants more children than {target.fullname}");
				PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
				return false;
		          }
		          //do we share values?
		          bool agree = ShareValues(me, target, degree);
		          if (degree >= 9 && (!agree || (!agree && !PepoUtilitiesPersonality.IsLying(target))))
		          {
				Debug.LogWarning($"[Relationship Manager]{me.fullname} and {target.fullname} don't value the same things");
				PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
				return false;
		          }
		          //are they attracted to me?		          
		          if (degree >= 10 && !recip)
		          {
				bool mutual = AmIAttracted(target, me, degree, true);
				if (!mutual || (!mutual && !PepoUtilitiesPersonality.IsLying(target)))
				{
					     Debug.LogWarning($"[Relationship Manager]{target.fullname} isnt attracted to {me.fullname}");
					     PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
					     return false;
				}
		          }
		          Debug.Log($"[Relationship Manager]{me.fullname} is attracted to {target.fullname}, a {PepoLogUtilities.GetGenderExpression(target)}. They value {PepoLogUtilities.Values(target, 3)} It seems like they are {PepoLogUtilities.Hexaco(target.personality, 2)}");
		          return true;
	     }

	     public static bool AreRelated(PepoData a, PepoData b, float degree = 5)
	     {
		          //ensure that there is no direct relation. if peep a is on peep b's list of children or vice versa, they're related.
		          if (degree >= 1 && (b.offspring.Contains(a) || a.offspring.Contains(b)))
		          {
				Debug.Log($"[Relationship Manager]{a.fullname} and {b.fullname} are very closely related");
				return true;
		          }
		          //check if they share parents
		          if (degree >= 2 && (PepoUtilitiesPopulation.HasSharedParent(a, b)))
		          {
				Debug.Log($"[Relationship Manager]{a.fullname} and {b.fullname} are very closely related");
				return true;
		          }
		          //check if we are aunts or uncles
		          if (degree >= 3 && (PepoUtilitiesPopulation.HasSharedParent(a.mother, b) || PepoUtilitiesPopulation.HasSharedParent(a.father, b) || PepoUtilitiesPopulation.HasSharedParent(a, b.mother) || PepoUtilitiesPopulation.HasSharedParent(a, b.father)))
		          {
				Debug.Log($"[Relationship Manager]{a.fullname} and {b.fullname} are closely related");
				return true;
		          }
		          //check if they share grandparents
		          if (degree >= 4 && (PepoUtilitiesPopulation.HasSharedParent(a.father, b.father) || PepoUtilitiesPopulation.HasSharedParent(a.mother, b.mother) || PepoUtilitiesPopulation.HasSharedParent(a.father, b.mother) || PepoUtilitiesPopulation.HasSharedParent(a.mother, b.father)))
		          {
				Debug.Log($"[Relationship Manager]{a.fullname} and {b.fullname} are related");
				return true;
		          }
		          //check if we share any cousins
		          if (degree >= 5 && (PepoUtilitiesPopulation.HasSharedCousins(a, b)))
		          {
				Debug.Log($"[Relationship Manager]{a.fullname} and {b.fullname} are indirectly related");
				return true;
		          }
		          //check is we share cousins with each others parents
		          if (degree >= 6 && (PepoUtilitiesPopulation.HasSharedCousins(a.mother, b) || PepoUtilitiesPopulation.HasSharedCousins(a.father, b) || PepoUtilitiesPopulation.HasSharedCousins(a, b.mother) || PepoUtilitiesPopulation.HasSharedCousins(a, b.father)))
		          {
				Debug.Log($"[Relationship Manager]{a.fullname} and {b.fullname} are distantly related");
				return true;
		          }
		          //check if our parents share cousins
		          if (degree >= 7 && (PepoUtilitiesPopulation.HasSharedCousins(a.father, b.father) || PepoUtilitiesPopulation.HasSharedCousins(a.mother, b.mother) || PepoUtilitiesPopulation.HasSharedCousins(a.father, b.mother) || PepoUtilitiesPopulation.HasSharedCousins(a.mother, b.father)))
		          {
				Debug.Log($"[Relationship Manager]{a.fullname} and {b.fullname} are very distantly related");
				return true;
		          }
		          //take your list of kin and my list of kin and see if anyone on it is  the same
		          return false;
	     }
	     public static bool ShareValues(PepoData a, PepoData b, float degree = 5)
	     {
		          var myValues = PepoUtilitiesValues.RankValuesByImportance(a.values);
		          var theirValues = PepoUtilitiesValues.RankValuesByImportance(b.values);
		          //do you hate anything I value?
		          if (degree >= 1)
		          {
				//iterate over my values by for the counnt of degree, if their value.description contails the values.opposite of my value, return false
				float d = degree;
				for (int i = 0; i <= d; i++)
				{
					     if (i >= myValues.Count || i >= theirValues.Count)
						          break;
					     Value myValue = myValues[i];
					     for (int j = 0; j <= d; j++)
					     {
						          Value theirValue = theirValues[j];
						          if ((myValue.value > 0 && theirValue.descriptor == myValue.opposite))
						          {
								Debug.Log($"[ValuesUtilities] {a.fullname} values {myValue.descriptor} and {b.fullname} values {theirValue.descriptor}'");
								return false;
						          }
					     }
				}
		          }
		          //do you value anything that I hate?
		          if (degree >= 2)
		          {
				//iterate over my values backwards. by for the counnt of degree, if their value.description contails the values.opposite of my value, return false
				float d = degree;
				for (int i = 0; i < d; i++)
				{
					     if (i >= myValues.Count || i >= theirValues.Count)
						          break;
					     int k = myValues.Count - 1 - i;
					     Value myValue = myValues[k];
					     for (int j = 0; j <= d; j++)
					     {
						          Value theirValue = theirValues[i];
						          // Also check if my descriptor is the opposite of their core value  
						          if ((myValue.value < 0 && myValue.descriptor == theirValue.opposite))
						          {
								Debug.Log($"[ValuesUtilities] {a.fullname}'s doesn't approve of {b.fullname}'s value of '{theirValue.descriptor}'");
								return false;
						          }
					     }
				}
		          }
		          int agreeCount = 0;
		          //do we value the same things
		          if (degree >= 3)
		          {
				//iterate over my values by for the counnt of degree, see if any of my top values match their top values, if we can't find amatch, return false. go through more of 
				bool agreeOnSomething = false;
				float d = degree;
				for (int i = 0; i <= d; i++)
				{
					     if (i >= myValues.Count || i >= theirValues.Count)
						          break;
					     Value myValue = myValues[i];
					     float degreeAgree = (degree + (a.personality.Agreeableness.value - 5)) / 2;
					     //if degreeagree is above their values.count set degreeagree to their values count
					     if (degreeAgree > theirValues.Count - 1)
					     {
						          degreeAgree = theirValues.Count - 1;
					     }
					     if (degreeAgree < 0)
					     {
						          degreeAgree = 1;
					     }
					     for (int j = 0; j <= degreeAgree; j++)
					     {
						          Value theirValue = theirValues[j];
						          if ((myValue.descriptor == theirValue.descriptor))
						          {
								agreeOnSomething = true;
								Debug.Log($"[ValuesUtilities]{a.fullname} and {b.fullname} both think {myValue.descriptor} is important");
								agreeCount += 1;
						          }
					     }
				}
				if (!agreeOnSomething)
				{
					     return false;
				}
		          }
		          //do you hate what I hate?
		          if (degree >= 4)
		          {
				//iterate over my values backwards, if i hate something, check if they hate it too.
				bool mutualDisdain = false;
				float d = degree;
				for (int i = 0; i < d; i++)
				{
					     if (i >= myValues.Count || i >= theirValues.Count)
						          break;
					     int k = myValues.Count - 1 - i;
					     Value myValue = myValues[k];
					     if (myValue.value < 0)
					     {
						          for (int j = 0; j <= theirValues.Count - 1; j++)
						          {
								int l = myValues.Count - 1 - j;
								Value theirValue = theirValues[l];

								if ((myValue.value < 0 && myValue.descriptor == theirValue.descriptor))
								{
									     Debug.Log($"[ValuesUtilities]{a.fullname} and {b.fullname} both hate {myValue.opposite} think {myValue.descriptor} is important");
									     agreeCount += 1;
									     mutualDisdain = true;
								}
						          }
					     }
				}
				if (!mutualDisdain)
				{
					     return false;
				}
		          }
		          if (agreeCount == 0)
		          {
				Debug.Log($"[ValuesUtilities]{a.fullname} and {b.fullname} don't disagree on anything major");
		          }
		          else
		          {

				Debug.Log($"[ValuesUtilities]{a.fullname} and {b.fullname} agree on {agreeCount} things");
		          }
		          return true;
	     }

	     // Separate concerns into focused classes
	     public static class BiologicalCompatibility
	     {
		          public static bool CheckBiologicalCompatibility(PepoData me, PepoData target, float strictness = 5f)
		          {
				// Age compatibility (half-your-age-plus-seven rule)
				if (!CheckAgeCompatibility(me, target, strictness))
					     return false;

				// Genetic compatibility (avoid incest)
				if (AreRelated(me, target, strictness))
					     return false;

				// Sexual orientation compatibility
				if (!CheckSexualOrientation(me, target))
					     return false;

				// Fertility preferences
				if (!CheckFertilityPreferences(me, target))
					     return false;

				return true;
		          }

		          private static bool CheckAgeCompatibility(PepoData a, PepoData b, float strictness)
		          {
				float minAge = (a.age / 2) + 7;
				float maxAge = (a.age - 7) * 2;

				if (b.age < minAge || b.age > maxAge)
				{
					     Debug.Log($"[Biological] Age incompatibility: {a.fullname} ({a.age}) and {b.fullname} ({b.age})");
					     return false;
				}
				return true;
		          }


		          private static bool CheckSexualOrientation(PepoData me, PepoData target)
		          {
				bool genderMatch = true;

				if (target.ssd.presentsFemme || target.ssd.isFemme)
					     genderMatch &= me.ssd.likesFemme;

				if (target.ssd.presentsMasc || target.ssd.isMasc)
					     genderMatch &= me.ssd.likesMasc;

				if (!genderMatch)
					     Debug.Log($"[Biological] Gender preference mismatch");

				return genderMatch;
		          }

		          private static bool CheckFertilityPreferences(PepoData me, PepoData target)
		          {
				// Check if both want children to similar degree
				if (me.ssd.wantsChildren[1] >= target.ssd.wantsChildren[0])
				{//if my max wanted kids is above their minimum we match
					     return true;
				}

				Debug.Log($"[Biological] Fertility preference mismatch: {me.ssd.wantsChildren} vs {target.ssd.wantsChildren}");
				return false;
		          }
	     }

	     public static class PersonalityCompatibility
	     {
		          public static float CalculatePersonalityCompatibility(PepoData a, PepoData b)
		          {
				float score = 0f;
				int factors = 0;

				// Honesty-Humility: Complementary values can work well
				score += CalculateTraitCompatibility(a.personality.Honesty, b.personality.Honesty, true);
				factors++;

				// Emotionality: Similar emotional levels often work better
				score += CalculateTraitCompatibility(a.personality.Emotionality, b.personality.Emotionality, false);
				factors++;

				// Extraversion: Can be complementary or similar depending on context
				float extraversionDiff = Mathf.Abs(a.personality.Extraversion.value - b.personality.Extraversion.value);
				score += 1f - (extraversionDiff / 10f); // Closer is better
				factors++;

				// Agreeableness: Higher agreeableness generally helps
				score += (a.personality.Agreeableness.value + b.personality.Agreeableness.value) / 20f;
				factors++;

				// Conscientiousness: Similar levels often work better
				float conscientiousnessDiff = Mathf.Abs(a.personality.Conscientiousness.value - b.personality.Conscientiousness.value);
				score += 1f - (conscientiousnessDiff / 10f);
				factors++;

				// Openness: Can be complementary - opposites sometimes attract
				float opennessDiff = Mathf.Abs(a.personality.Openness.value - b.personality.Openness.value);
				score += (opennessDiff / 10f) * 0.5f; // Some difference can be good
				factors++;

				return score / factors;
		          }

		          private static float CalculateTraitCompatibility(PersonalityAspect a, PersonalityAspect b, bool complementary)
		          {
				float diff = Mathf.Abs(a.value - b.value);
				if (complementary)
					     return (diff / 10f) * 0.7f; // Some difference can be good
				else
					     return 1f - (diff / 10f); // Similar is better
		          }
	     }

	     public static class ValuesCompatibility
	     {
		          public static float CalculateValuesCompatibility(PepoData a, PepoData b, int topN = 3)
		          {
				var aValues = PepoUtilitiesValues.RankValuesByImportance(a.values);
				var bValues = PepoUtilitiesValues.RankValuesByImportance(b.values);

				float compatibility = 0f;
				int matches = 0;

				// Check top values alignment
				for (int i = 0; i < Mathf.Min(topN, aValues.Count, bValues.Count); i++)
				{
					     if (aValues[i].descriptor == bValues[i].descriptor)
					     {
						          compatibility += 1f;
						          matches++;
					     }
					     else
					     {
						          // Check if values are complementary rather than opposing
						          if (!AreValuesOpposing(aValues[i], bValues[i]))
						          {
								compatibility += 0.5f;
						          }
					     }
				}

				// Check for deal-breaking value conflicts
				if (HasDealBreakingConflict(aValues, bValues))
					     return 0f;

				return matches > 0 ? compatibility / topN : 0.5f; // Neutral if no strong alignment
		          }

		          private static bool AreValuesOpposing(Value a, Value b)
		          {
				return a.opposite == b.descriptor || b.opposite == a.descriptor;
		          }

		          private static bool HasDealBreakingConflict(List<Value> aValues, List<Value> bValues)
		          {
				// Check if top values directly oppose each other
				for (int i = 0; i < Mathf.Min(2, aValues.Count, bValues.Count); i++)
				{
					     if (aValues[i].opposite == bValues[i].descriptor)
						          return true;
				}
				return false;
		          }
	     }

	     public static class SocialCompatibility
	     {
		          public static float CalculateSocialCompatibility(PepoData a, PepoData b)
		          {
				float score = 0f;

				// Existing relationship check
				var existingRelationship = PepoUtilitiesBonds.HaveWeMet(b, a);
				if (existingRelationship != null)
				{
					     // Positive existing relationships boost compatibility
					     if (existingRelationship.Affection > 0.3f)
						          score += existingRelationship.Affection;

					     // Negative existing relationships reduce compatibility
					     if (existingRelationship.Animosity > 0.3f)
						          score -= existingRelationship.Animosity;

					     // Time spent together indicates comfort
					     score += Mathf.Clamp01(existingRelationship.timeSpent / 100f);
				}

				// Social status compatibility (simplified)
				score += CalculateStatusCompatibility(a, b);

				// Shared social circles
				score += CalculateNetworkOverlap(a, b);

				return Mathf.Clamp01(score);
		          }

		          private static float CalculateStatusCompatibility(PepoData a, PepoData b)
		          {
				// Simple status calculation based on age, relationships, etc.
				float statusA = a.age / 10f + a.RelationshipLibrary.Values.Sum(r => r.Count) * 0.1f;
				float statusB = b.age / 10f + b.RelationshipLibrary.Values.Sum(r => r.Count) * 0.1f;

				float statusDiff = Mathf.Abs(statusA - statusB);
				return 1f - (statusDiff / 10f);
		          }

		          private static float CalculateNetworkOverlap(PepoData a, PepoData b)
		          {
				// Count mutual acquaintances
				var aFriends = PepoUtilitiesBonds.ShipsToPepos(a.RelationshipLibrary[RelationshipType.friend]);
				var bFriends = PepoUtilitiesBonds.ShipsToPepos(b.RelationshipLibrary[RelationshipType.friend]);

				int mutualFriends = aFriends.Intersect(bFriends).Count();
				return Mathf.Clamp01(mutualFriends / 5f); // Normalize by reasonable friend count
		          }
	     }

	     // Main refactored matchmaking system

	     public static MatchmakingResult AssessCompatibility(PepoData me, PepoData target, float strictness = 5f)
	     {
		          var result = new MatchmakingResult { IsCompatible = false };

		          // Step 1: Biological compatibility (hard requirements)
		          if (!BiologicalCompatibility.CheckBiologicalCompatibility(me, target, strictness))
		          {
				result.RejectionReason = "Biological incompatibility";
				return result;
		          }

		          // Step 2: Calculate component scores
		          result.PersonalityScore = PersonalityCompatibility.CalculatePersonalityCompatibility(me, target);
		          result.ValuesScore = ValuesCompatibility.CalculateValuesCompatibility(me, target);
		          result.SocialScore = SocialCompatibility.CalculateSocialCompatibility(me, target);

		          // Step 3: Weighted overall score
		          result.OverallScore = CalculateOverallScore(result, me.personality);

		          // Step 4: Mutual attraction check
		          result.IsMutual = CheckMutualAttraction(me, target, strictness);

		          // Step 5: Final compatibility decision
		          result.IsCompatible = result.OverallScore >= GetCompatibilityThreshold(strictness) &&
				 (!RequiresMutuality(strictness) || result.IsMutual);

		          return result;
	     }

	     private static float CalculateOverallScore(MatchmakingResult result, HEXACO personality)
	     {
		          // Personality traits influence what matters most in relationships
		          float personalityWeight = 0.4f; //low openness people value similarity more
		          float valuesWeight = 0.3f + (personality.Honesty.value * 0.02f); // Honest people value values more
		          float socialWeight = 0.3f + (personality.Extraversion.value * 0.02f); // Extraverts value social more
							     //emotional people value how they feel more

		          float totalWeight = personalityWeight + valuesWeight + socialWeight;

		          return (result.PersonalityScore * personalityWeight +
			   result.ValuesScore * valuesWeight +
			   result.SocialScore * socialWeight) / totalWeight;
	     }

	     private static bool CheckMutualAttraction(PepoData a, PepoData b, float strictness)
	     {
		          // Quick check without full recursion
		          var quickCheck = BiologicalCompatibility.CheckBiologicalCompatibility(b, a, strictness * 0.8f);
		          return quickCheck && b.ssd.likesFemme == a.ssd.presentsFemme && b.ssd.likesMasc == a.ssd.presentsMasc;
	     }

	     private static float GetCompatibilityThreshold(float strictness)
	     {
		          return 0.3f + (strictness * 0.07f); // 0.3 at strictness 0, 0.65 at strictness 5
	     }

	     private static bool RequiresMutuality(float strictness)
	     {
		          return strictness >= 7f;
	     }
}

public class MatchmakingResult
{
	     public bool IsCompatible
	     {
		          get; set;
	     }
	     public bool IsMutual
	     {
		          get; set;
	     }
	     public float OverallScore
	     {
		          get; set;
	     }
	     public float PersonalityScore
	     {
		          get; set;
	     }
	     public float ValuesScore
	     {
		          get; set;
	     }
	     public float SocialScore
	     {
		          get; set;
	     }
	     public string RejectionReason
	     {
		          get; set;
	     }
}




