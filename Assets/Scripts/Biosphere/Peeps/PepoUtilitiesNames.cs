using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class PepoUtilitiesNames
{
	     public static string InitialiseForname(PepoData lilGuy, List<string> exclude = null)
	     {
		          string forename = null;
		          List<string> names = new List<string>();
		          if (lilGuy.ssd.presentsMasc)
		          {
				names.AddRange(PepoLibraryNames.maleNames);
		          }
		          if (lilGuy.ssd.presentsFemme)
		          {
				names.AddRange(PepoLibraryNames.femaleNames);
		          }
		          names.AddRange(PepoLibraryNames.unisexNames);
		          if (exclude != null)
		          {
				names = FindUniqueNames(exclude, names);
		          }
		          forename = names[UnityEngine.Random.Range(0, names.Count)];

		          lilGuy.pepoName[0].value = forename;

		          return forename;

	     }
	     public static string InitialiseSurname(List<string> exclude = null)
	     {
		          List<string> names = new List<string>();
		          if (exclude != null)
		          {
				names = FindUniqueNames(PepoLibraryNames.surnames, exclude);
		          }
		          else
		          {
				names = PepoLibraryNames.surnames;
		          }
		          return names[UnityEngine.Random.Range(0, names.Count)];
	     }

	     //take a list of names and ensure that my name is not one of those names.
	     //  public static List<string> FindUniqueNames(List<string> exclude, List<string> names)
	     //  {
	     //        //pick a name from the namePool that isn't on the list of excluded names
	     //        List<string> longer = new List<string>();
	     //        List<string> shorter = new List<string>();
	     //        if (names.Count >= exclude.Count)
	     //        {
	     //shorter.AddRange(exclude);
	     //longer.AddRange(names);
	     //        }
	     //        else
	     //        {
	     //longer.AddRange(names);
	     //shorter.AddRange(exclude);
	     //        }
	     //        foreach (string name in longer)
	     //        {
	     //if (shorter.Contains(name))
	     //{
	     //	     names.Remove(name);
	     //}
	     //        }
	     //        return names;
	     //  }
	     public static List<string> FindUniqueNames(List<string> exclude, List<string> names)
	     {
		          if (exclude == null || exclude.Count == 0)
				return new List<string>(names);

		          // Use HashSet for O(1) lookups
		          HashSet<string> excludeSet = new HashSet<string>(exclude);
		          List<string> result = new List<string>();

		          foreach (string name in names)
		          {
				if (!excludeSet.Contains(name))
				{
					     result.Add(name);
				}
		          }

		          return result;
	     }
	     public static List<string> GetPepoNames(List<PepoData> pepos, bool surnames = false)
	     {
		          List<string> names = new List<string>();
		          if (!surnames)
		          {

				foreach (PepoData pepo in pepos)
				{
					     names.Add(pepo.forename);
				}
		          }
		          else
		          {
				foreach (PepoData pepo in pepos)
				{
					     names.Add(pepo.surname);
				}

		          }
		          return names;
	     }
	     public static void GiveFamilyName(string familyName, List<PepoData> lilGuys)
	     {
		          foreach (var lilGuy in lilGuys)
		          {
				lilGuy.pepoName[1].value = familyName;
		          }
	     }

	     public static List<string> PepoNamesToString(List<PepoName> nameData)
	     {
		          List<string> names = new List<string>();
		          if (nameData != null)
		          {
				foreach (PepoName name in nameData)
				{
					     // FIXED: Accessing the name value correctly
					     if (name != null && name.Name != null && name.Name.Length > 0)
					     {
						          // Get the first name part that has a value
						          var namePart = name.Name.FirstOrDefault(n => n != null && !string.IsNullOrEmpty(n.name.value));
						          if (namePart != null)
						          {
								names.Add(namePart.name.value);
						          }
					     }
				}
		          }
		          return names;
	     }
	     // 1. Get strings of a given length
	     public static List<string> GetStringsOfLength(List<string> strings, int length)
	     {
		          List<string> result = new List<string>();
		          foreach (string s in strings)
		          {
				if (s != null && s.Length == length)
				{
					     result.Add(s);
				}
		          }
		          return result;
	     }

	     // 2. Get strings that appear more than once (duplicates)
	     public static List<string> GetDuplicateStrings(List<string> strings)
	     {
		          List<string> duplicates = new List<string>();
		          Dictionary<string, int> frequency = new Dictionary<string, int>();
		          Dictionary<string, bool> alreadyAdded = new Dictionary<string, bool>();

		          foreach (string s in strings)
		          {
				if (s != null)
				{
					     if (frequency.ContainsKey(s))
					     {
						          frequency[s]++;
						          // Add to duplicates only once
						          if (frequency[s] == 2 && !alreadyAdded.ContainsKey(s))
						          {
								duplicates.Add(s);
								alreadyAdded[s] = true;
						          }
					     }
					     else
					     {
						          frequency[s] = 1;
					     }
				}
		          }

		          return duplicates;
	     }

	     // 3. Get substring option - first (length - n) or last n characters
	     public static string GetSubstringOption(string input, int n, bool getLastCharacters = false)
	     {
		          if (string.IsNullOrEmpty(input))
				return string.Empty;

		          if (n < 0 || n > input.Length)
				return input;

		          if (getLastCharacters)
		          {
				// Return last n characters
				return input.Substring(input.Length - n);
		          }
		          else
		          {
				// Return first (length - n) characters (the stem)
				return input.Substring(0, input.Length - n);
		          }
	     }

	     // 4. Get strings ending with a given suffix
	     public static List<string> GetStringsEndingWith(List<string> strings, string suffix,
				        StringComparison comparison = StringComparison.Ordinal)
	     {
		          List<string> result = new List<string>();

		          if (string.IsNullOrEmpty(suffix))
				return strings;

		          foreach (string s in strings)
		          {
				if (s != null && s.EndsWith(suffix, comparison))
				{
					     result.Add(s);
				}
		          }
		          return result;
	     }

	     // 5. Get strings beginning with a given prefix
	     public static List<string> GetStringsStartingWith(List<string> strings, string prefix,
				          StringComparison comparison = StringComparison.Ordinal)
	     {
		          List<string> result = new List<string>();

		          if (string.IsNullOrEmpty(prefix))
				return strings;

		          foreach (string s in strings)
		          {
				if (s != null && s.StartsWith(prefix, comparison))
				{
					     result.Add(s);
				}
		          }
		          return result;
	     }

	     // 6. Additional useful utility: Get strings containing substring
	     public static List<string> GetStringsContaining(List<string> strings, string substring,
				        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
	     {
		          List<string> result = new List<string>();

		          if (string.IsNullOrEmpty(substring))
				return strings;

		          foreach (string s in strings)
		          {
				if (s != null && s.IndexOf(substring, comparison) >= 0)
				{
					     result.Add(s);
				}
		          }
		          return result;
	     }

	     // 7. Get strings with length in a range
	     public static List<string> GetStringsInLengthRange(List<string> strings, int minLength, int maxLength)
	     {
		          List<string> result = new List<string>();

		          if (minLength > maxLength)
				throw new ArgumentException("minLength cannot be greater than maxLength");

		          foreach (string s in strings)
		          {
				if (s != null && s.Length >= minLength && s.Length <= maxLength)
				{
					     result.Add(s);
				}
		          }
		          return result;
	     }

	     // 8. Get unique strings (remove duplicates)
	     public static List<string> GetUniqueStrings(List<string> strings)
	     {
		          HashSet<string> uniqueSet = new HashSet<string>();
		          List<string> result = new List<string>();

		          foreach (string s in strings)
		          {
				if (s != null && uniqueSet.Add(s))
				{
					     result.Add(s);
				}
		          }
		          return result;
	     }

	     // 9. Sort strings by length
	     public static List<string> SortByLength(List<string> strings, bool ascending = true)
	     {
		          List<string> sorted = strings.Where(s => s != null).ToList();

		          if (ascending)
		          {
				sorted.Sort((a, b) => a.Length.CompareTo(b.Length));
		          }
		          else
		          {
				sorted.Sort((a, b) => b.Length.CompareTo(a.Length));
		          }

		          return sorted;
	     }

	     // 10. Group strings by length
	     public static Dictionary<int, List<string>> GroupByLength(List<string> strings)
	     {
		          Dictionary<int, List<string>> groups = new Dictionary<int, List<string>>();

		          foreach (string s in strings)
		          {
				if (s != null)
				{
					     int length = s.Length;
					     if (!groups.ContainsKey(length))
					     {
						          groups[length] = new List<string>();
					     }
					     groups[length].Add(s);
				}
		          }

		          return groups;
	     }

	     // 11. Get strings matching a pattern (using simple wildcard *)
	     public static List<string> GetStringsMatchingPattern(List<string> strings, string pattern)
	     {
		          List<string> result = new List<string>();

		          if (string.IsNullOrEmpty(pattern))
				return strings;

		          // Convert simple wildcard pattern to regex
		          string regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";

		          foreach (string s in strings)
		          {
				if (s != null && Regex.IsMatch(s, regexPattern))
				{
					     result.Add(s);
				}
		          }
		          return result;
	     }
}



