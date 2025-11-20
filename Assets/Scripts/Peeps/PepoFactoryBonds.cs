using System.Collections.Generic;

public static class PepoFactoryBonds
{
	     public static void MutualRelationship(PepoData me, PepoData target, RelationshipType type, string name = null)
	     {
		          me.RelationshipLibrary = PepoUtilitiesBonds.AddRelation(me, target, type, name);
		          target.RelationshipLibrary = PepoUtilitiesBonds.AddRelation(target, me, type, name);
	     }
	     public static void AsymetricalRelationship(PepoData me, RelationshipType myType, PepoData target, RelationshipType theirType, string myName = null, string theirName = null)
	     {
		          me.RelationshipLibrary = PepoUtilitiesBonds.AddRelation(me, target, myType, myName);
		          target.RelationshipLibrary = PepoUtilitiesBonds.AddRelation(target, me, theirType, theirName);
	     }
	     public static void ParasocialRelationship(PepoData me, PepoData target, RelationshipType type, string name = null)
	     {
		          me.RelationshipLibrary = PepoUtilitiesBonds.AddRelation(me, target, type, name);
	     }
	     public static void RethinkRelationship(PepoData me, PepoData target, RelationshipType newType, RelationshipType oldType, string name = null)
	     {
		          me.RelationshipLibrary = PepoUtilitiesBonds.AddRelation(me, target, newType, name);
		          me.RelationshipLibrary = PepoUtilitiesBonds.RemoveRelation(me, target, oldType);
	     }
	     public static void MutualRethink(PepoData me, PepoData target, RelationshipType newType, RelationshipType oldType, string myName = null, string theirName = null)
	     {
		          me.RelationshipLibrary = PepoUtilitiesBonds.AddRelation(me, target, newType, myName);
		          me.RelationshipLibrary = PepoUtilitiesBonds.RemoveRelation(me, target, oldType);
		          target.RelationshipLibrary = PepoUtilitiesBonds.AddRelation(target, me, newType, theirName);
		          target.RelationshipLibrary = PepoUtilitiesBonds.RemoveRelation(target, me, oldType);
	     }
	     public static void CreateParentalBond(PepoData parent, PepoData child)
	     {
		          string name = "parent";
		          if ((parent.ssd.isFemme || parent.ssd.presentsFemme) && !parent.ssd.isMasc)
		          {
				name = "mother";
		          }
		          else if ((parent.ssd.isMasc || parent.ssd.presentsMasc) && !parent.ssd.isFemme)
		          {
				name = "father";
		          }
		          PepoFactoryBonds.AsymetricalRelationship(parent, RelationshipType.parent, child, RelationshipType.offspring, name, "child");
		          // Find the child in the parent's offspring relationship list and set obligation to 10
		          if (parent.RelationshipLibrary.TryGetValue(RelationshipType.offspring, out List<PepoRelationshipData> offspringList))
		          {
				PepoRelationshipData childRelationship = offspringList.Find(rel => rel.pepo == child);
				if (childRelationship != null)
				{
					     childRelationship.Obligation = 10f;
				}
		          }
	     }
	     public static void CreateNuclearlBond(PepoData birthParent, PepoData secondaryParent, PepoData child)
	     {
		          CreateParentalBond(birthParent, child);
		          CreateParentalBond(secondaryParent, child);
		          MutualRelationship(secondaryParent, birthParent, RelationshipType.spouse);
		          foreach (var sib in birthParent.offspring)
		          {
				CreateSiblingBond(sib, child);
		          }
		          foreach (var sib in secondaryParent.offspring)
		          {
				CreateSiblingBond(sib, child);
		          }
	     }
	     public static void CreateSiblingBond(PepoData me, PepoData target, string prefix = null)
	     {
		          string name = "sibling";
		          string femme = "sister";
		          string masc = "brother";
		          string age = string.Empty;
		          if (me.mother != target.mother || me.father != target.father)
		          {
				prefix = "half ";
		          }
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
		          me.RelationshipLibrary = PepoUtilitiesBonds.AddRelation(me, target, RelationshipType.kin, name);
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
		          if (me.age > target.age)
		          {
				age = "younger ";
		          }
		          else if (me.age < target.age)
		          {
				age = "older ";
		          }
		          else if (me.age == target.age)
		          {
				age = "twin ";
		          }
		          if (prefix != null)
		          {
				name = string.Join(age, prefix, name);
		          }
		          target.RelationshipLibrary = PepoUtilitiesBonds.AddRelation(target, me, RelationshipType.kin, name);
	     }
	     public static void CreateCasualBond(PepoData me, PepoData target)
	     {
		          string name = "stranger";
		          me.RelationshipLibrary = PepoUtilitiesBonds.AddRelation(me, target, RelationshipType.acquaintence, name);
		          target.RelationshipLibrary = PepoUtilitiesBonds.AddRelation(target, me, RelationshipType.acquaintence, name);
	     }
	     public static void CreateFriendshipBond(PepoData me, PepoData target)
	     {
		          string name = "friend";
		          me.RelationshipLibrary = PepoUtilitiesBonds.AddRelation(me, target, RelationshipType.friend, name);
		          me.RelationshipLibrary = PepoUtilitiesBonds.RemoveRelation(me, target, RelationshipType.acquaintence);
	     }
}
