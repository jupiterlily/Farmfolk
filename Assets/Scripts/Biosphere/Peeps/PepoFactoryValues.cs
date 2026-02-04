
using System.Collections.Generic;
using UnityEngine;

public static class PepoFactoryValues
{
	     public static List<Value> RandomValues()
	     {
		          var values = new List<Value>();
		          values.AddRange(PepoLibraryValues.basicHumanValues());
		          foreach (var v in values)
		          {
				v.value = Random.Range(-1, 8);
				if (v.value < 0)
				{
					     v.descriptor = v.opposite;
				}
				else
				{
					     v.descriptor = v.name;
				}
		          }
		          return values;
	     }
	     public static List<Value> GroupValues(List<PepoData> dudes)
	     {                   //find the range in values
		          if (dudes.Count == 0)
				return null;
		          float min = dudes[0].values[0].value;
		          float max = dudes[0].values[0].value;
		          float avg = dudes[0].values[0].value;
		          var values = PepoLibraryValues.basicHumanValues();
		          for (int i = 0; i < values.Count - 1; i++)
		          {
				for (int j = 0; j < dudes.Count - 1; j++)
				{
					     if (dudes[j].values[i].value < min)
					     {
						          min = dudes[j].values[i].value;
					     }
					     if (dudes[j].values[i].value > max)
					     {
						          max = dudes[j].values[i].value;
					     }
					     avg += dudes[j].values[i].value;
				}
				avg = avg / dudes.Count;
				values[i].value = avg;
				Debug.Log($"this population of {dudes.Count} has an average value of {avg:0} for {values[i].name} ranging from {min} to {max}");
		          }
		          return values;
	     }
	     public static List<Value> IntegrateValues(List<Value> sponge, List<Value> water)
	     {
		          foreach (Value v in sponge)
		          {
				foreach (Value v2 in water)
				{
					     if (v2.value == 0)
						          continue;
					     if (v.name == v2.name)
					     {
						          v.IntegrateAspect(v2);
					     }
				}
		          }
		          return sponge;
	     }
	     public static List<Value> ValuesFromHexaco(HEXACO personality)
	     {

		          List<Value> values = PepoLibraryValues.basicHumanValues();
		          float factor = values[0].maxValue / (personality.maximumValue / 2);
		          foreach (var v in values)
		          {
				if (personality.Openness.value > 5)
				{                                       //foreach value in the openness category, add the value normalised from openness and extraversion. subtract 5 from each. openness adds more self direction, extraversion adds more stimulation. 
					     if (PepoUtilitiesValues.IsOpennessValue(v))
					     {
						          if (v.name == "curiosity")
						          {
								v.value += (personality.Openness.value * factor) / 2;
						          }
						          v.value += (personality.Openness.value * factor) / 2;

					     }


				}
				if (personality.Extraversion.value > 5)
				{
					     if (v.name == "excitement")
					     {
						          v.value += (personality.Extraversion.value * factor) / 2;
					     }
					     v.value += (personality.Extraversion.value * factor) / 2;
				}
				if (personality.Agreeableness.value > 5)
				{                                       //foreach value in the self transcendence values add a modifier from agreeableness, subtracted from 5 to accommodate the scale. add extra to benevolence.

					     if (PepoUtilitiesValues.IsSelfTranscendenceValue(v))
					     {
						          v.value += (personality.Agreeableness.value * factor) / 2;
						          if (v.name == "dependability")
						          {
								v.value += (personality.Agreeableness.value * factor) / 2;
						          }
					     }

				}

				if (personality.Extraversion.value > 5)
				{      //extraversion is correlated with self-enhancement and negatively with traditional values.

					     if (PepoUtilitiesValues.IsSelfEnhancementValue(v))
					     {
						          v.value += (personality.Extraversion.value * factor);
					     }
					     if (v.name == "tradition")
					     {
						          v.value = -1;
					     }

				}
				//Conscientiousness correlates with achievement, conformity and security.
				if (personality.Conscientiousness.value > 5)
				{
					     if (v.name == "accomplishment")
					     {
						          v.value += personality.Conscientiousness.value * factor;
					     }
					     if (v.name == "obedience")
					     {
						          v.value += personality.Conscientiousness.value * factor;
					     }
					     if (v.name == "security")
					     {
						          v.value += personality.Conscientiousness.value * factor;
					     }

				}
		          }

		          return values;
	     }

}
