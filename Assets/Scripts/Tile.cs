using UnityEngine;

public class Tile : MonoBehaviour
{
	     public TileData tileData;
	     float tileSize;
	     public bool isHovered;

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
		          if (isHovered)
		          {
				// Add hover effect, outline.
				GetComponent<Renderer>().material.color = Color.yellow;
		          }
		          else
		          {
				// Reset color when not hovered
				GetComponent<Renderer>().material.color = tileData.composition.GetSoilColor();

		          }
	     }
}
