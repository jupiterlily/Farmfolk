using System.Collections.Generic;

public static class PepoUtilitiesPopulation
{//things that return a list of pepos happen here
	     public static List<PepoData> FindPeposInCommon(List<PepoData> lilGuys, List<PepoData> lilDudes)
	     {
		          List<PepoData> overlap = new List<PepoData>();
		          foreach (var pepo in lilGuys)
		          {
				foreach (var otherPepo in lilDudes)
				{
					     if (pepo != null && otherPepo != null && otherPepo == pepo)
					     {
						          overlap.Add(pepo);
					     }
				}
		          }
		          return overlap;
	     }
	     public static List<PepoData> RemoveDuplicatePepos(List<PepoData> lilGuys, List<PepoData> lilDudes)
	     {
		          List<PepoData> overlap = FindPeposInCommon(lilGuys, lilDudes);
		          List<PepoData> clean = new List<PepoData>();
		          foreach (var pepo in lilGuys)
		          {
				if (!overlap.Contains(pepo))
				{
					     clean.Add(pepo);
				}
		          }
		          foreach (var otherPepo in lilDudes)
		          {
				if (!overlap.Contains(otherPepo))
				{
					     clean.Add(otherPepo);
				}
		          }
		          return clean;
	     }
	     public static List<PepoData> RemovePeposFromList(List<PepoData> pool, List<PepoData> exiles)
	     {
		          List<PepoData> lilGuys = new List<PepoData>();
		          lilGuys.AddRange(pool);
		          foreach (var pepo in pool)
		          {
				if (exiles.Contains(pepo))
				{
					     lilGuys.Remove(pepo);
				}
		          }

		          return lilGuys;
	     }
	     // take alist of pepos and return the live ones
	     public static List<PepoData> GetAlivePepos(List<PepoData> pepos)
	     {
		          var lilGuys = new List<PepoData>();
		          foreach (var pepo in pepos)
		          {
				if (pepo == null || !pepo.isAlive)
					     continue;
				lilGuys.Add(pepo);
		          }
		          return lilGuys;
	     }
	     // take alist of pepos and return the live ones
	     public static List<PepoData> GetDeadPepos(List<PepoData> pepos)
	     {
		          var lilGuys = new List<PepoData>();
		          foreach (var pepo in pepos)
		          {
				if (pepo == null || pepo.isAlive)
					     continue;
				lilGuys.Add(pepo);
		          }
		          return lilGuys;
	     }
	     // take alist of pepos anre return all fertile, childbearing pepos
	     public static List<PepoData> GetChildbearingPepos(List<PepoData> pepos)
	     {
		          var lilGuys = new List<PepoData>();
		          foreach (var pepo in pepos)
		          {
				if (pepo == null || pepo.ssd.isPrepubescent)
					     continue;
				if (pepo.womb != null && pepo.ssd.isFertile)
				{
					     lilGuys.Add(pepo);
				}
		          }
		          return lilGuys;
	     }
	     // take alist of pepos anre return all fertile, childbearing pepos
	     public static List<PepoData> GetPregnantPepos(List<PepoData> pepos)
	     {
		          var lilGuys = new List<PepoData>();
		          foreach (var pepo in pepos)
		          {
				if (pepo == null || pepo.ssd.isPrepubescent)
					     continue;
				if (pepo.womb != null && pepo.womb.Count > 0)
				{
					     lilGuys.Add(pepo);
				}
		          }
		          return lilGuys;
	     }
	     //take a list of pepos and return all fertile pepos without wombs
	     public static List<PepoData> GetImpregnatingPepos(List<PepoData> pepos)
	     {
		          var lilGuys = new List<PepoData>();
		          foreach (var pepo in pepos)
		          {
				if (pepo == null || pepo.ssd.isPrepubescent)
					     continue;
				if (pepo.womb == null && pepo.ssd.isFertile)
				{
					     lilGuys.Add(pepo);
				}
		          }
		          return lilGuys;
	     }
	     //take a list of pepos and return the partnered pepos
	     public static List<PepoData> GetWillingParentPepos(List<PepoData> pepos)
	     {
		          var lilGuys = new List<PepoData>();
		          foreach (var pepo in pepos)
		          {
				if (pepo == null || pepo.ssd.isPrepubescent || pepo.ssd.wantsChildren[1] < 1)
					     continue;
				lilGuys.Add(pepo);
		          }
		          return lilGuys;
	     }
	     //take a list of pepos and return the partnered pepos
	     public static List<PepoData> GetPartneredPepos(List<PepoData> pepos)
	     {
		          var lilGuys = new List<PepoData>();
		          foreach (var pepo in pepos)
		          {
				if (pepo == null || pepo.ssd.isPrepubescent || pepo.RelationshipLibrary[RelationshipType.lover].Count < 1)
					     continue;
				lilGuys.Add(pepo);
		          }
		          return lilGuys;
	     }

	     //take a list of pepos and two floats and return a list of pepos that fall between those ages
	     public static List<PepoData> GetPeposAtAge(List<PepoData> dudes, float minAge = 0f, float maxAge = 0f)
	     {
		          var lilGuys = new List<PepoData>();
		          foreach (var pepo in dudes)
		          {
				if (pepo == null)
					     continue;
				if (pepo.age >= minAge && pepo.age <= maxAge)
				{
					     lilGuys.Add(pepo);
				}
		          }

		          return lilGuys;
	     }
	     public static List<PepoData> GetPeposAtHexaco(List<PepoData> dudes, int Hexaco, float minValue = 0f, float maxValue = 0f)
	     {
		          var lilGuys = new List<PepoData>();
		          foreach (var pepo in dudes)
		          {
				if (pepo == null || Hexaco >= 6)
					     continue;
				if (pepo.personality.FullHexRange()[Hexaco].value >= minValue && pepo.personality.FullHexRange()[Hexaco].value <= maxValue)
				{
					     lilGuys.Add(pepo);
				}
		          }

		          return lilGuys;
	     }
	     public static List<PepoData> OrderByHexaco(List<PepoData> Peeps, int Hexaco)
	     {
		          List<PepoData> lilGuys = new List<PepoData>();
		          for (int i = 5; i >= 0; i--)
		          {
				lilGuys.AddRange(GetPeposAtHexaco(Peeps, Hexaco, i - 1, i));
		          }
		          return lilGuys;
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
	     public static bool HasSharedParent(PepoData parentA, PepoData parentB)
	     {
		          return parentA != null && parentB != null &&
			  (parentA.father == parentB.father || parentA.mother == parentB.mother);
	     }

	     public static bool HasSharedCousins(PepoData a, PepoData b)
	     {
		          foreach (var myCousin in PepoUtilitiesPopulation.FindBloodCousins(a))
		          {
				foreach (var theirCousin in PepoUtilitiesPopulation.FindBloodCousins(b))
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

	     public static List<PepoData> OthersInMyRoles(PepoData me, PepoPopulation population)
	     {
		          List<PepoData> peers = new List<PepoData>();
		          //go through the roles assignes to this population
		          foreach (var p in population.roleAssignments)
		          {
				//if the list of people doing the job isn't empty
				if (p.Value != null)
				{
					     //if iam on the list
					     if (p.Value.Contains(me))
					     {
						          //add my coworker to my list of peers if they aren't already on the list
						          foreach (var pepo in p.Value)
						          {
								if (!peers.Contains(pepo))
								{
									     peers.Add(pepo);
								}
						          }
					     }
				}
		          }
		          peers.Remove(me);
		          return peers;
	     }

	     public static float PairLovers(List<PepoData> singles, PepoPopulation pop, int attempts = 5)
	     {
		          bool match = false;
		          int i = 0;
		          int f = 0; //failed to find a match.
		          foreach (var single in singles)
		          {
				float attemptsTotal = attempts + single.personality.Conscientiousness.value + single.personality.Extraversion.value;
				if ((single.lovers == 0 || !single.ssd.isMonogamous) && single.womb != null)
				{
					     List<PepoData> pringles = PepoUtilitiesBonds.ShipsToPepos(single.RelationshipLibrary[RelationshipType.courting]);
					     pringles.AddRange(PepoUtilitiesBonds.ShipsToPepos(single.RelationshipLibrary[RelationshipType.friend]));
					     pringles.AddRange(PepoUtilitiesBonds.ShipsToPepos(single.RelationshipLibrary[RelationshipType.peer]));
					     pringles.AddRange(PepoUtilitiesBonds.ShipsToPepos(single.RelationshipLibrary[RelationshipType.acquaintence]));
					     pringles.AddRange(PepoUtilitiesBonds.ShipsToPepos(single.RelationshipLibrary[RelationshipType.follower]));
					     pringles.Remove(single);
					     foreach (var pringle in pringles)
					     {
						          if (!PepoUtilitiesBonds.ShipsToPepos(single.RelationshipLibrary[RelationshipType.lover]).Contains(pringle) && !PepoUtilitiesBonds.ShipsToPepos(single.RelationshipLibrary[RelationshipType.spouse]).Contains(pringle) && !PepoUtilitiesBonds.ShipsToPepos(single.RelationshipLibrary[RelationshipType.offspring]).Contains(pringle) && !PepoUtilitiesBonds.ShipsToPepos(single.RelationshipLibrary[RelationshipType.parent]).Contains(pringle) && !PepoUtilitiesBonds.ShipsToPepos(single.RelationshipLibrary[RelationshipType.kin]).Contains(pringle) && !PepoUtilitiesBonds.ShipsToPepos(single.RelationshipLibrary[RelationshipType.incompatible]).Contains(pringle) && i <= attemptsTotal)
						          {//check if we're related by an int of our average conscientousness scores
								float c = (single.personality.Conscientiousness.value + pringle.personality.Conscientiousness.value) / 2;
								if (pringle.isAlive && !PepoSystemMatchmaking.AreRelated(single, pringle, ((int)c)))
								{

									     if (PepoSystemMatchmaking.AmIAttracted(single, pringle, (int)single.personality.Conscientiousness.value))
									     {
										          //if his agreeableness is below 7 we check if he likes me back
										          RelationshipType type = RelationshipType.lover;
										          //if we both have a security value above 8 and we don't have a spouse already we make us spouses
										          if (single.values[5].value >= 7 && pringle.values[5].value >= 7 && single.RelationshipLibrary[RelationshipType.spouse].Count == 0 && pringle.RelationshipLibrary[RelationshipType.spouse].Count == 0)
										          {
												type = RelationshipType.spouse;
										          }
										          {
												if (pringle.personality.Agreeableness.value < 8)
												{
													     if (PepoSystemMatchmaking.AmIAttracted(pringle, single, (int)pringle.personality.Conscientiousness.value))
													     {
														          PepoFactoryBonds.MutualRelationship(single, pringle, type);
														          match = true;
														          break;
													     }
												}
												else
												{
													     //he likes me back automatically
													     PepoFactoryBonds.MutualRelationship(single, pringle, type);
													     match = true;
													     break;
												}
										          }

									     }
									     else
									     {
										          //add him to my list of kin
										          PepoFactoryBonds.MutualRelationship(single, pringle, RelationshipType.kin);
										          i++;
									     }
								}
						          }
						          if (!match)
						          {
								f++;
						          }
					     }
				}
				i = 0;
		          }
		          return f;
	     }
}
