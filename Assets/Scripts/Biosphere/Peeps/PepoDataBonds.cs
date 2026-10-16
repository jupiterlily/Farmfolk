using System.Collections.Generic;

public class PepoRelationshipData
{
	     public PepoData pepo;
	     public Dictionary<string, List<PepoData>> nickname = new Dictionary<string, List<PepoData>>();//the name and who called them that
	     public string relationshipName;
	     public List<int> met; //list of ints in simulationconstants
	     public float timeSpent = 0;
	     public List<RelationshipType> relationshipTypes = new List<RelationshipType>();
	     public List<PepoSecretData> secrets = new List<PepoSecretData>(); //list of secrets we have told them

	     public float Confidence = 0; //how much we trust them. Increased by them being reliable, honest, keeping promises, being competent. can be influenced by reputation. you can be confident in someone and still not like them.//how likely we are to respond positively to them persuadiong or decieving us.
	     public float Esteem = 0;    //how much we think they trust/like us. increased by us being reliable, honest, keeping promises, being competent. can be influenced by reputation. we can think someone trusts us and still not like them. how often we have persuaded or decieved them and had them respond positively.

	     public float Authority = 0; //how much authority they have over us, increased  by them having power over us (e.g. controlling resources we need, being able to punish us) how much they have persuaded or intimidated us. how likely we are to do as they say.
	     public float Influence = 0; //how much influence we think we have over them, gained by having them take advice, be persuaded, do you a favour or introducing them to people. how much we think we can/have successfully persuaded, intimidated or decieved them.

	     public float Obligation = 0; //how much we feel obliged to help them. increased by them doing us favours, giving us gifts, helping us in the past
	     public float Reciprocity = 0; //how much we expect them to help us. Increased by us doing them favours, giving them gifts or helping them in some way

	     public float Intimacy = 0; //how close we are to them. Increased by sharing personal information, spending time together, physical contact
	     public float Romance = 0; //how much we wannna romance them. Increased by spending time alone together, them doing romantic gestures for us, us thinking about them romantically
	     public float Desire = 0; //how much we wanna sex them. Increased by sexual attraction, flirting, sexual gestures, physical contact
	     public float Animosity = 0; //how much we dislike them. Increased by them doing us harm, insulting us, betraying us or from competition for resources or clashes in values, morals and goals
	     public float Affection = 0; //how much we like them. Increased by them doing us favours, spending time together, giving us gifts, compliments or physical contact
	     public float Affiliation = 0; //how much they're in our in group. Increased by shared values, goals, group membership, similarities and spending time together
	     public float Attachment = 0; //how much we wanna interact with them. can be parasocial. Increased by time spent together, shared experiences, emotional connection and dependence on them for resources or support or power.

	     public PepoRelationshipData(PepoData them, PepoData me, RelationshipType type, string name)
	     {
		          pepo = them;
		          //check if type is already on the lis
		          if (!relationshipTypes.Contains(type))
		          {
				relationshipTypes.Add(type);
				nickname.Add(me.forename, new List<PepoData>() { me });
				relationshipName = name;
				relationshipTypes.Add(type);
				met = SimulationConstants.TimeConstants;
		          }

		          PepoUtilitiesBonds.ApplyRelationshipModifiers(this, type, me);
	     }
	     public void SpendTime(float time, PepoData me)
	     {
		          timeSpent += time;
		          PepoRelationshipData card = PepoUtilitiesBonds.HaveWeMet(pepo, me);
		          if (card! == null)
		          {
				card.timeSpent += time;
		          }
	     }
	     public void GiveNickname(string name, PepoData giver)
	     {
		          //if the nickname already exists add the pepo to the list of people who call me that. Else add a new entry to the dictionary fot htat nickname.
		          if (nickname.ContainsKey(name))
		          {
				nickname[name].Add(giver);
		          }
		          else
		          {
				nickname.Add(name, new List<PepoData>() { giver });
		          }

	     }
	     public string GetNickname(PepoRelationshipData relationship, PepoData giver)
	     {
		          List<string> names = new List<string>();
		          foreach (var kvp in nickname)
		          {
				if (kvp.Value.Contains(giver))
				{
					     names.Add(kvp.Key);
				}
		          }
		          int index = UnityEngine.Random.Range(0, names.Count - 1);
		          return names[index];
	     }
}
public enum RelationshipType
{
	     kin, parent, offspring, coparent,
	     peer, acquaintence, friend, bestie,
	     enemy, rival,
	     lover, ex, spouse, incompatible, courting,
	     mentor, mentee, leader, follower, rolemodel,

	     // New hierarchical types
	     patron, client,
	     ritual_superior, ritual_inferior,
	     alliance_partner,
	     obligated_kin_elder, obligated_kin_younger,
	     ceremonial_connection,
	     trade_partner,
	     oath_bound,
	     status_rival,  // Same status level but competitive
	     lineage_superior, lineage_inferior,
	     skill_master, skill_apprentice,
	     resource_dependent,
	     protection_sworn,

	     // Kinship
	     Parent, Child, Sibling, MaternalGrandparent, PaternalGrandparent, Grandchild,
	     PaternalAvuncular, MaternalAvuncular, Nibling, Cousin, InLaw,

	     // Social Bonds
	     Peer, Acquaintance, Friend, CloseFriend, BestFriend,
	     Ally, Rival, Enemy, Nemesis,
	     Benefactor, Dependent, Patron, Client,

	     // Romantic/Sexual
	     Lover, ExLover, Spouse, ExSpouse,
	     Courting, Betrothed, Concubine, Paramour,
	     Incompatible, ForbiddenAttraction,

	     // Mentorship
	     Mentor, Mentee, Apprentice, Master,
	     Teacher, Student, Guru, Disciple,

	     // Leadership
	     Leader, Follower, Subordinate, Superior,
	     Chieftain, Tribesperson, Elder, Youth,
	     RoleModel, Idolized,

	     // Task/Professional
	     WorkPartner, TeamMember, TaskLeader,
	     Specialist, Generalist, Consultant,

	     // Ceremonial
	     RitualKin, BloodBrother, OathSworn,
	     CeremonialParent, InitiateGuide,

	     // Asymmetric Relationships
	     Protector, Protected,
	     Caregiver, CareReceiver,
	     Debtor, Creditor,
	     Hostage, Captor

}
//secret data structure
public class PepoSecretData
{
	     public string secret; //ie the secret itself
		            //who did the thing in the secret ie it's my ssecret that i cheated on my partner
	     public List<PepoData> owner;
	     //who is the secret being kept from. ie my partner
	     public List<PepoData> about = new List<PepoData>();

	     //people the secret was shared with
	     public List<PepoData> sharedWith = new List<PepoData>();
	     //people who know the secret
	     public List<PepoData> knowsSecret = new List<PepoData>();

	     public PepoSecretData(string sec, PepoData own, PepoData abt, float trust)
	     {
		          secret = sec;
	     }
}
//promise data structure
public class PepoPromiseData
{
	     public string promise; //ie the promise itself
	     public PepoData promiser; //who made the promise
	     public PepoData promisee; //who the promise was made to
	     public float timeMade; //when the promise was made
	     public float deadline; //when the promise is due
	     public bool isFulfilled = false; //whether the promise has been fulfilled
	     public PepoPromiseData(string prom, PepoData promi, PepoData promie, float time, float due)
	     {
		          promise = prom;
		          promiser = promi;
		          promisee = promie;
		          timeMade = time;
		          deadline = due;
	     }
}
