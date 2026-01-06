using System.Collections.Generic;

public class PepoPopulation
{
	     public List<PepoData> totalPop => PepoUtilitiesPopulation.GetAlivePepos(population);
	     public List<PepoData> childbearing => PepoUtilitiesPopulation.GetChildbearingPepos(population);
	     public List<PepoData> pregnancies => PepoUtilitiesPopulation.GetPregnantPepos(population);
	     public List<PepoData> infants => PepoUtilitiesPopulation.GetPeposAtAge(population, -3f, 1f);
	     public List<PepoData> toddlers => PepoUtilitiesPopulation.GetPeposAtAge(population, 2f, 4f);
	     public List<PepoData> children => PepoUtilitiesPopulation.GetPeposAtAge(population, 5f, 9f);
	     public List<PepoData> preteens => PepoUtilitiesPopulation.GetPeposAtAge(population, 10f, 12f);
	     public List<PepoData> adolescents => PepoUtilitiesPopulation.GetPeposAtAge(population, 11f, 20f);
	     public List<PepoData> fertileAged => PepoUtilitiesPopulation.GetPeposAtAge(population, 15f, 45f);
	     public List<PepoData> adults => PepoUtilitiesPopulation.GetPeposAtAge(population, 18f, 150f);
	     public List<PepoData> elderly => PepoUtilitiesPopulation.GetPeposAtAge(population, 70f, 150f);

	     public List<PepoData> population = new List<PepoData>();
	     public int AnnualBirths = 0;
	     public int AnnualDeaths = 0;

	     public int populationChange => AnnualBirths - AnnualDeaths;

	     public PepoPopulation(int size)
	     {
		          for (int i = 0; i < size; i++)
		          {
				population.Add(PepoFactory.InitialiseRandom());
		          }
	     }
}

