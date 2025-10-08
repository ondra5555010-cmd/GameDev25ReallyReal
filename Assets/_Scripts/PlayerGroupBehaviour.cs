using UnityEngine;
using UnityEngine.InputSystem; // required for new Input System

public class PlayerGroupBehaviour : MonoBehaviour
{
    //public GridBehaviour gridManager;
    public GridStat currentTile;

    void Start()
    {
        transform.position = currentTile.Position;
    }

    void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            Debug.Log("W pressed");
            if (currentTile.CanMoveNorth)
            {
                Debug.Log("Moving North");
                MoveTo(currentTile.northNeighbor);
            }
            else
            {
                Debug.Log("No North neighbor to move to");
            }
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            Debug.Log("S pressed");
            if (currentTile.CanMoveSouth)
            {
                Debug.Log("Moving South");
                MoveTo(currentTile.southNeighbor);
            }
            else
            {
                Debug.Log("No South neighbor to move to");
            }
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            Debug.Log("D pressed");
            if (currentTile.CanMoveEast)
            {
                Debug.Log("Moving East");
                MoveTo(currentTile.eastNeighbor);
            }
            else
            {
                Debug.Log("No East neighbor to move to");
            }
        }

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            Debug.Log("A pressed");
            if (currentTile.CanMoveWest)
            {
                Debug.Log("Moving West");
                MoveTo(currentTile.westNeighbor);
            }
            else
            {
                Debug.Log("No West neighbor to move to");
            }
        }
    }


    
    private void MoveTo(GridStat nextTile)
    {
        currentTile = nextTile;
        transform.position = currentTile.Position;
        Debug.Log($"Moved to tile at ({currentTile.x}, {currentTile.y})");
    }
}