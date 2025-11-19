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
		          List<PepoData> lilGuys = pool;
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
				if (pepo == null || pepo.ssd.isPrepubescent || pepo.ssd.wantsChildren < 1)
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
}
