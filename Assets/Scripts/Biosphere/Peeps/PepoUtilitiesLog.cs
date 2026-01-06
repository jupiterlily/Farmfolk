using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class PepoLogUtilities
{

	     public static void LogBirthEvent(List<PepoData> children, bool multipleBirth = false, bool identicalTwins = false)
	     {
		          if (children == null || children.Count == 0)
		          {
				Debug.LogWarning("<color=#6667ab>[BirthManager]</color> Attempted to log birth event with no children");
				return;
		          }

		          PepoData firstChild = children[0];
		          string motherName = firstChild.mother?.fullname ?? "Unknown Mother";
		          string fatherName = firstChild.father?.fullname ?? "Unknown Father";
		          float motherAge = firstChild.mother?.age ?? 0f;

		          // Determine birth type with more descriptive terms
		          string birthType = GetBirthType(children.Count, multipleBirth, identicalTwins);
		          string childrenDescription = GetChildrenDescription(children);

		          // Generate contextual message based on parents' age and situation
		          string contextualMessage = GetContextualBirthMessage(firstChild.mother, firstChild.father, children.Count);

		          // Build the main message
		          string mainMessage = $"<color=#6667ab>[BirthManager]</color> {motherName} ({motherAge:F0}) and {fatherName} welcome {birthType} {childrenDescription}";

		          // Add contextual message if available
		          if (!string.IsNullOrEmpty(contextualMessage))
		          {
				mainMessage += $". {contextualMessage}";
		          }

		          // Add family situation info
		          string familyInfo = GetFamilySituationInfo(firstChild.mother, firstChild.father);
		          if (!string.IsNullOrEmpty(familyInfo))
		          {
				mainMessage += $". {familyInfo}";
		          }

		          Debug.Log(mainMessage);
	     }

	     private static string GetBirthType(int childCount, bool multipleBirth, bool identicalTwins)
	     {
		          if (childCount == 1)
				return "a new baby";

		          if (multipleBirth)
		          {
				return childCount switch
				{
					     2 => identicalTwins ? "identical twins" : "twins",
					     3 => "triplets",
					     4 => "quadruplets",
					     5 => "quintuplets",
					     6 => "sextuplets",
					     _ => $"{childCount} babies"
				};
		          }

		          return $"{childCount} children";
	     }

	     private static string GetChildrenDescription(List<PepoData> children)
	     {
		          if (children.Count == 1)
		          {
				PepoData child = children[0];
				string gender = GetGenderTerm(child);
				return $"- a {gender} named {child.forename}";
		          }

		          // Group children by gender
		          var boys = children.Where(c => c.body.paternalChromosome.sexChromosome == "XY").ToList();
		          var girls = children.Where(c => c.body.paternalChromosome.sexChromosome == "XX").ToList();

		          List<string> descriptions = new List<string>();

		          if (boys.Count == 1)
		          {
				descriptions.Add($"a boy named {boys[0].forename}");
		          }
		          else if (boys.Count > 1)
		          {
				string boyNames = string.Join(", ", boys.Take(boys.Count - 1).Select(b => b.forename)) +
					  (boys.Count > 2 ? "," : "") +
					  (boys.Count > 1 ? " and " + boys.Last().forename : "");
				descriptions.Add($"{boys.Count} boys: {boyNames}");
		          }

		          if (girls.Count == 1)
		          {
				descriptions.Add($"a girl named {girls[0].forename}");
		          }
		          else if (girls.Count > 1)
		          {
				string girlNames = string.Join(", ", girls.Take(girls.Count - 1).Select(g => g.forename)) +
					   (girls.Count > 2 ? "," : "") +
					   (girls.Count > 1 ? " and " + girls.Last().forename : "");
				descriptions.Add($"{girls.Count} girls: {girlNames}");
		          }

		          // Handle mixed genders naturally
		          if (descriptions.Count == 2)
		          {
				return $"- {descriptions[0]} and {descriptions[1]}";
		          }

		          return $"- {string.Join(" and ", descriptions)}";
	     }

	     private static string GetGenderTerm(PepoData pepo)
	     {
		          // More nuanced gender terms based on presentation and identity although this needs to be modified to account for trans, in and out the closet, intersex, geenderfluid, agender, man/woman, girl boy also need to account for age
		          if (pepo.body.paternalChromosome.sexChromosome == "XY")
		          {
				if (pepo.ssd.presentsFemme && !pepo.ssd.presentsMasc)
					     return "trans girl";
				else if (!pepo.ssd.presentsFemme && pepo.ssd.presentsMasc)
					     return "boy";
				else
					     return pepo.ssd.isFemme ? "trans girl" : "boy";
		          }
		          else
		          {
				if (pepo.ssd.presentsMasc && !pepo.ssd.presentsFemme)
					     return "trans boy";
				else if (!pepo.ssd.presentsMasc && pepo.ssd.presentsFemme)
					     return "girl";
				else
					     return pepo.ssd.isMasc ? "trans boy" : "girl";
		          }
	     }

	     private static string GetContextualBirthMessage(PepoData mother, PepoData father, int childCount)
	     {
		          if (mother == null)
				return "";

		          List<string> messages = new List<string>();

		          // Age-based messages
		          if (mother.age < 18f)
		          {
				messages.Add("a very young mother");
		          }
		          else if (mother.age > 35f)
		          {
				messages.Add(mother.age > 40f ? "an older mother" : "a mature mother");
		          }

		          if (father != null && father.age - mother.age > 10f)
		          {
				messages.Add("a significant age gap between parents");
		          }

		          // Experience-based messages
		          int existingChildren = mother.offspring?.Count(o => o != null && o.isAlive) ?? 0;
		          if (existingChildren == 0)
		          {
				messages.Add("their first child");
		          }
		          else if (existingChildren >= 3)
		          {
				messages.Add($"expanding their already large family of {existingChildren} children");
		          }

		          // Multiple birth excitement
		          if (childCount > 1)
		          {
				messages.Add("quite a surprise with multiple births");
		          }

		          // Relationship status
		          if (mother.RelationshipLibrary != null &&
		              mother.RelationshipLibrary.ContainsKey(RelationshipType.lover))
		          {
				var lovers = PepoUtilitiesBonds.ShipsToPepos(mother.RelationshipLibrary[RelationshipType.lover]);
				if (!lovers.Contains(father))
				{
					     messages.Add("the biological father is not the mother's current partner");
				}
		          }

		          return messages.Count > 0 ? string.Join(", ", messages) : "";
	     }

	     private static string GetFamilySituationInfo(PepoData mother, PepoData father)
	     {
		          List<string> info = new List<string>();

		          if (mother != null)
		          {
				int motherLivingChildren = mother.offspring?.Count(o => o != null && o.isAlive) ?? 0;
				if (motherLivingChildren > 1)
				{
					     info.Add($"This brings {mother.forename}'s total to {motherLivingChildren} children");
				}

				// Check if grandparents are alive
				bool hasLivingGrandparents = (mother.mother?.isAlive == true) || (mother.father?.isAlive == true) ||
					             (father?.mother?.isAlive == true) || (father?.father?.isAlive == true);
				if (hasLivingGrandparents)
				{
					     info.Add("The newborn has living grandparents");
				}
		          }

		          // Check if parents are in a committed relationship
		          if (mother != null && father != null)
		          {
				bool arePartners = mother.RelationshipLibrary != null &&
					   mother.RelationshipLibrary.ContainsKey(RelationshipType.lover) &&
					   PepoUtilitiesBonds.ShipsToPepos(mother.RelationshipLibrary[RelationshipType.lover])
					       .Contains(father);

				if (arePartners)
				{
					     info.Add("The parents are in a committed relationship");
				}
		          }

		          return info.Count > 0 ? string.Join(". ", info) : "";
	     }

	     // Additional helper method to log pregnancy announcements
	     public static void LogPregnancyAnnouncement(PepoData mother, PepoData father)
	     {
		          if (mother?.womb == null)
				return;

		          string message = $"<color=#6667ab>[BirthManager]</color> {mother.fullname}";

		          if (father != null)
		          {
				message += $" and {father.fullname}";
		          }

		          message += " are expecting a child";

		          // Add due date approximation if available
		          if (mother.womb[0].gestation > 0)
		          {
				float weeksPregnant = 40f - mother.womb[0].weeks;
				message += $", {weeksPregnant:F0} weeks along";
		          }

		          Debug.Log(message);
	     }

	     public static void LogPepo(PepoData pepo)
	     {
		          string tense = "is";
		          if (!pepo.isAlive)
		          {
				tense = "was";
		          }
		          Debug.Log($"{PepoUtilities.GetFullName(pepo)} {tense} {Age(pepo)} old. They are {GetGenderExpression(pepo)}. They value {Values(pepo, 4)}. It seems like they are {Hexaco(pepo.personality)}");
	     }
	     public static string Age(PepoData pepo)
	     {
		          string age = "";
		          if (pepo.age < 1.5f)
		          {
				age = new string($"{pepo.age / 12:0} months");
		          }
		          if (pepo.age < 0.2308f)
		          {
				age = new string($"{pepo.age / 52:0} weeks");
		          }
		          else
		          {
				age = new string($"{pepo.age:0} years");
		          }
		          return age;
	     }
	     public static string GetPregnancyStatus(PepoData pepo)
	     {
		          //at the moment this just gets if they have awomb or not.
		          if (pepo.body.paternalChromosome.sexChromosome == "XX" && pepo.womb != null && pepo.womb.Count > 0)
				return $"({pepo.womb[0].bairns.Count} pregnancies)";
		          return "Not pregnant";
	     }
	     public static string GenderIdentity(PepoData pepo)
	     {
		          string genderIdentity = "";

		          if (pepo.ssd.isMasc && pepo.ssd.isFemme)
		          {
				genderIdentity = "genderfluid";
		          }
		          if (!pepo.ssd.isMasc && !pepo.ssd.isFemme)
		          {
				genderIdentity = "agender";
		          }
		          if (pepo.ssd.isMasc && !pepo.ssd.isFemme)
		          {
				genderIdentity = "a man";
		          }
		          if (!pepo.ssd.isMasc && pepo.ssd.isFemme)
		          {
				genderIdentity = "a woman";
		          }
		          if ((pepo.body.paternalChromosome.sexChromosome == "XY" && !pepo.ssd.isMasc && pepo.ssd.isFemme) || (pepo.body.paternalChromosome.sexChromosome == "XX" && !pepo.ssd.isFemme && pepo.ssd.isMasc))
		          {
				if (pepo.body.paternalChromosome.sexChromosome == "XY")
				{
					     genderIdentity = string.Concat("trans, ", genderIdentity);
				}
		          }
		          return genderIdentity;
	     }
	     public static string GetGenderExpression(PepoData pepo)
	     {
		          // Determine chromosomal sex first
		          string chromosomalSex;
		          bool intersex = false;
		          if (pepo.body.paternalChromosome.sexChromosome == "XY" && pepo.body.maternalChromosome.sexChromosome == "XX")
				chromosomalSex = "male";
		          else if (pepo.body.paternalChromosome.sexChromosome == "XX" && pepo.body.maternalChromosome.sexChromosome == "XX")
				chromosomalSex = "female";
		          else
		          {
				intersex = true;
				chromosomalSex = "intersex";
		          }

		          // Determine gender identity
		          string identity;
		          if (pepo.ssd.isMasc && pepo.ssd.isFemme)
				identity = "nonbinary";
		          else if (!pepo.ssd.isMasc && !pepo.ssd.isFemme)
				identity = "agender";
		          else if (pepo.ssd.isMasc && !pepo.ssd.isFemme)
				identity = "man";
		          else
				identity = "woman";

		          // Check if transgender (identity doesn't match chromosomal sex)
		          bool isTrans = (pepo.body.paternalChromosome.sexChromosome == "XY" && pepo.ssd.isFemme) || (pepo.body.paternalChromosome.sexChromosome == "XX" && pepo.ssd.isMasc);

		          // Determine openness about gender identity
		          string openness = "openly";
		          if (isTrans)
		          {
				// If presenting differently than identity, might be closeted
				bool presentsAsIdentity = (pepo.ssd.isFemme && pepo.ssd.presentsFemme) || (pepo.ssd.isMasc && pepo.ssd.presentsMasc);
				openness = presentsAsIdentity ? "openly" : "not openly";
		          }
		          else if (!intersex && !isTrans)
		          {
				openness = "cisgender";
		          }

		          // Determine presentation
		          string presentation;
		          if (pepo.ssd.presentsFemme && pepo.ssd.presentsMasc)
				presentation = "androgynous presentation";
		          else if (pepo.ssd.presentsFemme && !pepo.ssd.presentsMasc)
				presentation = "feminine presentation";
		          else if (!pepo.ssd.presentsFemme && pepo.ssd.presentsMasc)
				presentation = "masculine presentation";
		          else
				presentation = "neutral presentation";

		          // Age-appropriate terms
		          string ageTerm = pepo.age < 4 ? (pepo.age > 2 ? "baby" : "toddler") : (pepo.ssd.isPrepubescent ? "child" : "adolescent");
		          if (pepo.age < 23)
		          {
				return $"{ageTerm} with {presentation}";
		          }
		          // Build final description
		          if (isTrans)
		          {
				return $"{openness} trans {GetOrientationDescription(pepo)}, {identity} with {presentation}";
		          }
		          else
		          {
				return $"{openness} {GetOrientationDescription(pepo)}, {identity} with {presentation}";
		          }
	     }

	     public static string GetOrientationDescription(PepoData pepo)
	     {
		          if (pepo.ssd.isAsexual && !pepo.ssd.romanceFemme && !pepo.ssd.romanceMasc)
				return "asexual and aromantic";

		          if (pepo.ssd.isAsexual)
				return "asexual";

		          // Sexual orientation
		          string sexualOrientation = "straight";
		          if (pepo.ssd.likesMasc && pepo.ssd.likesFemme)
				sexualOrientation = "bisexual";
		          else if (pepo.ssd.likesMasc && !pepo.ssd.likesFemme && pepo.ssd.isFemme)
				sexualOrientation = "lesbian";
		          else if (!pepo.ssd.likesMasc && pepo.ssd.likesFemme && pepo.ssd.isMasc)
				sexualOrientation = "gay";
		          else
				sexualOrientation = "queer";

		          // Romantic orientation (if different from sexual)
		          string romanticOrientation = "pan";
		          if (pepo.ssd.romanceFemme && pepo.ssd.romanceMasc)
				romanticOrientation = "biromantic";

		          else if ((pepo.ssd.isFemme && pepo.ssd.romanceFemme && !pepo.ssd.romanceMasc) || (pepo.ssd.isMasc && pepo.ssd.romanceMasc && !pepo.ssd.romanceFemme))
				romanticOrientation = "homoromantic";

		          else if ((pepo.ssd.isFemme && !pepo.ssd.romanceFemme && pepo.ssd.romanceMasc) || (pepo.ssd.isMasc && !pepo.ssd.romanceMasc && pepo.ssd.romanceFemme))
				romanticOrientation = "heteroromantic";

		          else if (!pepo.ssd.romanceMasc && !pepo.ssd.romanceFemme)
				romanticOrientation = "aromantic";

		          return $"{sexualOrientation}, {romanticOrientation}";
	     }
	     public static string Values(PepoData pepo, int count = 3)
	     {
		          List<Value> ranked = PepoUtilitiesValues.GetTopValues(pepo.values, count);
		          string values = "";
		          for (int i = 0; i < count; i++)
		          {
				values += ranked[i].descriptor;
				if (i == count - 2)
				{
					     values += " and ";
				}
				else if (i < count - 2)
				{
					     values += ", ";
				}
		          }
		          return values;
	     }
	     public static string Needs(PepoData pepo, int count = 3)
	     {
		          if (pepo?.needs?.Physiological == null || pepo.needs.Safety == null)
				return string.Empty;

		          // Collect all urgent needs
		          var urgentNeeds = new List<string>();

		          // Check physiological needs
		          foreach (var need in pepo.needs.Physiological)
		          {
				if (need.value <= need.warningThreshold)
				{
					     urgentNeeds.Add(need.descriptor);
				}
		          }

		          // Check safety needs
		          foreach (var need in pepo.needs.Safety)
		          {
				if (need.value <= need.warningThreshold)
				{
					     urgentNeeds.Add(need.descriptor);
				}
		          }

		          // Limit to requested count
		          if (urgentNeeds.Count > count)
		          {
				urgentNeeds = urgentNeeds.Take(count).ToList();
		          }

		          // Format the output string
		          if (urgentNeeds.Count == 0)
		          {
				return string.Empty;
		          }
		          else if (urgentNeeds.Count == 1)
		          {
				return urgentNeeds[0];
		          }
		          else if (urgentNeeds.Count == 2)
		          {
				return $"{urgentNeeds[0]} and {urgentNeeds[1]}";
		          }
		          else
		          {
				string result = string.Join(", ", urgentNeeds.Take(urgentNeeds.Count - 1));
				result += $", and {urgentNeeds[urgentNeeds.Count - 1]}";
				return result;
		          }
	     }
	     public static void LogDemographicData(List<PepoData> people)
	     {
		          if (people == null || people.Count == 0)
		          {
				Debug.LogWarning("<color=#f88f58>[DemographicsManager] No people to analyze</color>");
				return;
		          }

		          var alivePeople = people.Where(p => p.isAlive).ToList();
		          var deceasedPeople = people.Where(p => !p.isAlive).ToList();

		          // Gender and identity breakdown
		          int men = 0, women = 0, nonBinary = 0, trans = 0, intersex = 0;
		          int openlyTrans = 0, closetedTrans = 0;

		          // Orientation breakdown
		          int straight = 0, gayLesbian = 0, bisexual = 0, asexual = 0, aromantic = 0, questioning = 0;

		          // Age groups
		          int infants = 0, children = 0, teens = 0, adults = 0, middleAged = 0, elderly = 0;
		          //average age
		          float totalAge = 0f;


		          foreach (var person in alivePeople)
		          {
				totalAge += person.age;
				// Age groups
				if (person.age < 2f)
					     infants++;
				else if (person.age < 13f)
					     children++;
				else if (person.age < 18f)
					     teens++;
				else if (person.age < 40f)
					     adults++;
				else if (person.age < 60f)
					     middleAged++;
				else
					     elderly++;
				//person.body.hasY
				// Gender and identity
				bool isTrans = (person.body.paternalChromosome.sexChromosome == "XY" && person.ssd.isFemme) || (person.body.paternalChromosome.sexChromosome == "XX" && person.ssd.isMasc);
				bool isNonBinary = (person.ssd.isMasc && person.ssd.isFemme) || (!person.ssd.isMasc && !person.ssd.isFemme);
				bool isIntersex = (person.body.maternalChromosome.sexChromosome == "XY");

				if (isIntersex)
					     intersex++;
				if (isTrans)
					     trans++;
				if (isNonBinary)
					     nonBinary++;

				if (isTrans)
				{
					     bool presentsAsIdentity = (person.ssd.isFemme && person.ssd.presentsFemme) ||
							(person.ssd.isMasc && person.ssd.presentsMasc);
					     if (presentsAsIdentity)
						          openlyTrans++;
					     else
						          closetedTrans++;
				}

				if (person.body.paternalChromosome.sexChromosome == "XY" && !isTrans && !isNonBinary)
					     men++;
				else if (person.body.paternalChromosome.sexChromosome == "XX" && !isTrans && !isNonBinary)
					     women++;

				// Orientation
				if (person.ssd.isAsexual)
					     asexual++;
				if (person.ssd.isAromantic)
					     aromantic++;

				if (person.ssd.likesMasc && person.ssd.likesFemme)
					     bisexual++;
				else if ((person.ssd.likesMasc && person.body.paternalChromosome.sexChromosome == "XY") || (person.ssd.likesFemme && person.body.paternalChromosome.sexChromosome == "XX"))
					     gayLesbian++;
				else if ((person.ssd.likesMasc && person.body.paternalChromosome.sexChromosome == "XX") || (person.ssd.likesFemme && person.body.paternalChromosome.sexChromosome == "XY"))
					     straight++;
				else
					     questioning++;
		          }

		          // Build demographic report
		          string report = $"<color=#f88f58>[DemographicsManager] Analysis of {alivePeople.Count} living people with an average age of {(totalAge / alivePeople.Count):0}:</color>\n";

		          report += $"<color=#6667ab>Gender Identity:</color> {men} men, {women} women, {nonBinary} non-binary, {trans} transgender ({openlyTrans} openly, {closetedTrans} closeted), {intersex} intersex\n";

		          report += $"<color=#6667ab>Sexual Orientation:</color> {straight} straight, {gayLesbian} gay/lesbian, {bisexual} bisexual, {asexual} asexual, {aromantic} aromantic, {questioning} questioning\n";

		          report += $"<color=#6667ab>Age Distribution:</color> {infants} infants, {children} children, {teens} teens, {adults} young adults, {middleAged} middle-aged, {elderly} elderly\n";

		          report += $"<color=#6667ab>Mortality:</color> {deceasedPeople.Count} deceased individuals";

		          Debug.Log(report);
	     }

	     // Example usage method that returns expression description for a single person
	     public static string GetPersonDescription(PepoData person)
	     {
		          if (person == null)
				return "Unknown person";

		          string expression = GetGenderExpression(person);
		          string orientation = GetOrientationDescription(person);
		          string ageDescription = Age(person);

		          return $"{person.fullname} is {ageDescription} old, {expression}, and {orientation}";
	     }

	     public static string Hexaco(HEXACO hexaco, int toList = 3)
	     {
		          string personality = string.Empty;
		          foreach (var aspect in hexaco.FullHexRange())
		          {
				if (aspect.value > 7 || aspect.value < 3)
				{

					     personality += string.Concat(AspectDegree(aspect.value, hexaco.maximumValue), aspect.descriptor, ", ");
				}
		          }
		          return personality;
	     }
	     public static string DescribePersonalityAspect(PersonalityAspect aspect)
	     {

		          return aspect.descriptor;

	     }

	     public static string AspectDegree(float value, float max)
	     {
		          string degree = string.Empty;
		          if (value > max * 0.9f || value < max * 0.1f)
		          {
				string[] options = { "incredibly ", "extremely ", "exceedingly ", "intensely ", "overly ", "exceptionally " };
				degree = options[Random.Range(0, options.Length)];
		          }
		          else if (value > max * 0.7f || value < max * 0.3f)
		          {
				string[] options = { "remarkably ", "really ", "pretty ", "rather ", "moderately ", "quite ", "especially ", "significantly ", "considerably ", "" };
				degree = options[Random.Range(0, options.Length)];
		          }
		          else if (value > max * 0.51f || value < max * 0.49f)
		          {
				string[] options = { "somewhat ", "slightly ", "marginally ", "more or less ", "" };
				degree = options[Random.Range(0, options.Length)];
		          }
		          else
		          {
				string[] options = { "kind of ", "a bit ", "a little " };
				degree = options[Random.Range(0, options.Length)];
		          }
		          return degree;
	     }

	     public static string LogDeath(PepoData Pepo)
	     {
		          string message = Pepo.age switch
		          {
				> 90f => $"{PepoUtilities.GetFullName(Pepo)} has passed away at the ripe old age of {Pepo.age:F0}",
				> 60f => $"{PepoUtilities.GetFullName(Pepo)} has died of old age at {Age(Pepo)} old",
				> 18f => $"{PepoUtilities.GetFullName(Pepo)} has died unexpectedly at just {Age(Pepo)} old",
				> 2f => $"{PepoUtilities.GetFullName(Pepo)} has died tragically young at {Age(Pepo)} old",
				< 2f => $"{PepoUtilities.GetFullName(Pepo)}, just {Age(Pepo)} old, didn't survive infancy",
				_ => $"{PepoUtilities.GetFullName(Pepo)} has died unexpectedly at {Age(Pepo)} old"
		          };

		          List<string> additionalMessages = new List<string>();

		          // Parent message - FIXED: was missing proper conditions
		          bool hasLivingMother = Pepo.mother != null && Pepo.mother.isAlive;
		          bool hasLivingFather = Pepo.father != null && Pepo.father.isAlive;

		          if (hasLivingMother && hasLivingFather)
		          {
				additionalMessages.Add($"Their parents {Pepo.mother.fullname} and {Pepo.father.fullname} are devastated by the loss of their child");
		          }
		          else if (hasLivingMother)
		          {
				additionalMessages.Add($"Their mother {Pepo.mother.fullname} feels lost without her child");
		          }
		          else if (hasLivingFather)
		          {
				additionalMessages.Add($"Their father {Pepo.father.fullname} is devastated");
		          }

		          // Child message - FIXED: was checking Count < 0 instead of > 0
		          int liveChildren = 0;
		          string myChildren = "";
		          if (Pepo.offspring.Count > 0) // FIXED: changed < to >
		          {
				foreach (var p in Pepo.offspring)
				{
					     if (p.isAlive)
					     {
						          liveChildren++;
						          myChildren += ". " + p.fullname;
					     }
				}
		          }

		          if (liveChildren >= 1)
		          {
				string childrenWord = liveChildren == 1 ? $"child, {Pepo.offspring[0].forename}, is" : "children";
				if (liveChildren >= 2)
				{
					     childrenWord += myChildren + " are";
					     string parentWord = Pepo.body.paternalChromosome.sexChromosome == "XY" == true ? "father" : "mother";
					     additionalMessages.Add($"{liveChildren} {childrenWord} now without a {parentWord}");
				}
		          }

		          if (Pepo.RelationshipLibrary.ContainsKey(RelationshipType.lover))
		          {
				// Partner message
				foreach (var partner in PepoUtilitiesBonds.ShipsToPepos(Pepo.RelationshipLibrary[RelationshipType.lover]))
				{
					     if (partner != null && partner.isAlive)
					     {
						          additionalMessages.Add($"Their partner, {partner.fullname}, is heartbroken");
					     }
				}
		          }
		          if (Pepo.RelationshipLibrary.ContainsKey(RelationshipType.friend))
		          {
				foreach (var kin in PepoUtilitiesBonds.ShipsToPepos(Pepo.RelationshipLibrary[RelationshipType.friend]))
				{
					     if (kin != null && kin.isAlive)
					     {
						          additionalMessages.Add($"Their kin, {kin.fullname}, is grieving");
					     }
				}
		          }
		          // Combine all messages
		          string fullMessage = message;
		          if (additionalMessages.Count > 0)
		          {
				fullMessage += ". " + string.Join(". ", additionalMessages);
		          }

		          Debug.Log($"<color=#ea6759>[DeathManager] {fullMessage}</color>");
		          return fullMessage;

	     }

	     public static void LogRelationship(PepoData lilGuy, PepoRelationshipData relationship)
	     {
		          string relations = string.Join(", ", relationship.relationshipTypes.ToString().ToArray());
		          Debug.Log($"<color=#4367ab>[RelationshipManager] {relationship.pepo.forename} is {lilGuy.forename}'s {relations} </color> ");
	     }

	     public static void LogPepoList(List<PepoData> lilGuys)
	     {

		          int infants = 0; //<2
		          int children = 0; //<18
		          int adults = 0; // >18
		          int elderly = 0; //>60
		          int men = 0;
		          int women = 0;
		          int alive = 0;
		          int deathtoll = 0;

		          foreach (var guy in lilGuys)
		          {
				if (guy.isAlive)
				{
					     if (guy.body.paternalChromosome.sexChromosome == "XY")
					     {
						          men++;
					     }
					     else
					     {
						          women++;
					     }
					     if (guy.age <= 18)
					     {
						          children++;
						          if (guy.age <= 2)
						          {
								infants++;
						          }
					     }
					     else if (guy.age >= 60)
					     {
						          elderly++;
					     }
					     else
					     {
						          adults++;
					     }
					     alive++;
				}
				else
				{
					     deathtoll++;
				}
		          }
		          Debug.Log($"<color=#f88f58>[DemographicsManager] {alive} peeps alive, {deathtoll} deaths, {women} women, {men} men. {children} children including {infants} infants and {elderly} elderly people supported by {adults} adults</color>");
	     }
}
