using System.Collections.Generic;
using System.Linq;

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

	     public Dictionary<PepoDataRoles, List<PepoData>> roleAssignments = new Dictionary<PepoDataRoles, List<PepoData>>();
	     public int populationChange => AnnualBirths - AnnualDeaths;



	     public PepoPopulation(int cohortSize, int cohortSeeds = 2)
	     {
		          ChromosomeData[] adamData = BodyFactory.CreateChromosomesFromAlleles(BodyLibrary.Default.Alleles.ToList());
		          ChromosomeData[] eveData = GeneticsUtilities.RandomiseGeneticValues(adamData);
		          if (cohortSize >= 1)
		          {
				for (int i = 0; i <= cohortSeeds; i++)
				{

					     //we want to create our adam and eve. we intialise a random pepo, then we clone it and flip the gender, then we mutate them both a whole bunch to have two distinct people that share the same genetic structure.					     
					     adamData = GeneticsUtilities.RandomiseGeneticRange(adamData);
					     adamData = GeneticsUtilities.RandomiseGeneticValues(adamData);
					     eveData = GeneticsUtilities.RandomiseGeneticRange(eveData);
					     eveData = GeneticsUtilities.RandomiseGeneticValues(eveData);

					     for (int j = 0; j < cohortSize; j++)
					     {
						          //int mutations = 5;
						          float mutationRate = .999f;
						          //   PepoData Adam = PepoFactory.CreateRandom(adamData);
						          //   PepoData Eve = PepoFactory.CreateRandom(eveData);

						          PepoData pepo = PepoFactory.CreateRandom(GeneticsUtilities.ExpressChromosomes(eveData, adamData));

						          //each pepo is a child of adam and eve that we mutate. after we generate apepo, we also randomise adam and eves genetic values.
						          //mutate the clone between 1 and 5 times
						          //    adamData = GeneticsUtilities.Mutate(adamData, mutationRate);
						          //  Adam.body.patChromosomes = GeneticsUtilities.Mutate(Adam.body.patChromosomes, mutationRate);
						          GeneticsUtilities.RandomiseGeneticRange(adamData);
						          GeneticsUtilities.RandomiseGeneticValues(adamData);

						          // Eve.body.matChromosomes = GeneticsUtilities.Mutate(Eve.body.matChromosomes, mutationRate);
						          // Eve.body.patChromosomes = GeneticsUtilities.Mutate(Eve.body.patChromosomes, mutationRate);
						          GeneticsUtilities.RandomiseGeneticRange(eveData);
						          GeneticsUtilities.RandomiseGeneticValues(eveData);

						          population.Add(pepo);
					     }
				}
		          }
	     }

	     //remove dead pepos from role assignments
	     public void RemovePepoFromRoles(PepoData pepo)
	     {
		          foreach (var role in roleAssignments.Keys)
		          {
				if (roleAssignments[role].Contains(pepo))
				{
					     roleAssignments[role].Remove(pepo);
				}
		          }
	     }
}
