using System.Collections.Generic;
using System.Linq;

public static class PepoUtilitiesValues
{
	     /// <summary>
	     /// Ranks a list of values by their current value in descending order
	     /// </summary>
	     /// <param name="values">List of values to rank</param>
	     /// <returns>List of values sorted by importance (highest to lowest)</returns>
	     public static List<Value> RankValuesByImportance(List<Value> values)
	     {
		          if (values == null || values.Count == 0)
				return new List<Value>();

		          return values.OrderByDescending(v => v.value)
			       .ThenBy(v => v.name) // Secondary sort by name for consistency
			       .ToList();
	     }
	     /// <summary>
	     /// Groups values into tiers based on their strength
	     /// </summary>
	     public static string GetValueTier(float currentValue, float maxValue)
	     {
		          float normalized = currentValue / maxValue;

		          if (normalized >= 0.8f)
				return "Core";
		          if (normalized >= 0.6f)
				return "Important";
		          if (normalized >= 0.4f)
				return "Moderate";
		          if (normalized >= 0.2f)
				return "Minor";
		          return "Negligible";
	     }

	     /// <summary>
	     /// Gets the top N most important values
	     /// </summary>
	     public static List<Value> GetTopValues(List<Value> values, int count)
	     {
		          return RankValuesByImportance(values).Take(count).ToList();
	     }

	     /// <summary>
	     /// Gets the dominant value category (Openness, Self-enhancement, Conservation, Self-transcendence)
	     /// </summary>
	     public static string GetDominantValueCategory(List<Value> values)
	     {
		          if (values == null || values.Count == 0)
				return "Unknown";

		          var categoryScores = new Dictionary<string, float>
        {
            { "Openness", values.Where(v => IsOpennessValue(v)).Sum(v => v.value) },
            { "Self-enhancement", values.Where(v => IsSelfEnhancementValue(v)).Sum(v => v.value) },
            { "Conservation", values.Where(v => IsConservationValue(v)).Sum(v => v.value) },
            { "Self-transcendence", values.Where(v => IsSelfTranscendenceValue(v)).Sum(v => v.value) }
        };

		          return categoryScores.OrderByDescending(kv => kv.Value).First().Key;
	     }

	     // Helper methods to categorize values
	     public static bool IsOpennessValue(Value value)
	     {
		          return value.name == "Agency" || value.name == "Stimulation";
	     }

	     public static bool IsSelfEnhancementValue(Value value)
	     {
		          return value.name == "Hedonism" || value.name == "Achievement" || value.name == "Power";
	     }

	     public static bool IsConservationValue(Value value)
	     {
		          return value.name == "Security" || value.name == "Conformity" || value.name == "Tradition";
	     }

	     public static bool IsSelfTranscendenceValue(Value value)
	     {
		          return value.name == "Benevolence" || value.name == "Universalism";
	     }
}


