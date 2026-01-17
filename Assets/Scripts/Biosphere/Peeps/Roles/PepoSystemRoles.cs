using System.Collections.Generic;
using UnityEngine;

public static class PepoRoleSystem
{
	     //take a pepo poulation and a list of roles and assign roles to pepos based on their suitability
	     public static void AssignRolesToPepoPopulation(PepoPopulation pop, List<PepoDataRoles> roles)
	     {
		          foreach (PepoData pepo in pop.adults)
		          {
				List<PepoDataRoles> pepoRoles = PepoUtilitiesRoles.MatchingRoles(pepo, roles);
				//if this is empty, say what the role is and what the pepos values and personality are.
				if (pepoRoles.Count == 0)
				{
					     Debug.Log($"{pepo.fullname} couldn't find a role. They are {PepoLogUtilities.Values(pepo, 10)}, and {PepoLogUtilities.Hexaco(pepo.personality, 6)} and their best match is {PepoUtilitiesRoles.MatchingRolesWithScore(pepo, roles)[0].role.roleName} with {PepoUtilitiesRoles.MatchingRolesWithScore(pepo, roles)[0].matchPercentage * 100f}%");
					     continue;
				}

				//foreach role on the list, we check if the precursor roles exist and if somebody is doing that job
				foreach (PepoDataRoles job in pepoRoles)
				{
					     if (PepoUtilitiesRoles.PrecursorRolesPopulated(pop.roleAssignments, job))
					     {
						          //add the role to the dictionary if it doesnt already exist.
						          if (pop.roleAssignments.ContainsKey(job))
						          {
								//if the job aint on the list, add the fisrt entry.
								if (pop.roleAssignments[job] == null)
								{
									     pop.roleAssignments[job] = new List<PepoData>() { pepo };
									     Debug.Log($"{pepo.fullname} became a {job.roleName}"
									    );
								}
								else if (!pop.roleAssignments[job].Contains(pepo))//if the job is on the list and the pepo isn't on that list
								{
									     pop.roleAssignments[job].Add(pepo);
									     Debug.Log($"{pepo.fullname} became a {job.roleName}"
									    );
								}
						          }
						          else
						          {
								pop.roleAssignments.Add(job, new List<PepoData>() { pepo });
								Debug.Log($"{pepo.fullname} became a {job.roleName}"
									    );
						          }
					     }
				}
		          }
	     }
}
