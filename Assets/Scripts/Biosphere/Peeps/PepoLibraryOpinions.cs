using System.Collections.Generic;


public static class PepoLibraryOpinions
{
	     public static Dictionary<Aspect, PepoOpinionData> opinionLibraryByAspect = new Dictionary<Aspect, PepoOpinionData>();
	     private static Dictionary<string, float> DTo(params Aspect[] values)
	     {
		          var d = new Dictionary<string, float>();
		          foreach (var v in values)
				d[v.name] = 1f;
		          return d;
	     }

	     private static Dictionary<string, float> DNeg(params Aspect[] values)
	     {
		          var d = new Dictionary<string, float>();
		          foreach (var v in values)
				d[v.name] = -1f;
		          return d;
	     }

	     private static Dictionary<string, float> DAway(params Aspect[] values)
	     {
		          var d = new Dictionary<string, float>();
		          foreach (var v in values)
				d[v.name] = -0.1f;
		          return d;
	     }
	     public static void InitialiseValueOpinions()
	     {
		          opinionLibraryByAspect.Add(PepoLibraryValues.AgencyThought, curiosity);
		          opinionLibraryByAspect.Add(PepoLibraryValues.AgencyAction, autonomy);
		          opinionLibraryByAspect.Add(PepoLibraryValues.Stimulation, excitement);
		          opinionLibraryByAspect.Add(PepoLibraryValues.Health, vitality);
		          opinionLibraryByAspect.Add(PepoLibraryValues.Hedonism, indulgence);
		          opinionLibraryByAspect.Add(PepoLibraryValues.Achievement, accomplishment);
		          opinionLibraryByAspect.Add(PepoLibraryValues.PowerDominance, dominance);
		          opinionLibraryByAspect.Add(PepoLibraryValues.PowerResources, resources);
		          opinionLibraryByAspect.Add(PepoLibraryValues.Face, status);
		          opinionLibraryByAspect.Add(PepoLibraryValues.SecurityPersonal, safety);
		          opinionLibraryByAspect.Add(PepoLibraryValues.SecuritySociety, stability);
		          opinionLibraryByAspect.Add(PepoLibraryValues.Tradition, tradition);
		          opinionLibraryByAspect.Add(PepoLibraryValues.ConformityRules, obedience);
		          opinionLibraryByAspect.Add(PepoLibraryValues.ConformityInterpersonal, cohesion);
		          opinionLibraryByAspect.Add(PepoLibraryValues.Humility, humility);
		          opinionLibraryByAspect.Add(PepoLibraryValues.UniversalismNature, preservation);
		          opinionLibraryByAspect.Add(PepoLibraryValues.UniversalismConcern, compassion);
		          opinionLibraryByAspect.Add(PepoLibraryValues.UniversalismTolerance, tolerance);
		          opinionLibraryByAspect.Add(PepoLibraryValues.BenevolenceCaring, caring);
		          opinionLibraryByAspect.Add(PepoLibraryValues.BenevolenceDependability, dependability);
	     }
	     public static PepoOpinionData curiosity = new PepoOpinionData()
	     {
		          //     aspect = (NumericalAspect)PepoLibraryValues.AgencyThought,
		          Towards = DTo(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.AgencyAction,
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.Health,
	             PepoLibraryValues.Hedonism,
	             PepoLibraryValues.UniversalismNature,
	             PepoLibraryValues.UniversalismConcern,
	             PepoLibraryValues.UniversalismTolerance
	         ),
		          Above = DTo(PepoLibraryValues.Achievement),
		          Against = DNeg(
	             PepoLibraryValues.Face,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.ConformityRules
	         ),
		          Away = DAway(
	             PepoLibraryValues.Tradition,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.Humility
	         )
	     };
	     //these should be grouped into a value ccategory.
	     public static PepoOpinionData autonomy = new PepoOpinionData()
	     {
		          //     aspect = (NumericalAspect)PepoLibraryValues.AgencyAction,
		          Towards = DTo(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.AgencyAction,
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.Health,
	             PepoLibraryValues.Hedonism,
	             PepoLibraryValues.UniversalismNature,
	             PepoLibraryValues.UniversalismConcern,
	             PepoLibraryValues.UniversalismTolerance
	         ),
		          Above = DTo(
	             PepoLibraryValues.Achievement,
	             PepoLibraryValues.PowerResources
	         ),
		          Against = DNeg(
	             PepoLibraryValues.PowerDominance,
	             PepoLibraryValues.Face,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.ConformityRules,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.Humility
	         ),
		          Away = DAway(
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.Tradition,
	             PepoLibraryValues.BenevolenceDependability
	         )
	     };

	     public static PepoOpinionData excitement = new PepoOpinionData()
	     {
		          //          aspect = (NumericalAspect)PepoLibraryValues.Stimulation,
		          Towards = DTo(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.AgencyAction,
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.Health,
	             PepoLibraryValues.Hedonism,
	             PepoLibraryValues.UniversalismTolerance
	         ),
		          Above = DTo(PepoLibraryValues.Achievement),
		          Against = DNeg(PepoLibraryValues.SecuritySociety),
		          Away = DAway(
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.Humility,
	             PepoLibraryValues.BenevolenceDependability
	         )
	     };

	     public static PepoOpinionData vitality = new PepoOpinionData()
	     {
		          //          aspect = (NumericalAspect)PepoLibraryValues.Health,
		          Towards = DTo(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.Health,
	             PepoLibraryValues.PowerResources,
	             PepoLibraryValues.Face,
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.UniversalismNature,
	             PepoLibraryValues.UniversalismConcern,
	             PepoLibraryValues.BenevolenceCaring
	         ),
		          Above = DTo(PepoLibraryValues.Achievement),
		          Away = DAway(PepoLibraryValues.Hedonism)
	     };

	     public static PepoOpinionData indulgence = new PepoOpinionData()
	     {
		          //       aspect = (NumericalAspect)PepoLibraryValues.Hedonism,
		          Towards = DTo(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.AgencyAction,
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.Hedonism
	         ),
		          Above = DTo(PepoLibraryValues.Face),
		          Against = DNeg(
	             PepoLibraryValues.Health,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.Humility,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Away = DAway(
	             PepoLibraryValues.Achievement,
	             PepoLibraryValues.PowerResources,
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.ConformityInterpersonal
	         )
	     };

	     public static PepoOpinionData accomplishment = new PepoOpinionData()
	     {
		          //      aspect = (NumericalAspect)PepoLibraryValues.Achievement,
		          Towards = DTo(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.AgencyAction,
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.Health,
	             PepoLibraryValues.Achievement,
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.Tradition,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Above = DTo(PepoLibraryValues.PowerResources),
		          Below = DTo(
	             PepoLibraryValues.Face,
	             PepoLibraryValues.ConformityRules,
	             PepoLibraryValues.Humility
	         ),
		          Away = DAway(
	             PepoLibraryValues.Hedonism,
	             PepoLibraryValues.PowerDominance,
	             PepoLibraryValues.UniversalismNature,
	             PepoLibraryValues.UniversalismConcern,
	             PepoLibraryValues.BenevolenceCaring
	         )
	     };

	     public static PepoOpinionData dominance = new PepoOpinionData()
	     {
		          //        aspect = (NumericalAspect)PepoLibraryValues.PowerDominance,
		          Towards = DTo(PepoLibraryValues.PowerResources),
		          Above = DTo(PepoLibraryValues.Achievement),
		          Below = DTo(
	             PepoLibraryValues.Face,
	             PepoLibraryValues.ConformityRules,
	             PepoLibraryValues.Humility
	         ),
		          Against = DNeg(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.PowerDominance,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.UniversalismConcern,
	             PepoLibraryValues.UniversalismTolerance,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Away = DAway(
	             PepoLibraryValues.Health,
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.BenevolenceCaring
	         )
	     };

	     public static PepoOpinionData resources = new PepoOpinionData()
	     {
		          //  aspect = (NumericalAspect)PepoLibraryValues.PowerResources,
		          Towards = DTo(
	             PepoLibraryValues.Achievement,
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.Tradition
	         ),
		          Above = DTo(
	             PepoLibraryValues.PowerDominance,
	             PepoLibraryValues.PowerResources
	         ),
		          Below = DTo(
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.Hedonism,
	             PepoLibraryValues.Face,
	             PepoLibraryValues.Humility
	         ),
		          Against = DNeg(
	             PepoLibraryValues.UniversalismNature,
	             PepoLibraryValues.UniversalismConcern
	         ),
		          Away = DAway(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.Health,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.UniversalismTolerance,
	             PepoLibraryValues.BenevolenceCaring,
	             PepoLibraryValues.BenevolenceDependability
	         )
	     };

	     public static PepoOpinionData status = new PepoOpinionData()
	     {
		          //   aspect = (NumericalAspect)PepoLibraryValues.Face,
		          Towards = DTo(PepoLibraryValues.SecurityPersonal),
		          Above = DTo(
	             PepoLibraryValues.Hedonism,
	             PepoLibraryValues.Achievement,
	             PepoLibraryValues.PowerDominance,
	             PepoLibraryValues.PowerResources,
	             PepoLibraryValues.Face
	         ),
		          Below = DTo(PepoLibraryValues.ConformityRules),
		          Against = DNeg(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.AgencyAction,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.UniversalismConcern,
	             PepoLibraryValues.UniversalismTolerance
	         ),
		          Away = DAway(
	             PepoLibraryValues.Health,
	             PepoLibraryValues.Humility,
	             PepoLibraryValues.BenevolenceCaring,
	             PepoLibraryValues.BenevolenceDependability
	         )
	     };

	     public static PepoOpinionData safety = new PepoOpinionData()
	     {
		          //    aspect = (NumericalAspect)PepoLibraryValues.SecurityPersonal,
		          Towards = DTo(
	             PepoLibraryValues.Health,
	             PepoLibraryValues.PowerResources,
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.Humility,
	             PepoLibraryValues.UniversalismNature,
	             PepoLibraryValues.UniversalismConcern,
	             PepoLibraryValues.UniversalismTolerance,
	             PepoLibraryValues.BenevolenceCaring,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Against = DNeg(
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.Hedonism
	         ),
		          Away = DAway(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.AgencyAction,
	             PepoLibraryValues.Achievement,
	             PepoLibraryValues.Face
	         )
	     };

	     public static PepoOpinionData stability = new PepoOpinionData()
	     {
		          //   aspect = (NumericalAspect)PepoLibraryValues.SecuritySociety,
		          Towards = DTo(
	             PepoLibraryValues.Health,
	             PepoLibraryValues.PowerResources,
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.Tradition,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.Humility,
	             PepoLibraryValues.UniversalismNature,
	             PepoLibraryValues.UniversalismTolerance,
	             PepoLibraryValues.BenevolenceCaring,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Above = DTo(PepoLibraryValues.PowerDominance),
		          Against = DNeg(
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.Hedonism
	         ),
		          Away = DAway(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.AgencyAction,
	             PepoLibraryValues.Achievement
	         )
	     };

	     public static PepoOpinionData tradition = new PepoOpinionData()
	     {
		          //     aspect = (NumericalAspect)PepoLibraryValues.Tradition,
		          Towards = DTo(
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.Tradition,
	             PepoLibraryValues.ConformityRules,
	             PepoLibraryValues.BenevolenceCaring,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Above = DTo(
	             PepoLibraryValues.Face,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.ConformityInterpersonal
	         ),
		          Below = DTo(PepoLibraryValues.Humility),
		          Against = DNeg(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.AgencyAction,
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.Hedonism
	         ),
		          Away = DAway(PepoLibraryValues.Achievement)
	     };

	     public static PepoOpinionData obedience = new PepoOpinionData()
	     {
		          //    aspect = (NumericalAspect)PepoLibraryValues.ConformityRules,
		          Towards = DTo(
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.ConformityRules,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Above = DTo(
	             PepoLibraryValues.Achievement,
	             PepoLibraryValues.PowerDominance,
	             PepoLibraryValues.PowerResources,
	             PepoLibraryValues.Tradition
	         ),
		          Below = DTo(PepoLibraryValues.Humility),
		          Against = DNeg(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.AgencyAction,
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.Hedonism
	         )
	     };

	     public static PepoOpinionData cohesion = new PepoOpinionData()
	     {
		          //     aspect = (NumericalAspect)PepoLibraryValues.ConformityInterpersonal,
		          Towards = DTo(
	             PepoLibraryValues.Face,
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.Tradition,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.UniversalismNature,
	             PepoLibraryValues.UniversalismConcern,
	             PepoLibraryValues.UniversalismTolerance,
	             PepoLibraryValues.BenevolenceCaring,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Above = DTo(PepoLibraryValues.PowerDominance),
		          Below = DTo(PepoLibraryValues.Humility),
		          Against = DNeg(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.AgencyAction,
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.Hedonism
	         )
	     };

	     public static PepoOpinionData humility = new PepoOpinionData()
	     {
		          //   aspect = (NumericalAspect)PepoLibraryValues.Humility,
		          Towards = DTo(
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.Humility,
	             PepoLibraryValues.BenevolenceCaring,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Above = DTo(
	             PepoLibraryValues.PowerDominance,
	             PepoLibraryValues.PowerResources,
	             PepoLibraryValues.Face,
	             PepoLibraryValues.Tradition,
	             PepoLibraryValues.ConformityRules
	         ),
		          Away = DAway(
	             PepoLibraryValues.AgencyThought,
	             PepoLibraryValues.AgencyAction,
	             PepoLibraryValues.Stimulation,
	             PepoLibraryValues.Hedonism,
	             PepoLibraryValues.Achievement
	         )
	     };

	     public static PepoOpinionData preservation = new PepoOpinionData()
	     {
		          //     aspect = (NumericalAspect)PepoLibraryValues.UniversalismNature,
		          Towards = DTo(
	             PepoLibraryValues.Health,
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.Humility,
	             PepoLibraryValues.UniversalismNature,
	             PepoLibraryValues.UniversalismConcern,
	             PepoLibraryValues.UniversalismTolerance,
	             PepoLibraryValues.BenevolenceCaring,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Against = DNeg(PepoLibraryValues.PowerResources),
		          Away = DAway(PepoLibraryValues.Stimulation)
	     };

	     public static PepoOpinionData compassion = new PepoOpinionData()
	     {
		          //     aspect = (NumericalAspect)PepoLibraryValues.UniversalismConcern,
		          Towards = DTo(
	             PepoLibraryValues.Health,
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.Humility,
	             PepoLibraryValues.UniversalismConcern,
	             PepoLibraryValues.UniversalismTolerance,
	             PepoLibraryValues.BenevolenceCaring,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Against = DNeg(PepoLibraryValues.PowerDominance),
		          Away = DAway(
	             PepoLibraryValues.Achievement,
	             PepoLibraryValues.PowerResources,
	             PepoLibraryValues.Face
	         )
	     };

	     public static PepoOpinionData tolerance = new PepoOpinionData()
	     {
		          //    aspect = (NumericalAspect)PepoLibraryValues.UniversalismTolerance,
		          Towards = DTo(
	             PepoLibraryValues.Hedonism,
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.Humility,
	             PepoLibraryValues.UniversalismConcern,
	             PepoLibraryValues.UniversalismTolerance,
	             PepoLibraryValues.BenevolenceCaring,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Away = DAway(
	             PepoLibraryValues.PowerDominance,
	             PepoLibraryValues.PowerResources
	         )
	     };

	     public static PepoOpinionData caring = new PepoOpinionData()
	     {
		          //  aspect = (NumericalAspect)PepoLibraryValues.BenevolenceCaring,
		          Towards = DTo(
	             PepoLibraryValues.Health,
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.Humility,
	             PepoLibraryValues.UniversalismConcern,
	             PepoLibraryValues.UniversalismTolerance,
	             PepoLibraryValues.BenevolenceCaring,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Against = DNeg(PepoLibraryValues.PowerDominance),
		          Away = DAway(
	             PepoLibraryValues.Achievement,
	             PepoLibraryValues.PowerResources,
	             PepoLibraryValues.Face
	         )
	     };

	     public static PepoOpinionData dependability = new PepoOpinionData()
	     {
		          // aspect = (NumericalAspect)PepoLibraryValues.BenevolenceDependability,
		          Towards = DTo(
	             PepoLibraryValues.Health,
	             PepoLibraryValues.Achievement,
	             PepoLibraryValues.PowerResources,
	             PepoLibraryValues.Face,
	             PepoLibraryValues.SecurityPersonal,
	             PepoLibraryValues.SecuritySociety,
	             PepoLibraryValues.Tradition,
	             PepoLibraryValues.ConformityInterpersonal,
	             PepoLibraryValues.UniversalismNature,
	             PepoLibraryValues.UniversalismConcern,
	             PepoLibraryValues.UniversalismTolerance,
	             PepoLibraryValues.BenevolenceCaring,
	             PepoLibraryValues.BenevolenceDependability
	         ),
		          Above = DTo(PepoLibraryValues.PowerDominance),
		          Below = DTo(PepoLibraryValues.Humility),
		          Away = DAway(
	             PepoLibraryValues.AgencyAction,
	             PepoLibraryValues.Stimulation
	         )
	     };
	     //then we have dictionaries with the numerical aspect being the key and the value being the PepoOpinionData.

	     //then we should do personality facets. thesefunction differently to values because hexaco traits and facets are polar with 5 being the mid point. above 5 son sincerity would be described as sincere, below would be insincere.

	     //public static PepoOpinionData highSincerity = new PepoOpinionData()
	     //{
	     //      Towards = DTo(PepoLibraryPersonality.HEXACODimension.H)
	     //};
	     //public static PepoOpinionData lowSincerity = new PepoOpinionData()
	     //{
	     //      Against = DNeg(PepoLibraryPersonality.HEXACODimension.H)
	     //};

	     public static Dictionary<string, PepoOpinionData> opinionLibrary = new Dictionary<string, PepoOpinionData>()
    {
        { PepoLibraryValues.AgencyThought.name, curiosity },
        { PepoLibraryValues.AgencyAction.name, autonomy },
        { PepoLibraryValues.Stimulation.name, excitement },
        { PepoLibraryValues.Health.name, vitality },
        { PepoLibraryValues.Hedonism.name, indulgence },
        { PepoLibraryValues.Achievement.name, accomplishment },
        { PepoLibraryValues.PowerDominance.name, dominance },
        { PepoLibraryValues.PowerResources.name, resources },
        { PepoLibraryValues.Face.name, status },
        { PepoLibraryValues.SecurityPersonal.name, safety },
        { PepoLibraryValues.SecuritySociety.name, stability },
        { PepoLibraryValues.Tradition.name, tradition },
        { PepoLibraryValues.ConformityRules.name, obedience },
        { PepoLibraryValues.ConformityInterpersonal.name, cohesion },
        { PepoLibraryValues.Humility.name, humility },
        { PepoLibraryValues.UniversalismNature.name, preservation },
        { PepoLibraryValues.UniversalismConcern.name, compassion },
        { PepoLibraryValues.UniversalismTolerance.name, tolerance },
        { PepoLibraryValues.BenevolenceCaring.name, caring },
        { PepoLibraryValues.BenevolenceDependability.name, dependability }
    };
}
public class PepoOpinionData
{
	     //dictionaries for the names of the traits and then the intensity of the opinion. 0 is neutral, 1 is strong positive, -1 is strong negative.
	     public Dictionary<string, float> Towards = new Dictionary<string, float>(); //i want to be around you
	     public Dictionary<string, float> Against = new Dictionary<string, float>(); //i have to work against you
	     public Dictionary<string, float> Above = new Dictionary<string, float>(); //i should be above you
	     public Dictionary<string, float> Below = new Dictionary<string, float>(); //i want to be below you
	     public Dictionary<string, float> Away = new Dictionary<string, float>(); //i want to get away from you    
}

public class PepoFilterData
{
	     // a pepo will have multiple filters with different weightings and sources. filters will be used in multiple places, both in the game logic and in ui etc. a filter is juust a way to organise restrictions on a group of pepos. for example, a filter could be people who are at least 5 curious, over 180cm and have sincerity of 7 or a filter could be men who are attracted to women. pepos will have filters for what traits they want in an acquaintence, mentor, friend, romantic partner, coparent etc. and these will be able to be transmitted, which will be part of how cultures happen. functions  will take filters and use them to return a bool, or alist of pepos

	     public Aspect[] requiredTraits;

	     public PepoFilterData(List<Aspect> traits)
	     {
		          requiredTraits = traits.ToArray();
	     }

	     public bool CheckMatch(PepoData pepo)
	     {
		          
		          if (pepo.FilterableAspects == null)
		          {
				return false;
		          }
		          //first we check the list to see if the pepos even has all of the traits.
		          foreach (var aspect in requiredTraits)
		          {
				if (!pepo.FilterableAspects.ContainsKey(aspect.name))
				{
					     return false;
				}
		          }
		          foreach (var trait in requiredTraits)
		          {
				//first we need to check what type of aspects. we will start with numerical aspects, which havve a value and a range. we will then check if the pepo has the trait, and if they do, we will check if the value is within the range set by the required trait. if it is not, we will return false. if it is, we will continue checking the other traits. if all traits are checked and are within the range, we will return true.					   
				NumericalAspect numericalTrait = trait as NumericalAspect;
				//if the trait is not within the range, return false
				if (pepo.FilterableAspects[trait.name].value < numericalTrait.minValue || pepo.FilterableAspects[trait.name].value > numericalTrait.maxValue)
				{
					     return false;
				}

		          }
		          return true;
	     }
	     public List<PepoData> FilterPepoList(List<PepoData> pepos)
	     {
		          List<PepoData> filteredPepoList = new List<PepoData>();
		          foreach (var pepo in pepos)
		          {
				if (CheckMatch(pepo))
				{
					     filteredPepoList.Add(pepo);
				}
		          }
		          return filteredPepoList;
	     }
}

//then i suppose we want one that takes a pepo and returns the traits they move towards, against, above, below and away from. because a pepo has a combination of values and personality aspects with different modifiers, each will have a different weighting for who they want tomove to and away from. 