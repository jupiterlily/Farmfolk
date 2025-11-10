using System.Collections.Generic;
using System.Linq;
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

	     private List<PepoData> Ancestors = new List<PepoData>();
	     private List<PepoData> lilGuys = new List<PepoData>();
	     private List<PepoData> mamas = new List<PepoData>();
	     private List<PepoData> papas = new List<PepoData>();

	     void Start()
	     {
		          maxSiblings = (int)Mathf.Round(agingIncrement * 0.8f);
		          InitialisePopulation();
		          SimulateGenerations();
		          generationSteps = 1;
		          //foreach pepo in lilguys, create a ui element for them and add it to the scroller as a child

	     }

	     private void InitialisePopulation()
	     {
		          for (int i = 0; i < initialPepos; i++)
		          {
				CreateMigrantPepo(true);
				amountOfPeopleToHaveExisted++;
		          }
		          Debug.Log($"[PepoManager] Initial population: {lilGuys.Count} total, {mamas.Count} mamas, {papas.Count} papas");

	     }

	     public void SimulateGenerations()
	     {
		          for (int gen = 0; gen < generationSteps; gen++)
		          {
				generations++;
				Debug.Log($"[DemographicsManager] Starting generation {generations}");

				var newGeneration = ProcessGeneration();
				lilGuys.AddRange(newGeneration);
				amountOfPeopleToHaveExisted += newGeneration.Count;

				float deathtoll = 0f;//ProcessDeaths();
				var toKill = new List<PepoData>();
				//whilst there are more lil gguys than are allowed in the sim, process deaths and add to the deathtoll.
				int popCheck = 0;
				int initialPop = lilGuys.Count;
				//if number of pepos is more than double the maximum we shold randomly kill 25% of people
				while (initialPop > maxPepos * 2)
				{
					     foreach (var pep in lilGuys)
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
					     toKill.AddRange(PepoUtilities.KillPepos(lilGuys));
					     HandleDeceased(toKill);
					     popCheck += toKill.Count;
					     deathtoll += toKill.Count;
					     initialPop -= toKill.Count;
				}
				if (popCheck > 0)
				{
					     Debug.LogWarning($"[DemographicsManager] There's been {popCheck} more peeps die due to harsher conditions this year");
				}

				//PepoLogUtilities.LogPepoList(lilGuys);
				HandleDeceased(PepoUtilities.KillPepos(lilGuys));
				CheckPopulationHealth();
				RebuildParentLists();

				PepoLogUtilities.LogDemographicData(lilGuys);

		          }
	     }
	     public void PairMamas(int attempts = 5)
	     {
		          foreach (var mum in mamas)
		          {
				int tries = attempts;
				if ((mum.lovers == 0 || !mum.ssd.isMonogamous) && mum.womb != null)
				{
					     foreach (var guy in papas)
					     {
						          if (!PepoRelationalUtilities.ShipsToPepos(mum.RelationshipLibrary[RelationshipType.lover]).Contains(guy) && !PepoRelationalUtilities.ShipsToPepos(mum.RelationshipLibrary[RelationshipType.kin]).Contains(guy))
						          {
								if (guy.isAlive && PepoUtilities.AmIAttracted(mum, guy))
								{
									     //check if we're related by an int of our average conscientousness scores
									     float c = (mum.personality.Conscientiousness.value + guy.personality.Conscientiousness.value) / 2;
									     if (!PepoUtilities.AreRelated(mum, guy, ((int)c)))
									     {
										          PepoRelationalBonds.MutualRelationship(mum, guy, RelationshipType.lover);
										          break;
									     }
									     else
									     {
										          //add him to my list of kin
										          PepoRelationalBonds.MutualRelationship(mum, guy, RelationshipType.kin);
									     }
								}
						          }
					     }
				}
		          }
	     }
	     private List<PepoData> ProcessGeneration()
	     {
		          var newGeneration = new List<PepoData>();
		          List<PepoData> losses = new List<PepoData>();

		          if (mamas.Count > 0 && papas.Count > 0)
		          {
				PairMamas();
				BreedPopulation(Mathf.Min(mamas.Count, papas.Count, lilGuys.Count));
				HandleDeceased(losses);
		          }
		          else
		          {
				Debug.LogWarning($"[PepoManager] Not enough mamas ({mamas.Count}) or papas ({papas.Count}) for breeding");
				AddNewImmigrants(immigrantCount);
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
				if (mum != null && (mum.ssd.potBabyDaddies.Count >= 1 || mum.lovers >= 1))
				{
					     mum.ssd.potBabyDaddies.AddRange(PepoRelationalUtilities.ShipsToPepos(mum.RelationshipLibrary[RelationshipType.lover]));
					     bool multipleBirth = Random.value < multipleBirthChance;
					     bool identicalTwins = Random.value < identicalTwinsChance;
					     int numSiblings = Random.Range(0, maxSiblings);
					     mum.InitialisePregnancy(multipleBirth ? numSiblings : 1, null, identicalTwins);
				}
				mums.RemoveAt(m); // Remove the processed mum to avoid reprocessing
		          }
	     }

	     //  public void CreateChildren(PepoData mama, PepoData papa, List<PepoData> newGeneration, List<PepoData> losses)
	     //  {
	     //        bool multipleBirth = Random.value < multipleBirthChance;
	     //        bool identicalTwins = Random.value < identicalTwinsChance;
	     //        int numSiblings = Random.Range(0, maxSiblings);

	     //        List<PepoData> children = PepoUtilities.GenerateChildren(mama, papa, multipleBirth, identicalTwins, numSiblings, agingIncrement);
	     //        newGeneration.AddRange(children);
	     //        //add children to parents lists		          

	     //        PepoLogUtilities.LogBirthEvent(children, multipleBirth, identicalTwins);
	     //        //ProcessPregnancyHealth(mama, losses, children);
	     //        foreach (PepoData child in children)
	     //        {
	     //if (!child.isAlive)
	     //{
	     //	     losses.Add(child);
	     //}
	     //        }
	     //        if (!mama.isAlive)
	     //        {
	     //losses.Add(mama);
	     //        }
	     //        HandleDeceased(losses);
	     //        mamas.Remove(mama); // Mama takes a generation off
	     //  }

	     public void AgePopulation()
	     {//ads time to a population ie tells them to age and adds any new babies to the population
		          List<PepoData> newBabies = new List<PepoData>(); // Collect new births here

		          foreach (var Pepo in lilGuys)
		          {
				if (Pepo.isAlive)
				{
					     Pepo.InitialiseAging(agingIncrement);
					     if (Pepo.ssd.isPrepubescent && Pepo.ssd.isFertile && Pepo.age >= childbearingMinAge - 3 && Pepo.age < childbearingMaxAge && Random.value > 0.1f)
					     {
						          Pepo.InitialisePuberty(childbearingMinAge);
					     }
					     else if (!Pepo.ssd.hasY && Pepo.ssd.isFertile && Pepo.age > childbearingMaxAge && Random.value > 0.1f)
					     {
						          Debug.Log($"[PepoManager]{PepoUtilities.GetFullName(Pepo)} has hit menopause");
						          Pepo.ssd.isFertile = false;
					     }

					     if (Pepo.womb != null)
					     {
						          int pregnancyCount = Pepo.womb.Count;
						          foreach (var pregnancy in Pepo.womb)
						          {
								pregnancy.Progress(agingIncrement, maternalMortalityChance, stillbirthChance, gestationalPeriod);
								if (pregnancy.inLabour)
								{
									     newBabies.AddRange(pregnancy.GiveBirth(Pepo, false)); // Collect instead of adding directly

								}
						          }
						          for (int p = pregnancyCount - 1; p >= 0; p--)
						          {
								Pepo.CompletePregnancy(Pepo.womb[p]);
						          }

					     }
				}
		          }
		          lilGuys.AddRange(newBabies); // Add all new babies after the iteration

	     }

	     private void RebuildParentLists()
	     {
		          mamas.Clear();
		          papas.Clear();

		          foreach (var pepo in lilGuys)
		          {
				if (!pepo.isAlive || !pepo.ssd.isFertile || pepo.ssd.wantsChildren == 0)
					     continue;

				// Check if of childbearing age
				bool isChildbearingAge = pepo.age > childbearingMinAge && pepo.age < childbearingMaxAge;

				if (pepo.ssd.hasY && pepo.age > childbearingMinAge)
				{
					     // Males - just need to be above min age
					     papas.Add(pepo);
				}
				else if (!pepo.ssd.hasY && isChildbearingAge)
				{
					     // Females - check if not currently pregnant or has empty womb
					     bool isNotPregnant = pepo.womb == null || pepo.womb.Count == 0 ||
						          !pepo.womb.Any(p => p != null && !p.aware);

					     if (isNotPregnant)
					     {
						          mamas.Add(pepo);
					     }
				}
		          }
	     }
	     private void HandleDeceased(List<PepoData> losses)
	     {//removes dead pepos from the population lists . adds them to a list of ancestors which will be used for genealogy
		          foreach (var dead in losses)
		          {
				lilGuys.Remove(dead);
				mamas.Remove(dead);
				papas.Remove(dead);
				Ancestors.Add(dead);
		          }

	     }

	     private void CheckPopulationHealth()
	     {//here is where we call for immigration, emigration and if we need to cull the pop
		          if (lilGuys.Count < minPopulationThreshold)
		          {
				Debug.LogWarning("[PepoManager] Population critically low! Adding new Pepos.");
				AddNewImmigrants(minPopulationThreshold - lilGuys.Count);
		          }
		          //go through each guy and if they're dead, remove them from the lsit
		          List<PepoData> losses = new List<PepoData>();
		          foreach (var dead in lilGuys)
		          {
				if (!dead.isAlive)
				{
					     losses.Add(dead);
					     Debug.LogWarning($"[PepoManager] {PepoUtilities.GetFullName(dead)} was still on the list of lilGuys");
				}
		          }
		          HandleDeceased(losses);
	     }

	     private void CreateMigrantPepo(bool logGreeting = false)
	     {
		          var pd = new PepoData(true, 1, 1, 1);
		          pd.isAlive = true;

		          if (pd.ssd.hasY && pd.age > childbearingMinAge)
		          {
				papas.Add(pd);
		          }
		          else if (!pd.ssd.hasY && pd.age > childbearingMinAge && pd.age < childbearingMaxAge)
		          {
				mamas.Add(pd);
		          }

		          lilGuys.Add(pd);

		          if (logGreeting)
		          {
				string gender = pd.ssd.hasY ? "man" : "lady";
				Debug.Log($"<color=#f3c65f>[PepoManager] Hello {PepoUtilities.GetFullName(pd)}! A {gender} aged {pd.age:F0} has come to the area</color>");
		          }
	     }

	     private void AddNewImmigrants(int count)
	     {
		          for (int i = 0; i < count; i++)
		          {
				CreateMigrantPepo(true);
		          }
	     }

}