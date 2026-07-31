using System.Collections.Generic;
using UnityEngine;

public static class UILibrary
{
	     public static UISection BuildSectionDataIndexTabs(UIManager uim, List<UITab> tabs, UIStructuralComponent sc)
	     { //This is the tab bar that switches between pages of a window
		          List<UIComponentData> buttons = new List<UIComponentData>();

		          {
				foreach (UITab tab in tabs)
				{
					     ButtonField button = new ButtonField("", tab.ComponentName);
					     button.DisplayLabel = false;
					     buttons.Add(button);
				}
		          }

		          UISection section = new UISection("TabIndex", !sc.VerticalLayout, buttons)
		          {
				Padding = sc.Padding,
				Spacing = sc.Spacing,
				VerticalLayout = !sc.VerticalLayout,
				ChildForceExpandWidth = sc.VerticalLayout,  // horizontal strip: expand width
				ChildForceExpandHeight = !sc.VerticalLayout, // vertical strip: expand height
				DisplayLabel = false

		          };
		          return section;
	     }
	     public static UISection BuildSectionDataTime(SimulationMaster sm, UIManager manager)
	     {//this is the clock readout
		          var yearField = new ReadOnlyField("Year", "", (int)manager.theme.BodyFontSize)
		          {
				Getter = () => (sm.year + 1).ToString(),
				VerticalLayout = false
		          };

		          var dayField = new ReadOnlyField("Day", "", (int)manager.theme.BodyFontSize)
		          {
				Getter = () => (sm.dayOfYear + 1).ToString(),
				VerticalLayout = false
		          };

		          var timeField = new ReadOnlyField("Time", "", (int)manager.theme.BodyFontSize)
		          {
				Getter = () =>
				{
					     int hour = Mathf.FloorToInt(SimulationConstants.TimeOfDay * 24);
					     int minute = Mathf.FloorToInt((SimulationConstants.TimeOfDay * 24 - hour) * 60);
					     return $"{hour:00}:{minute:00}";

				},
				VerticalLayout = false
		          };

		          var section = new UISection("Year, Day, Time Panel", true, new List<UIComponentData> { yearField, dayField, timeField });

		          return section;
	     }
	     //next, a section that shows what game speed we are on.
	     public static UISection BuildSectionDataGameSpeed(SimulationMaster sm, UIManager uim)
	     {
		          var gameSpeedField = new ReadOnlyField("Speed", "",  (int)uim.theme.BodyFontSize)
		          {
				Getter = () => (SimulationConstants.SimulationSpeed).ToString(),
				VerticalLayout = false
		          };
		          var section = new UISection("Game Speed Section", true, new List<UIComponentData>() { gameSpeedField });

		          return section;
	     }
	     public static UITab BuildTabDataClock(SimulationMaster simMaster, UIManager manager)
	     {

		          var tab = new UITab("Time", new List<UISection> { BuildSectionDataTime(simMaster, manager) });


		          return tab;
	     }



	     public static UIWindow BuildWindowDataSimulation(SimulationMaster simMaster, UIManager manager)
	     {
		          var window = new UIWindow
				("Simulation", new List<UITab>
				{ BuildTabDataClock
				(simMaster, manager), new UITab
				("GameSpeed", new List<UISection>
				{ BuildSectionDataGameSpeed(simMaster, manager) }) })
		          {
				PreferredSize = new Vector2(150, 150),
				CanResize = false,
				CanClose = false,
				CanMinimize = false
		          };

		          return window;
	     }

	     // ─── Block Composition Window ──────────────────────────────────────

	     /// <summary>
	     /// Builds the "Matter Composition" section displaying bar graphs for Atmosphere, Soil, and Water.
	     /// </summary>
	     //  public static UISection BuildSectionBlockCompositionDisplay(BlockData block, UIManager uim)
	     //  {
	     //        List<UIComponentData> components = new List<UIComponentData>();

	     //        // Atmosphere composition bar graph
	     //        var atmosphereField = new GraphField("Atmosphere", GraphType.bar, 1f, 100f)
	     //        {
	     //Getter = () =>
	     //{
	     //	     if (block == null)
	     //		          return new Graph { points = new Vector3[0], pointColors = new Color[0], axisLabels = new string[0], axisColors = new Color[0] };
	     //	     List<float> values = new List<float>();
	     //	     // Extract atmosphere composition values from BlockData
	     //	     // Assuming block has atmosphere matter composition
	     //	     foreach (var kvp in block.atmosphereComposition) // Adjust based on actual structure
	     //	     {
	     //		          values.Add(kvp.Value);
	     //	     }
	     //	     return UIUtilities.GraphFloats(values, 100f, GraphType.bar);
	     //},
	     //VerticalLayout = false
	     //        };

	     //        // Soil composition bar graph
	     //        var soilField = new GraphField("Soil", GraphType.bar, 1f, 100f)
	     //        {
	     //Getter = () =>
	     //{
	     //	     if (block == null)
	     //		          return new Graph { points = new Vector3[0], pointColors = new Color[0], axisLabels = new string[0], axisColors = new Color[0] };
	     //	     List<float> values = new List<float>();
	     //	     foreach (var kvp in block.soilComposition)
	     //	     {
	     //		          values.Add(kvp.Value);
	     //	     }
	     //	     return UIUtilities.GraphFloats(values, 100f, GraphType.bar);
	     //},
	     //VerticalLayout = false
	     //        };

	     //        // Water composition bar graph
	     //        var waterField = new GraphField("Water", GraphType.bar, 1f, 100f)
	     //        {
	     //Getter = () =>
	     //{
	     //	     if (block == null)
	     //		          return new Graph { points = new Vector3[0], pointColors = new Color[0], axisLabels = new string[0], axisColors = new Color[0] };
	     //	     List<float> values = new List<float>();
	     //	     foreach (var kvp in block.waterComposition)
	     //	     {
	     //		          values.Add(kvp.Value);
	     //	     }
	     //	     return UIUtilities.GraphFloats(values, 100f, GraphType.bar);
	     //},
	     //VerticalLayout = false
	     //        };

	     //        components.Add(atmosphereField);
	     //        components.Add(soilField);
	     //        components.Add(waterField);

	     //        var section = new UISection("Matter Composition", true, components)
	     //        {
	     //Padding = new RectOffset(5, 5, 5, 5),
	     //Spacing = 10,
	     //VerticalLayout = true,
	     //ChildForceExpandWidth = true,
	     //ChildForceExpandHeight = false,
	     //DisplayLabel = true
	     //        };

	     //        return section;
	     //  }

	     /// <summary>
	     /// Builds the "Modify Instance" section for altering block composition.
	     /// </summary>
	     public static UISection BuildSectionBlockModifyInstance(BlockData block, TileManager world, UIManager uim)
	     {
		          List<UIComponentData> components = new List<UIComponentData>();

		          // Block Reference (X, Y) dropdown
		          var blockRefField = new DropdownField("Block Reference (X,Y)", new List<string> { "Select block..." }, 0,  (int)uim.theme.BodyFontSize)
		          {
				Getter = () => 0,

				VerticalLayout = false
		          };

		          // Block to alter dropdown
		          var blockToAlterField = new DropdownField("Block to Alter", new List<string> { "Atmosphere", "Soil", "Water" }, 0,  (int)uim.theme.BodyFontSize)
		          {
				Getter = () => 0,
				VerticalLayout = false
		          };

		          // Matter to alter dropdown (dynamically populated based on selected block)
		          var matterToAlterField = new DropdownField("Matter to Alter", new List<string> { "Select block first..." }, 0,  (int)uim.theme.BodyFontSize)
		          {
				Getter = () => 0,
				VerticalLayout = false
		          };

		          // KG change input field
		          var kgChangeField = new TextInputField("KG Change", "0",  (int)uim.theme.BodyFontSize)
		          {
				Getter = () => "0",
				Setter = (val) =>
				{
					     if (int.TryParse(val?.ToString(), out int kgValue))
					     {
						          // Apply change to block composition
						          // Implementation depends on BlockData structure
					     }
				},
				VerticalLayout = false
		          };

		          // Amount to add/remove (convenience field)
		          var amountField = new RangeField("Amount to Add/Remove (kg)", -100f, 100f, true, (int)(int)uim.theme.BodyFontSize)
		          {
				Getter = () => 0f,
				Setter = (val) =>
				{
					     // Link to KG change or apply directly
				},
				VerticalLayout = false
		          };

		          // Apply button
		          var applyButton = new ButtonField("", "Apply Changes", () =>
		          {
				// Execute the block modification logic here

				Debug.Log("Block composition modified");
		          }, (int)(int)uim.theme.HeadingFontSize)
		          {
				DisplayLabel = false,
				VerticalLayout = false
		          };

		          components.Add(blockRefField);
		          components.Add(blockToAlterField);
		          components.Add(matterToAlterField);
		          components.Add(amountField);
		          components.Add(kgChangeField);
		          components.Add(applyButton);

		          var section = new UISection("Modify Instance", true, components)
		          {
				Padding = new RectOffset(5, 5, 5, 5),
				Spacing = 5,
				VerticalLayout = true,
				ChildForceExpandWidth = true,
				ChildForceExpandHeight = false,
				DisplayLabel = true
		          };

		          return section;
	     }

	     //  /// <summary>
	     //  /// Builds the complete Block Composition window with both display and modification sections.
	     //  /// </summary>
	     //  public static UIWindow BuildWindowBlockComposition(BlockData block, TileManager world, UIManager uim)
	     //  {
	     //        var compositionTab = new UITab("Composition", new List<UISection>
	     //        {
	     //BuildSectionBlockCompositionDisplay(block, uim),
	     //BuildSectionBlockModifyInstance(block, world, uim)
	     //        });

	     //        var window = new UIWindow("Block Composition", new List<UITab> { compositionTab })
	     //        {
	     //PreferredSize = new Vector2(400, 500),
	     //CanResize = true,
	     //CanClose = true,
	     //CanMinimize = false
	     //        };

	     //        return window;
	     //  }

	     // ─── Tile Composition Window ───────────────────────────────────────

	     /// <summary>
	     /// Builds the "Matter Composition" section with three bar graphs:
	     /// 1. Soil composition (sand, silt, clay, organic matter)
	     /// 2. Surface water content
	     /// 3. Atmosphere tile composition
	     /// </summary>
	     public static UISection BuildSectionTileCompositionDisplay(UISection_Tile rootSection)
	     {

		          List<UIComponentData> components = new List<UIComponentData>();

		          // ─── Soil Composition Graph ───
		          var soilCompositionField = new GraphField("Soil Composition", GraphType.bar, 1f, 120f)
		          {
				Getter = () =>
				{
					     return UIUtilities.CreateSoilGraph(rootSection.tileData.composition);
				},
				VerticalLayout = false
		          };

		          // ─── Surface Water Graph ───
		          var surfaceLiquidField = new GraphField("Water", GraphType.bar, 1f, 120f)
		          {
				Getter = () =>
				{
					     return UIUtilities.CreateWaterGraph(rootSection.tileData);
				},
				VerticalLayout = false
		          };

		          // ─── Atmosphere Composition Graph ───
		          var atmosphereCompositionField = new GraphField("Atmosphere Tile", GraphType.bar, 1f, 120f)
		          {
				Getter = () =>
				{
					     return UIUtilities.CreateAtmosphereGraph(rootSection.tileData.atmosphereTile);
				},
				VerticalLayout = false
		          };

		          components.Add(soilCompositionField);
		          components.Add(surfaceLiquidField);
		          components.Add(atmosphereCompositionField);
		          var section = new UISection("Matter Composition", true, components)
		          {
				Padding = new RectOffset(5, 5, 5, 5),
				Spacing = 5,
				VerticalLayout = true,
				ChildForceExpandWidth = true,
				ChildForceExpandHeight = false,
				DisplayLabel = true
		          };
		          return section;
	     }

	     /// <summary>
	     /// Builds the "Modify Instance" section for altering tile composition.
	     /// </summary>
	     public static UISection BuildSectionTileModifyInstance(UISection_Tile root, TileManager manager, UIManager uim)
	     {
		          // 1. Create the components list (empty for now)
		          List<UIComponentData> components = new List<UIComponentData>();

		          // 2. CREATE THE SECTION FIRST. 
		          // We pass 'initialTile' into it so its `tileData` field starts correctly.
		          var section = new UISection("Modify Tile", true, components)
		          {
				Padding = new RectOffset(5, 5, 5, 5),
				Spacing = 5,
				VerticalLayout = true,
				DisplayLabel = true
		          };

		          // ─── Build X and Y options separately ───
		          List<string> xOptions = new List<string>();
		          List<string> yOptions = new List<string>();
		          for (int x = 0; x < manager.worldsise.x; x++)
				xOptions.Add(x.ToString());
		          for (int y = 0; y < manager.worldsise.y; y++)
				yOptions.Add(y.ToString());

		          // ─── Horizontal layout to put X and Y side by side ───
		          var coordSection = new UISection("Tile Coordinates", false, new List<UIComponentData>())
		          {
				DisplayLabel = false,
				Spacing = 5,
				Padding = new RectOffset(0, 0, 0, 0),
				VerticalLayout = false,
				ChildForceExpandWidth = true
		          };

		          // ─── X Dropdown ───
		          var tileRefFieldX = new DropdownField("X", xOptions, root.tileData.tileGridRef.x,  (int)uim.theme.BodyFontSize)
		          {
				Getter = () => root.tileData.tileGridRef.x,
				Setter = (val) =>
				{
					     int newX = (int)val;
					     Vector2 pos = new Vector2(newX, root.tileData.tileGridRef.y);
					     TileData newTile = manager.GetTileData(pos);
					     if (newTile != null)
					     {
						          root.tileData = newTile;
						          UIUtilities.RefreshAllFields(coordSection);
					     }
				},
				VerticalLayout = false
		          };
		          coordSection.Components.Add(tileRefFieldX);

		          // ─── Y Dropdown ───
		          var tileRefFieldY = new DropdownField("Y", yOptions, root.tileData.tileGridRef.y,  (int)uim.theme.BodyFontSize)
		          {
				Getter = () => root.tileData.tileGridRef.y,
				Setter = (val) =>
				{
					     int newY = (int)val;
					     Vector2 pos = new Vector2(root.tileData.tileGridRef.x, newY);
					     TileData newTile = manager.GetTileData(pos);
					     if (newTile != null)
					     {
						          root.tileData = newTile;
						          UIUtilities.RefreshAllFields(coordSection);
					     }
				},
				VerticalLayout = false
		          };
		          coordSection.Components.Add(tileRefFieldY);

		          // 7. MODIFICATION CONTROLS
		          int selectedMatterType = 0;
		          float kgAmount = 1000f;

		          var matterTypeField = new DropdownField("Matter Type",
		              new List<string> { "Sand", "Silt", "Clay", "Organic", "Water (Soil)", "Ice (Soil)" },
		              0,  (int)uim.theme.BodyFontSize)
		          {
				Getter = () => selectedMatterType,
				Setter = (val) => selectedMatterType = (int)val,
				VerticalLayout = false
		          };

		          var amountField = new RangeField("Amount (kg)", -500f, 500f, true,  (int)uim.theme.BodyFontSize)
		          {
				Getter = () => kgAmount,
				Setter = (val) => kgAmount = (float)val,
				VerticalLayout = false
		          };

		          var applyButton = new ButtonField("", "Apply Changes", () =>
		          {
				// THIS uses the SECTION's current tile!
				if (root.tileData == null)
					     return;

				ThermalEnergy target = null;
				switch (selectedMatterType)
				{
				case 0:
				target = root.tileData.composition.sand;
				break;
				case 1:
				target = root.tileData.composition.silt;
				break;
				case 2:
				target = root.tileData.composition.clay;
				break;
				case 3:
				target = root.tileData.composition.organicMaterial;
				break;
				case 4:
				target = root.tileData.composition.water;
				break;
				case 5:
				target = root.tileData.composition.ice;
				break;
				}

				if (target != null)
				{
					     target.AddMass(kgAmount, target.Temperature);
					     Debug.Log($"Added {kgAmount}kg of {target.materialName} to {root.tileData.tileGridRef}");

					     // CRITICAL: Tell the UI to re-run the Getters so the graphs update!
					     UIUtilities.RefreshAllFields(section);
				}

		          }, (int)(int)uim.theme.HeadingFontSize)
		          {
				DisplayLabel = false,
				VerticalLayout = false
		          };

		          // 8. Add all components to the section's list
		          section.Components.Add(coordSection);
		          section.Components.Add(matterTypeField);
		          section.Components.Add(amountField);
		          section.Components.Add(applyButton);

		          // Return the section. The `components` list is already inside it.
		          return section;
	     }
	     /// <summary>
	     /// Builds the complete Tile Composition window with both display and modification sections.
	     /// </summary>
	     public static UIWindow BuildWindowTileComposition(Vector2 tileRef, TileManager tileManager, UIManager uim)
	     {
		          UISection_Tile rootTile = new UISection_Tile("Tile ref", tileManager.tiles[(int)tileRef.x - 1, (int)tileRef.y - 1].tileData, true);
		          var compositionTab = new UITab("Tile Information", new List<UISection>
		          {
				//rootTile,
				BuildSectionTileCompositionDisplay(rootTile)
		          });

		          var modifyTab = new UITab("Modify Tile", new List<UISection>
		          {
				BuildSectionTileModifyInstance(rootTile, tileManager, uim)
		          });

		          var window = new UIWindow("Tile Composition", new List<UITab> { modifyTab, compositionTab, })
		          {
				PreferredSize = new Vector2(250, 550),
				CanResize = true,
				CanClose = true,
				CanMinimize = false,
				VerticalLayout = true

		          };

		          return window;
	     }

}