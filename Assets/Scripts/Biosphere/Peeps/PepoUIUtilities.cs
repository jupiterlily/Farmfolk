using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PepoUIUtilities
{

	     public static GameObject CreatePortrait(PepoData pepo, Sprite[] sHairUp, Sprite[] sHairBack, Sprite[] sHairTendrils, Sprite[] sHairFront, Sprite[] wHairUp, Sprite[] wHairBack, Sprite[] wHairTendrils, Sprite[] wHairFront, Sprite[] cHairUp, Sprite[] cHairBack, Sprite[] cHairTendrils, Sprite[] cHairFront, Sprite[] kHairUp, Sprite[] kHairBack, Sprite[] kHairTendrils, Sprite[] kHairFront, Sprite[] heads, Sprite[] eyeColours, Sprite[] mascEyes, Sprite[] femmeEyes, Sprite[] noses, Sprite[] topLips, Sprite[] bottomLips, Sprite[] freckles, Sprite[] brows, Sprite[] stubbleSprites, Sprite[] moustaches)
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
		          float browHeightOffset = GetHeightOffset(pepo.body, 22); // browHeight trait	

		          Color hairColor = UIUtilities.GetPepoColors(pepo.body)[0];
		          Color eyeColor = UIUtilities.GetPepoColors(pepo.body)[1];
		          Color skinColor = UIUtilities.GetPepoColors(pepo.body)[2];
		          Color mouthColor = UIUtilities.GetPepoColors(pepo.body)[3];

		          if (pepo.age > 30f)
		          {
				hairColor = Color.Lerp(hairColor, Color.white, Mathf.Clamp01((pepo.age - 40f) / 60f));
		          }
		          //for stubble, lerp between hair colour and transparent based on stubble level (0-5)
		          Color stubbleColor = Color.Lerp(hairColor, Color.clear, 1f - (pepo.body.cosmeticFeatures.pepoGrooming["Stubble"] / 5f));

		          PortraitLayer HairUp = null;
		          PortraitLayer HairBack = null;
		          PortraitLayer HairTendrils = null;
		          PortraitLayer HairFront = null;

		          if (pepo.body.expressedChromosomes[18].value == 1)
		          {
				if (pepo.body.cosmeticFeatures.pepoGrooming["HairUp"] == -1 && pepo.body.cosmeticFeatures.pepoGrooming["HairBack"] == -1 && pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"] == -1 && pepo.body.cosmeticFeatures.pepoGrooming["HairFront"] == -1)
				{
					     pepo.body.cosmeticFeatures.pepoGrooming["HairUp"] = Random.Range(0, sHairUp.Length);
					     pepo.body.cosmeticFeatures.pepoGrooming["HairBack"] = Random.Range(0, sHairBack.Length);
					     pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"] = Random.Range(0, sHairTendrils.Length);
					     pepo.body.cosmeticFeatures.pepoGrooming["HairFront"] = Random.Range(0, sHairFront.Length);
				}
				HairUp = new PortraitLayer(sHairUp[pepo.body.cosmeticFeatures.pepoGrooming["HairUp"]], "HairUp", hairColor, Vector2.zero);
				HairBack = new PortraitLayer(sHairBack[pepo.body.cosmeticFeatures.pepoGrooming["HairBack"]], "HairBack", hairColor, Vector2.zero);
				HairTendrils = new PortraitLayer(sHairTendrils[pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"]], "HairTendrils", hairColor, Vector2.zero);
				HairFront = new PortraitLayer(sHairFront[pepo.body.cosmeticFeatures.pepoGrooming["HairFront"]], "HairFront", hairColor, Vector2.zero);
		          }
		          else if (pepo.body.expressedChromosomes[18].value == 2)
		          {
				if (pepo.body.cosmeticFeatures.pepoGrooming["HairUp"] == -1 && pepo.body.cosmeticFeatures.pepoGrooming["HairBack"] == -1 && pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"] == -1 && pepo.body.cosmeticFeatures.pepoGrooming["HairFront"] == -1)
				{
					     pepo.body.cosmeticFeatures.pepoGrooming["HairUp"] = Random.Range(0, wHairUp.Length);
					     pepo.body.cosmeticFeatures.pepoGrooming["HairBack"] = Random.Range(0, wHairBack.Length);
					     pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"] = Random.Range(0, wHairTendrils.Length);
					     pepo.body.cosmeticFeatures.pepoGrooming["HairFront"] = Random.Range(0, wHairFront.Length);
				}
				HairUp = new PortraitLayer(wHairUp[pepo.body.cosmeticFeatures.pepoGrooming["HairUp"]], "HairUp", hairColor, Vector2.zero);
				HairBack = new PortraitLayer(wHairBack[pepo.body.cosmeticFeatures.pepoGrooming["HairBack"]], "HairBack", hairColor, Vector2.zero);
				HairTendrils = new PortraitLayer(wHairTendrils[pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"]], "HairTendrils", hairColor, Vector2.zero);
				HairFront = new PortraitLayer(wHairFront[pepo.body.cosmeticFeatures.pepoGrooming["HairFront"]], "HairFront", hairColor, Vector2.zero);
		          }
		          else if (pepo.body.expressedChromosomes[18].value == 3)
		          {
				if (pepo.body.cosmeticFeatures.pepoGrooming["HairUp"] == -1 && pepo.body.cosmeticFeatures.pepoGrooming["HairBack"] == -1 && pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"] == -1 && pepo.body.cosmeticFeatures.pepoGrooming["HairFront"] == -1)
				{
					     pepo.body.cosmeticFeatures.pepoGrooming["HairUp"] = Random.Range(0, cHairUp.Length);
					     pepo.body.cosmeticFeatures.pepoGrooming["HairBack"] = Random.Range(0, cHairBack.Length);
					     pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"] = Random.Range(0, cHairTendrils.Length);
					     pepo.body.cosmeticFeatures.pepoGrooming["HairFront"] = Random.Range(0, cHairFront.Length);
				}
				HairUp = new PortraitLayer(cHairUp[pepo.body.cosmeticFeatures.pepoGrooming["HairUp"]], "HairUp", hairColor, Vector2.zero);
				HairBack = new PortraitLayer(cHairBack[pepo.body.cosmeticFeatures.pepoGrooming["HairBack"]], "HairBack", hairColor, Vector2.zero);
				HairTendrils = new PortraitLayer(cHairTendrils[pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"]], "HairTendrils", hairColor, Vector2.zero);
				HairFront = new PortraitLayer(cHairFront[pepo.body.cosmeticFeatures.pepoGrooming["HairFront"]], "HairFront", hairColor, Vector2.zero);

		          }
		          else if (pepo.body.expressedChromosomes[18].value == 4)
		          {
				if (pepo.body.cosmeticFeatures.pepoGrooming["HairUp"] == -1 && pepo.body.cosmeticFeatures.pepoGrooming["HairBack"] == -1 && pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"] == -1 && pepo.body.cosmeticFeatures.pepoGrooming["HairFront"] == -1)
				{
					     pepo.body.cosmeticFeatures.pepoGrooming["HairUp"] = Random.Range(0, kHairUp.Length);
					     pepo.body.cosmeticFeatures.pepoGrooming["HairBack"] = Random.Range(0, kHairBack.Length);
					     pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"] = Random.Range(0, kHairTendrils.Length);
					     pepo.body.cosmeticFeatures.pepoGrooming["HairFront"] = Random.Range(0, kHairFront.Length);
				}
				HairUp = new PortraitLayer(kHairUp[pepo.body.cosmeticFeatures.pepoGrooming["HairUp"]], "HairUp", hairColor, Vector2.zero);
				HairBack = new PortraitLayer(kHairBack[pepo.body.cosmeticFeatures.pepoGrooming["HairBack"]], "HairBack", hairColor, Vector2.zero);
				HairTendrils = new PortraitLayer(kHairTendrils[pepo.body.cosmeticFeatures.pepoGrooming["HairTendrils"]], "HairTendrils", hairColor, Vector2.zero);
				HairFront = new PortraitLayer(kHairFront[pepo.body.cosmeticFeatures.pepoGrooming["HairFront"]], "HairFront", hairColor, Vector2.zero);
		          }



		          PortraitLayer Head = new PortraitLayer(GetHeadSprite(pepo, heads), "Head", skinColor, Vector2.zero);
		          PortraitLayer Nose = new PortraitLayer(GetNoseSprite(pepo, noses), "Nose", skinColor, new Vector2(0, noseHeightOffset));

		          PortraitLayer Stubble = new PortraitLayer(GetStubbleSprite(pepo, stubbleSprites), "Stubble", stubbleColor, Vector2.zero);
		          PortraitLayer Eyebrows = new PortraitLayer(GetBrowSprite(pepo, brows), "Eyebrows", hairColor, new Vector2(0, browHeightOffset));

		          PortraitLayer EyeColour = new PortraitLayer(GetEyeColourSprite(pepo, eyeColours), "EyeColour", eyeColor, new Vector2(0, eyeHeightOffset));
		          PortraitLayer EyeShape = new PortraitLayer(GetEyeSprite(pepo, mascEyes, femmeEyes), "EyeShape", Color.white, new Vector2(0, eyeHeightOffset));

		          //  PortraitLayer Beard = new PortraitLayer(GetMoustacheSprite(pepo, beards), "Beard", hairColor, Vector2.zero);
		          PortraitLayer BottomLip = new PortraitLayer(GetMouthSprite(pepo, bottomLips, false), "BottomLip", mouthColor, new Vector2(0, mouthHeightOffset));
		          PortraitLayer TopLip = new PortraitLayer(GetMouthSprite(pepo, topLips, true), "TopLip", mouthColor, new Vector2(0, mouthHeightOffset));
		          PortraitLayer Moustache = new PortraitLayer(GetMoustacheSprite(pepo, moustaches), "Moustache", hairColor, new Vector2(0, mouthHeightOffset + noseHeightOffset));



		          pepo.portrait = new PepoPortrait(new List<PortraitLayer> { HairUp, HairBack, Head, Nose, Stubble, Eyebrows, EyeColour, EyeShape, BottomLip, TopLip, Moustache, HairTendrils, HairFront });

		          foreach (var layer in pepo.portrait.layers)
		          {
				CreateLayeredSprite(portrait.transform, layer.sprite, layer.layerName, layer.sortingOrder, layer.color, layer.positionOffset.y);
		          }

		          //CreateLayeredSprite(portrait.transform, GetHairBackSprite(pepo, fHairBack, fHairFront, mHairBack, mHairFront, uHairBack, uHairFront), "HairBack", 1, hairColor);

		          //        CreateLayeredSprite(portrait.transform, GetHeadSprite(pepo, heads), "Head", 2, UIUtilities.GetPepoColors(pepo.body)[2]);
		          //        CreateLayeredSprite(portrait.transform, GetNoseSprite(pepo, noses), "Nose", 4, UIUtilities.GetPepoColors(pepo.body)[2], noseHeightOffset);
		          //        //freckles
		          AddFrecklesSprite(pepo, freckles, portrait.transform);
		          //CreateLayeredSprite(portrait.transform, GetStubbleSprite(pepo, stubbleSprites), "Stubble", 4, stubbleColor);
		          ////brows. use hair colour
		          //CreateLayeredSprite(portrait.transform, GetBrowSprite(pepo, brows), "Brows", 3, hairColor, eyeHeightOffset + browHeightOffset);
		          //CreateLayeredSprite(portrait.transform, GetEyeColourSprite(pepo, eyeColours), "EyeColour", 3, UIUtilities.GetPepoColors(pepo.body)[1], eyeHeightOffset);
		          //CreateLayeredSprite(portrait.transform, GetEyeSprite(pepo, mascEyes, femmeEyes), "Eyes", 4, Color.white, eyeHeightOffset);
		          ////do bottom lip then top lip
		          //CreateLayeredSprite(portrait.transform, GetMouthSprite(pepo, bottomLips, false), "Bottom Lip", 5, UIUtilities.GetPepoColors(pepo.body)[3], mouthHeightOffset);
		          //CreateLayeredSprite(portrait.transform, GetMouthSprite(pepo, topLips, true), "Top Lip", 5, UIUtilities.GetPepoColors(pepo.body)[3], mouthHeightOffset);
		          //CreateLayeredSprite(portrait.transform, GetMoustacheSprite(pepo, moustaches), "Moustache", 6, hairColor, mouthHeightOffset + noseHeightOffset);

		          // NEW: Add stubble/facial hair if applicable
		          //AddFacialHairSprite(pepo, stubbleSprites, moustaches, portrait.transform, hairColor);

		          //age details for wrinkles. use head genetics as reference
		          //if they're less than 3 years old they have no hair sprite
		          if (pepo.age < 1)
		          {
				return portrait;
		          }
		          //CreateLayeredSprite(portrait.transform, GetHairFrontSprite(pepo, fHairBack, fHairFront, mHairBack, mHairFront, uHairBack, uHairFront), "HairFront", 6, hairColor);
		          //if they're above acertain age put in the wrinkle sprites for eyes, 

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

	     public static Sprite[] GetHairList(SocioSexualData ssd, bool isBackHair, Sprite[] fHairBack, Sprite[] fHairFront, Sprite[] mHairBack, Sprite[] mHairFront, Sprite[] uHairBack, Sprite[] uHairFront)
	     {
		          int fHairCount = isBackHair ? fHairBack.Length : fHairFront.Length;
		          int mHairCount = isBackHair ? mHairBack.Length : mHairFront.Length;
		          int uHairCount = isBackHair ? uHairBack.Length : uHairFront.Length;
		          // Determine which hair list to use based on gender presentation
		          if (ssd.presentsFemme && !ssd.presentsMasc)
		          {
				//pepos who present femme and not masc can have hair from femme and unisex categories
				//comnbine the two arrays into one and return it
				Sprite[] combinedHair = new Sprite[fHairCount + uHairCount];
				combinedHair.CopyTo(isBackHair ? fHairBack : fHairFront, 0);
				combinedHair.CopyTo(isBackHair ? uHairBack : uHairFront, fHairCount);
				return combinedHair;
		          }
		          else if (ssd.presentsMasc && !ssd.presentsFemme)
		          {
				Sprite[] combinedHair = new Sprite[mHairCount + uHairCount];
				combinedHair.CopyTo(isBackHair ? mHairBack : mHairFront, 0);
				combinedHair.CopyTo(isBackHair ? uHairBack : uHairFront, mHairCount);
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
				Sprite[] combinedHair = new Sprite[fHairCount + mHairCount + uHairCount];
				combinedHair.CopyTo(isBackHair ? fHairBack : fHairFront, 0);
				combinedHair.CopyTo(isBackHair ? mHairBack : mHairFront, fHairCount);
				combinedHair.CopyTo(isBackHair ? uHairBack : uHairFront, fHairCount + mHairCount);
				return combinedHair;
		          }
	     }
	     // NEW METHOD: Add facial hair sprites based on cosmetic choices
	     public static void AddFacialHairSprite(PepoData pepo, Sprite[] stubbleSprites, Sprite[] moustaches, Transform parent, Color hairColor)
	     {
		          // Check if cosmetic features exist and if any facial hair is selected
		          if (pepo?.body?.cosmeticFeatures == null)
		          {
				Debug.LogWarning("null cosmetic features");
				return;
		          }
		          if (pepo.body.cosmeticFeatures.pepoGrooming["Stubble"] == 0)
		          {
				Debug.LogWarning($"No stubble for{pepo.fullname}");
		          }
		          if (pepo.body.cosmeticFeatures.pepoGrooming["Moustache"] == 0)
		          {
				Debug.LogWarning($"No moustache for{pepo.fullname}");
		          }

		          CosmeticFeatures cosmetics = pepo.body.cosmeticFeatures;
		          Debug.LogWarning($"Adding facial hair for {pepo.fullname}: Stubble Level {cosmetics.pepoGrooming["Stubble"]}, Moustache Style {cosmetics.pepoGrooming["Moustache"]}");
		          // Add stubble if present
		          if (cosmetics.pepoGrooming["Stubble"] > 0 && stubbleSprites != null && stubbleSprites.Length > 0)
		          {
				Debug.LogWarning($"Adding stubble level {cosmetics.pepoGrooming["Stubble"]} for {pepo.fullname}");
				//stubble sprite is determined by head shape. theyre in the same order. Sprite becomes more opaque with higher levels
				int stubbleIndex = pepo.body.GetExpressedTrait(0); // headShape trait
				stubbleIndex = Mathf.Clamp(stubbleIndex - 1, 0, stubbleSprites.Length - 1);
				Sprite stubbleSprite = stubbleSprites[stubbleIndex];

				Color stubbleColor = new Color(hairColor.r, hairColor.g, hairColor.b, pepo.body.cosmeticFeatures.pepoGrooming["Stubble"] / 5);
				CreateLayeredSprite(parent, stubbleSprite, "Stubble", 4, stubbleColor);
		          }
		          int moustacheIndex = cosmetics.pepoGrooming["Moustache"];
		          // Add moustache if present
		          if (moustacheIndex > 0 && moustaches != null && moustaches.Length > 0)
		          {
				Debug.LogWarning($"Adding moustache style {moustacheIndex} for {pepo.fullname}");
				Sprite moustacheSprite = moustaches[moustacheIndex];

				CreateLayeredSprite(parent, moustacheSprite, "Moustache", 6, hairColor);
		          }
	     }
	     //Get Moustache Sprite
	     public static Sprite GetMoustacheSprite(PepoData pepo, Sprite[] moustaches)
	     {
		          if (pepo?.body?.cosmeticFeatures == null || moustaches == null || moustaches.Length == 0)
				return null;
		          int moustacheIndex = pepo.body.cosmeticFeatures.pepoGrooming["Moustache"];
		          if (moustacheIndex <= 0 || moustacheIndex >= moustaches.Length)
				return null;
		          return moustaches[moustacheIndex];
	     }
	     //Get stubble sprite based on head shape
	     public static Sprite GetStubbleSprite(PepoData pepo, Sprite[] stubbleSprites)
	     {
		          if (pepo?.body?.cosmeticFeatures == null || stubbleSprites == null || stubbleSprites.Length == 0)
				return null;
		          int stubbleIndex = pepo.body.GetExpressedTrait(0); // headShape trait
		          stubbleIndex = Mathf.Clamp(stubbleIndex - 1, 0, stubbleSprites.Length - 1);
		          return stubbleSprites[stubbleIndex];
	     }

	     // Stable hair index based on name hash
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
	     public static Sprite GetHeadSprite(PepoData pepo, Sprite[] headShape)
	     {
		          if (pepo?.body == null || headShape == null || headShape.Length == 0)
				return null;

		          int headIndex = pepo.body.GetExpressedTrait(0); // headShape trait
		          Sprite head = headShape[Mathf.Clamp(headIndex - 1, 0, headShape.Length - 1)];
		          return head;
	     }
	     public static Sprite GetEyeColourSprite(PepoData pepo, Sprite[] eyecolor)
	     {
		          if (pepo?.body == null)
				return null;
		          //if eye darkness is above a threshold use the 2nd sprite (darker eye color)
		          int eyeColorIndex = pepo.body.GetExpressedTrait(5); // eyeDarkness trait
		          if (eyeColorIndex <= 1)
		          {
				return eyecolor[0];
		          }
		          else if (eyeColorIndex == eyecolor.Length - 1)
		          {
				return eyecolor[2];
		          }
		          else
		          {
				return eyecolor[1];
		          }
	     }
	     public static Sprite GetEyeSprite(PepoData pepo, Sprite[] mascEyes, Sprite[] femmeEyes)
	     {
		          if (pepo?.body == null)
				return null;

		          int eyeShapeIndex = pepo.body.GetExpressedTrait(3); // eyeShape trait
		          Sprite[] eyeList = (!pepo.ssd.isFemme && !pepo.ssd.presentsFemme) ? mascEyes : femmeEyes;
		          if (eyeList == null || eyeList.Length == 0)
				return null;

		          return eyeList[Mathf.Clamp(eyeShapeIndex - 1, 0, eyeList.Length - 1)];
	     }
	     public static Sprite GetBrowSprite(PepoData pepo, Sprite[] brows)
	     {
		          if (pepo?.body == null || brows == null || brows.Length == 0)
				return null;

		          int browindex = pepo.body.GetExpressedTrait(21); // brow shape trait
		          Sprite brow = brows[Mathf.Clamp(browindex - 1, 0, brows.Length - 1)];
		          return brow;
	     }
	     public static Sprite GetNoseSprite(PepoData pepo, Sprite[] noseShape)
	     {
		          if (pepo?.body == null || noseShape == null || noseShape.Length == 0)
				return null;

		          int noseIndex = pepo.body.GetExpressedTrait(7); // noseShape trait
		          return noseShape[Mathf.Clamp(noseIndex - 1, 0, noseShape.Length - 1)];
	     }

	     public static Sprite GetMouthSprite(PepoData pepo, Sprite[] mouthShape, bool topLip)
	     {
		          if (pepo?.body == null || mouthShape == null || mouthShape.Length == 0)
				return null;
		          int mouthIndex = 0;
		          if (topLip)
		          {

				mouthIndex = pepo.body.GetExpressedTrait(9); // mouthShape trait
		          }
		          else
		          {
				mouthIndex = pepo.body.GetExpressedTrait(10); // mouthShape trait
		          }

		          return mouthShape[Mathf.Clamp(mouthIndex - 1, 0, mouthShape.Length - 1)];
	     }

	     // Hair selection methods based on SocioSexualData
	     public static Sprite GetHairBackSprite(PepoData pepo, Sprite[] fHairBack, Sprite[] fHairFront, Sprite[] mHairBack, Sprite[] mHairFront, Sprite[] uHairBack, Sprite[] uHairFront)
	     {
		          if (pepo?.ssd == null || uHairBack == null)
				return null;

		          Sprite[] hairList = GetHairList(pepo.ssd, true, fHairBack, fHairFront, mHairBack, mHairFront, uHairBack, uHairFront); // true for back hair
		          if (hairList == null || hairList.Length == 0)
				return null;

		          // Use a consistent random seed based on pepo's name for consistent hair selection
		          int hairIndex = GetStableHairIndex(pepo, hairList.Length);
		          return hairList[hairIndex];
	     }

	     public static Sprite GetHairFrontSprite(PepoData pepo, Sprite[] fHairBack, Sprite[] fHairFront, Sprite[] mHairBack, Sprite[] mHairFront, Sprite[] uHairBack, Sprite[] uHairFront)
	     {
		          Sprite[] hairList = GetHairList(pepo.ssd, false, fHairBack, fHairFront, mHairBack, mHairFront, uHairBack, uHairFront); // false for front hair
		          if (hairList == null || hairList.Length == 0)
				return null;

		          // Use same seed to ensure front/back hair match
		          int hairIndex = GetStableHairIndex(pepo, hairList.Length);
		          return hairList[hairIndex];
	     }
	     public static void AddFrecklesSprite(PepoData pepo, Sprite[] freckles, Transform parent)
	     {
		          if (pepo?.body == null || freckles == null || freckles.Length == 0)
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

		          int primaryFreckleIndex = Mathf.Clamp(frecklesTrait - 1, 0, freckles.Length - 1);
		          int secondaryFreckleIndex = Mathf.Clamp((isMaternalExpressed ? paternalFreckles : maternalFreckles) - 1, 0, freckles.Length - 1);

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
