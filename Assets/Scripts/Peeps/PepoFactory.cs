
using System.Collections.Generic;
using UnityEngine;

public static class PepoFactory
{

	     public static PepoData ClonePepo(PepoData donor, bool newborn = false)
	     {
		          PepoData clone = new PepoData() { };
		          clone.forename = donor.forename;
		          clone.surname = donor.surname;
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

		          pepo.forename = PepoUtilitiesNames.InitialiseForname(pepo);
		          pepo.surname = PepoUtilitiesNames.InitialiseSurname();
		          pepo.personality = new HEXACO();
		          if (pepo.age > 25)
		          {
				PepoSystemAgeing.InitialisePuberty(25, pepo);
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
		          PepoUtilitiesNames.InitialiseForname(bairn);
		          bairn.surname = mat.surname; //pat's surname
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
		          pepo.RelationshipLibrary.Add(RelationshipType.ex, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.offspring, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.parent, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.kin, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.incompatible, new List<PepoRelationshipData>());
		          pepo.RelationshipLibrary.Add(RelationshipType.friend, new List<PepoRelationshipData>());
	     }
}
