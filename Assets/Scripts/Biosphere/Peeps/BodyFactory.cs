using UnityEngine;

public static class BodyFactory
{
	     // Create offspring from two parents
	     public static BodyData CombineGeneticCode(BodyData mother, BodyData father)
	     {
		          // Each parent contributes one chromosome to the offspring
		          Chromosome matChromosome = SelectRandomChromosome(mother);
		          Chromosome patChromosome = SelectRandomChromosome(father);
		          BodyData body = new BodyData(matChromosome, patChromosome);
		          body.ExpressChromosomes();
		          return body;
	     }

	     // Helper method to randomly select one chromosome from a parent
	     private static Chromosome SelectRandomChromosome(BodyData parent)
	     {
		          return (Random.value > 0.5f)
		              ? new Chromosome(parent.maternalChromosome)
		              : new Chromosome(parent.paternalChromosome);
	     }

	     // Create a random individual (for initial population)
	     public static BodyData CreateRandomIndividual()
	     {
		          BodyData body = new BodyData();
		          body.ExpressChromosomes();
		          return body;
	     }
}