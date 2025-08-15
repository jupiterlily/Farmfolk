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
		          grid = new TileGrid(worldsize.x, worldsize.y);

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
					     grid.Map.SetValue(data, i, j);
					     tile.transform.position = new Vector3(i, 1 / 2, j);
					     //get a valid neighbor from ref
					     tile.GetComponent<Tile>().tileData.Initialize(TileUtilities.GetValidNeighbour(data, grid));

					     tile.GetComponent<Renderer>().material.color = TileUtilities.GetSoilColor(tile.GetComponent<Tile>().tileData.composition);

				}
		          }
	     }

}
