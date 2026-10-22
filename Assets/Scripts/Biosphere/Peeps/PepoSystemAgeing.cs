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
		          else if (pepo.body.physicalAttributes["SexChromosome"].descriptor == "XX" && pepo.ssd.isFertile && pepo.age > menopauseAge && Random.value > 0.1f * years)
		          {
				Debug.Log($"[PepoManager]{PepoUtilities.GetFullName(pepo)} has hit menopause");
				pepo.ssd.isFertile = false;
		          }
		          else if (pepo.body.physicalAttributes["SexChromosome"].descriptor == "XY" && pepo.ssd.presentsMasc)
		          {
				pepo.body.cosmeticFeatures.UpdateFacialHair(pepo);
		          }
		          pepo.body.cosmeticFeatures.UpdateHair(pepo);
	     }

	     public static void InitialisePuberty(float pubertyAge, PepoData pepo)
	     {
		          pepo.ssd.isFertile = true;
		          pepo.ssd.isPrepubescent = false;

		          if (pepo.body.physicalAttributes["SexChromosome"].descriptor == "XX")
		          {
				pepo.womb = new List<PregnancyData>();
		          }
		          else if (pepo.body.physicalAttributes["SexChromosome"].descriptor == "XY")
		          {
				// Initialize facial hair for XY individuals at puberty
				pepo.body.cosmeticFeatures.InitializeStubbleAndMoustache(pepo);
		          }

		          Debug.Log($"[PepoManager]{PepoUtilities.GetFullName(pepo)} has hit puberty. They are a {PepoLogUtilities.GetGenderExpression(pepo)}");
	     }
}
