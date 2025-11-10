using System.Collections.Generic;
using UnityEngine;

public class SocioSexualData
{

	     public bool isFertile = false;
	     public bool isPrepubescent = false;
	     public bool hasY = false; //can impregnate
	     public bool hasX = true; //can have a womb
	     public int wantsChildren = 0;

	     public string identity = string.Empty;

	     public bool isFemme = false; //gender identity internally
	     public bool isMasc = false;

	     public bool presentsFemme = false; //gender presentation
	     public bool presentsMasc = false;

	     public bool likesFemme = false; //sexuality
	     public bool likesMasc = false;

	     public bool platonicFemme = false; //who i will befriend
	     public bool platonicMasc = false;

	     public bool isMonogamous = false;
	     public bool isAsexual = true;
	     public bool isAromantic = true;
	     public bool isAplatonic = false;

	     public List<PepoData> potBabyDaddies = new List<PepoData>();
	     public SocioSexualData()
	     {
		          hasY = (Random.value > 0.5f) ? true : false;
		          hasX = (Random.value > 0.01f) ? true : false;
		          isFertile = (Random.value > 0.01f || (hasX && hasY) || (hasX && !hasY)) ? true : false; //allows intersex
		          isPrepubescent = true;
		          wantsChildren = Random.Range(0, 7);
		          isMonogamous = (Random.value > 0.3f) ? true : false;
		          isAsexual = (Random.value < 0.3f) ? true : false;
		          isAromantic = (Random.value < 0.3f) ? true : false;
		          isAplatonic = (Random.value < 0.7f) ? true : false;

		          if (hasY)
		          {
				isMasc = (Random.value > 0.05f) ? true : false;
				isFemme = (Random.value > 0.7f) ? true : false;
				likesMasc = (Random.value > 0.5f) ? true : false;
				likesFemme = (Random.value > 0.1f) ? true : false;
				presentsMasc = (Random.value > 0.05f) ? true : false;
				presentsFemme = (Random.value > 0.7f) ? true : false;
		          }
		          else
		          {
				isMasc = (Random.value > 0.7f) ? true : false;
				isFemme = (Random.value > 0.05f) ? true : false;
				likesMasc = (Random.value > 0.1f) ? true : false;
				likesFemme = (Random.value > 0.5f) ? true : false;
				presentsMasc = (Random.value > 0.7f) ? true : false;
				presentsFemme = (Random.value > 0.05f) ? true : false;
		          }
	     }
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
		          PepoData baba = new PepoData(false, gestation * -1, 0, 0, bm, bd);
		          bd.offspring.Add(baba);
		          bm.offspring.Add(baba);
		          baba.mother = bm;
		          baba.father = bd;
		          bairns = new List<PepoData>() { baba };
		          if (Identical)
		          {
				bairns.Add(baba.ClonePepo());
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
				PepoLogUtilities.LogPepo(baba);
		          }
		          NameBabies(PepoNameUtilities.GetPepoNames(PepoRelationalUtilities.ShipsToPepos(mama.RelationshipLibrary[RelationshipType.offspring])));
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
		          }
	     }
	     public void LearnPregnancy(PepoData parent)
	     {
		          if (!aware)
		          {
				Debug.Log($"[PepoManager]{PepoUtilities.GetFullName(parent)} learned they were having a baby in about {40 - weeks:0} weeks");
				foreach (var bairn in bairns)
				{
					     PepoRelationalBonds.CreateParentalBond(parent, bairn);
				}
		          }

	     }
	     public void NameBabies(List<string> exclude = null, List<string> required = null)
	     {
		          foreach (var bairn in bairns)
		          {
				PepoNameUtilities.InitialiseForname(bairn, exclude);
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
		          MaternalHealth(matMort * gestPeriod);
		          FoetalHealth(still * gestPeriod);
		          if (gestation < 0)
		          {
				inLabour = true;
		          }
	     }
}