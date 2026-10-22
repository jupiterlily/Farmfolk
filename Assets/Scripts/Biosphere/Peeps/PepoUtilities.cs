using System.Collections.Generic;
using UnityEngine;

public class PepoUtilities
{
	     public static string GetFullName(PepoData me)
	     {
		          return $"{me.forename} {me.surname}";

	     }
	     public static float BodySurfaceArea(float massKg, float heightM, bool gender)
	     {
		          float surfaceArea = 0f;
		          // Schlichformula formula for body surface area
		          if (gender == true)
		          {
				//Men BSA = 0.000579479 × W0.38 × H1.24
				surfaceArea = 0.000579479f * Mathf.Pow(massKg, 0.38f) * Mathf.Pow(heightM * 100f, 1.24f);
				return surfaceArea;
		          }
		          else
		          {
				//Women BSA = 0.000975482 × W0.46 × H1.08
				surfaceArea = 0.000975482f * Mathf.Pow(massKg, 0.46f) * Mathf.Pow(heightM * 100f, 1.08f);
				return surfaceArea;
		          }
	     }
	     public static float BMR(float massKg, float surfaceAreaM2, float ambientTempC, float bodyTempC)
	     {
		          // Basal Metabolic Rate estimation using Kleiber's law and temperature adjustment
		          // BMR (W) = 70 * (mass in kg)^0.75 * e^(0.04 * (bodyTemp - ambientTemp))
		          //returns bmr in joules per second (watts)
		          float bmrBase = 70f * Mathf.Pow(massKg, 0.75f);
		          float tempFactor = Mathf.Exp(0.04f * (bodyTempC - ambientTempC));
		          return bmrBase * tempFactor; // in Watts
	     }
	     public static List<PepoData> KillPepos(List<PepoData> guys, float years = 1)
	     {//algorithmically kill pepos based on age and random chance
		          var losses = new List<PepoData>();
		          foreach (var Pepo in guys)
		          {
				if (!Pepo.isAlive)
				{
					     losses.Add(Pepo);
					     continue;
				}
				if (ShouldDie(Pepo))
				{
					     Pepo.InitialiseDeath();
					     losses.Add(Pepo);
				}
				if (Pepo.age < 10f)
				{
					     Pepo.mother.ssd.numberOfChildren.value--;
					     Pepo.mother.ssd.numberOfChildren.value--;
				}
		          }
		          return losses;
	     }
	     public static bool ShouldDie(PepoData Pepo, float years = 1)
	     {
		          float deathRoll = Random.value * years;
		          float age = Pepo.age;

		          return age switch
		          {
				> 90f => deathRoll < 0.23f,
				> 75f => deathRoll < 0.032f,
				> 60f => deathRoll < 0.0075f,
				> 25f => deathRoll < 0.0003f,
				> 15f => deathRoll < 0.0002f,
				> 1f => deathRoll < 0.00015f,
				< 1f => deathRoll < 0.005f,
				_ => deathRoll < 0.2f
		          };
	     }

	     //Death

	     public static PepoData RemoveAncestors()
	     {
		          return null;
	     }



	     // Socio-sexual comparison (0..1)
	     public static float CompareSocioSexualData(SocioSexualData s1, SocioSexualData s2)
	     {
		          if (s1 == null || s2 == null)
				return 0.5f;

		          int matches = 0;
		          int total = 0;

		          bool[] flags1 = new bool[] { s1.isAsexual, s1.isAromantic, s1.isMonogamous, s1.isAplatonic };
		          bool[] flags2 = new bool[] { s2.isAsexual, s2.isAromantic, s2.isMonogamous, s2.isAplatonic };
		          for (int i = 0; i < flags1.Length; i++)
		          {
				if (flags1[i] == flags2[i])
					     matches++;
				total++;
		          }

		          bool[] prefs1 = new bool[] { s1.likesFemme, s1.likesMasc, s1.romanceFemme, s1.romanceMasc, s1.platonicFemme, s1.platonicMasc };
		          bool[] prefs2 = new bool[] { s2.likesFemme, s2.likesMasc, s2.romanceFemme, s2.romanceMasc, s2.platonicFemme, s2.platonicMasc };
		          for (int i = 0; i < prefs1.Length; i++)
		          {
				if (prefs1[i] == prefs2[i])
					     matches++;
				total++;
		          }
		          //see if we both want more children than we have, then see if one of our minimmums is greater than the others maximum. we get amatch if we both want children, a match if we both don't want children. a match if 
		          int aMax = (int)s1.numberOfChildren.maxValue - (int)s1.numberOfChildren.value;
		          int aMin = (int)s1.numberOfChildren.minValue - (int)s1.numberOfChildren.value;

		          int bMax = (int)s2.numberOfChildren.maxValue - (int)s2.numberOfChildren.value;
		          int bMin = (int)s2.numberOfChildren.minValue - (int)s2.numberOfChildren.value;

		          int overlapMin = Mathf.Max(aMin, bMin), overlapMax = Mathf.Min(aMax, bMax);
		          float overlap = Mathf.Max(0, overlapMax - overlapMin + 1);
		          float span = Mathf.Max(aMax - aMin + 1, bMax - bMin + 1);
		          // Normalize this single dimension into the match/total counts
		          matches += Mathf.RoundToInt(overlap);
		          total += Mathf.RoundToInt(span);


		          if (total == 0)
				return 0.5f;
		          return Mathf.Clamp01((float)matches / total);
	     }

	     // Body comparison by expressed chromosomes (0..1) although this should only be for appearance genes. it doesnt make sense that things like lifespan would be included in this
	     public static float CompareBodyData(BodyData b1, BodyData b2)
	     {
		          if (b1 == null || b2 == null || b1.physicalAttributes == null || b2.physicalAttributes == null)
				return 0.5f;
		          int count = 0;
		          int same = 0;
		          //go through our dictionary of appearance attributes and compare them to the other bodydata's dictionary of appearance attributes. if they have the same key and the same value, we count it as a match. if they have the same key but different values, we count it as a mismatch. if they don't have the same key, we ignore it.
		          foreach (var kvp in b1.physicalAttributes)
		          {
				if (b2.physicalAttributes.TryGetValue(kvp.Key, out var otherValue))
				{
					     count++;
					     if (Mathf.Approximately(kvp.Value.value, otherValue.value))
						          same++;
				}
		          }
		          same++;
		          return Mathf.Clamp01((float)same / count);
	     }
	     public static float ComparePepos(PepoData b1, PepoData b2, List<string> traits = null)
	     {
		          if (b1 == null || b2 == null || b1.FilterableAspects == null || b2.FilterableAspects == null || traits == null)
				return 0.5f;
		          int count = 0;
		          int same = 0;
		          //go through our dictionary of appearance attributes and compare them to the other bodydata's dictionary of appearance attributes. if they have the same key and the same value, we count it as a match. if they have the same key but different values, we count it as a mismatch. if they don't have the same key, we ignore it.
		          //probably better to see if these are all on both lists first maybe? the key would be something like headshape, really i think we should be checking if the 
		          foreach (var kvp in b1.FilterableAspects)
		          {
				if (b2.FilterableAspects.TryGetValue(kvp.Key, out var otherValue))
				{
					     if (traits.Contains(kvp.Key))
					     {
						          count++;
						          if (Mathf.Approximately(kvp.Value.value, otherValue.value))
								same++;
					     }
				}
		          }
		          same++;
		          return Mathf.Clamp01((float)same / count);
	     }

	     // HEXACO personality comparison (0..1)
	     public static float CompareHEXACO(HEXACO h1, HEXACO h2)
	     {
		          if (h1 == null || h2 == null)
				return 0.5f;
		          var a = h1.FullHexRange();
		          var b = h2.FullHexRange();
		          if (a == null || b == null || a.Length == 0 || b.Length == 0)
				return 0.5f;
		          int n = Mathf.Min(a.Length, b.Length);
		          float sum = 0f;
		          for (int i = 0; i < n; i++)
		          {
				float max = Mathf.Max(a[i].maxValue, b[i].maxValue);
				float diff = Mathf.Abs(a[i].value - b[i].value);
				float sim = 1f - (diff / (max > 0 ? max : 10f));
				sum += Mathf.Clamp01(sim);
		          }
		          return Mathf.Clamp01(sum / n);
	     }

	     // Mood comparison (0..1)
	     public static float CompareMood(Mood m1, Mood m2)
	     {
		          if (m1 == null || m2 == null)
				return 0.5f;
		          var a = m1.FullHexRange();
		          var b = m2.FullHexRange();
		          if (a == null || b == null || a.Length == 0 || b.Length == 0)
				return 0.5f;
		          int n = Mathf.Min(a.Length, b.Length);
		          float sum = 0f;
		          for (int i = 0; i < n; i++)
		          {
				float max = Mathf.Max(a[i].maxValue, b[i].maxValue);
				float diff = Mathf.Abs(a[i].value - b[i].value);
				float sim = 1f - (diff / (max > 0 ? max : 10f));
				sum += Mathf.Clamp01(sim);
		          }
		          return Mathf.Clamp01(sum / n);
	     }


	     private static List<List<PepoData>> CollectFamilyData(PepoData root, int maxGenerations)
	     {
		          var familyData = new List<List<PepoData>>();

		          // First, collect all ancestors (from oldest to youngest)
		          var ancestors = CollectAncestors(root, maxGenerations);

		          // Then, collect all descendants (from oldest to youngest)  
		          var descendants = CollectDescendants(root, maxGenerations);

		          // Combine them: ancestors + root + descendants
		          familyData.AddRange(ancestors);
		          familyData.Add(new List<PepoData> { root }); // The root person and their mothers children
		          familyData.AddRange(descendants);

		          return familyData;
	     }

	     private static List<List<PepoData>> CollectAncestors(PepoData root, int maxGenerations)
	     {
		          var ancestors = new List<List<PepoData>>();
		          var currentGeneration = new List<PepoData> { root };

		          for (int gen = 1; gen <= maxGenerations; gen++)
		          {
				var parentGeneration = new List<PepoData>();

				foreach (var person in currentGeneration)
				{
					     if (person?.mother != null && !parentGeneration.Contains(person.mother))
						          parentGeneration.Add(person.mother);
					     if (person?.father != null && !parentGeneration.Contains(person.father))
						          parentGeneration.Add(person.father);
				}

				if (parentGeneration.Count > 0)
				{
					     ancestors.Insert(0, parentGeneration); // Insert at beginning to maintain chronological order
					     currentGeneration = parentGeneration;
				}
				else
				{
					     break;
				}
		          }

		          return ancestors;
	     }

	     private static List<List<PepoData>> CollectDescendants(PepoData root, int maxGenerations)
	     {
		          var descendants = new List<List<PepoData>>();
		          var currentGeneration = new List<PepoData> { root };

		          for (int gen = 1; gen <= maxGenerations; gen++)
		          {
				var childGeneration = new List<PepoData>();

				foreach (var person in currentGeneration)
				{
					     if (person?.offspring != null)
					     {
						          foreach (var child in person.offspring)
						          {
								if (child != null && !childGeneration.Contains(child))
									     childGeneration.Add(child);
						          }
					     }
				}

				if (childGeneration.Count > 0)
				{
					     descendants.Add(childGeneration);
					     currentGeneration = childGeneration;
				}
				else
				{
					     break;
				}
		          }

		          return descendants;
	     }

}
