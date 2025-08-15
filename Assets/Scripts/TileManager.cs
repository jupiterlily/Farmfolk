using UnityEngine;

//instantiates tiles within a grid and  gives  them a grid reference

public class TileManager : MonoBehaviour
{
	     public Vector2Int worldsize;
	     public TileGrid grid;
	     public GameObject tilePrefab;
	     public WorldTile[,] tiles;
	     // Start is called before the first frame update
	     void Start()
	     {
		          grid = new TileGrid(worldsize.x, worldsize.y);


		          grid.tileWidth = 1;

		          grid.XGrid = worldsize.x;
		          grid.YGrid = worldsize.y;
		          tiles = new WorldTile[worldsize.x, worldsize.y];
		          //for each row of worlsize, allocate a tile and a position.
		          for (int i = 0; i < grid.XGrid; i++)
		          {
				for (int j = 0; j < grid.YGrid; j++)
				{
					     TileData data = new TileData();
					     data.tileWidth = grid.tileWidth;
					     data.tileGridRef = new Vector2Int(i, j);

					     GameObject tile = Instantiate(tilePrefab, transform);

					     tile.GetComponent<WorldTile>().Setup(data);
					     grid.Map.SetValue(data, i, j);
					     tiles.SetValue(tile.GetComponent<WorldTile>(), i, j);
					     tile.transform.position = new Vector3(i, 1 / 2, j);
					     //get a valid neighbor from ref
					     tile.GetComponent<WorldTile>().tileData.Initialize(TileUtilities.GetValidNeighbour(data, grid));

					     tile.GetComponent<Renderer>().material.color = TileUtilities.GetSoilColor(tile.GetComponent<WorldTile>().tileData.composition);

				}
		          }
		          AllocateNeighbors();

	     }
	     public void TileUpdate(float hours)
	     {
		          foreach (var tile in tiles)
		          {
				tile.TileVisualUpdate();
		          }
	     }
	     public void AllocateNeighbors()
	     {
		          for (int x = 0; x < worldsize.x; x++)
		          {
				for (int y = 0; y < worldsize.y; y++)
				{
					     WorldTile current = tiles[x, y];

					     // Wrapping coordinates
					     int xLeft = (x - 1 + worldsize.x) % worldsize.x;
					     int xRight = (x + 1) % worldsize.x;
					     int yDown = (y - 1 + worldsize.y) % worldsize.y;
					     int yUp = (y + 1) % worldsize.y;

					     // Assign neighbors with wrapping
					     current.NTile = tiles[x, yUp];
					     current.STile = tiles[x, yDown];
					     current.WTile = tiles[xLeft, y];
					     current.ETile = tiles[xRight, y];

					     current.NWTile = tiles[xLeft, yUp];
					     current.NETile = tiles[xRight, yUp];
					     current.SWTile = tiles[xLeft, yDown];
					     current.SETile = tiles[xRight, yDown];
				}
		          }
	     }



}
