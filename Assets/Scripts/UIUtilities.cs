using System.Collections.Generic;
using UnityEngine;

public static class UIUtilities
{
	     public static Graph GraphFloats(List<float> values, float max, GraphType type = GraphType.radar, float radius = 1f)
	     {
		          if (values == null || values.Count == 0)
		          {
				Debug.LogWarning("Cannot graph empty values list");
				return new Graph { points = new Vector3[0], type = type };
		          }

		          switch (type)
		          {
		          case GraphType.radar:
		          return CreateRadarGraph(values, max, radius);
		          case GraphType.bar:
		          return CreateBarGraph(values, max);
		          case GraphType.line:
		          return CreateLineGraph(values, max);
		          case GraphType.pie:
		          return CreatePieChart(values, max, radius);
		          default:
		          return CreateRadarGraph(values, max, radius);
		          }
	     }

	     public static Graph GraphBools(List<bool> values, float max, GraphType type = GraphType.radar)
	     {
		          List<float> floatValues = new List<float>();
		          foreach (bool value in values)
		          {
				floatValues.Add(value ? max : 0f);
		          }
		          return GraphFloats(floatValues, max, type);
	     }

	     private static Graph CreateRadarGraph(List<float> values, float max, float radius)
	     {
		          int pointCount = values.Count;
		          Vector3[] points = new Vector3[pointCount + 1]; // +1 to close the shape

		          float angleIncrement = 360f / pointCount * Mathf.Deg2Rad;

		          for (int i = 0; i < pointCount; i++)
		          {
				float normalisedValue = Mathf.Clamp01(values[i] / max);
				float angle = i * angleIncrement;
				float x = Mathf.Cos(angle) * normalisedValue * radius;
				float y = Mathf.Sin(angle) * normalisedValue * radius;
				points[i] = new Vector3(x, y, 0);
		          }

		          // Close the shape by connecting back to the first point
		          points[pointCount] = points[0];

		          return new Graph { points = points, type = GraphType.radar };
	     }

	     private static Graph CreateBarGraph(List<float> values, float max)
	     {
		          int barCount = values.Count;
		          Vector3[] points = new Vector3[barCount * 4]; // 4 points per bar: bottom-left, bottom-right, top-right, top-left

		          float barWidth = 1f / barCount;

		          for (int i = 0; i < barCount; i++)
		          {
				float normalisedValue = Mathf.Clamp01(values[i] / max);
				float xPos = i * barWidth;

				// Define bar vertices (quad)
				points[i * 4] = new Vector3(xPos, 0, 0); // bottom-left
				points[i * 4 + 1] = new Vector3(xPos + barWidth, 0, 0); // bottom-right
				points[i * 4 + 2] = new Vector3(xPos + barWidth, normalisedValue, 0); // top-right
				points[i * 4 + 3] = new Vector3(xPos, normalisedValue, 0); // top-left
		          }

		          return new Graph { points = points, type = GraphType.bar };
	     }

	     private static Graph CreateLineGraph(List<float> values, float max)
	     {
		          Vector3[] points = new Vector3[values.Count];

		          float xStep = 1f / (values.Count - 1);

		          for (int i = 0; i < values.Count; i++)
		          {
				float normalisedValue = Mathf.Clamp01(values[i] / max);
				points[i] = new Vector3(i * xStep, normalisedValue, 0);
		          }

		          return new Graph { points = points, type = GraphType.line };
	     }

	     private static Graph CreatePieChart(List<float> values, float max, float radius)
	     {
		          // For pie chart, we need to create wedges
		          // This is more complex and might require a different approach
		          // For now, return a simple representation
		          List<Vector3> piePoints = new List<Vector3>();

		          float total = 0f;
		          foreach (float value in values)
		          {
				total += value;
		          }

		          float currentAngle = 0f;

		          foreach (float value in values)
		          {
				float percentage = value / total;
				float wedgeAngle = percentage * 360f * Mathf.Deg2Rad;
				int segments = Mathf.Max(3, Mathf.RoundToInt(percentage * 12f)); // More segments for larger wedges

				// Add center point
				piePoints.Add(Vector3.zero);

				// Add points around the arc
				for (int i = 0; i <= segments; i++)
				{
					     float angle = currentAngle + (wedgeAngle * i / segments);
					     float x = Mathf.Cos(angle) * radius;
					     float y = Mathf.Sin(angle) * radius;
					     piePoints.Add(new Vector3(x, y, 0));
				}

				currentAngle += wedgeAngle;
		          }

		          return new Graph { points = piePoints.ToArray(), type = GraphType.pie };
	     }

	     // Helper method to create HEXACO radar graph
	     public static Graph CreateHexacoGraph(HEXACO hexaco, GraphType type = GraphType.radar)
	     {
		          List<float> values = new List<float>();
		          PersonalityAspect[] aspects = hexaco.FullHexRange();

		          foreach (PersonalityAspect aspect in aspects)
		          {
				values.Add(aspect.value);
		          }

		          return GraphFloats(values, hexaco.maximumValue, type);
	     }

	     // Helper method to create Plutchik mood graph
	     public static Graph CreateMoodGraph(Mood mood, GraphType type = GraphType.radar)
	     {
		          List<float> values = new List<float>();
		          EmotionAspect[] aspects = mood.FullHexRange();

		          foreach (EmotionAspect aspect in aspects)
		          {
				values.Add(aspect.value);
		          }

		          return GraphFloats(values, 10f, type); // Assuming max of 10 for emotions
	     }
}

public class Graph
{
	     public Vector3[] points;
	     public GraphType type;//radar, bar, line, pie
	     public Color color = Color.white;
	     public float lineWidth = 0.1f;
}

public enum GraphType
{
	     radar, bar, pie, line
}