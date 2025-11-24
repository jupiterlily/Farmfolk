using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BodyData
{
	     public Chromosome maternalChromosome;
	     public Chromosome paternalChromosome;
	     public List<int> expressedChromosomes;
	     public BodyData()
	     {
		          // Create maternal chromosome (always XX for mother)
		          maternalChromosome = new Chromosome("XX");

		          // Create paternal chromosome (randomly XX or XY for father)
		          paternalChromosome = new Chromosome(Random.value > 0.5f ? "XX" : "XY");
	     }

	     public BodyData(Chromosome mat, Chromosome pat)
	     {
		          maternalChromosome = mat;
		          paternalChromosome = pat;
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
				expressedChromosomes.Add(Random.Range(maternalChromosome.GetTraitByIndex(11), paternalChromosome.GetTraitByIndex(11)));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(12) : paternalChromosome.GetTraitByIndex(12));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(13) : paternalChromosome.GetTraitByIndex(13));
				//freckles - either or
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(14) : paternalChromosome.GetTraitByIndex(14));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(15) : paternalChromosome.GetTraitByIndex(15));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(16) : paternalChromosome.GetTraitByIndex(16));
				//get hair texture. 50% chance of being the higher value (curlyer hair) else it generates as a random value between the two
				expressedChromosomes.Add(Random.value > 0.5f ? Mathf.Max(maternalChromosome.GetTraitByIndex(17), paternalChromosome.GetTraitByIndex(17)) : Random.Range(Mathf.Min(maternalChromosome.GetTraitByIndex(17), paternalChromosome.GetTraitByIndex(17)), Mathf.Max(maternalChromosome.GetTraitByIndex(17), paternalChromosome.GetTraitByIndex(17))));

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

[System.Serializable]
public class Chromosome
{
	     // Sex chromosomes
	     public string sexChromosome = "XX";

	     // Physical traits
	     public int headShape = Random.Range(1, 6);//chose either or
	     public int hairEumelanin = Random.Range(1, 9);    // darkness, choose a range between parents
	     public int hairPheomelanin = Random.Range(1, 6);  // redness, choose a range between parents
	     public int eyeShape = Random.Range(1, 8);//choose  either or
	     public int eyeColor = Random.Range(1, 7);         // violet, blue, grey, green, brown
	     public int eyeDarkness = Random.Range(1, 4); //choose between arange from parents
	     public int eyeHeight = Random.Range(1, 4); //choose between arange from parents
	     public int noseShape = Random.Range(1, 11); //choose either or
	     public int noseHeight = Random.Range(1, 4); //choose between arange from parents
	     public int mouthShape = Random.Range(1, 10); //choose either or
	     public int mouthHeight = Random.Range(1, 4); //choose between arange from parents
	     public int skinDepth = Random.Range(1, 6); //use parents as a range
	     public int skinSaturation = Random.Range(1, 6); //use either or
	     public int skinHue = Random.Range(1, 6); //use either or
	     public int Freckles = Random.Range(1, 7); //choose either parent
	     public int invertFreckles = Random.Range(1, 3); //flips image on vertical axis
	     public int ExpressDoubleFreckles = Random.Range(1, 3); //makes us show the freckles from both parents
	     public int hairTexture = Random.Range(1, 5); //straight, wavy, curly, coily
	     public int height = Random.Range(1, 21); //overall height range of roughly 1.5m to 2.0m
	     public int massGenetics = Random.Range(50, 101); //overall mass range of roughly 50kg to 100kg

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
		          mouthShape = other.mouthShape;
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
		          return mouthShape;
		          case 10:
		          return mouthHeight;
		          case 11:
		          return skinDepth;
		          case 12:
		          return skinSaturation;
		          case 13:
		          return skinHue;
		          case 14:
		          return Freckles;
		          case 15:
		          return ExpressDoubleFreckles;
		          case 16:
		          return ExpressDoubleFreckles;
		          case 17:
		          return hairTexture;
		          case 18:
		          return height;
		          case 19:
		          return massGenetics;
		          default:
		          return -1;
		          }
	     }
}
