using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BodyUtilities
{
	     public static int GetExpressedTrait(string name, BodyData body) => (int)body.physicalAttributes[name].value;
	     public static string GetExpressedDescriptor(string name, BodyData body) => body.physicalAttributes[name].descriptor;

	     public static int ParentAlleleValue(ChromosomeData[] set, string trait)
	     {
		          foreach (var ch in set)
				foreach (var a in ch.alleles)
					     if (a.range.name == trait)
						          return (int)a.range.value;
		          return -1;
	     }
}

public static class GeneticsUtilities
{
	     // ---- allele level ----
	     /// <summary>
	     /// Returns a MUTATED COPY if the roll succeeds, otherwise the SAME instance.
	     /// Sharing the reference is safe because we never mutate in place —
	     /// so unchanged genes remain identical across every individual carrying them.
	     /// </summary>
	     public static AlleleData Mutate(AlleleData source, float rate)
	     {
		          if (source == null || rate <= 0f)
				return source;
		          if (rate < 1f && Random.value >= rate)
				return source;
		          return ApplyMutation(source);
	     }

	     private static AlleleData ApplyMutation(AlleleData source)
	     {
		          var nr = CloneAspect(source.range);

		          string gene = nr.name;
		          int libMin = 0;
		          int libMax = GeneLibrary.Descriptions[gene].Count - 1;

		          if (Random.value < 0.85f)
		          {
				int mod =
				    nr.value <= nr.minValue ? +1 :
				    nr.value >= nr.maxValue ? -1 :
				    Random.value < 0.5f ? +1 : -1;

				nr.value = Mathf.Clamp(nr.value + mod, nr.minValue, nr.maxValue);
		          }
		          else if (Random.value < 0.5f)
		          {
				int mod =
				    nr.minValue <= libMin ? +1 :
				    nr.minValue >= nr.maxValue - 1 ? -1 :
				    Random.value < 0.5f ? +1 : -1;

				nr.minValue = Mathf.Clamp(nr.minValue + mod, libMin, nr.maxValue - 1);
				if (nr.value < nr.minValue)
					     nr.value = nr.minValue;
		          }
		          else
		          {
				int mod =
				    nr.maxValue >= libMax ? -1 :
				    nr.maxValue <= nr.minValue + 1 ? +1 :
				    Random.value < 0.5f ? +1 : -1;

				nr.maxValue = Mathf.Clamp(nr.maxValue + mod, nr.minValue + 1, libMax);
				if (nr.value > nr.maxValue)
					     nr.value = nr.maxValue;
		          }

		          // descriptor intentionally left stale — it's rebuilt at expression time.
		          return new AlleleData(source.inheritance, nr);
	     }

	     private static AlleleData ExpressAllelePair(AlleleData maternal, AlleleData paternal)
	     {
		          if (maternal == null || paternal == null)
				return null;

		          string gene = maternal.range.name;
		          if (paternal.range.name != gene)
				throw new System.ArgumentException(
				    $"Cannot express non-homologous alleles: '{gene}' vs '{paternal.range.name}'.");

		          if (maternal.inheritance != paternal.inheritance)
				Debug.LogWarning(
				    $"[Expression] '{gene}' has mismatched inheritance rules " +
				    $"({maternal.inheritance} vs {paternal.inheritance}). Using maternal.");

		          int matVal = (int)maternal.range.value;
		          int patVal = (int)paternal.range.value;
		          int lo = Mathf.Min(matVal, patVal);
		          int hi = Mathf.Max(matVal, patVal);

		          int value = maternal.inheritance switch
		          {
				InheritanceRule.EitherOr => Random.value > 0.5f ? matVal : patVal,
				InheritanceRule.Average => Random.Range(lo, hi + 1),
				InheritanceRule.WithinRange => Random.Range(lo, hi + 1),
				InheritanceRule.ChooseDominant => hi,
				InheritanceRule.LeanDominant => Random.value > 0.5f ? hi : Random.Range(lo, hi + 1),
				InheritanceRule.EyeColor => ExpressEyeColour(matVal, patVal), // TODO: real hierarchy
				_ => matVal,
		          };

		          // Expressed allele keeps the union of both parents' allowed ranges so
		          // inspection/mutation of the expressed copy still knows its bounds.
		          int outMin = (int)Mathf.Min(maternal.range.minValue, paternal.range.minValue);
		          int outMax = (int)Mathf.Max(maternal.range.maxValue, paternal.range.maxValue);

		          var na = new NumericalAspect(
		              gene, outMax, outMin, false, 0,
		              AlleleLibrary.Describe(gene, value));
		          na.value = value;

		          return new AlleleData(maternal.inheritance, na);
	     }

	     // Eye colour is a dominance hierarchy, not a numeric comparison.
	     // Placeholder until you define brown > green > blue > etc.
	     private static int ExpressEyeColour(int mat, int pat) => Mathf.Max(mat, pat);

	     public static List<AlleleData> ListAlleles(ChromosomeData[] chromosomes)
	     {
		          if (chromosomes == null)
		          {
				return new List<AlleleData>();
		          }
		          var list = new List<AlleleData>();
		          foreach (var ch in chromosomes)
		          {
				if (ch?.alleles == null)
				{
					     continue;
				}
				list.AddRange(ch.alleles);
		          }
		          return list;
	     }



	     // ---- chromosome level ----
	     public static ChromosomeData Mutate(ChromosomeData source, float ratePerAllele)
	     {
		          if (source?.alleles == null || source.alleles.Length == 0)
				return source;

		          AlleleData[] mutated = null;  // lazy: only allocate if something changed
		          for (int i = 0; i < source.alleles.Length; i++)
		          {
				var m = Mutate(source.alleles[i], ratePerAllele);
				if (ReferenceEquals(m, source.alleles[i]))
					     continue;
				mutated ??= (AlleleData[])source.alleles.Clone();
				mutated[i] = m;
		          }

		          return mutated == null
		              ? source
		              : new ChromosomeData { name = source.name, alleles = mutated };
	     }

	     private static ChromosomeData ExpressChromosomePair(ChromosomeData mat, ChromosomeData pat)
	     {
		          if (mat == null || pat == null)
				return null;

		          if (mat.alleles.Length != pat.alleles.Length)
				throw new System.ArgumentException(
				    $"Chromosome '{mat.name}' is not homologous with '{pat.name}' " +
				    $"(mat={mat.alleles.Length}, pat={pat.alleles.Length}).");

		          var alleles = new AlleleData[mat.alleles.Length];
		          for (int i = 0; i < alleles.Length; i++)
				alleles[i] = ExpressAllelePair(mat.alleles[i], pat.alleles[i]);

		          return new ChromosomeData { name = mat.name, alleles = alleles };
	     }

	     // ---- body-part level ----
	     public static BodyPartData Mutate(BodyPartData source, float ratePerAllele)
	     {
		          if (source?.alleles == null || source.alleles.Length == 0)
				return source;

		          AlleleData[] mutated = null;
		          for (int i = 0; i < source.alleles.Length; i++)
		          {
				var m = Mutate(source.alleles[i], ratePerAllele);
				if (ReferenceEquals(m, source.alleles[i]))
					     continue;
				mutated ??= (AlleleData[])source.alleles.Clone();
				mutated[i] = m;
		          }

		          return mutated == null
		              ? source
		              : new BodyPartData(source.name, mutated.ToList());
	     }

	     // ---- whole genome ----
	     public static ChromosomeData[] Mutate(ChromosomeData[] source, float ratePerAllele)
	     {
		          if (source == null)
				return null;

		          ChromosomeData[] mutated = null;
		          for (int i = 0; i < source.Length; i++)
		          {
				var m = Mutate(source[i], ratePerAllele);
				if (ReferenceEquals(m, source[i]))
					     continue;
				mutated ??= (ChromosomeData[])source.Clone();
				mutated[i] = m;
		          }

		          return mutated ?? source;
	     }

	     public static ChromosomeData[] ExpressChromosomes(ChromosomeData[] mat, ChromosomeData[] pat)
	     {
		          if (mat == null || pat == null)
				return new ChromosomeData[0];

		          if (mat.Length != pat.Length)
				throw new System.ArgumentException(
				    $"Homologous chromosome sets must match (mat={mat.Length}, pat={pat.Length}).");

		          var expressed = new ChromosomeData[mat.Length];
		          for (int c = 0; c < mat.Length; c++)
				expressed[c] = ExpressChromosomePair(mat[c], pat[c]);

		          return expressed;
	     }

	     public static ChromosomeData[] RandomiseGeneticRange(ChromosomeData[] genome)
	     {
		          //takes a genome and randomises the numericAspects so that the minimums and the maximums for the range are set randomly. we will need to check it agains the max values in the GeneLibrary
		          ChromosomeData[] data = new ChromosomeData[genome.Length];
		          int i = 0;
		          foreach (ChromosomeData c in genome)
		          {
				ChromosomeData chromosomeData = new ChromosomeData();
				chromosomeData.name = c.name;
				List<AlleleData> alleles = new List<AlleleData>();
				foreach (AlleleData a in c.alleles)
				{

					     int libMin = (int)GeneLibrary.Lookup(a.range.name).range.minValue;
					     int libMax = (int)GeneLibrary.Lookup(a.range.name).range.maxValue;

					     int newMin, newMax;
					     if (libMax <= libMin)
					     {
						          // Single-value trait; nothing to randomise.
						          newMin = libMin;
						          newMax = libMax;
					     }
					     else
					     {
						          // Random sub-range of [libMin, libMax], at least two values wide.
						          newMin = Random.Range(libMin, libMax);           // int overload, upper exclusive
						          newMax = Random.Range(newMin + 1, libMax + 1);   // guaranteed > newMin
					     }

					     var aspect = new NumericalAspect(
					         a.range.name, newMax, newMin,
					         a.range.polar, a.range.polarPoint, "");
					     aspect.value = Random.Range((float)newMin, (float)newMax + 1f);

					     if (aspect.value < aspect.minValue || aspect.value > aspect.maxValue)
					     {
						          aspect.value = Random.Range(aspect.minValue, aspect.maxValue);
					     }
					     aspect.descriptor = AlleleLibrary.Describe(aspect.name, (int)aspect.value);
					     alleles.Add(new(a.inheritance, aspect));
					     //a.range = aspect;
				}
				chromosomeData.alleles = alleles.ToArray();
				data[i] = chromosomeData;
				i++;
		          }
		          return data;
	     }

	     public static ChromosomeData[] RandomiseGeneticValues(ChromosomeData[] genome)
	     {
		          //takes a genome and randomises the values of the numeric aspects to fit somewhere between their minimum and maximum range
		          int i = 0;
		          ChromosomeData[] data = new ChromosomeData[genome.Length];
		          foreach (ChromosomeData c in genome)
		          {
				ChromosomeData chromosomeData = new ChromosomeData();
				chromosomeData.name = c.name;
				List<AlleleData> alleles = new List<AlleleData>();
				foreach (AlleleData a in c.alleles)
				{
					     NumericalAspect aspect = new NumericalAspect(a.range.name, a.range.maxValue, a.range.minValue);
					     aspect.value = Random.Range(aspect.minValue, aspect.maxValue);
					     aspect.descriptor = AlleleLibrary.Describe(aspect.name, (int)aspect.value);
					     alleles.Add(new(a.inheritance, aspect));
					     //a.range = aspect;
				}
				chromosomeData.alleles = alleles.ToArray();
				data[i] = chromosomeData;
				i++;
		          }
		          return data;
	     }
	     public static Dictionary<string, NumericalAspect> CollateAppearance(ChromosomeData[] chromosomes)
	     {
		          Dictionary<string, NumericalAspect> dictionary = new Dictionary<string, NumericalAspect>();
		          foreach (ChromosomeData ch in chromosomes)
		          {
				foreach (AlleleData al in ch.alleles)
				{
					     dictionary.Add(al.range.name, al.range);
				}
		          }
		          return dictionary;
	     }
	     // ---- helpers ----
	     private static NumericalAspect CloneAspect(NumericalAspect src)
	     {
		          // If NumericalAspect is a struct, this is just: return src;
		          var c = new NumericalAspect(src.name, src.maxValue);
		          c.minValue = src.minValue;
		          c.maxValue = src.maxValue;
		          c.value = src.value;
		          c.descriptor = src.descriptor;
		          // c.polar      = src.polar;
		          // c.polarPoint = src.polarPoint;
		          return c;
	     }
}
