using UnityEngine;
using UnityEngine.InputSystem; // required for new Input System
using System.Collections;

//[RequireComponent(typeof(Animator))]
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

    private Vector3 offsetFromTileCenter;  // offset pro postavy od středu dlaždice
    public Transform visualRoot; // parent tří hrdinů
    Vector3 inputDir = Vector3.zero;
    public Transform[] heroes; // pole tří hrdinů

    void Awake()
    {        
        //_animator = GetComponent<Animator>();
        _animator = GetComponentInChildren<Animator>();
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
            inputDir = Vector3.forward;
            StartCoroutine(MoveTo(currentTile.northNeighbor));
        }
        else if (Keyboard.current.sKey.wasPressedThisFrame && currentTile.CanMoveSouth)
        {
            inputDir = Vector3.back;
            StartCoroutine(MoveTo(currentTile.southNeighbor));
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame && currentTile.CanMoveEast)
        {
            inputDir = Vector3.right;
            StartCoroutine(MoveTo(currentTile.eastNeighbor));
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame && currentTile.CanMoveWest)
        {   
            inputDir = Vector3.left;
            StartCoroutine(MoveTo(currentTile.westNeighbor));
        }

         if (inputDir != Vector3.zero)
        {
            foreach (var hero in heroes)
                hero.forward = inputDir;
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
        Debug.Log($"Moved to tile at ({currentTile.x}, {currentTile.y})");
        _animator.SetBool("IsMoving", false);
        isMoving = false;
    }
}