using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	     //these should all be elsewhere, but they are here for testing purposes.
	     //   public GameObject pepoPanel;
	     public GameObject world;
	     public PepoPopManager pepoPopManager;
	     public SimulationMaster simulationMaster;
	     public PepoUIManager pepoUI;
	     TileManager tm;
	     bool worldWindowsCreated = false;
	     bool pepoWindowsCreated = false;
	     // public int HeadingFontSize = 10;
	     //  public int BodyFontSize = 8;

	     public UITheme theme;

	     private List<GameObject> currentPanels = new List<GameObject>();

	     //here I'm going to start the non testing code. This will be the code that actually runs the UI, and it will be the code that is used in the final product.

	     public GameObject Header;
	     public GameObject LeftSidebar;
	     public GameObject Body;
	     public GameObject RightSidebar;
	     public GameObject Footer;

	     public GameObject Clock;

	     public UIFactory uiFactory;

	     //public GameObject[] UIWindows; //array of windows that can be dragged around and resized. they can also be closed or minimized.
	     public List<UIWindow> UIWindows = new List<UIWindow>();



	     void Start()
	     {
		          //if there's no theme
		          if (theme != null)
		          {
				SetPanelSizes();
		          }

		          // Ensure we have a reference to PepoUIManager
		          if (pepoUI == null)
		          {
				pepoUI = GetComponent<PepoUIManager>();
		          }
		          if (simulationMaster == null)
		          {
				//simulation master lives on the world gameObject.
				simulationMaster = world.GetComponent<SimulationMaster>();
		          }
		          if (pepoPopManager == null)
		          {
				pepoPopManager = world.GetComponent<PepoPopManager>();
		          }
		          // put a clock in the header from the UI  Library and using the factory.

		          uiFactory = new UIFactory();
		          UIWindows.Add(UILibrary.BuildWindowDataSimulation(simulationMaster, this));
		          tm = world.GetComponent<TileManager>();

		          //we should go through each and search for clock.
		          foreach (UIWindow window in UIWindows)
		          {
				if (window.ComponentName == "Simulation")
				{
					     Clock = uiFactory.CreateWindow(window, Header.transform, this);
				}
				else
				{
					     uiFactory.CreateWindow(window, LeftSidebar.transform, this);
				}
				UIUtilities.RefreshAllFields(window);
		          }
	     }


	     void Update()
	     {
		          // Only refresh UI occasionally or when population changes
		          //        if (Time.frameCount % 60 == 0) // 60 frames = 1 second
		          //        {
		          //RefreshPepoPanels();
		          //        }

	     }

	     [System.Obsolete]
	     private void FixedUpdate()
	     {
		          // Update the clock every fixed update to reflect simulation time
		          //UpdateClock(Clock);
		          if (!worldWindowsCreated)
		          {
				UIWindow tileWindow = UILibrary.BuildWindowTileComposition(tm.worldsise, tm, this);
				UIWindows.Add(tileWindow);
				uiFactory.CreateWindow(tileWindow, LeftSidebar.transform, this);
				worldWindowsCreated = true;


		          }
		          if (!pepoWindowsCreated)
		          {
				// Create new panels - ordered by surname then age
				foreach (PepoData pepo in pepoPopManager.Pepos.totalPop
				    .OrderBy(p => p.surname)
				    .ThenBy(p => p.age))
				{

					     UIWindow pepoWindow = PepoUILibrary.BuildWindowPepoData(pepo, this);
					     UIWindows.Add(pepoWindow);
					     uiFactory.CreateWindow(pepoWindow, Body.transform, this);
				}
				pepoWindowsCreated = true;

		          }

		          foreach (UIWindow window in UIWindows)
		          {
				UIUtilities.RefreshAllFields(window);
		          }
	     }
	     public void SetPanelSizes()
	     {
		          Header.GetComponent<LayoutElement>().minHeight = theme.screenHeight * theme.HeaderPercentage;
		          Header.GetComponent<LayoutElement>().preferredHeight = theme.screenHeight * theme.HeaderPercentage;

		          Footer.GetComponent<LayoutElement>().minHeight = theme.screenHeight * theme.FooterPercentage;
		          Footer.GetComponent<LayoutElement>().preferredHeight = theme.screenHeight * theme.FooterPercentage;

		          LeftSidebar.GetComponent<LayoutElement>().minWidth = theme.screenWidth * theme.LeftSidebarPercentage;
		          LeftSidebar.GetComponent<LayoutElement>().preferredWidth = theme.screenWidth * theme.LeftSidebarPercentage;

		          RightSidebar.GetComponent<LayoutElement>().preferredWidth = theme.screenWidth * theme.RightSidebarPercentage;
		          RightSidebar.GetComponent<LayoutElement>().minWidth = theme.screenWidth * theme.RightSidebarPercentage;
	     }
	     // Update the on-screen readout with the current state of the simulation.
	     public void UpdateClock(GameObject clock)
	     {
		          if (clock == null)
		          {
				Debug.LogWarning("Clock GameObject is null");
				return;
		          }

		          // Assuming 'clock' is the "Section Time" parent containing the three fields as direct children.
		          Transform yearField = clock.transform.GetChild(0); // "Field Readout Int"
		          Transform dayField = clock.transform.GetChild(1);  // "Field Readout Int (1)"
		          Transform timeField = clock.transform.GetChild(2); // "Field Readout Int (2)"

		          if (yearField == null || dayField == null || timeField == null)
		          {
				Debug.LogWarning("One or more clock fields are missing");
				return;
		          }

		          // Get the "Readout" child (index 1) from each field
		          Transform yearReadoutTransform = yearField.GetChild(1);
		          Transform dayReadoutTransform = dayField.GetChild(1);
		          Transform timeReadoutTransform = timeField.GetChild(1);

		          if (yearReadoutTransform == null || dayReadoutTransform == null || timeReadoutTransform == null)
		          {
				Debug.LogWarning("One or more 'Readout' children are missing");
				return;
		          }

		          // Get the TMP_Text component directly from the "Readout" GameObject itself
		          TMP_Text yearReadout = yearReadoutTransform.GetComponent<TMP_Text>();
		          TMP_Text dayReadout = dayReadoutTransform.GetComponent<TMP_Text>();
		          TMP_Text timeReadout = timeReadoutTransform.GetComponent<TMP_Text>();

		          if (yearReadout == null || dayReadout == null || timeReadout == null)
		          {
				Debug.LogWarning("One or more TMP_Text components are missing on 'Readout' objects");
				return;
		          }

		          // Calculate time
		          int currentHourOfDay = Mathf.FloorToInt(SimulationConstants.TimeOfDay * 24);
		          int currentMinuteOfHour = Mathf.FloorToInt((SimulationConstants.TimeOfDay * 24 - currentHourOfDay) * 60);

		          // Use '=' to replace the text, not '+='
		          yearReadout.text = $"{simulationMaster.year + 1}\n";
		          dayReadout.text = $"{simulationMaster.dayOfYear + 1}\n";
		          timeReadout.text = $"{currentHourOfDay:00}:{currentMinuteOfHour:00}\n";
	     }

	     void OnDestroy()
	     {
		          // Cleanup
		          foreach (GameObject panel in currentPanels)
		          {
				if (panel != null)
					     Destroy(panel);
		          }
		          currentPanels.Clear();
	     }
}