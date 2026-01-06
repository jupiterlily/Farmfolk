using System.Collections.Generic;
using System.Text;
using UnityEngine;

// these functions iterate over and/or parse the axiom string
public class LSystem_Utilities
{
	     //parse an axiom and consolidate the directional arrows ie ><><<>> becomes >
	     public static string CosolidateDirection(string str)
	     {
		          if (string.IsNullOrEmpty(str))
				return str;

		          string result = "";
		          int sequenceCount = 0;
		          int leftCount = 0;
		          int rightCount = 0;
		          int geotropicCount = 0;
		          int phototropicCount = 0;

		          for (int i = 0; i < str.Length; i++)
		          {
				if (str[i] == '<' || str[i] == '>' || str[i] == '^' || str[i] == 'v')
				{
					     if (str[i] == '<')
						          leftCount++;
					     else if (str[i] == '>')
						          rightCount++;
					     else if (str[i] == 'v')
						          geotropicCount++;
					     else if (str[i] == '^')
						          phototropicCount++;

					     sequenceCount++;
				}
				else
				{
					     // Process any accumulated directional symbols
					     if (sequenceCount > 0)
					     {
						          if (leftCount > rightCount)
						          {
								leftCount -= rightCount;
								result += new string('<', leftCount);
						          }
						          else if (rightCount > leftCount)
						          {
								rightCount -= leftCount;
								result += new string('>', rightCount);
						          }

						          if (geotropicCount > phototropicCount)
						          {
								geotropicCount -= phototropicCount;
								result += new string('v', geotropicCount);
						          }
						          else if (phototropicCount > geotropicCount)
						          {
								phototropicCount -= geotropicCount;
								result += new string('^', phototropicCount);
						          }

						          // Reset counters
						          sequenceCount = 0;
						          leftCount = 0;
						          rightCount = 0;
						          geotropicCount = 0;
						          phototropicCount = 0;
					     }

					     // Add the current non-directional character
					     result += str[i];
				}
		          }

		          // Handle any directional symbols at the end of the string
		          if (sequenceCount > 0)
		          {
				if (leftCount > rightCount)
				{
					     leftCount -= rightCount;
					     result += new string('<', leftCount);
				}
				else if (rightCount > leftCount)
				{
					     rightCount -= leftCount;
					     result += new string('>', rightCount);
				}

				if (geotropicCount > phototropicCount)
				{
					     geotropicCount -= phototropicCount;
					     result += new string('v', geotropicCount);
				}
				else if (phototropicCount > geotropicCount)
				{
					     phototropicCount -= geotropicCount;
					     result += new string('^', phototropicCount);
				}
		          }

		          return result;
	     }

	     // function to parse an axiom and return a dictionary of strings of the unested characters and their branch reference code. All branches off the main stem are A, B, C, D, etc, then subsequent branches are AA, AB, AC, AD, etc
	     public static Dictionary<string, string> ParseAxiomBranches(string axiom)
	     {
		          var branchMap = new Dictionary<string, string>();
		          var branchStack = new Stack<string>();
		          var currentBranch = new StringBuilder();
		          int branchDepth = 0;
		          int mainBranchCount = 0;

		          // Start with main trunk (branch code "0")
		          string currentBranchCode = "0";
		          branchStack.Push(currentBranchCode);

		          for (int i = 0; i < axiom.Length; i++)
		          {
				char c = axiom[i];

				if (c == '[')
				{
					     // Save current branch segment before nesting
					     if (currentBranch.Length > 0)
					     {
						          branchMap[currentBranchCode] = currentBranch.ToString();
						          currentBranch.Clear();
					     }

					     // Generate new branch code
					     if (branchDepth == 0)
					     {
						          // First-level branches: A, B, C...
						          currentBranchCode = ((char)(65 + mainBranchCount++)).ToString();
					     }
					     else
					     {
						          // Nested branches: parent code + new letter
						          currentBranchCode = branchStack.Peek() + ((char)(65 + GetChildCount(branchStack.Peek(), branchMap)));
					     }

					     branchStack.Push(currentBranchCode);
					     branchDepth++;
				}
				else if (c == ']')
				{
					     // Finalize current branch
					     if (currentBranch.Length > 0)
					     {
						          branchMap[currentBranchCode] = currentBranch.ToString();
						          currentBranch.Clear();
					     }

					     // Move back to parent branch
					     branchStack.Pop();
					     branchDepth--;
					     currentBranchCode = branchStack.Peek();
				}
				else
				{
					     // Add character to current branch (excluding nested branches)
					     currentBranch.Append(c);
				}
		          }

		          // Add the remaining main trunk segment
		          if (currentBranch.Length > 0)
		          {
				branchMap[currentBranchCode] = currentBranch.ToString();
		          }

		          return branchMap;
	     }

	     private static int GetChildCount(string parentCode, Dictionary<string, string> branchMap)
	     {
		          int count = 0;
		          foreach (var key in branchMap.Keys)
		          {
				if (key.StartsWith(parentCode) && key.Length == parentCode.Length + 1)
				{
					     count++;
				}
		          }
		          return count;
	     }

	     public static string ExpandString(string input, System.Random random, float scale, Dictionary<char, string[]> rules)
	     {
		          System.Text.StringBuilder newString = new System.Text.StringBuilder();

		          foreach (char c in input)
		          {
				if (rules.TryGetValue(c, out string[] replacements))
				{
					     // Calculate probability based on iteration depth
					     float applyRuleProbability = Mathf.Lerp(1f, 0.6f, scale);

					     if (random.NextDouble() < applyRuleProbability)
					     {
						          string chosen = replacements[random.Next(replacements.Length)];
						          newString.Append(chosen);
					     }
					     else
					     {
						          newString.Append(c); // Keep original character
					     }
				}
				else
				{
					     newString.Append(c);
				}
		          }

		          return newString.ToString();
	     }

	     public static string MirrorRule(string rule)
	     {
		          StringBuilder sb = new StringBuilder();
		          foreach (char c in rule)
		          {
				switch (c)
				{
				case '<':
				sb.Append('>');
				break;
				case '>':
				sb.Append('<');
				break;
				case 'v':
				sb.Append('^');
				break;
				case '^':
				sb.Append('v');
				break;
				case '/':
				sb.Append('|');
				break;
				case '|':
				sb.Append('/');
				break;
				default:
				sb.Append(c);
				break;
				}
		          }
		          return sb.ToString();
	     }

	     //take a plant and a branch dictionary and calculate a dictionary of branch references and locations.
	     //= is ground level, ^ is turn on x axes counter clockwise, v is turn on x axes clockwise, < is turn on y axes counter clockwise, > is turn on y axes clockwise, \ is turn on z axes counter clockwise, / is turn on z axes clockwise, + is turn towards gravity, - is turn away from gravity, * is turn towards light, S is grow by stem length, N is a node
	     public static Dictionary<string, Vector3> CalculateBranchLocations(Dictionary<string, string> branchMap, Vector3 startPosition, Vector3 startDirection, float sectionLength, float angleStep)
	     {

		          var branchLocations = new Dictionary<string, Vector3>();
		          var currentPosition = startPosition;
		          var direction = startDirection; // Initial direction is up
				            //
		          foreach (var branch in branchMap)
		          {
				string branchCode = branch.Key;
				string branchString = branch.Value;
				Vector3 branchDirection = direction;
				Vector3 branchStartPosition = currentPosition;
				branchLocations[branchCode] = branchStartPosition;
				// Iterate through each character in the branch string
				for (int i = 0; i < branchString.Length; i++)
				{
					     char plantPart = branchString[i];
					     //if character is a + or - turn the direction vector towards or away from gravity by the angle step
					     if (plantPart == '+')
					     {
						          branchDirection = Vector3.Lerp(branchDirection, Vector3.down, angleStep);
					     }
					     if (plantPart == '-')
					     {

						          branchDirection = Vector3.Lerp(branchDirection, Vector3.up, angleStep);
					     }
					     //if character is a directional arrow, turn the direction vector
					     if (plantPart == '<')
					     {

						          branchDirection = Quaternion.Euler(0, -angleStep, 0) * branchDirection;
					     }
					     else if (plantPart == '>')
					     {

						          branchDirection = Quaternion.Euler(0, angleStep, 0) * branchDirection;
					     }
					     else if (plantPart == '^')
					     {

						          branchDirection = Quaternion.Euler(-angleStep, 0, 0) * branchDirection;
					     }
					     else if (plantPart == 'v')
					     {

						          branchDirection = Quaternion.Euler(angleStep, 0, 0) * branchDirection;
					     }
					     else if (plantPart == '/')
					     {

						          branchDirection = Quaternion.Euler(0, 0, -angleStep) * branchDirection;
					     }
					     else if (plantPart == '|')
					     {

						          branchDirection = Quaternion.Euler(0, 0, angleStep) * branchDirection;
					     }


					     //if the character is a stem segment, move the current position vector in the direction of the direction vector
					     if (plantPart == 'S')
					     {
						          currentPosition += branchDirection * sectionLength;
					     }
					     //if the character is a node, add the current position to the branch locations dictionary
					     if (plantPart == 'N')
					     {

						          branchLocations[branchCode] = currentPosition;
					     }
				}
		          }
		          return branchLocations;


	     }


}