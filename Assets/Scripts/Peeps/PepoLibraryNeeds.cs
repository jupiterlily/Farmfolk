using System.Collections.Generic;

public static class PepoLibraryNeeds
{
	     public static List<Need> PhysiologicalNeeds = new List<Need>()
	     { new Need("hunger", "food", "eat", "stuffed", "full", "peckish", "hungry", "starving", 2000, true, decayRate: 83.33f), //2000 calories, 83.33 cal per hour = 24 hours to deplete
	     new Need("thirst", "water", "drink", "bloated", "satisfied", "thirsty", "parched", "dehydrated", 3000, true, decayRate: 125f), //3000 ml, 125 ml per hour = 24 hours to deplete
	     new Need("rest", "sleep", "rest", "well-rested", "alert", "sleepy", "tired", "exhausted", 8, true, decayRate: 1f), //8 hours of sleep, 1 hour per hour = 8 hours to deplete
									//  new Need("oxygen", "air", "breathe", "hyperventilating", "breathing", "breathless", "gasping", "suffocating", 1440, true, decayRate: 60f), //1440 minutes of oxygen, 60 minutes per hour = 24 hours to deplete
									//   new Need("temperature", "warmth", "warm up", "overheated", "warm", "comfortable", "cold", "freezing", 100, true, decayRate: 4.16f), //100 units of temperature comfort, 4.16 units per hour = 24 hours to deplete
									new Need("bladder", "toilet", "tinkle", "relieved", "comfortable", "urgent", "desperate", "incontinent", 450, false, decayRate: 85f), //100 units of bladder/bowel comfort, 4.16 units per hour = 24 hours to deplete
	     };

	     public static List<Need> SafetyNeeds = new List<Need>(){                      new Need("health", "care", "heal", "healthy", "fine", "suffering", "critical", "dying", 100, true), //health out of 100, decays slowly without care)
	     new Need("security", "shelter", "secure", "safe", "protected", "vulnerable", "threatened", "in danger", 100, false, decayRate: 0.5f), //security out of 100
										    };

	     public static List<Need> NeedTemplates = new List<Need>()
{
	     // Physiological Needs. breathing, flood, water, sleep, heat, cold, pee, poop
	     new Need("hunger", "food", "eat", "stuffed", "full", "peckish", "hungry", "starving", 2000, true, decayRate: 83.33f), //2000 calories, 83.33 cal per hour = 24 hours to deplete
	     new Need("thirst", "water", "drink", "bloated", "satisfied", "thirsty", "parched", "dehydrated", 3000, true, decayRate: 125f), //3000 ml, 125 ml per hour = 24 hours to deplete
	     new Need("rest", "sleep", "rest", "well-rested", "alert", "sleepy", "tired", "exhausted", 8, true, decayRate: 1f), //8 hours of sleep, 1 hour per hour = 8 hours to deplete
	     new Need("oxygen", "air", "breathe", "hyperventilating", "breathing", "breathless", "gasping", "suffocating", 1440, true, decayRate: 60f), //1440 minutes of oxygen, 60 minutes per hour = 24 hours to deplete
	     new Need("temperature", "warmth", "warm up", "overheated", "warm", "comfortable", "cold", "freezing", 100, true, decayRate: 4.16f), //100 units of temperature comfort, 4.16 units per hour = 24 hours to deplete
	     new Need("elimination", "toilet", "use the toilet", "relieved", "comfortable", "urge to go", "desperate", "incontinent", 100, true, decayRate: 4.16f), //100 units of bladder/bowel comfort, 4.16 units per hour = 24 hours to deplete
	     // Safety Needs. health, security of resources, morality, family, health, property
	     new Need("health", "care", "heal", "robust", "healthy", "suffering", "critical", "dying", 100, true, decayRate: 0.1f), //health out of 100, decays slowly without care)
	     new Need("security", "shelter", "secure", "safe", "protected", "vulnerable", "threatened", "in danger", 100, false, decayRate: 0.5f), //security out of 100
										    // Social Needs. connection, love, belonging, romance, friendhsip
	     new Need("social connection", "social interaction", "socialise", "overwhelmed", "popular", "connected", "lonely", "isolated", 100, false, decayRate: 1f), //social connection out of 100
	     new Need("intimacy", "intimate interaction", "be intimate", "satisfied", "fulfilled", "desiring", "craving", "deprived", 100, false, decayRate: 1f), //intimacy out of 100
											    // Esteem Needs. self-esteem, respect, recognition, strength, freedom
	     new Need("esteem", "achievement", "achieve", "accomplished", "confident", "doubtful", "insecure", "worthless", 100, false, decayRate: 0.5f), //esteem out of 100
										           // Cognitive Needs. knowledge, meaning, self-awareness
	     new Need("cognition", "learning", "learn", "enlightened", "knowledgeable", "curious", "confused", "ignorant", 100, false, decayRate: 0.5f), //cognition out of 100
										          // Aesthetic Needs. appreciation of beauty and excellence
	     new Need("aesthetic appreciation", "artistic experience", "appreciate", "inspired", "pleased", "indifferent", "displeased", "averse", 100, false, decayRate: 0.5f), //aesthetic appreciation out of 100
	     //self-actualization Needs. personal growth, self-fulfillment, peak experiences
	     new Need("self-actualization", "personal growth", "grow", "fulfilled", "satisfied", "unfulfilled", "stagnant", "lost", 100, false, decayRate: 0.2f), //self-actualization out of 100
	     // Transcendence Needs. helping others achieve self-actualization
	     new Need("transcendence", "helping others", "help", "enlightened", "fulfilled", "desiring", "craving", "deprived", 100, false, decayRate: 0.2f) }; //transcendence out of 100

}


