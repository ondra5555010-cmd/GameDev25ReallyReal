using UnityEngine;
using UnityEngine.InputSystem; // required for new Input System

[RequireComponent(typeof(Animator))]
// kdykoliv nebude na game objectu Animator, Unity ho přidá automaticky
// pokud je Animator, nic se nestane
public class PlayerGroupBehaviour : MonoBehaviour
{
    //public GridBehaviour gridManager;
    public GridStat currentTile;
    Animator _animator;

    [SerializeField]
    [Range(0.5f, 10f)]
    // atribut SerializeField umožní nastavit hodnotu v inspektoru, i když je pole private
    float speed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Vector3 offsetFromTileCenter;  // <-- přidáno
    void Awake()
    {
        _animator = GetComponent<Animator>();
        // zapamatuj si počáteční offset od středu dlaždice
        offsetFromTileCenter = transform.position - currentTile.Position;
        // získáme referenci na Animator, nemusíme ji ani testovat, protože RequireComponent zajistí, že tam bude
    }

    void Start()
    {
        //transform.position = offsetFromTileCenter;
    }

    void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            Debug.Log("W pressed");
            if (currentTile.CanMoveNorth)
            {
                _animator.SetBool("IsMoving", true);
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
                _animator.SetBool("IsMoving", true);
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
                _animator.SetBool("IsMoving", true);
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
                _animator.SetBool("IsMoving", true);
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
        _animator.SetBool("IsMoving", true);

        currentTile = nextTile;
        transform.position = currentTile.Position + offsetFromTileCenter;  // <-- zde zachováme offset

        Debug.Log($"Moved to tile at ({currentTile.x}, {currentTile.y})");
        _animator.SetBool("IsMoving", false);
    }
}