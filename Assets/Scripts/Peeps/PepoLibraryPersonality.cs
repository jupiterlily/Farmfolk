using System.Collections.Generic;

public static class PepoLibraryPersonality
{
	     public static readonly Dictionary<string, string> Feelings = new Dictionary<string, string>
	     {
		           // Single emotions
		          //{ "Anger", "Angry" },
		          //{ "Anticipation", "Anticipating" },
		          //{ "Disgust", "Disgusted" },
		          //{ "Fear", "Fearful" },
		          //{ "Joy", "Joyful" },
		          //{ "Sadness", "Sad" },
		          //{ "Surprise", "Surprised" },
		          //{ "Trust", "Trusting" },
        
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
}
