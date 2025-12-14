using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public GameObject unitModel;
    public object uiDisplay;

    public bool playerControlled;
    public BattleTile currentTile;

    public string unitName = "Unit";
    public string specialAbilityDescription = "This is unit has no special ability.";
    
    public int armorClass = 10;
    public int attackBonus = 2;
    
    public int damageDie = 6;
    public int damageDice = 1; 
    public int attackRange = 1;
    
    public int maxHitPoints = 8;
    public int currentHitPoints;
    
    public int maxMovementBudget;
    public int movementBudget;
    public bool isActionReady = true;
    [HideInInspector] public bool isMoving = false;
    
    public Vector3 floatingTextOffset = new Vector3(0, 6f, 0);
    
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
        if (UIManager.Instance != null)
            UIManager.Instance.ShowFloatingText(text, color, transform, floatingTextOffset);
        else
            Debug.LogWarning("UIManager instance missing!");
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
        
        if (TurnAndUnitsManager.Instance != null)
        {
            if (playerControlled)
            {
                if (!TurnAndUnitsManager.Instance.PlayerUnits.Contains(this))
                    TurnAndUnitsManager.Instance.PlayerUnits.Add(this);
                Debug.Log(TurnAndUnitsManager.Instance.PlayerUnits.Count);
            }
            else
            {
                if (!TurnAndUnitsManager.Instance.EnemyUnits.Contains(this))
                    TurnAndUnitsManager.Instance.EnemyUnits.Add(this);
                Debug.Log(TurnAndUnitsManager.Instance.EnemyUnits.Count);
            }
        }
        
        currentHitPoints = maxHitPoints;
        
        UpdatePosition();
    }

    public void ReplenishMovementBudget(){
        movementBudget = maxMovementBudget;
        RefreshUI();
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
        
        if (TurnAndUnitsManager.Instance != null)
        {
            if (playerControlled)
                TurnAndUnitsManager.Instance.PlayerUnits.Remove(this);
            else
                TurnAndUnitsManager.Instance.EnemyUnits.Remove(this);
        }
        
        Destroy(gameObject);

        if (UIManager.Instance != null)
        {
            if (playerControlled)
                UIManager.Instance.PopulatePlayerUnits(TurnAndUnitsManager.Instance.PlayerUnits);
            else
                UIManager.Instance.PopulateComputerUnits(TurnAndUnitsManager.Instance.EnemyUnits);
        }

    }

    public virtual void takeDamage(int damage)
    {
        currentHitPoints -= damage;

        var actions = new List<System.Action>();

        // update UI
        if (uiDisplay is UnitDisplay)
            actions.Add(() => RefreshUI());

        // handle death if HP drops to 0
        if (currentHitPoints <= 0)
            actions.Add(() => KillUnit());

        // queue floating text with associated actions
        UIManager.Instance.ShowFloatingText(
            $"-{damage}",
            Color.red,
            transform,
            floatingTextOffset,
            actions
        );
    }
    public void Heal(int amount)
    {
        if (amount <= 0) return;

        int oldHp = currentHitPoints;
        currentHitPoints = Mathf.Min(currentHitPoints + amount, maxHitPoints);

        int healedAmount = currentHitPoints - oldHp;
        if (healedAmount <= 0) return;

        var actions = new List<System.Action>();

        if (uiDisplay is UnitDisplay)
            actions.Add(() => RefreshUI());

        UIManager.Instance.ShowFloatingText(
            $"+{healedAmount}",
            Color.green,
            transform,
            floatingTextOffset,
            actions
        );
    }
    
    public virtual void Attack(BattleUnit target, bool isFree = false)
    {
        if (target == null) return;

        if (!isFree)
        {
            isActionReady = false;
        }

        int d20 = DiceRollManager.Instance.Roll(20);
        int attackRoll = d20 + attackBonus;
        bool hit = attackRoll >= target.armorClass;

        Color rollColor = hit ? Color.green : new Color(1f, 0.5f, 0f);

        ShowFloatingText($"Attack {attackRoll} vs AC {target.armorClass}", rollColor);

        if (!hit) return;

        int damage = DiceRollManager.Instance.Roll(damageDie, damageDice);

        ShowFloatingText($"{damageDice}d{damageDie}", Color.yellow);

        target.takeDamage(damage);
        RefreshUI();
    }
    
    private void CheckSpecialMoveToggle()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            ToggleSpecialMove();
        }
    }
    
    public void RefreshUI()
    {
        if (uiDisplay is UnitDisplay display)
            display.UpdateHp(this);

        // bottom selected panel (player-only)
        if (playerControlled &&
            UnitSelectionManager.Instance.selectedUnit == this)
        {
            UIManager.Instance.ShowSelectedUnit(this);
        }
    }



    public virtual void ToggleSpecialMove()
    {

    }
    
    public virtual void EnactSpecialMove(BattleUnit target)
    {
        
    }
    
    void OnMouseEnter()
    {
        if (currentTile != null)
            currentTile.OnMouseEnter();
    }

    void OnMouseExit()
    {
        if (currentTile != null)
            currentTile.OnMouseExit();
    }

    void OnMouseDown()
    {
        if (currentTile != null)
            currentTile.OnMouseDown();
    }

    // Virtuální metoda, kterou může Boss přepsat
    public virtual void OnTurnStart()
    {
        // Ve výchozím stavu nedělá nic, obyčejné jednotky to ignorují
    }
}
