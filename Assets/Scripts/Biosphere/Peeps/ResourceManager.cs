using System.Collections.Generic;
using UnityEngine;

// Resource Manager that handles allocation based on social rules
public class ResourceManager : MonoBehaviour
{
	     private Dictionary<string, ResourceData> allResources = new();
	     private Dictionary<PlaceData, List<PhysicalResource>> placeResources = new();

	     // Allocation rules by resource type
	     public Dictionary<string, ResourceAllocationRule> AllocationRules = new();

	     //  public void AllocateResources(List<PepoData> pepos, List<SocialCategory> categories)
	     //  {
	     //        foreach (var resourceEntry in allResources)
	     //        {
	     //var resource = resourceEntry.Value;
	     //var rule = GetApplicableRule(resource);

	     //switch (rule)
	     //{
	     //case ResourceAllocationRule.EqualDistribution:
	     //AllocateEqually(resource, pepos);
	     //break;

	     //case ResourceAllocationRule.NeedBased:
	     //AllocateByNeed(resource, pepos);
	     //break;

	     //case ResourceAllocationRule.MeritBased:
	     //AllocateByMerit(resource, pepos);
	     //break;

	     //case ResourceAllocationRule.AscriptionBased:
	     //AllocateByCategory(resource, categories, pepos);
	     //break;

	     //case ResourceAllocationRule.PowerBased:
	     //AllocateByPower(resource, pepos);
	     //break;

	     //case ResourceAllocationRule.RitualBased:
	     //AllocateByRitualStatus(resource, pepos);
	     //break;

	     //case ResourceAllocationRule.MarketBased:
	     //// Let trading handle it
	     //break;
	     //}
	     //        }
	     //  }

	     //  private void AllocateByCategory(ResourceData resource, List<SocialCategory> categories, List<PepoData> pepos)
	     //  {
	     //        // Allocate based on social category status
	     //        foreach (var category in categories.OrderByDescending(c => c.SocialStatus))
	     //        {
	     ////var categoryMembers = pepos.Where(p => p.CulturalData.SelfIdentifiedCategories.Any(sc => sc.Id == category.Id));

	     //// Higher status categories get more
	     //float allocationRatio = category.SocialStatus;
	     //int totalAmount = GetAvailableAmount(resource);
	     //int categoryShare = Mathf.RoundToInt(totalAmount * allocationRatio);

	     ////DistributeAmongMembers(categoryMembers, resource, categoryShare);
	     //        }
	     //  }

	     //  private void AllocateByRitualStatus(ResourceData resource, List<PepoData> pepos)
	     //  {
	     //        // Allocate based on ritual purity
	     //        var sortedByPurity = pepos.OrderByDescending(p => CalculateRitualPurity(p));

	     //        foreach (var pepo in sortedByPurity)
	     //        {
	     //float purityScore = CalculateRitualPurity(pepo);
	     //if (purityScore > 0.7f && resource.RitualPurity > 0)
	     //{
	     //	     // High purity individuals get pure resources
	     //	  //   GrantAccess(pepo, resource, ResourceAccessTier.Open);
	     //}
	     //else if (purityScore < -0.3f && resource.RitualPurity < 0)
	     //{
	     //	     // Low purity individuals handle polluting resources
	     //	   //  GrantAccess(pepo, resource, ResourceAccessTier.Open);
	     //}
	     //else
	     //{
	     //	     // Others get restricted access
	     //	  //   GrantAccess(pepo, resource, ResourceAccessTier.Restricted);
	     //}
	     //        }
	     //  }

	     //  private float CalculateRitualPurity(PepoData pepo)
	     //  {
	     //        // Calculate based on:
	     //        // - Social category purity
	     //        // - Recent rule violations
	     //        // - Ancestral purity
	     //        // - Occupation purity

	     //        float score = 0f;

	     //        // Start with category purity
	     //        foreach (var category in pepo.CulturalData.SelfIdentifiedCategories)
	     //        {
	     //score += category.SocialStatus * 0.5f;
	     //        }

	     //        // Deduct for transgressions
	     //        foreach (var transgression in pepo.CulturalData.Transgressions)
	     //        {
	     //if (transgression.Value.Any(t => t.Severity > 0.7f))
	     //	     score -= 0.3f;
	     //        }

	     //        // Occupation purity
	     //        if (pepo.CurrentRoles.Any(r => r.Name.Contains("corpse") || r.Name.Contains("waste")))
	     //score -= 0.4f;

	     //        return Mathf.Clamp(score, -1f, 1f);
	     //  }

	     public void UpdateResourceAccessRules(List<SocialCategory> categories)
	     {
		          // Update which social categories can access which resources

		          foreach (var resource in allResources.Values)
		          {
				// Clear old rules
				resource.ForbiddenCategories.Clear();
				resource.ExclusiveCategories.Clear();

				// Apply new rules based on current social structure
				foreach (var category in categories)
				{
					     // High status categories get exclusive access to status goods
					     if (resource.StatusValue > 0.5f && category.SocialStatus > 0.8f)
					     {
						          resource.ExclusiveCategories.Add(category.Id);
					     }

					     // Low status categories forbidden from sacred objects
					     if (resource.RitualPurity > 0.7f && category.SocialStatus < 0.3f)
					     {
						          resource.ForbiddenCategories.Add(category.Id);
					     }

					     // Polluting tasks assigned to low status categories
					     if (resource.RitualPurity < -0.5f && category.SocialStatus < 0.4f)
					     {
						          resource.ExclusiveCategories.Add(category.Id); // Exclusive to them (forced labor)
					     }
				}
		          }
	     }

	     //  public void HandleResourceBacklash(ResourceData resource, SocialCategory demandingCategory)
	     //  {
	     //        // When a category demands access to a restricted resource

	     //        // Check if they have the power to demand it
	     //        float categoryPower = demandingCategory.PrototypicalMembers.Count * demandingCategory.SocialStatus;
	     //        float resistance = resource.ExclusiveCategories
	     //            .Select(c => GetCategory(c))
	     //            .Sum(c => c.PrototypicalMembers.Count * c.SocialStatus);

	     //        if (categoryPower > resistance * 1.5f)
	     //        {
	     //// Backlash succeeds - resource access changes
	     //resource.ExclusiveCategories.RemoveAll(id => id != demandingCategory.Id);
	     //resource.ForbiddenCategories.Remove(demandingCategory.Id);

	     //// Log the change
	     //Debug.Log($"Resource access changed for {resource.Name}: {demandingCategory.Id} gained access");

	     //// This might trigger further social changes
	     //CheckForCascadingChanges(resource);
	     //        }
	     //  }
}
