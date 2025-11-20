using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PepoUIManager : MonoBehaviour
{
	     // Add sprite references for portrait generation
	     public List<Sprite> mascEyes;
	     public List<Sprite> femmeEyes;
	     public List<Sprite> headShape;
	     public List<Sprite> mouthShape;
	     public List<Sprite> noseShape;

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

		          // Change to Horizontal Layout Group for portrait + info side by side
		          var layoutGroup = panel.AddComponent<HorizontalLayoutGroup>();
		          layoutGroup.padding = new RectOffset(5, 5, 5, 5);
		          layoutGroup.spacing = 5f;
		          layoutGroup.childControlHeight = true;
		          layoutGroup.childForceExpandHeight = false;

		          // Add Content Size Fitter
		          var sizeFitter = panel.AddComponent<ContentSizeFitter>();
		          sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		          sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

		          panelRT.sizeDelta = panelSize;

		          // Set parent AFTER all components are added
		          if (parent != null)
				panel.transform.SetParent(parent, false);

		          // Create portrait container
		          GameObject portraitContainer = CreatePortrait(pepo);
		          portraitContainer.transform.SetParent(panel.transform, false);

		          // Create info container (vertical layout for text and graphs)
		          GameObject infoContainer = CreateInfoContainer(pepo);
		          infoContainer.transform.SetParent(panel.transform, false);

		          return panel;
	     }

	     private GameObject CreatePortrait(PepoData pepo)
	     {
		          GameObject portrait = new GameObject("Portrait");
		          RectTransform rt = portrait.AddComponent<RectTransform>();
		          rt.sizeDelta = new Vector2(64f, 64f); // Fixed size for portrait

		          // Add layout element to control sizing
		          var layoutElement = portrait.AddComponent<LayoutElement>();
		          layoutElement.preferredWidth = 64f;
		          layoutElement.preferredHeight = 64f;
		          layoutElement.minWidth = 32f;
		          layoutElement.minHeight = 32f;

		          // Create layered sprites
		          CreateLayeredSprite(portrait.transform, GetHeadSprite(pepo), "Head", 0);
		          CreateLayeredSprite(portrait.transform, GetEyeSprite(pepo), "Eyes", 1);
		          CreateLayeredSprite(portrait.transform, GetNoseSprite(pepo), "Nose", 2);
		          CreateLayeredSprite(portrait.transform, GetMouthSprite(pepo), "Mouth", 3);

		          return portrait;
	     }

	     private GameObject CreateInfoContainer(PepoData pepo)
	     {
		          GameObject infoContainer = new GameObject("InfoContainer");
		          RectTransform rt = infoContainer.AddComponent<RectTransform>();

		          // Add Vertical Layout Group
		          var layoutGroup = infoContainer.AddComponent<VerticalLayoutGroup>();
		          layoutGroup.spacing = 2f;
		          layoutGroup.childControlHeight = true;
		          layoutGroup.childForceExpandHeight = true;
		          layoutGroup.childControlWidth = true;
		          layoutGroup.childForceExpandWidth = true;

		          // Add Content Size Fitter
		          var sizeFitter = infoContainer.AddComponent<ContentSizeFitter>();
		          sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		          sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

		          // Create text elements
		          CreateTMPText(infoContainer.transform, PepoUtilities.GetFullName(pepo), 12, FontStyles.Bold);
		          CreateTMPText(infoContainer.transform, PepoLogUtilities.Age(pepo), 8);
		          CreateTMPText(infoContainer.transform, $"{PepoLogUtilities.GetGenderExpression(pepo)} ", 8);
		          CreateTMPText(infoContainer.transform, PepoLogUtilities.GetPregnancyStatus(pepo), 8);

		          // Create graph container
		          GameObject graphContainer = CreateGraphContainer(infoContainer.transform);

		          // Add radar graphs (your existing code)
		          if (pepo.personality != null)
		          {
				GameObject radar = CreateHexacoRadar(pepo, graphContainer.transform, 50f);
				if (radar != null)
				{
					     RectTransform radarRT = radar.GetComponent<RectTransform>();
					     radarRT.sizeDelta = new Vector2(80f, 80f);
					     var radarLayout = radar.AddComponent<LayoutElement>();
					     radarLayout.preferredWidth = 60f;
					     radarLayout.preferredHeight = 60f;
					     radarLayout.flexibleWidth = 0f;
					     radarLayout.flexibleHeight = 0f;
				}
		          }

		          // Add other radars (mood and values) - your existing code here
		          if (pepo.mood != null)
		          {
				GameObject moodRadar = CreateMoodRadar(pepo, graphContainer.transform, 50f);
				if (moodRadar != null)
				{
					     RectTransform radarRT = moodRadar.GetComponent<RectTransform>();
					     radarRT.sizeDelta = new Vector2(80f, 80f);
					     var radarLayout = moodRadar.AddComponent<LayoutElement>();
					     radarLayout.preferredWidth = 60f;
					     radarLayout.preferredHeight = 60f;
					     radarLayout.flexibleWidth = 0f;
					     radarLayout.flexibleHeight = 0f;
				}
		          }

		          if (pepo.values != null && pepo.values.Count > 0)
		          {
				GameObject valuesRadar = CreateValuesRadar(pepo, graphContainer.transform, 50f);
				if (valuesRadar != null)
				{
					     RectTransform radarRT = valuesRadar.GetComponent<RectTransform>();
					     radarRT.sizeDelta = new Vector2(80f, 80f);
					     var radarLayout = valuesRadar.AddComponent<LayoutElement>();
					     radarLayout.preferredWidth = 60f;
					     radarLayout.preferredHeight = 60f;
				}
		          }

		          return infoContainer;
	     }

	     private void CreateLayeredSprite(Transform parent, Sprite sprite, string name, int order)
	     {
		          if (sprite == null)
				return;

		          GameObject spriteObj = new GameObject(name);
		          spriteObj.transform.SetParent(parent, false);

		          RectTransform rt = spriteObj.AddComponent<RectTransform>();
		          rt.anchorMin = Vector2.zero;
		          rt.anchorMax = Vector2.one;
		          rt.sizeDelta = Vector2.zero;
		          rt.anchoredPosition = Vector2.zero;

		          Image image = spriteObj.AddComponent<Image>();
		          image.sprite = sprite;
		          image.preserveAspect = true;

		          // Set sorting order (you might need a custom component for this in UI)
		          // For UI, we rely on hierarchy order - later children render on top
	     }

	     // Sprite selection methods based on BodyData
	     private Sprite GetHeadSprite(PepoData pepo)
	     {
		          if (pepo?.body == null || headShape == null || headShape.Count == 0)
				return null;

		          int headIndex = GetExpressedTrait(pepo.body, 0); // headShape trait
		          return headShape[Mathf.Clamp(headIndex - 1, 0, headShape.Count - 1)];
	     }

	     private Sprite GetEyeSprite(PepoData pepo)
	     {
		          if (pepo?.body == null)
				return null;

		          int eyeShapeIndex = GetExpressedTrait(pepo.body, 3); // eyeShape trait
		          string genderExpression = PepoLogUtilities.GetGenderExpression(pepo);

		          List<Sprite> eyeList = (genderExpression == "Masculine") ? mascEyes : femmeEyes;
		          if (eyeList == null || eyeList.Count == 0)
				return null;

		          return eyeList[Mathf.Clamp(eyeShapeIndex - 1, 0, eyeList.Count - 1)];
	     }

	     private Sprite GetNoseSprite(PepoData pepo)
	     {
		          if (pepo?.body == null || noseShape == null || noseShape.Count == 0)
				return null;

		          int noseIndex = GetExpressedTrait(pepo.body, 6); // noseShape trait
		          return noseShape[Mathf.Clamp(noseIndex - 1, 0, noseShape.Count - 1)];
	     }

	     private Sprite GetMouthSprite(PepoData pepo)
	     {
		          if (pepo?.body == null || mouthShape == null || mouthShape.Count == 0)
				return null;

		          int mouthIndex = GetExpressedTrait(pepo.body, 7); // mouthShape trait
		          return mouthShape[Mathf.Clamp(mouthIndex - 1, 0, mouthShape.Count - 1)];
	     }

	     private int GetExpressedTrait(BodyData body, int traitIndex)
	     {
		          if (body.expressedChromosomes == null || body.expressedChromosomes.Count <= traitIndex)
				return 1; // Default to first variant

		          return body.expressedChromosomes[traitIndex] == 0 ?
		              body.maternalChromosome.GetTraitByIndex(traitIndex) :
		              body.paternalChromosome.GetTraitByIndex(traitIndex);
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

