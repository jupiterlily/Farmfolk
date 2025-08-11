using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileClickManager : MonoBehaviour
{
    public int worldsize = 10;
    public TileGrid grid;
    public GameObject tilePrefab;

    [Header("Grid Settings")]
    public float tileSize = 1f;
    public Vector3 gridOrigin = Vector3.zero;
    public Vector3 gridNormal = Vector3.up;
    public Transform gridTransform; // optional: use if grid is rotated/parented

    public TextMeshProUGUI tileInfoText; // optional: for displaying tile info

    // Assign your tiles here
    public GameObject[,] tiles;

    private enum OriginMode { Corner, Center }
    private OriginMode originMode = OriginMode.Center;

    // debug
    private Vector3 lastHitPoint;
    private Vector3 lastTileCenter;

    void Start()
    {
        grid = new TileGrid();
        grid.tileWidth = 1;
        grid.XGrid = worldsize;
        grid.YGrid = worldsize;

        tiles = new GameObject[worldsize, worldsize];

        for (int x = 0; x < worldsize; x++)
        {
            for (int z = 0; z < worldsize; z++)
            {
                TileData data = new TileData();
                data.tileWidth = grid.tileWidth;
                data.tileGridRef = new Vector2Int(x, z);

                GameObject tile = Instantiate(tilePrefab, new Vector3(x, 0, z), Quaternion.identity);
                tiles[x, z] = tile;

                tile.GetComponent<Tile>().Setup(data);
                tile.transform.position = new Vector3(x, 0 - data.compaction / 2, z);
            }
        }

        AutoDetectOriginMode();
    }

    void Update()
    {
        HighlightTileUnderMouse();

        if (Input.GetMouseButtonDown(0))
        {
            GameObject clickedTile = GetClickedTile();
            if (clickedTile != null)
            {
                Debug.Log($"Clicked tile: {clickedTile.name}");
                ShowTileStats(clickedTile);
            }
        }
    }

    void AutoDetectOriginMode()
    {
        if (tiles == null || tiles.Length == 0) return;

        // Check first tile position
        GameObject firstTile = tiles[0, 0];
        if (firstTile == null) return;

        Vector3 pos = (gridTransform != null) ? 
            gridTransform.InverseTransformPoint(firstTile.transform.position) : 
            firstTile.transform.position - gridOrigin;

        // If the first tile is roughly at (0,0), assume center mode
        if (Mathf.Abs(pos.x) < tileSize * 0.25f && Mathf.Abs(pos.z) < tileSize * 0.25f)
        {
            originMode = OriginMode.Center;
        }
        else
        {
            originMode = OriginMode.Corner;
        }

        Debug.Log($"Origin mode auto-detected as: {originMode}");
    }

    void HighlightTileUnderMouse()
    {
        GameObject hoveredTile = GetTileUnderMouse();
        for (int x = 0; x < worldsize; x++)
        {
            for (int z = 0; z < worldsize; z++)
            {
                Tile tileComp = tiles[x, z].GetComponent<Tile>();
                tileComp.isHovered = (tiles[x, z] == hoveredTile);
            }
        }
    }

    GameObject GetTileUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane gridPlane = (gridTransform != null) ? 
            new Plane(gridTransform.up, gridTransform.position) : 
            new Plane(gridNormal, gridOrigin);

        if (gridPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 localPoint = (gridTransform != null) ?
                gridTransform.InverseTransformPoint(hitPoint) :
                hitPoint - gridOrigin;

            float fx = localPoint.x / tileSize;
            float fz = localPoint.z / tileSize;
            float eps = 1e-5f;

            int tileX, tileZ;
            if (originMode == OriginMode.Corner)
            {
                tileX = Mathf.FloorToInt(fx + eps);
                tileZ = Mathf.FloorToInt(fz + eps);
            }
            else // Center
            {
                tileX = Mathf.RoundToInt(fx);
                tileZ = Mathf.RoundToInt(fz);
            }

            if (tiles != null &&
                tileX >= 0 && tileX < tiles.GetLength(0) &&
                tileZ >= 0 && tileZ < tiles.GetLength(1))
            {
                return tiles[tileX, tileZ];
            }
        }
        return null;
    }

    GameObject GetClickedTile()
    {
        return GetTileUnderMouse();
    }

    Vector3 GetTileCenterWorld(int x, int z)
    {
        Vector3 localCenter = (originMode == OriginMode.Corner) ?
            new Vector3((x + 0.5f) * tileSize, 0f, (z + 0.5f) * tileSize) :
            new Vector3(x * tileSize, 0f, z * tileSize);

        return (gridTransform != null) ? 
            gridTransform.TransformPoint(localCenter) : 
            gridOrigin + localCenter;
    }

    void ShowTileStats(GameObject tile)
    {
        Tile t = tile.GetComponent<Tile>();
        if (t != null)
        {

            tileInfoText.text = $"Tile: {tile.name}\n" +
                $"Position: {tile.transform.position}\n" +
                $"Size: {t.tileData.tileWidth}\n" +
                $"Compaction: {t.tileData.compaction:F2}";
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lastHitPoint, 0.05f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(lastTileCenter, new Vector3(tileSize, 0.05f, tileSize));
    }
}
