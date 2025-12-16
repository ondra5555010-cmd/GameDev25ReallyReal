using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerGroupBehaviour : MonoBehaviour
{
    public GridStat currentTile;
    private Animator[] _animators; 

    [SerializeField]
    [Range(0.5f, 20f)]
    float speed = 8f; 
    
    private bool isMoving = false;
    private Vector3 offsetFromTileCenter;
    
    Vector3 inputDir = Vector3.zero;
    public Transform[] heroes;

    public LayerMask interactionLayer; 

    void Awake()
    {       
        _animators = GetComponentsInChildren<Animator>();
        
        if(currentTile != null)
            offsetFromTileCenter = transform.position - currentTile.Position;
    }

    void Update()
    {
        // 1. INTERAKCE (L) - Zkusíme otevřít dveře v okolí
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            TryInteractWithDoors();
        }

        if (isMoving) return; 

        // 2. POHYB - Kontrolujeme cestu pomocí Linecastu
        
        if (Keyboard.current.wKey.wasPressedThisFrame && currentTile.CanMoveNorth)
        {
            // Posíláme tam "nextTile" (sever), abychom zkontrolovali cestu k němu
            if (!IsPathBlocked(currentTile.northNeighbor))
            {
                inputDir = Vector3.forward;
                StartCoroutine(MoveTo(currentTile.northNeighbor));
            }
        }
        else if (Keyboard.current.sKey.wasPressedThisFrame && currentTile.CanMoveSouth)
        {
            if (!IsPathBlocked(currentTile.southNeighbor))
            {
                inputDir = Vector3.back;
                StartCoroutine(MoveTo(currentTile.southNeighbor));
            }
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame && currentTile.CanMoveEast)
        {
            if (!IsPathBlocked(currentTile.eastNeighbor))
            {
                inputDir = Vector3.right;
                StartCoroutine(MoveTo(currentTile.eastNeighbor));
            }
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame && currentTile.CanMoveWest)
        {   
            if (!IsPathBlocked(currentTile.westNeighbor))
            {
                inputDir = Vector3.left;
                StartCoroutine(MoveTo(currentTile.westNeighbor));
            }
        }

        if (inputDir != Vector3.zero)
        {
            foreach (var hero in heroes)
            {
                if(hero != null) hero.forward = inputDir;
            }
        }
    }

    // --- ZMĚNA: Používáme Linecast (Čáru) místo Koule ---
    private bool IsPathBlocked(GridStat targetTile)
    {
        if (targetTile == null) return true;

        // Začátek čáry: Střed aktuální dlaždice
        Vector3 startPos = currentTile.Position;
        // Konec čáry: Střed cílové dlaždice
        Vector3 endPos = targetTile.Position;

        // DŮLEŽITÉ: Zvedneme čáru trochu nad zem (např. o 1 metr), 
        // aby nám paprsek omylem netrefil podlahu nebo koberec.
        startPos.y += 1.0f;
        endPos.y += 1.0f;

        RaycastHit hit;
        
        // Vystřelíme čáru z A do B. Pokud něco trefíme v interactionLayer...
        if (Physics.Linecast(startPos, endPos, out hit, interactionLayer))
        {
            DungeonDoor door = hit.transform.GetComponent<DungeonDoor>();
            
            // Pokud jsme trefili dveře a jsou zavřené -> Cesta je blokovaná
            if (door != null && !door.isOpen)
            {
                Debug.Log("Narazil jsem do zavřených dveří!");
                return true; 
            }
        }
        
        return false; // Cesta je volná (nebo jsou dveře otevřené)
    }

    // --- ZMĚNA: Interakce také používá paprsky ---
    private void TryInteractWithDoors()
    {
        GridStat[] neighbors = new GridStat[] { 
            currentTile.northNeighbor, 
            currentTile.southNeighbor, 
            currentTile.eastNeighbor, 
            currentTile.westNeighbor 
        };

        foreach (var tile in neighbors)
        {
            if (tile == null) continue;

            // Stejná logika - střelíme paprsek na sousední dlaždici
            Vector3 startPos = currentTile.Position;
            Vector3 endPos = tile.Position;
            startPos.y += 1.0f;
            endPos.y += 1.0f;

            RaycastHit hit;
            if (Physics.Linecast(startPos, endPos, out hit, interactionLayer))
            {
                DungeonDoor door = hit.transform.GetComponent<DungeonDoor>();
                if (door != null)
                {
                    // Zkusíme otevřít nalezené dveře
                    door.TryOpen();
                }
            }
        }
    }

    private IEnumerator MoveTo(GridStat nextTile)
    {
        isMoving = true;
        foreach(var anim in _animators) anim.SetBool("IsMoving", true);

        Vector3 endPos = nextTile.Position + offsetFromTileCenter;
        currentTile = nextTile;

        while (transform.position != endPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = endPos; 

        foreach(var anim in _animators) anim.SetBool("IsMoving", false);
        isMoving = false;
    }
}