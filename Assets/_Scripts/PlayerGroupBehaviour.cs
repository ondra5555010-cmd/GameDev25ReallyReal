using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerGroupBehaviour : MonoBehaviour
{
    public GridStat currentTile;
    
    // ZMĚNA 1: Místo jednoho animátoru jich budeme ovládat pole (pro všechny hrdiny)
    private Animator[] _animators; 

    [SerializeField]
    [Range(0.5f, 20f)] // Zvýšil jsem range, abyste mohl testovat rychlejší pohyb
    float speed = 8f; 
    
    private bool isMoving = false;
    private Vector3 offsetFromTileCenter;
    
    // Parent tří hrdinů (pokud je skript na tomto objektu, je to "transform")
    // public Transform visualRoot; // Toto už možná nebudete potřebovat, pokud hýbeme přímo Parentem
    
    Vector3 inputDir = Vector3.zero;
    public Transform[] heroes; // Zde v inspektoru přiřadíte jednotlivé hrdiny pro otáčení

    void Awake()
    {       
        // ZMĚNA 2: Najdeme všechny animátory v dětech (Children)
        // Toto najde animátory na hrdinech pod tímto Parent objektem
        _animators = GetComponentsInChildren<Animator>();
        
        // Pokud startujete hru a Parent není přesně na středu, spočítáme offset
        if(currentTile != null)
            offsetFromTileCenter = transform.position - currentTile.Position;
    }

    void Update()
    {
        if (isMoving) return; 

        // Logika výběru směru zůstává stejná
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

        // Otáčení jednotlivých hrdinů
        if (inputDir != Vector3.zero)
        {
            foreach (var hero in heroes)
            {
                if(hero != null) hero.forward = inputDir;
            }
        }
    }

    private IEnumerator MoveTo(GridStat nextTile)
    {
        isMoving = true;

        // ZMĚNA 3: Zapneme animaci Běhu na VŠECH hrdinech najednou
        foreach(var anim in _animators)
        {
            anim.SetBool("IsMoving", true);
        }

        Vector3 endPos = nextTile.Position + offsetFromTileCenter;
        currentTile = nextTile;

        // Použijeme spolehlivější MoveTowards místo Lerp (jak jsme řešili předtím)
        while (transform.position != endPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = endPos; 

        // ZMĚNA 4: Vypneme animaci na VŠECH hrdinech
        foreach(var anim in _animators)
        {
            anim.SetBool("IsMoving", false);
        }
        
        Debug.Log($"Moved to tile at ({currentTile.x}, {currentTile.y})");
        isMoving = false;
    }
}