using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtilities
{// Pale "empty" bases — the colour each part looks like with zero pigment.
	     private static readonly Color Keratin = Color.HSVToRGB(0.11f, 0.10f, 0.92f);  // pale cream
	     private static readonly Color PaleSkin = Color.HSVToRGB(0.06f, 0.25f, 0.95f);  // pale peach
	     private static readonly Color Sclera = new Color(0.95f, 0.94f, 0.92f);       // off-white
	     public static UISection NewSection(string name, UIStructuralComponent sc) =>
    new UISection(name, true, new List<UIComponentData>())
    {
	         Padding = sc.Padding,
	         Spacing = sc.Spacing,
	         DisplayLabel = false,
    };

	     public static RangeField TraitRange(string label, NumericalAspect aspect, float min, float max) =>
	         new RangeField(label, min, max, true)
	         {
		              Getter = () => aspect.value,
		              Setter = v => aspect.value = (float)v,
		              DisplayLabel = true,
	         };
	     public static Vector2 GetVector2FromString(string text)
	     {
		          if (string.IsNullOrWhiteSpace(text))
				return Vector2.zero;

		          // Remove parentheses and spaces, then split by comma
		          string cleaned = text.Trim('(', ')', ' ');
		          string[] parts = cleaned.Split(',');

		          if (parts.Length != 2)
		          {
				// Optionally throw or return default; here we return (0,0) for malformed input
				return Vector2.zero;
		          }

		          // Parse with invariant culture to handle '.' as decimal separator
		          if (float.TryParse(parts[0].Trim(), System.Globalization.NumberStyles.Float,
			              System.Globalization.CultureInfo.InvariantCulture, out float x) &&
		              float.TryParse(parts[1].Trim(), System.Globalization.NumberStyles.Float,
			              System.Globalization.CultureInfo.InvariantCulture, out float y))
		          {
				return new Vector2(x, y);
		          }

		          return Vector2.zero; // fallback for parsing failure
	     }
	     // Approximate width of a text label
	     public static Vector2 GetLabelSize(string text, int fontSize)
	     {
		          float width = text.Length * fontSize * 0.6f + 8; // 0.6 = average char width factor
		          float height = fontSize + 8;
		          return new Vector2(width, height);
	     }

	     private static float GetThemeMinValue(UITheme theme, float fallback)
	     {
		          return theme != null ? Mathf.Max(fallback, theme.ControlHeight * 2f) : fallback;
	     }

	     private static Vector2 ApplyParentConstraints(Vector2 size, UIComponentData component, RectTransform parentRect)
	     {
		          if (component == null || !component.ConstrainToParent || parentRect == null)
				return size;

		          Vector2 parentSize = parentRect.rect.size;
		          if (parentSize.x > 0f)
				size.x = Mathf.Min(size.x, parentSize.x);
		          if (parentSize.y > 0f)
				size.y = Mathf.Min(size.y, parentSize.y);

		          return size;
	     }

	     public static Vector2 ResolveAndApplySize(UIComponentData component, UITheme theme = null, RectTransform parentRect = null)
	     {
		          if (component == null)
				return Vector2.zero;

		          Vector2 resolvedSize = CalculateMinimumSize(component, theme, parentRect);
		          component.MinimumSize = resolvedSize;
		          component.PreferredSize = resolvedSize;
		          return resolvedSize;
	     }

	     public static Vector2 CalculateMinimumSize(UIComponentData component, UITheme theme = null, RectTransform parentRect = null)
	     {
		          if (component == null)
				return Vector2.zero;

		          switch (component)
		          {
		          case UIField field:
		          return GetFieldMinimumSize(field, theme, 25, parentRect);
		          case UISection section:
		          return GetSectionMinimumSize(section, theme, parentRect);
		          case UITab tab:
		          return GetTabMinimumSize(tab, theme, parentRect);
		          case UIWindow window:
		          return GetWindowMinimumSize(window, theme, 15, parentRect);
		          default:
		          return ApplyParentConstraints(new Vector2(GetThemeMinValue(theme, 20f), GetThemeMinValue(theme, 20f)), component, parentRect);
		          }
	     }

	     // Compute minimum size for any field (label + control)
	     public static Vector2 GetFieldMinimumSize(UIField field, UITheme theme = null, int defaultTextLength = 6, RectTransform parentRect = null)
	     {
		          float labelFontSize = theme != null ? Mathf.Max(theme.FontSize, field.FontSize) : field.FontSize;
		          Vector2 labelSize = GetLabelSize(field.LabelText, Mathf.RoundToInt(labelFontSize));
		          float controlWidth = 0f;
		          float controlHeight = Mathf.Max(field.FontSize + 4f, theme != null ? theme.ControlHeight : 24f);
		          float minControlHeight = theme != null ? Mathf.Max(theme.ControlHeight, theme.FieldHeight) : Mathf.Max(field.FontSize + 4f, 24f);
		          float gap = theme != null ? theme.SmallSpacing : 10f;

		          switch (field)
		          {
		          case TextInputField _:
		          case ReadOnlyField _:
		          controlWidth = theme != null ? theme.MinTextFieldWidth : Mathf.Max(field.FontSize * defaultTextLength, 80f);
		          break;
		          case RangeField _:
		          controlWidth = theme != null ? theme.MinRangeWidth : 80f;
		          break;
		          case ToggleField _:
		          controlWidth = theme != null ? theme.MinToggleWidth : 30f;
		          break;
		          case DropdownField _:
		          controlWidth = theme != null ? theme.MinDropdownWidth : 100f;
		          break;
		          case ButtonField btn:
		          controlWidth = Mathf.Max(GetLabelSize(btn.LabelText, btn.FontSize).x + (theme != null ? theme.SmallPadding : 20f), theme != null ? theme.MinButtonWidth : 60f);
		          controlHeight = Mathf.Max(GetLabelSize(btn.LabelText, btn.FontSize).y + (theme != null ? theme.SmallPadding : 8f), minControlHeight);
		          break;
		          case SpriteField sf:
		          if (sf.Image != null)
				return ApplyParentConstraints(new Vector2(sf.Image.rect.width, sf.Image.rect.height), field, parentRect);
		          controlWidth = theme != null ? theme.MinImageSize : 80f;
		          controlHeight = theme != null ? theme.MinImageSize : 80f;
		          break;
		          case GraphField gf:
		          float graphSize = Mathf.Max(gf.Size, theme != null ? theme.MinGraphSize : 80f);
		          return ApplyParentConstraints(new Vector2(graphSize, graphSize), field, parentRect);
		          default:
		          controlWidth = theme != null ? theme.MinTextFieldWidth : 60f;
		          break;
		          }

		          float totalWidth = Mathf.Max(labelSize.x + controlWidth + gap, controlWidth + gap);
		          float height = Mathf.Max(labelSize.y, controlHeight, minControlHeight);
		          return ApplyParentConstraints(new Vector2(totalWidth, height), field, parentRect);
	     }

	     // ─── Hierarchical Sizing ───────────────────────────────────────────

	     /// <summary>
	     /// Calculate minimum size for a section based on its contained components.
	     /// For vertical layouts: accumulate heights, use widest child.
	     /// For horizontal layouts: accumulate widths, use tallest child.
	     /// </summary>
	     public static Vector2 GetSectionMinimumSize(UISection section, UITheme theme = null, RectTransform parentRect = null)
	     {
		          if (section.Components == null || section.Components.Count == 0)
				return ApplyParentConstraints(new Vector2(theme != null ? theme.DefaultSectionMinWidth : 20f, theme != null ? theme.DefaultSectionMinHeight : 20f), section, parentRect);

		          float totalWidth = 0;
		          float totalHeight = 0;
		          float maxWidth = 0;
		          float maxHeight = 0;

		          foreach (var comp in section.Components)
		          {
				Vector2 compSize;

				// Get size based on component type. although this is wrong, we should probably be using content size fitters.
				if (comp is UIField field)
				{
					     compSize = field.MinimumSize.x > 0f || field.MinimumSize.y > 0f ? field.MinimumSize : GetFieldMinimumSize(field, theme, 6, parentRect);
				}
				else if (comp is UISection subSection)
				{
					     compSize = GetSectionMinimumSize(subSection, theme, parentRect);
				}
				else
				{
					     compSize = comp.MinimumSize.x > 0f || comp.MinimumSize.y > 0f ? comp.MinimumSize : new Vector2(20f, 20f);
				}

				maxWidth = Mathf.Max(maxWidth, compSize.x);
				maxHeight = Mathf.Max(maxHeight, compSize.y);

				if (section.Layout == LayoutType.Vertical)
				{
					     totalHeight += compSize.y;
					     totalWidth = Mathf.Max(totalWidth, compSize.x);
				}
				else
				{
					     totalWidth += compSize.x;
					     totalHeight = Mathf.Max(totalHeight, compSize.y);
				}
		          }

		          // Account for spacing between components
		          int spacingCount = Mathf.Max(0, section.Components.Count - 1);
		          if (section.Layout == LayoutType.Vertical)
				totalHeight += spacingCount * section.Spacing;
		          else
				totalWidth += spacingCount * section.Spacing;

		          // Account for padding
		          totalWidth += section.Padding.left + section.Padding.right;
		          totalHeight += section.Padding.top + section.Padding.bottom;

		          float minWidth = Mathf.Max(totalWidth, section.MinimumSize.x > 0f ? section.MinimumSize.x : 0f);
		          float minHeight = Mathf.Max(totalHeight, section.MinimumSize.y > 0f ? section.MinimumSize.y : 0f);
		          return ApplyParentConstraints(new Vector2(Mathf.Max(minWidth, theme != null ? theme.DefaultSectionMinWidth : 20f), Mathf.Max(minHeight, theme != null ? theme.DefaultSectionMinHeight : 20f)), section, parentRect);
	     }

	     /// <summary>
	     /// Calculate minimum size for a tab based on all its sections.
	     /// Tabs are typically vertical, so accumulate heights and use widest section.
	     /// </summary>
	     public static Vector2 GetTabMinimumSize(UITab tab, UITheme theme = null, RectTransform parentRect = null)
	     {
		          if (tab.Sections == null || tab.Sections.Count == 0)
				return ApplyParentConstraints(new Vector2(theme != null ? theme.DefaultTabMinWidth : 20f, theme != null ? theme.DefaultTabMinHeight : 20f), tab, parentRect);

		          float totalWidth = 0;
		          float totalHeight = 0;
		          float maxWidth = 0;
		          float maxHeight = 0;

		          foreach (var section in tab.Sections)
		          {
				Vector2 sectionSize = GetSectionMinimumSize(section, theme, parentRect);
				maxWidth = Mathf.Max(maxWidth, sectionSize.x);
				maxHeight = Mathf.Max(maxHeight, sectionSize.y);

				if (tab.Layout == LayoutType.Vertical)
				{
					     totalHeight += sectionSize.y;
					     totalWidth = Mathf.Max(totalWidth, sectionSize.x);
				}
				else
				{
					     totalWidth += sectionSize.x;
					     totalHeight = Mathf.Max(totalHeight, sectionSize.y);
				}
		          }

		          // Account for spacing between sections
		          int spacingCount = Mathf.Max(0, tab.Sections.Count - 1);
		          if (tab.Layout == LayoutType.Vertical)
				totalHeight += spacingCount * tab.Spacing;
		          else
				totalWidth += spacingCount * tab.Spacing;

		          // Account for padding
		          totalWidth += tab.Padding.left + tab.Padding.right;
		          totalHeight += tab.Padding.top + tab.Padding.bottom;

		          float minWidth = Mathf.Max(totalWidth, tab.MinimumSize.x > 0f ? tab.MinimumSize.x : 0f);
		          float minHeight = Mathf.Max(totalHeight, tab.MinimumSize.y > 0f ? tab.MinimumSize.y : 0f);
		          return ApplyParentConstraints(new Vector2(Mathf.Max(minWidth, theme != null ? theme.DefaultTabMinWidth : 20f), Mathf.Max(minHeight, theme != null ? theme.DefaultTabMinHeight : 20f)), tab, parentRect);
	     }

	     /// <summary>
	     /// Calculate minimum size for a window based on all its tabs.
	     /// Window takes the size of the largest tab plus space for tab buttons.
	     /// </summary>
	     public static Vector2 GetWindowMinimumSize(UIWindow window, UITheme theme = null, int buttonStripHeight = 25, RectTransform parentRect = null)
	     {
		          if (window.Tabs == null || window.Tabs.Count == 0)
				return ApplyParentConstraints(new Vector2(Mathf.Max(100f, theme != null ? theme.DefaultWindowMinWidth : 100f), Mathf.Max(100f, theme != null ? theme.DefaultWindowMinHeight : 100f)), window, parentRect);

		          float maxTabWidth = 0;
		          float maxTabHeight = 0;

		          foreach (var tab in window.Tabs)
		          {
				Vector2 tabSize = GetTabMinimumSize(tab, theme, parentRect);
				maxTabWidth = Mathf.Max(maxTabWidth, tabSize.x);
				maxTabHeight = Mathf.Max(maxTabHeight, tabSize.y);
		          }

		          // Add space for tab buttons if more than one tab
		          float totalWidth = maxTabWidth;
		          float totalHeight = maxTabHeight;

		          if (window.Tabs.Count > 1)
		          {
				// Button strip adds to width or height depending on window layout
				if (window.Layout == LayoutType.Vertical)
				{
					     // Horizontal button strip above tabs
					     totalHeight += buttonStripHeight + window.Spacing;
				}
				else
				{
					     // Vertical button strip to the side
					     totalWidth += buttonStripHeight + window.Spacing;
				}
		          }

		          // Account for window padding
		          totalWidth += window.Padding.left + window.Padding.right;
		          totalHeight += window.Padding.top + window.Padding.bottom;

		          float minWidth = Mathf.Max(totalWidth, window.MinimumSize.x > 0f ? window.MinimumSize.x : 0f);
		          float minHeight = Mathf.Max(totalHeight, window.MinimumSize.y > 0f ? window.MinimumSize.y : 0f);
		          return new Vector2(Mathf.Max(minWidth, theme != null ? theme.ControlHeight * 4f : 100f), Mathf.Max(minHeight, theme != null ? theme.ControlHeight * 4f : 100f));
	     }
	     public static void RefreshAllFields(UIComponentData root)
	     {
		          if (root == null)
				return;

		          // If it's a field, refresh it
		          if (root is UIField field)
		          {
				field.Refresh();
				return;
		          }

		          // Otherwise, traverse children
		          switch (root)
		          {
		          case UIWindow window:
		          foreach (var tab in window.Tabs)
				RefreshAllFields(tab);
		          break;

		          case UITab tab:
		          foreach (var section in tab.Sections)
				RefreshAllFields(section);
		          break;

		          case UISection section:
		          foreach (var comp in section.Components)
				RefreshAllFields(comp);
		          break;

		          }
	     }
	     //we need a method that applies the theme to the ui component
	     public static void ApplyUITheme(UITheme theme, UIComponentData sc)
	     {
	     }
	     public static void ApplyLayoutGroupData(GameObject go, UIComponentData sc)
	     {
		          if (go == null || sc == null)
				return;

		          //if sc says it's a vertical apply ddata, else add a horizontal and apply the data.
		          if (sc.Layout == LayoutType.Vertical)
		          {
				// Add a vertical layout group to stack tabs
				VerticalLayoutGroup layout = go.GetComponent<VerticalLayoutGroup>();
				if (layout == null)
					     layout = go.AddComponent<VerticalLayoutGroup>();

				layout.padding = sc.Padding;
				layout.spacing = sc.Spacing;

				layout.childAlignment = sc.ChildAlignment;
				layout.reverseArrangement = sc.ReverseArrangement;

				layout.childControlWidth = sc.ControlChildWidth;
				layout.childControlHeight = sc.ControlChildHeight;
				layout.childScaleWidth = sc.UseChildScaleWidth;
				layout.childScaleHeight = sc.UseChildScaleHeight;
				layout.childForceExpandWidth = sc.ChildForceExpandWidth;
				layout.childForceExpandHeight = sc.ChildForceExpandHeight;
		          }
		          else if (sc.Layout == LayoutType.Horizontal)
		          {
				HorizontalLayoutGroup layout = go.GetComponent<HorizontalLayoutGroup>();
				if (layout == null)
					     layout = go.AddComponent<HorizontalLayoutGroup>();

				layout.padding = sc.Padding;
				layout.spacing = sc.Spacing;

				layout.childAlignment = sc.ChildAlignment;
				layout.reverseArrangement = sc.ReverseArrangement;

				layout.childControlWidth = sc.ControlChildWidth;
				layout.childControlHeight = sc.ControlChildHeight;
				layout.childScaleWidth = sc.UseChildScaleWidth;
				layout.childScaleHeight = sc.UseChildScaleHeight;
				layout.childForceExpandWidth = sc.ChildForceExpandWidth;
				layout.childForceExpandHeight = sc.ChildForceExpandHeight;
		          }
		          else
		          {
				//grid
				GridLayoutGroup layout = go.GetComponent<GridLayoutGroup>();
				if (layout == null)
					     layout = go.AddComponent<GridLayoutGroup>();

				layout.padding = sc.Padding;
				layout.spacing = new Vector2(sc.Spacing, sc.Spacing);
				layout.childAlignment = sc.ChildAlignment;
				layout.startAxis = GridLayoutGroup.Axis.Horizontal;
				layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
				layout.cellSize = new Vector2(120f, 50f); // Default cell size, can be adjusted based on theme or component data
		          }

		          ApplyLayoutElementData(go, sc);
	     }

	     public static void ApplyLayoutElementData(GameObject go, UIComponentData cd)
	     {
		          if (go == null || cd == null)
				return;

		          LayoutElement le = go.GetComponent<LayoutElement>();
		          if (le == null)
				le = go.AddComponent<LayoutElement>();

		          float minWidth = cd.MinimumSize.x > 0f ? cd.MinimumSize.x : 20f;
		          float minHeight = cd.MinimumSize.y > 0f ? cd.MinimumSize.y : 20f;
		          float preferredWidth = cd.PreferredSize.x > 0f ? cd.PreferredSize.x : minWidth;
		          float preferredHeight = cd.PreferredSize.y > 0f ? cd.PreferredSize.y : minHeight;

		          //   le.minWidth = minWidth;
		          le.minHeight = minHeight;
		          //  le.preferredWidth = preferredWidth;
		          le.preferredHeight = preferredHeight;
		          // le.flexibleWidth = 0f;
		          //  le.flexibleHeight = 0f;

	     }

	     public static Graph GraphFloats(List<float> values, float max, GraphType type = GraphType.radar, float radius = 1f)
	     {
		          if (values == null || values.Count == 0)
		          {
				Debug.LogWarning("Cannot graph empty values list");
				return new Graph
				{
					     points = new Vector3[0],
					     type = type,
					     pointColors = new Color[0],
					     axisLabels = new string[0],
					     axisColors = new Color[0]
				};
		          }

		          switch (type)
		          {
		          case GraphType.radar:
		          return CreateRadarGraph(values, max, radius);
		          case GraphType.bar:
		          return CreateBarGraph(values, max);
		          case GraphType.line:
		          return CreateLineGraph(values, max);
		          case GraphType.pie:
		          return CreatePieChart(values, max, radius);
		          default:
		          return CreateRadarGraph(values, max, radius);
		          }
	     }

	     private static Graph CreateRadarGraph(List<float> values, float max, float radius)
	     {
		          int pointCount = values.Count;
		          Graph graph = new Graph(pointCount + 1); // +1 for closing point

		          float angleIncrement = 360f / pointCount * Mathf.Deg2Rad;

		          for (int i = 0; i < pointCount; i++)
		          {
				float normalisedValue = Mathf.Clamp01(values[i] / max);
				float angle = i * angleIncrement;
				float x = Mathf.Cos(angle) * normalisedValue * radius;
				float y = Mathf.Sin(angle) * normalisedValue * radius;
				graph.points[i] = new Vector3(x, y, 0);
				graph.pointColors[i] = Color.white; // Default color
		          }

		          // Close the shape by connecting back to the first point
		          graph.points[pointCount] = graph.points[0];
		          graph.pointColors[pointCount] = graph.pointColors[0];

		          graph.type = GraphType.radar;
		          return graph;
	     }

	     private static Graph CreateBarGraph(List<float> values, float max)
	     {
		          int barCount = values.Count;
		          Graph graph = new Graph(barCount * 4);

		          float barWidth = 1f / barCount;

		          for (int i = 0; i < barCount; i++)
		          {
				float normalisedValue = Mathf.Clamp01(values[i] / max);
				float xPos = i * barWidth;

				// Define bar vertices (quad)
				graph.points[i * 4] = new Vector3(xPos, 0, 0); // bottom-left
				graph.points[i * 4 + 1] = new Vector3(xPos + barWidth, 0, 0); // bottom-right
				graph.points[i * 4 + 2] = new Vector3(xPos + barWidth, normalisedValue, 0); // top-right
				graph.points[i * 4 + 3] = new Vector3(xPos, normalisedValue, 0); // top-left

				// Set colors for this bar
				Color barColor = graph.axisColors[i];
				for (int j = 0; j < 4; j++)
				{
					     graph.pointColors[i * 4 + j] = barColor;
				}
		          }

		          graph.type = GraphType.bar;
		          return graph;
	     }

	     private static Graph CreateLineGraph(List<float> values, float max)
	     {
		          Graph graph = new Graph(values.Count);

		          float xStep = 1f / (values.Count - 1);

		          for (int i = 0; i < values.Count; i++)
		          {
				float normalisedValue = Mathf.Clamp01(values[i] / max);
				graph.points[i] = new Vector3(i * xStep, normalisedValue, 0);
				graph.pointColors[i] = Color.Lerp(Color.blue, Color.red, normalisedValue);
		          }

		          graph.type = GraphType.line;
		          return graph;
	     }

	     private static Graph CreatePieChart(List<float> values, float max, float radius)
	     {
		          // Calculate total points needed
		          int totalSegments = 0;
		          float total = 0f;
		          foreach (float value in values)
		          {
				total += value;
				float percentage = value / total;
				int segments = Mathf.Max(3, Mathf.RoundToInt(percentage * 12f));
				totalSegments += segments + 2; // +2 for center and closing point
		          }

		          List<Vector3> piePoints = new List<Vector3>();
		          List<Color> pieColors = new List<Color>();

		          float currentAngle = 0f;

		          for (int wedgeIndex = 0; wedgeIndex < values.Count; wedgeIndex++)
		          {
				float value = values[wedgeIndex];
				float percentage = value / total;
				float wedgeAngle = percentage * 360f * Mathf.Deg2Rad;
				int segments = Mathf.Max(3, Mathf.RoundToInt(percentage * 12f));

				Color wedgeColor = Color.HSVToRGB(wedgeIndex / (float)values.Count, 0.8f, 0.8f);

				// Add center point
				piePoints.Add(Vector3.zero);
				pieColors.Add(wedgeColor);

				// Add points around the arc
				for (int i = 0; i <= segments; i++)
				{
					     float angle = currentAngle + (wedgeAngle * i / segments);
					     float x = Mathf.Cos(angle) * radius;
					     float y = Mathf.Sin(angle) * radius;
					     piePoints.Add(new Vector3(x, y, 0));
					     pieColors.Add(wedgeColor);
				}

				currentAngle += wedgeAngle;
		          }

		          Graph graph = new Graph(piePoints.Count);
		          graph.points = piePoints.ToArray();
		          graph.pointColors = pieColors.ToArray();
		          graph.type = GraphType.pie;
		          return graph;
	     }
	     public static Graph CreateSoilGraph(SoilComposition soil, GraphType type = GraphType.bar)
	     {
		          List<float> soilValues = new List<float>
					     {
						          soil.sand.Mass,
						         soil.Mass,
						          soil.clay.Mass,
						          soil.organicMaterial.Mass
					     };

		          float maxSoilValue = 0;
		          foreach (var value in soilValues)
		          {
				if (value > maxSoilValue)
				{
					     maxSoilValue = value;
				}
		          }
		          Graph graph = UIUtilities.GraphFloats(soilValues, Mathf.Max(maxSoilValue, 1f), GraphType.bar);
		          graph.axisLabels = new string[] { "Sand", "Silt", "Clay", "Organic" };
		          graph.axisColors = new Color[]
		          {
						          Constants.SAND_COLOR, Constants.SILT_COLOR, Constants.CLAY_COLOR, Constants.ORGANIC_COLOR
		          };
		          for (int i = 0; i < graph.axisLabels.Length; i++)
		          {
				graph.pointColors[i] = graph.axisColors[i];
		          }
		          return graph;
	     }
	     public static Graph CreateWaterGraph(TileData tile, GraphType type = GraphType.bar)
	     {
		          List<float> waterValues = new List<float> { tile.surfaceLiquid.Mass, tile.surfaceIce.Mass };
		          Graph graph = UIUtilities.GraphFloats(waterValues, Mathf.Max(tile.surfaceLiquid.Mass * 1.2f, 100f), GraphType.bar);
		          graph.axisLabels = new string[] { "Surface Water", "Surface Ice", "Soil Water", "Soil Ice" };
		          graph.axisColors = new Color[] {
						          Constants.SHALLOW_WATER_COLOR, Constants.ICE_COLOR, Constants.DEEP_WATER_COLOR, Constants.ICE_COLOR,
					     };
		          for (int i = 0; i < graph.axisLabels.Length; i++)
		          {
				graph.pointColors[i] = graph.axisColors[i];
		          }
		          return graph;
	     }
	     public static Graph CreateAtmosphereGraph(AtmosphereTile tile, GraphType type = GraphType.bar)
	     {
		          if (tile == null)
				return new Graph { points = new Vector3[0], pointColors = new Color[0], axisLabels = new string[0], axisColors = new Color[0] };

		          List<float> atmValues = new List<float>
					     {
						          tile.nitrogen.Mass,
						          tile.oxygen.Mass,
						          tile.carbonDioxide.Mass,
						          tile.waterVapor.Mass,
						          tile.cloudWaterContent.Mass
					     };
		          float maxAtmValue = 0;
		          foreach (var value in atmValues)
		          {
				if (value > maxAtmValue)
				{
					     maxAtmValue = value;
				}
		          }
		          Graph graph = UIUtilities.GraphFloats(atmValues, Mathf.Max(maxAtmValue, 1f), GraphType.pie);
		          graph.axisLabels = new string[] { "N2", "O2", "CO2", "H2O", "Cloud" };
		          graph.axisColors = new Color[]
		          {
						          new Color(0.7f, 0.8f, 0.9f),    // Nitrogen - light blue
						          new Color(0.5f, 0.7f, 1f),      // Oxygen - sky blue
						          new Color(0.4f, 0.4f, 0.4f),    // CO2 - gray
						          new Color(0.8f, 0.9f, 1f),      // Water vapor - pale blue
						          new Color(1f, 1f, 1f)           // Cloud - white
		          };
		          for (int i = 0; i < graph.axisLabels.Length; i++)
		          {
				graph.pointColors[i] = graph.axisColors[i];
		          }
		          return graph;
	     }
	     // Helper method to create HEXACO radar graph
	     public static Graph CreateHexacoGraph(HEXACO hexaco, GraphType type = GraphType.radar)
	     {
		          List<float> values = new List<float>();
		          PersonalityAspect[] aspects = hexaco.FullHexRange();

		          foreach (PersonalityAspect aspect in aspects)
		          {
				values.Add(aspect.value);
		          }

		          Graph graph = GraphFloats(values, hexaco.maximumValue, type);
		          graph.title = string.Join(", ", PepoUtilitiesPersonality.GetPersonalityName(hexaco).Take(3));
		          // Set HEXACO-specific colors
		          graph.axisLabels = new string[] { "H", "E", "X", "A", "C", "O" };
		          graph.axisColors = new Color[]
		          {
            Color.magenta,    // Honesty-Humility should be green
            Color.red,        // Emotionality is yellow
            Color.yellow,     // eXtraversion is peachy orange
            Color.green,      // Agreeableness is salmon pink
            Color.cyan,       // Conscientiousness is purple
            Color.blue        // Openness
		          };

		          // Set point colors based on values and axis colors
		          for (int i = 0; i < values.Count && i < graph.pointColors.Length; i++)
		          {
				float saturation = Mathf.Clamp01(values[i] / hexaco.maximumValue);
				graph.pointColors[i] = SaturateColor(graph.axisColors[i], saturation);
		          }

		          return graph;
	     }

	     // Helper method to create Plutchik mood graph
	     public static Graph CreateMoodGraph(Mood mood, GraphType type = GraphType.radar)
	     {
		          List<float> values = new List<float>();
		          EmotionAspect[] aspects = mood.FullHexRange();

		          //at the moment we're adding them in acrazy order, need to fix that later
		          //        foreach (EmotionAspect aspect in aspects)
		          //        {
		          //values.Add(aspect.value);
		          //        }
		          values.Add(mood.Fear.value);
		          values.Add(mood.Trust.value);
		          values.Add(mood.Joy.value);
		          values.Add(mood.Anticipation.value);
		          values.Add(mood.Anger.value);
		          values.Add(mood.Disgust.value);
		          values.Add(mood.Sadness.value);
		          values.Add(mood.Surprise.value);
		          //fear, trust. joy, anticipation, anger, disgust, sadness, surprise
		          Graph graph = GraphFloats(values, 10f, type);
		          //the label is the highest one by the degree.
		          graph.title = PepoUtilitiesPersonality.GetFeelingName(mood);
		          // Set mood-specific colors (Plutchik's wheel colors)
		          graph.axisLabels = new string[] { "fear", "", "joy", "", "anger", "", "woe", "" };
		          graph.axisColors = new Color[]
		          {
		          new Color(0,1,0.25f),       // Fear
		          new Color(0.25f,1,0),      // Trust
		          Color.yellow,     // Joy
		          new Color(1,0.25f,0),      // Anticipation
		          Color.red,        // Anger
		          new Color(0.5f,0,1),    // Disgust purple
		          Color.blue,       // Sadness blue 
		          new Color(0,0.75f,1),      // Surprise teal        
            
		          };

		          // Set point colors based on values
		          for (int i = 0; i < values.Count && i < graph.pointColors.Length; i++)
		          {
				float saturation = Mathf.Clamp01(values[i] / 10f);
				graph.pointColors[i] = SaturateColor(graph.axisColors[i], saturation);
		          }

		          return graph;
	     }

	     // Helper method to create Values radar graph
	     public static Graph CreateValuesGraph(List<Value> valuesList, GraphType type = GraphType.radar)
	     {
		          if (valuesList == null || valuesList.Count == 0)
		          {
				Debug.LogWarning("Cannot create values graph with empty values list");
				return new Graph
				{
					     points = new Vector3[0],
					     type = type,
					     pointColors = new Color[0],
					     axisLabels = new string[0],
					     axisColors = new Color[0]
				};
		          }

		          List<float> values = new List<float>();
		          foreach (Value value in valuesList)
		          {
				values.Add(value.value);
		          }

		          Graph graph = GraphFloats(values, 7f, type); // max value is 7 based on your basicHumanValues

		          // Set Values-specific colors by category
		          graph.axisLabels = new string[valuesList.Count];
		          graph.axisColors = new Color[valuesList.Count];

		          for (int i = 0; i < valuesList.Count; i++)
		          {
				Value currentValue = valuesList[i];
				graph.axisLabels[i] = GetValueAbbreviation(currentValue.name);
				graph.axisColors[i] = GetValueColor(currentValue.name);
		          }

		          // Set point colors based on values and axis colors
		          for (int i = 0; i < values.Count && i < graph.pointColors.Length; i++)
		          {
				float saturation = Mathf.Clamp01(values[i] / 7f); // Normalize to 0-1 based on max value of 7
				graph.pointColors[i] = SaturateColor(graph.axisColors[i], saturation);
		          }

		          return graph;
	     }

	     private static string GetValueAbbreviation(string valueName)
	     {
		          switch (valueName.ToLower())
		          {
		          case "curiosity":
		          return "Cu";
		          case "autonomy":
		          return "Au";
		          case "excitement":
		          return "Ex";
		          case "vitality":
		          return "Vi";
		          case "indulgence":
		          return "In";
		          case "accomplishment":
		          return "Am";
		          case "dominance":
		          return "Do";
		          case "resources":
		          return "Ac";
		          case "status":
		          return "St";
		          case "safety":
		          return "Sf";
		          case "security":
		          return "Se";
		          case "tradition":
		          return "Tr";
		          case "obedience":
		          return "Ob";
		          case "cohesion":
		          return "Cs";
		          case "humility":
		          return "Hm";
		          case "preservation":
		          return "Pr";
		          case "compassion":
		          return "Cm";
		          case "tolerance":
		          return "To";
		          case "caring":
		          return "Cg";
		          case "dependability":
		          return "Dp";
		          default:
		          return valueName.Substring(0, 2);
		          }
	     }




	     private static Color GetValueColor(string valueName)
	     {
		          switch (valueName.ToLower())
		          {
		          // Openness to change - Blues
		          case "curiosity":
		          return new Color(0.2f, 0.6f, 1f); // Bright blue
		          case "autonomy":
		          return new Color(0.4f, 0.8f, 1f); // Light blue
		          case "excitement":
		          return new Color(1f, 0.4f, 0.8f); // Magenta
		          case "vitality":
		          return new Color(0.2f, 0.8f, 0.8f); // Teal

		          // Self-enhancement - Reds/Oranges
		          case "indulgence":
		          return new Color(0.7f, 0.4f, 0.9f); // Violet
		          case "accomplishment":
		          return new Color(1f, 0.2f, 0.2f); // Red
		          case "dominance":
		          return new Color(0.0f, 0f, 0f); // Black
		          case "resources":
		          return new Color(1f, 0.8f, 0f); // Gold
		          case "status":
		          return new Color(0.8f, 0.4f, 1f); // Purple

		          // Conservation - Greens
		          case "safety":
		          return new Color(0.6f, 1f, 0.6f); // Light green
		          case "security":
		          return new Color(1f, 0.6f, 0.2f); // Orange
		          case "tradition":
		          return new Color(1f, 0.6f, 0.8f); // Rose
		          case "obedience":
		          return new Color(0.8f, 1f, 0.4f); // Lime
		          case "cohesion":
		          return new Color(0.6f, 0.8f, 0.2f); // Yellow-green
		          case "humility":
		          return new Color(0.5f, 0.5f, 0.2f); // Olive

		          // Self-transcendence - Purples
		          case "preservation":
		          return new Color(0.2f, 0.6f, 0.2f); // Dark green
		          case "compassion":
		          return new Color(1f, 0.4f, 0.6f); // Pink
		          case "tolerance":
		          return new Color(0.9f, 0.6f, 1f); // Lavender
		          case "caring":
		          return new Color(0.4f, 0.8f, 0.4f); // Green
		          case "dependability":
		          return new Color(0.6f, 1f, 1f); // Cyan

		          default:
		          return Color.gray;
		          }
	     }

	     private static Color SaturateColor(Color baseColor, float saturation)
	     {
		          Color.RGBToHSV(baseColor, out float h, out float s, out float v);
		          return Color.HSVToRGB(h, saturation, v);
	     }
	     public static Color[] GetPepoColors(BodyData body)
	     {
		          if (body == null)
				return new Color[4];

		          var hairAspects = new List<KeyValuePair<string, NumericalAspect>>();
		          var skinAspects = new List<KeyValuePair<string, NumericalAspect>>();
		          var eyeAspects = new List<KeyValuePair<string, NumericalAspect>>();

		          foreach (var kvp in body.physicalAttributes)
		          {
				if (kvp.Key.StartsWith("Hair"))
					     hairAspects.Add(kvp);
				else if (kvp.Key.StartsWith("Skin"))
					     skinAspects.Add(kvp);
				else if (kvp.Key.StartsWith("Eye"))
					     eyeAspects.Add(kvp);
		          }

		          Color hair = GetBodyColour(hairAspects, Keratin);
		          Color eye = GetBodyColour(eyeAspects, Sclera);
		          Color skin = GetBodyColour(skinAspects, PaleSkin);
		          Color mouth = GetMouthColor(skin);

		          return new[] { hair, eye, skin, mouth };
	     }

	     private static Color GetMouthColor(Color skinColor)
	     {
		          Color.RGBToHSV(skinColor, out float h, out float s, out float v);
		          h = Mathf.Repeat(h - 0.04f, 1f);                 // nudge toward red
		          s = Mathf.Clamp(s + 0.15f, 0.25f, 0.60f);
		          v = Mathf.Clamp(v * 0.85f, 0.35f, 0.80f);
		          return Color.HSVToRGB(h, s, v);
	     }
	     public static Color GetBodyColour(
	         IEnumerable<KeyValuePair<string, NumericalAspect>> aspects,
	         Color baseColour)
	     {
		          var list = aspects as IList<KeyValuePair<string, NumericalAspect>>
		              ?? aspects.ToList();

		          // The presence of EyeIrisStructure marks this as the eye set.
		          //        if (list.Any(kvp => kvp.Key == "EyeIrisStructure"))
		          //return GetEyeColour(list, baseColour);

		          return GetGenericBodyColour(list, baseColour);
	     }

	     // Hair and skin: unchanged in spirit, but pigment overlays now route
	     // through WorldColourData.Apply so Screen / Add / Overlay actually work
	     // (the old BlendColour silently fell back to Lerp for anything that
	     // wasn't Multiply).
	     private static Color GetGenericBodyColour(
	         IList<KeyValuePair<string, NumericalAspect>> list,
	         Color baseColour)
	     {
		          int? eyeDensity = null;
		          foreach (var kvp in list)
				if (kvp.Key == "EyePigmentDensity")
				{
					     eyeDensity = (int)kvp.Value.value;
					     break;
				}

		          Color result = baseColour;

		          // Pass 1 — structural (full replace at a fixed sample point).
		          foreach (var kvp in list)
		          {
				var wcd = ResolveStructuralColour(kvp.Key, (int)kvp.Value.value);
				if (wcd == null)
					     continue;
				Color structural = SampleColour(wcd.Colours[0], 0.5f);
				result = BlendColour(result, structural, wcd.Blend, 1f);
		          }

		          // Pass 2 — biological pigments.
		          foreach (var kvp in list)
		          {
				var res = ResolvePigment(kvp.Key, (int)kvp.Value.value, eyeDensity);
				if (res == null)
					     continue;
				var (wcd, weight) = res.Value;
				if (weight <= 0f)
					     continue;
				result = wcd.Apply(result, weight);
		          }

		          return result;
	     }

	     //  // Eyes: dedicated pipeline.
	     //  private static Color GetEyeColour(
	     //      IList<KeyValuePair<string, NumericalAspect>> list,
	     //      Color baseColour)
	     //  {
	     //        Color iris = baseColour;

	     //        float? structure = FindAspect(list, "EyeIrisStructure");
	     //        float? saturation = FindAspect(list, "EyeIrisSaturation");
	     //        float? pigmentType = FindAspect(list, "EyePigmentType");
	     //        float? pigmentDensity = FindAspect(list, "EyePigmentDensity");

	     //        // Pass 1 — structural base. Sample at a per-pepo t so two pepos with
	     //        // the same iris structure differ, then modulate S/V by the saturation
	     //        // allele so "pale" vs "intense" is visible.
	     //        if (structure.HasValue)
	     //        {
	     //var wcd = ResolveStructuralColour("EyeIrisStructure", (int)structure.Value);
	     //if (wcd != null)
	     //{
	     //	     float t = Mathf.Repeat(
	     //	         structure.Value * 0.37f + (saturation ?? 0f) * 0.63f, 1f);

	     //	     Color structural = SampleColour(wcd.Colours[0], t);

	     //	     if (saturation.HasValue)
	     //	     {
	     //		          int n = GeneLibrary.Descriptions["EyeIrisSaturation"].Count;
	     //		          float s01 = (saturation.Value - 1f) / (n - 1f);
	     //		          Color.RGBToHSV(structural, out float h, out float s, out float v);
	     //		          structural = Color.HSVToRGB(
	     //		              h,
	     //		              Mathf.Clamp01(s * Mathf.Lerp(0.4f, 1.3f, s01)),
	     //		              Mathf.Clamp01(v * Mathf.Lerp(0.75f, 1.10f, s01)));
	     //	     }

	     //	     iris = BlendColour(iris, structural, wcd.Blend, 1f);
	     //}
	     //        }

	     //        // Pass 2 — pigment overlay. Density 1 = no pigment, 7 = opaque.
	     //        if (pigmentType.HasValue && pigmentDensity.HasValue)
	     //        {
	     //float weight = Mathf.Clamp01((pigmentDensity.Value - 1f) / 6f);
	     //if (weight > 0f)
	     //{
	     //	     var names = GeneticsLibrary.BiologicalPigmentNames();
	     //	     int idx = (int)pigmentType.Value - 1;
	     //	     if (idx >= 0 && idx < names.Length &&
	     //	         ColourLibrary.Raw.TryGetValue(names[idx], out var wcd))
	     //	     {
	     //		          iris = wcd.Apply(iris, weight);
	     //	     }
	     //}
	     //        }

	     //        return iris;
	     //  }

	     private static float? FindAspect(
	         IList<KeyValuePair<string, NumericalAspect>> list, string key)
	     {
		          foreach (var kvp in list)
				if (kvp.Key == key)
					     return kvp.Value.value;
		          return null;
	     }
	     //private static Color GetSkinColor(int hue, int saturation, int depth)
	     //{
	     //      // Convert trait values (1-6) to HSV ranges
	     //      float h = MapRange(hue, 1, GeneLibrary.Descriptions["SkinHue"].Count, 25f / 360, 40f / 360); // Full hue spectrum
	     //      float s = MapRange(saturation, 1, GeneLibrary.Descriptions["SkinSaturation"].Count, 0.25f, 0.6f); // Moderate to high saturation
	     //      float v = MapRange(depth, 1, GeneLibrary.Descriptions["SkinDepth"].Count, 0.95f, 0.25f); // Dark to light

	     //      Color skinColor = Color.HSVToRGB(h, s, v);
	     //      return skinColor;
	     //}

	     //private static Color GetEyeColor(int color, int darkness)
	     //{
	     //      // Map eye color to specific hue ranges
	     //      float h = 0f;
	     //      float sMin = 0.35f, sMax = 0.8f;
	     //      float vMin = 0.4f, vMax = 0.95f;
	     //      switch (color)
	     //      {
	     //      case 1: // Violet
	     //      h = 275f / 360f;
	     //      sMin = 0.25f;
	     //      sMax = 0.60f;
	     //      vMin = 0.40f;
	     //      vMax = 0.55f;
	     //      break;
	     //      case 2: // Blue
	     //      h = 215f / 360f;
	     //      sMin = 0.20f;
	     //      sMax = 0.65f;
	     //      vMin = 0.45f;
	     //      vMax = 0.95f; // keep blues visibly bright
	     //      break;
	     //      case 3: // Blue-Green / teal
	     //      h = 195f / 360f;
	     //      sMin = 0.20f;
	     //      sMax = 0.6f;
	     //      vMin = 0.25f;
	     //      vMax = 0.95f;
	     //      break;
	     //      case 4: // Green
	     //      h = 120f / 360f;
	     //      sMin = 0.35f;
	     //      sMax = 0.7f;
	     //      vMin = 0.45f;
	     //      vMax = 0.65f;
	     //      break;
	     //      case 5: // Hazel (yellow-brown)
	     //      h = 55f / 360f;
	     //      sMin = 0.4f;
	     //      sMax = 0.7f;
	     //      vMin = 0.4f;
	     //      vMax = 0.6f;
	     //      break;
	     //      case 6: // Brown
	     //      h = 25f / 360f;
	     //      sMin = 0.30f;
	     //      sMax = 0.65f;
	     //      vMin = 0.20f;
	     //      vMax = 0.6f;
	     //      break;
	     //      case 7:
	     //      h = 0f; //albino red
	     //      sMin = 0.5f;
	     //      sMax = 0.8f;
	     //      vMin = 0.55f;
	     //      vMax = 0.95f;
	     //      break;
	     //      }

	     //      // darkness is 1..4 (1 = lightest). Map so darker -> lower value
	     //      float s = MapRange(darkness, 1, 4, sMax, sMin);
	     //      float v = MapRange(darkness, 1, 4, vMax, vMin);

	     //      Color eyeColor = Color.HSVToRGB(h, s, v);



	     //      return eyeColor;
	     //}

	     //private static Color GetHairColor(int eumelanin, int pheomelanin)
	     //{
	     //      // Eumelanin controls darkness (black/brown), Pheomelanin controls redness

	     //      float baseHue = MapRange(pheomelanin, 1, GeneLibrary.Descriptions["HairPigmentPheomelanin"].Count, 0.02f, 0.15f); // Slight yellow to orange-red
	     //      float saturation = MapRange(eumelanin, 1, GeneLibrary.Descriptions["HairPigmentEumelanin"].Count, 0.75f, 0.3f); // More pheomelanin = more saturation
	     //      float value = MapRange(eumelanin, 1, GeneLibrary.Descriptions["HairPigmentEumelanin"].Count, 0.1f, 0.8f); // more eumelanin = darker


	     //      Color hairColor = Color.HSVToRGB(baseHue, saturation, value);
	     //      return hairColor;
	     //}

	     //private static Color GetMouthColor(int skinHue, int skinSaturation, int skinDepth)
	     //{
	     //      // Start with skin color but make it slightly more red/pink
	     //      Color skinColor = GetSkinColor(skinHue, skinSaturation, skinDepth);

	     //      // Convert to HSV to adjust
	     //      Color.RGBToHSV(skinColor, out float h, out float s, out float v);

	     //      // Make slightly more red and saturated for lips
	     //      h = Mathf.Clamp(h + 0.873f, 0.9f, 1f);
	     //      s = Mathf.Clamp(s + 0.2f, 0.2f, 0.35f);
	     //      v = Mathf.Clamp(v - 0.1f, 0.35f, .7f);

	     //      return Color.HSVToRGB(h, s, v);
	     //}

	     private static float MapRange(float value, float fromMin, float fromMax, float toMin, float toMax)
	     {
		          return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
	     }

	     // "HairPigmentEumelanin" → WorldColourData("eumelanin"); "SkinPigmentCarotenoid" → ("carotenoid")
	     // "EyePigmentType" + eyeDensity → the selected biological pigment with the sibling's density.
	     private static (WorldColourData wcd, float weight)? ResolvePigment(
	         string key, int value, int? eyeDensity)
	     {
		          const string Marker = "Pigment";

		          // EyePigmentType is special — its allele is the pigment name, its density lives in a sibling.
		          //        if (key == "EyePigment")
		          //        {
		          //if (!eyeDensity.HasValue)
		          //	     return null;
		          //var names = GeneticsLibrary.BiologicalPigmentNames();
		          //int idx = value - 1;
		          //if (idx < 0 || idx >= names.Length)
		          //	     return null;
		          //if (!ColourLibrary.Raw.TryGetValue(names[idx], out var eyeWcd))
		          //	     return null;

		          //// EyePigmentDensity descriptors are 1..7 ("clear".."black")
		          //float eyeWeight = Mathf.Clamp01((eyeDensity.Value - 1) / 6f);
		          //return (eyeWcd, eyeWeight);
		          //        }
		          if (key == "EyePigmentDensity")
				return null;           // consumed by its sibling

		          // "HairPigment*" / "SkinPigment*" — suffix is the pigment name.
		          int mi = key.IndexOf(Marker, System.StringComparison.Ordinal);
		          if (mi < 0)
				return null;
		          string suffix = key.Substring(mi + Marker.Length);    // "Eumelanin" / "Carotenoid" / ...
		          if (suffix.Length == 0)
				return null;

		          string pigmentName = char.ToLowerInvariant(suffix[0]) + suffix.Substring(1);
		          if (!ColourLibrary.Raw.TryGetValue(pigmentName, out var wcd))
				return null;

		          // Every density gene uses the same 1..6 descriptors (no X → saturated X)
		          float weight = Mathf.Clamp01((value - 1) / 5f);
		          return (wcd, weight);
	     }

	     // "EyeIrisStructure" → WorldColourData registered as "blue iris" / "green iris" / etc.
	     private static WorldColourData ResolveStructuralColour(string key, int value)
	     {
		          if (key != "EyeIrisStructure")
				return null;

		          var names = GeneticsLibrary.StructuralColourNames();  // "grey", "violet", "blue", ...
		          int idx = value - 1;
		          if (idx < 0 || idx >= names.Length)
				return null;
		          string allele = names[idx];

		          foreach (var wcd in GeneticsLibrary.StructuralColours)
				if (wcd.Name.Replace(" iris", "") == allele)
					     return wcd;
		          return null;
	     }

	     private static Color SampleColour(ColourData cd, float t)
	     {
		          float h = Mathf.Lerp(cd.hue.x, cd.hue.y, t);
		          float s = Mathf.Lerp(cd.saturation.x, cd.saturation.y, t);
		          float v = Mathf.Lerp(cd.value.x, cd.value.y, t);
		          return Color.HSVToRGB(h, s, v);
	     }

	     private static Color BlendColour(Color baseColour, Color pigmentColour, ColourBlend blend, float weight)
	     {
		          switch (blend)
		          {
		          case ColourBlend.Multiply:
		          var mult = new Color(baseColour.r * pigmentColour.r,
				 baseColour.g * pigmentColour.g,
				 baseColour.b * pigmentColour.b,
				 baseColour.a);
		          return Color.Lerp(baseColour, mult, weight);

		          case ColourBlend.Lerp:
		          default:
		          return Color.Lerp(baseColour, pigmentColour, weight);
		          }
	     }
}

public class Graph
{
	     public Vector3[] points;
	     public GraphType type;
	     public Color[] pointColors; // Color for each point
	     public Color lineColor = Color.white;
	     public float lineWidth = 0.1f;
	     public string title;
	     public string[] axisLabels; // Labels for each axis
	     public Color[] axisColors; // Colors for each axis
	     public bool showAxes = true;
	     public bool showLabels = true;

	     // Default constructor for serialization
	     public Graph()
	     {
	     }

	     public Graph(int pointCount)
	     {
		          points = new Vector3[pointCount];
		          pointColors = new Color[pointCount];
		          axisLabels = new string[pointCount];
		          axisColors = new Color[pointCount];

		          // Initialize with default values
		          for (int i = 0; i < pointCount; i++)
		          {
				pointColors[i] = Color.white;
				axisColors[i] = Color.gray;
		          }
	     }
}

public enum GraphType
{
	     radar, bar, pie, line
}
public static class GraphFactory
{
	     public static GameObject CreateRadarGraph(Graph graph, Transform parent = null, float size = 80f, string name = "RadarGraph")
	     {
		          if (graph?.points == null || graph.points.Length < 3)
				return null;

		          GameObject graphGO = new GameObject(name);
		          RectTransform rt = graphGO.AddComponent<RectTransform>();
		          rt.sizeDelta = new Vector2(size, size);

		          // Draw the radar graph
		          DrawRadarGraph(graphGO, graph, size * 0.55f); // Scale to fit

		          if (parent != null)
				graphGO.transform.SetParent(parent, false);

		          return graphGO;
	     }
	     public static GameObject CreateBarGraph(Graph graph, Transform parent, float size, string name)
	     {
		          GameObject graphGO = new GameObject(name);
		          RectTransform rt = graphGO.AddComponent<RectTransform>();
		          rt.sizeDelta = new Vector2(size, size);
		          rt.anchorMin = new Vector2(0.5f, 0.5f);
		          rt.anchorMax = new Vector2(0.5f, 0.5f);
		          rt.pivot = new Vector2(0.5f, 0.5f);

		          int barCount = graph.points.Length / 4;
		          if (barCount == 0)
				return graphGO;

		          // Find max height for scaling
		          float maxHeight = 0;
		          for (int i = 0; i < graph.points.Length; i++)
				if (graph.points[i].y > maxHeight)
					     maxHeight = graph.points[i].y;
		          if (maxHeight == 0)
				maxHeight = 1;

		          // Draw each bar as an Image
		          for (int i = 0; i < barCount; i++)
		          {
				Vector3 bl = graph.points[i * 4];
				Vector3 br = graph.points[i * 4 + 1];
				Vector3 tr = graph.points[i * 4 + 2];
				// Calculate width and height in normalized space
				float width = Mathf.Abs(br.x - bl.x);
				float height = Mathf.Abs(tr.y - br.y);
				float centerX = (bl.x + br.x) / 2f;
				float centerY = (bl.y + tr.y) / 2f;

				// Map to pixel coordinates (assume graph center is (0,0), range -0.5..0.5)
				float xPos = (centerX - 0.5f) * size;
				float yPos = (centerY - 0.5f) * size;
				float barWidth = width * size;
				float barHeight = height * size;

				GameObject barGO = new GameObject($"Bar_{i}");
				barGO.transform.SetParent(graphGO.transform, false);
				RectTransform barRT = barGO.AddComponent<RectTransform>();
				barRT.anchorMin = new Vector2(0.5f, 0.5f);
				barRT.anchorMax = new Vector2(0.5f, 0.5f);
				barRT.pivot = new Vector2(0.5f, 0.5f);
				barRT.sizeDelta = new Vector2(barWidth, barHeight);
				barRT.anchoredPosition = new Vector2(xPos, yPos);

				Image barImage = barGO.AddComponent<Image>();
				Color barColor = (graph.pointColors != null && i < graph.pointColors.Length)
				    ? graph.pointColors[i * 4]
				    : Color.white;
				barImage.color = barColor;
		          }

		          if (parent != null)
				graphGO.transform.SetParent(parent, false);

		          return graphGO;
	     }
	     private static void DrawRadarGraph(GameObject parent, Graph graph, float scale)
	     {
		          // Create background
		          CreateBackground(parent, new Color(0.1f, 0.1f, 0.2f, 0.3f));
		          //put graph title here
		          if (!string.IsNullOrEmpty(graph.title))
		          {
				CreateGraphTitle(parent, graph.title);
		          }
		          // Create axes if requested
		          if (graph.showAxes && graph.axisColors != null && graph.axisLabels != null)
		          {
				CreateRadarAxes(parent, graph, scale * 0.9f);
		          }

		          // Create the radar outline with interpolated colors
		          CreateRadarOutline(parent, graph, scale);

		          // Create data points
		          CreateDataPoints(parent, graph, scale);
	     }

	     private static void CreateBackground(GameObject parent, Color color)
	     {
		          GameObject background = new GameObject("Background");
		          background.transform.SetParent(parent.transform, false);

		          RectTransform bgRT = background.AddComponent<RectTransform>();
		          bgRT.anchorMin = Vector2.zero;
		          bgRT.anchorMax = Vector2.one;
		          bgRT.sizeDelta = Vector2.zero;
		          bgRT.anchoredPosition = Vector2.zero;

		          Image bgImage = background.AddComponent<Image>();
		          bgImage.color = color;
	     }
	     private static void CreateGraphTitle(GameObject parent, string title)
	     {
		          GameObject titleGO = new GameObject("GraphTitle");
		          titleGO.transform.SetParent(parent.transform, false);

		          RectTransform titleRT = titleGO.AddComponent<RectTransform>();
		          titleRT.anchorMin = new Vector2(0.5f, 1f); // Top center
		          titleRT.anchorMax = new Vector2(0.5f, 1f);
		          titleRT.pivot = new Vector2(0.5f, 1f);
		          titleRT.sizeDelta = new Vector2(80f, 20f);
		          titleRT.anchoredPosition = new Vector2(0f, 3f); // Slightly below top edge

		          TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
		          titleText.text = title;
		          titleText.fontSize = 6;
		          titleText.color = Color.white;
		          titleText.alignment = TextAlignmentOptions.Top;
		          titleText.fontStyle = FontStyles.Normal;
	     }

	     private static void CreateRadarAxes(GameObject parent, Graph graph, float axisLength)
	     {
		          int axisCount = graph.points.Length - 1; // -1 because last point closes the shape
		          float angleIncrement = 360f / axisCount * Mathf.Deg2Rad;

		          for (int i = 0; i < axisCount; i++)
		          {
				float angle = i * angleIncrement;
				float x = Mathf.Cos(angle) * axisLength;
				float y = Mathf.Sin(angle) * axisLength;

				// Create axis line with safe color access
				Color axisColor = (graph.axisColors != null && i < graph.axisColors.Length)
				    ? graph.axisColors[i]
				    : Color.gray;

				axisColor.a = 0.1f; // Semi-transparent

				CreateUILine(parent, Vector3.zero, new Vector3(x, y, 0), axisColor, 1f);
				axisColor.a = 0.6f;
				// Create axis label if available
				if (graph.showLabels && graph.axisLabels != null && i < graph.axisLabels.Length)
				{
					     CreateAxisLabel(parent, new Vector3(x, y, 0), graph.axisLabels[i], axisColor);
				}
		          }
	     }

	     private static void CreateRadarOutline(GameObject parent, Graph graph, float scale)
	     {
		          int pointCount = graph.points.Length;

		          for (int i = 0; i < pointCount - 1; i++)
		          {
				Vector3 start = graph.points[i] * scale;
				Vector3 end = graph.points[i + 1] * scale;

				// Safe color access with fallbacks
				Color startColor = (graph.pointColors != null && i < graph.pointColors.Length)
				    ? graph.pointColors[i]
				    : graph.lineColor;

				Color endColor = (graph.pointColors != null && (i + 1) < graph.pointColors.Length)
				    ? graph.pointColors[i + 1]
				    : graph.lineColor;

				// Use lerped color for the line segment
				Color lineColor = Color.Lerp(startColor, endColor, 0.5f);

				CreateUILine(parent, start, end, lineColor, graph.lineWidth * 10f);
		          }
	     }

	     private static void CreateDataPoints(GameObject parent, Graph graph, float scale)
	     {
		          int pointCount = graph.points.Length - 1; // Don't create point for closing vertex

		          for (int i = 0; i < pointCount; i++)
		          {
				Vector3 position = graph.points[i] * scale;

				// Safe color access
				Color pointColor = (graph.pointColors != null && i < graph.pointColors.Length)
				    ? graph.pointColors[i]
				    : graph.lineColor;

				CreateUIPoint(parent, position, pointColor, 1f);
		          }
	     }

	     private static void CreateUILine(GameObject parent, Vector3 start, Vector3 end, Color color, float width)
	     {
		          GameObject line = new GameObject("Line");
		          line.transform.SetParent(parent.transform, false);

		          RectTransform rt = line.AddComponent<RectTransform>();

		          Vector2 direction = (end - start);
		          float distance = direction.magnitude;
		          Vector2 midPoint = (start + end) * 0.5f;

		          rt.sizeDelta = new Vector2(distance, width);
		          rt.anchoredPosition = midPoint;

		          float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		          rt.localRotation = Quaternion.Euler(0, 0, angle);

		          Image lineImage = line.AddComponent<Image>();
		          lineImage.color = color;
	     }

	     private static void CreateUIPoint(GameObject parent, Vector3 position, Color color, float size)
	     {
		          GameObject point = new GameObject("Point");
		          point.transform.SetParent(parent.transform, false);

		          RectTransform rt = point.AddComponent<RectTransform>();
		          rt.sizeDelta = new Vector2(size, size);
		          rt.anchoredPosition = position;

		          Image pointImage = point.AddComponent<Image>();
		          pointImage.color = color;
	     }

	     private static void CreateAxisLabel(GameObject parent, Vector3 position, string label, Color color)
	     {
		          GameObject labelGO = new GameObject($"Label_{label}");
		          labelGO.transform.SetParent(parent.transform, false);

		          RectTransform labelRT = labelGO.AddComponent<RectTransform>();
		          labelRT.sizeDelta = new Vector2(50f, 8f);
		          labelRT.anchoredPosition = position * 1.2f; // Slightly outside the axis

		          TextMeshProUGUI labelText = labelGO.AddComponent<TextMeshProUGUI>();
		          labelText.text = label;
		          labelText.fontSize = 6;
		          labelText.color = color;
		          //        if (label.Length >= 3)
		          //        {
		          //labelText.color = Color.white;
		          //        }
		          labelText.alignment = TextAlignmentOptions.Center;
	     }



}