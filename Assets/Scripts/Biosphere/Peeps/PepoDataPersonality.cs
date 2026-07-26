using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PersonalityAspect : NumericalAspect
{//exists on a scale of 0-10 with 5 being the mid point
	     public PersonalityAspect(char n, float max, List<string> high, List<string> low, float v = -1, string hardname = null)
	     {
		          if (hardname != null)
		          {
				name = hardname;
				value = v;
		          }
		          else if (n == 'h' || n == 'H')
		          {
				name = "Honesty";
				value = v;
		          }
		          else if (n == 'e' || n == 'E')
		          {
				name = "Emotionality";
				value = v;
		          }
		          else if (n == 'x' || n == 'X')
		          {
				name = "Extraversion";
				value = v;
		          }
		          else if (n == 'a' || n == 'A')
		          {
				name = "Agreeableness";
				value = v;
		          }
		          else if (n == 'c' || n == 'C')
		          {
				name = "Conscientiousness";
				value = v;
		          }
		          else if (n == 'o' || n == 'O')
		          {
				name = "Openness";
				value = v;
		          }

		          if (value > max / 2)
		          {
				descriptor = high[UnityEngine.Random.Range(0, high.Count)];
		          }
		          else
		          {
				descriptor = low[UnityEngine.Random.Range(0, low.Count)];
		          }
		          maxValue = max;

	     }

}

public class HEXACO
{
	     public PersonalityAspect Honesty = PepoLibraryPersonality.HEXACODimension.H;
	     public PersonalityAspect Emotionality = PepoLibraryPersonality.HEXACODimension.E;
	     public PersonalityAspect Extraversion = PepoLibraryPersonality.HEXACODimension.X;
	     public PersonalityAspect Agreeableness = PepoLibraryPersonality.HEXACODimension.A;
	     public PersonalityAspect Conscientiousness = PepoLibraryPersonality.HEXACODimension.C;
	     public PersonalityAspect Openness = PepoLibraryPersonality.HEXACODimension.O;

	     public PersonalityAspect[] HonestyFacets = PepoLibraryPersonality.HFacets.Values.ToArray();
	     public PersonalityAspect[] EmotionalityFacets = PepoLibraryPersonality.EFacets.Values.ToArray();
	     public PersonalityAspect[] ExtraversionFacets = PepoLibraryPersonality.XFacets.Values.ToArray();
	     public PersonalityAspect[] AgreeablenessFacets = PepoLibraryPersonality.AFacets.Values.ToArray();
	     public PersonalityAspect[] ConscientiousnessFacets = PepoLibraryPersonality.CFacets.Values.ToArray();
	     public PersonalityAspect[] OpennessFacets = PepoLibraryPersonality.OFacets.Values.ToArray();

	     public float maximumValue = 10f;

	     public HEXACO(float H = -1, float E = -1, float X = -1, float A = -1, float C = -1, float O = -1)
	     {
		          if (H < 0)
		          {
				Honesty.value = Random.Range(0, maximumValue);
				Honesty.descriptor = (Honesty.value > maximumValue / 2) ? "honest" : "manipulative";
		          }
		          if (E < 0)
		          {
				Emotionality.value = Random.Range(0, maximumValue);
				Emotionality.descriptor = (Emotionality.value > maximumValue / 2) ? "emotional" : "detatched";
		          }
		          if (X < 0)
		          {
				Extraversion.value = Random.Range(0, maximumValue);
				Extraversion.descriptor = (Extraversion.value > maximumValue / 2) ? "extraverted" : "introverted";
		          }
		          if (A < 0)
		          {
				Agreeableness.value = Random.Range(0, maximumValue);
				Agreeableness.descriptor = (Agreeableness.value > maximumValue / 2) ? "forgiving" : "resentful";
		          }
		          if (C < 0)
		          {
				Conscientiousness.value = Random.Range(0, maximumValue);
				Conscientiousness.descriptor = (Conscientiousness.value > maximumValue / 2) ? "careful" : "reckless";
		          }
		          if (O < 0)
		          {
				Openness.value = Random.Range(0, maximumValue);
				Openness.descriptor = (Openness.value > maximumValue / 2) ? "open" : "stubborn";
		          }

	     }
	     public PersonalityAspect[] FullHexRange()
	     {
		          PersonalityAspect[] r = new PersonalityAspect[6];
		          r[0] = Honesty;
		          r[1] = Emotionality;
		          r[2] = Extraversion;
		          r[3] = Agreeableness;
		          r[4] = Conscientiousness;
		          r[5] = Openness;
		          return r;
	     }
}


public class Mood
{
	     public EmotionAspect Surprise = PlutchickDimension.Surprise;
	     public EmotionAspect Anticipation = PlutchickDimension.Anticipation;
	     public EmotionAspect Trust = PlutchickDimension.Trust;
	     public EmotionAspect Disgust = PlutchickDimension.Disgust;
	     public EmotionAspect Fear = PlutchickDimension.Fear;
	     public EmotionAspect Anger = PlutchickDimension.Anger;
	     public EmotionAspect Joy = PlutchickDimension.Joy;
	     public EmotionAspect Sadness = PlutchickDimension.Sadness;

	     public Mood(float s = 0, float e = 0, float t = 0, float d = 0, float f = 0, float a = 0, float j = 0, float m = 0)
	     {
		          Surprise.value = s;
		          Anticipation.value = e;
		          Trust.value = t;
		          Disgust.value = d;
		          Fear.value = f;
		          Anger.value = a;
		          Joy.value = j;
		          Sadness.value = m;
	     }
	     public EmotionAspect[] FullHexRange()
	     {
		          EmotionAspect[] r = new EmotionAspect[8];
		          r[0] = Surprise;
		          r[1] = Anticipation;
		          r[2] = Trust;
		          r[3] = Disgust;
		          r[4] = Fear;
		          r[5] = Anger;
		          r[6] = Joy;
		          r[7] = Sadness;
		          return r;
	     }
	     public void IntegrateFeeling(Feeling feeling)
	     {
		          int i = 0;
		          foreach (var feel in FullHexRange())
		          {
				if (feel.name == feeling.aspects[i].name)
				{
					     feel.IntegrateAspect(feeling);
				}
		          }
	     }
	     //Apply Mood effect
	     public void ApplyMoodEffect(float[] moodEffect)
	     {

	     }
}


public class PlutchickDimension
{
	     public static EmotionAspect Surprise => new EmotionAspect('s', 10f, new List<string> { "amazement" }, new List<string> { "distraction" });
	     public static EmotionAspect Anticipation => new EmotionAspect('a', 10f, new List<string> { "vigilance" }, new List<string> { "interest" });
	     public static EmotionAspect Trust => new EmotionAspect('t', 10f, new List<string> { "admiration" }, new List<string> { "acceptance" });
	     public static EmotionAspect Disgust => new EmotionAspect('d', 10f, new List<string> { "loathing" }, new List<string> { "boredom" });
	     public static EmotionAspect Fear => new EmotionAspect('f', 10f, new List<string> { "terror" }, new List<string> { "apprehension" });
	     public static EmotionAspect Anger => new EmotionAspect('r', 10f, new List<string> { "rage" }, new List<string> { "annoyance" });
	     public static EmotionAspect Joy => new EmotionAspect('j', 10f, new List<string> { "ecstacy" }, new List<string> { "serenity" });
	     public static EmotionAspect Sadness => new EmotionAspect('m', 10f, new List<string> { "grief" }, new List<string> { "pensiveness" });

}
public class EmotionAspect : NumericalAspect
{
	     public string mild;
	     public string intense;
	     public EmotionAspect(char n, float max, List<string> high, List<string> low, float v = 5, string hardname = null)
	     {
		          maxValue = max;
		          if (n == 's' || n == 'S')
		          {
				name = "Surprise"; //oh
				mild = "Distraction";
				intense = "Amazement";
				value = v;
		          }
		          if (n == 'a' || n == 'A')
		          {
				name = "Anticipation"; //hm
				mild = "Interest";
				intense = "Vigilence";
				value = v;
		          }
		          if (n == 't' || n == 'T')
		          {
				name = "Trust"; //aw
				mild = "Acceptance";
				intense = "Admiration";
				value = v;
		          }
		          if (n == 'd' || n == 'D')
		          {
				name = "Disgust"; //ew
				mild = "Boredom";
				intense = "Loathing";
				value = v;
		          }
		          if (n == 'f' || n == 'F')
		          {
				name = "Fear"; //ah
				mild = "Apprehension";
				intense = "Terror";
				value = v;
		          }
		          if (n == 'r' || n == 'R')
		          {
				name = "Anger"; //uh
				mild = "Annoyance";
				intense = "Rage";
				value = v;
		          }
		          if (n == 'j' || n == 'J')
		          {
				name = "Joy"; //om
				mild = "Serene";
				intense = "Ecstacy";
				value = v;
		          }
		          if (n == 'm' || n == 'M')
		          {
				name = "Sadness"; //wa
				mild = "Pensiveness";
				intense = "Grief";
				value = v;
		          }
		          if (hardname != null)
		          {
				name = hardname;
				value = v;
		          }
		          if (value > max * 0.66f)
		          {
				intense = high[UnityEngine.Random.Range(0, high.Count)];
		          }
		          if (value > max * 0.33f && value <= max * 0.66f)
		          {
				descriptor = name;
		          }
		          if (value <= max * 0.33f)
		          {
				mild = low[UnityEngine.Random.Range(0, low.Count)];
		          }

	     }
}
public class Feeling : NumericalAspect
{
	     public List<EmotionAspect> aspects;

	     public float timeLeft;
	     public Feeling(List<EmotionAspect> emotions)
	     {
		          int i;
		          if (emotions == null)
		          {
				emotions = new List<EmotionAspect>();
		          }
		          aspects = emotions;
		          i = aspects.Count;
		          value = 0;
		          if (i > 0)
		          {
				foreach (var emotion in aspects)
				{
					     IntegrateAspect(emotion);
				}
		          }

	     }
}


