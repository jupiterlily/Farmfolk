
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PepoFactory
{

	     public static PepoData ClonePepo(PepoData donor, bool newborn = false)
	     {
		          PepoData clone = new PepoData() { };

		          clone.pepoName = donor.pepoName;
		          clone.massGenetics = donor.massGenetics;
		          clone.height = donor.height;
		          clone.ssd = donor.ssd;
		          InitialiseRelationshipLibrary(clone);
		          clone.ssd = donor.ssd;
		          clone.personality = donor.personality;
		          clone.needs = new MaslowHeirachy(clone);
		          clone.age = donor.age;
		          clone.mass = donor.mass;
		          clone.mood = new Mood();
		          clone.body = new BodyData(clone.body.maternalChromosome, clone.body.paternalChromosome);
		          clone.body.expressedChromosomes = donor.body.expressedChromosomes;
		          foreach (var emotion in clone.mood.FullHexRange())
		          {
				emotion.value = Random.Range(0, 10f);
		          }
		          if (newborn)
		          {
				clone.age = 0.0001f;
		          }
		          return clone;
	     }
	     public static PepoData InitialiseFixed(float ageYears, float weightKg, float htMeters, BodyData body = null, SocioSexualData sosex = null, PepoData donor = null)
	     {
		          PepoData pepo = new PepoData();

		          if (donor == null)
		          {
				//here we copy the values from the donor to our own pepodata

		          }
		          if (sosex == null)
		          {
				sosex = new SocioSexualData(pepo.body);
		          }
		          if (body == null)
		          {
				body = BodyFactory.CreateRandomIndividual();
		          }
		          else
		          {
				pepo.body = new BodyData(body.maternalChromosome, body.paternalChromosome);
		          }
		          pepo.age = ageYears;
		          pepo.massGenetics = weightKg;
		          pepo.height = htMeters;
		          pepo.ssd = sosex;
		          InitialiseRelationshipLibrary(pepo);
		          pepo.needs = new MaslowHeirachy(pepo);
		          pepo.mood = new Mood();
		          foreach (var emotion in pepo.mood.FullHexRange())
		          {
				emotion.value = Random.Range(0, 10f);
		          }
		          return pepo;
	     }
	     public static PepoData InitialiseRandom()
	     {
		          PepoData pepo = new PepoData();
		          pepo.age = Random.Range(16f, 50f);
		          pepo.massGenetics = pepo.massGenetics * Random.Range(0.6f, 2f);
		          pepo.mass = pepo.massGenetics;//
		          pepo.height = pepo.height * Random.Range(0.8f, 1.2f);
		          pepo.body = BodyFactory.CreateRandomIndividual();
		          pepo.ssd = new SocioSexualData(pepo.body);
		          if (pepo.body.paternalChromosome.sexChromosome != "XY")
		          {
				pepo.womb = new List<PregnancyData>();
		          }
		          pepo.mother = pepo;
		          pepo.father = pepo;
		          pepo.homeLocation = new Vector3(Random.Range(0, (pepo.mass + pepo.age) / 2), Random.Range(0, (pepo.mass + pepo.age) / 2), 0);
		          pepo.currentLocation = pepo.homeLocation;

		          InitialiseRelationshipLibrary(pepo);
		          //
		          pepo.pepoName[0].value = PepoUtilitiesNames.InitialiseForname(pepo);
		          pepo.pepoName[1].value = PepoUtilitiesNames.InitialiseSurname();
		          pepo.personality = new HEXACO();
		          if (pepo.age > 19)
		          {
				PepoSystemAgeing.InitialisePuberty(19, pepo);
		          }
		          pepo.needs = new MaslowHeirachy(pepo);
		          pepo.mood = new Mood();
		          foreach (var emotion in pepo.mood.FullHexRange())
		          {
				emotion.value = Random.Range(0, pepo.personality.Emotionality.value);
		          }
		          PepoLogUtilities.LogPepo(pepo);
		          return pepo;
	     }
	     public static PepoData InitialiseFromParents(PepoData mat, PepoData pat)
	     {
		          PepoData bairn = new PepoData();
		          //Inherit average of parents' traits with some variation
		          bairn.age = 0.0001f; //newborn
		          bairn.massGenetics = (mat.massGenetics + pat.massGenetics) / 2f * Random.Range(0.9f, 1.1f);
		          bairn.mass = bairn.massGenetics * 0.03428f; //newborn mass is about 3.428% of adult mass
		          bairn.height = (mat.height + pat.height) / 2f * Random.Range(0.95f, 1.05f);
		          bairn.body = BodyFactory.CombineGeneticCode(mat.body, pat.body);
		          bairn.ssd = new SocioSexualData(bairn.body);
		          bairn.ssd.isPrepubescent = true;
		          InitialiseRelationshipLibrary(bairn);
		          bairn.personality = new HEXACO();
		          bairn.mother = mat;
		          bairn.father = pat;
		          mat.offspring.Add(bairn);
		          pat.offspring.Add(bairn);
		          //create a list of names from me, my parents, my partner and my other children.
		          List<string> excludedNames = new List<string>();

		          // Siblings
		          excludedNames.AddRange(mat.offspring.Select(s => s.forename));
		          excludedNames.AddRange(pat.offspring.Select(s => s.forename));

		          // Aunts/uncles (excluding parents)
		          if (mat.mother != null)
				excludedNames.AddRange(mat.mother.offspring.Where(a => a != mat).Select(a => a.forename));

		          if (pat.mother != null)
				excludedNames.AddRange(pat.mother.offspring.Where(a => a != pat).Select(a => a.forename));

		          // Remove duplicates if needed
		          excludedNames = excludedNames.Distinct().ToList();
		          PepoUtilitiesNames.InitialiseForname(bairn, excludedNames);
		          bairn.pepoName[1].value = mat.surname; //mothers surname. this should become a method that filters through a pepos values and cluture to return aname they wulod give to a child. default tohaving the mother choose for now but pepos should have cultural rules that determine who chooses the name.
		          bairn.currentLocation = mat.currentLocation;
		          bairn.homeLocation = mat.homeLocation;
		          bairn.needs = new MaslowHeirachy(bairn);
		          bairn.mood = new Mood();
		          bairn.values = PepoFactoryValues.IntegrateValues(bairn.values, PepoFactoryValues.ValuesFromHexaco(bairn.personality));
		          bairn.values = PepoFactoryValues.IntegrateValues(bairn.values, PepoFactoryValues.ValuesFromHexaco(bairn.mother.personality));
		          bairn.values = PepoFactoryValues.IntegrateValues(bairn.values, bairn.mother.values);
		          foreach (var emotion in bairn.mood.FullHexRange())
		          {
				emotion.value = Random.Range(0, bairn.personality.Emotionality.value);
		          }
		          return bairn;
	     }

	     public static void InitialiseRelationshipLibrary(PepoData pepo)
	     {
		          pepo.RelationshipLibrary.Add(RelationshipType.lover, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.spouse, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.ex, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.offspring, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.parent, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.coparent, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.kin, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.incompatible, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.friend, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.bestie, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.acquaintence, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.enemy, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.rival, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.courting, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.mentor, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.mentee, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.leader, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.follower, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.rolemodel, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.peer, new List<PepoRelationshipData>());
	     }
}
