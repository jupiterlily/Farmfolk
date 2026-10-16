using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFactory
{
	     private static Color ResolveColor(UITheme theme, Color fallback)
	     {
		          return theme != null ? fallback : fallback;
	     }

	     private static float ResolveFloat(UITheme theme, float themeValue, float fallback)
	     {
		          return theme != null ? themeValue : fallback;
	     }

	     private static Vector2 ResolvePanelSize(Transform parent, UITheme theme, float fallbackWidth, float fallbackHeight)
	     {
		          if (parent != null)
		          {
				RectTransform rt = parent.GetComponent<RectTransform>();
				if (rt != null && rt.rect.size.x > 0f && rt.rect.size.y > 0f)
					     return rt.rect.size;
		          }

		          return new Vector2(ResolveFloat(theme, theme != null ? theme.DefaultWindowMinWidth : 0f, fallbackWidth), ResolveFloat(theme, theme != null ? theme.DefaultWindowMinHeight : 0f, fallbackHeight));
	     }

	     // --- Public API -------------------------------------------------
	     // In UIFactory.cs — replace CreateWindow and CreateTabs

	     //  public GameObject CreateWindow(UIWindow windowDef, Transform parent, UIManager uim)
	     //  {
	     //        GameObject windowGO = new GameObject(windowDef.ComponentName);
	     //        windowGO.transform.SetParent(parent, false);

	     //        Image bg = windowGO.AddComponent<Image>();
	     //        bg.color = new Color(0.15f, 0.15f, 0.2f, 0.95f);

	     //        UIUtilities.ApplyLayoutGroupData(windowGO, windowDef);

	     //        UIWindowController controller = windowGO.AddComponent<UIWindowController>();

	     //        List<GameObject> tabGOs = new List<GameObject>();
	     //        List<GameObject> buttonGOs = new List<GameObject>();

	     //        if (windowDef.Tabs != null && windowDef.Tabs.Count > 0)
	     //        {
	     //// Button strip — axis is perpendicular to the window

	     ////if there's more than one tab we need to create index tabs as a new section.
	     //if (windowDef.Tabs.Count > 1)
	     //{


	     //	     //link up the tab buttons actions here.
	     //	     foreach (UITab tabDef in windowDef.Tabs)
	     //	     {
	     //		          GameObject tabGO = CreateTab(tabDef, windowGO.transform);
	     //		          tabGOs.Add(tabGO);
	     //	     }

	     //	     CreateSection(UILibrary.BuildSectionDataIndexTabs(uim, windowDef.Tabs, windowDef), parent);

	     //	     //now the game objects for these buttons live on the fields within the section we just  created on the window

	     //	     controller.RegisterTabClick(windowDef, tabGOs, buttonGOs);

	     //}
	     //else
	     //{

	     //	     // Tabs
	     //	     foreach (UITab tabDef in windowDef.Tabs)
	     //	     {
	     //		          GameObject tabGO = CreateTab(tabDef, windowGO.transform);
	     //		          tabGOs.Add(tabGO);
	     //	     }
	     //}
	     //        }
	     //        else
	     //        {
	     //Debug.LogWarning($"Window '{windowDef.ComponentName}' has no tabs.");
	     //        }

	     //        windowDef.ComponentObject = windowGO;
	     //        return windowGO;
	     //  }
	     public GameObject CreateWindow(UIWindow windowDef, Transform parent, UIManager uim)
	     {
		          GameObject windowGO = new GameObject(windowDef.ComponentName);
		          windowGO.transform.SetParent(parent, false);

		          Image bg = windowGO.AddComponent<Image>();
		          bg.color = uim.theme != null ? uim.theme.WindowBG : new Color(0.15f, 0.15f, 0.2f, 0.95f);

		          UIUtilities.ApplyLayoutGroupData(windowGO, windowDef);
		          //windowGO.AddComponent<ContentSizeFitter>();
		          UIWindowController controller = windowGO.AddComponent<UIWindowController>();

		          List<GameObject> tabGOs = new List<GameObject>();
		          List<GameObject> buttonGOs = new List<GameObject>();
		          UISection buttonStripSection = null;

		          if (windowDef.Tabs != null && windowDef.Tabs.Count > 1)
		          {
				buttonStripSection = UILibrary.BuildSectionDataIndexTabs(uim, windowDef.Tabs, windowDef);
				CreateSection(buttonStripSection, windowGO.transform, uim.theme);

				foreach (var comp in buttonStripSection.Components)
				{
					     if (comp is ButtonField bf && bf.ComponentObject != null)
						          buttonGOs.Add(bf.ComponentObject);
				}
		          }

		          foreach (UITab tabDef in windowDef.Tabs)
		          {
				GameObject tabGO = CreateTab(tabDef, windowGO.transform, uim.theme);
				//tabGO.AddComponent<ContentSizeFitter>();
				tabGOs.Add(tabGO);
		          }

		          controller.Register(windowDef, tabGOs, buttonGOs);

		          if (buttonStripSection != null)
		          {
				for (int i = 0; i < buttonStripSection.Components.Count; i++)
				{
					     var comp = buttonStripSection.Components[i];
					     if (comp is ButtonField bf)
					     {
						          int index = i;
						          bf.OnClick = () => controller.SwitchTab(index);
						          if (bf.UnityButton != null)
						          {
								bf.UnityButton.onClick.RemoveAllListeners();
								bf.UnityButton.onClick.AddListener(() => bf.OnClick?.Invoke());
						          }
					     }
				}
		          }

		          RectTransform parentRect = parent != null ? parent.GetComponent<RectTransform>() : null;
		          Vector2 derivedWindowSize = UIUtilities.ResolveAndApplySize(windowDef, uim.theme, parentRect);
		          //  bg.minWidth = derivedWindowSize.x;
		          //   bg.minHeight = derivedWindowSize.y;
		          UIUtilities.ApplyLayoutElementData(windowGO, windowDef);
		          windowDef.ComponentObject = windowGO;
		          return windowGO;
	     }

	     //        UISection stripLayout = new UISection("ButtonStrip", !windowDef.VerticalLayout)
	     //        {
	     //Padding = new RectOffset(2, 2, 2, 2),
	     //Spacing = 2,
	     //VerticalLayout = !windowDef.VerticalLayout,
	     //ChildForceExpandWidth = windowDef.VerticalLayout,  // horizontal strip: expand width
	     //ChildForceExpandHeight = !windowDef.VerticalLayout // vertical strip: expand height
	     //        };
	     //        UIUtilities.ApplyLayoutGroupData(stripGO, stripLayout);

	     //        UIWindowController controller = parent.GetComponent<UIWindowController>();
	     //        int index = 0;

	     //        foreach (UITab tabDef in windowDef.Tabs)
	     //        {
	     //int capturedIndex = index;
	     //ButtonField btnDef = new ButtonField("", tabDef.ComponentName, () =>
	     //{
	     //	     controller.SwitchTab(capturedIndex);
	     //});

	     //GameObject btnGO = CreateButton(btnDef, stripGO.transform);
	     //buttonGOs.Add(btnGO);
	     //index++;
	     //        }

	     //        return stripGO;
	     //  }

	     private GameObject CreateTab(UITab tabDef, Transform parent, UITheme theme)
	     {
		          GameObject tabGO = new GameObject($"Tab_{tabDef.ComponentName}");
		          tabGO.transform.SetParent(parent, false);

		          foreach (UISection sectionDef in tabDef.Sections)
		          {
				CreateSection(sectionDef, tabGO.transform, theme);
		          }

		          RectTransform parentRect = parent != null ? parent.GetComponent<RectTransform>() : null;
		          UIUtilities.ResolveAndApplySize(tabDef, theme, parentRect);
		          UIUtilities.ApplyLayoutGroupData(tabGO, tabDef);
		          tabGO.SetActive(false);
		          return tabGO;
	     }

	     // --- Section Creation -------------------------------------------
	     private void CreateSection(UISection sectionDef, Transform parent, UITheme theme)
	     {
		          // Create a container for the section
		          GameObject sectionGO = new GameObject(sectionDef.ComponentName);
		          sectionGO.transform.SetParent(parent, false);

		          // Add RectTransform
		          RectTransform sectionRT = sectionGO.AddComponent<RectTransform>();
		          sectionRT.sizeDelta = new Vector2(0, 0); // will be controlled by layout

		          // Add a background image (optional)
		          Image bg = sectionGO.AddComponent<Image>();
		          bg.color = theme != null ? theme.SectionBG : new Color(0.2f, 0.2f, 0.3f, 0.5f);

		          // Store reference
		          sectionDef.ComponentObject = sectionGO;

		          // Recursively create components
		          foreach (UIComponentData comp in sectionDef.Components)
		          {
				if (comp is UISection subSection)
				{
					     CreateSection(subSection, sectionGO.transform, theme);
				}
				else if (comp is UIField field)
				{
					     CreateField(field, sectionGO.transform, theme);
				}
		          }
		          RectTransform parentRect = sectionGO.transform.parent != null ? sectionGO.transform.parent.GetComponent<RectTransform>() : null;
		          UIUtilities.ResolveAndApplySize(sectionDef, theme, parentRect);
		          UIUtilities.ApplyLayoutGroupData(sectionGO, sectionDef);
		          //  sectionGO.AddComponent<ContentSizeFitter>();
	     }

	     private void CreateField(UIField fieldDef, Transform parent, UITheme theme)
	     {
		          // Base container for the field (label + control)
		          GameObject fieldGO = new GameObject(fieldDef.ComponentName);
		          fieldGO.transform.SetParent(parent, false);



		          // Create label
		          if (fieldDef.DisplayLabel)
		          {

				// --- Label creation ---
				GameObject labelGO = new GameObject($"Label_{fieldDef.ComponentName}");
				labelGO.transform.SetParent(fieldGO.transform, false);
				TextMeshProUGUI tmp = labelGO.AddComponent<TextMeshProUGUI>();
				tmp.text = fieldDef.LabelText;
				tmp.fontSize = theme != null ? Mathf.RoundToInt(theme.BodyFontSize) : fieldDef.FontSize;
				tmp.alignment = TextAlignmentOptions.MidlineLeft;
				tmp.color = Color.white;

				// Add LayoutElement to label
				LayoutElement labelLE = labelGO.AddComponent<LayoutElement>();
				Vector2 labelSize = UIUtilities.GetLabelSize(fieldDef.LabelText, theme != null ? Mathf.RoundToInt(theme.BodyFontSize) : fieldDef.FontSize);
				float labelMinWidth = Mathf.Max(labelSize.x, theme != null ? theme.LabelMinWidth : 30f);
				float labelMinHeight = Mathf.Max(labelSize.y, theme != null ? theme.ControlHeight : fieldDef.FontSize + 4f);
				labelLE.minWidth = labelMinWidth;
				//labelLE.preferredWidth = labelMinWidth;
				labelLE.minHeight = labelMinHeight;
				labelLE.preferredHeight = labelMinHeight;
				labelLE.flexibleWidth = 0; // prevents stretching

				// Optionally set anchors so it stays left
				RectTransform labelRT = labelGO.GetComponent<RectTransform>();
				labelRT.anchorMin = new Vector2(0, 0.5f);
				labelRT.anchorMax = new Vector2(0, 0.5f);
				labelRT.pivot = new Vector2(0, 0.5f);
				labelRT.sizeDelta = new Vector2(labelSize.x, labelSize.y);
		          }


		          // Create the control based on field type
		          GameObject controlGO = null;
		          switch (fieldDef)
		          {
		          case TextInputField textField:
		          controlGO = CreateTextInput(textField, fieldGO.transform, theme);
		          break;
		          case ReadOnlyField readField:
		          controlGO = CreateReadOnly(readField, fieldGO.transform, theme);
		          break;
		          case RangeField rangeField:
		          controlGO = CreateRange(rangeField, fieldGO.transform, theme);
		          break;
		          case ToggleField toggleField:
		          controlGO = CreateToggle(toggleField, fieldGO.transform, theme);
		          break;
		          case DropdownField dropdownField:
		          controlGO = CreateDropdown(dropdownField, fieldGO.transform, theme);
		          break;
		          case SpriteField imageField:
		          controlGO = CreateImageDisplay(imageField, fieldGO.transform, theme);
		          break;
		          case GraphField graphField:
		          controlGO = CreateGraph(graphField, fieldGO.transform, theme);
		          break;
		          case ButtonField buttonField:
		          controlGO = CreateButton(buttonField, fieldGO.transform, theme);
		          break;
		          case PortraitField portraitField:
		          controlGO = CreatePortrait(portraitField, fieldGO.transform, theme);
		          break;
		          default:
		          Debug.LogWarning($"Unknown field type: {fieldDef.GetType()}");
		          controlGO = new GameObject("Unsupported");
		          controlGO.transform.SetParent(fieldGO.transform, false);
		          break;

		          }

		          // Make control flexible width. this is probably wrong but also where we should set our layout properties, probably. 
		          if (controlGO != null)
		          {
				LayoutElement controlLE = controlGO.GetComponent<LayoutElement>();
				if (controlLE == null)
					     controlLE = controlGO.AddComponent<LayoutElement>();
				controlLE.flexibleWidth = 1;
				controlLE.preferredWidth = 0;
		          }
		          // Add a horizontal layout to place label and control side by side
		          RectTransform parentRect = parent != null ? parent.GetComponent<RectTransform>() : null;
		          UIUtilities.ResolveAndApplySize(fieldDef, theme, parentRect);
		          UIUtilities.ApplyLayoutGroupData(fieldGO, fieldDef);
		          // Store reference
		          fieldDef.ComponentObject = fieldGO;
	     }

	     // --- Individual Control Creators ---------------------------------
	     private GameObject CreatePortrait(PortraitField def, Transform parent, UITheme theme)
	     {
		          GameObject go = PepoUIUtilities.CreatePortrait(def.subject);
		          go.transform.SetParent(parent, false);

		          return go;
	     }
	     private GameObject CreateTextInput(TextInputField def, Transform parent, UITheme theme)
	     {
		          GameObject go = new GameObject("TextInput");
		          go.transform.SetParent(parent, false);

		          // Background image (so it's visible and clickable)
		          Image bg = go.AddComponent<Image>();
		          bg.color = theme != null ? theme.InputBackground : new Color(0.1f, 0.1f, 0.1f, 0.5f);
		          bg.raycastTarget = true;

		          TMP_InputField input = go.AddComponent<TMP_InputField>();
		          input.text = def.DefaultText;
		          input.characterLimit = def.MaxLength;

		          // Text component
		          GameObject textGO = new GameObject("Text");
		          textGO.transform.SetParent(go.transform, false);
		          TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
		          tmp.fontSize = theme != null ? Mathf.RoundToInt(theme.BodyFontSize) : def.FontSize;
		          tmp.color = theme != null ? theme.InputTextColor : Color.white;
		          tmp.alignment = TextAlignmentOptions.MidlineLeft;
		          RectTransform textRT = textGO.GetComponent<RectTransform>();
		          textRT.anchorMin = Vector2.zero;
		          textRT.anchorMax = Vector2.one;
		          textRT.offsetMin = new Vector2(5, 2);
		          textRT.offsetMax = new Vector2(-5, -2);
		          input.textComponent = tmp;

		          // Placeholder
		          GameObject placeholderGO = new GameObject("Placeholder");
		          placeholderGO.transform.SetParent(go.transform, false);
		          TextMeshProUGUI placeholderTmp = placeholderGO.AddComponent<TextMeshProUGUI>();
		          placeholderTmp.text = "Enter text...";
		          placeholderTmp.fontSize = theme != null ? Mathf.RoundToInt(theme.BodyFontSize) : def.FontSize;
		          placeholderTmp.color = theme != null ? theme.InputPlaceholderColor : Color.gray;
		          placeholderTmp.alignment = TextAlignmentOptions.MidlineLeft;
		          RectTransform placeholderRT = placeholderGO.GetComponent<RectTransform>();
		          placeholderRT.anchorMin = Vector2.zero;
		          placeholderRT.anchorMax = Vector2.one;
		          placeholderRT.offsetMin = new Vector2(5, 2);
		          placeholderRT.offsetMax = new Vector2(-5, -2);
		          input.placeholder = placeholderTmp;

		          // Data binding
		          if (def.Getter != null)
				input.text = def.GetValue<string>();
		          input.onValueChanged.AddListener((val) => def.SetValue(val));

		          // --- CRITICAL: Apply layout properties ---
		          UIUtilities.ApplyLayoutGroupData(go, def);

		          return go;
	     }

	     private GameObject CreateReadOnly(ReadOnlyField def, Transform parent, UITheme theme)
	     {
		          GameObject go = new GameObject("ReadOnly");
		          go.transform.SetParent(parent, false);
		          TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
		          tmp.text = def.DefaultText;
		          tmp.fontSize = theme != null ? Mathf.RoundToInt(theme.BodyFontSize) : def.FontSize;
		          tmp.color = theme != null ? theme.TextColor : Color.white;
		          tmp.alignment = TextAlignmentOptions.MidlineLeft;
		          // Set rect transform
		          RectTransform rt = go.GetComponent<RectTransform>();
		          rt.anchorMin = new Vector2(0, 0.5f);
		          rt.anchorMax = new Vector2(1, 0.5f);
		          rt.pivot = new Vector2(0.5f, 0.5f);
		          rt.sizeDelta = new Vector2(0, theme != null ? theme.ControlHeight : 30f);
		          rt.anchoredPosition = Vector2.zero;

		          // For binding, we'll update text via a timer or explicit call.
		          // We can expose a method to update.
		          if (def.Getter != null)
				tmp.text = def.GetValue<string>();

		          // We'll use a simple polling approach: we could register for updates.
		          // For now, we just set once; the UIManager can call UpdateAllReadOnlyFields.
		          // Store reference for later updates.
		          def.Text = tmp;          // ✅ store reference
		          def.Refresh();           // ✅ initialise from data
		          return go;
	     }

	     private GameObject CreateRange(RangeField def, Transform parent, UITheme theme)
	     {
		          GameObject go = new GameObject("Range");
		          go.transform.SetParent(parent, false);

		          // --- Layout Element: give the slider a minimum height ---
		          LayoutElement mainLE = go.AddComponent<LayoutElement>();
		          float rangeHeight = theme != null ? theme.ControlHeight : def.FontSize + 12f;
		          mainLE.minHeight = rangeHeight;
		          mainLE.preferredHeight = rangeHeight;
		          mainLE.flexibleHeight = 0;

		          // --- Background (the track) ---
		          Image bg = go.AddComponent<Image>();
		          bg.color = theme != null ? theme.SliderTrackColor : new Color(0.15f, 0.15f, 0.15f, 0.8f);
		          bg.raycastTarget = true;

		          Slider slider = go.AddComponent<Slider>();
		          slider.minValue = def.MinValue;
		          slider.maxValue = def.MaxValue;
		          slider.wholeNumbers = def.UseIntStep;
		          slider.direction = Slider.Direction.LeftToRight;

		          // --- FILL AREA (the progress bar background) ---
		          GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
		          fillArea.transform.SetParent(go.transform, false);
		          RectTransform fillAreaRT = fillArea.GetComponent<RectTransform>();
		          fillAreaRT.anchorMin = new Vector2(0, 0.2f);
		          fillAreaRT.anchorMax = new Vector2(1, 0.8f);
		          fillAreaRT.offsetMin = new Vector2(8, 0);
		          fillAreaRT.offsetMax = new Vector2(-8, 0);

		          // --- FILL (the colored progress) ---
		          GameObject fill = new GameObject("Fill", typeof(RectTransform));
		          fill.transform.SetParent(fillArea.transform, false);
		          RectTransform fillRT = fill.GetComponent<RectTransform>();
		          fillRT.anchorMin = Vector2.zero;
		          fillRT.anchorMax = Vector2.zero; // Anchored to left – slider will stretch it
		          fillRT.pivot = new Vector2(0, 0.5f);
		          fillRT.sizeDelta = new Vector2(0, 0);
		          Image fillImage = fill.AddComponent<Image>();
		          fillImage.color = theme != null ? theme.SliderFillColor : new Color(0.2f, 0.5f, 0.8f);
		          slider.fillRect = fillRT;

		          // --- HANDLE SLIDE AREA (the clickable zone) ---
		          GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
		          handleArea.transform.SetParent(go.transform, false);
		          RectTransform handleAreaRT = handleArea.GetComponent<RectTransform>();
		          handleAreaRT.anchorMin = Vector2.zero;
		          handleAreaRT.anchorMax = Vector2.one;
		          handleAreaRT.sizeDelta = Vector2.zero;
		          handleAreaRT.offsetMin = Vector2.zero;
		          handleAreaRT.offsetMax = Vector2.zero;

		          // --- HANDLE (the draggable circle) ---
		          GameObject handle = new GameObject("Handle", typeof(RectTransform));
		          handle.transform.SetParent(handleArea.transform, false);
		          RectTransform handleRT = handle.GetComponent<RectTransform>();
		          handleRT.anchorMin = new Vector2(0, 0.5f);
		          handleRT.anchorMax = new Vector2(0, 0.5f);
		          handleRT.pivot = new Vector2(0.5f, 0.5f);
		          float handleSize = theme != null ? theme.SliderHandleSize : 18f;
		          handleRT.sizeDelta = new Vector2(handleSize, handleSize);
		          handleRT.anchoredPosition = Vector2.zero;
		          Image handleImage = handle.AddComponent<Image>();
		          handleImage.color = theme != null ? theme.SliderHandleColor : Color.white;
		          slider.handleRect = handleRT;

		          // --- VALUE LABEL ---
		          GameObject valueLabel = new GameObject("Value");
		          valueLabel.transform.SetParent(go.transform, false);
		          TextMeshProUGUI tmp = valueLabel.AddComponent<TextMeshProUGUI>();
		          tmp.fontSize = theme != null ? Mathf.RoundToInt(theme.SmallFontSize) : def.FontSize - 2;
		          tmp.color = theme != null ? theme.TextColor : Color.white;
		          tmp.alignment = TextAlignmentOptions.MidlineRight;
		          RectTransform valueRT = valueLabel.GetComponent<RectTransform>();
		          valueRT.anchorMin = new Vector2(1, 0.5f);
		          valueRT.anchorMax = new Vector2(1, 0.5f);
		          valueRT.pivot = new Vector2(1, 0.5f);
		          valueRT.sizeDelta = new Vector2(theme != null ? theme.ControlMinWidth : 50f, theme != null ? theme.ControlHeight : 30f);
		          valueRT.anchoredPosition = new Vector2(-8, 0);

		          // --- Initial value ---
		          if (def.Getter != null)
				slider.value = def.GetValue<float>();

		          // --- Update label & data on change ---
		          slider.onValueChanged.AddListener((val) =>
		          {
				tmp.text = def.UseIntStep ? ((int)val).ToString() : val.ToString("F1");
				def.SetValue(val);
		          });
		          tmp.text = slider.value.ToString(def.UseIntStep ? "0" : "F1");

		          // Store reference
		          def.Slider = slider;
		          def.Refresh();

		          // --- Apply layout ---
		          UIUtilities.ApplyLayoutGroupData(go, def);

		          return go;
	     }
	     private GameObject CreateToggle(ToggleField def, Transform parent, UITheme theme)
	     {
		          GameObject go = new GameObject("Toggle");
		          go.transform.SetParent(parent, false);
		          Toggle toggle = go.AddComponent<Toggle>();
		          // Add a label for the toggle (we already have a label, but toggle can have its own)
		          // We'll just use the toggle's built-in label? Actually we can add a child text.
		          GameObject labelGO = new GameObject("ToggleLabel");
		          labelGO.transform.SetParent(go.transform, false);
		          TextMeshProUGUI tmp = labelGO.AddComponent<TextMeshProUGUI>();
		          tmp.text = " "; // empty
		          tmp.fontSize = theme != null ? Mathf.RoundToInt(theme.BodyFontSize) : def.FontSize;
		          tmp.color = theme != null ? theme.TextColor : Color.white;
		          tmp.alignment = TextAlignmentOptions.MidlineLeft;
		          // We already have a main label, so we might not need this; we can set toggle.isOn and add listener.

		          if (def.Getter != null)
				toggle.isOn = def.GetValue<bool>();
		          toggle.onValueChanged.AddListener((val) => def.SetValue(val));

		          // Layout: toggle should be small, maybe align left.
		          LayoutElement le = go.AddComponent<LayoutElement>();
		          float toggleSize = theme != null ? theme.MinToggleWidth : 30f;
		          le.preferredWidth = toggleSize;
		          le.minWidth = toggleSize;
		          return go;
	     }
	     private GameObject CreateDropdown(DropdownField def, Transform parent, UITheme theme)
	     {
		          // 1. Main GameObject
		          GameObject go = new GameObject("Dropdown", typeof(RectTransform));
		          go.transform.SetParent(parent, false);

		          RectTransform mainRT = go.GetComponent<RectTransform>();
		          mainRT.anchorMin = new Vector2(0, 0.5f);
		          mainRT.anchorMax = new Vector2(1, 0.5f);
		          mainRT.pivot = new Vector2(0.5f, 0.5f);
		          float dropdownHeight = theme != null ? theme.ControlHeight : def.FontSize + 8f;
		          mainRT.sizeDelta = new Vector2(theme != null ? theme.MinDropdownWidth : 100f, dropdownHeight);

		          TMP_Dropdown dropdown = go.AddComponent<TMP_Dropdown>();
		          def.dropdown = dropdown;

		          // 2. Caption (visible selected option)
		          GameObject captionGO = new GameObject("Caption", typeof(RectTransform));
		          captionGO.transform.SetParent(go.transform, false);
		          RectTransform captionRT = captionGO.GetComponent<RectTransform>();
		          captionRT.anchorMin = Vector2.zero;
		          captionRT.anchorMax = Vector2.one;
		          captionRT.offsetMin = new Vector2(5, 2);
		          captionRT.offsetMax = new Vector2(-5, -2);
		          TextMeshProUGUI captionText = captionGO.AddComponent<TextMeshProUGUI>();
		          captionText.fontSize = theme != null ? Mathf.RoundToInt(theme.BodyFontSize) : def.FontSize;
		          captionText.color = theme != null ? theme.TextColor : Color.white;
		          captionText.alignment = TextAlignmentOptions.MidlineLeft;
		          dropdown.captionText = captionText;

		          // 3. Template (dropdown list) – child of dropdown
		          GameObject templateGO = new GameObject("Template", typeof(RectTransform));
		          templateGO.transform.SetParent(go.transform, false);
		          templateGO.SetActive(false);
		          RectTransform templateRT = templateGO.GetComponent<RectTransform>();
		          templateRT.anchorMin = new Vector2(0, 1);
		          templateRT.anchorMax = new Vector2(1, 1);
		          templateRT.pivot = new Vector2(0.5f, 1);
		          templateRT.sizeDelta = new Vector2(0, theme != null ? theme.DropdownListMaxHeight : 150f); // Height will auto-size

		          // Background image for the list
		          Image templateBg = templateGO.AddComponent<Image>();
		          templateBg.color = theme != null ? theme.DropdownBackgroundColor : new Color(0.15f, 0.15f, 0.2f, 0.95f);
		          templateBg.raycastTarget = true; // Block clicks behind list

		          // Prevent parent layout from affecting template
		          LayoutElement le = templateGO.AddComponent<LayoutElement>();
		          le.ignoreLayout = true;

		          // 4. ScrollRect
		          ScrollRect scrollRect = templateGO.AddComponent<ScrollRect>();
		          scrollRect.horizontal = false;
		          scrollRect.vertical = true;

		          // 5. Viewport
		          GameObject viewportGO = new GameObject("Viewport", typeof(RectTransform));
		          viewportGO.transform.SetParent(templateGO.transform, false);
		          RectTransform viewportRT = viewportGO.GetComponent<RectTransform>();
		          viewportRT.anchorMin = Vector2.zero;
		          viewportRT.anchorMax = Vector2.one;
		          viewportRT.sizeDelta = Vector2.zero;
		          viewportRT.offsetMin = Vector2.zero;
		          viewportRT.offsetMax = Vector2.zero;
		          viewportGO.AddComponent<Mask>().showMaskGraphic = false;
		          Image viewportImage = viewportGO.AddComponent<Image>();
		          viewportImage.color = theme != null ? theme.InputBackground : new Color(0.1f, 0.1f, 0.1f, 0.95f);

		          // 6. Content
		          GameObject contentGO = new GameObject("Content", typeof(RectTransform));
		          contentGO.transform.SetParent(viewportGO.transform, false);
		          RectTransform contentRT = contentGO.GetComponent<RectTransform>();
		          contentRT.anchorMin = new Vector2(0, 1);
		          contentRT.anchorMax = new Vector2(1, 1);
		          contentRT.pivot = new Vector2(0.5f, 1);
		          contentRT.sizeDelta = new Vector2(0, 0);

		          VerticalLayoutGroup vlg = contentGO.AddComponent<VerticalLayoutGroup>();
		          vlg.childAlignment = TextAnchor.UpperLeft;
		          vlg.childControlHeight = true;
		          vlg.childControlWidth = true;
		          vlg.childForceExpandHeight = false;
		          vlg.childForceExpandWidth = true;
		          vlg.spacing = 2;

		          ContentSizeFitter csf = contentGO.AddComponent<ContentSizeFitter>();
		          csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

		          // 7. Item Template (each option)
		          GameObject itemGO = new GameObject("Item", typeof(RectTransform));
		          itemGO.transform.SetParent(contentGO.transform, false);
		          RectTransform itemRT = itemGO.GetComponent<RectTransform>();
		          // Set anchors so the item stretches horizontally and is pinned to top
		          itemRT.anchorMin = new Vector2(0, 1);
		          itemRT.anchorMax = new Vector2(1, 1);
		          itemRT.pivot = new Vector2(0.5f, 1);
		          float itemHeight = theme != null ? theme.ControlHeight : def.FontSize + 8f;
		          itemRT.sizeDelta = new Vector2(0, itemHeight); // height will be set by layout group

		          LayoutElement itemLE = itemGO.AddComponent<LayoutElement>();
		          itemLE.minHeight = itemHeight;
		          itemLE.preferredHeight = itemHeight;
		          itemLE.flexibleHeight = 0;
		          itemLE.flexibleWidth = 1;

		          Toggle toggle = itemGO.AddComponent<Toggle>();
		          toggle.isOn = false;
		          toggle.group = null;

		          Image itemImage = itemGO.AddComponent<Image>();
		          itemImage.color = theme != null ? theme.DropdownItemNormal : new Color(0.2f, 0.2f, 0.2f);
		          itemImage.raycastTarget = true; // allow clicking
		          toggle.targetGraphic = itemImage;

		          // Label inside Toggle
		          GameObject itemLabelGO = new GameObject("Label", typeof(RectTransform));
		          itemLabelGO.transform.SetParent(itemGO.transform, false);
		          RectTransform itemLabelRT = itemLabelGO.GetComponent<RectTransform>();
		          itemLabelRT.anchorMin = Vector2.zero;
		          itemLabelRT.anchorMax = Vector2.one;
		          itemLabelRT.offsetMin = new Vector2(5, 0);
		          itemLabelRT.offsetMax = new Vector2(-5, 0);
		          TextMeshProUGUI itemText = itemLabelGO.AddComponent<TextMeshProUGUI>();
		          itemText.fontSize = theme != null ? Mathf.RoundToInt(theme.BodyFontSize) : def.FontSize;
		          itemText.color = theme != null ? theme.TextColor : Color.white;
		          itemText.alignment = TextAlignmentOptions.MidlineLeft;

		          // 8. Link everything
		          scrollRect.viewport = viewportRT;
		          scrollRect.content = contentRT;

		          dropdown.template = templateRT;
		          dropdown.itemText = itemText;

		          // 9. Populate options
		          List<string> optionList = def.OptionsGetter?.Invoke() ?? def.Options;
		          if (optionList != null)
		          {
				dropdown.options.Clear();
				foreach (string opt in optionList)
					     dropdown.options.Add(new TMP_Dropdown.OptionData(opt));
				dropdown.RefreshShownValue();
		          }

		          // 10. Bind value
		          if (def.Getter != null)
				dropdown.value = (int)def.Getter();
		          dropdown.onValueChanged.AddListener((val) => def.Setter?.Invoke(val));

		          // Store reference for later refresh
		          def.dropdown = dropdown;

		          // 11. Apply layout to the main dropdown GameObject
		          UIUtilities.ApplyLayoutGroupData(go, def);

		          return go;
	     }
	     private GameObject CreateImageDisplay(SpriteField sprite, Transform parent, UITheme theme)
	     {
		          GameObject go = new GameObject("ImageDisplay");
		          go.transform.SetParent(parent, false);
		          Image img = go.AddComponent<Image>();
		          img.sprite = sprite.Image;
		          img.preserveAspect = true;
		          LayoutElement le = go.AddComponent<LayoutElement>();
		          float imageSize = theme != null ? theme.MinImageSize : 100f;
		          le.preferredWidth = imageSize;
		          le.preferredHeight = imageSize;
		          le.minWidth = imageSize;
		          le.minHeight = imageSize;
		          return go;
	     }

	     private GameObject CreateGraph(GraphField def, Transform parent, UITheme theme)
	     {
		          if (def.Getter == null)
				return new GameObject("EmptyGraph");
		          Graph graphData = def.GetValue<Graph>();
		          if (graphData == null)
				return new GameObject("EmptyGraph");

		          float graphSize = theme != null ? Mathf.Max(def.Size, theme.MinGraphSize) : def.Size;
		          GameObject graphGO = null;
		          switch (def.GraphType)
		          {
		          case GraphType.radar:
		          graphGO = GraphFactory.CreateRadarGraph(graphData, parent, graphSize, def.ComponentName);
		          break;
		          case GraphType.bar:
		          graphGO = GraphFactory.CreateBarGraph(graphData, parent, graphSize, def.ComponentName);
		          break;
		          // add line, pie later if needed
		          default:
		          graphGO = GraphFactory.CreateRadarGraph(graphData, parent, graphSize, def.ComponentName);
		          break;
		          }

		          if (graphGO != null)
		          {
				LayoutElement le = graphGO.AddComponent<LayoutElement>();
				le.preferredWidth = graphSize;
				le.preferredHeight = graphSize;
				le.minWidth = graphSize;
				le.minHeight = graphSize;
				// Ensure anchors
				RectTransform rt = graphGO.GetComponent<RectTransform>();
				if (rt != null)
				{
					     rt.anchorMin = new Vector2(0.5f, 0.5f);
					     rt.anchorMax = new Vector2(0.5f, 0.5f);
					     rt.pivot = new Vector2(0.5f, 0.5f);
					     rt.sizeDelta = new Vector2(graphSize, graphSize);
					     rt.anchoredPosition = Vector2.zero;
				}
		          }
		          return graphGO ?? new GameObject("NullGraph");
	     }

	     private GameObject CreateButton(ButtonField def, Transform parent, UITheme theme)
	     {
		          GameObject go = new GameObject($"Button_{def.LabelText}");
		          go.transform.SetParent(parent, false);
		          UnityEngine.UI.Button btn = go.AddComponent<UnityEngine.UI.Button>();
		          def.UnityButton = btn; // store

		          // Label
		          TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
		          tmp.text = def.LabelText;
		          tmp.fontSize = theme != null ? Mathf.RoundToInt(theme.BodyFontSize) : def.FontSize;
		          tmp.alignment = TextAlignmentOptions.Center;
		          tmp.color = theme != null ? theme.ButtonTextColor : Color.white;
		          RectTransform labelRT = go.GetComponent<RectTransform>();
		          labelRT.anchorMin = Vector2.zero;
		          labelRT.anchorMax = Vector2.one;
		          labelRT.offsetMin = Vector2.zero;
		          labelRT.offsetMax = Vector2.zero;

		          // Colors
		          ColorBlock colors = btn.colors;
		          colors.normalColor = theme != null ? theme.ButtonNormalColor : new Color(0.2f, 0.4f, 0.6f);
		          colors.highlightedColor = theme != null ? theme.ButtonHighlightedColor : new Color(0.3f, 0.5f, 0.7f);
		          colors.pressedColor = theme != null ? theme.ButtonPressedColor : new Color(0.1f, 0.2f, 0.3f);
		          btn.colors = colors;

		          // Listener will be added later – keep it empty for now
		          // (We'll add after wiring OnClick)
		          btn.onClick.AddListener(() => def.OnClick?.Invoke());
		          // Layout
		          LayoutElement le = go.AddComponent<LayoutElement>();
		          float buttonMinWidth = def.MinimumSize.x > 0f ? def.MinimumSize.x : (theme != null ? theme.MinButtonWidth : 60f);
		          float buttonMinHeight = def.MinimumSize.y > 0f ? def.MinimumSize.y : (theme != null ? theme.ButtonHeight : 24f);
		          le.minWidth = buttonMinWidth;
		          le.minHeight = buttonMinHeight;
		          le.preferredWidth = buttonMinWidth;
		          le.preferredHeight = buttonMinHeight;
		          le.flexibleWidth = 1; // Stretch in a strip
		          le.flexibleHeight = 0;

		          return go;
	     }
}