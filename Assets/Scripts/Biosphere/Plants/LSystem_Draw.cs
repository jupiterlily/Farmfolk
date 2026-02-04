using System.Collections.Generic;
using UnityEngine;

public class LSystem_Draw : MonoBehaviour
{
	     //this script takes a plant class and derives a colour palette and a size guide, it creates then, at a prescribed location, it draws the plant according to the axiom provided in the branch dictionary and the size and colour data.

	     //function that takes a string and aposition and draws a line according to the number of S segments and directional arrows 
	     public void DrawPlant(Plant plant, Vector3 position)
	     {
		          //leaf colour, size and shape
		          Color leafColor;
		          float leafSize;
		          //stem colour, size and shape
		          Color stemColor;
		          float stemLength;
		          float stemWidth;
		          //flower colour, size and shape
		          Color flowerColor;
		          float flowerSize;
		          //fruit colour, size and shape
		          Color fruitColor;
		          float fruitSize;

		          PlantDrawingContext drawingContext = new PlantDrawingContext();


		          //Create a dictionary to hold the branch segment strings
		          Dictionary<string, string> branchMap = LSystem_Utilities.ParseAxiomBranches(plant.lSystem.axiom);
		          //Create a dictionary to hold the branch segment and node locations
		          Dictionary<string, Vector3> branchLocations = new Dictionary<string, Vector3>();

		          //go through each branch in branch map and calculate the location of each branch segment and add it to the branch locations dictionary
		          foreach (var branch in branchMap)
		          {
				DrawBranch(branch.Key, branchLocations[branch.Key]);
		          }
	     }

	     //function that takes a string and aposition and draws a line according to the number of S segments and directional arrows
	     public GameObject DrawBranch(string axiom, Vector3 position)
	     {
		          if (string.IsNullOrEmpty(axiom))
		          {
				Debug.LogError("Cannot draw branch, axiom is null or empty.");
				return null;

		          }
		          // Create a new GameObject for the branch 
		          return new GameObject() { name = axiom };
	     }

	     //create stem part
	     public LineRenderer CreateStemPart(Vector3[] positions, float width, Color color)
	     {

		          GameObject stemPart = new GameObject("Stem");
		          LineRenderer lineRenderer = stemPart.AddComponent<LineRenderer>();
		          lineRenderer.startWidth = width;
		          lineRenderer.endWidth = width;
		          lineRenderer.startColor = color;
		          lineRenderer.endColor = color;
		          lineRenderer.positionCount = positions.Length;

		          int index = 0;
		          foreach (var position in positions)
		          {
				lineRenderer.SetPosition(index, position);
				index++;
		          }
		          return lineRenderer;
	     }

	     //create leaf part

	     //create bud part

	     //create flower part

	     //create fruit part
}


