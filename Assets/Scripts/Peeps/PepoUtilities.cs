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
	     public static List<PepoData> KillPepos(List<PepoData> guys)
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
		          }
		          return losses;
	     }
	     public static bool ShouldDie(PepoData Pepo)
	     {
		          float deathRoll = Random.value;
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
	     public static List<PepoData> GenerateChildren(PepoData mama, PepoData papa, bool multipleBirth, bool identicalTwins, int numSiblings, float increment)
	     {
		          var children = new List<PepoData>();
		          //if multiple births, then say what happens if identical or not, then handle regular siblings/births/only children, we just iterate over an int so it's all the same
		          if (multipleBirth)
		          {
				if (identicalTwins)
				{
					     children.AddRange(CreateIdenticalTwins(mama, papa, multipleBirth ? numSiblings : 1));
				}
				else
				{
					     //create siblings and then set all their ages to the same
					     children.AddRange(CreateSiblings(mama, papa, numSiblings, increment));
					     foreach (PepoData child in children)
					     {
						          child.age = 0;
					     }
				}
				//bond siblings
				foreach (PepoData child in children)
				{
					     foreach (PepoData sib in children)
					     {
						          if (child != sib)
						          {
								PepoRelationalBonds.CreateSiblingBond(child, sib, "twin");
						          }
					     }
				}
		          }
		          else if (numSiblings > 0)
		          {
				children.AddRange(CreateSiblings(mama, papa, numSiblings, increment));
		          }
		          else
		          {
				children.Add(CreateSingleChild(mama, papa, increment));
		          }
		          return children;
	     }

	     public static PepoData CreateSingleChild(PepoData mama, PepoData papa, float agingIncrement)
	     {
		          PepoData child;
		          if (mama.womb == null || mama.womb.Count == 0 || mama.womb[0].babyDaddy == null)
		          {
				child = new PepoData(false, 0, 0, 0, mama, papa);
				child.isAlive = true;
				child.age = Random.Range(0f, agingIncrement * 0.8f);
		          }
		          else
		          {
				mama.InitialisePregnancy(1, papa);
				child = mama.womb[0].GiveBirth(mama)[0];
		          }
		          PepoRelationalBonds.CreateParentalBond(mama, child);
		          mama.offspring.Add(child);
		          papa.offspring.Add(child);
		          PepoLogUtilities.LogPepo(child);
		          return child;
	     }

	     public static List<PepoData> CreateSiblings(PepoData mama, PepoData papa, int numSiblings, float agingIncrement)
	     {
		          var children = new List<PepoData>();
		          float baseAge = Random.Range(0f, (agingIncrement * 0.8f) / numSiblings);
		          mama.InitialisePregnancy(numSiblings, papa);
		          foreach (PregnancyData pregnancy in mama.womb)
		          {
				children.AddRange(pregnancy.GiveBirth(mama, true));
		          }
		          // Siblings with age differences
		          int s = 0;
		          foreach (PepoData sibling in children)
		          {
				sibling.age += baseAge * (s + 1);
				mama.offspring.Add(sibling);
				papa.offspring.Add(sibling);
				PepoLogUtilities.LogPepo(sibling);
				s++;
		          }

		          return children;
	     }
	     public static List<PepoData> CreateIdenticalTwins(PepoData mama, PepoData papa, int twinCount)
	     {
		          var children = new List<PepoData>();
		          var firstChild = CreateSingleChild(mama, papa, 0f);

		          children.Add(firstChild);
		          for (int i = 1; i <= twinCount; i++)
		          {
				PepoData twin = new PepoData(false, firstChild.age, firstChild.mass, firstChild.height, null, null, null, null);

				twin.mother = firstChild.mother;
				twin.father = firstChild.father;
				if (firstChild.ssd.hasY)
				{
					     twin.ssd.hasY = true;
				}
				else
				{
					     twin.ssd.hasY = false;
				}
				PepoNameUtilities.InitialiseForname(twin, PepoNameUtilities.GetPepoNames(children));
				children.Add(twin);
		          }

		          return children;
	     }
	     public static bool AreRelated(PepoData a, PepoData b, int degree = 5)
	     {
		          //ensure that there is no direct relation. if peep a is on peep b's list of children or vice versa, they're related.
		          if (degree >= 1 && (b.offspring.Contains(a) || a.offspring.Contains(b)))
		          {
				Debug.Log($"[Relationship Manager]{GetFullName(a)} and {GetFullName(b)} are very closely related");
				return true;
		          }
		          //check if they share parents
		          if (degree >= 2 && (HasSharedParent(a, b)))
		          {
				Debug.Log($"[Relationship Manager]{GetFullName(a)} and {GetFullName(b)} are very closely related");
				return true;
		          }
		          //check if we are aunts or uncles
		          if (degree >= 3 && (HasSharedParent(a.mother, b) || HasSharedParent(a.father, b) || HasSharedParent(a, b.mother) || HasSharedParent(a, b.father)))
		          {
				Debug.Log($"[Relationship Manager]{GetFullName(a)} and {GetFullName(b)} are closely related");
				return true;
		          }
		          //check if they share grandparents
		          if (degree >= 4 && (HasSharedParent(a.father, b.father) || HasSharedParent(a.mother, b.mother) || HasSharedParent(a.father, b.mother) || HasSharedParent(a.mother, b.father)))
		          {
				Debug.Log($"[Relationship Manager]{GetFullName(a)} and {GetFullName(b)} are related");
				return true;
		          }
		          //check if we share any cousins
		          if (degree >= 5 && (HasSharedCousins(a, b)))
		          {
				Debug.Log($"[Relationship Manager]{GetFullName(a)} and {GetFullName(b)} are indirectly related");
				return true;
		          }
		          //check is we share cousins with each others parents
		          if (degree >= 6 && (HasSharedCousins(a.mother, b) || HasSharedCousins(a.father, b) || HasSharedCousins(a, b.mother) || HasSharedCousins(a, b.father)))
		          {
				Debug.Log($"[Relationship Manager]{GetFullName(a)} and {GetFullName(b)} are distantly related");
				return true;
		          }
		          //check if our parents share cousins
		          if (degree >= 7 && (HasSharedCousins(a.father, b.father) || HasSharedCousins(a.mother, b.mother) || HasSharedCousins(a.father, b.mother) || HasSharedCousins(a.mother, b.father)))
		          {
				Debug.Log($"[Relationship Manager]{GetFullName(a)} and {GetFullName(b)} are very distantly related");
				return true;
		          }
		          //take your list of kin and my list of kin and see if anyone on it is  the same
		          return false;
	     }

	     public static bool HasSharedParent(PepoData parentA, PepoData parentB)
	     {
		          return parentA != null && parentB != null &&
			  (parentA.father == parentB.father || parentA.mother == parentB.mother);
	     }

	     public static bool HasSharedCousins(PepoData a, PepoData b)
	     {
		          foreach (var myCousin in PepoUtilities.FindBloodCousins(a))
		          {
				foreach (var theirCousin in PepoUtilities.FindBloodCousins(b))
				{
					     if (theirCousin != null && myCousin != null && myCousin == theirCousin)
					     {
						          return true;
					     }
				}
		          }
		          return false;
	     }
	     //take a pepo and a RelationshipType and return a list of Pepos that match that type
	     public static List<PepoData> FindFullSiblings(PepoData me)
	     {
		          List<PepoData> siblings = new List<PepoData>();
		          List<PepoData> mat = FindMaternalSiblings(me);
		          List<PepoData> pat = FindPaternalSiblings(me);
		          //iterate through my materrnal siblings, for eacch one check if that sibling is on my list of paternal siblings
		          foreach (PepoData child in mat)
		          {
				if (pat.Contains(child))
				{
					     siblings.Add(child);
				}
		          }
		          siblings.Remove(me);
		          return siblings;
	     }
	     public static List<PepoData> FindAllSiblings(PepoData me)
	     {
		          List<PepoData> siblings = new List<PepoData>();
		          List<PepoData> mat = FindMaternalSiblings(me);
		          List<PepoData> pat = FindPaternalSiblings(me);
		          //iterate through my materrnal siblings, for eacch one check if that sibling is on my list of paternal siblings
		          foreach (PepoData child in mat)
		          {
				if (pat.Contains(child))
				{
					     pat.Remove(child);
				}
		          }
		          siblings.AddRange(mat);
		          siblings.AddRange(pat);
		          siblings.Remove(me);
		          return siblings;
	     }
	     public static List<PepoData> FindMaternalSiblings(PepoData me)
	     {
		          List<PepoData> siblings = new List<PepoData>();
		          foreach (var child in me.mother.offspring)
		          {
				siblings.Add(child);
		          }
		          siblings.Remove(me);
		          return siblings;
	     }
	     public static List<PepoData> FindPaternalSiblings(PepoData me)
	     {
		          List<PepoData> siblings = new List<PepoData>();
		          foreach (var child in me.father.offspring)
		          {
				siblings.Add(child);
		          }
		          siblings.Remove(me);
		          return siblings;
	     }
	     public static List<PepoData> FindOffspring(List<PepoData> lilGuys)
	     {
		          List<PepoData> children = new List<PepoData>();
		          foreach (var parent in lilGuys)
		          {
				if (parent.offspring.Count <= 1)
				{
					     children.AddRange(parent.offspring);
				}
		          }
		          return children;
	     }
	     public static List<PepoData> FindBloodCousins(PepoData me)
	     {
		          List<PepoData> Cousins = new List<PepoData>();
		          List<PepoData> AuntsUncles = new List<PepoData>();
		          AuntsUncles.AddRange(FindMaternalSiblings(me.mother));
		          AuntsUncles.AddRange(FindPaternalSiblings(me.mother));
		          AuntsUncles.AddRange(FindMaternalSiblings(me.father));
		          AuntsUncles.AddRange(FindPaternalSiblings(me.father));
		          Cousins = FindOffspring(AuntsUncles);
		          return Cousins;
	     }
	     public static List<PepoData> FindFullCousins(PepoData me)
	     {
		          List<PepoData> Cousins = new List<PepoData>();
		          List<PepoData> AuntsUncles = new List<PepoData>();
		          AuntsUncles.AddRange(FindFullSiblings(me.mother));
		          AuntsUncles.AddRange(FindFullSiblings(me.father));
		          return Cousins;
	     }
	     public static bool AmIAttracted(PepoData me, PepoData target, int degree = 5)
	     {
		          if (target == null || me == null)
		          {
				Debug.LogWarning("No targets to check attraction for");
				return false;
		          }
		          //if 

		          {

		          }
		          if ((!me.ssd.likesFemme && (target.ssd.presentsFemme || target.ssd.isFemme)) && (!me.ssd.likesMasc && (target.ssd.presentsMasc || target.ssd.likesFemme)))
		          {
				Debug.LogWarning($"[Relationship Manager]{GetFullName(me)} doesn't like {GetFullName(target)}'s gender expression");
				return false;
		          }
		          //if i am monogomous i am not attracted to people who aren't monogomous, unlessss they are dishonest about it
		          if (me.ssd.isMonogamous && (target.lovers != 0 || (target.lovers != 0 && target.personality.Honesty.value > 5)))
		          {
				return false;
		          }
		          int iWant = me.ssd.wantsChildren;
		          int theyWant = target.ssd.wantsChildren;
		          if (iWant >= theyWant || (iWant > theyWant && target.personality.Honesty.value > 5))
		          {
				return false;
		          }
		          Debug.Log($"[Relationship Manager]{GetFullName(me)} is attracted to {GetFullName(target)}");
		          return true;
	     }
	     public static List<PepoData> FindSingles(List<PepoData> Peeps)
	     {
		          List<PepoData> pringles = new List<PepoData>();
		          foreach (PepoData p in Peeps)
		          {
				if (p.lovers == 0 || (!p.ssd.isMonogamous && p.personality.Honesty.value < 5))
				{
					     pringles.Add(p);
				}
		          }

		          return pringles;
	     }

	     //Death

	     public static PepoData RemoveAncestors()
	     {
		          return null;
	     }
}
