using System.Collections.Generic;

public static class PepoLibraryRoles
{
	     // Schwartz values: agency, stimulation, hedonism, achievement, power, 
	     // security, conformity, tradition, benevolence, universalism 
	     // Range: -1 (harmful) to +7 (live daily)
	     // Personality traits: 0-10 with 5 being neutral
	     public static List<PepoDataRoles> HunterGatherer = new List<PepoDataRoles>() {
		          new PepoDataRoles(
    "law speaker",
    "Memorizes and recounts the band's traditional laws and precedents.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Honesty", 7f, 10f),
	         new PepoRolePrerequisite("tradition", 3f, 7f),
	         new PepoRolePrerequisite("stability", 2f, 7f),
	         new PepoRolePrerequisite("dependability", 3f, 7f),
	         new PepoRolePrerequisite("Age", 35f, 80f),
    }, 0.01f, 0.02f,
    new List<string>() {
	         "mediator"
    }
),
		          new PepoDataRoles(
    "councillor",
    "Elder or advisor who helps make major decisions for the band.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("stability", 4f, 7f),
	         new PepoRolePrerequisite("tradition", 4f, 7f),
	         new PepoRolePrerequisite("Agreeableness", 4f, 8f),
	         new PepoRolePrerequisite("Age", 30f, 80f),
    }, 0.05f, 0.1f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "challenger",
    "Formally questions leaders and decisions to test their merit.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Honesty", 7f, 10f),
	         new PepoRolePrerequisite("Extraversion", 5f, 10f),
	         new PepoRolePrerequisite("dominance", 3f, 7f),
	         new PepoRolePrerequisite("curiosity", 4f, 7f),
	         new PepoRolePrerequisite("Age", 25f, 70f),
    }, 0f, 0.01f,
    new List<string>() {
	         "councillor"
    }
),
		          new PepoDataRoles(
    "scout",
    "Explores and reconnoiters new areas for resources and threats.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 5f, 10f),
	         new PepoRolePrerequisite("curiosity", 2f, 7f),
	         new PepoRolePrerequisite("autonomy", 3f, 7f),
	         new PepoRolePrerequisite("vitality", 5f, 7f),
	         new PepoRolePrerequisite("Age", 12f, 50f),
    }, 0.1f, 0.15f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "runner",
    "Courier and messenger for the band over long distances.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("vitality", 3f, 7f),
	         new PepoRolePrerequisite("dependability", 2f, 7f),
	         new PepoRolePrerequisite("accomplishment", 3f, 7f),
	         new PepoRolePrerequisite("autonomy", 3f, 7f),
	         new PepoRolePrerequisite("Age", 11f, 50f),
    }, 0.02f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "fisher",
    "Catches fish using various methods (spear, net, line).",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 4f, 8f),
	         new PepoRolePrerequisite("vitality", 3f, 7f),
	         new PepoRolePrerequisite("stability", 3f, 7f),
	         new PepoRolePrerequisite("excitement", 0f, 5f),
	         new PepoRolePrerequisite("Age", 8f, 120f),
    }, 0.02f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "ceremony leader",
    "Leads major rites of passage and seasonal rituals.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Extraversion", 6f, 10f),
	         new PepoRolePrerequisite("tradition", 5f, 7f),
	         new PepoRolePrerequisite("cohesion", 2f, 7f),
	         new PepoRolePrerequisite("Emotionality", 5f, 10f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
    }, 0.01f, 0.02f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "hunt leader",
    "Plans and leads hunting expeditions.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Extraversion", 6f, 10f),
	         new PepoRolePrerequisite("dominance", 4f, 7f),
	         new PepoRolePrerequisite("dependability", 2f, 7f),
	         new PepoRolePrerequisite("accomplishment", 4f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
    }, 0.02f, 0.03f,
    new List<string>() {
	         "animal tracker",
	         "scout",
	         "solo hunter"
    }
),
		          new PepoDataRoles(
    "midwife",
    "Manages pregnancy, childbirth, and postnatal care.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("caring", 5f, 7f),
	         new PepoRolePrerequisite("compassion", 3f, 7f),
	         new PepoRolePrerequisite("stability", 2f, 7f),
	         new PepoRolePrerequisite("dependability", 3f, 7f),
	         new PepoRolePrerequisite("Emotionality", 5f, 10f),
    }, 0.02f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "solo hunter",
    "Hunts small to medium game alone with stealth and skill.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("autonomy", 3f, 7f),
	         new PepoRolePrerequisite("vitality", 3f, 7f),
	         new PepoRolePrerequisite("accomplishment", 3f, 7f),
	         new PepoRolePrerequisite("safety", 2f, 5f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("Age", 16f, 50f),
    }, 0.05f, 0.1f,
    new List<string>() {
	         "aimer",
	         "striker",
	         "thrower",
	         "animal tracker"
    }
),
		          new PepoDataRoles(
    "healer",
    "Holistic practitioner treating illness and injury with multiple methods.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	         new PepoRolePrerequisite("caring", 3f, 7f),
	         new PepoRolePrerequisite("compassion", 3f, 7f),
	         new PepoRolePrerequisite("dependability", 3f, 7f),
	         new PepoRolePrerequisite("Openness", 5f, 8f),
	         new PepoRolePrerequisite("Emotionality", 4f, 8f),
    }, 0.02f, 0.05f,
    new List<string>() {
	         "bonesetter",
	         "midwife",
	         "plant identifier",
	         "mushroom identifier"
    }
),
		          new PepoDataRoles(
    "linguist",
    "Learns and speaks the languages of neighboring bands.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 7f, 10f),
	         new PepoRolePrerequisite("curiosity", 3f, 7f),
	         new PepoRolePrerequisite("tolerance", 5f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("Extraversion", 4f, 10f),
    }, 0.01f, 0.02f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "gatherer",
    "Collects plant foods, eggs, shellfish, and other staples.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 4f, 8f),
	         new PepoRolePrerequisite("curiosity", 4f, 7f),
	         new PepoRolePrerequisite("vitality", 3f, 7f),
	         new PepoRolePrerequisite("preservation", 4f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 4f, 8f),
    }, 0.3f, 0.5f,
    new List<string>() {
	         "scout",
	         "plant identifier"
    }
),
		          new PepoDataRoles(
    "enforcer",
    "Upholds the band's decisions and rules through physical authority.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("dominance", 4f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("obedience", 4f, 7f),
	         new PepoRolePrerequisite("cohesion", 3f, 7f),
	         new PepoRolePrerequisite("Extraversion", 4f, 8f),
	         new PepoRolePrerequisite("Honesty", 6f, 10f),
	         new PepoRolePrerequisite("Age", 18f, 50f),
    }, 0.01f, 0.02f,
    new List<string>() {
	         "councillor",
	         "law speaker"
    }
),
		          new PepoDataRoles(
    "trader",
    "Facilitates the exchange of goods and ideas between bands.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Extraversion", 5f, 10f),
	         new PepoRolePrerequisite("Agreeableness", 5f, 10f),
	         new PepoRolePrerequisite("accumulation", 3f, 7f),
	         new PepoRolePrerequisite("Honesty", 5f, 10f),
	         new PepoRolePrerequisite("curiosity", 4f, 7f),
	         new PepoRolePrerequisite("Age", 20f, 60f),
    }, 0.005f, 0.01f,
    new List<string>() {
	         "envoy",
	         "linguist"
    }
),
		          new PepoDataRoles(
    "envoy",
    "Represents the band in formal interactions with other groups.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Extraversion", 5f, 10f),
	         new PepoRolePrerequisite("Agreeableness", 5f, 10f),
	         new PepoRolePrerequisite("tolerance", 5f, 7f),
	         new PepoRolePrerequisite("Honesty", 5f, 10f),
	         new PepoRolePrerequisite("curiosity", 4f, 7f),
	         new PepoRolePrerequisite("Age", 20f, 60f),
    }, 0.005f, 0.01f,
    new List<string>() {
	         "runner",
	         "mediator"
    }
),
		          new PepoDataRoles(
    "boundary patrol",
    "Monitors the band's territory for intruders and threats.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("dependability", 3f, 7f),
	         new PepoRolePrerequisite("safety", 2f, 7f),
	         new PepoRolePrerequisite("stability", 3f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("vitality", 4f, 7f),
	         new PepoRolePrerequisite("Extraversion", 0f, 7f),
    }, 0.05f, 0.1f,
    new List<string>() {
	         "scout"
    }
),
		          new PepoDataRoles(
    "matchmaker",
    "Fosters alliances and relationships through strategic partnerships.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Extraversion", 5f, 10f),
	         new PepoRolePrerequisite("Agreeableness", 6f, 10f),
	         new PepoRolePrerequisite("Emotionality", 4f, 8f),
	         new PepoRolePrerequisite("cohesion", 4f, 7f),
	         new PepoRolePrerequisite("compassion", 3f, 7f),
    }, 0f, 0.01f,
    new List<string>() {
	         "mediator",
	         "councillor"
    }
),
		          new PepoDataRoles(
    "butcher",
    "Expertly and efficiently processes carcasses, honoring the animal.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("vitality", 4f, 7f),
	         new PepoRolePrerequisite("tradition", 3f, 7f),
	         new PepoRolePrerequisite("preservation", 4f, 7f),
	         new PepoRolePrerequisite("Emotionality", 2f, 6f),
    }, 0.03f, 0.05f,
    new List<string>() {
	         "solo hunter",
	         "team hunter"
    }
),
		          new PepoDataRoles(
    "animal tracker",
    "Reads and interprets animal signs for hunting.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 5f, 10f),
	         new PepoRolePrerequisite("curiosity", 4f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 8f),
	         new PepoRolePrerequisite("autonomy", 4f, 7f),
	         new PepoRolePrerequisite("excitement", 0f, 5f),
    }, 0.05f, 0.1f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "driving animals",
    "Directs game herds into traps or ambushes.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("accumulation", 3f, 7f),
	         new PepoRolePrerequisite("dominance", 3f, 7f),
	         new PepoRolePrerequisite("accomplishment", 3f, 7f),
	         new PepoRolePrerequisite("Extraversion", 3f, 10f),
	         new PepoRolePrerequisite("preservation", -1f, 5f),
    }, 0.02f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "sky reader",
    "Predicts weather and short-term climate patterns.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 5f, 10f),
	         new PepoRolePrerequisite("curiosity", 2f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 4f, 7f),
	         new PepoRolePrerequisite("stability", 3f, 7f),
	         new PepoRolePrerequisite("excitement", -1f, 4f),
    }, 0.02f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "firekeeper",
    "Master of creating, maintaining, and transporting fire.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	         new PepoRolePrerequisite("preservation", 3f, 7f),
	         new PepoRolePrerequisite("stability", 1f, 7f),
	         new PepoRolePrerequisite("dependability", 1f, 7f),
	         new PepoRolePrerequisite("excitement", -1f, 4f),
    }, 0.02f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "ancestor communer",
    "Acts as a conduit to the wisdom and will of ancestors.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Emotionality", 6f, 10f),
	         new PepoRolePrerequisite("tradition", 5f, 7f),
	         new PepoRolePrerequisite("preservation", 3f, 7f),
	         new PepoRolePrerequisite("Age", 40f, 120f),
    }, 0.01f, 0.02f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "striker",
    "Expert in close-quarter combat (club, spear).",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("vitality", 3f, 7f),
	         new PepoRolePrerequisite("dominance", 3f, 6f),
	         new PepoRolePrerequisite("accomplishment", 3f, 7f),
	         new PepoRolePrerequisite("Age", 14f, 75f),
    }, 0.05f, 0.1f,
    new List<string>() {
	         "aimer"
    }
),
		          new PepoDataRoles(
    "diver",
    "Dives for shellfish, crustaceans, and other aquatic resources.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("vitality", 3f, 7f),
	         new PepoRolePrerequisite("excitement", 4f, 7f),
	         new PepoRolePrerequisite("autonomy", 4f, 7f),
	         new PepoRolePrerequisite("Age", 14f, 45f),
    }, 0.01f, 0.03f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "climber",
    "Specialist in climbing trees or cliffs for food or vantage.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("vitality", 3f, 7f),
	         new PepoRolePrerequisite("excitement", 4f, 7f),
	         new PepoRolePrerequisite("autonomy", 4f, 7f),
	         new PepoRolePrerequisite("Age", 12f, 40f),
    }, 0.02f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "mediator",
    "Resolves interpersonal disputes and conflicts.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Agreeableness", 7f, 10f),
	         new PepoRolePrerequisite("Emotionality", 5f, 8f),
	         new PepoRolePrerequisite("tolerance", 2f, 7f),
	         new PepoRolePrerequisite("Honesty", 6f, 10f),
    }, 0.02f, 0.03f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "musician",
    "Creates and plays instruments for ritual and entertainment.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 6f, 10f),
	         new PepoRolePrerequisite("curiosity", 1f, 7f),
	         new PepoRolePrerequisite("cohesion", 3f, 7f),
	         new PepoRolePrerequisite("Emotionality", 5f, 10f),
    }, 0.02f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "star reader",
    "Uses celestial bodies for navigation and marking time.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 6f, 10f),
	         new PepoRolePrerequisite("curiosity", 3f, 7f),
	         new PepoRolePrerequisite("tradition", 3f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
    }, 0.01f, 0.02f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "bee whisperer",
    "Safely harvests honey and wax from wild bees.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Emotionality", 5f, 8f),
	         new PepoRolePrerequisite("tolerance", 2f, 7f),
	         new PepoRolePrerequisite("preservation", 4f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
    }, 0.01f, 0.02f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "painter",
    "Creates cave paintings, body art, and object decoration.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 6f, 10f),
	         new PepoRolePrerequisite("curiosity", 5f, 7f),
	         new PepoRolePrerequisite("excitement", 4f, 7f),
	         new PepoRolePrerequisite("Emotionality", 5f, 10f),
    }, 0.01f, 0.03f,
    new List<string>() {
	         "dye maker"
    }
),
		          new PepoDataRoles(
    "pathfinder",
    "Identifies routes between places. Master of navigation and territory.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 6f, 10f),
	         new PepoRolePrerequisite("curiosity", 2f, 7f),
	         new PepoRolePrerequisite("autonomy", 4f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
    }, 0.005f, 0.01f,
    new List<string>() {
	         "scout",
	         "land reader",
	         "star reader"
    }
),
		          new PepoDataRoles(
    "team hunter",
    "Leads and executes complex group hunts for large game.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("cohesion", 2f, 7f),
	         new PepoRolePrerequisite("Agreeableness", 5f, 8f),
	         new PepoRolePrerequisite("dominance", 2f, 7f),
	         new PepoRolePrerequisite("Extraversion", 5f, 10f),
    }, 0.03f, 0.05f,
    new List<string>() {
	         "hunt leader",
	         "coordinator"
    }
),
		          new PepoDataRoles(
    "storyteller",
    "Keeps the history, myths, and identity of the band alive.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Extraversion", 6f, 10f),
	         new PepoRolePrerequisite("Openness", 6f, 10f),
	         new PepoRolePrerequisite("tradition", 1f, 7f),
	         new PepoRolePrerequisite("Emotionality", 5f, 10f),
    }, 0.02f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "trapper",
    "Designs, sets, and maintains traps for game.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("Openness", 4f, 10f),
	         new PepoRolePrerequisite("stability", 3f, 7f),
	         new PepoRolePrerequisite("autonomy", 5f, 7f),
    }, 0.02f, 0.05f,
    new List<string>() {
	         "animal tracker"
    }
),
		          new PepoDataRoles(
    "nature communer",
    "Connects with the natural world for spiritual insight.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Emotionality", 7f, 10f),
	         new PepoRolePrerequisite("Openness", 6f, 10f),
	         new PepoRolePrerequisite("curiosity", 4f, 7f),
	         new PepoRolePrerequisite("preservation", 5f, 7f),
    }, 0.01f, 0.02f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "jokester",
    "Uses humor to relieve tension, critique, and entertain.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Extraversion", 5f, 10f),
	         new PepoRolePrerequisite("excitement", 4f, 7f),
	         new PepoRolePrerequisite("cohesion", 3f, 7f),
	         new PepoRolePrerequisite("Emotionality", 4f, 8f),
    }, 0.01f, 0.03f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "dancer",
    "Leads and performs ritual and social dances.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("excitement", 3f, 7f),
	         new PepoRolePrerequisite("Emotionality", 5f, 10f),
	         new PepoRolePrerequisite("cohesion", 4f, 7f),
	         new PepoRolePrerequisite("Extraversion", 4f, 8f),
    }, 0.05f, 0.1f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "dream interpreter",
    "Interprets dreams for spiritual guidance and decision-making.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 7f, 10f),
	         new PepoRolePrerequisite("Emotionality", 6f, 10f),
	         new PepoRolePrerequisite("curiosity", 4f, 7f),
	         new PepoRolePrerequisite("Extraversion", 4f, 8f),
    }, 0.01f, 0.02f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "clay worker",
    "Creates unfired pottery, figurines, and hearths.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 5f, 10f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("accomplishment", 4f, 7f),
	         new PepoRolePrerequisite("curiosity", 4f, 7f),
    }, 0.02f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "plant identifier",
    "Expert on edible, medicinal, and poisonous flora.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 6f, 10f),
	         new PepoRolePrerequisite("curiosity", 3f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 8f),
	         new PepoRolePrerequisite("preservation", 4f, 7f),
    }, 0.05f, 0.1f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "meat preserver",
    "Preserves meat for long-term storage.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	         new PepoRolePrerequisite("preservation", 5f, 7f),
	         new PepoRolePrerequisite("dependability", 3f, 7f),
	         new PepoRolePrerequisite("safety", 4f, 7f),
    }, 0.02f, 0.05f,
    new List<string>() {
	         "food smoker",
	         "food salter"
    }
),
		          new PepoDataRoles(
    "fermenter",
    "Creates fermented foods and beverages.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	         new PepoRolePrerequisite("Openness", 5f, 10f),
	         new PepoRolePrerequisite("curiosity", 3f, 7f),
	         new PepoRolePrerequisite("indulgence", 4f, 7f),
    }, 0.02f, 0.05f,
    new List<string>() {
	         "plant identifier"
    }
),
		          new PepoDataRoles(
    "dye maker",
    "Creates pigments and dyes for decoration.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 5f, 10f),
	         new PepoRolePrerequisite("curiosity", 2f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("preservation", 3f, 7f),
    }, 0.01f, 0.02f,
    new List<string>() {
	         "plant identifier"
    }
),
		          new PepoDataRoles(
    "carer",
    "Tends to the elderly, young, and infirm.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("caring", 3f, 7f),
	         new PepoRolePrerequisite("compassion", 2f, 7f),
	         new PepoRolePrerequisite("Emotionality", 5f, 8f),
	         new PepoRolePrerequisite("dependability", 3f, 7f),
    }, 0.05f, 0.1f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "mushroom identifier",
    "Expert on edible, medicinal, and poisonous fungi.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 6f, 10f),
	         new PepoRolePrerequisite("curiosity", 3f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	         new PepoRolePrerequisite("safety", 3f, 7f),
    }, 0.02f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "shelter builder",
    "Constructs and maintains dwellings and communal structures.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("safety", 4f, 7f),
	         new PepoRolePrerequisite("stability", 3f, 7f),
	         new PepoRolePrerequisite("accomplishment", 3f, 7f),
    }, 0.05f, 0.1f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "bonesetter",
    "Sets fractures and treats joint/muscle injuries.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	         new PepoRolePrerequisite("caring", 2f, 7f),
	         new PepoRolePrerequisite("stability", 3f, 7f),
	         new PepoRolePrerequisite("Emotionality", 3f, 7f),
    }, 0.01f, 0.02f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "teacher",
    "Passes down skills, knowledge, and cultural norms to the young.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 5f, 10f),
	         new PepoRolePrerequisite("caring", 2f, 7f),
	         new PepoRolePrerequisite("dependability", 3f, 7f),
	         new PepoRolePrerequisite("cohesion", 3f, 7f),
    }, 0.1f, 0.15f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "coordinator",
    "Organizes daily tasks and labor for the band.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Extraversion", 5f, 10f),
	         new PepoRolePrerequisite("Agreeableness", 4f, 8f),
	         new PepoRolePrerequisite("dependability", 2f, 7f),
	         new PepoRolePrerequisite("accomplishment", 3f, 7f),
    }, 0.02f, 0.03f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "aimer",
    "Expert with projectile weapons (bow, atlatl).",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("dependability", 2f, 7f),
	         new PepoRolePrerequisite("accomplishment", 2f, 7f),
	         new PepoRolePrerequisite("dominance", 1f, 7f),
    }, 0.05f, 0.1f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "net maker",
    "Weaves nets for fishing, trapping, and carrying.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	         new PepoRolePrerequisite("dependability", 2f, 7f),
	         new PepoRolePrerequisite("excitement", -1f, 4f),
    }, 0.03f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "food smoker",
    "Smokes meat and fish to preserve them.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	         new PepoRolePrerequisite("preservation", 5f, 7f),
	         new PepoRolePrerequisite("excitement", -1f, 4f),
    }, 0.03f, 0.05f,
    new List<string>() {
	         "firekeeper"
    }
),
		          new PepoDataRoles(
    "weaver",
    "Creates textiles, mats, and complex fiber constructions.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	         new PepoRolePrerequisite("Openness", 5f, 8f),
	         new PepoRolePrerequisite("excitement", -1f, 4f),
    }, 0.03f, 0.05f,
    new List<string>() {
	         "cordage maker"
    }
),
		          new PepoDataRoles(
    "basketmaker",
    "Weaves baskets and containers for storage and transport.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("Openness", 4f, 7f),
	         new PepoRolePrerequisite("excitement", -1f, 4f),
    }, 0.05f, 0.1f,
    new List<string>() {
	         "weaver"
    }
),
		          new PepoDataRoles(
    "tanner",
    "Processes animal hides into usable leather.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("preservation", 2f, 7f),
	         new PepoRolePrerequisite("tolerance", 3f, 7f),
    }, 0.05f, 0.1f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "singer",
    "Leads songs for ceremony, ritual, and collective work.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Emotionality", 6f, 10f),
	         new PepoRolePrerequisite("indulgence", 1f, 7f),
	         new PepoRolePrerequisite("Extraversion", 4f, 10f),
    }, 0.02f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "thrower",
    "Expert in the use of thrown weapons (spears, rocks).",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("vitality", 3f, 7f),
	         new PepoRolePrerequisite("accomplishment", 3f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
    }, 0.05f, 0.1f,
    new List<string>() {
	         "aimer"
    }
),
		          new PepoDataRoles(
    "land reader",
    "Interprets terrain, geology, and resources.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Openness", 5f, 10f),
	         new PepoRolePrerequisite("curiosity", 2f, 7f),
	         new PepoRolePrerequisite("preservation", 4f, 7f),
    }, 0.02f, 0.05f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "stone knapper",
    "The foundational skill of shaping stone into tools.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	         new PepoRolePrerequisite("Openness", 4f, 7f),
	         new PepoRolePrerequisite("accomplishment", 4f, 7f),
    }, 0.1f, 0.15f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "woodcarver",
    "Crafts items from wood (bowls, handles, tools).",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("Openness", 5f, 10f),
	         new PepoRolePrerequisite("accomplishment", 4f, 7f),
    }, 0.03f, 0.05f,
    new List<string>() {
	         "stone knapper"
    }
),
		          new PepoDataRoles(
    "soap maker",
    "Makes cleansing agents from fats and ashes.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("preservation", 3f, 7f),
	         new PepoRolePrerequisite("safety", 4f, 7f),
    }, 0.01f, 0.02f,
    new List<string>() {
	         "firekeeper",
	         "clay worker",
    }
),
		          new PepoDataRoles(
    "adhesive maker",
    "Produces glues and tar from natural materials.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	         new PepoRolePrerequisite("Openness", 5f, 8f),
	         new PepoRolePrerequisite("curiosity", 5f, 7f),
    }, 0.01f, 0.02f,
    new List<string>() {
	         "plant identifier",
	         "shelter builder",
	         "toolmaker"
    }
),
		          new PepoDataRoles(
    "cordage maker",
    "Creates string, rope, and cord from plant fibers and sinew.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("humility", 5f, 7f),
	         new PepoRolePrerequisite("dependability", 3f, 7f),
    }, 0.05f, 0.1f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "food salter",
    "Uses salt (where available) to preserve food.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	         new PepoRolePrerequisite("preservation", 5f, 7f),
	         new PepoRolePrerequisite("dependability", 3f, 7f),
    }, 0.01f, 0.02f,
    new List<string>() {
	         "scout"
    }
),
		          new PepoDataRoles(
    "water gatherer",
    "Ensures a clean and reliable water supply.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("dependability", 3f, 7f),
	         new PepoRolePrerequisite("preservation", 4f, 7f),
	         new PepoRolePrerequisite("Conscientiousness", 4f, 8f),
    }, 0.05f, 0.1f,
    new List<string>() // No prerequisites
),
		          new PepoDataRoles(
    "garment maker",
    "Crafts clothing and footwear from hides and fibers.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	         new PepoRolePrerequisite("Openness", 4f, 7f),
	         new PepoRolePrerequisite("dependability", 3f, 7f),
    }, 0.05f, 0.1f,
    new List<string>() {
	         "tanner",
	         "cordage maker"
    }
),
		          new PepoDataRoles(
    "toolmaker",
    "Crafts and repairs essential stone and bone tools.",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
	         new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	         new PepoRolePrerequisite("accomplishment", 3f, 7f),
	         new PepoRolePrerequisite("Openness", 4f, 8f),
    }, 0.05f, 0.1f,
    new List<string>() {
	         "stone knapper"
    }
),
		          new PepoDataRoles(
    "apprentice",
    "learning from someone",
    "Hunter Gatherer Era",
    new List<PepoRolePrerequisite>()
    {
    }, 0.1f, 0.3f,
    new List<string>() // No prerequisites
) };




	     //     public static List<PepoDataRoles> HunterGatherer = new List<PepoDataRoles>()
	     //   {
	     //       // BASIC SURVIVAL ROLES (no prerequisites - foundational)
	     //       new PepoDataRoles(
	     //           "Trapper",
	     //           "trapping and hunting small animals.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("agency", 3f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 4f, 10f),
	     // new PepoRolePrerequisite("Emotionality", 0f, 7f)
	     //           }, 0.02f, 0.05f,
	     //           new List<string>(){ } // No prerequisites - basic skill
	     //           ),

	     //       new PepoDataRoles(
	     //           "Bone setter",
	     //           "setting broken bones and treating injuries.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("benevolence", 4f, 7f),
	     // new PepoRolePrerequisite("security", 4f, 7f),
	     // new PepoRolePrerequisite("Emotionality", 2f, 6f),
	     // new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	     // new PepoRolePrerequisite("Agreeableness", 3f, 7f),
	     // new PepoRolePrerequisite("Openness", 4f, 10f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() // Basic healing knowledge
	     //           ),

	     //       // SOCIAL ROLES (require basic community structure)
	     //       new PepoDataRoles(
	     //           "Gift exchanger",
	     //           "exchanging gifts and maintaining social bonds between groups.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("benevolence", 3f, 7f),
	     // new PepoRolePrerequisite("power", 3f, 7f),
	     // new PepoRolePrerequisite("Extraversion", 5f, 10f),
	     // new PepoRolePrerequisite("Agreeableness", 5f, 10f),
	     // new PepoRolePrerequisite("Honesty", 4f, 10f)
	     //           }, 0.01f, 0.02f,
	     //           new List<string>(){"Scout"}
	     //           ),

	     //       // PHYSICAL LABOR ROLES (no specific prerequisites)
	     //       new PepoDataRoles(
	     //           "Load bearer",
	     //           "carrying heavy loads during migration and transport.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("benevolence", 0f, 7f),
	     // new PepoRolePrerequisite("conformity", 0f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 4f, 10f),
	     // new PepoRolePrerequisite("Openness", 1f, 8f)
	     //           }, 0.05f, 0.1f,
	     //           new List<string>(){ "Scout", "Runner" }
	     //           ),

	     //       // YOUTH/MESSENGER ROLES
	     //       new PepoDataRoles(
	     //           "runner",
	     //           "messengers communicating between groups and running errands.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("stimulation", 4f, 7f),
	     // new PepoRolePrerequisite("Extraversion", 3f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 3f, 9f)
	     //           }, 0.05f, 0.1f,
	     //           new List<string>() // No prerequisites
	     //           ),

	     //       // CONSTRUCTION ROLE
	     //       new PepoDataRoles(
	     //           "Shelter builder",
	     //           "constructing temporary shelters and dwellings.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("achievement", 3f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	     // new PepoRolePrerequisite("Openness", 3f, 9f),
	     // new PepoRolePrerequisite("Extraversion", 1f, 8f)
	     //           }, 0.02f, 0.05f,
	     //           new List<string>() // No prerequisites - basic skill
	     //           ),

	     //       // FIRE MANAGEMENT
	     //       new PepoDataRoles(
	     //           "Fire tender",
	     //           "maintaining and controlling the group's fire.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("security", 4f, 7f),
	     // new PepoRolePrerequisite("tradition", 3f, 7f),
	     // new PepoRolePrerequisite("stimulation", -1f, 5f),
	     // new PepoRolePrerequisite("Openness", 1f, 7f),
	     // new PepoRolePrerequisite("Honesty", 4f, 10f)
	     //           }, 0.01f, 0.02f,
	     //           new List<string>() // Requires fire-making knowledge
	     //           ),

	     //       // FORAGING SPECIALIZATIONS (require basic foraging knowledge)
	     //       new PepoDataRoles(
	     //           "Marine forager",
	     //           "gathering food from marine environments.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("stimulation", 3f, 7f),
	     // new PepoRolePrerequisite("Openness", 3f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 3f, 9f)
	     //           }, 0.05f, 0.1f,
	     //           new List<string>() { "Food forager" } // Requires basic foraging skills
	     //           ),

	     //       // FOUNDATIONAL FORAGING (no prerequisites - base skill)
	     //       new PepoDataRoles(
	     //           "Food forager",
	     //           "gathering plant foods and edible vegetation.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("benevolence", 1f, 7f),
	     // new PepoRolePrerequisite("security", 1f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 2f, 10f)
	     //           }, 0.15f, 0.3f,
	     //           new List<string>() // Foundational skill - no prerequisites
	     //           ),

	     //       // OBSERVATION/PREDICTION ROLES
	     //       new PepoDataRoles(
	     //           "Weather interpreter",
	     //           "predicting weather patterns and seasonal changes.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("security", 4f, 7f),
	     // new PepoRolePrerequisite("universalism", 4f, 7f),
	     // new PepoRolePrerequisite("Openness", 6f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	     // new PepoRolePrerequisite("Extraversion", 1f, 7f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() { } // Requires observation of nature
	     //           ),

	     //       // SOCIAL CHALLENGE ROLES
	     //       new PepoDataRoles(
	     //           "Challenger",
	     //           "questioning norms and proposing new ways of doing things.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("agency", 5f, 7f),
	     // new PepoRolePrerequisite("stimulation", 4f, 7f),
	     // new PepoRolePrerequisite("conformity", -1f, 3f),
	     // new PepoRolePrerequisite("Openness", 6f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 2f, 6f),
	     //	  new PepoRolePrerequisite("Agreeableness", 0f, 8f)
	     //           }, 0.005f, 0.02f,
	     //           new List<string>() {  }
	     //           ),

	     //       // EXPLORATION ROLES
	     //       new PepoDataRoles(
	     //           "Scout",
	     //           "exploring new territories and assessing resources.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     //new PepoRolePrerequisite("stimulation", 1f, 7f),
	     //new PepoRolePrerequisite("conformity", -1f, 5f),
	     //new PepoRolePrerequisite("agency", 1f, 7f),
	     //new PepoRolePrerequisite("security", -1f, 5f),
	     // new PepoRolePrerequisite("Openness", 5f, 10f),
	     //new PepoRolePrerequisite("Extraversion", 0f, 6f)
	     //           }, 0.02f, 0.05f,
	     //           new List<string>() {  } //foundational
	     //           ),

	     //       // ASTRONOMICAL KNOWLEDGE
	     //       new PepoDataRoles(
	     //           "Star reader",
	     //           "reading stars for navigation and seasonal timing.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("tradition", 4f, 7f),
	     // new PepoRolePrerequisite("Openness", 7f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 6f, 10f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() {  } // Requires observation skills
	     //           ),

	     //       // BONE WORKING (requires hunting)
	     //       new PepoDataRoles(
	     //           "Bone carver",
	     //           "carving bones into tools and decorative items.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("achievement", 4f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	     // new PepoRolePrerequisite("Openness", 5f, 10f),
	     // new PepoRolePrerequisite("Extraversion", 1f, 7f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() { "Trapper", "Small game hunter" } // Requires access to bones from hunting
	     //           ),

	     //       // FOOD PROCESSING SPECIALIZATION
	     //       new PepoDataRoles(
	     //           "Fermenter",
	     //           "fermenting foods and beverages for preservation and consumption.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("tradition", 3f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	     // new PepoRolePrerequisite("Openness", 4f, 9f)
	     //           }, 0.01f, 0.02f,
	     //           new List<string>() { "Gatherer-Processor", "Unfired Clay Worker" } // Requires food processing knowledge
	     //           ),

	     //       // CLAY WORKING PROGRESSION
	     //       new PepoDataRoles(
	     //           "Unfired Clay Worker",
	     //           "working with clay for simple unfired objects and materials.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("achievement", 3f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	     // new PepoRolePrerequisite("Openness", 4f, 9f)
	     //           }, 0.02f, 0.05f,
	     //           new List<string>() { "Clay collector" } // Requires clay source knowledge
	     //           ),

	     //       // PIGMENT WORKING PROGRESSION
	     //       new PepoDataRoles(
	     //           "Pigment mixer",
	     //           "mixing natural pigments for painting and decoration.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("hedonism", 3f, 7f),
	     // new PepoRolePrerequisite("Openness", 6f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 5f, 10f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() { "Clay collector" } // Requires material sourcing knowledge
	     //           ),

	     //       // MEDICINAL SPECIALIZATION
	     //       new PepoDataRoles(
	     //           "Medicinal forager",
	     //           "identifying and collecting medicinal plants.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("agency", 4f, 7f),
	     // new PepoRolePrerequisite("universalism", 3f, 7f),
	     // new PepoRolePrerequisite("Openness", 5f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	     // new PepoRolePrerequisite("Agreeableness", 4f, 10f)
	     //           }, 0.01f, 0.02f,
	     //           new List<string>() { "Food forager" } // Requires plant identification
	     //           ),

	     //       // FIBER/TEXTILE SPECIALIZATION
	     //       new PepoDataRoles(
	     //           "Fiber/Textile Forager",
	     //           "gathering plants suitable for fibers and textiles.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("tradition", 3f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	     // new PepoRolePrerequisite("Openness", 4f, 9f)
	     //           }, 0.02f, 0.05f,
	     //           new List<string>() { "Food forager" } // Requires plant knowledge
	     //           ),

	     //       // MYCOLOGY SPECIALIZATION
	     //       new PepoDataRoles(
	     //           "Mushroom identifier",
	     //           "identifying edible and medicinal mushrooms.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("security", 4f, 7f),
	     // new PepoRolePrerequisite("Openness", 6f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 6f, 10f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() { "Food forager" } // Requires foraging knowledge
	     //           ),

	     //       // FOOD PROCESSING FOUNDATION
	     //       new PepoDataRoles(
	     //           "Gatherer-Processor",
	     //           "processing gathered foods for storage.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("security", 3f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 4f, 10f),
	     // new PepoRolePrerequisite("Openness", 2f, 8f)
	     //           }, 0.2f, 0.4f,
	     //           new List<string>() { "Food forager" } // Requires foraging first
	     //           ),

	     //       // MINERAL SPECIALIZATION
	     //       new PepoDataRoles(
	     //           "Salt gatherer",
	     //           "gathering and processing salt from natural sources.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("stimulation", 3f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	     // new PepoRolePrerequisite("Openness", 4f, 9f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() { "Food forager", "Scout" } // Requires foraging AND exploration
	     //           ),

	     //       // DANGEROUS FORAGING
	     //       new PepoDataRoles(
	     //           "Honey gatherer",
	     //           "gathering honey from wild bee nests.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("stimulation", 4f, 7f),
	     // new PepoRolePrerequisite("Emotionality", 0f, 5f),
	     // new PepoRolePrerequisite("Openness", 5f, 10f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() { "Food forager" } // Requires foraging knowledge
	     //           ),

	     //       // EMOTIONAL SUPPORT ROLE
	     //       new PepoDataRoles(
	     //           "Grief Tender",
	     //           "supporting community members through loss and grief.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("benevolence", 5f, 7f),
	     // new PepoRolePrerequisite("Agreeableness", 6f, 10f),
	     // new PepoRolePrerequisite("Emotionality", 4f, 8f),
	     // new PepoRolePrerequisite("Extraversion", 4f, 10f)
	     //           }, 0.02f, 0.04f,
	     //           new List<string>() {  } // Requires healing knowledge
	     //           ),

	     //       // CONFLICT RESOLUTION
	     //       new PepoDataRoles(
	     //           "Conflict De-escalator",
	     //           "mediating disputes and preventing violence.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("security", 4f, 7f),
	     // new PepoRolePrerequisite("benevolence", 4f, 7f),
	     // new PepoRolePrerequisite("Agreeableness", 6f, 10f),
	     // new PepoRolePrerequisite("Emotionality", 2f, 6f),
	     // new PepoRolePrerequisite("Honesty", 5f, 10f)
	     //           }, 0.03f, 0.08f,
	     //           new List<string>() { "Challenger" } // Requires someone to de escalate
	     //           ),

	     //       // ORAL TRADITION
	     //       new PepoDataRoles(
	     //           "Hearth Story-Weaver",
	     //           "telling stories and preserving oral history.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("tradition", 4f, 7f),
	     // new PepoRolePrerequisite("power", 3f, 7f),
	     // new PepoRolePrerequisite("Extraversion", 6f, 10f),
	     // new PepoRolePrerequisite("Openness", 6f, 10f),
	     // new PepoRolePrerequisite("Agreeableness", 4f, 10f)
	     //           }, 0.01f, 0.03f,
	     //           new List<string>() { "Fire tender" } // Requires centrality at hearth
	     //           ),

	     //       // SPIRITUAL INTERPRETATION
	     //       new PepoDataRoles(
	     //           "Dream Interpreter",
	     //           "interpreting dreams and visions for the group.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("tradition", 4f, 7f),
	     // new PepoRolePrerequisite("Openness", 6f, 10f),
	     // new PepoRolePrerequisite("Extraversion", 4f, 10f),
	     // new PepoRolePrerequisite("Agreeableness", 4f, 10f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() { "Medicinal forager", "Hearth Story-Weaver" } // Requires medicinal AND storytelling
	     //           ),

	     //       // ADVANCED HEALING
	     //       new PepoDataRoles(
	     //           "Nurse-Midwife",
	     //           "assisting with childbirth and caring for the ill.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("benevolence", 5f, 7f),
	     // new PepoRolePrerequisite("Agreeableness", 6f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	     // new PepoRolePrerequisite("Emotionality", 2f, 7f)
	     //           }, 0.01f, 0.03f,
	     //           new List<string>() { "Bone setter", "Medicinal forager" } // Requires both healing skills
	     //           ),

	     //       // ELDER WISDOM
	     //       new PepoDataRoles(
	     //           "Elder Councillor",
	     //           "providing wisdom and guidance based on long experience.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("tradition", 5f, 7f),
	     // new PepoRolePrerequisite("benevolence", 4f, 7f),
	     // new PepoRolePrerequisite("Agreeableness", 5f, 10f),
	     // new PepoRolePrerequisite("Openness", 5f, 10f),
	     // new PepoRolePrerequisite("Honesty", 5f, 10f)
	     //           }, 0.01f, 0.03f,
	     //           new List<string>() { "Hearth Story-Weaver", "Weather interpreter" } // Requires knowledge AND wisdom roles
	     //           ),

	     //       // MUSICAL TRADITION
	     //       new PepoDataRoles(
	     //           "Song-Keeper",
	     //           "preserving and teaching songs and chants.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("tradition", 5f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	     // new PepoRolePrerequisite("Openness", 4f, 10f),
	     // new PepoRolePrerequisite("Extraversion", 4f, 10f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() { "Hearth Story-Weaver" } // Requires oral tradition
	     //           ),

	     //       // DYE MAKING PROGRESSION
	     //       new PepoDataRoles(
	     //           "Dye Maker",
	     //           "creating dyes from natural materials for coloring fibers.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("tradition", 3f, 7f),
	     // new PepoRolePrerequisite("Openness", 6f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 6f, 10f)
	     //           }, 0.001f, 0.005f,
	     //           new List<string>() { "Pigment mixer", "Fiber/Textile Forager" } // Requires pigments AND fibers
	     //           ),

	     //       // CAVE ART PROGRESSION
	     //       new PepoDataRoles(
	     //           "Cave painter",
	     //           "creating paintings and artwork on cave walls.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("tradition", 4f, 7f),
	     // new PepoRolePrerequisite("Openness", 7f, 10f),
	     // new PepoRolePrerequisite("Extraversion", 5f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 6f, 10f)
	     //           }, 0.001f, 0.005f,
	     //           new List<string>() { "Pigment mixer", "Bone carver" } // Requires pigments AND artistic skill
	     //           ),

	     //       // RITUAL SPECIALIZATION
	     //       new PepoDataRoles(
	     //           "Ritual Cleanser",
	     //           "performing cleansing rituals for people and spaces.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("tradition", 4f, 7f),
	     // new PepoRolePrerequisite("security", 3f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	     // new PepoRolePrerequisite("Extraversion", 4f, 9f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() { "Medicinal forager", "Fire tender" } // Requires healing AND ritual knowledge
	     //           ),

	     //       // CLAY SOURCING
	     //       new PepoDataRoles(
	     //           "Clay collector",
	     //           "locating and collecting suitable clay deposits.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("benevolence", 2f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	     // new PepoRolePrerequisite("Openness", 4f, 9f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() { "Scout" } // Requires exploration skills
	     //           ),

	     //       // WATER SOURCING
	     //       new PepoDataRoles(
	     //           "Water finder",
	     //           "locating water sources and ensuring clean water supply.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("security", 4f, 7f),
	     // new PepoRolePrerequisite("Openness", 6f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 5f, 10f)
	     //           }, 0.01f, 0.02f,
	     //           new List<string>() { "Scout", "Weather interpreter" } // Requires terrain AND weather knowledge
	     //           ),

	     //       // TERRITORIAL DEFENSE
	     //       new PepoDataRoles(
	     //           "Boundary Scout",
	     //           "monitoring territorial boundaries and approaches.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("security", 4f, 7f),
	     // new PepoRolePrerequisite("stimulation", 3f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	     // new PepoRolePrerequisite("Extraversion", 5f, 9f)
	     //           }, 0.01f, 0.02f,
	     //           new List<string>() { "Scout" } // Requires scouting skills
	     //           ),

	     //       // ADVANCED NAVIGATION
	     //       new PepoDataRoles(
	     //           "Pathfinder",
	     //           "finding and marking safe travel routes.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("stimulation", 4f, 7f),
	     // new PepoRolePrerequisite("conformity", 3f, 7f),
	     // new PepoRolePrerequisite("Openness", 7f, 10f),
	     // new PepoRolePrerequisite("Extraversion", 6f, 10f)
	     //           }, 0.01f, 0.02f,
	     //           new List<string>() { "Scout", "Star reader" } // Requires exploration AND navigation
	     //           ),

	     //       // HUNTING SPECIALIZATION
	     //       new PepoDataRoles(
	     //           "Animal tracker",
	     //           "tracking animals for hunting and observation.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("achievement", 3f, 7f),
	     // new PepoRolePrerequisite("stimulation", 3f, 7f),
	     // new PepoRolePrerequisite("Openness", 7f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 6f, 10f)
	     //           }, 0.01f, 0.02f,
	     //           new List<string>() { "Trapper", "Scout" } // Requires hunting AND tracking
	     //           ),

	     //       // EMERGENCY MANAGEMENT
	     //       new PepoDataRoles(
	     //           "Crisis Manager",
	     //           "managing group response during emergencies.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("security", 5f, 7f),
	     // new PepoRolePrerequisite("benevolence", 4f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	     // new PepoRolePrerequisite("Extraversion", 6f, 10f)
	     //           }, 0.01f, 0.03f,
	     //           new List<string>() { "Shelter builder", "Conflict De-escalator" } // Requires construction AND conflict skills
	     //           ),

	     //       // CAMP ORGANIZATION
	     //       new PepoDataRoles(
	     //           "Camp Arranger",
	     //           "organizing camp layout and arrangements.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("security", 3f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	     // new PepoRolePrerequisite("Openness", 4f, 8f)
	     //           }, 0.01f, 0.02f,
	     //           new List<string>() { "Shelter builder", "Scout" } // Requires building AND terrain knowledge
	     //           ),

	     //       // LARGE GAME HUNTING
	     //       new PepoDataRoles(
	     //           "Big game hunter",
	     //           "hunting large and dangerous animals.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("achievement", 4f, 7f),
	     // new PepoRolePrerequisite("stimulation", 4f, 7f),
	     // new PepoRolePrerequisite("Emotionality", 0f, 6f),
	     // new PepoRolePrerequisite("Conscientiousness", 4f, 10f),
	     // new PepoRolePrerequisite("Extraversion", 3f, 9f)
	     //           }, 0.02f, 0.05f,
	     //           new List<string>() { "Flint Knapper", "Cordage Maker" } // Requires hunting progression
	     //           ),

	     //       // CULTURAL CURATION
	     //       new PepoDataRoles(
	     //           "Memory-Object Curator",
	     //           "preserving and caring for objects of cultural significance.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("tradition", 5f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	     // new PepoRolePrerequisite("Openness", 6f, 10f)
	     //           }, 0.001f, 0.005f,
	     //           new List<string>() { "Bone carver", "Hearth Story-Weaver" } // Requires craft AND tradition
	     //           ),

	     //       // SOCIAL PLAY ORGANIZATION
	     //       new PepoDataRoles(
	     //           "Game Leader",
	     //           "organizing games and playful activities for the group.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("stimulation", 5f, 7f),
	     // new PepoRolePrerequisite("Extraversion", 6f, 10f),
	     // new PepoRolePrerequisite("Openness", 5f, 9f)
	     //           }, 0.05f, 0.15f,
	     //           new List<string>() { "Runner", "Gift exchanger" } // Requires youth energy AND social skills
	     //           ),

	     //       // COMMUNICATION SPECIALIZATION
	     //       new PepoDataRoles(
	     //           "Message Interpreter",
	     //           "interpreting messages from other groups or signs.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("universalism", 3f, 7f),
	     // new PepoRolePrerequisite("Openness", 6f, 10f),
	     // new PepoRolePrerequisite("Agreeableness", 5f, 10f),
	     // new PepoRolePrerequisite("Extraversion", 5f, 10f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() { "Gift exchanger", "Runner" } // Requires diplomacy AND communication
	     //           ),

	     //       // INTERMEDIATE HUNTING
	     //       new PepoDataRoles(
	     //           "Small game hunter",
	     //           "hunting small animals and birds.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("achievement", 3f, 7f),
	     // new PepoRolePrerequisite("security", 3f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	     // new PepoRolePrerequisite("Openness", 4f, 9f)
	     //           }, 0.05f, 0.1f,
	     //           new List<string>() { "Trapper" } // Requires basic trapping skills
	     //           ),

	     //       // ADVANCED ENVIRONMENTAL READING
	     //       new PepoDataRoles(
	     //           "Land-Reader",
	     //           "reading landscapes for resources and changes.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("tradition", 4f, 7f),
	     // new PepoRolePrerequisite("Openness", 7f, 10f),
	     // new PepoRolePrerequisite("Conscientiousness", 6f, 10f)
	     //           }, 0.01f, 0.02f,
	     //           new List<string>() { "Weather interpreter", "Animal tracker" } // Requires weather AND tracking knowledge
	     //           ),

	     //       new PepoDataRoles(
	     //           "Flint Knapper",
	     //           "shaping flint and stone into tools and weapons.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("security", 3f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	     // new PepoRolePrerequisite("Emotionality", 0f, 8f)
	     //           }, 0.01f, 0.02f,
	     //           new List<string>() // Foundational skill
	     //           ),

	     //       // FIBER PROCESSING
	     //       new PepoDataRoles(
	     //           "Cordage Maker",
	     //           "creating ropes and cords from natural fibers.",
	     //           "Hunter-Gatherer Era",
	     //           new List<PepoRolePrerequisite>()
	     //           {
	     // new PepoRolePrerequisite("conformity", 2f, 7f),
	     // new PepoRolePrerequisite("Conscientiousness", 5f, 10f),
	     // new PepoRolePrerequisite("Openness", 2f, 8f)
	     //           }, 0.005f, 0.01f,
	     //           new List<string>() { "Fiber/Textile Forager" } // Requires fiber knowledge FIRST
	     //           )
	     //   };
	     public static List<PepoDataRoles> EarlyAgrarian = new List<PepoDataRoles>()
    {
        // BASIC CRAFTS (foundational agrarian skills - no prerequisites)
        new PepoDataRoles(
            "Carpenter",
            "working wood to create tools, structures, and furniture.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("achievement", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Openness", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 4f, 8f)
            }, 0.01f, 0.02f,
            new List<string>( ){ "Flint Knapper" } // Foundational agrarian skill
            ),


        // CHILDCARE (becomes more formalized)
        new PepoDataRoles(
            "Childminder",
            "caring for children while parents work.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("benevolence", 4f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Agreeableness", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 5f, 9f),
	 new PepoRolePrerequisite("Conscientiousness", 5f, 10f)
            }, 0.05f, 0.1f,
            new List<string>() // Formalization of informal childcare
            ),

        // CONFLICT RESOLUTION (more formal)
        new PepoDataRoles(
            "Dispute Mediator",
            "formally mediating conflicts and disputes.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("benevolence", 4f, 7f),
	 new PepoRolePrerequisite("universalism", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Agreeableness", 6f, 10f),
	 new PepoRolePrerequisite("Honesty", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 5f, 9f)
            }, 0.005f, 0.01f,
            new List<string>() { "Conflict De-escalator" } // Formalization of conflict resolution
            ),

        // CONSTRUCTION SPECIALIZATIONS
        new PepoDataRoles(
            "Plasterer",
            "applying plaster to walls and structures.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Openness", 2f, 6f),
	 new PepoRolePrerequisite("Honesty", 5f, 8f)
            }, 0.005f, 0.01f,
            new List<string>() { "Shelter builder",  "Clay collector" } // Requires construction knowledge
            ),

        // COMMUNICATION
        new PepoDataRoles(
            "Courier",
            "carrying messages between settlements.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Extraversion", 5f, 9f),
	 new PepoRolePrerequisite("Honesty", 6f, 9f)
            }, 0.005f, 0.01f,
            new List<string>() { "Adolescent 'Runner'" } // Formalization of running messages
            ),

        // CRAFT SPECIALIZATIONS
        new PepoDataRoles(
            "Basketmaker",
            "weaving baskets and containers from natural materials.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f),
	 new PepoRolePrerequisite("Agreeableness", 4f, 7f)
            }, 0.01f, 0.02f,
            new List<string>() { "Cordage Maker" } // Requires fiber working knowledge
            ),

        // TERRITORIAL MARKING
        new PepoDataRoles(
            "Boundary Marker",
            "marking and maintaining territorial boundaries.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 4f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Honesty", 7f, 10f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.001f, 0.005f,
            new List<string>() { "Boundary Scout" } // Formalization of boundary monitoring
            ),

        // FIRE CONTINUITY (carried over from HG)
        new PepoDataRoles(
            "Fire Tender", // Same name, but in new era
            "maintaining community fires and communal hearths.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 4f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f),
	 new PepoRolePrerequisite("Honesty", 5f, 8f)
            }, 0.005f, 0.01f,
            new List<string>() { "Fire tender" } // Continuation from HG era
            ),

        // CONSTRUCTION SPECIALIZATION
        new PepoDataRoles(
            "Thatcher",
            "creating thatched roofs for buildings.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f),
	 new PepoRolePrerequisite("Agreeableness", 4f, 7f)
            }, 0.005f, 0.01f,
            new List<string>() { "Shelter builder" } // Requires construction knowledge
            ),

        // INFRASTRUCTURE MAINTENANCE
        new PepoDataRoles(
            "Pathkeeper",
            "maintaining paths and trails between settlements.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Extraversion", 2f, 5f),
	 new PepoRolePrerequisite("Openness", 2f, 6f)
            }, 0.005f, 0.01f,
            new List<string>() { "Pathfinder" } // Requires knowledge of paths
            ),

        // AGRICULTURAL INFRASTRUCTURE
        new PepoDataRoles(
            "Drainage Digger",
            "creating and maintaining drainage systems for fields.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Openness", 2f, 6f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.01f, 0.02f,
            new List<string>() { "Well Digger" } // Requires digging/earthworking skills
            ),

        // FISHING SPECIALIZATION
        new PepoDataRoles(
            "Fisher",
            "fishing in rivers, lakes, and coastal waters.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("stimulation", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 5f, 8f),
	 new PepoRolePrerequisite("Openness", 4f, 8f),
	 new PepoRolePrerequisite("Emotionality", 3f, 7f)
            }, 0.02f, 0.05f,
            new List<string>() { "Marine forager" } // Requires knowledge of aquatic environments
            ),

        // FOUNDATIONAL AGRICULTURE
        new PepoDataRoles(
            "Arable Farmer",
            "growing crops in fields for food.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 4f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("benevolence", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Agreeableness", 4f, 7f),
	 new PepoRolePrerequisite("Openness", 2f, 6f)
            }, 0.3f, 0.4f,
            new List<string>() { "Food forager", "Gatherer-Processor" } // Requires plant knowledge AND processing
            ),

        // ANIMAL HUSBANDRY
        new PepoDataRoles(
            "Pastoral Farmer",
            "raising and herding livestock.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Extraversion", 4f, 8f),
	 new PepoRolePrerequisite("Agreeableness", 4f, 7f)
            }, 0.1f, 0.2f,
            new List<string>() { "Trapper", "Animal tracker" } // Requires animal knowledge
            ),

        // TRADE SPECIALIZATION
        new PepoDataRoles(
            "Trader",
            "trading goods between settlements and groups.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("agency", 3f, 7f),
	 new PepoRolePrerequisite("stimulation", 3f, 7f),
	 new PepoRolePrerequisite("achievement", 3f, 7f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f),
	 new PepoRolePrerequisite("Openness", 6f, 10f),
	 new PepoRolePrerequisite("Honesty", 3f, 7f)
            }, 0.02f, 0.05f,
            new List<string>() { "Gift exchanger" } // Formalization of exchange
            ),

        // FUNERARY PRACTICES
        new PepoDataRoles(
            "Funeral Worker",
            "preparing bodies and conducting funeral rites.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 4f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("benevolence", 3f, 7f),
	 new PepoRolePrerequisite("Extraversion", 5f, 9f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.005f, 0.01f,
            new List<string>() { "Grief Tender" } // Formalization of grief work
            ),

        // RITUAL ORGANIZATION
        new PepoDataRoles(
            "Ritual Coordinator",
            "organizing and coordinating community rituals.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 4f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("power", 3f, 7f),
	 new PepoRolePrerequisite("Openness", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f)
            }, 0.005f, 0.01f,
            new List<string>() { "Dream Interpreter", "Hearth Story-Weaver" } // Requires spiritual AND storytelling
            ),

        // SPIRITUAL LEADERSHIP
        new PepoDataRoles(
            "Oracle",
            "providing spiritual guidance and divination.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 4f, 7f),
	 new PepoRolePrerequisite("power", 3f, 7f),
	 new PepoRolePrerequisite("stimulation", 3f, 7f),
	 new PepoRolePrerequisite("Openness", 7f, 10f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f)
            }, 0.001f, 0.005f,
            new List<string>() { "Dream Interpreter", "Ritual coordinator" } // Requires dream interpretation AND ritual knowledge
            ),

        // ARBORICULTURE
        new PepoDataRoles(
            "Orchard Tender",
            "caring for fruit trees and orchards.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("universalism", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 5f, 9f),
	 new PepoRolePrerequisite("Openness", 4f, 8f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.01f, 0.03f,
            new List<string>() { "Food forager", "Weather interpreter" } // Requires plant AND seasonal knowledge
            ),

        // ARTISTIC EXPRESSION
        new PepoDataRoles(
            "Figure Maker",
            "creating figurines and symbolic objects.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("agency", 3f, 7f),
	 new PepoRolePrerequisite("achievement", 3f, 7f),
	 new PepoRolePrerequisite("Openness", 6f, 10f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 4f, 8f)
            }, 0.005f, 0.01f,
            new List<string>() { "Bone carver", "Unfired Clay Worker" } // Requires carving OR clay work
            ),

        // KNOWLEDGE PRESERVATION
        new PepoDataRoles(
            "Memory Keeper",
            "preserving community history and knowledge.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 4f, 7f),
	 new PepoRolePrerequisite("universalism", 3f, 7f),
	 new PepoRolePrerequisite("benevolence", 3f, 7f),
	 new PepoRolePrerequisite("Openness", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f)
            }, 0.005f, 0.01f,
            new List<string>() { "Hearth Story-Weaver", "Song-Keeper" } // Requires oral tradition
            ),

        // AGRICULTURAL SPECIALIZATION
        new PepoDataRoles(
            "Seed Selector",
            "selecting and saving the best seeds for planting.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("benevolence", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Openness", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.01f, 0.02f,
            new List<string>() { "Arable Farmer", "Memory Keeper" } // Requires farming AND knowledge preservation
            ),

        // GRAIN PROCESSING
        new PepoDataRoles(
            "Thresher",
            "separating grain from husks and stalks.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Honesty", 5f, 8f),
	 new PepoRolePrerequisite("Emotionality", 2f, 6f)
            }, 0.03f, 0.05f,
            new List<string>() { "Arable Farmer" } // Requires farming knowledge
            ),

        // WATER INFRASTRUCTURE
        new PepoDataRoles(
            "Well Digger",
            "digging and maintaining wells for water access.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 4f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Openness", 4f, 7f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.02f, 0.03f,
            new List<string>() { "Water finder", "Arable Farmer" } // Requires water finding AND settlement needs
            ),

        // HYGIENE SPECIALIZATION
        new PepoDataRoles(
            "Soap Maker",
            "making soap from ashes and fats.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Openness", 4f, 7f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.002f, 0.005f,
            new List<string>() { "Ash Collector", "Dairy Processer" } // Requires ash AND fat processing knowledge
            ),

        // LAND MANAGEMENT
        new PepoDataRoles(
            "Arable Commons Guide",
            "managing shared agricultural lands.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Agreeableness", 4f, 7f),
	 new PepoRolePrerequisite("Openness", 2f, 5f)
            }, 0.005f, 0.01f,
            new List<string>() { "Boundary Marker", "Arable Farmer" } // Requires boundaries AND farming
            ),

        // PASTURE MANAGEMENT
        new PepoDataRoles(
            "Pastoral Commons Guide",
            "managing shared grazing lands.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Extraversion", 5f, 8f),
	 new PepoRolePrerequisite("Openness", 2f, 5f)
            }, 0.005f, 0.01f,
            new List<string>() { "Boundary Marker", "Pastoral Farmer" } // Requires boundaries AND herding
            ),

        // CONSTRUCTION SPECIALIZATION
        new PepoDataRoles(
            "Wall Builder",
            "constructing walls for defense and boundaries.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 4f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Extraversion", 4f, 8f),
	 new PepoRolePrerequisite("Agreeableness", 5f, 8f)
            }, 0.02f, 0.05f,
            new List<string>() { "Brickmaker" } // Requires brick making knowledge
            ),

        // TRANSPORTATION INFRASTRUCTURE
        new PepoDataRoles(
            "Ferry-Keeper",
            "operating ferries across rivers and waterways.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("benevolence", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Agreeableness", 5f, 8f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.002f, 0.005f,
            new List<string>() { "Carpenter", "Pathkeeper", "Fisher" } // Requires boat building, path knowledge, AND water skills
            ),

        // CERAMICS TECHNOLOGY
        new PepoDataRoles(
            "Potter",
            "creating fired clay pots and vessels.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("achievement", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("agency", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Openness", 5f, 9f),
	 new PepoRolePrerequisite("Honesty", 5f, 8f)
            }, 0.01f, 0.02f,
            new List<string>() { "Clay collector", "Fire tender" } // Requires clay AND fire knowledge
            ),

        // CONSTRUCTION MATERIALS
        new PepoDataRoles(
            "Brickmaker",
            "making bricks from clay for construction.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Openness", 2f, 6f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.01f, 0.02f,
            new List<string>() { "Clay collector", "Shelter builder" } // Requires clay knowledge
            ),

        // FORMAL COMMUNICATION
        new PepoDataRoles(
            "Herald",
            "making formal announcements and proclamations.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Extraversion", 6f, 9f),
	 new PepoRolePrerequisite("Extraversion", 5f, 8f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f)
            }, 0.001f, 0.005f,
            new List<string>() { "Courier", "Memory Keeper" } // Requires message delivery AND memorization
            ),

        // RESOURCE ALLOCATION
        new PepoDataRoles(
            "Space Allocator",
            "allocating living and working spaces in settlements.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("power", 3f, 7f),
	 new PepoRolePrerequisite("Honesty", 6f, 9f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Agreeableness", 4f, 7f)
            }, 0.005f, 0.01f,
            new List<string>() { "Dispute Mediator", "Boundary Marker" } // Requires conflict resolution AND boundary knowledge
            ),

        // SOCIAL REGULATION
        new PepoDataRoles(
            "Social Mirror",
            "reflecting community norms and behaviors back to members.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("agency", 3f, 7f),
	 new PepoRolePrerequisite("benevolence", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("Honesty", 7f, 10f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	 new PepoRolePrerequisite("Agreeableness", 1f, 4f)
            }, 0.01f, 0.03f,
            new List<string>() { "Dispute Mediator", "Hearth Story-Weaver" } // Requires conflict AND social knowledge
            ),

        // PUBLIC HEALTH
        new PepoDataRoles(
            "Sanitation Keeper",
            "maintaining community cleanliness and waste management.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 4f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("benevolence", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Openness", 2f, 5f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.005f, 0.01f,
            new List<string>() { "Drainage Digger", "Fire Tender" } // Requires waste management AND fire for sanitation
            ),

        // LABOR ORGANIZATION
        new PepoDataRoles(
            "Labor Coordinator",
            "organizing and directing community work projects.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("benevolence", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Agreeableness", 6f, 9f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Extraversion", 5f, 8f)
            }, 0.005f, 0.01f,
            new List<string>() { "Festival Organizer", "Space Allocator" } // Requires event AND space organization
            ),

        // TEXTILE PRODUCTION
        new PepoDataRoles(
            "Weaver",
            "weaving textiles and fabrics on looms.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f),
	 new PepoRolePrerequisite("Agreeableness", 4f, 7f)
            }, 0.02f, 0.05f,
            new List<string>() { "Bone Carver", "Spinner" } // Requires fiber knowledge AND spinning
            ),

        new PepoDataRoles(
            "Spinner",
            "spinning fibers into thread and yarn.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f),
	 new PepoRolePrerequisite("Agreeableness", 4f, 7f)
            }, 0.02f, 0.05f,
            new List<string>() { "Fiber/Textile Forager" } // Requires fiber knowledge
            ),

        // FIRE BYPRODUCT UTILIZATION
        new PepoDataRoles(
            "Ash Collector",
            "collecting and processing ashes for various uses.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Openness", 2f, 5f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.005f, 0.01f,
            new List<string>() { "Fire Tender" } // Requires fire management
            ),

        // EMOTIONAL SUPPORT
        new PepoDataRoles(
            "Hearth Confidant",
            "providing emotional support and confidentiality at the hearth.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("benevolence", 4f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Agreeableness", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 5f, 9f),
	 new PepoRolePrerequisite("Extraversion", 1f, 5f)
            }, 0.04f, 0.1f,
            new List<string>() { "Fire Tender", "Memory Keeper" } // Requires hearth presence AND discretion
            ),

        // TOOL MAINTENANCE
        new PepoDataRoles(
            "Tool Repairer",
            "repairing and maintaining tools and implements.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("achievement", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Openness", 5f, 9f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.005f, 0.01f,
            new List<string>() { "Flint Knapper" } // Requires tool making knowledge
            ),

        // RESOURCE DISTRIBUTION
        new PepoDataRoles(
            "Grain Allocator",
            "allocating grain stores to community members.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("power", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("Honesty", 7f, 10f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Agreeableness", 4f, 7f)
            }, 0.001f, 0.005f,
            new List<string>() { "Grain Measurer" } // Requires measurement skills
            ),

        new PepoDataRoles(
            "Warehouse Keeper",
            "storing and protecting community food stores.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("security", 4f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("Honesty", 7f, 10f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.005f, 0.01f,
            new List<string>() { "Grain Measurer" } // Requires measurement AND storage knowledge
            ),

        // TAXATION/TRIBUTE
        new PepoDataRoles(
            "Tribute Collector",
            "collecting tribute or taxes for community leaders.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("power", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Extraversion", 5f, 8f),
	 new PepoRolePrerequisite("Agreeableness", 2f, 6f)
            }, 0.002f, 0.005f,
            new List<string>() { "Grain Measurer", "Ritual Coordinator" } // Requires measurement AND authority
            ),

        // HOSPITALITY
        new PepoDataRoles(
            "Host",
            "providing hospitality to visitors and travelers.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("benevolence", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f),
	 new PepoRolePrerequisite("Agreeableness", 6f, 10f),
	 new PepoRolePrerequisite("Conscientiousness", 5f, 9f)
            }, 0.08f, 0.15f,
            new List<string>() { "Hearth Tender", "Trader" } // Requires hearth management AND visitor experience
            ),

        // SOCIAL NETWORKING
        new PepoDataRoles(
            "Kinship Networker",
            "maintaining and mapping kinship relationships.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 4f, 7f),
	 new PepoRolePrerequisite("benevolence", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f),
	 new PepoRolePrerequisite("Honesty", 5f, 9f),
	 new PepoRolePrerequisite("Agreeableness", 5f, 9f)
            }, 0.02f, 0.05f,
            new List<string>() { "Matchmaker", "Gift Exchanger", "Memory Keeper" } // Requires matchmaking, exchange, AND memory
            ),

        // MEDICINAL SPECIALIZATION
        new PepoDataRoles(
            "Herbalist",
            "preparing and administering herbal medicines.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("benevolence", 4f, 7f),
	 new PepoRolePrerequisite("universalism", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Openness", 6f, 10f),
	 new PepoRolePrerequisite("Agreeableness", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 5f, 9f)
            }, 0.005f, 0.01f,
            new List<string>() { "Medicinal forager" } // Formalization of medicinal knowledge
            ),

        // PROFESSIONAL STORYTELLING
        new PepoDataRoles(
            "Storyteller",
            "professionally telling stories for entertainment and education.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 4f, 7f),
	 new PepoRolePrerequisite("stimulation", 3f, 7f),
	 new PepoRolePrerequisite("universalism", 3f, 7f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f),
	 new PepoRolePrerequisite("Openness", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f)
            }, 0.005f, 0.01f,
            new List<string>() { "Memory Keeper" } // Professionalization of storytelling
            ),

        // MARRIAGE BROKERING
        new PepoDataRoles(
            "Matchmaker",
            "arranging marriages and partnerships.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 4f, 7f),
	 new PepoRolePrerequisite("benevolence", 3f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("Agreeableness", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 5f, 9f),
	 new PepoRolePrerequisite("Honesty", 6f, 9f)
            }, 0.005f, 0.01f,
            new List<string>() { "Memory Keeper", "Gift Exchanger" } // Requires social knowledge AND exchange
            ),

        // LEGAL WITNESSING
        new PepoDataRoles(
            "Oath-Witness",
            "witnessing and remembering oaths and agreements.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 4f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Honesty", 7f, 10f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Extraversion", 2f, 5f)
            }, 0.005f, 0.01f,
            new List<string>() { "Memory Keeper", "Ritual Coordinator" } // Requires memory AND ritual authority
            ),

        // FOOD PROCESSING SPECIALIZATION
        new PepoDataRoles(
            "Baker",
            "baking bread and other baked goods.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("hedonism", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Openness", 2f, 6f),
	 new PepoRolePrerequisite("Honesty", 5f, 8f)
            }, 0.01f, 0.02f,
            new List<string>() { "Miller", "Fermenter" } // Requires flour processing
            ),

        // ANIMAL PRODUCT PROCESSING
        new PepoDataRoles(
            "Tanner",
            "tanning animal hides into leather.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("achievement", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Openness", 2f, 6f),
	 new PepoRolePrerequisite("Honesty", 5f, 8f)
            }, 0.005f, 0.01f,
            new List<string>() {  } // Requires access to animal hides
            ),

        new PepoDataRoles(
            "Dairy Processer",
            "processing milk into cheese, yogurt, and other products.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Agreeableness", 4f, 8f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.02f, 0.03f,
            new List<string>() { "Pastoral Farmer", "Fermenter" } // Requires dairy animals
            ),

        // RELIGIOUS LEADERSHIP
        new PepoDataRoles(
            "Ritual Leader",
            "leading major rituals and religious ceremonies.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 4f, 7f),
	 new PepoRolePrerequisite("power", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f),
	 new PepoRolePrerequisite("Openness", 7f, 10f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f)
            }, 0.001f, 0.005f,
            new List<string>() { "Ritual Coordinator" } // Requires ritual organization experience
            ),

        // FESTIVAL ORGANIZATION
        new PepoDataRoles(
            "Festival Organizer",
            "organizing community festivals and celebrations.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 4f, 7f),
	 new PepoRolePrerequisite("hedonism", 3f, 7f),
	 new PepoRolePrerequisite("benevolence", 3f, 7f),
	 new PepoRolePrerequisite("Extraversion", 6f, 9f),
	 new PepoRolePrerequisite("Extraversion", 6f, 9f),
	 new PepoRolePrerequisite("Openness", 5f, 9f)
            }, 0.005f, 0.01f,
            new List<string>() { "Ritual Coordinator", "Brewer", "Baker" } // Requires ritual, drink, AND food
            ),

        // LIFE TRANSITION GUIDANCE
        new PepoDataRoles(
            "Rites of Passage Guide",
            "guiding individuals through life transitions and rituals.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("tradition", 4f, 7f),
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Openness", 6f, 10f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 10f),
	 new PepoRolePrerequisite("Extraversion", 5f, 9f)
            }, 0.005f, 0.02f,
            new List<string>() { "Ritual Leader", "Elder Councillor", "Herbalist" } // Requires ritual, wisdom, AND medicinal knowledge
            ),

        // LEATHER WORKING
        new PepoDataRoles(
            "Leatherworker",
            "working leather into clothing, bags, and equipment.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("achievement", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Openness", 5f, 9f),
	 new PepoRolePrerequisite("Agreeableness", 4f, 7f)
            }, 0.005f, 0.01f,
            new List<string>() { "Tanner" } // Requires tanned leather
            ),

        // RECYCLING/REUSE
        new PepoDataRoles(
            "Bone & Hide Recycler",
            "finding uses for bone and hide scraps and leftovers.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Openness", 2f, 5f),
	 new PepoRolePrerequisite("Extraversion", 2f, 6f)
            }, 0.005f, 0.01f,
            new List<string>() { "Tanner", "Bone carver" } // Requires both bone AND hide work
            ),

        // MEASUREMENT SYSTEMS
        new PepoDataRoles(
            "Grain Measurer",
            "measuring and quantifying grain stores.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("security", 3f, 7f),
	 new PepoRolePrerequisite("Honesty", 7f, 10f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Extraversion", 4f, 7f)
            }, 0.005f, 0.01f,
            new List<string>() { "Thresher" } // Requires grain processing knowledge
            ),

        // GRAIN MILLING
        new PepoDataRoles(
            "Miller",
            "milling grain into flour using querns or mills.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("conformity", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 7f, 10f),
	 new PepoRolePrerequisite("Extraversion", 2f, 5f),
	 new PepoRolePrerequisite("Honesty", 5f, 8f)
            }, 0.01f, 0.02f,
            new List<string>() { "Thresher" } // Requires grain processing
            ),

        // ALCOHOL PRODUCTION
        new PepoDataRoles(
            "Brewer",
            "brewing beer and fermented beverages.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("hedonism", 3f, 7f),
	 new PepoRolePrerequisite("tradition", 3f, 7f),
	 new PepoRolePrerequisite("Conscientiousness", 6f, 9f),
	 new PepoRolePrerequisite("Openness", 5f, 8f),
	 new PepoRolePrerequisite("Emotionality", 3f, 7f)
            }, 0.01f, 0.02f,
            new List<string>() { "Thresher", "Potter" } // Requires grain AND containers
            ),

        // LONG-DISTANCE TRADE
        new PepoDataRoles(
            "Caravaner",
            "leading trading caravans between distant settlements.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("stimulation", 4f, 7f),
	 new PepoRolePrerequisite("agency", 3f, 7f),
	 new PepoRolePrerequisite("achievement", 3f, 7f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f),
	 new PepoRolePrerequisite("Openness", 7f, 10f),
	 new PepoRolePrerequisite("Emotionality", 2f, 6f)
            }, 0.005f, 0.01f,
            new List<string>() { "Trader" } // Requires trading experience
            ),

        // MARGINAL/OUTSIDER ROLES
        new PepoDataRoles(
            "Fence",
            "dealing in stolen or illegal goods.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("agency", 3f, 7f),
	 new PepoRolePrerequisite("hedonism", 3f, 7f),
	 new PepoRolePrerequisite("power", 3f, 7f),
	 new PepoRolePrerequisite("Extraversion", 6f, 10f),
	 new PepoRolePrerequisite("Agreeableness", 2f, 6f),
	 new PepoRolePrerequisite("Honesty", 6f, 9f)
            }, 0.001f, 0.005f,
            new List<string>() { "Trader" } // Requires trading network
            ),

        // ILLEGAL HUNTING
        new PepoDataRoles(
            "Poacher",
            "hunting on others' lands without permission.",
            "Early Agrarian Era",
            new List<PepoRolePrerequisite>()
            {
	 new PepoRolePrerequisite("stimulation", 3f, 7f),
	 new PepoRolePrerequisite("agency", 3f, 7f),
	 new PepoRolePrerequisite("hedonism", 3f, 7f),
	 new PepoRolePrerequisite("Extraversion", 5f, 9f),
	 new PepoRolePrerequisite("Emotionality", 2f, 6f),
	 new PepoRolePrerequisite("Conscientiousness", 3f, 7f)
            }, 0.005f, 0.02f,
            new List<string>() { "Trapper", "Scout" } // Requires hunting AND stealth skills
            )
    };
}
