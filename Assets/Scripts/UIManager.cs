using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	     public GameObject pepoPanel;
	     public PepoPopManager pepoPopManager;
	     public PepoUIManager pepoUI;

	     private List<GameObject> currentPanels = new List<GameObject>();

	     void Start()
	     {
		          // Ensure we have a reference to PepoUIManager
		          if (pepoUI == null)
				pepoUI = new PepoUIManager();
		          // Initial population of UI
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