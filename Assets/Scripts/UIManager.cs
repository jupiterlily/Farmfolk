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

	     public List<Sprite> pepoEyes;
	     public List<Sprite> linerEyes;
	     public List<Sprite> eyeColor;
	     public List<Sprite> headShape;
	     public List<Sprite> freckles;
	     public List<Sprite> topLips;
	     public List<Sprite> bottomLips;
	     public List<Sprite> noseShape;
	     //straight hair
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
	     void Start()
	     {
		          // Ensure we have a reference to PepoUIManager
		          if (pepoUI == null)
				pepoUI = GetComponent<PepoUIManager>() ?? gameObject.AddComponent<PepoUIManager>();

		          // Pass sprite references to PepoUIManager
		          if (pepoUI != null)
		          {
				pepoUI.mascEyes = pepoEyes;
				pepoUI.femmeEyes = linerEyes;
				pepoUI.headShape = headShape;
				pepoUI.freckles = freckles;
				pepoUI.topLips = topLips;
				pepoUI.bottomLips = bottomLips;
				pepoUI.noseShape = noseShape;
				pepoUI.eyecolor = eyeColor;

				pepoUI.brows = brows;
				pepoUI.stubble = stubble;
				pepoUI.moustaches = moustaches;

				pepoUI.HairUp_1 = HairUp_1;
				pepoUI.HairBack_1 = HairBack_1;
				pepoUI.HairTendrils_1 = HairTendrils_1;
				pepoUI.HairFringe_1 = HairFringe_1;

				pepoUI.HairUp_2 = HairUp_2;
				pepoUI.HairBack_2 = HairBack_2;
				pepoUI.HairTendrils_2 = HairTendrils_2;
				pepoUI.HairFringe_2 = HairFringe_2;

				pepoUI.HairUp_3 = HairUp_3;
				pepoUI.HairBack_3 = HairBack_3;
				pepoUI.HairTendrils_3 = HairTendrils_3;
				pepoUI.HairFringe_3 = HairFringe_3;

				pepoUI.HairUp_4 = HairUp_4;
				pepoUI.HairBack_4 = HairBack_4;
				pepoUI.HairTendrils_4 = HairTendrils_4;
				pepoUI.HairFringe_4 = HairFringe_4;
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