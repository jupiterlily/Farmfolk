using System.Collections.Generic;
using UnityEngine;

public static class PepoNameUtilities
{
	     public static string InitialiseForname(PepoData lilGuy, List<string> exclude = null)
	     {
		          string forename = null;
		          List<string> names = new List<string>();
		          if (lilGuy.ssd.presentsMasc)
		          {
				names.AddRange(PepoDataNames.maleNames);
		          }
		          if (lilGuy.ssd.presentsFemme)
		          {
				names.AddRange(PepoDataNames.femaleNames);
		          }
		          names.AddRange(PepoDataNames.unisexNames);
		          if (exclude != null)
		          {
				names = FindUniqueNames(exclude, names);
		          }
		          forename = names[Random.Range(0, names.Count)];

		          lilGuy.forename = forename;

		          return forename;

	     }
	     public static string InitialiseSurname(List<string> exclude = null)
	     {
		          List<string> names = new List<string>();
		          if (exclude != null)
		          {
				names = FindUniqueNames(PepoDataNames.surnames, exclude);
		          }
		          else
		          {
				names = PepoDataNames.surnames;
		          }
		          return names[Random.Range(0, names.Count)];
	     }

	     //take a list of names and ensure that my name is not one of those names.
	     public static List<string> FindUniqueNames(List<string> exclude, List<string> names)
	     {
		          //pick a name from the namePool that isn't on the list of excluded names
		          List<string> longer = new List<string>();
		          List<string> shorter = new List<string>();
		          if (names.Count >= exclude.Count)
		          {
				shorter.AddRange(exclude);
				longer.AddRange(names);
		          }
		          else
		          {
				longer.AddRange(names);
				shorter.AddRange(exclude);
		          }
		          foreach (string name in longer)
		          {
				if (shorter.Contains(name))
				{
					     names.Remove(name);
				}
		          }
		          return names;
	     }

	     public static List<string> GetPepoNames(List<PepoData> pepos)
	     {
		          List<string> names = new List<string>();
		          foreach (PepoData pepo in pepos)
		          {
				names.Add(pepo.forename);
		          }
		          return names;
	     }
}
