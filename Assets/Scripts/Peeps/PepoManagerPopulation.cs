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

		          Debug.Log($"[PepoManager] Initial population: {Pepos.population.Count} total, {mamas.Count} mamas, {papas.Count} papas");

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
	     public float PairMamas(int attempts = 5)
	     {
		          bool match = false;
		          int i = 0;
		          int f = 0; //failed to find a match.
		          foreach (var mum in mamas)
		          {
				if ((mum.lovers == 0 || !mum.ssd.isMonogamous) && mum.womb != null)
				{
					     foreach (var guy in papas)
					     {
						          if (!PepoUtilitiesBonds.ShipsToPepos(mum.RelationshipLibrary[RelationshipType.lover]).Contains(guy) && !PepoUtilitiesBonds.ShipsToPepos(mum.RelationshipLibrary[RelationshipType.offspring]).Contains(guy) && !PepoUtilitiesBonds.ShipsToPepos(mum.RelationshipLibrary[RelationshipType.parent]).Contains(guy) && !PepoUtilitiesBonds.ShipsToPepos(mum.RelationshipLibrary[RelationshipType.kin]).Contains(guy) && !PepoUtilitiesBonds.ShipsToPepos(mum.RelationshipLibrary[RelationshipType.incompatible]).Contains(guy) && i <= attempts)
						          {//check if we're related by an int of our average conscientousness scores
								float c = (mum.personality.Conscientiousness.value + guy.personality.Conscientiousness.value) / 2;
								if (guy.isAlive && !PepoSystemMatchmaking.AreRelated(mum, guy, ((int)c)))
								{

									     if (PepoSystemMatchmaking.AmIAttracted(mum, guy, (int)mum.personality.Conscientiousness.value))
									     {
										          PepoFactoryBonds.MutualRelationship(mum, guy, RelationshipType.lover);
										          match = true;
										          break;
									     }

								}
								else
								{
									     //add him to my list of kin
									     PepoFactoryBonds.MutualRelationship(mum, guy, RelationshipType.kin);
									     i++;
								}
						          }
					     }
					     if (!match)
					     {
						          f++;
					     }
				}
		          }
		          return f;
	     }
	     private List<PepoData> ProcessGeneration()
	     {
		          var newGeneration = new List<PepoData>();
		          List<PepoData> losses = new List<PepoData>();

		          if (mamas.Count > 0 && papas.Count > 0)
		          {
				if (PairMamas() > (mamas.Count / 2))
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
					     mum.ssd.potBabyDaddies.AddRange(PepoUtilitiesBonds.ShipsToPepos(mum.RelationshipLibrary[RelationshipType.lover]));
					     bool multipleBirth = Random.value < multipleBirthChance;
					     bool identicalTwins = Random.value < identicalTwinsChance;
					     int numSiblings = Random.Range(0, maxSiblings);
					     mum.InitialisePregnancy(multipleBirth ? numSiblings : 1, null, identicalTwins);
					     mum.ssd.isFertile = false; // Set fertility to false after conceiving
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