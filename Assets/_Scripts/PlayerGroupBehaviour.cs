using UnityEngine;
using UnityEngine.InputSystem; // required for new Input System
using System.Collections;

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
    float speed = 8f; //1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool isMoving = false; // zabrání spouštění pohybu, pokud už postava běží

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
        if (isMoving) return; // ignoruj vstupy během pohybu

        if (Keyboard.current.wKey.wasPressedThisFrame && currentTile.CanMoveNorth)
        {
            StartCoroutine(MoveTo(currentTile.northNeighbor));
        }
        else if (Keyboard.current.sKey.wasPressedThisFrame && currentTile.CanMoveSouth)
        {
            StartCoroutine(MoveTo(currentTile.southNeighbor));
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame && currentTile.CanMoveEast)
        {
            StartCoroutine(MoveTo(currentTile.eastNeighbor));
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame && currentTile.CanMoveWest)
        {
            StartCoroutine(MoveTo(currentTile.westNeighbor));
        }
    }


    
    /*private void MoveTo(GridStat nextTile)
    {
        _animator.SetBool("IsMoving", true);

        currentTile = nextTile;
        transform.position = currentTile.Position + offsetFromTileCenter;  // <-- zde zachováme offset

        Debug.Log($"Moved to tile at ({currentTile.x}, {currentTile.y})");
        _animator.SetBool("IsMoving", false);
    }*/

    private IEnumerator MoveTo(GridStat nextTile)
    {
        isMoving = true;
        _animator.SetBool("IsMoving", true);

        // --- ROTACE PODLE SMĚRU ---
        Vector3 dir = nextTile.Position - currentTile.Position;

        if (dir.x > 0)      transform.rotation = Quaternion.Euler(0, 90, 0);   // East (D)
        else if (dir.x < 0) transform.rotation = Quaternion.Euler(0, -90, 0);  // West (A)
        else if (dir.z > 0) transform.rotation = Quaternion.Euler(0, 0, 0);    // North (W)
        else if (dir.z < 0) transform.rotation = Quaternion.Euler(0, 180, 0);  // South (S) 

        Vector3 startPos = transform.position;
        Vector3 endPos = nextTile.Position + offsetFromTileCenter;
        currentTile = nextTile;

        float distance = Vector3.Distance(startPos, endPos);
        float travelTime = distance / speed; // čas potřebný k přeběhnutí
        float elapsed = 0f;

        while (elapsed < travelTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / travelTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos; // přesně na cílové dlaždici
        _animator.SetBool("IsMoving", false);
        isMoving = false;
    }
}