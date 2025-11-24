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

	     public List<Sprite> mascEyes;
	     public List<Sprite> femmeEyes;
	     public List<Sprite> eyeColor;
	     public List<Sprite> headShape;
	     public List<Sprite> freckles;
	     public List<Sprite> mouthShape;
	     public List<Sprite> noseShape;
	     public List<Sprite> uHairBack;
	     public List<Sprite> uHhairFront;
	     public List<Sprite> mHairBack;
	     public List<Sprite> mHairFront;
	     public List<Sprite> fHairBack;
	     public List<Sprite> fHairFront;
	     void Start()
	     {
		          // Ensure we have a reference to PepoUIManager
		          if (pepoUI == null)
				pepoUI = GetComponent<PepoUIManager>() ?? gameObject.AddComponent<PepoUIManager>();

		          // Pass sprite references to PepoUIManager
		          if (pepoUI != null)
		          {
				pepoUI.mascEyes = mascEyes;
				pepoUI.femmeEyes = femmeEyes;
				pepoUI.headShape = headShape;
				pepoUI.freckles = freckles;
				pepoUI.mouthShape = mouthShape;
				pepoUI.noseShape = noseShape;
				pepoUI.eyecolor = eyeColor;
				pepoUI.uHairBack = uHairBack;
				pepoUI.uHairFront = uHhairFront;
				pepoUI.mHairBack = mHairBack;
				pepoUI.mHairFront = mHairFront;
				pepoUI.fHairBack = fHairBack;
				pepoUI.fHairFront = fHairFront;
		          }

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