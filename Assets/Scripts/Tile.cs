using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
	     public TileData tileData;
	     float tileSize;
	     public bool isHovered = false;
	     public bool visible = true;
	     public WorldTile NWTile;
	     public WorldTile NTile;
	     public WorldTile NETile;
	     public WorldTile WTile;
	     public WorldTile ETile;
	     public WorldTile SWTile;
	     public WorldTile STile;
	     public WorldTile SETile;

	     public void Setup(TileData td)
	     {
		          tileData = td;
		          tileSize = tileData.tileWidth;
		          //draw  the  tile based on the volume of  material   in  it
		          transform.localScale = new Vector3(tileSize, tileSize, tileSize);
	     }

	     // Update is called once per frame
	     void Update()
	     {
		          if (isHovered)
		          {
				GetComponent<Renderer>().material.color = Color.yellow;
		          }
	     }

	     public void TileVisualUpdate()
	     {
		          SoilUtilities.CalculateDerivedProperties(tileData.composition);
		          float height = SoilUtilities.CalculateEffectiveVolume(tileData.composition);
		          transform.localScale = new Vector3(tileSize, height, tileSize);

		          // Set position so that bottom of tile sits on ground
		          // Since scale.y is height, center is at height / 2
		          transform.position = new Vector3(
		              tileData.tileGridRef.x,
		              height / 2f,
		              tileData.tileGridRef.y
		          );

		          GetComponent<Renderer>().material.color = TileUtilities.GetSoilColor(tileData.composition);

	     }

	     public List<WorldTile> GetNeighbors()
	     {
		          return new List<WorldTile> { NTile, STile, ETile, WTile, NETile, NWTile, SETile, SWTile };
	     }


}
