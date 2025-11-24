using System.Linq;
using UnityEngine;
public static class PepoUtilitiesMatchmaking
{
	     public static bool CheckRomanticAttraction(PepoData me, PepoData target, float degree)
	     {
		          if (degree >= 3)
		          {
				bool incompatibleRomance = (!me.ssd.romanceFemme && (target.ssd.presentsFemme || target.ssd.isFemme)) ||
					           (!me.ssd.romanceMasc && (target.ssd.presentsMasc || target.ssd.isMasc));

				if (incompatibleRomance && !PepoUtilitiesPersonality.IsLying(target))
				{
					     Debug.LogWarning($"[Relationship Manager]{me.fullname} isn't romantically attracted to {target.fullname}");
					     PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
					     return false;
				}
		          }
		          return true;
	     }

	     public static bool CheckSexualCompatibility(PepoData me, PepoData target, float degree)
	     {
		          if (degree >= 4 && !me.ssd.isAsexual && (target.ssd.isAsexual || target.ssd.isAsexual) && !PepoUtilitiesPersonality.IsLying(target))
		          {
				Debug.LogWarning($"[Relationship Manager]{me.fullname} and {target.fullname} don't want the same things sexually");
				PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
				return false;
		          }
		          return true;
	     }
	     public static bool CheckMonogamyCompatibility(PepoData me, PepoData target, float degree)
	     {
		          if (degree >= 8 && me.ssd.isMonogamous && (target.lovers != 0 || (target.lovers != 0 && !PepoUtilitiesPersonality.IsLying(target))))
		          {
				Debug.LogWarning($"[Relationship Manager]{me.fullname} wants a monogamous relationship but {target.fullname} prefers to keep things open");
				PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
				return false;
		          }

		          // Check monogamy compatibility
		          if (me.ssd.isMonogamous && target.lovers > 0)
		          {
				Debug.Log($"[Biological] Monogamy mismatch");
				return false;
		          }
		          return true;
	     }

	     public static bool CheckChildrenCompatibility(PepoData me, PepoData target, float degree)
	     {
		          if (degree >= 9)
		          {
				int iWant = me.ssd.wantsChildren[1] - (me.knownOffspring?.Count(o => o != null && o.isAlive) ?? 0);
				int theyWant = target.ssd.wantsChildren[0] - (target.knownOffspring?.Count(o => o != null && o.isAlive) ?? 0);
				// Check if both want children to similar degree
				if (iWant > theyWant && !PepoUtilitiesPersonality.IsLying(target))
				{
					     Debug.LogWarning($"[Relationship Manager]{me.fullname} wants more children than {target.fullname}");
					     PepoFactoryBonds.ParasocialRelationship(me, target, RelationshipType.incompatible, "strictly platonic acquaintence");
					     return false;
				}
		          }
		          return true;
	     }

	     //take two pepos and compare their bodies to see how similar their physical appearance is
	     public static float ComparePhysicalAppearance(PepoData p1, PepoData p2)
	     {
		          float similarityScore = 0f;
		          // Compare body size
		          float heightDifference = Mathf.Abs(p1.height / 180 - p2.height / 180);
		          float massGeneticsDifference = Mathf.Abs(p1.massGenetics / 70 - p2.massGenetics / 70);

		          //go through the expressed chromosome traits and compare them. return 1 if they are the same, 0 if they are completely different
		          int i = 0;
		          while (i < p1.body.expressedChromosomes.Count && i < p2.body.expressedChromosomes.Count)
		          {

				if (p1.body.expressedChromosomes[i] == p2.body.expressedChromosomes[i])
				{
					     similarityScore += 1f;
				}
				i++;

		          }

		          return similarityScore / p1.body.expressedChromosomes.Count;
	     }

}
