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
	     public string ruleSetName = "L_System";
	     public Dictionary<char, string[]> rules = new Dictionary<char, string[]>();
	     public Dictionary<char, string[]> activeRules = new Dictionary<char, string[]>();
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

