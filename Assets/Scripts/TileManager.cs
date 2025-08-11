using UnityEngine;

//instantiates tiles within a grid and  gives  them a grid reference

public class TileManager : MonoBehaviour
{
	     public Vector2Int worldsize;
	     public TileGrid grid;
	     public GameObject tilePrefab;
	     // Start is called before the first frame update
	     void Start()
	     {
		          grid = new TileGrid();

		          grid.tileWidth = 1;

		          grid.XGrid = worldsize.x;
		          grid.YGrid = worldsize.y;
		          //for each row of worlsize, allocate a tile and a position.
		          for (int i = 0; i < grid.XGrid; i++)
		          {
				for (int j = 0; j < grid.YGrid; j++)
				{
					     TileData data = new TileData();
					     data.tileWidth = grid.tileWidth;
					     data.tileGridRef = new Vector2Int(i, j);

					     GameObject tile = Instantiate(tilePrefab, transform);

					     tile.GetComponent<Tile>().Setup(data);
					     tile.transform.position = new Vector3(i, 0 - data.compaction / 2, j);

					     tile.GetComponent<Tile>().tileData.Initialize();
				}
		          }
	     }

}
