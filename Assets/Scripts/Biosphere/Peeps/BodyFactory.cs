using System.Collections.Generic;
using UnityEngine;

public static class BodyFactory
{
	     public static BodyData CombineGeneticCode(BodyData mother, BodyData father)
	     {
		          ChromosomeData[] matSet = SelectRandomChromosomeSet(mother);
		          ChromosomeData[] patSet = SelectRandomChromosomeSet(father);
		          return new BodyData(matSet, patSet);   // ctor does expression + collate
	     }

	     private static ChromosomeData[] SelectRandomChromosomeSet(BodyData parent)
	         => (Random.value > 0.5f) ? parent.matChromosomes : parent.patChromosomes;

	     public static ChromosomeData[] CreateChromosomesFromAlleles(List<AlleleData> genes)
	     {
		          if (genes == null || genes.Count == 0)
				return new ChromosomeData[0];

		          // Factors returns rows of factor pairs (a*b == genes.Count)
		          int[,] factors = MathsUtilities.Factors(genes.Count);
		          int pairCount = factors.GetLength(0);

		          if (pairCount == 0)
				return new ChromosomeData[0];

		          // Randomly choose the species' chromosomal structure from its possible factor pairs.
		          int pair = Random.Range(0, pairCount);
		          int allelesPerChromosome = factors[pair, 0];
		          int ChromosomeCount = factors[pair, 1];

		          var chromosomes = new ChromosomeData[ChromosomeCount];
		          for (int c = 0; c < ChromosomeCount; c++)
		          {
				var alleles = new AlleleData[allelesPerChromosome];
				for (int a = 0; a < allelesPerChromosome; a++)
					     alleles[a] = genes[c * allelesPerChromosome + a];

				chromosomes[c] = new ChromosomeData
				{
					     name = $"Chromosome{c + 1}",
					     alleles = alleles
				};
		          }

		          return chromosomes;
	     }
}