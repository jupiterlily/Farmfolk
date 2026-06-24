using System.Collections.Generic;
using UnityEngine;

public static class UILibrary
{
	     public static UISection BuildSectionDataIndexTabs(UIManager uim, List<UITab> tabs, UIStructuralComponent sc)
	     {
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
	     {
		          var yearField = new ReadOnlyField("Year", "", manager.BodyFontSize)
		          {
				Getter = () => (sm.year + 1).ToString(),
				VerticalLayout = false
		          };

		          var dayField = new ReadOnlyField("Day", "", manager.BodyFontSize)
		          {
				Getter = () => (sm.dayOfYear + 1).ToString(),
				VerticalLayout = false
		          };

		          var timeField = new ReadOnlyField("Time", "", manager.BodyFontSize)
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
		          var gameSpeedField = new ReadOnlyField("Speed", "", uim.BodyFontSize)
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
				PreferredSize = new Vector2(300, 150),
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
		          var blockRefField = new DropdownField("Block Reference (X,Y)", new List<string> { "Select block..." }, 0, uim.BodyFontSize)
		          {
				Getter = () => 0,

				VerticalLayout = false
		          };

		          // Block to alter dropdown
		          var blockToAlterField = new DropdownField("Block to Alter", new List<string> { "Atmosphere", "Soil", "Water" }, 0, uim.BodyFontSize)
		          {
				Getter = () => 0,
				VerticalLayout = false
		          };

		          // Matter to alter dropdown (dynamically populated based on selected block)
		          var matterToAlterField = new DropdownField("Matter to Alter", new List<string> { "Select block first..." }, 0, uim.BodyFontSize)
		          {
				Getter = () => 0,
				VerticalLayout = false
		          };

		          // KG change input field
		          var kgChangeField = new TextInputField("KG Change", "0", uim.BodyFontSize)
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
		          var amountField = new RangeField("Amount to Add/Remove (kg)", -100f, 100f, true, uim.BodyFontSize)
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
		          }, uim.HeadingFontSize)
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
	     public static UISection BuildSectionTileCompositionDisplay(TileData tile, UIManager uim)
	     {
		          if (tile == null)
				return new UISection("Matter Composition", true, new List<UIComponentData>());

		          List<UIComponentData> components = new List<UIComponentData>();

		          // ─── Soil Composition Graph ───
		          var soilCompositionField = new GraphField("Soil Composition", GraphType.bar, 1f, 120f)
		          {
				Getter = () =>
				{
					     List<float> soilValues = new List<float>
					     {
						          tile.composition.sand.Mass,
						          tile.composition.silt.Mass,
						          tile.composition.clay.Mass,
						          tile.composition.organicMaterial.Mass
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
						          new Color(1f, 0.9f, 0.7f),      // Sand - tan
						          new Color(0.8f, 0.7f, 0.5f),    // Silt - brown
						          new Color(0.6f, 0.5f, 0.3f),    // Clay - dark brown
						          new Color(0.3f, 0.2f, 0.1f)     // Organic - very dark
					     };
					     return graph;
				},
				VerticalLayout = false
		          };

		          // ─── Surface Water Graph ───
		          var surfaceWaterField = new GraphField("Surface Water", GraphType.bar, 1f, 120f)
		          {
				Getter = () =>
				{
					     List<float> waterValues = new List<float> { tile.surfaceWater.Mass };
					     Graph graph = UIUtilities.GraphFloats(waterValues, Mathf.Max(tile.surfaceWater.Mass * 1.2f, 100f), GraphType.bar);
					     graph.axisLabels = new string[] { "Water" };
					     graph.axisColors = new Color[] { new Color(0.3f, 0.6f, 1f) }; // Water blue
					     return graph;
				},
				VerticalLayout = false
		          };

		          // ─── Atmosphere Composition Graph ───
		          var atmosphereCompositionField = new GraphField("Atmosphere Tile", GraphType.bar, 1f, 120f)
		          {
				Getter = () =>
				{
					     if (tile.atmosphereTile == null)
						          return new Graph { points = new Vector3[0], pointColors = new Color[0], axisLabels = new string[0], axisColors = new Color[0] };

					     List<float> atmValues = new List<float>
					     {
						          tile.atmosphereTile.nitrogen.Mass,
						          tile.atmosphereTile.oxygen.Mass,
						          tile.atmosphereTile.carbonDioxide.Mass,
						          tile.atmosphereTile.waterVapor.Mass,
						          tile.atmosphereTile.cloudWaterContent.Mass
					     };
					     float maxAtmValue = 0;
					     foreach (var value in atmValues)
					     {
						          if (value > maxAtmValue)
						          {
								maxAtmValue = value;
						          }
					     }
					     Graph graph = UIUtilities.GraphFloats(atmValues, Mathf.Max(maxAtmValue, 1f), GraphType.bar);
					     graph.axisLabels = new string[] { "N2", "O2", "CO2", "H2O", "Cloud" };
					     graph.axisColors = new Color[]
					     {
						          new Color(0.7f, 0.8f, 0.9f),    // Nitrogen - light blue
						          new Color(0.5f, 0.7f, 1f),      // Oxygen - sky blue
						          new Color(0.4f, 0.4f, 0.4f),    // CO2 - gray
						          new Color(0.8f, 0.9f, 1f),      // Water vapor - pale blue
						          new Color(1f, 1f, 1f)           // Cloud - white
					     };
					     return graph;
				},
				VerticalLayout = false
		          };

		          components.Add(soilCompositionField);
		          components.Add(surfaceWaterField);
		          components.Add(atmosphereCompositionField);

		          var section = new UISection("Matter Composition", true, components)
		          {
				Padding = new RectOffset(5, 5, 5, 5),
				Spacing = 10,
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
	     public static UISection BuildSectionTileModifyInstance(TileData tile, TileManager tileManager, UIManager uim)
	     {
		          if (tile == null)
				return new UISection("Modify Instance", true, new List<UIComponentData>());

		          List<UIComponentData> components = new List<UIComponentData>();
		          // Variables for binding
		          int selectedTileIndex = 0;
		          int selectedMatterType = 0;
		          float kgAmount = 0f;

		          // Build tile reference dropdown from all tiles
		          List<string> tileOptions = new List<string>();
		          for (int x = 0; x < tileManager.worldsise.x; x++)
				for (int y = 0; y < tileManager.worldsise.y; y++)
					     tileOptions.Add($"{x}, {y}");

		          var tileRefField = new DropdownField("Block Reference (X,Y)", tileOptions, 0, uim.BodyFontSize)
		          {
				OptionsGetter = () =>
				{
					     List<string> options = new List<string>();
					     for (int x = 0; x < tileManager.worldsise.x; x++)
						          for (int y = 0; y < tileManager.worldsise.y; y++)
								options.Add($"({x},{y})");
					     return options;
				},
				Getter = () => selectedTileIndex, // an int variable
				Setter = (val) => selectedTileIndex = (int)val,
				VerticalLayout = false
		          };

		          var matterTypeField = new DropdownField("Matter Type",
		              new List<string> { "Sand", "Silt", "Clay", "Organic", "Water (Soil)", "Water (Surface)" },
		              0, uim.BodyFontSize)
		          {
				Getter = () => selectedMatterType,
				Setter = (val) => selectedMatterType = (int)val,
				VerticalLayout = false
		          };

		          var amountField = new RangeField("Amount (kg)", -500f, 500f, true, uim.BodyFontSize)
		          {
				Getter = () => kgAmount,
				Setter = (val) => kgAmount = (float)val,
				VerticalLayout = false
		          };

		          var applyButton = new ButtonField("", "Apply Changes", () =>
		          {
				Debug.Log($"Applying {kgAmount} kg of matter type {selectedMatterType} to tile ({tile.tileGridRef.x}, {tile.tileGridRef.y})");
				// Implement actual modification logic here 
				//we should call a function on the tile? on the matterObject? 

		          }, uim.HeadingFontSize)
		          {
				DisplayLabel = false,
				VerticalLayout = false
		          };

		          components.Add(tileRefField);
		          components.Add(matterTypeField);
		          components.Add(amountField);
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

	     /// <summary>
	     /// Builds the complete Tile Composition window with both display and modification sections.
	     /// </summary>
	     public static UIWindow BuildWindowTileComposition(Vector2 tileRef, TileManager tileManager, UIManager uim)
	     {
		          var compositionTab = new UITab("Composition", new List<UISection>
		          {
				BuildSectionTileCompositionDisplay(tileManager.tiles[(int)tileRef.x-1,(int)tileRef.y-1].tileData, uim),
				BuildSectionTileModifyInstance(tileManager.tiles[(int)tileRef.x-1,(int) tileRef.y - 1].tileData, tileManager, uim)
		          });

		          var window = new UIWindow("Tile Composition", new List<UITab> { compositionTab })
		          {
				PreferredSize = new Vector2(450, 550),
				CanResize = true,
				CanClose = true,
				CanMinimize = false
		          };

		          return window;
	     }
}
