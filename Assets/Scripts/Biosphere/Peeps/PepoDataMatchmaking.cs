public class MatchmakingCriteria
{
	     string name;
	     bool satisfied;

	     //body criteria

	     //sociosexual criteria
	     public bool[] isFertile = { true, false };
	     public bool[] isPrepubescent = { true, false };

	     public int[] wantsChildren;

	     public bool[] isFemme = { true, false }; //gender identity internally
	     public bool[] isMasc = { true, false };

	     public bool[] presentsFemme = { true, false }; //gender presentation
	     public bool[] presentsMasc = { true, false };

	     public bool[] likesFemme = { true, false }; //sexuality
	     public bool[] likesMasc = { true, false };

	     public bool[] platonicFemme = { true, false }; //who i will befriend
	     public bool[] platonicMasc = { true, false };

	     public bool[] romanceFemme = { true, false };
	     public bool[] romanceMasc = { true, false };

	     public bool[] isMonogamous = { true, false };
	     public bool[] isAsexual = { true, false };
	     public bool[] isAplatonic = { true, false };
	     public bool[] isAromantic = { true, false };
	     //value criteria

	     //moral criteria


}
public class PepoArchetype
{
	     public string name;
	     //here we will have a list or range of all the possible traits for a pepo that matches this archetype
}
