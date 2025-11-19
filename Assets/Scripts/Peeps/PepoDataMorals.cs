using System.Collections.Generic;

// Base moral foundation class
public class MoralFoundation : PepoAspect
{
	     public string PositivePole;
	     public string NegativePole;
	     public float Sensitivity;
	     public List<string> Virtues;
	     public List<string> Vices;
	     public MoralFoundation(string name, string pos, string neg, float sensitivity)
	     {

	     }
	     protected float BaseAssessment(float actionWeight, bool alignsWithFoundation)
	     {
		          float assessment = actionWeight * Sensitivity;
		          return alignsWithFoundation ? assessment : -assessment;
	     }
}

// Care/Harm Foundation
public static class MoralFoundations
{
	     public static MoralFoundation CareHarm()
	     {
		          MoralFoundation CareHarm = new MoralFoundation("Care/Harm", "care", "harm", 0.5f);
		          CareHarm.Vices = new List<string> { "cruel", "indifferent", "hurtful", "neglectful", "vicious" };
		          CareHarm.Virtues = new List<string> { "compassionate", "nurturing", "protective", "kind", "empathetic" };
		          return CareHarm;
	     }
	     public static MoralFoundation FairCheat()
	     {
		          MoralFoundation FairnessCheating = new MoralFoundation("Care/Harm", "care", "harm", 0.5f);
		          FairnessCheating.Vices = new List<string> { "cheater", "unfair", "deceptive", "corrupt", "dishonest" };
		          FairnessCheating.Virtues = new List<string> { "fair", "just", "honest", "equitable", "trustworthy" };
		          return FairnessCheating;
	     }
	     public static MoralFoundation LoyaltyBetrayal()
	     {
		          MoralFoundation LoyaltyBetrayal = new MoralFoundation("Care/Harm", "care", "harm", 0.5f);
		          LoyaltyBetrayal.Vices = new List<string> { "traitor", "disloyal", "betrayer", "turncoat", "unfaithful" };
		          LoyaltyBetrayal.Virtues = new List<string> { "loyal", "patriotic", "faithful", "dedicated", "team-player" };
		          return LoyaltyBetrayal;
	     }
	     public static MoralFoundation AuthoritySubversion()
	     {
		          MoralFoundation AuthoritySubversion = new MoralFoundation("Care/Harm", "care", "harm", 0.5f);
		          AuthoritySubversion.Vices = new List<string> { "rebellious", "disrespectful", "insubordinate", "defiant", "impudent" };
		          AuthoritySubversion.Virtues = new List<string> { "respectful", "obedient", "deferential", "reverent", "dutiful" };
		          return AuthoritySubversion;
	     }
	     public static MoralFoundation SanctityDegradation()
	     {
		          MoralFoundation SanctityDegradation = new MoralFoundation("Care/Harm", "clean", "degrade", 0.5f);
		          SanctityDegradation.Vices = new List<string> { "degraded", "impure", "defiled", "unclean", "profane" };
		          SanctityDegradation.Virtues = new List<string> { "pure", "clean", "reverent", "devout", "chaste" };
		          return SanctityDegradation;

	     }
	     public static MoralFoundation LibertyOppression()
	     {
		          MoralFoundation LibertyOppression = new MoralFoundation("Care/Harm", "clean", "degrade", 0.5f);
		          LibertyOppression.Vices = new List<string> { "oppressive", "controlling", "domineering", "tyrannical", "authoritarian" };
		          LibertyOppression.Virtues = new List<string> { "free", "independent", "autonomous", "self-determining", "liberated" };
		          return LibertyOppression;
	     }
}
