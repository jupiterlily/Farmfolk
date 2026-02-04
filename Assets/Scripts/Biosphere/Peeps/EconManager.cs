using UnityEngine;

public class EconManager : MonoBehaviour
{
	     [SerializeField]
	     private EconVisual econVisual;
	     private Market market;

	     public void Start()
	     {
		          market = MarketUtilities.TestEconomy();
	     }

	     void Update()
	     {
		          // Update logic can be added here if needed
	     }

	     public void RunMarket()
	     {
		          if (market != null)
		          {

		          }
		          else
		          {
				Debug.LogError("Market is not initialized.");
		          }
	     }
}
