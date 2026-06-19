using System.Collections.Generic;

// Data/PepoCulturalData.cs
public class PepoCulturalData
{
	     public string PersonalNarrative = ""; // Story they tell about themselves
	     public Dictionary<string, float> CulturalTraitValues = new(); // Map of trait name to value
					       // public Dictionary<string, List<BoundaryTransgression>> Transgressions = new(); // By category
	     public List<SocialCategory> SelfIdentifiedCategories = new();
	     public Dictionary<string, float> BoundarySensitivities = new(); // How much they care about each boundary
	     public float ConformityDrive = 0.5f; // Base tendency to follow norms
	     public float InnovationDrive = 0.3f; // Base tendency to challenge norms

	     // Gender performance tracking
	     public Dictionary<string, float> GenderPerformanceScores = new(); // Score in each recognized gender construct
	     public string PrimaryGenderPerformance = "";
}

// Data/SocialCategory.cs
public class SocialCategory
{
	     public string Id;
	     public string EmergentName; // "The skilled", "The pure ones", "Land-workers"
	     public List<PepoData> PrototypicalMembers = new();
	     public List<CulturalTrait> DefiningTraits = new();
	     public float BoundaryStrength = 0.5f;
	     public float SocialStatus = 0.5f;
	     public Dictionary<ResourceAccessTier, float> ResourceAccessLevels = new();
	     public List<CategoryRule> Rules = new();
	     public int Generation = 0; // How many cycles this category has existed
}

// Data/CulturalTrait.cs
public class CulturalTrait
{
	     public string Name;
	     public float BaseValue = 0.5f;
	     public float Heritability = 0.7f; // How much is passed to children
	     public DifferentiationAxis PrimaryAxis;
	     public Dictionary<SocialDomain, float> DomainImpacts = new()
    {
        { SocialDomain.EconomicStatus, 0.3f },
        { SocialDomain.RitualStatus, 0.3f },
        { SocialDomain.KnowledgeAccess, 0.3f },
        { SocialDomain.PoliticalPower, 0.3f },
        { SocialDomain.MarriageEligibility, 0.3f }
    };
	     public float Visibility = 0.5f; // How easily observable this trait is

	     // Transmission methods weighted by society
	     public Dictionary<TransmissionMethod, float> TransmissionWeights = new()
    {
        { TransmissionMethod.Ascriptive, 0.5f },
        { TransmissionMethod.Meritocratic, 0.3f },
        { TransmissionMethod.Charismatic, 0.1f },
        { TransmissionMethod.Appointive, 0.1f }
    };
}

// Data/CategoryRule.cs
public class CategoryRule
{
	     public string Description; // "Cannot marry outside category", "Must cover hair", "Cannot handle food"
	     public float EnforcementStrength = 0.7f;
	     public float SanctionSeverity = 0.5f; // Punishment for breaking
	     public List<ResourceAllocationRule> AssociatedResourceRules = new();
	     public int TimesBroken = 0;
	     public int TimesEnforced = 0;

	     // Backlash tracking
	     public float ResistanceLevel = 0f; // When this gets high, rule might change
	     public List<PepoData> MajorTransgressors = new(); // Those who broke it significantly
}

public enum SocialDomain
{
	     EconomicStatus,
	     RitualStatus,
	     KnowledgeAccess,
	     PoliticalPower,
	     MarriageEligibility,
}
public enum DifferentiationAxis
{
	     Kinship,           // Blood relation patterns
	     PhysicalTraits,    // Appearance markers
	     EconomicFunction,  // What you do for resources
	     RitualStatus,      // Purity/pollution concepts
	     KnowledgeAccess,   // Who knows what
	     RelationshipPatterns, // Who marries whom
	     SpatialProximity,  // Where you live/work
}
public enum TransmissionMethod
{
	     Ascriptive,        // Assigned at birth (caste)
	     Meritocratic,      // Earned through achievement (class)
	     Charismatic,       // Based on personal qualities
	     Appointive,        // Chosen by authority
	     Elective,          // Chosen by peers
	     Contestive,        // Won through competition
	     Random             // No pattern
}
public enum ResourceAllocationRule
{
	     EqualDistribution,     // Everyone gets same
	     NeedBased,            // To those who need most
	     MeritBased,           // To those who achieve
	     AscriptionBased,      // Based on birth category
	     PowerBased,           // To those who can take
	     Random,               // No pattern
	     RitualBased,          // Based on purity/status
	     MarketBased           // Trade/exchange
}
