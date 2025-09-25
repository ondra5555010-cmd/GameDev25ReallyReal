using UnityEngine;
using UnityEngine.InputSystem; // required for new Input System

public class PlayerGroupBehaviour : MonoBehaviour
{
    public GridBehaviour gridManager;
    public GridStat currentTile;

    void Start()
    {
        if (gridManager.startingTile != null)
        {
            currentTile = gridManager.startingTile;
            transform.position = currentTile.Position;
        }
        else
        {
            Debug.LogError("GridManager has no starting position set!");
        }
    }

    void Update()
    {
        if (Keyboard.current.numpad7Key.wasPressedThisFrame)
        {
            Debug.Log("Numpad 7 pressed");
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

        if (Keyboard.current.numpad3Key.wasPressedThisFrame)
        {
            Debug.Log("Numpad 3 pressed");
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

        if (Keyboard.current.numpad9Key.wasPressedThisFrame)
        {
            Debug.Log("Numpad 9 pressed");
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

        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            Debug.Log("Numpad 1 pressed");
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