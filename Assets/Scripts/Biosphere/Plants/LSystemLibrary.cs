using System.Collections.Generic;

public static class LSystemLibrary
{

	     public static Dictionary<char, string[]> MasterRules = new Dictionary<char, string[]>()
	     {
	     	     // ========== Structural Components ==========
	     	     { 'S', new string[] {
	     		          // "S",       // Minimal growth
	     		          "S",
			          //"SS",
			          //"S/S",
	     	     } },

	     	     // ========== <Leaf Production ==========
	     	     { 'l', new string[] {
	     		          // Simple leaf types
	     		          "<L",                    // Simple alternate leaf
	     		          "<L[S-L]",              // Leaf with petiole (stalk)
	     		          "<[+++L][---L]",           // Bifid/cleft leaf (split leaf)

	     		          // Compound leaves
	     		          "<[+++L][---L][S---L]",      // Trifoliate (clover---like)
	     		          "<[+++L][---L][+++L][---L]",   // Palmate compound (4 leaflets)
	     		          "<[S---L][+++L][---L]",      // Ternate compound

	     		          // Pinnate arrangements
	     		          "<L[/S<L]",            // Simple pinnate (2 leaflets)
	     		          "<L[/S<L][/S>L]",      // Pinnate with opposite leaflets
	     		          "<L[/S<L[/S<L]]",      // Bipinnate (twice compound)

	     		          // Whorled/rosette arrangements
	     		          "[<L][vL][>L]",        // Whorl of 3
	     		          "[<L][vL][>L][^L]",    // Whorl of 4 (basal rosette)
	     		          "[<L<L][vL<L][>L<L]",  // Double whorl

	     		          // Scale---like/needle leaves
	     		          "<[+++L][---L]S<[+++L][---L]",  // Scale---like (conifer)
	     		          "<L<L<L<L",            // Fascicle (pine needle cluster)
	     		          "<[S<L][S<L][S<L]",    // Needle whorl

	     		          // Lobed leaves
	     		          "<[+++L]S[---L]",           // 3---lobed (maple---like)
	     		          "<[+++L][+++L]S[---L][---L]",  // 5---lobed
	     		          "<[/L<L][/L>L]",       // Deeply lobed

	     		          // Leaf pairs with variations
	     		          "[<L][>L]",            // Opposite leaves
	     		          "[<L---S][>L---S]",        // Opposite with stem between
	     		          "[<L]<[>L]",           // Decussate (alternating pairs)

	     		          // Compound with buds
	     		          "<[+++b---L][---b---L]",       // Buds at leaf base
	     		          "<L[/b---S<L]",          // Bud in leaf axil
	     		          "<[b+++L][b---L]",         // Buds on both sides

	     		          // Flower/fruit integration
	     		          "<[L][b]",             // Leaf with terminal bud
	     		          "<L[S---b]",             // Leaf with subterminal bud
	     		          "<[b]S<L",             // Bud then leaf

	     		          // Tendril modifications
	     		          "vL[/S---b]",             // Leaf with tendril (~ = tendril symbol)
	     		          "v[L][/S---b]",           // Leaf and separate tendril

	     		          // Asymmetric arrangements
	     		          "<[/S<L][/S>L]",       // Alternate pinnate
	     		          "<[+++S<L][---S>L]",       // Spiral alternate

	     		          // Dense arrangements
	     		          "<L<L<L<L<L<L",        // Dense alternate
	     		          "[<L][vL][>L]S[<L][vL][>L]",  // Double whorled node

	     		          // Leaf size variations (using symbols for size)
	     		          "<L<l",                // Large then small leaf (l = small leaf)
	     		          "<[+++L][---l]",           // Large and small pair

	     		          // Petiolate variations
	     		          "[/S---<L]",             // Long petiole
	     		          "[/S/S---<L]",           // Twisted petiole
	     		          "[//S---<L]",            // Zigzag petiole

	     		          // Specialized forms
	     		          "<[+++L+++L][---L---L]",       // Deeply divided
	     		          "<[S[<L][>L]]",        // Leaf on branched stem
	     		          "<[[+++L][---L]][[+++L][---L]]", // Double compound

	     		          // Water---conserving forms
	     		          "<L<L<L",              // Reduced leaves (xerophyte)
	     		          "<[S<L]S[S<L]",        // Sparse arrangement

	     		          // Leaf clusters at nodes
	     		          "[<L<L][vL<L][>L<L]",  // Multiple leaves per position
	     		          "[<L<L<L][>L<L<L]",    // Dense clusters

	     		          // Spiraled phyllotaxy
	     		          "</L</L</L",           // 3---leaf spiral
	     		          "</L</L</L</L</L",     // 5---leaf spiral

	     		          "</SL</SL</SL",           // 3---leaf spiral
	     		          "</SL</SL</SL</SL</SL",     // 5---leaf spiral

	     		          "</SL</Sl",           // 2---leaf spiral
	     		          "v/SL</SL</Sl",     // 3---leaf spiral    
	     		          "v/SL</SL</SL</Sl",     // 4---leaf spiral    
	     		          "v/SL</SL</SL</SL</Sl",     // 5---leaf spiral

	     		          // Mixed arrangements
	     		          "<L[<b---L][>b---L]",      // Leaf with bud pairs
	     		          "<[+++L---S][---L---S]S<L",    // Sequential compound
	     	     } },
		     { 'j', new string[] { // leaf marker
				"J",       
	     			       // Leaf spot
	     	     } },
	     	     { 'J', new string[] {    // baby leaf
				"K",

		          } },
		     { 'K', new string[] {      // Mature Leaf
				//"L",
				"K",
		          } },
		     { 'L', new string[] {    // Yellowing Leaf 50% of original green value
				"M",
				"j",

		          } },
		     { 'M', new string[] {   // Orange Leaf 25% of original green value
				"N",
				"j",
		          } },
		      { 'N', new string[] {    // Red Leaf 0% of original green value
				"j",    
	     			       // Leaf Drop
	     	     } },
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
	     	     } },

	     	     { 'a', new string[] { // Axillary bud
	     		          "a<S",
				"a/S",
				"aT",
				"b",// 
	     	     } },

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

	     		          // Terminal inflorescence variations
	     		          "/[b]||[b]T",  // Double bud terminal
	     		          "l[l]T",  // Double leaf terminal
	     		          "b[l]bT", // Bud-leaf-bud sequence
	     		          "l[b]lT", // Leaf-bud-leaf sequence

	     		          // Flower-fruit sequences
	     		          "fFT",  // Flower then fruit
	     		          "FfT",  // Fruit then flower
	     		          "f[F]T", // Flower with fruiting branch

	     		          // Mixed reproductive structures
	     		          "[l][b][f]T", // Leaf, bud, flower
	     		          "[b][f][F]T", // Bud, flower, fruit
	     		          "[l][f][F]T", // Leaf, flower, fruit

	     		          // Spiral phyllotaxy at terminal
	     		          "</l</b</lT",  // Spiral leaf-bud-leaf
	     		          "</b</l</bT",  // Spiral bud-leaf-bud
	     		          "</f</b</fT",  // Spiral flower-bud-flower

	     		          // Whorled terminal
	     		          "[<l][vl][>l][^b]T",  // Leaves with central bud
	     		          "[<b][vb][>b][^l]T",  // Buds with central leaf
	     		          "[<f][vf][>f][^b]T",  // Flowers with central bud

	     		          // Branching terminal
	     		          "[S[l]][S[b]]T",  // Leaf and bud on separate stems
	     		          "[S[l]][S[f]][S[b]]T", // Three separate structures

	     		          // Asymmetric terminals
	     		          "[<l][>b][>f]T",  // Lopsided arrangement
	     		          "[<l<l][>b]T",    // Multiple leaves on one side

	     		          // Dense terminals
	     		          "l<l<l<bT",  // Tight cluster
	     		          "b<b<f<fT",  // Dense buds and flowers
	     		          "l<l<l<l<lT", // Very dense leaves

	     		          // Sparse terminals
	     		          "SSlT",  // Long stem to leaf
	     		          "SSSbT", // Long stem to bud
	     		          "SSSfT", // Long stem to flower

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
	     	     } },

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

	     		          // Additional branch patterns:

	     		          // Dichotomous branching (even split)
	     		          "a[S[<T]][S[>T]]T",  // Even fork
	     		          "a[S[<ST]][S[>ST]]T", // Fork with stems
	     		          "a[/S[<T]][/S[>T]]T", // Twisted fork

	     		          // Monopodial (main axis continues)
	     		          "aSS[<T]ST",  // Side branch, main continues
	     		          "aSS[>T]ST",
				"aSS[<T][>T]ST", // Two side branches

	     		          // Sympodial (appears to fork)
	     		          "a[T]S[<T]T",  // Appears forked
	     		          "a[T]S[>T]T",
				"a[T]S[<T][>T]T",

	     		          // Tripartite branching
	     		          "a[<T][^T][>T]T",  // Three-way
	     		          "a[<ST][^ST][>ST]T",
				"a[/<T][/^T][/>T]T", // Twisted three-way

	     		          // Alternate branching patterns
	     		          "aS[<T]S[>T]S[<T]T",  // Zigzag
	     		          "aS[<T]/S[>T]/S[<T]T", // Spiral alternate
	     		          "aS[<T]vS[>T]^S[<T]T", // 3D alternate

	     		          // Whorled branching
	     		          "a[<T][vT][>T][^T]T",  // Four branches in whorl
	     		          "a[<ST][vST][>ST][^ST]T",
				"a[/<T][/vT][/>T][/^T]T", // Twisted whorl

	     		          // Tiered branching
	     		          "a[S[<T][>T]]S[<T][>T]T",  // Two tiers
	     		          "a[S[<T][>T]]S[S[<T][>T]]T", // Nested tiers

	     		          // Layered branching (like pagoda tree)
	     		          "aSS[<T]SS[>T]SS[<T]T",
				"aSS[<T]SS[>T]SS[<T]SS[>T]T",

	     		          // Drooping/weeping branches
	     		          "avS[<T]T",  // Downward branch
	     		          "avS[<T]vS[<T]T",  // Cascading
	     		          "avS[<T]vS[<T]vS[<T]T",

	     		          // Upright branches
	     		          "a^S[<T]T",  // Upward branch
	     		          "a^S[<T]^S[<T]T",

	     		          // Horizontal branches
	     		          "aS[<T]T",  // Horizontal
	     		          "aS[<T]S[<T]T",

	     		          // Irregular/assymetric patterns
	     		          "a[<T][<T][>T]T",  // Lopsided
	     		          "aS[<T]SS[>T]T",  // Uneven spacing

	     		          // Dense branching
	     		          "a[<T<][<T<][<T<]T",  // Tight cluster
	     		          "a[<T][<T][<T][<T][<T]T",

	     		          // Sparse branching
	     		          "aSSSS[<T]T",  // Long internode
	     		          "aSSSS[<T]SSSS[>T]T",

	     		          // Compound branching
	     		          "a[S[<T[<T]]]T",  // Branch on branch
	     		          "a[S[<T[<T[<T]]]]T",

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
	     	     } },

	     	     { 'C', new string[] { // treated as a stem segment
	     		          "D",
				"E",
				"SS",
				"S/S",
				"SvS",
				"S<S",
				"SS[B]",
				"S[B]S",
				"SS/[B]",
				"S/[B]S",
				"S[B]SD",
				"S/[B]SD",

				"SS[C]",
				"S[C]S",
				"SS/[C]",
				"S/[C]S",
				"S[C]SD",
				"S/[C]SD",

				"SS[D]",
				"S[D]S",
				"SS/[D]",
				"S/[D]S",
				"S[D]SD",
				"S/[D]SD",

				"SS[E]",
				"S[E]S",
				"SS/[E]",
				"S/[E]S",
				"S[E]SD",
				"S/[E]SD",

				"S[B][DT]S",
				"S[C][DT]ST",
				"S[D][DT]ST",
				"S[E][DT]ST",

				"S[B][BT]ST",
				"S[B][CT]ST",
				"S[B][DT]ST",
				"S[B][ET]ST",

				"S[C][BT]ST",
				"S[C][CT]ST",
				"S[C][DT]ST",
				"S[C][ET]ST",

				"S[D][BT]ST",
				"S[D][CT]ST",
				"S[D][DT]ST",
				"S[D][ET]ST",

				"S[E][BT]ST",
				"S[E][CT]ST",
				"S[E][DT]ST",
				"S[E][ET]ST",

				"S[B][CT][DT][ET]S",
				"S[B][CT][CT][BT]S",
				"S[B][DT][DT][BT]S",
				"S[B][ET][ET][BT]S",
				"aST",

				"DT",
				"ET",
				"SST",
				"S/ST",
				"SvST",
				"S<ST",
				"SS[B]T",
				"S[B]ST",
				"S[B]SCT",
				"SS[C]T",
				"S[C]ST",
				"S[C]ST",
				"S[B][CT]ST",
				"S[B][CT][DT][ET]ST",
				"aST",

				"Db",
				"Eb",
				"SSb",
				"S/Sb",
				"SvSb",
				"S<Sb",
				"SS[B]b",
				"S[B]Sb",
				"S[B]SCb",
				"SS[C]b",
				"S[C]Sb",
				"S[C]Sb",
				"S[B][CT]Sb",
				"S[B][CT][DT][ET]Sb",
				"aSb",
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

	     		          // Additional branch patterns:

	     		          // Dichotomous branching (even split)
	     		          "a[S[<T]][S[>T]]T",  // Even fork
	     		          "a[S[<ST]][S[>ST]]T", // Fork with stems
	     		          "a[/S[<T]][/S[>T]]T", // Twisted fork

	     		          // Monopodial (main axis continues)
	     		          "aSS[<T]ST",  // Side branch, main continues
	     		          "aSS[>T]ST",
				"aSS[<T][>T]ST", // Two side branches

	     		          // Sympodial (appears to fork)
	     		          "a[T]S[<T]T",  // Appears forked
	     		          "a[T]S[>T]T",
				"a[T]S[<T][>T]T",

	     		          // Tripartite branching
	     		          "a[<T][^T][>T]T",  // Three-way
	     		          "a[<ST][^ST][>ST]T",
				"a[/<T][/^T][/>T]T", // Twisted three-way

	     		          // Alternate branching patterns
	     		          "aS[<T]S[>T]S[<T]T",  // Zigzag
	     		          "aS[<T]/S[>T]/S[<T]T", // Spiral alternate
	     		          "aS[<T]vS[>T]^S[<T]T", // 3D alternate

	     		          // Whorled branching
	     		          "a[<T][vT][>T][^T]T",  // Four branches in whorl
	     		          "a[<ST][vST][>ST][^ST]T",
				"a[/<T][/vT][/>T][/^T]T", // Twisted whorl

	     		          // Tiered branching
	     		          "a[S[<T][>T]]S[<T][>T]T",  // Two tiers
	     		          "a[S[<T][>T]]S[S[<T][>T]]T", // Nested tiers

	     		          // Layered branching (like pagoda tree)
	     		          "aSS[<T]SS[>T]SS[<T]T",
				"aSS[<T]SS[>T]SS[<T]SS[>T]T",

	     		          // Drooping/weeping branches
	     		          "avS[<T]T",  // Downward branch
	     		          "avS[<T]vS[<T]T",  // Cascading
	     		          "avS[<T]vS[<T]vS[<T]T",

	     		          // Upright branches
	     		          "a^S[<T]T",  // Upward branch
	     		          "a^S[<T]^S[<T]T",

	     		          // Horizontal branches
	     		          "aS[<T]T",  // Horizontal
	     		          "aS[<T]S[<T]T",

	     		          // Irregular/assymetric patterns
	     		          "a[<T][<T][>T]T",  // Lopsided
	     		          "aS[<T]SS[>T]T",  // Uneven spacing

	     		          // Dense branching
	     		          "a[<T<][<T<][<T<]T",  // Tight cluster
	     		          "a[<T][<T][<T][<T][<T]T",

	     		          // Sparse branching
	     		          "aSSSS[<T]T",  // Long internode
	     		          "aSSSS[<T]SSSS[>T]T",

	     		          // Compound branching
	     		          "a[S[<T[<T]]]T",  // Branch on branch
	     		          "a[S[<T[<T[<T]]]]T",

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
	     	     } },

	     	     { 'D', new string[] { // threated as a stem segment
	     		          "C",
				"E",
				"SS",
				"S/S",
				"SvS",
				"S<S",
				"SS[B]",
				"S[B]S",
				"SS/[B]",
				"S/[B]S",
				"S[B]SD",
				"S/[B]SD",

				"SS[C]",
				"S[C]S",
				"SS/[C]",
				"S/[C]S",
				"S[C]SD",
				"S/[C]SD",

				"SS[D]",
				"S[D]S",
				"SS/[D]",
				"S/[D]S",
				"S[D]SD",
				"S/[D]SD",

				"SS[E]",
				"S[E]S",
				"SS/[E]",
				"S/[E]S",
				"S[E]SD",
				"S/[E]SD",

				"S[B][DT]S",
				"S[C][DT]ST",
				"S[D][DT]ST",
				"S[E][DT]ST",

				"S[B][BT]ST",
				"S[B][CT]ST",
				"S[B][DT]ST",
				"S[B][ET]ST",

				"S[C][BT]ST",
				"S[C][CT]ST",
				"S[C][DT]ST",
				"S[C][ET]ST",

				"S[D][BT]ST",
				"S[D][CT]ST",
				"S[D][DT]ST",
				"S[D][ET]ST",

				"S[E][BT]ST",
				"S[E][CT]ST",
				"S[E][DT]ST",
				"S[E][ET]ST",

				"S[B][CT][DT][ET]S",
				"S[B][CT][CT][BT]S",
				"S[B][DT][DT][BT]S",
				"aST",

				"DT",
				"ET",
				"SST",
				"S/ST",
				"SvST",
				"S<ST",
				"SS[B]T",
				"S[B]ST",
				"S[B]SCT",
				"SS[D]T",
				"S[D]ST",
				"S[D]ST",
				"S[B][CT][DT][ET]ST",
				"aST",

				"Db",
				"Eb",
				"SSb",
				"S/Sb",
				"SvSb",
				"S<Sb",
				"SS[B]b",
				"S[B]Sb",
				"S[B]SCb",
				"SS[C]b",
				"S[C]Sb",
				"S[C]Sb",
				"S[B][CT]Sb",
				"S[B][CT][DT][ET]Sb",
				"aSb",
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

	     		          // Additional branch patterns:

	     		          // Dichotomous branching (even split)
	     		          "a[S[<T]][S[>T]]T",  // Even fork
	     		          "a[S[<ST]][S[>ST]]T", // Fork with stems
	     		          "a[/S[<T]][/S[>T]]T", // Twisted fork

	     		          // Monopodial (main axis continues)
	     		          "aSS[<T]ST",  // Side branch, main continues
	     		          "aSS[>T]ST",
				"aSS[<T][>T]ST", // Two side branches

	     		          // Sympodial (appears to fork)
	     		          "a[T]S[<T]T",  // Appears forked
	     		          "a[T]S[>T]T",
				"a[T]S[<T][>T]T",

	     		          // Tripartite branching
	     		          "a[<T][^T][>T]T",  // Three-way
	     		          "a[<ST][^ST][>ST]T",
				"a[/<T][/^T][/>T]T", // Twisted three-way

	     		          // Alternate branching patterns
	     		          "aS[<T]S[>T]S[<T]T",  // Zigzag
	     		          "aS[<T]/S[>T]/S[<T]T", // Spiral alternate
	     		          "aS[<T]vS[>T]^S[<T]T", // 3D alternate

	     		          // Whorled branching
	     		          "a[<T][vT][>T][^T]T",  // Four branches in whorl
	     		          "a[<ST][vST][>ST][^ST]T",
				"a[/<T][/vT][/>T][/^T]T", // Twisted whorl

	     		          // Tiered branching
	     		          "a[S[<T][>T]]S[<T][>T]T",  // Two tiers
	     		          "a[S[<T][>T]]S[S[<T][>T]]T", // Nested tiers

	     		          // Layered branching (like pagoda tree)
	     		          "aSS[<T]SS[>T]SS[<T]T",
				"aSS[<T]SS[>T]SS[<T]SS[>T]T",

	     		          // Drooping/weeping branches
	     		          "avS[<T]T",  // Downward branch
	     		          "avS[<T]vS[<T]T",  // Cascading
	     		          "avS[<T]vS[<T]vS[<T]T",

	     		          // Upright branches
	     		          "a^S[<T]T",  // Upward branch
	     		          "a^S[<T]^S[<T]T",

	     		          // Horizontal branches
	     		          "aS[<T]T",  // Horizontal
	     		          "aS[<T]S[<T]T",

	     		          // Irregular/assymetric patterns
	     		          "a[<T][<T][>T]T",  // Lopsided
	     		          "aS[<T]SS[>T]T",  // Uneven spacing

	     		          // Dense branching
	     		          "a[<T<][<T<][<T<]T",  // Tight cluster
	     		          "a[<T][<T][<T][<T][<T]T",

	     		          // Sparse branching
	     		          "aSSSS[<T]T",  // Long internode
	     		          "aSSSS[<T]SSSS[>T]T",

	     		          // Compound branching
	     		          "a[S[<T[<T]]]T",  // Branch on branch
	     		          "a[S[<T[<T[<T]]]]T",

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
	     	     } },

	     	     { 'E', new string[] { // threated as a stem segment
	     		          "C",
				"D",
				"SS",
				"S/S",
				"SvS",
				"S<S",
				"SS[B]",
				"S[B]S",
				"SS/[B]",
				"S/[B]S",
				"S[B]SD",
				"S/[B]SD",

				"SS[C]",
				"S[C]S",
				"SS/[C]",
				"S/[C]S",
				"S[C]SD",
				"S/[C]SD",

				"SS[D]",
				"S[D]S",
				"SS/[D]",
				"S/[D]S",
				"S[D]SD",
				"S/[D]SD",

				"SS[E]",
				"S[E]S",
				"SS/[E]",
				"S/[E]S",
				"S[E]SD",
				"S/[E]SD",

				"S[B][DT]S",
				"S[C][DT]ST",
				"S[D][DT]ST",
				"S[E][DT]ST",

				"S[B][BT]ST",
				"S[B][CT]ST",
				"S[B][DT]ST",
				"S[B][ET]ST",

				"S[C][BT]ST",
				"S[C][CT]ST",
				"S[C][DT]ST",
				"S[C][ET]ST",

				"S[D][BT]ST",
				"S[D][CT]ST",
				"S[D][DT]ST",
				"S[D][ET]ST",

				"S[E][BT]ST",
				"S[E][CT]ST",
				"S[E][DT]ST",
				"S[E][ET]ST",

				"S[B][CT][DT][ET]S",
				"S[B][CT][CT][BT]S",
				"S[B][DT][DT][BT]S",
				"S[B][ET][ET][BT]S",
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

	     		          // Additional branch patterns:

	     		          // Dichotomous branching (even split)
	     		          "a[S[<T]][S[>T]]T",  // Even fork
	     		          "a[S[<ST]][S[>ST]]T", // Fork with stems
	     		          "a[/S[<T]][/S[>T]]T", // Twisted fork

	     		          // Monopodial (main axis continues)
	     		          "aSS[<T]ST",  // Side branch, main continues
	     		          "aSS[>T]ST",
				"aSS[<T][>T]ST", // Two side branches

	     		          // Sympodial (appears to fork)
	     		          "a[T]S[<T]T",  // Appears forked
	     		          "a[T]S[>T]T",
				"a[T]S[<T][>T]T",

	     		          // Tripartite branching
	     		          "a[<T][^T][>T]T",  // Three-way
	     		          "a[<ST][^ST][>ST]T",
				"a[/<T][/^T][/>T]T", // Twisted three-way

	     		          // Alternate branching patterns
	     		          "aS[<T]S[>T]S[<T]T",  // Zigzag
	     		          "aS[<T]/S[>T]/S[<T]T", // Spiral alternate
	     		          "aS[<T]vS[>T]^S[<T]T", // 3D alternate

	     		          // Whorled branching
	     		          "a[<T][vT][>T][^T]T",  // Four branches in whorl
	     		          "a[<ST][vST][>ST][^ST]T",
				"a[/<T][/vT][/>T][/^T]T", // Twisted whorl

	     		          // Tiered branching
	     		          "a[S[<T][>T]]S[<T][>T]T",  // Two tiers
	     		          "a[S[<T][>T]]S[S[<T][>T]]T", // Nested tiers

	     		          // Layered branching (like pagoda tree)
	     		          "aSS[<T]SS[>T]SS[<T]T",
				"aSS[<T]SS[>T]SS[<T]SS[>T]T",

	     		          // Drooping/weeping branches
	     		          "avS[<T]T",  // Downward branch
	     		          "avS[<T]vS[<T]T",  // Cascading
	     		          "avS[<T]vS[<T]vS[<T]T",

	     		          // Upright branches
	     		          "a^S[<T]T",  // Upward branch
	     		          "a^S[<T]^S[<T]T",

	     		          // Horizontal branches
	     		          "aS[<T]T",  // Horizontal
	     		          "aS[<T]S[<T]T",

	     		          // Irregular/assymetric patterns
	     		          "a[<T][<T][>T]T",  // Lopsided
	     		          "aS[<T]SS[>T]T",  // Uneven spacing

	     		          // Dense branching
	     		          "a[<T<][<T<][<T<]T",  // Tight cluster
	     		          "a[<T][<T][<T][<T][<T]T",

	     		          // Sparse branching
	     		          "aSSSS[<T]T",  // Long internode
	     		          "aSSSS[<T]SSSS[>T]T",

	     		          // Compound branching
	     		          "a[S[<T[<T]]]T",  // Branch on branch
	     		          "a[S[<T[<T[<T]]]]T",

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
	     	     } },
	     	     // ========== Direction Modifiers ==========
	     	     { '>', new string[] {
				">",        // Small right turn

	     	     } },

	     	     { '<', new string[] {
				"<",        // Small left turn

	     	     } },

	     	     { 'v', new string[] {
				"v",        // Downward adjustment
	     	     } },

	     	     // ========== Reproductive Structures ==========
	     	     { 'c', new string[] {
				"SLA",        // Simple cotyledon
	     		          "S[+++L][---L]A",     // Opposite cotyledons
	     				              //"LL",
	     				              // "L"
	     	     } },

	     	     { 's', new string[] {
				"r-S[BT]c",     // Basic seed
	     		          "r-S<[[<B]>[>B]]c",
				"r-S[[<B]>[>B]vBSc]",

				"r-S[CT]c",     // Basic seed
	     		          "r-S<[[<C]>[>C]]c",
				"r-S[[<C]>[>C]vCSc]",

				"r-S[DT]c",     // Basic seed
	     		          "r-S<[[<D]>[>D]]c",
				"r-S[[<D]>[>D]vDSc]",

				"r-S[ET]c",     // Basic seed
	     		          "r-S<[[<E]>[>E]]c",
				"r-S[[<E]>[>E]vESc]",

				"r-S<[[<B]>[>B]]c",
				"r-S<[[<C]>[>C]]c",
				"r-S<[[<D]>[>D]]c",
				"r-S<[[<E]>[>E]]c",

				"r-S[[<B]>[>B]vBSc]",
				"r-S[[<C]>[>C]vCSc]",
				"r-S[[<D]>[>D]vDSc]",
				"r-S[[<E]>[>E]vESc]",

	     	     } },

	     	     { 'b', new string[] {
				"[Sf]",                    // Simple flower on pedicel
	     		          "[<Sf]",                   // Lateral flower
	     		          "[<Sf][>Sf]",              // Paired flowers
	     		          "[<Sf][vSf][>Sf]",         // Whorl of 3 flowers
	     		          "[<Sf][vSf][>Sf][^Sf]",    // Complete whorl (4)

	     		          // Raceme/panicle structures
	     		          "[Sf]b",                   // Terminal flower with continuing bud
	     		          "[Sf][Sf]b",               // Multiple flowers then bud
	     		          "[S[Sf][Sf]]b",            // Branched inflorescence

	     		          // Flower clusters
	     		          "[fSfSf]",                   // Cluster along stem
	     		          "[f<f<f<f]",                 // Dense cluster
	     		          "[</f</f</f]",               // Spiral cluster

	     		          // Flowers with leaves/bracts
	     		          "[l][Sf]",                 // Leaf then flower
	     		          "[<l][Sf][>l]",            // Flower between leaves
	     		          "lS[Sf]",                  // Leaf subtending flower

	     		          // Terminal flower structures
	     		          "[f]",                       // Terminal flower (no pedicel)
	     		          "[/Sf]",                      // Terminal flower on stem
	     		          "[S[Sf]]",                 // Flower on branched pedicel
			            "[b/S[Sf]]",
			             "[/S[bSf]]",      

	     		          // Compound inflorescences
	     		          "[<[Sf]][>[Sf]]",          // Compound umbel
	     		          "[S[Sf][Sf][Sf]]",         // Panicle
	     		          "[<Sf]S[>Sf]",             // Zigzag inflorescence
	     	     } },

	     	     { 'f', new string[] {
				"F",                       // Mature to fruit
	     		          "[-S-F]",                     // Develops into large fruit
	     		          "[S-F]",                   // Fruit on elongated pedicel
	     		          "vS-F",                    // Pendulous fruit
	     		          "[<F][>F]",                // Flower produces paired fruits
	     	     } },

	     	     { 'F', new string[] {
				"f",        // Fruit
	     			       //"b",
	     			       //"bl",       // Fruit with flower
	     		          ">l<<f",       // Fruit with leaf

	     	     } },

	     	     // ========== Root System ==========
	     	     { 'r', new string[] {
				"r",        // Primary root

	     	     } }
	     };
}
