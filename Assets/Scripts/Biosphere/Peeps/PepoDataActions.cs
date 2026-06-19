using System.Collections.Generic;
using UnityEngine;

// ===== BASE ACTION SYSTEM =====

public interface IAction
{
	     string Name
	     {
		          get;
	     }
	     float BaseDuration
	     {
		          get;
	     } // in hours
	     float EnergyCost
	     {
		          get;
	     }

	     // Check if this action can be performed by this Pepo in this context
	     bool CanExecute(PepoData actor, object context = null);

	     // Calculate how much this action meets the Pepo's current needs (0-1)
	     float CalculateUtility(PepoData actor);

	     // Execute the action and return results
	     ActionResult Execute(PepoData actor, object context = null);
}

public class ActionResult
{
	     public bool Success
	     {
		          get; set;
	     }
	     public float TimeTaken
	     {
		          get; set;
	     } // Actual time spent
	     public List<IEffect> Effects { get; set; } = new List<IEffect>();
	     public string Message
	     {
		          get; set;
	     }
}

public interface IEffect
{
	     void Apply(PepoData target);
}

// ===== CONDITION SYSTEM =====

public interface ICondition
{
	     string Description
	     {
		          get;
	     }
	     bool IsMet(PepoData pepo);
}

//public class ResourceCondition : ICondition
//{
//	     public string ResourceName
//	     {
//		          get;
//	     }
//	     public float RequiredAmount
//	     {
//		          get;
//	     }
//	     public ComparisonType Comparison
//	     {
//		          get;
//	     }

//	     public enum ComparisonType
//	     {
//		          AtLeast, AtMost, Exactly
//	     }

//	     public ResourceCondition(string resourceName, float amount, ComparisonType comparison)
//	     {
//		          ResourceName = resourceName;
//		          RequiredAmount = amount;
//		          Comparison = comparison;
//	     }

//	     public string Description => $"{ResourceName} {GetComparisonString()} {RequiredAmount}";

//	     private string GetComparisonString()
//	     {
//		          return Comparison switch
//		          {
//				ComparisonType.AtLeast => "?",
//				ComparisonType.AtMost => "?",
//				ComparisonType.Exactly => "=",
//				_ => ""
//		          };
//	     }

//	   //  public bool IsMet(PepoData pepo)
//	   //  {

//		  //        // Find resource in pepo's resources
//		  //        var resource = pepo.resources.inventory?.Find(r => r.name == ResourceName || r.category.Contains(ResourceName));
//		  //        if (resource == null)
//				//return false;

//		  //        return Comparison switch
//		  //        {
//				//ComparisonType.AtLeast => resource.count >= RequiredAmount,
//				//ComparisonType.AtMost => resource.count <= RequiredAmount,
//				//ComparisonType.Exactly => Mathf.Approximately(resource.count, RequiredAmount),
//				//_ => false
//		  //        };
//	   //  }
//}

//public class AspectCondition : ICondition
//{
//	     public string AspectName
//	     {
//		          get;
//	     }
//	     public float RequiredValue
//	     {
//		          get;
//	     }
//	     public ComparisonType Comparison
//	     {
//		          get;
//	     }

//	     public AspectCondition(string aspectName, float value, ComparisonType comparison)
//	     {
//		          AspectName = aspectName;
//		          RequiredValue = value;
//		          Comparison = comparison;
//	     }

//	     public string Description => $"{AspectName} {GetComparisonString()} {RequiredValue}";

//	     private string GetComparisonString() => Comparison switch
//	     {
//		          ComparisonType.AtLeast => "?",
//		          ComparisonType.AtMost => "?",
//		          ComparisonType.Exactly => "=",
//		          _ => ""
//	     };

//	     public bool IsMet(PepoData pepo)
//	     {
//		          // This is trickier - we need a way to get aspects from PepoData
//		          // For now, let's assume we can find them somehow
//		          // You'll need to implement this based on your aspect structure
//		          return true; // Placeholder
//	     }
//}

public class LocationCondition : ICondition
{
	     public Vector3 RequiredLocation
	     {
		          get;
	     }
	     public float MaxDistance
	     {
		          get;
	     }

	     public LocationCondition(Vector3 location, float maxDistance = 1f)
	     {
		          RequiredLocation = location;
		          MaxDistance = maxDistance;
	     }

	     public string Description => $"Near {RequiredLocation} (within {MaxDistance}m)";

	     public bool IsMet(PepoData pepo)
	     {
		          return Vector3.Distance(pepo.currentLocation, RequiredLocation) <= MaxDistance;
	     }
}

// ===== EFFECTS =====

//public class ResourceEffect : IEffect
//{
//	     public string ResourceName
//	     {
//		          get;
//	     }
//	     public float ChangeAmount
//	     {
//		          get;
//	     }
//	     public bool IsAbsolute
//	     {
//		          get;
//	     } // If true, sets to exact value instead of adding

//	     public ResourceEffect(string resourceName, float amount, bool absolute = false)
//	     {
//		          ResourceName = resourceName;
//		          ChangeAmount = amount;
//		          IsAbsolute = absolute;
//	     }

//	   //  public void Apply(PepoData target)
//	   //  {
//		  //        //if i already have the resource, change it, else add it.
//		  //        if (target.resources.inventory.Exists(r => r.name == ResourceName || r.category.Contains(ResourceName)))
//		  //        {
//				//var resource = target.resources.inventory.Find(r => r.name == ResourceName || r.category.Contains(ResourceName));
//				//if (IsAbsolute)
//				//{
//				//	     resource.count = (int)ChangeAmount;
//				//}
//				//else
//				//{
//				//	     resource.count += (int)ChangeAmount;
//				//}
//		  //        }
//		  //        else
//		  //        {
//				////var newResource = new ResourceData { name = ResourceName, count = (int)ChangeAmount, description = $"{ResourceName} added by action." };
//		  //        }
//	   //  }
//}

public class MoodEffect : IEffect
{
	     public float[] PlutchickModifier = new float[7]; // Changes to Plutchik's 8 basic emotions

	     public MoodEffect(float surprise = 0, float anticipation = 0, float trust = 0, float disgust = 0, float fear = 0, float anger = 0, float joy = 0, float sad = 0)
	     {
		          PlutchickModifier[0] = surprise;
		          PlutchickModifier[1] = anticipation;
		          PlutchickModifier[2] = trust;
		          PlutchickModifier[3] = disgust;
		          PlutchickModifier[4] = fear;
		          PlutchickModifier[5] = anger;
		          PlutchickModifier[6] = joy;
		          PlutchickModifier[7] = sad;
	     }

	     public void Apply(PepoData target)
	     {
		          if (target.mood != null)
		          {
				target.mood.ApplyMoodEffect(PlutchickModifier);

		          }
	     }
}

public class RelationshipEffect : IEffect
{
	     public PepoData OtherPepo
	     {
		          get;
	     }
	     public float TrustChange
	     {
		          get;
	     }
	     public float AffectionChange
	     {
		          get;
	     }
	     public float FamiliarityChange
	     {
		          get;
	     }

	     public RelationshipEffect(PepoData other, float trust = 0, float affection = 0, float familiarity = 0)
	     {
		          OtherPepo = other;
		          TrustChange = trust;
		          AffectionChange = affection;
		          FamiliarityChange = familiarity;
	     }

	     public void Apply(PepoData target)
	     {
		          // Find or create relationship data
		          // You'll need to implement this based on your relationship system
	     }
}

// ===== CONCRETE ACTIONS =====

//public abstract class BaseAction : IAction
//{
//	     public abstract string Name
//	     {
//		          get;
//	     }
//	     public abstract float BaseDuration
//	     {
//		          get;
//	     }
//	     public abstract float EnergyCost
//	     {
//		          get;
//	     }

//	     public List<ICondition> Preconditions { get; protected set; } = new List<ICondition>();
//	     public List<IEffect> Effects { get; protected set; } = new List<IEffect>();

//	     public virtual bool CanExecute(PepoData actor, object context = null)
//	     {
//		          // Check all preconditions
//		          foreach (var condition in Preconditions)
//		          {
//				if (!condition.IsMet(actor))
//					     return false;
//		          }

//		          // Check energy level
//		          if (actor.mood?.energy < EnergyCost * 0.5f) // Need at least 50% of required energy
//				return false;

//		          return true;
//	     }

//	     public virtual float CalculateUtility(PepoData actor)
//	     {
//		          // Base utility calculation based on Maslow's hierarchy
//		          float utility = 0f;

//		          // Physiological needs (food, water, sleep)
//		          if (actor.needs != null)
//		          {
//				utility += (1f - actor.needs.hunger) * 0.3f;
//				utility += (1f - actor.needs.thirst) * 0.3f;
//				utility += (1f - actor.needs.fatigue) * 0.2f;
//		          }

//		          // Safety needs (energy, health)
//		          if (actor.mood != null)
//		          {
//				utility += actor.mood.energy * 0.1f;
//		          }

//		          // Adjust for personality traits
//		          if (actor.personality != null)
//		          {
//				// Extraversion affects social actions
//				if (this is SocialAction)
//					     utility *= (0.5f + actor.personality.extraversion * 0.5f);

//				// Conscientiousness affects task completion
//				utility *= (0.5f + actor.personality.conscientiousness * 0.5f);
//		          }

//		          return Mathf.Clamp01(utility);
//	     }

//	     public virtual ActionResult Execute(PepoData actor, object context = null)
//	     {
//		          var result = new ActionResult { TimeTaken = BaseDuration };

//		          try
//		          {
//				// Apply all effects
//				foreach (var effect in Effects)
//				{
//					     effect.Apply(actor);
//				}

//				// Deduct energy
//				if (actor.mood != null)
//				{
//					     actor.mood.energy = Mathf.Clamp01(actor.mood.energy - EnergyCost);
//				}

//				result.Success = true;
//				result.Message = $"{actor.fullname} performed {Name}";
//		          }
//		          catch (Exception ex)
//		          {
//				result.Success = false;
//				result.Message = $"Failed to perform {Name}: {ex.Message}";
//		          }

//		          return result;
//	     }
//}
//// ===== ACTION PLANNER =====

//public class ActionPlanner
//{
//	     public List<IAction> AvailableActions { get; private set; } = new List<IAction>();
//	     public Queue<IAction> ScheduledActions { get; private set; } = new Queue<IAction>();

//	     private PepoData owner;

//	     public ActionPlanner(PepoData pepo)
//	     {
//		          owner = pepo;
//		          InitializeDefaultActions();
//	     }

//	     private void InitializeDefaultActions()
//	     {
//		          // Basic survival actions
//		          AvailableActions.Add(new EatAction());
//		          AvailableActions.Add(new SleepAction());
//		          AvailableActions.Add(new WorkAction());

//		          // You would add more actions based on pepo's skills, location, etc.
//	     }

//	     public void PlanDay(float hoursAvailable = 16f) // 16 waking hours
//	     {
//		          ScheduledActions.Clear();
//		          float timeBudget = hoursAvailable;

//		          // Get all executable actions sorted by utility
//		          var possibleActions = new List<IAction>();
//		          foreach (var action in AvailableActions)
//		          {
//				if (action.CanExecute(owner))
//				{
//					     possibleActions.Add(action);
//				}
//		          }

//		          // Sort by utility (descending)
//		          possibleActions.Sort((a, b) =>
//		              b.CalculateUtility(owner).CompareTo(a.CalculateUtility(owner)));

//		          // Schedule actions while we have time
//		          foreach (var action in possibleActions)
//		          {
//				if (timeBudget >= action.BaseDuration)
//				{
//					     ScheduledActions.Enqueue(action);
//					     timeBudget -= action.BaseDuration;
//				}

//				if (timeBudget <= 0)
//					     break;
//		          }
//	     }

//	     public IAction GetNextAction()
//	     {
//		          if (ScheduledActions.Count > 0)
//				return ScheduledActions.Dequeue();

//		          // If no scheduled actions, pick the best available one
//		          IAction bestAction = null;
//		          float bestUtility = -1f;

//		          foreach (var action in AvailableActions)
//		          {
//				if (action.CanExecute(owner))
//				{
//					     float utility = action.CalculateUtility(owner);
//					     if (utility > bestUtility)
//					     {
//						          bestUtility = utility;
//						          bestAction = action;
//					     }
//				}
//		          }

//		          return bestAction;
//	     }

//	     public void ReevaluateSchedule()
//	     {
//		          // Check if current needs are being met
//		          bool needsReplanning = false;

//		          if (owner.needs != null)
//		          {
//				// If any critical need is too low, replan
//				if (owner.needs.hunger > 0.8f || owner.needs.thirst > 0.8f ||
//				    owner.needs.fatigue > 0.8f)
//				{
//					     needsReplanning = true;
//				}
//		          }

//		          if (needsReplanning)
//		          {
//				PlanDay();
//		          }
//	     }

//	     public void AddAction(IAction action)
//	     {
//		          AvailableActions.Add(action);
//	     }
//}

//// ===== PEPO CONTROLLER =====

//public class PepoController
//{
//	     private PepoData pepo;
//	     private ActionPlanner planner;
//	     private IAction currentAction;
//	     private float actionTimer;

//	     public PepoController(PepoData pepoData)
//	     {
//		          pepo = pepoData;
//		          planner = new ActionPlanner(pepo);
//		          planner.PlanDay();
//	     }

//	     public void Update(float deltaTime)
//	     {
//		          // Check if we need to replan
//		          planner.ReevaluateSchedule();

//		          // If no current action, get one
//		          if (currentAction == null)
//		          {
//				currentAction = planner.GetNextAction();
//				if (currentAction != null)
//				{
//					     actionTimer = currentAction.BaseDuration;
//					     Debug.Log($"{pepo.fullname} starting: {currentAction.Name}");
//				}
//		          }

//		          // If we have an action, process it
//		          if (currentAction != null)
//		          {
//				actionTimer -= deltaTime;

//				// Action completed
//				if (actionTimer <= 0)
//				{
//					     var result = currentAction.Execute(pepo);
//					     Debug.Log($"{pepo.fullname} finished {currentAction.Name}: {result.Message}");

//					     // Check if action was successful
//					     if (result.Success)
//					     {
//						          // Update needs based on action completion
//						          UpdateNeedsAfterAction(currentAction);
//					     }

//					     currentAction = null;
//				}
//		          }
//	     }

//	     private void UpdateNeedsAfterAction(IAction action)
//	     {
//		          // This is where you'd update the Maslow hierarchy needs
//		          // based on what action was completed
//		          if (action is EatAction)
//		          {
//				pepo.needs.hunger = Mathf.Max(0, pepo.needs.hunger - 0.5f);
//		          }
//		          else if (action is SleepAction)
//		          {
//				pepo.needs.fatigue = Mathf.Max(0, pepo.needs.fatigue - 0.8f);
//		          }
//		          else if (action is SocializeAction)
//		          {
//				pepo.needs.belonging = Mathf.Min(1, pepo.needs.belonging + 0.2f);
//		          }
//	     }

//	     public void ForceAction(IAction action)
//	     {
//		          currentAction = null; // Cancel current action
//		          actionTimer = 0;
//		          currentAction = action;
//	     }
//}

// ===== INTEGRATION WITH EXISTING PEPODATA =====

// Add this to your existing PepoData class:
// public ActionPlanner actionPlanner;
// public PepoController controller;

// In your Pepo initialization:
// controller = new PepoController(this);

// In your simulation update loop for each Pepo:
// controller.Update(deltaTime);