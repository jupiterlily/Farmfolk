using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PepoUIUtilities
{

	     public static GameObject CreatePortrait(PepoData pepo, List<Sprite> fHairBack, List<Sprite> fHairFront, List<Sprite> mHairBack, List<Sprite> mHairFront, List<Sprite> uHairBack, List<Sprite> uHairFront, List<Sprite> heads, List<Sprite> eyeColours, List<Sprite> mascEyes, List<Sprite> femmeEyes, List<Sprite> noses, List<Sprite> mouths, List<Sprite> freckles)
	     {
		          GameObject portrait = new GameObject("Portrait");
		          RectTransform rt = portrait.AddComponent<RectTransform>();
		          rt.sizeDelta = new Vector2(64f, 64f);

		          var layoutElement = portrait.AddComponent<LayoutElement>();
		          layoutElement.preferredWidth = 64f;
		          layoutElement.preferredHeight = 64f;
		          layoutElement.minWidth = 32f;
		          layoutElement.minHeight = 32f;

		          // Get height offsets for features
		          float eyeHeightOffset = GetHeightOffset(pepo.body, 6); // eyeHeight trait
		          float noseHeightOffset = GetHeightOffset(pepo.body, 8); // noseHeight trait  
		          float mouthHeightOffset = GetHeightOffset(pepo.body, 10); // mouthHeight trait

		          Color hairColor = UIUtilities.GetPepoColors(pepo.body)[0];
		          if (pepo.age > 30f)
		          {
				hairColor = Color.Lerp(hairColor, Color.white, Mathf.Clamp01((pepo.age - 40f) / 60f));
		          }

		          CreateLayeredSprite(portrait.transform, GetHairBackSprite(pepo, fHairBack, fHairFront, mHairBack, mHairFront, uHairBack, uHairFront), "HairBack", 1, hairColor);
		          CreateLayeredSprite(portrait.transform, GetHeadSprite(pepo, heads), "Head", 2, UIUtilities.GetPepoColors(pepo.body)[2]);
		          CreateLayeredSprite(portrait.transform, GetNoseSprite(pepo, noses), "Nose", 4, UIUtilities.GetPepoColors(pepo.body)[2], noseHeightOffset);

		          //freckles
		          AddFrecklesSprite(pepo, freckles, portrait.transform);
		          CreateLayeredSprite(portrait.transform, GetEyeColourSprite(pepo, eyeColours), "EyeColour", 3, UIUtilities.GetPepoColors(pepo.body)[1], eyeHeightOffset);
		          CreateLayeredSprite(portrait.transform, GetEyeSprite(pepo, mascEyes, femmeEyes), "Eyes", 4, Color.white, eyeHeightOffset);
		          CreateLayeredSprite(portrait.transform, GetMouthSprite(pepo, mouths), "Mouth", 5, UIUtilities.GetPepoColors(pepo.body)[3], mouthHeightOffset);
		          //if they're less than 3 years old they have no hair sprite
		          if (pepo.age < 1)
		          {
				return portrait;
		          }
		          CreateLayeredSprite(portrait.transform, GetHairFrontSprite(pepo, fHairBack, fHairFront, mHairBack, mHairFront, uHairBack, uHairFront), "HairFront", 6, hairColor);

		          return portrait;
	     }

	     public static GameObject CreateInfoContainer(PepoData pepo)
	     {
		          GameObject infoContainer = new GameObject("InfoContainer");
		          RectTransform rt = infoContainer.AddComponent<RectTransform>();

		          // Add Vertical Layout Group
		          var layoutGroup = infoContainer.AddComponent<VerticalLayoutGroup>();
		          layoutGroup.spacing = 2f;
		          layoutGroup.childControlHeight = true;
		          layoutGroup.childForceExpandHeight = true;
		          layoutGroup.childControlWidth = true;
		          layoutGroup.childForceExpandWidth = false;

		          // Add Content Size Fitter
		          var sizeFitter = infoContainer.AddComponent<ContentSizeFitter>();
		          sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		          sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

		          List<PepoRelationshipData> relationships = new List<PepoRelationshipData>();
		          foreach (var relList in pepo.RelationshipLibrary.Values)
		          {
				relationships.AddRange(relList);
		          }
		          // Create text elements
		          CreateTMPText(infoContainer.transform, PepoUtilities.GetFullName(pepo), 12, FontStyles.Bold);
		          CreateTMPText(infoContainer.transform, new string($"{PepoLogUtilities.Age(pepo)} with {PepoUtilitiesBonds.TotalPower(relationships)} social power and {PepoUtilitiesBonds.TotalPowerOverMe(relationships)}"), 8);
		          CreateTMPText(infoContainer.transform, $"{PepoLogUtilities.GetGenderExpression(pepo)} ", 6);
		          CreateTMPText(infoContainer.transform, PepoLogUtilities.GetPregnancyStatus(pepo), 6);



		          return infoContainer;
	     }
	     public static GameObject CreateGraphContainer(PepoData pepo)
	     {
		          // Create graph container
		          GameObject graphContainer = new GameObject("GraphContainer");
		          var layoutGroup = graphContainer.AddComponent<HorizontalLayoutGroup>();
		          layoutGroup.padding = new RectOffset(2, 2, 2, 2);
		          layoutGroup.spacing = 5f;
		          layoutGroup.childAlignment = TextAnchor.MiddleCenter;
		          layoutGroup.childControlWidth = false;
		          layoutGroup.childControlHeight = false;
		          layoutGroup.childForceExpandWidth = false;
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
		          return graphContainer;
	     }
	     public static void CreateLayeredSprite(Transform parent, Sprite sprite, string name, int order, Color color, float heightOffset = 0f)
	     {
		          if (sprite == null)
				return;

		          GameObject spriteObj = new GameObject(name);
		          spriteObj.transform.SetParent(parent, false);

		          RectTransform rt = spriteObj.AddComponent<RectTransform>();
		          rt.anchorMin = Vector2.zero;
		          rt.anchorMax = Vector2.one;
		          rt.sizeDelta = Vector2.zero;

		          // Apply height offset by adjusting the anchored position
		          if (heightOffset != 0f)
		          {
				rt.anchoredPosition = new Vector2(0f, heightOffset);
		          }

		          Image image = spriteObj.AddComponent<Image>();
		          image.sprite = sprite;
		          image.preserveAspect = true;
		          image.color = color;
	     }

	     public static List<Sprite> GetHairList(SocioSexualData ssd, bool isBackHair, List<Sprite> fHairBack, List<Sprite> fHairFront, List<Sprite> mHairBack, List<Sprite> mHairFront, List<Sprite> uHairBack, List<Sprite> uHairFront)
	     {
		          // Determine which hair list to use based on gender presentation
		          if (ssd.presentsFemme && !ssd.presentsMasc)
		          {
				List<Sprite> combinedHair = new List<Sprite>();
				combinedHair.AddRange(isBackHair ? fHairBack : fHairFront);
				combinedHair.AddRange(isBackHair ? uHairBack : uHairFront);
				return combinedHair;
		          }
		          else if (ssd.presentsMasc && !ssd.presentsFemme)
		          {
				List<Sprite> combinedHair = new List<Sprite>();
				combinedHair.AddRange(isBackHair ? mHairBack : mHairFront);
				combinedHair.AddRange(isBackHair ? uHairBack : uHairFront);
				return combinedHair;
		          }
		          else if (!ssd.presentsFemme && !ssd.presentsMasc)
		          {
				// Default to unisex if no specific presentation or both
				return isBackHair ? uHairBack : uHairFront;
		          }
		          else
		          {
				//pepos who present both masc and femme can have hair from all three categories
				List<Sprite> combinedHair = new List<Sprite>();
				combinedHair.AddRange(isBackHair ? fHairBack : fHairFront);
				combinedHair.AddRange(isBackHair ? mHairBack : mHairFront);
				combinedHair.AddRange(isBackHair ? uHairBack : uHairFront);
				return combinedHair;
		          }
	     }

	     public static int GetStableHairIndex(PepoData pepo, int maxCount)
	     {
		          // Create a stable hash based on pepo's name for consistent hair selection
		          string hashString = pepo.forename + pepo.surname;
		          int hash = hashString.GetHashCode();

		          // Ensure positive index and within range
		          return Mathf.Abs(hash) % maxCount;
	     }
	     public static GameObject CreateGraphContainer()
	     {
		          GameObject container = new GameObject("GraphContainer");

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

	     public static void CreateTMPText(Transform parent, string text, int fontSize, FontStyles style = FontStyles.Normal)
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
	     public static float GetHeightOffset(BodyData body, int traitIndex)
	     {
		          if (body?.expressedChromosomes == null || body.expressedChromosomes.Count <= traitIndex)
				return 0f;

		          int heightValue = body.GetExpressedTrait(traitIndex);

		          // Convert height trait (1-3) to pixel offset
		          // 1 = down 1 pixel, 2 = neutral, 3 = up 1 pixel
		          switch (heightValue)
		          {
		          case 1:
		          return -1f; // One pixel down
		          case 3:
		          return 1f;  // One pixel up
		          default:
		          return 0f; // Neutral
		          }
	     }
	     // Sprite selection methods based on BodyData
	     public static Sprite GetHeadSprite(PepoData pepo, List<Sprite> headShape)
	     {
		          if (pepo?.body == null || headShape == null || headShape.Count == 0)
				return null;

		          int headIndex = pepo.body.GetExpressedTrait(0); // headShape trait
		          Sprite head = headShape[Mathf.Clamp(headIndex - 1, 0, headShape.Count - 1)];
		          return head;
	     }
	     public static Sprite GetEyeColourSprite(PepoData pepo, List<Sprite> eyecolor)
	     {
		          if (pepo?.body == null)
				return null;
		          //if eye darkness is above a threshold use the 2nd sprite (darker eye color)
		          int eyeColorIndex = pepo.body.GetExpressedTrait(5); // eyeDarkness trait
		          if (eyeColorIndex <= 1)
		          {
				return eyecolor[0];
		          }
		          else if (eyeColorIndex == eyecolor.Count - 1)
		          {
				return eyecolor[2];
		          }
		          else
		          {
				return eyecolor[1];
		          }
	     }
	     public static Sprite GetEyeSprite(PepoData pepo, List<Sprite> mascEyes, List<Sprite> femmeEyes)
	     {
		          if (pepo?.body == null)
				return null;

		          int eyeShapeIndex = pepo.body.GetExpressedTrait(3); // eyeShape trait
		          List<Sprite> eyeList = (!pepo.ssd.isFemme && !pepo.ssd.presentsFemme) ? mascEyes : femmeEyes;
		          if (eyeList == null || eyeList.Count == 0)
				return null;

		          return eyeList[Mathf.Clamp(eyeShapeIndex - 1, 0, eyeList.Count - 1)];
	     }

	     public static Sprite GetNoseSprite(PepoData pepo, List<Sprite> noseShape)
	     {
		          if (pepo?.body == null || noseShape == null || noseShape.Count == 0)
				return null;

		          int noseIndex = pepo.body.GetExpressedTrait(7); // noseShape trait
		          return noseShape[Mathf.Clamp(noseIndex - 1, 0, noseShape.Count - 1)];
	     }

	     public static Sprite GetMouthSprite(PepoData pepo, List<Sprite> mouthShape)
	     {
		          if (pepo?.body == null || mouthShape == null || mouthShape.Count == 0)
				return null;

		          int mouthIndex = pepo.body.GetExpressedTrait(9); // mouthShape trait
		          return mouthShape[Mathf.Clamp(mouthIndex - 1, 0, mouthShape.Count - 1)];
	     }

	     // Hair selection methods based on SocioSexualData
	     public static Sprite GetHairBackSprite(PepoData pepo, List<Sprite> fHairBack, List<Sprite> fHairFront, List<Sprite> mHairBack, List<Sprite> mHairFront, List<Sprite> uHairBack, List<Sprite> uHairFront)
	     {
		          if (pepo?.ssd == null || uHairBack == null)
				return null;

		          List<Sprite> hairList = GetHairList(pepo.ssd, true, fHairBack, fHairFront, mHairBack, mHairFront, uHairBack, uHairFront); // true for back hair
		          if (hairList == null || hairList.Count == 0)
				return null;

		          // Use a consistent random seed based on pepo's name for consistent hair selection
		          int hairIndex = GetStableHairIndex(pepo, hairList.Count);
		          return hairList[hairIndex];
	     }

	     public static Sprite GetHairFrontSprite(PepoData pepo, List<Sprite> fHairBack, List<Sprite> fHairFront, List<Sprite> mHairBack, List<Sprite> mHairFront, List<Sprite> uHairBack, List<Sprite> uHairFront)
	     {
		          List<Sprite> hairList = GetHairList(pepo.ssd, false, fHairBack, fHairFront, mHairBack, mHairFront, uHairBack, uHairFront); // false for front hair
		          if (hairList == null || hairList.Count == 0)
				return null;

		          // Use same seed to ensure front/back hair match
		          int hairIndex = GetStableHairIndex(pepo, hairList.Count);
		          return hairList[hairIndex];
	     }
	     public static void AddFrecklesSprite(PepoData pepo, List<Sprite> freckles, Transform parent)
	     {
		          if (pepo?.body == null || freckles == null || freckles.Count == 0)
				return;

		          int frecklesTrait = pepo.body.GetExpressedTrait(14);
		          int doubleFrecklesTrait = pepo.body.GetExpressedTrait(15);

		          // Determine which parent contributed which freckles
		          int maternalFreckles = pepo.body.maternalChromosome.GetTraitByIndex(14);
		          int paternalFreckles = pepo.body.paternalChromosome.GetTraitByIndex(14);
		          bool maternalInverted = pepo.body.maternalChromosome.GetTraitByIndex(16) == 1;
		          bool paternalInverted = pepo.body.paternalChromosome.GetTraitByIndex(16) == 1;

		          // Figure out which is expressed vs unexpressed
		          bool isMaternalExpressed = frecklesTrait == maternalFreckles;

		          int primaryFreckleIndex = Mathf.Clamp(frecklesTrait - 1, 0, freckles.Count - 1);
		          int secondaryFreckleIndex = Mathf.Clamp((isMaternalExpressed ? paternalFreckles : maternalFreckles) - 1, 0, freckles.Count - 1);

		          bool primaryInverted = isMaternalExpressed ? maternalInverted : paternalInverted;
		          bool secondaryInverted = isMaternalExpressed ? paternalInverted : maternalInverted;

		          Color skinColor = UIUtilities.GetPepoColors(pepo.body)[2];
		          skinColor.a *= 0.5f;

		          // Primary freckles
		          if (frecklesTrait > 1)
		          {
				CreateLayeredSpriteWithInversion(
				    parent,
				    freckles[primaryFreckleIndex],
				    "PrimaryFreckles",
				    3,
				    skinColor,
				    primaryInverted
				);
		          }

		          // Secondary freckles
		          if (doubleFrecklesTrait == 2 && (isMaternalExpressed ? paternalFreckles : maternalFreckles) > 1)
		          {
				CreateLayeredSpriteWithInversion(
				    parent,
				    freckles[secondaryFreckleIndex],
				    "SecondaryFreckles",
				    3,
				    skinColor,
				    secondaryInverted
				);
		          }
	     }

	     // Helper method to create layered sprites with optional inversion
	     public static GameObject CreateLayeredSpriteWithInversion(Transform parent, Sprite sprite, string name, int order, Color color, bool invertY = false)
	     {
		          if (sprite == null)
				return null;

		          GameObject spriteObj = new GameObject(name);
		          spriteObj.transform.SetParent(parent, false);

		          RectTransform rt = spriteObj.AddComponent<RectTransform>();
		          rt.anchorMin = Vector2.zero;
		          rt.anchorMax = Vector2.one;
		          rt.sizeDelta = Vector2.zero;

		          // Apply inversion if needed
		          if (invertY)
		          {
				rt.localScale = new Vector3(-1f, 1f, 1f);
		          }

		          Image image = spriteObj.AddComponent<Image>();
		          image.sprite = sprite;
		          image.preserveAspect = true;
		          image.color = color;

		          return spriteObj;
	     }
}
