using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PepoUIManager
{
	     public static GameObject CreateStatusPanel(PepoData pepo, Transform parent = null, Vector2 panelSize = default)
	     {
		          if (pepo == null)
		          {
				Debug.LogWarning("CreateStatusPanel called with null PepoData");
				return null;
		          }

		          if (panelSize == default)
				panelSize = new Vector2(300f, 200f);

		          // Create panel with proper layout
		          GameObject panel = new GameObject($"PepoStatusPanel_{pepo.forename}");
		          RectTransform panelRT = panel.AddComponent<RectTransform>();
		          panel.AddComponent<CanvasRenderer>();

		          // Add background
		          var panelImage = panel.AddComponent<Image>();
		          panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

		          // Add Vertical Layout Group for automatic arrangement
		          var layoutGroup = panel.AddComponent<VerticalLayoutGroup>();
		          layoutGroup.padding = new RectOffset(10, 10, 10, 10);
		          layoutGroup.spacing = 5f;
		          layoutGroup.childControlHeight = false;
		          layoutGroup.childForceExpandHeight = false;

		          panelRT.sizeDelta = panelSize;
		          if (parent != null)
				panel.transform.SetParent(parent, false);

		          // Create text elements using TextMeshPro
		          CreateTMPText(panel.transform, PepoUtilities.GetFullName(pepo), 24, FontStyles.Bold);
		          CreateTMPText(panel.transform, PepoLogUtilities.Age(pepo), 12);
		          CreateTMPText(panel.transform, $"{PepoLogUtilities.GetGenderExpression(pepo.ssd, pepo.age)} ", 12);
		          CreateTMPText(panel.transform, PepoLogUtilities.GetPregnancyStatus(pepo), 12);

		          // Only add radar if we have personality data
		          if (pepo.personality != null)
		          {
				GameObject radar = CreateHexacoRadar(pepo, panel.transform, 40f, new Color(0.2f, 0.8f, 0.9f, 1f));
				if (radar != null)
				{
					     // Set radar size
					     RectTransform radarRT = radar.GetComponent<RectTransform>();
					     radarRT.sizeDelta = new Vector2(80f, 80f);
				}
		          }

		          return panel;
	     }

	     private static void CreateTMPText(Transform parent, string text, int fontSize, FontStyles style = FontStyles.Normal)
	     {
		          GameObject textGO = new GameObject("TMP_Text");
		          textGO.transform.SetParent(parent, false);

		          RectTransform rt = textGO.AddComponent<RectTransform>();
		          rt.sizeDelta = new Vector2(280f, 30f);

		          var tmpText = textGO.AddComponent<TextMeshProUGUI>();
		          tmpText.text = text;
		          tmpText.fontSize = fontSize;
		          tmpText.fontStyle = style;
		          tmpText.color = Color.white;
		          tmpText.alignment = TextAlignmentOptions.Left;
	     }


	     public static GameObject CreateHexacoRadar(PepoData pepo, Transform parent = null, float pixelRadius = 60f, Color? lineColor = null)
	     {
		          if (pepo?.personality == null)
				return null;

		          GameObject graphGO = new GameObject($"HexacoRadar_{pepo.forename}");
		          if (parent != null)
				graphGO.transform.SetParent(parent, false);

		          RectTransform rt = graphGO.AddComponent<RectTransform>();
		          rt.sizeDelta = new Vector2(pixelRadius * 2, pixelRadius * 2);

		          // For now, just create a placeholder - you'll need to implement the actual radar visualization
		          var placeholder = graphGO.AddComponent<Image>();
		          placeholder.color = new Color(0.2f, 0.2f, 0.4f, 0.3f);

		          // Add a simple text showing personality summary instead of broken radar
		          var tmpText = graphGO.AddComponent<TextMeshProUGUI>();
		          tmpText.text = PepoLogUtilities.Hexaco(pepo.personality);
		          tmpText.fontSize = 8;
		          tmpText.color = Color.black;
		          tmpText.alignment = TextAlignmentOptions.Center;

		          return graphGO;
	     }

}
