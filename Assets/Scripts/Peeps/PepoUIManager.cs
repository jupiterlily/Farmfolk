using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PepoUIManager : MonoBehaviour
{
	     public GameObject CreateStatusPanel(PepoData pepo, Transform parent = null, Vector2 panelSize = default)
	     {
		          if (pepo == null)
		          {
				Debug.LogWarning("CreateStatusPanel called with null PepoData");
				return null;
		          }

		          if (panelSize == default)
				panelSize = new Vector2(180f, 180f);

		          // Create panel with proper layout
		          GameObject panel = new GameObject($"PepoStatusPanel_{pepo.forename}");
		          RectTransform panelRT = panel.AddComponent<RectTransform>();
		          panel.AddComponent<CanvasRenderer>();

		          // Add background
		          var panelImage = panel.AddComponent<Image>();
		          panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

		          // Add Vertical Layout Group for automatic arrangement
		          var layoutGroup = panel.AddComponent<VerticalLayoutGroup>();
		          layoutGroup.padding = new RectOffset(5, 5, 5, 5);
		          layoutGroup.spacing = 2f;
		          layoutGroup.childControlHeight = true;
		          layoutGroup.childForceExpandHeight = true;

		          // Add Content Size Fitter to automatically size the panel
		          var sizeFitter = panel.AddComponent<ContentSizeFitter>();
		          sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		          sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

		          panelRT.sizeDelta = panelSize;

		          // Set parent AFTER all components are added
		          if (parent != null)
				panel.transform.SetParent(parent, false);

		          // Create text elements using TextMeshPro
		          CreateTMPText(panel.transform, PepoUtilities.GetFullName(pepo), 12, FontStyles.Bold);
		          CreateTMPText(panel.transform, PepoLogUtilities.Age(pepo), 8);
		          CreateTMPText(panel.transform, $"{PepoLogUtilities.GetGenderExpression(pepo)} ", 8);
		          CreateTMPText(panel.transform, PepoLogUtilities.GetPregnancyStatus(pepo), 8);

		          // Create a dedicated graph container with horizontal layout
		          GameObject graphContainer = CreateGraphContainer(panel.transform);

		          // Only add radar if we have personality data
		          if (pepo.personality != null)
		          {
				GameObject radar = CreateHexacoRadar(pepo, graphContainer.transform, 50f);
				if (radar != null)
				{
					     // Set radar size and layout element
					     RectTransform radarRT = radar.GetComponent<RectTransform>();
					     radarRT.sizeDelta = new Vector2(80f, 80f);

					     // Add layout element to control sizing
					     var layoutElement = radar.AddComponent<LayoutElement>();
					     layoutElement.preferredWidth = 60f;
					     layoutElement.preferredHeight = 60f;
					     layoutElement.flexibleWidth = 0f;
					     layoutElement.flexibleHeight = 0f;
				}
		          }

		          // Add mood radar if available
		          if (pepo.mood != null)
		          {
				GameObject moodRadar = CreateMoodRadar(pepo, graphContainer.transform, 50f);
				if (moodRadar != null)
				{
					     // Set radar size and layout element
					     RectTransform radarRT = moodRadar.GetComponent<RectTransform>();
					     radarRT.sizeDelta = new Vector2(80f, 80f);

					     // Add layout element to control sizing
					     var layoutElement = moodRadar.AddComponent<LayoutElement>();
					     layoutElement.preferredWidth = 60f;
					     layoutElement.preferredHeight = 60f;
					     layoutElement.flexibleWidth = 0f;
					     layoutElement.flexibleHeight = 0f;
				}
		          }
		          // Add values radar
		          if (pepo.values != null && pepo.values.Count > 0)
		          {
				GameObject valuesRadar = CreateValuesRadar(pepo, graphContainer.transform, 50f);
				if (valuesRadar != null)
				{
					     RectTransform radarRT = valuesRadar.GetComponent<RectTransform>();
					     radarRT.sizeDelta = new Vector2(80f, 80f);
					     var layoutElement = valuesRadar.AddComponent<LayoutElement>();
					     layoutElement.preferredWidth = 60f;
					     layoutElement.preferredHeight = 60f;
				}
		          }

		          return panel;
	     }

	     private static GameObject CreateGraphContainer(Transform parent)
	     {
		          GameObject container = new GameObject("GraphContainer");
		          container.transform.SetParent(parent, false);

		          RectTransform containerRT = container.AddComponent<RectTransform>();
		          containerRT.sizeDelta = new Vector2(160f, 50f); // Width to fit 2 graphs

		          // Add Horizontal Layout Group
		          var horizontalLayout = container.AddComponent<HorizontalLayoutGroup>();
		          horizontalLayout.padding = new RectOffset(2, 2, 2, 2);
		          horizontalLayout.spacing = 5f;
		          horizontalLayout.childAlignment = TextAnchor.MiddleCenter;
		          horizontalLayout.childControlWidth = false;
		          horizontalLayout.childControlHeight = false;
		          horizontalLayout.childForceExpandWidth = false;
		          horizontalLayout.childForceExpandHeight = false;

		          // Add Content Size Fitter
		          var sizeFitter = container.AddComponent<ContentSizeFitter>();
		          sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		          sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

		          return container;
	     }

	     private static void CreateTMPText(Transform parent, string text, int fontSize, FontStyles style = FontStyles.Normal)
	     {
		          GameObject textGO = new GameObject("TMP_Text");
		          textGO.transform.SetParent(parent, false);

		          RectTransform rt = textGO.AddComponent<RectTransform>();
		          rt.sizeDelta = new Vector2(160f, 20f);

		          var tmpText = textGO.AddComponent<TextMeshProUGUI>();
		          tmpText.text = text;
		          tmpText.fontSize = fontSize;
		          tmpText.fontStyle = style;
		          tmpText.color = Color.white;
		          tmpText.alignment = TextAlignmentOptions.Center;

		          // Add layout element for text
		          var layoutElement = textGO.AddComponent<LayoutElement>();
		          layoutElement.preferredHeight = 15f;
		          layoutElement.minHeight = 12f;
	     }

	     public static GameObject CreateHexacoRadar(PepoData pepo, Transform parent = null, float pixelRadius = 60f)
	     {
		          if (pepo?.personality == null)
				return null;

		          Graph pepoGraph = UIUtilities.CreateHexacoGraph(pepo.personality);
		          return RadarGraphFactory.CreateRadarGraph(pepoGraph, parent, pixelRadius, $"HexacoRadar_{pepo.forename}");
	     }

	     public static GameObject CreateMoodRadar(PepoData pepo, Transform parent = null, float pixelRadius = 60f)
	     {
		          if (pepo?.mood == null)
				return null;

		          Graph pepoGraph = UIUtilities.CreateMoodGraph(pepo.mood);
		          return RadarGraphFactory.CreateRadarGraph(pepoGraph, parent, pixelRadius, $"MoodRadar_{pepo.forename}");
	     }
	     public static GameObject CreateValuesRadar(PepoData pepo, Transform parent = null, float pixelRadius = 60f)
	     {
		          if (pepo?.values == null || pepo.values.Count == 0)
				return null;

		          Graph valuesGraph = UIUtilities.CreateValuesGraph(pepo.values);
		          return RadarGraphFactory.CreateRadarGraph(valuesGraph, parent, pixelRadius, $"ValuesRadar_{pepo.forename}");
	     }
	     public static GameObject CreateCustomRadar(Graph graph, Transform parent = null, float size = 60f, string name = "CustomRadar")
	     {
		          return RadarGraphFactory.CreateRadarGraph(graph, parent, size, name);
	     }

}
