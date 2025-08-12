using UnityEngine;

public class Tile : MonoBehaviour
{
	     public TileData tileData;
	     float tileSize;
	     public bool isHovered = false;
	     public bool visible = true;

	     public void Setup(TileData td)
	     {
		          tileData = td;
		          tileSize = tileData.tileWidth;
		          //draw  the  tile based on the volume of  material   in  it
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
				SoilUtilities.CalculateDerivedProperties(tileData.composition);
				transform.localScale = new Vector3(tileSize, tileSize - tileData.compaction, tileSize);
				// Reset color when not hovered
				GetComponent<Renderer>().material.color = TileUtilities.GetSoilColor(tileData.composition);

		          }
	     }
}
