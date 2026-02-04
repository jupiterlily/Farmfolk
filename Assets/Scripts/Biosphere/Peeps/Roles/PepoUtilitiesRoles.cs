using System.Collections.Generic;
using System.Linq;
public class PepoUtilitiesRoles
{
	     public static bool MeetsRoleRequirements(PepoData pepo, PepoDataRoles role)
	     {
		          //evaluate role prerequisites on a case by case basis for hexaco, mood, needs, values, resources
		          foreach (PepoRolePrerequisite condition in role.rolePrerequisites)
		          {
				if (GetPrerequisiteType(condition.prerequisiteName) == "hexaco")
				{
					     //identify which hexaco trait to check
					     float traitValue = 100;
					     switch (condition.prerequisiteName)
					     {
					     case "Honesty":
					     traitValue = pepo.personality.Honesty.value;
					     break;
					     case "Emotionality":
					     traitValue = pepo.personality.Emotionality.value;
					     break;
					     case "Extraversion":
					     traitValue = pepo.personality.Extraversion.value;
					     break;
					     case "Agreeableness":
					     traitValue = pepo.personality.Agreeableness.value;
					     break;
					     case "Conscientiousness":
					     traitValue = pepo.personality.Conscientiousness.value;
					     break;
					     case "Openness":
					     traitValue = pepo.personality.Openness.value;
					     break;
					     }
					     if (traitValue < condition.minimum || traitValue > condition.maximum)
					     {
						          return false; // prerequisite not met
					     }
				}
				else if (GetPrerequisiteType(condition.prerequisiteName) == "mood")
				{
					     float moodValue = 150;
					     switch (condition.prerequisiteName)
					     {
					     case "Joy":
					     moodValue = pepo.mood.Joy.value;
					     break;
					     case "Sadness":
					     moodValue = pepo.mood.Sadness.value;
					     break;
					     case "Anger":
					     moodValue = pepo.mood.Anger.value;
					     break;
					     case "Fear":
					     moodValue = pepo.mood.Fear.value;
					     break;

					     case "Disgust":
					     moodValue = pepo.mood.Disgust.value;
					     break;
					     case "Trust":
					     moodValue = pepo.mood.Trust.value;
					     break;
					     case "Anticipation":
					     moodValue = pepo.mood.Anticipation.value;
					     break;
					     case "Surprise":
					     moodValue = pepo.mood.Surprise.value;
					     break;
					     }
					     if (moodValue < condition.minimum || moodValue > condition.maximum)
					     {
						          return false; // prerequisite not met
					     }
				}
				else if (GetPrerequisiteType(condition.prerequisiteName) == "need")
				{
					     float needValue = 150;
					     foreach (Need need in pepo.needs.Physiological)
					     {
						          if (need.name == condition.prerequisiteName)
						          {
								needValue = need.value;
						          }
					     }
					     foreach (Need need in pepo.needs.Safety)
					     {
						          if (need.name == condition.prerequisiteName)
						          {
								needValue = need.value;
						          }
					     }
					     if (needValue < condition.minimum || needValue > condition.maximum)
					     {
						          return false; // prerequisite not met
					     }
				}
				else if (GetPrerequisiteType(condition.prerequisiteName) == "value")
				{
					     float valueValue = 150;
					     foreach (Value value in pepo.values)
					     {
						          if (value.name == condition.prerequisiteName || value.opposite == condition.prerequisiteName)
						          {
								valueValue = value.value;
						          }
					     }
					     if (valueValue < condition.minimum || valueValue > condition.maximum)
					     {
						          return false; // prerequisite not met
					     }
				}
				else if (GetPrerequisiteType(condition.prerequisiteName) == "age")
				{
					     if (pepo.age < condition.minimum || pepo.age > condition.maximum)
					     {
						          return false; // prerequisite not met
					     }
				}
				else
				{
					     //    Debug.Log($"{role.roleName} prerequisite {condition.prerequisiteName} couldn't be found on {pepo.fullname}");//this error never fires
				}
		          }
		          return true; // All prerequisites met
	     }
	     public static float RoleMatchPercentage(PepoData pepo, PepoDataRoles role)
	     {
		          if (role.rolePrerequisites == null || role.rolePrerequisites.Count == 0)
				return 1.0f; // No prerequisites = 100% match

		          int totalPrerequisites = role.rolePrerequisites.Count;
		          int metPrerequisites = 0;

		          foreach (PepoRolePrerequisite condition in role.rolePrerequisites)
		          {
				if (GetPrerequisiteType(condition.prerequisiteName) == "hexaco")
				{
					     float traitValue = 100;
					     switch (condition.prerequisiteName)
					     {
					     case "Honesty":
					     traitValue = pepo.personality.Honesty.value;
					     break;
					     case "Emotionality":
					     traitValue = pepo.personality.Emotionality.value;
					     break;
					     case "Extraversion":
					     traitValue = pepo.personality.Extraversion.value;
					     break;
					     case "Agreeableness":
					     traitValue = pepo.personality.Agreeableness.value;
					     break;
					     case "Conscientiousness":
					     traitValue = pepo.personality.Conscientiousness.value;
					     break;
					     case "Openness":
					     traitValue = pepo.personality.Openness.value;
					     break;
					     }
					     if (traitValue >= condition.minimum && traitValue <= condition.maximum)
					     {
						          metPrerequisites++;
					     }
				}
				else if (GetPrerequisiteType(condition.prerequisiteName) == "mood")
				{
					     float moodValue = 150;
					     switch (condition.prerequisiteName)
					     {
					     case "Joy":
					     moodValue = pepo.mood.Joy.value;
					     break;
					     case "Sadness":
					     moodValue = pepo.mood.Sadness.value;
					     break;
					     case "Anger":
					     moodValue = pepo.mood.Anger.value;
					     break;
					     case "Fear":
					     moodValue = pepo.mood.Fear.value;
					     break;
					     case "Disgust":
					     moodValue = pepo.mood.Disgust.value;
					     break;
					     case "Trust":
					     moodValue = pepo.mood.Trust.value;
					     break;
					     case "Anticipation":
					     moodValue = pepo.mood.Anticipation.value;
					     break;
					     case "Surprise":
					     moodValue = pepo.mood.Surprise.value;
					     break;
					     }
					     if (moodValue >= condition.minimum && moodValue <= condition.maximum)
					     {
						          metPrerequisites++;
					     }
				}
				else if (GetPrerequisiteType(condition.prerequisiteName) == "need")
				{
					     float needValue = 150;
					     foreach (Need need in pepo.needs.Physiological)
					     {
						          if (need.name == condition.prerequisiteName)
						          {
								needValue = need.value;
						          }
					     }
					     foreach (Need need in pepo.needs.Safety)
					     {
						          if (need.name == condition.prerequisiteName)
						          {
								needValue = need.value;
						          }
					     }
					     if (needValue >= condition.minimum && needValue <= condition.maximum)
					     {
						          metPrerequisites++;
					     }
				}
				else if (GetPrerequisiteType(condition.prerequisiteName) == "value")
				{
					     float valueValue = 150;
					     foreach (Value value in pepo.values)
					     {
						          if (value.name == condition.prerequisiteName || value.opposite == condition.prerequisiteName)
						          {
								valueValue = value.value;
						          }
					     }
					     if (valueValue >= condition.minimum && valueValue <= condition.maximum)
					     {
						          metPrerequisites++;
					     }
				}
				else if (GetPrerequisiteType(condition.prerequisiteName) == "age")
				{
					     if (pepo.age >= condition.minimum && pepo.age <= condition.maximum)
					     {
						          metPrerequisites++;
					     }
				}
				else
				{
					     // Debug.Log($"{role.roleName} prerequisite {condition.prerequisiteName} couldn't be found on {pepo.fullname}");
					     // Count unknown prerequisites as not met
				}
		          }

		          // Return the percentage of prerequisites met
		          return (float)metPrerequisites / totalPrerequisites;
	     }

	     // For backward compatibility - returns true if all prerequisites are met (100% match)
	     public static bool MeetsAllRoleRequirements(PepoData pepo, PepoDataRoles role)
	     {
		          return RoleMatchPercentage(pepo, role) >= 1.0f;
	     }

	     // Take a pepo and a list of roles and return a list of roles with their match percentages
	     public static List<(PepoDataRoles role, float matchPercentage)> MatchingRolesWithScore(PepoData lilGuy, List<PepoDataRoles> roles)
	     {
		          var matches = new List<(PepoDataRoles role, float matchPercentage)>();
		          foreach (PepoDataRoles role in roles)
		          {
				float matchPercentage = RoleMatchPercentage(lilGuy, role);
				if (matchPercentage > 0)
				{
					     matches.Add((role, matchPercentage));
				}
		          }
		          // Optional: Sort by match percentage (highest first)
		          matches = matches.OrderByDescending(x => x.matchPercentage).ToList();
		          return matches;
	     }

	     // Original MatchingRoles method updated to use threshold (default 1.0 for 100% match)
	     public static List<PepoDataRoles> MatchingRoles(PepoData lilGuy, List<PepoDataRoles> roles, float minMatchPercentage = 1.0f)
	     {
		          List<PepoDataRoles> matches = new List<PepoDataRoles>();
		          foreach (PepoDataRoles role in roles)
		          {
				float matchPercentage = RoleMatchPercentage(lilGuy, role);
				if (matchPercentage >= minMatchPercentage)
				{
					     matches.Add(role);
				}
		          }
		          return matches;
	     }

	     // Helper method to get best matching role
	     public static PepoDataRoles GetBestMatchingRole(PepoData lilGuy, List<PepoDataRoles> roles)
	     {
		          if (roles == null || roles.Count == 0)
				return null;

		          PepoDataRoles bestRole = null;
		          float bestMatch = 0f;

		          foreach (PepoDataRoles role in roles)
		          {
				float matchPercentage = RoleMatchPercentage(lilGuy, role);
				if (matchPercentage > bestMatch)
				{
					     bestMatch = matchPercentage;
					     bestRole = role;
				}
		          }

		          return bestRole;
	     }


	     //take a list of PepoDataRoles and a list of strings and see if those strings match PepoDataRoles.roleName
	     //  public static bool PrecursorRolesExist(List<PepoDataRoles> roles, List<string> precursors)
	     //  {
	     //        bool result = false;
	     //        int matches = precursors.Count;

	     //        foreach (PepoDataRoles role in roles)
	     //        {
	     //foreach (string precursor in precursors)
	     //{
	     //	     if (!string.IsNullOrEmpty(precursor))
	     //	     {
	     //		          result = true;
	     //	     }
	     //	     if (role.roleName == precursor)
	     //	     {
	     //		          matches--;
	     //	     }
	     //}
	     //        }
	     //        if (matches == 0)
	     //        {
	     //result = true;
	     //        }
	     //        return result;
	     //  }

	     public static bool PrecursorRolesExist(List<PepoDataRoles> roles, List<string> precursors)
	     {
		          if (precursors == null || precursors.Count == 0)
				return true;

		          foreach (string precursor in precursors)
		          {
				bool found = false;
				foreach (PepoDataRoles role in roles)
				{
					     if (role.roleName == precursor)
					     {
						          found = true;
						          break;
					     }
				}
				if (!found)
					     return false;
		          }
		          return true;
	     }
	     //take the dictionary and astring and return an int for the amount of people doing that job
	     public static int PeposInRole(Dictionary<PepoDataRoles, List<PepoData>> roleDictionary, string roleString)
	     {
		          int result = 0;
		          foreach (PepoDataRoles role in roleDictionary.Keys)
		          {
				if (role.roleName == roleString)
				{
					     result = roleDictionary[role].Count;
				}
		          }
		          return result;
	     }
	     //take a prerequisite string and return the "type" of prerequisite it is
	     public static string GetPrerequisiteType(string prerequisiteName)
	     {
		          if (prerequisiteName.Contains("Age"))
		          {
				return "age";
		          }
		          //determine if prerequisiteName is hexaco, mood, needs, values, resources
		          if (prerequisiteName == "Honesty" || prerequisiteName == "Emotionality" || prerequisiteName == "Extraversion" || prerequisiteName == "Agreeableness" || prerequisiteName == "Conscientiousness" || prerequisiteName == "Openness")
				return "hexaco";
		          if (prerequisiteName == "Joy" || prerequisiteName == "Sadness" || prerequisiteName == "Anger" || prerequisiteName == "Fear" || prerequisiteName == "Disgust" || prerequisiteName == "Trust" || prerequisiteName == "Anticipation" || prerequisiteName == "Surprise")
				return "mood";
		          foreach (Need need in PepoLibraryNeeds.PhysiologicalNeeds)
		          {
				if (prerequisiteName == need.name)
				{
					     return "need";
				}
		          }
		          foreach (Need need in PepoLibraryNeeds.SafetyNeeds)
		          {
				if (prerequisiteName == need.name)
				{
					     return "need";
				}
		          }
		          foreach (Value value in PepoLibraryValues.basicHumanValues())
		          {
				if (prerequisiteName == value.name)
				{
					     return "value";
				}
		          }

		          return "unknown";
	     }

	     //take a dicitonary and a role and return true if all the precursor positions are filled by at least one pepo.
	     public static bool PrecursorRolesPopulated(Dictionary<PepoDataRoles, List<PepoData>> roleDictionary, PepoDataRoles role)
	     {
		          if (role.supportingRoles.Count == 0)
		          {
				return true;
		          }
		          if (PrecursorRolesExist(roleDictionary.Keys.ToList(), role.supportingRoles))
		          {
				//foreach supporting role, check if there is at least one pepo in the role. if there is, return true.
				int roles = 0;
				foreach (var job in role.supportingRoles)
				{
					     if (PeposInRole(roleDictionary, job) >= 1)
					     {
						          roles++;
					     }
				}
				if (role.supportingRoles.Count == roles)
				{
					     return true;
				}
		          }
		          return false;
	     }

}