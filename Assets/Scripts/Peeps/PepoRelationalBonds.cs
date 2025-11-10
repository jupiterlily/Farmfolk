public static class PepoRelationalBonds
{

	     public static void MutualRelationship(PepoData me, PepoData target, RelationshipType type, string name = null)
	     {
		          me.RelationshipLibrary = PepoRelationalUtilities.AddRelation(me, target, type, name);
		          target.RelationshipLibrary = PepoRelationalUtilities.AddRelation(target, me, type, name);
	     }
	     public static void AsymetricalRelationship(PepoData me, RelationshipType myType, PepoData target, RelationshipType theirType, string myName = null, string theirName = null)
	     {
		          me.RelationshipLibrary = PepoRelationalUtilities.AddRelation(me, target, myType, myName);
		          target.RelationshipLibrary = PepoRelationalUtilities.AddRelation(target, me, theirType, theirName);
	     }
	     public static void ParasocialRelationship(PepoData me, PepoData target, RelationshipType type, string name = null)
	     {
		          me.RelationshipLibrary = PepoRelationalUtilities.AddRelation(me, target, type, name);
	     }
	     public static void RethinkRelationship(PepoData me, PepoData target, RelationshipType newType, RelationshipType oldType, string name = null)
	     {
		          me.RelationshipLibrary = PepoRelationalUtilities.AddRelation(me, target, newType, name);
		          me.RelationshipLibrary = PepoRelationalUtilities.RemoveRelation(me, target, oldType);
	     }
	     public static void MutualRethink(PepoData me, PepoData target, RelationshipType newType, RelationshipType oldType, string myName = null, string theirName = null)
	     {
		          me.RelationshipLibrary = PepoRelationalUtilities.AddRelation(me, target, newType, myName);
		          me.RelationshipLibrary = PepoRelationalUtilities.RemoveRelation(me, target, oldType);
		          target.RelationshipLibrary = PepoRelationalUtilities.AddRelation(target, me, newType, theirName);
		          target.RelationshipLibrary = PepoRelationalUtilities.RemoveRelation(target, me, oldType);
	     }

	     public static void CreateParentalBond(PepoData parent, PepoData child)
	     {
		          string name = "parent";
		          if (parent.ssd.isFemme)
		          {
				name = "mother";
		          }
		          else
		          {
				name = "father";
		          }
		          PepoRelationalBonds.AsymetricalRelationship(parent, RelationshipType.parent, child, RelationshipType.offspring, name, "child");
	     }
	     public static void CreateSiblingBond(PepoData me, PepoData target, string prefix = null)
	     {
		          string name = "sibling";
		          string femme = "sister";
		          string masc = "brother";
		          if (target.ssd.isFemme && !target.ssd.isMasc)
		          {
				name = femme;
		          }
		          else if (target.ssd.isMasc && !target.ssd.isFemme)
		          {
				name = masc;
		          }
		          if (prefix != null)
		          {
				name = string.Join(prefix, name);
		          }
		          me.RelationshipLibrary = PepoRelationalUtilities.AddRelation(me, target, RelationshipType.kin, name);
		          if (me.ssd.isFemme && !me.ssd.isMasc)
		          {
				name = femme;
		          }
		          else if (me.ssd.isMasc && !me.ssd.isFemme)
		          {
				name = masc;
		          }
		          else
		          {
				name = "sibling";
		          }
		          if (prefix != null)
		          {
				name = string.Join(prefix, name);
		          }
		          target.RelationshipLibrary = PepoRelationalUtilities.AddRelation(target, me, RelationshipType.kin, name);
	     }
	     public static void CreateCasualBond(PepoData me, PepoData target)
	     {
		          string name = "stranger";
		          me.RelationshipLibrary = PepoRelationalUtilities.AddRelation(me, target, RelationshipType.acquaintence, name);
		          target.RelationshipLibrary = PepoRelationalUtilities.AddRelation(target, me, RelationshipType.acquaintence, name);
	     }
	     public static void CreateFriendshipBond(PepoData me, PepoData target)
	     {
		          string name = "friend";
		          me.RelationshipLibrary = PepoRelationalUtilities.AddRelation(me, target, RelationshipType.friend, name);
		          me.RelationshipLibrary = PepoRelationalUtilities.RemoveRelation(me, target, RelationshipType.acquaintence);
	     }


}
