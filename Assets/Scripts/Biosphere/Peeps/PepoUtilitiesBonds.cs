using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PepoUtilitiesBonds
{
	     // take alist of relationshipData and terutn a list of Pepos
	     public static List<PepoData> ShipsToPepos(List<PepoRelationshipData> ships)
	     {
		          List<PepoData> pepos = new List<PepoData>();
		          foreach (var lilGuy in ships)
		          {
				if (lilGuy == null)
				{
					     UnityEngine.Debug.LogWarning("No RelationshipData to Process");
					     return pepos;
				}
				else
				{
					     pepos.Add(lilGuy.pepo);
				}
		          }
		          return pepos;
	     }
	     // take a pepo, a target and a relationship type and add a it to the RelationshipLibrary dictionary
	     public static Dictionary<RelationshipType, List<PepoRelationshipData>> AddRelation(PepoData me, PepoData them, RelationshipType type, string name = null)
	     {
		          if (name == null)
		          {
				name = type.ToString();
		          }
		          //if this type isn't in the dictionary, add it to the dictionary
		          if (me == null || them == null)
		          {
				UnityEngine.Debug.LogWarning("No PepoData to Process");
				return null;
		          }
		          PepoRelationshipData relationshipData = HaveWeMet(me, them);
		          //check if we have met before. if we have, use that relationship data, if we haven't create a new one.
		          if (relationshipData == null)
		          {
				relationshipData = new PepoRelationshipData(them, me, type, name);
				relationshipData.met = SimulationConstants.TimeConstants;
		          }


		          if (me.RelationshipLibrary.ContainsKey(type))
		          {//check if the list exists
				if (me.RelationshipLibrary[type] == null)
				{
					     me.RelationshipLibrary[type] = new List<PepoRelationshipData>();
				}
		          }
		          else
		          {
				me.RelationshipLibrary.Add(type, new List<PepoRelationshipData>());
		          }
		          relationshipData.relationshipTypes.Add(type);
		          ApplyRelationshipModifiers(relationshipData, type, me);
		          me.RelationshipLibrary[type].Add(relationshipData);
		          UnityEngine.Debug.Log($"[Relationship Manager]{PepoUtilities.GetFullName(me)} is now {PepoUtilities.GetFullName(them)}'s {name}");
		          return me.RelationshipLibrary;
	     }
	     public static Dictionary<RelationshipType, List<PepoRelationshipData>> RemoveRelation(PepoData me, PepoData them, RelationshipType type)
	     {
		          HaveWeMet(me, them).relationshipTypes.Remove(type);
		          me.RelationshipLibrary[type].Remove(HaveWeMet(me, them));
		          return me.RelationshipLibrary;
	     }
	     //check if two lilguys know each other
	     public static PepoRelationshipData HaveWeMet(PepoData me, PepoData you)
	     {
		          // List<PepoData> list = new List<PepoData>();
		          //for each relationship data type that I have
		          foreach (var type in me.RelationshipLibrary.Keys)
		          {
				//if you are on that list of relationships
				if (ShipsToPepos(me.RelationshipLibrary[type]).Contains(you))
				{
					     //go through my relationshipData of that type
					     foreach (var rd in me.RelationshipLibrary[type])
					     {
						          //and return the relationshipData with my information about you
						          if (rd.pepo == you)
						          {
								UnityEngine.Debug.Log($"[Relationship Manager]{PepoUtilities.GetFullName(me)} is already {rd.nickname}'s {rd.relationshipName}");
								return rd;
						          }
					     }
				}
		          }
		          return null; //because i do not know you
	     }
	     public static void ApplyRelationshipModifiers(PepoRelationshipData relData, RelationshipType type, PepoData me)
	     {
		          // First calculate all modifiers based on personality and values
		          float ConfidenceModifier = CalculateConfidenceModifier(me);
		          float AuthorityModifier = CalculateAuthorityModifier(me);
		          float InfluenceModifier = CalculateInfluenceModifier(me);
		          float ObligationModifier = CalculateObligationModifier(me);
		          float ReciprocityModifier = CalculateReciprocityModifier(me);
		          float IntimacyModifier = CalculateIntimacyModifier(me);
		          float RomanceModifier = CalculateRomanceModifier(me);
		          float DesireModifier = CalculateDesireModifier(me);
		          float AnimosityModifier = CalculateAnimosityModifier(me);
		          float AffectionModifier = CalculateAffectionModifier(me);
		          float AffiliationModifier = CalculateAffiliationModifier(me);
		          float AttachmentModifier = CalculateAttachmentModifier(me);

		          // Now apply base relationship values scaled by individual modifiers
		          switch (type)
		          {
		          case RelationshipType.kin:
		          relData.Affiliation += 8 * AffiliationModifier;
		          relData.Obligation += 6 * ObligationModifier;
		          relData.Reciprocity += 6 * ReciprocityModifier;
		          // Kin authority/influence determined by age difference
		          if (me.age > relData.pepo.age)
		          {
				float ageDiff = me.age - relData.pepo.age;
				ageDiff /= 80f; // scale down age difference
				relData.Authority += 3 * AuthorityModifier * ageDiff;
				relData.Influence -= 2 * InfluenceModifier;
		          }
		          else
		          {
				relData.Influence += 2 * InfluenceModifier;
		          }
		          break;

		          case RelationshipType.parent:
		          relData.Authority += 7 * AuthorityModifier;
		          relData.Obligation += 5 * ObligationModifier;
		          relData.Affiliation += 6 * AffiliationModifier;
		          relData.Confidence += 4 * ConfidenceModifier;
		          relData.Reciprocity -= 2 * ReciprocityModifier;
		          break;

		          case RelationshipType.offspring:
		          relData.Influence += 6 * InfluenceModifier;
		          relData.Obligation += 3 * ObligationModifier;
		          relData.Affiliation += 6 * AffiliationModifier;
		          relData.Confidence += 5 * ConfidenceModifier;
		          relData.Reciprocity += 4 * ReciprocityModifier;
		          break;

		          case RelationshipType.coparent:
		          relData.Obligation += 7 * ObligationModifier;
		          relData.Reciprocity += 6 * ReciprocityModifier;
		          relData.Affiliation += 6 * AffiliationModifier;
		          relData.Confidence += 5 * ConfidenceModifier;
		          relData.Influence += 3 * InfluenceModifier;
		          relData.Authority += 2 * AuthorityModifier;
		          break;

		          case RelationshipType.peer:
		          relData.Affiliation += 4 * AffiliationModifier;
		          relData.Influence += 3 * InfluenceModifier;
		          relData.Reciprocity += 3 * ReciprocityModifier;
		          relData.Confidence += 2 * ConfidenceModifier;
		          break;

		          case RelationshipType.acquaintence:
		          relData.Affiliation += 1 * AffiliationModifier;
		          relData.Confidence += 1 * ConfidenceModifier;
		          break;

		          case RelationshipType.friend:
		          relData.Affiliation += 5 * AffiliationModifier;
		          relData.Intimacy += 3 * IntimacyModifier;
		          relData.Confidence += 4 * ConfidenceModifier;
		          relData.Reciprocity += 4 * ReciprocityModifier;
		          relData.Obligation += 3 * ObligationModifier;
		          relData.Influence += 2 * InfluenceModifier;
		          break;

		          case RelationshipType.bestie:
		          relData.Affiliation += 7 * AffiliationModifier;
		          relData.Intimacy += 6 * IntimacyModifier;
		          relData.Confidence += 6 * ConfidenceModifier;
		          relData.Reciprocity += 6 * ReciprocityModifier;
		          relData.Obligation += 5 * ObligationModifier;
		          relData.Influence += 4 * InfluenceModifier;
		          relData.Attachment += 5 * AttachmentModifier;
		          break;

		          case RelationshipType.enemy:
		          relData.Animosity += 6 * AnimosityModifier;
		          relData.Affiliation -= 3 * AffiliationModifier;
		          relData.Confidence -= 4 * ConfidenceModifier;
		          relData.Reciprocity -= 2 * ReciprocityModifier;
		          break;

		          case RelationshipType.rival:
		          relData.Animosity += 4 * AnimosityModifier;
		          relData.Affiliation -= 1 * AffiliationModifier;
		          relData.Confidence += 2 * ConfidenceModifier;
		          relData.Reciprocity += 1 * ReciprocityModifier;
		          break;

		          case RelationshipType.lover:
		          relData.Intimacy += 8 * IntimacyModifier;
		          relData.Romance += 9 * RomanceModifier;
		          relData.Desire += 8 * DesireModifier;
		          relData.Affection += 7 * AffectionModifier;
		          relData.Attachment += 6 * AttachmentModifier;
		          relData.Confidence += 6 * ConfidenceModifier;
		          relData.Reciprocity += 5 * ReciprocityModifier;
		          relData.Obligation += 4 * ObligationModifier;
		          break;

		          case RelationshipType.ex:
		          relData.Animosity += 4 * AnimosityModifier;
		          relData.Romance -= 6 * RomanceModifier;
		          relData.Desire -= 5 * DesireModifier;
		          relData.Intimacy -= 3 * IntimacyModifier;
		          relData.Confidence -= 3 * ConfidenceModifier;
		          relData.Affiliation -= 2 * AffiliationModifier;
		          break;

		          case RelationshipType.spouse:
		          relData.Obligation += 8 * ObligationModifier;
		          relData.Reciprocity += 7 * ReciprocityModifier;
		          relData.Intimacy += 7 * IntimacyModifier;
		          relData.Confidence += 7 * ConfidenceModifier;
		          relData.Affiliation += 8 * AffiliationModifier;
		          relData.Attachment += 6 * AttachmentModifier;
		          relData.Romance += 6 * RomanceModifier;
		          relData.Desire += 6 * DesireModifier;
		          break;

		          case RelationshipType.mentor:
		          relData.Authority += 6 * AuthorityModifier;
		          relData.Confidence += 5 * ConfidenceModifier;
		          relData.Obligation += 3 * ObligationModifier;
		          relData.Influence += 4 * InfluenceModifier;
		          relData.Affiliation += 3 * AffiliationModifier;
		          break;

		          case RelationshipType.mentee:
		          relData.Influence += 2 * InfluenceModifier;
		          relData.Confidence += 4 * ConfidenceModifier;
		          relData.Obligation += 2 * ObligationModifier;
		          relData.Reciprocity += 3 * ReciprocityModifier;
		          relData.Affiliation += 3 * AffiliationModifier;
		          break;

		          case RelationshipType.leader:
		          relData.Authority += 8 * AuthorityModifier;
		          relData.Confidence += 5 * ConfidenceModifier;
		          relData.Obligation += 4 * ObligationModifier;
		          relData.Influence += 6 * InfluenceModifier;
		          relData.Affiliation += 4 * AffiliationModifier;
		          break;

		          case RelationshipType.follower:
		          relData.Influence += 3 * InfluenceModifier;
		          relData.Confidence += 4 * ConfidenceModifier;
		          relData.Obligation += 5 * ObligationModifier;
		          relData.Reciprocity += 4 * ReciprocityModifier;
		          relData.Affiliation += 4 * AffiliationModifier;
		          break;

		          case RelationshipType.incompatible:
		          // No changes
		          break;

		          case RelationshipType.courting:
		          relData.Romance += 6 * RomanceModifier;
		          relData.Desire += 5 * DesireModifier;
		          relData.Intimacy += 3 * IntimacyModifier;
		          relData.Confidence += 3 * ConfidenceModifier;
		          relData.Reciprocity += 4 * ReciprocityModifier;
		          relData.Affiliation += 3 * AffiliationModifier;
		          break;
		          }
	     }

	     // Now define all the modifier calculation functions:

	     private static float CalculateConfidenceModifier(PepoData me)
	     {
		          // Base confidence influenced by Honesty and Agreeableness
		          float modifier = 1.0f;
		          modifier += (me.personality.Honesty.value - 5f) * 0.04f; // Honest people are more trusting
		          modifier += (me.personality.Agreeableness.value - 5f) * 0.03f; // Agreeable people trust more
		          modifier += (me.mood.Trust.value) * 0.02f; // Mood affects trust

		          // Values affecting confidence
		          foreach (Value value in me.values)
		          {
				float strength = (value.value - 5f) / 5f;
				switch (value.descriptor)
				{
				case "Security":
				modifier -= strength * 0.1f;
				break; // Security-conscious = less trusting
				}
		          }

		          return Mathf.Clamp(modifier, 0.5f, 2.0f);
	     }

	     private static float CalculateAuthorityModifier(PepoData me)
	     {
		          float modifier = 1.0f;
		          modifier -= (me.personality.Honesty.value - 5f) * 0.03f; // dishonest people are harder to control
		          modifier += (me.personality.Emotionality.value - 5f) * 0.02f; // emotional people feel everything more strongly
		          modifier += (me.personality.Agreeableness.value - 5f) * 0.02f; // Agreeable people are easier to control

		          foreach (Value value in me.values)
		          {
				float strength = (value.value - 5f) / 5f;
				switch (value.descriptor)
				{
				case "Power":
				modifier -= strength * 0.2f;
				break; // Power-seeking = high authority
				case "Conformity":
				modifier += strength * 0.1f;
				break; // Conformist = accept authority
				case "Agency":
				modifier -= strength * 0.15f;
				break; // Autonomous = resist authority
				case "Universalism":
				modifier -= strength * 0.1f;
				break; // Egalitarian = resist hierarchy
				}
		          }

		          return Mathf.Clamp(modifier, 0.3f, 2.5f);
	     }

	     private static float CalculateInfluenceModifier(PepoData me)
	     {
		          float modifier = 1.0f;

		          modifier += (me.personality.Emotionality.value - 5f) * 0.02f; // emotional people feel everything more strongly
		          modifier += (me.personality.Honesty.value - 5f) * 0.03f; // dishonest people percieve themselves to have more influence
		          modifier -= (me.personality.Agreeableness.value - 5f) * 0.02f; // Agreeable people don't try to sway others

		          foreach (Value value in me.values)
		          {
				float strength = (value.value - 5f) / 5f;
				switch (value.descriptor)
				{
				case "Power":
				modifier += strength * 0.15f;
				break;
				case "Achievement":
				modifier += strength * 0.1f;
				break;
				case "Agency":
				modifier += strength * 0.1f;
				break; // Autonomous people seek influence
				case "Conformity":
				modifier -= strength * 0.1f;
				break; // Conformists don't seek personal influence
				}
		          }

		          return Mathf.Clamp(modifier, 0.4f, 2.0f);
	     }

	     private static float CalculateObligationModifier(PepoData me)
	     {
		          float modifier = 1.0f;

		          modifier += (me.personality.Emotionality.value - 5f) * 0.02f; // emotional people feel everything more strongly
		          modifier += (me.personality.Conscientiousness.value - 5f) * 0.05f; // Conscientious = feel obligation
		          modifier += (me.personality.Agreeableness.value - 5f) * 0.04f; // Agreeable = feel obligation

		          foreach (Value value in me.values)
		          {
				float strength = (value.value - 5f) / 5f;
				switch (value.descriptor)
				{
				case "Benevolence":
				modifier += strength * 0.2f;
				break; // Helpful = high obligation
				case "Conformity":
				modifier += strength * 0.15f;
				break; // Rule-following = high obligation
				case "Tradition":
				modifier += strength * 0.1f;
				break; // Traditional = feel obligation
				case "Power":
				modifier -= strength * 0.15f;
				break; // Power-seeking = resist obligation
				case "Agency":
				modifier -= strength * 0.1f;
				break; // Autonomous = resist obligation
				}
		          }

		          return Mathf.Clamp(modifier, 0.3f, 2.5f);
	     }

	     private static float CalculateReciprocityModifier(PepoData me)
	     {
		          float modifier = 1.0f;

		          modifier += (me.personality.Emotionality.value - 5f) * 0.02f; // emotional people feel everything more strongly
		          modifier += (me.personality.Honesty.value - 5f) * 0.04f; // Honest people expect fairness
		          modifier += (me.personality.Conscientiousness.value - 5f) * 0.03f; // Conscientious = keep track

		          foreach (Value value in me.values)
		          {
				float strength = (value.value - 5f) / 5f;
				switch (value.descriptor)
				{
				case "Fairness":
				modifier += strength * 0.15f;
				break;
				case "Benevolence":
				modifier -= strength * 0.1f;
				break; // Benevolent give without expecting return
				case "Power":
				modifier += strength * 0.1f;
				break; // Power-seekers take without reciprocating
				case "Hedonism":
				modifier += strength * 0.08f;
				break; // Hedonists focus on receiving
				}
		          }

		          return Mathf.Clamp(modifier, 0.4f, 2.0f);
	     }

	     private static float CalculateIntimacyModifier(PepoData me)
	     {
		          float modifier = 1.0f;

		          modifier += (me.personality.Openness.value - 5f) * 0.04f; // Open people form intimate connections
		          modifier += (me.personality.Emotionality.value - 5f) * 0.03f; // Emotional people seek intimacy

		          foreach (Value value in me.values)
		          {
				float strength = (value.value - 5f) / 5f;
				switch (value.descriptor)
				{
				case "Stimulation":
				modifier += strength * 0.1f;
				break; // Seek intense experiences
				case "Security":
				modifier -= strength * 0.08f;
				break; // Security-conscious = guarded
				}
		          }

		          return Mathf.Clamp(modifier, 0.5f, 1.8f);
	     }

	     private static float CalculateRomanceModifier(PepoData me)
	     {
		          float modifier = 1.0f;
		          modifier += (me.personality.Emotionality.value - 5f) * 0.05f; // Emotional = romantic
		          modifier += (me.personality.Openness.value - 5f) * 0.03f; // Open = appreciate romance

		          foreach (Value value in me.values)
		          {
				float strength = (value.value - 5f) / 5f;
				switch (value.descriptor)
				{
				case "Hedonism":
				modifier += strength * 0.15f;
				break; // Pleasure-seeking = romantic
				case "Stimulation":
				modifier += strength * 0.1f;
				break; // Seek intense experiences
				case "Achievement":
				modifier -= strength * 0.08f;
				break; // Achievement-focused = less romantic
				}
		          }

		          return Mathf.Clamp(modifier, 0.4f, 2.0f);
	     }

	     private static float CalculateDesireModifier(PepoData me)
	     {
		          float modifier = 1.0f;
		          modifier += (me.personality.Emotionality.value - 5f) * 0.04f; // Emotional = more desire

		          foreach (Value value in me.values)
		          {
				float strength = (value.value - 5f) / 5f;
				switch (value.descriptor)
				{
				case "Hedonism":
				modifier += strength * 0.2f;
				break; // Pleasure-seeking = high desire
				case "Stimulation":
				modifier += strength * 0.15f;
				break; // Seek intense experiences
				case "Tradition":
				modifier -= strength * 0.1f;
				break; // Traditional = restrained desire
				case "Conformity":
				modifier -= strength * 0.08f;
				break; // Conformist = restrained desire
				}
		          }

		          return Mathf.Clamp(modifier, 0.3f, 2.2f);
	     }

	     private static float CalculateAnimosityModifier(PepoData me)
	     {
		          float modifier = 1.0f;
		          modifier -= (me.personality.Agreeableness.value - 5f) * 0.06f; // Agreeable = less animosity
		          modifier += (me.personality.Emotionality.value - 5f) * 0.04f; // Emotional = more animosity

		          foreach (Value value in me.values)
		          {
				float strength = (value.value - 5f) / 5f;
				switch (value.descriptor)
				{
				case "Benevolence":
				modifier -= strength * 0.15f;
				break; // Helpful = less animosity
				case "Universalism":
				modifier -= strength * 0.1f;
				break; // Tolerant = less animosity
				case "Security":
				modifier += strength * 0.08f;
				break; // Insecure = more animosity
				}
		          }

		          return Mathf.Clamp(modifier, 0.2f, 2.0f);
	     }

	     private static float CalculateAffectionModifier(PepoData me)
	     {
		          float modifier = 1.0f;
		          modifier += (me.personality.Agreeableness.value - 5f) * 0.05f; // Agreeable = more affectionate
		          modifier += (me.personality.Emotionality.value - 5f) * 0.04f; // Emotional = more affectionate

		          foreach (Value value in me.values)
		          {
				float strength = (value.value - 5f) / 5f;
				switch (value.descriptor)
				{
				case "Benevolence":
				modifier += strength * 0.15f;
				break; // Helpful = affectionate
				case "Hedonism":
				modifier += strength * 0.1f;
				break; // Pleasure-seeking = affectionate
				case "Achievement":
				modifier -= strength * 0.08f;
				break; // Achievement-focused = less affectionate
				}
		          }

		          return Mathf.Clamp(modifier, 0.5f, 1.8f);
	     }

	     private static float CalculateAffiliationModifier(PepoData me)
	     {
		          float modifier = 1.0f;
		          modifier += (me.personality.Extraversion.value - 5f) * 0.05f; // Extraverts seek affiliation

		          foreach (Value value in me.values)
		          {
				float strength = (value.value - 5f) / 5f;
				switch (value.descriptor)
				{
				case "Universalism":
				modifier += strength * 0.15f;
				break; // Inclusive = high affiliation
				case "Benevolence":
				modifier += strength * 0.1f;
				break; // Helpful = high affiliation
				case "Conformity":
				modifier += strength * 0.1f;
				break; // Conformist = group affiliation
				case "Agency":
				modifier -= strength * 0.08f;
				break; // Autonomous = less affiliation
				}
		          }

		          return Mathf.Clamp(modifier, 0.4f, 1.8f);
	     }

	     private static float CalculateAttachmentModifier(PepoData me)
	     {
		          float modifier = 1.0f;
		          modifier += (me.personality.Emotionality.value - 5f) * 0.06f; // Emotional = more attached
		          modifier += (me.personality.Agreeableness.value - 5f) * 0.03f; // Agreeable = more attached

		          foreach (Value value in me.values)
		          {
				float strength = (value.value - 5f) / 5f;
				switch (value.descriptor)
				{
				case "Security":
				modifier += strength * 0.15f;
				break; // Security-seeking = high attachment
				case "Stimulation":
				modifier -= strength * 0.1f;
				break; // Novelty-seeking = less attachment
				case "Agency":
				modifier -= strength * 0.08f;
				break; // Autonomous = less attachment
				}
		          }

		          return Mathf.Clamp(modifier, 0.4f, 1.8f);
	     }

	     //take a list of pepos and return it in an ordered list based on total power (influence + recipricocity - obligation - authority). this tells me who i have the most power over
	     public static List<PepoData> OrderByPower(List<PepoRelationshipData> relationships)
	     {
		          List<PepoData> orderedPepo = new List<PepoData>();
		          Dictionary<PepoData, float> powerDict = new Dictionary<PepoData, float>();
		          foreach (var rel in relationships)
		          {
				float power = rel.Influence + rel.Reciprocity - rel.Obligation - rel.Authority;
				powerDict.Add(rel.pepo, power);
		          }
		          // Order by power in descending order (highest power first) and return as list
		          return powerDict.OrderByDescending(kvp => kvp.Value)
			          .Select(kvp => kvp.Key)
			          .ToList();
	     }

	     //take alist of relationship data and return a float to tell me how much power I have over my whole social circle
	     public static float TotalPower(List<PepoRelationshipData> relationships)
	     {
		          float total = 0f;
		          foreach (var rel in relationships)
		          {
				total += rel.Influence + rel.Reciprocity - rel.Obligation - rel.Authority;
		          }
		          return total;
	     }

	     //take alist of pepos and return it in an ordered list based on how much power they have over me (authority + obligation - influence - reciprocity)
	     public static List<PepoData> OrderByPowerOverMe(List<PepoRelationshipData> relationships)
	     {
		          List<PepoData> orderedPepo = new List<PepoData>();
		          Dictionary<PepoData, float> powerDict = new Dictionary<PepoData, float>();
		          foreach (var rel in relationships)
		          {
				float power = rel.Authority + rel.Obligation - rel.Influence - rel.Reciprocity;
				powerDict.Add(rel.pepo, power);
		          }
		          // Order by power in descending order (highest power first) and return as list
		          return powerDict.OrderByDescending(kvp => kvp.Value)
			          .Select(kvp => kvp.Key)
			          .ToList();
	     }
	     //take alist of relationship data and return a float to tell me how much power people have over me
	     public static float TotalPowerOverMe(List<PepoRelationshipData> relationships)
	     {
		          float total = 0f;
		          foreach (var rel in relationships)
		          {
				total += rel.Authority + rel.Obligation - rel.Influence - rel.Reciprocity;
		          }
		          return total;
	     }
	     //take a list of pepos and rank them by power. ie get their totalPower, subract totalPowerOverMe and order them by that value
	     public static List<PepoData> OrderByNetPower(List<PepoData> population)
	     {
		          List<PepoData> orderedPepo = new List<PepoData>();
		          Dictionary<PepoData, float> powerDict = new Dictionary<PepoData, float>();
		          foreach (var pepo in population)
		          {
				List<PepoRelationshipData> relationships = new List<PepoRelationshipData>();
				foreach (var relList in pepo.RelationshipLibrary.Values)
				{
					     relationships.AddRange(relList);
				}
				float netPower = TotalPower(relationships) - TotalPowerOverMe(relationships);
				powerDict.Add(pepo, netPower);
		          }
		          // Order by net power in descending order (highest net power first) and return as list
		          return powerDict.OrderByDescending(kvp => kvp.Value)
			          .Select(kvp => kvp.Key)
			          .ToList();
	     }
	     //take a list of relationship data and return the sum of confidence
	     public static float TotalConfidence(List<PepoRelationshipData> relationships)
	     {
		          float total = 0f;
		          foreach (var rel in relationships)
		          {
				total += rel.Confidence;
		          }
		          return total;
	     }
	     //take a list of relationship data and return the sum of influence
	     public static float TotalInfluence(List<PepoRelationshipData> relationships)
	     {
		          float total = 0f;
		          foreach (var rel in relationships)
		          {
				total += rel.Influence;
		          }
		          return total;
	     }
	     //take a list of relationship data and return the sum of authority
	     public static float TotalAuthority(List<PepoRelationshipData> relationships)
	     {
		          float total = 0f;
		          foreach (var rel in relationships)
		          {
				total += rel.Authority;
		          }
		          return total;
	     }
	     //take a list of relationship data and return the sum of obligation
	     public static float TotalObligation(List<PepoRelationshipData> relationships)
	     {
		          float total = 0f;
		          foreach (var rel in relationships)
		          {
				total += rel.Obligation;
		          }
		          return total;
	     }
	     //take a list of relationship data and return the sum of reciprocity
	     public static float TotalReciprocity(List<PepoRelationshipData> relationships)
	     {
		          float total = 0f;
		          foreach (var rel in relationships)
		          {
				total += rel.Reciprocity;
		          }
		          return total;
	     }
	     //take a list of relationship data and return the sum of affiliation
	     public static float TotalAffiliation(List<PepoRelationshipData> relationships)
	     {
		          float total = 0f;
		          foreach (var rel in relationships)
		          {
				total += rel.Affiliation;
		          }
		          return total;
	     }
	     //take a list of relationship data and return the sum of animosity
	     public static float TotalAnimosity(List<PepoRelationshipData> relationships)
	     {
		          float total = 0f;
		          foreach (var rel in relationships)
		          {
				total += rel.Animosity;
		          }
		          return total;
	     }
}
