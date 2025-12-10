using System;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public GameObject unitModel;
    public GameObject floatingTextPrefab;
    public bool playerControlled;
    public BattleTile currentTile;

    public int armorClass = 10;
    public int attackBonus = 2;
    
    public int damageDie = 6;
    public int damageDice = 1;
    
    public int maxHitPoints = 8;
    public int currentHitPoints;
    
    public int maxMovementBudget;
    public int movementBudget;
    [HideInInspector] public bool isMoving = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowFloatingText(string text, Color color)
    {
        Debug.Log($"SHOWING FLOATING TEXT: {text.ToUpper()}");

        if (floatingTextPrefab == null) 
        {
            Debug.LogWarning("FLOATING TEXT PREFAB IS NULL!");
            return;
        }

        // Instantiate the prefab at the unit's position
        var go = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);

        // Try to get the FloatingTextScript from root or any child
        var ft = go.GetComponent<FloatingTextScript>() ?? go.GetComponentInChildren<FloatingTextScript>();

        if (ft != null) 
        {
            ft.Show(text, color, transform);
        }
        else
        {
            Debug.LogWarning("FLOATING TEXT COMPONENT MISSING ON PREFAB OR ITS CHILDREN!");
        }
    }


    
    public void UpdatePosition()
    {
        if (currentTile == null) return;
        if (isMoving) return; // don't snap while animating
        transform.position = currentTile.transform.position + Vector3.up * 0.5f;
    }
    
    public Renderer GetRenderer()
    {
        if (unitModel != null)
            return unitModel.GetComponent<Renderer>();
    
        return GetComponent<Renderer>();
    }
    
    public void Initialize(GameObject model = null, bool isPlayerControlled = false)
    {
        playerControlled = isPlayerControlled;

        if (model != null)
        {
            //Debug.Log(model.name);
            unitModel = Instantiate(model, transform);
            unitModel.transform.localPosition = Vector3.zero;
        }
        
        if (TurnAndUnitManager.Instance != null)
        {
            if (playerControlled)
            {
                if (!TurnAndUnitManager.Instance.PlayerUnits.Contains(this))
                    TurnAndUnitManager.Instance.PlayerUnits.Add(this);
                Debug.Log(TurnAndUnitManager.Instance.PlayerUnits.Count);
            }
            else
            {
                if (!TurnAndUnitManager.Instance.EnemyUnits.Contains(this))
                    TurnAndUnitManager.Instance.EnemyUnits.Add(this);
                Debug.Log(TurnAndUnitManager.Instance.EnemyUnits.Count);
            }
        }
        
        currentHitPoints = maxHitPoints;
        
        UpdatePosition();
    }

    public void ReplenishMovementBudget(){
        movementBudget = maxMovementBudget;
    }
    
    public void KillUnit()
    {
        if (UnitSelectionManager.Instance != null &&
            UnitSelectionManager.Instance.selectedUnit == this)
        {
            UnitSelectionManager.Instance.DeselectUnit();
        }
        
        if (currentTile != null)
        {
            currentTile.ClearUnit();
        }
        Destroy(gameObject);
    }

    public void takeDamage(int damage)
    {
        currentHitPoints -= damage;

        ShowFloatingText($"-{damage}", Color.red);

        if (currentHitPoints <= 0)
        {
            currentHitPoints = 0;
            KillUnit();
        }
    }
    
    public void Attack(BattleUnit target)
    {
        if (target == null) return;

        // Attack roll
        int d20 = DiceRollManager.Instance.Roll(20);
        int attackRoll = d20 + attackBonus;
        bool hit = attackRoll >= target.armorClass;

        // Single line feedback: "Attack 17 vs AC 13"
        string rollInfo = $"Attack {attackRoll} vs AC {target.armorClass}";
        Color rollColor = hit ? Color.green : new Color(1f, 0.5f, 0f); // orange

        ShowFloatingText(rollInfo, rollColor);

        if (!hit) return;

        // Damage roll
        int damage = DiceRollManager.Instance.Roll(damageDie, damageDice);

        // Show damage die used: e.g. "1d6"
        ShowFloatingText($"{damageDice}d{damageDie}", Color.yellow);

        target.takeDamage(damage);
    }
}
