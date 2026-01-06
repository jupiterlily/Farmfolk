//using UnityEngine;

//public class PepoLibraryActions
//{

//}

//// ===== SPECIFIC ACTION TYPES =====

//public class EatAction : BaseAction
//{
//	     public override string Name => "Eat";
//	     public override float BaseDuration => 0.5f; // 30 minutes
//	     public override float EnergyCost => 0.1f;

//	     private float nutritionValue;

//	     public EatAction(float nutrition = 0.3f)
//	     {
//		          nutritionValue = nutrition;

//		          // Preconditions
//		          Preconditions.Add(new ResourceCondition("food", 0.1f, ResourceCondition.ComparisonType.AtLeast));

//		          // Effects
//		          Effects.Add(new ResourceEffect("food", -0.1f)); // Consume food
//		          Effects.Add(new MoodEffect(
//		              happiness: 0.1f,
//		              energy: nutritionValue,
//		              stress: -0.05f
//		          ));
//	     }

//	     public override float CalculateUtility(PepoData actor)
//	     {
//		          float baseUtility = base.CalculateUtility(actor);

//		          // Eating is more urgent when hungry
//		          if (actor.needs != null)
//		          {
//				float hungerModifier = actor.needs.Physiological[0].currentLevel * 2f; // Double weight for hunger
//				return baseUtility * (0.3f + hungerModifier * 0.7f);
//		          }

//		          return baseUtility;
//	     }
//}

//public class SleepAction : BaseAction
//{
//	     public override string Name => "Sleep";
//	     public override float BaseDuration => 8f; // 8 hours
//	     public override float EnergyCost => 0f; // Actually restores energy

//	     private float sleepQuality;

//	     public SleepAction(float quality = 0.8f)
//	     {
//		          sleepQuality = quality;

//		          // Preconditions
//		          Preconditions.Add(new LocationCondition(Vector3.zero, 5f)); // Must be near bed/home

//		          // Effects - sleep restores energy
//		          Effects.Add(new MoodEffect(
//		              happiness: 0.05f,
//		              energy: sleepQuality * 0.7f, // Restore 70% of sleep quality as energy
//		              stress: -0.15f
//		          ));
//	     }

//	     public override float CalculateUtility(PepoData actor)
//	     {
//		          float baseUtility = base.CalculateUtility(actor);

//		          // Sleeping is more urgent when tired
//		          if (actor.needs != null)
//		          {
//				float fatigueModifier = (actor.needs.Physiological[2].maxValue - actor.needs.Physiological[2].value) * 2f;
//				return baseUtility * (0.2f + fatigueModifier * 0.8f);
//		          }

//		          return baseUtility;
//	     }
//}

//public class SocializeAction : BaseAction
//{
//	     public override string Name => "Socialize";
//	     public override float BaseDuration => 2f; // 2 hours
//	     public override float EnergyCost => 0.3f;

//	     private PepoData target;

//	     public SocializeAction(PepoData withWhom)
//	     {
//		          target = withWhom;

//		          // Preconditions
//		          Preconditions.Add(new LocationCondition(withWhom.currentLocation, 3f)); // Must be near target

//		          // Effects
//		          Effects.Add(new MoodEffect(
//		              happiness: 0.2f,
//		              energy: -0.1f,
//		              stress: -0.1f
//		          ));
//		          Effects.Add(new RelationshipEffect(withWhom,
//		              trust: 0.05f,
//		              affection: 0.1f,
//		              familiarity: 0.15f
//		          ));
//	     }

//	     public override float CalculateUtility(PepoData actor)
//	     {
//		          float baseUtility = base.CalculateUtility(actor);

//		          // Socializing is affected by extraversion
//		          if (actor.personality != null)
//		          {
//				float extraversionBonus = actor.personality.Extraversion.value * 0.5f;
//				return baseUtility * (0.5f + extraversionBonus);
//		          }

//		          return baseUtility;
//	     }
//}

//public class WorkAction : BaseAction
//{
//	     public override string Name => "Work";
//	     public override float BaseDuration => 4f; // 4 hours
//	     public override float EnergyCost => 0.4f;

//	     private float productivity;
//	     private string jobType;

//	     public WorkAction(string type = "generic", float productivityMultiplier = 1f)
//	     {
//		          jobType = type;
//		          productivity = productivityMultiplier;

//		          // Effects
//		          Effects.Add(new ResourceEffect("money", 0.1f * productivity));
//		          Effects.Add(new MoodEffect(
//		              happiness: -0.1f,
//		              energy: -0.3f,
//		              stress: 0.2f
//		          ));
//	     }

//	     public override float CalculateUtility(PepoData actor)
//	     {
//		          float baseUtility = base.CalculateUtility(actor);

//		          // Working is affected by conscientiousness
//		          if (actor.personality != null)
//		          {
//				float conscientiousnessBonus = actor.personality.Conscientiousness.value * 0.3f;
//				return baseUtility * (0.7f + conscientiousnessBonus);
//		          }

//		          return baseUtility;
//	     }
//}