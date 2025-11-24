using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtilities
{

	     public static Graph GraphFloats(List<float> values, float max, GraphType type = GraphType.radar, float radius = 1f)
	     {
		          if (values == null || values.Count == 0)
		          {
				Debug.LogWarning("Cannot graph empty values list");
				return new Graph
				{
					     points = new Vector3[0],
					     type = type,
					     pointColors = new Color[0],
					     axisLabels = new string[0],
					     axisColors = new Color[0]
				};
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

	     private static Graph CreateRadarGraph(List<float> values, float max, float radius)
	     {
		          int pointCount = values.Count;
		          Graph graph = new Graph(pointCount + 1); // +1 for closing point

		          float angleIncrement = 360f / pointCount * Mathf.Deg2Rad;

		          for (int i = 0; i < pointCount; i++)
		          {
				float normalisedValue = Mathf.Clamp01(values[i] / max);
				float angle = i * angleIncrement;
				float x = Mathf.Cos(angle) * normalisedValue * radius;
				float y = Mathf.Sin(angle) * normalisedValue * radius;
				graph.points[i] = new Vector3(x, y, 0);
				graph.pointColors[i] = Color.white; // Default color
		          }

		          // Close the shape by connecting back to the first point
		          graph.points[pointCount] = graph.points[0];
		          graph.pointColors[pointCount] = graph.pointColors[0];

		          graph.type = GraphType.radar;
		          return graph;
	     }

	     private static Graph CreateBarGraph(List<float> values, float max)
	     {
		          int barCount = values.Count;
		          Graph graph = new Graph(barCount * 4);

		          float barWidth = 1f / barCount;

		          for (int i = 0; i < barCount; i++)
		          {
				float normalisedValue = Mathf.Clamp01(values[i] / max);
				float xPos = i * barWidth;

				// Define bar vertices (quad)
				graph.points[i * 4] = new Vector3(xPos, 0, 0); // bottom-left
				graph.points[i * 4 + 1] = new Vector3(xPos + barWidth, 0, 0); // bottom-right
				graph.points[i * 4 + 2] = new Vector3(xPos + barWidth, normalisedValue, 0); // top-right
				graph.points[i * 4 + 3] = new Vector3(xPos, normalisedValue, 0); // top-left

				// Set colors for this bar
				Color barColor = Color.HSVToRGB(i / (float)barCount, 0.8f, 0.8f);
				for (int j = 0; j < 4; j++)
				{
					     graph.pointColors[i * 4 + j] = barColor;
				}
		          }

		          graph.type = GraphType.bar;
		          return graph;
	     }

	     private static Graph CreateLineGraph(List<float> values, float max)
	     {
		          Graph graph = new Graph(values.Count);

		          float xStep = 1f / (values.Count - 1);

		          for (int i = 0; i < values.Count; i++)
		          {
				float normalisedValue = Mathf.Clamp01(values[i] / max);
				graph.points[i] = new Vector3(i * xStep, normalisedValue, 0);
				graph.pointColors[i] = Color.Lerp(Color.blue, Color.red, normalisedValue);
		          }

		          graph.type = GraphType.line;
		          return graph;
	     }

	     private static Graph CreatePieChart(List<float> values, float max, float radius)
	     {
		          // Calculate total points needed
		          int totalSegments = 0;
		          float total = 0f;
		          foreach (float value in values)
		          {
				total += value;
				float percentage = value / total;
				int segments = Mathf.Max(3, Mathf.RoundToInt(percentage * 12f));
				totalSegments += segments + 2; // +2 for center and closing point
		          }

		          List<Vector3> piePoints = new List<Vector3>();
		          List<Color> pieColors = new List<Color>();

		          float currentAngle = 0f;

		          for (int wedgeIndex = 0; wedgeIndex < values.Count; wedgeIndex++)
		          {
				float value = values[wedgeIndex];
				float percentage = value / total;
				float wedgeAngle = percentage * 360f * Mathf.Deg2Rad;
				int segments = Mathf.Max(3, Mathf.RoundToInt(percentage * 12f));

				Color wedgeColor = Color.HSVToRGB(wedgeIndex / (float)values.Count, 0.8f, 0.8f);

				// Add center point
				piePoints.Add(Vector3.zero);
				pieColors.Add(wedgeColor);

				// Add points around the arc
				for (int i = 0; i <= segments; i++)
				{
					     float angle = currentAngle + (wedgeAngle * i / segments);
					     float x = Mathf.Cos(angle) * radius;
					     float y = Mathf.Sin(angle) * radius;
					     piePoints.Add(new Vector3(x, y, 0));
					     pieColors.Add(wedgeColor);
				}

				currentAngle += wedgeAngle;
		          }

		          Graph graph = new Graph(piePoints.Count);
		          graph.points = piePoints.ToArray();
		          graph.pointColors = pieColors.ToArray();
		          graph.type = GraphType.pie;
		          return graph;
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

		          Graph graph = GraphFloats(values, hexaco.maximumValue, type);
		          graph.title = string.Join(", ", PepoUtilitiesPersonality.GetPersonalityName(hexaco).Take(3));
		          // Set HEXACO-specific colors
		          graph.axisLabels = new string[] { "H", "E", "X", "A", "C", "O" };
		          graph.axisColors = new Color[]
		          {
            Color.magenta,    // Honesty-Humility should be green
            Color.red,        // Emotionality is yellow
            Color.yellow,     // eXtraversion is peachy orange
            Color.green,      // Agreeableness is salmon pink
            Color.cyan,       // Conscientiousness is purple
            Color.blue        // Openness
		          };

		          // Set point colors based on values and axis colors
		          for (int i = 0; i < values.Count && i < graph.pointColors.Length; i++)
		          {
				float saturation = Mathf.Clamp01(values[i] / hexaco.maximumValue);
				graph.pointColors[i] = SaturateColor(graph.axisColors[i], saturation);
		          }

		          return graph;
	     }

	     // Helper method to create Plutchik mood graph
	     public static Graph CreateMoodGraph(Mood mood, GraphType type = GraphType.radar)
	     {
		          List<float> values = new List<float>();
		          EmotionAspect[] aspects = mood.FullHexRange();

		          //at the moment we're adding them in acrazy order, need to fix that later
		          //        foreach (EmotionAspect aspect in aspects)
		          //        {
		          //values.Add(aspect.value);
		          //        }
		          values.Add(mood.Fear.value);
		          values.Add(mood.Trust.value);
		          values.Add(mood.Joy.value);
		          values.Add(mood.Anticipation.value);
		          values.Add(mood.Anger.value);
		          values.Add(mood.Disgust.value);
		          values.Add(mood.Sadness.value);
		          values.Add(mood.Surprise.value);
		          //fear, trust. joy, anticipation, anger, disgust, sadness, surprise
		          Graph graph = GraphFloats(values, 10f, type);
		          //the label is the highest one by the degree.
		          graph.title = PepoUtilitiesPersonality.GetFeelingName(mood);
		          // Set mood-specific colors (Plutchik's wheel colors)
		          graph.axisLabels = new string[] { "fear", "", "joy", "", "anger", "", "woe", "" };
		          graph.axisColors = new Color[]
		          {
		          new Color(0,1,0.25f),       // Fear
		          new Color(0.25f,1,0),      // Trust
		          Color.yellow,     // Joy
		          new Color(1,0.25f,0),      // Anticipation
		          Color.red,        // Anger
		          new Color(0.5f,0,1),    // Disgust purple
		          Color.blue,       // Sadness blue 
		          new Color(0,0.75f,1),      // Surprise teal        
            
		          };

		          // Set point colors based on values
		          for (int i = 0; i < values.Count && i < graph.pointColors.Length; i++)
		          {
				float saturation = Mathf.Clamp01(values[i] / 10f);
				graph.pointColors[i] = SaturateColor(graph.axisColors[i], saturation);
		          }

		          return graph;
	     }

	     // Helper method to create Values radar graph
	     // Helper method to create Values radar graph
	     public static Graph CreateValuesGraph(List<Value> valuesList, GraphType type = GraphType.radar)
	     {
		          if (valuesList == null || valuesList.Count == 0)
		          {
				Debug.LogWarning("Cannot create values graph with empty values list");
				return new Graph
				{
					     points = new Vector3[0],
					     type = type,
					     pointColors = new Color[0],
					     axisLabels = new string[0],
					     axisColors = new Color[0]
				};
		          }

		          List<float> values = new List<float>();
		          foreach (Value value in valuesList)
		          {
				values.Add(value.value);
		          }

		          Graph graph = GraphFloats(values, 7f, type); // max value is 7 based on your basicHumanValues

		          // Set Values-specific colors by category
		          graph.axisLabels = new string[valuesList.Count];
		          graph.axisColors = new Color[valuesList.Count];

		          for (int i = 0; i < valuesList.Count; i++)
		          {
				Value currentValue = valuesList[i];
				graph.axisLabels[i] = GetValueAbbreviation(currentValue.name);
				graph.axisColors[i] = GetValueColor(currentValue.name);
		          }

		          // Set point colors based on values and axis colors
		          for (int i = 0; i < values.Count && i < graph.pointColors.Length; i++)
		          {
				float saturation = Mathf.Clamp01(values[i] / 7f); // Normalize to 0-1 based on max value of 7
				graph.pointColors[i] = SaturateColor(graph.axisColors[i], saturation);
		          }

		          return graph;
	     }

	     private static string GetValueAbbreviation(string valueName)
	     {
		          switch (valueName.ToLower())
		          {
		          case "agency":
		          return "Ag";
		          case "stimulation":
		          return "St";
		          case "hedonism":
		          return "He";
		          case "achievement":
		          return "Ac";
		          case "power":
		          return "Po";
		          case "security":
		          return "Se";
		          case "conformity":
		          return "Co";
		          case "tradition":
		          return "Tr";
		          case "benevolence":
		          return "Be";
		          case "universalism":
		          return "Un";
		          default:
		          return valueName.Substring(0, 2);
		          }
	     }

	     private static Color GetValueColor(string valueName)
	     {
		          switch (valueName.ToLower())
		          {
		          // Openness to change - Blues
		          case "agency":
		          return new Color(0.2f, 0.6f, 1f); // Bright blue
		          case "stimulation":
		          return new Color(0.4f, 0.8f, 1f); // Light blue

		          // Self-enhancement - Reds/Oranges
		          case "hedonism":
		          return new Color(1f, 0.4f, 0.2f); // Orange
		          case "achievement":
		          return new Color(1f, 0.6f, 0f); // Gold
		          case "power":
		          return new Color(1f, 0.2f, 0.2f); // Red

		          // Conservation - Greens
		          case "security":
		          return new Color(0.3f, 0.8f, 0.3f); // Green
		          case "conformity":
		          return new Color(0.5f, 0.9f, 0.5f); // Light green
		          case "tradition":
		          return new Color(0.1f, 0.6f, 0.1f); // Dark green

		          // Self-transcendence - Purples
		          case "benevolence":
		          return new Color(0.8f, 0.4f, 1f); // Purple
		          case "universalism":
		          return new Color(0.6f, 0.2f, 0.8f); // Dark purple

		          default:
		          return Color.gray;
		          }
	     }

	     private static Color SaturateColor(Color baseColor, float saturation)
	     {
		          Color.RGBToHSV(baseColor, out float h, out float s, out float v);
		          return Color.HSVToRGB(h, saturation, v);
	     }

	     public static Color[] GetPepoColors(BodyData body)
	     {
		          if (body == null)
				return new Color[4]; // Return empty array if no body data

		          // Get expressed traits
		          int skinHue = body.expressedChromosomes[10];
		          int skinSaturation = body.expressedChromosomes[9];
		          int skinDepth = body.expressedChromosomes[8];
		          int eyeColor = body.expressedChromosomes[4];
		          int eyeDarkness = body.expressedChromosomes[5];
		          int hairEumelanin = body.expressedChromosomes[1];
		          int hairPheomelanin = body.expressedChromosomes[2];

		          return new Color[4]
		          {
        GetHairColor(hairEumelanin, hairPheomelanin),    // Index 0
        GetEyeColor(eyeColor, eyeDarkness),              // Index 1
        GetSkinColor(skinHue, skinSaturation, skinDepth), // Index 2
        GetMouthColor(skinHue, skinSaturation, skinDepth) // Index 3
		          };
	     }
	     private static Color GetSkinColor(int hue, int saturation, int depth)
	     {
		          // Convert trait values (1-6) to HSV ranges
		          float h = MapRange(hue, 1, 6, 25f / 360, 40f / 360); // Full hue spectrum
		          float s = MapRange(saturation, 1, 6, 0.2f, 0.6f); // Moderate to high saturation
		          float v = MapRange(depth, 1, 6, 0.95f, 0.25f); // Dark to light

		          Color skinColor = Color.HSVToRGB(h, s, v);
		          return skinColor;
	     }

	     private static Color GetEyeColor(int color, int darkness)
	     {
		          // Map eye color to specific hue ranges
		          float h = 0f;
		          float sMin = 0.35f, sMax = 0.8f;
		          float vMin = 0.4f, vMax = 0.95f;
		          switch (color)
		          {
		          case 1: // Violet
		          h = 275f / 360f;
		          sMin = 0.45f;
		          sMax = 078f;
		          vMin = 0.65f;
		          vMax = 0.95f;
		          break;
		          case 2: // Blue
		          h = 220f / 360f;
		          sMin = 0.05f;
		          sMax = 0.65f;
		          vMin = 0.25f;
		          vMax = 0.95f; // keep blues visibly bright
		          break;
		          case 3: // Blue-Green / teal
		          h = 195f / 360f;
		          sMin = 0.05f;
		          sMax = 0.6f;
		          vMin = 0.25f;
		          vMax = 0.95f;
		          break;
		          case 4: // Green
		          h = 130f / 360f;
		          sMin = 0.1f;
		          sMax = 0.5f;
		          vMin = 0.45f;
		          vMax = 0.95f;
		          break;
		          case 5: // Hazel (yellow-brown)
		          h = 45f / 360f;
		          sMin = 0.4f;
		          sMax = 0.8f;
		          vMin = 0.5f;
		          vMax = 0.9f;
		          break;
		          case 6: // Brown
		          h = 20f / 360f;
		          sMin = 0.15f;
		          sMax = 0.7f;
		          vMin = 0.15f;
		          vMax = 0.7f;
		          break;
		          default:
		          h = 0f; //albino red
		          sMin = 0.5f;
		          sMax = 0.8f;
		          vMin = 0.55f;
		          vMax = 0.95f;
		          break;
		          }

		          // darkness is 1..4 (1 = lightest). Map so darker -> lower value
		          float s = MapRange(darkness, 1, 4, sMax, sMin);
		          float v = MapRange(darkness, 1, 4, vMax, vMin);

		          Color eyeColor = Color.HSVToRGB(h, s, v);



		          return eyeColor;
	     }

	     private static Color GetHairColor(int eumelanin, int pheomelanin)
	     {
		          // Eumelanin controls darkness (black/brown), Pheomelanin controls redness
		          float baseHue = MapRange(pheomelanin, 1, 6, 0.02f, 0.15f); // Slight yellow to orange-red
		          float saturation = MapRange(eumelanin + pheomelanin, 1, 14, 0.3f, 0.55f); // More pheomelanin = more saturation
		          float value = MapRange(eumelanin, 1, 9, 0.8f, 0.02f); // More eumelanin = darker

		          Color hairColor = Color.HSVToRGB(baseHue, saturation, value);
		          return hairColor;
	     }

	     private static Color GetMouthColor(int skinHue, int skinSaturation, int skinDepth)
	     {
		          // Start with skin color but make it slightly more red/pink
		          Color skinColor = GetSkinColor(skinHue, skinSaturation, skinDepth);

		          // Convert to HSV to adjust
		          Color.RGBToHSV(skinColor, out float h, out float s, out float v);

		          // Make slightly more red and saturated for lips
		          h = Mathf.Clamp(h + 0.853f, 0.9f, 1f);
		          s = Mathf.Clamp(s + 0.2f, 0.2f, 0.35f);
		          v = Mathf.Clamp(v - 0.1f, 0.35f, .7f);

		          return Color.HSVToRGB(h, s, v);
	     }

	     private static float MapRange(float value, float fromMin, float fromMax, float toMin, float toMax)
	     {
		          return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
	     }
}

public class Graph
{
	     public Vector3[] points;
	     public GraphType type;
	     public Color[] pointColors; // Color for each point
	     public Color lineColor = Color.white;
	     public float lineWidth = 0.1f;
	     public string title;
	     public string[] axisLabels; // Labels for each axis
	     public Color[] axisColors; // Colors for each axis
	     public bool showAxes = true;
	     public bool showLabels = true;

	     // Default constructor for serialization
	     public Graph()
	     {
	     }

	     public Graph(int pointCount)
	     {
		          points = new Vector3[pointCount];
		          pointColors = new Color[pointCount];
		          axisLabels = new string[pointCount];
		          axisColors = new Color[pointCount];

		          // Initialize with default values
		          for (int i = 0; i < pointCount; i++)
		          {
				pointColors[i] = Color.white;
				axisColors[i] = Color.gray;
		          }
	     }
}

public enum GraphType
{
	     radar, bar, pie, line
}
public static class RadarGraphFactory
{
	     public static GameObject CreateRadarGraph(Graph graph, Transform parent = null, float size = 80f, string name = "RadarGraph")
	     {
		          if (graph?.points == null || graph.points.Length < 3)
				return null;

		          GameObject graphGO = new GameObject(name);
		          RectTransform rt = graphGO.AddComponent<RectTransform>();
		          rt.sizeDelta = new Vector2(size, size);

		          // Draw the radar graph
		          DrawRadarGraph(graphGO, graph, size * 0.55f); // Scale to fit

		          if (parent != null)
				graphGO.transform.SetParent(parent, false);

		          return graphGO;
	     }

	     private static void DrawRadarGraph(GameObject parent, Graph graph, float scale)
	     {
		          // Create background
		          CreateBackground(parent, new Color(0.1f, 0.1f, 0.2f, 0.3f));
		          //put graph title here
		          if (!string.IsNullOrEmpty(graph.title))
		          {
				CreateGraphTitle(parent, graph.title);
		          }
		          // Create axes if requested
		          if (graph.showAxes && graph.axisColors != null && graph.axisLabels != null)
		          {
				CreateRadarAxes(parent, graph, scale * 0.9f);
		          }

		          // Create the radar outline with interpolated colors
		          CreateRadarOutline(parent, graph, scale);

		          // Create data points
		          CreateDataPoints(parent, graph, scale);
	     }

	     private static void CreateBackground(GameObject parent, Color color)
	     {
		          GameObject background = new GameObject("Background");
		          background.transform.SetParent(parent.transform, false);

		          RectTransform bgRT = background.AddComponent<RectTransform>();
		          bgRT.anchorMin = Vector2.zero;
		          bgRT.anchorMax = Vector2.one;
		          bgRT.sizeDelta = Vector2.zero;
		          bgRT.anchoredPosition = Vector2.zero;

		          Image bgImage = background.AddComponent<Image>();
		          bgImage.color = color;
	     }
	     private static void CreateGraphTitle(GameObject parent, string title)
	     {
		          GameObject titleGO = new GameObject("GraphTitle");
		          titleGO.transform.SetParent(parent.transform, false);

		          RectTransform titleRT = titleGO.AddComponent<RectTransform>();
		          titleRT.anchorMin = new Vector2(0.5f, 1f); // Top center
		          titleRT.anchorMax = new Vector2(0.5f, 1f);
		          titleRT.pivot = new Vector2(0.5f, 1f);
		          titleRT.sizeDelta = new Vector2(80f, 20f);
		          titleRT.anchoredPosition = new Vector2(0f, 3f); // Slightly below top edge

		          TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
		          titleText.text = title;
		          titleText.fontSize = 6;
		          titleText.color = Color.white;
		          titleText.alignment = TextAlignmentOptions.Top;
		          titleText.fontStyle = FontStyles.Normal;
	     }

	     private static void CreateRadarAxes(GameObject parent, Graph graph, float axisLength)
	     {
		          int axisCount = graph.points.Length - 1; // -1 because last point closes the shape
		          float angleIncrement = 360f / axisCount * Mathf.Deg2Rad;

		          for (int i = 0; i < axisCount; i++)
		          {
				float angle = i * angleIncrement;
				float x = Mathf.Cos(angle) * axisLength;
				float y = Mathf.Sin(angle) * axisLength;

				// Create axis line with safe color access
				Color axisColor = (graph.axisColors != null && i < graph.axisColors.Length)
				    ? graph.axisColors[i]
				    : Color.gray;

				axisColor.a = 0.1f; // Semi-transparent

				CreateUILine(parent, Vector3.zero, new Vector3(x, y, 0), axisColor, 1f);
				axisColor.a = 0.6f;
				// Create axis label if available
				if (graph.showLabels && graph.axisLabels != null && i < graph.axisLabels.Length)
				{
					     CreateAxisLabel(parent, new Vector3(x, y, 0), graph.axisLabels[i], axisColor);
				}
		          }
	     }

	     private static void CreateRadarOutline(GameObject parent, Graph graph, float scale)
	     {
		          int pointCount = graph.points.Length;

		          for (int i = 0; i < pointCount - 1; i++)
		          {
				Vector3 start = graph.points[i] * scale;
				Vector3 end = graph.points[i + 1] * scale;

				// Safe color access with fallbacks
				Color startColor = (graph.pointColors != null && i < graph.pointColors.Length)
				    ? graph.pointColors[i]
				    : graph.lineColor;

				Color endColor = (graph.pointColors != null && (i + 1) < graph.pointColors.Length)
				    ? graph.pointColors[i + 1]
				    : graph.lineColor;

				// Use lerped color for the line segment
				Color lineColor = Color.Lerp(startColor, endColor, 0.5f);

				CreateUILine(parent, start, end, lineColor, graph.lineWidth * 10f);
		          }
	     }

	     private static void CreateDataPoints(GameObject parent, Graph graph, float scale)
	     {
		          int pointCount = graph.points.Length - 1; // Don't create point for closing vertex

		          for (int i = 0; i < pointCount; i++)
		          {
				Vector3 position = graph.points[i] * scale;

				// Safe color access
				Color pointColor = (graph.pointColors != null && i < graph.pointColors.Length)
				    ? graph.pointColors[i]
				    : graph.lineColor;

				CreateUIPoint(parent, position, pointColor, 1f);
		          }
	     }

	     private static void CreateUILine(GameObject parent, Vector3 start, Vector3 end, Color color, float width)
	     {
		          GameObject line = new GameObject("Line");
		          line.transform.SetParent(parent.transform, false);

		          RectTransform rt = line.AddComponent<RectTransform>();

		          Vector2 direction = (end - start);
		          float distance = direction.magnitude;
		          Vector2 midPoint = (start + end) * 0.5f;

		          rt.sizeDelta = new Vector2(distance, width);
		          rt.anchoredPosition = midPoint;

		          float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		          rt.localRotation = Quaternion.Euler(0, 0, angle);

		          Image lineImage = line.AddComponent<Image>();
		          lineImage.color = color;
	     }

	     private static void CreateUIPoint(GameObject parent, Vector3 position, Color color, float size)
	     {
		          GameObject point = new GameObject("Point");
		          point.transform.SetParent(parent.transform, false);

		          RectTransform rt = point.AddComponent<RectTransform>();
		          rt.sizeDelta = new Vector2(size, size);
		          rt.anchoredPosition = position;

		          Image pointImage = point.AddComponent<Image>();
		          pointImage.color = color;
	     }

	     private static void CreateAxisLabel(GameObject parent, Vector3 position, string label, Color color)
	     {
		          GameObject labelGO = new GameObject($"Label_{label}");
		          labelGO.transform.SetParent(parent.transform, false);

		          RectTransform labelRT = labelGO.AddComponent<RectTransform>();
		          labelRT.sizeDelta = new Vector2(50f, 8f);
		          labelRT.anchoredPosition = position * 1.2f; // Slightly outside the axis

		          TextMeshProUGUI labelText = labelGO.AddComponent<TextMeshProUGUI>();
		          labelText.text = label;
		          labelText.fontSize = 6;
		          labelText.color = color;
		          //        if (label.Length >= 3)
		          //        {
		          //labelText.color = Color.white;
		          //        }
		          labelText.alignment = TextAlignmentOptions.Center;
	     }

}