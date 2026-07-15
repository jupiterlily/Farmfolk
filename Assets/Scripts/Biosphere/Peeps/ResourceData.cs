using System.Collections.Generic;
using UnityEngine;

// Enums first
public enum ResourceDomain
{
	     Material,      // Physical goods (food, tools)
	     Social,        // Relationships, reputation
	     Ritual,        // Sacred objects, purity
	     Knowledge,     // Information, skills
	     Status,        // Honor, prestige tokens
	     Environmental  // Land, natural resources
}

public enum ResourceAccessTier
{
	     Forbidden,     // Cannot access at all
	     Restricted,    // Can access with permission
	     Limited,       // Can access but limited amount
	     Open,          // Free access
	     Exclusive      // Only this person/can access
}

// Base resource class
public abstract class ResourceData
{
	     public string Id;
	     public string Name;
	     public string Description;
	     public ResourceDomain Domain;
	     public List<string> Categories = new(); // food, tool, sacred, skill, 

	     // Production
	     public bool IsProductive = false;
	     public int BaseProductionAmount = 0;
	     public string ResourceProducedId = "";
	     public float ProductionCycleDays = 1f;
	     public Dictionary<string, float> ProductionRequirements = new(); // ResourceId -> amount needed

	     // Social properties
	     public bool IsExcludable = true; // Can access be restricted?
	     public bool IsRivalrous = true;  // Does use by one reduce availability to others?
	     public float RitualPurity = 0f;  // -1 (polluting) to 1 (sacred)
	     public float StatusValue = 0f;   // How much status does owning this confer?

	     // Access rules (can be overridden by social categories)
	     public ResourceAccessTier BaseAccessTier = ResourceAccessTier.Open;
	     public List<string> ForbiddenCategories = new(); // Social categories that can't access
	     public List<string> ExclusiveCategories = new(); // Social categories with exclusive access

	     public ResourceData(string id, string name, ResourceDomain domain)
	     {
		          Id = id;
		          Name = name;
		          Domain = domain;
	     }
}

// Social/Cultural Resources
public class SocialResource : ResourceData
{
	     public float DecayRate = 0.1f; // How quickly this resource fades if not maintained
	     public List<string> TransmissionMethods = new(); // How this resource can be passed on

	     public SocialResource(string id, string name) : base(id, name, ResourceDomain.Social)
	     {
		          IsExcludable = false; // Social resources often non-excludable
		          IsRivalrous = false;  // My honor doesn't reduce yours
	     }

	     // Examples: "Honor", "SocialCapital", "Reputation", "FavorDebt"
}

// Physical Resources
public class PhysicalResource : ResourceData
{
	     public float VolumePerUnit; // cm³ per count
	     public float MassPerUnit;   // kg per count
	     public Vector3 Location;    // Where it's stored/available
	     public bool IsPortable = true;
	     public float Durability = 1f; // 0-1, wears down with use

	     private int _count = 0;
	     public int Count
	     {
		          get => _count;
		          set
		          {
				_count = Mathf.Max(0, value);
				UpdatePhysicalProperties();
		          }
	     }

	     public PhysicalResource(string id, string name, float volume, float mass, Vector3 location)
	         : base(id, name, ResourceDomain.Material)
	     {
		          VolumePerUnit = volume;
		          MassPerUnit = mass;
		          Location = location;
		          IsExcludable = true;
		          IsRivalrous = true;
	     }

	     private void UpdatePhysicalProperties()
	     {
		          // Mass and volume scale with count
		          // These would be used for storage/transport calculations
	     }

	     public void TransferTo(PhysicalResource target, int amount)
	     {
		          if (amount <= 0 || Count < amount)
				return;

		          Count -= amount;
		          target.Count += amount;
	     }
}

// Knowledge Resources
public class KnowledgeResource : ResourceData
{
	     public bool IsSecret = false;        // Can only be learned through initiation
	     public float LearningDifficulty = 0.5f; // 0-1, how hard to learn
	     public List<string> PrerequisiteKnowledge = new(); // Must know these first

	     public KnowledgeResource(string id, string name) : base(id, name, ResourceDomain.Knowledge)
	     {
		          IsExcludable = true;  // Knowledge can be restricted
		          IsRivalrous = false;  // My knowing doesn't stop you knowing
	     }

	     // Examples: "FireMaking", "AnimalTracking", "HerbalMedicine", "RitualChants"
}

// Status Resources
public class StatusResource : ResourceData
{
	     public bool IsDisplayable = true;    // Can be shown off
	     public float RecognitionValue = 1f;  // How widely recognized as status marker
	     public bool IsInheritable = true;    // Can be passed to children

	     public StatusResource(string id, string name) : base(id, name, ResourceDomain.Status)
	     {
		          IsExcludable = true;
		          IsRivalrous = true; // Status is often zero-sum
	     }

	     // Examples: "Trophy", "Heirloom", "Title", "SacredObject"
}

// Pepo Resource Management with Social Integration
public class PepoResources
{
	     public Dictionary<string, ResourceData> Inventory = new();  // ResourceId -> ResourceData
	     public Dictionary<string, float> ResourcePreferences = new(); // ResourceId -> desire strength (0-1)

	     // Storage locations with access rights
	     public class StorageAccess
	     {
		          public PlaceData Place;
		          public ResourceAccessTier AccessLevel;
		          public List<string> AllowedResourceTypes = new();
	     }
	     public List<StorageAccess> StorageAccessRights = new();

	     // Wishlist with urgency
	     public class ResourceWish
	     {
		          public string ResourceId;
		          public int DesiredAmount;
		          public float Urgency; // 0-1 based on needs
				  //  public ResourceAcquisitionMethod PreferredMethod;
	     }
	     public List<ResourceWish> Wishlist = new();

	     // Social resource trackers
	     public Dictionary<string, float> SocialResources = new(); // "Honor", "Influence", "Debt"
	     public Dictionary<string, float> KnowledgeResources = new(); // Skills and knowledge

	     public void AddResource(ResourceData resource, int amount = 1)
	     {
		          if (resource is PhysicalResource physical)
		          {
				// Handle physical resources
				if (!Inventory.ContainsKey(resource.Id))
				{
					     Inventory[resource.Id] = resource;
					     (resource as PhysicalResource).Count = amount;
				}
				else
				{
					     var existing = Inventory[resource.Id] as PhysicalResource;
					     existing.Count += amount;
				}
		          }
		          else if (resource is SocialResource)
		          {
				// Handle social resources
				if (SocialResources.ContainsKey(resource.Id))
					     SocialResources[resource.Id] += amount;
				else
					     SocialResources[resource.Id] = amount;
		          }
		          else if (resource is KnowledgeResource)
		          {
				// Handle knowledge - binary (you know it or not)
				if (!KnowledgeResources.ContainsKey(resource.Id))
					     KnowledgeResources[resource.Id] = 1f; // Known
		          }

		          UpdateWishlist(resource.Id, -amount);
	     }

	     private void UpdateWishlist(string resourceId, int change)
	     {
		          var wish = Wishlist.Find(w => w.ResourceId == resourceId);
		          if (wish != null)
		          {
				wish.DesiredAmount = Mathf.Max(0, wish.DesiredAmount + change);
				if (wish.DesiredAmount <= 0)
					     Wishlist.Remove(wish);
		          }
	     }

	     public bool CanAccessResource(ResourceData resource, SocialCategory pepoCategory)
	     {
		          // Check social category restrictions
		          if (resource.ForbiddenCategories.Contains(pepoCategory.Id))
				return false;

		          if (resource.ExclusiveCategories.Count > 0 &&
		              !resource.ExclusiveCategories.Contains(pepoCategory.Id))
				return false;

		          // Check ritual purity if applicable
		          if (resource.RitualPurity < -0.5f && pepoCategory.SocialStatus < 0.3f)
				return false;

		          return true;
	     }

	     public ResourceAccessTier GetAccessTier(ResourceData resource, SocialCategory pepoCategory)
	     {
		          // Start with base tier
		          ResourceAccessTier tier = resource.BaseAccessTier;

		          // Social category overrides
		          if (resource.ExclusiveCategories.Contains(pepoCategory.Id))
				return ResourceAccessTier.Exclusive;

		          if (resource.ForbiddenCategories.Contains(pepoCategory.Id))
				return ResourceAccessTier.Forbidden;

		          // Ritual purity restrictions
		          if (resource.RitualPurity > 0.7f && pepoCategory.SocialStatus < 0.5f)
				tier = ResourceAccessTier.Restricted;

		          return tier;
	     }
}
