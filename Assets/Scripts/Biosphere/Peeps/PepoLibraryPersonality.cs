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

		       // Triads (combinations of 3 emotions) - built logically from dyad relationships
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

			   { "Disgust-Fear-Joy", "Ashamed" },
			   { "Disgust-Fear-Sadness", "Despairing" },
			   { "Disgust-Fear-Surprise", "Horrified" },
			   { "Disgust-Fear-Trust", "Distrustful" },

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
}
