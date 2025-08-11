using UnityEngine;

public class Tile : MonoBehaviour
{
	     public TileData tileData;
	     float tileSize;

	     // Start is called before the first frame update
	     public void Setup(TileData td)
	     {
		          tileData = td;
		          tileSize = tileData.tileWidth;
		          tileData.compaction = Random.Range(0.99f, 0.75f);
		          transform.localScale = new Vector3(tileSize, tileSize - tileData.compaction, tileSize);
	     }

	     // Update is called once per frame
	     void Update()
	     {

	     }
}
