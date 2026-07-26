using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	     //these should all be elsewhere, but they are here for testing purposes.
	     public GameObject pepoPanel;
	     public GameObject world;
	     public PepoPopManager pepoPopManager;
	     public SimulationMaster simulationMaster;
	     public PepoUIManager pepoUI;
	     TileManager tm;
	     bool worldWindowsCreated = false;
	     bool pepoWindowsCreated = false;
	     public int HeadingFontSize = 10;
	     public int BodyFontSize = 8;

	     private List<GameObject> currentPanels = new List<GameObject>();

	     //public List<Sprite> pepoEyes;
	     //public List<Sprite> linerEyes;
	     //public List<Sprite> eyeColor;
	     //public List<Sprite> headShape;
	     //public List<Sprite> freckles;
	     //public List<Sprite> topLips;
	     //public List<Sprite> bottomLips;
	     //public List<Sprite> noseShape;
	     ////straight hair
	     //public List<Sprite> HairUp_1;
	     //public List<Sprite> HairBack_1;
	     //public List<Sprite> HairTendrils_1;
	     //public List<Sprite> HairFringe_1;
	     ////wavy hair
	     //public List<Sprite> HairUp_2;
	     //public List<Sprite> HairBack_2;
	     //public List<Sprite> HairTendrils_2;
	     //public List<Sprite> HairFringe_2;
	     ////curly hair
	     //public List<Sprite> HairUp_3;
	     //public List<Sprite> HairBack_3;
	     //public List<Sprite> HairTendrils_3;
	     //public List<Sprite> HairFringe_3;
	     ////coily hair
	     //public List<Sprite> HairUp_4;
	     //public List<Sprite> HairBack_4;
	     //public List<Sprite> HairTendrils_4;
	     //public List<Sprite> HairFringe_4;

	     //public List<Sprite> brows;
	     //public List<Sprite> stubble;
	     //public List<Sprite> moustaches;

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

		          RefreshPepoPanels();
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
				UIWindow pepoWindow = PepoUILibrary.BuildWindowPepoData(pepoPopManager.Pepos.totalPop[Random.RandomRange(0, pepoPopManager.Pepos.totalPop.Count)], this);
				UIWindows.Add(pepoWindow);
				uiFactory.CreateWindow(pepoWindow, Body.transform, this);
				pepoWindowsCreated = true;

		          }

		          foreach (UIWindow window in UIWindows)
		          {
				UIUtilities.RefreshAllFields(window);
		          }
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
	     public void RefreshPepoPanels()
	     {
		          // Clear existing panels
		          foreach (GameObject panel in currentPanels)
		          {
				if (panel != null)
					     Destroy(panel);
		          }
		          currentPanels.Clear();

		          // Check if we have the necessary references
		          if (pepoPopManager == null || pepoPopManager.Pepos == null || pepoPopManager.Pepos.totalPop == null)
		          {
				Debug.LogWarning("PepoPopManager or population data is missing");
				return;
		          }

		          if (pepoPanel == null)
		          {
				Debug.LogWarning("Pepo panel reference is missing");
				return;
		          }

		          // Create new panels - ordered by surname then age
		          foreach (PepoData pepo in pepoPopManager.Pepos.totalPop
		              .OrderBy(p => p.surname)
		              .ThenBy(p => p.age))
		          {
				GameObject panel = pepoUI.CreateStatusPanel(pepo, pepoPanel.transform);
				if (panel != null)
				{
					     currentPanels.Add(panel);

					     // Add Layout Element to control card sizing
					     var layoutElement = panel.AddComponent<LayoutElement>();
					     layoutElement.preferredWidth = 180f;
					     layoutElement.preferredHeight = 120f;
					     layoutElement.flexibleWidth = 0f;
					     layoutElement.flexibleHeight = 0f;
				}
		          }
	     }

	     // Call this method when you know the population has changed
	     public void ForceRefreshUI()
	     {
		          RefreshPepoPanels();
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