using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PepoUIManager : MonoBehaviour
{
	     // Add sprite references for portrait generation
	     public Sprite[] mascEyes;
	     public Sprite[] femmeEyes;
	     public Sprite[] headShape;
	     public Sprite[] freckles;
	     public Sprite[] bottomLips;
	     public Sprite[] topLips;
	     public Sprite[] nostrils;
	     public Sprite[] noseBridge;
	     public Sprite[] eyecolor;

	     public Sprite[] EyesMascInner;
	     public Sprite[] EyesFemmeInner;
	     public Sprite[] EyesMascOuter;
	     public Sprite[] EyesFemmeOuter;

	     public Sprite[] browsInner1;
	     public Sprite[] browsOuter1;
	     public Sprite[] browsInner2;
	     public Sprite[] browsOuter2;
	     public Sprite[] browsInner3;
	     public Sprite[] browsOuter3;
	     public Sprite[] browsInner4;
	     public Sprite[] browsOuter4;

	     public Sprite[] HairUp_1;
	     public Sprite[] HairBack_1;
	     public Sprite[] HairTendrils_1;
	     public Sprite[] HairFringe_1;
	     //wavy hair
	     public Sprite[] HairUp_2;
	     public Sprite[] HairBack_2;
	     public Sprite[] HairTendrils_2;
	     public Sprite[] HairFringe_2;
	     //curly hair
	     public Sprite[] HairUp_3;
	     public Sprite[] HairBack_3;
	     public Sprite[] HairTendrils_3;
	     public Sprite[] HairFringe_3;
	     //coily hair
	     public Sprite[] HairUp_4;
	     public Sprite[] HairBack_4;
	     public Sprite[] HairTendrils_4;
	     public Sprite[] HairFringe_4;

	     public Sprite[] brows;
	     public Sprite[] stubble;
	     public Sprite[] teeth;
	     public Sprite[] moustaches;
	     public Sprite[] ears;

	     //take a 2d texture and slice it into a 2d array of sprites. this is for the pepo appearance sprites, which are used to generate the pepo portraits. 
	     //public Texture2D mascEyesTexture;
	     //public Texture2D femmeEyesTexture;
	     // public Sprite[] teefies;
	     public Sprite[][] pepoAppearanceSprites;

	     Dictionary<string, List<Sprite>> portraitLibrary;

	     private void Awake()
	     {
		          //put all the lists in an array so that appearace data can get its values from here and bodydata can read from here instead of having to set hard values. adding more sprites just means adding them to the array and not having to change any code in bodydata or pepodata. this is a more modular approach.
		          pepoAppearanceSprites = new Sprite[][] { mascEyes, femmeEyes, headShape, freckles, bottomLips, topLips, nostrils, eyecolor, HairUp_1, HairBack_1, HairTendrils_1, HairFringe_1, HairUp_2, HairBack_2, HairTendrils_2, HairFringe_2, HairUp_3, HairBack_3, HairTendrils_3, HairFringe_3, HairUp_4, HairBack_4, HairTendrils_4, HairFringe_4, brows, stubble, teeth, moustaches, ears, noseBridge, EyesMascInner, EyesFemmeInner, EyesMascOuter, EyesFemmeOuter, browsInner1, browsOuter1, browsInner2, browsOuter2, browsInner3, browsOuter3, browsInner4, browsOuter4 };
		          PepoUIUtilities.InitialiseSprites(pepoAppearanceSprites);
		          if (portraitLibrary == null)
		          {
				portraitLibrary = new Dictionary<string, List<Sprite>>();
		          }



	     }
	     private static void AddGenerationLabel(Transform parent, int generationNumber, int totalGenerations)
	     {
		          GameObject labelObj = new GameObject("GenerationLabel");
		          labelObj.transform.SetParent(parent, false);

		          var layoutElement = labelObj.AddComponent<LayoutElement>();
		          layoutElement.preferredWidth = 50;
		          layoutElement.minWidth = 20;

		          var text = labelObj.AddComponent<TextMeshProUGUI>();
		          text.text = GetGenerationLabel(generationNumber, totalGenerations);
		          text.fontSize = 4;
		          text.color = Color.gray;
		          text.alignment = TextAlignmentOptions.Center;
		          text.verticalAlignment = VerticalAlignmentOptions.Middle;
	     }

	     private static string GetGenerationLabel(int generationNumber, int totalGenerations)
	     {
		          if (generationNumber == 0)
				return "Self";
		          if (generationNumber == 1)
				return "Parents";
		          if (generationNumber == 2)
				return "Grandparents";
		          if (generationNumber == 3)
				return "Great-Grandparents";
		          return $"{generationNumber}G Back";
	     }

	     private void CreateFamilyMemberPanel(Transform parent, PepoData person, int generationNumber)
	     {
		          GameObject memberPanel = new GameObject($"FamilyMember_{person.forename}");
		          memberPanel.transform.SetParent(parent, false);

		          // Vertical layout for portrait + name
		          var verticalLayout = memberPanel.AddComponent<VerticalLayoutGroup>();
		          verticalLayout.spacing = 2f;
		          verticalLayout.padding = new RectOffset(2, 2, 2, 2);
		          verticalLayout.childAlignment = TextAnchor.MiddleCenter;
		          verticalLayout.childControlHeight = false;
		          verticalLayout.childForceExpandHeight = false;
		          verticalLayout.childControlWidth = true;
		          verticalLayout.childForceExpandWidth = true;


		          var layoutElement = memberPanel.AddComponent<LayoutElement>();
		          layoutElement.preferredWidth = 30;
		          layoutElement.minWidth = 30;
		          layoutElement.flexibleWidth = 0f;

		          // Create portrait then make it smaller etc.
		          GameObject portrait = PepoUIUtilities.CreatePortrait(person);
		          portrait.transform.SetParent(memberPanel.transform, false);
		          portrait.transform.localScale = Vector3.one * 0.25f;

		          // Add name and basic info
		          AddMemberInfo(memberPanel.transform, person, generationNumber);


	     }

	     private static void AddMemberInfo(Transform parent, PepoData person, int generationNumber)
	     {
		          GameObject infoPanel = new GameObject("MemberInfo");
		          infoPanel.transform.SetParent(parent, false);

		          var verticalLayout = infoPanel.AddComponent<VerticalLayoutGroup>();
		          verticalLayout.spacing = 1f;
		          verticalLayout.childControlHeight = false;
		          verticalLayout.childForceExpandHeight = false;

		          // Name
		          CreateFamilyTreeText(infoPanel.transform, person.forename, 5, FontStyles.Normal, Color.white);

		          // Age and status for older generations
		          if (generationNumber > 0)
		          {
				string status = person.isAlive ? $"Age: {person.age}" : "Deceased";
				Color statusColor = person.isAlive ? Color.white : Color.gray;
				CreateFamilyTreeText(infoPanel.transform, status, 4, FontStyles.Normal, statusColor);
		          }
	     }

	     private static void CreateFamilyTreeText(Transform parent, string text, int fontSize, FontStyles style, Color color)
	     {
		          GameObject textGO = new GameObject("FamilyTreeText");
		          textGO.transform.SetParent(parent, false);

		          var layoutElement = textGO.AddComponent<LayoutElement>();
		          layoutElement.preferredHeight = 6;
		          layoutElement.minHeight = 4;

		          var tmpText = textGO.AddComponent<TextMeshProUGUI>();
		          tmpText.text = text;
		          tmpText.fontSize = fontSize;
		          tmpText.fontStyle = style;
		          tmpText.color = color;
		          tmpText.alignment = TextAlignmentOptions.Center;
		          tmpText.verticalAlignment = VerticalAlignmentOptions.Middle;
	     }
}

