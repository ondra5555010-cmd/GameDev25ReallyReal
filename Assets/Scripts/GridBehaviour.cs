using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    public float scale = 4.84f;
    public GameObject gridPrefab;
    public Vector3 leftBottomLocation = Vector3.zero;
    public GridStat[,] gridArray;
    public GridStat startingTile;

    // Example layout: '-' = empty, 'X' = tile
    private string[] mapLayout = new string[]
    {
        "XXX",
        "XXX",
        "XXX",
        "-X-",
        "-X-",
        "-X-",
        "-S-"
    };

    void Awake()
    {
        if (gridPrefab)
        {
            GenerateGridFromString(FlipArrayUpsideDown(mapLayout));
            AssignNeighbours();
        }
        else
        {
            Debug.LogWarning("No grid prefab assigned");
        }
    }
    
    private string[] FlipArrayUpsideDown(string[] original)
    {
        int rows = original.Length;
        string[] flipped = new string[rows];

        for (int i = 0; i < rows; i++)
        {
            flipped[i] = original[rows - 1 - i];
        }

        return flipped;
    }

    void GenerateGridFromString(string[] layout)
    {
        int rows = layout.Length;
        int columns = layout[0].Length;

        gridArray = new GridStat[columns, rows];

        // Generate tiles so that row 0 is at bottom (z = 0)
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                char symbol = layout[r][c];

                if (symbol == 'X' || symbol == 'S')
                {
                    GameObject tile = Instantiate(
                        gridPrefab,
                        new Vector3(
                            leftBottomLocation.x + scale * c,
                            leftBottomLocation.y,
                            leftBottomLocation.z + scale * r),
                        Quaternion.identity);

                    tile.transform.SetParent(transform);

                    GridStat stat = tile.GetComponent<GridStat>();
                    stat.x = c;
                    stat.y = r;

                    gridArray[c, r] = stat;

                    Debug.Log($"Tile created at grid coords x={c}, y={r} | World pos: {tile.transform.position} | Symbol: {symbol}");

                    if (symbol == 'S')
                    {
                        startingTile = stat;
                        Debug.Log($"Starting tile assigned at grid coords x={stat.x}, y={stat.y} | World pos: {stat.transform.position}");
                    }
                }
                else
                {
                    gridArray[c, r] = null;
                }
            }
        }
    }

    public void AssignNeighbours()
    {
        int columns = gridArray.GetLength(0);
        int rows = gridArray.GetLength(1);

        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                GridStat current = gridArray[c, r];
                if (current == null) continue;

                // Keep original logic: north = r+1, south = r-1
                GridStat north = (r < rows - 1) ? gridArray[c, r + 1] : null;
                GridStat south = (r > 0) ? gridArray[c, r - 1] : null;
                GridStat east  = (c < columns - 1) ? gridArray[c + 1, r] : null;
                GridStat west  = (c > 0) ? gridArray[c - 1, r] : null;

                current.AssignNeighbors(north, south, east, west);

                // Debug to verify neighbors
                Debug.Log($"Tile ({current.x},{current.y}) neighbors -> " +
                          $"N: {(current.CanMoveNorth ? "Yes" : "No")}, " +
                          $"S: {(current.CanMoveSouth ? "Yes" : "No")}, " +
                          $"E: {(current.CanMoveEast ? "Yes" : "No")}, " +
                          $"W: {(current.CanMoveWest ? "Yes" : "No")}");
            }
        }
    }
}
