using System;
using System.Collections.Generic;

public static class PepoFactoryNames
{
	     //take a list of names, foreach name, split it into all possible combinations of stem and suffix and add them to a dictionary of stems and suffix lists and then adictionary for stems nad suffixes and the amount of times they appear.. then we can ignore/remove all the stems and suffixes that only show up once from the stem/suffix dictionary. returns  two dictionaries. one of stems and suffixes, another of suffixes and stems

	     //use name library to generate possible name mutation rules
	     //x=2
	     //go through the list of names and only count names that are longer than x+1

	     //split off the last x characters

	     //get all the other strings that start with the same stem.. discard the unique ones
	     //get all the other strings that end with the same suffix,

	     //note down the remaining stems as a root name.
	     //note down all the suffixes that are 2, 3 or 4 characters in length

	     [Serializable]
	     public class NameMutationRules
	     {
		          //we create master rules and then cultures have their own subset of rules that pepos utilise

		          public List<string> rootNames = new List<string>(); // a root name is
		          public List<string> commonStems = new List<string>();
		          public Dictionary<string, List<string>> stemToSuffixes = new Dictionary<string, List<string>>(); //what stems can 
		          public Dictionary<string, List<string>> suffixToStems = new Dictionary<string, List<string>>();
		          public Dictionary<string, List<string>> femmeSuffix = new Dictionary<string, List<string>>(); //show how feminine suffixes can change
		          public Dictionary<string, List<string>> mascSuffix = new Dictionary<string, List<string>>();
		          public Dictionary<string, List<string>> unisexSuffix = new Dictionary<string, List<string>>();
		          //public List<string> suffixes2 = new List<string>();
		          //public List<string> suffixes3 = new List<string>();
		          //public List<string> suffixes4 = new List<string>();
	     }

	     // Main method to generate name mutation rules
	     public static NameMutationRules GenerateMutationRules(List<string> nameList, int x = 2)
	     {
		          var rules = new NameMutationRules();

		          // Filter names longer than x+1
		          var eligibleNames = PepoUtilitiesNames.GetStringsInLengthRange(nameList, x + 2, int.MaxValue);

		          // Process each eligible name
		          foreach (var name in eligibleNames)
		          {
				if (string.IsNullOrEmpty(name) || name.Length <= x)
					     continue;

				string stem = name.Substring(0, name.Length - x);
				string suffix = name.Substring(name.Length - x);

				// Add to stem dictionary if  
				if (!rules.stemToSuffixes.ContainsKey(stem))
					     rules.stemToSuffixes[stem] = new List<string>();
				rules.stemToSuffixes[stem].Add(suffix);

				// Add to suffix dictionary
				if (!rules.suffixToStems.ContainsKey(suffix))
					     rules.suffixToStems[suffix] = new List<string>();
				rules.suffixToStems[suffix].Add(stem);

		          }

		          // Identify common stems (stems that appear with multiple suffixes)
		          foreach (var kvp in rules.stemToSuffixes)
		          {
				if (kvp.Value.Count > 1)
				{
					     rules.commonStems.Add(kvp.Key);
				}
		          }

		          // Identify root names (unique stems that aren't common)
		          foreach (var kvp in rules.stemToSuffixes)
		          {
				if (kvp.Value.Count == 1)
				{
					     rules.rootNames.Add(kvp.Key);
				}
		          }

		          return rules;
	     }

	     // Generate a new name using mutation rules
	     public static string GenerateNewName(NameMutationRules rules, bool useCommonStems = true)
	     {
		          if (rules == null)
				return string.Empty;

		          string newName = string.Empty;

		          // 50% chance to combine existing stem with random suffix
		          // 50% chance to combine existing suffix with random stem
		          if (UnityEngine.Random.value > 0.5f && rules.commonStems.Count > 0)
		          {
				// Pick a random stem
				string randomStem = rules.commonStems[UnityEngine.Random.Range(0, rules.commonStems.Count)];

				// Get all possible suffixes for this stem
				if (rules.stemToSuffixes.TryGetValue(randomStem, out var possibleSuffixes) && possibleSuffixes.Count > 0)
				{
					     // Pick a random suffix (could be from a different stem for variety)
					     string randomSuffix = possibleSuffixes[UnityEngine.Random.Range(0, possibleSuffixes.Count)];
					     newName = randomStem + randomSuffix;
				}
		          }


		          // Fallback if no name generated
		          if (string.IsNullOrEmpty(newName) && rules.rootNames.Count > 0)
		          {
				newName = rules.rootNames[UnityEngine.Random.Range(0, rules.rootNames.Count)];
		          }

		          return newName;
	     }

	     // Generate multiple new names
	     public static List<string> GenerateNewNames(NameMutationRules rules, int count)
	     {
		          var names = new List<string>();
		          var generatedNames = new HashSet<string>();

		          for (int i = 0; i < count; i++)
		          {
				string newName;
				int attempts = 0;

				// Try to generate a unique name
				do
				{
					     newName = GenerateNewName(rules);
					     attempts++;
				}
				while (generatedNames.Contains(newName) && attempts < 10);

				if (!string.IsNullOrEmpty(newName))
				{
					     names.Add(newName);
					     generatedNames.Add(newName);
				}
		          }

		          return names;
	     }

	     // Analyze name patterns in a list
	     public static Dictionary<string, PatternInfo> AnalyzeNamePatterns(List<string> names, int maxSuffixLength = 4)
	     {
		          var patterns = new Dictionary<string, PatternInfo>();

		          foreach (var name in names)
		          {
				if (string.IsNullOrEmpty(name))
					     continue;

				for (int suffixLength = 1; suffixLength <= Math.Min(maxSuffixLength, name.Length - 1); suffixLength++)
				{
					     string stem = name.Substring(0, name.Length - suffixLength);
					     string suffix = name.Substring(name.Length - suffixLength);

					     string patternKey = $"{stem.Length}_{suffixLength}";

					     if (!patterns.ContainsKey(patternKey))
						          patterns[patternKey] = new PatternInfo();

					     patterns[patternKey].AddExample(stem, suffix);
				}
		          }

		          return patterns;
	     }

	     [Serializable]
	     public class PatternInfo
	     {
		          public int stemLength;
		          public int suffixLength;
		          public List<string> stems = new List<string>();
		          public List<string> suffixes = new List<string>();

		          public void AddExample(string stem, string suffix)
		          {
				if (!stems.Contains(stem))
					     stems.Add(stem);
				if (!suffixes.Contains(suffix))
					     suffixes.Add(suffix);

				stemLength = stem.Length;
				suffixLength = suffix.Length;
		          }
	     }
}