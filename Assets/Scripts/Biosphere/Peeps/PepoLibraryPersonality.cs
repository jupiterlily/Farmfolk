using System.Collections.Generic;

public static class PepoLibraryPersonality
{
	     public static readonly Dictionary<string, string> Feelings = new Dictionary<string, string>
	     {        
		          // Dyads (combinations of 2 emotions)
		          { "Anger-Anticipation", "Aggressive" },
		          { "Anger-Disgust", "Contempt" },
		          { "Anger-Fear", "Hate" },//
		          { "Anger-Joy", "Prideful" },
		          { "Anger-Sadness", "Envious" },
		          { "Anger-Surprise", "Outraged" },
		          { "Anger-Trust", "Domineering" },

		          { "Anticipation-Disgust", "Cynical" },
		          { "Anticipation-Fear", "Anxious" },
		          { "Anticipation-Joy", "Optimistic" },
		          { "Anticipation-Sadness", "Pessimistic" },
		          { "Anticipation-Surprise", "Uncertain" },//
		          { "Anticipation-Trust", "Hopeful" },

		          { "Disgust-Fear", "Shame" },
		          { "Disgust-Joy", "Morbid" },
		          { "Disgust-Sadness", "Remorseful" },
		          { "Disgust-Surprise", "Revolted" },
		          { "Disgust-Trust", "Ambivalent" },//

		          { "Fear-Joy", "Guilty" },
		          { "Fear-Sadness", "Despair" },
		          { "Fear-Surprise", "Alarmed" },
		          { "Fear-Trust", "Submissive" },

		          { "Joy-Sadness", "Bittersweet" },//
		          { "Joy-Surprise", "Delighted" },
		          { "Joy-Trust", "Love" },

		          { "Sadness-Surprise", "Disappointed" },
		          { "Sadness-Trust", "Sentimental" },

		          { "Surprise-Trust", "Curious" },

		       // Triads (combinations of 3 emotions) - built from dyad relationships
			   { "Anger-Anticipation-Disgust", "Resentful" },
			   { "Anger-Anticipation-Fear", "Defensive" },
			   { "Anger-Anticipation-Joy", "Passionate" },
			   { "Anger-Anticipation-Sadness", "Bitter" },
			   { "Anger-Anticipation-Surprise", "Outraged" },
			   { "Anger-Anticipation-Trust", "Assertive" },

			   { "Anger-Disgust-Fear", "Contemptuous" },
			   { "Anger-Disgust-Joy", "Spiteful" },
			   { "Anger-Disgust-Sadness", "Cynical" },
			   { "Anger-Disgust-Surprise", "Revolted" },
			   { "Anger-Disgust-Trust", "Disdainful" },

			   { "Anger-Fear-Joy", "Defiant" },
			   { "Anger-Fear-Sadness", "Desperate" },
			   { "Anger-Fear-Surprise", "Panicked" },
			   { "Anger-Fear-Trust", "Paranoid" },

			   { "Anger-Joy-Sadness", "Melancholic" },
			   { "Anger-Joy-Surprise", "Indignant" },
			   { "Anger-Joy-Trust", "Prideful" },

			   { "Anger-Sadness-Surprise", "Betrayed" },
			   { "Anger-Sadness-Trust", "Resigned" },
			   { "Anger-Surprise-Trust", "Indignant" },

			   { "Anticipation-Disgust-Fear", "Apprehensive" },
			   { "Anticipation-Disgust-Joy", "Skeptical" },
			   { "Anticipation-Disgust-Sadness", "Pessimistic" },
			   { "Anticipation-Disgust-Surprise", "Shocked" },
			   { "Anticipation-Disgust-Trust", "Cautious" },

			   { "Anticipation-Fear-Joy", "Nervous" },
			   { "Anticipation-Fear-Sadness", "Dreadful" },
			   { "Anticipation-Fear-Surprise", "Startled" },
			   { "Anticipation-Fear-Trust", "Wary" },

			   { "Anticipation-Joy-Sadness", "Wistful" },
			   { "Anticipation-Joy-Surprise", "Excited" },
			   { "Anticipation-Joy-Trust", "Hopeful" },

			   { "Anticipation-Sadness-Surprise", "Disappointed" },
			   { "Anticipation-Sadness-Trust", "Reflective" },
			   { "Anticipation-Surprise-Trust", "Curious" },

			   { "Disgust-Fear-Joy", "Ashamed" }, //morbid + shame + guilty
			   { "Disgust-Fear-Sadness", "Despairing" },
			   { "Disgust-Fear-Surprise", "Horrified" },
			   { "Disgust-Fear-Trust", "Distrustful" },//needs adjusting

			   { "Disgust-Joy-Sadness", "Remorseful" },
			   { "Disgust-Joy-Surprise", "Disgusted" },
			   { "Disgust-Joy-Trust", "Conflicted" },

			   { "Disgust-Sadness-Surprise", "Disillusioned" },
			   { "Disgust-Sadness-Trust", "Disheartened" },
			   { "Disgust-Surprise-Trust", "Appalled" },

			   { "Fear-Joy-Sadness", "Vulnerable" },
			   { "Fear-Joy-Surprise", "Overwhelmed" },
			   { "Fear-Joy-Trust", "Compliant" },

			   { "Fear-Sadness-Surprise", "Terror" },
			   { "Fear-Sadness-Trust", "Resignation" },
			   { "Fear-Surprise-Trust", "Awestruck" },

			   { "Joy-Sadness-Surprise", "Touched" },
			   { "Joy-Sadness-Trust", "Sentimental" },
			   { "Joy-Surprise-Trust", "Delighted" },

			   { "Sadness-Surprise-Trust", "Moved" }
	     };
	     public static readonly Dictionary<string, string> PersonalityDyads = new Dictionary<string, string>
	         {
		             // Honesty-Humility (Sincere/Fair vs. Greedy/Pretentious)
		          { "HighHonesty-HighEmotionality", "Idealistic" },          // H-High, E-High: Genuine in emotions
		          { "HighHonesty-LowEmotionality", "Stoic" },          // H-High, E-Low: Moral without emotional display
		          { "LowHonesty-HighEmotionality", "Mercurial" },     // H-Low, E-High:
		          { "LowHonesty-LowEmotionality", "Cold" },   // H-Low, E-Low: 

		          // Honesty-Humility × Extraversion
		          { "HighHonesty-HighExtraversion", "Sincere" },      // H-High, X-High: Openly sincere
		          { "HighHonesty-LowExtraversion", "Humble" },          // H-High, X-Low: Quietly authentic
		          { "LowHonesty-HighExtraversion", "Charming" },   // H-Low, X-High: Socially manipulative
		          { "LowHonesty-LowExtraversion", "Antisocial" },   // H-Low, X-Low: Withdrawn but deceptive

		          // Honesty-Humility × Agreeableness (Patient/Forgiving vs. Irritable/Stubborn)
		          { "HighHonesty-HighAgreeableness", "Benevolent" },         // H-High, A-High: Sincerely forgiving
		          { "HighHonesty-LowAgreeableness", "Righteous" },      // H-High, A-Low: Morally rigid
		          { "LowHonesty-HighAgreeableness", "Duplicitous" },     // H-Low, A-High: Strategic forgiveness
		          { "LowHonesty-LowAgreeableness", "Vindictive" },// H-Low, A-Low: Spiteful and deceptive

		          // Honesty-Humility × Conscientiousness (Organized/Disciplined vs. Disorganized/Careless)
		          { "HighHonesty-HighConscientiousness", "Trustworthy" },           // H-High, C-High: Morally meticulous
		          { "HighHonesty-LowConscientiousness", "Casual" },           // H-High, C-Low: 
		          { "LowHonesty-HighConscientiousness", "Strategic" },      // H-Low, C-High: Carefully manipulative
		          { "LowHonesty-LowConscientiousness", "Unscrupulous" },  // H-Low, C-Low: Carelessly deceptive

		          // Honesty-Humility × Openness (Creative/Curious vs. Conventional/Uncreative)
		          { "HighHonesty-HighOpenness", "Candid" },                  // H-High, O-High: Openly truthful
		          { "HighHonesty-LowOpenness", "Straightforward" },    // H-High, O-Low: Simply honest
		          { "LowHonesty-HighOpenness", "Complex" },           // H-Low, O-High: Deceptively creative
		          { "LowHonesty-LowOpenness", "Simplistic" },   // H-Low, O-Low: Basic manipulation

		          // Emotionality (Anxious/Sentimental vs. Brave/Confident)
		          { "HighEmotionality-HighExtraversion", "Expressive" },    // E-High, X-High: Emotionally outgoing
		          { "HighEmotionality-LowExtraversion", "Sensitive" },     // E-High, X-Low: Internal emotional depth
		          { "LowEmotionality-HighExtraversion", "Poised" },         // E-Low, X-High: Socially confident
		          { "LowEmotionality-LowExtraversion", "Self-reliant" },   // E-Low, X-Low: Independent and calm

		          // Emotionality × Agreeableness
		          { "HighEmotionality-HighAgreeableness", "Empathetic" },      // E-High, A-High: Emotionally compassionate
		          { "HighEmotionality-LowAgreeableness", "Resentful" },   // E-High, A-Low: Emotionally bitter
		          { "LowEmotionality-HighAgreeableness", "Tolerant" },        // E-Low, A-High: Rationally forgiving
		          { "LowEmotionality-LowAgreeableness", "Rigid" },         // E-Low, A-Low: Unemotionally rigid

		          // Emotionality × Conscientiousness
		          { "HighEmotionality-HighConscientiousness", "Cautious" },          // E-High, C-High: Anxiously meticulous
		          { "HighEmotionality-LowConscientiousness", "Volatile" },        // E-High, C-Low: Emotionally disorganized
		          { "LowEmotionality-HighConscientiousness", "Analytical" },         // E-Low, C-High: Calmly organized
		          { "LowEmotionality-LowConscientiousness", "Disengaged" },        // E-Low, C-Low: Calmly disorganized

		          // Emotionality × Openness
		          { "HighEmotionality-HighOpenness", "Passionate" },           // E-High, O-High: Emotionally curious
		          { "HighEmotionality-LowOpenness", "Sentimental" },     // E-High, O-Low: Emotionally traditional
		          { "LowEmotionality-HighOpenness", "Objective" },            // E-Low, O-High: Rationally curious
		          { "LowEmotionality-LowOpenness", "Nonchalant" },        // E-Low, O-Low: Unemotional and conventional

		          // Extraversion × Agreeableness
		          { "HighExtraversion-HighAgreeableness", "Gregarious" },    // X-High, A-High: Socially tolerant
		          { "HighExtraversion-LowAgreeableness", "Dominant" },  // X-High, A-Low: Socially assertive
		          { "LowExtraversion-HighAgreeableness", "Accepting" },     // X-Low, A-High: Quietly tolerant
		          { "LowExtraversion-LowAgreeableness", "Withdrawn" }, // X-Low, A-Low: Quietly stubborn

		          // Extraversion × Conscientiousness
		          { "HighExtraversion-HighConscientiousness", "Organized" },       // X-High, C-High: Socially disciplined
		          { "HighExtraversion-LowConscientiousness", "Spontaneous" },    // X-High, C-Low: Socially impulsive
		          { "LowExtraversion-HighConscientiousness", "Meticulous" },      // X-Low, C-High: Quietly organized
		          { "LowExtraversion-LowConscientiousness", "Unsystematic" },   // X-Low, C-Low: Quietly disorganized

		          // Extraversion × Openness
		          { "HighExtraversion-HighOpenness", "Enthusiastic" },       // X-High, O-High: Socially curious
		          { "HighExtraversion-LowOpenness", "Conventional" },      // X-High, O-Low:
		          { "LowExtraversion-HighOpenness", "Reflective" },         // X-Low, O-High: Quietly curious
		          { "LowExtraversion-LowOpenness", "Reserved" },      // X-Low, O-Low: Quietly conventional

		          // Agreeableness × Conscientiousness
		          { "HighAgreeableness-HighConscientiousness", "Considerate" },       // A-High, C-High: Patiently organized
		          { "HighAgreeableness-LowConscientiousness", "Lenient" },          // A-High, C-Low: Patiently disorganized
		          { "LowAgreeableness-HighConscientiousness", "Rigid" },         // A-Low, C-High: Stubbornly organized
		          { "LowAgreeableness-LowConscientiousness", "Hostile" },      // A-Low, C-Low: Stubbornly disorganized

		          // Agreeableness × Openness
		          { "HighAgreeableness-HighOpenness", "Welcoming" },             // A-High, O-High: HighOpenness-minded
		          { "HighAgreeableness-LowOpenness", "Compliant" },       // A-High, O-Low: Traditionally agreeable
		          { "LowAgreeableness-HighOpenness", "Critical" },         // A-Low, O-High: Skeptical and stubborn
		          { "LowAgreeableness-LowOpenness", "Intolerant" },  // A-Low, O-Low: Traditionally stubborn

		          // Conscientiousness × Openness
		          { "HighConscientiousness-HighOpenness", "Methodical" },             // C-High, O-High: Organized curiosity
		          { "HighConscientiousness-LowOpenness", "Conservative" },       // C-High, O-Low: Organized conventionality
		          { "LowConscientiousness-HighOpenness", "Experimental" },          // C-Low, O-High: Disorganized curiosity
		          { "LowConscientiousness-LowOpenness", "Uninspired" }         // C-Low, O-Low: Disorganized conventionality
};


	     public static class HEXACODimension
	     {
		          public static PersonalityAspect H => new PersonalityAspect('h', new List<string> { "honest" }, new List<string> { "manipulative" });
		          public static PersonalityAspect E => new PersonalityAspect('e', new List<string> { "emotional" }, new List<string> { "detatched" });
		          public static PersonalityAspect X => new PersonalityAspect('x', new List<string> { "extraverted" }, new List<string> { "introverted" });
		          public static PersonalityAspect A => new PersonalityAspect('a', new List<string> { "forgiving" }, new List<string> { "resentful" });
		          public static PersonalityAspect C => new PersonalityAspect('c', new List<string> { "careful" }, new List<string> { "reckless" });
		          public static PersonalityAspect O => new PersonalityAspect('o', new List<string> { "open" }, new List<string> { "nonchalant" });
		          public static List<string> HFacets(bool invert)
		          {
				if (!invert)
				{
					     return new List<string> { "sincere", "honest", "faithful", "loyal", "modest", "unassuming", "authentic", "decent", "trustworthy", "honourable", "genuine", "direct" };
				}
				else
				{
					     return new List<string> { "sly", "deceitful", "greedy", "pretentious", "hypocritical", "boastful", "proud", "deceptive", "false", "duplicitous", "underhanded", "misleading", "two faced", "disingenuous" };
				}
		          }
		          public static List<string> EFacets(bool invert)
		          {
				if (!invert)
				{
					     return new List<string> { "emotional", "sensitive", "sentimental", "anxious", "romantic", "idealistic", "hopeful", "passionate" };
				}
				else
				{
					     return new List<string> { "brave", "stoic", "independent", "self-assured", "stable", "serious", "cold", "pragmatic" };
				}
				;
		          }
		          public static List<string> XFacets(bool invert)
		          {
				if (!invert)
				{
					     return new List<string> { "outgoing", "lively", "extraverted", "sociable", "talkative", "bold", "active" };
				}
				else
				{
					     return new List<string> { "shy", "passive", "withdrawn", "introverted", "quiet", "reserved", "unenthusastic" };
				}
		          }
		          public static List<string> AFacets(bool invert)
		          {
				if (!invert)
				{
					     return new List<string> { "patient", "tolerant", "peaceful", "mild", "agreeable", "lenient", "gentle", "accommodating" };
				}
				else
				{
					     return new List<string> { "ill-tempered", "quarrelsome", "stubborn", "choleric", "cynical" };
				}
				;
		          }
		          public static List<string> CFacets(bool invert)
		          {
				if (!invert)
				{
					     return new List<string> { "organised", "disciplined", "diligent", "careful", "thorough", "precise" };
				}
				else
				{
					     return new List<string> { "sloppy", "negligent", "reckless", "irresponsible", "absent-minded" };
				}
		          }
		          public static List<string> OFacets(bool invert)
		          {
				if (!invert)
				{
					     return new List<string> { "intellectual", "creative", "unconventional", "innovative", "curious", "accepting" };
				}
				else
				{
					     return new List<string> { "shallow", "unimaginative", "conventional" };
				}
				;
		          }
	     }
	     //facets are subcategories of the six HEXACO personality dimensions. Each facet represents a specific aspect of an individual's personality within that dimension. a pepos personality aspects are the sum og their facets.

	     //honest/humilty domain contains facets of sincerity, fairness, greed aversion, and modesty.

	     //emotionality domain contains facets of fearfulness, anxiety, dependence, and sentimentality.

	     //extraversion domain contains facets of social self-esteem, social boldness, sociability, and liveliness.

	     //agreeableness domain contains facets of forgiveness, gentleness, flexibility, and patience.

	     //conscientiousness domain contains facets of organization, diligence, perfectionism, and prudence.

	     //openness to experience domain contains facets of aesthetic appreciation, inquisitiveness, creativity, and unconventionality.

	     //we need arrays of strings instead od hardcoding the dictionary entries. what happens if personalities change?

	     public static readonly Dictionary<string, PersonalityAspect> HFacets = new Dictionary<string, PersonalityAspect>
	     {
		          { "Sincerity", new PersonalityAspect('.',  new List<string> {"sincere", "genuine", "truthful"}, new List<string> {"deceptive", "insincere", "dishonest", "manipulative"}, 5,"Sincerity") },
		          { "Fairness", new PersonalityAspect('.',  new List<string> {"fair", "just", "equitable"}, new List<string> {"unfair", "biased", "unjust"}, 5,"Fairness") },
		          { "Greed Aversion", new PersonalityAspect('.',  new List<string> {"selfless", "generous", "altruistic"}, new List<string> {"greedy", "selfish", "avaricious"}, 5,"Greed Aversion") },
		          { "Modesty", new PersonalityAspect('.',  new List<string> {"humble", "unassuming", "modest"}, new List<string> {"boastful", "conceited", "egotistical"}, 5,"Modesty") }
	     };
	     public static readonly Dictionary<string, PersonalityAspect> EFacets = new Dictionary<string, PersonalityAspect>
	     {
		          { "Fearfulness", new PersonalityAspect('.',  new List<string> {"cautious", "vigilant", "alert"}, new List<string> {"fearless", "reckless", "courageous"}, 5,"Fearfulness") },
		          { "Anxiety", new PersonalityAspect('.',  new List<string> {"nervous", "tense", "worried"}, new List<string> {"unflappable", "relaxed", "untroubled"}, 5,"Anxiety") },
		          { "Dependence", new PersonalityAspect('.',  new List<string> {"reliant", "trusting", "faithful"}, new List<string> {"independent", "self-reliant", "autonomous"}, 5,"Dependence") },
		          { "Sentimentality", new PersonalityAspect('.',  new List<string> {"compassionate", "empathetic", "tender-hearted"}, new List<string> {"unfeeling", "cold-hearted", "unsympathetic"}, 5,"Sentimentality") }
	     };
	     public static readonly Dictionary<string, PersonalityAspect> XFacets = new Dictionary<string, PersonalityAspect>
	     {
		          { "Social Self-Esteem", new PersonalityAspect('.',  new List<string> {"secure", "self-assured"}, new List<string> {"insecure", "self-doubting" }, 5,"Social Self-Esteem") },
		          { "Social Boldness", new PersonalityAspect('.',  new List<string> {"outgoing", "bold", "confident"}, new List<string> {"shy", "reserved", "timid"}, 5,"Social Boldness") },
		          { "Sociability", new PersonalityAspect('.',  new List<string> {"friendly", "socoiable", "gregarious"}, new List<string> {"solitary", "introverted", "antisocial"}, 5,"Sociability") },
		          { "Liveliness", new PersonalityAspect('.',  new List<string> {"energetic", "enthusiastic", "vibrant"}, new List<string> {"lethargic", "apathetic", "listless"}, 5,"Liveliness") }
	     };
	     public static readonly Dictionary<string, PersonalityAspect> AFacets = new Dictionary<string, PersonalityAspect>
	     {
		          { "Forgiveness", new PersonalityAspect('.',  new List<string> {"forgiving", "tolerant", "understanding"}, new List<string> {"resentful", "vindictive", "unforgiving"}, 5,"Forgiveness") },
		          { "Gentleness", new PersonalityAspect('.',  new List<string> {"gentle", "kind", "compassionate"}, new List<string> {"critical", "blunt", "abrasive"}, 5,"Gentleness") },
		          { "Flexibility", new PersonalityAspect('.',  new List<string> {"adaptable", "open-minded", "versatile"}, new List<string> {"rigid", "inflexible", "stubborn"}, 5,"Flexibility") },
		          { "Patience", new PersonalityAspect('.',  new List<string> {"patient", "calm", "tolerant"}, new List<string> {"impatient", "restless", "irritable"}, 5,"Patience") }
	     };
	     public static readonly Dictionary<string, PersonalityAspect> CFacets = new Dictionary<string, PersonalityAspect>
	     {
		          { "Organization", new PersonalityAspect('.',  new List<string> {"organized", "systematic", "methodical"}, new List<string> {"disorganized", "chaotic", "haphazard"}, 5,"Organization") },
		          { "Diligence", new PersonalityAspect('.',  new List<string> {"diligent", "industrious", "hardworking"}, new List<string> {"unmotivated", "negligent", "relaxed"}, 5,"Diligence") },
		          { "Perfectionism", new PersonalityAspect('.',  new List<string> {"meticulous", "precise", "thorough"}, new List<string> {"careless", "sloppy", "hasty"}, 5,"Perfectionism") },
		          { "Prudence", new PersonalityAspect('.',  new List<string> {"prudent", "cautious", "judicious"}, new List<string> {"reckless", "impulsive", "rash"}, 5,"Prudence") }
	     };
	     public static readonly Dictionary<string, PersonalityAspect> OFacets = new Dictionary<string, PersonalityAspect>
	     {
		          { "Aesthetic Appreciation", new PersonalityAspect('.',  new List<string> {"artistic", "sensitive", "perceptive"}, new List<string> {"unimaginative", "uncreative", "insensitive"}, 5,"Aesthetic Appreciation") },
		          { "Inquisitiveness", new PersonalityAspect('.',  new List<string> {"curious", "inquisitive", "exploratory"}, new List<string> {"uninterested", "apathetic", "indifferent"}, 5,"Inquisitiveness") },
		          { "Creativity", new PersonalityAspect('.',  new List<string> {"innovative", "original", "inventive"}, new List<string> {"conventional", "unimaginative", "derivative"}, 5,"Creativity") },
		          { "Unconventionality", new PersonalityAspect('.',  new List<string> {"nonconformist", "individualistic", "rebellious"}, new List<string> {"conformist", "traditionalist", "orthodox"}, 5,"Unconventionality") }
	     };
}
