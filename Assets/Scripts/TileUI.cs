using UnityEngine;

public class TileUI : MonoBehaviour
{

	     public TileGrid grid;
	     // Start is called before the first frame update
	     void Start()
	     {
		          grid = gameObject.GetComponent<TileManager>().grid;
	     }

	     // Update is called once per frame
	     void Update()
	     {

	     }

}
