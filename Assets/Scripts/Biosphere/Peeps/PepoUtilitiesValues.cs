using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
		          return value.name == "tolerance" || value.name == "curiosity" || value.name == "autonomy" || value.name == "excitement";
	     }

	     public static bool IsSelfEnhancementValue(Value value)
	     {
		          return value.name == "vitality" || value.name == "indulgence" || value.name == "accomplishment" || value.name == "dominance" || value.name == "resources" || value.name == "status";
	     }

	     public static bool IsConservationValue(Value value)
	     {
		          return value.name == "resources" || value.name == "status" || value.name == "safety" || value.name == "security" || value.name == "tradition" || value.name == "obedience" || value.name == "cohesion";
	     }

	     public static bool IsSelfTranscendenceValue(Value value)
	     {
		          return value.name == "humility" || value.name == "preservation" || value.name == "compassion" || value.name == "tolerance" || value.name == "caring" || value.name == "dependability";
	     }

	     /// <summary>
	     /// Compares two lists of values and returns a normalized compatibility score (0..1).
	     /// The comparison focuses on top-N values, penalises direct oppositions, and rewards shared dominant category.
	     /// </summary>
	     /// <param name="a">First pe person's values</param>
	     /// <param name="b">Second pe person's values</param>
	     /// <param name="topN">How many top values to compare (defaults to 3)</param>
	     public static float CompareValuesCompatibility(List<Value> a, List<Value> b, int topN = 3)
	     {
		          if (a == null || b == null)
				return 0.5f;

		          var ra = RankValuesByImportance(a);
		          var rb = RankValuesByImportance(b);

		          if (ra.Count == 0 || rb.Count == 0)
				return 0.5f;

		          // Quick deal-breaker: if each other's top value directly opposes the other's top value, return 0
		          if (ra.Count > 0 && rb.Count > 0 && AreValuesOpposing(ra[0], rb[0]))
				return 0f;

		          // Compare top-N with partial credit and small penalty for opposites
		          int considered = Mathf.Min(topN, Mathf.Min(ra.Count, rb.Count));
		          float matchScore = 0f;
		          float maxPositive = considered * 1f;   // each exact match gives +1
		          float maxNegative = considered * 0.5f; // each direct opposite gives -0.5

		          for (int i = 0; i < considered; i++)
		          {
				var vA = ra[i];
				bool matched = false;
				for (int j = 0; j < considered; j++)
				{
					     var vB = rb[j];
					     if (vA.descriptor == vB.descriptor)
					     {
						          matchScore += 1f; // exact match
						          matched = true;
						          break;
					     }
					     // if not exact match, check direct opposition
					     if (AreValuesOpposing(vA, vB))
					     {
						          matchScore -= 0.5f; // mild penalty for direct opposition
						          matched = true; // considered
						          break;
					     }
				}
				if (!matched)
				{
					     // small fuzzy credit for similar categories (e.g., both in Self-transcendence)
					     var catA = GetDominantValueCategory(new List<Value> { vA });
					     var catB = GetDominantValueCategory(rb.Take(considered).ToList());
					     if (catA == catB)
						          matchScore += 0.25f;
				}
		          }

		          // Normalize top-match score into 0..1
		          float min = -maxNegative;
		          float max = maxPositive;
		          float topNormalized = Mathf.Clamp01((matchScore - min) / (max - min));

		          // Dominant category alignment (coarse but quick)
		          float categoryScore = GetDominantValueCategory(a) == GetDominantValueCategory(b) ? 1f : 0.5f;

		          // Combine with weights (tuneable)
		          const float topWeight = 0.7f;
		          const float categoryWeight = 0.3f;

		          float overall = topNormalized * topWeight + categoryScore * categoryWeight;
		          return Mathf.Clamp01(overall);
	     }

	     // Helper to detect direct opposition between two values
	     private static bool AreValuesOpposing(Value a, Value b)
	     {
		          if (a == null || b == null)
				return false;
		          // If one value's explicit opposite matches the other's descriptor, treat as opposition
		          if (!string.IsNullOrEmpty(a.opposite) && a.opposite == b.descriptor)
				return true;
		          if (!string.IsNullOrEmpty(b.opposite) && b.opposite == a.descriptor)
				return true;
		          return false;
	     }
}


