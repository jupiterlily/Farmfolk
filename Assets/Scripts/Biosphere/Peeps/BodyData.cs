using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BodyData
{
	     //genetic data
	     public Chromosome maternalChromosome;
	     public Chromosome paternalChromosome;
	     public List<NumericalAspect> expressedChromosomes;

	     //cosmetic data - managed independently from genetics
	     public CosmeticFeatures cosmeticFeatures;

	     public BodyData()
	     {
		          // Create maternal chromosome (always XX for mother)
		          maternalChromosome = new Chromosome("XX");

		          // Create paternal chromosome (randomly XX or XY for father)
		          paternalChromosome = new Chromosome(Random.value > 0.5f ? "XX" : "XY");

		          // Initialize cosmetic features
		          cosmeticFeatures = new CosmeticFeatures();
	     }

	     public BodyData(Chromosome mat, Chromosome pat)
	     {
		          maternalChromosome = mat;
		          paternalChromosome = pat;
		          ExpressChromosomes();
		          // Initialize cosmetic features after chromosomes are expressed
		          cosmeticFeatures = new CosmeticFeatures();
	     }

	     public void ExpressChromosomes(List<NumericalAspect> XpressedChromosomes = null)
	     {
		          if (XpressedChromosomes == null)
		          {
				expressedChromosomes = new List<NumericalAspect>();

				// Head shape - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Head Shape",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(0) : paternalChromosome.GetTraitByIndex(0),
				    1, 9, false, 0, 0));

				// Hair redness - choose between parents, preferring darker if different
				float hairRedness;
				int matHairRed = maternalChromosome.GetTraitByIndex(1);
				int patHairRed = paternalChromosome.GetTraitByIndex(1);
				if (matHairRed <= patHairRed + 1 && matHairRed >= patHairRed - 1)
					     hairRedness = Random.value > 0.5f ? matHairRed : patHairRed;
				else
					     hairRedness = matHairRed > patHairRed ? matHairRed : patHairRed;
				expressedChromosomes.Add(CreateNumericalAspect("Hair Redness", hairRedness, 1, 6, false, 0, 1));

				// Hair darkness - average between parents
				float hairDarkness = Random.Range(
				    Mathf.Min(maternalChromosome.GetTraitByIndex(2), paternalChromosome.GetTraitByIndex(2)),
				    Mathf.Max(maternalChromosome.GetTraitByIndex(2), paternalChromosome.GetTraitByIndex(2)));
				expressedChromosomes.Add(CreateNumericalAspect("Hair Darkness", hairDarkness, 1, 9, false, 0, 2));

				// Eye shape - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Eye Shape",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(3) : paternalChromosome.GetTraitByIndex(3),
				    1, 11, false, 0, 3));

				// Eye color - complex inheritance
				int matEye = maternalChromosome.GetTraitByIndex(4);
				int patEye = paternalChromosome.GetTraitByIndex(4);
				float eyeColor;
				if (matEye == 1 && patEye == 1)
					     eyeColor = 1; // Both violet -> violet
				else if (matEye <= 5 && patEye <= 5)
					     eyeColor = Random.value > 0.5f ? matEye : patEye; // Both in blue/green/grey group -> random
				else if (matEye == 7 || patEye == 7)
					     eyeColor = 7; // Brown dominant
				else if (matEye == 6 || patEye == 6)
					     eyeColor = 6; // Hazel dominant
				else
					     eyeColor = Mathf.Max(matEye, patEye); // Mixed case - choose darker
				expressedChromosomes.Add(CreateNumericalAspect("Eye Color", eyeColor, 1, 7, false, 0, 4));

				// Eye darkness - average between parents
				float eyeDarkness = Random.Range(
				    Mathf.Min(maternalChromosome.GetTraitByIndex(5), paternalChromosome.GetTraitByIndex(5)),
				    Mathf.Max(maternalChromosome.GetTraitByIndex(5), paternalChromosome.GetTraitByIndex(5)));
				expressedChromosomes.Add(CreateNumericalAspect("Eye Darkness", eyeDarkness, 1, 4, false, 0, 5));

				// Eye height - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Eye Height",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(6) : paternalChromosome.GetTraitByIndex(6),
				    1, 4, false, 0, 6));

				// Nose shape - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Nose Shape",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(7) : paternalChromosome.GetTraitByIndex(7),
				    1, 26, false, 0, 7));

				// Nose height - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Nose Height",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(8) : paternalChromosome.GetTraitByIndex(8),
				    1, 4, false, 0, 8));

				// Top lip shape - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Top Lip Shape",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(9) : paternalChromosome.GetTraitByIndex(9),
				    1, 9, false, 0, 9));

				// Bottom lip shape - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Bottom Lip Shape",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(10) : paternalChromosome.GetTraitByIndex(10),
				    1, 9, false, 0, 10));

				// Mouth height - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Mouth Height",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(11) : paternalChromosome.GetTraitByIndex(11),
				    1, 4, false, 0, 11));

				// Skin depth - average between parents
				float skinDepth = Random.Range(
				    Mathf.Min(maternalChromosome.GetTraitByIndex(12), paternalChromosome.GetTraitByIndex(12)),
				    Mathf.Max(maternalChromosome.GetTraitByIndex(12), paternalChromosome.GetTraitByIndex(12)));
				expressedChromosomes.Add(CreateNumericalAspect("Skin Depth", skinDepth, 1, 6, false, 0, 12));

				// Skin saturation - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Skin Saturation",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(13) : paternalChromosome.GetTraitByIndex(13),
				    1, 6, false, 0, 13));

				// Skin hue - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Skin Hue",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(14) : paternalChromosome.GetTraitByIndex(14),
				    1, 6, false, 0, 14));

				// Freckles - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Freckles",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(15) : paternalChromosome.GetTraitByIndex(15),
				    1, 9, false, 0, 15));

				// Invert freckles - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Invert Freckles",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(16) : paternalChromosome.GetTraitByIndex(16),
				    1, 3, false, 0, 16));

				// Express double freckles - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Express Double Freckles",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(17) : paternalChromosome.GetTraitByIndex(17),
				    1, 3, false, 0, 17));

				// Hair texture - 50% chance of higher value, else random between
				int matHairTexture = maternalChromosome.GetTraitByIndex(18);
				int patHairTexture = paternalChromosome.GetTraitByIndex(18);
				float hairTexture = Random.value > 0.5f ?
				    Mathf.Max(matHairTexture, patHairTexture) :
				    Random.Range(Mathf.Min(matHairTexture, patHairTexture), Mathf.Max(matHairTexture, patHairTexture) + 1);
				expressedChromosomes.Add(CreateNumericalAspect("Hair Texture", hairTexture, 1, 5, false, 0, 18));

				// Height - average between parents
				float height = Random.Range(
				    Mathf.Min(maternalChromosome.GetTraitByIndex(19), paternalChromosome.GetTraitByIndex(19)),
				    Mathf.Max(maternalChromosome.GetTraitByIndex(19), paternalChromosome.GetTraitByIndex(19)));
				expressedChromosomes.Add(CreateNumericalAspect("Height", height, 1, 21, false, 0, 19));

				// Mass genetics - average between parents
				float massGenetics = Random.Range(
				    Mathf.Min(maternalChromosome.GetTraitByIndex(20), paternalChromosome.GetTraitByIndex(20)),
				    Mathf.Max(maternalChromosome.GetTraitByIndex(20), paternalChromosome.GetTraitByIndex(20)));
				expressedChromosomes.Add(CreateNumericalAspect("Mass Genetics", massGenetics, 50, 101, false, 0, 20));

				// Brows - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Brows",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(21) : paternalChromosome.GetTraitByIndex(21),
				    1, 7, false, 0, 21));

				// Brow height - either or
				expressedChromosomes.Add(CreateNumericalAspect(
				    "Brow Height",
				    Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(22) : paternalChromosome.GetTraitByIndex(22),
				    1, 4, false, 0, 22));
		          }
		          else
		          {
				expressedChromosomes = XpressedChromosomes;
		          }
	     }

	     // Helper method to create NumericalAspect objects
	     private NumericalAspect CreateNumericalAspect(string name, float value, float minValue, float maxValue, bool polar, float polarPoint, int index)
	     {
		          return new NumericalAspect()
		          {
				name = name,
				value = value,
				minValue = minValue,
				maxValue = maxValue,
				polar = polar,
				polarPoint = polarPoint,
				descriptor = GetTraitDescription(index, value),
				timesIntegrated = 1
		          };
	     }

	     public int GetExpressedTrait(int traitIndex)
	     {
		          if (expressedChromosomes == null || expressedChromosomes.Count <= traitIndex)
				return 1; // Default to first variant

		          return (int)expressedChromosomes[traitIndex].value;
	     }

	     public float GetUnexpressedFrecklesTrait()
	     {
		          if (expressedChromosomes == null || expressedChromosomes.Count <= 15)
				return 1f;

		          // If expressed chromosome is maternal, return paternal, and vice versa
		          return (int)expressedChromosomes[15].value == paternalChromosome.GetTraitByIndex(15) ?
		              paternalChromosome.GetTraitByIndex(15) :
		              maternalChromosome.GetTraitByIndex(15);
	     }
	     private string GetTraitDescription(int traitIndex, float value)
	     {
		          // Convert value to integer if it's meant to be an integer trait
		          int intValue = Mathf.RoundToInt(value);

		          switch (traitIndex)
		          {
		          case 0: // Head shape
		          return $"Head shape type {intValue}";
		          case 1: // Hair redness (pheomelanin)
		          if (intValue <= 2)
				return "Very red hair";
		          else if (intValue <= 3)
				return "Red hair";
		          else if (intValue <= 4)
				return "Auburn hair";
		          else
				return "Dark red hair";
		          case 2: // Hair darkness (eumelanin)
		          if (intValue <= 3)
				return "Light brown hair";
		          else if (intValue <= 5)
				return "Brown hair";
		          else if (intValue <= 7)
				return "Dark brown hair";
		          else
				return "Black hair";
		          case 3: // Eye shape
		          return $"Eye shape type {intValue}";
		          case 4: // Eye color
		          switch (intValue)
		          {
		          case 1:
		          return "violet eyes";
		          case 2:
		          return "blue eyes";
		          case 3:
		          return "turquoise eyes";
		          case 4:
		          return "green eyes";
		          case 5:
		          return "hazel eyes";
		          case 6:
		          return "brown eyes";
		          case 7:
		          return "red eyes";
		          default:
		          return "Unknown eye color";
		          }
		          case 5: // Eye darkness
		          if (intValue == 1)
				return "Light ";
		          else if (intValue == 2)
				return "Medium ";
		          else if (intValue == 3)
				return "Dark ";
		          else
				return "Very dark ";
		          case 6: // Eye height
		          if (intValue == 1)
				return "Low-set";
		          else if (intValue == 2)
				return "Medium-set";
		          else
				return "High-set";
		          case 7: // Nose shape
		          return $"Nose shape type {intValue}";
		          case 8: // Nose height
		          if (intValue == 1)
				return "Low nose bridge";
		          else if (intValue == 2)
				return "Medium nose bridge";
		          else
				return "High nose bridge";
		          case 9: // Top lip shape
		          return $"Top lip shape type {intValue}";
		          case 10: // Bottom lip shape
		          return $"Bottom lip shape type {intValue}";
		          case 11: // Mouth height
		          if (intValue == 1)
				return "Low mouth height";
		          else if (intValue == 2)
				return "Medium mouth height";
		          else
				return "High mouth height";
		          case 12: // Skin depth (lightness/darkness)
		          if (intValue <= 2)
				return "Very light skin";
		          else if (intValue <= 3)
				return "Light skin";
		          else if (intValue <= 4)
				return "Medium skin";
		          else if (intValue <= 5)
				return "Dark skin";
		          else
				return "Very dark skin";
		          case 13: // Skin saturation
		          if (intValue <= 2)
				return "muted";
		          else if (intValue <= 4)
				return "Moderate skin saturation";
		          else
				return "Highly saturated skin";
		          case 14: // Skin hue
		          if (intValue <= 2)
				return "cool";
		          else if (intValue <= 4)
				return "neutral";
		          else
				return "warm";
		          case 15: // Freckles
		          if (intValue <= 0)
				return "No freckles";
		          else
		          {

				return "freckles";
		          }
		          case 16: // Invert freckles
		          return intValue == 1 ? "Freckles not inverted" : "Freckles inverted";
		          case 17: // Express double freckles
		          return intValue == 1 ? "some freckles" : "lots of freckles";
		          case 18: // Hair texture
		          switch (intValue)
		          {
		          case 1:
		          return "Straight hair";
		          case 2:
		          return "Wavy hair";
		          case 3:
		          return "Curly hair";
		          case 4:
		          return "Coily hair";
		          default:
		          return "Unknown hair texture";
		          }
		          case 19: // Height
			    // Assuming height is in cm and the value is an index that maps to a range
			    // We don't have the actual height in cm, so we use the index to describe
		          if (intValue <= 7)
				return "Short";
		          else if (intValue <= 14)
				return "Average height";
		          else
				return "Tall";
		          case 20: // Mass genetics
			    // Similarly, mass is an index
		          if (intValue <= 60)
				return "Light build";
		          else if (intValue <= 80)
				return "Average build";
		          else
				return "Heavy build";
		          case 21: // Brows
		          return $"Eyebrow type {intValue}";
		          case 22: // Brow height
		          if (intValue == 1)
				return "Low eyebrows";
		          else if (intValue == 2)
				return "Medium eyebrows";
		          else
				return "High eyebrows";
		          default:
		          return $"Trait {traitIndex}: {value}";
		          }
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
		          if (pepo.body.paternalChromosome.sexChromosome == "XX")
		          {
				pepo.body.cosmeticFeatures.pepoGrooming["Stubble"] = 0;
				pepo.body.cosmeticFeatures.pepoGrooming["Moustache"] = 0;
				return;
		          }

		          // For XY individuals, check if they present as masculine
		          if (pepo.ssd != null && pepo.ssd.presentsMasc)
		          {
				Debug.Log($"[PepoManager] Initializing facial hair for {PepoUtilities.GetFullName(pepo)}");
				// 70% chance to have some stubble if presenting masculine
				if (Random.value < 0.7f)
				{
					     pepo.body.cosmeticFeatures.pepoGrooming["Stubble"] = Random.Range(1, 6);
					     // 60% chance of having a moustache if they have stubble
					     if (pepo.body.cosmeticFeatures.pepoGrooming["Stubble"] >= 2 && Random.value < 0.6f)
					     {
						          pepo.body.cosmeticFeatures.pepoGrooming["Moustache"] = Random.Range(1, 16); // 1-15 moustache styles
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
					     if (pepo.ssd.presentsMasc && pepo.body.paternalChromosome.sexChromosome == "XY")
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
				if (relative.body.cosmeticFeatures != null)
				{
					     if (relative.body.expressedChromosomes != null)
					     {
						          int hairTexture = relative.body.GetExpressedTrait(18);
						          if (hairTexture == pepo.body.GetExpressedTrait(18))
						          {
								filteredPepoPool.Add(relative);
						          }
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

[System.Serializable]
public class Chromosome
{
	     //this should change, achromosome is abase class and then different chromosomes do different things.
	     // Sex chromosomes
	     public string sexChromosome = "XX";

	     // Physical traits
	     public int headShape = Random.Range(1, 10);//chose either or
	     public int hairEumelanin = Random.Range(1, 9);    // darkness, choose a range between parents
	     public int hairPheomelanin = Random.Range(1, 6);  // redness, choose a range between parents
	     public int eyeShape = Random.Range(1, 11);//choose  either or
	     public int eyeColor = Random.Range(1, 7);         // violet, blue, grey, green, brown
	     public int eyeDarkness = Random.Range(1, 4); //choose between arange from parents
	     public int eyeHeight = Random.Range(1, 4); //choose between arange from parents
	     public int noseShape = Random.Range(1, 26); //choose either or
	     public int noseHeight = Random.Range(1, 4); //choose between arange from parents
	     public int topLipShape = Random.Range(1, 9); //choose either or
	     public int bottomLipShape = Random.Range(1, 9); //choose either or
	     public int mouthHeight = Random.Range(1, 4); //choose between arange from parents
	     public int skinDepth = Random.Range(1, 6); //use parents as a range
	     public int skinSaturation = Random.Range(1, 6); //use either or
	     public int skinHue = Random.Range(1, 6); //use either or
	     public int Freckles = Random.Range(1, 9); //choose either parent
	     public int invertFreckles = Random.Range(1, 3); //flips image on vertical axis
	     public int ExpressDoubleFreckles = Random.Range(1, 3); //makes us show the freckles from both parents
	     public int hairTexture = Random.Range(1, 5); //straight, wavy, curly, coily
	     public int height = Random.Range(1, 21); //overall height range of roughly 1.5m to 2.0m
	     public int massGenetics = Random.Range(50, 101); //overall mass range of roughly 50kg to 100kg
	     public int brows = Random.Range(1, 19);
	     public int browHeight = Random.Range(1, 4); //choose either parent

	     // Constructor for creating a random chromosome
	     public Chromosome()
	     {
	     }

	     // Constructor for creating a chromosome with specific sex chromosome
	     public Chromosome(string sex)
	     {
		          sexChromosome = sex;
	     }

	     // Copy constructor
	     public Chromosome(Chromosome other)
	     {
		          sexChromosome = other.sexChromosome;
		          headShape = other.headShape;
		          hairEumelanin = other.hairEumelanin;
		          hairPheomelanin = other.hairPheomelanin;
		          eyeShape = other.eyeShape;
		          eyeColor = other.eyeColor;
		          eyeDarkness = other.eyeDarkness;
		          eyeHeight = other.eyeHeight;
		          noseShape = other.noseShape;
		          noseHeight = other.noseHeight;
		          topLipShape = other.topLipShape;
		          bottomLipShape = other.bottomLipShape;
		          mouthHeight = other.mouthHeight;
		          skinDepth = other.skinDepth;
		          skinSaturation = other.skinSaturation;
		          skinHue = other.skinHue;
		          Freckles = other.Freckles;
		          ExpressDoubleFreckles = other.ExpressDoubleFreckles;
		          invertFreckles = other.invertFreckles;
		          hairTexture = other.hairTexture;
		          height = other.height;
		          massGenetics = other.massGenetics;
		          brows = other.brows;
		          browHeight = other.browHeight;
	     }

	     public int GetTraitByIndex(int index)
	     {
		          switch (index)
		          {
		          case 0:
		          return headShape;
		          case 1:
		          return hairEumelanin;
		          case 2:
		          return hairPheomelanin;
		          case 3:
		          return eyeShape;
		          case 4:
		          return eyeColor;
		          case 5:
		          return eyeDarkness;
		          case 6:
		          return eyeHeight;
		          case 7:
		          return noseShape;
		          case 8:
		          return noseHeight;
		          case 9:
		          return topLipShape;
		          case 10:
		          return bottomLipShape;
		          case 11:
		          return mouthHeight;
		          case 12:
		          return skinDepth;
		          case 13:
		          return skinSaturation;
		          case 14:
		          return skinHue;
		          case 15:
		          return Freckles;
		          case 16:
		          return invertFreckles;
		          case 17:
		          return ExpressDoubleFreckles;
		          case 18:
		          return hairTexture;
		          case 19:
		          return height;
		          case 20:
		          return massGenetics;
		          case 21:
		          return brows;
		          case 22:
		          return browHeight;
		          default:
		          return -1;
		          }
	     }
}

