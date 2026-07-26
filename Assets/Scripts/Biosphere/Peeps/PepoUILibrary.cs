using System.Collections.Generic;
using UnityEngine;

public class PepoUILibrary
{
	     public static UISection BuildSectionPepoName(PepoData pepo, UIManager uim, UIStructuralComponent sc)
	     {
		          var section = new UISection("Name", true, new List<UIComponentData>())
		          {
				Padding = sc.Padding,
				Spacing = sc.Spacing,
				VerticalLayout = !sc.VerticalLayout,
				ChildForceExpandWidth = sc.VerticalLayout,  // horizontal strip: expand width
				ChildForceExpandHeight = !sc.VerticalLayout, // vertical strip: expand height
				DisplayLabel = false
		          };

		          // First name (editable)
		          var firstNameField = new TextInputField("First Name", pepo.forename, (int)uim.BodyFontSize)
		          {
				Getter = () => pepo.forename,
				Setter = (val) => pepo.pepoName[0].value = val?.ToString() ?? "Unknown",
				VerticalLayout = false,
				DisplayLabel = true,
				FontSize = (int)uim.BodyFontSize
		          };
		          section.Components.Add(firstNameField);

		          // Last name (editable)
		          var lastNameField = new TextInputField("Last Name", pepo.surname, (int)uim.BodyFontSize)
		          {
				Getter = () => pepo.surname,
				Setter = (val) => pepo.pepoName[1].value = val?.ToString() ?? "Unknown",
				VerticalLayout = false,
				DisplayLabel = true,
				FontSize = (int)uim.BodyFontSize
		          };
		          section.Components.Add(lastNameField);

		          //        // Nicknames (read‑only)
		          //        var nicknamesField = new ReadOnlyField("Nicknames", "", (int)uim.BodyFontSize)
		          //        {
		          //Getter = () => string.Join(", ", GetNicknames(pepo)),
		          //VerticalLayout = false,
		          //DisplayLabel = true,
		          //FontSize = (int)uim.BodyFontSize
		          //        };
		          //section.Components.Add(nicknamesField);

		          return section;
	     }
	     public static UISection BuildSectionPepoPortrait(PepoData pepo, UIManager uim, UIStructuralComponent sc)
	     {
		          //we should just have a field that displays images on top of each other.
		          var section = new UISection($"{pepo.fullname} portrait", true, new List<UIComponentData>())
		          {
		          };
		          section.Components.Add(new PortraitField("Portrait", uim.BodyFontSize));
		          return section;
	     }
	     public static UISection BuildSectionPepoPanel(PepoData pepo, UIManager uim, UIStructuralComponent sc)
	     {
		          //this should be a a panel that displays the pepo's portrait
		          var section = new UISection($"{pepo.fullname} panel", true, new List<UIComponentData>
		          {
				BuildSectionPepoPortrait(pepo, uim, sc), BuildSectionPepoName(pepo, uim, sc)
		          });
		          return section;

	     }

	     public static UISection BuildSectionPepoBiodata(PepoData pepo, UIManager uim, UIStructuralComponent sc)
	     {
		          var section = new UISection("Biodata", true, new List<UIComponentData>())
		          {
				Padding = sc.Padding,
				Spacing = sc.Spacing,
				VerticalLayout = !sc.VerticalLayout,
				ChildForceExpandWidth = sc.VerticalLayout,  // horizontal strip: expand width
				ChildForceExpandHeight = !sc.VerticalLayout, // vertical strip: expand height
				DisplayLabel = false
		          };

		          // Age (read‑only)
		          var ageField = new ReadOnlyField("Age", "", (int)uim.BodyFontSize)
		          {
				Getter = () => Mathf.Floor(pepo.age).ToString(),
				VerticalLayout = false,
				DisplayLabel = true
		          };
		          section.Components.Add(ageField);

		          // Mother
		          var motherField = new ReadOnlyField("Mother", "", (int)uim.BodyFontSize)
		          {
				Getter = () => pepo.mother != null ? pepo.mother.fullname : "Unknown",
				VerticalLayout = false
		          };
		          section.Components.Add(motherField);

		          // Father
		          var fatherField = new ReadOnlyField("Father", "", (int)uim.BodyFontSize)
		          {
				Getter = () => pepo.father != null ? pepo.father.fullname : "Unknown",
				VerticalLayout = false
		          };
		          section.Components.Add(fatherField);

		          // Children (Portrait scroller)
		          var childrenField = new PortraitScrollerField("Children",
		              pepo.knownOffspring ?? new List<PepoData>(),
		             100, 100)
		          {
				VerticalLayout = false,
				DisplayLabel = true
		          };
		          section.Components.Add(childrenField);

		          // Siblings

		          List<PepoData> GetSiblings(PepoData pepo)
		          {
				List<PepoData> siblings = new List<PepoData>();
				if (pepo.mother != null)
				{
					     foreach (var child in pepo.mother.knownOffspring)
					     {
						          if (child != pepo && !siblings.Contains(child))
						          {
								siblings.Add(child);
						          }
					     }
				}
				if (pepo.father != null)
				{
					     foreach (var child in pepo.father.knownOffspring)
					     {
						          if (child != pepo && !siblings.Contains(child))
						          {
								siblings.Add(child);
						          }
					     }
				}
				return siblings;
		          }
		          var siblingsField = new PortraitScrollerField("Siblings",
	         GetSiblings(pepo),
	         200, 100)
		          {
				VerticalLayout = false,
				DisplayLabel = true
		          };
		          section.Components.Add(siblingsField);

		          //        // Roles
		          //        var rolesField = new ReadOnlyField("Roles", "", (int)uim.BodyFontSize)
		          //        {
		          //Getter = () => pepo.,
		          //VerticalLayout = false,
		          //DisplayLabel = true
		          //        };
		          //    section.Components.Add(rolesField);

		          return section;
	     }
	     public static UISection BuildSectionPepoStatus(PepoData pepo, UIManager uim, UIStructuralComponent sc)
	     {
		          var section = new UISection("Status", true, new List<UIComponentData>())
		          {
				Padding = sc.Padding,
				Spacing = sc.Spacing,
				VerticalLayout = !sc.VerticalLayout,
				ChildForceExpandWidth = sc.VerticalLayout,  // horizontal strip: expand width
				ChildForceExpandHeight = !sc.VerticalLayout, // vertical strip: expand height
				DisplayLabel = false
		          };

		          // HEXACO Graph
		          var hexacoField = new GraphField("HEXACO", GraphType.radar, 50f, 100)
		          {
				Getter = () => UIUtilities.CreateHexacoGraph(pepo.personality),
				VerticalLayout = false,
				DisplayLabel = true
		          };
		          section.Components.Add(hexacoField);

		          //        // Maslow Graph
		          //        var maslowField = new GraphField("Maslow", GraphType.bar, 1f, uim.ThumbnailSize.x)
		          //        {
		          //Getter = () => UIUtilities.CreateMaslowGraph(pepo.needs),
		          //VerticalLayout = false,
		          //DisplayLabel = true
		          //        };
		          //        section.Components.Add(maslowField);

		          // Values Graph
		          var valuesField = new GraphField("Values", GraphType.radar, 50f, 100)
		          {
				Getter = () => UIUtilities.CreateValuesGraph(pepo.values),
				VerticalLayout = false,
				DisplayLabel = true
		          };
		          section.Components.Add(valuesField);

		          return section;
	     }

	     public static UISection BuildSectionPepoBodyAppearance(PepoData pepo, UIManager uim, UIStructuralComponent sc)
	     {
		          var section = new UISection("Appearance", true, new List<UIComponentData>())
		          {
				Padding = sc.Padding,
				Spacing = sc.Spacing,
				VerticalLayout = !sc.VerticalLayout,
				ChildForceExpandWidth = sc.VerticalLayout,  // horizontal strip: expand width
				ChildForceExpandHeight = !sc.VerticalLayout, // vertical strip: expand height
				DisplayLabel = false
		          };

		          // Map trait index → name (from BodyData)
		          string[] traitNames = {
        "Head Shape", "Hair Redness", "Hair Darkness", "Eye Shape", "Eye Color",
        "Eye Darkness", "Eye Height", "Nose Shape", "Nose Height", "Top Lip Shape",
        "Bottom Lip Shape", "Mouth Height", "Skin Depth", "Skin Saturation",
        "Skin Hue", "Freckles", "Invert Freckles", "Express Double Freckles",
        "Hair Texture", "Height", "Mass Genetics", "Brows", "Brow Height"
    };

		          var traits = pepo.body.expressedChromosomes;
		          for (int i = 0; i < traits.Count && i < traitNames.Length; i++)
		          {
				int index = i; // capture for lambda
				var trait = traits[index];
				var rangeField = new RangeField(traitNames[index], 1, trait.maxValue, true, (int)uim.BodyFontSize)
				{
					     Getter = () => trait.value,
					     Setter = (val) => trait.value = (float)val,
					     VerticalLayout = false,
					     DisplayLabel = true
				};
				section.Components.Add(rangeField);
		          }

		          return section;
	     }

	     public static UISection BuildSectionPepoPersonalityHEXACO(PepoData pepo, UIManager uim, UIStructuralComponent sc)
	     {
		          var section = new UISection("HEXACO", true, new List<UIComponentData>())
		          {
				Padding = sc.Padding,
				Spacing = sc.Spacing,
				VerticalLayout = !sc.VerticalLayout,
				ChildForceExpandWidth = sc.VerticalLayout,  // horizontal strip: expand width
				ChildForceExpandHeight = !sc.VerticalLayout, // vertical strip: expand height
				DisplayLabel = false
		          };

		          // Build fields for each HEXACO trait
		          var hexacoTraits = new (string name, NumericalAspect aspect)[]
		          {
        ("Honesty", pepo.personality.Honesty),
        ("Emotionality", pepo.personality.Emotionality),
        ("Extraversion", pepo.personality.Extraversion),
        ("Agreeableness", pepo.personality.Agreeableness),
        ("Conscientiousness", pepo.personality.Conscientiousness),
        ("Openness", pepo.personality.Openness)
		          };

		          foreach (var t in hexacoTraits)
		          {
				var field = new RangeField(t.name, 0, 10, true, (int)uim.BodyFontSize)
				{
					     Getter = () => t.aspect.value,
					     Setter = (val) => t.aspect.value = (float)val,
					     VerticalLayout = false,
					     DisplayLabel = true
				};
				section.Components.Add(field);
		          }

		          return section;
	     }
	     public static UISection BuildSectionPepoRelationshipsList(PepoData pepo, UIManager uim, UIStructuralComponent sc)
	     {
		          var section = new UISection("Relationships", true, new List<UIComponentData>())
		          {
				Padding = sc.Padding,
				Spacing = sc.Spacing,
				VerticalLayout = !sc.VerticalLayout,
				ChildForceExpandWidth = sc.VerticalLayout,  // horizontal strip: expand width
				ChildForceExpandHeight = !sc.VerticalLayout, // vertical strip: expand height
				DisplayLabel = false
		          };

		          // For each relationship type, show a summary
		          foreach (var kvp in pepo.RelationshipLibrary)
		          {
				var relType = kvp.Key;
				var relList = kvp.Value;
				if (relList == null || relList.Count == 0)
					     continue;

				var label = new ReadOnlyField(relType.ToString(), "", (int)uim.BodyFontSize)
				{
					     Getter = () => $"{relList.Count} relationships",
					     VerticalLayout = false,
					     DisplayLabel = true
				};
				section.Components.Add(label);
		          }

		          return section;
	     }

	     public static UIWindow BuildWindowPepoData(PepoData pepo, UIManager uim)
	     {
		          UIWindow window = new UIWindow("Pepo Inspector")
		          {
				PreferredSize = new Vector2(450, 650),
				CanResize = true,
				CanClose = true,
				CanMinimize = false
		          };
		          // Create tabs by grouping related sections
		          var infoTab = new UITab("Info", new List<UISection>
    {
        BuildSectionPepoPanel(pepo, uim, window),
        BuildSectionPepoBiodata(pepo, uim, window),
        BuildSectionPepoStatus(pepo, uim,window)
    });

		          var bodyTab = new UITab("Body", new List<UISection>
    {
        BuildSectionPepoBodyAppearance(pepo, uim,window),
        //BuildSectionPepoBodyReproduction(pepo, uim), // if you need it
        // BuildSectionPepoBodyMaslow(pepo, uim)        // if you need it
    });

		          var personalityTab = new UITab("Personality", new List<UISection>
    {
        BuildSectionPepoPersonalityHEXACO(pepo, uim,window),
        //BuildSectionPepoPersonalitySociosexuality(pepo, uim),
        // BuildSectionPepoPersonalityValues(pepo, uim)
    });

		          var relationshipsTab = new UITab("Relationships", new List<UISection>
    {
        BuildSectionPepoRelationshipsList(pepo, uim,window)
        // BuildSectionPepoRelationshipInstance(pepo, uim) // for a specific relationship
    });
		          window.Tabs.Add(infoTab);
		          window.Tabs.Add(bodyTab);
		          window.Tabs.Add(personalityTab);
		          window.Tabs.Add(relationshipsTab);
		          return window;
	     }
}
