using System.Collections.Generic;
using UnityEngine;

public static class PepoSystemAgeing
{
	     public static void InitialiseAging(PepoData pepo, float years = 0, float pubertyAge = 13, float menopauseAge = 45)
	     {
		          pepo.age += years;
		          if (pepo.age >= pubertyAge + 4 && pepo.ssd.isPrepubescent)
		          {
				InitialisePuberty(pepo.age, pepo);
		          }
		          if (pepo.ssd.isPrepubescent && pepo.ssd.isFertile && pepo.age >= pubertyAge - 3 && pepo.age < menopauseAge && Random.value > 0.1f * years)
		          {
				PepoSystemAgeing.InitialisePuberty(pubertyAge, pepo);
		          }
		          else if (pepo.body.paternalChromosome.sexChromosome == "XX" && pepo.ssd.isFertile && pepo.age > menopauseAge && Random.value > 0.1f * years)
		          {
				Debug.Log($"[PepoManager]{PepoUtilities.GetFullName(pepo)} has hit menopause");
				pepo.ssd.isFertile = false;
		          }

		          //        if (pepo.womb != null && pepo.womb.Count > 0)
		          //        {
		          //foreach (var p in pepo.womb)
		          //{
		          //	     float timeleft = p.gestation;
		          //	     if (p.gestation < years)
		          //	     {
		          //		          p.Progress(timeleft);
		          //		          foreach (var bairn in p.bairns)
		          //		          {
		          //				bairn.age += years - timeleft;
		          //		          }
		          //	     }
		          //}
		          //        }

	     }
	     public static void InitialisePuberty(float pubertyAge, PepoData pepo)
	     {
		          pepo.ssd.isFertile = true;
		          pepo.ssd.isPrepubescent = false;
		          if (pepo.body.paternalChromosome.sexChromosome == "XX")
		          {
				pepo.womb = new List<PregnancyData>();
		          }
		          Debug.Log($"[PepoManager]{PepoUtilities.GetFullName(pepo)} has hit puberty. They are a {PepoLogUtilities.GetGenderExpression(pepo)}");
	     }
}
