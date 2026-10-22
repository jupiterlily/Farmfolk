using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PepoFactory
{
	     // ---- Shared helpers ---------------------------------------------------

	     /// <summary>
	     /// Structural clone of a BodyData: shares the allele objects (immutable
	     /// after mutation) but preserves the donor's expressed chromosomes and
	     /// appearance so the clone looks identical to the source.
	     /// </summary>
	     private static BodyData CloneBody(BodyData src)
	     {
		          if (src == null)
				return null;

		          return new BodyData(src.matChromosomes, src.patChromosomes);

	     }

	     private static void RandomiseMood(Mood mood, float ceiling)
	     {
		          foreach (var emotion in mood.FullHexRange())
				emotion.value = Random.Range(0f, ceiling);
	     }

	     /// <summary>
	     /// Everything a PepoData needs before it can be safely observed by the rest
	     /// of the game: an empty relationship library, needs, and a fresh mood.
	     /// </summary>
	     private static void WireUp(PepoData pepo)
	     {
		          InitialiseRelationshipLibrary(pepo);
		          pepo.needs = new MaslowHeirachy(pepo);
		          pepo.mood = new Mood();
	     }

	     // ---- Public constructors ---------------------------------------------

	     public static PepoData ClonePepo(PepoData donor, bool newborn = false)
	     {
		          var clone = new PepoData
		          {
				body = CloneBody(donor.body),
				ssd = new SocioSexualData(donor.body),
				personality = donor.personality,  // see note about HEXACO copy ctor
				massGenetics = donor.massGenetics,
				height = donor.height,
				mass = donor.mass,
				age = newborn ? 0.0001f : donor.age,
		          };

		          clone.pepoName[0].value = donor.forename;
		          clone.pepoName[1].value = donor.surname;

		          WireUp(clone);
		          RandomiseMood(clone.mood, 10f);
		          return clone;
	     }

	     public static PepoData InitialiseFixed(
	         float ageYears, float weightKg, float htMeters,
	         BodyData body = null,
	         SocioSexualData sosex = null,
	         PepoData donor = null)
	     {
		          var pepo = new PepoData
		          {
				body = body != null ? CloneBody(body) : new BodyData(donor.body.matChromosomes, donor.body.patChromosomes)
		          };

		          // donor hook is currently unused (was an empty TODO branch in the
		          // original). Populate it if you want fixed init from a template.
		          _ = donor;

		          pepo.ssd = sosex ?? new SocioSexualData(pepo.body);
		          pepo.age = ageYears;
		          pepo.massGenetics = weightKg;
		          pepo.height = htMeters;

		          WireUp(pepo);
		          RandomiseMood(pepo.mood, 10f);
		          return pepo;
	     }

	     public static PepoData CreateRandom(ChromosomeData[] genome)
	     {
		          var pepo = new PepoData
		          {
				body = new(GeneticsUtilities.Mutate(genome, 0.55f),
           GeneticsUtilities.Mutate(genome, 0.55f))
		          };

		          pepo.ssd = new SocioSexualData(pepo.body);
		          pepo.age = Random.Range(16f, 50f);
		          pepo.massGenetics = pepo.massGenetics * Random.Range(0.6f, 2f);
		          pepo.mass = pepo.massGenetics;
		          pepo.height = pepo.height * Random.Range(0.8f, 1.2f);

		          if (BodyUtilities.GetExpressedTrait("SexChromosome", pepo.body) == 1) // XX
				pepo.womb = new List<PregnancyData>();


		          pepo.mother = pepo;
		          pepo.father = pepo;

		          float footprint = Mathf.Max(1f, (pepo.mass + pepo.age) / 2f);
		          pepo.homeLocation = new Vector3(Random.Range(0f, footprint), Random.Range(0f, footprint), 0f);
		          pepo.currentLocation = pepo.homeLocation;

		          WireUp(pepo);

		          pepo.pepoName[0].value = PepoUtilitiesNames.InitialiseForname(pepo);
		          pepo.pepoName[1].value = PepoUtilitiesNames.InitialiseSurname();
		          pepo.personality = new HEXACO();

		          if (pepo.age > 19)
				PepoSystemAgeing.InitialisePuberty(19, pepo);

		          RandomiseMood(pepo.mood, pepo.personality.Emotionality.value);
		          PepoLogUtilities.LogPepo(pepo);
		          return pepo;
	     }

	     public static PepoData InitialiseFromParents(PepoData mat, PepoData pat)
	     {
		          var bairn = new PepoData
		          {
				age = 0.0001f,
				massGenetics = (mat.massGenetics + pat.massGenetics) / 2f * Random.Range(0.9f, 1.1f),
				height = (mat.height + pat.height) / 2f * Random.Range(0.95f, 1.05f),
				body = BodyFactory.CombineGeneticCode(mat.body, pat.body),
				personality = new HEXACO(),
				mother = mat,
				father = pat,
		          };
		          bairn.mass = bairn.massGenetics * 0.03428f; // newborn ≈ 3.43% of adult mass
		          bairn.ssd = new SocioSexualData(bairn.body) { isPrepubescent = true };

		          WireUp(bairn);

		          // Register as offspring of both parents. NOTE: field name here was
		          // `offspring` in the original; UI code uses `knownOffspring`. Check
		          // whether these are two different collections or a rename miss.
		          mat.knownOffspring?.Add(bairn);
		          pat.knownOffspring?.Add(bairn);

		          // Name: exclude siblings and aunts/uncles.
		          var excludedNames = new HashSet<string>();
		          if (mat.knownOffspring != null)
				foreach (var s in mat.knownOffspring)
					     if (s != bairn)
						          excludedNames.Add(s.forename);
		          if (pat.knownOffspring != null)
				foreach (var s in pat.knownOffspring)
					     if (s != bairn)
						          excludedNames.Add(s.forename);
		          if (mat.mother?.knownOffspring != null)
				foreach (var a in mat.mother.knownOffspring)
					     if (a != mat)
						          excludedNames.Add(a.forename);
		          if (pat.mother?.knownOffspring != null)
				foreach (var a in pat.mother.knownOffspring)
					     if (a != pat)
						          excludedNames.Add(a.forename);

		          bairn.pepoName[0].value = PepoUtilitiesNames.InitialiseForname(bairn, excludedNames.ToList());
		          bairn.pepoName[1].value = mat.surname;

		          bairn.currentLocation = mat.currentLocation;
		          bairn.homeLocation = mat.homeLocation;

		          bairn.values = PepoFactoryValues.IntegrateValues(bairn.values, PepoFactoryValues.ValuesFromHexaco(bairn.personality));
		          bairn.values = PepoFactoryValues.IntegrateValues(bairn.values, PepoFactoryValues.ValuesFromHexaco(bairn.mother.personality));
		          bairn.values = PepoFactoryValues.IntegrateValues(bairn.values, bairn.mother.values);

		          RandomiseMood(bairn.mood, bairn.personality.Emotionality.value);
		          return bairn;
	     }

	     // ---- Relationship scaffolding ----------------------------------------

	     public static void InitialiseRelationshipLibrary(PepoData pepo)
	     {
		          foreach (RelationshipType rt in System.Enum.GetValues(typeof(RelationshipType)))
				pepo.RelationshipLibrary[rt] = new List<PepoRelationshipData>();
	     }
}