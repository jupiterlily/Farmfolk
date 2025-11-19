
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class PepoUtilitiesPersonality
{
	     //is liar?
	     public static bool IsLiar(PepoData dude)
	     {                   //if trust is super high, i won't lie
		          if (dude.mood.Trust.value >= 9)
		          {
				return false;
		          }
		          //if i have high honest i'm not a liar
		          if (dude.personality.Honesty.value >= 7)
		          {
				return false;
		          }
		          //if hedonism, power, conformity, negative then i won't lie
		          if (dude.values[2].value < 0 || dude.values[4].value < 0 || dude.values[6].value < 0)
		          {
				//if i value sacrifice, authenticity or solidarity i am not a liar
				return false;
		          }
		          return true;
	     }
	     //is lying?
	     public static bool IsLying(PepoData dude)
	     {
		          if (!IsLiar(dude))
		          {
				return false;
		          }
		          float lieChance = 0;
		          float lieRoll = Random.value;

		          float honesty = dude.personality.Honesty.value / 10;
		          float agreeable = dude.personality.Agreeableness.value / 10;

		          lieChance = ((honesty + agreeable) / 2) - 1 * -1f;

		          float valueModifier = 0;
		          //make it more likely to lie
		          float agency = (dude.values[0].value * (1 / 0.7f)) / 10;
		          float stimulation = (dude.values[1].value * (1 / 0.7f)) / 10;
		          float hedonism = (dude.values[2].value * (1 / 0.7f)) / 10;
		          float achievement = (dude.values[3].value * (1 / 0.7f)) / 10;
		          float power = (dude.values[4].value * (1 / 0.7f)) / 10;
		          //conditional
		          //only if openness is below midway
		          float closed = honesty;
		          if (dude.personality.Openness.value < 5)
		          {
				closed = (dude.personality.Openness.value - 10) * -1f;
		          }
		          float exploitation = 0;
		          float authenticity = 0; // use stimulation , self direction and hedonism.
				    //only if value is negative
		          if (dude.values[8].value < 0)
		          {
				exploitation = (power + achievement) / 2f; //use power and achievement
		          }
		          if (dude.values[6].value < 0)
		          {
				authenticity = (power + achievement) / 2f; //use power and achievement
		          }
		          valueModifier = (agency + stimulation + hedonism + achievement + power + closed + exploitation + authenticity) / 8;
		          //malke it less likely to lie
		          float mood = dude.mood.Trust.value / 10;
		          lieChance = (lieChance + valueModifier - mood) / 3;
		          //lie chance is the average of honesty(agreeablness, mood and value)
		          if (lieRoll < lieChance)
		          {
				Debug.LogWarning($"[PersonalityManager]{dude.fullname} is a filthy fucking liar");
				return true;
		          }
		          return false;
	     }

	     public static string GetFeelingName(Mood mood)
	     {
		          if (mood == null)
				return "Neutral";

		          // Get all emotions and their values
		          EmotionAspect[] emotions = mood.FullHexRange();

		          // Filter emotions that are above a minimum threshold (e.g., 3.0)
		          var significantEmotions = emotions
		              .Where(e => e != null && e.value >= 2f)
		              .OrderByDescending(e => e.value)
		              .ToList();

		          if (significantEmotions.Count == 0)
				return "Neutral";

		          // Get the top emotion
		          EmotionAspect topEmotion = significantEmotions[0];
		          float topValue = topEmotion.value;

		          var matchingEmotions = significantEmotions
		              .Where(e => Mathf.Abs(topValue - e.value) <= e.value / 3)
		              .Take(3) // Limit to top 3 by value
		              .Where(e => !string.IsNullOrEmpty(e.name)) // Filter out null/empty names
		              .OrderBy(e => e.name) // Sort alphabetically for consistent lookup
		              .ToList();

		          // Build the emotion combination key
		          string combinationKey = string.Join("-", matchingEmotions.Select(e => e.name));


		          // Look up in the feeling library
		          if (!string.IsNullOrEmpty(combinationKey) && PepoLibraryPersonality.Feelings.TryGetValue(combinationKey, out string feelingName))
		          {
				return feelingName;
		          }

		          // Try alternative combinations if exact match not found
		          string fallbackResult = GetFallbackFeeling(matchingEmotions, topEmotion);
		          return fallbackResult;
	     }

	     private static string GetFallbackFeeling(List<EmotionAspect> matchingEmotions, EmotionAspect topEmotion)
	     {
		          // Try with just the top 2 emotions
		          if (matchingEmotions.Count >= 2)
		          {
				var twoEmotions = matchingEmotions.Take(2).Where(e => !string.IsNullOrEmpty(e.name)).ToList();
				if (twoEmotions.Count >= 2)
				{
					     string twoEmotionKey = string.Join("-", twoEmotions.Select(e => e.name).OrderBy(n => n));
					     if (PepoLibraryPersonality.Feelings.TryGetValue(twoEmotionKey, out string feelingName))
					     {
						          return feelingName;
					     }
				}
		          }

		          // Try with just the top emotion
		          if (topEmotion != null && !string.IsNullOrEmpty(topEmotion.name))
		          {
				string singleEmotionKey = topEmotion.name;
				if (PepoLibraryPersonality.Feelings.TryGetValue(singleEmotionKey, out string singleFeeling))
				{
					     return singleFeeling;
				}
		          }

		          // Final fallback: return the top emotion's descriptor
		          if (topEmotion != null)
		          {
				return GetEmotionIntensityDescriptor(topEmotion);
		          }

		          return "Neutral";
	     }

	     private static string GetEmotionIntensityDescriptor(EmotionAspect emotion)
	     {
		          if (emotion.value > emotion.maxValue * 0.66f)
				return !string.IsNullOrEmpty(emotion.intense) ? emotion.intense : emotion.name;
		          else if (emotion.value > emotion.maxValue * 0.33f)
				return !string.IsNullOrEmpty(emotion.descriptor) ? emotion.descriptor : emotion.name;
		          else
				return !string.IsNullOrEmpty(emotion.mild) ? emotion.mild : emotion.name;
	     }
}
