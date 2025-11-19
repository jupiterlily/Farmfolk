using UnityEngine;

[System.Serializable]
public class BodyData
{
	     public Chromosome maternalChromosome;
	     public Chromosome paternalChromosome;

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
}

[System.Serializable]
public class Chromosome
{
	     // Sex chromosomes
	     public string sexChromosome = "XX";

	     // Physical traits
	     public int headShape = Random.Range(1, 6);
	     public int hairEumelanin = Random.Range(1, 9);    // darkness
	     public int hairPheomelanin = Random.Range(1, 6);  // redness
	     public int eyeShape = Random.Range(1, 5);
	     public int eyeColor = Random.Range(1, 5);         // blue, grey, green, brown
	     public int eyeDarkness = Random.Range(1, 4);
	     public int noseShape = Random.Range(1, 6);
	     public int mouthShape = Random.Range(1, 6);
	     public int skinDepth = Random.Range(1, 6);
	     public int skinSaturation = Random.Range(1, 6);
	     public int skinHue = Random.Range(1, 6);

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
}
