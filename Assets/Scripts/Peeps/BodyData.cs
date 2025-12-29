using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BodyData
{
	     //genetic data
	     public Chromosome maternalChromosome;
	     public Chromosome paternalChromosome;
	     public List<int> expressedChromosomes;

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

	     public void ExpressChromosomes(List<int> XpressedChromosomes = null)
	     {
		          if (XpressedChromosomes == null)
		          {
				expressedChromosomes = new List<int>();
				//either ors, 
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(0) : paternalChromosome.GetTraitByIndex(0));
				//hair colour/redness. base hue of hair. lower number is redder so we should make it rarest. if they're similar, choose randomly between them, else choose the darker one
				expressedChromosomes.Add(maternalChromosome.GetTraitByIndex(1) <= paternalChromosome.GetTraitByIndex(1) + 1 && maternalChromosome.GetTraitByIndex(1) >= paternalChromosome.GetTraitByIndex(1) - 1 ? Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(1) : paternalChromosome.GetTraitByIndex(1) : maternalChromosome.GetTraitByIndex(1) > paternalChromosome.GetTraitByIndex(1) ? maternalChromosome.GetTraitByIndex(1) : paternalChromosome.GetTraitByIndex(1));
				//hair darkness
				expressedChromosomes.Add(Random.Range(maternalChromosome.GetTraitByIndex(2), paternalChromosome.GetTraitByIndex(2)));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(3) : paternalChromosome.GetTraitByIndex(3));
				//choose the highest number for eye color
				int matEye = maternalChromosome.GetTraitByIndex(4);
				int patEye = paternalChromosome.GetTraitByIndex(4);

				if (matEye == 1 && patEye == 1)
				{
					     expressedChromosomes.Add(1); // Both violet -> violet
				}
				else if (matEye <= 5 && patEye <= 5)
				{
					     // Both in blue/green/grey group -> random
					     expressedChromosomes.Add(Random.value > 0.5f ? matEye : patEye);
				}
				else if (matEye == 7 || patEye == 7)
				{
					     expressedChromosomes.Add(7); // Brown dominant
				}
				else if (matEye == 6 || patEye == 6)
				{
					     expressedChromosomes.Add(6); // Hazel dominant
				}
				else
				{
					     // Mixed case (one light, one dark) - choose darker
					     expressedChromosomes.Add(Mathf.Max(matEye, patEye));
				}
				expressedChromosomes.Add(Random.Range(maternalChromosome.GetTraitByIndex(5), paternalChromosome.GetTraitByIndex(5)));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(6) : paternalChromosome.GetTraitByIndex(6));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(7) : paternalChromosome.GetTraitByIndex(7));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(8) : paternalChromosome.GetTraitByIndex(8));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(9) : paternalChromosome.GetTraitByIndex(9));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(10) : paternalChromosome.GetTraitByIndex(10));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(11) : paternalChromosome.GetTraitByIndex(11));
				expressedChromosomes.Add(Random.Range(maternalChromosome.GetTraitByIndex(12), paternalChromosome.GetTraitByIndex(12)));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(13) : paternalChromosome.GetTraitByIndex(13));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(14) : paternalChromosome.GetTraitByIndex(14));
				//freckles - either or
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(15) : paternalChromosome.GetTraitByIndex(15));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(16) : paternalChromosome.GetTraitByIndex(16));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(17) : paternalChromosome.GetTraitByIndex(17));
				//get hair texture. 50% chance of being the higher value (curlyer hair) else it generates as a random value between the two
				expressedChromosomes.Add(Random.value > 0.5f ? Mathf.Max(maternalChromosome.GetTraitByIndex(18), paternalChromosome.GetTraitByIndex(18)) : Random.Range(Mathf.Min(maternalChromosome.GetTraitByIndex(18), paternalChromosome.GetTraitByIndex(18)), Mathf.Max(maternalChromosome.GetTraitByIndex(18), paternalChromosome.GetTraitByIndex(18))));
				//add 19 and 20.
				expressedChromosomes.Add(Random.Range(maternalChromosome.GetTraitByIndex(19), paternalChromosome.GetTraitByIndex(19)));
				expressedChromosomes.Add(Random.Range(maternalChromosome.GetTraitByIndex(20), paternalChromosome.GetTraitByIndex(20)));
				//brows
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(21) : paternalChromosome.GetTraitByIndex(21));
				//brows
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(22) : paternalChromosome.GetTraitByIndex(22));

		          }
		          else
		          {
				expressedChromosomes = XpressedChromosomes;
		          }
	     }

	     public int GetUnexpressedFrecklesTrait()
	     {
		          if (expressedChromosomes == null || expressedChromosomes.Count <= 14)
				return 1;

		          // If expressed chromosome is maternal, return paternal, and vice versa
		          return expressedChromosomes[14] == paternalChromosome.GetTraitByIndex(14) ?
		              paternalChromosome.GetTraitByIndex(14) :
		              maternalChromosome.GetTraitByIndex(14);
	     }

	     public int GetExpressedTrait(int traitIndex)
	     {
		          if (expressedChromosomes == null || expressedChromosomes.Count <= traitIndex)
				return 1; // Default to first variant

		          return expressedChromosomes[traitIndex] == 0 ?
		              maternalChromosome.GetTraitByIndex(traitIndex) :
		              paternalChromosome.GetTraitByIndex(traitIndex);
	     }
}

/// <summary>
/// Cosmetic features that are influenced by genetics but managed independently.
/// These change throughout life based on age, maintenance, and personal choice.
/// </summary>
[System.Serializable]
public class CosmeticFeatures
{

	     // Facial hair (for those with genetic capacity)
	     public int stubbleLevel = 0; // 0-5: clean shaven - heavy stubble
	     public int moustacheStyle = 0; // 0: none, 1-14 different styles

	     // Other cosmetics
	     public bool wearsMakeup = false;

	     // Constructor - initializes with genetic defaults
	     public CosmeticFeatures()
	     {
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
				stubbleLevel = 0;
				moustacheStyle = 0;
				return;
		          }

		          // For XY individuals, check if they present as masculine
		          if (pepo.ssd != null && pepo.ssd.presentsMasc)
		          {
				Debug.Log($"[PepoManager] Initializing facial hair for {PepoUtilities.GetFullName(pepo)}");
				// 70% chance to have some stubble if presenting masculine
				if (Random.value < 0.7f)
				{
					     stubbleLevel = Random.Range(1, 6);
					     // 60% chance of having a moustache if they have stubble
					     if (stubbleLevel >= 2 && Random.value < 0.6f)
					     {
						          moustacheStyle = Random.Range(1, 16); // 1-15 moustache styles
					     }
				}
				else
				{
					     //might still have a moustache even if no stubble
					     moustacheStyle = (Random.value < 0.3f) ? Random.Range(1, 16) : 0;
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
						          int style = relative.body.cosmeticFeatures.moustacheStyle;
						          int stubble = relative.body.cosmeticFeatures.stubbleLevel;
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
				stubbleLevel = Mathf.Min(3, stubbleLevel + Random.Range(0, 2));
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
					     stubblePref.Add(kvp.Key, (int)(kvp.Value * (pepo.values.Find(v => v.name == "conformity").value) + pepo.values.Find(v => v.name == "tradition").value));
					     //counterweight by self-direction and stimulation
					     stubblePref[kvp.Key] -= (int)(kvp.Value * (pepo.values.Find(v => v.name == "agency").value) + pepo.values.Find(v => v.name == "stimulation").value);
				}
				foreach (var kvp in styleCounts)
				{
					     //weight by conformity and tradition
					     moustachePref.Add(kvp.Key, (int)(kvp.Value * (pepo.values.Find(v => v.name == "conformity").value) + pepo.values.Find(v => v.name == "tradition").value));
					     //counterweight by self-direction and stimulation
					     moustachePref[kvp.Key] -= (int)(kvp.Value * (pepo.values.Find(v => v.name == "agency").value) + pepo.values.Find(v => v.name == "stimulation").value);
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
					     stubbleLevel = maxStubble;
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
					     moustacheStyle = maxStyle;

				}
				// Facial hair styles influenced by personality and experimentation
				if (experimentation > 0.6f && pepo.values.Find(v => v.name == "agency").value > 5 && Random.value < 0.3f)
				{
					     stubbleLevel = Random.Range(0, 6); // Try different levels
					     moustacheStyle = Random.Range(0, 16); // Try different styles
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
	     public int brows = Random.Range(1, 7);
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

