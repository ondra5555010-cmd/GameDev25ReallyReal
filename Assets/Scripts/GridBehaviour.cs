using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    public int rows = 10;
    public int columns = 10;
    public int scale = 1;
    public GameObject gridPrefab;
    public Vector3 leftBottomLocation = new Vector3(0, 0, 0);
    void Awake()
    {
        if (gridPrefab)
        {
            GenerateGrid();
        }
        else
        {
            print("No grid prefab assigned");
        }
    }

    void Update()
    {
        
    }

    void GenerateGrid()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                GameObject grid = Instantiate(gridPrefab, new Vector3(leftBottomLocation.x+scale*c,leftBottomLocation.y,leftBottomLocation.z+scale*r), Quaternion.identity);
                grid.transform.SetParent(transform);
                grid.GetComponent<GridStat>().x = c;
                grid.GetComponent<GridStat>().y = r;
            }
        }
    }
}
