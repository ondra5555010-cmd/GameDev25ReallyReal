using UnityEngine;

public class GridStat : MonoBehaviour
{
    public int x = 0;
    public int y = 0;
    public Vector3 Position => transform.position;

    public GridStat northNeighbor;
    public GridStat southNeighbor;
    public GridStat eastNeighbor;
    public GridStat westNeighbor;

    // Cached movement flags
    public bool CanMoveNorth { get; private set; }
    public bool CanMoveSouth { get; private set; }
    public bool CanMoveEast { get; private set; }
    public bool CanMoveWest { get; private set; }

    public void AssignNeighbors(GridStat north, GridStat south, GridStat east, GridStat west)
    {
        northNeighbor = north;
        southNeighbor = south;
        eastNeighbor = east;
        westNeighbor = west;

        CanMoveNorth = (north != null);
        CanMoveSouth = (south != null);
        CanMoveEast = (east != null);
        CanMoveWest = (west != null);
        
        Debug.Log($"Tile ({x}, {y}) neighbors -> " +
                  $"N: {(CanMoveNorth ? "Yes" : "No")}, " +
                  $"S: {(CanMoveSouth ? "Yes" : "No")}, " +
                  $"E: {(CanMoveEast ? "Yes" : "No")}, " +
                  $"W: {(CanMoveWest ? "Yes" : "No")}");
    }
}