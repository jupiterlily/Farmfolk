using System.Collections.Generic;
using UnityEngine;

public static class PepoFactoryPopulation
{

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
								PepoFactoryBonds.CreateSiblingBond(child, sib, "twin");

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
				children.Add(PepoFactory.InitialiseFromParents(mama, papa));
		          }
		          foreach (PepoData child in children)
		          {
				child.isAlive = true;
				mama.offspring.Add(child);
				papa.offspring.Add(child);
				child.mother = mama;
				child.father = papa;
				PepoFactoryBonds.CreateParentalBond(mama, child);
				PepoLogUtilities.LogPepo(child);
				PepoUtilitiesNames.InitialiseForname(child, PepoUtilitiesNames.GetPepoNames(mama.offspring));
		          }
		          return children;
	     }
	     public static PepoData CreateSingleChild(PepoData mama, PepoData papa, float agingIncrement)
	     {
		          PepoData child;
		          child = PepoFactory.InitialiseFromParents(mama, papa);
		          child.age = Random.Range(0f, agingIncrement * 0.8f);
		          return child;
	     }

	     public static List<PepoData> CreateSiblings(PepoData mama, PepoData papa, int numSiblings, float agingIncrement)
	     {
		          var children = new List<PepoData>();
		          float baseAge = Random.Range(0f, (agingIncrement * 0.8f) / numSiblings);
		          List<string> names = new List<string>();
		          for (int s = 0; s < numSiblings; s++)
		          {
				PepoData child = PepoFactory.InitialiseFromParents(mama, papa);
				PepoUtilitiesNames.InitialiseForname(child, names);
				child.age += baseAge * (s + 1);
				children.Add(child);
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
				PepoData twin = PepoFactory.ClonePepo(firstChild, true);


				if (firstChild.body.physicalAttributes["SexChromosome"].descriptor == "XY")

					     PepoUtilitiesNames.InitialiseForname(twin, PepoUtilitiesNames.GetPepoNames(children));
				children.Add(twin);
		          }

		          return children;
	     }
}
