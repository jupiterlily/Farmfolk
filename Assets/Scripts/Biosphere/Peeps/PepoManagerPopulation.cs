using System.Collections.Generic;
using UnityEngine;

public class PepoPopManager : MonoBehaviour
{
	     [Header("Population Settings")]
	     public int initialPepos = 12;
	     public int generationSteps = 15;
	     public int maxPepos = 50;
	     public int minPopulationThreshold = 10;
	     public int immigrantCount = 5;

	     [Header("Birth Settings")]
	     public float multipleBirthChance = 0.02f;
	     public float identicalTwinsChance = 0.004f;
	     public float maternalMortalityChance = 0.008f;
	     public float stillbirthChance = 0.02f;
	     public int maxSiblings = 4;

	     [Header("Age Settings")]
	     public float childbearingMinAge = 16f;
	     public float childbearingMaxAge = 45f;
	     public float agingIncrement = 4f;
	     public float gestationalPeriod = 0.77f;

	     public int amountOfPeopleToHaveExisted = 0;
	     public int generations = 0;

	     public PepoPopulation Pepos;
	     public List<PlaceData> placeDatas;

	     private List<PepoData> Ancestors = new List<PepoData>();
	     // private List<PepoData> Pepos.population = new List<PepoData>();
	     private List<PepoData> mamas = new List<PepoData>();
	     private List<PepoData> papas = new List<PepoData>();
	     public UIManager uiManager;
	     void Start()
	     {
		          maxSiblings = (int)Mathf.Round(agingIncrement * 0.8f);
		          InitialisePopulation();
		          SimulateGenerations();
		          generationSteps = 1;
		          //foreach pepo in Pepos.population, create a ui element for them and add it to the scroller as a child

	     }

	     private void InitialisePopulation()
	     {
		          Pepos = new PepoPopulation(initialPepos);
		          //        foreach (var pepo in Pepos.population)
		          //        {
		          //int number = Random.Range(1, 5);
		          //pepo.body.maternalChromosome.eyeColor = number;
		          //pepo.body.paternalChromosome.eyeColor = number;
		          //pepo.body.ExpressChromosomes();
		          //        }
		          Debug.Log($"[PepoManager] Initial population: {Pepos.population.Count} total, {mamas.Count} mamas, {papas.Count} papas");
		          PepoRoleSystem.AssignRolesToPepoPopulation(Pepos, PepoLibraryRoles.HunterGatherer);
	     }

	     public void SimulateGenerations()
	     {
		          for (int gen = 0; gen < generationSteps; gen++)
		          {
				generations++;
				Debug.Log($"[DemographicsManager] Starting generation {generations}");

				var newGeneration = ProcessGeneration();
				Pepos.population.AddRange(newGeneration);
				amountOfPeopleToHaveExisted += newGeneration.Count;

				float deathtoll = 0f;//ProcessDeaths();
				var toKill = new List<PepoData>();
				//whilst there are more lil gguys than are allowed in the sim, process deaths and add to the deathtoll.
				int popCheck = 0;
				int initialPop = Pepos.population.Count;
				//if number of pepos is more than double the maximum we shold randomly kill 25% of people
				while (initialPop > maxPepos * 2)
				{
					     foreach (var pep in Pepos.population)
					     {
						          if (pep.isAlive && Random.value < 0.25f) // Only target alive pepos
						          {
								toKill.Add(pep);
								pep.InitialiseDeath();
								popCheck++;
						          }
					     }
					     initialPop -= toKill.Count;

				}
				while (initialPop > maxPepos)
				{
					     toKill.AddRange(PepoUtilities.KillPepos(Pepos.population, agingIncrement));
					     popCheck += toKill.Count;
					     deathtoll += toKill.Count;
					     initialPop -= toKill.Count;
				}
				if (popCheck > 0)
				{
					     HandleDeceased(toKill);
					     Debug.LogWarning($"[DemographicsManager] There's been {popCheck} more peeps die due to harsher conditions this year");
				}

				//PepoLogUtilities.LogPepoList(Pepos.population);
				HandleDeceased(PepoUtilities.KillPepos(Pepos.population, agingIncrement));
				CheckPopulationHealth();
				RebuildParentLists();
				PepoLogUtilities.LogDemographicData(Pepos.population);
				PepoRoleSystem.AssignRolesToPepoPopulation(Pepos, PepoLibraryRoles.HunterGatherer);
		          }
	     }
	     public void SimulateNeeds(float hours)
	     {
		          foreach (var pep in Pepos.population)
		          {
				PepoUtilitiesNeeds.ProgressNeedHours(hours, pep.needs);

				if (PepoLogUtilities.Needs(pep) == "")
				{
					     return;
				}
				else
				{
					     // Debug.Log($"{pep.fullname} is {PepoLogUtilities.Needs(pep)}");
				}

		          }
	     }

	     private List<PepoData> ProcessGeneration()
	     {
		          var newGeneration = new List<PepoData>();
		          List<PepoData> losses = new List<PepoData>();
		          //run introductions
		          foreach (var pep in Pepos.population)
		          {
				foreach (var p in PepoUtilitiesInteractions.MakeAcquaintences(pep, PepoUtilitiesPopulation.RemovePeposFromList(PepoUtilitiesPopulation.OthersInMyRoles(pep, Pepos), PepoUtilitiesBonds.ShipsToPepos(pep.RelationshipLibrary[RelationshipType.acquaintence])), generationSteps * 365 * 8))
				{
					     PepoUtilitiesBonds.AddRelation(pep, p, RelationshipType.peer);
				}
		          }
		          //run interactions

		          if (mamas.Count > 0 && papas.Count > 0)
		          {
				//if half of the mamas are unpaired, we bring in new people
				if (PepoUtilitiesPopulation.PairLovers(mamas, Pepos, 10) > (mamas.Count / 2))
				{
					     for (int i = 0; i < immigrantCount; i++)
					     {
						          Pepos.population.Add(PepoFactory.InitialiseRandom());
						          amountOfPeopleToHaveExisted++;
					     }
				}
				BreedPopulation(Mathf.Min(mamas.Count, papas.Count, Pepos.population.Count));
				HandleDeceased(losses);
		          }
		          else
		          {
				Debug.LogWarning($"[PepoManager] Not enough mamas ({mamas.Count}) or papas ({papas.Count}) for breeding");
				for (int i = 0; i < immigrantCount; i++)
				{
					     Pepos.population.Add(PepoFactory.InitialiseRandom());
					     amountOfPeopleToHaveExisted++;
				}
		          }
		          AgePopulation();
		          return newGeneration;
	     }

	     public void BreedPopulation(int maxPregnancies)
	     {//concieves babies for a set number of mamas
		          List<PepoData> mums = new List<PepoData>(mamas);

		          for (int i = 0; i <= maxPregnancies && mums.Count > 0 && i < mums.Count; i++)
		          {
				int m = Random.Range(0, mums.Count);
				PepoData mum = mums[m];
				//if mum is not nill and has potential baby daddies and is fertile and not already pregnant
				if (mum != null && (mum.ssd.potBabyDaddies.Count >= 1 || mum.lovers >= 1) && mum.ssd.isFertile && mum.womb.Count <= 0)
				{
					     mum.ssd.potBabyDaddies.AddRange(PepoUtilitiesPopulation.GetImpregnatingPepos(PepoUtilitiesBonds.ShipsToPepos(mum.RelationshipLibrary[RelationshipType.lover])));
					     if (mum.ssd.potBabyDaddies.Count > 0)
					     {
						          bool multipleBirth = Random.value < multipleBirthChance;
						          bool identicalTwins = Random.value < identicalTwinsChance;
						          int numSiblings = Random.Range(0, maxSiblings);
						          mum.InitialisePregnancy(multipleBirth ? numSiblings : 1, null, identicalTwins);
						          mum.ssd.isFertile = false; // Set fertility to false after conceiving
					     }
				}
				mums.RemoveAt(m); // Remove the processed mum to avoid reprocessing
		          }
	     }

	     public void AgePopulation()
	     {//ads time to a population ie tells them to age and adds any new babies to the population
		          List<PepoData> newBabies = new List<PepoData>(); // Collect new births here

		          foreach (var Pepo in Pepos.population)
		          {
				if (Pepo.isAlive)
				{//age the pepo
					     PepoSystemAgeing.InitialiseAging(Pepo, agingIncrement);
					     if (Pepo.womb != null)
					     {//progress any pregnancies they have by the aging increment
						          bool finalise = false;
						          int pregnancyCount = Pepo.womb.Count;
						          foreach (var pregnancy in Pepo.womb)
						          {

								pregnancy.Progress(agingIncrement, maternalMortalityChance, stillbirthChance, gestationalPeriod);
								if (pregnancy.inLabour)
								{//give birth and collect new babies
									     newBabies.AddRange(pregnancy.GiveBirth(Pepo, false)); // Collect instead of adding directly
									     finalise = true;
								}
						          }
						          // Complete pregnancies in reverse order to avoid index issues	
						          if (finalise)
						          {
								for (int p = pregnancyCount - 1; p >= 0; p--)
								{
									     Pepo.CompletePregnancy(Pepo.womb[p]);
								}
						          }

					     }
				}
		          }
		          Pepos.population.AddRange(newBabies); // Add all new babies after the iteration

	     }

	     private void RebuildParentLists()
	     {
		          mamas.Clear();
		          papas.Clear();

		          List<PepoData> maybeMoms = PepoUtilitiesPopulation.RemoveDuplicatePepos(Pepos.childbearing, Pepos.pregnancies);
		          List<PepoData> maybeDads = PepoUtilitiesPopulation.RemoveDuplicatePepos(Pepos.adults, Pepos.childbearing);

		          mamas.AddRange(PepoUtilitiesPopulation.GetWillingParentPepos(maybeMoms));
		          papas.AddRange(PepoUtilitiesPopulation.GetWillingParentPepos(maybeDads));



		          //        foreach (var pepo in Pepos.mamas)
		          //        {
		          //if (!pepo.isAlive || !pepo.ssd.isFertile || pepo.ssd.wantsChildren == 0)
		          //	     continue;

		          //// Check if of childbearing age
		          //bool isChildbearingAge = pepo.age > childbearingMinAge && pepo.age < childbearingMaxAge;

		          //if (pepo.ssd.hasY && pepo.age > childbearingMinAge)
		          //{
		          //	     // Males - just need to be above min age
		          //	     papas.Add(pepo);
		          //}
		          //else if (!pepo.ssd.hasY && isChildbearingAge)
		          //{
		          //	     // Females - check if not currently pregnant or has empty womb
		          //	     bool isNotPregnant = pepo.womb == null || pepo.womb.Count == 0 ||
		          //		          !pepo.womb.Any(p => p != null && !p.aware);

		          //	     if (isNotPregnant)
		          //	     {
		          //		          mamas.Add(pepo);
		          //	     }
		          //}
		          //        }
		          papas = PepoUtilitiesPopulation.OrderByHexaco(papas, 1);
		          mamas = PepoUtilitiesPopulation.OrderByHexaco(mamas, 4);
	     }
	     private void HandleDeceased(List<PepoData> losses)
	     {//removes dead pepos from the population lists . adds them to a list of ancestors which will be used for genealogy
		          foreach (var dead in losses)
		          {
				Pepos.population.Remove(dead);
				mamas.Remove(dead);
				papas.Remove(dead);
				Ancestors.Add(dead);
				Pepos.RemovePepoFromRoles(dead);
		          }

	     }

	     private void CheckPopulationHealth()
	     {//here is where we call for immigration, emigration and if we need to cull the pop
		          if (Pepos.population.Count < minPopulationThreshold)
		          {
				Debug.LogWarning("[PepoManager] Population critically low! Adding new Pepos.");
				for (int i = 0; i < minPopulationThreshold - Pepos.population.Count; i++)
				{
					     Pepos.population.Add(PepoFactory.InitialiseRandom());
					     amountOfPeopleToHaveExisted++;
				}
		          }
		          //go through each guy and if they're dead, remove them from the lsit
		          List<PepoData> losses = new List<PepoData>();
		          foreach (var dead in Pepos.population)
		          {
				if (!dead.isAlive)
				{
					     losses.Add(dead);
					     Debug.LogWarning($"[PepoManager] {PepoUtilities.GetFullName(dead)} was still on the list of Pepos.population");
				}
		          }
		          HandleDeceased(losses);
	     }

}