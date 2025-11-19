using System.Collections.Generic;
using UnityEngine;

public class PepoResource : PepoAspect
{                   //can be anything, physical, social, emotional
	     //food	     
	     public string category;
}
public class PhysicalResource : PepoResource
{
	     public Vector3 location; //coordinates in lat, long, alt
	     public float mass; //mass per unit
	     public float amount;
	     public string units; //unit for value
	     public PhysicalResource(string name, float amount, float value, Vector3 location, string description)
	     {
		          this.name = name;
		          this.value = value;
		          this.amount = amount;
		          this.location = location;
		          this.descriptor = description;
	     }
}
public class PepoState
{
	     //moral weight, emotional impact, value change, need change, resource change
	     public string name;
	     public float targetValue;
	     public bool exceedValue;
	     public bool reduceValue;
	     public bool exactValue;
}

public class ResourceState : PepoState
{
	     public string resourceName;
	     public ResourceState(string resourceName, float amount, bool above = true, bool exact = false)
	     {
		          if (above && !exact)
		          {
				this.name = new string($"more than {amount} {resourceName}");
				this.targetValue = amount;
				this.exceedValue = above;
				this.exactValue = exact;
				this.reduceValue = !above;
		          }
		          if (above && exact)
		          {
				this.name = new string($"at least {amount} {resourceName}");
				this.targetValue = amount;
				this.exceedValue = above;
				this.exactValue = exact;
				this.reduceValue = !above;
		          }
		          if (!above && exact)
		          {
				this.name = new string($"at most {amount} {resourceName}");
				this.targetValue = amount;
				this.exceedValue = above;
				this.exactValue = exact;
				this.reduceValue = !above;
		          }
		          if (!above && !exact)
		          {
				this.name = new string($"fewer than {amount} {resourceName}");
				this.targetValue = amount;
				this.exceedValue = above;
				this.exactValue = exact;
				this.reduceValue = !above;
		          }

	     }
	     public bool CheckCriteria(List<PepoResource> resources)
	     {
		          bool isMet = false;
		          float i = 0; //category counter
		          foreach (var r in resources)
		          {
				if (r.category == resourceName)
				{
					     i += r.value;
				}
				if (r.name == resourceName)
				{
					     if (exceedValue && !exactValue)
					     {
						          if (r.value > targetValue)
						          {
								isMet = true;
								break;
						          }
					     }
					     if (exceedValue && exactValue)
					     {
						          if (r.value >= targetValue)
						          {
								isMet = true;
								break;
						          }
					     }
					     if (reduceValue && exactValue)
					     {
						          if (r.value <= targetValue)
						          {
								isMet = true;
								break;
						          }
					     }
					     if (reduceValue && !exactValue)
					     {
						          if (r.value < targetValue)
						          {
								isMet = true;
								break;
						          }
					     }
				}
		          }
		          if (exceedValue && !exactValue)
		          {
				if (i > targetValue)
				{
					     isMet = true;
					     return isMet;
				}
		          }
		          if (exceedValue && exactValue)
		          {
				if (i >= targetValue)
				{
					     isMet = true;
					     return isMet;
				}
		          }
		          if (reduceValue && exactValue)
		          {
				if (i <= targetValue)
				{
					     isMet = true;
					     return isMet;
				}
		          }
		          if (reduceValue && !exactValue)
		          {
				if (i < targetValue)
				{
					     isMet = true;
					     return isMet;
				}
		          }
		          return isMet;
	     }
}

public class AspectState : PepoState
{
	     public string requiredAspect;
	     public AspectState(string asepectName, float amount, bool above = true, bool exact = false)
	     {
		          if (above && !exact)
		          {
				this.name = new string($"more than {amount} {asepectName}");
				this.targetValue = amount;
				this.exceedValue = above;
				this.exactValue = exact;
				this.reduceValue = !above;
		          }
		          if (above && exact)
		          {
				this.name = new string($"at least {amount} {asepectName}");
				this.targetValue = amount;
				this.exceedValue = above;
				this.exactValue = exact;
				this.reduceValue = !above;
		          }
		          if (!above && exact)
		          {
				this.name = new string($"at most {amount} {asepectName}");
				this.targetValue = amount;
				this.exceedValue = above;
				this.exactValue = exact;
				this.reduceValue = !above;
		          }
		          if (!above && !exact)
		          {
				this.name = new string($"fewer than {amount} {asepectName}");
				this.targetValue = amount;
				this.exceedValue = above;
				this.exactValue = exact;
				this.reduceValue = !above;
		          }

	     }
	     public bool CheckCriteria(PepoAspect aspect)
	     {
		          bool isMet = false;
		          if (aspect.descriptor == requiredAspect)
		          {
				if (exceedValue && !exactValue)
				{
					     if (aspect.value > targetValue)
					     {
						          isMet = true;
						          return isMet;
					     }
				}
				if (exceedValue && exactValue)
				{
					     if (aspect.value >= targetValue)
					     {
						          isMet = true;
						          return isMet;
					     }
				}
				if (reduceValue && exactValue)
				{
					     if (aspect.value <= targetValue)
					     {
						          isMet = true;
						          return isMet;
					     }
				}
				if (reduceValue && !exactValue)
				{
					     if (aspect.value < targetValue)
					     {
						          isMet = true;
						          return isMet;
					     }
				}
		          }
		          return isMet;

	     }
}
public class PepoTask
{
	     //a series of actions and acondition for success
	     public List<PepoState> precondition;
	     public List<PepoState> objectives;
	     public List<PepoState> consequences;
	     //get 10 food, get food for 10 minutes, eat all the foor, eat enough food, eat food for 10 minutes
}
//run actions based on conscientiousness
public class PepoAction
{
	     //eat, talk, ask, take	     
	     public PepoData actor; //actor
	     public Verb action; //verb
	     public List<ActionType> actions;
	     public List<PepoState> precondition;
	     public List<PepoState> objectives;
	     public List<PepoState> consequences;
}
public class PhysicalAction : PepoAction
{

}
public class MentalAction : PepoAction
{

}
//run social actions based on extraversion
public class SocialAction : PepoAction
{

}
public class VerbalInteraction : SocialAction
{

}
public class NonVerbalInteraction : SocialAction
{

}
public class GroupSocialAction : SocialAction
{

}

public class PepoGoal
{
	     //a series of tasks, criteria for success, a timeframe
}
public enum ActionType
{
	     // Basic Physiological Needs
	     Eat, Drink, Forage, Hunt, Cook, StoreFood, ShareFood, WasteFood,
	     FindWater, CollectWater, PurifyWater, StoreWater, ShareWater, WasteWater,
	     FindShelter, BuildShelter, MaintainShelter, ImproveShelter, ShareShelter, AbandonShelter,
	     Sleep, Rest, Nap, WakeUp, FindRestingPlace, ShareRestingPlace,
	     FindToilet, UseToilet, HoldUrine, ShareToilet, WasteUrine,
	     Bathe, CleanSelf, Groom, Dress, Undress,

	     // Safety & Security
	     SeekSafety, SecureArea, DefendSelf, DefendOthers, AlertDanger, WarnOthers,
	     Hide, Escape, Evade, Surrender, Submit,
	     FindResources, GatherResources, StoreResources, ProtectResources, ShareResources, WasteResources,
	     Plan, Prepare, Organize, Systematize,

	     // Social Connection & Belonging
	     Socialize, MeetNewPeople, Introduce, Welcome, Include,
	     JoinGroup, LeaveGroup, FormGroup, DissolveGroup,
	     HostEvent, AttendEvent, Participate, Observe,
	     Communicate, Listen, Speak, ShareStory, ShareExperience,
	     Bond, Connect, Reconnect, DriftApart, Separate,

	     // Intimacy & Relationships
	     Flirt, Date, Court, Woo, Seduce, Reject, BeAffectionate, WithholdAffection,
	     HaveSex, RefuseSex, InitiateIntimacy, AvoidIntimacy,
	     Commit, BreakCommitment, Propose, AcceptProposal, RejectProposal,
	     Marry, Divorce, Reconcile,
	     ShareSecrets, KeepConfidence, BetrayConfidence,

	     // Esteem & Recognition
	     AchieveGoal, FailGoal, SetGoal, AbandonGoal,
	     AcceptPraise, GivePraise, SeekRecognition, AvoidSpotlight,
	     BuildConfidence, Encourage, Discourage, Undermine,
	     Compete, Cooperate, Collaborate,
	     Defer, ChallengeAuthority,

	     // Cognitive & Learning
	     Learn, Study, Practice, Master, Teach, Mentor, Tutor,
	     SolveProblem, Analyze, Synthesize, CreateSolution,
	     Explore, Discover, Investigate, Research,
	     Innovate, Invent, Improve, Optimize,
	     Question, Criticize, Debate, Argue, Persuade, Convince,

	     // Aesthetic & Creative
	     AppreciateArt, CreateArt, Perform, DisplayArt, CritiqueArt,
	     VisitBeautifulPlace, CreateBeauty, MaintainBeauty, DestroyBeauty,
	     ComposeMusic, PlayMusic, Sing, Dance,
	     Write, Read, Recite, TellStory,

	     // Self-Actualization
	     PursuePassion, DevelopTalent, ExpressSelf, Authenticate,
	     OvercomeChallenge, Persevere, GiveUp,
	     Reflect, SelfExamine, Grow, Transform,
	     FindMeaning, Purpose, QuestionExistence,

	     // Transcendence & Spirituality
	     HelpOthers, Volunteer, BeSelfish,
	     Guide, Counsel, Advise, Misguide,
	     ShareWisdom, Inspire, Motivate, Demotivate,
	     Worship, Pray, Meditate, PracticeRitual, BreakTaboo,
	     ConnectWithNature, AppreciateUniverse, FeelAwe,

	     // Moral Foundations - Care/Harm
	     CareFor, Nurture, Protect, Heal, Comfort, Soothe,
	     Neglect, Abandon, Harm, Hurt, Injure, Torture, Bully,

	     // Fairness/Cheating
	     Share, Distribute, Allocate, Reciprocate, ReturnFavor,
	     Request, Ask, Beg, Demand,
	     BeHonest, Truth, Deceive, Lie, Cheat, Steal,
	     KeepPromise, BreakPromise, MakeVow, BreakVow,
	     Judge, Assess, Evaluate, Prejudice,

	     // Loyalty/Betrayal
	     SupportGroup, DefendGroup, PromoteGroup, Exclude, Ostracize,
	     Betray, Sabotage,
	     Unite, Divide, Integrate, Segregate,
	     CelebrateGroup, MournGroup, HonorGroup, DishonorGroup,

	     // Authority/Subversion
	     Respect, Honor, Obey,
	     Disobey, Rebel, Challenge, Disrespect, Insult,
	     Lead, Command, Govern, Rule,
	     Follow, Serve, Resist, Overthrow,

	     // Sanctity/Degradation
	     Purify, Clean, Sanctify, Consecrate,
	     Defile, Desecrate, Pollute, Corrupt,
	     HonorTradition, BreakTradition, Conserve,
	     MockRitual, Celebrate,

	     // Liberty/Oppression
	     Free, Liberate, Empower, Enable, Emancipate,
	     Oppress, Restrict, Control, Dominate, Enslave,
	     ProtectRights, ViolateRights, Advocate, Suppress,
	     Choose, Decide, Consent, Coerce, Force,

	     // HEXACO Personality-Based Actions
	     // Honesty-Humility
	     BeModest, Boast, BeHumble, BeArrogant,
	     BeSincere, Manipulate, BeGenuine, BeDeceptive,
	     AvoidGreed, BeGenerous, Hoard, ShareWealth,

	     // Emotionality
	     ExpressFear, HideFear, ExpressAnxiety, CalmAnxiety,
	     ExpressDependence, BeIndependent, SeekSupport, OfferSupport,
	     ExpressSentimentality, BeDetached, Nostalgize, MoveOn,

	     // Extraversion
	     Withdraw, ExpressJoy, ExpressSadness,
	     BeAssertive, BePassive, TakeCharge, FollowLead,
	     SeekExcitement, AvoidRisk, Adventure, PlaySafe,

	     // Agreeableness
	     Forgive, HoldGrudge, Compromise, BeStubborn,
	     BePatient, BeImpatient, Tolerate,
	     BeFlexible, BeRigid, Adapt, ResistChange,

	     // Conscientiousness
	     Disorganize, Improvise,
	     WorkHard, BeLazy, Quit,
	     BeThorough, BeCareless, Detail, Overlook,

	     // Openness to Experience
	     BeCreative, BeConventional, Maintain,
	     BeCurious, BeIndifferent, Stay,
	     IgnoreArt, SeekBeauty, IgnoreBeauty,

	     // Plutchik's Emotions - Joy
	     Rejoice, Enjoy, Delight, Amuse, Entertain,
	     Laugh, Smile, Play,

	     // Sadness
	     Grieve, Mourn, Cry, Lament, Isolate,
	     Console, Support, Empathize,

	     // Trust
	     Trust, Distrust, Believe, Doubt, HaveFaith,
	     RelyOn, DependOn,

	     // Disgust
	     Avoid, Spurn, Condemn,
	     Disinfect, Contaminate,

	     // Anger
	     Fight, Attack, Defend, Confront,
	     ExpressAnger, ControlAnger, Vent,
	     Retaliate, SeekRevenge, Absolve,

	     // Fear
	     Fear, Anxiety, Worry, Panic, Calm, Reassure,
	     Guard, Secure, Threaten, Intimidate,

	     // Anticipation
	     Expect, Hope, Dread, Wait,
	     Predict, Forecast, Speculate, Gamble,

	     // Surprise
	     Surprise, Astonish, Amaze, Shock, Startle,
	     Reveal, Conceal,

	     // Color Wheel Theory of Love
	     // Eros (Passionate Love)
	     PassionatelyLove, DesireIntensely, Romance,
	     ExpressPhysicalAffection, ShareIntimacy,

	     // Ludus (Game-Playing Love)
	     PlayGames, FlirtCasually, BeNonCommittal, DateMultiple,
	     KeepOptionsOpen, AvoidAttachment,

	     // Storge (Friendship Love)
	     DevelopFriendship, BuildCompanionship, ShareInterests,
	     GrowTogether, MaintainFriendship,

	     // Pragma (Practical Love)
	     AssessCompatibility, ConsiderPracticalities, MakePracticalChoice,
	     BuildStableRelationship, PlanFuture,

	     // Mania (Possessive Love)
	     Obsess, BePossessive, BeJealous, ControlPartner,
	     DemandAttention, BeNeedy,

	     // Agape (Selfless Love)
	     LoveSelflessly, Sacrifice, GiveUnconditionally,
	     SupportUnconditionally, ForgiveGenerously,

	     // Relationship Maintenance Actions
	     CheckIn, Update, ShareNews, CelebrateMilestone,
	     ResolveConflict, Apologize,
	     SetBoundary, RespectBoundary, ViolateBoundary,
	     InvestTime, Prioritize, Deprioritize,

	     // Emergent Behavior Enablers
	     Imitate, Copy, FollowTrend, SetTrend, RebelAgainstTrend,
	     Conform, NonConform, Traditionalize,
	     SpreadIdea, SuppressIdea, Dissuade,
	     FormMovement, JoinMovement, LeaveMovement, OpposeMovement,
	     CreateCulture, MaintainCulture, ChangeCulture, RejectCulture
}