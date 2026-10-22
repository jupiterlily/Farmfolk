//using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum InheritanceRule
{
	     EitherOr, Average, WithinRange, ChooseDominant, LeanDominant, EyeColor, Additive
}
[System.Serializable]
public class BodyData
{
	     public ChromosomeData[] matChromosomes;
	     public ChromosomeData[] patChromosomes;
	     public ChromosomeData[] expChromosomes;
	     //genetic data
	     public Dictionary<string, NumericalAspect> physicalAttributes;

	     //cosmetic data - managed independently from genetics
	     public CosmeticFeatures cosmeticFeatures;

	     //public BodyData()
	     //{
	     //      // UnityEngine.Debug.Log($"{PepoUIUtilities.topLips.Length} top lips and {PepoUIUtilities.bottomLips.Length} bottom lips available.");
	     //      // Create maternal chromosome (always XX for mother)
	     //      //    maternalChromosome = new Chromosome("XX");

	     //      // Create paternal chromosome (randomly XX or XY for father)
	     //      //     paternalChromosome = new Chromosome(Random.value > 0.5f ? "XX" : "XY");

	     //      // Initialize cosmetic features

	     //}

	     public BodyData(ChromosomeData[] maternalData, ChromosomeData[] pateralData)
	     {
		          matChromosomes = maternalData;
		          patChromosomes = pateralData;
		          expChromosomes = GeneticsUtilities.ExpressChromosomes(matChromosomes, patChromosomes);

		          physicalAttributes = GeneticsUtilities.CollateAppearance(expChromosomes);
		          cosmeticFeatures = new CosmeticFeatures();
	     }

}
public sealed class BodyPlan
{
	     public string Name
	     {
		          get;
	     }
	     public IReadOnlyList<BodyPartData> Parts
	     {
		          get;
	     }
	     public IReadOnlyDictionary<string, IReadOnlyList<BodyPartData>> Groups
	     {
		          get;
	     }
	     public IEnumerable<AlleleData> Alleles => Parts.SelectMany(p => p.alleles);

	     private BodyPlan(string name, List<BodyPartData> parts, Dictionary<string, IReadOnlyList<BodyPartData>> groups)
	     {
		          Name = name;
		          Parts = parts;
		          Groups = groups;
	     }

	     public static Builder Build(string name) => new Builder(name);

	     public sealed class Builder
	     {
		          private readonly string _name;
		          private readonly List<BodyPartData> _parts = new();
		          private readonly Dictionary<string, List<BodyPartData>> _groups = new();
		          public Builder(string name)
		          {
				_name = name;
		          }

		          public Builder Part(string partName)
		          {
				var p = BodyPartLibrary.Lookup(partName);
				if (!_parts.Contains(p))
					     _parts.Add(p);
				return this;
		          }
		          public Builder Group(string groupName, params string[] partNames)
		          {
				var list = new List<BodyPartData>();
				foreach (var n in partNames)
				{
					     var p = BodyPartLibrary.Lookup(n);
					     list.Add(p);
					     if (!_parts.Contains(p))
						          _parts.Add(p);
				}
				_groups[groupName] = list;
				return this;
		          }
		          public BodyPlan Finish() => new BodyPlan(_name, _parts,
		              _groups.ToDictionary(kv => kv.Key, kv => (IReadOnlyList<BodyPartData>)kv.Value));
	     }
}
/// <summary>
/// Cosmetic features that are influenced by genetics but managed independently.
/// These change throughout life based on age, maintenance, and personal choice.
/// </summary>
[System.Serializable]
public class CosmeticFeatures
{
	     //We should have a dictionary that we can run through and reference
	     public Dictionary<string, int> pepoGrooming = new Dictionary<string, int>();

	     // Facial hair (for those with genetic capacity)
	     // public int stubbleLevel = 0; // 0-5: clean shaven - heavy stubble
	     //  public int moustacheStyle = 0; // 0: none, 1-14 different styles

	     // Other cosmetics
	     //  public bool wearsMakeup = false;
	     public int moustacheVariations = 18; // 0: none, 1-14: different styles
			            //hair

	     // Constructor - initializes with genetic defaults
	     public CosmeticFeatures()
	     {
		          pepoGrooming.Add("Stubble", -1);
		          pepoGrooming.Add("Moustache", -1);
		          pepoGrooming.Add("Beard", -1);

		          pepoGrooming.Add("HairUp", -1);
		          pepoGrooming.Add("HairBack", -1);
		          pepoGrooming.Add("HairTendrils", -1);
		          pepoGrooming.Add("HairFront", -1);

		          pepoGrooming.Add("Eyeshadow", -1);
		          pepoGrooming.Add("Eyeliner", -1);
		          pepoGrooming.Add("BlushHue", -1);
		          pepoGrooming.Add("BlushSat", -1);
		          pepoGrooming.Add("BlushValue", -1);
		          pepoGrooming.Add("BlushOpacity", -1);

		          pepoGrooming.Add("LipstickHue", -1);
		          pepoGrooming.Add("LipstickSat", -1);
		          pepoGrooming.Add("LipstickValue", -1);
		          pepoGrooming.Add("LipstickOpacity", -1);
	     }

	     /// <summary>
	     /// Initialize facial hair based on sex chromosome and gender presentation.
	     /// For debugging: XY individuals who present masculine get random stubble/moustache.
	     /// </summary>
	     public void InitializeStubbleAndMoustache(PepoData pepo)
	     {
		          // Only initialize facial hair for XY individuals (biological males)
		          if (pepo.body.physicalAttributes["SexChromosome"].descriptor == "XX")
		          {
				pepo.body.cosmeticFeatures.pepoGrooming["Stubble"] = 0;
				pepo.body.cosmeticFeatures.pepoGrooming["Moustache"] = 0;
				return;
		          }

		          // For XY individuals, check if they present as masculine
		          if (pepo.ssd != null && pepo.ssd.presentsMasc)
		          {
				//Debug.Log($"[PepoManager] Initializing facial hair for {PepoUtilities.GetFullName(pepo)}");
				// 70% chance to have some stubble if presenting masculine
				if (Random.value < 0.7f)
				{
					     pepo.body.cosmeticFeatures.pepoGrooming["Stubble"] = Random.Range(1, 6);
					     // 60% chance of having a moustache if they have stubble
					     if (pepo.body.cosmeticFeatures.pepoGrooming["Stubble"] >= 2 && Random.value < 0.6f)
					     {
						          pepo.body.cosmeticFeatures.pepoGrooming["Moustache"] = Random.Range(1, moustacheVariations); // 1-15 moustache styles
					     }
				}
				else
				{
					     //might still have a moustache even if no stubble
					     pepo.body.cosmeticFeatures.pepoGrooming["Moustache"] = (Random.value < 0.3f) ? Random.Range(1, 16) : 0;
				}

		          }

	     }

	     /// <summary>
	     /// Update cosmetic features based on age and personal choices.
	     /// Called during age/lifecycle updates.
	     /// </summary>
	     public void UpdateFacialHair(PepoData pepo)
	     {
		          if (pepo.personality == null)
				return;

		          float grooming = (10f - pepo.personality.Conscientiousness.value) / 10f;
		          float experimentation = pepo.personality.Openness.value / 10f;
		          //people who value conformity and tradition will copy their facial hair from their peers, kin, father and mentor figures.
		          //traditionalists weigh more heavily on kin and mentor figures.
		          //conformists will pick the most common style.
		          //create a mode of facial hair from a list of pepos.
		          List<PepoData> pepoPool = PepoUtilitiesBonds.ShipsToPepos(pepo.RelationshipLibrary[RelationshipType.kin]);
		          pepoPool.AddRange(PepoUtilitiesBonds.ShipsToPepos(pepo.RelationshipLibrary[RelationshipType.mentor]));
		          pepoPool.AddRange(PepoUtilitiesBonds.ShipsToPepos(pepo.RelationshipLibrary[RelationshipType.peer]));
		          pepoPool.AddRange(PepoUtilitiesBonds.ShipsToPepos(pepo.RelationshipLibrary[RelationshipType.parent]));
		          pepoPool.AddRange(PepoUtilitiesBonds.ShipsToPepos(pepo.RelationshipLibrary[RelationshipType.friend]));

		          //dictionary of styles and their counts
		          Dictionary<int, int> styleCounts = new Dictionary<int, int>();
		          Dictionary<int, int> stubbleCounts = new Dictionary<int, int>();
		          foreach (var relative in pepoPool)
		          {
				if (relative.body.cosmeticFeatures != null)
				{
					     if (pepo.ssd.presentsMasc && pepo.body.physicalAttributes["SexChromosome"].descriptor == "XY")
					     {
						          int style = relative.body.cosmeticFeatures.pepoGrooming["Moustache"];
						          int stubble = relative.body.cosmeticFeatures.pepoGrooming["Stubble"];
						          if (style > 0)
						          {
								if (styleCounts.ContainsKey(style))
								{
									     styleCounts[style]++;
								}
								else
								{
									     styleCounts[style] = 1;
								}
						          }
						          if (stubbleCounts.ContainsKey(stubble))
						          {
								stubbleCounts[stubble]++;
						          }
						          else
						          {
								stubbleCounts[stubble] = 1;
						          }
					     }
				}
		          }
		          //values determine what styles they pick from. conformists will pick common styles, self directed will pick unusual styles, stimulation seekers will change styles often.
		          // Randomly determine stubble based on grooming
		          if (Random.value < grooming * 0.3f) // 30% chance per update cycle if low conscientiousness
		          {
				pepo.body.cosmeticFeatures.pepoGrooming["Stubble"] = Mathf.Min(3, pepo.body.cosmeticFeatures.pepoGrooming["Stubble"] + Random.Range(0, 2));
		          }
		          else
		          {
				//set to desired level based on grooming
				Dictionary<int, int> stubblePref = new Dictionary<int, int>();
				Dictionary<int, int> moustachePref = new Dictionary<int, int>();
				//weight styles based on personal values. traditionalists and conformists will prefer common styles, self-directed and stimulation seekers will prefer uncommon styles.
				foreach (var kvp in stubbleCounts)
				{
					     //weight by conformity and tradition
					     stubblePref.Add(kvp.Key, (int)(kvp.Value * (pepo.values.Find(v => v.name == "cohesion").value) + pepo.values.Find(v => v.name == "tradition").value));
					     //counterweight by self-direction and stimulation
					     stubblePref[kvp.Key] -= (int)(kvp.Value * (pepo.values.Find(v => v.name == "autonomy").value) + pepo.values.Find(v => v.name == "excitement").value);
				}
				foreach (var kvp in styleCounts)
				{
					     //weight by conformity and tradition
					     moustachePref.Add(kvp.Key, (int)(kvp.Value * (pepo.values.Find(v => v.name == "cohesion").value) + pepo.values.Find(v => v.name == "tradition").value));
					     //counterweight by self-direction and stimulation
					     moustachePref[kvp.Key] -= (int)(kvp.Value * (pepo.values.Find(v => v.name == "autonomy").value) + pepo.values.Find(v => v.name == "excitement").value);
				}
				//choose the styles with the highest weight
				if (stubblePref.Count > 0)
				{
					     int maxStubble = -1;
					     int maxCount = -1;
					     foreach (var kvp in stubblePref)
					     {
						          if (kvp.Value > maxCount)
						          {
								maxCount = kvp.Value;
								maxStubble = kvp.Key;
						          }
					     }
					     pepo.body.cosmeticFeatures.pepoGrooming["Stubble"] = maxStubble;
				}
				if (moustachePref.Count > 0)
				{
					     int maxStyle = -1;
					     int maxCount = -1;
					     foreach (var kvp in moustachePref)
					     {
						          if (kvp.Value > maxCount)
						          {
								maxCount = kvp.Value;
								maxStyle = kvp.Key;
						          }
					     }
					     pepo.body.cosmeticFeatures.pepoGrooming["Moustache"] = maxStyle;

				}
				// Facial hair styles influenced by personality and experimentation
				if (experimentation > 0.6f && pepo.values.Find(v => v.name == "autonomy").value > 5 && Random.value < 0.3f)
				{
					     pepo.body.cosmeticFeatures.pepoGrooming["Stubble"] = Random.Range(0, 6); // Try different levels
					     pepo.body.cosmeticFeatures.pepoGrooming["Moustache"] = Random.Range(0, 16); // Try different styles
				}
		          }
	     }

	     public void UpdateHair(PepoData pepo)
	     {
		          if (pepo.personality == null)
				return;

		          float grooming = (10f - pepo.personality.Conscientiousness.value) / 10f;
		          float experimentation = pepo.personality.Openness.value / 10f;
		          //people who value conformity and tradition will copy their facial hair from their peers, kin, father and mentor figures.
		          //traditionalists weigh more heavily on kin and mentor figures.
		          //conformists will pick the most common style.
		          //create a mode of facial hair from a list of pepos.
		          List<PepoData> pepoPool = PepoUtilitiesBonds.ShipsToPepos(pepo.RelationshipLibrary[RelationshipType.kin]);
		          pepoPool.AddRange(PepoUtilitiesBonds.ShipsToPepos(pepo.RelationshipLibrary[RelationshipType.mentor]));
		          pepoPool.AddRange(PepoUtilitiesBonds.ShipsToPepos(pepo.RelationshipLibrary[RelationshipType.peer]));
		          pepoPool.AddRange(PepoUtilitiesBonds.ShipsToPepos(pepo.RelationshipLibrary[RelationshipType.parent]));
		          pepoPool.AddRange(PepoUtilitiesBonds.ShipsToPepos(pepo.RelationshipLibrary[RelationshipType.friend]));

		          List<PepoData> filteredPepoPool = new List<PepoData>();
		          foreach (var relative in pepoPool)
		          {
				//filter by pepos with the same hair texture
				//do this by checking that both their appearance attributes contain the "HairTexture" key. if they do check that the value.descriptor of the key is the same. if it is, add them to the filtered list.
				if (relative.body.physicalAttributes.ContainsKey("HairTexture") && pepo.body.physicalAttributes.ContainsKey("HairTexture"))
				{
					     if (relative.body.physicalAttributes["HairTexture"].descriptor == pepo.body.physicalAttributes["HairTexture"].descriptor)
					     {
						          filteredPepoPool.Add(relative);
					     }
				}

		          }
		          pepoPool = filteredPepoPool;
		          //dictionary of styles and their counts
		          Dictionary<int, int> HairUp = new Dictionary<int, int>();
		          Dictionary<int, int> HairBack = new Dictionary<int, int>();
		          Dictionary<int, int> HairTendrils = new Dictionary<int, int>();
		          Dictionary<int, int> HairFront = new Dictionary<int, int>();
		          foreach (var relative in pepoPool)
		          {
				if (relative.body.cosmeticFeatures != null)
				{
					     if (relative.ssd.presentsMasc == pepo.ssd.presentsMasc || relative.ssd.presentsFemme == pepo.ssd.presentsFemme)
					     {
						          int hairUp = relative.body.cosmeticFeatures.pepoGrooming["HairUp"];
						          int hairBack = relative.body.cosmeticFeatures.pepoGrooming["HairBack"];
						          int hairTendrils = relative.body.cosmeticFeatures.pepoGrooming["HairTendrils"];
						          int hairFront = relative.body.cosmeticFeatures.pepoGrooming["HairFront"];
						          if (hairUp > 0)
						          {
								if (HairUp.ContainsKey(hairUp))
								{
									     HairUp[hairUp]++;
								}
								else
								{
									     HairUp[hairUp] = 1;
								}
						          }
						          if (hairBack > 0)
						          {
								if (HairBack.ContainsKey(hairBack))
								{
									     HairBack[hairBack]++;
								}
								else
								{
									     HairBack[hairBack] = 1;
								}
						          }
						          if (hairTendrils > 0)
						          {
								if (HairTendrils.ContainsKey(hairTendrils))
								{
									     HairTendrils[hairTendrils]++;
								}
								else
								{
									     HairTendrils[hairTendrils] = 1;
								}
						          }
						          if (hairFront > 0)
						          {
								if (HairFront.ContainsKey(hairFront))
								{
									     HairFront[hairFront]++;
								}
								else
								{
									     HairFront[hairFront] = 1;
								}
						          }
					     }
				}
		          }
		          //values determine what styles they pick from. conformists will pick common styles, self directed will pick unusual styles, stimulation seekers will change styles often.
		          // Randomly determine stubble based on grooming

		          if (Random.value > grooming * 0.3f) // 30% chance per update cycle if low conscientiousness
		          {
				//set to desired level based on grooming
				Dictionary<int, int> HairUpPref = new Dictionary<int, int>();
				Dictionary<int, int> HairBackPref = new Dictionary<int, int>();
				Dictionary<int, int> HairTendrilsPref = new Dictionary<int, int>();
				Dictionary<int, int> HairFrontPref = new Dictionary<int, int>();
				//weight styles based on personal values. traditionalists and conformists will prefer common styles, self-directed and stimulation seekers will prefer uncommon styles.
				foreach (var kvp in HairUp)
				{
					     //weight by conformity and tradition
					     HairUpPref.Add(kvp.Key, (int)(kvp.Value * (pepo.values.Find(v => v.name == "cohesion").value) + pepo.values.Find(v => v.name == "tradition").value));
					     //counterweight by self-direction and stimulation
					     HairUpPref[kvp.Key] -= (int)(kvp.Value * (pepo.values.Find(v => v.name == "autonomy").value) + pepo.values.Find(v => v.name == "excitement").value);
				}
				foreach (var kvp in HairBack)
				{
					     //weight by conformity and tradition
					     HairBackPref.Add(kvp.Key, (int)(kvp.Value * (pepo.values.Find(v => v.name == "cohesion").value) + pepo.values.Find(v => v.name == "tradition").value));
					     //counterweight by self-direction and stimulation
					     HairBackPref[kvp.Key] -= (int)(kvp.Value * (pepo.values.Find(v => v.name == "autonomy").value) + pepo.values.Find(v => v.name == "excitement").value);
				}
				foreach (var kvp in HairTendrils)
				{
					     //weight by conformity and tradition
					     HairTendrilsPref.Add(kvp.Key, (int)(kvp.Value * (pepo.values.Find(v => v.name == "cohesion").value) + pepo.values.Find(v => v.name == "tradition").value));
					     //counterweight by self-direction and stimulation
					     HairTendrilsPref[kvp.Key] -= (int)(kvp.Value * (pepo.values.Find(v => v.name == "autonomy").value) + pepo.values.Find(v => v.name == "excitement").value);
				}
				foreach (var kvp in HairFront)
				{
					     //weight by conformity and tradition
					     HairFrontPref.Add(kvp.Key, (int)(kvp.Value * (pepo.values.Find(v => v.name == "cohesion").value) + pepo.values.Find(v => v.name == "tradition").value));
					     //counterweight by self-direction and stimulation
					     HairFrontPref[kvp.Key] -= (int)(kvp.Value * (pepo.values.Find(v => v.name == "autonomy").value) + pepo.values.Find(v => v.name == "excitement").value);
				}
				//choose the styles with the highest weight
				if (HairUpPref.Count > 0)
				{
					     int maxHairUp = -1;
					     int maxCount = -1;
					     foreach (var kvp in HairUpPref)
					     {
						          if (kvp.Value > maxCount)
						          {
								maxCount = kvp.Value;
								maxHairUp = kvp.Key;
						          }
					     }
					     pepo.body.cosmeticFeatures.pepoGrooming["HairUp"] = maxHairUp;
				}
				if (HairBackPref.Count > 0)
				{
					     int maxHairBack = -1;
					     int maxCount = -1;
					     foreach (var kvp in HairBackPref)
					     {
						          if (kvp.Value > maxCount)
						          {
								maxCount = kvp.Value;
								maxHairBack = kvp.Key;
						          }
					     }
					     pepo.body.cosmeticFeatures.pepoGrooming["HairBack"] = maxHairBack;
				}
				if (HairTendrilsPref.Count > 0)
				{
					     int maxHairTendrils = -1;
					     int maxCount = -1;
					     foreach (var kvp in HairTendrilsPref)
					     {
						          if (kvp.Value > maxCount)
						          {
								maxCount = kvp.Value;
								maxHairTendrils = kvp.Key;
						          }
					     }
					     pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"] = maxHairTendrils;
				}
				if (HairFrontPref.Count > 0)
				{
					     int maxHairFront = -1;
					     int maxCount = -1;
					     foreach (var kvp in HairFrontPref)
					     {
						          if (kvp.Value > maxCount)
						          {
								maxCount = kvp.Value;
								maxHairFront = kvp.Key;
						          }
					     }
					     pepo.body.cosmeticFeatures.pepoGrooming["HairFront"] = maxHairFront;
				}
				// Facial hair styles influenced by personality and experimentation
				if (experimentation > 0.6f && pepo.values.Find(v => v.name == "autonomy").value > 5 && Random.value < 0.3f)
				{
					     pepo.body.cosmeticFeatures.pepoGrooming["HairUp"] = Random.Range(0, HairUpPref.Count); // Try different levels
					     pepo.body.cosmeticFeatures.pepoGrooming["HairBack"] = Random.Range(0, HairBackPref.Count); // Try different styles
					     pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"] = Random.Range(0, HairTendrilsPref.Count); // Try different levels
					     pepo.body.cosmeticFeatures.pepoGrooming["HairFront"] = Random.Range(0, HairFrontPref.Count); // Try different styles
				}
		          }
	     }

}

public enum ColourBlend
{
	     Multiply, Lerp, Screen, Overlay, Add
}
public enum ColourType
{
	     StructuralColour, InorganicPigment, OrganicPigment, BiologicalPigment, PearlescentPigment, LuminescentPigment
}

public sealed class WorldColourData
{
	     public string Name
	     {
		          get;
	     }
	     public ColourData[] Colours
	     {
		          get;
	     }
	     public ColourBlend Blend
	     {
		          get;
	     }
	     public ColourType type;
	     public WorldColourData(string name, ColourType type, ColourBlend blend, params ColourData[] colours)
	     {
		          Name = name;
		          Colours = colours;
		          Blend = blend;
		          this.type = type;
	     }

	     /// <summary>Apply at a 0..1 density. t is used to sample the palette so density also shifts hue.</summary>
	     public Color Apply(Color underneath, float density)
	     {
		          density = Mathf.Clamp01(density);
		          if (density <= 0f)
				return underneath;

		          Color top = Colours[0].Sample(density);
		          return Blend switch
		          {
				ColourBlend.Multiply => Color.Lerp(underneath, underneath * top, density),
				ColourBlend.Lerp => Color.Lerp(underneath, top, density),
				ColourBlend.Screen => Color.Lerp(underneath, Color.white - ((Color.white - underneath) * (Color.white - top)), density),
				ColourBlend.Overlay => Color.Lerp(underneath, Overlay(underneath, top), density),
				ColourBlend.Add => Color.Lerp(underneath, underneath + top, density),
				_ => underneath,
		          };
	     }

	     private static Color Overlay(Color b, Color t) =>
	         new Color(
	             b.r < 0.5f ? 2 * b.r * t.r : 1 - 2 * (1 - b.r) * (1 - t.r),
	             b.g < 0.5f ? 2 * b.g * t.g : 1 - 2 * (1 - b.g) * (1 - t.g),
	             b.b < 0.5f ? 2 * b.b * t.b : 1 - 2 * (1 - b.b) * (1 - t.b),
	             1f);
}

[System.Serializable]
public struct ColourData
{
	     public Vector2 hue;         // 0..1 each end
	     public Vector2 saturation;
	     public Vector2 value;

	     public ColourData(float hMin, float hMax, float sMin, float sMax, float vMin, float vMax)
	     {
		          hue = new Vector2(hMin, hMax);
		          saturation = new Vector2(sMin, sMax);
		          value = new Vector2(vMin, vMax);
	     }

	     /// <summary>Fixed colour: no variation.</summary>
	     public static ColourData Fixed(Color c)
	     {
		          Color.RGBToHSV(c, out var h, out var s, out var v);
		          return new ColourData(h, h, s, s, v, v);
	     }

	     /// <summary>Deterministic sample — t in [0,1] picks the point on the gradient.</summary>
	     public Color Sample(float t)
	     {
		          t = Mathf.Clamp01(t);
		          return Color.HSVToRGB(
		              Mathf.Lerp(hue.x, hue.y, t),
		              Mathf.Lerp(saturation.x, saturation.y, t),
		              Mathf.Lerp(value.x, value.y, t));
	     }

	     /// <summary>Random sample in the ranges — use for individual identity variation.</summary>
	     public Color SampleRandom() => Color.HSVToRGB(
	         Random.Range(hue.x, hue.y),
	         Random.Range(saturation.x, saturation.y),
	         Random.Range(value.x, value.y));
}
public class AlleleData
{
	     public InheritanceRule inheritance;
	     public NumericalAspect range;

	     // Template ctor — descriptors are injected by GeneLibrary so no static-init cycle.
	     public AlleleData(string name, int min, int max, InheritanceRule inheritance, IReadOnlyList<string> descriptors)
	     {
		          this.inheritance = inheritance;
		          range = new NumericalAspect(name, max, min);
		          range.value = UnityEngine.Random.Range(min, max + 1);  // seed value (was exclusive-bug)
		          range.descriptor = DescribeFrom(descriptors, (int)range.value);
	     }

	     // Mutated / expressed ctor
	     public AlleleData(InheritanceRule inheritance, NumericalAspect range)
	     {
		          this.inheritance = inheritance;
		          this.range = range;
	     }

	     private static string DescribeFrom(IReadOnlyList<string> d, int v)
	     {
		          int i = v - 1;
		          return (i >= 0 && i < d.Count) ? d[i] : $"value {v}";
	     }
}
public sealed class GeneData
{
	     public string Name
	     {
		          get;
	     }
	     public InheritanceRule Inheritance
	     {
		          get;
	     }
	     public int Min
	     {
		          get;
	     }
	     public int Max
	     {
		          get;
	     }
	     public IReadOnlyList<string> Descriptors
	     {
		          get;
	     }
	     public bool visible;

	     private GeneData(string name, InheritanceRule inheritance, int min, int max, IReadOnlyList<string> desc, bool vs = false)
	     {
		          Name = name;
		          Inheritance = inheritance;
		          Min = min;
		          Max = max;
		          Descriptors = desc;
		          visible = vs;
	     }

	     /// <summary>Variant count comes from a sprite pool / runtime source. Rebuild() picks up new sprites.</summary>
	     public static GeneData Count(string name, InheritanceRule inheritance, System.Func<int> poolCount, string prefix = null, bool vs = false)
	     {
		          int n = poolCount();
		          if (n <= 0)
				throw new System.InvalidOperationException($"Gene '{name}' has empty pool.");
		          return new GeneData(name, inheritance, 1, n,
		              Generated(prefix ?? name, n), vs);
	     }

	     /// <summary>Hand-authored descriptors. Max = descriptors.Length.</summary>
	     public static GeneData Explicit(string name, InheritanceRule inheritance, bool visible = false, params string[] descriptors)
	         => new GeneData(name, inheritance, 1, descriptors.Length, descriptors, visible);

	     /// <summary>Deterministic generated descriptors ("reaches menopause at 40", etc).</summary>
	     public static GeneData Generated(string name, InheritanceRule inheritance, int count, string prefix, int startAt = 1, bool visible = false)
	         => new GeneData(name, inheritance, 1, count, Generated(prefix, count, startAt), visible);

	     /// <summary>Numeric range with auto-generated "name N" descriptors.</summary>
	     public static GeneData Range(string name, InheritanceRule inheritance, int min, int max, bool visible = false)
	         => new GeneData(name, inheritance, min, max, Generated(name, max - min + 1, min), visible);

	     private static string[] Generated(string prefix, int count, int startAt = 1)
	     {
		          var a = new string[count];
		          for (int i = 0; i < count; i++)
				a[i] = $"{prefix} {i + startAt}";
		          return a;
	     }
}
public class BodyPartData
{
	     //although really this is an organ.	     
	     public string name; //ie eye /// top lip
	     public AlleleData[] alleles; //
	     public string[] allelesRequired; //list of allele names that are required for this body part to be expressed. if any are missing, the body part will not be expressed.
	     public string[] allelesOptional; //list of allele names that are optional for this body part to be expressed. if any are missing, the body part will still be expressed.
	     public BodyPartData(string nom, List<AlleleData> alleles, List<string> required = null, List<string> optional = null)
	     {
		          name = nom;
		          if (required == null)
		          {
				required = new List<string>();
				foreach (AlleleData allele in alleles)
				{
					     required.Add(allele.range.name);
				}
		          }
		          if (optional == null)
		          {
				optional = new List<string>();
		          }
		          allelesRequired = required.ToArray();
		          allelesOptional = optional.ToArray();
		          this.alleles = alleles.ToArray();
	     }
}
public class ChromosomeData
{
	     public string name;
	     public AlleleData[] alleles;
}
