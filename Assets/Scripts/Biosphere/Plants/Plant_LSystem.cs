using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LSystem : MonoBehaviour
{
	     [Header("L-System Configuration")]
	     [Tooltip("The initial axiom (starting string) for the L-system")]
	     public string axiom = "s";

	     [Tooltip("Scale factor for each iteration")]
	     public float iterationScaleFactor = 0.8f;

	     // Rules - kept the same as yours
	     public static Dictionary<char, string[]> MasterRules = new Dictionary<char, string[]>()
{
	     // ========== Structural Components ==========
	      { 'S', new string[] {
		         // "S",       // Minimal growth
		          "S",
	     }},
    
	     // ========== <Leaf Production ==========
	     { 'l', new string[] {
		          "<L",        // Single <leaf
		          "<L<L",       // <Leaf pair
		          "<L<L<L", // <Leaf triplet
		          "<L<L<L<L",
		          "<L<L<L<L<L",
		          "[/S-<L]",
		          "[/S-<L<L]",
		          "[/S-<L<L<L]",
		          "[/S-<L<L<L<L]",
		          "[/b-S<L]",
		          "[/b-S<L<L]",
		          "[/b-S<L<L<L]",
		          "[/b-S<L<L<L<L]",
		          "[/S-b<L]",
		          "[/S-b<L<L]",
		          "[/S-b<L<L<L]",
		          "[/S-b<L<L<L<L]",
		          "[<S<L][>S<L]",
		          "[<S<L<L][>S<L<L]",
		          "[<S<L<L<L][>S<L<L<L]",
		          "[<S<L<L<L<L][>S<L<L<L<L]",
		          "[<S<L][vS<L][>S<L]",
		          "[<S<L<L][vS<L<L][>S<L<L]",
		          "[<S<L<L<L][vS<L<L<L][>S<L<L<L]",
		          "[<S<L<L<L<L][vS<L<L<L<L][>S<L<L<L<L]",
		          "[<S<L]<[<S<L]<[>S<L]",
		          "[<S<L<L]<[<S<L<L]<[>S<L<L]",
		          "[<S<L<L<L]<[<S<L<L<L]<[>S<L<L<L]",
		          "[vS<L]",
		          "[<S<L<L]",
		          "[<S<L<L<L]",
		          "[<S<L<L<L<L]",
		          "[<bS<L]",
		          "[<bS<L<L]",
		          "[<bS<L<L<L]",
		          "[<bS<L<L<L<L]",
		          "[<Sb<L]",
		          "[<Sb<L<L]",
		          "[<Sb<L<L<L]",
		          "[<Sb<L<L<L<L]",
		          "[<Sb[<S<L]]",
		          "[<Sb[<S<L<L]]",
		          "[<Sb[<S<L<L<L]]",
		          "[<Sb[<S<L<L<L<L]]",
		          "[vSb[<S<L]]",
		          "[vSb[<S<L<L]]",
		          "[vSb[<S<L<L<L]]",
		          "[vSb[<S<L<L<L<L]]",
		          "[<Sb[<S<L]]",
		          "[<Sb[<S<L>S<L]]",
		          "[<Sb[<S<L>S<L<S<L]]",
		          "[<Sb[<S<L>S<L<S<L>S<L]]",
		          "[<Sb[<S<L]]",
		          "[<Sb[<S<L<S<L]]",
		          "[<Sb[<S<L<S<L<S<L]]",
		          "[<Sb[<S<L<S<L<S<L<S<L]]",
		          "[vSb[<S<L]]",
		          "[vSb[<S<L>S<L]]",
		          "[vSb[<S<L>S<L<S<L]]",
		          "[vSb[<S<L>S<L<S<L>S<L]]",
		          "[vSb[<S<L]]",
		          "[vSb[<S<L<S<L]]",
		          "[vSb[<S<L<S<L<S<L]]",
		          "[vSb[<S<L<S<L<S<L<S<L]]",
		          "<L",        // Single <leaf
		          "<L>>L",       // <Leaf pair
		          "<L>>L<<<L", // <Leaf triplet
		          "<L>>L<<<L>>>>L",
		          "<L>>L<<<L>>>>L<<<<<L",
		          "[/S-<L]",
		          "[/S-<L>>L]",
		          "[/S-<L>>L<<<L]",
		          "[/S-<L>>L<<<L>>>>L]",
		          "[/b-S<L]",
		          "[/b-S<L>>L]",
		          "[/b-S<L>>L<<<L]",
		          "[/b-S<L>>L<<<L>>>>L]",
		          "[/S-b<L]",
		          "[/S-b<L>>L]",
		          "[/S-b<L>>L<<<L]",
		          "[/S-b<L>>L<<<L>>>>L]",
		          "[<S<L][>S<L]",
		          "[<S<L>>L][>S<L>>L]",
		          "[<S<L>>L<<<L][>S<L>>L<<<L]",
		          "[<S<L>>L<<<L>>>>L][>S<L>>L<<<L>>>>L]",
		          "[<S<L][vS<L][>S<L]",
		          "[<S<L>>L][vS<L>>L][>S<L>>L]",
		          "[<S<L>>L<<<L][vS<L>>L<<<L][>S<L>>L<<<L]",
		          "[<S<L>>L<<<L>>>>L][vS<L>>L<<<L>>>>L][>S<L>>L<<<L>>>>L]",
		          "[<S<L]<[<S<L]<[>S<L]",
		          "[<S<L>>L]<[<S<L>>L]<[>S<L>>L]",
		          "[<S<L>>L<<<L]<[<S<L>>L<<<L]<[>S<L>>L<<<L]",
		          "[vS<L]",
		          "[<S<L>>L]",
		          "[<S<L>>L<<<L]",
		          "[<S<L>>L<<<L>>>>L]",
		          "[<bS<L]",
		          "[<bS<L>>L]",
		          "[<bS<L>>L<<<L]",
		          "[<bS<L>>L<<<L>>>>L]",
		          "[<Sb<L]",
		          "[<Sb<L>>L]",
		          "[<Sb<L>>L<<<L]",
		          "[<Sb<L>>L<<<L>>>>L]",
		          "[<Sb[<S<L]]",
		          "[<Sb[<S<L>>L]]",
		          "[<Sb[<S<L>>L<<<L]]",
		          "[<Sb[<S<L>>L<<<L>>>>L]]",
		          "[vSb[<S<L]]",
		          "[vSb[<S<L>>L]]",
		          "[vSb[<S<L>>L<<<L]]",
		          "[vSb[<S<L>>L<<<L>>>>L]]",
		          "[<Sb[<S>-L]]",
		          "[<Sb[<S>-L>S>-L]]",
		          "[<Sb[<S>-L>S>-L<S>-L]]",
		          "[<Sb[<S>-L>S>-L<S>-L>S>-L]]",
		          "[<Sb[<S>-L]]",
		          "[<Sb[<S>-L<S>-L]]",
		          "[<Sb[<S>-L<S>-L<S>-L]]",
		          "[<Sb[<S>-L<S>-L<S>-L<S>-L]]",
		          "[vSb[<S>-L]]",
		          "[vSb[<S>-L>S>-L]]",
		          "[vSb[<S>-L>S>-L<S>-L]]",
		          "[vSb[<S>-L>S>-L<S>-L>S>-L]]",
		          "[vSb[<S>-L]]",
		          "[vSb[<S>-L<S>-L]]",
		          "[vSb[<S>-L<S>-L<S>-L]]",
		          "[vSb[<S>-L<S>-L<S>-L<S>-L]]",
	     }},

	     { 'L', new string[] {
		          "L",        // Leaf
		               // Leaf Drop
	     }},
	     // ========== Apical Bud System ==========
	         { 'A', new string[] { // Apical bud. Must contain a B and an A. When adding this to plant rulesets, iterate over them to add stems and turns and branch variety ie (B becomes [B]B, l, b, T, CT or DT or ET)
	     "SA",       // Continue growing
	     "B^A",
	     "BS^A",
	     "SB^A",
	     "S[B]^SA",
	     //double branch structures
	     "S[<B][>B]^A", //two branches
	     "S[<<B]>B]]^A", //nested branches
	     "S[<B]S[>B]^A", //branches along stem
	     "S[<B]S[<B]^A", //branches along stem

	     "S[<B][>C]^A", //two branches
	     "S[<<B]>C]]^A", //nested branches
	     "S[<B]S[>C]^A", //branches along stem
	     "S[<B]S[<C]^A", //branches along stem
	     "S[<C][>B]^A", //two branches
	     "S[<<C]>B]]^A", //nested branches
	     "S[<C]S[>B]^A", //branches along stem
	     "S[<C]S[<B]^A", //branches along stem

	     "S[<B][>D]^A", //two brandhes
	     "S[<<B]>D]]^A", //nested brandhes
	     "S[<B]S[>D]^A", //brandhes along stem
	     "S[<B]S[<D]^A", //brandhes along stem
	     "S[<D][>B]^A", //two brandhes
	     "S[<<D]>B]]^A", //nested brandhes
	     "S[<D]S[>B]^A", //brandhes along stem
	     "S[<D]S[<B]^A", //brandhes along stem

	     "S[<B][>E]^A", //two branehes
	     "S[<<B]>E]]^A", //nestee branehes
	     "S[<B]S[>E]^A", //branehes along stem
	     "S[<B]S[<E]^A", //branehes along stem
	     "S[<E][>B]^A", //two branehes
	     "S[<<E]>B]]^A", //nestee branehes
	     "S[<E]S[>B]^A", //branehes along stem
	     "S[<E]S[<B]^A", //branehes along stem

	     "S[<E][>C]^A", //two eranches
	     "S[<<E]>C]]^A", //nested eranches
	     "S[<E]S[>C]^A", //eranches along stem
	     "S[<E]S[<C]^A", //eranches along stem
	     "S[<C][>E]^A", //two eranches
	     "S[<<C]>E]]^A", //nested eranches
	     "S[<C]S[>E]^A", //eranches along stem
	     "S[<C]S[<E]^A", //eranches along stem

	     "S[<E][>D]^A", //two erandhes
	     "S[<<E]>D]]^A", //nested erandhes
	     "S[<E]S[>D]^A", //erandhes along stem
	     "S[<E]S[<D]^A", //erandhes along stem
	     "S[<D][>E]^A", //two erandhes
	     "S[<<D]>E]]^A", //nested erandhes
	     "S[<D]S[>E]^A", //erandhes along stem
	     "S[<D]S[<E]^A", //erandhes along stem

	     "S[<C][>E]^A", //two cranehes
	     "S[<<C]>E]]^A", //nestee cranehes
	     "S[<C]S[>E]^A", //cranehes along stem
	     "S[<C]S[<E]^A", //cranehes along stem
	     "S[<E][>C]^A", //two cranehes
	     "S[<<E]>C]]^A", //nestee cranehes
	     "S[<E]S[>C]^A", //cranehes along stem
	     "S[<E]S[<C]^A", //cranehes along stem
	     //triple branch structures
	     "S[vB][B^A][^B]", //three branches
	     "S[<<B][<B][>B]^A", //three branches
	     "S[[[B]B]B]^A", //nested branches
	     "S/[/<[/<[B]B]B]^A", //nested branches
	     "S[[B]B][B]^A",
	     "S[B][[B]B]^A",
	     "S[B]B]]S[B]^A",
	     "S[B]S[B]B]]^A",
	     "S[[[SB]SB]SB]^A",

	     "S[vB][C^A][^D]", //three branches
	     "S[<<B][<C][>D]^A", //three branches
	     "S<[<[<[B]C]D]^A", //nested branches
	     "S/[/<[/<[<B]^C]>D]^A", //nested branches
	     "S[[<B]^C][>D]^A",
	     "S[B][[C]D]^A",
	     "S[B]C]]S[D]^A",
	     "S[B]S[C]D]]^A",
	     "S[[[SB]SC]SD]^A",

	     //four branch structures
	     "S[<B][^B][vB][>B]^A", //four branches
	     "S[<[<B]B][>[>B]B]^A", //nested branches
	     "S[B][[B]B][B]^A", //alternating nested branches

	     //five branch structures
	     "S[B][B][B][B][B]^A", //five branches
	     "S[[B]B][[B]B][B]^A", //nested branches
	     "S[B][[B]B][[B]B]^A", //alternating nested branches
	     "S[[[B]B]B][B][B]^A", //alternating nested branches
	     "S[[[B]B]B][[B[B]^A", //alternating nested branches

	     	     "S/A",       // Continue growing
	     "/B^A",
	     "/BS^A",
	     "/SB^A",
	     "/S[B]^SA",
	     //double branch structures
	     "/S[B][B]^A", //two branches
	     "/S[B]B]]^A", //nested branches
	     "/S[B]S[B]^A", //branches along stem
	     //triple branch structures
	     "/S[B][B][B]^A", //three branches
	     "/S/[/[/[B]B]B]^A", //nested branches
	     "/S[[B]B][B]^A",
	     "/S[B][[B]B]^A",
	     "/S[B]B]]S[B]^A",
	     "/S[B]S[B]B]]^A",
	     "/S[/[/[SB]SB]SB]^A",
	       	     //four branch structures
	     "/S[B][B][B][B]^A", //four branches
	     "/S[[B]B][[B]B]^A", //nested branches
	     "/S[B][[B]B][B]^A", //alternating nested branches


	     //five branch structures
	     "/S[B][B][B][B][B]^A", //five branches
	     "/S[[B]B][[B]B][B]^A", //nested branches
	     "/S[B][[B]B][[B]B]^A", //alternating nested branches
	     "/S[[[B]B]B][B][B]^A", //alternating nested branches
	     "/S[[[B]B]B][[B[B]^A", //alternating nested branches
	         }},

	             { 'a', new string[] { // Axillary bud
        "a<S",
        "a/S",
        "aT",
        "b",// 
    }},

    { 'T', new string[] { // Terminal buds .Dictates structure for flowers and leaves
	         //basic structures
	         "lbT",
	         "blT",
	     "[l][b]T",
	     "[[b]l]T",
	     "[[l]b]T",
	     //double structures
	     "[[l][b]][[l][b]]T",
	     "[[b]l][[b]l]T",
	     "[[l]b][[l]b]T",
	     "SlbT",
	     "SblT",
	     "S[l][b]T",
	     "S[[b]l]T",
	     "S[[l]b]T",
	     //double structures
	     "S[[l][b]][[l][b]]T",
	     "S[[b]l][[b]l]T",
	     "S[[l]b][[l]b]T",

	     "<SlbT",
	     "<SblT",
	     "<S[l][b]T",
	     "<S[[b]l]T",
	     "<S[[l]b]T",
	     //double structures
	     "<S[[l][b]][[l][b]]T",
	     "<S[[b]l][[b]l]T",
	     "<S[[l]b][[l]b]T",

	     ">S[lb][lb]^T", //two lbranches
	     ">S[lb]lb]]^T", //nested lbranches
	     ">S[lb]S[lb]^T", //lbranches along stem
	     //triple lbranch structures
	     ">S[lb][lb][lb]^T", //three lbranches
	     ">S[[[lb]lb]lb]^T", //nested lbranches
	     ">S[[lb]lb][lb]^T",
	     ">S[lb][[lb]lb]^T",
	     ">S[lb]lb]]S[lb]^T",
	     ">S[lb]S[lb]lb]]^T",
	     ">S[[[Slb]Slb]Slb]^T",

	     //four lbranch structures
	     ">S[lb][lb][lb][lb]^T", //four lbranches
	     ">S[[lb]lb][[lb]lb]^T", //nested lbranches
	     ">S[lb][[lb]lb][lb]^T", //alternating nested lbranches

	     //five lbranch structures
	     ">S[lb][lb][lb][lb][lb]^T", //five lbranches
	     ">S[[lb]lb][[lb]lb][lb]^T", //nested lbranches
	     ">S[lb][[lb]lb][[lb]lb]^T", //alternating nested lbranches
	     ">S[[[lb]lb]lb][lb][lb]^T", //alternating nested lbranches
	     ">S[[[lb]lb]lb][[lb[lb]^T", //alternating nested lbranches
    }},
    
	     // ========== Branching System ==========
	     { 'B', new string[] {
	     "aST",
	     //single branch structures
	     "a[T]T",
	     "aS[T]T",
	     //two branch structures
	     "a[T][T]T",
	     "a[[T]T]T",
	     "a[T[T]]T",
	     "aS[<T]S[<T]T",
	     "aS/[<T]/S[>T]T",
	     "aS[<T[<T]]T",
	     //three branch structures
	     "aS[<T][>T][<^T]T",
	     "aS[<T]S[<T]S[<T]T",
	     "aS[<T]/S[<T]/S[<T]T",
	     "aS[<T[<T[<T]]]T",
	     //complex branch structures
	     "a<S[>CT]/S[^DT]ST",
	     "a<S[+[T]/C/D/E]T", //spiral
	     "a<S[+[T]/C/[T]/CT]T", //spiral

	     ">aS[T][T]T", //two branches
	     ">aS[T]T]]T", //nested branches
	     ">aS[T]S[T]T", //branches along stem
	     //triple branch structures
	     ">aS[T][T][T]T", //three branches
	     ">aS[[[T]T]T]T", //nested branches
	     ">aS[[T]T][T]T",
	     ">aS[T][[T]T]T",
	     ">aS[T]T]]S[T]T",
	     ">aS[T]S[T]T]]T",
	     ">aS[[[ST]ST]ST]T",

	     //four branch structures
	     ">aS[T][T][T][T]T", //four branches
	     ">aS[[T]T][[T]T]T", //nested branches
	     ">aS[T][[T]T][T]T", //alternating nested branches

	     //five branch structures
	     ">aS[T][T][T][T][T]T", //five branches
	     ">aS[[T]T][[T]T][T]T", //nested branches
	     ">aS[T][[T]T][[T]T]T", //alternating nested branches
	     ">aS[[[T]T]T][T][T]T", //alternating nested branches
	     ">aS[[[T]T]T][[T[T]T", //alternating nested branches

	     }},

     { 'C', new string[] { // treated as a stem segment
	         "D",
	         "E",
	     "SS",
	     "S/S",
	     "SvS",
	     "S<S",
	     "SS[B]",
	     "S[B]S",
	     "S[B]SC",
	         "S[B][CT]S",
	         "S[B][CT][DT][ET]S",
	          "aST",
	     //single branch structures
	     "a[T]T",
	     "aS[T]T",
	     //two branch structures
	     "a[T][T]T",
	     "a[[T]T]T",
	     "a[T[T]]T",
	     "aS[<T]S[<T]T",
	     "aS/[<T]/S[>T]T",
	     "aS[<T[<T]]T",
	     //three branch structures
	     "aS[<T][>T][<^T]T",
	     "aS[<T]S[<T]S[<T]T",
	     "aS[<T]/S[<T]/S[<T]T",
	     "aS[<T[<T[<T]]]T",
	     //complex branch structures
	     "a<S[>CT]/S[^DT]ST",
	     "a<S[+[T]/C/D/ET]T", //spiral
	     "a<S[+[T]/C/[T]/CT]T", //spiral

	     ">aS[T][T]T", //two branches
	     ">aS[T]T]]T", //nested branches
	     ">aS[T]S[T]T", //branches along stem
	     //triple branch structures
	     ">aS[T][T][T]T", //three branches
	     ">aS[[[T]T]T]T", //nested branches
	     ">aS[[T]T][T]T",
	     ">aS[T][[T]T]T",
	     ">aS[T]T]]S[T]T",
	     ">aS[T]S[T]T]]T",
	     ">aS[[[ST]ST]ST]T",

	     //four branch structures
	     ">aS[T][T][T][T]T", //four branches
	     ">aS[[T]T][[T]T]T", //nested branches
	     ">aS[T][[T]T][T]T", //alternating nested branches

	     //five branch structures
	     ">aS[T][T][T][T][T]T", //five branches
	     ">aS[[T]T][[T]T][T]T", //nested branches
	     ">aS[T][[T]T][[T]T]T", //alternating nested branches
	     ">aS[[[T]T]T][T][T]T", //alternating nested branches
	     ">aS[[[T]T]T][[T[T]T", //alternating nested branches
    }},

    { 'D', new string[] { // threated as a stem segment
	         "C",
	         "E",
	     "SS",
	     "S/S",
	     "SvS",
	     "S<S",
	     "SS[B]",
	     "S[B]S",
	     "S[B]SC",
	         "S[B][CT]S",
	         "S[B][CT][DT][ET]S",
	          "aST",
	     //single branch structures
	     "a[T]T",
	     "aS[T]T",
	     //two branch structures
	     "a[T][T]T",
	     "a[[T]T]T",
	     "a[T[T]]T",
	     "aS[<T]S[<T]T",
	     "aS/[<T]/S[>T]T",
	     "aS[<T[<T]]T",
	     //three branch structures
	     "aS[<T][>T][<^T]T",
	     "aS[<T]S[<T]S[<T]T",
	     "aS[<T]/S[<T]/S[<T]T",
	     "aS[<T[<T[<T]]]T",
	     //complex branch structures
	     "a<S[>CT]/S[^DT]ST",
	     "a<S[+[T]/C/D/ET]T", //spiral
	     "a<S[+[T]/C/[T]/CT]T", //spiral

	     ">aS[T][T]T", //two branches
	     ">aS[T]T]]T", //nested branches
	     ">aS[T]S[T]T", //branches along stem
	     //triple branch structures
	     ">aS[T][T][T]T", //three branches
	     ">aS[[[T]T]T]T", //nested branches
	     ">aS[[T]T][T]T",
	     ">aS[T][[T]T]T",
	     ">aS[T]T]]S[T]T",
	     ">aS[T]S[T]T]]T",
	     ">aS[[[ST]ST]ST]T",

	     //four branch structures
	     ">aS[T][T][T][T]T", //four branches
	     ">aS[[T]T][[T]T]T", //nested branches
	     ">aS[T][[T]T][T]T", //alternating nested branches

	     //five branch structures
	     ">aS[T][T][T][T][T]T", //five branches
	     ">aS[[T]T][[T]T][T]T", //nested branches
	     ">aS[T][[T]T][[T]T]T", //alternating nested branches
	     ">aS[[[T]T]T][T][T]T", //alternating nested branches
	     ">aS[[[T]T]T][[T[T]T", //alternating nested branches
    }},

		          { 'E', new string[] { // threated as a stem segment
	         "C",
	         "D",
	     "SS",
	     "S/S",
	     "SvS",
	     "S<S",
	     "SS[B]",
	     "S[B]S",
	     "S[B]SC",
	         "S[B][CT]S",
	         "S[B][CT][DT][ET]S",
	          "aST",
	     //single branch structures
	     "a[T]T",
	     "aS[T]T",
	     //two branch structures
	     "a[T][T]T",
	     "a[[T]T]T",
	     "a[T[T]]T",
	     "aS[<T]S[<T]T",
	     "aS/[<T]/S[>T]T",
	     "aS[<T[<T]]T",
	     //three branch structures
	     "aS[<T][>T][<^T]T",
	     "aS[<T]S[<T]S[<T]T",
	     "aS[<T]/S[<T]/S[<T]T",
	     "aS[<T[<T[<T]]]T",
	     //complex branch structures
	     "a<S[>CT]/S[^DT]ST",
	     "a<S[+[T]/C/D/ET]T", //spiral
	     "a<S[+[T]/C/[T]/CT]T", //spiral

	     ">aS[T][T]T", //two branches
	     ">aS[T]T]]T", //nested branches
	     ">aS[T]S[T]T", //branches along stem
	     //triple branch structures
	     ">aS[T][T][T]T", //three branches
	     ">aS[[[T]T]T]T", //nested branches
	     ">aS[[T]T][T]T",
	     ">aS[T][[T]T]T",
	     ">aS[T]T]]S[T]T",
	     ">aS[T]S[T]T]]T",
	     ">aS[[[ST]ST]ST]T",

	     //four branch structures
	     ">aS[T][T][T][T]T", //four branches
	     ">aS[[T]T][[T]T]T", //nested branches
	     ">aS[T][[T]T][T]T", //alternating nested branches

	     //five branch structures
	     ">aS[T][T][T][T][T]T", //five branches
	     ">aS[[T]T][[T]T][T]T", //nested branches
	     ">aS[T][[T]T][[T]T]T", //alternating nested branches
	     ">aS[[[T]T]T][T][T]T", //alternating nested branches
	     ">aS[[[T]T]T][[T[T]T", //alternating nested branches
    }},
	     // ========== Direction Modifiers ==========
	     { '>', new string[] {
		          ">",        // Small right turn

	     }},

	     { '<', new string[] {
		          "<",        // Small left turn

	     }},

	     { 'v', new string[] {
		          "v",        // Downward adjustment
	     }},
    
	     // ========== Reproductive Structures ==========
	     { 'c', new string[] {
		          "SAL",        // Simple cotyledon
		          "Svv+++>>L------<<<<L+++>>^^A",     // Opposite cotyledons
		          //"LL",
		          // "L"
	     }},

	     { 's', new string[] {
		          "r-S[BT]c",     // Basic seed
		          "r-S<[[<B]>[>B]]c",
		          "r-S[[<B]>[>B]vBSc]",
		          "r-[B<BT]c",
		          "r-[BT]c",
		          "r-[[B]<[B]T]c",
		          "r-[[<B[>B]]T]c",
			   "r^c",
        "r^Sc",
        "r^SSc"

	     }},

	     { 'b', new string[] {
		          "f",        // Bud to flower
		          "+lf",        // Bud with leaf
		          "+[<Slf]",       // Bud with stem
		          "+[<Slf][>Slf]",
		          "+l[>Sf][>>Sf]",
		          "+[[Sl]Sf]",
		          "+l[Sf]/[>>Sf]/[<Sf]/[<<Sf]",
		          "+lS[Sf]>S[Sf]<S[Sf]>S[Sf]",
		          "+lS[Sf]>S[Sf]<S[Sf]>S[Sf]",
		          "+<lS[Sf]>S[Sf]<S[Sf]>S[Sf]",
		          "+[l>Sf]",
		          "B[>Slf]",
	     }},

	     { 'f', new string[] {
		          //"f",        // Flower
		          "F",        // Fruit
		          "FF",      // double fruit
		          "vSF",      // fruit with stem
	     }},

	     { 'F', new string[] {
		          "f",        // Fruit
		          //"b",
		          //"bl",       // Fruit with flower
		          "l",       // Fruit with leaf

	     }},
    
	     // ========== Root System ==========
	     { 'r', new string[] {
		          "r",        // Primary root

	     }}
		          };

	     // Private fields
	     private Plant plant;
	     private Vector3 position;
	     public float angle;
	     public AnimationCurve widthCurve;

	     // A simpler approach: store all lines to be drawn
	     private List<BranchData> allBranches = new List<BranchData>();
	     private GameObject plantContainer;

	     // Data structure for branches
	     private class BranchData
	     {
		          public Vector3 startPos;
		          public Vector3 endPos;
		          public float startWidth;
		          public float endWidth;
		          public Color color;
		          public GameObject parent;
		          public int depth;
	     }

	     // Stack state for L-system processing
	     private struct TurtleState
	     {
		          public Vector3 position;
		          public Vector3 direction;
		          public Quaternion rotation;
		          public float currentWidth;
		          public int depth;
	     }

	     /// <summary>
	     /// Initialize the L-system with plant data
	     /// </summary>
	     public void Initialize(Plant plant, Vector3 position)
	     {
		          this.plant = plant;
		          this.position = position;

		          if (!string.IsNullOrEmpty(plant.axiom))
		          {
				axiom = plant.axiom;
		          }

		          // Calculate angle
		          this.angle = Mathf.Lerp(7.5f, 30f,
		              Mathf.Lerp(0.0f, 1f, plant.Stems.StemColor.g));

		          SetupWidthCurve();
	     }

	     private void SetupWidthCurve()
	     {
		          widthCurve = new AnimationCurve(
		              new Keyframe(0, plant.Stems.Diameter, 0, 0.1f),
		              new Keyframe(0.5f, plant.Stems.Diameter * 0.9f, -0.2f, -0.2f),
		              new Keyframe(1, plant.Stems.Diameter * 0.7f)
		          );
		          widthCurve.preWrapMode = WrapMode.Clamp;
		          widthCurve.postWrapMode = WrapMode.Clamp;
	     }

	     /// <summary>
	     /// Main method to draw the plant
	     /// </summary>
	     public void DrawPlant(string axiomToDraw, GameObject plantGameObject)
	     {
		          if (string.IsNullOrEmpty(axiomToDraw))
		          {
				Debug.LogError("Cannot draw plant - axiom string is empty!");
				return;
		          }

		          plantContainer = plantGameObject;
		          allBranches.Clear();

		          // Clear existing LineRenderers
		          foreach (var lr in plantGameObject.GetComponentsInChildren<LineRenderer>())
		          {
				Destroy(lr.gameObject);
		          }

		          // Process the axiom
		          ProcessAxiom(axiomToDraw);

		          // Draw all collected branches
		          DrawAllBranches();
	     }

	     /// <summary>
	     /// Process the entire axiom string
	     /// </summary>
	     private void ProcessAxiom(string axiom)
	     {
		          Stack<TurtleState> stateStack = new Stack<TurtleState>();
		          TurtleState currentState = new TurtleState
		          {
				position = position,
				direction = Vector3.up,
				rotation = Quaternion.identity,
				currentWidth = plant.Stems.Diameter,
				depth = 0
		          };

		          // Track the current line segment points
		          List<Vector3> currentLinePoints = new List<Vector3>();
		          List<float> currentLineWidths = new List<float>();

		          // Start with initial point
		          currentLinePoints.Add(currentState.position);
		          currentLineWidths.Add(currentState.currentWidth);

		          int leafCount = 0;

		          for (int i = 0; i < axiom.Length; i++)
		          {
				char c = axiom[i];

				switch (c)
				{
				case 'S': // Draw stem segment
				ProcessStemSegment(ref currentState, ref currentLinePoints, ref currentLineWidths);
				break;

				case '[': // Save state and start new branch
				          // Finalize current line if it has points
				if (currentLinePoints.Count > 1)
				{
					     CreateLineRendererFromPoints(currentLinePoints, currentLineWidths, currentState.depth);
				}

				// Save current state
				stateStack.Push(currentState);

				// Start new line for the branch
				currentLinePoints.Clear();
				currentLineWidths.Clear();
				currentLinePoints.Add(currentState.position);
				currentLineWidths.Add(currentState.currentWidth * 0.8f); // Branches are thinner

				currentState.depth++;
				break;

				case ']': // End current branch and restore state
				          // Finalize current branch line
				if (currentLinePoints.Count > 1)
				{
					     CreateLineRendererFromPoints(currentLinePoints, currentLineWidths, currentState.depth);
				}

				// Restore previous state
				if (stateStack.Count > 0)
				{
					     currentState = stateStack.Pop();

					     // Start new line from the branch point
					     currentLinePoints.Clear();
					     currentLineWidths.Clear();
					     currentLinePoints.Add(currentState.position);
					     currentLineWidths.Add(currentState.currentWidth);
				}
				break;

				case '+': // Turn right
				currentState.rotation *= Quaternion.Euler(0, 0, -angle);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case '-': // Turn left
				currentState.rotation *= Quaternion.Euler(0, 0, angle);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				// Use Z rotations for these as well (they previously used Y/X and had no effect on Vector3.up)
				case '>': // Yaw right
				currentState.rotation *= Quaternion.Euler(0, angle, 0);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case '<': // Yaw left
				currentState.rotation *= Quaternion.Euler(0, -angle, 0);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case '^': // Pitch up
				currentState.rotation *= Quaternion.Euler(-angle, 0, 0);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case 'v': // Pitch down
				currentState.rotation *= Quaternion.Euler(angle, 0, 0);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case '/': // Roll right
				currentState.rotation *= Quaternion.Euler(0, 0, angle);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case '|': // Roll left
				currentState.rotation *= Quaternion.Euler(0, 0, -angle);
				currentState.direction = currentState.rotation * Vector3.up;
				break;

				case 'L': // Create leaf
				CreateLeaf(currentState.position, currentState.direction, currentState.currentWidth);
				leafCount++;
				break;

				case 'f': // Create flower
				CreateFlower(currentState.position, currentState.direction, currentState.currentWidth);
				break;

				case 'F': // Create fruit
				CreateFruit(currentState.position, currentState.direction, currentState.currentWidth);
				break;

				case 'B': // Branch indicator (just a marker, doesn't draw)
				case 'C': // Branch indicator (just a marker, doesn't draw)
				case 'D': // Branch indicator (just a marker, doesn't draw)
				case 'E': // Branch indicator (just a marker, doesn't draw)
				case 'A': // Apical bud
				case 'a': // Axillary bud
				case 'T': // Terminal bud
				case 'b': // Bud
				case 'l': // leafNode
				case 'c': // Cotyledon
				case 's': // Seed
				case 'r': // Root
				          // These symbols are structural but don't draw directly
				break;

				default:
				Debug.LogWarning($"Unhandled L-system symbol: '{c}'");
				break;
				}
		          }

		          // Finalize the main line
		          if (currentLinePoints.Count > 1)
		          {
				CreateLineRendererFromPoints(currentLinePoints, currentLineWidths, currentState.depth);
		          }

		          Debug.Log($"Processed axiom. Leaf count = {leafCount}");
	     }

	     /// <summary>
	     /// Process a single stem segment
	     /// </summary>
	     private void ProcessStemSegment(ref TurtleState state,
			       ref List<Vector3> points,
			       ref List<float> widths)
	     {
		          // Calculate new position
		          Vector3 newPosition = state.position + state.direction * (plant.Stems.Length * 0.01f);

		          // Calculate width based on depth and progression
		          float progression = points.Count > 0 ? (float)points.Count / 20f : 0f; // Adjust 20 as needed
		          float newWidth = widthCurve.Evaluate(progression) * Mathf.Pow(0.85f, state.depth);

		          // Add new point
		          points.Add(newPosition);
		          widths.Add(newWidth);

		          // Update state
		          state.position = newPosition;
		          state.currentWidth = newWidth;
	     }

	     /// <summary>
	     /// Create a LineRenderer from collected points
	     /// </summary>
	     private void CreateLineRendererFromPoints(List<Vector3> points, List<float> widths, int depth)
	     {
		          if (points.Count < 2)
				return;

		          // Create branch GameObject
		          GameObject branch = new GameObject($"Branch_Depth{depth}");
		          branch.transform.SetParent(plantContainer.transform);
		          branch.transform.position = Vector3.zero;

		          // Add LineRenderer
		          LineRenderer lr = branch.AddComponent<LineRenderer>();
		          lr.useWorldSpace = true;
		          lr.positionCount = points.Count;
		          lr.SetPositions(points.ToArray());

		          // Configure appearance
		          lr.startWidth = widths[0];
		          lr.endWidth = widths[widths.Count - 1];
		          lr.startColor = plant.Stems.StemColor;
		          lr.endColor = plant.Stems.StemColor;
		          lr.material = new Material(Shader.Find("Sprites/Default"));
		          lr.material.color = plant.Stems.StemColor;
		          lr.numCornerVertices = 5;
		          lr.numCapVertices = 5;

		          // Create width curve
		          AnimationCurve widthCurveLR = new AnimationCurve();
		          for (int i = 0; i < points.Count; i++)
		          {
				float t = i / (float)(points.Count - 1);
				widthCurveLR.AddKey(t, widths[i]);
		          }
		          lr.widthCurve = widthCurveLR;
	     }

	     /// <summary>
	     /// Draw all collected branches
	     /// </summary>
	     private void DrawAllBranches()
	     {
		          // Group branches by depth for better organization
		          var branchesByDepth = allBranches.GroupBy(b => b.depth);

		          foreach (var depthGroup in branchesByDepth)
		          {
				GameObject depthContainer = new GameObject($"Depth_{depthGroup.Key}");
				depthContainer.transform.SetParent(plantContainer.transform);
				depthContainer.transform.position = Vector3.zero;

				foreach (var branch in depthGroup)
				{
					     CreateSingleBranchRenderer(branch, depthContainer);
				}
		          }
	     }

	     /// <summary>
	     /// Create a single branch LineRenderer
	     /// </summary>
	     private void CreateSingleBranchRenderer(BranchData branch, GameObject parent)
	     {
		          GameObject branchObj = new GameObject("Branch");
		          branchObj.transform.SetParent(parent.transform);
		          branchObj.transform.position = Vector3.zero;

		          LineRenderer lr = branchObj.AddComponent<LineRenderer>();
		          lr.useWorldSpace = true;
		          lr.positionCount = 2;
		          lr.SetPosition(0, branch.startPos);
		          lr.SetPosition(1, branch.endPos);
		          lr.startWidth = branch.startWidth;
		          lr.endWidth = branch.endWidth;
		          lr.startColor = branch.color;
		          lr.endColor = branch.color;
		          lr.material = new Material(Shader.Find("Sprites/Default"));
		          lr.material.color = branch.color;
	     }

	     // Leaf creation
	     private void CreateLeaf(Vector3 position, Vector3 direction, float branchWidth)
	     {
		          if (plant?.Leaves == null)
				return;

		          GameObject leaf = new GameObject("Leaf");
		          leaf.transform.SetParent(plantContainer.transform);
		          leaf.transform.position = position;

		          LineRenderer lr = leaf.AddComponent<LineRenderer>();
		          float leafSize = Mathf.Max(0.0001f, branchWidth) * Mathf.Sqrt(plant.Leaves.SurfaceArea) * 2f;

		          Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;

		          Vector3[] leafPoints = new Vector3[3];
		          leafPoints[0] = position + perpendicular * (branchWidth * 0.5f); // offset so base doesn't coincide with stem center
		          leafPoints[1] = position + (direction + perpendicular * 0.3f).normalized * leafSize * 0.5f;
		          leafPoints[2] = position + direction.normalized * leafSize;

		          lr.positionCount = 3;
		          lr.SetPositions(leafPoints);
		          lr.startWidth = leafSize * 0.3f;
		          lr.endWidth = leafSize * 0.1f;
		          lr.startColor = plant.Leaves.LeafColor;
		          lr.endColor = plant.Leaves.LeafColor;
		          lr.material = new Material(Shader.Find("Sprites/Default"));
		          lr.material.color = plant.Leaves.LeafColor;
	     }

	     // Flower creation
	     private void CreateFlower(Vector3 position, Vector3 direction, float branchWidth)
	     {
		          if (plant?.Flowers == null)
				return;

		          GameObject flower = new GameObject("Flower");
		          flower.transform.SetParent(plantContainer.transform);
		          flower.transform.position = position;

		          LineRenderer lr = flower.AddComponent<LineRenderer>();
		          float size = plant.Flowers.Size / 5f;

		          // Simple circle for flower
		          int segments = 8;
		          lr.positionCount = segments + 1;
		          lr.loop = true;

		          for (int i = 0; i <= segments; i++)
		          {
				float angle = i * Mathf.PI * 2f / segments;
				lr.SetPosition(i, position + new Vector3(
				    Mathf.Cos(angle) * size,
				    Mathf.Sin(angle) * size,
				    0
				));
		          }

		          lr.startWidth = lr.endWidth = size * 0.5f;
		          lr.startColor = plant.Flowers.FlowerColor;
		          lr.endColor = plant.Flowers.FlowerColor;
		          lr.material = new Material(Shader.Find("Sprites/Default"));
		          lr.material.color = plant.Flowers.FlowerColor;
	     }

	     // Fruit creation
	     private void CreateFruit(Vector3 position, Vector3 direction, float branchWidth)
	     {
		          if (plant?.Fruits == null)
				return;

		          GameObject fruit = new GameObject("Fruit");
		          fruit.transform.SetParent(plantContainer.transform);
		          fruit.transform.position = position;

		          LineRenderer lr = fruit.AddComponent<LineRenderer>();
		          float radius = (plant.Flowers.Size + plant.Stems.Diameter) / 10f;

		          // Simple circle for fruit
		          int segments = 12;
		          lr.positionCount = segments + 1;
		          lr.loop = true;

		          for (int i = 0; i <= segments; i++)
		          {
				float angle = i * Mathf.PI * 2f / segments;
				lr.SetPosition(i, position + new Vector3(
				    Mathf.Cos(angle) * radius,
				    Mathf.Sin(angle) * radius,
				    0
				));
		          }

		          lr.startWidth = lr.endWidth = radius * 0.3f;
		          lr.startColor = plant.Fruits.FruitColor;
		          lr.endColor = plant.Fruits.FruitColor;
		          lr.material = new Material(Shader.Find("Sprites/Default"));
		          lr.material.color = plant.Fruits.FruitColor;
	     }

	     /// <summary>
	     /// Generate L-system string (keep your existing method)
	     /// </summary>
	     public string GenerateLSystem(string plantAxiom, int iterations)
	     {
		          if (iterations < 0 || iterations > 10)
		          {
				Debug.LogWarning($"Unusual iteration count: {iterations}. Clamping to 0-10 range.");
				iterations = Mathf.Clamp(iterations, 0, 10);
		          }

		          string currentString = plantAxiom;
		          System.Random random = new System.Random();

		          for (int i = 0; i < iterations; i++)
		          {
				float scale = Mathf.Pow(iterationScaleFactor, i);
				currentString = LSystem_Utilities.ExpandString(currentString, random, scale, plant.Rules);
		          }

		          Debug.Log($"Generated L-System string ({iterations} iterations): {currentString}");
		          return currentString;
	     }

	     private void OnDestroy()
	     {
		          // Cleanup
		          if (plantContainer != null)
		          {
				Destroy(plantContainer);
		          }
	     }
}