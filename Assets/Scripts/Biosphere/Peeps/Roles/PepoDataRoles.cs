using System.Collections.Generic;

public class PepoDataRoles
{
	     public string roleName;
	     public string roleDescription;
	     string roleEra;
	     public List<PepoRolePrerequisite> rolePrerequisites;
	     public float minOccurance = 0f; //how common is this role? a value between 0 and 1 is a percentage chance of being assigned. a value above 1 determines the max number of pepos that can have this role in a given population.
	     public float maxOccurance = 0f;
	     public List<string> supportingRoles = new List<string>(); //roles that need to exist for this role to function/be applicable.
	     public float minimumAge = 0f;
	     public float maximumAge = 2000f;
	     public PepoDataRoles(string name, string description, string era, List<PepoRolePrerequisite> prerequisites, float min, float max, List<string> precursors = null)
	     {
		          roleName = name;
		          roleDescription = description;
		          roleEra = era;
		          rolePrerequisites = prerequisites;
		          minOccurance = min;
		          maxOccurance = max;
		          supportingRoles = precursors;
	     }
}

public class PepoRolePrerequisite
{
	     public string prerequisiteName; //hunger, speed, honesty, self direction, strength
	     public float minimum;
	     public float maximum;
	     public PepoRolePrerequisite(string name, float min, float max)
	     {
		          prerequisiteName = name;
		          minimum = min;
		          maximum = max;
	     }
}