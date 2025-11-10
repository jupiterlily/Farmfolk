using System.Collections.Generic;

public class PepoRelationshipData
{
	     public PepoData pepo;
	     public string nickname;
	     public string relationshipName;
	     public List<int> met; //list of ints in simulationconstants
	     public float timeSpent = 0;
	     public List<RelationshipType> relationshipTypes = new List<RelationshipType>();

	     public float Confidence = 0; //how much we trust them, although this should be 

	     public float Authority = 0; //how much authority they have over us
	     public float Influence = 0; //how much influence we have over them
	     public float Obligation = 0; //how much we feel obliged to help them

	     public float Intimacy = 0; //how close we are to them
	     public float Romance = 0; //how much we wannna romance them
	     public float Desire = 0; //how much we wanna sex them
	     public float Animosity = 0; //how much we dislike them
	     public float Affection = 0; //how much we like them
	     public float Affiliation = 0; //how much they're in our in group
	     public float Affinity = 0; //how much we wanna interact with them

	     public PepoRelationshipData(PepoData them, PepoData me, RelationshipType type, string name)
	     {
		          pepo = them;
		          //check if type is already on the lis
		          if (!relationshipTypes.Contains(type))
		          {
				relationshipTypes.Add(type);
				nickname = pepo.forename;
				relationshipName = name;
				relationshipTypes.Add(type);
				met = SimulationConstants.TimeConstants;
		          }
		          // a switch for each enum

	     }
	     public void SpendTime(float time, PepoData me)
	     {
		          timeSpent += time;
		          PepoRelationshipData card = PepoRelationalUtilities.HaveWeMet(pepo, me);
		          if (card! == null)
		          {
				card.timeSpent += time;
		          }
	     }
	     public void GiveNickname(string name)
	     {
		          nickname = name;
	     }
}
public enum RelationshipType
{
	     kin, parent, offspring, coparent,
	     peer, acquaintence, friend, bestie,
	     enemy, rival,
	     lover, ex, spouse, rebuffed,
	     mentor, mentee, leader, follower,
}
