using System.Collections.Generic;
using UnityEngine;

public class PepoDataPregnancy
{

}

public class PregnancyData
{
	     public float gestation = 0f; //1 = 1 year. listed as time left until birth
	     public int foetus = 0;
	     public bool identical = false;
	     public bool aware = false;
	     public bool inLabour = false;
	     public PepoData babyDaddy => bairns[0].father;

	     public List<PepoData> bairns;

	     public bool stillbirth = false;
	     public float danger = 0f;
	     public float weeks => 40 - (gestation * 52);
	     public List<int> conception; //date of conception as per simulation.constants format

	     public PregnancyData(PepoData bd, PepoData bm, bool Identical = false)
	     {
		          gestation = 0.77f;
		          PepoData baba = PepoFactory.InitialiseFromParents(bm, bd);

		          baba.mother = bm;
		          baba.father = bd;
		          bairns = new List<PepoData>() { baba };
		          if (Identical)
		          {
				bairns.Add(PepoFactory.ClonePepo(baba));
		          }
		          conception = SimulationConstants.TimeConstants;
	     }
	     public List<PepoData> GiveBirth(PepoData mama, bool safe = true)
	     {
		          if (conception == null)
		          {
				return null;
		          }
		          conception = null;

		          //PepoData baba = new PepoData(false, 0, 1, 1, mama, babyDaddy);
		          foreach (PepoData baba in bairns)
		          {
				baba.isAlive = true;
				if (!safe)
				{
					     MaternalHealth(danger);
					     FoetalHealth(danger);
					     if (stillbirth)
					     {
						          baba.InitialiseDeath();
						          MaternalHealth(danger);
					     }
					     if (Random.value < danger)
					     {
						          mama.InitialiseDeath();
					     }
				}
		          }
		          NameBabies(PepoUtilitiesNames.GetPepoNames(PepoUtilitiesBonds.ShipsToPepos(mama.RelationshipLibrary[RelationshipType.offspring])));
		          LearnPregnancy(mama);
		          PepoLogUtilities.LogBirthEvent(bairns);
		          gestation = 0f;
		          aware = false;
		          inLabour = false;
		          danger = danger * 0.1f;
		          var b = new List<PepoData>(bairns);
		          bairns.Clear();
		          return b;
	     }
	     public void FoetalHealth(float still)
	     {
		          stillbirth = (Random.value < still) ? true : false;
	     }
	     public void MaternalHealth(float mat)
	     {
		          if (Random.value < mat)
		          {
				danger += mat;
				Debug.LogWarning($"[PepoManager]{PepoUtilities.GetFullName(bairns[0].mother)} is encountering pregnancy difficulties at {40 - weeks:0} weeks");
		          }
	     }
	     public void LearnPregnancy(PepoData parent)
	     {
		          if (!aware)//at the moment we can't use this to tell dads about pregnancy
		          {
				if (gestation < 0f)
				{
					     Debug.Log($"[PepoManager]{PepoUtilities.GetFullName(parent)} had a baby {weeks:0} weeks ago");
				}
				else
				{
					     Debug.Log($"[PepoManager]{PepoUtilities.GetFullName(parent)} learned they were having a baby in about {40 - weeks:0} weeks");
				}
				foreach (var bairn in bairns)
				{
					     PepoFactoryBonds.CreateParentalBond(parent, bairn);
				}
		          }

	     }
	     public void NameBabies(List<string> exclude = null, List<string> required = null)
	     {
		          foreach (var bairn in bairns)
		          {
				PepoUtilitiesNames.InitialiseForname(bairn, exclude);
		          }
	     }

	     public void Progress(float progress, float matMort = 0, float still = 0, float gestPeriod = 0.77f)
	     {
		          //takes a float in years and progresses the pregnancy by that amount
		          gestation -= progress;
		          foreach (var bairn in bairns)
		          {
				bairn.age += Random.Range(progress * 0.9f, progress * 1.1f);
		          }
		          MaternalHealth(matMort * gestPeriod * progress);
		          FoetalHealth(still * gestPeriod * progress);
		          if (gestation < 0)
		          {
				inLabour = true;
		          }
	     }
}
