using System.Collections.Generic;

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
				UnityEngine.Debug.LogWarning("giving default relationship name");
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

}
