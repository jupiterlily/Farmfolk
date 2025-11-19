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
					     Pepo.mother.ssd.wantsChildren++;
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
				> 90f => deathRoll < 0.6f,
				> 75f => deathRoll < 0.1f,
				> 60f => deathRoll < 0.2f,
				> 10f => deathRoll < 0.001f,
				> 5f => deathRoll < 0.05f,
				> 3f => deathRoll < 0.15f,
				< 1f => deathRoll < 0.3f,
				_ => deathRoll < 0.2f
		          };
	     }

	     //Death

	     public static PepoData RemoveAncestors()
	     {
		          return null;
	     }
}
