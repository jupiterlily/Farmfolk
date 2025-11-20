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
				//either ors, head shape 0, eye shape 3, nose shape 6, mouth shape 7, skin saturation 9, skin hue 10
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(0) : paternalChromosome.GetTraitByIndex(0));
				expressedChromosomes.Add(Random.Range(maternalChromosome.GetTraitByIndex(1), paternalChromosome.GetTraitByIndex(1)));
				expressedChromosomes.Add(Random.Range(maternalChromosome.GetTraitByIndex(2), paternalChromosome.GetTraitByIndex(2)));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(3) : paternalChromosome.GetTraitByIndex(3));
				expressedChromosomes.Add(Random.Range(maternalChromosome.GetTraitByIndex(4), paternalChromosome.GetTraitByIndex(4)));
				expressedChromosomes.Add(Random.Range(maternalChromosome.GetTraitByIndex(5), paternalChromosome.GetTraitByIndex(5)));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(6) : paternalChromosome.GetTraitByIndex(6));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(7) : paternalChromosome.GetTraitByIndex(7));
				expressedChromosomes.Add(Random.Range(maternalChromosome.GetTraitByIndex(8), paternalChromosome.GetTraitByIndex(8)));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(9) : paternalChromosome.GetTraitByIndex(9));
				expressedChromosomes.Add(Random.value > 0.5f ? maternalChromosome.GetTraitByIndex(10) : paternalChromosome.GetTraitByIndex(10));
		          }
		          else
		          {
				expressedChromosomes = XpressedChromosomes;
		          }
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
	     public int eyeShape = Random.Range(1, 5);//choose  either or
	     public int eyeColor = Random.Range(1, 5);         // blue, grey, green, brown
	     public int eyeDarkness = Random.Range(1, 4); //choose between arange from parents
	     public int noseShape = Random.Range(1, 6); //choose either or
	     public int mouthShape = Random.Range(1, 6); //choose either or
	     public int skinDepth = Random.Range(1, 6); //use parents as a range
	     public int skinSaturation = Random.Range(1, 6); //use either or
	     public int skinHue = Random.Range(1, 6); //use either or

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
		          noseShape = other.noseShape;
		          mouthShape = other.mouthShape;
		          skinDepth = other.skinDepth;
		          skinSaturation = other.skinSaturation;
		          skinHue = other.skinHue;
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
		          return noseShape;
		          case 7:
		          return mouthShape;
		          case 8:
		          return skinDepth;
		          case 9:
		          return skinSaturation;
		          case 10:
		          return skinHue;
		          default:
		          return 1;
		          }
	     }
}
