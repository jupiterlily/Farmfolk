public class ResourceSystems
{

	     // Cultural feedback: Resources shape social structure
	     //	     public class ResourceCultureFeedback
	     //	     {
	     //		          public void UpdateSocialCategoriesBasedOnResourceControl(List<PepoData> pepos)
	     //		          {
	     //				// Which categories control which resources?
	     //				var resourceControlByCategory = new Dictionary<SocialCategory, Dictionary<string, float>>();

	     //				foreach (var pepo in pepos)
	     //				{
	     //					     foreach (var category in pepo.CulturalData.SelfIdentifiedCategories)
	     //					     {
	     //						          if (!resourceControlByCategory.ContainsKey(category))
	     //								resourceControlByCategory[category] = new Dictionary<string, float>();

	     //						          // Count resources this pepo has
	     //						          foreach (var resource in pepo.resources.Inventory)
	     //						          {
	     //								if (!resourceControlByCategory[category].ContainsKey(resource.Key))
	     //									     resourceControlByCategory[category][resource.Key] = 0;

	     //								resourceControlByCategory[category][resource.Key] +=
	     //								    resource.Value is PhysicalResource pr ? pr.Count : 1;
	     //						          }
	     //					     }
	     //				}

	     //				// Update category status based on resource control
	     //				foreach (var entry in resourceControlByCategory)
	     //				{
	     //					     var category = entry.Key;
	     //					     var resources = entry.Value;

	     //					     float totalValue = 0;
	     //					     foreach (var resource in resources)
	     //					     {
	     //						          var resourceData = GetResourceData(resource.Key);
	     //						          totalValue += resource.Value * (1 + resourceData.StatusValue);
	     //					     }

	     //					     // Status increases with valuable resource control
	     //					     category.SocialStatus = Mathf.Lerp(category.SocialStatus,
	     //					         Mathf.Clamp01(totalValue * 0.1f), 0.1f);
	     //				}
	     //		          }

	     //		          public void CreateTaboosAroundResources(List<ResourceData> resources)
	     //		          {
	     //				// Societies might develop taboos around certain resources

	     //				foreach (var resource in resources)
	     //				{
	     //					     // If a resource is scarce AND valuable, might become restricted
	     //					     if (GetScarcity(resource) > 0.7f && GetValue(resource) > 0.5f)
	     //					     {
	     //						          // Create ritual purity rules
	     //						          resource.RitualPurity = 0.8f; // Make it sacred

	     //						          // Create exclusive access for high status
	     //						          resource.BaseAccessTier = ResourceAccessTier.Restricted;

	     //						          // Generate justifying narrative
	     //						          string narrative = $"The {resource.Name} is sacred and must only be handled by the pure.";
	     //						          AddCulturalNarrative(narrative);
	     //					     }

	     //					     // If a resource is associated with "dirty" work
	     //					     if (resource.Categories.Contains("Waste") || resource.Name.Contains("corpse"))
	     //					     {
	     //						          resource.RitualPurity = -0.9f; // Deeply polluting

	     //						          // Assign to low status categories
	     //						          resource.ExclusiveCategories.Clear();
	     //						          foreach (var lowStatusCategory in GetLowStatusCategories())
	     //						          {
	     //								resource.ExclusiveCategories.Add(lowStatusCategory.Id);
	     //						          }

	     //						          string narrative = $"Handling {resource.Name} is necessary but polluting work.";
	     //						          AddCulturalNarrative(narrative);
	     //					     }
	     //				}
	     //		          }
	     //	     }
	     //}
}