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
	     public List<Sprite> freckles;
	     public List<Sprite> bottomLips;
	     public List<Sprite> topLips;
	     public List<Sprite> noseShape;
	     public List<Sprite> eyecolor;

	     public List<Sprite> HairUp_1;
	     public List<Sprite> HairBack_1;
	     public List<Sprite> HairTendrils_1;
	     public List<Sprite> HairFringe_1;
	     //wavy hair
	     public List<Sprite> HairUp_2;
	     public List<Sprite> HairBack_2;
	     public List<Sprite> HairTendrils_2;
	     public List<Sprite> HairFringe_2;
	     //curly hair
	     public List<Sprite> HairUp_3;
	     public List<Sprite> HairBack_3;
	     public List<Sprite> HairTendrils_3;
	     public List<Sprite> HairFringe_3;
	     //coily hair
	     public List<Sprite> HairUp_4;
	     public List<Sprite> HairBack_4;
	     public List<Sprite> HairTendrils_4;
	     public List<Sprite> HairFringe_4;

	     public List<Sprite> brows;
	     public List<Sprite> stubble;
	     public List<Sprite> moustaches;

	     Dictionary<string, List<Sprite>> portraitLibrary;
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
		          GameObject panel = new GameObject($"PepoStatusPanel_{pepo.fullname}");
		          RectTransform panelRT = panel.AddComponent<RectTransform>();
		          panel.AddComponent<CanvasRenderer>();

		          // Add background
		          var panelImage = panel.AddComponent<Image>();
		          panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

		          // Change to Horizontal Layout Group for portrait + info side by side
		          var layoutGroup = panel.AddComponent<VerticalLayoutGroup>();
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
		          GameObject personalPanel = new GameObject("PersonalPanel");
		          RectTransform personalRT = personalPanel.AddComponent<RectTransform>();
		          var PanelLayoutGroup = personalPanel.AddComponent<HorizontalLayoutGroup>();
		          PanelLayoutGroup.padding = new RectOffset(5, 5, 5, 5);
		          PanelLayoutGroup.spacing = 5f;
		          PanelLayoutGroup.childControlHeight = true;
		          PanelLayoutGroup.childForceExpandHeight = false;

		          // Create info container (vertical layout for text and graphs)		          
		          GameObject portraitContainer = PepoUIUtilities.CreatePortrait(pepo, HairUp_1, HairBack_1, HairTendrils_1, HairFringe_1, HairUp_2, HairBack_2, HairTendrils_2, HairFringe_2, HairUp_3, HairBack_3, HairTendrils_3, HairFringe_3, HairUp_4, HairBack_4, HairTendrils_4, HairFringe_4, headShape, eyecolor, mascEyes, femmeEyes, noseShape, topLips, bottomLips, freckles, brows, stubble, moustaches);
		          portraitContainer.transform.SetParent(personalPanel.transform, false);

		          GameObject infoContainer = PepoUIUtilities.CreateInfoContainer(pepo);
		          infoContainer.transform.SetParent(personalPanel.transform, false);
		          personalPanel.transform.SetParent(panel.transform, false);
		          GameObject graphContainer = PepoUIUtilities.CreateGraphContainer(pepo);
		          graphContainer.transform.SetParent(panel.transform, false);
		          //GameObject familyTree = CreateFamilyTree(pepo, 2, panel.transform, new Vector2(panelSize.x - 10f, panelSize.y));
		          return panel;
	     }

	     public GameObject CreateFamilyTree(PepoData pepo, int generations, Transform parent = null, Vector2 size = default)
	     {
		          if (pepo == null)
				return null;

		          if (size == default)
				size = new Vector2(500f, 400f);

		          GameObject tree = new GameObject($"FamilyTree_{pepo.forename}_{pepo.surname}");
		          RectTransform treeRT = tree.AddComponent<RectTransform>();
		          treeRT.sizeDelta = size;

		          if (parent != null)
				tree.transform.SetParent(parent, false);

		          // Main vertical layout for generations
		          var verticalLayout = tree.AddComponent<VerticalLayoutGroup>();
		          verticalLayout.spacing = 3f;
		          verticalLayout.padding = new RectOffset(2, 2, 2, 2);
		          verticalLayout.childControlHeight = false;
		          verticalLayout.childForceExpandHeight = false;

		          // Collect family data (ancestors + root + descendants)
		          var familyData = CollectFamilyData(pepo, generations);

		          // Create each generation with appropriate labels
		          for (int i = 0; i < familyData.Count; i++)
		          {
				string generationLabel = GetGenerationLabel(i, familyData.Count, pepo);
				CreateGenerationPanel(tree.transform, familyData[i], generationLabel, i);
		          }

		          return tree;
	     }

	     private static string GetGenerationLabel(int index, int totalGenerations, PepoData root)
	     {
		          int rootIndex = (totalGenerations - 1) / 2; // Root should be in the middle

		          if (index < rootIndex)
		          {
				// Ancestors
				int generationsBack = rootIndex - index;
				if (generationsBack == 1)
					     return "Parents";
				if (generationsBack == 2)
					     return "Grandparents";
				if (generationsBack == 3)
					     return "Great-Grandparents";
				return $"{generationsBack - 1}x Great-Grandparents";
		          }
		          else if (index == rootIndex)
		          {
				return $"{root.forename} {root.surname}";
		          }
		          else
		          {
				// Descendants
				int generationsForward = index - rootIndex;
				if (generationsForward == 1)
					     return "Children";
				if (generationsForward == 2)
					     return "Grandchildren";
				if (generationsForward == 3)
					     return "Great-Grandchildren";
				return $"{generationsForward - 1}x Great-Grandchildren";
		          }
	     }

	     private void CreateGenerationPanel(Transform parent, List<PepoData> generation, string label, int generationIndex)
	     {
		          GameObject generationPanel = new GameObject($"Generation_{generationIndex}");
		          generationPanel.transform.SetParent(parent, false);

		          // Vertical layout for label + members
		          var verticalLayout = generationPanel.AddComponent<VerticalLayoutGroup>();
		          verticalLayout.spacing = 3f;
		          verticalLayout.childAlignment = TextAnchor.MiddleCenter;

		          var sizeFitter = generationPanel.AddComponent<ContentSizeFitter>();
		          sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		          sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

		          // Add generation label
		          AddGenerationLabel(generationPanel.transform, label);

		          // Horizontal layout for family members
		          GameObject membersContainer = new GameObject("Members");
		          membersContainer.transform.SetParent(generationPanel.transform, false);

		          var horizontalLayout = membersContainer.AddComponent<HorizontalLayoutGroup>();
		          horizontalLayout.spacing = 3f;
		          horizontalLayout.childAlignment = TextAnchor.MiddleCenter;
		          horizontalLayout.childControlHeight = false;
		          horizontalLayout.childForceExpandHeight = false;

		          // Create individual panels for each person in this generation
		          foreach (var person in generation)
		          {
				if (person != null)
				{
					     CreateFamilyMemberPanel(membersContainer.transform, person, 2);
				}
		          }
	     }



	     private static void AddGenerationLabel(Transform parent, string label)
	     {
		          GameObject labelObj = new GameObject("GenerationLabel");
		          labelObj.transform.SetParent(parent, false);

		          var layoutElement = labelObj.AddComponent<LayoutElement>();
		          layoutElement.preferredHeight = 8;
		          layoutElement.minHeight = 6;

		          var text = labelObj.AddComponent<TextMeshProUGUI>();
		          text.text = label;
		          text.fontSize = 4;
		          text.color = Color.white;
		          text.alignment = TextAlignmentOptions.Center;
		          text.fontStyle = FontStyles.Bold;
	     }
	     private static List<List<PepoData>> CollectFamilyData(PepoData root, int maxGenerations)
	     {
		          var familyData = new List<List<PepoData>>();

		          // First, collect all ancestors (from oldest to youngest)
		          var ancestors = CollectAncestors(root, maxGenerations);

		          // Then, collect all descendants (from oldest to youngest)  
		          var descendants = CollectDescendants(root, maxGenerations);

		          // Combine them: ancestors + root + descendants
		          familyData.AddRange(ancestors);
		          familyData.Add(new List<PepoData> { root }); // The root person and their mothers children
		          familyData.AddRange(descendants);

		          return familyData;
	     }

	     private static List<List<PepoData>> CollectAncestors(PepoData root, int maxGenerations)
	     {
		          var ancestors = new List<List<PepoData>>();
		          var currentGeneration = new List<PepoData> { root };

		          for (int gen = 1; gen <= maxGenerations; gen++)
		          {
				var parentGeneration = new List<PepoData>();

				foreach (var person in currentGeneration)
				{
					     if (person?.mother != null && !parentGeneration.Contains(person.mother))
						          parentGeneration.Add(person.mother);
					     if (person?.father != null && !parentGeneration.Contains(person.father))
						          parentGeneration.Add(person.father);
				}

				if (parentGeneration.Count > 0)
				{
					     ancestors.Insert(0, parentGeneration); // Insert at beginning to maintain chronological order
					     currentGeneration = parentGeneration;
				}
				else
				{
					     break;
				}
		          }

		          return ancestors;
	     }

	     private static List<List<PepoData>> CollectDescendants(PepoData root, int maxGenerations)
	     {
		          var descendants = new List<List<PepoData>>();
		          var currentGeneration = new List<PepoData> { root };

		          for (int gen = 1; gen <= maxGenerations; gen++)
		          {
				var childGeneration = new List<PepoData>();

				foreach (var person in currentGeneration)
				{
					     if (person?.offspring != null)
					     {
						          foreach (var child in person.offspring)
						          {
								if (child != null && !childGeneration.Contains(child))
									     childGeneration.Add(child);
						          }
					     }
				}

				if (childGeneration.Count > 0)
				{
					     descendants.Add(childGeneration);
					     currentGeneration = childGeneration;
				}
				else
				{
					     break;
				}
		          }

		          return descendants;
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
		          GameObject portrait = PepoUIUtilities.CreatePortrait(person, HairUp_1, HairBack_1, HairTendrils_1, HairFringe_1, HairUp_2, HairBack_2, HairTendrils_2, HairFringe_2, HairUp_3, HairBack_3, HairTendrils_3, HairFringe_3, HairUp_4, HairBack_4, HairTendrils_4, HairFringe_4, headShape, eyecolor, mascEyes, femmeEyes, noseShape, topLips, bottomLips, freckles, brows, stubble, moustaches);
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

