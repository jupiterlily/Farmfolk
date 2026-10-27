using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public static class PepoUIUtilities
{
	     //we should store the base sprites for each feature in a dictionary or list, and then use the pepo's cosmetic features to determine which sprite to use for each feature. This way, we can easily add new features or change existing ones without having to modify the code.
	     public static Sprite[] sHairUp; public static Sprite[] sHairBack; public static Sprite[] sHairTendrils; public static Sprite[] sHairFront; public static Sprite[] wHairUp; public static Sprite[] wHairBack; public static Sprite[] wHairTendrils; public static Sprite[] wHairFront; public static Sprite[] cHairUp; public static Sprite[] cHairBack; public static Sprite[] cHairTendrils; public static Sprite[] cHairFront; public static Sprite[] kHairUp; public static Sprite[] kHairBack; public static Sprite[] kHairTendrils; public static Sprite[] kHairFront; public static Sprite[] heads; public static Sprite[] eyeColours; public static Sprite[] mascEyes; public static Sprite[] femmeEyes; public static Sprite[] nostrils; public static Sprite[] topLips; public static Sprite[] bottomLips; public static Sprite[] freckles; public static Sprite[] brows; public static Sprite[] stubbleSprites; public static Sprite[] moustaches; public static Sprite[] teeth; public static Sprite[] ears; public static Sprite[] noseBridge; public static Sprite[] eyesMascInner; public static Sprite[] eyesFemmeInner; public static Sprite[] eyesMascOuter; public static Sprite[] eyesFemmeOuter; public static Sprite[] browsInner1; public static Sprite[] browsOuter1; public static Sprite[] browsInner2; public static Sprite[] browsOuter2; public static Sprite[] browsInner3; public static Sprite[] browsOuter3; public static Sprite[] browsInner4; public static Sprite[] browsOuter4;

	     public static void InitialiseSprites(Sprite[][] pepoAppearanceSprites)
	     {
		          //this is the order they get fed in.
		          //0 mascEyes, 1 femmeEyes, 2 headShape,3 freckles,4 bottomLips, 5 topLips, 6 noseShape, 7 eyecolor, 8 HairUp_1, 9 HairBack_1, 10 HairTendrils_1, 11 HairFringe_1, 12  HairUp_2, 13 HairBack_2, 14 HairTendrils_2, 15 HairFringe_2, 16 HairUp_3, 17 HairBack_3, 18 HairTendrils_3, 19 HairFringe_3, 20 HairUp_4, 21 HairBack_4, 22 HairTendrils_4, 23 HairFringe_4, 24  brows, 25 stubble, 26 teeth, 27 moustaches }
		          sHairUp = pepoAppearanceSprites[8];
		          sHairBack = pepoAppearanceSprites[9];
		          sHairTendrils = pepoAppearanceSprites[10];
		          sHairFront = pepoAppearanceSprites[11];
		          wHairUp = pepoAppearanceSprites[12];
		          wHairBack = pepoAppearanceSprites[13];
		          wHairTendrils = pepoAppearanceSprites[14];
		          wHairFront = pepoAppearanceSprites[15];
		          cHairUp = pepoAppearanceSprites[16];
		          cHairBack = pepoAppearanceSprites[17];
		          cHairTendrils = pepoAppearanceSprites[18];
		          cHairFront = pepoAppearanceSprites[19];
		          kHairUp = pepoAppearanceSprites[20];
		          kHairBack = pepoAppearanceSprites[21];
		          kHairTendrils = pepoAppearanceSprites[22];
		          kHairFront = pepoAppearanceSprites[23];
		          heads = pepoAppearanceSprites[2];
		          eyeColours = pepoAppearanceSprites[7];
		          mascEyes = pepoAppearanceSprites[0];
		          femmeEyes = pepoAppearanceSprites[1];
		          nostrils = pepoAppearanceSprites[6];
		          topLips = pepoAppearanceSprites[5];
		          bottomLips = pepoAppearanceSprites[4];
		          freckles = pepoAppearanceSprites[3];
		          brows = pepoAppearanceSprites[24];
		          stubbleSprites = pepoAppearanceSprites[25];
		          teeth = pepoAppearanceSprites[26];
		          moustaches = pepoAppearanceSprites[27];
		          ears = pepoAppearanceSprites[28];
		          noseBridge = pepoAppearanceSprites[29];
		          eyesMascInner = pepoAppearanceSprites[30];
		          eyesFemmeInner = pepoAppearanceSprites[31];
		          eyesMascOuter = pepoAppearanceSprites[32];
		          eyesFemmeOuter = pepoAppearanceSprites[33];
		          browsInner1 = pepoAppearanceSprites[34];
		          browsOuter1 = pepoAppearanceSprites[35];
		          browsInner2 = pepoAppearanceSprites[36];
		          browsOuter2 = pepoAppearanceSprites[37];
		          browsInner3 = pepoAppearanceSprites[38];
		          browsOuter3 = pepoAppearanceSprites[39];
		          browsInner4 = pepoAppearanceSprites[40];
		          browsOuter4 = pepoAppearanceSprites[41];

	     }

	     private static Sprite[][] HairSets(string texture) => texture switch
	     {
		          "straight" => new[] { sHairUp, sHairBack, sHairTendrils, sHairFront },
		          "wavy" => new[] { wHairUp, wHairBack, wHairTendrils, wHairFront },
		          "curly" => new[] { cHairUp, cHairBack, cHairTendrils, cHairFront },
		          "coily" => new[] { kHairUp, kHairBack, kHairTendrils, kHairFront },
		          _ => new[] { sHairUp, sHairBack, sHairTendrils, sHairFront },
	     };

	     public static GameObject CreatePortrait(PepoData pepo)
	     {
		          if (pepo == null)
		          {
				Debug.LogWarning("Pepo is null, creating a random pepo for portrait.");
				pepo = PepoFactory.CreateRandom(BodyFactory.CreateChromosomesFromAlleles(BodyLibrary.Plans["human"].Alleles.ToList()));
		          }

		          GameObject portrait = new GameObject("Portrait");
		          var rt = portrait.AddComponent<RectTransform>();
		          rt.sizeDelta = new Vector2(64f, 64f);
		          var le = portrait.AddComponent<LayoutElement>();
		          le.preferredWidth = le.minWidth = 32f;
		          le.preferredHeight = le.minHeight = 32f;

		          var body = pepo.body;

		          float eyeHeightOffset = GetHeightOffset(body, "EyeHeight");
		          float noseHeightOffset = GetHeightOffset(body, "NoseHeight");
		          float mouthHeightOffset = GetHeightOffset(body, "MouthHeight");
		          float browHeightOffset = GetHeightOffset(body, "BrowHeight");

		          Color hairColor = UIUtilities.GetPepoColors(body)[0];
		          Color eyeColor = UIUtilities.GetPepoColors(body)[1];
		          Color skinColor = UIUtilities.GetPepoColors(body)[2];
		          Color mouthColor = UIUtilities.GetPepoColors(body)[3];

		          float greyingAge = BodyUtilities.GetExpressedTrait("HairCanitiesOnset", body);
		          float greyingComplete = BodyUtilities.GetExpressedTrait("HairCanitiesComplete", body);
		          float lifespan = BodyUtilities.GetExpressedTrait("Lifespan", body);

		          if (pepo.age > greyingAge) //this is where greying happens. these magic numbers will be replaced with references to genetic data aroung greying age and lifespan.
				hairColor = Color.Lerp(hairColor, Color.white, Mathf.Clamp01((pepo.age - greyingComplete) / lifespan));

		          Color stubbleColor = Color.Lerp(hairColor, Color.clear,
		              1f - (body.cosmeticFeatures.pepoGrooming["Stubble"] / 5f));

		          // --- hair: 4 slots, texture-driven ---------------------------------
		          var hairSets = HairSets(BodyUtilities.GetExpressedDescriptor("HairTexture", body));
		          var hairKeys = new[] { "HairUp", "HairBack", "HairTendrils", "HairFront" };
		          var hairLayers = new PortraitLayer[4];
		          for (int i = 0; i < 4; i++)
		          {
				int slot = body.cosmeticFeatures.pepoGrooming[hairKeys[i]];
				if (slot < 0)
					     slot = body.cosmeticFeatures.pepoGrooming[hairKeys[i]] =
					         Random.Range(0, hairSets[i].Length);

				hairLayers[i] = new PortraitLayer(hairSets[i][slot], hairKeys[i], hairColor, Vector2.zero);
		          }
		          var HairUp = hairLayers[0];
		          var HairBack = hairLayers[1];
		          var HairTendrils = hairLayers[2];
		          var HairFront = hairLayers[3];

		          // --- face base -----------------------------------------------------
		          var Head = new PortraitLayer(GetHeadSprite(pepo, heads), "Head", skinColor, Vector2.zero);
		          var Ears = new PortraitLayer(GetEarSprite(pepo, ears), "Ears", skinColor, Vector2.zero);
		          var Nostrils = new PortraitLayer(GetNostrilSprite(pepo, nostrils), "Nostrils", skinColor, new Vector2(0, noseHeightOffset));
		          var NoseBridge = new PortraitLayer(GetNasalBridgeSprite(pepo, noseBridge), "Nasal Bridge", skinColor, new Vector2(0, noseHeightOffset));
		          var Stubble = new PortraitLayer(GetStubbleSprite(pepo, stubbleSprites), "Stubble", stubbleColor, Vector2.zero);

		          // --- brows: thickness selects the pair, then inner/outer by name ------
		          var (innerBrows, outerBrows) = BodyUtilities.GetExpressedTrait("BrowsThickness", body) switch
		          {
				1 => (browsInner1, browsOuter1),
				2 => (browsInner2, browsOuter2),
				3 => (browsInner3, browsOuter3),
				_ => (browsInner4, browsOuter4),
		          };
		          var InnerEyebrows = new PortraitLayer(GetBrowSprite(pepo, true), "BrowInnerCorner", hairColor, new Vector2(0, browHeightOffset));
		          var OuterEyebrows = new PortraitLayer(GetBrowSprite(pepo, false), "BrowOuterCorner", hairColor, new Vector2(0, browHeightOffset));

		          // --- eyes: corners share one helper ---------------------------------
		          var EyeColour = new PortraitLayer(GetEyeColourSprite(pepo, eyeColours), "EyeColour", eyeColor, new Vector2(0, eyeHeightOffset));
		          var EyeInnerCorner = new PortraitLayer(GetEyeCornerSprite(pepo, "EyeInnerCorner", eyesMascInner, eyesFemmeInner), "EyeInnerCorner", Color.white, new Vector2(0, eyeHeightOffset));
		          var EyeOuterCorner = new PortraitLayer(GetEyeCornerSprite(pepo, "EyeOuterCorner", eyesMascOuter, eyesFemmeOuter), "EyeOuterCorner", Color.white, new Vector2(0, eyeHeightOffset));

		          // --- mouth ---------------------------------------------------------
		          var BottomLip = new PortraitLayer(GetMouthSprite(pepo, bottomLips, false), "BottomLip", mouthColor, new Vector2(0, mouthHeightOffset));
		          var TopLip = new PortraitLayer(GetMouthSprite(pepo, topLips, true), "TopLip", mouthColor, new Vector2(0, mouthHeightOffset));
		          var Teeth = new PortraitLayer(GetTeethSprite(pepo, teeth), "Teeth", Color.white, new Vector2(0, mouthHeightOffset));
		          var Moustache = new PortraitLayer(GetMoustacheSprite(pepo, moustaches), "Moustache", hairColor, new Vector2(0, mouthHeightOffset + noseHeightOffset));

		          // --- freckles ------------------------------------------------------
		          var PrimaryFreckles = new PortraitLayer(GetPrimaryFrecklesSprite(pepo, freckles), "PrimaryFreckles", skinColor, Vector2.zero);
		          var SecondaryFreckles = new PortraitLayer(GetSecondaryFrecklesSprite(pepo, freckles), "SecondaryFreckles", skinColor, Vector2.zero);

		          if (BodyUtilities.GetExpressedTrait("InvertFreckles", body) == 2)
				PrimaryFreckles.positionOffset = new Vector2(-PrimaryFreckles.positionOffset.x, PrimaryFreckles.positionOffset.y);
		          if (BodyUtilities.GetExpressedTrait("InvertSecondaryFreckles", body) == 2)
				SecondaryFreckles.positionOffset = new Vector2(-SecondaryFreckles.positionOffset.x, SecondaryFreckles.positionOffset.y);

		          pepo.portrait = new PepoPortrait(new List<PortraitLayer>
    {
        HairUp, HairBack, Ears, Head, Nostrils, NoseBridge, Stubble,
        InnerEyebrows, OuterEyebrows, EyeColour, EyeInnerCorner, EyeOuterCorner,
        BottomLip, TopLip, Teeth, Moustache, HairTendrils, HairFront,
        PrimaryFreckles, SecondaryFreckles,
    });

		          foreach (var layer in pepo.portrait.layers)
				CreateLayeredSprite(portrait.transform, layer.sprite, layer.layerName, layer.sortingOrder, layer.color, layer.positionOffset.y);

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

		          // Set anchors and pivot to center
		          rt.anchorMin = new Vector2(0.5f, 0.5f);
		          rt.anchorMax = new Vector2(0.5f, 0.5f);
		          rt.pivot = new Vector2(0.5f, 0.5f);

		          // Get sprite dimensions
		          float spriteWidth = sprite.rect.width;
		          float spriteHeight = sprite.rect.height;

		          // Determine the larger dimension to maintain square proportions
		          float targetSize = Mathf.Max(spriteWidth, spriteHeight);

		          // Set size to the larger dimension (makes it square)
		          rt.sizeDelta = new Vector2(targetSize, targetSize);

		          // Apply height offset
		          rt.anchoredPosition = new Vector2(0f, heightOffset);

		          Image image = spriteObj.AddComponent<Image>();
		          image.sprite = sprite;
		          image.preserveAspect = true;
		          image.color = color;
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
		          return GraphFactory.CreateRadarGraph(pepoGraph, parent, pixelRadius, $"HexacoRadar_{pepo.forename}");
	     }

	     public static GameObject CreateMoodRadar(PepoData pepo, Transform parent = null, float pixelRadius = 60f)
	     {
		          if (pepo?.mood == null)
				return null;

		          Graph pepoGraph = UIUtilities.CreateMoodGraph(pepo.mood);
		          return GraphFactory.CreateRadarGraph(pepoGraph, parent, pixelRadius, $"MoodRadar_{pepo.forename}");
	     }
	     public static GameObject CreateValuesRadar(PepoData pepo, Transform parent = null, float pixelRadius = 60f)
	     {
		          if (pepo?.values == null || pepo.values.Count == 0)
				return null;

		          Graph valuesGraph = UIUtilities.CreateValuesGraph(pepo.values);
		          return GraphFactory.CreateRadarGraph(valuesGraph, parent, pixelRadius, $"ValuesRadar_{pepo.forename}");
	     }
	     public static GameObject CreateCustomRadar(Graph graph, Transform parent = null, float size = 60f, string name = "CustomRadar")
	     {
		          return GraphFactory.CreateRadarGraph(graph, parent, size, name);
	     }
	     // Sprite selection methods based on BodyData
	     // ---- Core sprite picking ------------------------------------------------

	     private static Sprite PickTraitSprite(BodyData body, string traitName, Sprite[] pool)
	     {
		          if (body?.physicalAttributes == null || pool == null || pool.Length == 0)
				return null;
		          if (!body.physicalAttributes.TryGetValue(traitName, out var aspect))
				return null;

		          int idx = Mathf.Clamp((int)aspect.value - 1, 0, pool.Length - 1);
		          return pool[idx];
	     }

	     private static Sprite[] PickByPresentation(PepoData pepo, Sprite[] masc, Sprite[] femme)
	         => (!pepo.ssd.isFemme && !pepo.ssd.presentsFemme) ? masc : femme;

	     // ---- Thin wrappers (stable public API, one line each) -------------------

	     public static Sprite GetHeadSprite(PepoData pepo, Sprite[] pool)
	         => PickTraitSprite(pepo.body, "HeadShape", pool);

	     public static Sprite GetEarSprite(PepoData pepo, Sprite[] pool)
	         => PickTraitSprite(pepo.body, "Ears", pool);

	     public static Sprite GetNostrilSprite(PepoData pepo, Sprite[] pool)
	         => PickTraitSprite(pepo.body, "NostrilShape", pool);

	     public static Sprite GetNasalBridgeSprite(PepoData pepo, Sprite[] pool)
	         => PickTraitSprite(pepo.body, "NasalBridge", pool);

	     public static Sprite GetTeethSprite(PepoData pepo, Sprite[] pool)
	         => PickTraitSprite(pepo.body, "Teeth", pool);

	     public static Sprite GetStubbleSprite(PepoData pepo, Sprite[] pool)
	         => PickTraitSprite(pepo.body, "HeadShape", pool);       // stubble is indexed by head shape

	     public static Sprite GetPrimaryFrecklesSprite(PepoData pepo, Sprite[] pool)
	         => PickTraitSprite(pepo.body, "Freckles", pool);

	     public static Sprite GetSecondaryFrecklesSprite(PepoData pepo, Sprite[] pool)
	         => PickTraitSprite(pepo.body, "SecondaryFreckles", pool);

	     public static Sprite GetMouthSprite(PepoData pepo, Sprite[] pool, bool topLip)
	         => PickTraitSprite(pepo.body, topLip ? "TopLipShape" : "BottomLipShape", pool);

	     // ---- Eye corners (masc/femme sprite sets) -------------------------------

	     public static Sprite GetEyeCornerSprite(PepoData pepo, string traitName, Sprite[] masc, Sprite[] femme)
	         => PickTraitSprite(pepo.body, traitName, PickByPresentation(pepo, masc, femme));

	     // ---- Eye colour (darkness picks the sprite variant, not the colour) -----

	     public static Sprite GetEyeColourSprite(PepoData pepo, Sprite[] pool)
	     {
		          if (pepo?.body?.physicalAttributes == null || pool == null || pool.Length == 0)
				return null;

		          int density = 1;
		          if (pepo.body.physicalAttributes.TryGetValue("EyePigmentDensity", out var d))
				density = Mathf.Clamp((int)d.value, 1, 7);

		          // density 1..7 → index 0..pool.Length-1, so any sprite-pool size works
		          float t = (density - 1f) / 6f;
		          int idx = Mathf.RoundToInt(t * (pool.Length - 1));
		          return pool[idx];
	     }

	     // ---- Brows (thickness picks the array pair, corner picks the side) ------

	     public static Sprite GetBrowSprite(PepoData pepo, bool inner)
	     {
		          var (innerSet, outerSet) = BodyUtilities.GetExpressedTrait("BrowsThickness", pepo.body) switch
		          {
				1 => (browsInner1, browsOuter1),
				2 => (browsInner2, browsOuter2),
				3 => (browsInner3, browsOuter3),
				_ => (browsInner4, browsOuter4),
		          };
		          return PickTraitSprite(pepo.body, inner ? "BrowInnerCorner" : "BrowOuterCorner",
				   inner ? innerSet : outerSet);
	     }

	     // ---- Moustache (cosmetic, not genetic) ----------------------------------

	     public static Sprite GetMoustacheSprite(PepoData pepo, Sprite[] pool)
	     {
		          if (pepo?.body?.cosmeticFeatures == null || pool == null || pool.Length == 0)
				return null;
		          int idx = pepo.body.cosmeticFeatures.pepoGrooming["Moustache"];
		          if (idx <= 0 || idx >= pool.Length)
				return null;
		          return pool[idx];
	     }

	     // ---- Height offset ------------------------------------------------------

	     public static float GetHeightOffset(BodyData body, string traitName)
	     {
		          if (body?.physicalAttributes == null ||
		              !body.physicalAttributes.TryGetValue(traitName, out var aspect))
				return 0f;

		          return (int)aspect.value switch
		          {
				1 => -1f,
				3 => 1f,
				_ => 0f
		          };
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
