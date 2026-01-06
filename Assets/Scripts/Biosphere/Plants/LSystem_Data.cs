using System.Collections.Generic;
using UnityEngine;

public class LSystem_Data
{
	     Dictionary<char, string[]> rules = new Dictionary<char, string[]>();
	     public string axiom = "s";
	     public float iterationScaleFactor = 0.9f;
}
public class AxiomCharacter
{
	     //this struct holds the character
	     public char character;
	     enum Type
	     {
		          Movement, //Forward, S for stem, R for root, Z for rhizome or aerial root
		          Node, //Used to reference world position for branches etc.
		          PartSite, //used to indicate that a part will be drawn at this location ie l or b
		          Part, //Part to be drawn in (Stem, Leaf, Flower, Fruit)
		          Brackets, //Used to string position for branches etc.
		          Branch, //Branch structure
		          GrowthPoint,
		          PointDirection, // points the branch towards a specific point ie the sun
		          TurnDirection, //rotates the branch around a specific axis
		          InvertDirection, //inverts the branch angle on a specific axis
		          Memory, //used to store and restore structures

	     }

}
public class AxiomMovement : AxiomCharacter
{
	     public float distance;
	     public AxiomMovement(float dist, char c)
	     {
		          distance = 1 * dist;
		          character = c;
	     }
}
public class AxiomNode : AxiomCharacter
{

	     public Vector3 position;
	     public AxiomNode(Vector3 pos, char c)
	     {

		          position = pos;
		          character = c;
	     }
}
public class AxiomDirection : AxiomCharacter
{

	     public Vector3 direction;
	     public AxiomDirection(Vector3 dir, char c)
	     {

		          direction = dir;
		          character = c;
	     }
}
public class AxiomPrimordia : AxiomCharacter
{

	     public Vector3 position;
	     public AxiomPrimordia(Vector3 pos, char c)
	     {


		          position = pos;
		          character = c;
	     }
}
public struct PlantDrawingContext
{
	     internal bool isNewBranch;
	     public int branchDepth;
	     public Stack<TransformInfo> stack;
	     public GameObject currentBranch;
	     public Vector3 currentPosition;
	     public Vector3 direction;
	     public float length;
	     public float totalDistance;
	     public List<KeyValuePair<Vector3, float>> positions;
	     public int totalSCount;
	     public Vector3 targetUpDirection;
	     public LSystem_Branch parent;
	     public LSystem_Branch[] children;
}

public struct TransformInfo
{
	     public Vector3 position;
	     public Vector3 direction;
	     public GameObject currentBranch;
	     public List<KeyValuePair<Vector3, float>> positions;
	     public float totalDistance;
	     public int totalSCount;
	     public AnimationCurve widthCurve;

	     public TransformInfo(Vector3 position, Vector3 direction, GameObject currentBranch,
		        List<KeyValuePair<Vector3, float>> positions, float totalDistance,
		        int totalSCount, AnimationCurve widthCurve)
	     {
		          this.position = position;
		          this.direction = direction;
		          this.currentBranch = currentBranch;
		          this.positions = positions;
		          this.totalDistance = totalDistance;
		          this.totalSCount = totalSCount;
		          this.widthCurve = widthCurve;
	     }
}
public class LSystem_Ruleset
{
	     public string ruleSetName;
	     public Dictionary<char, string[]> rules = new Dictionary<char, string[]>();
	     public static Dictionary<char, string[]> PlantMasterRules = new Dictionary<char, string[]>()
{
    // ========== Growth Points ==========
    { 'A', new string[] { // Apical bud. Must contain a B and an A. When adding this to plant rulesets, iterate over them to add stems and turns and branch variety ie (B becomes [B]B, l, b, T, CT or DT or ET)
	     "SA",       // Continue growing
	     "B*A",
	     "BS*A",
	     "SB*A",
	     "S[B]*SA",

	     "/SA",       // Continue growing
	     "B*A",
	     "B/S*A",
	     "/SB*A",
	     "/S[B]*/SA",
	     //double branch /structure/s
	     "/S[B][B]*A", //two branche/s
	     "/S[B]B]]*A", //ne/sted branche/s
	     "/S[B]/S[B]*A", //branche/s along /stem
	     //triple branch /structure/s
	     "/S[B][B][B]*A", //three branche/s
	     "/S[[[B]B]B]*A", //ne/sted branche/s
	     "/S[[B]B][B]*A",
	     "/S[B][[B]B]*A",
	     "/S[B]B]]/S[B]*A",
	     "/S[B]/S[B]B]]*A",
	     "/S[/[/[/SB]SB]SB]*A",

	     //four branch /structure/s
	     "/S[B][B][B][B]*A", //four branche/s
	     "/S[[B]B][[B]B]*A", //ne/sted branche/s
	     "/S[B][[B]B][B]*A", //alternating ne/sted branche/s

	     //five branch /structure/s
	     "/S[B][B][B][B][B]*A", //five branche/s
	     "/S[[B]B][[B]B][B]*A", //ne/sted branche/s
	     "/S[B][[B]B][[B]B]*A", //alternating ne/sted branche/s
	     "/S[[[B]B]B][B][B]*A", //alternating ne/sted branche/s
	     "/S[[[B]B]B][[B[B]*A", //alternating ne/sted branche/s

    }},

    { 'a', new string[] { // Axillary bud
        "a",
        "aT",       // 
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
    }},
    
    // ========== Branching Structures ==========
    { 'B', new string[] { // Primary branch
	     "aST",
	     //single branch structures
	     "a-[-B]T",
	     "aS-[-B]T",
	     //two branch structures
	     "a[-B][-B]T",
	     "a[[-B]-B]T",
	     "a[-B[-B]]T",
	     "aS[<B]S[<B]T",
	     "aS/[<B]/S[>B]T",
	     "aS[<B[<B]]T",
	     //three branch structures
	     "aS[<B][>B][<^B]T",
	     "aS[<B]S[<B]S[<B]T",
	     "aS[<B]/S[<B]/S[<B]T",
	     "aS[<B[<B[<B]]]T",
	     //complex branch structures
	     "a<S[>CT]/S[^DT]ST",
	     "a<S[+[B]/C/D/ET]T", //spiral
	     "a<S[+[B]/C/[B]/CT]T" //spiral
    }},

    { 'C', new string[] { // threated as a stem segment
	         "C",
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
    }},

    { 'D', new string[] { // Tertiary branch
	         "SS"
    }},

    { 'E', new string[] { // Quaternary branch
	         "SS"
    }},
    
    // ========== Reproductive Structures ==========
    { 'b', new string[] { // Flower bud
	         // Single flower structures
        "f",       
        //double flower structures
        "ff",
        "[<Pf][>Pf]",
	         "[+Pf[+Pf]]",
	         //triple flower structures
	         "fff",
	         "[>Pf][Pf][<Pf]",
	         "[>>Pf][Pf][<<Pf]",
	         "P[+Pf]P[+Pf]P[+Pf]",
	         "[+Pf[+Pf[+Pf]]]",
	         "[/Pf[/Pf[/Pf]]]",	         
	         // four flower structures
	         //five flower structures
	         //recursive flower patterns
	         "[+Pfb]", //waterfall flower
	         "[+Pf[/Pfb]]",
	         "+Pf>Pfb",

    }},

    { 'f', new string[] { // Flower
        "F",       // Develop into fruit
    }},

    { 'F', new string[] { // Fruit
        "F",       // Persistent fruit
        "F<F",      // Fruit cluster
        "[F]",     // Stem fruit
        "F+"       // Geotropic fruit
    }},

    { 'c', new string[] { "cd", "ce", "cdf" }}, // Flower parts
    { 'd', new string[] { "de", "df", "dc" }},  // Flower sequences
    { 'e', new string[] { "ef", "ec", "ed" }},  // Flower variations
    
    // ========== Leaf Structures ==========
    
    { 'l', new string[] { // Leaf structure

    }},

    { 'L', new string[] { // Leaf indicator
        "H",       // Young leaf
    }},

    { 'H', new string[] { "H", "I" }}, // Leaf aging sequence
    { 'I', new string[] { "I", "J" }},
    { 'J', new string[] { "J", "K" }},
    { 'K', new string[] { "K", "L" }},

    { 'h', new string[] { "h", "i" }}, // Leaflet sequences
    { 'i', new string[] { "i", "j" }},
    { 'j', new string[] { "j", "k" }},
    { 'k', new string[] { "k" }},

    { 'p', new string[] { // Petiole structure
        "P"
    }},

    { 'P', new string[] { // Mature petiole
        "P",
        "P+",      // Geotropic adjustment
        "P*"       // Phototropic adjustment
    }},
    
    // ========== Stem Structures ==========
    { 'S', new string[] { // Stem
        "S",       // Minimal growth
        "SS",      // Extended growth
        "S/S",     // Spiral growth        
    }},

	      { 'X', new string[] { // Stem segment
		           "S[[B][B]]S"
	     }},

	     { 'Y', new string[] { // Stem segment
		          "S[B][B]"
	     }},

	    // { 'Z', new string[] { // Stem segment
	//	          "[B]S[B]"
	   //  }},

{ 'n', new string[] { // Node
        "N",       // Prominent node
        "[l]N",       // Minimal node
    }},

    { 'N', new string[] { // Prominent node
        "N",
    }},
    
    // ========== Root Structures ==========
    { 'r', new string[] { // Root branch
        "RRr",
    }},
        { 'R', new string[] { // Primary root
        "R",
        "RR"
    }},

    { 'z', new string[] { // Rhizome structure
        "z",
        "Z",
    }},

    { 'Z', new string[] { // Rhizome
        "Z",
        "ZZ",
        "Z[-s]",
        "ZZ[-s]"    // Rhizome with shoot
    }},
    
    // ========== Seed Structures ==========
    { 's', new string[] { // Seed
        "r=*A",
        "r=*SLA",
        "r=*SLLA"

    }},
    
    // ========== Direction Modifiers ==========
    { '^', new string[] { "^", "^^", "^+" }}, // X-axis up
    { 'v', new string[] { "v", "vv", "v+" }}, // X-axis down
    { '<', new string[] { "<", "<<", "<+" }}, // Y-axis left
    { '>', new string[] { ">", ">>", ">+" }}, // Y-axis right
    { '/', new string[] { "/", "//", "/+" }}, // Z-axis twist
    { '|', new string[] { "|", "||", "|+" }}, // Z-axis counter-twist
    { '+', new string[] { "+", "++" }},       // Toward gravity
    { '-', new string[] { "-", "--" }},       // Away gravity
    { '*', new string[] { "*", "**" }}        // Toward light
};

}
public class LSystem_Branch
{
	     public string axiom;
	     public LSystem_Branch parent;
	     public LSystem_Branch[] children;
}

// a class that holds the parameters for the branch width. can probably fuck off.
public class BranchWidthParameters
{
	     public float baseWidth;
	     public float widthReductionFactor;
	     public int depth;
	     public AnimationCurve widthCurve;

	     // Constructor
	     public BranchWidthParameters CreateChildParameters()
	     {
		          // Calculate the width reduction factor based on the depth
		          return new BranchWidthParameters
		          {
				baseWidth = this.baseWidth * widthReductionFactor,
				widthReductionFactor = this.widthReductionFactor,
				depth = this.depth + 1,
				widthCurve = new AnimationCurve(this.widthCurve.keys)
		          };
	     }
}



