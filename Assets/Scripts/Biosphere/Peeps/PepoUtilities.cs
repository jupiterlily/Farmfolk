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
					     Pepo.mother.ssd.wantsChildren[0]++;
					     Pepo.mother.ssd.wantsChildren[1]++;
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

		          if (s1.wantsChildren != null && s2.wantsChildren != null && s1.wantsChildren.Length >= 2 && s2.wantsChildren.Length >= 2)
		          {
				int aMin = s1.wantsChildren[0], aMax = s1.wantsChildren[1];
				int bMin = s2.wantsChildren[0], bMax = s2.wantsChildren[1];
				int overlapMin = Mathf.Max(aMin, bMin), overlapMax = Mathf.Min(aMax, bMax);
				float overlap = Mathf.Max(0, overlapMax - overlapMin + 1);
				float span = Mathf.Max(aMax - aMin + 1, bMax - bMin + 1);
				// Normalize this single dimension into the match/total counts
				matches += Mathf.RoundToInt(overlap);
				total += Mathf.RoundToInt(span);
		          }

		          if (total == 0)
				return 0.5f;
		          return Mathf.Clamp01((float)matches / total);
	     }

	     // Body comparison by expressed chromosomes (0..1)
	     public static float CompareBodyData(BodyData b1, BodyData b2)
	     {
		          if (b1 == null || b2 == null || b1.expressedChromosomes == null || b2.expressedChromosomes == null)
				return 0.5f;
		          int count = Mathf.Min(b1.expressedChromosomes.Count, b2.expressedChromosomes.Count);
		          if (count == 0)
				return 0.5f;
		          int same = 0;
		          for (int i = 0; i < count; i++)
				if (b1.expressedChromosomes[i] == b2.expressedChromosomes[i])
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

}
